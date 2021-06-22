using Dapper;
using Microsoft.Data.Sqlite;
using PNetDll.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PNetDll.Sqlite
{
    public class HostsQuery
    {
        SqliteConnection connection;

        public string sqlQuery;
        public Regex queryRegex;

        public HostsQuery(SqliteConnection connection)
        {
            this.connection = connection;
            queryRegex = new Regex(
                @"(?<SELECT>SELECT \S+)(?<FROM> FROM \S+)(?<JOIN> JOIN \S+ ON \S+ = \S+)*(?<WHERE> WHERE \S+ (= | LIKE) \S+)?"
                );
        }

        public HostsQuery JoinIps()
        {
            string joinQuery = "JOIN HostIps ON Hosts.HostId = HostIps.IPId " +
                            "JOIN Ips ON HostIps.IPId = Ips.IpId";
            sqlQuery = queryRegex.Replace(sqlQuery, 
                $"SELECT Hosts.HostId,Hostname,Ips.IpId as IpId,Ips.IPAddress as IPAddress${{FROM}} {joinQuery}${{WHERE}} ");
            return this;
        }

        public HostsQuery GetHosts(int hostId)
        {
            sqlQuery = $"SELECT * FROM Hosts WHERE HostId = {hostId}";
            return this;
        }

        public HostsQuery GetHosts(string hostname)
        {
            sqlQuery = $"SELECT * FROM Hosts WHERE Hostname LIKE '{hostname}'";
            return this;
        }

        public List<Hosts> Execute()
        {
            return connection.Query<Hosts>(sqlQuery).ToList();
        }

        public async Task<List<Hosts>> ExecuteAsync()
        {
            return connection.QueryAsync<Hosts>(sqlQuery).Result.ToList();
        }

        public async Task<int> AddHostAsync(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new NullReferenceException("Hostname cannot be null/empty");
            return await connection.ExecuteAsync("INSERT INTO Hosts (Hostname) VALUES (@Hostname)",
                new
                {
                    Hostname = hostname
                });
        }
    }
}
