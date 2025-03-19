using System.Runtime.Versioning;

namespace QuakeMapFast
{
    [SupportedOSPlatform("windows7.0")]
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CtrlForm());
        }
    }
}
