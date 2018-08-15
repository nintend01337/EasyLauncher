
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zlo.Extras;
using ZloGUILauncher.Servers;

namespace ZloGUILauncher.Views
{
    public partial class BF4ServerListView : UserControl
    {
        public CollectionViewSource ViewSource => TryFindResource("ServersView") as CollectionViewSource;
        private ObservableCollection<BF4_GUI_Server> _mBf4Servers;
        public ObservableCollection<BF4_GUI_Server> Bf4GuiServers => _mBf4Servers ?? (_mBf4Servers = new ObservableCollection<BF4_GUI_Server>());

        public API_BF4ServersListBase DataServersList => App.Client.BF4Servers;

        public BF4ServerListView()
        {
            InitializeComponent();
            DataServersList.ServerAdded += DataServersList_ServerAdded;
            DataServersList.ServerUpdated += DataServersList_ServerUpdated;
            DataServersList.ServerRemoved += DataServersList_ServerRemoved;
            ViewSource.Source = Bf4GuiServers;
            fly.IsOpen = false;
        }       
        private void DataServersList_ServerRemoved(uint id , API_BF4ServerBase server)
        {
            if (server != null)
            {
                Dispatcher.Invoke(() =>
                {
                    //remove from current list
                    var ser = Bf4GuiServers.Find(s => s.ID == id);
                    if (ser != null)
                    {
                        Bf4GuiServers.Remove(ser);
                    }                   
                });
            }
        }
        private void DataServersList_ServerUpdated(uint id , API_BF4ServerBase server)
        {
            Dispatcher.Invoke(() =>
            {
                var equi = Bf4GuiServers.Find(x => x.raw == server);
                if (equi == null) return;
                equi.UpdateAllProps();
                AnimateRow(equi);
            });
        }
        private void DataServersList_ServerAdded(uint id , API_BF4ServerBase server)
        {
            Dispatcher.Invoke(() =>
            {
                var newserv = new BF4_GUI_Server(server);
                Bf4GuiServers.Add(newserv);                
            });
        }

        public void AnimateRow(BF4_GUI_Server element)
        {
            var row = ServersDG.ItemContainerGenerator.ContainerFromItem(element) as DataGridRow;
            if (row == null) return;

            var switchOnAnimation = new ColorAnimation
            {
                From = Colors.Transparent,
                To = Colors.Lime,
                Duration = TimeSpan.FromSeconds(0.75),
                AutoReverse = true
            };
            var blinkStoryboard = new Storyboard();

            blinkStoryboard.Children.Add(switchOnAnimation);
            Storyboard.SetTargetProperty(switchOnAnimation, new PropertyPath("Background.Color"));
            //animate changed server
            Storyboard.SetTarget(switchOnAnimation, row);

            row.BeginStoryboard(blinkStoryboard);
        }

        private void JoinButton_Click(object sender , RoutedEventArgs e)
        {
            var b = sender as Button;
            var server = (BF4_GUI_Server)b.DataContext;
            if (server == null) return;
            if (server.Moded && Settings.Default.Config.config.ModSupport)
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = "-i",
                    FileName = "Mod.exe"
                };
                var modProc = Process.Start(startInfo);
                modProc?.WaitForExit();
                App.Client.JoinOnlineGame(OnlinePlayModes.BF4_Multi_Player, server.ID);
            }

            if (!server.Moded && Settings.Default.Config.config.ModSupport)
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = "-u",
                    FileName = "Mod.exe"
                };
                var modProc = Process.Start(startInfo);
                modProc?.WaitForExit();
                App.Client.JoinOnlineGame(OnlinePlayModes.BF4_Multi_Player, server.ID);
            }

            else
            {
                App.Client.JoinOnlineGame(OnlinePlayModes.BF4_Multi_Player, server.ID);
            }


        }
        private void JoinSpectatorButton_Click(object sender , RoutedEventArgs e)
        {
            var b = sender as Button;
            var server = (BF4_GUI_Server)b.DataContext;
            if (server != null)
                App.Client.JoinOnlineGame(OnlinePlayModes.BF4_Spectator , server.ID); 
        }
        private void JoinCommanderButton_Click(object sender , RoutedEventArgs e)
        {
            var b = sender as Button;
            var server = (BF4_GUI_Server)b.DataContext;
            if (server != null)
                App.Client.JoinOnlineGame(OnlinePlayModes.BF4_Commander , server.ID); 
        }

        //private void ScrollViewer_PreviewMouseWheel(object sender , MouseWheelEventArgs e)
        //{
        //    if (sender.GetType() == typeof(ScrollViewer))
        //    {
        //        ScrollViewer scrollviewer = sender as ScrollViewer;
        //        if (e.Delta > 0)
        //            scrollviewer.LineLeft();
        //        else
        //            scrollviewer.LineRight();
        //        e.Handled = true;
        //    }
        //    else
        //    {
        //        var d = sender as DependencyObject;
        //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
        //        {
        //            if (VisualTreeHelper.GetChild(d , i) is ScrollViewer)
        //            {
        //                ScrollViewer scroll = (ScrollViewer)(VisualTreeHelper.GetChild(d , i));
        //                if (e.Delta > 0)
        //                    scroll.LineLeft();
        //                else
        //                    scroll.LineRight();
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //}

        private void ServersDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fly.IsOpen = true;
        }
    }
}
