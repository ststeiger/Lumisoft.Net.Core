using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The IMAP_Settings object represents IMAP settings in LumiSoft Mail Server virtual server.
    /// </summary>
    public class IMAP_Settings
    {
        private System_Settings m_pSysSettings   = null;
        private bool            m_Enabled        = false;
        private string          m_Greeting       = "";
        private int             m_IdleTimeout    = 0;
        private int             m_MaxConnections = 0;
        private int             m_MaxConnsPerIP  = 0;
        private int             m_MaxBadCommands = 0;
        private IPBindInfo[]    m_pBinds         = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sysSettings">Reference to system settings.</param>
        /// <param name="enabled">Specifies if IMAP service is enabled.</param>
        /// <param name="greeting">Greeting text.</param>
        /// <param name="idleTimeout">Session idle timeout seconds.</param>
        /// <param name="maxConnections">Maximum conncurent connections.</param>
        /// <param name="maxConnectionsPerIP">Maximum conncurent connections fro 1 IP address.</param>
        /// <param name="maxBadCommands">Maximum bad commands per session.</param>
        /// <param name="bindings">Specifies IMAP listening info.</param>
        internal IMAP_Settings(System_Settings sysSettings,bool enabled,string greeting,int idleTimeout,int maxConnections,int maxConnectionsPerIP,int maxBadCommands,IPBindInfo[] bindings)
        {
            m_pSysSettings   = sysSettings;
            m_Enabled        = enabled;
            m_Greeting       = greeting;
            m_IdleTimeout    = idleTimeout;
            m_MaxConnections = maxConnections;
            m_MaxConnsPerIP  = maxConnectionsPerIP;
            m_MaxBadCommands = maxBadCommands;
            m_pBinds         = bindings;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets or sets if IMAP server is enabled.
        /// </summary>
        public bool Enabled
        {
            get{ return m_Enabled; }

            set{
                if(m_Enabled != value){
                    m_Enabled = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets greeting text reported to connected clients. If "", default server greeting text is used.
        /// </summary>
        public string GreetingText
        {
            get{ return m_Greeting; }

            set{
                if(m_Greeting != value){
                    m_Greeting = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets how many seconds session can idle before timed out.
        /// </summary>
        public int SessionIdleTimeOut
        {
            get{ return m_IdleTimeout; }

            set{
                if(m_IdleTimeout != value){
                    m_IdleTimeout = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum conncurent connections server accepts.
        /// </summary>
        public int MaximumConnections
        {
            get{ return m_MaxConnections; }

            set{
                if(m_MaxConnections != value){
                    m_MaxConnections = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum conncurent connections from 1 IP address. Value 0, means unlimited connections.
        /// </summary>
        public int MaximumConnectionsPerIP
        {
            get{ return m_MaxConnsPerIP; }

            set{
                if(m_MaxConnsPerIP != value){
                    m_MaxConnsPerIP = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum bad commands can happen before server terminates connection.
        /// </summary>
        public int MaximumBadCommands
        {
            get{ return m_MaxBadCommands; }

            set{
                if(m_MaxBadCommands != value){
                    m_MaxBadCommands = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets IP bindings.
        /// </summary>
        public IPBindInfo[] Binds
        {
            get{ return m_pBinds; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Binds");
                }                

                if(!Net_Utils.CompareArray(m_pBinds,value)){
                    m_pBinds = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        #endregion

    }
}
