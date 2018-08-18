﻿using MahApps.Metro;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZloGUILauncher.Addons;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.Forms.MessageBoxOptions;

namespace ZloGUILauncher.Views
{
    public partial class Options 
    {
        ClientSwitcher csw = new ClientSwitcher();
        public bool GameisEa = false;
        public string gPath = null;
        public Options()
        {
            InitializeComponent();
            gPath = csw.FindGameInstallation();
            GameisEa = csw.IsEALicense(gPath);
            if (GameisEa)
                Gswitch.Content = "Текущая версия игры: Лиц";
            else Gswitch.Content = "Текущая версия игры: Zlo";
        }
        private  void WriteSettings(object sender,RoutedEventArgs e)
        {
            Settings.Default.Save();
            //await DialogManager.ShowMessageAsync(new MahApps.Metro.Controls.MetroWindow(), "Ура", "Настройки сохранены", MessageDialogStyle.Affirmative);
        }

        private  void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            //DialogManager.ShowMessageAsync(new MahApps.Metro.Controls.MetroWindow(),"СБРОС", $"Внимание сейчас сбросятся настройки. \n Продолжить?", MessageDialogStyle.AffirmativeAndNegative)
            //var mySettings = new MetroDialogSettings()
            //{
            //    AffirmativeButtonText = "ДА",
            //    NegativeButtonText = "НЕТ",
            //};
            //MessageDialogResult result = await DialogManager.ShowMessageAsync(new MahApps.Metro.Controls.MetroWindow(), "СБРОС", $"Внимание сейчас сбросятся настройки. \n Продолжить?", MessageDialogStyle.AffirmativeAndNegative,mySettings);

            //if (result == MessageDialogResult.Affirmative) { Settings.Default.Reset(); }
            Settings.Default.Reset();
            var accent = ThemeManager.GetAccent(Settings.Default.Config.config.AccentName);
            var theme = ThemeManager.GetAppTheme(Settings.Default.Config.config.Theme);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, accent, theme);
            Settings.Default.Save();
        }
      
        private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccentSelector.SelectedItem is Accent selectedAccent)
            {
                var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current, selectedAccent, theme.Item1);
                if (System.Windows.Application.Current.MainWindow != null)
                    System.Windows.Application.Current.MainWindow.Activate();
                Settings.Default.Config.config.AccentName = selectedAccent.Name;
                Settings.Default.Config.config.AccentColorType = "accent".ToLower();         
                Settings.Default.Save();
            }
        }
        private void ChangeAppThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + ((((System.Windows.Controls.ComboBox)sender).SelectedValue) as ListBoxItem).Content));
            if (System.Windows.Application.Current.MainWindow != null)
                System.Windows.Application.Current.MainWindow.Activate();
            var newtheme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            Settings.Default.Config.config.Theme = newtheme.Item1.Name;
            Settings.Default.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BrowseZpath();
        }


        private void Zclient_checked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.Default.Config.config.ZclientPath)) return;
            BrowseZpath();
            if (Settings.Default.Config.config.ZclientPath != "") return;
            Settings.Default.Config.config.AutostartZclient = false;
            runzclient.IsChecked = false;
        }

        private void BrowseImage_click(object sender, RoutedEventArgs e)
        {
            BrowseImagePath();
            try
            {
                if (Settings.Default.Config.config.UseExternalImage)
                {
                    ApplyImage();
                }
            }
            catch (Exception)
            {
                //todo nothing
            }
          
        }

        public void BrowseZpath()
        {
            var openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                Filter = @"ZClient|ZClient.exe",
                Title = @"Укажите расположение ZClient"
            };
            openFileDialog.ShowDialog();
            Settings.Default.Config.config.ZclientPath = openFileDialog.FileName;
            zbox.Text = Settings.Default.Config.config.ZclientPath;
            if (Settings.Default.Config.config.ZclientPath == "")
            {
                Settings.Default.Config.config.AutostartZclient = false;
            }
        }

        public void BrowseImagePath()
        {
            var openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                Filter = @"Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = @"Укажите расположение Изображения"
            };
            openFileDialog.ShowDialog();
            Settings.Default.Config.config.ImagePath = openFileDialog.FileName;
            ImagePathBox.Text = Settings.Default.Config.config.ImagePath;

        }

        public void ApplyImage()
        {
            var background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Settings.Default.Config.config.ImagePath))
            };
            if (System.Windows.Application.Current.MainWindow != null)
                System.Windows.Application.Current.MainWindow.Background = background;
        }

        private void externalImage_Checked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.Config.config.ImagePath))
            {
                BrowseImagePath();
            }
            try
            {
                ApplyImage();
            }
            catch (Exception)
            {
                Settings.Default.Config.config.UseExternalImage = false;
                externalImage.IsChecked = false;
            }
        }

        private void externalImage_Unchecked(object sender, RoutedEventArgs e)
        {
            var background = new ImageBrush();
            var resource = System.Windows.Application.Current.TryFindResource("wallper");
            background.ImageSource = new BitmapImage(new Uri (resource.ToString()));
            if (System.Windows.Application.Current.MainWindow != null)
                System.Windows.Application.Current.MainWindow.Background = background;
        }

        private void btn_color_picker_Click(object sender, RoutedEventArgs e)
        {
            var clr = new Color();
            var cdl = new ColorDialog();

            if (cdl.ShowDialog() != DialogResult.OK) return;
            clr.R = cdl.Color.R;
            clr.B = cdl.Color.B;
            clr.G = cdl.Color.G;
            clr.A = cdl.Color.A;

            ThemeManagerHelper.CreateAppStyleBy(clr);
            var resDictName = $"ДОПЦВЕТ_{clr.ToString().Replace("#", string.Empty)}.xaml";
            var combine = Path.Combine(Path.GetTempPath(), resDictName);
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            var accent = ThemeManager.GetAccent(resDictName);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, accent, theme.Item1);
            Settings.Default.Config.config.Clr = clr;
            Settings.Default.Config.config.AccentColorType = "color".ToLower();
        }

        private void Gswitch_Click(object sender, RoutedEventArgs e)
        {
            switch (GameisEa)
            {
                case true:
                    csw.SwitchToZlo(gPath);
                    if (!csw.IsEALicense(gPath))
                    {
                        GameisEa = false;
                        Gswitch.Content = "Текущая версия игры: Zlo";
                    }
                    break;
                case false:
                    csw.SwitchToEa(gPath);
                    if (csw.IsEALicense(gPath))
                    {
                        GameisEa = true;
                        Gswitch.Content = "Текущая версия игры: Лиц";
                        MessageBox.Show("Не забудьте закрыть этот Лаунчер, ZLOrigin и ZClient!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;
            }            
        }
    }

    public class ClientSwitcher
    {
        private readonly string[] _gFiles =
        {
            @"Core\winhttp.dll",
            @"ZUpdaterx32.dll",
            @"ZUpdaterx64.dll",
            @"Engine.BuildInfo_Win32_retail.dll",
            @"Engine.BuildInfo_Win64_retail.dll",
            @"Engine_BuildInfo_Win32_retail.dll",
            @"Engine_BuildInfo_Win64_retail.dll",
            @"dinput8.dll"
        };


        public bool IsEALicense(string gamepath)
        {
            string[] zloFiles = { "ZUpdaterx32.dll", "ZUpdaterx64.dll", "dinput8.dll" };
            var fileList = new DirectoryInfo(gamepath + @"\").GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            return !fileList.Any(file => zloFiles.Contains(file.Name));
        }

        public string FindGameInstallation()
        {
            const string registry_key = @"SOFTWARE\WOW6432Node\EA Games";
            string Il = null;
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        try
                        {
                            if (subkey != null && subkey.GetValue("DisplayName").ToString().Contains("Battlefield 4"))
                                Il = subkey.GetValue("Install Dir").ToString();
                        }
                        catch { }
                    }
                }
            }

            return Il;
        }

        private void DeleteFiles(string gamepath)
        {
            DeleteLicenseFiles();
            foreach (var file in _gFiles)
            {
                try
                {
                    File.Delete(gamepath + file);
                }
                catch (Exception ex) { }
            }
        }

        public void SwitchToEa(string gamepath)
        {
            DeleteFiles(gamepath);
            try
            {
                File.WriteAllBytes(gamepath + @"Engine.BuildInfo_Win32_retail.dll", Properties.Resources._Engine_BuildInfo_Win32_retail);
                File.WriteAllBytes(gamepath + @"Engine.BuildInfo_Win64_retail.dll", Properties.Resources._Engine_BuildInfo_Win64_retail);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public void SwitchToZlo(string gamepath)
        {
            DeleteFiles(gamepath);
            try
            {
                File.WriteAllBytes(gamepath + @"Engine.BuildInfo_Win32_retail.dll", Properties.Resources.Engine_BuildInfo_Win32_retail);
                File.WriteAllBytes(gamepath + @"Engine.BuildInfo_Win64_retail.dll", Properties.Resources.Engine_BuildInfo_Win64_retail);
                File.WriteAllBytes(gamepath + @"ZUpdaterx32.dll", Properties.Resources.ZUpdaterx32);
                File.WriteAllBytes(gamepath + @"ZUpdaterx64.dll", Properties.Resources.ZUpdaterx64);
                File.WriteAllBytes(gamepath + @"Core\winhttp.dll", Properties.Resources.winhttp);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void DeleteLicenseFiles()
        {
            try
            {
                var fileList = new DirectoryInfo(@"C:\ProgramData\Electronic Arts\EA Services\License\").GetFiles("*.dlf", SearchOption.TopDirectoryOnly);
                foreach (var file in fileList)
                {
                    var a = File.ReadAllBytes(file.FullName);
                    var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                    fs.Read(a, 30, 130);
                    var aa = "45-14-00-A4-FB-A8-4D-C4-B7-88-A3-D8-39-83-73-62-F1-26-0B-18-D9-A8-AD-9F-B1-5A-C8-F1-74-80-BB-B8-EB-E7-76-4A-07-76-5A-D8-79-F6-3D-11-62-7C-26-59-5D-45-EB-53-4E-74-52-96-A1-31-34-E6-C9-9B-DF-24-AD-41-43-8E-56-52-FC-F3-FF-48-8B-F3-52-6F-81-79-F4-F6-59-F9-28-71-36-88-63-CD-28-C0-9E-C3-4D-49-2E-3D-3A-85-AA-48-67-8B-8C-FE-8F-AB-DE-F0-61-D9-4D-F1-D4-DA-3F-57-16-6C-A1-B5-81-A6-5E-B1-3F-67-3F-80-6C-5B-F1-C5-65-8E-8A-49-E1-8E-61-36-E4-A6-49-8A-AB-C2-A7-86-A0-30-93-C3-D1-AC-B4-B8-77-49-44-A1-BF-99-E3-2F-9B-A0-3B-C8-69-0C-BF-7F-A4-90";
                    if (!BitConverter.ToString(ReadFully(fs)).Contains(aa)) continue;
                    fs.Close();
                    File.Delete(file.FullName);
                }
            }
            catch (Exception ex) { }

        }

        private byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}

