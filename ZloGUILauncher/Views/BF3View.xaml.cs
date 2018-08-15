using System.Windows;
using System.Windows.Controls;

namespace ZloGUILauncher.Views
{
    /// <summary>
    /// Interaction logic for BF3View.xaml
    /// </summary>
    public partial class BF3View : UserControl
    {
        public BF3View()
        {
            InitializeComponent();

            // App.Client.StatsReceived += Client_StatsReceived;
        }

        //private void Client_StatsReceived(Zlo.Extras.ZloGame Game , Dictionary<string , float> List)
        //{
        //    if (Game == Zlo.Extras.ZloGame.BF_3)
        //    {
        //        Dispatcher.Invoke(() => { StatsDG.ItemsSource = List; });
        //    }
        //}

        private void JoinSPButton_Click(object sender, RoutedEventArgs e)
        {
            App.Client.JoinOfflineGame(Zlo.Extras.OfflinePlayModes.BF3_Single_Player);
        }

        private void StatsRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            //App.Client.GetStats(Zlo.Extras.ZloGame.BF_3);
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tc)
            {
                if (tc.SelectedIndex < 0) return;
                switch (tc.SelectedIndex)
                {
                    case 1:
                        App.Client.JoinOfflineGame(Zlo.Extras.OfflinePlayModes.BF3_Single_Player);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
