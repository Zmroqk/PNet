using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNetDll.Sqlite;
using PNetDll.Sqlite.Models;

namespace PNetDll.Logging
{
    public class DatabaseLogger : Logger
    {
        public DatabaseLogger(PingTestManager manager) : base(manager) { }

        public override void StartAutomaticLogging()
        {
            try
            {
                PingContext db = Database.Db;
                if (db.Ips.Where(ip => ip.IPAddress == Manager.DestinationHost.ToString()).FirstOrDefault() == null)
                {
                    Ip newIp = new Ip() { IPAddress = Manager.DestinationHost.ToString() };
                    db.Ips.Add(newIp);
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            foreach (PingTest pt in Manager.History.Keys)
            {
                Manager.History[pt].CollectionChanged += HistoryCollectionChanged;
            }              
        }

        /// <summary>
        /// Log operation called on history CollectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HistoryCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action != NotifyCollectionChangedAction.Add)
                    return;
                foreach (PingData data in e.NewItems)
                {
                    if (data.Ping >= Manager.PingLogValue)
                    {
                        using (PingContext db = Database.Db)
                        {
                            db.TestCases.Attach(Manager.TestCase);
                            PingDataModel pData = new PingDataModel()
                            {
                                Date = data.DateTime,
                                Ip = db.Ips.Where(ip => ip.IpId == data.Ip.IpId).FirstOrDefault(),
                                TestCase = Manager.TestCase,
                                Ping = (int)data.Ping,
                                Success = data.Success
                            };
                            db.PingsData.Add(pData);
                            db.SaveChanges();
                        }                        
                    }
                        
                }
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
        }

        public override void Dispose()
        {
            foreach (PingTest pt in Manager.History.Keys)
                Manager.History[pt].CollectionChanged -= HistoryCollectionChanged;
        }

    }
}
