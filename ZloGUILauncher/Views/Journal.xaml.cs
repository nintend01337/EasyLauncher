using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZloGUILauncher.Addons;

namespace ZloGUILauncher.Views
{

    public partial class Journal : UserControl
    {
        public CollectionViewSource collectionViewSource
        {
            get { return TryFindResource("HelpGrid") as CollectionViewSource; }
        }

        private ObservableCollection<Help> m_Helps;
        public ObservableCollection<Help> Helps
        {
            get
            {
                if (m_Helps == null)
                {
                    m_Helps = new ObservableCollection<Help>();
                }
                return m_Helps;
            }
        }

        public Journal()
        {
            InitializeComponent();
            collectionViewSource.Source = Helps;
            Helps.Add(new Help { Code = " 5   1  -1 ", Params = "->", Description = "Ошибка х64 бит версии игры, использовать x86 ", });
            Helps.Add(new Help { Code = " 3   1  -1 ", Params = "->", Description = "Ошибка лицензии, удалить папку - C:\\ProgramData\\Electronic Arts (папка скрытая )", });
            Helps.Add(new Help { Code = " 0   4   x ", Params = "Game disconnected", Description = "Вас кикнул админ сервера  ", });
            Helps.Add(new Help { Code = " 0   5   x ", Params = "Game disconnected", Description = "Вы забанены на этом сервере", });
            Helps.Add(new Help { Code = " 0   6  -1 ", Params = "Game disconnected", Description = "Вы были отключены от мастер сервера ", });
            Helps.Add(new Help { Code = " 0  14   x ", Params = "Game disconnected", Description = "Вы были отключены от игры ", });
            Helps.Add(new Help { Code = " 0  20   1 ", Params = "Game disconnected", Description = "Вы были отключены за AFK ", });
            Helps.Add(new Help { Code = " 0  22  -1 ", Params = "Game disconnected", Description = "Кикнуло за тимкил (по идее я хз :) ) ", });
            Helps.Add(new Help { Code = " 0  23   x ", Params = "Game disconnected", Description = "Вас выгнал Админ (не знаю что это значит)", });
            Helps.Add(new Help { Code = " 0  24  -1 ", Params = "Game disconnected", Description = "Вас пнул PunkBuster. Описание в логе", });
            Helps.Add(new Help { Code = " 0  38   x ", Params = "Game disconnected", Description = "Вас кикнул или забанил FAIR FIGHT ", });
            Helps.Add(new Help { Code = " 1   1  -1 ", Params = "Game disconnected", Description = "Игровые файлы повреждены ", });
            Helps.Add(new Help { Code = " 4  12   x ", Params = "Game disconnected", Description = "Вы забанены на сервере ", });
            Helps.Add(new Help { Code = " 27  0   x ", Params = "Game disconnected", Description = "Установить оригинальный Origin", });
            Helps.Add(new Help { Code = " 3  16  -1 ", Params = "Origin error!", Description = "Запустите игру в режиме x86 ", });
            Helps.Add(new Help { Code = " 0   7   1 ", Params = "Game disconnected", Description = "На Сервере Пароль ", });
        }
    }
}
