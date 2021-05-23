using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PNetClient
{
    public partial class ConfigurationPage : UserControl
    {
        public ConfigurationPage()
        {
            InitializeComponent();
            DataContext = Config.Instance;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            Config.Instance.SaveConfiguration();
        }
    }
}
