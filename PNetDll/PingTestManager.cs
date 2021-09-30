using PNetDll.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using PNetDll.Sqlite.Models;

namespace PNetDll
{
    /// <summary>
    /// Class that creates test for testing connection between source and destination
    /// </summary>
    public class PingTestManager : IDisposable
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
        public Dictionary<PingTest, ObservableCollection<PingData>> History { get; private set; }

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

        /// <summary>
        /// Database test case model for this test
        /// </summary>
        TestCase TestCase { get; set; }

        /// <summary>
        /// Create ping tests manager
        /// </summary>
        /// <param name="destinationHost">Host to which connection shoudl be tested</param>
        /// <param name="pingLogValue">Minimal value of ping which will call PingLimitExceeded</param>
        /// <param name="interval">Interval between tests</param>
        /// <param name="traceRoute">Use traceroute</param>
        /// <param name="pingMode">Ping mode for tests</param>
        /// <param name="errorsCount">Errors count after which destination host will be ... as disconnected</param>
        /// <param name="reconnectInterval">Interval used when trying to reconnect</param>
        /// <param name="logHistory">Log history</param>
        /// <param name="streamData">Create output data stream</param>
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
            LogHistory = logHistory;
            //StreamData = streamData;
            if (LogHistory)
                History = new Dictionary<PingTest, ObservableCollection<PingData>>();
            //if(StreamData)
            //    HistoryStream = new Dictionary<PingTest, MemoryStream>();
            pingIndex = 0;
            Interval = interval;
            ErrorsCount = errorsCount;
            ReconnectInterval = reconnectInterval;          
        }

        /// <summary>
        /// Start manager tests
        /// </summary>
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
                pt.PingCompleted += PingCompleted;
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

        /// <summary>
        /// Create history for ping test
        /// </summary>
        /// <param name="pt">Ping test for which history should be created</param>
        private void CreateHistory(PingTest pt)
        {
            if (LogHistory)
                History.Add(pt, new ObservableCollection<PingData>());
            //if (StreamData)
            //    HistoryStream.Add(pt, new MemoryStream());
        }

        /// <summary>
        /// Find all hosts between source and destination
        /// </summary>
        void FindHostsOnPath()
        {
            int ttl = 1;
            Ping ping = new Ping();
            PingReply pr;
            List<IPAddress> IPs = new List<IPAddress>();
            do
            {
                pr = ping.Send(DestinationHost, 1000, new byte[32], new PingOptions() { Ttl = ttl });
                if (!IPs.Contains(pr.Address))
                    IPs.Add(pr.Address);
                if(pr.Status != IPStatus.Success)
                    ttl++;
            }
            while (pr.Status != IPStatus.Success);
            List<Task<PingTest>> pings = new List<Task<PingTest>>();
            for (int i = 0; i < IPs.Count; i++)
            {
                int actualI = i;
                pings.Add(Task.Run(() =>
                {
                    try
                    {
                        Ping ping = new Ping();
                        PingReply pr;
                        pr = ping.Send(IPs[actualI], 2000);
                        if (pr.Status == IPStatus.Success)
                        {
                            return new PingTest(pr.Address);                         
                        }                        
                        return null;
                    }
                    catch (Exception e) {
                        return null; 
                    }                  
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
            using (PingContext db = Database.Db)
            {
                List<Ip> ips = new List<Ip>();
                foreach(IPAddress iPAddress in IPs)
                {
                    Ip ip = db.Ips.Where((ip) => ip.IPAddress == iPAddress.ToString()).FirstOrDefault();
                    ips.Add(ip);
                    db.Ips.Attach(ip);
                }
                TestCase = new TestCase()
                {
                    DestinationHost = db.Ips.Where((ip) => ip.IPAddress == DestinationHost.ToString()).FirstOrDefault(),
                    testStarted = DateTime.Now
                };
                TestCase.Ips.AddRange(ips);
                db.TestCases.Add(TestCase);
                db.SaveChanges();
            }

        }

        /// <summary>
        /// Operation called after ping
        /// </summary>
        /// <param name="sender">Test that called this event</param>
        /// <param name="pingData">Ping data</param>
        private async void PingCompleted(object sender, PingData pingData)
        {
            PingTest pt = (PingTest)sender;
            if (pingData.ErrorCount > ErrorsCount)
            {
                pt.PingCompleted -= PingCompleted;
                lock(blockedPings)
                    blockedPings.Add(pt);          
                lock (availablePings)
                {
                    availablePings.Remove(pt);
                    if(availablePings.Count > 0)
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
                PingContext db = Database.Db;
                Sqlite.Models.Ip ip = db.Ips.Where(ip => ip.IPAddress == pingData.IPAddress.ToString()).FirstOrDefault();
                db.Disconnects.Add(new Sqlite.Models.Disconnect() { DisconnectDate = pingData.DateTime, ConnectedIp = ip });
                db.SaveChanges();
            }
            if(pingData.Success)
            {
                if(pingData.Ping >= PingLogValue)
                    PingLimitExceeded?.Invoke(this, new PingLimitExceededData() { PingData = pingData, PingTest = pt });
                if (LogHistory)
                    lock (History[pt])
                    {
                        if (History[pt].Count > 100)
                            History[pt].RemoveAt(0);
                        History[pt].Add(pingData);
                    }
                        
                //if (StreamData)
                //   PingDataSerializationTool.Serialize(HistoryStream[pt], pingData);
            }
        }

        /// <summary>
        /// Operation called after BlockedPingTimer.Elapsed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlockedPingsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (blockedPings)
            {
                foreach (PingTest pt in blockedPings)
                {
                    pt.PingAsync();
                }
            }
        }

        /// <summary>
        /// Operation called after blocked ping PingCompleted event
        /// </summary>
        /// <param name="sender">Test that called this event</param>
        /// <param name="pingData">Ping data</param>
        private void PingReconnectAttemptCompleted(object sender, PingData pingData)
        {
            PingTest pt = (PingTest)sender;
            if (pingData.Success)
            {
                pt.PingCompleted -= PingReconnectAttemptCompleted;
                lock(blockedPings)
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

        /// <summary>
        /// Operation called after pingTimer.Elapsed event with ping mode set to Simultenous
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PingSimultaneous(object sender, ElapsedEventArgs e)
        {
            lock (availablePings)
            {
                foreach (PingTest pt in availablePings)
                {
                    pt.PingAsync();
                }
            }
        }

        /// <summary>
        /// Operation called after pingTimer.Elapsed event with ping mode set to Asynchronously
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PingAsynchronously(object sender, ElapsedEventArgs e)
        {
            lock (availablePings)
            {
                availablePings[pingIndex].PingAsync();
                if (availablePings.Count > 0)
                    pingIndex = ++pingIndex % availablePings.Count;
            }            
        }

        public void Dispose()
        {
            pingTimer?.Dispose();
            blockedPingsTimer?.Dispose();
            using(PingContext db = Database.Db)
            {
                db.TestCases.Attach(TestCase);
                TestCase.testEnded = DateTime.Now;
                db.SaveChanges();
            }
        }
    }
}
