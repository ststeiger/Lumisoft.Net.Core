using System;
using System.IO;
using System.Data;
using System.Timers;

using LumiSoft.MailServer;
using LumiSoft.Net;
using LumiSoft.Net.Log;
using LumiSoft.Net.POP3.Client;
using LumiSoft.Net.IMAP.Server;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// Downloads messages from remote pop3 server to local server.
	/// </summary>
	internal class FetchPop3 : IDisposable
	{
		private VirtualServer  m_pServer       = null;
		private IMailServerApi m_pApi          = null;
        private bool           m_Enabled       = true;
		private bool           m_Fetching      = false;
		private DateTime       m_LastFetch;
		private string         m_LogPath       = "";
		private bool           m_LogCommands   = false;
        private Timer          m_pTimer        = null;
        private int            m_FetchInterval = 300;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="server"></param>
		/// <param name="api"></param>
		public FetchPop3(VirtualServer server,IMailServerApi api)
		{	
			m_pServer   = server;
			m_pApi      = api;
			m_LastFetch = DateTime.Now.AddMinutes(-5);

            m_pTimer = new Timer();
            m_pTimer.Interval = 15000;
            m_pTimer.Elapsed += new ElapsedEventHandler(m_pTimer_Elapsed);
        }
                
        #region method Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if(m_pTimer != null){
                m_pTimer.Dispose();
                m_pTimer = null;
            }
        }

        #endregion


        #region Events handling

        #region method m_pTimer_Elapsed

        private void m_pTimer_Elapsed(object sender,ElapsedEventArgs e)
        {
            if(this.Enabled && this.FetchTime){
                StartFetching();
            }
        }

        #endregion

        #region method Pop3_WriteLog

        /// <summary>
        /// This method is called when new POP3 log entry available.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void Pop3_WriteLog(object sender,WriteLogEventArgs e)
        {
            Logger.WriteLog(m_LogPath + "fetch-" + DateTime.Today.ToString("yyyyMMdd") + ".log",e.LogEntry);
        }

        #endregion

        #endregion
                        

        #region method StartFetching

        /// <summary>
		/// Starts messages fetching.
		/// </summary>
		public void StartFetching()
		{
			if(m_Fetching){
				return;
			}

			m_Fetching = true;

            try{
			    DataView dvUsers = m_pApi.GetUsers("ALL");

			    using(DataView dvServers = m_pApi.GetUserRemoteServers("")){
				    foreach(DataRowView drV in dvServers){
					    try{
                            if(!ConvertEx.ToBoolean(drV["Enabled"])){
                                continue;
                            }

						    // Find user name from user ID
						    string userName = "";
						    dvUsers.RowFilter = "UserID='" + drV["UserID"] + "'";
						    if(dvUsers.Count > 0){
							    userName = dvUsers[0]["UserName"].ToString();
						    }
						    else{
							    continue;
						    }

						    string server = drV.Row["RemoteServer"].ToString();
						    int    port   = Convert.ToInt32(drV.Row["RemotePort"]);
						    string user   = drV.Row["RemoteUserName"].ToString();
						    string passw  = drV.Row["RemotePassword"].ToString();
                            bool   useSSL = ConvertEx.ToBoolean(drV["UseSSL"]);
    					
						    // Connect and login to pop3 server
						    using(POP3_Client clnt = new POP3_Client()){
                                clnt.Logger = new LumiSoft.Net.Log.Logger();
                                clnt.Logger.WriteLog += new EventHandler<WriteLogEventArgs>(Pop3_WriteLog);
							    clnt.Connect(server,port,useSSL);
                                clnt.Login(user,passw);

                                foreach(POP3_ClientMessage message in clnt.Messages){
                                    // Store message
								    m_pServer.ProcessUserMsg("","",userName,"Inbox",new MemoryStream(message.MessageToByte()),null);

                                    message.MarkForDeletion();
                                }
						    }
					    }
					    catch{
					    }
				    }
			    }

			    m_LastFetch = DateTime.Now;
            }
            catch(Exception x){
                Error.DumpError(m_pServer.Name,x);
            }
			m_Fetching = false;
		}
                                
		#endregion


		#region Properties Implementation

        /// <summary>
        /// Gets or sets if fetch remote messages service is enabled.
        /// </summary>
        public bool Enabled 
        {
            get{ return m_Enabled; }

            set{ 
                m_Enabled = value; 
                m_pTimer.Enabled = value;
            }
        }

		/// <summary>
		/// Gets if fetching in progress.
		/// </summary>
		public bool IsFetching
		{
			get{ return m_Fetching; }
		}

		/// <summary>
		/// Gets if time to fetch.
		/// </summary>
		public bool FetchTime
		{
			get{
				if(m_LastFetch.AddSeconds(m_FetchInterval) < DateTime.Now){
					return true;
				}
				else{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets or sets log path.
		/// </summary>
		public string LogPath
		{
			get{ return m_LogPath; }

			set{ m_LogPath = value; }
		}

		/// <summary>
		/// Gets or sets if to log fetch pop3 commands.
		/// </summary>
		public bool LogCommands
		{
			get{ return m_LogCommands; }

			set{ m_LogCommands = value; }
		}

        /// <summary>
        /// Gets or sets fetch interval(in seconds).
        /// </summary>
        public int FetchInterval
        {
            get{ return m_FetchInterval; }

            set{ m_FetchInterval = value; }
        }

		#endregion
		
	}
}
