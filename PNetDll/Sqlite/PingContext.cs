using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PNetDll.Sqlite.Models;

namespace PNetDll.Sqlite
{
    public class PingContext : DbContext
    {
        public DbSet<Disconnect> Disconnects { get; set; }
        public DbSet<Ip> Ips { get; set; }
        public DbSet<PingDataModel> PingsData { get; set; }

        public string DbPath { get; private set; }

        public PingContext()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            DbPath = Path.Join(path, "PNet", "PNetDb.db");
            if (Database.GetMigrations().GetEnumerator().MoveNext())
            {
                Database.Migrate();
            }          
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");            
        }
    }
}
