using System;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// Error handling.
	/// </summary>
	internal class Error
	{
		private static string m_Path = "";

		#region static method DumpError
        
        /// <summary>
		/// Writes error to error log file.
		/// </summary>
		/// <param name="x"></param>
		public static void DumpError(Exception x)
		{
            DumpError("",x,new StackTrace());
        }

        /// <summary>
        /// Writes error to error log file.
        /// </summary>
		/// <param name="virtualServer">Virtual server name.</param>
        /// <param name="x"></param>
		public static void DumpError(string virtualServer,Exception x)
		{
            DumpError("",x,new StackTrace());
        }

        /// <summary>
		/// Writes error to error log file.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="stackTrace"></param>
		public static void DumpError(Exception x,StackTrace stackTrace)
		{
            DumpError("",x,stackTrace);
        }

		/// <summary>
		/// Writes error to error log file.
		/// </summary>
		/// <param name="virtualServer">Virtual server name.</param>
		/// <param name="x"></param>
		/// <param name="stackTrace"></param>
		public static void DumpError(string virtualServer,Exception x,StackTrace stackTrace)
		{
			try{
				string source = stackTrace.GetFrame(0).GetMethod().DeclaringType.FullName + "." + stackTrace.GetFrame(0).GetMethod().Name + "()";
                
				string errorText  = x.Message + "\r\n";
					   errorText += "//------------- function: " + source + "  " + DateTime.Now.ToString() + "------------//\r\n";
                       errorText += new StackTrace().ToString() + "\r\n";
                       errorText += "//--- Excetption info: -------------------------------------------------\r\n";
					   errorText += x.ToString() + "\r\n";

				if(x is System.Data.SqlClient.SqlException){
					System.Data.SqlClient.SqlException sX = (System.Data.SqlClient.SqlException)x;
					errorText += "\r\n\r\nSql errors:\r\n";

					foreach(System.Data.SqlClient.SqlError sErr in sX.Errors){
						errorText += "\n";
						errorText += "Procedure: '" + sErr.Procedure + "'  line: " + sErr.LineNumber.ToString() + "  error: " + sErr.Number.ToString() + "\r\n";
						errorText += "Message: " + sErr.Message + "\r\n";
					}				
				}

                if(x.InnerException != null){
                       errorText += "\r\n\r\n//------------- Innner Exception ----------\r\n" + x.Message + "\r\n";
					   errorText += x.InnerException.ToString();
                }

				DumpError(virtualServer,errorText);
			}
			catch{
			}
		}

		/// <summary>
		/// Writes error to error log file.
		/// </summary> 
		/// <param name="virtualServer">Virtual server name.</param>
		/// <param name="errorText">Error text to dump.</param>
		public static void DumpError(string virtualServer,string errorText)
		{
            try{
                DataSet ds = new DataSet("dsEvents");
                ds.Tables.Add("Events");
                ds.Tables["Events"].Columns.Add("ID");
                ds.Tables["Events"].Columns.Add("VirtualServer");
                ds.Tables["Events"].Columns.Add("CreateDate",typeof(DateTime));
                ds.Tables["Events"].Columns.Add("Type");
                ds.Tables["Events"].Columns.Add("Text");

                if(File.Exists(SCore.PathFix(m_Path + "Settings\\Events.xml"))){
                    ds.ReadXml(SCore.PathFix(m_Path + "Settings\\Events.xml"));
                }

                DataRow dr = ds.Tables["Events"].NewRow();
                dr["ID"]            = Guid.NewGuid().ToString();
                dr["VirtualServer"] = virtualServer;
                dr["CreateDate"]    = DateTime.Now;
                dr["Type"]          = 0;
                dr["Text"]          = errorText;
                ds.Tables["Events"].Rows.Add(dr);

                ds.WriteXml(SCore.PathFix(m_Path + "Settings\\Events.xml"));
            }
            catch{
            }
		}

		#endregion


		#region Propertis Implementation

		/// <summary>
		/// Gets or sets error file path.
		/// </summary>
		public static string ErrorFilePath
		{
			get{ return m_Path; }

			set{ m_Path = value; }
		}

		#endregion

	}
}
