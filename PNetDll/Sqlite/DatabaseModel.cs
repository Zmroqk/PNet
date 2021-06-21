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
        public DatesQuery Dates { get; private set; }

        public DatabaseModel(SqliteConnection connection)
        {
            Dates = new DatesQuery(connection);
        }
    }
}
