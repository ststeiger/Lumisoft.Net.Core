using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Timers;
using System.Data;

using LumiSoft.MailServer.Monitoring;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// This class represent mail server.
    /// </summary>
    public class Server : IDisposable
    {
        private string              m_StartupPath       = "";
        private bool                m_Running           = false;
        private Timer               m_pSettingsTimer    = null;
        private DateTime            m_ServersFileDate   = DateTime.MinValue;
        private MonitoringServer    m_pManagementServer = null;
        private DateTime            m_StartTime;
        private List<VirtualServer> m_pVirtualServers   = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Server()
        {
            m_pVirtualServers = new List<VirtualServer>();
            
            // Add unhandled exception handler
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Get startup path
            m_StartupPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + System.IO.Path.DirectorySeparatorChar;

            // Set error file locaton
            Error.ErrorFilePath = m_StartupPath;
                        
            m_pSettingsTimer = new Timer(10000);
            m_pSettingsTimer.AutoReset = true;
            m_pSettingsTimer.Elapsed += new ElapsedEventHandler(m_pSettingsTimer_Elapsed);
        }

        #region method Dispose

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		public void Dispose()
		{
            Stop();
        }

        #endregion


        #region Events Handling

        #region method CurrentDomain_UnhandledException

        private void CurrentDomain_UnhandledException(object sender,UnhandledExceptionEventArgs e)
        {
            Error.DumpError((Exception)e.ExceptionObject,new System.Diagnostics.StackTrace());
        }

        #endregion


        #region method m_pSettingsTimer_Elapsed

        private void m_pSettingsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoadVirtualServers();
        }

        #endregion

        #endregion


        #region method Start

        /// <summary>
        /// Starts mail server.
        /// </summary>
        public void Start()
        {
            // Server is already running, so do nothing.
            if(m_Running){
                return;
            }

            m_ServersFileDate = DateTime.MinValue;
            m_pSettingsTimer_Elapsed(this,null);
            m_pSettingsTimer.Enabled = true;

            // Start management server
            m_pManagementServer = new MonitoringServer(this);
		    m_pManagementServer.Start();

            m_Running   = true;
            m_StartTime = DateTime.Now;
        }

        #endregion

        #region method Stop

        /// <summary>
        /// Stops mail server.
        /// </summary>
        public void Stop()
        {
            // Server isn't running, so do nothing.
            if(!m_Running){
                return;
            }

            m_pSettingsTimer.Enabled = false;

            // Kill all running virtual servers
            foreach(VirtualServer server in m_pVirtualServers){
                server.Stop();
            }
            m_pVirtualServers.Clear();

            // Kill management server
            m_pManagementServer.Dispose();
            m_pManagementServer = null;

            m_Running = false;

            // Logging stuff
			Logger.WriteLog(m_StartupPath + "Logs\\Server\\server.log","//---- Server stopped " + DateTime.Now);
        }

        #endregion


        #region method LoadApi

        /// <summary>
        /// Loads specified virtual server API.
        /// </summary>
        /// <param name="assembly">API assembly name.</param>
        /// <param name="type">API type name.</param>
        /// <param name="initString">API init string</param>
        /// <returns></returns>
		internal IMailServerApi LoadApi(string assembly,string type,string initString)
		{
			string apiAssemblyPath = "";

			if(File.Exists(SCore.PathFix(m_StartupPath + "\\" + assembly))){
				apiAssemblyPath = SCore.PathFix(m_StartupPath + "\\" + assembly);
			}
			else{
				apiAssemblyPath = SCore.PathFix(assembly);
			}

			Assembly ass = Assembly.LoadFile(apiAssemblyPath);
			return (IMailServerApi)Activator.CreateInstance(ass.GetType(type),new object[]{initString});
		}

		#endregion

        #region method LoadVirtualServers

        /// <summary>
        /// Loads virtual server from xml file.
        /// </summary>
        internal void LoadVirtualServers()
        {
            try{
                if(!File.Exists(SCore.PathFix(m_StartupPath + "Settings\\localServers.xml"))){
                    return;
                }

				DateTime dateServers = File.GetLastWriteTime(SCore.PathFix(m_StartupPath + "Settings\\localServers.xml"));

				if(DateTime.Compare(dateServers,m_ServersFileDate) != 0){
					m_ServersFileDate = dateServers;

					DataSet ds = new DataSet();
                    ds.Tables.Add("Servers");
                    ds.Tables["Servers"].Columns.Add("ID");
                    ds.Tables["Servers"].Columns.Add("Enabled");
                    ds.Tables["Servers"].Columns.Add("Name");
                    ds.Tables["Servers"].Columns.Add("API_assembly");
                    ds.Tables["Servers"].Columns.Add("API_class");
                    ds.Tables["Servers"].Columns.Add("API_initstring");
					ds.ReadXml(SCore.PathFix(m_StartupPath + "Settings\\localServers.xml"));

                    if(ds.Tables.Contains("Servers")){
                        // Delete running virtual servers what has deleted.
                        for(int i=0;i<m_pVirtualServers.Count;i++){
                            VirtualServer server = m_pVirtualServers[i];
                            bool exists = false;
                            foreach(DataRow dr in ds.Tables["Servers"].Rows){
                                if(server.ID == dr["ID"].ToString()){
                                    exists = true;
                                    break;
                                }
                            }
                            if(!exists){
                                server.Stop();
                                m_pVirtualServers.Remove(server);
                                i--;
                            }
                        }

                        // Add new added virtual servers what aren't running already.
					    foreach(DataRow dr in ds.Tables["Servers"].Rows){
                            //--- See if specified server already running, if so, skip it. --//
                            bool exists = false;
                            foreach(VirtualServer server in m_pVirtualServers){
                                if(server.ID == dr["ID"].ToString()){
                                    exists = true;
                                    server.Enabled = ConvertEx.ToBoolean(dr["Enabled"],true);
                                    break;
                                }
                            }
                            if(exists){
                                continue;
                            }
                            //--------------------------------------------------------------//

						    string id       = dr["ID"].ToString();
                            string name     = dr["Name"].ToString();
						    string assembly = dr["API_assembly"].ToString();
						    string apiClass = dr["API_class"].ToString();
						    string intiStr  = dr["API_initstring"].ToString();

						    IMailServerApi api = LoadApi(assembly,apiClass,intiStr);
                            VirtualServer virtualServer = new VirtualServer(this,id,name,intiStr,api);
						    m_pVirtualServers.Add(virtualServer);
                            virtualServer.Enabled = ConvertEx.ToBoolean(dr["Enabled"],true);
					    }
                    }
				}							
			}
			catch(Exception x){
				Error.DumpError(x,new System.Diagnostics.StackTrace());
			}
        }


        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets path where mail server started.
        /// </summary>
        public string StartupPath
        {
            get{ return m_StartupPath; }
        }

        /// <summary>
        /// Gets if mail server is running.
        /// </summary>
        public bool Running
        {
            get{ return m_Running; }
        }

        /// <summary>
        /// Gets when server started. Returns DateTime.MinValue if server not running.
        /// </summary>
        public DateTime StartTime
        {
            get{ 
                if(m_Running){
                    return m_StartTime; 
                }
                else{                    
                    return DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Gets mail server running virtual servers. This property is available only if this.Running = true.
        /// </summary>
        public VirtualServer[] VirtualServers
        {
            get{ return m_pVirtualServers.ToArray(); }
        }

        /// <summary>
        /// Gets all mail server current sessions.
        /// </summary>
        public object[] Sessions
        {
            get{
                ArrayList sessions = new ArrayList();
                foreach(VirtualServer virtualServer in m_pVirtualServers){
                    if(virtualServer.Enabled){
                        if(virtualServer.SmtpServer.IsRunning){
                            sessions.AddRange(virtualServer.SmtpServer.Sessions.ToArray());
                        }
                        if(virtualServer.Pop3Server.IsRunning){
                            sessions.AddRange(virtualServer.Pop3Server.Sessions.ToArray());
                        }
                        if(virtualServer.ImapServer.IsRunning){
                            sessions.AddRange(virtualServer.ImapServer.Sessions.ToArray());
                        }
                        if(virtualServer.RelayServer.IsRunning){
                            sessions.AddRange(virtualServer.RelayServer.Sessions.ToArray());
                        }
                    }
                }
                sessions.AddRange(m_pManagementServer.Sessions.ToArray());

                return sessions.ToArray(); 
            }
        }

        #endregion

    }
}
