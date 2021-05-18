using System;
using System.Net;

namespace PNetDll
{
    /// <summary>
    /// Ping data schema
    /// </summary>
    [Serializable]
    public class PingData
    {
        /// <summary>
        /// Destination ip address for this ping
        /// </summary>
        [field: NonSerialized]
        public IPAddress IPAddress { get; init; }

        /// <summary>
        /// Destination ip address for this ping as string
        /// </summary>
        public string IPAddressString { get; init; }

        /// <summary>
        /// Resolved dns name for ip address
        /// </summary>
        public string Hostname { get; init; }

        /// <summary>
        /// Time when this ping has been performed
        /// </summary>
        public DateTime DateTime { get; init; }

        /// <summary>
        /// Index for this ping
        /// </summary>
        public int Index { get; init; }

        /// <summary>
        /// Ping value
        /// </summary>
        public long Ping { get; init; }

        /// <summary>
        /// True, if ping returned success
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Current error count
        /// </summary>
        public ushort ErrorCount { get; init; }

        public override string ToString()
        {
            return $"Test for: {IPAddress, -20}, {Hostname}\n" +
                $"Ping: {Ping,-5}\n" +
                $"Ping index: {Index, -5} Received at: {DateTime.ToShortTimeString(), -15}\n";
        }
    }
}