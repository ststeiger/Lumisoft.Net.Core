using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
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


        #region overide method Equals

        /// <summary>
        /// Checks if specfied object value is equal to this.
        /// </summary>
        /// <param name="obj">Object to compre.</param>
        /// <returns>Returns true if specified object value equals this value.</returns>
        public override bool Equals(object obj)
        {
            if(obj != null && obj is ServerReturnMessage){
                ServerReturnMessage m = (ServerReturnMessage)obj;
                if(m.Subject != this.Subject){
                    return false;
                }
                if(m.BodyTextRtf != this.BodyTextRtf){
                    return false;
                }

                return true;
            }
            else{
                return base.Equals(obj);
            }
        }

        #endregion

        #region override method GetHashCode

        /// <summary>
        /// Server hash function to particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion


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
