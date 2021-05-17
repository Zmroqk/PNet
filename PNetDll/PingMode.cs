using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll
{
    /// <summary>
    /// Ping mode for tests
    /// </summary>
    public enum PingMode
    {
        /// <summary>
        /// All pings are performed at the same time
        /// </summary>
        Simultaneous,
        /// <summary>
        /// Only one ping is performed at the tick
        /// </summary>
        Asynchronously
    }
}
