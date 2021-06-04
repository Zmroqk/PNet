using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetClient
{
    /// <summary>
    /// View model for loading data for charts
    /// </summary>
    public class ServicePageViewModel
    {
        ServicePage ServicePage;      

        public ServicePageViewModel(ServicePage servicePage)
        {
            ServicePage = servicePage;
        }

        /// <summary>
        /// Read data from all files created by service
        /// </summary>
        /// <returns>Dictionary of hosts and List of data connected to them</returns>
        public Task<Dictionary<string, List<(DateTime Time, int Ping)>>> ReadDataAsync()
        {
            return Task.Run(() => {
                Dictionary<string, List<(DateTime Time, int Ping)>> ServicePings = new Dictionary<string, List<(DateTime Time, int Ping)>>();
                DirectoryInfo di;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"PNet"));
                }
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    di = new DirectoryInfo("/var/log/PNet.d");
                }
                else
                {
                    return ServicePings;
                }
                if (!di.Exists)
                    return ServicePings;
                foreach(DirectoryInfo diIn in di.GetDirectories())
                {                  
                    foreach(FileInfo fi in diIn.GetFiles())
                    {                      
                        try
                        {
                            List<(DateTime Time, int Ping)> Pings = new List<(DateTime Time, int Ping)>();
                            string[] fileName = fi.Name.Split("___");
                            string host = fileName[1].Replace('_', '.');
                            StreamReader sr = new StreamReader(fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                            while (!sr.EndOfStream)
                            {
                                string? line = sr.ReadLine();
                                if (line != null)
                                {
                                        string[] data = line.Split('|');
                                        Pings.Add((DateTime.Parse($"{fileName[0].Replace('_', '.')} {data[0]}"), int.Parse(data[1])));
                                }
                            }
                            sr.Close();
                            if(ServicePings.ContainsKey(host))
                                ServicePings[host].AddRange(Pings);
                            else
                                ServicePings.Add(host, Pings);
                            ServicePings[host].Sort((first, second) => first.Time.CompareTo(second.Time));
                            if (ServicePings[host].Count > 10000)
                                ServicePings[host].RemoveRange(0, ServicePings[host].Count - 10000);
                        }                       
                        catch (Exception e) { }
                    }
                }
                return ServicePings;
            });
        }
    }
}
