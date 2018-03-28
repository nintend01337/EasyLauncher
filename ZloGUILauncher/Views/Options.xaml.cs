using MahApps.Metro;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private async void btn_reset_Click(object sender, RoutedEventArgs e)
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
            var accent = ThemeManager.GetAccent(Settings.Default.Config.config.AccentColor);
            var theme = ThemeManager.GetAppTheme(Settings.Default.Config.config.Theme);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, accent, theme);
            Settings.Default.Save();
        }
      
        private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAccent = AccentSelector.SelectedItem as Accent;
            if (selectedAccent != null)
            {
                var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
                ThemeManager.ChangeAppStyle(System.Windows.Application.Current, selectedAccent, theme.Item1);
                System.Windows.Application.Current.MainWindow.Activate();
                Settings.Default.Config.config.AccentColor = selectedAccent.Name;
                Settings.Default.Save();
            }
        }
        private void ChangeAppThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + ((((System.Windows.Controls.ComboBox)sender).SelectedValue) as ListBoxItem).Content));
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
            if (string.IsNullOrEmpty(Settings.Default.Config.config.ZclientPath))
            {
                BrowseZpath();
                if (Settings.Default.Config.config.ZclientPath == "")
                {
                    Settings.Default.Config.config.autostartZclient = false;
                    runzclient.IsChecked = false;
                }
            }
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.AddExtension = true;
            openFileDialog.Filter = "ZClient|ZClient.exe";
            openFileDialog.Title = "Укажите расположение ZClient";
            openFileDialog.ShowDialog();
            Settings.Default.Config.config.ZclientPath = openFileDialog.FileName;
            zbox.Text = Settings.Default.Config.config.ZclientPath;
            if (Settings.Default.Config.config.ZclientPath == "")
            {
                Settings.Default.Config.config.autostartZclient = false;
            }
        }

        public void BrowseImagePath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.AddExtension = true;
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Title = "Укажите расположение Изображения";
            openFileDialog.ShowDialog();
            Settings.Default.Config.config.ImagePath = openFileDialog.FileName;
            ImagePathBox.Text = Settings.Default.Config.config.ImagePath;

        }

        public void ApplyImage()
        {
                ImageBrush background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri(Settings.Default.Config.config.ImagePath));
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
            ImageBrush background = new ImageBrush();
            object resource = System.Windows.Application.Current.TryFindResource("background");
            background.ImageSource = new BitmapImage(new Uri (resource.ToString()));
            System.Windows.Application.Current.MainWindow.Background = background;
        }
    }
}

