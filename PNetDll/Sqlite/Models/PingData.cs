using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll.Sqlite.Models
{
    public class PingData
    {
        public uint PingId { get; set; }
        public long Date { get; set; }
        public uint HostId { get; set; }
        public int Ping { get; set; }
        public bool Success { get; set; }
    }
}
