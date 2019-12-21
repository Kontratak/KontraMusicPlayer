using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kontra_Music_Player
{
    static class Program
    {

        
        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError=true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags,IntPtr dwItem1, IntPtr dwItem2);

        [STAThread]
        static void Main(String[] args)
        {
            if (System.Diagnostics.Process.GetProcessesByName("Kontra Music Player").Length > 1)
            {
                Application.Exit();
            }
            else
            {
                SetAssociation();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //args = new[] { "C:\\Users\\turk_\\Desktop\\Duman - Bal.mp3" };
                if (args != null && args.Length > 0)
                {
                    string filepath = args[0].ToString();
                    Application.Run(new Form1(args[0]));
                }
                else
                    Application.Run(new Form1());
            }
            
        }

        public static void SetAssociation()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.mp3");
            RegistryKey appreg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\Applications\\Kontra Music Player");
            RegistryKey appassoc = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.mp3");

            key.CreateSubKey("DefaultIcon").SetValue("", Application.StartupPath + "\\images\\iconmp3.ico");
            key.CreateSubKey("PerceivedType").SetValue("", "Music Player");

            appreg.CreateSubKey("shell\\open\\command").SetValue("", "\"" + Application.ExecutablePath + "\" %1");
            appreg.CreateSubKey("DefaultIcon").SetValue("", Application.StartupPath + "\\images\\iconmp3.ico");

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

    }
       
}