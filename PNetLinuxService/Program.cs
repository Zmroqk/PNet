using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using PNetDll;
using System.Threading.Tasks;

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
                IPAddress address = Dns.GetHostAddresses(host)?[0];
                if(address != null)
                {
                    ips.Add(address);
                }
            }
            ips.AddRange(Config.Instance.Hosts.Ips.Select(ipString => IPAddress.Parse(ipString)));
            Dictionary<PingTestManager, Logger> loggers = new Dictionary<PingTestManager, Logger>();
            foreach(IPAddress address in ips)
            {
                try
                {
                    PingTestManager manager = new PingTestManager(address, Config.Instance.PingLogValue, Config.Instance.Interval,
                                                                Config.Instance.UseTraceroute, Config.Instance.PingMode,
                                                                Config.Instance.ErrorsCount, Config.Instance.ReconnectInterval,
                                                                logHistory: true);
                    Logger logger = new Logger(manager);
                    manager.StartTest();
                    loggers.Add(manager, logger);
                    logger.StartLogging();
                }
                catch(Exception e){
                    Console.Error.WriteLine(e.Message);
                }
            }
            Task.Run(() =>
            {
                ResetLoggersAtNewDay(loggers);
            });

            while (true)
            {
                Task.Delay(60000).Wait();
            }
        }

        private static async void ResetLoggersAtNewDay(Dictionary<PingTestManager, Logger> loggers)
        {
            await Task.Delay((int)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds + 1000);
            foreach (PingTestManager manager in loggers.Keys)
            {
                loggers[manager] = new Logger(manager);
            }
            ResetLoggersAtNewDay(loggers);
        }
    }
}
