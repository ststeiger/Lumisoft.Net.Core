using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The FetchMessages_Settings object represents Fetch remote server messages settings in LumiSoft Mail Server virtual server.
    /// </summary>
    public class FetchMessages_Settings
    {
        private System_Settings m_pSysSettings  = null;
        private bool            m_Enabled       = false;
        private int             m_FetchInterval = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sysSettings">Reference to system settings.</param>
        /// <param name="enabled">Specifies if remote messages fetching service is enabled.</param>
        /// <param name="fetchInterval">Messages fetching inteval seconds.</param>
        internal FetchMessages_Settings(System_Settings sysSettings,bool enabled,int fetchInterval)
        {
            m_pSysSettings  = sysSettings;
            m_Enabled       = enabled;
            m_FetchInterval = fetchInterval;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets or sets if remote messages fetching service is enabled.
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
        /// Gets or sets how fetch seconds, how often to fetch messages from remote server.
        /// </summary>
        public int FetchInterval
        {
            get{ return m_FetchInterval; }

            set{
                if(m_FetchInterval != value){
                    m_FetchInterval = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        #endregion

    }
}
