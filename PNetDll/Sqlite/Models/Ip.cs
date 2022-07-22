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
        public List<TestSnapshot> TestSnapshots { get; } = new List<TestSnapshot>();

        public override bool Equals(object obj)
        {
            Ip otherIp = obj as Ip;
            if(otherIp == null)
            {
                return false;
            }
            return otherIp.IpId == this.IpId;
        }
    }
}
