using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Provides server info.
    /// </summary>
    public class ServerInfo
    {
        private string   m_OS            = "";
        private string   m_ServerVersion = "";
        private int      m_MemoryUsage   = 0;
        private int      m_CpuUsage      = 0;
        private DateTime m_pServerStartTime;
        private int      m_ReadsSec      = 0;
        private int      m_WitesSec      = 0;
        private int      m_SmtpSessions  = 0;
        private int      m_Pop3Sessions  = 0;
        private int      m_ImapSessions  = 0;
        private int      m_RelaySessions = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="os">Operatin system on what mail server is running.</param>
        /// <param name="mailserverVersion">Mail server version.</param>
        /// <param name="memUsage">Memory used by mail server.</param>
        /// <param name="cpuUsage">Cpu used by mail server.</param>
        /// <param name="serverDateTime">Time when server started.</param>
        /// <param name="readsSec">Specifies how many KB / Second server reads.</param>
        /// <param name="writesSec">Specifies how many KB / Second server writes.</param>
        /// <param name="smtpSessions">Virtual servers total SMTP sessions.</param>
        /// <param name="pop3Sessions">Virtual servers total POP3 sessions.</param>
        /// <param name="imapSessions">Virtual servers total IMAP sessions.</param>
        /// <param name="relaySessions">Virtual servers total Relay sessions.</param>
        internal ServerInfo(string os,string mailserverVersion,int memUsage,int cpuUsage,DateTime serverDateTime,int readsSec,int writesSec,int smtpSessions,int pop3Sessions,int imapSessions,int relaySessions)
        {
            m_OS               = os;
            m_ServerVersion    = mailserverVersion;
            m_MemoryUsage      = memUsage;
            m_CpuUsage         = cpuUsage;
            m_pServerStartTime = serverDateTime;
            m_ReadsSec         = readsSec;
            m_WitesSec         = writesSec;
            m_SmtpSessions     = smtpSessions;
            m_Pop3Sessions     = pop3Sessions;
            m_ImapSessions     = imapSessions;
            m_RelaySessions    = relaySessions;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets operatin system on what mail server is running.
        /// </summary>
        public string OS
        {
            get{ return m_OS; }
        }

        /// <summary>
        /// Gets mail server version.
        /// </summary>
        public string MailServerVersion
        {
            get{ return m_ServerVersion; }
        }

        /// <summary>
        /// Gets how many MB memory mail server consumes.
        /// </summary>
        public int MemoryUsage
        {
            get{ return m_MemoryUsage; }
        }

        /// <summary>
        /// Gets much CPU % mail server consumes.
        /// </summary>
        public int CpuUsage
        {
            get{ return m_CpuUsage; }
        }

        /// <summary>
        /// Gets server start time.
        /// </summary>
        public DateTime ServerStartTime
        {
            get{ return m_pServerStartTime; }
        }

        /// <summary>
        /// Gets how many KB / Second server reads.
        /// </summary>
        public int ReadsInSecond
        {
            get{ return m_ReadsSec; }
        }

        /// <summary>
        /// Gets how many KB / Second server writes.
        /// </summary>
        public int WritesInSecond
        {
            get{ return m_WitesSec; }
        }

        /// <summary>
        /// Gets all virtual servers total SMTP sessions.
        /// </summary>
        public int TotalSmtpSessions
        {
            get{ return m_SmtpSessions; }
        }

        /// <summary>
        /// Gets all virtual servers total POP3 sessions.
        /// </summary>
        public int TotalPop3Sessions
        {
            get{ return m_Pop3Sessions; }
        }

        /// <summary>
        /// Gets all virtual servers total IMAP sessions.
        /// </summary>
        public int TotalImapSessions
        {
            get{ return m_ImapSessions; }
        }

        /// <summary>
        /// Gets all virtual servers total Relay sessions.
        /// </summary>
        public int TotalRelaySessions
        {
            get{ return m_RelaySessions; }
        }

        #endregion

    }
}
