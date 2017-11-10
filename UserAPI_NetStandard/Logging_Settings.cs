using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Logging_Settings object represents logging settings in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Logging_Settings
    {
        private System_Settings m_pSysSettings          = null;
        private bool            m_LogSMTP               = false;
        private string          m_SmtpLogsPath          = "";
        private bool            m_LogPOP3               = false;
        private string          m_Pop3LogsPath          = "";
        private bool            m_LogIMAP               = false;
        private string          m_ImapLogsPath          = "";
        private bool            m_LogRelay              = false;
        private string          m_RelayLogsPath         = "";
        private bool            m_LogFetchMessages      = false;
        private string          m_FetchMessagesLogsPath = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sysSettings">Reference to system settings.</param>
        /// <param name="logSMTP">Specifies if SMTP is logged.</param>
        /// <param name="smtpLogsPath">SMTP logs path.</param>
        /// <param name="logPOP3">Specifies if POP3 is logged.</param>
        /// <param name="pop3LogsPath">POP3 logs path.</param>
        /// <param name="logIMAP">Specifies if IMAP is logged.</param>
        /// <param name="imapLogsPath">IMAP logs path.</param>
        /// <param name="logRelay">Specifies if Relay is logged.</param>
        /// <param name="relayLogsPath">Relay logs path.</param>
        /// <param name="logFetchMessages">Specifies if fetch messages is logged.</param>
        /// <param name="fetchMessagesLogsPath">Fetch messages logs path.</param>
        internal Logging_Settings(System_Settings sysSettings,bool logSMTP,string smtpLogsPath,bool logPOP3,string pop3LogsPath,bool logIMAP,string imapLogsPath,bool logRelay,string relayLogsPath,bool logFetchMessages,string fetchMessagesLogsPath)
        {
            m_pSysSettings          = sysSettings;
            m_LogSMTP               = logSMTP;
            m_SmtpLogsPath          = smtpLogsPath;
            m_LogPOP3               = logPOP3;
            m_Pop3LogsPath          = pop3LogsPath;
            m_LogIMAP               = logIMAP;
            m_ImapLogsPath          = imapLogsPath;
            m_LogRelay              = logRelay;
            m_RelayLogsPath         = relayLogsPath;
            m_LogFetchMessages      = logFetchMessages;
            m_FetchMessagesLogsPath = fetchMessagesLogsPath;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets or sets if SMTP is logged.
        /// </summary>
        public bool LogSMTP
        {
            get{ return m_LogSMTP; }

            set{
                if(m_LogSMTP != value){
                    m_LogSMTP = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets SMTP logs path.
        /// </summary>
        public string SmtpLogsPath
        {
            get{ return m_SmtpLogsPath; }

            set{
                if(m_SmtpLogsPath != value){
                    m_SmtpLogsPath = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets if POP3 is logged.
        /// </summary>
        public bool LogPOP3
        {
            get{ return m_LogPOP3; }

            set{
                if(m_LogPOP3 != value){
                    m_LogPOP3 = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets POP3 logs path.
        /// </summary>
        public string Pop3LogsPath
        {
            get{ return m_Pop3LogsPath; }

            set{
                if(m_Pop3LogsPath != value){
                    m_Pop3LogsPath = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets if IMAP is logged.
        /// </summary>
        public bool LogIMAP
        {
            get{ return m_LogIMAP; }

            set{
                if(m_LogIMAP != value){
                    m_LogIMAP = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets IMAP logs path.
        /// </summary>
        public string ImapLogsPath
        {
            get{ return m_ImapLogsPath; }

            set{
                if(m_ImapLogsPath != value){
                    m_ImapLogsPath = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets if Relay is logged.
        /// </summary>
        public bool LogRelay
        {
            get{ return m_LogRelay; }

            set{
                if(m_LogRelay != value){
                    m_LogRelay = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets Relay logs path.
        /// </summary>
        public string RelayLogsPath
        {
            get{ return m_RelayLogsPath; }

            set{
                if(m_RelayLogsPath != value){
                    m_RelayLogsPath = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets if remote messages fetching is logged.
        /// </summary>
        public bool LogFetchMessages
        {
            get{ return m_LogFetchMessages; }

            set{
                if(m_LogFetchMessages != value){
                    m_LogFetchMessages = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets fetch messages logs path.
        /// </summary>
        public string FetchMessagesLogsPath
        {
            get{ return m_FetchMessagesLogsPath; }

            set{
                if(m_FetchMessagesLogsPath != value){
                    m_FetchMessagesLogsPath = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        #endregion

    }
}
