using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PNetDll;
using System.Threading;
using System.Collections.ObjectModel;

namespace PNetService
{
    public class Logger : IDisposable
    {
        PingTestManager Manager { get; set; }
        Dictionary<PingTest, FileStream> FileStreams { get; set; }
        Dictionary<PingTest, CancellationTokenSource> Tokens { get; set; }

        public Logger(PingTestManager manager)
        {
            Manager = manager;
            FileStreams = new Dictionary<PingTest, FileStream>();
            Tokens = new Dictionary<PingTest, CancellationTokenSource>();
        }

        public void StartLogging()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Config.Instance.OutputPath, Manager.DestinationHost.ToString()));
            if (!di.Exists)
                di.Create();
            foreach (PingTest pt in Manager.PingTests)
            {
                string path = Path.Combine(Config.Instance.OutputPath, 
                    Manager.DestinationHost.ToString(), 
                    DateTime.Today.ToString("yyyy_MM_dd") + "___" + pt.IpAddress.ToString().Replace('.', '_'));
                FileStreams.Add(pt, new FileStream(path, FileMode.Append));
                LogTest(pt);
            }
        }

        async void LogTest(PingTest test)
        {
            PingTest pt = test;
            StreamWriter sw = new StreamWriter(FileStreams[pt]);
            //MemoryStream dataStream = Manager.HistoryStream[pt];
            Manager.History[pt].CollectionChanged += (sender, e) => {
                try
                {
                    if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                        return;
                    ObservableCollection<PingData> collection = (ObservableCollection<PingData>)sender;
                    foreach (PingData data in e.NewItems)
                    {
                        sw.WriteLine($"{data.DateTime.ToLongTimeString(),-15} | IP: {data.IPAddress,-40} | Ping: {data.Ping,-4} ");
                        collection.Remove(data);
                    }
                    sw.Flush();
                }
                catch (Exception exc)
                {
                    Console.Error.WriteLine(exc.Message);
                }
            };
        }

        public void Dispose()
        {
            foreach (FileStream fs in FileStreams.Values)
            {
                fs.Close();
            }
        }
    }
}
