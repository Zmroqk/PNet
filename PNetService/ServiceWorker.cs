﻿using Microsoft.Extensions.Hosting;
using PNetDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PNetService
{
    /// <summary>
    /// Run service worker
    /// </summary>
    public class ServiceWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Config.ReadConfig();
            List<IPAddress> ips = new List<IPAddress>();
            foreach (string host in Config.Instance.Hosts.Domains)
            {
                IPAddress address = Dns.GetHostAddresses(host)?[0];
                if (address != null)
                {
                    ips.Add(address);
                }
            }
            ips.AddRange(Config.Instance.Hosts.Ips.Select(ipString => IPAddress.Parse(ipString)));
            Dictionary<PingTestManager, Logger> loggers = new Dictionary<PingTestManager, Logger>();
            foreach (IPAddress address in ips)
            {
                Console.WriteLine($"Starting test for: {address,-15}");
                PingTestManager manager = new PingTestManager(address, Config.Instance.PingLogValue, Config.Instance.Interval,
                                                                Config.Instance.UseTraceroute, Config.Instance.PingMode,
                                                                Config.Instance.ErrorsCount, Config.Instance.ReconnectInterval,
                                                                logHistory: true);
                Logger logger = new Logger(manager);
                manager.StartTest();
                loggers.Add(manager, logger);
                logger.StartLogging();
            }
            _ = Task.Run(() =>
              {
                  while (true)
                  {
                      ResetLoggersAtNewDay(loggers);
                      Task.Delay((int)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds + 1000).Wait();
                  }                    
              });

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000);
            }
        }

        /// <summary>
        /// Reset loggers after midnight
        /// </summary>
        /// <param name="loggers">Dictionary of managers for which loggers should be restarted</param>
        private async void ResetLoggersAtNewDay(Dictionary<PingTestManager, Logger> loggers)
        {
            await Task.Delay((int)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds + 1000);
            foreach (PingTestManager manager in loggers.Keys)
            {
                loggers[manager].Dispose();
                loggers[manager] = new Logger(manager);
                loggers[manager].StartLogging();
            }
        }
    }
}
