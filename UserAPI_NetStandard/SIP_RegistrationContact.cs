using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// This class represents SIP registration contact.
    /// </summary>
    public class SipRegistrationContact
    {
        private string m_ContactUri = "";
        private int    m_Expires    = 0;
        private double m_Priority   = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="contactUri">Contact URI.</param>
        /// <param name="exprires">Seconds after how many seconds this contact will expire.</param>
        /// <param name="priority">Contact priority.</param>
        public SipRegistrationContact(string contactUri,int exprires,double priority)
        {
            m_ContactUri = contactUri;
            m_Expires    = exprires;
            m_Priority   = priority;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets contact URI.
        /// </summary>
        public string ContactUri
        {
            get{ return m_ContactUri; }
        }

        /// <summary>
        /// Gets after how many seconds this contact will expire.
        /// </summary>
        public int Expires
        {
            get{ return m_Expires; }
        }

        /// <summary>
        /// Gets contact priority. Higer value means higher priority.
        /// </summary>
        public double Priority
        {
            get{
                if(m_Priority == -1){
                    return (double)1.0;
                }
                else{
                    return m_Priority;
                }                
            }
        }

        #endregion

    }
}
