using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Zlo.Extras;
using ZloGUILauncher.Servers;

namespace ZloGUILauncher.Views
{
    public partial class BF3ServerListView : UserControl
    {
        public CollectionViewSource ViewSource
        {
            get { return TryFindResource("ServersView") as CollectionViewSource; }
        }

        public BF3ServerListView()
        {
            InitializeComponent();            
		    App.BFListViewModel.DataGrids[ZloBFGame.BF_3] = ServersDG;
            ViewSource.Source = App.BFListViewModel.BF3_GUI_Servers;
		
            fly.IsOpen = false;
            if (Settings.Default.Config.config.AccentColorType != "accent")
            {
                Application.Current.Resources["SelectionBrushColor"] = Settings.Default.Config.config.Clr;
            }
        }
        #region Deprecated
        //public void AnimateRow(BF3_GUI_Server element)
        //{
        //    var row = ServersDG.ItemContainerGenerator.ContainerFromItem(element) as DataGridRow;
        //    if (row == null) return;

        //    var switchOnAnimation = new ColorAnimation
        //    {
        //        From = Colors.Transparent,
        //        To = Colors.Lime ,
        //        Duration = TimeSpan.FromSeconds(0.75) ,
        //        AutoReverse = true
        //    };

        //    var blinkStoryboard = new Storyboard();

        //    blinkStoryboard.Children.Add(switchOnAnimation);
        //    Storyboard.SetTargetProperty(switchOnAnimation , new PropertyPath("Background.Color"));
        //    //animate changed server
        //    Storyboard.SetTarget(switchOnAnimation , row);

        //    row.BeginStoryboard(blinkStoryboard);
        //}
        #endregion
        private void JoinButton_Click(object sender , RoutedEventArgs e)
        {
            var b = sender as Button;
            var server = (BF3_GUI_Server)b.DataContext;

            if(server != null)
            {
                App.Client.JoinOnlineServer(OnlinePlayModes.BF3_Multi_Player, server.ID);
            } 
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var server = (BF3_GUI_Server)b.DataContext;
            ServerInfoViewer si = new ServerInfoViewer(server);
            si.Show();
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
