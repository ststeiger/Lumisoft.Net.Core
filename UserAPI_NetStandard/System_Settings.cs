using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using LumiSoft.Net;
using LumiSoft.Net.SMTP.Relay;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The System_Settings object represents System settings in LumiSoft Mail Server virtual server.
    /// </summary>
    public class System_Settings
    {
        private VirtualServer          m_pVirtualServer  = null;
        private IPAddress[]            m_pDnsServers     = null;
        private Auth_Settings          m_pAuth           = null;
        private SMTP_Settings          m_pSMTP           = null;
        private POP3_Settings          m_pPOP3           = null;
        private IMAP_Settings          m_pIMAP           = null;
        private Relay_Settings         m_pRelay          = null;
        private FetchMessages_Settings m_pFetchMessages  = null;
        private SIP_Settings           m_pSIP            = null;
        private Logging_Settings       m_pLogging        = null;
        private ServerReturnMessages   m_pReturnMessages = null;
        private bool                   m_ValuesChanged   = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal System_Settings(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            Bind();
        }

        
        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't changed, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }
                        
            MemoryStream m = new MemoryStream();            
            this.ToDataSet().WriteXml(m);
            byte[] settingsData = m.ToArray();

            /* UpdateSettings <virtualServerID> <dataLength><CRLF>
                <data>  -> data length of dataLength
             
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            // Call TCP UpdateSettings
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateSettings " + 
                m_pVirtualServer.VirtualServerID + " " + 
                settingsData.Length.ToString()
            );
            m_pVirtualServer.Server.TcpClient.TcpStream.Write(settingsData,0,settingsData.Length);
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion

        #region method LoadSettings

        /// <summary>
        /// Loads settings from DataSet.
        /// </summary>
        /// <param name="dsSettings">DataSet what contains settings.</param>
        internal void LoadSettings(DataSet dsSettings)
        {
            DataSet ds = new DataSet();
            CreateSettingsSchema(ds);
            ds.Clear();
            MemoryStream ms = new MemoryStream();
            dsSettings.WriteXml(ms);            
            ms.Position = 0;
            ds.ReadXml(ms);

            DataRow dr = ds.Tables["Settings"].Rows[0];
            //--- General -------------------------------------
            List<IPAddress> dnsServers = new List<IPAddress>();
            foreach(DataRow drX in ds.Tables["DnsServers"].Rows){
                dnsServers.Add(IPAddress.Parse(drX["IP"].ToString()));
            }
            m_pDnsServers = dnsServers.ToArray();

            //--- AUTH -------------------------------------------//
            m_pAuth.AuthenticationType = (ServerAuthenticationType_enum)Convert.ToInt32(dr["ServerAuthenticationType"]);
            m_pAuth.WinDomain          = dr["ServerAuthWinDomain"].ToString();
            m_pAuth.LdapServer         = dr["LdapServer"].ToString();
            m_pAuth.LdapDn             = dr["LdapDN"].ToString();
            
            //--- SMTP -------------------------------------------//
            // Get bindings
            IPBindInfo[] smtpBinds = new IPBindInfo[ds.Tables["SMTP_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["SMTP_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["SMTP_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["SMTP_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                smtpBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pSMTP.Enabled                     = Convert.ToBoolean(dr["SMTP_Enabled"]);
            m_pSMTP.GreetingText                = dr["SMTP_GreetingText"].ToString();
            m_pSMTP.DefaultDomain               = dr["SMTP_DefaultDomain"].ToString();
            m_pSMTP.SessionIdleTimeOut          = Convert.ToInt32(dr["SMTP_SessionIdleTimeOut"]);
            m_pSMTP.MaximumConnections          = Convert.ToInt32(dr["SMTP_Threads"]);
            m_pSMTP.MaximumConnectionsPerIP     = Convert.ToInt32(dr["SMTP_MaxConnectionsPerIP"]);
            m_pSMTP.MaximumBadCommands          = Convert.ToInt32(dr["SMTP_MaxBadCommands"]);
            m_pSMTP.MaximumRecipientsPerMessage = Convert.ToInt32(dr["MaxRecipients"]);
            m_pSMTP.MaximumMessageSize          = Convert.ToInt32(dr["MaxMessageSize"]);
            m_pSMTP.RequireAuthentication       = Convert.ToBoolean(dr["SMTP_RequireAuth"]);
            m_pSMTP.Binds                       = smtpBinds;
                      
            //--- POP3 ------------------------------------------//
            // Get bindings
            IPBindInfo[] pop3Binds = new IPBindInfo[ds.Tables["POP3_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["POP3_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["POP3_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["POP3_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                pop3Binds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pPOP3.Enabled                 = Convert.ToBoolean(dr["POP3_Enabled"]);
            m_pPOP3.GreetingText            = dr["POP3_GreetingText"].ToString();
            m_pPOP3.SessionIdleTimeOut      = Convert.ToInt32(dr["POP3_SessionIdleTimeOut"]);
            m_pPOP3.MaximumConnections      = Convert.ToInt32(dr["POP3_Threads"]);
            m_pPOP3.MaximumConnectionsPerIP = Convert.ToInt32(dr["POP3_MaxConnectionsPerIP"]);
            m_pPOP3.MaximumBadCommands      = Convert.ToInt32(dr["POP3_MaxBadCommands"]);
            m_pPOP3.Binds                   = pop3Binds;
            
            //--- IMAP ------------------------------------------//
            // Get bindings
            IPBindInfo[] imapBinds = new IPBindInfo[ds.Tables["IMAP_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["IMAP_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["IMAP_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["IMAP_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                imapBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pIMAP.Enabled                 = Convert.ToBoolean(dr["IMAP_Enabled"]);
            m_pIMAP.GreetingText            = dr["IMAP_GreetingText"].ToString();
            m_pIMAP.SessionIdleTimeOut      = Convert.ToInt32(dr["IMAP_SessionIdleTimeOut"]);
            m_pIMAP.MaximumConnections      = Convert.ToInt32(dr["IMAP_Threads"]);
            m_pIMAP.MaximumConnectionsPerIP = Convert.ToInt32(dr["IMAP_MaxConnectionsPerIP"]);
            m_pIMAP.MaximumBadCommands      = Convert.ToInt32(dr["IMAP_MaxBadCommands"]);
            m_pIMAP.Binds                   = imapBinds;
           
            //--- Relay ----------------------------------------//
            // Get smart hosts
            Relay_SmartHost[] relaySmartHosts = new Relay_SmartHost[ds.Tables["Relay_SmartHosts"].Rows.Count];
            for(int i=0;i<relaySmartHosts.Length;i++){
                DataRow drX = ds.Tables["Relay_SmartHosts"].Rows[i];
                relaySmartHosts[i] = new Relay_SmartHost(
                    ConvertEx.ToString(drX["Host"]),
                    ConvertEx.ToInt32(drX["Port"]),
                    (SslMode)Enum.Parse(typeof(SslMode),drX["SslMode"].ToString()),
                    ConvertEx.ToString(drX["UserName"]),
                    ConvertEx.ToString(drX["Password"])
                );
            }

            // Get bindings
            IPBindInfo[] relayBinds = new IPBindInfo[ds.Tables["Relay_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["Relay_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["Relay_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["Relay_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                relayBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pRelay.RelayMode                   = (Relay_Mode)Enum.Parse(typeof(Relay_Mode),dr["Relay_Mode"].ToString());
            m_pRelay.SmartHostsBalanceMode       = (BalanceMode)Enum.Parse(typeof(BalanceMode),dr["Relay_SmartHostsBalanceMode"].ToString());
            m_pRelay.SmartHosts                  = relaySmartHosts;
            m_pRelay.SessionIdleTimeOut          = Convert.ToInt32(dr["Relay_SessionIdleTimeOut"]);
            m_pRelay.MaximumConnections          = Convert.ToInt32(dr["MaxRelayThreads"]);
            m_pRelay.MaximumConnectionsPerIP     = Convert.ToInt32(dr["Relay_MaxConnectionsPerIP"]);
            m_pRelay.RelayInterval               = Convert.ToInt32(dr["RelayInterval"]);
            m_pRelay.RelayRetryInterval          = Convert.ToInt32(dr["RelayRetryInterval"]);
            m_pRelay.SendUndeliveredWarningAfter = Convert.ToInt32(dr["RelayUndeliveredWarning"]);
            m_pRelay.SendUndeliveredAfter        = Convert.ToInt32(dr["RelayUndelivered"]);
            m_pRelay.StoreUndeliveredMessages    = Convert.ToBoolean(dr["StoreUndeliveredMessages"]);
            m_pRelay.UseTlsIfPossible            = Convert.ToBoolean(dr["Relay_UseTlsIfPossible"]);
            m_pRelay.Binds                       = relayBinds;
            
            //--- Fetch messages ------------------------------//
            m_pFetchMessages.Enabled       = Convert.ToBoolean(dr["FetchPOP3_Enabled"]);
            m_pFetchMessages.FetchInterval = Convert.ToInt32(dr["FetchPOP3_Interval"]);

            //--- SIP ----------------------------------------//
            m_pSIP.Enabled        = Convert.ToBoolean(dr["SIP_Enabled"]);
            m_pSIP.ProxyMode      = (LumiSoft.Net.SIP.Proxy.SIP_ProxyMode)Enum.Parse(typeof(LumiSoft.Net.SIP.Proxy.SIP_ProxyMode),dr["SIP_ProxyMode"].ToString());
            m_pSIP.MinimumExpires = Convert.ToInt32(dr["SIP_MinExpires"]);

            // Get bindings
            IPBindInfo[] sipBinds = new IPBindInfo[ds.Tables["SIP_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["SIP_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["SIP_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["SIP_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                sipBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    (BindInfoProtocol)Enum.Parse(typeof(BindInfoProtocol),drBinding["Protocol"].ToString()),
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }
            m_pSIP.Binds = sipBinds;

            // Construct gateways
            SIP_GatewayCollection gateways = new SIP_GatewayCollection(this);
            foreach(DataRow drGw in ds.Tables["SIP_Gateways"].Rows){
                gateways.AddInternal(
                    drGw["UriScheme"].ToString(),
                    drGw["Transport"].ToString(),
                    drGw["Host"].ToString(),
                    Convert.ToInt32(drGw["Port"]),
                    drGw["Realm"].ToString(),
                    drGw["UserName"].ToString(),
                    drGw["Password"].ToString()
                );
            }
                       
            //--- Logging ------------------------------------//
            m_pLogging.LogSMTP               = ConvertEx.ToBoolean(dr["LogSMTPCmds"],false);
            m_pLogging.SmtpLogsPath          = dr["SMTP_LogPath"].ToString();
            m_pLogging.LogPOP3               = ConvertEx.ToBoolean(dr["LogPOP3Cmds"],false);
            m_pLogging.Pop3LogsPath          = dr["POP3_LogPath"].ToString();
            m_pLogging.LogIMAP               = ConvertEx.ToBoolean(dr["LogIMAPCmds"],false);
            m_pLogging.ImapLogsPath          = dr["IMAP_LogPath"].ToString();
            m_pLogging.LogRelay              = ConvertEx.ToBoolean(dr["LogRelayCmds"],false);
            m_pLogging.RelayLogsPath         = dr["Relay_LogPath"].ToString();
            m_pLogging.LogFetchMessages      = ConvertEx.ToBoolean(dr["LogFetchPOP3Cmds"],false);
            m_pLogging.FetchMessagesLogsPath = dr["FetchPOP3_LogPath"].ToString();

            //--- Server return messages
            string delayedNotifyBodyRtf = "" +
            "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Verdana;}{\\f1\\fnil\\fcharset186 Verdana;}{\\f2\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}{\\f3\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n" +
            "{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n" +
            "\\viewkind4\\uc1\\pard\\f0\\fs20 This e-mail is generated by the Server(\\cf1 <#relay.hostname>\\cf0 )  to notify you, \\par\r\n" +
            "\\lang1061\\f1 that \\lang1033\\f0 your message to \\cf1 <#relay.to>\\cf0  dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0  \\fs20 could not be sent at the first attempt.\\par\r\n" +
            "\\par\r\n" +
            "Recipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n" +
            "\\par\r\n" +
            "\\par\r\n" +
            "Please note Server will attempt to deliver this message for \\b <#relay.undelivered_after>\\b0  hours.\\par\r\n" +
            "\\par\r\n" +
            "--------\\par\r\n" +
            "\\par\r\n" +
            "Your original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n" +
            "\\par\r\n" +
            "\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n" +
            "\\lang1061\\f2\\par\r\n" +
            "\\pard\\lang1033\\f3\\fs17\\par\r\n" +
            "}\r\n";
            ServerReturnMessage delayedDeliveryWarning = new ServerReturnMessage("Delayed delivery notice: <#message.header[\"Subject:\"]>",delayedNotifyBodyRtf);
            string undeliveredNotifyBodyRtf = "" +
            "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0 Verdana;}{\\f1\\fswiss\\fprq2\\fcharset186 Verdana;}{\\f2\\fnil\\fcharset0 Verdana;}{\\f3\\fnil\\fcharset186 Verdana;}{\\f4\\fswiss\\fcharset0 Arial;}{\\f5\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n" +
            "{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n" +
            "\\viewkind4\\uc1\\pard\\f0\\fs20 Your message t\\lang1061\\f1 o \\cf1\\lang1033\\f2 <#relay.to>\\cf0\\f0 , dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0\\fs20 , could not be delivered.\\par\r\n" +
            "\\par\r\n" +
            "Recipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n" +
            "\\par\r\n" +
            "\\par\r\n" +
            "\\b * Server will not attempt to deliver this message anymore\\b0 .\\par\r\n" +
            "\\par\r\n" +
            "--------\\par\r\n" +
            "\\par\r\n" +
            "Your original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n" +
            "\\par\r\n" +
            "\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n" +
            "\\lang1061\\f5\\par\r\n" +
            "\\lang1033\\f2\\par\r\n" +
            "}\r\n";
            ServerReturnMessage undelivered = new ServerReturnMessage("Undelivered notice: <#message.header[\"Subject:\"]>",undeliveredNotifyBodyRtf);
            if(ds.Tables.Contains("ServerReturnMessages")){
                foreach(DataRow dr1 in ds.Tables["ServerReturnMessages"].Rows){
                    if(dr1["MessageType"].ToString() == "delayed_delivery_warning"){
                        delayedDeliveryWarning = new ServerReturnMessage(dr1["Subject"].ToString(),dr1["BodyTextRtf"].ToString());
                    }
                    else if(dr1["MessageType"].ToString() == "undelivered"){
                        undelivered = new ServerReturnMessage(dr1["Subject"].ToString(),dr1["BodyTextRtf"].ToString());
                    }
                }
            }
            m_pReturnMessages = new ServerReturnMessages(
                this,
                delayedDeliveryWarning,
                undelivered
            );

        }

        #endregion


        #region method SetValuesChanged

        /// <summary>
        /// Sets property HasChanges to true.
        /// </summary>
        internal void SetValuesChanged()
        {
            m_ValuesChanged = true;
        }

        #endregion

        #region method CreateSettingsSchema

        /// <summary>
		/// Creates settings schema.
		/// </summary>
		/// <param name="ds"></param>
		private void CreateSettingsSchema(DataSet ds)
        {
            #region table Settings

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
			if(!ds.Tables["Settings"].Columns.Contains("RelayStore")){
				ds.Tables["Settings"].Columns.Add("RelayStore").DefaultValue = "c:\\MailStore\\";
			}
			if(!ds.Tables["Settings"].Columns.Contains("SMTP_HostName")){
				ds.Tables["Settings"].Columns.Add("SMTP_HostName").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("POP3_HostName")){
				ds.Tables["Settings"].Columns.Add("POP3_HostName").DefaultValue = "";
			}
			if(!ds.Tables["Settings"].Columns.Contains("IMAP_HostName")){
				ds.Tables["Settings"].Columns.Add("IMAP_HostName").DefaultValue = "";
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
            if(!ds.Tables["Settings"].Columns.Contains("SMTP_MaxTransactions")){
				ds.Tables["Settings"].Columns.Add("SMTP_MaxTransactions").DefaultValue = "0";
            }
            if(!ds.Tables["Settings"].Columns.Contains("POP3_MaxConnectionsPerIP")){
				ds.Tables["Settings"].Columns.Add("POP3_MaxConnectionsPerIP").DefaultValue = "0";
            }
            if(!ds.Tables["Settings"].Columns.Contains("IMAP_MaxConnectionsPerIP")){
				ds.Tables["Settings"].Columns.Add("IMAP_MaxConnectionsPerIP").DefaultValue = "0";
            }
            if(!ds.Tables["Settings"].Columns.Contains("SIP_Enabled")){
				ds.Tables["Settings"].Columns.Add("SIP_Enabled").DefaultValue = false;
            }
            if(!ds.Tables["Settings"].Columns.Contains("SIP_HostName")){
				ds.Tables["Settings"].Columns.Add("SIP_HostName").DefaultValue = "";
            }
            if(!ds.Tables["Settings"].Columns.Contains("Relay_MaxConnectionsPerIP")){
				ds.Tables["Settings"].Columns.Add("Relay_MaxConnectionsPerIP").DefaultValue = "10";
            }
            if(!ds.Tables["Settings"].Columns.Contains("SIP_MinExpries")){
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

            #endregion

            #region table DnsServers

            if(!ds.Tables.Contains("DnsServers")){
                ds.Tables.Add("DnsServers");
            }
            if(!ds.Tables["DnsServers"].Columns.Contains("IP")){
                ds.Tables["DnsServers"].Columns.Add("IP").DefaultValue = "";
            }

            #endregion

            #region table SMTP_Bindings

            if (!ds.Tables.Contains("SMTP_Bindings")){
                ds.Tables.Add("SMTP_Bindings");
            }
            if(!ds.Tables["SMTP_Bindings"].Columns.Contains("HostName")){
                ds.Tables["SMTP_Bindings"].Columns.Add("HostName").DefaultValue = "";
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

            if (!ds.Tables.Contains("POP3_Bindings")){
                ds.Tables.Add("POP3_Bindings");
            }
            if(!ds.Tables["POP3_Bindings"].Columns.Contains("HostName")){
                ds.Tables["POP3_Bindings"].Columns.Add("HostName").DefaultValue = "";
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

            if (!ds.Tables.Contains("IMAP_Bindings")){
                ds.Tables.Add("IMAP_Bindings");
            }
            if(!ds.Tables["IMAP_Bindings"].Columns.Contains("HostName")){
                ds.Tables["IMAP_Bindings"].Columns.Add("HostName").DefaultValue = "";
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


            ds.Tables["Settings"].Rows.Add(ds.Tables["Settings"].NewRow());
            foreach(DataRow dr in ds.Tables["Settings"].Rows){
				foreach(DataColumn dc in ds.Tables["Settings"].Columns){
					if(dr.IsNull(dc.ColumnName)){
						dr[dc.ColumnName] = dc.DefaultValue;
					}
				}
			}            
        }

		#endregion

        #region method ToDataSet

        /// <summary>
        /// Copies all system settings to DataSet.
        /// </summary>
        /// <returns></returns>
        internal DataSet ToDataSet()
        {
            //--- Construct settings DataSet -----------------------------------------------------//
            DataSet ds =  new DataSet();
            CreateSettingsSchema(ds);

            DataRow dr = ds.Tables["Settings"].Rows[0];

            foreach(IPAddress ip in m_pDnsServers){
                DataRow drX = ds.Tables["DnsServers"].NewRow();
                drX["IP"] = ip.ToString();
                ds.Tables["DnsServers"].Rows.Add(drX);
            }

            dr["ServerAuthenticationType"] = (int)this.Authentication.AuthenticationType;
            dr["ServerAuthWinDomain"]      = this.Authentication.WinDomain;
            dr["LdapServer"]               = this.Authentication.LdapServer;
            dr["LdapDN"]                   = this.Authentication.LdapDn;

            dr["SMTP_Enabled"]             = this.SMTP.Enabled;
            dr["SMTP_GreetingText"]        = this.SMTP.GreetingText;
            dr["SMTP_DefaultDomain"]       = this.SMTP.DefaultDomain;
            dr["SMTP_SessionIdleTimeOut"]  = this.SMTP.SessionIdleTimeOut;
            dr["SMTP_Threads"]             = this.SMTP.MaximumConnections;
            dr["SMTP_MaxConnectionsPerIP"] = this.SMTP.MaximumConnectionsPerIP;
            dr["SMTP_MaxBadCommands"]      = this.SMTP.MaximumBadCommands;
            dr["MaxRecipients"]            = this.SMTP.MaximumRecipientsPerMessage;
            dr["MaxMessageSize"]           = this.SMTP.MaximumMessageSize;
            dr["SMTP_MaxTransactions"]     = this.SMTP.MaximumTransactions;
            dr["SMTP_RequireAuth"]         = this.SMTP.RequireAuthentication;
            // Add IP binds
            foreach(IPBindInfo bindInfo in this.SMTP.Binds){
                DataRow drBinding = ds.Tables["SMTP_Bindings"].NewRow();
                drBinding["HostName"]        = bindInfo.HostName;
                drBinding["IP"]              = bindInfo.IP.ToString();
                drBinding["Port"]            = bindInfo.Port;
                drBinding["SSL"]             = bindInfo.SslMode;
                if(bindInfo.Certificate != null){
                    drBinding["SSL_Certificate"] = bindInfo.Certificate.Export(X509ContentType.Pfx);
                }
                else{
                    drBinding["SSL_Certificate"] = DBNull.Value;
                }
                ds.Tables["SMTP_Bindings"].Rows.Add(drBinding);
            }
            
            dr["POP3_Enabled"]             = this.POP3.Enabled;
            dr["POP3_GreetingText"]        = this.POP3.GreetingText;
            dr["POP3_SessionIdleTimeOut"]  = this.POP3.SessionIdleTimeOut;
            dr["POP3_Threads"]             = this.POP3.MaximumConnections;
            dr["POP3_MaxConnectionsPerIP"] = this.POP3.MaximumConnectionsPerIP;
            dr["POP3_MaxBadCommands"]      = this.POP3.MaximumBadCommands;
            // Add IP binds
            foreach(IPBindInfo bindInfo in this.POP3.Binds){
                DataRow drBinding = ds.Tables["POP3_Bindings"].NewRow();               
                drBinding["HostName"]        = bindInfo.HostName;               
                drBinding["IP"]              = bindInfo.IP.ToString();
                drBinding["Port"]            = bindInfo.Port;
                drBinding["SSL"]             = bindInfo.SslMode;
                if(bindInfo.Certificate != null){
                    drBinding["SSL_Certificate"] = bindInfo.Certificate.Export(X509ContentType.Pfx);
                }
                else{
                    drBinding["SSL_Certificate"] = DBNull.Value;
                }
                ds.Tables["POP3_Bindings"].Rows.Add(drBinding);
            }
            
            dr["IMAP_Enabled"]             = this.IMAP.Enabled;
            dr["IMAP_GreetingText"]        = this.IMAP.GreetingText;
            dr["IMAP_SessionIdleTimeOut"]  = this.IMAP.SessionIdleTimeOut;
            dr["IMAP_Threads"]             = this.IMAP.MaximumConnections;
            dr["IMAP_MaxConnectionsPerIP"] = this.IMAP.MaximumConnectionsPerIP;
            dr["IMAP_MaxBadCommands"]      = this.IMAP.MaximumBadCommands;
            // Add IP binds
            foreach(IPBindInfo bindInfo in this.IMAP.Binds){
                DataRow drBinding = ds.Tables["IMAP_Bindings"].NewRow();               
                drBinding["HostName"]        = bindInfo.HostName;               
                drBinding["IP"]              = bindInfo.IP.ToString();
                drBinding["Port"]            = bindInfo.Port;
                drBinding["SSL"]             = bindInfo.SslMode;
                if(bindInfo.Certificate != null){
                    drBinding["SSL_Certificate"] = bindInfo.Certificate.Export(X509ContentType.Pfx);
                }
                else{
                    drBinding["SSL_Certificate"] = DBNull.Value;
                }
                ds.Tables["IMAP_Bindings"].Rows.Add(drBinding);
            }

            dr["Relay_Mode"]                  = this.Relay.RelayMode.ToString();
            dr["Relay_SmartHostsBalanceMode"] = this.Relay.SmartHostsBalanceMode.ToString();
            dr["Relay_SessionIdleTimeOut"]    = this.Relay.SessionIdleTimeOut;
            dr["MaxRelayThreads"]             = this.Relay.MaximumConnections;
            dr["Relay_MaxConnectionsPerIP"]   = this.Relay.MaximumConnectionsPerIP;
            dr["RelayInterval"]               = this.Relay.RelayInterval;
            dr["RelayRetryInterval"]          = this.Relay.RelayRetryInterval;
            dr["RelayUndeliveredWarning"]     = this.Relay.SendUndeliveredWarningAfter;
            dr["RelayUndelivered"]            = this.Relay.SendUndeliveredAfter;
            dr["StoreUndeliveredMessages"]    = this.Relay.StoreUndeliveredMessages;
            dr["Relay_UseTlsIfPossible"]      = this.Relay.UseTlsIfPossible;
            // Add smart hosts
            foreach(Relay_SmartHost smartHost in this.Relay.SmartHosts){
                DataRow drX = ds.Tables["Relay_SmartHosts"].NewRow();
                drX["Host"]     = smartHost.Host;
                drX["Port"]     = smartHost.Port.ToString();
                drX["SslMode"]  = smartHost.SslMode.ToString();
                drX["UserName"] = smartHost.UserName;
                drX["Password"] = smartHost.Password;
                ds.Tables["Relay_SmartHosts"].Rows.Add(drX);
            }
            // Add IP binds
            foreach(IPBindInfo bindInfo in this.Relay.Binds){
                DataRow drBinding = ds.Tables["Relay_Bindings"].NewRow();
                drBinding["HostName"]        = bindInfo.HostName;
                drBinding["IP"]              = bindInfo.IP.ToString();
                drBinding["Port"]            = bindInfo.Port;
                drBinding["SSL"]             = bindInfo.SslMode;
                if(bindInfo.Certificate != null){
                    drBinding["SSL_Certificate"] = bindInfo.Certificate.Export(X509ContentType.Pfx);
                }
                else{
                    drBinding["SSL_Certificate"] = DBNull.Value;
                }
                ds.Tables["Relay_Bindings"].Rows.Add(drBinding);
            }

            dr["FetchPOP3_Enabled"]  = this.FetchMessages.Enabled;
            dr["FetchPOP3_Interval"] = this.FetchMessages.FetchInterval;

            dr["SIP_Enabled"]    = this.SIP.Enabled;
            dr["SIP_ProxyMode"]  = this.SIP.ProxyMode.ToString();
            dr["SIP_MinExpires"] = this.SIP.MinimumExpires;
            // Add IP binds
            foreach(IPBindInfo bindInfo in this.SIP.Binds){
                DataRow drBinding = ds.Tables["SIP_Bindings"].NewRow();              
                drBinding["HostName"]        = bindInfo.HostName;
                drBinding["Protocol"]        = bindInfo.Protocol;
                drBinding["IP"]              = bindInfo.IP.ToString();
                drBinding["Port"]            = bindInfo.Port;
                drBinding["SSL"]             = bindInfo.SslMode;
                if(bindInfo.Certificate != null){
                    drBinding["SSL_Certificate"] = bindInfo.Certificate.Export(X509ContentType.Pfx);
                }
                else{
                    drBinding["SSL_Certificate"] = DBNull.Value;
                }
                ds.Tables["SIP_Bindings"].Rows.Add(drBinding);
            }
            // Add Gateways
            foreach(SIP_Gateway gw in this.SIP.Gateways){
                DataRow drGw = ds.Tables["SIP_Gateways"].NewRow();
                drGw["UriScheme"] = gw.UriScheme;
                drGw["Transport"] = gw.Transport;
                drGw["Host"]      = gw.Host;
                drGw["Port"]      = gw.Port;
                drGw["Realm"]     = gw.Realm;
                drGw["UserName"]  = gw.UserName;
                drGw["Password"]  = gw.Password;
                ds.Tables["SIP_Gateways"].Rows.Add(drGw);
            }

            dr["LogSMTPCmds"]       = this.Logging.LogSMTP;
            dr["SMTP_LogPath"]      = this.Logging.SmtpLogsPath;
            dr["LogPOP3Cmds"]       = this.Logging.LogPOP3;
            dr["POP3_LogPath"]      = this.Logging.Pop3LogsPath;
            dr["LogIMAPCmds"]       = this.Logging.LogIMAP;
            dr["IMAP_LogPath"]      = this.Logging.ImapLogsPath;
            dr["LogRelayCmds"]      = this.Logging.LogRelay;
            dr["Relay_LogPath"]     = this.Logging.RelayLogsPath;
            dr["LogFetchPOP3Cmds"]  = this.Logging.LogFetchMessages;
            dr["FetchPOP3_LogPath"] = this.Logging.FetchMessagesLogsPath;

            DataRow dr1 = ds.Tables["ServerReturnMessages"].NewRow();
            dr1["MessageType"] = "delayed_delivery_warning";
            dr1["Subject"]     = m_pReturnMessages.DelayedDeliveryWarning.Subject;
            dr1["BodyTextRtf"] = m_pReturnMessages.DelayedDeliveryWarning.BodyTextRtf;
            ds.Tables["ServerReturnMessages"].Rows.Add(dr1);
            DataRow dr2 = ds.Tables["ServerReturnMessages"].NewRow();
            dr2["MessageType"] = "undelivered";
            dr2["Subject"]     = m_pReturnMessages.Undelivered.Subject;
            dr2["BodyTextRtf"] = m_pReturnMessages.Undelivered.BodyTextRtf;
            ds.Tables["ServerReturnMessages"].Rows.Add(dr2);
            //------------------------------------------------------------------------------------//

            return ds;
        }

        #endregion

        #region method Bind

        /// <summary>
        /// Gets server settings and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetSettings <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            // Call TCP GetSettings
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetSettings " + m_pVirtualServer.VirtualServerID);

            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
            MemoryStream ms = new MemoryStream();
            m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
            ms.Position = 0;

            DataSet ds = new DataSet();
            CreateSettingsSchema(ds);
            ds.Clear();
            ds.ReadXml(ms);

            DataRow dr = ds.Tables["Settings"].Rows[0];
            //--- General -------------------------------------
            List<IPAddress> dnsServers = new List<IPAddress>();
            foreach(DataRow drX in ds.Tables["DnsServers"].Rows){
                dnsServers.Add(IPAddress.Parse(drX["IP"].ToString()));
            }
            m_pDnsServers = dnsServers.ToArray();

            //--- AUTH -------------------------------------------//
            m_pAuth = new Auth_Settings(
                this,
                (ServerAuthenticationType_enum)Convert.ToInt32(dr["ServerAuthenticationType"]),
                dr["ServerAuthWinDomain"].ToString(),
                dr["LdapServer"].ToString(),
                dr["LdapDN"].ToString()
            );
            //--- SMTP -------------------------------------------//
            // Get bindings
            IPBindInfo[] smtpBinds = new IPBindInfo[ds.Tables["SMTP_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["SMTP_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["SMTP_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["SMTP_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                smtpBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pSMTP = new SMTP_Settings(
                this,
                Convert.ToBoolean(dr["SMTP_Enabled"]),
                dr["SMTP_GreetingText"].ToString(),
                dr["SMTP_DefaultDomain"].ToString(),
                Convert.ToInt32(dr["SMTP_SessionIdleTimeOut"]),
                Convert.ToInt32(dr["SMTP_Threads"]),
                Convert.ToInt32(dr["SMTP_MaxConnectionsPerIP"]),
                Convert.ToInt32(dr["SMTP_MaxBadCommands"]),
                Convert.ToInt32(dr["MaxRecipients"]),
                Convert.ToInt32(dr["MaxMessageSize"]),
                Convert.ToInt32(dr["SMTP_MaxTransactions"]),
                Convert.ToBoolean(dr["SMTP_RequireAuth"]),
                smtpBinds
            );            
            //--- POP3 ------------------------------------------//
            // Get bindings
            IPBindInfo[] pop3Binds = new IPBindInfo[ds.Tables["POP3_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["POP3_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["POP3_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["POP3_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                pop3Binds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pPOP3 = new POP3_Settings(
                this,
                Convert.ToBoolean(dr["POP3_Enabled"]),
                dr["POP3_GreetingText"].ToString(),                
                Convert.ToInt32(dr["POP3_SessionIdleTimeOut"]),
                Convert.ToInt32(dr["POP3_Threads"]),
                Convert.ToInt32(dr["POP3_MaxConnectionsPerIP"]),
                Convert.ToInt32(dr["POP3_MaxBadCommands"]),
                pop3Binds
            );
            //--- IMAP ------------------------------------------//
            // Get bindings
            IPBindInfo[] imapBinds = new IPBindInfo[ds.Tables["IMAP_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["IMAP_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["IMAP_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["IMAP_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                imapBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pIMAP = new IMAP_Settings(
                this,
                Convert.ToBoolean(dr["IMAP_Enabled"]),
                dr["IMAP_GreetingText"].ToString(),                
                Convert.ToInt32(dr["IMAP_SessionIdleTimeOut"]),
                Convert.ToInt32(dr["IMAP_Threads"]),
                Convert.ToInt32(dr["IMAP_MaxConnectionsPerIP"]),
                Convert.ToInt32(dr["IMAP_MaxBadCommands"]),
                imapBinds
            );
            //--- Relay ----------------------------------------//
            // Get smart hosts
            Relay_SmartHost[] relaySmartHosts = new Relay_SmartHost[ds.Tables["Relay_SmartHosts"].Rows.Count];
            for(int i=0;i<relaySmartHosts.Length;i++){
                DataRow drX = ds.Tables["Relay_SmartHosts"].Rows[i];
                relaySmartHosts[i] = new Relay_SmartHost(
                    ConvertEx.ToString(drX["Host"]),
                    ConvertEx.ToInt32(drX["Port"]),
                    (SslMode)Enum.Parse(typeof(SslMode),drX["SslMode"].ToString()),
                    ConvertEx.ToString(drX["UserName"]),
                    ConvertEx.ToString(drX["Password"])
                );
            }

            // Get bindings
            IPBindInfo[] relayBinds = new IPBindInfo[ds.Tables["Relay_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["Relay_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["Relay_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["Relay_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                relayBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    BindInfoProtocol.TCP,
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            m_pRelay = new Relay_Settings(
                this,
                (Relay_Mode)Enum.Parse(typeof(Relay_Mode),dr["Relay_Mode"].ToString()),
                (BalanceMode)Enum.Parse(typeof(BalanceMode),dr["Relay_SmartHostsBalanceMode"].ToString()),
                relaySmartHosts,
                Convert.ToInt32(dr["Relay_SessionIdleTimeOut"]),
                Convert.ToInt32(dr["MaxRelayThreads"]),
                Convert.ToInt32(dr["Relay_MaxConnectionsPerIP"]),
                Convert.ToInt32(dr["RelayInterval"]),
                Convert.ToInt32(dr["RelayRetryInterval"]),
                Convert.ToInt32(dr["RelayUndeliveredWarning"]),
                Convert.ToInt32(dr["RelayUndelivered"]),
                Convert.ToBoolean(dr["StoreUndeliveredMessages"]),
                Convert.ToBoolean(dr["Relay_UseTlsIfPossible"]),
                relayBinds
            );
            //--- Fetch messages ------------------------------//
            m_pFetchMessages = new FetchMessages_Settings(
                this,
                Convert.ToBoolean(dr["FetchPOP3_Enabled"]),
                Convert.ToInt32(dr["FetchPOP3_Interval"])
            );
            //--- SIP ----------------------------------------//
            // Get bindings
            IPBindInfo[] sipBinds = new IPBindInfo[ds.Tables["SIP_Bindings"].Rows.Count];
            for(int i=0;i<ds.Tables["SIP_Bindings"].Rows.Count;i++){
                DataRow drBinding = ds.Tables["SIP_Bindings"].Rows[i];
                byte[] certificate = null;
                if(ds.Tables["SIP_Bindings"].Columns.Contains("SSL_Certificate") && drBinding["SSL_Certificate"] != null && !drBinding.IsNull("SSL_Certificate")){
                    if(((byte[])drBinding["SSL_Certificate"]).Length > 0){
                        certificate = (byte[])drBinding["SSL_Certificate"];
                    }
                }
                sipBinds[i] = new IPBindInfo(
                    drBinding["HostName"].ToString(),
                    (BindInfoProtocol)Enum.Parse(typeof(BindInfoProtocol),drBinding["Protocol"].ToString()),
                    IPAddress.Parse(drBinding["IP"].ToString()),
                    Convert.ToInt32(drBinding["Port"]),
                    ParseSslMode(drBinding["SSL"].ToString()),
                    PaseCertificate(certificate)
                );
            }

            // Construct gateways
            SIP_GatewayCollection gateways = new SIP_GatewayCollection(this);
            foreach(DataRow drGw in ds.Tables["SIP_Gateways"].Rows){
                gateways.AddInternal(
                    drGw["UriScheme"].ToString(),
                    drGw["Transport"].ToString(),
                    drGw["Host"].ToString(),
                    Convert.ToInt32(drGw["Port"]),
                    drGw["Realm"].ToString(),
                    drGw["UserName"].ToString(),
                    drGw["Password"].ToString()
                );
            }

            m_pSIP = new SIP_Settings(
                this,                
                Convert.ToBoolean(dr["SIP_Enabled"]),
                (LumiSoft.Net.SIP.Proxy.SIP_ProxyMode)Enum.Parse(typeof(LumiSoft.Net.SIP.Proxy.SIP_ProxyMode),dr["SIP_ProxyMode"].ToString()),
                Convert.ToInt32(dr["SIP_MinExpires"]),
                sipBinds,
                gateways
            );
            //--- Logging ------------------------------------//
            m_pLogging = new Logging_Settings(
                this,
                ConvertEx.ToBoolean(dr["LogSMTPCmds"],false),
                dr["SMTP_LogPath"].ToString(),
                ConvertEx.ToBoolean(dr["LogPOP3Cmds"],false),
                dr["POP3_LogPath"].ToString(),
                ConvertEx.ToBoolean(dr["LogIMAPCmds"],false),
                dr["IMAP_LogPath"].ToString(),
                ConvertEx.ToBoolean(dr["LogRelayCmds"],false),
                dr["Relay_LogPath"].ToString(),
                ConvertEx.ToBoolean(dr["LogFetchPOP3Cmds"],false),
                dr["FetchPOP3_LogPath"].ToString()
            );
            //--- Server return messages
            string delayedNotifyBodyRtf = "" +
            "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Verdana;}{\\f1\\fnil\\fcharset186 Verdana;}{\\f2\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}{\\f3\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n" +
            "{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n" +
            "\\viewkind4\\uc1\\pard\\f0\\fs20 This e-mail is generated by the Server(\\cf1 <#relay.hostname>\\cf0 )  to notify you, \\par\r\n" +
            "\\lang1061\\f1 that \\lang1033\\f0 your message to \\cf1 <#relay.to>\\cf0  dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0  \\fs20 could not be sent at the first attempt.\\par\r\n" +
            "\\par\r\n" +
            "Recipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n" +
            "\\par\r\n" +
            "\\par\r\n" +
            "Please note Server will attempt to deliver this message for \\b <#relay.undelivered_after>\\b0  hours.\\par\r\n" +
            "\\par\r\n" +
            "--------\\par\r\n" +
            "\\par\r\n" +
            "Your original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n" +
            "\\par\r\n" +
            "\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n" +
            "\\lang1061\\f2\\par\r\n" +
            "\\pard\\lang1033\\f3\\fs17\\par\r\n" +
            "}\r\n";
            ServerReturnMessage delayedDeliveryWarning = new ServerReturnMessage("Delayed delivery notice: <#message.header[\"Subject:\"]>",delayedNotifyBodyRtf);
            string undeliveredNotifyBodyRtf = "" +
            "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0 Verdana;}{\\f1\\fswiss\\fprq2\\fcharset186 Verdana;}{\\f2\\fnil\\fcharset0 Verdana;}{\\f3\\fnil\\fcharset186 Verdana;}{\\f4\\fswiss\\fcharset0 Arial;}{\\f5\\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n" +
            "{\\colortbl ;\\red0\\green64\\blue128;\\red255\\green0\\blue0;\\red0\\green128\\blue0;}\r\n" +
            "\\viewkind4\\uc1\\pard\\f0\\fs20 Your message t\\lang1061\\f1 o \\cf1\\lang1033\\f2 <#relay.to>\\cf0\\f0 , dated \\b\\fs16 <#message.header[\"Date:\"]>\\b0\\fs20 , could not be delivered.\\par\r\n" +
            "\\par\r\n" +
            "Recipient \\i <#relay.to>'\\i0 s Server (\\cf1 <#relay.session_hostname>\\cf0 ) returned the following response: \\cf2 <#relay.error>\\cf0\\par\r\n" +
            "\\par\r\n" +
            "\\par\r\n" +
            "\\b * Server will not attempt to deliver this message anymore\\b0 .\\par\r\n" +
            "\\par\r\n" +
            "--------\\par\r\n" +
            "\\par\r\n" +
            "Your original message is attached to this e-mail (\\b data.eml\\b0 )\\par\r\n" +
            "\\par\r\n" +
            "\\fs16 The tracking number for this message is \\cf3 <#relay.session_messageid>\\cf0\\fs20\\par\r\n" +
            "\\lang1061\\f5\\par\r\n" +
            "\\lang1033\\f2\\par\r\n" +
            "}\r\n";
            ServerReturnMessage undelivered = new ServerReturnMessage("Undelivered notice: <#message.header[\"Subject:\"]>",undeliveredNotifyBodyRtf);
            if(ds.Tables.Contains("ServerReturnMessages")){
                foreach(DataRow dr1 in ds.Tables["ServerReturnMessages"].Rows){
                    if(dr1["MessageType"].ToString() == "delayed_delivery_warning"){
                        delayedDeliveryWarning = new ServerReturnMessage(dr1["Subject"].ToString(),dr1["BodyTextRtf"].ToString());
                    }
                    else if(dr1["MessageType"].ToString() == "undelivered"){
                        undelivered = new ServerReturnMessage(dr1["Subject"].ToString(),dr1["BodyTextRtf"].ToString());
                    }
                }
            }
            m_pReturnMessages = new ServerReturnMessages(
                this,
                delayedDeliveryWarning,
                undelivered
            );
        }

        #endregion

        #region method PaseCertificate

        /// <summary>
        /// Parses x509 certificate from specified data. Returns null if no certificate to load.
        /// </summary>
        /// <param name="cert">Certificate data.</param>
        /// <returns>Returns parse certificate or null if no certificate.</returns>
        private X509Certificate2 PaseCertificate(byte[] cert)
        {
            if(cert == null){
                return null;
            }
            else{
                X509Certificate2 c = new X509Certificate2(cert,"",X509KeyStorageFlags.Exportable);

                return c;
            }
        }

        #endregion

        #region method ParseSslMode

        /// <summary>
        /// Parses SSL mode from string.
        /// </summary>
        /// <param name="value">Ssl mode string.</param>
        /// <returns>Returns parsed SSL mode.</returns>
        private SslMode ParseSslMode(string value)
        {
            if(value.ToLower() == "false"){ // REMOVE ME: remove in next versions
                return SslMode.None;
            }
            else if(value.ToLower() == "true"){ // REMOVE ME: remove in next versions
                return SslMode.SSL;
            }
            else{
                return (SslMode)Enum.Parse(typeof(SslMode),value);
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets DNS servers.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public IPAddress[] DnsServers
        {
            get{ return m_pDnsServers; }

            set{
                if(value == null){
                    throw new ArgumentNullException("DnsServers");
                }

                // See if DNS server changed.                
                bool hasChanges = false;
                if(m_pDnsServers.Length == value.Length){
                    for(int i=0;i<m_pDnsServers.Length;i++){
                        if(!m_pDnsServers[i].Equals(value[i])){
                            hasChanges = true;
                            break;
                        }
                    }
                }
                else{
                    hasChanges = true;
                }

                if(hasChanges){
                    m_pDnsServers = value;
                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets authentication settings.
        /// </summary>
        public Auth_Settings Authentication
        {
            get{ return m_pAuth; }
        }

        /// <summary>
        /// Gets SMTP settings.
        /// </summary>
        public SMTP_Settings SMTP
        {
            get{ return m_pSMTP; }
        }

        /// <summary>
        /// Gets POP3 settings.
        /// </summary>
        public POP3_Settings POP3
        {
            get{ return m_pPOP3; }
        }

        /// <summary>
        /// Gets IMAP settings.
        /// </summary>
        public IMAP_Settings IMAP
        {
            get{ return m_pIMAP; }
        }
                
        /// <summary>
        /// Gets relay settings.
        /// </summary>
        public Relay_Settings Relay
        {
            get{ return m_pRelay; }
        }

        /// <summary>
        /// Gets fetch messages service settings.
        /// </summary>
        public FetchMessages_Settings FetchMessages
        {
            get{ return m_pFetchMessages; }
        }

        /// <summary>
        /// Gets SIP settings.
        /// </summary>
        public SIP_Settings SIP
        {
            get{ return m_pSIP; }
        }

        /// <summary>
        /// Gets logging settings.
        /// </summary>
        public Logging_Settings Logging
        {
            get{ return m_pLogging; }
        }

        /// <summary>
        /// Gets server return messages object.
        /// </summary>
        public ServerReturnMessages ReturnMessages
        {
            get{ return m_pReturnMessages; }
        }

        #endregion

    }
}
