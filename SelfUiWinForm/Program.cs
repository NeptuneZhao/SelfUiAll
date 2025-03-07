global using System;
global using System.Runtime.Versioning;
global using System.Windows.Forms;

namespace SelfUiWinForm
{
    [SupportedOSPlatform("Windows")]
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(mainForm);

        }

        public static MainForm mainForm = new();
    }
}
