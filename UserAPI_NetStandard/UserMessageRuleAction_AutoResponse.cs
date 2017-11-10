using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserMessageRuleAction_AutoResponse object represents user message rule "Auto Response" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserMessageRuleAction_AutoResponse : UserMessageRuleActionBase
    {
        private string m_From    = "";
        private byte[] m_Message = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal UserMessageRuleAction_AutoResponse(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(UserMessageRuleAction_enum.AutoResponse,rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <From></From>
                        <Message></Message>
                    </ActionData>
            */

            
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_From    = table.GetValue("From");
            m_Message = System.Text.Encoding.Default.GetBytes(table.GetValue("Message"));  
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="from">MAIL FROM: what is reported to destination SMTP server when auto response is sent.</param>
        /// <param name="message">Full auto response message. This must be rfc 2822 defined message.</param>
        internal UserMessageRuleAction_AutoResponse(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,string from,byte[] message) : base(UserMessageRuleAction_enum.AutoResponse,rule,owner,id,description)
        {
            m_From    = from;
            m_Message = message;
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
                        <From></From>
                        <Message></Message>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("From"   ,m_From);
            table.Add("Message",System.Text.Encoding.Default.GetString(m_Message));

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
        /// Gets or sets MAIL FROM: what is reported to destination SMTP server when auto response is sent.
        /// </summary>
        public string From
        {
            get{ return m_From; }

            set{
                if(m_From != value){
                    m_From = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets full auto response message. This must be rfc 2822 defined message.
        /// You can use LumiSoft.Net.Mime class to parse and to construct valid message.
        /// </summary>
        public byte[] Message
        {
            get{ return m_Message; }

            set{
                if(m_Message != value){
                    m_Message = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
