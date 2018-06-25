using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ZloGUILauncher.Properties;

namespace ZloGUILauncher
{
    [Serializable]
    public class Params 
    {
        public string AccentName { get; set; }
        public string AccentColorType { get; set; }
        public string Theme { get; set; }
        public bool MoreColors { get; set; }
        public bool isDebug { get; set; }
        public bool MaximizeGameWindow { get; set; }
        public bool UseExternalImage { get; set; }
        public string ImagePath { get; set; }
        public bool autostartZclient { get; set; }
        public bool isMusicEnabled { get; set; }
        public string ZclientPath { get; set; }
        public bool SaveLogInFile { get; set;}
        public bool CheckUpdates { get; set; }
        public   Color clr { get; set; } 
        public bool ModSupport { get; set; }
    }

    [Serializable]
    public class Config 
    {
        public Params config { get; set; }
      
        public Config()
        {
            config  = new Params();
            config.AccentName = "Cobalt";
            config.AccentColorType = "accent".ToLower();
            config.Theme = "BaseDark";
            config.isDebug = true;
            config.MoreColors = false;
            config.MaximizeGameWindow = false;
            config.UseExternalImage = false;
            config.ImagePath = "";
            config.ZclientPath = "";
            config.autostartZclient = false;
            config.isMusicEnabled = false;
            config.SaveLogInFile = false;
            config.CheckUpdates = true;
            config.ModSupport = false;
        }

        //public bool isDebug
        //{
        //    get { return config.isDebug; }

        //    set
        //    {
        //        config.isDebug = value;
        //        RaisePropertyChanged("isDebug");
        //    }
        //}

        //public bool isMusicEnabled
        //{
        //    get { return config.isMusicEnabled; }
        //    set
        //    {
        //        config.isMusicEnabled = value;
        //        RaisePropertyChanged("isMusicEnabled");
        //    }
        //}

        //public void RaisePropertyChanged(string propertyName)
        //{
        //    // Если кто-то на него подписан, то вызывем его
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}


    }
}
