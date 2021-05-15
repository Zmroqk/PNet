﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PNetDll
{
    public class PingTestManager
    {
        /// <summary>
        /// Interval for ping tests
        /// </summary>
        int _Interval;
        /// <summary>
        /// Interval used when hosts disconnects
        /// </summary>
        int _ReconnectInterval;
        /// <summary>
        /// Timer for ping tests
        /// </summary>
        Timer pingTimer;
        /// <summary>
        /// Available tests to ping
        /// </summary>
        List<PingTest> availablePings;
        /// <summary>
        /// Tests marked as disconnected
        /// </summary>
        List<PingTest> blockedPings;
        /// <summary>
        /// Timer for tests marked as disconnected, used to reconnect tests
        /// </summary>
        Timer blockedPingsTimer;
        /// <summary>
        /// Current ping index
        /// </summary>
        int pingIndex;
        /// <summary>
        /// Event called when ping in test exceeds PingLogValue
        /// </summary>
        public event PingLimitExceededEventHandler PingLimitExceeded;

       /// <summary>
        /// Destination host
        /// </summary>
        public IPAddress DestinationHost { get; init; }      
 
        /// <summary>
        /// Mode for ping tests
        /// </summary>
        public PingMode Mode { get; init; }
  
        /// <summary>
        /// Should test run route discovery
        /// </summary>
        public bool TracerouteEnabled { get; init; }
 
        /// <summary>
        /// Should history be saved in memory
        /// </summary>
        public bool LogHistory { get; init; }
 
        /// <summary>
        /// History of pings
        /// </summary>
        public Dictionary<PingTest, List<PingData>> History { get; private set; }
 
        /// <summary>
        /// Should history be streamed
        /// </summary>
        public bool StreamData { get; init; }

        /// <summary>
        /// Dictionary of streams for each test
        /// </summary>
        public Dictionary<PingTest, MemoryStream> HistoryStream { get; private set; }

        /// <summary>
        /// Limit for ping before calling PingLimitExceeded
        /// </summary>
        public int PingLogValue { get; set; }

        /// <summary>
        /// List of running tests
        /// </summary>
        public List<PingTest> PingTests { get; private set; }

        /// <summary>
        /// Error limit before setting destination host as disconnected
        /// </summary>
        public int ErrorsCount { get; set; }

        /// <summary>
        /// Interval for ping tests
        /// </summary>
        public int Interval {
            get {
                return _Interval;
            } 
            set {
                _Interval = value;
                if (pingTimer != null)
                    pingTimer.Interval = _Interval;
            } 
        }    

        /// <summary>
        /// Interval used when hosts disconnects
        /// </summary>
        public int ReconnectInterval
        {
            get
            {
                return _ReconnectInterval;
            }
            set
            {
                _ReconnectInterval = value;
                if (blockedPingsTimer != null)
                    blockedPingsTimer.Interval = _ReconnectInterval;
            }
        }

    public PingTestManager(IPAddress destinationHost, int pingLogValue = 100, int interval = 5000, 
                                    bool traceRoute = false, PingMode pingMode = PingMode.Asynchronously,
                                    short errorsCount = 3, int reconnectInterval = 10000, 
                                    bool logHistory = false, bool streamData = false)
        {
            DestinationHost = destinationHost;
            TracerouteEnabled = traceRoute;
            PingLogValue = pingLogValue;
            Mode = pingMode;
            PingTests = new List<PingTest>();
            availablePings = new List<PingTest>();
            blockedPings = new List<PingTest>();
            if(LogHistory)
                History = new Dictionary<PingTest, List<PingData>>();
            if(StreamData)
                HistoryStream = new Dictionary<PingTest, MemoryStream>();
            pingIndex = 0;
            Interval = interval;
            ErrorsCount = errorsCount;
            ReconnectInterval = reconnectInterval;
            LogHistory = logHistory;
            StreamData = streamData;
        }

        public void StartTest()
        {
            if (pingTimer != null)
                return;         
            if (TracerouteEnabled)
            {
                FindHostsOnPath();
            }
            else
            {
                PingTest pt = new PingTest(DestinationHost);
                PingTests.Add(pt);
                availablePings.Add(pt);
                CreateHistory(pt);
            }
            pingTimer = new Timer();
            pingTimer.Interval = Interval;
            if (Mode == PingMode.Simultaneous)
                pingTimer.Elapsed += PingSimultaneous;
            else if (Mode == PingMode.Asynchronously)
                pingTimer.Elapsed += PingAsynchronously;
            pingTimer.Start();
        }

        private void CreateHistory(PingTest pt)
        {
            if (LogHistory)
                History.Add(pt, new List<PingData>());
            if (StreamData)
                HistoryStream.Add(pt, new MemoryStream());
        }

        void FindHostsOnPath()
        {
            int ttl = 1;
            Ping ping = new Ping();
            PingReply pr;
            do
            {
                pr = ping.Send(DestinationHost, 5000, new byte[32], new PingOptions() { Ttl = ttl });
                if(pr.Status != IPStatus.Success)
                    ttl++;
            }
            while (pr.Status != IPStatus.Success);
            List<Task<PingTest>> pings = new List<Task<PingTest>>();
            for (int i = 1; i <= ttl; i++)
            {
                int actualTtl = i;
                pings.Add(Task.Run(() =>
                {
                    Ping ping = new Ping();
                    PingReply pr;
                    pr = ping.Send(DestinationHost, 5000, new byte[32], new PingOptions() { Ttl = actualTtl });
                    if (pr.Status == IPStatus.Success)
                        return new PingTest(pr.Address);
                    return null;
                }));
            }
            Task<PingTest>[] pingsTasks = pings.ToArray();
            Task.WaitAll(pingsTasks);
            foreach(Task<PingTest> pt in pingsTasks)
            {
                if (pt.Result != null)
                {
                    pt.Result.PingCompleted += PingCompleted;
                    PingTests.Add(pt.Result);
                    availablePings.Add(pt.Result);
                    CreateHistory(pt.Result);
                }              
            }
        }

        private void PingCompleted(object sender, PingData pingData)
        {
            PingTest pt = (PingTest)sender;
            if (pingData.ErrorCount > ErrorsCount)
            {
                pt.PingCompleted -= PingCompleted;
                blockedPings.Add(pt);          
                lock (availablePings)
                {
                    availablePings.Remove(pt);
                    pingIndex = pingIndex % availablePings.Count;
                }              
                pt.PingCompleted += PingReconnectAttemptCompleted;
                if(blockedPingsTimer != null)
                {
                    blockedPingsTimer = new Timer();
                    blockedPingsTimer.Interval = ReconnectInterval;
                    blockedPingsTimer.Elapsed += BlockedPingsTimer_Elapsed;
                    blockedPingsTimer.Start();
                }                
            }
            if(pingData.Success)
            {
                if(pingData.Ping >= PingLogValue)
                    PingLimitExceeded?.Invoke(this, new PingLimitExceededData() { PingData = pingData, PingTest = pt });
                if(LogHistory)
                    History[pt].Add(pingData);
                if (StreamData)
                    PingDataSerializationTool.Serialize(HistoryStream[pt], pingData);
            }
        }

        private void BlockedPingsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (PingTest pt in blockedPings)
            {
                pt.PingAsync();
            }
        }

        private void PingReconnectAttemptCompleted(object sender, PingData pingData)
        {
            PingTest pt = (PingTest)sender;
            if (pingData.Success)
            {
                pt.PingCompleted -= PingReconnectAttemptCompleted;
                blockedPings.Remove(pt);
                lock (availablePings)
                {
                    availablePings.Add(pt);
                }
                pt.PingCompleted += PingCompleted;
                if(blockedPings.Count == 0)
                {
                    blockedPingsTimer.Dispose();
                    blockedPingsTimer = null;
                }
            }
        }

        void PingSimultaneous(object sender, ElapsedEventArgs e)
        {
            foreach (PingTest pt in availablePings)
            {
                pt.PingAsync();
            }
        }

        void PingAsynchronously(object sender, ElapsedEventArgs e)
        {
            lock (availablePings)
            {
                availablePings[pingIndex].PingAsync();
                pingIndex = ++pingIndex % availablePings.Count;
            }            
        }
    }
}