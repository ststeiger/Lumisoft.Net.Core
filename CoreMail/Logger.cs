using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.Log;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Provides logging related methods.
    /// </summary>
    internal class Logger
    {
        #region method WriteLog

        /// <summary>
		/// Writes specified text to log file.
		/// </summary>
		/// <param name="fileName">Log file name.</param>
		/// <param name="text">Log text.</param>
		public static void WriteLog(string fileName,string text)
		{
			try{
                fileName = SCore.PathFix(fileName);
                                
				// If there isn't such directory, create it.
                if(!Directory.Exists(Path.GetDirectoryName(fileName))){
				    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				}
				
				using(FileStream fs = new FileStream(fileName,FileMode.OpenOrCreate,FileAccess.Write)){
					StreamWriter w = new StreamWriter(fs);     // create a Char writer 
					w.BaseStream.Seek(0, SeekOrigin.End);      // set the file pointer to the end
					w.Write(text + "\r\n");
					w.Flush();  // update underlying file
				}
			}
			catch{
			}
        }

        #endregion

        #region static method WriteLog

        /// <summary>
        /// Writes specified log entry to log file.
        /// </summary>
        /// <param name="file">Log file.</param>
        /// <param name="e">Log entry to write.</param>
        public static void WriteLog(string file,LogEntry e)
        {
            try{
                using(TextDb db = new TextDb('\t')){
                    db.OpenOrCreate(file);

                    //db.AppendComment("Fields: ID Time RemoteEndPoint AuthenticatedUser LogType LogText");
                    
                    string logText = "";
                    if(e.Text != null){
                        logText = e.Text.Replace("\r","");
                        if(logText.EndsWith("\n")){
                            logText = logText.Substring(0,logText.Length - 1);
                        }
                    }

                    string logType = "";
                    if(e.EntryType == LogEntryType.Text){
                        logType = "xxx";
                    }
                    else if(e.EntryType == LogEntryType.Read){
                        logType = "<<<";
                    }
                    else if(e.EntryType == LogEntryType.Write){
                        logType = ">>>";
                    }

                    foreach(string logLine in logText.Split('\n')){
                        db.Append(new string[]{
                            e.ID,
                            DateTime.Now.ToString(),
                            e.RemoteEndPoint != null ? e.RemoteEndPoint.ToString() : "",
                            e.UserIdentity != null ? e.UserIdentity.Name : "",
                            logType,
                            logLine
                        });
                    }                    
                }
            }
            catch(Exception x){
                Error.DumpError(x,new System.Diagnostics.StackTrace());
            }
        }

        #endregion
    }
}
