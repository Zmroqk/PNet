using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace PNetDll.Sqlite
{
    public class DatabaseModel
    {
        public PingDataQuery PingData { get; private set; }
        public HostsQuery Hosts { get; private set; }

        public DatabaseModel(SqliteConnection connection)
        {
            PingData = new PingDataQuery(connection);
            Hosts = new HostsQuery(connection);
        }
    }
}
