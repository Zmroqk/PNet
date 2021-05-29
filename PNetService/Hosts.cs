using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetService
{
    /// <summary>
    /// Hosts to which service should run tests
    /// </summary>
    public class Hosts
    {
        /// <summary>
        /// Hosts domains list
        /// </summary>
        public List<string> Domains { get; set; } = new List<string>();

        /// <summary>
        /// Hosts Ips list
        /// </summary>
        public List<string> Ips { get; set; } = new List<string>();
    }
}
