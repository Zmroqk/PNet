using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNetDll.Sqlite;


namespace PNetDll
{
    public class Database
    {
        public static PingContext Db {
            get {
                return new PingContext();
            }
        }
    }
}
