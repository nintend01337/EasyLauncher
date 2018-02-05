using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZloGUILauncher
{
    [Serializable]
    public sealed class Preferences
    {
        public Accent Accent { get; set; }
        public AppTheme AppTheme { get; set; }
        public bool isDebug { get; set; }
        public bool MaximizeGameWindow { get; set; }
        public bool UseExtImage { get; set; }
        public string ImagePath { get; set; }
        public bool startZclient { get; set; }

        static Preferences instance = null;

        public Preferences()
        { }

        public static Preferences Instance
        {
            get
            {
                if (instance == null)
                {
                    // instance = new Preferences();
                    instance = Preferences.Load();
                }
                return instance;
            }
             set { }
        }


        public static Preferences Load()
        {
            var AppThemeName = ThemeManager.GetAppTheme(Settings.Default.AppThemeName);
            var AppAccent = ThemeManager.GetAccent(Settings.Default.AccentName);
            bool isDebug = Settings.Default.isDebug;
            bool MaximizeGameWindow = Settings.Default.maximizeWindow;
            bool UsingExternalImage = Settings.Default.ExternalImage;
            bool isZstartup = Settings.Default.ZclStart;
            string ImagePath = Settings.Default.ExtImgPath;

            var settings = new Preferences
            {
                Accent = AppAccent,
                AppTheme = AppThemeName,
                isDebug = isDebug,
                MaximizeGameWindow = MaximizeGameWindow,
                UseExtImage = UsingExternalImage,
                ImagePath = ImagePath,
                startZclient = isZstartup
            };
                return settings;
        }

        public void  Save()
        {
            Settings.Default.AppThemeName = AppTheme.Name;
            Settings.Default.AccentName = Accent.Name;
            Settings.Default.isDebug = isDebug;
            Settings.Default.maximizeWindow = MaximizeGameWindow;
            Settings.Default.ExternalImage = UseExtImage;
            Settings.Default.ExtImgPath = ImagePath;
            Settings.Default.ZclStart = startZclient;

            Settings.Default.Save();
        }

        public void ResetbyDefault()
        {
            Settings.Default.AppThemeName = "BaseDark";
            Settings.Default.AccentName = "Cobalt";
            Settings.Default.isDebug = false;
            Settings.Default.maximizeWindow = false;
            Settings.Default.ExternalImage = false;
            Settings.Default.ExtImgPath = "";
            Settings.Default.ZclStart = false;
        }
    }
}
