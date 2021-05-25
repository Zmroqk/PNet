using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace PNetClient
{
    public partial class AddTestPage : UserControl
    {
        AddTestPageViewModel ViewModel { get; }

        public AddTestPage()
        {
            InitializeComponent();
            ViewModel = new AddTestPageViewModel();
            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Task.Run(() => ViewModel.TryLoadSavedHosts());
        }

        private void OnSavedTestsSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                string? selection = (string?)e.AddedItems[0];
                if(selection != null)
                {
                    ViewModel.dropDownSelection = selection;
                }
            }
        }

        private void OnButtonClickStart(object sender, RoutedEventArgs e)
        {
            ViewModel.StartTest();
        }
    }
}
