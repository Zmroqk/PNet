using PNetDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PNetService
{
    /// <summary>
    /// Service wide config data
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Instance to current config
        /// </summary>
        public static Config Instance { get; set; }

        /// <summary>
        /// Minimal value for ping when it should be logged
        /// </summary>
        public int PingLogValue { get; set; } = 1;

        /// <summary>
        /// Interval between pings 
        /// </summary>
        public int Interval { get; set; } = 60000;

        /// <summary>
        /// Use traceroute test
        /// </summary>
        public bool UseTraceroute { get; set; } = false;

        /// <summary>
        /// Ping mode used by tests
        /// </summary>
        public PingMode PingMode { get; set; } = PingMode.Simultaneous;

        /// <summary>
        /// Errors after which tests switches to reconnect mode
        /// </summary>
        public short ErrorsCount { get; set; } = 3;

        /// <summary>
        /// Interval between reconnect pings
        /// </summary>
        public int ReconnectInterval { get; set; } = 120000;

        /// <summary>
        /// Output path for logs
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Hosts to which service should launch tests
        /// </summary>
        public Hosts Hosts { get; set; } = new Hosts();

        public Config()
        {
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PNet");
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                OutputPath = "var/log/PNet.d";
            }
        }

        /// <summary>
        /// Read service configuration from file
        /// </summary>
        public static void ReadConfig()
        {
            if (Instance == null)
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    Instance = JsonSerializer.Deserialize<Config>(File.ReadAllText("/etc/PNet"));
                    if (!File.Exists("/etc/PNet"))
                    {
                        File.WriteAllText("/etc/PNet", JsonSerializer.Serialize(Instance));
                    }                   
                }
                    
                else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PNet");
                    Instance = JsonSerializer.Deserialize<Config>(File.ReadAllText(path + "\\PNet.json"));
                    if (!File.Exists(path + "\\PNet.json"))
                    {
                        File.WriteAllText(path + "\\PNet.json", JsonSerializer.Serialize(Instance));
                    }
                }
                    
            }              
        }
    }
}
