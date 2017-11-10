using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The GlobalMessageRuleAction_PostToHttp object represents global message rule "Post To HTTP" action in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRuleAction_PostToHttp : GlobalMessageRuleActionBase
    {
        private string m_Url = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionData">Action data.</param>
        internal GlobalMessageRuleAction_PostToHttp(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,byte[] actionData) : base(rule,owner,id,description)
        {
            /*  Action data structure:
                    <ActionData>
                        <URL></URL>
                        <FileName></FileName>
                    </ActionData>
            */
                        
            XmlTable table = new XmlTable("ActionData");
            table.Parse(actionData);
            m_Url = table.GetValue("URL");
         // table.GetValue("FileName");            
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that ows this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="url">HTTP URL where to post message.</param>
        internal GlobalMessageRuleAction_PostToHttp(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description,string url) : base(rule,owner,id,description)
        {
            m_Url = url;
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
                        <URL></URL>
                        <FileName></FileName>
                    </ActionData>
            */

            XmlTable table = new XmlTable("ActionData");
            table.Add("URL"     ,m_Url);
            table.Add("FileName","");

            return table.ToByteData();
        }

        #endregion


        #region Properties Impelementation

        /// <summary>
        /// Get global message rule action type.
        /// </summary>
        public override GlobalMessageRuleAction_enum ActionType
        {
           get{ return GlobalMessageRuleAction_enum.PostToHTTP; }
        }

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets HTTP URL where to post message.
        /// </summary>
        public string Url
        {
            get{ return m_Url; }

            set{
                if(m_Url != value){
                    m_Url = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
