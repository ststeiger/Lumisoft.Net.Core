
using System;
using System.Windows.Forms;
using System.Threading;

// namespace MailServerManager
namespace LumiSoft.MailServer.UI
{


    /// <summary>
    /// Application main class.
    /// </summary>
    public class Program
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
            Application.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Application.Run(new wfrm_Main());
        }

        #endregion

        #region static method Application_ThreadException

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            wfrm_sys_Error frm = new wfrm_sys_Error(e.Exception, new System.Diagnostics.StackTrace());
            frm.ShowDialog(null);
        }

        #endregion

        #region static method CurrentDomain_UnhandledException

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            wfrm_sys_Error frm = new wfrm_sys_Error((Exception)e.ExceptionObject, new System.Diagnostics.StackTrace());
            frm.ShowDialog(null);
        }

        #endregion

    }


}
