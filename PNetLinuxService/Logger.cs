using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PNetDll;
using System.Threading;
using System.Collections.ObjectModel;

namespace PNetLinuxService
{
    public class Logger
    {
        PingTestManager Manager { get; set; }
        Dictionary<PingTest, FileStream> FileStreams { get; set; }

        public Logger(PingTestManager manager)
        {
            Manager = manager;
            FileStreams = new Dictionary<PingTest, FileStream>();
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
                if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    return;
                ObservableCollection<PingData> collection = (ObservableCollection<PingData>)sender;
                foreach(PingData data in e.NewItems)
                {
                    sw.WriteLine($"{data.DateTime.ToLongTimeString(),-25} | IP: {data.IPAddressString,-50} | Ping: {data.Ping,-4} ");
                    collection.Remove(data);
                }           
                sw.Flush();
            };
        }
    }
}
