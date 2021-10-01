using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace PNetClient.Pages
{
    public partial class PdfGenerationPage : UserControl
    {

        PdfGenerationPageViewModel ViewModel { get; set; }

        public PdfGenerationPage()
        {
            InitializeComponent();
            ViewModel = new PdfGenerationPageViewModel();
            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ViewModel.LoadMoreData();
        }
    }
}
