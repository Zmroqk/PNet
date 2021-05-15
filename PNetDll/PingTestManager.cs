using System;
using System.Collections.Generic;
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
        public int PingLogValue { get; set; }

        public int Interval { get; set; }

        public PingMode Mode { get; init; }

        public List<PingTest> PingTests { get; set; }

        public IPAddress DestinationHost { get; init; }

        public bool TracerouteEnabled { get; init; }

        public int ErrorsCount { get; set; }

        public int ReconnectInterval { get; set; }

        Timer pingTimer;

        List<PingTest> availablePings;
        List<PingTest> blockedPings;

        Timer blockedPingsTimer;

        int pingIndex;

        public PingTestManager(IPAddress destinationHost, int pingLogValue = 100, int interval = 5000, 
                                    bool traceRoute = false, PingMode pingMode = PingMode.Asynchronously,
                                    short errorsCount = 3, int reconnectInterval = 10000)
        {
            DestinationHost = destinationHost;
            TracerouteEnabled = traceRoute;
            PingLogValue = pingLogValue;
            Mode = pingMode;
            PingTests = new List<PingTest>();
            availablePings = new List<PingTest>();
            blockedPings = new List<PingTest>();
            pingIndex = 0;
            Interval = interval;
            ErrorsCount = errorsCount;
            ReconnectInterval = reconnectInterval;
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
            }
            pingTimer = new Timer();
            pingTimer.Interval = Interval;
            if (Mode == PingMode.Simultaneous)
                pingTimer.Elapsed += PingSimultaneous;
            else if (Mode == PingMode.Asynchronously)
                pingTimer.Elapsed += PingAsynchronously;
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
            }
        }

        private void PingReconnectAttemptCompleted(object sender, PingData pingData)
        {
            throw new NotImplementedException();
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
