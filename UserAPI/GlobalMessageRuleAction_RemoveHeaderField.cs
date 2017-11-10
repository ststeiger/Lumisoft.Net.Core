using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The GlobalMessageRuleAction_RemoveHeaderField object represents global message rule "Remove Header Field" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRuleAction_RemoveHeaderField : GlobalMessageRuleActionBase
    {
        private string m_HeaderFieldName = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal GlobalMessageRuleAction_RemoveHeaderField(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <HeaderFieldName></HeaderFieldName>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_HeaderFieldName = table.GetValue("HeaderFieldName");            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="headerField">Header field name what to remove.</param>
        internal GlobalMessageRuleAction_RemoveHeaderField(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,string headerField) : base(rule,owner,id,description)
        {
            m_HeaderFieldName = headerField;
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
                        <HeaderFieldName></HeaderFieldName>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("HeaderFieldName",m_HeaderFieldName);

            return table.ToByteData();
        }

        #endregion


        #region Properties Impelementation

        /// <summary>
        /// Get global message rule action type.
        /// </summary>
        public override GlobalMessageRuleAction_enum ActionType
        {
           get{ return GlobalMessageRuleAction_enum.RemoveHeaderField; }
        }

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets header field name what to remove.
        /// </summary>
        public string HeaderFieldName
        {
            get{ return m_HeaderFieldName; }

            set{
                if(m_HeaderFieldName != value){
                    m_HeaderFieldName = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
