using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System.Collections.Generic;

namespace PNetClient
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance;

        public static readonly StyledProperty<IControl> CurrentViewProperty =
            AvaloniaProperty.Register<MenuRow, IControl>(nameof(CurrentView));

        public static List<TestPage> TestPages { get; } = new List<TestPage>();

        public IControl CurrentView { 
            get { return GetValue(CurrentViewProperty); } 
            set { SetValue(CurrentViewProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Config.ReadConfiguration();
            CurrentView = new ConfigurationPage();
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
