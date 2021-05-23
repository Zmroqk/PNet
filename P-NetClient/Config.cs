using PNetDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PNetClient
{
    public class Config
    {
        public static Config Instance { get; set; } = new Config();
        public int PingLogValue { get; set; } = 100;
        public int Interval { get; set; } = 1000;
        public bool UseTraceroute { get; set; } = true;
        public PingMode PingMode { get; set; } = PingMode.Asynchronously;
        public short ErrorsCount { get; set; } = 3;
        public int ReconnectInterval { get; set; } = 5000;
        public string OutputPath { get; set; } = "./Logs";

        public void SaveConfiguration()
        {
            File.WriteAllText(".config", JsonSerializer.Serialize(this));
        }

        public static void ReadConfiguration()
        {
            if (File.Exists(".config"))
            {
                Config? config = JsonSerializer.Deserialize<Config>(File.ReadAllText(".config"));
                if (config != null)
                    Instance = config;
            }             
        }
    }
}
