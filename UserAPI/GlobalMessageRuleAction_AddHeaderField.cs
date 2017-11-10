using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The GlobalMessageRuleAction_AddHeaderField object represents global message rule "Add Header Field" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRuleAction_AddHeaderField : GlobalMessageRuleActionBase
    {
        private string m_HeaderFieldName  = "";
        private string m_HeaderFieldValue = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal GlobalMessageRuleAction_AddHeaderField(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <HeaderFieldName></HeaderFieldName>
                        <HeaderFieldValue></HeaderFieldValue>
                    </ActionData>
            */

            
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_HeaderFieldName  = table.GetValue("HeaderFieldName");
            m_HeaderFieldValue = table.GetValue("HeaderFieldValue");
        }

        /// <summary>
        /// Default constructor. 
        /// </summary>
        /// <param name="rule">Onwer rule that owns this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="headerFieldName">Header field name.</param>
        /// <param name="headerFieldValue">Header field name.</param>
        internal GlobalMessageRuleAction_AddHeaderField(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,string headerFieldName,string headerFieldValue) : base(rule,owner,id,description)
        {
            m_HeaderFieldName  = headerFieldName;
            m_HeaderFieldValue = headerFieldValue;
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
                        <HeaderFieldValue></HeaderFieldValue>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("HeaderFieldName" ,m_HeaderFieldName);
            table.Add("HeaderFieldValue",m_HeaderFieldValue);

            return table.ToByteData();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Get global message rule action type.
        /// </summary>
        public override GlobalMessageRuleAction_enum ActionType
        {
           get{ return GlobalMessageRuleAction_enum.AddHeaderField; }
        }

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets header field name what is added.
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

        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public string HeaderFieldValue
        {
            get{ return m_HeaderFieldValue; }

            set{
                if(m_HeaderFieldValue != value){
                    m_HeaderFieldValue = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
