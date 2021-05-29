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

    /// <summary>
    /// Logs pings data from tests
    /// </summary>
    public class Logger : IDisposable
    {
        /// <summary>
        /// Manager for this instance of logger
        /// </summary>
        PingTestManager Manager { get; set; }
        /// <summary>
        /// File stream to specific PingTests
        /// </summary>
        Dictionary<PingTest, FileStream> FileStreams { get; set; }
        Dictionary<PingTest, CancellationTokenSource> Tokens { get; set; }

        /// <summary>
        /// Default constructor, requires specyfing PingTedtManager from which logger should be created
        /// </summary>
        /// <param name="manager">Manager from which logger should take data</param>
        public Logger(PingTestManager manager)
        {
            Manager = manager;
            FileStreams = new Dictionary<PingTest, FileStream>();
            Tokens = new Dictionary<PingTest, CancellationTokenSource>();
        }

        /// <summary>
        /// Initialise logging
        /// </summary>
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
                FileStreams.Add(pt, new FileStream(path, FileMode.Append, FileAccess.Write));
                LogTest(pt);
            }
        }

        /// <summary>
        /// Create log test for specific PingTest
        /// </summary>
        /// <param name="test"></param>
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
                        sw.WriteLine($"{data.DateTime.ToLongTimeString()}|{data.Ping}");
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
