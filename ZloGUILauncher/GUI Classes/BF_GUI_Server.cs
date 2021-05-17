using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zlo.Extras;

namespace ZloGUILauncher.Servers
{
    public abstract class BF_GUI_Server : INotifyPropertyChanged
    {
        public IBFServerBase Raw { get; }
        public BF_GUI_Server(IBFServerBase raw)
        {
            Raw = raw;
            Players = new GUI_PlayerList(raw.Players);
            Maps = new GUI_MapRotation(raw.MapRotation);
        }

        public uint ID => Raw.ServerID;
        public string Name => Raw.ServerName;

        public int Current_Players => Raw.Players.Count;
        public int Max_Players => Raw.MaxPlayers;   
       
        public string RepPlayers => $"{Current_Players}/{Max_Players}";


        private IPAddress m_IP;
        public IPAddress IP
        {
            get
            {
                if (m_IP == null || BitConverter.ToUInt32(m_IP.GetAddressBytes(), 0) == Raw.ServerIP)
                {
                    m_IP = new IPAddress(BitConverter.GetBytes(Raw.ServerIP));
                }
                return m_IP;
            }
        }

        public ushort Port => Raw.ServerPort;

        public GUI_PlayerList Players { get; }

        public GUI_MapRotation Maps { get; }

        public bool IsHasPW => Raw.IsPasswordProtected;

        public bool IsHasPB => Raw.Attributes.TryGetValue("punkbuster",out var pb) ? YesNo(pb) : false;
        public int Ping { get; set; }
        public string Country { get; set; }

        public virtual void UpdateAllProps()
        {
            OPC(nameof(ID));
            OPC(nameof(Name));
            OPC(nameof(Current_Players));
            OPC(nameof(Max_Players));
            OPC(nameof(IP));
            OPC(nameof(Port));
            OPC(nameof(RepPlayers));
            OPC(nameof(Players));
            OPC(nameof(Maps));
            OPC(nameof(IsHasPW));
            OPC(nameof(IsHasPB));

            Maps.Update();
            Players.Update();
            PingUpdate();
        //    CountryUpdate();
        }

        private void CountryUpdate()
        {
            var t = Task.Run(() =>
            {
                string strFile = "Unknown";
                using (var objClient = new System.Net.WebClient())
                { strFile = objClient.DownloadString("http://ipwhois.app/xml/" + Raw.ServerIP.ToString()); }
                int firstlocation = strFile.IndexOf("<country>") + "<country>".Length;
                int lastlocation = strFile.IndexOf("</", firstlocation);
                string location = strFile.Substring(firstlocation, lastlocation - firstlocation);
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => { Country = location; }));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Country"));
            });
        
       
        }

        private void PingUpdate()
        {
            Task t = Task.Run(() => 
            {

                    if (IP == null) return;
                    for (int i = 0; i < 3; i++)
                    {
                        PingReply pingReply = new System.Net.NetworkInformation.Ping().Send(Raw.ServerIP.ToString(), 500);
                        if (pingReply.Status == IPStatus.Success) this.Ping = ((int)pingReply.RoundtripTime);
                        if (this.Ping == 0)
                            Ping = -1;
                    }

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Ping"));
            }
                );

        }

        public static bool YesNo(string toconv)
        {
            if (toconv.ToLowerInvariant() == "yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void OPC(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}