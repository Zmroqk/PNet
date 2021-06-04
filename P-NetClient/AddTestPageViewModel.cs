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
using System.Net.Sockets;

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

        /// <summary>
        /// Try loading saved hosts from file
        /// </summary>
        public void TryLoadSavedHosts()
        {
            try
            {
                if (!File.Exists(".hosts"))
                    return;
                ObservableCollection<string> savedHosts = JsonSerializer.Deserialize<ObservableCollection<string>>(File.ReadAllText(".hosts"));
                if (savedHosts.Count > 0)
                    savedHosts.Insert(0, "Unselect");
                if (savedHosts != null)
                    foreach (string host in savedHosts)
                        SavedHosts.Add(host);
            }
            catch (Exception e) { }                    
        }

        /// <summary>
        /// Save SavedHosts to file
        /// </summary>
        public void SaveHosts()
        {
            File.WriteAllText(".hosts", JsonSerializer.Serialize(SavedHosts.Where((elem) => elem != "Unselect")));
        }

        /// <summary>
        /// Start tests for selected or provided host
        /// </summary>
        public void StartTest()
        {
            try
            {              
                IPHostEntry hostEntry;
                try
                {
                    if (!string.IsNullOrEmpty(dropDownSelection))
                        hostEntry = Dns.GetHostEntry(dropDownSelection);
                    else
                        hostEntry = Dns.GetHostEntry(HostOrAddress);
                }
                catch (SocketException e)
                {
                    hostEntry = new IPHostEntry();
                    IPAddress address;
                    if (IPAddress.TryParse(HostOrAddress, out address))
                        hostEntry.AddressList = new IPAddress[] { address };
                    if (IPAddress.TryParse(dropDownSelection, out address))
                        hostEntry.AddressList = new IPAddress[] { address };
                }
                if (hostEntry.AddressList.Length > 0)
                {
                    if (!SavedHosts.Contains(HostOrAddress) && !string.IsNullOrEmpty(HostOrAddress))
                    {
                        SavedHosts.Add(HostOrAddress);
                        SaveHosts();
                    }
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
                        HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
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
