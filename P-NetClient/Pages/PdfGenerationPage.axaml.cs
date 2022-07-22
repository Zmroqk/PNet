using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.IO;
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

        public void ButtonOpenPdfFolder(object sender, RoutedEventArgs e)
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PNet", "pdfs");
            if (Directory.Exists(folderName))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(folderName)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch(Exception) { }
            }
        }
    }
}
