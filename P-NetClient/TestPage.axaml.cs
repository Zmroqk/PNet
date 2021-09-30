using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using OxyPlot;
using OxyPlot.Avalonia;
using PNetDll;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PNetDll.Logging;

namespace PNetClient
{
    public partial class TestPage : UserControl
    {
        public PingTestManager Manager { get; set; }
        public List<Logger> Loggers { get; set; }
        ListBox ListBox { get; set; }
        Plot PingPlot { get; set; }
        public Button CallerButton { get; set; }

        public ObservableCollection<DataPoint> Values { get; set; }

        bool TestInitializationCompleted { get; set; }

        public static readonly StyledProperty<string> TestNameProperty =
            AvaloniaProperty.Register<MenuRow, string>(nameof(TestName));

        public string TestName {
            get { return GetValue(TestNameProperty); }
            set { SetValue(TestNameProperty , $"Test for: {value}"); this.RaisePropertyChanged(TestNameProperty, null, _testName); } 
        }
        string _testName;

        public TestPage()
        {
            InitializeComponent();
            DataContext = this;
            ListBox = this.Find<ListBox>("TestsList");
            PingPlot = this.Find<Plot>("PingPlot");
            Values = new ObservableCollection<DataPoint>();
            //PingPlot.Series.Add(new LineSeries() { Items = Values });
            PingPlot.Series[0].Items = Values;
            PingPlot.ActualModel.Axes[0].MajorStep = 25;
            PingPlot.ActualModel.Axes[0].Title = "Index";
            PingPlot.ActualModel.Axes[1].MajorStep = 50;
            PingPlot.ActualModel.Axes[1].Title = "Ping";
            PlotView plotView = (PlotView)App.Current.Resources["PlotView"];
            plotView.Model = PingPlot.ActualModel;
            PingPlot.DefaultTrackerTemplate = plotView.DefaultTrackerTemplate;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TestName = Manager.DestinationHost.ToString();
            Task.Run(() => {
                Manager.StartTest();
                foreach (Logger logger in Loggers)
                {
                    logger.StartAutomaticLogging();
                }
                PingTest pt = Manager.PingTests.Find((pt) => pt.IpAddress.MapToIPv4().Equals(Manager.DestinationHost.MapToIPv4()));               
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if(pt == null)
                    {
                        throw new System.Exception("Test failed to start");
                    }
                    Manager.History[pt].CollectionChanged += TestPage_CollectionChanged;
                    ListBox.Items = Manager.PingTests;
                    TestInitializationCompleted = true;
                });
            });
        }

        private void TestPage_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                lock (Values)
                {
                    if (e.NewItems != null)
                    {
                        foreach (PingData pd in e.NewItems)
                        {
                            Values.Add(new DataPoint(pd.Index, pd.Ping));                        
                            while (Values.Count > 200)
                                Values.RemoveAt(0);
                        }
                        PingPlot.InvalidatePlot(true);
                    }
                }
            });
        }

        private void btnStopTest(object sender, RoutedEventArgs e)
        {
            PingTest pt = Manager.PingTests.Find((pt) => pt.IpAddress.MapToIPv4().Equals(Manager.DestinationHost.MapToIPv4()));
            if(TestInitializationCompleted)
                Manager.History[pt].CollectionChanged -= TestPage_CollectionChanged;
            foreach(Logger logger in Loggers)
            {
                logger.Dispose();
            }
            Manager.Dispose();
            MainWindow.TestPages.Remove(this);
            MainWindow.Instance.CurrentView = new AddTestPage();
            Menu.Tests.SubmenuStack.Children.Remove(CallerButton);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
