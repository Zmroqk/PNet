using PNetDll;
using System.IO;
using System.Text.Json;

namespace PNetClient
{
    /// <summary>
    /// Application wide config data
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Instance to current config
        /// </summary>
        public static Config Instance { get; set; } = new Config();

        /// <summary>
        /// Minimal value for ping when it should be logged
        /// </summary>
        public int PingLogValue { get; set; } = 100;

        /// <summary>
        /// Interval between pings 
        /// </summary>
        public int Interval { get; set; } = 1000;

        /// <summary>
        /// Use traceroute test
        /// </summary>
        public bool UseTraceroute { get; set; } = true;

        /// <summary>
        /// Ping mode used by tests
        /// </summary>
        public PingMode PingMode { get; set; } = PingMode.Asynchronously;

        /// <summary>
        /// Save mode used by tests
        /// </summary>
        public bool UseFileSave { get; set; } = false;

        /// <summary>
        /// Errors after which tests switches to reconnect mode
        /// </summary>
        public short ErrorsCount { get; set; } = 3;

        /// <summary>
        /// Interval between reconnect pings
        /// </summary>
        public int ReconnectInterval { get; set; } = 5000;

        /// <summary>
        /// Output path for logs
        /// </summary>
        public string OutputPath { get; set; } = Path.Combine(App.AppExecutablePath, "Logs");

        /// <summary>
        /// Save config instance to file, change config in already running tests
        /// </summary>
        public void SaveConfiguration()
        {
            File.WriteAllText(Path.Combine(App.AppExecutablePath, ".config"), JsonSerializer.Serialize(this));
            foreach (TestPage tp in MainWindow.TestPages)
            {
                tp.Manager.Interval = Interval;
                tp.Manager.ReconnectInterval = ReconnectInterval;
                tp.Manager.ErrorsCount = ErrorsCount;
                tp.Manager.PingLogValue = PingLogValue;
            }               
        }

        /// <summary>
        /// Read configuration from file
        /// </summary>
        public static void ReadConfiguration()
        {
            if (File.Exists(Path.Combine(App.AppExecutablePath, ".config")))
            {
                Config? config = JsonSerializer.Deserialize<Config>(File.ReadAllText(Path.Combine(App.AppExecutablePath, ".config")));
                if (config != null)
                    Instance = config;
            }
        }
    }
}
