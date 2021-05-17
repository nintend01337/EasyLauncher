using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Zlo;
using Zlo.Extras;
using System.Windows.Forms;
using System.IO;

namespace ZloGUILauncher.Addons
{
    public class Injector: IDisposable
    {
        #region signatures
        [DllImport("kernel32")]
        public static extern IntPtr CreateRemoteThread(
              IntPtr hProcess,
              IntPtr lpThreadAttributes,
              uint dwStackSize,
              UIntPtr lpStartAddress,
              IntPtr lpParameter,
              uint dwCreationFlags,
              out IntPtr lpThreadId
            );

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            UInt32 dwDesiredAccess,
            Int32 bInheritHandle,
            Int32 dwProcessId
            );

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(
        IntPtr hObject
        );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            UIntPtr dwSize,
            uint dwFreeType
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern UIntPtr GetProcAddress(
            IntPtr hModule,
            string procName
            );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect
            );

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            string lpBuffer,
            UIntPtr nSize,
            out IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName
            );

        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        internal static extern Int32 WaitForSingleObject(
            IntPtr handle,
            Int32 milliseconds
            );

        public Int32 GetProcessId(String proc)
        {
            Process[] ProcList;
            ProcList = Process.GetProcessesByName(proc);
            return ProcList[0].Id;
        }
        #endregion

        //private List<string> QuickDlls
        //{
        //    get
        //    {
        //        return App.Client?.GetDllsList(ZloBFGame.BF_3);
        //    }
        //}

        public static string dllname = "BF3FX.dll";
        public static string path = AppDomain.CurrentDomain.BaseDirectory;
        public string fullpath = Path.Combine(path, dllname);
        private static int skip = 354;

        public Injector()
        {
            var di = new DllInjector();
            di.Show();
        }
       
        public Injector(string procname)
        {
            //Int32 ProcID = GetProcessId(procname);
            //if (ProcID >= 0)
            //{
            //    IntPtr hProcess = (IntPtr)OpenProcess(0x1F0FFF, 1, ProcID);
            //    if (hProcess == null)
            //    {
            //        MessageBox.Show("FAIL");
            //        return;
            //    }
            //    else
            //    {
            //        InjectLibrary(hProcess, dllname);
            //        MessageBox.Show("TRUE");
            //    }
            //}
        }
        private void InjectLibrary(IntPtr hProcess, String strDLLName)
        {
            IntPtr bytesout;
            Int32 LenWrite = strDLLName.Length + 1;
            IntPtr AllocMem = (IntPtr)VirtualAllocEx(hProcess, (IntPtr)null, (uint)LenWrite, 0x1000, 0x40);
            WriteProcessMemory(hProcess, AllocMem, strDLLName, (UIntPtr)LenWrite, out bytesout);
            UIntPtr Injector = (UIntPtr)GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (Injector == null)
            {
                MessageBox.Show(" Injecto Error! \n ");
                return;
            }
            IntPtr hThread = (IntPtr)CreateRemoteThread(hProcess, (IntPtr)null, 0, Injector, AllocMem, 0, out bytesout);
            if (hThread == null)
            {
                MessageBox.Show("Thread injection Failed");
                return;
            }
            int Result = WaitForSingleObject(hThread, 10 * 1000);
            if (Result == 0x00000080L || Result == 0x00000102L || Result == 0xFFFFFFFF)
            {
                MessageBox.Show("Thread 2 inject failed");
                if (hThread != null)
                {
                    CloseHandle(hThread);
                }
                return;
            }
            Thread.Sleep(1000);
            VirtualFreeEx(hProcess, AllocMem, (UIntPtr)0, 0x8000);
            if (hThread != null)
            {
                CloseHandle(hThread);
            }
           
        }

        public void Install()
        {
            if (!File.Exists(fullpath))
            {
                File.WriteAllBytes(fullpath, Properties.Resources.BF3FX);
            }

         //   string data = File.ReadAllText("Zlo_settings.json");
            //string part1, part2 = "";
            //part2 = data.Substring(353);
            //part1 = data.Substring(0, 353);
            //string newdata = part1 +'"'+ fullpath + '"'+ part2;
            //data.Insert(skip+1,fullpath);



         //   File.WriteAllText("Zlo_settings1.json",newdata);
        }


        public void Dispose()
        {
      
        }
    }
}
