using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll
{
    /// <summary>
    /// Delegate for PingLimitExceeded event
    /// </summary>
    /// <param name="sender">Manager that called this event</param>
    /// <param name="pingLimitExceededData">Data about what test called this event and ping data</param>
    public delegate void PingLimitExceededEventHandler(object? sender, PingLimitExceededData pingLimitExceededData);

    public class PingLimitExceededData
    {
        /// <summary>
        /// Ping data
        /// </summary>
        public PingData PingData { get; init; }

        /// <summary>
        /// Test that sends ping data
        /// </summary>
        public PingTest PingTest { get; init; }
    }
}
