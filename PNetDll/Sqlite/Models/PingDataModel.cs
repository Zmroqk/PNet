using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll.Sqlite.Models
{
    public class PingDataModel
    {
        public uint PingDataModelId { get; set; }
        public DateTime Date { get; set; }
        public Ip Ip { get; set; }
        public int Ping { get; set; }
        public bool Success { get; set; }
    }
}
