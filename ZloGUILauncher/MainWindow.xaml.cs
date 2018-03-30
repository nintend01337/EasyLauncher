using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
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
using System.Windows.Forms;

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

        public const string download_music_link = "https://dl.dropbox.com/s/r2cz0vk26aji58n/music.mp3?dl=0";
        public const string remote_version_link = "https://dl.dropbox.com/s/aaw46c567xdtziy/version.txt?dl=0";
        public const string download_launcher_link = "https://dl.dropbox.com/s/rydql8l6fi8o5ac/Easy%20Launcher.exe?dl=0";
        public const string changelog_link = "https://dl.dropbox.com/s/81gigeh9dk9ekyx/changelog.txt?dl=0";
        public const string AssemblyName = "Easy Launcher";
        public const string LauncherNew = "Easy_New.exe";
        public const string Log = "Easy.log";
        public const string autor = "nintend01337";
        public string version = "1.5.3";
        public string ApiVersion;
        public string soldiername;
        public string soldierID;
        public bool isDebug = Settings.Default.Config.config.isDebug;                    //enable-disable debugg messages
        public bool isMusicEnabled = Settings.Default.Config.config.isMusicEnabled;
        MediaPlayer player = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            PrintDebug(DebugLevel.Info, "Инициализация компонентов завершена!");
            LoadImage();
            CheckUpdates();
            CheckZclient();
            
            App.Current.MainWindow = this;
            App.Client.ErrorOccured += Client_ErrorOccured;
            App.Client.UserInfoReceived += Client_UserInfoReceived;
            App.Client.GameStateReceived += Client_GameStateReceived;
            App.Client.APIVersionReceived += Client_APIVersionReceived;
            App.Client.Disconnected += Client_Disconnected;
            App.Client.ConnectionStateChanged += Client_ConnectionStateChanged;

            if (App.Client.Connect())
            {
                PrintDebug(DebugLevel.Info, $"Подключились к ZLO ;)");
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

                if (isDebug)
                {
                    LogBox.Document.Blocks.Add(NewParagraph);
                }
            });

        }

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
            PrintDebug(DebugLevel.Info, $"Загружены настройки: \n Тема: {Settings.Default.Config.config.AccentColor} Цвет: {Settings.Default.Config.config.ImagePath}");
        }

        private void Client_ConnectionStateChanged(bool IsConnectedToZloClient)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsConnectedToZloClient)
                {
                    PrintDebug(DebugLevel.Info, "Подключен");
                }
                else
                {
                  PrintDebug(DebugLevel.Error, "Отключен");
                  
                }
            });
        }

        private void Client_UserInfoReceived(uint UserID, string UserName)
        {
            PrintDebug(DebugLevel.Info,  $" Получение информации о пользователе : {UserName} ,  {UserID}");
            soldiername = UserName;
            soldierID = UserID.ToString();
        }

        private void Client_Disconnected(Zlo.Extras.DisconnectionReasons Reason)
        {
            Dispatcher.Invoke(async () =>
            {
                await this.ShowMessageAsync("",$" Вылет по причине : {Reason}",MessageDialogStyle.Affirmative);
                PrintDebug(DebugLevel.Error, $" Вылет по причине : {Reason}");
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
                    Title = AssemblyName + " | " + version  +  " | " + "API version " + ApiVersion + " | " + (soldiername != null ? "WELCOME, " + soldiername : "NOT CONNECTED") /*+"  ID : " + soldierID */;     //soldier ID нужен ли ?
                });
            }
            PrintDebug(DebugLevel.Info, $"Получение информации о версиях API: \n Текущая : {Current}, Последняя : {Latest}, \n Требуется обновление API ? : {IsNeedUpdate}");
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
                PrintDebug(DebugLevel.Error, string.Format("{0} \n {1} \n {2}","Возникла ошибка при обновлении Zlo.dll" , e.Error.ToString() , e.Error.StackTrace.ToString()));
            }
            else
            {
                //no errors
                ApplyUpdate("Zlo.dll","Zlo_New.dll");
            }
        }

        private void ApplyUpdate(string Src, string NewSrc)
        {
            string executablePath = Directory.GetCurrentDirectory();
            string Source = Src;
            string NewSource = NewSrc;
            string Old = "old_".ToUpper() + Source;
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
rename  ""{Source}"" ""{Old}""
move /y ""{Old}"" backup
move /y ""{NewSource}"" ""{Source}"" 
start """" ""{AppDomain.CurrentDomain.FriendlyName}"" ""done""
Exit
";
            var bat_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateBat.bat");
            //create the bat file
            File.WriteAllText(bat_path, BatchText);
            ProcessStartInfo si = new ProcessStartInfo(bat_path);
            si.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Process.Start(si);
            Dispatcher.Invoke(() => { System.Windows.Application.Current.Shutdown(); });
        }

        private async void Wc_DownloadMusicCompletedAsync(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                    PrintDebug(DebugLevel.Info, "Загрузка файла music.mp3 Завершена. Перезапуск");
                    await this.ShowMessageAsync("Загрузка", "Файлы загружены.. \n Лаунчер будет перезапущен", MessageDialogStyle.Affirmative);
                    Dispatcher.Invoke(() => {
                      App.Client.Close();
                       Process.Start(System.Windows.Application.ResourceAssembly.Location);
                        System.Windows.Application.Current.Shutdown();
            });
        }
        #region Setup Events

        private void Client_GameStateReceived(ZloGame game, string type, string message)
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
                if (message.Contains("State_GameLoading State_ClaimReservation") || message.Contains("State_GameLoading State_LaunchPlayground") || message.Contains("State_GameLoading State_ResumeCampaign")) MaximizeWindow(game);
                
            });
        }

        private void Client_ErrorOccured(Exception Error, string CustomMessage)
        {
            string message = Error.ToString() + " " + CustomMessage;
            PrintDebug(DebugLevel.Error, message);
        }
        
        private void RestartLauncherButton_Click(object sender, RoutedEventArgs e)
        {
            SaveLogInFile();
            Dispatcher.Invoke(() => {
                App.Client.Close();
                Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            });
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Views.Options options = new Views.Options();
            options.Visibility = Visibility.Visible;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TabControl tc){
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
            player.Stop();
        }

        private void OnGameClosed()
        {
            MainTabControl.Visibility = Visibility.Visible;
            Banner.Visibility = Visibility.Hidden;
            MainTabControl.SelectedIndex = 0;
            if(Settings.Default.Config.config.isMusicEnabled)
                player.Play();
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
           var  processes = Process.GetProcesses();
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
            if (Settings.Default.Config.config.isMusicEnabled)
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

        private void CheckZclient()
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
                    Thread.Sleep(10000);                //спим 10 сек на случай обновления Зшки
                }
            }
            
        }

        private void RunZClient()
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
        }
        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            
            //player.Open(new Uri("http://air2.radiorecord.ru:805/trap_320", UriKind.RelativeOrAbsolute));
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
            SaveLogInFile();
        }

        #region  Updates
        private  void CheckUpdates()
        {
            if (Settings.Default.Config.config.CheckUpdates)
            {
                string CurrentVersion = version.ToLower();
                string RemoteVersion = "";
                string Changelog = "";

                var obj = new WebClient();

                PrintDebug(DebugLevel.Warn, "Проверяю обновления Лаунчера");
                
                RemoteVersion = obj.DownloadString(new Uri(remote_version_link)).ToLower();
            
                if (string.IsNullOrEmpty(RemoteVersion))
                {
                    PrintDebug(DebugLevel.Error, "Невозможно проверить ОБНОВЛЕНИЯ");
                }
                

                if (!string.Equals(RemoteVersion,CurrentVersion))
                {
                    PrintDebug(DebugLevel.System, "Доступно Обновление лаунчера !");
                    obj.DownloadFile(new Uri(changelog_link), "changelog.txt");

                    Changelog = File.ReadAllText("changelog.txt");
                    if (Changelog == null || Changelog == string.Empty)
                        Changelog = "АВТОР НЕ УКАЗАЛ";

                    // byte[] bytes = Encoding.UTF8.GetBytes(Changelog);
                    //Changelog = Encoding.UTF8.GetString(bytes);

                    MessageBoxResult mbr = System.Windows.MessageBox.Show($"Текущая  версия Лаунчера : {version} \n Последняя  версия Лаунчера : {RemoteVersion}\n  \n Список Изменений : \n {Changelog} \n \n \n Обновить сейчас?", "Обновление Лаунчера", MessageBoxButton.YesNo);
                    
                    if(mbr == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Dispatcher.InvokeAsync(() =>
                            {
                                DownloadUpdate();
                            });
                            PrintDebug(DebugLevel.System, "Пытаюсь Скачать обновление");
                        }
                        catch (Exception)
                        {

                            PrintDebug(DebugLevel.Error, string.Format("{0} \n {1}", "Что-то пошло не так.", this.ToString()));
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
                    wc.DownloadFileAsync(new Uri(download_launcher_link), LauncherNew);
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
