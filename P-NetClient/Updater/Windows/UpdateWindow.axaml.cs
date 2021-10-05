using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.ComponentModel;

namespace PNetClient.Updater.Windows
{
    public partial class UpdateWindow : Window
    {
        UpdateWindowViewModel ViewModel;

        public UpdateWindow(Updater updater)
        {
            InitializeComponent();
            ViewModel = new UpdateWindowViewModel(this, updater);
            DataContext = ViewModel;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public UpdateWindow()
        {
            InitializeComponent();
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
