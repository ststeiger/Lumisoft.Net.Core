using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The ServerReturnMessages object represents server return messages in LumiSoft Mail Server virtual server.
    /// </summary>
    public class ServerReturnMessages
    {
        private System_Settings     m_pSysSettings            = null;
        private ServerReturnMessage m_pDelayedDeliveryWarning = null;
        private ServerReturnMessage m_pUndelivered            = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sysSettings">Reference to system settings.</param>
        /// <param name="delayedDeliveryWarning">Delayed delivery warning message template.</param>
        /// <param name="undelivered">Undelivered notice message template</param>
        internal ServerReturnMessages(System_Settings sysSettings,ServerReturnMessage delayedDeliveryWarning,ServerReturnMessage undelivered)
        {
            m_pSysSettings            = sysSettings;
            m_pDelayedDeliveryWarning = delayedDeliveryWarning;
            m_pUndelivered            = undelivered;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets or sets message template what is sent when message delayed delivery, immediate delivery failed.
        /// </summary>
        public ServerReturnMessage DelayedDeliveryWarning
        {
            get{ return m_pDelayedDeliveryWarning; }

            set{ 
                if(value == null){
                    throw new ArgumentNullException("DelayedDeliveryWarning");
                }
                
                if(!m_pDelayedDeliveryWarning.Equals(value)){
                    m_pDelayedDeliveryWarning = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }


        /// <summary>
        /// Gets or sets message template what is sent when message delivery has failed.
        /// </summary>
        public ServerReturnMessage Undelivered
        {
            get{ return m_pUndelivered; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Undelivered");
                }

                if(!m_pUndelivered.Equals(value)){
                    m_pUndelivered = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        #endregion

    }
}
