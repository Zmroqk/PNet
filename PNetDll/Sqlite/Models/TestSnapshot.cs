using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll.Sqlite.Models
{
    public class TestSnapshot
    {
        [Key]
        public int TestSnapshotId { get; set; }
        public TestCase TestCase { get; set; }
        public Ip Ip { get; set; }
        public DateTime SnapshotTaken { get; set; }
        public long AveragePing { get; set; }
        public long MaxPing { get; set; }
        public int PacketsSend { get; set; }
        public int PacketsReceived { get; set; }
    }
}
