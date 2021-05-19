using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using PNetDll;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace PNetLinuxService
{
    class Program
    {
        public static void Main(string[] args)
        {
            if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                CreateLinuxHostBuilder(args).Build().Run();
            }
            else if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                CreateWindowsHostBuilder(args).Build().Run();
            }          
        }

        public static IHostBuilder CreateLinuxHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder().UseSystemd().ConfigureServices((services) =>
            {
                services.AddHostedService<ServiceWorker>();
            });
        }

        public static IHostBuilder CreateWindowsHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder().UseWindowsService().ConfigureServices((services) =>
            {
                services.AddHostedService<ServiceWorker>();
            });
        }
    }
}
