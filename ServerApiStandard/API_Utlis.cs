using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Provides API implementation helper methods.
    /// </summary>
    public class API_Utlis
    {
        #region static method NormalizeFolder

        /// <summary>
        /// Normalizes folder value. Replaces \ to /, removes duplicate //, removes / from folder start and end.
        /// </summary>
        /// <param name="folder">Folder to normalize.</param>
        /// <returns></returns>
        public static string NormalizeFolder(string folder)
        {
            // API uses only / as path separator.
            folder = folder.Replace("\\","/");

            // Remove // duplicate continuos path separators, if any.
            while(folder.IndexOf("//") > -1){
                folder = folder.Replace("//","/");
            }

            // Remove from folder start, if any.
            if(folder.StartsWith("/")){
                folder = folder.Substring(1);
            }

            // Remove from folder end, if any.
            if(folder.EndsWith("/")){
                folder = folder.Substring(0,folder.Length - 1);
            }

            return folder;
        }

        #endregion

        #region static method PathFix

        /// <summary>
        /// Fixes path separator, replaces / \ with platform separator char.
        /// </summary>
        /// <param name="path">Path to fix.</param>
        /// <returns></returns>
        public static string PathFix(string path)
        {
            return path.Replace('\\',Path.DirectorySeparatorChar).Replace('/',Path.DirectorySeparatorChar);
        }

        #endregion

        #region static method DirectoryExists

        /// <summary>
        /// Checks if directory exists. If linux, checks with case-insenstively (linux is case-sensitive). 
        /// Returns actual dir (In linux it may differ from requested directory, because of case-sensitivity.)
        /// or null if directory doesn't exist.
        /// </summary>
        /// <param name="dirName">Directory to check.</param>
        /// <returns></returns>
        public static string DirectoryExists(string dirName)
		{
			// Windows we can use Directory.Exists
			if(Environment.OSVersion.Platform.ToString().ToLower().IndexOf("win") > -1){
				if(Directory.Exists(dirName)){
					return dirName;
				}
			}
			// Unix,Linux we can't trust Directory.Exists value because of case-sensitive file system
			else{
				if(Directory.Exists(dirName)){
					return dirName;
				}
				else{
                    // Remove / if path starts with /.
                    if(dirName.StartsWith("/")){
                        dirName = dirName.Substring(1);
                    }
                    // Remove / if path ends with /.
                    //if(dirName.EndsWith("/")){
                    //    dirName = dirName.Substring(0,dirName.Length - 1);
                    //}

					string[] pathParts   = dirName.Split('/');
					string   currentPath = "/";
				
					// See if dirs path is valid
					for(int i=0;i<pathParts.Length;i++){
						bool dirExists = false;
						string[] dirs = Directory.GetDirectories(currentPath);
						foreach(string dir in dirs){
							string[] dirParts = dir.Split('/');
							if(pathParts[i].ToLower() == dirParts[dirParts.Length - 1].ToLower()){
								currentPath = dir;
								dirExists = true;
								break;
							}
						}				
						if(!dirExists){
							return null;
						}				
					}
					
					return currentPath;
				}
			}
			
			return null;
        }

        #endregion

        #region static method EnsureDirectory

        /// <summary>
        /// Ensures that specified folder exists, if not it will be created.
        /// Returns actual dir (In linux it may differ from requested directory, because of case-sensitivity.).
        /// </summary>
        /// <param name="folder">Folder name with path.</param>
        public static string EnsureFolder(string folder)
        {
            string normalizedFolder = DirectoryExists(folder);
            if(normalizedFolder == null){
                Directory.CreateDirectory(folder);

                return folder;
            }
            else{
                return normalizedFolder;
            }
        }

        #endregion

        #region static method FileExists

        /// <summary>
        /// Checks if file exists. If linux, checks with case-insenstively (linux is case-sensitive). 
        /// Returns actual file (In linux it may differ from requested file, because of case-sensitivity.)
        /// or null if file doesn't exist.
        /// </summary>
        /// <param name="fileName">File to check.</param>
        /// <returns></returns>
		public static string FileExists(string fileName)
		{
			// Windows we can use File.Exists
			if(Environment.OSVersion.Platform.ToString().ToLower().IndexOf("win") > -1){
				if(File.Exists(fileName)){
					return fileName;
				}
			}
			// Unix,Linux we can't trust File.Exists value because of case-sensitive file system
			else{
				if(File.Exists(fileName)){
					return fileName;
				}
				else{
                    // Remove / if path starts with /.
                    if(fileName.StartsWith("/")){
                        fileName = fileName.Substring(1);
                    }

					string[] pathParts   = fileName.Split('/');
					string   currentPath = "/";
					
					// See if dirs path is valid
					for(int i=0;i<pathParts.Length - 1;i++){
						bool dirExists = false;
						string[] dirs = Directory.GetDirectories(currentPath);
						foreach(string dir in dirs){
							string[] dirParts = dir.Split('/');
							if(pathParts[i].ToLower() == dirParts[dirParts.Length - 1].ToLower()){
								currentPath = dir;
								dirExists = true;
								break;
							}
						}				
						if(!dirExists){
							return null;
						}				
					}
					
					// Check that file exists
					string[] files = Directory.GetFiles(currentPath);
					foreach(string file in files){
						if(pathParts[pathParts.Length - 1].ToLower() == Path.GetFileName(file).ToLower()){
							return file;
						}			
					}
				}
			}
			
			return null;
        }

        #endregion

        #region static method StreamCopy

        /// <summary>
        /// Copies all data from source stream to destination stream.
        /// Copy starts from source stream current position and will be copied to the end of source stream.
        /// </summary>
        /// <param name="source">Source stream.</param>
        /// <param name="destination">Destination stream.</param>
        /// <returns>Returns number of bytes copied.</returns>
        [Obsolete("Use Net_utils.StreamCopy instead.")]
        public static long StreamCopy(Stream source,Stream destination)
        {
            byte[] buffer           = new byte[16000];
            long   totalReadedCount = 0;
            while(true){
                int readedCount = source.Read(buffer,0,buffer.Length);
                // End of stream reached.
                if(readedCount == 0){
                    return totalReadedCount;
                }

                totalReadedCount += readedCount;
                destination.Write(buffer,0,readedCount);
            }
        }

        #endregion


        #region method CreateSettingsSchema

        /// <summary>
		/// Creates system settings schema.
		/// </summary>
		/// <param name="ds">DataSet where to create schema.</param>
		public static void CreateSettingsSchema(DataSet ds)
		{
			if(!ds.Tables.Contains("Settings")){
				ds.Tables.Add("Settings");
			}

			if(!ds.Tables["Settings"].Columns.Contains("ErrorFile")){
				ds.Tables["Settings"].Columns.Add("ErrorFile").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_Threads")){
				ds.Tables["Settings"].Columns.Add("SMTP_Threads").DefaultValue = 100;
			}
			if(!ds.Tables["Settings"].Columns.Contains("POP3_Threads")){
				ds.Tables["Settings"].Columns.Add("POP3_Threads").DefaultValue = 100;
			}
			if(!ds.Tables["Settings"].Columns.Contains("SmartHost")){
				ds.Tables["Settings"].Columns.Add("SmartHost").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SmartHostPort")){
				ds.Tables["Settings"].Columns.Add("SmartHostPort",typeof(int)).DefaultValue = 25;
			}
			if(!ds.Tables["Settings"].Columns.Contains("UseSmartHost")){
				ds.Tables["Settings"].Columns.Add("UseSmartHost").DefaultValue = false;
			}
			if(!ds.Tables["Settings"].Columns.Contains("Dns1")){
				ds.Tables["Settings"].Columns.Add("Dns1").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("Dns2")){
				ds.Tables["Settings"].Columns.Add("Dns2").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("LogServer")){
				ds.Tables["Settings"].Columns.Add("LogServer").DefaultValue = false;
			}
			if(!ds.Tables["Settings"].Columns.Contains("LogSMTPCmds")){
				ds.Tables["Settings"].Columns.Add("LogSMTPCmds").DefaultValue = false;
			}
			if(!ds.Tables["Settings"].Columns.Contains("LogPOP3Cmds")){
				ds.Tables["Settings"].Columns.Add("LogPOP3Cmds").DefaultValue = false;
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_SessionIdleTimeOut")){
				ds.Tables["Settings"].Columns.Add("SMTP_SessionIdleTimeOut").DefaultValue = 30;
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_CommandIdleTimeOut")){
				ds.Tables["Settings"].Columns.Add("SMTP_CommandIdleTimeOut").DefaultValue = 30;
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_MaxBadCommands")){
				ds.Tables["Settings"].Columns.Add("SMTP_MaxBadCommands").DefaultValue = 8;
			}
			if(!ds.Tables["Settings"].Columns.Contains("POP3_SessionIdleTimeOut")){
				ds.Tables["Settings"].Columns.Add("POP3_SessionIdleTimeOut").DefaultValue = 30;
			}
			if(!ds.Tables["Settings"].Columns.Contains("POP3_CommandIdleTimeOut")){
				ds.Tables["Settings"].Columns.Add("POP3_CommandIdleTimeOut").DefaultValue = 30;
			}
			if(!ds.Tables["Settings"].Columns.Contains("POP3_MaxBadCommands")){
				ds.Tables["Settings"].Columns.Add("POP3_MaxBadCommands").DefaultValue = 8;
			}
			if(!ds.Tables["Settings"].Columns.Contains("MaxMessageSize")){
				ds.Tables["Settings"].Columns.Add("MaxMessageSize").DefaultValue = 1000;
			}
			if(!ds.Tables["Settings"].Columns.Contains("MaxRecipients")){
				ds.Tables["Settings"].Columns.Add("MaxRecipients").DefaultValue = 100;
			}
			if(!ds.Tables["Settings"].Columns.Contains("MaxRelayThreads")){
				ds.Tables["Settings"].Columns.Add("MaxRelayThreads").DefaultValue = 100;
			}
			if(!ds.Tables["Settings"].Columns.Contains("RelayInterval")){
				ds.Tables["Settings"].Columns.Add("RelayInterval").DefaultValue = 15;
			}
			if(!ds.Tables["Settings"].Columns.Contains("RelayRetryInterval")){
				ds.Tables["Settings"].Columns.Add("RelayRetryInterval").DefaultValue = 300;
			}
			if(!ds.Tables["Settings"].Columns.Contains("RelayUndeliveredWarning")){
				ds.Tables["Settings"].Columns.Add("RelayUndeliveredWarning").DefaultValue = 300;
			}
			if(!ds.Tables["Settings"].Columns.Contains("RelayUndelivered")){
				ds.Tables["Settings"].Columns.Add("RelayUndelivered").DefaultValue = 1;
			}
			if(!ds.Tables["Settings"].Columns.Contains("StoreUndeliveredMessages")){
				ds.Tables["Settings"].Columns.Add("StoreUndeliveredMessages").DefaultValue = false;
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_LogPath")){
				ds.Tables["Settings"].Columns.Add("SMTP_LogPath").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("POP3_LogPath")){
				ds.Tables["Settings"].Columns.Add("POP3_LogPath").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("Server_LogPath")){
				ds.Tables["Settings"].Columns.Add("Server_LogPath").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("IMAP_LogPath")){
				ds.Tables["Settings"].Columns.Add("IMAP_LogPath").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("IMAP_SessionIdleTimeOut")){
				ds.Tables["Settings"].Columns.Add("IMAP_SessionIdleTimeOut").DefaultValue = 800;
			}
			if(!ds.Tables["Settings"].Columns.Contains("IMAP_CommandIdleTimeOut")){
				ds.Tables["Settings"].Columns.Add("IMAP_CommandIdleTimeOut").DefaultValue = 60;
			}
			if(!ds.Tables["Settings"].Columns.Contains("IMAP_MaxBadCommands")){
				ds.Tables["Settings"].Columns.Add("IMAP_MaxBadCommands").DefaultValue = 10;
			}
			if(!ds.Tables["Settings"].Columns.Contains("LogIMAPCmds")){
				ds.Tables["Settings"].Columns.Add("LogIMAPCmds").DefaultValue = "false";
			}
			if(!ds.Tables["Settings"].Columns.Contains("IMAP_Threads")){
				ds.Tables["Settings"].Columns.Add("IMAP_Threads").DefaultValue = 100;
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_Enabled")){
				ds.Tables["Settings"].Columns.Add("SMTP_Enabled").DefaultValue = "false";
			}
			if(!ds.Tables["Settings"].Columns.Contains("POP3_Enabled")){
				ds.Tables["Settings"].Columns.Add("POP3_Enabled").DefaultValue = "false";
			}
			if(!ds.Tables["Settings"].Columns.Contains("IMAP_Enabled")){
				ds.Tables["Settings"].Columns.Add("IMAP_Enabled").DefaultValue = "false";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_DefaultDomain")){
				ds.Tables["Settings"].Columns.Add("SMTP_DefaultDomain").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_MaxTransactions")){
				ds.Tables["Settings"].Columns.Add("SMTP_MaxTransactions").DefaultValue = "0";
			}
			if(!ds.Tables["Settings"].Columns.Contains("RelayStore")){
				ds.Tables["Settings"].Columns.Add("RelayStore").DefaultValue = "c:\\MailStore\\";
			}
			if(!ds.Tables["Settings"].Columns.Contains("LogRelayCmds")){
				ds.Tables["Settings"].Columns.Add("LogRelayCmds").DefaultValue = false;
			}
			if(!ds.Tables["Settings"].Columns.Contains("Relay_LogPath")){
				ds.Tables["Settings"].Columns.Add("Relay_LogPath").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SmartHostUserName")){
				ds.Tables["Settings"].Columns.Add("SmartHostUserName").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SmartHostPassword")){
				ds.Tables["Settings"].Columns.Add("SmartHostPassword").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("RelayLocalIP")){
				ds.Tables["Settings"].Columns.Add("RelayLocalIP").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("LogFetchPOP3Cmds")){
				ds.Tables["Settings"].Columns.Add("LogFetchPOP3Cmds").DefaultValue = "false";
			}
			if(!ds.Tables["Settings"].Columns.Contains("FetchPOP3_LogPath")){
				ds.Tables["Settings"].Columns.Add("FetchPOP3_LogPath").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_RequireAuth")){
				ds.Tables["Settings"].Columns.Add("SMTP_RequireAuth").DefaultValue = "false";
            }
            if(!ds.Tables["Settings"].Columns.Contains("Relay_SmartHost_UseSSL")){
				ds.Tables["Settings"].Columns.Add("Relay_SmartHost_UseSSL").DefaultValue = "false";
            }
            if(!ds.Tables["Settings"].Columns.Contains("FetchPOP3_Interval")){
				ds.Tables["Settings"].Columns.Add("FetchPOP3_Interval").DefaultValue = 300;
            }
            if(!ds.Tables["Settings"].Columns.Contains("SMTP_GreetingText")){
				ds.Tables["Settings"].Columns.Add("SMTP_GreetingText").DefaultValue = "";
            }
            if(!ds.Tables["Settings"].Columns.Contains("POP3_GreetingText")){
				ds.Tables["Settings"].Columns.Add("POP3_GreetingText").DefaultValue = "";
            }
            if(!ds.Tables["Settings"].Columns.Contains("IMAP_GreetingText")){
				ds.Tables["Settings"].Columns.Add("IMAP_GreetingText").DefaultValue = "";
            }
            if(!ds.Tables["Settings"].Columns.Contains("ServerAuthenticationType")){
				ds.Tables["Settings"].Columns.Add("ServerAuthenticationType").DefaultValue = 1;
            }
            if(!ds.Tables["Settings"].Columns.Contains("ServerAuthWinDomain")){
				ds.Tables["Settings"].Columns.Add("ServerAuthWinDomain").DefaultValue = "";
            }
            if(!ds.Tables["Settings"].Columns.Contains("FetchPop3_Enabled")){
				ds.Tables["Settings"].Columns.Add("FetchPop3_Enabled").DefaultValue = "true";
            }
            if(!ds.Tables["Settings"].Columns.Contains("Relay_HostName")){
				ds.Tables["Settings"].Columns.Add("Relay_HostName").DefaultValue = "";
            }
            if(!ds.Tables["Settings"].Columns.Contains("Relay_SessionIdleTimeOut")){
				ds.Tables["Settings"].Columns.Add("Relay_SessionIdleTimeOut").DefaultValue = "30";
            }
            if(!ds.Tables["Settings"].Columns.Contains("SMTP_MaxConnectionsPerIP")){
				ds.Tables["Settings"].Columns.Add("SMTP_MaxConnectionsPerIP").DefaultValue = "0";
            }
            if(!ds.Tables["Settings"].Columns.Contains("POP3_MaxConnectionsPerIP")){
				ds.Tables["Settings"].Columns.Add("POP3_MaxConnectionsPerIP").DefaultValue = "0";
            }
            if(!ds.Tables["Settings"].Columns.Contains("IMAP_MaxConnectionsPerIP")){
				ds.Tables["Settings"].Columns.Add("IMAP_MaxConnectionsPerIP").DefaultValue = "0";
            }
            if(!ds.Tables["Settings"].Columns.Contains("SmartHostUseSSL")){
				ds.Tables["Settings"].Columns.Add("SmartHostUseSSL").DefaultValue = false;
			}
            if(!ds.Tables["Settings"].Columns.Contains("SIP_Enabled")){
				ds.Tables["Settings"].Columns.Add("SIP_Enabled").DefaultValue = false;
			}
            if(!ds.Tables["Settings"].Columns.Contains("Relay_MaxConnectionsPerIP")){
				ds.Tables["Settings"].Columns.Add("Relay_MaxConnectionsPerIP").DefaultValue = "10";
            }
            if(!ds.Tables["Settings"].Columns.Contains("SIP_MinExpires")){
				ds.Tables["Settings"].Columns.Add("SIP_MinExpires").DefaultValue = 60;
            }
            if(!ds.Tables["Settings"].Columns.Contains("Relay_Mode")){
				ds.Tables["Settings"].Columns.Add("Relay_Mode").DefaultValue = "Dns";
            }
            if(!ds.Tables["Settings"].Columns.Contains("Relay_SmartHostsBalanceMode")){
				ds.Tables["Settings"].Columns.Add("Relay_SmartHostsBalanceMode").DefaultValue = "LoadBalance";
            }            
            if(!ds.Tables["Settings"].Columns.Contains("SIP_ProxyMode")){
				ds.Tables["Settings"].Columns.Add("SIP_ProxyMode").DefaultValue = "Registrar,B2BUA";
            }           
            if(!ds.Tables["Settings"].Columns.Contains("LdapServer")){
				ds.Tables["Settings"].Columns.Add("LdapServer").DefaultValue = "";
            }          
            if(!ds.Tables["Settings"].Columns.Contains("LdapDN")){
				ds.Tables["Settings"].Columns.Add("LdapDN").DefaultValue = "";
            }                      
            if(!ds.Tables["Settings"].Columns.Contains("SettingsDate")){
				ds.Tables["Settings"].Columns.Add("SettingsDate",typeof(DateTime)).DefaultValue = DateTime.Now;
            }            
            if(!ds.Tables["Settings"].Columns.Contains("Relay_UseTlsIfPossible")){
				ds.Tables["Settings"].Columns.Add("Relay_UseTlsIfPossible").DefaultValue = false;
            }

            #region table DnsServers

            if(!ds.Tables.Contains("DnsServers")){
                ds.Tables.Add("DnsServers");
            }
            if(!ds.Tables["DnsServers"].Columns.Contains("IP")){
                ds.Tables["DnsServers"].Columns.Add("IP").DefaultValue = "";
            }

            #endregion

            #region table SMTP_Bindings

            if(!ds.Tables.Contains("SMTP_Bindings")){
                ds.Tables.Add("SMTP_Bindings");
            }
            if(!ds.Tables["SMTP_Bindings"].Columns.Contains("HostName")){
                ds.Tables["SMTP_Bindings"].Columns.Add("HostName").DefaultValue = "";
            }
            if(!ds.Tables["SMTP_Bindings"].Columns.Contains("Protocol")){
                ds.Tables["SMTP_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
            }
            if(!ds.Tables["SMTP_Bindings"].Columns.Contains("IP")){
                ds.Tables["SMTP_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
            }
            if(!ds.Tables["SMTP_Bindings"].Columns.Contains("Port")){
                ds.Tables["SMTP_Bindings"].Columns.Add("Port").DefaultValue = "25";
            }
            if(!ds.Tables["SMTP_Bindings"].Columns.Contains("SSL")){
                ds.Tables["SMTP_Bindings"].Columns.Add("SSL").DefaultValue = "None";
            }
            if(!ds.Tables["SMTP_Bindings"].Columns.Contains("SSL_Certificate")){
                ds.Tables["SMTP_Bindings"].Columns.Add("SSL_Certificate",typeof(byte[]));
            }

            #endregion

            #region table Relay_Bindings

            if(!ds.Tables.Contains("Relay_Bindings")){
                ds.Tables.Add("Relay_Bindings");
            }
            if(!ds.Tables["Relay_Bindings"].Columns.Contains("HostName")){
                ds.Tables["Relay_Bindings"].Columns.Add("HostName").DefaultValue = "";
            }
            if(!ds.Tables["Relay_Bindings"].Columns.Contains("Protocol")){
                ds.Tables["Relay_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
            }
            if(!ds.Tables["Relay_Bindings"].Columns.Contains("IP")){
                ds.Tables["Relay_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
            }
            if(!ds.Tables["Relay_Bindings"].Columns.Contains("Port")){
                ds.Tables["Relay_Bindings"].Columns.Add("Port").DefaultValue = "25";
            }
            if(!ds.Tables["Relay_Bindings"].Columns.Contains("SSL")){
                ds.Tables["Relay_Bindings"].Columns.Add("SSL",typeof(bool)).DefaultValue = false;
            }
            if(!ds.Tables["Relay_Bindings"].Columns.Contains("SSL_Certificate")){
                ds.Tables["Relay_Bindings"].Columns.Add("SSL_Certificate",typeof(byte[]));
            }

            #endregion

            #region table Relay_SmartHosts

            if(!ds.Tables.Contains("Relay_SmartHosts")){
                ds.Tables.Add("Relay_SmartHosts");
            }
            if(!ds.Tables["Relay_SmartHosts"].Columns.Contains("Host")){
                ds.Tables["Relay_SmartHosts"].Columns.Add("Host").DefaultValue = "";
            }
            if(!ds.Tables["Relay_SmartHosts"].Columns.Contains("Port")){
                ds.Tables["Relay_SmartHosts"].Columns.Add("Port").DefaultValue = "25";
            }
            if(!ds.Tables["Relay_SmartHosts"].Columns.Contains("SslMode")){
                ds.Tables["Relay_SmartHosts"].Columns.Add("SslMode").DefaultValue = "None";
            }
            if(!ds.Tables["Relay_SmartHosts"].Columns.Contains("UserName")){
                ds.Tables["Relay_SmartHosts"].Columns.Add("UserName").DefaultValue = "";
            }
            if(!ds.Tables["Relay_SmartHosts"].Columns.Contains("Password")){
                ds.Tables["Relay_SmartHosts"].Columns.Add("Password").DefaultValue = "";
            }

            #endregion

            #region table POP3_Bindings

            if(!ds.Tables.Contains("POP3_Bindings")){
                ds.Tables.Add("POP3_Bindings");
            }
            if(!ds.Tables["POP3_Bindings"].Columns.Contains("HostName")){
                ds.Tables["POP3_Bindings"].Columns.Add("HostName").DefaultValue = "";
            }
            if(!ds.Tables["POP3_Bindings"].Columns.Contains("Protocol")){
                ds.Tables["POP3_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
            }
            if(!ds.Tables["POP3_Bindings"].Columns.Contains("IP")){
                ds.Tables["POP3_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
            }
            if(!ds.Tables["POP3_Bindings"].Columns.Contains("Port")){
                ds.Tables["POP3_Bindings"].Columns.Add("Port").DefaultValue = "110";
            }
            if(!ds.Tables["POP3_Bindings"].Columns.Contains("SSL")){
                ds.Tables["POP3_Bindings"].Columns.Add("SSL").DefaultValue = "None";
            }
            if(!ds.Tables["POP3_Bindings"].Columns.Contains("SSL_Certificate")){
                ds.Tables["POP3_Bindings"].Columns.Add("SSL_Certificate",typeof(byte[]));
            }

            #endregion

            #region table IMAP_Bindings

            if(!ds.Tables.Contains("IMAP_Bindings")){
                ds.Tables.Add("IMAP_Bindings");
            }
            if(!ds.Tables["IMAP_Bindings"].Columns.Contains("HostName")){
                ds.Tables["IMAP_Bindings"].Columns.Add("HostName").DefaultValue = "";
            }
            if(!ds.Tables["IMAP_Bindings"].Columns.Contains("Protocol")){
                ds.Tables["IMAP_Bindings"].Columns.Add("Protocol").DefaultValue = "TCP";
            }
            if(!ds.Tables["IMAP_Bindings"].Columns.Contains("IP")){
                ds.Tables["IMAP_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
            }
            if(!ds.Tables["IMAP_Bindings"].Columns.Contains("Port")){
                ds.Tables["IMAP_Bindings"].Columns.Add("Port").DefaultValue = "143";
            }
            if(!ds.Tables["IMAP_Bindings"].Columns.Contains("SSL")){
                ds.Tables["IMAP_Bindings"].Columns.Add("SSL").DefaultValue = "None";
            }
            if(!ds.Tables["IMAP_Bindings"].Columns.Contains("SSL_Certificate")){
                ds.Tables["IMAP_Bindings"].Columns.Add("SSL_Certificate",typeof(byte[]));
            }

            #endregion

            #region table SIP_Bindings

            if(!ds.Tables.Contains("SIP_Bindings")){
                ds.Tables.Add("SIP_Bindings");
            }
            if(!ds.Tables["SIP_Bindings"].Columns.Contains("HostName")){
                ds.Tables["SIP_Bindings"].Columns.Add("HostName").DefaultValue = "";
            }
            if(!ds.Tables["SIP_Bindings"].Columns.Contains("Protocol")){
                ds.Tables["SIP_Bindings"].Columns.Add("Protocol").DefaultValue = "UDP";
            }
            if(!ds.Tables["SIP_Bindings"].Columns.Contains("IP")){
                ds.Tables["SIP_Bindings"].Columns.Add("IP").DefaultValue = "0.0.0.0";
            }
            if(!ds.Tables["SIP_Bindings"].Columns.Contains("Port")){
                ds.Tables["SIP_Bindings"].Columns.Add("Port").DefaultValue = "5060";
            }
            if(!ds.Tables["SIP_Bindings"].Columns.Contains("SSL")){
                ds.Tables["SIP_Bindings"].Columns.Add("SSL").DefaultValue = "None";
            }
            if(!ds.Tables["SIP_Bindings"].Columns.Contains("SSL_Certificate")){
                ds.Tables["SIP_Bindings"].Columns.Add("SSL_Certificate",typeof(byte[]));
            }

            if(!ds.Tables.Contains("SIP_Gateways")){
                ds.Tables.Add("SIP_Gateways");
            }
            if(!ds.Tables["SIP_Gateways"].Columns.Contains("UriScheme")){
                ds.Tables["SIP_Gateways"].Columns.Add("UriScheme");
            }
            if(!ds.Tables["SIP_Gateways"].Columns.Contains("Transport")){
                ds.Tables["SIP_Gateways"].Columns.Add("Transport");
            }
            if(!ds.Tables["SIP_Gateways"].Columns.Contains("Host")){
                ds.Tables["SIP_Gateways"].Columns.Add("Host");
            }
            if(!ds.Tables["SIP_Gateways"].Columns.Contains("Port")){
                ds.Tables["SIP_Gateways"].Columns.Add("Port");
            }
            if(!ds.Tables["SIP_Gateways"].Columns.Contains("Realm")){
                ds.Tables["SIP_Gateways"].Columns.Add("Realm");
            }
            if(!ds.Tables["SIP_Gateways"].Columns.Contains("UserName")){
                ds.Tables["SIP_Gateways"].Columns.Add("UserName");
            }
            if(!ds.Tables["SIP_Gateways"].Columns.Contains("Password")){
                ds.Tables["SIP_Gateways"].Columns.Add("Password");
            }

            #endregion

            #region table ServerReturnMessages

            if(!ds.Tables.Contains("ServerReturnMessages")){
                ds.Tables.Add("ServerReturnMessages");
            }
            if(!ds.Tables["ServerReturnMessages"].Columns.Contains("MessageType")){
                ds.Tables["ServerReturnMessages"].Columns.Add("MessageType");
            }
            if(!ds.Tables["ServerReturnMessages"].Columns.Contains("Subject")){
                ds.Tables["ServerReturnMessages"].Columns.Add("Subject");
            }
            if(!ds.Tables["ServerReturnMessages"].Columns.Contains("BodyTextRtf")){
                ds.Tables["ServerReturnMessages"].Columns.Add("BodyTextRtf");
            }

            #endregion

        }

		#endregion

        #region method GenerateBadMessage

        /// <summary>
        /// Generates message parsing failed message.
        /// </summary>
        /// <param name="message">Message stream.</param>
        /// <returns>Returns message parsing failed message.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>message</b> is null reference.</exception>
        public static Mail_Message GenerateBadMessage(Stream message)
        {
            if(message == null){
                throw new ArgumentNullException("message");
            }

            Mail_Message msg = new Mail_Message();
            msg.MimeVersion = "1.0";
            msg.MessageID = MIME_Utils.CreateMessageID();
            msg.Date = DateTime.Now;
            msg.From = new Mail_t_MailboxList();
            msg.From.Add(new Mail_t_Mailbox("system","system"));
            msg.To = new Mail_t_AddressList();
            msg.To.Add(new Mail_t_Mailbox("system","system"));
            msg.Subject = "[BAD MESSAGE] Bad message, message parsing failed !";

            //--- multipart/mixed -------------------------------------------------------------------------------------------------
            MIME_h_ContentType contentType_multipartMixed = new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed);
            contentType_multipartMixed.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
            MIME_b_MultipartMixed multipartMixed = new MIME_b_MultipartMixed(contentType_multipartMixed);
            msg.Body = multipartMixed;

                //--- text/plain ---------------------------------------------------------------------------------------------------
                MIME_Entity entity_text_plain = new MIME_Entity();
                MIME_b_Text text_plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
                entity_text_plain.Body = text_plain;
                text_plain.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,"NOTE: Bad message, message parsing failed.\r\n\r\nOriginal message attached as 'data.eml'\r\n");
                multipartMixed.BodyParts.Add(entity_text_plain);

                //--- application/octet-stream --------------------------------------------------------------------------------------
                MIME_Entity entity_application_octet_stream = new MIME_Entity();
                entity_application_octet_stream.ContentDisposition = new MIME_h_ContentDisposition("attachment");
                entity_application_octet_stream.ContentDisposition.Param_FileName = "data.eml";
                MIME_b_Application application_octet_stream = new MIME_b_Application(MIME_MediaTypes.Application.octet_stream);
                entity_application_octet_stream.Body = application_octet_stream;
                application_octet_stream.SetData(message,"base64");
                multipartMixed.BodyParts.Add(entity_application_octet_stream);

            return msg;
        }

        #endregion
    }
}
