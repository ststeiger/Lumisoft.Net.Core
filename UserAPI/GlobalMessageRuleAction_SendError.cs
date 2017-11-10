using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The GlobalMessageRuleAction_SendError object represents global message rule "Send Error To Client" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRuleAction_SendError : GlobalMessageRuleActionBase
    {
        private string m_ErrorText = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal GlobalMessageRuleAction_SendError(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <ErrorText></ErrorText>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_ErrorText = table.GetValue("ErrorText");            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="errorText">Error text.</param>
        internal GlobalMessageRuleAction_SendError(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,string errorText) : base(rule,owner,id,description)
        {
            m_ErrorText = errorText;
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
                        <ErrorText></ErrorText>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("ErrorText",m_ErrorText);

            return table.ToByteData();
        }

        #endregion


        #region Properties Impelementation

        /// <summary>
        /// Get global message rule action type.
        /// </summary>
        public override GlobalMessageRuleAction_enum ActionType
        {
           get{ return GlobalMessageRuleAction_enum.SendErrorToClient; }
        }

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets SMTP error text to return connected client.
        /// </summary>
        public string SmtpErrorText
        {
            get{ return m_ErrorText; }

            set{
                if(m_ErrorText != value){
                    m_ErrorText = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
