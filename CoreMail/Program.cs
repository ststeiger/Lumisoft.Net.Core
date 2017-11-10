
// namespace CoreMail 
namespace LumiSoft.MailServer
{

    // static class Program

    /// <summary>
    /// Application main class.
    /// </summary>
    public class MainX // class Program
    {


        /// <summary>
        /// Application main entry point.
        /// </summary>
        /// <param name="args">Command line argumnets.</param>
        // [System.STAThread()]
        public static void Main(string[] args) // static void Main(string[] args)
        {
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new Form1());

            // Add app domain unhandled exception handler.
            System.AppDomain.CurrentDomain.UnhandledException +=
                new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            args = new string[] { "-daemon" };

            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                try
                {
                    System.IO.File.Delete(System.IO.Path.Combine(startupPath, "System.Data.SQLite.dll"));

                    // x64
                    if (System.IntPtr.Size == 8)
                    {
                        System.IO.File.Copy(
                              System.IO.Path.Combine(startupPath, "System.Data.SQLite.x64.dll")
                            , System.IO.Path.Combine(startupPath, "System.Data.SQLite.dll")
                        );
                    }
                    // x32
                    else
                    {
                        System.IO.File.Copy(
                              System.IO.Path.Combine(startupPath, "System.Data.SQLite.x86.dll")
                            , System.IO.Path.Combine(startupPath, "System.Data.SQLite.dll")
                        );
                    }
                }
                catch
                {
                    // We dont care File does not exist or in use errors here.
                }
            }
            catch (System.Exception x)
            {
                // System.Windows.Forms.MessageBox.Show("Error: " + x.ToString(), "Error:", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                System.Console.WriteLine("Error: " + x.ToString());
            }



            // if (args.Length > 0){
            if (args[0].ToLower() == "/?" || args[0].ToLower() == "/h")
            {
                string text = "";
                text += "Possible keys:\r\n";
                text += "\r\n";
                text += "\t -daemon, runs server as daemon application.\r\n";

                // After all, we removed them...
                // text += "\t -trayapp, runs server as Windows tray application.\r\n";
                // text += "\t -winform, runs server in Windows Forms window.\r\n";

                // System.Windows.Forms.MessageBox.Show(null, text, "Info:", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                System.Console.WriteLine(text);
            }
            else if (args[0].ToLower() == "-daemon")
            {
                System.Console.WriteLine("Starting Server");
                Server server = new Server();
                server.Start();

                System.Console.WriteLine("Server Started - Press any key to quit");
                //while (true)
                while (!System.Console.KeyAvailable)
                {
                    System.Threading.Thread.Sleep(1000);
                }

                server.Stop();
                System.Console.WriteLine("running");

            }
            //else if (args[0].ToLower() == "-winform")
            //{
            //    System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //    System.Windows.Forms.Application.EnableVisualStyles();
            //    System.Windows.Forms.Application.Run(new wfrm_WinForm());
            //}
            //else if (args[0].ToLower() == "-trayapp")
            //{
            //    System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //    System.Windows.Forms.Application.EnableVisualStyles();
            //    System.Windows.Forms.Application.Run(new wfrm_Tray());
            //}
            else
            {
                // System.Windows.Forms.MessageBox.Show("Invalid command line argument was specified ! (try /? or /h for help)", "Error:", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                System.Console.WriteLine("Error: Invalid command line argument was specified ! (try /? or /h for help)");
            }
            //}
        }


        /// <summary>
        /// This is called when unhandled exception happened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Error: " + ((Exception)e.ExceptionObject).ToString(), "Error:", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            System.Console.WriteLine("Error: " + ((System.Exception)e.ExceptionObject).ToString(), "Error:");
        }

        /// <summary>
        /// This method is called when unhandled excpetion happened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            System.Console.WriteLine("Error: " + e.Exception.ToString(), "Error:");
            // System.Windows.Forms.MessageBox.Show("Error: " + e.Exception.ToString(), "Error:", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }


    }


}
