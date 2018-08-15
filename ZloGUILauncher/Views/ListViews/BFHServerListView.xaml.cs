using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    public partial class BFHServerListView : UserControl
    {

        public CollectionViewSource ViewSource => TryFindResource("ServersView") as CollectionViewSource;
        private ObservableCollection<BFH_GUI_Server> _mBfhServers;
        public ObservableCollection<BFH_GUI_Server> BfhGuiServers => _mBfhServers ?? (_mBfhServers = new ObservableCollection<BFH_GUI_Server>());

        public API_BFHServersListBase DataServersList => App.Client.BFHServers;

        public BFHServerListView()
        {
            InitializeComponent();
            DataServersList.ServerAdded += DataServersList_ServerAdded;
            DataServersList.ServerUpdated += DataServersList_ServerUpdated;
            DataServersList.ServerRemoved += DataServersList_ServerRemoved;
            ViewSource.Source = BfhGuiServers;
            fly.IsOpen = false;
        }
        private void DataServersList_ServerRemoved(uint id, API_BFHServerBase server)
        {
            if (server == null) return;
            Dispatcher.Invoke(() =>
            {
                //remove from current list
                var ser = BfhGuiServers.Find(s => s.ID == id);
                if (ser != null)
                {
                    BfhGuiServers.Remove(ser);
                }
            });
        }
        private void DataServersList_ServerUpdated(uint id, API_BFHServerBase server)
        {
            Dispatcher.Invoke(() =>
            {
                var equi = BfhGuiServers.Find(x => x.raw == server);
                if (equi == null) return;
                equi.UpdateAllProps();
                AnimateRow(equi);
            });
        }
        private void DataServersList_ServerAdded(uint id, API_BFHServerBase server)
        {
            Dispatcher.Invoke(() =>
            {
                var newserv = new BFH_GUI_Server(server);
                BfhGuiServers.Add(newserv);
                AnimateRow(newserv);
            });
        }

        public void AnimateRow(BFH_GUI_Server element)
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

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button b)) return;
            var server = (BFH_GUI_Server)b.DataContext;
            if(server != null)
                App.Client.JoinOnlineGame(OnlinePlayModes.BFH_Multi_Player, server.ID);
        }

        //private void JoinSpectatorButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var b = sender as Button;
        //    var server = (BFH_GUI_Server)b.DataContext;
        //    App.Client.JoinOnlineGame(OnlinePlayModes.BFH_Spectator, server.ID);
        //}
        //private void JoinCommanderButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var b = sender as Button;
        //    var server = (BFH_GUI_Server)b.DataContext;
        //    App.Client.JoinOnlineGame(OnlinePlayModes.BFH_Commander, server.ID);
        //}

        //private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
        //            if (VisualTreeHelper.GetChild(d, i) is ScrollViewer scroll)
        //            {
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
            //if (e.AddedItems != null && e.AddedItems.Count > 0)
            //{
            //    if (e.AddedItems[0] is BFH_GUI_Server serv)
            //    {
            //        serv.getCountry();
            //    }
            //}
            fly.IsOpen = true;
        }
    }
}
