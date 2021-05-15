using System;
using System.Net;

namespace PNetDll
{
    [Serializable]
    public class PingData
    {
        [field: NonSerialized]
        public IPAddress IPAddress { get; init; }
        public string IPAddressString { get; init; }
        public string Hostname { get; init; }
        public DateTime DateTime { get; init; }
        public int Index { get; init; }
        public long Ping { get; init; }
        public bool Success { get; init; }
        public ushort ErrorCount { get; init; }

        public override string ToString()
        {
            return $"Test for: {IPAddress, -20}, {Hostname}\n" +
                $"Ping: {Ping,-5}\n" +
                $"Ping index: {Index, -5} Received at: {DateTime.ToShortTimeString(), -15}\n";
        }
    }
}