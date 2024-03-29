﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PNetDll;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PNetDll.Logging;

namespace PNetClient
{
    /// <summary>
    /// Logs pings data from tests
    /// </summary>
    public class FileLogger : Logger, IDisposable
    {
        /// <summary>
        /// File stream to specific PingTests
        /// </summary>
        FileStream? FileStream { get; set; }

        /// <summary>
        /// Streamwriter to file
        /// </summary>
        StreamWriter? sw;

        /// <summary>
        /// Default constructor, requires specyfing PingTestManager from which logger should be created
        /// </summary>
        /// <param name="manager">Manager from which logger should take data</param>
        public FileLogger(PingTestManager manager) : base(manager) { }

        /// <summary>
        /// Initialise logging
        /// </summary>
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

        /// <summary>
        /// Create log test for specific PingTest
        /// </summary>
        /// <param name="test"></param>
        void LogTest()
        {
            sw = new StreamWriter(FileStream);
            //MemoryStream dataStream = Manager.HistoryStream[pt];
            foreach (PingTest pt in Manager.History.Keys)
                Manager.History[pt].CollectionChanged += HistoryCollectionChanged;
        }

        /// <summary>
        /// Log operation called on history CollectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        sw.WriteLine($"Packet Index: {data.Index,-6} | {data.DateTime.ToLongTimeString(),-15} | IP: {data.IPAddress,-40} | Ping: {data.Ping,-4}");
                }
                sw.Flush();
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
        }

        public override void Dispose()
        {
            if(FileStream != null)
                FileStream.Close();
            foreach (PingTest pt in Manager.History.Keys)
                Manager.History[pt].CollectionChanged -= HistoryCollectionChanged;
        }

        public override void StartAutomaticLogging()
        {
            StartLogging();
        }
    }
}
