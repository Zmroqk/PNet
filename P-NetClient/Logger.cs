using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PNetDll;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PNetClient
{
    public class Logger : IDisposable
    {
        PingTestManager Manager { get; set; }
        FileStream FileStream { get; set; }
        StreamWriter sw;
        public Logger(PingTestManager manager)
        {
            Manager = manager;
        }

        public void StartLogging()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Config.Instance.OutputPath, Manager.DestinationHost.ToString()));
            if (!di.Exists)
                di.Create();
            string path = Path.Combine(Config.Instance.OutputPath,
                Manager.DestinationHost.ToString(),
                DateTime.Today.ToString("yyyy_MM_dd"));
            FileStream = new FileStream(path, FileMode.Append);
            LogTest();
        }

        void LogTest()
        {
            sw = new StreamWriter(FileStream);
            //MemoryStream dataStream = Manager.HistoryStream[pt];
            foreach (PingTest pt in Manager.History.Keys)
                Manager.History[pt].CollectionChanged += HistoryCollectionChanged;
        }

        private void HistoryCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    return;
                ObservableCollection<PingData> collection = (ObservableCollection<PingData>)sender;
                foreach (PingData data in e.NewItems)
                {
                    if (data.Ping >= Config.Instance.PingLogValue)
                        sw.WriteLine($"{data.DateTime.ToLongTimeString(),-15} | IP: {data.IPAddress,-40} | Ping: {data.Ping,-4} ");
                }
                sw.Flush();
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
        }

        public void Dispose()
        {
            FileStream.Close();
            foreach (PingTest pt in Manager.History.Keys)
                Manager.History[pt].CollectionChanged -= HistoryCollectionChanged;
        }
    }
}
