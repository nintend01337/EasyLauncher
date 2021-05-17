using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZloGUILauncher.Servers;

namespace ZloGUILauncher.Views
{
    public partial class ServerInfoViewer : MetroWindow
    {
       public BF_GUI_Server Serverdata;
        
        private ServerInfoViewer()
        {
            InitializeComponent();
        }

        public ServerInfoViewer(BF3_GUI_Server server)
        {
           InitializeComponent();
           Serverdata = server;
           Serverdata =  (BF3_GUI_Server)Serverdata;
           DataContext = Serverdata;
        }

        public ServerInfoViewer(BF4_GUI_Server server)
        {
            InitializeComponent();
            Serverdata = server;
            Serverdata = (BF4_GUI_Server)Serverdata;
            DataContext = Serverdata;
        }

        public ServerInfoViewer(BFH_GUI_Server server)
        {
            InitializeComponent();
            Serverdata = server;
            Serverdata = (BFH_GUI_Server)Serverdata;
            DataContext = Serverdata;
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender.GetType() == typeof(ScrollViewer))
            {
                ScrollViewer scrollviewer = sender as ScrollViewer;
                if (e.Delta > 0)
                    scrollviewer.LineLeft();
                else
                    scrollviewer.LineRight();
                e.Handled = true;
            }
            else
            {
                var d = sender as DependencyObject;
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
                {
                    if (VisualTreeHelper.GetChild(d, i) is ScrollViewer)
                    {
                        ScrollViewer scroll = (ScrollViewer)(VisualTreeHelper.GetChild(d, i));
                        if (e.Delta > 0)
                            scroll.LineLeft();
                        else
                            scroll.LineRight();
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
