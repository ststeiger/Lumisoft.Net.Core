using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserMessageRuleAction_PostToNntpNewsgroup object represents user message rule "Post To NNTP Newsgroup" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserMessageRuleAction_PostToNntpNewsgroup : UserMessageRuleActionBase
    {
        private string m_Server    = "";
        private int    m_Port      = 119;
        private string m_Newsgroup = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal UserMessageRuleAction_PostToNntpNewsgroup(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(UserMessageRuleAction_enum.PostToNNTPNewsGroup,rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <Server></Server>
                        <Port></Server>
                        <User></User>
                        <Password></Password>
                        <Newsgroup></Newsgroup>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Server    = table.GetValue("Server");
            m_Port      = Convert.ToInt32(table.GetValue("Port"));
            // table.Add("User","");
            // table.Add("Password","");
            m_Newsgroup = table.GetValue("Newsgroup");            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="host">NTTP server where to post message.</param>
        /// <param name="port">NNTP server port.</param>
        /// <param name="newsgroup">NTTP newsgroup where to post message.</param>
        internal UserMessageRuleAction_PostToNntpNewsgroup(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,string host,int port,string newsgroup) : base(UserMessageRuleAction_enum.PostToNNTPNewsGroup,rule,owner,id,description)
        {
            m_Server    = host;
            m_Port      = port;
            m_Newsgroup = newsgroup;
        }


        #region method Serialize

        /// <summary>
        /// Serialices action object.
        /// </summary>
        /// <returns>Returns serialized action data.</returns>
        internal override byte[] Serialize()
        {
            /*  Action data structure:
                    <ActionData>
                        <Server></Server>
                        <Port></Server>
                        <User></User>
                        <Password></Password>
                        <Newsgroup></Newsgroup>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("Server"   ,m_Server);
            table.Add("Port"     ,m_Port.ToString());
            table.Add("User"     ,"");
            table.Add("Password" ,"");
            table.Add("Newsgroup",m_Newsgroup);

            return table.ToByteData();
        }

        #endregion


        #region Properties Impelementation

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets NTTP server where to post message.
        /// </summary>
        public string Server
        {
            get{ return m_Server; }

            set{
                if(m_Server != value){
                    m_Server = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets NNTP server port.
        /// </summary>
        public int Port
        {
            get{ return m_Port; }

            set{
                if(m_Port != value){
                    m_Port = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets NNTP newsgroup where to post message.
        /// </summary>
        public string Newsgroup
        {
            get{ return m_Newsgroup; }

            set{
                if(m_Newsgroup != value){
                    m_Newsgroup = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
