using MahApps.Metro;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Zlo;

namespace ZloGUILauncher
{
    public partial class App : Application
    {
        App() : base()
        {
            try
            {                
                var args = Environment.GetCommandLineArgs();
                if (args.Length > 1 && args.Last().Trim('"') == "done")
                {
                    var bat_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "UpdateBat.bat");
                    File.Delete(bat_path);
                }

                if (args.Length > 1 && args.Last().Trim('"') == "reset")
                {
                    Settings.Default.Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private static API_ZloClient m_Client;
        public static API_ZloClient Client
        {
            get
            {
                if (m_Client == null)
                {
                    m_Client = new API_ZloClient();
                }
                return m_Client;
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
          //  string config = "Config.json";
            base.OnStartup(e);
            if (Settings.Default.Config ==null )
            {
                Settings.Default.Config = new Config();
                  // Configuration cfg = Settings.Default;
                 //  cfg.GetSectionGroup("userSettings");
                //  cfg.SaveAs(config);
               //string defaultAccent = "Cobalt";
              //string defaultTheme = "BaseDark";
             //ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(defaultAccent), ThemeManager.GetAppTheme(defaultTheme));
            }
            else
            {
                var acc = Settings.Default.Config.config.AccentColor;
                var Theme = Settings.Default.Config.config.Theme.ToString();
                ThemeManager.ChangeAppStyle(Current, ThemeManager.GetAccent(acc), ThemeManager.GetAppTheme(Theme.ToString()));
            }
        }
    }
}
