using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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

        public string[] DesiredGames = { "Battlefield 3", "Battlefield 4", "Battlefield Hardline" };
        public List<Game> gameslist = new List<Game>();

        public ReshadeInstaller()
        {
            InitializeComponent();
            //GameSelector.Items.Add("Battlefiled 3");
            //GameSelector.Items.Add("Battlefiled 4");
            //GameSelector.Items.Add("Battlefiled Hardline");
            CheckInstalledGames();
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
    }


    public class Game
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool isInstalled { get; set; }
        public bool isReshadeInstalled { get; set; }
    }
}
