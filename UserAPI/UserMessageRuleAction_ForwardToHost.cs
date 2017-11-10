using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserMessageRuleAction_ForwardToHost object represents user message rule "Forward To Host" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserMessageRuleAction_ForwardToHost : UserMessageRuleActionBase
    {
        private string m_Host = "";
        private int    m_Port = 25;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal UserMessageRuleAction_ForwardToHost(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(UserMessageRuleAction_enum.ForwardToHost,rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <Host></Host>
                        <Port></Port>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Host = table.GetValue("Host");
            m_Port = Convert.ToInt32(table.GetValue("Port"));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="host">Host name or IP where to forward message.</param>
        /// <param name="port">Destination host port.</param>
        internal UserMessageRuleAction_ForwardToHost(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,string host,int port) : base(UserMessageRuleAction_enum.ForwardToHost,rule,owner,id,description)
        {
            m_Host = host;
            m_Port = port;
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
                        <Host></Host>
                        <Port></Port>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("Host",m_Host);
            table.Add("Port",m_Port.ToString());

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
        /// Gets or sets host name or IP where to forward message.
        /// </summary>
        public string Host
        {
            get{ return m_Host; }

            set{
                if(m_Host != value){
                    m_Host = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets destination host port.
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

        #endregion

    }
}
