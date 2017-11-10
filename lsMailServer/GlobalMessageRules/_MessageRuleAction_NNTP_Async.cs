using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using LumiSoft.Net.NNTP.Client;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// NNTP asynchronous message poster. This class is used by internally by 'Store To NNTP Newsgroup' message rule action.
    /// </summary>
    internal class _MessageRuleAction_NNTP_Async
    {
        private string       m_Server         = "";
        private int          m_Port           = 119;
        private string       m_Newsgroup      = "";
        private MemoryStream m_pMessageStream = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">NNTP server.</param>
        /// <param name="port">NNTP server port. Default is 119.</param>
        /// <param name="newsgroup">NNTP newsgroup where to post message.</param>
        /// <param name="message">Message to post.</param>
        public _MessageRuleAction_NNTP_Async(string server,int port,string newsgroup,MemoryStream message)
        {
            m_Server         = server;
            m_Port           = port;
            m_Newsgroup      = newsgroup;
            m_pMessageStream = message;

            Thread tr = new Thread(new ThreadStart(this.Post));
            tr.Start();
        }


        #region method Post

        /// <summary>
        /// Posts message.
        /// </summary>
        private void Post()
        {     
            try{
                using(NNTP_Client nntp = new NNTP_Client()){
                    nntp.Connect(m_Server,m_Port);
                                                            
                    nntp.PostMessage(m_Newsgroup,m_pMessageStream);
                }
            }
            catch(Exception x){
                Error.DumpError(x,new System.Diagnostics.StackTrace());
            }
        }

        #endregion

    }
}
