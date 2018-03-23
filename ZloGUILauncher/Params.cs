using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZloGUILauncher
{
    [Serializable]
    public class Params 
    {
        public string AccentColor { get; set; }
        public string Theme { get; set; }
        public bool isDebug { get; set; }
        public bool MaximizeGameWindow { get; set; }
        public bool UseExternalImage { get; set; }
        public string ImagePath { get; set; }
        public bool autostartZclient { get; set; }
        public bool isMusicEnabled { get; set; }
        public string ZclientPath { get; set; }
    }

    [Serializable]
    public class Config : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Params config { get; set; }
        //public Theme theme { get; set; }
        //  internal string config = "Config.json";
        public Config()
        {
            config  = new Params();
            config.AccentColor = "Cobalt";
            config.Theme = "BaseDark";
            config.isDebug = false;
            config.MaximizeGameWindow = false;
            config.UseExternalImage = false;
            config.ImagePath = "";
            config.ZclientPath = "";
            config.autostartZclient = false;
            config.isMusicEnabled = false;
        }

        public bool isDebug
        {
            get { return config.isDebug; }

            set
            {
                config.isDebug = value;
                RaisePropertyChanged("isDebug");
            }
        }

        public bool isMusicEnabled
        {
            get { return config.isMusicEnabled; }
            set
            {
                config.isMusicEnabled = value;
                RaisePropertyChanged("isMusicEnabled");
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
