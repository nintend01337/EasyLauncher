﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using Zlo.Extras;
using System.Configuration;
using static ZloGUILauncher.Debuging;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using ZloGUILauncher.Views;
using Microsoft.Win32;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using ZloGUILauncher.Addons;
using System.Reflection;

namespace ZloGUILauncher
{
    public partial class MainWindow : MetroWindow
    {
        #region siganture
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMAZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32")]
        public static extern int EnumWindows(MainWindow.CallBack x, int y);
        #endregion

        public const string download_music_link = "https://dl.dropbox.com/s/r2cz0vk26aji58n/music.mp3?dl=0";
        public const string RemoteVersionLink = "https://dl.dropbox.com/s/aaw46c567xdtziy/version.txt?dl=0";
        public const string DownloadLauncherLink = "https://dl.dropbox.com/s/rydql8l6fi8o5ac/Easy%20Launcher.exe?dl=0";
        public const string ChangelogLink = "https://dl.dropbox.com/s/81gigeh9dk9ekyx/changelog.txt?dl=0";
        public const string AssemblyName = "Easy Launcher";
        public const string LauncherNew = "Easy_New.exe";
        public const string Log = "Easy.log";
        public const string Autor = "nintend01337";
        public const string GUID = "0xFDCD2686";
        public string Version = "1.6.4";
        public string ApiVersion { get; set; }
        public string Soldiername { get; set; }
        public string SoldierId { get; set; }
        public bool IsDebug = Settings.Default.Config.config.IsDebug;   //enable-disable debugg messages
        public bool IsMusicEnabled = Settings.Default.Config.config.IsMusicEnabled;
        public static MediaPlayer player = new MediaPlayer();
                 
        protected ManualResetEvent _resetEvent = new ManualResetEvent(false);   //ждун


        public MainWindow()
        {
            InitializeComponent();
            PrintDebug(DebugLevel.Info, "Инициализация компонентов завершена!");
            PrintDebug(DebugLevel.System,$"Application guid : {GUID}");
            LoadImage();
            CheckUpdates();
            CheckZclient();

            player.MediaEnded += playFinished;
            App.Client.ErrorOccured += Client_ErrorOccured;
            App.Client.UserInfoReceived += Client_UserInfoReceived;
            App.Client.GameStateReceived += Client_GameStateReceived;
            App.Client.ConnectionStateChanged += Client_ConnectionStateChanged;
            App.Client.Connect();
            StateChanged += MainWindow_resize;

            
            PrintDebug(DebugLevel.Info, $"API ВЕРСИЯ : {App.Client.CurrentApiVersion}");
            PrintDebug(DebugLevel.Info, "Подключились к ZLO ;)");
            PrintDebug(DebugLevel.Info,$"Подключен  к Zclient ? {App.Client.IsConnectedToZCLient}");
            PrintDebug(DebugLevel.Warn, Settings.Default.Config.config.AccentName);
            PrintDebug(DebugLevel.Warn, Settings.Default.Config.config.AccentColorType);
            Settings.Default.Config.config.DiscordRPCEnabled = App.Client.IsEnableDiscordRPC;


        #region deprecated         
        //switch (App.Client.SavedActiveServerListener)
        //{
        //    case ZloBFGame.BF_3:
        //        MainTabControl.SelectedIndex = 0;
        //        //App.Client.GetStats(ZloGame.BF_3);
        //        break;

        //    case ZloBFGame.BF_4:
        //        MainTabControl.SelectedIndex = 1;
        //        //App.Client.GetStats(ZloGame.BF_4);
        //        //App.Client.GetItems(ZloGame.BF_4);
        //        break;

        //    case ZloBFGame.BF_HardLine:
        //        MainTabControl.SelectedIndex = 2;
        //        //App.Client.GetStats(ZloGame.BF_HardLine);
        //        //App.Client.GetItems(ZloGame.BF_HardLine);
        //        break;
        //    case ZloBFGame.None:
        //        break;
        //    default:
        //        throw new ArgumentOutOfRangeException();
        //}
        #endregion
    }

        #region Launcher update

        private void ApplyUpdate(string src, string newSrc)
        {
            var source = src;
            var newSource = newSrc;
            var old = "old_".ToUpper() + source;
            var batchText =
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
rename  ""{source}"" ""{old}""
move /y ""{old}"" backup
move /y ""{newSource}"" ""{source}"" 
start """" ""{AppDomain.CurrentDomain.FriendlyName}"" ""done""
Exit
";
            var batPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateBat.bat");
            //create the bat file
            File.WriteAllText(batPath, batchText);
            var si = new ProcessStartInfo(batPath) { WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory };
            Process.Start(si);
            Dispatcher.Invoke(() => { Application.Current.Shutdown(); });
        }

        private void CheckUpdates()
        {
            Dispatcher.Invoke(() =>
            {
                if (!Settings.Default.Config.config.CheckUpdates) return;
                string currentVersion = Version + ":" + GUID.ToUpper();
                string remoteVersion = "";
                var obj = new WebClient();
                PrintDebug(DebugLevel.Warn, "Проверяю обновления Лаунчера");
                remoteVersion = obj.DownloadString(new Uri(RemoteVersionLink)).ToUpper();

                if (string.IsNullOrEmpty(remoteVersion))
                    PrintDebug(DebugLevel.Error, "Невозможно проверить ОБНОВЛЕНИЯ");

                if (!string.Equals(remoteVersion, currentVersion))
                {
                    PrintDebug(DebugLevel.System, "Доступно Обновление лаунчера !");
                    obj.DownloadFile(new Uri(ChangelogLink), "changelog.txt");

                    var changelog = File.ReadAllText("changelog.txt");
                    if (changelog == string.Empty)
                        changelog = "АВТОР НЕ УКАЗАЛ";

                    var mbr = MessageBox.Show(
                        $"Текущая  версия Лаунчера : {currentVersion} \n Последняя  версия Лаунчера : {remoteVersion}\n  \n Список Изменений : \n {changelog} \n \n \n Обновить сейчас?",
                        "Обновление Лаунчера", MessageBoxButton.YesNo);

                    if (mbr != MessageBoxResult.Yes) return;
                    try
                    {
                        Dispatcher.InvokeAsync(DownloadUpdate);
                        PrintDebug(DebugLevel.System, "Пытаюсь Скачать обновление");
                    }
                    catch (Exception)
                    {
                        PrintDebug(DebugLevel.Error, $"Что-то пошло не так. \n {this.ToString()}");
                    }
                }
                else
                {
                    PrintDebug(DebugLevel.System, "ЛАУНЧЕР НЕ НУЖДАЕТСЯ В ОБНОВЛЕНИИ");
                }
            });
        }

        private void DownloadUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFileAsync(new Uri(DownloadLauncherLink), LauncherNew);
                    wc.DownloadFileCompleted += Wc_Downloaded;
                }
            });
        }

        private void Wc_Downloaded(object sender, AsyncCompletedEventArgs e)
        {
            PrintDebug(DebugLevel.System, "Обновление Успешно Скачано. \n Применение...");
            ApplyUpdate(AppDomain.CurrentDomain.FriendlyName, LauncherNew);
        }

        #endregion

        #region API Events

        private void Client_ConnectionStateChanged(bool isConnectedToZloClient)
        {
            if (isConnectedToZloClient)
            {
                PrintDebug(DebugLevel.Info, "Подключен");
                ChangeWindowHeader(string.Join(string.Empty, AssemblyName, " | ", Version, " | ", "API version ", ApiVersion, " | ", Soldiername, "   CONNECTED"));
            }
            else
            {
                PrintDebug(DebugLevel.Error, "Отключен");
                ChangeWindowHeader(string.Join(string.Empty, AssemblyName, " | ", Version, " | ", "API version ", ApiVersion, " | ", "   DISCONNECTED"));
            }
        }
        private void Client_UserInfoReceived(uint userId, string userName)
        {
            PrintDebug(DebugLevel.Info, $" Получение информации о пользователе : {userName} ,  {userId}");
            Soldiername = userName;
            SoldierId = userId.ToString();
            ApiVersion = App.Client.CurrentApiVersion.ToString();
            _resetEvent.Set();
            ChangeWindowHeader(string.Join(string.Empty, AssemblyName , " | " ,   Version ,   " | " , "API version " , ApiVersion , " | " , Soldiername , "   CONNECTED"));
        }

        private void Client_Disconnected(DisconnectionReasons reason)
        {
            this.ShowMessageAsync("", $" Вылет по причине : {reason}");
            PrintDebug(DebugLevel.Error, $" Вылет по причине : {reason}");
        }
        #region Deprecated

        //private void Client_APIVersionReceived(Version Current, Version Latest, bool IsNeedUpdate, string DownloadAdress)
        //{
        //    PrintDebug(DebugLevel.Info, $"Получение информации о версиях API: \n Текущая : {Current}, Последняя : {Latest}, \n Требуется обновление API ? : {IsNeedUpdate}");
        //    _resetEvent.WaitOne();

        //    if (IsNeedUpdate && Settings.Default.Config.config.AutoUpdateAPi)
        //    {
        //        Dispatcher.Invoke(async () =>
        //        {
        //            if (await this.ShowMessageAsync("Обновление", $"Текущая dll версия : {Current}\n Последняя dll версия : {Latest}\n Обновить сейчас?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
        //            {
        //                string Sourcedll = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zlo.dll");
        //                string Newdll = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zlo_New.dll");
        //                using (WebClient wc = new WebClient())
        //                {
        //                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
        //                    wc.DownloadFileAsync(new Uri(DownloadAdress), Newdll);
        //                }
        //            }
        //        });
        //        PrintDebug(DebugLevel.Warn, "Вы используете не последнюю версию API.Видимо Разработчик отключил автоматическое обновление API.");
        //        ApiVersion = Current.ToString();
        //        Title = AssemblyName + " | " + Version + " | " + "API version " + ApiVersion + " | " + ( Soldiername!= null ? "WELCOME, " + Soldiername : "NOT CONNECTED") /*+"  ID : " + soldierID */;     //soldier ID нужен ли ?
        //    }
        //    else
        //    {
        //        Dispatcher.Invoke(() => {
        //            ApiVersion = Current.ToString();
        //            Title = AssemblyName + " | " + Version + " | " + "API version " + ApiVersion + " | " + (Soldiername != null ? "WELCOME, " + Soldiername : "NOT CONNECTED") /*+"  ID : " + soldierID */;     //soldier ID нужен ли ?
        //        });
        //    }
        //}

        //private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    PrintDebug(DebugLevel.Info, "Загрузка Завершена!!!");
        //    //Zlo.dll completed
        //    if (e.Error == null)
        //    {
        //        //no errors
        //        ApplyUpdate("Zlo.dll", "Zlo_New.dll");
        //    }
        //    else
        //    {
        //        //error occured
        //        Client_ErrorOccured(e.Error, "Возникла ошибка при обновлении Zlo.dll");
        //        Debug.Write(e.Error);
        //        PrintDebug(DebugLevel.Error,
        //            $"Возникла ошибка при обновлении Zlo.dll \n {e.Error} \n {e.Error.StackTrace}");
        //    }
        //}
        #endregion
        private void Client_GameStateReceived(ZloBFGame game, string type, string message)
        {
            Dispatcher.Invoke(() =>
            {
                LatestGameStateTextBlock.Text = $"[{game}] [{type}] {message}";
                var t = DateTime.Now;
                var dateText = new Run($"{t.ToShortTimeString()} : ") { Foreground = new SolidColorBrush(Colors.White) };
                var gameText = new Run($"[{game}] ") { Foreground = new SolidColorBrush(Colors.LightGreen) };
                var typeText = new Run($"[{type}] ") { Foreground = new SolidColorBrush(Color.FromRgb(77, 188, 233)) };
                var messageText = new Run($"{message}") { Foreground = new SolidColorBrush(Colors.White) };
                var newParagraph = new Paragraph();
                newParagraph.Inlines.Add(dateText);
                newParagraph.Inlines.Add(gameText);
                newParagraph.Inlines.Add(typeText);
                newParagraph.Inlines.Add(messageText);
                LogBox.Document.Blocks.Add(newParagraph);
                switch (type)
                {
                    case "StateChanged":
                        OnGameStarted(); //Это может тоже по красоте
                        break;
                    case "Alert":
                        OnGameClosed();
                        break;
                    case "State_GameLoading":
                        MaximizeWindow(game);
                        break;
                    case "State_Game State_NA":
                        OnGameClosed();
                        break;

                }
                if (message.Contains("State_Connecting")|| message.Contains("State_GameLoading State_LaunchPlayground") || message.Contains("State_GameLoading State_ResumeCampaign")) MaximizeWindow(game);
            });
        }

        private void Client_ErrorOccured(Exception error, string customMessage)
        {
            var message = error.ToString() + " " + customMessage;
            PrintDebug(DebugLevel.Error, message);
        }

        #endregion

        public void LoadImage()
        {
            if (!Settings.Default.Config.config.UseExternalImage) return;
            PrintDebug(DebugLevel.Warn, "Загружаю Изображение");
            try
            {
                var background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(Settings.Default.Config.config.ImagePath))
                };
                if (Application.Current.MainWindow != null)
                    Application.Current.MainWindow.Background = background;
                PrintDebug(DebugLevel.Warn, "Изображение Успешно Загружено!!!.");
            }
            catch (Exception)
            {
                PrintDebug(DebugLevel.Error, string.Format($"Возникла ошибка при загрузке изображения \n {ToString()}"));
            }
        }

        private void MaximizeWindow(ZloBFGame game)
        {
            IntPtr hWnd;
            var windowname = string.Empty;

            if (!Settings.Default.Config.config.MaximizeGameWindow) return;
            switch (game)
            {
                case ZloBFGame.BF_3:
                    windowname = "Battlefield 3™";
                    break;


                case ZloBFGame.BF_4:
                    windowname = "Battlefield 4™";
                    break;

                case ZloBFGame.BF_HardLine:
                    windowname = "Battlefield Hardline";
                    break;
                case ZloBFGame.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(game), game, null);
            }

            hWnd = FindWindow(null, windowname);
            ShowWindow(hWnd, SW_SHOWMAXIMIZED);
        }

        private bool hideBanner = false;
        private void OnGameStarted()
        {
            Banner.Visibility = Visibility.Visible;
            MainTabControl.Visibility = Visibility.Hidden;
            player.Pause();
        }

        private void OnGameClosed()
        {
            MainTabControl.Visibility = Visibility.Visible;
            Banner.Visibility = Visibility.Hidden;
            MainTabControl.SelectedIndex = 0;
            if (IsMusicEnabled)
                player.Play();
        }

        /*
                евенты окон обработчики событий 
        */
        #region MetroWindow events and listeners    
        private void RestartLauncherButton_Click(object sender, RoutedEventArgs e)
        {
            SaveLogInFile();
            Dispatcher.Invoke(() =>
            {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            });
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new Views.Options { Visibility = Visibility.Visible };
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TabControl tc)
            {
                if (tc.SelectedIndex < 0) return;
                switch (tc.SelectedIndex)
                {
                    case 0:
                        App.Client.SubToServerList(ZloBFGame.BF_3);
                        break;
                    case 1:
                        App.Client.SubToServerList(ZloBFGame.BF_4);
                        break;
                    case 2:
                        App.Client.SubToServerList(ZloBFGame.BF_HardLine);
                        break;
                    default:
                        break;
                }
            }
        }

        private void OfficialDiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/m5n5Dvd");
            PrintDebug(DebugLevel.System, "Пошел в дискорд к разработчику !");
        }

        private void OnHelpButton_Click(object sender, RoutedEventArgs e) => MainTabControl.SelectedIndex = 3;

        private void ClearButton_Click(object sender, RoutedEventArgs e) => LogBox.Document.Blocks?.Clear();


        private void CloseGameBtn_Click(object sender, RoutedEventArgs e)
        {
            OnGameClosed();
            var processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (p.ProcessName.Contains("bf3") || p.ProcessName.Contains("bf4") || p.ProcessName.Contains("bfh"))
                {
                    p.Kill();
                    PrintDebug(DebugLevel.Warn, $"Завершен процесс {p.ProcessName}");
                }
            }
        }

        private void playFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                player.Stop();
                player.Play();
            });
            PrintDebug(DebugLevel.Warn, "Проигрывание завершено!");
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayMusic();
        }

        public void PlayMusic()
        {
            if (Settings.Default.Config.config.IsMusicEnabled)
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

        public static void ShutOffMusic()
        {
            player.Stop();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
            SaveLogInFile();
            App.Client.Dispose();
            var p = Process.GetCurrentProcess();
            p.Kill();
        }

        private void StatusBar_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainTabControl.SelectedIndex = 5;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hideBanner = true;
            MainTabControl.Visibility = Visibility.Visible;
            Banner.Visibility = Visibility.Hidden;
        }

        private void MainWindow_resize(object sender, EventArgs e)
        {
            if (!hideBanner) return;
            Banner.Visibility = Visibility.Visible;
            MainTabControl.Visibility = Visibility.Hidden;
            hideBanner = false;
        }

        #endregion                      //

        #region Logs
        public void PrintDebug(DebugLevel lvl, string message)
        {
            Dispatcher.Invoke(() =>
            {
                var t = DateTime.Now;
                var dateText = new Run($"{t.ToShortTimeString()} : ") { Foreground = new SolidColorBrush(Colors.White) };
                var level = new Run($"[{lvl}] ");
                var messageText = new Run($"{message}");
                switch (lvl)
                {
                    case DebugLevel.Info:
                        level.Foreground = new SolidColorBrush(Colors.LimeGreen);
                        messageText.Foreground = new SolidColorBrush(Colors.LimeGreen);
                        break;

                    case DebugLevel.Warn:
                        level.Foreground = new SolidColorBrush(Colors.Orange);
                        messageText.Foreground = new SolidColorBrush(Colors.Orange);
                        break;

                    case DebugLevel.Error:
                        level.Foreground = new SolidColorBrush(Colors.Red);
                        messageText.Foreground = new SolidColorBrush(Colors.Red);
                        break;

                    case DebugLevel.System:
                        level.Foreground = new SolidColorBrush(Colors.Cyan);
                        messageText.Foreground = new SolidColorBrush(Colors.Cyan);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lvl), lvl, null);
                }
                var newParagraph = new Paragraph();
                newParagraph.Inlines.Add(dateText);
                newParagraph.Inlines.Add(level);
                newParagraph.Inlines.Add(messageText);
                if (!IsDebug) return;
                LogBox.Document.Blocks.Add(newParagraph);
            });
        }

        private void SaveLogInFile()
        {
            if (!Settings.Default.Config.config.SaveLogInFile) return;
            string content = null;
            LogBox.SelectAll();
            LogBox.Copy();
            content += Clipboard.GetText();
            if (File.Exists(Log))
            {
                var temp = File.ReadAllText(Log);
                File.WriteAllText(Log,
                    string.Format(
                        $"{temp} \n \n Дата : {DateTime.Now} \n ___________________________________________________ \n {Content} \n "));
            }
            else
            {
                File.WriteAllText(Log,
                    string.Format(
                        $"Дата : {DateTime.Now} \n ___________________________________________________ \n {Content} \n "));
            }
            GC.Collect();
        }

        private void CreateLog_Click(object sender, RoutedEventArgs e)
        {
            string filename = "Log__" + DateTime.Now.ToShortDateString() + ".log";
            string Content = "";
            LogBox.SelectAll();
            LogBox.Copy();
            Content += System.Windows.Clipboard.GetText().ToString();
            File.WriteAllText(filename, string.Format($"Дата : {DateTime.Now} \n___________________________________________________ \n {Content} \n "));
        }
        #endregion

        #region Utils

        private void Settings_loaded(object sender, SettingsLoadedEventArgs e)
        {
            PrintDebug(DebugLevel.Info, $"Загружены настройки: \n Тема: {Settings.Default.Config.config.AccentName} Цвет: {Settings.Default.Config.config.ImagePath}");
        }

        private async void Wc_DownloadMusicCompletedAsync(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            PrintDebug(DebugLevel.Info, "Загрузка файла music.mp3 Завершена. Перезапуск");
            await this.ShowMessageAsync("Загрузка", "Файлы загружены.. \nЛаунчер будет перезапущен", MessageDialogStyle.Affirmative);
            Dispatcher.Invoke(() =>
            {
                App.Client.Dispose();
                Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            });
        }

        private void CheckZclient()
        {
            PrintDebug(DebugLevel.Info, "Проверяю запущен ли Zclient");
            var proc = Process.GetProcessesByName("ZClient");
            if (proc.Length > 0)
                App.Client.Connect();
           
            if (proc.Length == 0)
            {
                PrintDebug(DebugLevel.Error, "ZCLIENT НЕ ЗАПУЩЕН, влючите пункт в автозапуске либо запустите его вручную".ToUpper());
                if (Settings.Default.Config.config.AutostartZclient)
                {
                    RunZClient();
                    PrintDebug(DebugLevel.Warn, "НЕ ЗАПУЩЕН ZCLIENT,ЗАПУСКАЮ!!!!");
                    Thread.Sleep(8000);
                }
            }
        }

        private void RunZClient()
        {
            var result = tryToFindZclient();
            if (!result)
            {
                PrintDebug(DebugLevel.Error, "ZLO CLIENT автоматически не найден, укажите путь вручную!");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.AddExtension = true;
                openFileDialog.Filter = "ZClient|ZClient.exe";
                openFileDialog.Title = "Укажите расположение ZClient";
                openFileDialog.ShowDialog();
                Settings.Default.Config.config.ZclientPath = openFileDialog.FileName;
                Settings.Default.Save();
            }
            Task t = Task.Run(() => { Process.Start(Settings.Default.Config.config.ZclientPath); });
        }

        private bool tryToFindZclient()
        {
           string procname = "ZClient.exe";
           string workingDirrectory = AppDomain.CurrentDomain.BaseDirectory;
           string path = Path.Combine(workingDirrectory, procname);
            if (File.Exists(path))
            {
                PrintDebug(DebugLevel.Info, "ZLO CLIENT найден автоматически и сейчас будет запущен!");
                Settings.Default.Config.config.ZclientPath = path;
                return true;
            }
            else
                return false;
        }

        public string BuildString(string[] arg)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string str in arg)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        private void ChangeWindowHeader(string param)
        {
            Dispatcher.Invoke(() => 
            {
                Title = param;
            });
        }
        #endregion

        public delegate bool CallBack(int hwnd, int lParam);
    }

   


}
