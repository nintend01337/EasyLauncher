﻿using System;
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
    public class BFH_GUI_Server : INotifyPropertyChanged
    {
        public API_BFHServerBase raw;

        public BFH_GUI_Server(API_BFHServerBase b)
        {
            raw = b;
        }
        public uint ID
        {
            get { return raw.ServerID; }
        }
        public string Name
        {
            get { return raw.ServerName; }
        }

        public int Current_Players
        {
            get { return raw.Players.Count; }
        }
        public int Max_Players
        {
            get
            {
                return raw.PlayerCapacities[0];
            }
        }

        public string RepPlayers
        {
            get
            {
                return $"{Current_Players}/{Max_Players}";
            }
        }

        public int Ping { get; set; }
        public string Country { get; set; }
        private IPAddress m_IP;
        public IPAddress IP
        {
            get
            {
                if (m_IP == null || BitConverter.ToUInt32(m_IP.GetAddressBytes(), 0) == raw.ServerIP)
                {
                    m_IP = new IPAddress(BitConverter.GetBytes(raw.ServerIP));
                }
                return m_IP;
            }
        }
        public ushort Port
        {
            get
            {
                return raw.ServerPort;
            }
        }

        public string ServerType
        {
            get
            {
                return raw.Attributes["servertype"];
            }
        }

        private GUI_PlayerList m_Players;
        public GUI_PlayerList Players
        {
            get
            {
                if (m_Players == null)
                {
                    m_Players = new GUI_PlayerList(raw.Players);
                }
                return m_Players;
            }
        }

        private GUI_MapRotation m_Maps;
        public GUI_MapRotation Maps
        {
            get
            {
                if (m_Maps == null)
                {
                    m_Maps = new GUI_MapRotation(raw.MapRotation);
                }
                return m_Maps;
            }
        }



        public bool IsHasPW
        {
            get
            {
                return raw.IsPasswordProtected;
            }
        }
        public bool YesNo(string toconv)
        {
            if (toconv == "YES")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsHasPB
        {
            get
            {
                if (raw.Attributes.ContainsKey("punkbuster"))
                {
                    return YesNo(raw.Attributes["punkbuster"]);
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsHasFF
        {
            get
            {
                if (raw.Attributes.ContainsKey("fairfight"))
                {
                    return YesNo(raw.Attributes["fairfight"]);
                }
                else
                {
                    return false;
                }
            }
        }
        public void UpdateAllProps()
        {
            OPC(nameof(ID));
            OPC(nameof(Name));
            OPC(nameof(Current_Players));
            OPC(nameof(Max_Players));
            OPC(nameof(IP));
            OPC(nameof(Port));
            OPC(nameof(RepPlayers));
            OPC(nameof(ServerType));
            OPC(nameof(Players));
            OPC(nameof(Maps));
            OPC(nameof(IsHasPW));
            OPC(nameof(IsHasPB));
            OPC(nameof(IsHasFF));
            UpdatePing();

            Maps.Update();
            Players.Update();
        }

        public void UpdatePing()
        {
            Task.Run((Action)(() =>
            {
                try
                {
                    if (IP == null)
                        return;
                    PingReply pingReply = new System.Net.NetworkInformation.Ping().Send(raw.ServerIP.ToString(), 500);
                    if (pingReply.Status == IPStatus.Success)
                        this.Ping = ((int)pingReply.RoundtripTime);
                }
                catch (Exception ex)
                {
                    Ping = 666;
                    ex.Message.ToString();                 // сообщение в случаи неудачи проверки пинга незнаю зачем :)
                }
            }));
        }

            public void OPC(string prop)
             {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
              }
             public event PropertyChangedEventHandler PropertyChanged;

        public void getCountry()
        {
            try
            {
                string strFile = "Unknown";
                using (var objClient = new System.Net.WebClient())
                {
                    strFile = objClient.DownloadString("http://freegeoip.net/xml/" + IP.ToString());
                }
                int firstlocation = strFile.IndexOf("<CountryName>") + "<CountryName>".Length;
                int lastlocation = strFile.IndexOf("</", firstlocation);
                string location = strFile.Substring(firstlocation, lastlocation - firstlocation);
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    Country = location;
                }));
            }
            catch
            {
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    Country = "Неизвестно";
                }));
            }
        }
    }
}

