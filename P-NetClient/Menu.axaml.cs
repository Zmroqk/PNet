using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using PNetClient.Pages;

namespace PNetClient
{
    public partial class Menu : UserControl
    {
        public static MenuRow? Tests { get; set; }

        public Menu()
        {
            InitializeComponent();
            Tests = this.FindControl<MenuRow>("TestsMenu");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ButtonAddTestClick(object sender, RoutedEventArgs e)
        {
            if (Parent != null)
                MainWindow.Instance.CurrentView = new AddTestPage();
        }

        public void ButtonConfigClick(object sender, RoutedEventArgs e)
        {
            if(Parent != null)
                MainWindow.Instance.CurrentView = new ConfigurationPage();
        }

        public void ButtonServiceClick(object sender, RoutedEventArgs e)
        {
            if (Parent != null)
                MainWindow.Instance.CurrentView = new ServicePage();
        }

        public void ButtonPdfClick(object sender, RoutedEventArgs e)
        {
            if (Parent != null)
                MainWindow.Instance.CurrentView = new PdfGenerationPage();
        }
    }
}
