using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// This is base class for user mesage rule actions.
    /// </summary>
    public class UserMessageRuleActionBase
    {
        /// <summary>
        /// Holds flag if some values are changed.
        /// </summary>
        protected bool            m_ValuesChanged = false;

        //--- private variables --------------------------------------
        private UserMessageRule                 m_pRule       = null;
        private UserMessageRuleActionCollection m_pOwner      = null;
        private string                          m_ID          = "";
        private string                          m_Description = "";
        private UserMessageRuleAction_enum      m_ActionType  = UserMessageRuleAction_enum.AutoResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="actionType">Specifies action type.</param>
        /// <param name="rule">Onwer rule that owns this action.</param>
        /// <param name="owner">Owner UserMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        internal UserMessageRuleActionBase(UserMessageRuleAction_enum actionType,UserMessageRule rule,UserMessageRuleActionCollection owner,string id,string description)
        {
            m_ActionType  = actionType;
            m_pRule       = rule;
            m_pOwner      = owner;
            m_ID          = id;
            m_Description = description;
        }

        
        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't chnaged, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }

            /* UpdateUserMessageRuleAction <virtualServerID> "<userRuleID>" "<messageRuleID>" "<messageRuleActionID>" "<description>" <actionType> "<actionData>:base64"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
        
            // Call TCP UpdateUserMessageRuleAction
            m_pRule.Owner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateUserMessageRuleAction " + 
                m_pRule.Owner.VirtualServer.VirtualServerID + " " +
                TextUtils.QuoteString(m_pRule.Owner.Owner.UserID) + " " +
                TextUtils.QuoteString(m_pRule.ID) + " " +
                TextUtils.QuoteString(m_ID) + " " +
                TextUtils.QuoteString(m_Description) + " " +
                ((int)ActionType).ToString() + " " + 
                Convert.ToBase64String(this.Serialize())            
            );
                        
            string response = m_pRule.Owner.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region method Serialize

        /// <summary>
        /// Serialices action object.
        /// </summary>
        /// <returns>Returns serialized action data.</returns>
        internal virtual byte[] Serialize()
        {
            return new byte[0];
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner UserMessageRuleActionCollection that owns this action.
        /// </summary>
        public UserMessageRuleActionCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets user message rule action action ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }
                
        /// <summary>
        /// Get user message rule action type.
        /// </summary>
        public virtual UserMessageRuleAction_enum ActionType
        {
           get{ return m_ActionType; }
        }

        /// <summary>
        /// Gets or sets action description text.
        /// </summary>
        public string Description
        {
            get{ return m_Description; }

            set{
                if(m_Description != value){
                    m_Description = value; 

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
