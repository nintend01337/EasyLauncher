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

namespace ZloGUILauncher
{
    public partial class MainWindow : MetroWindow
    {        
        public const string AssemblyName = "Easy Launcher";
        public const string autor = "nintend01337";
        public string version = "1.2.5 beta";
        public string ApiVersion;
        public string soldiername;
        public string soldierID;
        public bool isDebug = false;

        public MainWindow()
        {
            InitializeComponent();
            App.Current.MainWindow = this;
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            App.Client.ErrorOccured += Client_ErrorOccured;
            App.Client.UserInfoReceived += Client_UserInfoReceived;
            App.Client.GameStateReceived += Client_GameStateReceived;
            App.Client.APIVersionReceived += Client_APIVersionReceived;
            App.Client.Disconnected += Client_Disconnected;
            //App.Client.ConnectionStateChanged += Client_ConnectionStateChanged;
           
            if (App.Client.Connect())
            {
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
            //Title = AssemblyName + " | " + "Welcome " + UserName;
            soldiername = UserName;
            soldierID = UserID.ToString();
        }

        private void Client_Disconnected(Zlo.Extras.DisconnectionReasons Reason)
        {
            // MessageBox.Show($"Клиент отключен по причине : {Reason}");
            Dispatcher.Invoke(async () =>
            {
                await this.ShowMessageAsync("",$"Вылет по причине : {Reason}",MessageDialogStyle.Affirmative);
            });
        }

        private void Client_APIVersionReceived(Version Current, Version Latest, bool IsNeedUpdate, string DownloadAdress)
        {            
            if (IsNeedUpdate) {
                Dispatcher.Invoke(async() =>
                {
                    // MessageBox.Show($"Текущая dll версия : {Current}\n Последняя dll версия : {Latest}\n Обновить сейчас? Zlo.dll", "Обновление", MessageBoxButton.YesNo);
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
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //Zlo.dll completed
            if (e.Error != null)
            {
                //error occured
                Client_ErrorOccured(e.Error, "Возникла ошибка при обновлении Zlo.dll");
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
            if (isDebug) MessageBox.Show($"{Error.ToString()}", CustomMessage);
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
        }

        private void OnGameClosed()
        {
            MainTabControl.Visibility = Visibility.Visible;
            Banner.Visibility = Visibility.Hidden;            
        }

        private void CloseGameBtn_Click(object sender, RoutedEventArgs e)
        {
            //Zlo.Extras.ZloGame game = new ZloGame();
            OnGameClosed();
            //Process[] proc = Process.GetProcessesByName(game.ToString());
            //proc[0].Kill();
        } // Надо это по красоте сделать

        private void LogsGrid_MouseLeave(object sender, MouseEventArgs e) { LogGrid.Visibility = Visibility.Hidden; }

        #endregion

        private void StatusBar_MouseDoubleClick(object sender, MouseButtonEventArgs e){LogGrid.Visibility = Visibility.Visible;}

        private void LogGrid_MouseLeave(object sender, MouseEventArgs e){LogGrid.Visibility = Visibility.Hidden;}
    }            
}
