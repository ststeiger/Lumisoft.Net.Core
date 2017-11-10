using System;
using System.IO;
using System.Data;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

using LumiSoft.MailServer;
using LumiSoft.MailServer.Filters;
using LumiSoft.Net;
using LumiSoft.Net.DNS;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.SMTP.Server;


namespace LumiSoft.MailServer
{
	/// <summary>
	/// LumiSoft DNSBL filter.
	/// </summary>
	public class lsDNSBL_Filter : ISmtpSenderFilter,ISettingsUI
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public lsDNSBL_Filter()
		{
        }


        #region method Filter

        /// <summary>
		/// Filters sender.
		/// </summary>
		/// <param name="from">Sender.</param>
		/// <param name="api">Reference to server API.</param>
		/// <param name="session">Reference to SMTP session.</param>
		/// <param name="errorText">Filtering error text what is returned to client. ASCII text, 100 chars maximum.</param>
		/// <returns>Returns true if sender is ok or false if rejected.</returns>
		public bool Filter(string from,IMailServerApi api,SMTP_Session session,out string errorText)
		{	
			errorText = null;
			bool ok = true;

			// Don't check authenticated users or LAN IP
			if(session.IsAuthenticated || IsPrivateIP(session.RemoteEndPoint.Address)){
				return true;
			}
			
			try{
				//--- Load data -----------------------
				DataSet ds = new DataSet();				
                ds.Tables.Add("General");
                ds.Tables["General"].Columns.Add("CheckHelo");
                ds.Tables["General"].Columns.Add("LogRejections");
                ds.Tables.Add("BlackListSettings");
                ds.Tables["BlackListSettings"].Columns.Add("ErrorText");
                ds.Tables.Add("BlackList");
                ds.Tables["BlackList"].Columns.Add("IP");
                ds.Tables.Add("Servers");
                ds.Tables["Servers"].Columns.Add("Cost");
                ds.Tables["Servers"].Columns.Add("Server");
                ds.Tables["Servers"].Columns.Add("DefaultRejectionText");
				
				ds.ReadXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\lsDNSBL_Filter_db.xml");

                bool logRejections = false;

                #region General

                if(ds.Tables["General"].Rows.Count == 1){
                    if(Convert.ToBoolean(ds.Tables["General"].Rows[0]["CheckHelo"])){            
                        DnsServerResponse response = Dns_Client.Static.Query(session.EhloHost,DNS_QType.A);
                        // If dns server connection errors, don't block.
                        if(response.ConnectionOk && response.ResponseCode != DNS_RCode.SERVER_FAILURE){
                            bool found = false;
                            foreach(DNS_rr_A a in response.GetARecords()){
                                if(session.RemoteEndPoint.Address.Equals(a.IP)){
                                    found = true;

                                    break;
                                }
                            }
                            if(!found){
                                errorText = "Not valid DNS EHLO/HELO name for your IP '" + session.EhloHost + "' !";

                                return false;
                            }
                        }
                    }
                    logRejections = ConvertEx.ToBoolean(ds.Tables["General"].Rows[0]["LogRejections"]);
                }

                #endregion

                #region Balck List

                foreach(DataRow dr in ds.Tables["BlackList"].Rows){
                    if(IsAstericMatch(dr["IP"].ToString(),session.RemoteEndPoint.Address.ToString())){
                        errorText = ds.Tables["BlackListSettings"].Rows[0]["ErrorText"].ToString();

                        return false;
                    }
                }

                #endregion

                #region DNSBL
                                               
				foreach(DataRow dr in ds.Tables["Servers"].Rows){
                    DnsServerResponse dnsResponse =  Dns_Client.Static.Query(ReverseIP(session.RemoteEndPoint.Address) + "." + dr["Server"].ToString(),DNS_QType.ANY);                    
					DNS_rr_A[] recs = dnsResponse.GetARecords();
					if(recs.Length > 0){
					    if(logRejections){
						    WriteFilterLog("Sender:" + from + " IP:" + session.RemoteEndPoint.Address.ToString() + " blocked\r\n");
                        }

                        errorText = dr["DefaultRejectionText"].ToString();
                        // Server provided return text, use it
                        if(dnsResponse.GetTXTRecords().Length > 0){
                            errorText = dnsResponse.GetTXTRecords()[0].Text;
                        }
                        if(errorText == ""){
                            errorText = "You are in '" + dr["Server"].ToString() + "' rejection list !";
                        }

						return false;
					}
                }

                #endregion                
            }
			catch{
			}

			return ok;
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


        #region method IsAstericMatch

        /// <summary>
		/// Checks if text matches to search pattern.
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private bool IsAstericMatch(string pattern,string text)
		{
            pattern = pattern.ToLower();
			text = text.ToLower();

			if(pattern == ""){
				pattern = "*";
			}

			while(pattern.Length > 0){
				// *xxx[*xxx...]
				if(pattern.StartsWith("*")){
					// *xxx*xxx
					if(pattern.IndexOf("*",1) > -1){
						string indexOfPart = pattern.Substring(1,pattern.IndexOf("*",1) - 1);
						if(text.IndexOf(indexOfPart) == -1){
							return false;
						}

                        text = text.Substring(text.IndexOf(indexOfPart) + indexOfPart.Length);
                        pattern = pattern.Substring(pattern.IndexOf("*", 1));
					}
					// *xxx   This is last pattern	
					else{				
						return text.EndsWith(pattern.Substring(1));
					}
				}
				// xxx*[xxx...]
				else if(pattern.IndexOfAny(new char[]{'*'}) > -1){
					string startPart = pattern.Substring(0,pattern.IndexOfAny(new char[]{'*'}));
		
					// Text must startwith
					if(!text.StartsWith(startPart)){
						return false;
					}

					text = text.Substring(text.IndexOf(startPart) + startPart.Length);
					pattern = pattern.Substring(pattern.IndexOfAny(new char[]{'*'}));
				}
				// xxx
				else{
					return text == pattern;
				}
			}

            return true;
		}

		#endregion

        #region method ReverseIP

        /// <summary>
        /// Reverses IP address blocks and converts to string. For example: 192.168.1.1 will be 1.1.168.192.
        /// </summary>
        /// <param name="ip">IP address to reverse.</param>
        /// <returns></returns>
        private string ReverseIP(IPAddress ip)
		{			
			byte[] ipBlocks = ip.GetAddressBytes();

			return ipBlocks[3].ToString() + "." + ipBlocks[2].ToString() + "." + ipBlocks[1].ToString() + "." + ipBlocks[0].ToString();
        }

        #endregion

        #region method IsPrivateIP

		/// <summary>
		/// Gets if sepcified IP is private IP address. For example 192.168.x.x is private ip.
		/// </summary>
		/// <param name="ip">IP address to check.</param>
		/// <returns>Returns true if IP address is private IP.</returns>
		private bool IsPrivateIP(IPAddress ip)
		{			
			if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork){
				byte[] ipBytes = ip.GetAddressBytes();

				/* Private IPs:
					First Octet = 192 AND Second Octet = 168 (Example: 192.168.X.X) 
					First Octet = 172 AND (Second Octet >= 16 AND Second Octet <= 31) (Example: 172.16.X.X - 172.31.X.X)
					First Octet = 10 (Example: 10.X.X.X)
					First Octet = 169 AND Second Octet = 254 (Example: 169.254.X.X)

				*/

				if(ipBytes[0] == 192 && ipBytes[1] == 168){
					return true;
				}
				if(ipBytes[0] == 172 && ipBytes[1] >= 16 && ipBytes[1] <= 31){
					return true;
				}
				if(ipBytes[0] == 10){
					return true;
				}
				if(ipBytes[0] == 169 && ipBytes[1] == 254){
					return true;
				}
			}

			return false;
		}

		#endregion

        #region method WriteFilterLog

        /// <summary>
        /// Logs specified text to log file.
        /// </summary>
        /// <param name="text">Text to log.</param>
        private void WriteFilterLog(string text)
		{
			try{
				using(FileStream fs = new FileStream(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\lsDNSBL_Filter_block.log",FileMode.OpenOrCreate)){
					fs.Seek(0,SeekOrigin.End);
					byte[] data = System.Text.Encoding.ASCII.GetBytes(text);
					fs.Write(data,0,data.Length);					
				}
			}
			catch{
			}
        }

        #endregion
    }
}
