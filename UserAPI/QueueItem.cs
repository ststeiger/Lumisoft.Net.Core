using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Queues object represents SMTP/Relay queue item in LumiSoft Mail Server virtual server.
    /// </summary>
    public class QueueItem
    {
        private DateTime m_CreateTime;
        private string   m_Header = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="createTime">Queue item create time.</param>
        /// <param name="header">Message header.</param>
        internal QueueItem(DateTime createTime,string header)
        {
            m_CreateTime = createTime;
            m_Header     = header;
        }


        #region Properties Implementation
         
        /// <summary>
        /// Gets queue item create time.
        /// </summary>
        public DateTime CreateTime
        {
            get{ return m_CreateTime; }
        }

        /// <summary>
        /// Gets message header.
        /// </summary>
        public string Header
        {
            get{ return m_Header; }
        }

        #endregion

    }
}
