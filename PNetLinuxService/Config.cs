using PNetDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PNetLinuxService
{
    public class Config
    {
        public static Config Instance { get; set; }
        public int PingLogValue { get; set; } = 1;
        public int Interval { get; set; } = 60000;
        public bool UseTraceroute { get; set; } = false;
        public PingMode PingMode { get; set; } = PingMode.Simultaneous;
        public short ErrorsCount { get; set; } = 3;
        public int ReconnectInterval { get; set; } = 120000;
        public string OutputPath { get; set; } = "C:/var/log/PNet.d";
        public Hosts Hosts { get; set; } = new Hosts();


        public static void ReadConfig()
        {
            if (Instance == null)
                Instance = JsonSerializer.Deserialize<Config>(File.ReadAllText("C:/etc/PNet"));
        }
    }
}
