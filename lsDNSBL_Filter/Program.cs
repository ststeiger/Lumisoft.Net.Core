/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lsDNSBL_Filter
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
*/
using System;
using System.Windows.Forms;
using System.Threading;

namespace LumiSoft.MailServer.Filters
{
    /// <summary>
    /// Application main class.
    /// </summary>
    public class MainX
    {
        #region static method Main

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.Run(new wfrm_Main());
        }

        #endregion

        #region static method Application_ThreadException

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(null, e.Exception.ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region static method CurrentDomain_UnhandledException

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(null, ((Exception)e.ExceptionObject).ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
