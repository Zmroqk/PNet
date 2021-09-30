using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using PNetDll.Sqlite.Models;

namespace PNetDll.Sqlite
{
    public class PingContext : DbContext
    {
        public DbSet<Disconnect> Disconnects { get; set; }
        public DbSet<Ip> Ips { get; set; }
        public DbSet<PingDataModel> PingsData { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<TestSnapshot> Snapshots { get; set; }

        public string DbPath { get; private set; }

        public PingContext()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            DbPath = Path.Join(path, "PNet", "PNetDb.db");        
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}").EnableSensitiveDataLogging();            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ip>()
                .HasMany(ip => ip.TestCases)
                .WithMany(testCase => testCase.Ips);

            modelBuilder.Entity<TestCase>()
                .HasOne(tc => tc.DestinationHost)
                .WithOne();
        }
    }
}
