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
    public class DatesQuery
    {
        SqliteConnection connection;
        
        public DatesQuery(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public async Task<List<Models.Dates>> GetDatesAsync()
        {
            return connection.QueryAsync<Models.Dates>("SELECT * FROM Dates").Result.ToList();
        }

        public async Task<List<Models.Dates>> GetDatesAsync(DateTime from, bool reverse=false)
        {
            
            SqliteCommand cmd = connection.CreateCommand();
            if(reverse)
                return connection.QueryAsync<Models.Dates>("SELECT * FROM Dates WHERE Date <= @date", new { date = from.Ticks }).Result.ToList();
            else
                return connection.QueryAsync<Models.Dates>("SELECT * FROM Dates WHERE Date >= @date", new { date = from.Ticks }).Result.ToList();
        }

        public async Task<List<Models.Dates>> GetDatesAsync(DateTime from, DateTime to)
        {
            if (from < to)
                throw new ArgumentException("From date cannot be older that to date");
            return connection.QueryAsync<Models.Dates>("SELECT * FROM Dates WHERE Date <= @to AND Date >= @from", 
                new { 
                    from = from.Ticks,
                    to = to.Ticks
                }).Result.ToList();
        }

        public async Task<int> AddDateAsync(DateTime dateTime)
        {
            return await connection.ExecuteAsync("INSERT INTO Dates (Date) VALUES (@date)", new { date = dateTime.Ticks });
        }
    }
}
