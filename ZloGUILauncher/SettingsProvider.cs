using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace ZloGUILauncher
{
   public class SettingsProvider
    {
        public string XMLPath = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
        public string ZloPath = " ";
        public bool Debug = true;

            SettingsProvider()
             {}

        public void Write()
        {
            XmlSerializer xser = new XmlSerializer(typeof(SettingsProvider));
                TextWriter tw = new StreamWriter(XMLPath);
             xser.Serialize(tw, new SettingsProvider());
          MessageBox.Show("Settings saved at" + XMLPath);
        }
        
        public void Read()
        {
            XmlSerializer xser = new XmlSerializer(typeof(SettingsProvider));
            TextReader reader = new StreamReader(XMLPath);
                xser.Deserialize(reader);
            reader.Close();
        }
    }
}
