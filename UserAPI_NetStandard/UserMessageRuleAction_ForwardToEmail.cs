using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserMessageRuleAction_ForwardToEmail object represents user message rule "Forward To Email" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserMessageRuleAction_ForwardToEmail : UserMessageRuleActionBase
    {
        private string m_EmailAddress = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal UserMessageRuleAction_ForwardToEmail(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(UserMessageRuleAction_enum.ForwardToEmail,rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <Email></Email>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_EmailAddress = table.GetValue("Email");            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="email">Email address where to forward message.</param>
        internal UserMessageRuleAction_ForwardToEmail(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description,string email) : base(UserMessageRuleAction_enum.ForwardToEmail,rule,owner,id,description)
        {
            m_EmailAddress = email;
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
                        <Email></Email>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("Email",m_EmailAddress);

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
        /// Gets or sets email address where to forward message.
        /// </summary>
        public string EmailAddress
        {
            get{ return m_EmailAddress; }

            set{
                if(m_EmailAddress != value){
                    m_EmailAddress = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
