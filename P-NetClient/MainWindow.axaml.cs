using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PNetClient
{
    public partial class MainWindow : Window
    {
        public static readonly StyledProperty<IControl> CurrentViewProperty =
            AvaloniaProperty.Register<MenuRow, IControl>(nameof(CurrentView));

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
