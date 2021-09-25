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
        DateTime dateTimeTest;


        public SqliteTest()
        {
            dateTimeTest = DateTime.Now;
        }
    }
}
