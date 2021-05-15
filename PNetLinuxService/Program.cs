using System;
using System.Net;
using PNetDll;

namespace PNetLinuxService
{
    class Program
    {
        static void Main(string[] args)
        {
            PingTestManager manager = new PingTestManager(IPAddress.Parse("212.14.57.130"), traceRoute: true);
            manager.StartTest();
        }
    }
}
