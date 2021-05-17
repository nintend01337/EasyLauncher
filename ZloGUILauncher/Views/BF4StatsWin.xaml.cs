using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using Zlo.Extras;
using Newtonsoft.Json.Linq;

namespace ZloGUILauncher.Views
{
    public partial class BF4StatsWin : Window
    {
        public BF4StatsWin()
        {
            InitializeComponent();
            DataContext = Bf4Stats;
          //  App.Client.StatsReceived += Client_StatsReceived;
        }

        //private void Client_StatsReceived(ZloGame Game , Dictionary<string , float> List)
        //{
        //    if (Game == ZloGame.BF_4)
        //    {
        //        BF4Stats.UpdateObject();
        //    }
        //}

        private void Window_Closing(object sender , CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private static BF4StatsDataContext _mBf4Stats;
        public static BF4StatsDataContext Bf4Stats => _mBf4Stats ?? (_mBf4Stats = new BF4StatsDataContext());

        private void BackGroundViewer_MediaEnded(object sender, RoutedEventArgs e)
        {
            var me = (MediaElement)sender;
            me.Position = TimeSpan.Zero;
        }

        private void RefreshBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Client.GetStats(ZloBFGame.BF_4);
        }

        private void CloseBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }
    }

    public class BF4StatsDataContext : INotifyPropertyChanged
    {
        public void UpdateObject()
        {
            Opc(nameof(Raw));
            Opc(nameof(Player));
            Opc(nameof(CurrRankImage));
            Opc(nameof(NextRankImage));
            Opc(nameof(Kdr));
        }

        public JObject Raw => App.Client.BF4_Stats;

        public JObject Player => (JObject)Raw["player"];

        public double Kdr => Raw["stats"]["kills"].ToObject<double>() / Raw["stats"]["deaths"].ToObject<double>();

        public BitmapImage CurrRankImage => new BitmapImage(new Uri("pack://application:,,,/Media/" + Player["rank"]["imgLarge"]));

        public BitmapImage NextRankImage => new BitmapImage(new Uri("pack://application:,,,/Media/" + Player["rank"]["next"]["imgLarge"]));

        public void Opc(string propname)
        {
            PropertyChanged?.Invoke(this , new PropertyChangedEventArgs(propname));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
