using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PNetDll.Sqlite.Models
{
    [Index("IPAddress", IsUnique = true)]
    public class Ip
    {
        public int IpId { get; set; }
        public string IPAddress { get; set; }
        public string Hostname { get; set; }
        public List<TestCase> TestCases { get; } = new List<TestCase>();
    }
}
