using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetLinuxService
{
    public class Hosts
    {
        public List<string> Domains { get; set; } = new List<string>();
        public List<string> Ips { get; set; } = new List<string>();
    }
}
