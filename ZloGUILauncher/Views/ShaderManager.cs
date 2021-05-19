using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Navigation;
using Zlo;
using Zlo.Extras;

namespace ZloGUILauncher.Views
{
    public static class ShaderManager
    {
        public static string dllname = "BF3FX.dll";
        public static string path = AppDomain.CurrentDomain.BaseDirectory;

        private static List<string> QuickDlls
        {
            get
            {
                return App.Client?.GetDllsList(ZloBFGame.BF_3);
            }
        }

        public  static void Install()
        {
            string fullpath = Path.Combine(path, dllname);
            if (!File.Exists(fullpath))
            {
                File.WriteAllBytes(fullpath, Properties.Resources.BF3FX);
            }

            var list = QuickDlls;
            if (list != null)
            {
                list.Add(fullpath);
                Zlo.Settings.TrySave();
            }
        }

        public static void Remove()
        {
            //string fullpath = Path.Combine(path, dllname);
            //if (File.Exists(fullpath))
            //{
            //    File.Delete(fullpath);
            //}
            var dz = QuickDlls;
            if (dz != null)
            {
                dz.Clear();
                Zlo.Settings.TrySave();
            }
        }
    }
}

