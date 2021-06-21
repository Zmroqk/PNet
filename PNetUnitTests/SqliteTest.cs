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

        [TestMethod("Test insert and get")]
        public void test1()
        {
            _1_InsertDate();
            _2_GetDate();
        }
  
        private void _2_GetDate()
        {
            List<Dates> dates = DatabaseConnection.Db.Dates.GetDatesAsync(dateTimeTest).Result;
            Assert.AreEqual(dateTimeTest, new DateTime(dates[0].Date));
        }

        private void _1_InsertDate()
        {
            Assert.AreEqual(1, DatabaseConnection.Db.Dates.AddDateAsync(dateTimeTest).Result);
        }
    }
}
