using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll
{
    /// <summary>
    /// Delegate for PingCompleted event
    /// </summary>
    /// <param name="sender">Test that called this event</param>
    /// <param name="pingData">Ping data</param>
    public delegate void PingCompletedEventHandler(object? sender, PingData pingData);
}
