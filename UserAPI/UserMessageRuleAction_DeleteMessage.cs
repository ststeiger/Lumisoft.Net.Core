using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserlMessageRuleAction_DeleteMessage object represents user message rule "Delete Message" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserMessageRuleAction_DeleteMessage : UserMessageRuleActionBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        internal UserMessageRuleAction_DeleteMessage(UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description) : base(UserMessageRuleAction_enum.DeleteMessage,rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                    </ActionData>
            */
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
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");

            return table.ToByteData();
        }

        #endregion

    }
}
