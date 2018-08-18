using System.Windows;
using System.Windows.Controls;
using Zlo.Extras;

namespace ZloGUILauncher.Views
{
    /// <summary>
    /// Interaction logic for BF4View.xaml
    /// </summary>
    public partial class BF4View : UserControl
    {
        public BF4View()
        {
            InitializeComponent();
            //App.Client.StatsReceived += Client_StatsReceived;
            //App.Client.ItemsReceived += Client_ItemsReceived;
        }

        //private void Client_ItemsReceived(Zlo.Extras.ZloGame Game , Dictionary<string , API_Item> List)
        //{
        //    if (Game == Zlo.Extras.ZloGame.BF_4)
        //    {
        //        Dispatcher.Invoke(() => { ItemsDG.ItemsSource = List; });

        //    }
        //}

        //private void Client_StatsReceived(Zlo.Extras.ZloGame Game , Dictionary<string , float> List)
        //{
        //    if (Game == ZloGame.BF_4)
        //    {
        //        Dispatcher.Invoke(() => { StatsListWin.StatsDG.ItemsSource = List; });
        //    }
        //}

        private static BF4StatsListWindow _mStatsListWin;
        public static BF4StatsListWindow StatsListWin => _mStatsListWin ?? (_mStatsListWin = new BF4StatsListWindow());

        private static BF4StatsWin _mStatsWin;
        public static BF4StatsWin StatsWin => _mStatsWin ?? (_mStatsWin = new BF4StatsWin());
        
        
        private void StatsRefreshButton_Click(object sender , RoutedEventArgs e)
        {
           // App.Client.GetStats(Zlo.Extras.ZloGame.BF_4);
        }

        private void ItemsRefreshButton_Click(object sender , RoutedEventArgs e)
        {
          //  App.Client.GetItems(Zlo.Extras.ZloGame.BF_4);
        }

        private void JoinSinglePlayerButton_Click(object sender , RoutedEventArgs e)
        {
            App.Client.JoinOfflineGame(Zlo.Extras.OfflinePlayModes.BF4_Single_Player);
        }

        private void JoinTestRangeButton_Click(object sender , RoutedEventArgs e)
        {
            App.Client.JoinOfflineGame(Zlo.Extras.OfflinePlayModes.BF4_Test_Range);
        }

        private void StatsAsListButton_Click(object sender , RoutedEventArgs e)
        {
            StatsListWin.Show();
        }

        private void StatsAsWindowButton_Click(object sender , RoutedEventArgs e)
        {
            StatsWin.Show();            
        }

        private void MetroTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tc)
            {
                if (tc.SelectedIndex < 0) return;
                switch (tc.SelectedIndex)
                {
                    default:
                        LScr.Visibility = Visibility.Hidden;
                        Btnscr.Visibility = Visibility.Visible;
                        LScr2.Visibility = Visibility.Hidden;
                        Btnscr2.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Client.JoinOfflineGame(OfflinePlayModes.BF4_Single_Player);
            LScr.Visibility = Visibility.Visible;
            Btnscr.Visibility = Visibility.Hidden;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            App.Client.JoinOfflineGame(OfflinePlayModes.BF4_Test_Range);
            LScr2.Visibility = Visibility.Visible;
            Btnscr2.Visibility = Visibility.Hidden;
        }
    }
}
