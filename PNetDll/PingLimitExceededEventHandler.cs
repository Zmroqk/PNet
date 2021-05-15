using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll
{
    public delegate void PingLimitExceededEventHandler(object? sender, PingLimitExceededData pingLimitExceededData);

    public class PingLimitExceededData
    {
        public PingData PingData { get; init; }
        public PingTest PingTest { get; init; }
    }
}
