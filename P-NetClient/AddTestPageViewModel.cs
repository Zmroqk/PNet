using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using PNetDll;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia.Styling;

namespace PNetClient
{
    public class AddTestPageViewModel
    {
        public ObservableCollection<string> SavedHosts { get; set; }
        public string HostOrAddress { get; set; }
        public string? dropDownSelection = null;

        public AddTestPageViewModel()
        {
            SavedHosts = new ObservableCollection<string>();        
        }

        public void TryLoadSavedHosts()
        {
            if (!File.Exists(".hosts"))
                return;
            ObservableCollection<string> savedHosts = JsonSerializer.Deserialize<ObservableCollection<string>>(File.ReadAllText(".hosts"));
            if (savedHosts != null)
                foreach (string host in savedHosts)
                SavedHosts.Add(host);
        }

        public void SaveHosts()
        {
            File.WriteAllText(".hosts", JsonSerializer.Serialize(SavedHosts));
        }

        public void StartTest()
        {
            try
            {
                IPHostEntry hostEntry;
                if (dropDownSelection != null)
                    hostEntry = Dns.GetHostEntry(dropDownSelection);
                else
                {
                    hostEntry = Dns.GetHostEntry(HostOrAddress);
                    if (!SavedHosts.Contains(HostOrAddress))
                    {
                        SavedHosts.Add(HostOrAddress);
                        SaveHosts();
                    }
                }
                if (hostEntry.AddressList.Length > 0)
                {
                    PingTestManager pingTestManager = new PingTestManager(hostEntry.AddressList[0], Config.Instance.PingLogValue, Config.Instance.Interval,
                                                                        Config.Instance.UseTraceroute, Config.Instance.PingMode, Config.Instance.ErrorsCount,
                                                                        Config.Instance.ReconnectInterval, true);
                    Logger logger = new Logger(pingTestManager);                   
                    Button btnTest = new Button()
                    {
                        Content = pingTestManager.DestinationHost.ToString(),
                        Margin = new Thickness(20, 0, 0, 0),
                        Width = 120,
                        MaxWidth = 120,
                        HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Background = new SolidColorBrush(Color.Parse("Gray"), 0.2f)
                    };
                    TestPage testPage = new TestPage() { Manager = pingTestManager, Logger = logger, CallerButton = btnTest };
                    MainWindow.TestPages.Add(testPage);
                    MainWindow.Instance.CurrentView = testPage;
                    Menu.Tests.SubmenuStack.Children.Add(btnTest);
                    btnTest.Click += (sender, e) => { MainWindow.Instance.CurrentView = testPage; };
                }
            }
            catch (Exception e) { }
        }
    }
}
