using MahApps.Metro;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Zlo;
using ZloGUILauncher.Addons;

namespace ZloGUILauncher
{
    public partial class App : Application
    {
        App()
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length > 1 && args.Last().Trim('"') == "done")
                {
                    var batPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateBat.bat");
                    File.Delete(batPath);
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
        private static API_ZloClient _mClient;
        public static API_ZloClient Client => _mClient ?? (_mClient = new API_ZloClient());

        protected override void OnStartup(StartupEventArgs e)
        {
            //  string config = "Config.json";
            SetupAccents();
            base.OnStartup(e);
            if (Settings.Default.Config == null)
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
                var acc = Settings.Default.Config.config.AccentName;
                var Theme = Settings.Default.Config.config.Theme;

                if (Settings.Default.Config.config.AccentColorType == "accent".ToLower())
                    ThemeManager.ChangeAppStyle(Current, ThemeManager.GetAccent(acc), ThemeManager.GetAppTheme(Theme));
               
                else
                {
                    var clr = Settings.Default.Config.config.Clr;
                    SolidColorBrush brush = new SolidColorBrush(Settings.Default.Config.config.Clr);
                    ThemeManagerHelper.CreateAppStyleBy((Color)clr);
                    var resDictName = $"ДОПЦВЕТ_{clr.ToString().Replace("#", string.Empty)}.xaml";
                    var fileName = Path.Combine(Path.GetTempPath(), resDictName);
                    var theme = ThemeManager.DetectAppStyle(Current);
                    var accent = ThemeManager.GetAccent(resDictName);
                    ThemeManager.ChangeAppStyle(Current, accent, theme.Item1);
                }
            }
        }

        private static void SetupAccents()
        {
            //Type col = (typeof(Colors));
            //var colors = col.GetProperties();

            ////var values = Enum.GetValues(typeof(Colors));
            //int i = 0;

            //using (StreamWriter sw = new StreamWriter("colors.txt"))
            //{
            //    foreach (var c in colors)
            //    {
            //        sw.Write(string.Format($"\"{c.GetValue(c)}\","));
            //        i++;
            //        if (i == 10) { sw.WriteLine();i = 0; }
            //    }
            //}

            string[] colors = {
"#FFF0F8FF","#FFFAEBD7","#FF00FFFF","#FF7FFFD4","#FFF0FFFF","#FFF5F5DC","#FFFFE4C4","#FF000000","#FFFFEBCD","#FF0000FF",
"#FF8A2BE2","#FFA52A2A","#FFDEB887","#FF5F9EA0","#FF7FFF00","#FFD2691E","#FFFF7F50","#FF6495ED","#FFFFF8DC","#FFDC143C",
"#FF00FFFF","#FF00008B","#FF008B8B","#FFB8860B","#FFA9A9A9","#FF006400","#FFBDB76B","#FF8B008B","#FF556B2F","#FFFF8C00",
"#FF9932CC","#FF8B0000","#FFE9967A","#FF8FBC8F","#FF483D8B","#FF2F4F4F","#FF00CED1","#FF9400D3","#FFFF1493","#FF00BFFF",
"#FF696969","#FF1E90FF","#FFB22222","#FFFFFAF0","#FF228B22","#FFFF00FF","#FFDCDCDC","#FFF8F8FF","#FFFFD700","#FFDAA520",
"#FF808080","#FF008000","#FFADFF2F","#FFF0FFF0","#FFFF69B4","#FFCD5C5C","#FF4B0082","#FFFFFFF0","#FFF0E68C","#FFE6E6FA",
"#FFFFF0F5","#FF7CFC00","#FFFFFACD","#FFADD8E6","#FFF08080","#FFE0FFFF","#FFFAFAD2","#FFD3D3D3","#FF90EE90","#FFFFB6C1",
"#FFFFA07A","#FF20B2AA","#FF87CEFA","#FF778899","#FFB0C4DE","#FFFFFFE0","#FF00FF00","#FF32CD32","#FFFAF0E6","#FFFF00FF",
"#FF800000","#FF66CDAA","#FF0000CD","#FFBA55D3","#FF9370DB","#FF3CB371","#FF7B68EE","#FF00FA9A","#FF48D1CC","#FFC71585",
"#FF191970","#FFF5FFFA","#FFFFE4E1","#FFFFE4B5","#FFFFDEAD","#FF000080","#FFFDF5E6","#FF808000","#FF6B8E23","#FFFFA500",
"#FFFF4500","#FFDA70D6","#FFEEE8AA","#FF98FB98","#FFAFEEEE","#FFDB7093","#FFFFEFD5","#FFFFDAB9","#FFCD853F","#FFFFC0CB",
"#FFDDA0DD","#FFB0E0E6","#FF800080","#FFFF0000","#FFBC8F8F","#FF4169E1","#FF8B4513","#FFFA8072","#FFF4A460","#FF2E8B57",
"#FFFFF5EE","#FFA0522D","#FFC0C0C0","#FF87CEEB","#FF6A5ACD","#FF708090","#FFFFFAFA","#FF00FF7F","#FF4682B4","#FFD2B48C",
"#FF008080","#FFD8BFD8","#FFFF6347","#00FFFFFF","#FF40E0D0","#FFEE82EE","#FFF5DEB3","#FFFFFFFF","#FFF5F5F5","#FFFFFF00",
"#FF9ACD32" };

            if (!Settings.Default.Config.config.MoreColors) return;
            foreach (string v in colors)
            {
                var clr = ColorConverter.ConvertFromString(v);
                if (clr != null) ThemeManagerHelper.CreateAppStyleBy((Color) clr);
            }

        }
    }
}
