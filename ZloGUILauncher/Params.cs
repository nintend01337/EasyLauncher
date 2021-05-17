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
        public bool IsDebug { get; set; }
        public bool MaximizeGameWindow { get; set; }
        public bool UseExternalImage { get; set; }
        public string ImagePath { get; set; }
        public bool AutostartZclient { get; set; }
        public bool IsMusicEnabled { get; set; }
        public string ZclientPath { get; set; }
        public bool SaveLogInFile { get; set;}
        public bool CheckUpdates { get; set; }
        public Color Clr { get; set; } 
        public bool ModSupport { get; set; }
        public bool DiscordRPCEnabled { get; set; }
        public bool UseExternalShaders { get; set; }
    }

    [Serializable]
    public class Config 
    {
        public Params config { get; set; }
      
        public Config()
        {
            config = new Params
            {
                AccentName = "Crimson",
                AccentColorType = "accent".ToLower(),
                Theme = "BaseDark",
                IsDebug = true,
                MoreColors = false,
                MaximizeGameWindow = true,
                UseExternalImage = false,
                ImagePath = "",
                ZclientPath = "..",
                AutostartZclient = true,
                IsMusicEnabled = true,
                SaveLogInFile = false,
                CheckUpdates = true,
                ModSupport = false,
                DiscordRPCEnabled = true,
                UseExternalShaders = false,
              };
        }
    }
}
