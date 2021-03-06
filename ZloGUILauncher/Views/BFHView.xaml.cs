﻿using System.Windows;
using System.Windows.Controls;
using Zlo.Extras;
namespace ZloGUILauncher.Views
{
    /// <summary>
    /// Логика взаимодействия для BFHView.xaml
    /// </summary>
    public partial class BFHView : UserControl
    {
        public BFHView()
        {
            InitializeComponent();
			//App.Client.StatsReceived += Client_StatsReceived;
            //App.Client.ItemsReceived += Client_ItemsReceived;
          //  App.Client.ClanDogTagsReceived += Client_ClanDogTagsReceived;
		}

        //private void Client_ClanDogTagsReceived(ZloGame game, ushort dogtag1, ushort dogtag2, string clanTag)
        //     {
        //         if (game == ZloGame.BF_HardLine)
        //         {
        //             Dispatcher.Invoke(() =>
        //             {
        //                 BFH_DT1.Text = dogtag1.ToString();
        //                 BFH_DT2.Text = dogtag2.ToString();
        //                 BFH_CT.Text = clanTag;
        //             });

        //         }
        //     }

        //private void Client_ItemsReceived(ZloGame Game, Dictionary<string, API_Item> List)
        //{
        //    if (Game == ZloGame.BF_HardLine)
        //    {
        //        Dispatcher.Invoke(() => { ItemsDG.ItemsSource = List; });
        //    }
        //}

        //private void Client_StatsReceived(ZloGame Game, Dictionary<string, float> List)
        //{
        //    if (Game == ZloGame.BF_HardLine)
        //    {
        //        Dispatcher.Invoke(() => { StatsDG.ItemsSource = List; });
        //    }
        //}

        private void StatsRefreshButton_Click(object sender, RoutedEventArgs e)
        {
         //   App.Client.GetStats(ZloGame.BF_HardLine);
        }

        private void ItemsRefreshButton_Click(object sender, RoutedEventArgs e)
        {
         //   App.Client.GetItems(ZloGame.BF_HardLine);
        }

        private void JoinSinglePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            App.Client.JoinOfflineGame(OfflinePlayModes.BFH_Single_Player);
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
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Client.JoinOfflineGame(Zlo.Extras.OfflinePlayModes.BFH_Single_Player);
            LScr.Visibility = Visibility.Visible;
            Btnscr.Visibility = Visibility.Hidden;
        }

        //private void SetterButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var tag = (sender as Button).Tag.ToString();
        //    switch (tag)
        //    {
        //        case "dt1":
        //            {
        //                if (ushort.TryParse(BFH_DT1.Text, out ushort holder))
        //                {
        //                    App.Client.SetClanDogTags(dt_advanced: holder);
        //                }
        //                break;
        //            }
        //        case "dt2":
        //            {
        //                if (ushort.TryParse(BFH_DT2.Text, out ushort holder))
        //                {
        //                    App.Client.SetClanDogTags(dt_basic: holder);
        //                }
        //                break;
        //            }
        //        case "ct":
        //            {
        //                App.Client.SetClanDogTags(clantag: BFH_CT.Text);
        //                break;
        //            }
        //        case "all":
        //            {
        //                ushort? finaldt1 = null, finaldt2 = null;
        //                if (ushort.TryParse(BFH_DT1.Text, out ushort holderdt1))
        //                {
        //                    finaldt1 = holderdt1;
        //                }
        //                if (ushort.TryParse(BFH_DT2.Text, out ushort holderdt2))
        //                {
        //                    finaldt2 = holderdt2;
        //                }
        //                App.Client.SetClanDogTags(finaldt1, finaldt2, BFH_CT.Text);
        //                break;
        //            }
        //        default:
        //            break;
        //    }
        //}

        //private void GetterButton_Click(object sender, RoutedEventArgs e)
        //{
        //    App.Client.GetClanDogTags();
        //}
    }
}
