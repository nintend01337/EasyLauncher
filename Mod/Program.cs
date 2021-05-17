using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Mod
{
    class Program
    {
        public static void Draw()
        {
            Console.WriteLine(@"
         _         _                    _   ___   _  _____ _____ _____ 
  _ __  (_) _ __  | |_  ___  _ __    __| | / _ \ / ||___ /|___ /|___  |
 | '_ \ | || '_ \ | __|/ _ \| '_ \  / _` || | | || |  |_ \  |_ \   / / 
 | | | || || | | || |_|  __/| | | || (_| || |_| || | ___) |___) | / /  
 |_| |_||_||_| |_| \__|\___||_| |_| \__,_| \___/ |_||____/|____/ /_/   
");

            Console.WriteLine(@"
                                 _   _              _ 
             _ __ ___   ___   __| | | |_ ___   ___ | |
            | '_ ` _ \ / _ \ / _` | | __/ _ \ / _ \| |
            | | | | | | (_) | (_| | | || (_) | (_) | |
            |_| |_| |_|\___/ \__,_|  \__\___/ \___/|_|
");
        }

        private static void UnInstall()
        {
            Console.WriteLine("Удаление..........");

            string path = InstallPath();

            if (File.Exists(Path.Combine(path, "dinput8.dll")))
            {
                Console.WriteLine("Удаляю..........");
                //Console.WriteLine("Установка не требуется");
                File.Move(Path.Combine(path,"dinput8.dll"), Path.Combine(path,"mod_dinput8.dll"));

            }
        }

        private static void Install()
        {
            Console.WriteLine("Установка..........");

            string path = InstallPath();

            if (!File.Exists(Path.Combine(path, "dinput8.dll")))
            {
                if (File.Exists(Path.Combine(path, "mod_dinput8.dll")))
                { 
                    File.Move(Path.Combine(path, "mod_dinput8.dll"), (Path.Combine(path, "dinput8.dll")));
                    Console.WriteLine("Установка завершена");
                }

                else
                {
                    Console.WriteLine("Не нахожу файлов в папке с игрой. Качаю со ZLO....");
                    DownloadFile();
                    
                }
            }
            else
                Console.WriteLine("Установка не требуется");
        }

        private static void DownloadFile()
        {
            string DownloadLib = "https://zloemu.net/files/dinput8_mod.dll";
            string path = InstallPath();

            //если  рядом нет файла то качаем с интернета, если есть сразу ложим в папку.

            if (File.Exists( "dinput8.dll"))
            {
                Console.WriteLine("Вижю рядом файл. Устанавливаю....");
                File.Copy("dinput8.dll", Path.Combine(path, "dinput8.dll"));
                Console.WriteLine("Установка завершена......");
            }

            else
            {
                using (WebClient wc = new WebClient())
                {
                    Console.WriteLine("Начинаю закачивать файл со ЗЛО.....");
                    wc.DownloadFileAsync(new Uri(DownloadLib,UriKind.Absolute), "dinput8.dll");
                    wc.DownloadFileCompleted += wc_downloadFileEnd;
                }

            }

        }

        private static void wc_downloadFileEnd(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine("Закачка завершена");
            DownloadFile();
        }

        
        

        public static string InstallPath()
        {
            string path = "";
            using (var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EA Games\Battlefield 4"))
            {
                if (reg != null)
                {
                    var val = reg.GetValue("Install Dir", null) as string;
                    path =  val;
                    return path;
                }
                return "Игра не установлена исправте это !!!";
            }
        }

       
        static void Main(string[] args)
        {
            Draw();
            Console.Title = "mod install tool";
            var p = InstallPath();
            Console.WriteLine($"Путь по которому установлена игра : \n {p} \n");

            // parse args using switch
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-i":
                        Install();
                        break;
                    case "-u":
                        UnInstall();
                        break;
                    default:
                        Console.WriteLine("Неверные входящие аргументы  -i для установки -u для удаления!");
                        break;
                }    
            }
            else
            {
                Console.WriteLine("Отсутствуют входящие аргументы, запустите программу с параметром -i для установки -u для удаления!");
            }
            
                
            
            //    Console.WriteLine("Закройте это окно, иначе не запустится игра.");
            Console.ReadKey();
        }
    }
}
