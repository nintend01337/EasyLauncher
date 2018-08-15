using System;
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
using System.Runtime.InteropServices;

namespace ZloGUILauncher
{
    public partial class MainWindow : MetroWindow
    {
        #region siganture
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMAZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion

        //public const string download_music_link = "https://dl.dropbox.com/s/r2cz0vk26aji58n/music.mp3?dl=0";
        public const string RemoteVersionLink = "https://dl.dropbox.com/s/aaw46c567xdtziy/version.txt?dl=0";
        public const string DownloadLauncherLink = "https://dl.dropbox.com/s/rydql8l6fi8o5ac/Easy%20Launcher.exe?dl=0";
        public const string ChangelogLink = "https://dl.dropbox.com/s/81gigeh9dk9ekyx/changelog.txt?dl=0";
        public const string AssemblyName = "Easy Launcher";
        public const string LauncherNew = "Easy_New.exe";
        public const string Log = "Easy.log";
        public const string Autor = "nintend01337";
        public string Version = "1.5.9.1";
        public string ApiVersion;
        public string Soldiername;
        public string SoldierId;
        public bool IsDebug = Settings.Default.Config.config.IsDebug;                    //enable-disable debugg messages
        //public bool isMusicEnabled = Settings.Default.Config.config.isMusicEnabled;
        //MediaPlayer player = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            PrintDebug(DebugLevel.Info, "Инициализация компонентов завершена!");
            LoadImage();
            CheckUpdates();
            /*CheckZclient();*/

            //App.Current.MainWindow = this;
            App.Client.ErrorOccured += Client_ErrorOccured;
            App.Client.UserInfoReceived += Client_UserInfoReceived;
            App.Client.GameStateReceived += Client_GameStateReceived;
            //App.Client.APIVersionReceived += Client_APIVersionReceived;
            App.Client.Disconnected += Client_Disconnected;
            App.Client.ConnectionStateChanged += Client_ConnectionStateChanged;

            if (App.Client.Connect())
            {
                PrintDebug(DebugLevel.Info, $"Подключились к ZLO ;)");
                PrintDebug(DebugLevel.Warn, Settings.Default.Config.config.AccentName);
                PrintDebug(DebugLevel.Warn, Settings.Default.Config.config.AccentColorType);

                switch (App.Client.SavedActiveServerListener)
                {
                    case ZloGame.BF_3:
                        MainTabControl.SelectedIndex = 0;
                        //App.Client.GetStats(ZloGame.BF_3);
                        break;

                    case ZloGame.BF_4:
                        MainTabControl.SelectedIndex = 1;
                        //App.Client.GetStats(ZloGame.BF_4);
                        //App.Client.GetItems(ZloGame.BF_4);
                        break;

                    case ZloGame.BF_HardLine:
                        MainTabControl.SelectedIndex = 2;
                        //App.Client.GetStats(ZloGame.BF_HardLine);
                        //App.Client.GetItems(ZloGame.BF_HardLine);
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

                    case DebugLevel.System:
                        Level.Foreground = new SolidColorBrush(Colors.Cyan);
                        MessageText.Foreground = new SolidColorBrush(Colors.Cyan);
                        break;
                }

                Paragraph NewParagraph = new Paragraph();
                NewParagraph.Inlines.Add(DateText);
                NewParagraph.Inlines.Add(Level);
                NewParagraph.Inlines.Add(MessageText);

                if (IsDebug)
                {
                    LogBox.Document.Blocks.Add(NewParagraph);
                }
            });

        }

        #region events
        public void LoadImage()
        {
            if (Settings.Default.Config.config.UseExternalImage)
            {
                PrintDebug(DebugLevel.Warn, "Загружаю Изображение");
                try
                {
                    ImageBrush background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri(Settings.Default.Config.config.ImagePath));
                    System.Windows.Application.Current.MainWindow.Background = background;
                    PrintDebug(DebugLevel.Warn, "Изображение Успешно Загружено!!!.");
                }
                catch (Exception)
                {
                    //Console.WriteLine(this.ToString());
                    PrintDebug(DebugLevel.Error, string.Format($"Возникла ошибка при загрузке изображения \n {this.ToString()}"));
                }
            }
        }

        private void Settings_loaded(object sender, SettingsLoadedEventArgs e)
        {
            PrintDebug(DebugLevel.Info, $"Загружены настройки: \n Тема: {Settings.Default.Config.config.AccentName} Цвет: {Settings.Default.Config.config.ImagePath}");
        }

        private void Client_ConnectionStateChanged(bool isConnectedToZloClient)
        {
            Dispatcher.Invoke(() =>
            {
                if (isConnectedToZloClient)
                {
                    PrintDebug(DebugLevel.Info, "Подключен");
                }
                else
                {
                    PrintDebug(DebugLevel.Error, "Отключен");

                }
            });
        }

        private void Client_UserInfoReceived(uint userId, string userName)
        {
            PrintDebug(DebugLevel.Info, $" Получение информации о пользователе : {userName} ,  {userId}");
            Soldiername = userName;
            SoldierId = userId.ToString();
        }

        private void Client_Disconnected(Zlo.Extras.DisconnectionReasons reason)
        {
            Dispatcher.Invoke(async () =>
            {
                await this.ShowMessageAsync("", $" Вылет по причине : {reason}", MessageDialogStyle.Affirmative);
                PrintDebug(DebugLevel.Error, $" Вылет по причине : {reason}");
            });
        }

        private void Client_APIVersionReceived(Version current, Version latest, bool isNeedUpdate, string downloadAdress)
        {
            PrintDebug(DebugLevel.Info, $"Получение информации о версиях API: \n Текущая : {current}, Последняя : {latest}, \n Требуется обновление API ? : {isNeedUpdate}");

            if (isNeedUpdate)
            {
                Dispatcher.Invoke(async () =>
                {
                    if (await this.ShowMessageAsync("Обновление", $"Текущая dll версия : {current}\n Последняя dll версия : {latest}\n Обновить сейчас?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                    {
                        string sourcedll = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zlo.dll");
                        string newdll = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zlo_New.dll");
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                            wc.DownloadFileAsync(new Uri(downloadAdress), newdll);
                        }
                    }
                    Title = AssemblyName + " | " + Version + " | " + "API version " + ApiVersion + " | " + (Soldiername != null ? "WELCOME, " + Soldiername : "NOT CONNECTED") /*+"  ID : " + soldierID */;     //soldier ID нужен ли ?
                });
                PrintDebug(DebugLevel.Warn, "Вы используете не последнюю версию API.Видимо Разработчик отключил автоматическое обновление API.");
                ApiVersion = current.ToString();

            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    ApiVersion = current.ToString();
                    Title = AssemblyName + " | " + Version + " | " + "API version " + ApiVersion + " | " + (Soldiername != null ? "WELCOME, " + Soldiername : "NOT CONNECTED") /*+"  ID : " + soldierID */;     //soldier ID нужен ли ?
                });
            }
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
                PrintDebug(DebugLevel.Error, string.Format("{0} \n {1} \n {2}", "Возникла ошибка при обновлении Zlo.dll", e.Error.ToString(), e.Error.StackTrace.ToString()));
            }
            else
            {
                //no errors
                ApplyUpdate("Zlo.dll", "Zlo_New.dll");
            }
        }

        private void ApplyUpdate(string src, string newSrc)
        {
            string executablePath = Directory.GetCurrentDirectory();
            string source = src;
            string newSource = newSrc;
            string old = "old_".ToUpper() + source;
            string batchText =
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
            var batPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateBat.bat");
            //create the bat file
            File.WriteAllText(batPath, batchText);
            ProcessStartInfo si =
                new ProcessStartInfo(batPath) {WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory};
            Process.Start(si);
            Dispatcher.Invoke(() => { System.Windows.Application.Current.Shutdown(); });
        }

        private async void Wc_DownloadMusicCompletedAsync(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            /*PrintDebug(DebugLevel.Info, "Загрузка файла music.mp3 Завершена. Перезапуск");
            await this.ShowMessageAsync("Загрузка", "Файлы загружены.. \n Лаунчер будет перезапущен", MessageDialogStyle.Affirmative);
            Dispatcher.Invoke(() =>
            {
                App.Client.Close();
                Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            });*/
        }

        #endregion

        #region Setup Events

        private void Client_GameStateReceived(ZloGame game, string type, string message)
        {
            Dispatcher.Invoke(() =>
            {
                LatestGameStateTextBlock.Text = $"[{game}] [{type}] {message}";

                var t = DateTime.Now;
                Run dateText = new Run($"{t.ToShortTimeString()} : ");
                dateText.Foreground = new SolidColorBrush(Colors.White);

                Run gameText = new Run($"[{game}] ");
                gameText.Foreground = new SolidColorBrush(Colors.LightGreen);

                Run typeText = new Run($"[{type}] ");
                typeText.Foreground = new SolidColorBrush(Color.FromRgb(77, 188, 233));

                Run messageText = new Run($"{message}");
                messageText.Foreground = new SolidColorBrush(Colors.White);

                Paragraph newParagraph = new Paragraph();
                newParagraph.Inlines.Add(dateText);
                newParagraph.Inlines.Add(gameText);
                newParagraph.Inlines.Add(typeText);
                newParagraph.Inlines.Add(messageText);

                LogBox.Document.Blocks.Add(newParagraph);

                if (type == "StateChanged") OnGameStarted(); //Это может тоже по красоте
                if (type == "Alert") OnGameClosed();
                if (message.Contains("State_GameLoading State_ClaimReservation") || message.Contains("State_GameLoading State_LaunchPlayground") || message.Contains("State_GameLoading State_ResumeCampaign")) MaximizeWindow(game);

            });
        }

        private void Client_ErrorOccured(Exception error, string customMessage)
        {
            string message = error.ToString() + " " + customMessage;
            PrintDebug(DebugLevel.Error, message);
        }

        private void RestartLauncherButton_Click(object sender, RoutedEventArgs e)
        {
            SaveLogInFile();
            Dispatcher.Invoke(() =>
            {
                App.Client.Close();
                Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            });
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Views.Options options = new Views.Options {Visibility = Visibility.Visible};
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TabControl tc)
            {
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
            Process.Start("https://discord.gg/FUk56Rc");
            PrintDebug(DebugLevel.System, "Пошел в дискорд к разработчику !");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            LogBox.Document.Blocks?.Clear();
        }

        private void OnGameStarted()
        {
            Banner.Visibility = Visibility.Visible;
            MainTabControl.Visibility = Visibility.Hidden;
            //player.Stop();
        }

        private void OnGameClosed()
        {
            MainTabControl.Visibility = Visibility.Visible;
            Banner.Visibility = Visibility.Hidden;
            MainTabControl.SelectedIndex = 0;
            //if (Settings.Default.Config.config.isMusicEnabled)
            //player.Play();
        }

        private void MaximizeWindow(ZloGame game)
        {
            IntPtr hWnd;
            string windowname = string.Empty;

            if (Settings.Default.Config.config.MaximizeGameWindow)
            {
                switch (game)
                {
                    case ZloGame.BF_3:
                        windowname = "Battlefield 3";
                        break;

                    case ZloGame.BF_4:
                        windowname = "Battlefield 4";
                        break;

                    case ZloGame.BF_HardLine:
                        windowname = "Battlefield Hardline";
                        break;
                }

                hWnd = FindWindow(null, windowname);
                ShowWindow(hWnd, SW_SHOWMAXIMIZED);
            }
        }

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
        #endregion

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*if (Settings.Default.Config.config.isMusicEnabled)
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
            }*/
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            /*if (Settings.Default.Config.config.isMusicEnabled)
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
            }*/
        }

        /*private void CheckZclient()
        {
            PrintDebug(DebugLevel.Info, "Проверяю запущен ли Zclient");          
            var proc =  Process.GetProcessesByName("ZClient");
            if (proc.Length == 0)
            {
                PrintDebug(DebugLevel.Error, "ZCLIENT НЕ ЗАПУЩЕН, влючите пункт в автозапуске либо запустите его вручную".ToUpper());
                if (Settings.Default.Config.config.autostartZclient)
                {
                    RunZClient();
                    PrintDebug(DebugLevel.Warn, "НЕ ЗАПУЩЕН ZCLIENT,ЗАПУСКАЮ!!!!");
                    Thread.Sleep(10000);
                }
            }
            App.Client.ReConnect();
        }*/

        /*private void RunZClient()
        {
            if (!File.Exists(Settings.Default.Config.config.ZclientPath))
            {
                PrintDebug(DebugLevel.Error, "ZLO CLIENT путь не указан");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.AddExtension = true;
                openFileDialog.Filter = "ZClient|ZClient.exe";
                openFileDialog.Title = "Укажите расположение ZClient";
                openFileDialog.ShowDialog();
                Settings.Default.Config.config.ZclientPath = openFileDialog.FileName;
                Settings.Default.Save();
            }
            Process.Start(Settings.Default.Config.config.ZclientPath);
        }*/

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
            SaveLogInFile();
            App.Client.Close();
            Process p = Process.GetCurrentProcess();
            p.Kill();
        }

        #region  Updates
        private void CheckUpdates()
        {
            if (Settings.Default.Config.config.CheckUpdates)
            {
                string currentVersion = Version.ToLower();
                string remoteVersion = "";
                var obj = new WebClient();
                PrintDebug(DebugLevel.Warn, "Проверяю обновления Лаунчера");
                remoteVersion = obj.DownloadString(new Uri(RemoteVersionLink)).ToLower();

                if (string.IsNullOrEmpty(remoteVersion))
                    PrintDebug(DebugLevel.Error, "Невозможно проверить ОБНОВЛЕНИЯ");

                if (!string.Equals(remoteVersion, currentVersion))
                {
                    PrintDebug(DebugLevel.System, "Доступно Обновление лаунчера !");
                    obj.DownloadFile(new Uri(ChangelogLink), "changelog.txt");

                    var changelog = File.ReadAllText("changelog.txt");
                    if (changelog == string.Empty)
                        changelog = "АВТОР НЕ УКАЗАЛ";

                    //byte[] bytes = Encoding.UTF8.GetBytes(Changelog);
                    //Changelog = Encoding.UTF8.GetString(bytes);

                    MessageBoxResult mbr = System.Windows.MessageBox.Show($"Текущая  версия Лаунчера : {Version} \n Последняя  версия Лаунчера : {remoteVersion}\n  \n Список Изменений : \n {changelog} \n \n \n Обновить сейчас?", "Обновление Лаунчера", MessageBoxButton.YesNo);

                    if (mbr == MessageBoxResult.Yes)
                    {
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
                }

                else
                {
                    PrintDebug(DebugLevel.System, "ЛАУНЧЕР НЕ НУЖДАЕТСЯ В ОБНОВЛЕНИИ");
                }
            }
        }


        private void DownloadUpdate()
        {
            Dispatcher.Invoke(() =>
            {
                using (WebClient wc = new WebClient())
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

        #region Logs
        private void SaveLogInFile()
        {
            if (Settings.Default.Config.config.SaveLogInFile)
            {
                string Content = "";
                LogBox.SelectAll();
                LogBox.Copy();
                Content += System.Windows.Clipboard.GetText().ToString();

                if (!File.Exists(Log))
                {
                    File.WriteAllText(Log, string.Format($"Дата : {DateTime.Now} \n ___________________________________________________ \n {Content} \n "));
                }
                else
                {
                    string temp = File.ReadAllText(Log);
                    File.WriteAllText(Log, string.Format($"{temp} \n \n Дата : {DateTime.Now} \n ___________________________________________________ \n {Content} \n "));
                }
                GC.Collect();
            }
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
    }
}
