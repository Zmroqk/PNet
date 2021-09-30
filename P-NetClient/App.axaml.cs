using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using PNetDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PNetClient
{
    public class App : Application
    {
        public static string? AppExecutablePath { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            char delimiter = '/';
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                delimiter = '\\';
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
                delimiter = '/';
            AppExecutablePath = location.Substring(0, location.LastIndexOf(delimiter));
            string path = AppExecutablePath + "/Images/";
            this.Resources.Add("PlusIcon", new Bitmap(path + "Plusik.png"));
            this.Resources.Add("CogIcon", new Bitmap(path + "Trybik.png"));
            this.Resources.Add("TestsIcon", new Bitmap(path + "TracerouteIcon.png"));
            this.Resources.Add("ServiceIcon", new Bitmap(path + "ServerIcon.png"));
            this.Resources.Add("TracerouteIcon", new Bitmap(path + "TracerouteIcon.ico"));

            Database.Db.Database.EnsureCreated();

            try
            {
                new Pdf.PdfGenerator().GeneratePdf(DateTime.Today, DateTime.Today.AddDays(1), Database.Db.Ips.Where((ip) => ip.IpId == 2).FirstOrDefault());
            }
            catch(Pdf.PdfGenerationException e)
            {
                Console.Error.WriteLine(e.Message);
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
