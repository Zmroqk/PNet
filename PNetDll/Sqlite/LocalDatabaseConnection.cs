using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Reflection;

namespace PNetDll.Sqlite
{
    public class LocalDatabaseConnection
    {
        public SqliteConnection Connection { get; private set; }
        public DatabaseModel Db { get; private set; }

        string databaseFileName;

        public LocalDatabaseConnection(string databaseFileName) { this.databaseFileName = databaseFileName; }

        public async void Connect()
        {
            Connection = new SqliteConnection($"Data Source={databaseFileName}");
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string path = assemblyPath.Substring(0, assemblyPath.LastIndexOf(Path.DirectorySeparatorChar));
            await Connection.OpenAsync();
            string firstCmd = File.ReadAllText(Path.Join(path, "Sqlite", "PNetDb.db.sql"));
            SqliteCommand cmd = Connection.CreateCommand();
            cmd.CommandText = firstCmd;
            _ = await cmd.ExecuteNonQueryAsync();
            string versionCheckCmd = @"SELECT Version FROM Version LIMIT 1";
            cmd = Connection.CreateCommand();
            cmd.CommandText = versionCheckCmd;
            string version = (string)await cmd.ExecuteScalarAsync();
            if (version != "1.0.0") { }
            Db = new DatabaseModel(Connection);
        }
    }
}
