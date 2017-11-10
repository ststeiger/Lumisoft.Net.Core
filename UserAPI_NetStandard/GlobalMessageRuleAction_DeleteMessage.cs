using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The GlobalMessageRuleAction_DeleteMessage object represents global message rule "Delete Message" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRuleAction_DeleteMessage : GlobalMessageRuleActionBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        internal GlobalMessageRuleAction_DeleteMessage(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description) : base(rule,owner,id,description)
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


        #region Properties Impelementation

        /// <summary>
        /// Get global message rule action type.
        /// </summary>
        public override GlobalMessageRuleAction_enum ActionType
        {
           get{ return GlobalMessageRuleAction_enum.DeleteMessage; }
        }

        #endregion

    }
}
