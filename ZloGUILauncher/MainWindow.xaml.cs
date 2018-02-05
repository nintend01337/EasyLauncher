using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using Zlo.Extras;
using System.Configuration;
using static ZloGUILauncher.Debuging;

namespace ZloGUILauncher
{
    public partial class MainWindow : MetroWindow
    {
        public const string download_music_link = "https://dl.dropbox.com/s/r2cz0vk26aji58n/music.mp3?dl=0";
        public const string AssemblyName = "Easy Launcher";
        public const string autor = "nintend01337";
        public string version = "1.5.0 beta";
        public string ApiVersion;
        public string soldiername;
        public string soldierID;
        public bool isDebug = Settings.Default.Config.config.isDebug;                    //enable-disable debugg messages
        public bool isMusicEnabled = Settings.Default.Config.config.isMusicEnabled;
        MediaPlayer player = new MediaPlayer();
        
        public MainWindow()
        {
            InitializeComponent();
                                                              //
            Settings.Default.SettingsLoaded += Settings_loaded;     //событие на загрузку настроек
            App.Current.MainWindow = this;
            App.Client.ErrorOccured += Client_ErrorOccured;
            App.Client.UserInfoReceived += Client_UserInfoReceived;
            App.Client.GameStateReceived += Client_GameStateReceived;
            App.Client.APIVersionReceived += Client_APIVersionReceived;
            App.Client.Disconnected += Client_Disconnected;
            //App.Client.ConnectionStateChanged += Client_ConnectionStateChanged;

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args.Contains("debug"))
            {
                isDebug = true;
                PrintDebug(DebugLevel.Warn, "Отладочные сообщения включены!");
            }

            if (App.Client.Connect())
            {
                PrintDebug(DebugLevel.Info, $"Подключились к ZLO ;)");
                switch (App.Client.SavedActiveServerListener)
                {                   
                    case ZloGame.BF_3:
                        MainTabControl.SelectedIndex = 0;
                       //App.Client.SubToServerList(ZloGame.BF_3);
                        App.Client.GetStats(ZloGame.BF_3);
                        break;

                    case ZloGame.BF_4:
                        MainTabControl.SelectedIndex = 1;
                      //  App.Client.SubToServerList(ZloGame.BF_4);
                        App.Client.GetStats(ZloGame.BF_4);
                        App.Client.GetItems(ZloGame.BF_4);
                        break;

                    case ZloGame.BF_HardLine:
                        MainTabControl.SelectedIndex = 2;
                        //App.Client.SubToServerList(ZloGame.BF_HardLine);
                        App.Client.GetStats(ZloGame.BF_HardLine);
                        App.Client.GetItems(ZloGame.BF_HardLine);
                        break;
                }
            }                
        }

         public void PrintDebug(DebugLevel lvl, string message)
        {
            Dispatcher.Invoke(() =>
            {
                var t = DateTime.Now;
                Run DateText = new Run($"{t.ToShortTimeString()} : ");
                DateText.Foreground = new SolidColorBrush(Colors.White);

                Run Level = new Run($"[{lvl}] ");
                Run MessageText = new Run($"{message}");

                switch (lvl)
                {
                    case DebugLevel.Info:
                        Level.Foreground = new SolidColorBrush(Colors.LimeGreen);
                        MessageText.Foreground = new SolidColorBrush(Colors.LimeGreen);
                        break;

                    case DebugLevel.Warn:
                        Level.Foreground = new SolidColorBrush(Colors.Orange);
                        MessageText.Foreground = new SolidColorBrush(Colors.Orange);
                        break;

                    case DebugLevel.Error:
                        Level.Foreground = new SolidColorBrush(Colors.Red);
                        MessageText.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                }

                Paragraph NewParagraph = new Paragraph();
                NewParagraph.Inlines.Add(DateText);
                NewParagraph.Inlines.Add(Level);
                NewParagraph.Inlines.Add(MessageText);

                if (isDebug)
                {
                    LogBox.Document.Blocks.Add(NewParagraph);
                }
            });

        }

        private void Settings_loaded(object sender, SettingsLoadedEventArgs e)
        {
          //  Preferences.Load();
          //  Preferences pf = new Preferences();
            // var config = Preferences.Load();
            // config.Accent = ThemeManager.GetAccent(Settings.Default.AccentName);
            //config.AppTheme = ThemeManager.GetAppTheme(Settings.Default.AppThemeName);

          // pf.AppTheme = ThemeManager.GetAppTheme(pf.AppTheme.Name);
           // pf.Accent = ThemeManager.GetAccent(pf.Accent.Name);
            //ThemeManager.ChangeAppStyle(Application.Current,pf.Accent,pf.AppTheme);
            PrintDebug(DebugLevel.Info, $"Загружены настройки: \n Тема: {Settings.Default.Config.config.AccentColor} Цвет: {Settings.Default.Config.config.ImagePath}");
        }

        /*private void Client_ConnectionStateChanged(bool IsConnectedToZloClient)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsConnectedToZloClient)
                {                    
                    //connected
                    /*IsConnectedTextBlock.Text = "Подключен";
                    IsConnectedTextBlock.Foreground = Brushes.LimeGreen;
                }
                else
                {
                    /*IsConnectedTextBlock.Text = "Отключен";
                    IsConnectedTextBlock.Foreground = Brushes.Red;
                }
            });
        }*/

        private void Client_UserInfoReceived(uint UserID, string UserName)
        {
            PrintDebug(DebugLevel.Info,  $"Получение информации о пользователе : {UserName} ,  {UserID}");
            soldiername = UserName;
            soldierID = UserID.ToString();
        }

        private void Client_Disconnected(Zlo.Extras.DisconnectionReasons Reason)
        {
            Dispatcher.Invoke(async () =>
            {
                await this.ShowMessageAsync("",$"Вылет по причине : {Reason}",MessageDialogStyle.Affirmative);
                PrintDebug(DebugLevel.Error, $"Вылет по причине : {Reason}");
            });
        }

        private void Client_APIVersionReceived(Version Current, Version Latest, bool IsNeedUpdate, string DownloadAdress)
        {            
            if (IsNeedUpdate) {
                Dispatcher.Invoke(async() =>
                {
                    if ( await this.ShowMessageAsync("Обновление", $"Текущая dll версия : {Current}\n Последняя dll версия : {Latest}\n Обновить сейчас?", MessageDialogStyle.AffirmativeAndNegative)==MessageDialogResult.Affirmative){ 
                        string Sourcedll = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zlo.dll");
                        string Newdll = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zlo_New.dll");
                        using (WebClient wc = new WebClient()){
                            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                            wc.DownloadFileAsync(new Uri(DownloadAdress), Newdll);
                        }
                   }
                });
            }
            else {
                Dispatcher.Invoke(() => {
                    ApiVersion = Current.ToString();
                    Title = AssemblyName + " | " + version + " | " + "API version " + ApiVersion + " | " + (soldiername != null ? "WELCOME, " + soldiername : "NOT CONNECTED") /*+"  ID : " + soldierID */;     //soldier ID нужен ли ?
                });
            }
            PrintDebug(DebugLevel.Info, $"Получение информации о версиях API: \n Текущая : {Current}, Последняя : {Latest}, \n Требуется обновление ? : {IsNeedUpdate}");
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            PrintDebug(DebugLevel.Info, "Загрузка Завершена!!!");
            //Zlo.dll completed
            if (e.Error != null)
            {
                //error occured
                Client_ErrorOccured(e.Error, "Возникла ошибка при обновлении Zlo.dll");
                Debug.Write(e.Error);
                PrintDebug(DebugLevel.Error, "Возникла ошибка при обновлении Zlo.dll" + e.Error.ToString());
            }
            else
            {
                //no errors
                string executablePath = Directory.GetCurrentDirectory();
                string Sourcedll = "Zlo.dll";
                string Newdll = "Zlo_New.dll";
                string Olddll = "Zlo_old.dll";
                string BatchText =
                   $@"
@ECHO off
SETLOCAL EnableExtensions
set EXE={AppDomain.CurrentDomain.FriendlyName}
echo Waiting for process %EXE% to close ...
:LOOP
@Timeout /T 1 /NOBREAK>nul
tasklist /FI ""IMAGENAME eq %EXE%"" 2>NUL | find /I /N ""%EXE%"">NUL
if ""%ERRORLEVEL%""==""0"" goto LOOP
echo Process %EXE% closed
mkdir backup
rename  ""{Sourcedll}"" ""{Olddll}""
move /y ""{Olddll}"" backup
move /y ""{Newdll}"" ""{Sourcedll}"" 
start """" ""{AppDomain.CurrentDomain.FriendlyName}"" ""done""
Exit
";
                var bat_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateBat.bat");
                //create the bat file
                File.WriteAllText(bat_path, BatchText);
                ProcessStartInfo si = new ProcessStartInfo(bat_path);
                si.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                Process.Start(si);
                Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
            }
        }

            private async void Wc_DownloadMusicCompletedAsync(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                    PrintDebug(DebugLevel.Info, "Загрузка файла music.mp3 Завершена. Перезапуск");
                    await this.ShowMessageAsync("Загрузка", "Файлы загружены.. \n Лаунчер будет перезапущен", MessageDialogStyle.Affirmative);
                    Dispatcher.Invoke(() => {
                      App.Client.Close();
                       Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
            });
        }
        #region Setup Events

        private void Client_GameStateReceived(Zlo.Extras.ZloGame game, string type, string message)
        {
            Dispatcher.Invoke(() =>
            {
                LatestGameStateTextBlock.Text = $"[{game}] [{type}] {message}";

                var t = DateTime.Now;
                Run DateText = new Run($"{t.ToShortTimeString()} : ");
                DateText.Foreground = new SolidColorBrush(Colors.White);

                Run GameText = new Run($"[{game}] ");
                GameText.Foreground = new SolidColorBrush(Colors.LightGreen);

                Run TypeText = new Run($"[{type}] ");
                TypeText.Foreground = new SolidColorBrush(Color.FromRgb(77, 188, 233));

                Run MessageText = new Run($"{message}");
                MessageText.Foreground = new SolidColorBrush(Colors.White);

                Paragraph NewParagraph = new Paragraph();
                NewParagraph.Inlines.Add(DateText);
                NewParagraph.Inlines.Add(GameText);
                NewParagraph.Inlines.Add(TypeText);
                NewParagraph.Inlines.Add(MessageText);

                LogBox.Document.Blocks.Add(NewParagraph);                

                if (type == "StateChanged") OnGameStarted(); //Это может тоже по красоте
                if (type == "Alert") OnGameClosed();
                
            });
        }

        private void Client_ErrorOccured(Exception Error, string CustomMessage)
        {
            string message = Error.ToString() + " " + CustomMessage;
            PrintDebug(DebugLevel.Error, message);
        }
        
        private void RestartLauncherButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                App.Client.Close();
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            });
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Views.Options options = new Views.Options();
            options.Visibility = Visibility.Visible;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tc){
                if (tc.SelectedIndex < 0) return;
                switch (tc.SelectedIndex)
                {
                    case 0:
                        App.Client.SubToServerList(ZloGame.BF_3);
                        break;
                    case 1:
                        App.Client.SubToServerList(ZloGame.BF_4);
                        break;
                    case 2:
                        App.Client.SubToServerList(ZloGame.BF_HardLine);
                        break;
                    default:
                        break;
                }
            }
        }

        private void OfficialDiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/QrBvQtt");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            LogBox.Document.Blocks?.Clear();
        }

        private void OnGameStarted()
        {
            Banner.Visibility = Visibility.Visible;
            MainTabControl.Visibility = Visibility.Hidden;
            player.Stop();
        }

        private void OnGameClosed()
        {
            MainTabControl.Visibility = Visibility.Visible;
            Banner.Visibility = Visibility.Hidden;
            MainTabControl.SelectedIndex = 0;
            if(isMusicEnabled)
                player.Play();
        }

        private void CloseGameBtn_Click(object sender, RoutedEventArgs e)
        {
            //Zlo.Extras.ZloGame game = new ZloGame();
            OnGameClosed();
           var  processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (p.ProcessName.Contains("bf3") || p.ProcessName.Contains("bf4") || p.ProcessName.Contains("bfh"))
                {
                    p.Kill();
                    PrintDebug(DebugLevel.Info, $"Завершен процесс {p.ProcessName}");
                }
            }
            // Надо это по красоте сделать
        }
        #endregion

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (isMusicEnabled)
            {
                if (!File.Exists("music.mp3"))
                {
                    PrintDebug(DebugLevel.Warn, $"Отсутствует музыкальный файл music.mp3 \n Начинаю скачивать с {download_music_link}");
                    string musicfile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "music.mp3");
                    // Нет музыкального файла скачать
                    Dispatcher.Invoke(() =>
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += Wc_DownloadMusicCompletedAsync;
                            wc.DownloadFileAsync(new Uri(download_music_link), musicfile);
                        }
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                     player.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "music.mp3", UriKind.RelativeOrAbsolute));
                        string uri = AppDomain.CurrentDomain.BaseDirectory + "music.mp3";
                        PrintDebug(DebugLevel.Info, "Включаю музыку :D");
                        player.Play();
                    });
                }
               
            }
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            //player.Open(new Uri("http://air2.radiorecord.ru:805/trap_320", UriKind.RelativeOrAbsolute));
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
       
        }
    }            
}
