using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Implements GlobalMessageRuleProcessor.DoActions method result data.
    /// </summary>
    public class GlobalMessageRuleActionResult
    {
    //  private bool   m_MessageChanged = false;
        private bool   m_DeleteMessage  = false;
        private string m_StoreFolder    = null;
        private string m_ErrorText      = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="deleteMessage">Specifies if message must be deleted.</param>
        /// <param name="storeFolder">Specifies message folder where message must be stored. Pass null if not specified.</param>
        /// <param name="errorText">Gets SMTP error text what to report to connected client. Pass null if not specified.</param>
        internal GlobalMessageRuleActionResult(bool deleteMessage,string storeFolder,string errorText)
        {
            m_DeleteMessage = deleteMessage;
            m_StoreFolder   = storeFolder;
            m_ErrorText     = errorText;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets if message must be deleted.
        /// </summary>
        public bool DeleteMessage
        {
            get{ return m_DeleteMessage; }
        }

        /// <summary>
        /// Gets to what message folder to store message. Returns null if not specified.
        /// </summary>
        public string StoreFolder
        {
            get{ return m_StoreFolder; }
        }

        /// <summary>
        /// Gets SMTP error text what to report to connected client. Returns null if not specified.
        /// </summary>
        public string ErrorText
        {
            get{ return m_ErrorText; }
        }

        #endregion

    }
}
