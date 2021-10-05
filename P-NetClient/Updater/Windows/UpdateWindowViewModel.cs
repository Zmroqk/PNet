using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetClient.Updater.Windows
{
    class UpdateWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string UpdateVersion {
            get
            {
                return _UpdateVersion;
            }
            set
            {
                _UpdateVersion = $"Update to {value} is available";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UpdateVersion"));
            }
        }
        string _UpdateVersion = "";
        Updater Updater { get; set; }
        UpdateWindow Window {get; set;}
        public UpdateWindowViewModel(UpdateWindow window, Updater updater)
        {
            Updater = updater;
            Window = window;
            ButtonsEnabled = true;
            UpdateVersion = Updater.UpdateData.tag_name;
        }
        public bool ButtonsEnabled
        {
            get
            {
                return _ButtonsEnabled;
            }
            set
            {
                _ButtonsEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonsEnabled"));
            }
        }

        public bool _ButtonsEnabled;

        public async void DownloadNewVersion()
        {
            ButtonsEnabled = false;
            await Updater.DownloadNewVersion();
            Window.Close();
        }

        public void SkipVersion()
        {
            Window.Close();
        }
    }
}
