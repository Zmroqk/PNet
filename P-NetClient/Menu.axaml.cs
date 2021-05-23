using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace PNetClient
{
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnButtonClick(object sender, RoutedEventArgs e)
        {
            
        }

        public void ButtonConfigClick(object sender, RoutedEventArgs e)
        {
            if(Parent != null)
                ((MainWindow)Parent.Parent).CurrentView = new ConfigurationPage();
        }

        public void ButtonServiceClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
