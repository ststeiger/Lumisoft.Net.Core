using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// This is base class for global mesage rule actions.
    /// </summary>
    public abstract class GlobalMessageRuleActionBase
    {
        /// <summary>
        /// Holds flag if some values are changed.
        /// </summary>
        protected bool            m_ValuesChanged = false;

        //--- private variables --------------------------------------
        private GlobalMessageRule                 m_pRule       = null;
        private GlobalMessageRuleActionCollection m_pOwner      = null;
        private string                            m_ID          = "";
        private string                            m_Description = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Onwer rule that owns this action.</param>
        /// <param name="owner">Owner GlobalMessageRuleActionCollection that owns this action.</param>
        /// <param name="id">Action ID.</param>
        /// <param name="description">Action description.</param>
        internal GlobalMessageRuleActionBase(GlobalMessageRule rule,GlobalMessageRuleActionCollection owner,string id,string description)
        {
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

            /* UpdateGlobalMessageRuleAction <virtualServerID> "<messageRuleID>" "<messageRuleActionID>" "<description>" <actionType> "<actionData>:base64"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
        
            // Call TCP UpdateGlobalMessageRuleAction
            m_pRule.VirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateGlobalMessageRuleAction " + 
                m_pRule.VirtualServer.VirtualServerID + " " +
                TextUtils.QuoteString(m_pRule.ID) + " " +
                TextUtils.QuoteString(m_ID) + " " +
                TextUtils.QuoteString(m_Description) + " " +
                ((int)ActionType).ToString() + " " + 
                Convert.ToBase64String(this.Serialize())            
            );
                        
            string response = m_pRule.VirtualServer.Server.ReadLine();
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
        /// Gets owner GlobalMessageRuleActionCollection that owns this action.
        /// </summary>
        public GlobalMessageRuleActionCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets global message rule action action ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }
                
        /// <summary>
        /// Get global message rule action type.
        /// </summary>
        public virtual GlobalMessageRuleAction_enum ActionType
        {
           get{ return GlobalMessageRuleAction_enum.AutoResponse; }
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
