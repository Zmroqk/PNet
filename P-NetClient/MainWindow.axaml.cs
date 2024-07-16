using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.ComponentModel;

namespace PNetClient
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance;

        public static readonly StyledProperty<Control> CurrentViewProperty =
            AvaloniaProperty.Register<MenuRow, Control>(nameof(CurrentView));

        public static List<TestPage> TestPages { get; } = new List<TestPage>();

        public Control CurrentView { 
            get { return GetValue(CurrentViewProperty); } 
            set { SetValue(CurrentViewProperty, value); }
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            foreach(TestPage testPage in TestPages)
            {
                testPage.Manager.Dispose();
            }
            base.OnClosing(e);
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Config.ReadConfiguration();
            CurrentView = new AddTestPage();
            Instance = this;
            Icon = new WindowIcon((Bitmap)App.Current.Resources["TracerouteIcon"]);
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
