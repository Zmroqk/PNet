using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PNetDll.Sqlite.Models
{
    [Index("DestinationHostId", IsUnique=false)]
    public class TestCase
    {
        [Key]
        public int TestCaseId { get; set; }
        public int DestinationHostId { get; set; }
        [ForeignKey("DestinationHostId")]
        public Ip DestinationHost { get; set; }        
        public List<Ip> Ips { get; } = new List<Ip>();
        public List<Disconnect> Disconnects { get; } = new List<Disconnect>();
        public List<TestSnapshot> Snapshots { get; } = new List<TestSnapshot>();
        public DateTime testStarted { get; set; }
        public DateTime testEnded { get; set; }      
    }
}
