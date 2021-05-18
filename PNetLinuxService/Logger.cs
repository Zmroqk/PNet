using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PNetDll;
using System.Threading;

namespace PNetLinuxService
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
                    Manager.DestinationHost.ToString().Replace('.', '_'), 
                    DateTime.Today.ToString("yyyy_MM_dd") + "___" + pt.IpAddress.ToString().Replace('.', '_'));
                FileStreams.Add(pt, new FileStream(path, FileMode.Append));
                Tokens.Add(pt, new CancellationTokenSource());
                new Task(LogTest, pt, Tokens[pt].Token);
            }
        }

        async void LogTest(object test)
        {
            PingTest pt = (PingTest)test;
            StreamWriter sw = new StreamWriter(FileStreams[pt]);
            CancellationTokenSource cts = Tokens[pt];
            MemoryStream dataStream = Manager.HistoryStream[pt];
            while (true)
            {
                PingData data = await PingDataSerializationTool.DeserializeAsync(dataStream, cts);
                sw.WriteLine($"{data.DateTime.ToShortTimeString(),-25} | IP: {data.IPAddress,-50} | Ping: {data.Ping,-4} ");
                cts.Token.ThrowIfCancellationRequested();
            }
        }

        public void Dispose()
        {
            foreach(CancellationTokenSource cts in Tokens.Values)
            {
                cts.Cancel();
            }
        }
    }
}
