using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZloGUILauncher.Views
{
    public partial class ReshadeInstaller : MetroWindow
    {
        //private static ReshadeInstaller installer;

        public readonly string gamenode = @"SOFTWARE\WOW6432Node\EA Games";
        public string gamepath;
        readonly string[] _files =
        {
                @"dxgi.dll",
                @"dxgi.log",
                @"ReShade.ini",
       };

        public string[] DesiredGames = { "Battlefield 3", "Battlefield 4", "Battlefield Hardline" };
        public List<Game> gameslist = new List<Game>();

        public ReshadeInstaller()
        {
            InitializeComponent();
            //GameSelector.Items.Add("Battlefiled 3");
            //GameSelector.Items.Add("Battlefiled 4");
            //GameSelector.Items.Add("Battlefiled Hardline");
            CheckInstalledGames();
            CheckReshadeInstall();
            DataContext = gameslist;
            GameSelector.ItemsSource = gameslist;
            GameSelector.DisplayMemberPath = "Name";

        }

        private void CheckInstalledGames()
        {
            foreach (string game in DesiredGames)
            {
                GetInstallPath(game);
            }
        }

        private void CheckReshadeInstall()
        {
            foreach(Game g in gameslist)
            {
                g.isReshadeInstalled = IsReshadeInstalled(g.Path);
            //    PropertyChanged.Invoke(g, new PropertyChangedEventArgs("isReshadeInstalled"));
            }
        }

        private bool IsReshadeInstalled(string dir)
        {
            try
            {
                var filelist = new DirectoryInfo(dir).GetFiles("*.*", SearchOption.TopDirectoryOnly);
                var dirslist = new DirectoryInfo(dir).GetDirectories("*", SearchOption.TopDirectoryOnly);
                return (filelist.Any(file => _files.Contains(file.Name) || dirslist.Any(d => d.Name == "Reshade")));
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }            
        }

        private void GetInstallPath(string name)
        {
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(gamenode))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (subkey != null && subkey.GetValue("DisplayName").ToString().Contains(name))
                        {
                            //  gamepath = subkey.GetValue("Install Dir").ToString();
                            gameslist.Add(new Game() { Name = name, Path = subkey.GetValue("Install Dir").ToString(), isInstalled = true, });
                        }
                    }
                }
            }
        }

        private void InstallReshade(object sender, RoutedEventArgs e)
        {
            byte[] bytes = Properties.Resources.Reshade;
            Stream stream = new MemoryStream(bytes);
            int index = GameSelector.SelectedIndex;
            string path = gameslist.ElementAt(index).Path;

            using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                zip.ExtractToDirectory(path);
            }

            gameslist.ElementAt(index).isReshadeInstalled = true;
            gameslist.ElementAt(index).OPC("isReshadeInstalled");
            //  this.Close();
        }

        private void DeleteReshade(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить решейд? \n Все пресеты и скриншоты будут удалены также!", "Reshade REMOVE!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
               // var s = GameSelector.SelectedItem;
                var index = GameSelector.SelectedIndex;
                var obj = gameslist.ElementAt(index);
                string path = gameslist.ElementAt(index).Path;

                var dir = new DirectoryInfo(path);

                var dirs = dir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                var files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);

                foreach (var f in files)
                {
                    if (_files.Contains(f.Name))
                    {
                        f.Delete();
                    }
                }

                foreach(var d in dirs)
                {
                    if(d.Name== "Reshade")
                    {
                        d.Delete(true);
                    }
                }
                gameslist.ElementAt(index).isReshadeInstalled = false;
                gameslist.ElementAt(index).OPC("isReshadeInstalled");
                //PropertyChanged.Invoke(this, new PropertyChangedEventArgs("isReshadeInstalled"));
                //  this.Close();

            }
        }
    }


    public class Game : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool isInstalled { get; set; }
        public bool isReshadeInstalled { get; set; }

        public void OPC(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
