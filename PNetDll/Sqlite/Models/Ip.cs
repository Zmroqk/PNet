using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PNetDll.Sqlite.Models
{
    [Index("IPAddress", IsUnique = true)]
    public class Ip
    {
        public uint IpId { get; set; }
        public string IPAddress { get; set; }
        public string Hostname { get; set; }
        public List<PingDataModel> PingsData { get; set; }
    }
}
