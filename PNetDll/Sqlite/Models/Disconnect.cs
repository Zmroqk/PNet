using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll.Sqlite.Models
{
    public class Disconnect
    {
        public int DisconnectId { get; set; }
        public Ip ConnectedIp { get; set; }
        public TestCase Test { get; set; }
        public DateTime DisconnectDate { get; set; }
        public DateTime ReconnectDate { get; set; }
    }
}
