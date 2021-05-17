using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Zlo.Extras;
using ZloGUILauncher.Servers;

namespace ZloGUILauncher.Views
{
    public partial class BFHServerListView : UserControl
    {

        public CollectionViewSource ViewSource
        {
            get { return TryFindResource("ServersView") as CollectionViewSource; }
        }
        public BFHServerListView()
        {
            InitializeComponent();

            App.BFListViewModel.DataGrids[ZloBFGame.BF_HardLine] = ServersDG;
            ViewSource.Source = App.BFListViewModel.BFH_GUI_Servers;
        }
        #region Deprecated
        //public void AnimateRow(BFH_GUI_Server element)
        //{
        //    var row = ServersDG.ItemContainerGenerator.ContainerFromItem(element) as DataGridRow;
        //    if (row == null) return;

        //    var switchOnAnimation = new ColorAnimation
        //    {
        //        From = Colors.Transparent,
        //        To = Colors.Lime,
        //        Duration = TimeSpan.FromSeconds(0.75),
        //        AutoReverse = true
        //    };
        //    var blinkStoryboard = new Storyboard();

        //    blinkStoryboard.Children.Add(switchOnAnimation);
        //    Storyboard.SetTargetProperty(switchOnAnimation, new PropertyPath("Background.Color"));
        //    //animate changed server
        //    Storyboard.SetTarget(switchOnAnimation, row);

        //    row.BeginStoryboard(blinkStoryboard);
        //}
        #endregion
        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button b)) return;
            var server = (BFH_GUI_Server)b.DataContext;
            if(server != null)
                App.Client.JoinOnlineServer (OnlinePlayModes.BFH_Multi_Player, server.ID);
        }
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var server = (BFH_GUI_Server)b.DataContext;
            ServerInfoViewer si = new ServerInfoViewer(server);
            si.Show();
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

        private void ServersDG_Select(object sender, SelectionChangedEventArgs e)
        {
            fly.IsOpen = true;
        }

        private void ServersDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            fly.IsOpen = true;
        }

        private void fly_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            fly.IsOpen = false;
        }

        private void ServersDG_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            fly.IsOpen = true;
        }
    }
}
