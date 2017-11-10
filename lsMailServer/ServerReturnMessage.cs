using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// This class represents server return message template. 
    /// For example for undelivered messagege notice, delayed delivery warning, ... .
    /// </summary>
    public class ServerReturnMessage
    {
        private string m_Subject     = "";
        private string m_BodyTextRtf = ""; 

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="subject">Message subject template.</param>
        /// <param name="bodyTextRft">Messge body text template in RTF format.</param>
        public ServerReturnMessage(string subject,string bodyTextRft)
        {
            m_Subject     = subject;
            m_BodyTextRtf = bodyTextRft;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets message subject template.
        /// </summary>
        public string Subject
        {
            get{ return m_Subject; }
        }

        /// <summary>
        /// Gets body text template in RTF format.
        /// </summary>
        public string BodyTextRtf
        {
            get{ return m_BodyTextRtf; }
        }

        #endregion

    }
}
