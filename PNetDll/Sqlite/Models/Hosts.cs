using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll.Sqlite.Models
{
    public class Hosts
    {
        public uint HostId { get; set; }
        public string Hostname { get; set; }
        public uint? IpId { get; set; }
        public string IPAddress { get; set; }
    }
}
