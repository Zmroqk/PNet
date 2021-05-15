namespace PNetDll
{
    public class PingData
    {
        public int Index { get; init; }
        public long Ping { get; init; }
        public bool Success { get; init; }
        public ushort ErrorCount { get; init; }
    }
}