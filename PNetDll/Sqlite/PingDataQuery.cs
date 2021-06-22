using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PNetDll.Sqlite.Models;

namespace PNetDll.Sqlite
{
    public class PingDataQuery
    {
        SqliteConnection connection;
        
        public PingDataQuery(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public async Task<int> AddPingDataAsync(uint hostId, PingData pingData)
        {
            return await connection.ExecuteAsync("INSERT INTO PingData (HostId, Date, Ping, Success) VALUES (@HostId, @Date, @Ping, @Success)", 
                new {
                    HostId = hostId,
                    Date = pingData.DateTime.Ticks,
                    Ping = pingData.Ping,
                    Success = pingData.Success
                });
        }
    }
}
