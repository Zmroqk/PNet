using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using PNetDll;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PNetClient
{
    public class App : Application
    {
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
            string path = location.Substring(0, location.LastIndexOf(delimiter)) + "/Images/";
            this.Resources.Add("PlusIcon", new Bitmap(path + "Plusik.png"));
            this.Resources.Add("CogIcon", new Bitmap(path + "Trybik.png"));
            this.Resources.Add("TestsIcon", new Bitmap(path + "TracerouteIcon.png"));
            this.Resources.Add("ServiceIcon", new Bitmap(path + "ServerIcon.png"));

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
