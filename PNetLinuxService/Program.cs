using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using PNetDll;

namespace PNetLinuxService
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.ReadConfig();
            List<IPAddress> ips = new List<IPAddress>();
            foreach(string host in Config.Instance.Hosts.Domains)
            {
                IPAddress address = Dns.GetHostAddresses("host")?[0];
                if(address != null)
                {
                    ips.Add(address);
                }
            }
            ips.AddRange(Config.Instance.Hosts.Ips.Select(ipString => IPAddress.Parse(ipString)));
            foreach(IPAddress address in ips)
            {
                PingTestManager manager = new PingTestManager(address, Config.Instance.PingLogValue, Config.Instance.Interval,
                                                                Config.Instance.UseTraceroute, Config.Instance.PingMode,
                                                                Config.Instance.ErrorsCount, Config.Instance.ReconnectInterval,
                                                                streamData: true);
                manager.StartTest();
                foreach(PingTest pt in manager.PingTests)
                {
                    
                }
            }
            
        }
    }
}
