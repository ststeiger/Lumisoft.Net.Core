using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Threading;
using System.DirectoryServices.Protocols;

using LumiSoft.Net;
using LumiSoft.Net.IO;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.TCP;
using LumiSoft.Net.SMTP;
using LumiSoft.Net.SMTP.Server;
using LumiSoft.Net.SMTP.Relay;
using LumiSoft.Net.POP3.Client;
using LumiSoft.Net.POP3.Server;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.IMAP.Server;
using LumiSoft.MailServer.Relay;
using LumiSoft.Net.SIP;
using LumiSoft.Net.SIP.Stack;
using LumiSoft.Net.SIP.Proxy;
using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Implements mail server virtual server.
    /// </summary>
    public class VirtualServer
    {
        private Server              m_pOwnerServer       = null;
        private string              m_ID                 = "";
        private string              m_Name               = "";
        private string              m_ApiInitString      = "";
        private IMailServerApi      m_pApi               = null;
        private bool                m_Running            = false;
        private Dns_Client          m_pDnsClient         = null;
        private SMTP_Server         m_pSmtpServer        = null;
        private POP3_Server         m_pPop3Server        = null;
        private IMAP_Server         m_pImapServer        = null;
        private RelayServer         m_pRelayServer       = null;
        private SIP_Proxy           m_pSipServer         = null;
        private FetchPop3           m_pFetchServer       = null;
        private RecycleBinManager   m_pRecycleBinManager = null;
        private BadLoginManager     m_pBadLoginManager   = null;
        private System.Timers.Timer m_pTimer             = null;
        // Settings
        private DateTime                     m_SettingsDate;
        private string                       m_MailStorePath      = "";
        private MailServerAuthType_enum      m_AuthType           = MailServerAuthType_enum.Integrated;
        private string                       m_Auth_Win_Domain    = "";
        private string                       m_Auth_LDAP_Server   = "";
        private string                       m_Auth_LDAP_DN       = "";
        private bool                         m_SMTP_RequireAuth   = false;
        private string                       m_SMTP_DefaultDomain = "";        
        private string                       m_Server_LogPath     = "";
		private string                       m_SMTP_LogPath       = "";
		private string                       m_POP3_LogPath       = "";
		private string                       m_IMAP_LogPath       = "";
        private string                       m_Relay_LogPath      = "";
		private string                       m_Fetch_LogPath      = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Server what owns this virtual server.</param>
        /// <param name="id">Virtual server ID.</param>
        /// <param name="name">Virtual server name.</param>
        /// <param name="apiInitString">Virtual server api initi string.</param>
        /// <param name="api">Virtual server API.</param>
        public VirtualServer(Server server,string id,string name,string apiInitString,IMailServerApi api)
        {
            m_pOwnerServer  = server;
            m_ID            = id;
            m_Name          = name;
            m_ApiInitString = apiInitString;
            m_pApi          = api;
        }


        #region Events Handling

        #region SMTP Events

        #region method m_pSmtpServer_SessionCreated

        /// <summary>
        /// Is called when new SMTP server session has created.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_SessionCreated(object sender,TCP_ServerSessionEventArgs<SMTP_Session> e)
        {            
            e.Session.Started += new EventHandler<SMTP_e_Started>(m_pSmtpServer_Session_Started);
            e.Session.Ehlo += new EventHandler<SMTP_e_Ehlo>(m_pSmtpServer_Session_Ehlo);
            e.Session.MailFrom += new EventHandler<SMTP_e_MailFrom>(m_pSmtpServer_Session_MailFrom);
            e.Session.RcptTo += new EventHandler<SMTP_e_RcptTo>(m_pSmtpServer_Session_RcptTo);
            e.Session.GetMessageStream += new EventHandler<SMTP_e_Message>(m_pSmtpServer_Session_GetMessageStream);
            e.Session.MessageStoringCanceled += new EventHandler(m_pSmtpServer_Session_MessageStoringCanceled);
            e.Session.MessageStoringCompleted += new EventHandler<SMTP_e_MessageStored>(m_pSmtpServer_Session_MessageStoringCompleted);

            // Add session supported authentications.
            if(m_AuthType == MailServerAuthType_enum.Windows || m_AuthType == MailServerAuthType_enum.Ldap){
                // For windows or LDAP auth, we can allow only plain text authentications, because otherwise 
                // we can't do auth against windows (it requires user name and password).

                #region PLAIN

                AUTH_SASL_ServerMechanism_Plain auth_plain = new AUTH_SASL_ServerMechanism_Plain(false);
                auth_plain.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_plain.Name,auth_plain);

                #endregion

                #region LOGIN

                AUTH_SASL_ServerMechanism_Login auth_login = new AUTH_SASL_ServerMechanism_Login(false);
                auth_login.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_login.Name,auth_login);

                #endregion
            }
            else{
                #region DIGEST-MD5

                AUTH_SASL_ServerMechanism_DigestMd5 auth_digestmMd5 = new AUTH_SASL_ServerMechanism_DigestMd5(false);                
                auth_digestmMd5.Realm = e.Session.LocalHostName;
                auth_digestmMd5.GetUserInfo += new EventHandler<AUTH_e_UserInfo>(delegate(object s,AUTH_e_UserInfo e1){
            
                    FillUserInfo(e1);
                });
                e.Session.Authentications.Add(auth_digestmMd5.Name,auth_digestmMd5);

                #endregion

                #region CRAM-MD5

                AUTH_SASL_ServerMechanism_CramMd5 auth_cramMd5 = new AUTH_SASL_ServerMechanism_CramMd5(false);
                auth_cramMd5.GetUserInfo += new EventHandler<AUTH_e_UserInfo>(delegate(object s,AUTH_e_UserInfo e1){
                    FillUserInfo(e1);
                });
                e.Session.Authentications.Add(auth_cramMd5.Name,auth_cramMd5);

                #endregion

                #region PLAIN

                AUTH_SASL_ServerMechanism_Plain auth_plain = new AUTH_SASL_ServerMechanism_Plain(false);
                auth_plain.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_plain.Name,auth_plain);

                #endregion

                #region LOGIN

                AUTH_SASL_ServerMechanism_Login auth_login = new AUTH_SASL_ServerMechanism_Login(false);
                auth_login.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_login.Name,auth_login);

                #endregion                                
            }
        }
                                                
        #endregion

        #region method m_pSmtpServer_Session_Started

        /// <summary>
        /// Is called when SMTP server sessions starts session processing.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_Session_Started(object sender,SMTP_e_Started e)
        {
            if(!IsAccessAllowed(Service_enum.SMTP,e.Session.RemoteEndPoint.Address)){
                e.Reply = new SMTP_Reply(554,"Your IP address is blocked.");
            }
        }

        #endregion

        #region method m_pSmtpServer_Session_Ehlo

        /// <summary>
        /// Is called when SMTP server session gets EHLO/HELO command.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_Session_Ehlo(object sender,SMTP_e_Ehlo e)
        {
            
        }

        #endregion

        #region method m_pSmtpServer_Session_MailFrom

        /// <summary>
        /// Is called when SMTP server session gets MAIL command.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_Session_MailFrom(object sender,SMTP_e_MailFrom e)
        {
            if(m_SMTP_RequireAuth && !e.Session.IsAuthenticated){
                e.Reply = new SMTP_Reply(530,"5.7.0  Authentication required.");
				return;
			}

            // Block blank domain(user@) email addresses.
            if(e.MailFrom.Mailbox.IndexOf('@') != -1 && e.MailFrom.Mailbox.Substring(e.MailFrom.Mailbox.IndexOf('@') + 1).Length < 1){
                e.Reply = new SMTP_Reply(501,"MAIL FROM: address(" + e.MailFrom + ") domain name must be specified.");
                return;
            }

			try{
				//--- Filter sender -----------------------------------------------------//					
				DataView dvFilters = m_pApi.GetFilters();
				dvFilters.RowFilter = "Enabled=true AND Type='ISmtpSenderFilter'";
				dvFilters.Sort = "Cost";			
				foreach(DataRowView drViewFilter in dvFilters){
					string assemblyFile = drViewFilter.Row["Assembly"].ToString();
					// File is without path probably, try to load it from filters folder
					if(!File.Exists(assemblyFile)){
						assemblyFile = m_pOwnerServer.StartupPath + "\\Filters\\" + assemblyFile;
					}

					Assembly ass = Assembly.LoadFrom(assemblyFile);
					Type tp = ass.GetType(drViewFilter.Row["ClassName"].ToString());
					object filterInstance = Activator.CreateInstance(tp);
					ISmtpSenderFilter filter = (ISmtpSenderFilter)filterInstance;
								
					string error = null;
					if(!filter.Filter(e.MailFrom.Mailbox,m_pApi,e.Session,out error)){						
						if(error != null){
                            e.Reply = new SMTP_Reply(550,error);
						}
                        else{
                            e.Reply = new SMTP_Reply(550,"Sender rejected.");
                        }

						return;
					}
				}
				//----------------------------------------------------------------------//
			}
			catch(Exception x){
                e.Reply = new SMTP_Reply(500,"Internal server error.");
				Error.DumpError(this.Name,x,new System.Diagnostics.StackTrace());
			}
        }

        #endregion

        #region method m_pSmtpServer_Session_RcptTo

        /// <summary>
        /// Is called when SMTP server session gets RCPT command.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_Session_RcptTo(object sender,SMTP_e_RcptTo e)
        {
            try{
				string mailTo = e.RcptTo.Mailbox;

				// If domain isn't specified, add default domain
				if(mailTo.IndexOf("@") == -1){
					mailTo += "@" + m_SMTP_DefaultDomain;
				}

				//1) is local domain or relay needed
				//2) can map email address to mailbox
				//3) is alias
				//4) if matches any routing pattern

				// check if e-domain is local
				if(m_pApi.DomainExists(mailTo)){
                    string user = m_pApi.MapUser(mailTo);
					if(user == null){
                        e.Reply = new SMTP_Reply(550,"No such user here.");

						// Check if mailing list.
						if(m_pApi.MailingListExists(mailTo)){
                            // Not authenticated, see is anyone access allowed
                            if(!e.Session.IsAuthenticated){
                                if(m_pApi.CanAccessMailingList(mailTo,"anyone")){
                                    e.Reply = new SMTP_Reply(250,"OK.");
                                }
                            }
                            // Authenticated, see if user has access granted
                            else{
                                if(m_pApi.CanAccessMailingList(mailTo,e.Session.AuthenticatedUserIdentity.Name)){
                                    e.Reply = new SMTP_Reply(250,"OK.");
                                }
                            }
						}
                        // At last check if matches any routing pattern.
                        else{
                            DataView dv = m_pApi.GetRoutes();
                            foreach(DataRowView drV in dv){
                                // We have matching route
                                if(Convert.ToBoolean(drV["Enabled"]) && SCore.IsAstericMatch(drV["Pattern"].ToString(),mailTo)){
                                    e.Reply = new SMTP_Reply(250,"OK.");
                                    break;
                                }
                            }
                        }
					}
					else{
                        // Validate mailbox size
                        if(m_pApi.ValidateMailboxSize(user)){
                            e.Reply = new SMTP_Reply(552,"Requested mail action aborted: Mailbox <" + e.RcptTo.Mailbox + "> is full.");                            
                        }
                        else{
                            e.Reply = new SMTP_Reply(250,"OK.");
                        }						
					}
				}
				// Foreign recipient.
				else{
                    e.Reply = new SMTP_Reply(550,"Relay not allowed.");

					// This isn't domain what we want.
					// 1)If user Authenticated, check if relay is allowed for this user.
					// 2)Check if relay is allowed for this ip.
					if(e.Session.IsAuthenticated){
						if(IsRelayAllowed(e.Session.AuthenticatedUserIdentity.Name,e.Session.RemoteEndPoint.Address)){
							e.Reply = new SMTP_Reply(250,"User not local will relay.");
						}
					}
					else if(IsRelayAllowed("",e.Session.RemoteEndPoint.Address)){
						e.Reply = new SMTP_Reply(250,"User not local will relay.");
					}		
				}
			}
			catch(Exception x){
                e.Reply = new SMTP_Reply(500,"Internal server error.");
				Error.DumpError(this.Name,x,new System.Diagnostics.StackTrace());
			}
        }

        #endregion

        #region method m_pSmtpServer_Session_GetMessageStream

        /// <summary>
        /// Is raised when SMTP server session needs to get stream where to store incoming message.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_Session_GetMessageStream(object sender,SMTP_e_Message e)
        {
            if(!Directory.Exists(m_MailStorePath + "IncomingSMTP")){
                Directory.CreateDirectory(m_MailStorePath + "IncomingSMTP");
            }
            
            e.Stream = new FileStream(API_Utlis.PathFix(m_MailStorePath + "IncomingSMTP\\" + Guid.NewGuid().ToString().Replace("-","") + ".eml"),FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite,32000,FileOptions.DeleteOnClose); 
            e.Session.Tags["MessageStream"] = e.Stream;
        }

        #endregion

        #region method m_pSmtpServer_Session_MessageStoringCanceled

        /// <summary>
        /// Is called when message storing has canceled.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_Session_MessageStoringCanceled(object sender,EventArgs e)
        {
            try{
                // Close file. .NET will delete that file we use FileOptions.DeleteOnClose.
                ((IDisposable)((SMTP_Session)sender).Tags["MessageStream"]).Dispose();
            }
            catch{
                // We don't care about errors here.
            }            
        }

        #endregion

        #region method m_pSmtpServer_Session_MessageStoringCompleted

        /// <summary>
        /// Is called when SMTP server has completed message storing.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSmtpServer_Session_MessageStoringCompleted(object sender,SMTP_e_MessageStored e)
        {
            try{
                e.Stream.Position = 0;

                ProcessAndStoreMessage(e.Session.From.ENVID,e.Session.From.Mailbox,e.Session.From.RET,e.Session.To,e.Stream,e);
            }
            catch(Exception x){
                Error.DumpError(this.Name,x);

                e.Reply = new SMTP_Reply(552,"Requested mail action aborted: Internal server error.");
            }
            finally{
                // Close file. .NET will delete that file we use FileOptions.DeleteOnClose.
                ((FileStream)e.Stream).Dispose();
            }
        }

        #endregion
                        

		#region method SMTP_Server_SessionLog

        private void SMTP_Server_SessionLog(object sender,LumiSoft.Net.Log.WriteLogEventArgs e)
        {
            Logger.WriteLog(m_SMTP_LogPath + "smtp-" + DateTime.Today.ToString("yyyyMMdd") + ".log",e.LogEntry);
        }		

		#endregion

		#endregion		

        #region Relay Events

        #region method m_pRelayServer_WriteLog

        private void m_pRelayServer_WriteLog(object sender,LumiSoft.Net.Log.WriteLogEventArgs e)
        {
            Logger.WriteLog(m_Relay_LogPath + "relay-" + DateTime.Today.ToString("yyyyMMdd") + ".log",e.LogEntry);
        }

        #endregion

        #endregion

        #region POP3 Events

        #region method m_pPop3Server_SessionCreated

        /// <summary>
        /// Is called when new POP3 server session has created.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pPop3Server_SessionCreated(object sender,TCP_ServerSessionEventArgs<POP3_Session> e)
        {
            e.Session.Started += new EventHandler<POP3_e_Started>(m_pPop3Server_Session_Started);
            e.Session.Authenticate += new EventHandler<POP3_e_Authenticate>(m_pPop3Server_Session_Authenticate);
            e.Session.GetMessagesInfo += new EventHandler<POP3_e_GetMessagesInfo>(m_pPop3Server_Session_GetMessagesInfo);
            e.Session.GetTopOfMessage += new EventHandler<POP3_e_GetTopOfMessage>(m_pPop3Server_Session_GetTopOfMessage);
            e.Session.GetMessageStream += new EventHandler<POP3_e_GetMessageStream>(m_pPop3Server_Session_GetMessageStream);
            e.Session.DeleteMessage += new EventHandler<POP3_e_DeleteMessage>(m_pPop3Server_Session_DeleteMessage);
                        
            // Add session supported authentications.
            if(m_AuthType == MailServerAuthType_enum.Windows || m_AuthType == MailServerAuthType_enum.Ldap){
                // For windows or LDAP auth, we can allow only plain text authentications, because otherwise 
                // we can't do auth against windows (it requires user name and password).

                #region PLAIN

                AUTH_SASL_ServerMechanism_Plain auth_plain = new AUTH_SASL_ServerMechanism_Plain(false);
                auth_plain.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.POP3) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_plain.Name,auth_plain);

                #endregion

                #region LOGIN

                AUTH_SASL_ServerMechanism_Login auth_login = new AUTH_SASL_ServerMechanism_Login(false);
                auth_login.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.POP3) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_login.Name,auth_login);

                #endregion
            }
            else{
                #region DIGEST-MD5

                AUTH_SASL_ServerMechanism_DigestMd5 auth_digestmMd5 = new AUTH_SASL_ServerMechanism_DigestMd5(false);                
                auth_digestmMd5.Realm = e.Session.LocalHostName;
                auth_digestmMd5.GetUserInfo += new EventHandler<AUTH_e_UserInfo>(delegate(object s,AUTH_e_UserInfo e1){
                    // Check that user is allowed to access this service.
                    if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.POP3) == 0){
                        e1.UserExists = false;

                        return;
                    }

                    FillUserInfo(e1);
                });
                e.Session.Authentications.Add(auth_digestmMd5.Name,auth_digestmMd5);

                #endregion

                #region CRAM-MD5

                AUTH_SASL_ServerMechanism_CramMd5 auth_cramMd5 = new AUTH_SASL_ServerMechanism_CramMd5(false);
                auth_cramMd5.GetUserInfo += new EventHandler<AUTH_e_UserInfo>(delegate(object s,AUTH_e_UserInfo e1){
                    // Check that user is allowed to access this service.
                    if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.POP3) == 0){
                        e1.UserExists = false;

                        return;
                    }

                    FillUserInfo(e1);
                });
                e.Session.Authentications.Add(auth_cramMd5.Name,auth_cramMd5);

                #endregion

                #region PLAIN

                AUTH_SASL_ServerMechanism_Plain auth_plain = new AUTH_SASL_ServerMechanism_Plain(false);
                auth_plain.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.POP3) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_plain.Name,auth_plain);

                #endregion

                #region LOGIN

                AUTH_SASL_ServerMechanism_Login auth_login = new AUTH_SASL_ServerMechanism_Login(false);
                auth_login.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.POP3) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_login.Name,auth_login);

                #endregion                                
            }
        }
                                                                                                                                                                
        #endregion

        #region method m_pPop3Server_Session_Started

        private void m_pPop3Server_Session_Started(object sender,POP3_e_Started e)
        {
            if(!IsAccessAllowed(Service_enum.POP3,((POP3_Session)sender).RemoteEndPoint.Address)){
                e.Response = "-ERR Your IP address is blocked.";
            }
        }

        #endregion

        #region method m_pPop3Server_Session_Authenticate

        private void m_pPop3Server_Session_Authenticate(object sender,POP3_e_Authenticate e)
        {
            // Check that user is allowed to access this service
            if((m_pApi.GetUserPermissions(e.User) & UserPermissions_enum.POP3) == 0){
                e.IsAuthenticated = false;

                return;
            }

            e.IsAuthenticated = Authenticate(((POP3_Session)sender).RemoteEndPoint.Address,e.User,e.Password);
        }

        #endregion

        #region method m_pPop3Server_Session_GetMessagesInfo

        private void m_pPop3Server_Session_GetMessagesInfo(object sender,POP3_e_GetMessagesInfo e)
        {
            try{
				string userName = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;

                List<IMAP_MessageInfo> messages = new List<IMAP_MessageInfo>();
                m_pApi.GetMessagesInfo(userName,userName,"Inbox",messages); 
                foreach(IMAP_MessageInfo msgInfo in messages){
                    e.Messages.Add(new POP3_ServerMessage(msgInfo.UID.ToString(),(int)msgInfo.Size,msgInfo.ID));
                }
			}
			catch(Exception x){
				Error.DumpError(this.Name,x,new System.Diagnostics.StackTrace());
			}
        }

        #endregion

        #region method m_pPop3Server_Session_GetTopOfMessage

        private void m_pPop3Server_Session_GetTopOfMessage(object sender,POP3_e_GetTopOfMessage e)
        {
            try{	
			    string userName = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;

				e.Data = m_pApi.GetMessageTopLines(userName,userName,"Inbox",e.Message.Tag.ToString(),e.LineCount);				
			}
			catch(Exception x){
				Error.DumpError(x,new System.Diagnostics.StackTrace());
			}
        }

        #endregion

        #region method m_pPop3Server_Session_GetMessageStream

        private void m_pPop3Server_Session_GetMessageStream(object sender,POP3_e_GetMessageStream e)
        {            
            try{ 
                string userName = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;

                EmailMessageItems eArgs = new EmailMessageItems(e.Message.Tag.ToString(),IMAP_MessageItems_enum.Message);
                m_pApi.GetMessageItems(userName,userName,"Inbox",eArgs);
                
                if(eArgs.MessageStream != null){
                    e.MessageStream = eArgs.MessageStream;
                }
			}
			catch(Exception x){
				Error.DumpError(this.Name,x,new System.Diagnostics.StackTrace());
			}            
        }

        #endregion

        #region method m_pPop3Server_Session_DeleteMessage

        private void m_pPop3Server_Session_DeleteMessage(object sender,POP3_e_DeleteMessage e)
        {
            try{
                string userName = ((POP3_Session)sender).AuthenticatedUserIdentity.Name;

				m_pApi.DeleteMessage(userName,userName,"Inbox",e.Message.Tag.ToString(),Convert.ToInt32(e.Message.UID));
			}
			catch(Exception x){
				Error.DumpError(this.Name,x,new System.Diagnostics.StackTrace());
			}
        }

        #endregion


        #region method POP3_Server_SessionLog

        private void POP3_Server_SessionLog(object sender,LumiSoft.Net.Log.WriteLogEventArgs e)
        {
            Logger.WriteLog(m_POP3_LogPath + "pop3-" + DateTime.Today.ToString("yyyyMMdd") + ".log",e.LogEntry);
        }		

		#endregion
		
		#endregion

		#region IMAP Events

        #region method m_pImapServer_SessionCreated

        /// <summary>
        /// Is called when new POP3 server session has created.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pImapServer_SessionCreated(object sender,TCP_ServerSessionEventArgs<IMAP_Session> e)
        {            
            e.Session.Started += new EventHandler<IMAP_e_Started>(m_pImapServer_Session_Started);
            e.Session.Login += new EventHandler<IMAP_e_Login>(m_pImapServer_Session_Login);
            //--- Authenticated state
            e.Session.Namespace += new EventHandler<IMAP_e_Namespace>(m_pImapServer_Session_Namespace);
            e.Session.LSub += new EventHandler<IMAP_e_LSub>(m_pImapServer_Session_LSub);
            e.Session.Subscribe += new EventHandler<IMAP_e_Folder>(m_pImapServer_Session_Subscribe);
            e.Session.Unsubscribe += new EventHandler<IMAP_e_Folder>(m_pImapServer_Session_Unsubscribe);
            e.Session.List += new EventHandler<IMAP_e_List>(m_pImapServer_Session_List);
            e.Session.Create += new EventHandler<IMAP_e_Folder>(m_pImapServer_Session_Create);
            e.Session.Delete += new EventHandler<IMAP_e_Folder>(m_pImapServer_Session_Delete);
            e.Session.Rename += new EventHandler<IMAP_e_Rename>(m_pImapServer_Session_Rename);
            e.Session.GetQuotaRoot += new EventHandler<IMAP_e_GetQuotaRoot>(m_pImapServer_Session_GetQuotaRoot);
            e.Session.GetQuota += new EventHandler<IMAP_e_GetQuota>(m_pImapServer_Session_GetQuota);
            e.Session.GetAcl += new EventHandler<IMAP_e_GetAcl>(m_pImapServer_Session_GetAcl);
            e.Session.SetAcl += new EventHandler<IMAP_e_SetAcl>(m_pImapServer_Session_SetAcl);
            e.Session.DeleteAcl += new EventHandler<IMAP_e_DeleteAcl>(m_pImapServer_Session_DeleteAcl);
            e.Session.ListRights += new EventHandler<IMAP_e_ListRights>(m_pImapServer_Session_ListRights);
            e.Session.MyRights += new EventHandler<IMAP_e_MyRights>(m_pImapServer_Session_MyRights);
            e.Session.Select += new EventHandler<IMAP_e_Select>(m_pImapServer_Session_Select);
            e.Session.Append += new EventHandler<IMAP_e_Append>(m_pImapServer_Session_Append);
            //--- Selected state
            e.Session.GetMessagesInfo += new EventHandler<IMAP_e_MessagesInfo>(m_pImapServer_Session_GetMessagesInfo);
            e.Session.Search += new EventHandler<IMAP_e_Search>(m_pImapServer_Session_Search);
            e.Session.Fetch += new EventHandler<IMAP_e_Fetch>(m_pImapServer_Session_Fetch);
            e.Session.Expunge += new EventHandler<IMAP_e_Expunge>(m_pImapServer_Session_Expunge);
            e.Session.Store += new EventHandler<IMAP_e_Store>(m_pImapServer_Session_Store);
            e.Session.Copy += new EventHandler<IMAP_e_Copy>(m_pImapServer_Session_Copy);
            
            // Add session supported authentications.
            if(m_AuthType == MailServerAuthType_enum.Windows || m_AuthType == MailServerAuthType_enum.Ldap){
                // For windows or LDAP auth, we can allow only plain text authentications, because otherwise 
                // we can't do auth against windows (it requires user name and password).

                #region PLAIN

                AUTH_SASL_ServerMechanism_Plain auth_plain = new AUTH_SASL_ServerMechanism_Plain(false);
                auth_plain.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.IMAP) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_plain.Name,auth_plain);

                #endregion

                #region LOGIN

                AUTH_SASL_ServerMechanism_Login auth_login = new AUTH_SASL_ServerMechanism_Login(false);
                auth_login.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.IMAP) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_login.Name,auth_login);

                #endregion
            }
            else{
                #region DIGEST-MD5

                AUTH_SASL_ServerMechanism_DigestMd5 auth_digestmMd5 = new AUTH_SASL_ServerMechanism_DigestMd5(false);                
                auth_digestmMd5.Realm = e.Session.LocalHostName;
                auth_digestmMd5.GetUserInfo += new EventHandler<AUTH_e_UserInfo>(delegate(object s,AUTH_e_UserInfo e1){
                    // Check that user is allowed to access this service.
                    if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.IMAP) == 0){
                        e1.UserExists = false;

                        return;
                    }

                    FillUserInfo(e1);
                });
                e.Session.Authentications.Add(auth_digestmMd5.Name,auth_digestmMd5);

                #endregion

                #region CRAM-MD5

                AUTH_SASL_ServerMechanism_CramMd5 auth_cramMd5 = new AUTH_SASL_ServerMechanism_CramMd5(false);
                auth_cramMd5.GetUserInfo += new EventHandler<AUTH_e_UserInfo>(delegate(object s,AUTH_e_UserInfo e1){
                    // Check that user is allowed to access this service.
                    if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.IMAP) == 0){
                        e1.UserExists = false;

                        return;
                    }

                    FillUserInfo(e1);
                });
                e.Session.Authentications.Add(auth_cramMd5.Name,auth_cramMd5);

                #endregion

                #region PLAIN

                AUTH_SASL_ServerMechanism_Plain auth_plain = new AUTH_SASL_ServerMechanism_Plain(false);
                auth_plain.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.IMAP) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_plain.Name,auth_plain);

                #endregion

                #region LOGIN

                AUTH_SASL_ServerMechanism_Login auth_login = new AUTH_SASL_ServerMechanism_Login(false);
                auth_login.Authenticate += new EventHandler<AUTH_e_Authenticate>(delegate(object s,AUTH_e_Authenticate e1){
                    try{
                        // Check that user is allowed to access this service.
                        if((m_pApi.GetUserPermissions(e1.UserName) & UserPermissions_enum.IMAP) == 0){
                            e1.IsAuthenticated = false;

                            return;
                        }

                        e1.IsAuthenticated = Authenticate(e.Session.RemoteEndPoint.Address,e1.UserName,e1.Password);
                    }
                    catch(Exception x){
                        OnError(x);
                        e1.IsAuthenticated = false;
                    }
                });
                e.Session.Authentications.Add(auth_login.Name,auth_login);

                #endregion                                
            }
        }
                                                                                                                                                                                                                                                                                                                                                                                                        
        #endregion

        #region method m_pImapServer_Session_Started

        private void m_pImapServer_Session_Started(object sender,IMAP_e_Started e)
        {
            if(!IsAccessAllowed(Service_enum.IMAP,((IMAP_Session)sender).RemoteEndPoint.Address)){
                e.Response = new IMAP_r_u_ServerStatus("NO","Your IP address is blocked.");
            }
        }

        #endregion

        #region method m_pImapServer_Session_Login

        private void m_pImapServer_Session_Login(object sender,IMAP_e_Login e)
        {
            // Check that user is allowed to access this service
            if((m_pApi.GetUserPermissions(e.UserName) & UserPermissions_enum.IMAP) == 0){
                e.IsAuthenticated = false;

                return;
            }

            e.IsAuthenticated = Authenticate(((IMAP_Session)sender).RemoteEndPoint.Address,e.UserName,e.Password);
        }

        #endregion

            
        #region method m_pImapServer_Session_Namespace

        private void m_pImapServer_Session_Namespace(object sender,IMAP_e_Namespace e)
        {
            SharedFolderRoot[] rootFolders =  m_pApi.GetSharedFolderRoots();
            List<IMAP_Namespace_Entry> publicFolders = new List<IMAP_Namespace_Entry>();
            List<IMAP_Namespace_Entry> usersFolders = new List<IMAP_Namespace_Entry>();
            foreach(SharedFolderRoot rootFolder in rootFolders){
                if(rootFolder.Enabled){
                    if(rootFolder.RootType == SharedFolderRootType_enum.BoundedRootFolder){
                        publicFolders.Add(new IMAP_Namespace_Entry(rootFolder.FolderName,'/'));
                    }
                    else{
                        usersFolders.Add(new IMAP_Namespace_Entry(rootFolder.FolderName,'/'));
                    }
                }
            }
            
            e.NamespaceResponse = new IMAP_r_u_Namespace(new IMAP_Namespace_Entry[]{new IMAP_Namespace_Entry("",'/')},usersFolders.ToArray(),publicFolders.ToArray());
        }

        #endregion

        #region method m_pImapServer_Session_LSub

        private void m_pImapServer_Session_LSub(object sender,IMAP_e_LSub e)
        {
            IMAP_Session ses = (IMAP_Session)sender;
                    
			string[] folders = m_pApi.GetSubscribedFolders(ses.AuthenticatedUserIdentity.Name);
			foreach(string folder in folders){                
                if(string.IsNullOrEmpty(e.FolderReferenceName) || folder.StartsWith(e.FolderReferenceName,StringComparison.InvariantCultureIgnoreCase)){
                    if(!string.IsNullOrEmpty(folder) && FolderMatches(e.FolderFilter,folder)){
                        e.Folders.Add(new IMAP_r_u_LSub(folder,'/',null));
                    }
                }
	        }
        }

        #endregion

        #region method m_pImapServer_Session_Subscribe

        private void m_pImapServer_Session_Subscribe(object sender,IMAP_e_Folder e)
        {
            IMAP_Session ses = (IMAP_Session)sender;

			m_pApi.SubscribeFolder(ses.AuthenticatedUserIdentity.Name,e.Folder);
        }

        #endregion

        #region method m_pImapServer_Session_Unsubscribe

        private void m_pImapServer_Session_Unsubscribe(object sender,IMAP_e_Folder e)
        {
            IMAP_Session ses = (IMAP_Session)sender;

			m_pApi.UnSubscribeFolder(ses.AuthenticatedUserIdentity.Name,e.Folder);
        }

        #endregion

        #region method m_pImapServer_Session_List

        private void m_pImapServer_Session_List(object sender,IMAP_e_List e)
        {
            IMAP_Session ses = (IMAP_Session)sender;
                        
			string[] folders = m_pApi.GetFolders(ses.AuthenticatedUserIdentity.Name,true);
			foreach(string folder in folders){                
                if(string.IsNullOrEmpty(e.FolderReferenceName) || folder.StartsWith(e.FolderReferenceName,StringComparison.InvariantCultureIgnoreCase)){
                    if(FolderMatches(e.FolderFilter,folder)){
                        e.Folders.Add(new IMAP_r_u_List(folder,'/',null));
                    }
                }
			}
        }

        #endregion

        #region method m_pImapServer_Session_Create

        private void m_pImapServer_Session_Create(object sender,IMAP_e_Folder e)
        {
            try{
				IMAP_Session ses = (IMAP_Session)sender;

				m_pApi.CreateFolder(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,e.Folder);
			}
			catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
			}
        }

        #endregion

        #region method m_pImapServer_Session_Delete

        private void m_pImapServer_Session_Delete(object sender,IMAP_e_Folder e)
        {
            try{
				IMAP_Session ses = (IMAP_Session)sender;

				m_pApi.DeleteFolder(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,e.Folder);
			}
			catch(Exception x){
				e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
			}
        }

        #endregion

        #region method m_pImapServer_Session_Rename

        private void m_pImapServer_Session_Rename(object sender,IMAP_e_Rename e)
        {
            try{
				IMAP_Session ses = (IMAP_Session)sender;

				m_pApi.RenameFolder(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,e.CurrentFolder,e.NewFolder);

                e.Response = new IMAP_r_ServerStatus(e.CmdTag,"OK","RENAME command completed.");
			}
			catch(Exception x){
				e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
			}
        }

        #endregion

        #region method m_pImapServer_Session_GetQuotaRoot

        private void m_pImapServer_Session_GetQuotaRoot(object sender,IMAP_e_GetQuotaRoot e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                e.QuotaRootResponses.Add(new IMAP_r_u_QuotaRoot(e.Folder,new string[]{"root"}));

                foreach(DataRowView drv in m_pApi.GetUsers("ALL")){
                    if(drv["UserName"].ToString().ToLower() == ses.AuthenticatedUserIdentity.Name.ToLower()){
                        e.QuotaResponses.Add(new IMAP_r_u_Quota("root",new IMAP_Quota_Entry[]{
                            new IMAP_Quota_Entry("STORAGE",m_pApi.GetMailboxSize(ses.AuthenticatedUserIdentity.Name),(ConvertEx.ToInt32(drv["Mailbox_Size"]) * 1000 * 1000))
                        }));
                        break;
                    }
                }
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_GetQuota

        private void m_pImapServer_Session_GetQuota(object sender,IMAP_e_GetQuota e)
        {   
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                foreach(DataRowView drv in m_pApi.GetUsers("ALL")){
                    if(drv["UserName"].ToString().ToLower() == ses.AuthenticatedUserIdentity.Name.ToLower()){
                        e.QuotaResponses.Add(new IMAP_r_u_Quota(e.QuotaRoot,new IMAP_Quota_Entry[]{
                            new IMAP_Quota_Entry("STORAGE",m_pApi.GetMailboxSize(ses.AuthenticatedUserIdentity.Name),(ConvertEx.ToInt32(drv["Mailbox_Size"]) * 1000 * 1000))
                        }));
                        break;
                    }
                }
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_GetAcl

        private void m_pImapServer_Session_GetAcl(object sender,IMAP_e_GetAcl e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                DataView dv = m_pApi.GetFolderACL(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,e.Folder);
                List<IMAP_Acl_Entry> list = new List<IMAP_Acl_Entry>();
			    foreach(DataRowView drV in dv){
                    list.Add(new IMAP_Acl_Entry(drV["User"].ToString(),drV["Permissions"].ToString()));
			    }
                e.AclResponses.Add(new IMAP_r_u_Acl(e.Folder,list.ToArray()));
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_SetAcl

        private void m_pImapServer_Session_SetAcl(object sender,IMAP_e_SetAcl e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                m_pApi.SetFolderACL(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,e.Folder,e.Identifier,e.FlagsSetType,IMAP_Utils.ACL_From_String(e.Rights));
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_DeleteAcl

        private void m_pImapServer_Session_DeleteAcl(object sender,IMAP_e_DeleteAcl e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                m_pApi.DeleteFolderACL(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,e.Folder,e.Identifier);
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_ListRights

        private void m_pImapServer_Session_ListRights(object sender,IMAP_e_ListRights e)
        {
            try{
                e.ListRightsResponse = new IMAP_r_u_ListRights(e.Folder,e.Identifier,"","l r s w i p c d a");
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_MyRights

        private void m_pImapServer_Session_MyRights(object sender,IMAP_e_MyRights e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                e.MyRightsResponse = new IMAP_r_u_MyRights(
                    e.Folder,
                    IMAP_Utils.ACL_to_String(m_pApi.GetUserACL(ses.AuthenticatedUserIdentity.Name,e.Folder,ses.AuthenticatedUserIdentity.Name))
                );
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_Select

        private void m_pImapServer_Session_Select(object sender,IMAP_e_Select e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;
            
                // FIX ME:
                e.FolderUID = 1237333;
            }
            catch(Exception x){
                e.ErrorResponse = new IMAP_r_ServerStatus(e.CmdTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_Append

        private void m_pImapServer_Session_Append(object sender,IMAP_e_Append e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                e.Stream = new MemoryStreamEx(32000);
                e.Completed += new EventHandler(delegate(object s1,EventArgs e1){
                    e.Stream.Position = 0;

                    m_pApi.StoreMessage(
                        ses.AuthenticatedUserIdentity.Name,
                        ses.AuthenticatedUserIdentity.Name,
                        e.Folder,
                        e.Stream,
                        e.InternalDate == DateTime.MinValue ? DateTime.Now : e.InternalDate,
                        e.Flags
                    );
                });               
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion


        #region method m_pImapServer_Session_GetMessagesInfo

        private void m_pImapServer_Session_GetMessagesInfo(object sender,IMAP_e_MessagesInfo e)
        {
            IMAP_Session ses = (IMAP_Session)sender;

            List<IMAP_MessageInfo> msgs = new List<IMAP_MessageInfo>();
            m_pApi.GetMessagesInfo(
                ses.AuthenticatedUserIdentity.Name,
                ses.AuthenticatedUserIdentity.Name,
                e.Folder,
                msgs
            );
            e.MessagesInfo.AddRange(msgs);            
        }

        #endregion

        #region method m_pImapServer_Session_Search

        private void m_pImapServer_Session_Search(object sender,IMAP_e_Search e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                m_pApi.Search(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,ses.SelectedFolderName,e);
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_Fetch

        private void m_pImapServer_Session_Fetch(object sender,IMAP_e_Fetch e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;
                                                
                foreach(IMAP_MessageInfo msgInfo in e.MessagesInfo){                    
                    if(e.FetchDataType == IMAP_Fetch_DataType.MessageHeader){                
                        EmailMessageItems eArgs = new EmailMessageItems(msgInfo.ID,IMAP_MessageItems_enum.Header);
                        m_pApi.GetMessageItems(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,ses.SelectedFolderName,eArgs);

                        Mail_Message msg = null;
                        try{
                            if(eArgs.MessageExists){
                                msg = Mail_Message.ParseFromByte(eArgs.Header);
                            }
                            else{
                                msg = GenerateMessageMissing();
                            }
                        }
                        catch{
                            msg = API_Utlis.GenerateBadMessage(new MemoryStream(eArgs.Header));
                        }
                        e.AddData(msgInfo,msg);
                    }
                    else if(e.FetchDataType == IMAP_Fetch_DataType.MessageStructure){
                        EmailMessageItems eArgs = new EmailMessageItems(msgInfo.ID,IMAP_MessageItems_enum.Message);                        
                        m_pApi.GetMessageItems(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,ses.SelectedFolderName,eArgs);

                        Mail_Message msg = null;
                        try{
                            if(eArgs.MessageExists){
                                msg = Mail_Message.ParseFromStream(eArgs.MessageStream);
                            }
                            else{
                                msg = GenerateMessageMissing();
                                MemoryStream ms = new MemoryStream(msg.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q,Encoding.UTF8),Encoding.UTF8));
                                eArgs.MessageStream = ms;
                            }
                        }
                        catch{
                            msg = API_Utlis.GenerateBadMessage(eArgs.MessageStream);
                        }
                        e.AddData(msgInfo,msg);
                        eArgs.MessageStream.Close();
                    }
                    else{
                        EmailMessageItems eArgs = new EmailMessageItems(msgInfo.ID,IMAP_MessageItems_enum.Message);
                        m_pApi.GetMessageItems(ses.AuthenticatedUserIdentity.Name,ses.AuthenticatedUserIdentity.Name,ses.SelectedFolderName,eArgs);

                        Mail_Message msg = null;
                        try{
                            if(eArgs.MessageExists){
                                msg = Mail_Message.ParseFromStream(eArgs.MessageStream);
                            }
                            else{
                                msg = GenerateMessageMissing();
                                MemoryStream ms = new MemoryStream(msg.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q,Encoding.UTF8),Encoding.UTF8));
                                eArgs.MessageStream = ms;
                            }
                        }
                        catch{
                            msg = API_Utlis.GenerateBadMessage(eArgs.MessageStream);
                        }
                        e.AddData(msgInfo,msg);
                        eArgs.MessageStream.Close();
                    }                    
                }	
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_Expunge

        private void m_pImapServer_Session_Expunge(object sender,IMAP_e_Expunge e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;
                              
                m_pApi.DeleteMessage(
                    ses.AuthenticatedUserIdentity.Name,
                    ses.AuthenticatedUserIdentity.Name,
                    ses.SelectedFolderName,
                    e.MessageInfo.ID,
                    (int)e.MessageInfo.UID
                );
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_Store

        private void m_pImapServer_Session_Store(object sender,IMAP_e_Store e)
        {
            try{
                IMAP_Session ses = (IMAP_Session)sender;

                List<string> flags = new List<string>();
                if(e.FlagsSetType == IMAP_Flags_SetType.Add){
                    flags.AddRange(IMAP_Utils.MessageFlagsAdd(e.MessageInfo.Flags,e.Flags));
                }
                else if(e.FlagsSetType == IMAP_Flags_SetType.Remove){
                    flags.AddRange(IMAP_Utils.MessageFlagsRemove(e.MessageInfo.Flags,e.Flags));
                }
                else{
                    flags.AddRange(e.Flags);
                }
                             
                m_pApi.StoreMessageFlags(
                    ses.AuthenticatedUserIdentity.Name,
                    ses.AuthenticatedUserIdentity.Name,
                    ses.SelectedFolderName,
                    e.MessageInfo,
                    flags.ToArray()
                );
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }
        }

        #endregion

        #region method m_pImapServer_Session_Copy

        private void m_pImapServer_Session_Copy(object sender,IMAP_e_Copy e)
        {
            /* RFC 3501 6.4.7. COPY Command.
                If the COPY command is unsuccessful for any reason, server
                implementations MUST restore the destination mailbox to its state
                before the COPY attempt.
            */

            try{
                IMAP_Session ses = (IMAP_Session)sender;                  

                List<IMAP_MessageInfo> copiedMessageInfos = new List<IMAP_MessageInfo>();
                try{
                    foreach(IMAP_MessageInfo msgInfo in e.MessagesInfo){
                        m_pApi.CopyMessage(
                            ses.AuthenticatedUserIdentity.Name,
                            ses.AuthenticatedUserIdentity.Name,
                            e.SourceFolder,
                            ses.AuthenticatedUserIdentity.Name,
                            e.TargetFolder,
                            msgInfo
                        );
                        copiedMessageInfos.Add(msgInfo);
                    }
                }
                catch(Exception x){
                    // Delete copied messages.
                    foreach(IMAP_MessageInfo msgInfo in copiedMessageInfos){
                        try{
                            m_pApi.DeleteMessage(
                                ses.AuthenticatedUserIdentity.Name,
                                ses.AuthenticatedUserIdentity.Name,
                                e.TargetFolder,
                                msgInfo.ID,
                                (int)msgInfo.UID
                            );
                        }
                        catch{
                            // We don't care about deleting errors here.
                        }
                    }

                    throw x;
                }
            }
            catch(Exception x){
                e.Response = new IMAP_r_ServerStatus(e.Response.CommandTag,"NO","Error: " + x.Message);
            }            
        }

        #endregion

        
        #region method IMAP_Server_SessionLog

        private void IMAP_Server_SessionLog(object sender,LumiSoft.Net.Log.WriteLogEventArgs e)
        {
            Logger.WriteLog(m_IMAP_LogPath + "imap-" + DateTime.Today.ToString("yyyyMMdd") + ".log",e.LogEntry);
        }		

		#endregion

		#endregion

        #region SIP Events

        #region method m_pSipServer_Authenticate

        /// <summary>
        /// This method is called by SIP proxy when it need to authenticate specified user.
        /// </summary>
        /// <param name="e">Event data.</param>
        private void m_pSipServer_Authenticate(SIP_AuthenticateEventArgs e)
        {
            // TODO: change api to return 1 user
        
            foreach(DataRowView drV in m_pApi.GetUsers("ALL")){
                if(e.AuthContext.UserName.ToLower() == drV["UserName"].ToString().ToLower()){
                    // Check that user has SIP access.
                    if(((UserPermissions_enum)Convert.ToInt32(drV["Permissions"]) & UserPermissions_enum.SIP) == 0){
                        return;
                    }
                
                    e.Authenticated = e.AuthContext.Authenticate(drV["UserName"].ToString(),drV["Password"].ToString());
                    return;
                }
            }

            e.Authenticated = false;
        }

        #endregion

        #region method m_pSipServer_IsLocalUri

        private bool m_pSipServer_IsLocalUri(string uri)
        {
            // TODO: get domain

            return m_pApi.DomainExists(uri);
        }

        #endregion

        #region method m_pSipServer_AddressExists

        private bool m_pSipServer_AddressExists(string address)
        {
            return m_pApi.MapUser(address) != null;
        }

        #endregion

        #region method m_pSipServer_CanRegister

        private bool m_pSipServer_CanRegister(string userName,string address)
        {
            foreach(DataRowView drV in m_pApi.GetUserAddresses(userName)){
                if(drV["Address"].ToString().ToLower() == address.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method m_pSipServer_Error

        /// <summary>
        /// This method is called when unhandeld SIP server error happens.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pSipServer_Error(object sender,ExceptionEventArgs e)
        {
            OnError(e.Exception);
        }

        #endregion

        #endregion

        #region Common Events

		#region method OnServer_SysError

		private void OnServer_SysError(object sender,LumiSoft.Net.Error_EventArgs e)
		{
			OnError(e.Exception);
		}

		#endregion

		#endregion



        #region method m_pTimer_Elapsed

        private void m_pTimer_Elapsed(object sender,System.Timers.ElapsedEventArgs e)
        {
            try{
				LoadSettings();				
			}
			catch(Exception x){
                OnError(x);
			}
        }

        #endregion

        #endregion


        #region method Start

        /// <summary>
        /// Starts this virtual server.
        /// </summary>
        public void Start()
        {
            if(m_Running){
                return;
            }
            m_Running = true;

            m_pDnsClient = new Dns_Client();

            m_pSmtpServer = new SMTP_Server();
            m_pSmtpServer.Error += new LumiSoft.Net.ErrorEventHandler(OnServer_SysError);
            m_pSmtpServer.SessionCreated += new EventHandler<TCP_ServerSessionEventArgs<SMTP_Session>>(m_pSmtpServer_SessionCreated);

            m_pPop3Server = new POP3_Server();            
            m_pPop3Server.Error += new LumiSoft.Net.ErrorEventHandler(OnServer_SysError);            
            m_pPop3Server.SessionCreated += new EventHandler<TCP_ServerSessionEventArgs<POP3_Session>>(m_pPop3Server_SessionCreated);
                        
            m_pImapServer = new IMAP_Server();
            m_pImapServer.Error += new LumiSoft.Net.ErrorEventHandler(OnServer_SysError);
            m_pImapServer.SessionCreated += new EventHandler<TCP_ServerSessionEventArgs<IMAP_Session>>(m_pImapServer_SessionCreated);
            
            m_pRelayServer = new RelayServer(this);
            m_pRelayServer.DnsClient = m_pDnsClient;

            m_pFetchServer = new FetchPop3(this,m_pApi);

            m_pSipServer = new SIP_Proxy(new SIP_Stack());
            m_pSipServer.Authenticate += new SIP_AuthenticateEventHandler(m_pSipServer_Authenticate);
            m_pSipServer.IsLocalUri += new SIP_IsLocalUriEventHandler(m_pSipServer_IsLocalUri);
            m_pSipServer.AddressExists += new SIP_AddressExistsEventHandler(m_pSipServer_AddressExists);
            m_pSipServer.Registrar.CanRegister += new SIP_CanRegisterEventHandler(m_pSipServer_CanRegister);
            m_pSipServer.Stack.Error += new EventHandler<ExceptionEventArgs>(m_pSipServer_Error);
            
            m_pRecycleBinManager = new RecycleBinManager(m_pApi);
 
            m_pBadLoginManager = new BadLoginManager();

            m_pTimer = new System.Timers.Timer();            
            m_pTimer.Interval = 15000;
            m_pTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_pTimer_Elapsed);
            m_pTimer.Enabled = true;

            LoadSettings();
        }
                                                                                                                                                        
        #endregion

        #region method Stop

        /// <summary>
        /// Stops this virtual server.
        /// </summary>
        public void Stop()
        {
            m_Running = false;

            if(m_pDnsClient != null){
                m_pDnsClient.Dispose();
                m_pDnsClient = null;
            }
            if(m_pSmtpServer != null){
                try{
                    m_pSmtpServer.Dispose();
                }
                catch{
                }
                m_pSmtpServer = null;
            }
            if(m_pPop3Server != null){
                try{
                    m_pPop3Server.Dispose();
                }
                catch{
                }
                m_pPop3Server = null;
            }
            if(m_pImapServer != null){
                try{
                    m_pImapServer.Dispose();
                }
                catch{
                }
                m_pImapServer = null;
            }
            if(m_pRelayServer != null){
                try{
                    m_pRelayServer.Dispose();
                }
                catch{
                }
                m_pRelayServer = null;
            }
            if(m_pFetchServer != null){
                try{
                    m_pFetchServer.Dispose();                    
                }
                catch{
                }
                m_pFetchServer = null;
            } 
            if(m_pSipServer != null){
                try{
                    m_pSipServer.Stack.Stop();                    
                }
                catch{
                }
                m_pSipServer = null;
            } 
            if(m_pTimer != null){
                try{
                    m_pTimer.Dispose();
                }
                catch{
                }
                m_pTimer = null;
            }
            if(m_pRecycleBinManager != null){
                try{
                    m_pRecycleBinManager.Dispose();
                }
                catch{
                }
                m_pRecycleBinManager = null;
            }
            if(m_pBadLoginManager != null){
                try{
                    m_pBadLoginManager.Dispose();
                }
                catch{
                }
                m_pBadLoginManager = null;
            }
        }

        #endregion


        #region method Authenticate

        /// <summary>
        /// Authenticates specified user.
        /// </summary>
        /// <param name="ip">IP address of remote computer.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns true if user authenticated sucessfully, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> or <b>userName</b> is null reference.</exception>
        public bool Authenticate(IPAddress ip,string userName,string password)
        {
            if(ip == null){
                throw new ArgumentNullException("ip");
            }
            if(userName == null){
                throw new ArgumentNullException("userName");
            }
          
            try{ 
                // See if too many bad logins for specified IP and user if so block auth.
                if(m_pBadLoginManager.IsExceeded(ip.ToString(),userName)){
                    return false;
                }

                bool validated = false;            
                // Integrated auth
                if(m_AuthType == MailServerAuthType_enum.Integrated){
                    foreach(DataRowView dr in m_pApi.GetUsers("ALL")){
                        if(userName.ToLowerInvariant() == dr["UserName"].ToString().ToLowerInvariant()){
                            if(password == dr["Password"].ToString()){
                                validated = true;
                            }
                            break;
                        }
                    }
                }
                // Windows auth
                else if(m_AuthType == MailServerAuthType_enum.Windows){    
                    if(m_pApi.UserExists(userName)){
                        validated = WinLogon.Logon(m_Auth_Win_Domain,userName,password);
                    }
                }
                // LDAP auth
                else if(m_AuthType == MailServerAuthType_enum.Ldap){
                    try{
                        string dn = m_Auth_LDAP_DN.Replace("%user",userName);

                        using(LdapConnection ldap = new LdapConnection(new LdapDirectoryIdentifier(m_Auth_LDAP_Server),new System.Net.NetworkCredential(dn,password),System.DirectoryServices.Protocols.AuthType.Basic)){
                            ldap.SessionOptions.ProtocolVersion = 3;
                            ldap.Bind();
                        }

                        validated = true;
                    }
                    catch{
                    }
                }
                                               						
			    // Increase bad login info
			    if(!validated){
                    m_pBadLoginManager.Put(ip.ToString(),userName);
			    }

                // Update user last login
                if(validated){
                    m_pApi.UpdateUserLastLoginTime(userName);
                }

			    return validated;
            }
            catch(Exception x){
                OnError(x);
                return false;
            }
        }

        #endregion


        #region method LoadSettings

        private void LoadSettings()
		{
			try{
				lock(this){
					DataRow dr = m_pApi.GetSettings();

                    // See if settings changed. Skip loading if steeings has not changed.
                    if(Convert.ToDateTime(dr["SettingsDate"]).Equals(m_SettingsDate)){
                        return;
                    }
                    m_SettingsDate = Convert.ToDateTime(dr["SettingsDate"]);
                                  
                    //--- Try to get mailstore path from API init string ----------------------------------//
                    m_MailStorePath = "Settings\\MailStore";
                    // mailstorepath=
			        string[] parameters = m_ApiInitString.Replace("\r\n","\n").Split('\n');
			        foreach(string param in parameters){
                        if(param.ToLower().IndexOf("mailstorepath=") > -1){
					        m_MailStorePath = param.Substring(14);
                        }
			        }
                    // Fix mail store path, if isn't ending with \
			        if(m_MailStorePath.Length > 0 && !m_MailStorePath.EndsWith("\\")){
				        m_MailStorePath += "\\"; 
			        }
                    if(!Path.IsPathRooted(m_MailStorePath)){
				        m_MailStorePath = m_pOwnerServer.StartupPath + m_MailStorePath;
			        }
                    // Make path directory separator to suit for current platform
                    m_MailStorePath = API_Utlis.PathFix(m_MailStorePath);                    
                    //------------------------------------------------------------------------------------//

                    //--- System settings -----------------------------------------------------------------//
                    m_AuthType         = (MailServerAuthType_enum)ConvertEx.ToInt32(dr["ServerAuthenticationType"]);
                    m_Auth_Win_Domain  = ConvertEx.ToString(dr["ServerAuthWinDomain"]);
                    m_Auth_LDAP_Server = ConvertEx.ToString(dr["LdapServer"]);
                    m_Auth_LDAP_DN     = ConvertEx.ToString(dr["LdapDN"]);
                    //-------------------------------------------------------------------------------------//

                    #region General

                    List<string> dnsServers = new List<string>();
                    foreach(DataRow drX in dr.Table.DataSet.Tables["DnsServers"].Rows){
                        dnsServers.Add(drX["IP"].ToString());
                    }
                    LumiSoft.Net.DNS.Client.Dns_Client.DnsServers = dnsServers.ToArray();

                    #endregion

                    #region SMTP

                    //------- SMTP Settings ---------------------------------------------//
                    try{
                        List<IPBindInfo> smtpIpBinds = new List<IPBindInfo>();
                        foreach(DataRow dr_Bind in dr.Table.DataSet.Tables["SMTP_Bindings"].Rows){
                            smtpIpBinds.Add(new IPBindInfo(
                                ConvertEx.ToString(dr_Bind["HostName"]),
                                IPAddress.Parse(ConvertEx.ToString(dr_Bind["IP"])),
                                ConvertEx.ToInt32(dr_Bind["Port"]),
                                ParseSslMode(dr_Bind["SSL"].ToString()),
                                PaseCertificate(dr_Bind["SSL_Certificate"])
                            ));
                        }                        
                        m_pSmtpServer.Bindings = smtpIpBinds.ToArray();                        
					    m_pSmtpServer.MaxConnections      = ConvertEx.ToInt32(dr["SMTP_Threads"]);
                        m_pSmtpServer.MaxConnectionsPerIP = ConvertEx.ToInt32(dr["SMTP_MaxConnectionsPerIP"]);
					    m_pSmtpServer.SessionIdleTimeout  = ConvertEx.ToInt32(dr["SMTP_SessionIdleTimeOut"]);
					    m_pSmtpServer.MaxMessageSize      = ConvertEx.ToInt32(dr["MaxMessageSize"]) * 1000000;       // Mb to byte.
					    m_pSmtpServer.MaxRecipients       = ConvertEx.ToInt32(dr["MaxRecipients"]);
					    m_pSmtpServer.MaxBadCommands      = ConvertEx.ToInt32(dr["SMTP_MaxBadCommands"]);
                        m_pSmtpServer.MaxTransactions     = ConvertEx.ToInt32(dr["SMTP_MaxTransactions"]);
                        m_pSmtpServer.GreetingText        = ConvertEx.ToString(dr["SMTP_GreetingText"]);                        
                        m_pSmtpServer.ServiceExtentions   = new string[]{
                            SMTP_ServiceExtensions.PIPELINING,
                            SMTP_ServiceExtensions.SIZE,
                            SMTP_ServiceExtensions.STARTTLS,
                            SMTP_ServiceExtensions._8BITMIME,
                            SMTP_ServiceExtensions.BINARYMIME,
                            SMTP_ServiceExtensions.CHUNKING,
                            SMTP_ServiceExtensions.DSN
                        };
                        m_SMTP_RequireAuth                = ConvertEx.ToBoolean(dr["SMTP_RequireAuth"]);					
					    m_SMTP_DefaultDomain              = ConvertEx.ToString(dr["SMTP_DefaultDomain"]);                        
                        if(ConvertEx.ToBoolean(dr["SMTP_Enabled"])){
                            m_pSmtpServer.Start();
                        }
                        else{
                            m_pSmtpServer.Stop();
                        }
                    }
                    catch(Exception x){
                        OnError(x);
                    }
                    //-------------------------------------------------------------------//

                    #endregion

                    #region POP3

                    //------- POP3 Settings -------------------------------------//
                    try{
					    List<IPBindInfo> pop3IpBinds = new List<IPBindInfo>();
                        foreach(DataRow dr_Bind in dr.Table.DataSet.Tables["POP3_Bindings"].Rows){
                            pop3IpBinds.Add(new IPBindInfo(
                                ConvertEx.ToString(dr_Bind["HostName"]),
                                IPAddress.Parse(ConvertEx.ToString(dr_Bind["IP"])),
                                ConvertEx.ToInt32(dr_Bind["Port"]),
                                ParseSslMode(dr_Bind["SSL"].ToString()),
                                PaseCertificate(dr_Bind["SSL_Certificate"])
                            ));
                        }
                        m_pPop3Server.Bindings = pop3IpBinds.ToArray();
					    m_pPop3Server.MaxConnections      = ConvertEx.ToInt32(dr["POP3_Threads"]);
                        m_pPop3Server.MaxConnectionsPerIP = ConvertEx.ToInt32(dr["POP3_MaxConnectionsPerIP"]);
					    m_pPop3Server.SessionIdleTimeout  = ConvertEx.ToInt32(dr["POP3_SessionIdleTimeOut"]);
					    m_pPop3Server.MaxBadCommands      = ConvertEx.ToInt32(dr["POP3_MaxBadCommands"]);
                        m_pPop3Server.GreetingText        = ConvertEx.ToString(dr["POP3_GreetingText"]);
                        if(ConvertEx.ToBoolean(dr["POP3_Enabled"])){
                            m_pPop3Server.Start();
                        }
                        else{
                            m_pPop3Server.Stop();
                        }
                    }
                    catch(Exception x){
                        OnError(x);
                    }
                    //-----------------------------------------------------------//

                    #endregion

                    #region IMAP

                    //------- IMAP Settings -------------------------------------//
                    try{
					    List<IPBindInfo> imapIpBinds = new List<IPBindInfo>();
                        foreach(DataRow dr_Bind in dr.Table.DataSet.Tables["IMAP_Bindings"].Rows){
                            imapIpBinds.Add(new IPBindInfo(
                                ConvertEx.ToString(dr_Bind["HostName"]),
                                IPAddress.Parse(ConvertEx.ToString(dr_Bind["IP"])),
                                ConvertEx.ToInt32(dr_Bind["Port"]),
                                ParseSslMode(dr_Bind["SSL"].ToString()),
                                PaseCertificate(dr_Bind["SSL_Certificate"])
                            ));
                        }
                        m_pImapServer.Bindings = imapIpBinds.ToArray();
					    m_pImapServer.MaxConnections      = ConvertEx.ToInt32(dr["IMAP_Threads"]);
                        m_pImapServer.MaxConnectionsPerIP = ConvertEx.ToInt32(dr["IMAP_Threads"]);
					    m_pImapServer.SessionIdleTimeout  = ConvertEx.ToInt32(dr["IMAP_SessionIdleTimeOut"]);
					    m_pImapServer.MaxBadCommands      = ConvertEx.ToInt32(dr["IMAP_MaxBadCommands"]);                        
                        m_pImapServer.GreetingText = ConvertEx.ToString(dr["IMAP_GreetingText"]);
					    if(ConvertEx.ToBoolean(dr["IMAP_Enabled"])){
                            m_pImapServer.Start();
                        }
                        else{
                            m_pImapServer.Stop();
                        }
                    }
                    catch(Exception x){
                        OnError(x);
                    }
                    //-----------------------------------------------------------//

                    #endregion

                    #region Relay

                    //------- Relay ----------------------------------------------
                    try{
                        List<IPBindInfo> relayIpBinds = new List<IPBindInfo>();
                        foreach(DataRow dr_Bind in dr.Table.DataSet.Tables["Relay_Bindings"].Rows){
                            relayIpBinds.Add(new IPBindInfo(
                                ConvertEx.ToString(dr_Bind["HostName"]),
                                IPAddress.Parse(ConvertEx.ToString(dr_Bind["IP"])),
                                0,
                                SslMode.None,
                                null
                            ));
                        }

                        List<Relay_SmartHost> relaySmartHosts = new List<Relay_SmartHost>();
                        foreach(DataRow drX in dr.Table.DataSet.Tables["Relay_SmartHosts"].Rows){
                            relaySmartHosts.Add(new Relay_SmartHost(
                                ConvertEx.ToString(drX["Host"]),
                                ConvertEx.ToInt32(drX["Port"]),
                                (SslMode)Enum.Parse(typeof(SslMode),drX["SslMode"].ToString()),
                                ConvertEx.ToString(drX["UserName"]),
                                ConvertEx.ToString(drX["Password"])
                            ));
                        }
                        
                        m_pRelayServer.RelayMode = (Relay_Mode)Enum.Parse(typeof(Relay_Mode),dr["Relay_Mode"].ToString());
                        m_pRelayServer.SmartHostsBalanceMode = (BalanceMode)Enum.Parse(typeof(BalanceMode),dr["Relay_SmartHostsBalanceMode"].ToString());
                        m_pRelayServer.SmartHosts = relaySmartHosts.ToArray();
                        m_pRelayServer.SessionIdleTimeout = ConvertEx.ToInt32(dr["Relay_SessionIdleTimeOut"]);
                        m_pRelayServer.MaxConnections = ConvertEx.ToInt32(dr["MaxRelayThreads"]);
                        m_pRelayServer.MaxConnectionsPerIP = ConvertEx.ToInt32(dr["Relay_MaxConnectionsPerIP"]);    
                        m_pRelayServer.RelayInterval = ConvertEx.ToInt32(dr["RelayInterval"]);
                        m_pRelayServer.RelayRetryInterval = ConvertEx.ToInt32(dr["RelayRetryInterval"]);
                        m_pRelayServer.DelayedDeliveryNotifyAfter = ConvertEx.ToInt32(dr["RelayUndeliveredWarning"]);
                        m_pRelayServer.UndeliveredAfter = ConvertEx.ToInt32(dr["RelayUndelivered"]) * 60;
                        m_pRelayServer.DelayedDeliveryMessage = null;
                        m_pRelayServer.UndeliveredMessage = null;
                        foreach(DataRow drReturnMessage in dr.Table.DataSet.Tables["ServerReturnMessages"].Rows){
                            if(drReturnMessage["MessageType"].ToString() == "delayed_delivery_warning"){
                                m_pRelayServer.DelayedDeliveryMessage = new ServerReturnMessage(drReturnMessage["Subject"].ToString(),drReturnMessage["BodyTextRtf"].ToString());
                            }
                            else if(drReturnMessage["MessageType"].ToString() == "undelivered"){
                                m_pRelayServer.UndeliveredMessage = new ServerReturnMessage(drReturnMessage["Subject"].ToString(),drReturnMessage["BodyTextRtf"].ToString());
                            }                            
                        }
                        m_pRelayServer.Bindings = relayIpBinds.ToArray();
                        if(ConvertEx.ToBoolean(dr["LogRelayCmds"])){
                            m_pRelayServer.Logger = new LumiSoft.Net.Log.Logger();
                            m_pRelayServer.Logger.WriteLog += new EventHandler<LumiSoft.Net.Log.WriteLogEventArgs>(m_pRelayServer_WriteLog);
                        }
                        else{
                            if(m_pRelayServer.Logger != null){
                                m_pRelayServer.Logger.Dispose();
                                m_pRelayServer.Logger = null;
                            }                            
                        }               
                        if(dr["Relay_LogPath"].ToString().Length == 0){
						    m_Relay_LogPath = m_pOwnerServer.StartupPath + "Logs\\Relay\\";
					    }
					    else{
						    m_Relay_LogPath = dr["Relay_LogPath"].ToString() + "\\";
					    }
                        m_pRelayServer.StoreUndeliveredMessages = ConvertEx.ToBoolean(dr["StoreUndeliveredMessages"]);
                        m_pRelayServer.UseTlsIfPossible = ConvertEx.ToBoolean(dr["Relay_UseTlsIfPossible"]);
                        if(!m_pRelayServer.IsRunning){
                            m_pRelayServer.Start();
                        }
                    }
                    catch(Exception x){
                        OnError(x);
                    }
                    //------------------------------------------------------------

                    #endregion

                    #region FETCH

                    //----- Fetch POP3 settings ----------------------------------//
                    try{
                        m_pFetchServer.Enabled       = ConvertEx.ToBoolean(dr["FetchPop3_Enabled"]);
                        m_pFetchServer.FetchInterval = ConvertEx.ToInt32(dr["FetchPOP3_Interval"]);
                    }                    
                    catch(Exception x){
                        OnError(x);
                    }
                    //------------------------------------------------------------//

                    #endregion

                    #region SIP

                    List<IPBindInfo> sipIpBinds = new List<IPBindInfo>();
                    foreach(DataRow dr_Bind in dr.Table.DataSet.Tables["SIP_Bindings"].Rows){
                        if(dr_Bind["Protocol"].ToString().ToUpper() == "TCP"){
                            sipIpBinds.Add(new IPBindInfo(
                                ConvertEx.ToString(dr_Bind["HostName"]),
                                IPAddress.Parse(ConvertEx.ToString(dr_Bind["IP"])),
                                ConvertEx.ToInt32(dr_Bind["Port"]),
                                ParseSslMode(dr_Bind["SSL"].ToString()),
                                PaseCertificate(dr_Bind["SSL_Certificate"])
                            ));
                        }
                        else{
                            sipIpBinds.Add(new IPBindInfo(
                                ConvertEx.ToString(dr_Bind["HostName"]),
                                BindInfoProtocol.UDP,
                                IPAddress.Parse(ConvertEx.ToString(dr_Bind["IP"])),
                                ConvertEx.ToInt32(dr_Bind["Port"])
                            ));
                        }
                    }
                    m_pSipServer.Stack.BindInfo = sipIpBinds.ToArray();

                    if(ConvertEx.ToBoolean(dr["SIP_Enabled"])){
                        m_pSipServer.Stack.Start();
                    }
                    else{
                        m_pSipServer.Stack.Stop();
                    }
                    m_pSipServer.Stack.MinimumExpireTime = ConvertEx.ToInt32(dr["SIP_MinExpires"]);
                    m_pSipServer.ProxyMode               = (SIP_ProxyMode)Enum.Parse(typeof(SIP_ProxyMode),dr["SIP_ProxyMode"].ToString());

                    #endregion

                    #region LOGGING

                    //----- Logging settings -------------------------------------//
                    try{
                        if(ConvertEx.ToBoolean(dr["LogSMTPCmds"],false)){                            
                            m_pSmtpServer.Logger = new LumiSoft.Net.Log.Logger();
                            m_pSmtpServer.Logger.WriteLog += new EventHandler<LumiSoft.Net.Log.WriteLogEventArgs>(SMTP_Server_SessionLog);
                        }
                        else{
                            m_pSmtpServer.Logger = null;
                        }
                        if(ConvertEx.ToBoolean(dr["LogPOP3Cmds"],false)){                            
                            m_pPop3Server.Logger = new LumiSoft.Net.Log.Logger();
                            m_pPop3Server.Logger.WriteLog += new EventHandler<LumiSoft.Net.Log.WriteLogEventArgs>(POP3_Server_SessionLog);
                        }
                        else{
                            m_pPop3Server.Logger = null;
                        }
                        if(ConvertEx.ToBoolean(dr["LogIMAPCmds"],false)){                            
                            m_pImapServer.Logger = new LumiSoft.Net.Log.Logger();
                            m_pImapServer.Logger.WriteLog += new EventHandler<LumiSoft.Net.Log.WriteLogEventArgs>(IMAP_Server_SessionLog);
                        }
                        else{
                            m_pImapServer.Logger = null;
                        }
					    m_pFetchServer.LogCommands = ConvertEx.ToBoolean(dr["LogFetchPOP3Cmds"],false);
    					
					    m_SMTP_LogPath   = API_Utlis.PathFix(ConvertEx.ToString(dr["SMTP_LogPath"]) + "\\");
					    m_POP3_LogPath   = API_Utlis.PathFix(ConvertEx.ToString(dr["POP3_LogPath"]) + "\\");
					    m_IMAP_LogPath   = API_Utlis.PathFix(ConvertEx.ToString(dr["IMAP_LogPath"]) + "\\");
					    m_Server_LogPath = API_Utlis.PathFix(ConvertEx.ToString(dr["Server_LogPath"]) + "\\");
					    m_Fetch_LogPath  = API_Utlis.PathFix(ConvertEx.ToString(dr["FetchPOP3_LogPath"]) + "\\");
    					
					    //----- If no log path, use default ----------------
					    if(dr["SMTP_LogPath"].ToString().Trim().Length == 0){
						    m_SMTP_LogPath = API_Utlis.PathFix(m_pOwnerServer.StartupPath + "Logs\\SMTP\\");
					    }
					    if(dr["POP3_LogPath"].ToString().Trim().Length == 0){
						    m_POP3_LogPath = API_Utlis.PathFix(m_pOwnerServer.StartupPath + "Logs\\POP3\\");
					    }
					    if(dr["IMAP_LogPath"].ToString().Trim().Length == 0){
						    m_IMAP_LogPath = API_Utlis.PathFix(m_pOwnerServer.StartupPath + "Logs\\IMAP\\");
					    }
					    if(dr["Server_LogPath"].ToString().Trim().Length == 0){
						    m_Server_LogPath = API_Utlis.PathFix(m_pOwnerServer.StartupPath + "Logs\\Server\\");
					    }					
					    if(dr["FetchPOP3_LogPath"].ToString().Trim().Length == 0){
						    m_Fetch_LogPath = API_Utlis.PathFix(m_pOwnerServer.StartupPath + "Logs\\FetchPOP3\\");
					    }
					    m_pFetchServer.LogPath = m_Fetch_LogPath;
				    }
                    catch(Exception x){
                        OnError(x);
                    }
					//------------------------------------------------------------//

                    #endregion

			//		SCore.WriteLog(m_Server_LogPath + "server.log","//---- Server settings loaded " + DateTime.Now);
				}
			}
			catch(Exception x){
				Error.DumpError(x,new System.Diagnostics.StackTrace());
			}
		}                
                
		#endregion

        #region method IsRelayAllowed

		/// <summary>
		/// Checks if relay is allowed to specified User/IP.
        /// First user 'Allow Relay' checked, if not allowed, then checked if relay denied for that IP,
        /// at last checks if relay is allowed for that IP.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="ip"></param>
		/// <returns>Returns true if relay is allowed.</returns>
		private bool IsRelayAllowed(string userName,IPAddress ip)
		{
			if(userName != null && userName.Length > 0){
                if((m_pApi.GetUserPermissions(userName) & UserPermissions_enum.Relay) != 0){
                    return true;
                }
            }
            			
            using(DataView dv = m_pApi.GetSecurityList()){
                // Check if ip is denied
                foreach(DataRowView drV in dv){
                    if(Convert.ToBoolean(drV["Enabled"]) && Convert.ToInt32(drV["Service"]) == (int)Service_enum.Relay && Convert.ToInt32(drV["Action"]) == (int)IPSecurityAction_enum.Deny){
                        // See if IP matches range
                        if(Net_Utils.CompareIP(IPAddress.Parse(drV["StartIP"].ToString()),ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(drV["EndIP"].ToString()),ip) <= 0){
                            return false;
                        }
                    }
                }

                // Check if ip is allowed
                foreach(DataRowView drV in dv){
                    if(Convert.ToBoolean(drV["Enabled"]) && Convert.ToInt32(drV["Service"]) == (int)Service_enum.Relay && Convert.ToInt32(drV["Action"]) == (int)IPSecurityAction_enum.Allow){
                        // See if IP matches range
                        if(Net_Utils.CompareIP(IPAddress.Parse(drV["StartIP"].ToString()),ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(drV["EndIP"].ToString()),ip) <= 0){
                            return true;
                        }
                    }
                }
            }

			return false;
		}
		
		#endregion

        #region method IsAccessAllowed

		/// <summary>
		/// Checks if specified service access is allowed for specified IP.
		/// </summary>
		/// <param name="service">SMTP or POP3 or IMAP.</param>
		/// <param name="ip"></param>
		/// <returns>Returns true if allowed.</returns>
		public bool IsAccessAllowed(Service_enum service,IPAddress ip)
		{
			using(DataView dv = m_pApi.GetSecurityList()){
                // Check if ip is denied
                foreach(DataRowView drV in dv){
                    if(Convert.ToBoolean(drV["Enabled"]) && Convert.ToInt32(drV["Service"]) == (int)service && Convert.ToInt32(drV["Action"]) == (int)IPSecurityAction_enum.Deny){
                        // See if IP matches range
                        if(Net_Utils.CompareIP(IPAddress.Parse(drV["StartIP"].ToString()),ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(drV["EndIP"].ToString()),ip) <= 0){
                            return false;
                        }
                    }
                }

                // Check if ip is allowed
                foreach(DataRowView drV in dv){
                    if(Convert.ToBoolean(drV["Enabled"]) && Convert.ToInt32(drV["Service"]) == (int)service && Convert.ToInt32(drV["Action"]) == (int)IPSecurityAction_enum.Allow){
                        // See if IP matches range
                        if(Net_Utils.CompareIP(IPAddress.Parse(drV["StartIP"].ToString()),ip) >= 0 && Net_Utils.CompareIP(IPAddress.Parse(drV["EndIP"].ToString()),ip) <= 0){
                            return true;
                        }
                    }
                }
            }
            
            return false;
		}
		
		#endregion
        
        #region method PaseCertificate

        /// <summary>
        /// Parses x509 certificate from specified data. Returns null if no certificate to load.
        /// </summary>
        /// <param name="cert">Certificate data.</param>
        /// <returns>Returns parse certificate or null if no certificate.</returns>
        private X509Certificate2 PaseCertificate(object cert)
        {
            if(cert == null){
                return null;
            }
            if(cert == DBNull.Value){
                return null;
            }
            else{
                /* NOTE: MS X509Certificate2((byte[])) has serious bug, it will create temp file
                 * and leaves it open. The result is temp folder will get full.
                */
                String tmpFile = Path.GetTempFileName();
                try{                    
                    using(FileStream fs = File.Open(tmpFile,FileMode.Open)){
                        fs.Write((byte[])cert,0,((byte[])cert).Length);
                    }

                    X509Certificate2 c = new X509Certificate2(tmpFile);
               
                    return c;
                }
                finally{
                    File.Delete(tmpFile);
                }                
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

        #region method FillUserInfo

        /// <summary>
        /// Gets and fills specified user info.
        /// </summary>
        /// <param name="userInfo">User info.</param>
        /// <exception cref="ArgumentNullException">Is riased when <b>userInfo</b> is null reference.</exception>
        private void FillUserInfo(AUTH_e_UserInfo userInfo)
        {
            if(userInfo == null){
                throw new ArgumentNullException("userInfo");
            }

            try{
                foreach(DataRowView dr in m_pApi.GetUsers("")){
                    if(userInfo.UserName.ToLowerInvariant() == dr["UserName"].ToString().ToLowerInvariant()){
                        userInfo.UserExists = true;
                        userInfo.Password = dr["Password"].ToString();
                        break;
                    }
                }
            }
            catch(Exception x){
                OnError(x);
            }
        }

        #endregion

        #region method ProcessAndStoreMessage

        /// <summary>
        /// Processes and stores message.
        /// </summary>
        /// <param name="sender">Mail from.</param>
        /// <param name="recipient">Recipient to.</param>
        /// <param name="msgStream">Message stream. Stream position must be there where message begins.</param>
        /// <param name="e">Event data.</param>
		public void ProcessAndStoreMessage(string sender,string[] recipient,Stream msgStream,SMTP_e_MessageStored e)
		{
            List<SMTP_RcptTo> recipients = new List<SMTP_RcptTo>();
            foreach(string r in recipient){
                recipients.Add(new SMTP_RcptTo(r,SMTP_DSN_Notify.NotSpecified,null));
            }

            ProcessAndStoreMessage(null,sender,SMTP_DSN_Ret.NotSpecified,recipients.ToArray(),msgStream,e);
        }

        /// <summary>
        /// Processes and stores message.
        /// </summary>
        /// <param name="envelopeID">Envelope ID_(MAIL FROM: ENVID).</param>
        /// <param name="sender">Mail from.</param>
        /// <param name="ret">Specifies what parts of message are returned in DSN report.</param>
        /// <param name="recipients">Message recipients.</param>
        /// <param name="msgStream">Message stream. Stream position must be there where message begins.</param>
        /// <param name="e">Event data.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>recipients</b> or <b>msgStream</b> is nulll reference.</exception>
		public void ProcessAndStoreMessage(string envelopeID,string sender,SMTP_DSN_Ret ret,SMTP_RcptTo[] recipients,Stream msgStream,SMTP_e_MessageStored e)
		{
            if(recipients == null){
                throw new ArgumentNullException("recipients");
            }
            if(msgStream == null){
                throw new ArgumentNullException("msgStream");
            }

            /* Message processing.
                *) Message filters.
                *) Global message rules.
                *) Process recipients.
            */

            List<SMTP_RcptTo> dsn_Delivered = new List<SMTP_RcptTo>();

            string[] to = new string[recipients.Length];
            for(int i=0;i<to.Length;i++){
                to[i] = recipients[i].Mailbox;
            }
         
			#region Global Filtering stuff
            
			//--- Filter message -----------------------------------------------//
			Stream filteredMsgStream = msgStream;
			DataView dvFilters = m_pApi.GetFilters();
			dvFilters.RowFilter = "Enabled=true AND Type='ISmtpMessageFilter'";
			dvFilters.Sort = "Cost";			
			foreach(DataRowView drViewFilter in dvFilters){
                try{
				    filteredMsgStream.Position = 0;

				    string assemblyFile = API_Utlis.PathFix(drViewFilter.Row["Assembly"].ToString());
				    // File is without path probably, try to load it from filters folder
				    if(!File.Exists(assemblyFile)){
					    assemblyFile = API_Utlis.PathFix(m_pOwnerServer.StartupPath + "\\Filters\\" + assemblyFile);
				    }

				    Assembly ass = Assembly.LoadFrom(assemblyFile);
				    Type tp = ass.GetType(drViewFilter.Row["ClassName"].ToString());
				    object filterInstance = Activator.CreateInstance(tp);
				    ISmtpMessageFilter filter = (ISmtpMessageFilter)filterInstance;
    						
				    string errorText = "";
                    SMTP_Session session = null;
                    if(e != null){
                        session = e.Session;
                    }
				    FilterResult result = filter.Filter(filteredMsgStream,out filteredMsgStream,sender,to,m_pApi,session,out errorText);
                    if(result == FilterResult.DontStore){
                        // Just skip messge, act as message is stored
                        e.Reply = new SMTP_Reply(552,"Requested mail action aborted: Message discarded by server filter.");
						return; 
                    }
                    else if(result == FilterResult.Error){
                        if(e != null){
                            e.Reply = new SMTP_Reply(552,"Requested mail action aborted: " + errorText);
                        }
                        else{
                            // NOTE: 26.01.2006 - e maybe null if that method is called server internally and no smtp session.                            
                        }
						return;
                    }
                    
                    // Filter didn't return message stream
                    if(filteredMsgStream == null){
				        e.Reply = new SMTP_Reply(552,"Requested mail action aborted: Message discarded by server filter.");
                        return;
                    }
                }
                catch(Exception x){
                    // Filtering failed, log error and allow message through.
                    OnError(x);
                }
			}
			//---------------------------------------------------------------//
			#endregion

            #region Global Message Rules
  
            filteredMsgStream.Position = 0;
                         
            Mail_Message mime = null;
            try{
                mime = Mail_Message.ParseFromStream(filteredMsgStream);
            }
            // Invalid message syntax, block such message.
            catch{
                e.Reply = new SMTP_Reply(552,"Requested mail action aborted: Message has invalid structure/syntax.");
                               
                try{
                    if(!Directory.Exists(this.MailStorePath + "Unparseable")){
                        Directory.CreateDirectory(this.MailStorePath + "Unparseable");
                    }
                                
                    using(FileStream fs = File.Create(this.MailStorePath + "Unparseable\\" + Guid.NewGuid().ToString().Replace("-","") + ".eml")){
                        filteredMsgStream.Position = 0;
                        Net_Utils.StreamCopy(filteredMsgStream,fs,32000);
                    }
                }
                catch{
                }

                return;
            }

            //--- Check Global Message Rules --------------------------------------------------------------//
            bool   deleteMessage = false;
            string storeFolder   = "Inbox";
            string smtpErrorText = null;   

            // Loop rules
            foreach(DataRowView drV_Rule in m_pApi.GetGlobalMessageRules()){
                // Reset stream position
                filteredMsgStream.Position = 0;

                if(Convert.ToBoolean(drV_Rule["Enabled"])){
                    string ruleID = drV_Rule["RuleID"].ToString();
                    GlobalMessageRule_CheckNextRule_enum checkNextIf = (GlobalMessageRule_CheckNextRule_enum)(int)drV_Rule["CheckNextRuleIf"];
                    string matchExpression = drV_Rule["MatchExpression"].ToString();

                    // e may be null if server internal method call and no actual session !
                    SMTP_Session session = null;
                    if(e != null){
                        session = e.Session;
                    }
                    GlobalMessageRuleProcessor ruleEngine = new GlobalMessageRuleProcessor();
                    bool matches = ruleEngine.Match(matchExpression,sender,to,session,mime,(int)filteredMsgStream.Length);
                    if(matches){                        
                        // Do actions
                        GlobalMessageRuleActionResult result = ruleEngine.DoActions(
                            m_pApi.GetGlobalMessageRuleActions(ruleID),
                            this,
                            filteredMsgStream,
                            sender,
                            to
                        );

                        if(result.DeleteMessage){
                            deleteMessage = true;
                        }
                        if(result.StoreFolder != null){                            
                            storeFolder = result.StoreFolder;                           
                        }
                        if(result.ErrorText != null){
                            smtpErrorText = result.ErrorText;
                        }
                    }

                    //--- See if we must check next rule -------------------------------------------------//
                    if(checkNextIf == GlobalMessageRule_CheckNextRule_enum.Always){
                        // Do nothing
                    }
                    else if(checkNextIf == GlobalMessageRule_CheckNextRule_enum.IfMatches && !matches){
                        break;
                    }
                    else if(checkNextIf == GlobalMessageRule_CheckNextRule_enum.IfNotMatches && matches){
                        break;
                    }
                    //------------------------------------------------------------------------------------//
                }
            }

            // Return error to connected client
            if(smtpErrorText != null){
                e.Reply = new SMTP_Reply(552,"Requested mail action aborted: " + smtpErrorText);
                return;
            }

            // Just don't store message
            if(deleteMessage){
                return;
            }
            
            // Reset stream position
            filteredMsgStream.Position = 0;
            //--- End of Global Rules -------------------------------------------------------------------//

            #endregion
            
            #region Process recipients
                  
            HashSet<string> processedItems = new HashSet<string>();

            Queue<SMTP_RcptTo> recipientsQueue = new Queue<SMTP_RcptTo>();
            // Queue current recipients for processing.
            foreach(SMTP_RcptTo recipient in recipients){
                recipientsQueue.Enqueue(recipient);
            }

            while(recipientsQueue.Count > 0){
                /* Process order
                    *) Local user
                    *) Local address
                    *) Local mailing list address
                    *) Route
                    *) Relay
                */

                SMTP_RcptTo recipient = recipientsQueue.Dequeue();

                // Check if we already have processed this item. Skip dublicate items.
                // This method also avoids loops when 2 recipients reference each other.
                if(processedItems.Contains(recipient.Mailbox)){
                    continue;
                }
                processedItems.Add(recipient.Mailbox);
                               

                #region Local user

                if(recipient.Mailbox.IndexOf('@') == -1 && m_pApi.UserExists(recipient.Mailbox)){
                    // Add user to processed list.
                    processedItems.Add(recipient.Mailbox);

                    // Delivery status notification(DSN) requested to this user.
                    if((recipient.Notify & SMTP_DSN_Notify.Success) != 0){
                        dsn_Delivered.Add(recipient);
                    }

                    ProcessUserMsg(sender,recipient.Mailbox,recipient.Mailbox,storeFolder,filteredMsgStream,e);

                    continue;
                }

                #endregion

                #region Local address

                string localUser = m_pApi.MapUser(recipient.Mailbox);
                if(localUser != null){
                    // Add user to processed list.
                    processedItems.Add(localUser);

                    // Delivery status notification(DSN) requested to this user.
                    if((recipient.Notify & SMTP_DSN_Notify.Success) != 0){
                        dsn_Delivered.Add(recipient);
                    }

                    ProcessUserMsg(sender,recipient.Mailbox,localUser,storeFolder,filteredMsgStream,e);
                }

                #endregion

                #region Mailing list address

                else if(m_pApi.MailingListExists(recipient.Mailbox)){
                    // Delivery status notification(DSN) requested to this user.
                    if((recipient.Notify & SMTP_DSN_Notify.Success) != 0){
                        dsn_Delivered.Add(recipient);
                    }

                    Queue<string> processQueue = new Queue<string>();
                    processQueue.Enqueue(recipient.Mailbox);

                    // Loop while there are mailing lists or nested mailing list available
                    while(processQueue.Count > 0){
                        string mailingList = processQueue.Dequeue(); 
                          
                        // Process mailing list members
					    foreach(DataRowView drV in m_pApi.GetMailingListAddresses(mailingList)){
                            string member = drV["Address"].ToString();

                            // Member is asteric pattern matching server emails
                            if(member.IndexOf('*') > -1){
                                DataView dvServerAddresses = m_pApi.GetUserAddresses("");
                                foreach(DataRowView drvServerAddress in dvServerAddresses){
                                    string serverAddress = drvServerAddress["Address"].ToString();
                                    if(SCore.IsAstericMatch(member,serverAddress)){
                                        recipientsQueue.Enqueue(new SMTP_RcptTo(serverAddress,SMTP_DSN_Notify.NotSpecified,null));                                        
                                    }
                                }
                            }
                            // Member is user or group, not email address
                            else if(member.IndexOf('@') == -1){                            
                                // Member is group, replace with actual users
                                if(m_pApi.GroupExists(member)){
                                    foreach(string user in m_pApi.GetGroupUsers(member)){
                                        recipientsQueue.Enqueue(new SMTP_RcptTo(user,SMTP_DSN_Notify.NotSpecified,null));                                        
                                    }
                                }
                                // Member is user
                                else if(m_pApi.UserExists(member)){
                                    recipientsQueue.Enqueue(new SMTP_RcptTo(member,SMTP_DSN_Notify.NotSpecified,null));                                    
                                }
                                // Unknown member, skip it.
                                else{
                                }
                            }
                            // Member is nested mailing list
                            else if(m_pApi.MailingListExists(member)){
                                processQueue.Enqueue(member);                                
                            }
                            // Member is normal email address
                            else{
                                recipientsQueue.Enqueue(new SMTP_RcptTo(member,SMTP_DSN_Notify.NotSpecified,null));                                
                            }					
					    }
                    }
                }

                #endregion

                else{
                    bool isRouted = false;

                    #region Check Routing

                    foreach(DataRowView drRoute in m_pApi.GetRoutes()){
                        // We have matching route
                        if(Convert.ToBoolean(drRoute["Enabled"]) && SCore.IsAstericMatch(drRoute["Pattern"].ToString(),recipient.Mailbox)){
                            string           description = drRoute["Action"].ToString();    
                            RouteAction_enum action      = (RouteAction_enum)Convert.ToInt32(drRoute["Action"]);
                            byte[]           actionData  = (byte[])drRoute["ActionData"];

                            #region RouteToEmail

                            if(action == RouteAction_enum.RouteToEmail){
                                XmlTable table = new XmlTable("ActionData");
                                table.Parse(actionData);

                                // Add email to process queue.
                                recipientsQueue.Enqueue(new SMTP_RcptTo(table.GetValue("EmailAddress"),SMTP_DSN_Notify.NotSpecified,null));

                                // Log
                                if(e != null){
                                    e.Session.LogAddText("Route '[" + description + "]: " + drRoute["Pattern"].ToString() + "' routed to email '" + table.GetValue("EmailAddress") + "'.");
                                }
                            }

                            #endregion

                            #region RouteToHost

                            else if(action == RouteAction_enum.RouteToHost){
                                XmlTable table = new XmlTable("ActionData");
                                table.Parse(actionData);  

                                msgStream.Position = 0;

                                // Route didn't match, so we have relay message.
                                this.RelayServer.StoreRelayMessage(                    
                                    Guid.NewGuid().ToString(),
                                    envelopeID,
                                    msgStream,
                                    HostEndPoint.Parse(table.GetValue("Host") + ":" + table.GetValue("Port")),
                                    sender,
                                    recipient.Mailbox,
                                    recipient.ORCPT,
                                    recipient.Notify,
                                    ret
                                );
                             
                                // Log
                                if(e != null){
                                    e.Session.LogAddText("Route '[" + description + "]: " + drRoute["Pattern"].ToString() + "' routed to host '" + table.GetValue("Host") + ":" + table.GetValue("Port") + "'.");
                                }
                            }

                            #endregion

                            #region RouteToMailbox

                            else if(action == RouteAction_enum.RouteToMailbox){
                                XmlTable table = new XmlTable("ActionData");
                                table.Parse(actionData);

                                ProcessUserMsg(sender,recipient.Mailbox,table.GetValue("Mailbox"),storeFolder,filteredMsgStream,e);

                                // Log
                                if(e != null){
                                    e.Session.LogAddText("Route '[" + description + "]: " + drRoute["Pattern"].ToString() + "' routed to user '" + table.GetValue("Mailbox") + "'.");
                                }
                            }

                            #endregion
                             
                            isRouted = true;
                            break;
                        }
                    }

                   #endregion

                    // Route didn't match, so we have relay message.
                    if(!isRouted){
                        filteredMsgStream.Position = 0;

                        this.RelayServer.StoreRelayMessage(                    
                            Guid.NewGuid().ToString(),
                            envelopeID,
                            filteredMsgStream,
                            null,
                            sender,
                            recipient.Mailbox,
                            recipient.ORCPT,
                            recipient.Notify,
                            ret
                        );
                    }
                }
            }

            #endregion

            
            #region DSN "delivered"

            // Send DSN for requested recipients.
            if(dsn_Delivered.Count > 0 && !string.IsNullOrEmpty(sender)){
                try{
                    string dsn_to = "";
                    for(int i=0;i<dsn_Delivered.Count;i++){
                        if(i == (dsn_Delivered.Count - 1)){
                            dsn_to += dsn_Delivered[i].Mailbox;
                        }
                        else{
                            dsn_to += dsn_Delivered[i].Mailbox + "; ";
                        }
                    }

                    string reportingMTA = "";
                    if(e != null && !string.IsNullOrEmpty(e.Session.LocalHostName)){
                        reportingMTA = e.Session.LocalHostName;
                    }
                    else{
                        reportingMTA = System.Net.Dns.GetHostName();
                    }

                    ServerReturnMessage messageTemplate = null;
                    if(messageTemplate == null){
                        string bodyRtf = "" +
                        "{\\rtf1\\ansi\\ansicpg1257\\deff0\\deflang1061{\\fonttbl{\\f0\\froman\\fcharset0 Times New Roman;}{\\f1\froman\\fcharset186{\\*\\fname Times New Roman;}Times New Roman Baltic;}{\\f2\fswiss\\fcharset186{\\*\\fname Arial;}Arial Baltic;}}\r\n" +
                        "{\\colortbl ;\\red0\\green128\\blue0;\\red128\\green128\\blue128;}\r\n" +
                        "{\\*\\generator Msftedit 5.41.21.2508;}\\viewkind4\\uc1\\pard\\sb100\\sa100\\lang1033\\f0\\fs24\\par\r\n" +
                        "Your message WAS SUCCESSFULLY DELIVERED to:\\line\\lang1061\\f1\\tab\\cf1\\lang1033\\b\\f0 " + dsn_to + "\\line\\cf0\\b0 and you explicitly requested a delivery status notification on success.\\par\\par\r\n" +
                        "\\cf2 Your original message\\lang1061\\f1 /header\\lang1033\\f0  is attached to this e-mail\\lang1061\\f1 .\\lang1033\\f0\\par\\r\\n" +
                        "\\cf0\\line\\par\r\n" +
                        "\\pard\\lang1061\\f2\\fs20\\par\r\n" +
                        "}\r\n";

                        messageTemplate = new ServerReturnMessage("DSN SUCCESSFULLY DELIVERED: " + mime.Subject,bodyRtf);
                    }

                    string rtf = messageTemplate.BodyTextRtf;

                    Mail_Message dsnMsg = new Mail_Message();
                    dsnMsg.MimeVersion = "1.0";
                    dsnMsg.Date = DateTime.Now;
                    dsnMsg.From = new Mail_t_MailboxList();
                    dsnMsg.From.Add(new Mail_t_Mailbox("Mail Delivery Subsystem","postmaster@local"));
                    dsnMsg.To = new Mail_t_AddressList();
                    dsnMsg.To.Add(new Mail_t_Mailbox(null,sender));
                    dsnMsg.Subject = messageTemplate.Subject;

                    //--- multipart/report -------------------------------------------------------------------------------------------------
                    MIME_h_ContentType contentType_multipartReport = new MIME_h_ContentType(MIME_MediaTypes.Multipart.report);            
                    contentType_multipartReport.Parameters["report-type"] = "delivery-status";
                    contentType_multipartReport.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
                    MIME_b_MultipartReport multipartReport = new MIME_b_MultipartReport(contentType_multipartReport);
                    dsnMsg.Body = multipartReport;

                        //--- multipart/alternative -----------------------------------------------------------------------------------------
                        MIME_Entity entity_multipart_alternative = new MIME_Entity();
                        MIME_h_ContentType contentType_multipartAlternative = new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative);
                        contentType_multipartAlternative.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
                        MIME_b_MultipartAlternative multipartAlternative = new MIME_b_MultipartAlternative(contentType_multipartAlternative);
                        entity_multipart_alternative.Body = multipartAlternative;
                        multipartReport.BodyParts.Add(entity_multipart_alternative);

                            //--- text/plain ---------------------------------------------------------------------------------------------------
                            MIME_Entity entity_text_plain = new MIME_Entity();
                            MIME_b_Text text_plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
                            entity_text_plain.Body = text_plain;
                            text_plain.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,SCore.RtfToText(rtf));
                            multipartAlternative.BodyParts.Add(entity_text_plain);

                            //--- text/html -----------------------------------------------------------------------------------------------------
                            MIME_Entity entity_text_html = new MIME_Entity();
                            MIME_b_Text text_html = new MIME_b_Text(MIME_MediaTypes.Text.html);
                            entity_text_html.Body = text_html;
                            text_html.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,SCore.RtfToHtml(rtf));
                            multipartAlternative.BodyParts.Add(entity_text_html);

                        //--- message/delivery-status
                        MIME_Entity entity_message_deliveryStatus = new MIME_Entity();                        
                        MIME_b_MessageDeliveryStatus body_message_deliveryStatus = new MIME_b_MessageDeliveryStatus();
                        entity_message_deliveryStatus.Body = body_message_deliveryStatus;
                        multipartReport.BodyParts.Add(entity_message_deliveryStatus);
            
                        //--- per-message-fields ----------------------------------------------------------------------------
                        MIME_h_Collection messageFields = body_message_deliveryStatus.MessageFields;
                        if(!string.IsNullOrEmpty(envelopeID)){
                            messageFields.Add(new MIME_h_Unstructured("Original-Envelope-Id",envelopeID));
                        }
                        messageFields.Add(new MIME_h_Unstructured("Arrival-Date",MIME_Utils.DateTimeToRfc2822(DateTime.Now)));
                        if(e != null && !string.IsNullOrEmpty(e.Session.EhloHost)){
                            messageFields.Add(new MIME_h_Unstructured("Received-From-MTA","dns;" + e.Session.EhloHost));
                        }
                        messageFields.Add(new MIME_h_Unstructured("Reporting-MTA","dns;" + reportingMTA));
                        //---------------------------------------------------------------------------------------------------

                        foreach(SMTP_RcptTo r in dsn_Delivered){
                            //--- per-recipient-fields --------------------------------------------------------------------------
                            MIME_h_Collection recipientFields = new MIME_h_Collection(new MIME_h_Provider());
                            if(r.ORCPT != null){
                                recipientFields.Add(new MIME_h_Unstructured("Original-Recipient",r.ORCPT));
                            }
                            recipientFields.Add(new MIME_h_Unstructured("Final-Recipient","rfc822;" + r.Mailbox));
                            recipientFields.Add(new MIME_h_Unstructured("Action","delivered"));
                            recipientFields.Add(new MIME_h_Unstructured("Status","2.0.0"));
                            body_message_deliveryStatus.RecipientBlocks.Add(recipientFields);
                            //---------------------------------------------------------------------------------------------------
                        }
                                            
                        //--- message/rfc822
                        if(mime != null){
                            MIME_Entity entity_message_rfc822 = new MIME_Entity();
                            MIME_b_MessageRfc822 body_message_rfc822 = new MIME_b_MessageRfc822();
                            entity_message_rfc822.Body = body_message_rfc822;
                            if(ret == SMTP_DSN_Ret.FullMessage){
                                body_message_rfc822.Message = mime;
                            }
                            else{
                                MemoryStream ms = new MemoryStream();
                                mime.Header.ToStream(ms,null,null);
                                ms.Position = 0;
                                body_message_rfc822.Message = Mail_Message.ParseFromStream(ms);
                            }
                            multipartReport.BodyParts.Add(entity_message_rfc822);
                        }

                    using(MemoryStream strm = new MemoryStream()){
					    dsnMsg.ToStream(strm,new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q,Encoding.UTF8),Encoding.UTF8);
					    ProcessAndStoreMessage("",new string[]{sender},strm,null);
				    }
                }
                catch(Exception x){
                    Error.DumpError(this.Name,x);
                }
            }

            #endregion
        }
                
        #endregion

        #region method ProcessUserMsg

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="userName"></param>
        /// <param name="storeFolder">Message folder where message will be stored. For example 'Inbox'.</param>
        /// <param name="msgStream"></param>
        /// <param name="e">Event data.</param>
		internal void ProcessUserMsg(string sender,string recipient,string userName,string storeFolder,Stream msgStream,SMTP_e_MessageStored e)
        {
            string userID = m_pApi.GetUserID(userName);
            // This value can be null only if user deleted during this session, so just skip next actions.
            if(userID == null){
                return;
            }

            #region User Message rules
	
            Stream filteredMsgStream = msgStream;
			filteredMsgStream.Position = 0;

            Mail_Message mime = null;
            try{
                mime = Mail_Message.ParseFromStream(filteredMsgStream);
            }
            // Invalid message syntax, block such message.
            catch{
                e.Reply = new SMTP_Reply(552,"Requested mail action aborted: Message has invalid structure/syntax.");
                return;
            }

            string[] to = new string[]{recipient};

            //--- Check User Message Rules --------------------------------------------------------------//
            bool   deleteMessage = false;
            string smtpErrorText = null;   

            // Loop rules
            foreach(DataRowView drV_Rule in m_pApi.GetUserMessageRules(userName)){
                // Reset stream position
                filteredMsgStream.Position = 0;

                if(Convert.ToBoolean(drV_Rule["Enabled"])){
                    string ruleID = drV_Rule["RuleID"].ToString();
                    GlobalMessageRule_CheckNextRule_enum checkNextIf = (GlobalMessageRule_CheckNextRule_enum)(int)drV_Rule["CheckNextRuleIf"];
                    string matchExpression = drV_Rule["MatchExpression"].ToString();

                    // e may be null if server internal method call and no actual session !
                    SMTP_Session session = null;
                    if(e != null){
                        session = e.Session;
                    }
                    GlobalMessageRuleProcessor ruleEngine = new GlobalMessageRuleProcessor();
                    bool matches = ruleEngine.Match(matchExpression,sender,to,session,mime,(int)filteredMsgStream.Length);
                    if(matches){                        
                        // Do actions
                        GlobalMessageRuleActionResult result = ruleEngine.DoActions(
                            m_pApi.GetUserMessageRuleActions(userID,ruleID),
                            this,
                            filteredMsgStream,
                            sender,
                            to
                        );

                        if(result.DeleteMessage){
                            deleteMessage = true;
                        }
                        if(result.StoreFolder != null){                            
                            storeFolder = result.StoreFolder;                           
                        }
                        if(result.ErrorText != null){
                            smtpErrorText = result.ErrorText;
                        }
                    }

                    //--- See if we must check next rule -------------------------------------------------//
                    if(checkNextIf == GlobalMessageRule_CheckNextRule_enum.Always){
                        // Do nothing
                    }
                    else if(checkNextIf == GlobalMessageRule_CheckNextRule_enum.IfMatches && !matches){
                        break;
                    }
                    else if(checkNextIf == GlobalMessageRule_CheckNextRule_enum.IfNotMatches && matches){
                        break;
                    }
                    //------------------------------------------------------------------------------------//
                }
            }

            // Return error to connected client
            if(smtpErrorText != null){
                e.Reply = new SMTP_Reply(552,"Requested mail action aborted: " + smtpErrorText);
                return;
            }

            // Just don't store message
            if(deleteMessage){
                return;
            }
            
            // Reset stream position
            filteredMsgStream.Position = 0;
            //--- End of Global Rules -------------------------------------------------------------------//
            
            #endregion

            // ToDo: User message filtering
			#region User message filtering

				//--- Filter message -----------------------------------------------//
		/*		MemoryStream filteredMsgStream = msgStream;
				DataView dvFilters = m_pApi.GetFilters();
				dvFilters.RowFilter = "Enabled=true AND Type=ISmtpUserMessageFilter";
				dvFilters.Sort = "Cost";			
				foreach(DataRowView drViewFilter in dvFilters){
					string assemblyFile = drViewFilter.Row["Assembly"].ToString();
					// File is without path probably, try to load it from filters folder
					if(!File.Exists(assemblyFile)){
						assemblyFile = m_SartUpPath + "\\Filters\\" + assemblyFile;
					}

					Assembly ass = Assembly.LoadFrom(assemblyFile);
					Type tp = ass.GetType(drViewFilter.Row["ClassName"].ToString());
					object filterInstance = Activator.CreateInstance(tp);
					ISmtpMessageFilter filter = (ISmtpMessageFilter)filterInstance;
												
					FilterResult result = filter.Filter(filteredMsgStream,out filteredMsgStream,sender,recipient,m_pApi);
					switch(result)
					{
						case FilterResult.DontStore:
							return; // Just skip messge, act as message is stored

						case FilterResult.Error:
							// ToDO: Add implementaion here or get rid of it (use exception instead ???).
							return;
					}
				}*/
				//---------------------------------------------------------------//

				#endregion

            filteredMsgStream.Position = 0;

            /* RFC 2821 4.4.
                When the delivery SMTP server makes the "final delivery" of a message, it inserts 
                a return-path line at the beginning of the mail data. This use of return-path is 
                required; mail systems MUST support it. The return-path line preserves the information 
                in the <reverse-path> from the MAIL command.
            */
            MultiStream finalStoreStream = new MultiStream();
            // e can be null if method caller isn't SMTP session.
            if(e != null){
                finalStoreStream.AppendStream(new MemoryStream(Encoding.Default.GetBytes("Return-Path: <" + e.Session.From.Mailbox + ">\r\n")));
            }
            finalStoreStream.AppendStream(filteredMsgStream);
            
			try{
				m_pApi.StoreMessage("system",userName,storeFolder,finalStoreStream,DateTime.Now,new string[]{"Recent"});
			}
			catch{
                // Storing probably failed because there isn't such folder, just store to user inbox.
				m_pApi.StoreMessage("system",userName,"Inbox",finalStoreStream,DateTime.Now,new string[]{"Recent"});
			}
        }

        #endregion

        #region method AstericMatch

		/// <summary>
		/// Checks if specified text matches to specified asteric pattern.
		/// </summary>
		/// <param name="pattern">Asteric pattern. Foe example: *xxx,*xxx*,xx*aa*xx, ... .</param>
		/// <param name="text">Text to match.</param>
		/// <returns></returns>
		public bool AstericMatch(string pattern,string text)
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

						text = text.Substring(text.IndexOf(indexOfPart) + indexOfPart.Length + 1);
						pattern = pattern.Substring(pattern.IndexOf("*",1) + 1);
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

        #region method FolderMatches

		/// <summary>
		/// Gets if folder matches to specified folder pattern.
		/// </summary>
		/// <param name="folderPattern">Folder pattern. * and % between path separators have same meaning (asteric pattern). 
		/// If % is at the end, then matches only last folder child folders and not child folder child folders.</param>
		/// <param name="folder">Folder name with full path.</param>
		/// <returns></returns>
		private bool FolderMatches(string folderPattern,string folder)
		{
			folderPattern = folderPattern.ToLower();
			folder = folder.ToLower();

			string[] folderParts = folder.Split('/');
			string[] patternParts = folderPattern.Split('/');

			// pattern is more nested than folder
			if(folderParts.Length < patternParts.Length){
				return false;				
			}
			// This can happen only if * at end
			else if(folderParts.Length > patternParts.Length && !folderPattern.EndsWith("*")){
				return false;					
			}
			else{
				// Loop patterns
				for(int i=0;i<patternParts.Length;i++){
					string patternPart = patternParts[i].Replace("%","*");
					
					// This is asteric pattern
					if(patternPart.IndexOf('*') > -1){
						if(!AstericMatch(patternPart,folderParts[i])){
							return false;
						}
						// else process next pattern
					}
					// No *, this must be exact match
					else{
						if(folderParts[i] != patternPart){
							return false;
						}
					}
				}
			}

			return true;
		}

		#endregion

        #region method GenerateMessageMissing

        /// <summary>
        /// Generates message missing message.
        /// </summary>
        /// <returns>Returns message missing message.</returns>
        public static Mail_Message GenerateMessageMissing()
        {
            Mail_Message msg = new Mail_Message();
            msg.MimeVersion = "1.0";
            msg.MessageID = MIME_Utils.CreateMessageID();
            msg.Date = DateTime.Now;
            msg.From = new Mail_t_MailboxList();
            msg.From.Add(new Mail_t_Mailbox("system","system"));
            msg.To = new Mail_t_AddressList();
            msg.To.Add(new Mail_t_Mailbox("system","system"));
            msg.Subject = "[MESSAGE MISSING] Message no longer exists on server !";

            //--- multipart/mixed -------------------------------------------------------------------------------------------------
            MIME_h_ContentType contentType_multipartMixed = new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed);
            contentType_multipartMixed.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
            MIME_b_MultipartMixed multipartMixed = new MIME_b_MultipartMixed(contentType_multipartMixed);
            msg.Body = multipartMixed;

                //--- text/plain ---------------------------------------------------------------------------------------------------
                MIME_Entity entity_text_plain = new MIME_Entity();
                MIME_b_Text text_plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
                entity_text_plain.Body = text_plain;
                text_plain.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,"NOTE: Message no longer exists on server.\r\n\r\nMessage is deleted by Administrator or anti-virus software.\r\n");
                multipartMixed.BodyParts.Add(entity_text_plain);

            return msg;
        }

        #endregion


        #region method OnError

        /// <summary>
        /// Is called when error happens.
        /// </summary>
        /// <param name="x"></param>
        private void OnError(Exception x)
        {
            Error.DumpError(this.Name,x);
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets virtual server ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Starts or stops server.
        /// </summary>
        public bool Enabled
        {
            get{ return m_Running; }

            set{
                if(value){
                    Start();
                }
                else{
                    Stop();
                }
            }
        }

        /// <summary>
        /// Gets virtual server name
        /// </summary>
        public string Name
        {
            get{ return m_Name; }
        }

        /// <summary>
        /// Gets this virtual server API.
        /// </summary>
        public IMailServerApi API
        {
            get{ return m_pApi; }
        }

        /// <summary>
        /// Gets mailstore path.
        /// </summary>
        public string MailStorePath
        {
            get{ return m_MailStorePath; }
        }

        /// <summary>
        /// Gets virtual server DNS client.
        /// </summary>
        public Dns_Client DnsClient
        {
            get{ return m_pDnsClient; }
        }

        /// <summary>
        /// Gets this virtual server SMTP server. Returns null if server is stopped.
        /// </summary>
        public SMTP_Server SmtpServer
        {
            get{ return m_pSmtpServer; }
        }

        /// <summary>
        /// Gets this virtual server POP3 server. Returns null if server is stopped.
        /// </summary>
        public POP3_Server Pop3Server
        {
            get{ return m_pPop3Server; }
        }

        /// <summary>
        /// Gets this virtual server IMAP server. Returns null if server is stopped.
        /// </summary>
        public IMAP_Server ImapServer
        {
            get{ return m_pImapServer; }
        }

        /// <summary>
        /// Gets this virtual server Relay server. Returns null if server is stopped.
        /// </summary>
        public RelayServer RelayServer
        {
            get{ return m_pRelayServer; }
        }

        /// <summary>
        /// Gets this virtual server SIP server. Returns null if server is stopped.
        /// </summary>
        public SIP_Proxy SipServer
        {
            get{ return m_pSipServer; }
        }


        //---- ?? Used by management server log viewer.

        internal string SMTP_LogsPath
        {
            get{ return m_SMTP_LogPath; }
        }

        internal string POP3_LogsPath
        {
            get{ return m_POP3_LogPath; }
        }

        internal string IMAP_LogsPath
        {
            get{ return m_IMAP_LogPath; }
        }

        internal string RELAY_LogsPath
        {
            get{ return m_Relay_LogPath; }
        }

        internal string FETCH_LogsPath
        {
            get{ return m_Fetch_LogPath; }
        }
                
        #endregion

    }
}
