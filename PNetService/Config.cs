﻿using PNetDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PNetService
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
        public string OutputPath { get; set; }
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

        public static void ReadConfig()
        {
            if (Instance == null)
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                Instance = JsonSerializer.Deserialize<Config>(File.ReadAllText("/etc/PNet"));
                else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PNet");
                    Instance = JsonSerializer.Deserialize<Config>(File.ReadAllText(path + "\\PNet.json"));
                }
                    
            }              
        }
    }
}