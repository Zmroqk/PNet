using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNetDll.Sqlite;
using System;
using System.Threading.Tasks;
using PNetDll.Sqlite.Models;
using System.Collections.Generic;

namespace PNetUnitTests
{
    [TestClass]
    public class SqliteTest
    {
        LocalDatabaseConnection DatabaseConnection;
        DateTime dateTimeTest;


        public SqliteTest()
        {
            DatabaseConnection = new LocalDatabaseConnection("PNetDb-test.db");
            DatabaseConnection.Connect();
            dateTimeTest = DateTime.Now;
        }

        [TestMethod("Test insert and get for host")]
        public void test_hosts()
        {
            Random r = new Random();       
            string hostname = r.Next().ToString();
            InsertHost(hostname);
            GetHost(hostname);
        }

        [TestMethod("Test host join ips")]
        public void test_join()
        {
            List<Hosts> hosts = DatabaseConnection.Db.Hosts.GetHosts("HostnameTest").JoinIps().Execute();
            Assert.AreEqual(1, hosts.Count);
            Assert.AreEqual("192.168.1.0", hosts[0].IPAddress);
        }

        private void GetHost(string hostname)
        {
            List<Hosts> hosts = DatabaseConnection.Db.Hosts.GetHosts(hostname).ExecuteAsync().Result;
            Assert.AreEqual(1, hosts.Count);
            Assert.AreEqual(hostname, hosts[0].Hostname);
        }

        private void InsertHost(string hostname)
        {
            Assert.AreEqual(1, DatabaseConnection.Db.Hosts.AddHostAsync(hostname).Result);
        }
    }
}
