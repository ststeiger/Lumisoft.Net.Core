using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using LumiSoft.MailServer.Filters;
using LumiSoft.Net.SMTP.Server;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// LumiSoft virus filter.
	/// </summary>
	public class lsVirusFilter : ISmtpMessageFilter,ISettingsUI
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public lsVirusFilter()
		{
        }

        #region method Filter

        /// <summary>
		/// Filters message.
		/// </summary>
		/// <param name="messageStream">Message stream which to filter.</param>
		/// <param name="filteredStream">Filtered stream.</param>
		/// <param name="sender">Senders email address.</param>
		/// <param name="recipients">Recipients email addresses.</param>
		/// <param name="api">Access to server API.</param>
		/// <param name="session">Reference to SMTP session.</param>
		/// <param name="errorText">Filtering error text what is returned to client. ASCII text, 500 chars maximum.</param>
		public FilterResult Filter(Stream messageStream,out Stream filteredStream,string sender,string[] recipients,IMailServerApi api,SMTP_Session session,out string errorText)
		{
			errorText = null;
			filteredStream = null;

            string file = API_Utlis.PathFix(Path.GetTempPath() + "\\" + Guid.NewGuid().ToString() + ".eml");
                        
			try{                
				// Store message to tmp file				
				using(FileStream fs = File.Create(file)){
                    byte[] data = new byte[messageStream.Length];
                    messageStream.Read(data,0,data.Length);
					fs.Write(data,0,data.Length);
				}

				// Execute virus program to scan tmp message
				// #FileName - place holder is replaced with file
				DataSet ds = new DataSet();
				ds.Tables.Add("Settings");
                ds.Tables["Settings"].Columns.Add("Program");
			    ds.Tables["Settings"].Columns.Add("Arguments");
                ds.Tables["Settings"].Columns.Add("VirusExitCode");
				ds.ReadXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\lsVirusFilter_db.xml");

				string virusSoft     = ds.Tables["Settings"].Rows[0]["Program"].ToString();
				string virusSoftArgs = ds.Tables["Settings"].Rows[0]["Arguments"].ToString().Replace("#FileName",file);
                int    virusExitCode = ConvertEx.ToInt32(ds.Tables["Settings"].Rows[0]["Program"],1);

                int exitCode = 0;
				System.Diagnostics.ProcessStartInfo sInf = new System.Diagnostics.ProcessStartInfo(virusSoft,virusSoftArgs);
				sInf.CreateNoWindow = true;
				sInf.UseShellExecute = false;
				System.Diagnostics.Process p = System.Diagnostics.Process.Start(sInf);
				if(p != null){
					p.WaitForExit(60000);
                    exitCode = p.ExitCode;
				}
                				
				if(File.Exists(file)){
					// Return scanned message and delete tmp file
					using(FileStream fs = File.OpenRead(file)){
						byte[] data = new byte[fs.Length];
						fs.Read(data,0,data.Length);

						filteredStream = new MemoryStream(data);
					}

					File.Delete(file);
				}
                // Virus scanner deleted message, probably contains virus
                else{
                    virusExitCode = exitCode;
                }
				
                // Do exit code mapping
                if(virusExitCode == exitCode){
                    errorText = "Message is blocked, contains virus !";
					return FilterResult.Error;
                }
			}
			catch(Exception x){
                string dummy = x.Message;
                // Virus scanning failed, allow message through
                filteredStream = messageStream;                
			}
            finally{
                if(File.Exists(file)){
                    File.Delete(file);
                }
            }
			
			return FilterResult.Store;
        }

        #endregion

        #region method GetUI

        /// <summary>
        /// Gets settings UI window.
        /// </summary>
        /// <returns></returns>
        public Form GetUI()
        {
            return new wfrm_Main();
        }

        #endregion

    }
}
