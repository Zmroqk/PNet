using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll.Sqlite.Models
{
    public class Disconnects
    {
        public uint DiscId { get; set; }
        public uint IPId { get; set; }
        public uint DisconnectDateId { get; set; }
        public uint ReconnectDateId { get; set; }
    }
}
