using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Queues object represents queues in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Queues
    {
        private VirtualServer       m_pServer = null;
        private QueueItemCollection m_pSMTP   = null;
        private QueueItemCollection m_pRelay  = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal Queues(VirtualServer virtualServer)
        {
            m_pServer = virtualServer;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets incoming SMTP queue.
        /// </summary>
        public QueueItemCollection SMTP
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pSMTP == null){
                    m_pSMTP = new QueueItemCollection(m_pServer,true);
                }

                return m_pSMTP; 
            }
        }
        
        /// <summary>
        /// Gets outgoing relay queue.
        /// </summary>
        public QueueItemCollection Relay
        {
            get{
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pRelay == null){
                    m_pRelay = new QueueItemCollection(m_pServer,false);
                }

                return m_pRelay; 
            }
        }

        #endregion

    }
}
