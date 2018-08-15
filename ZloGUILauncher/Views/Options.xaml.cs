using MahApps.Metro;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZloGUILauncher.Addons;

namespace ZloGUILauncher.Views
{
    public partial class Options 
    {
        public Options()
        {
            InitializeComponent();
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
    }
}

