using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The GlobalMessageRule object represents global message rule in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRule
    {
        private VirtualServer                        m_pVirtualServer  = null;
        private GlobalMessageRuleCollection          m_pOwner          = null;
        private string                               m_ID              = "";
        private long                                 m_Cost            = 0;
        private bool                                 m_Enabled         = false;
        private string                               m_Description     = "";
        private string                               m_MatchExpression = "";
        private GlobalMessageRule_CheckNextRule_enum m_CheckNext       = GlobalMessageRule_CheckNextRule_enum.Always;
        private GlobalMessageRuleActionCollection    m_pActions        = null;
        private bool                                 m_ValuesChanged   = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <param name="owner">Owner GlobalMessageRuleCollection collection that owns this rule.</param>
        /// <param name="id">Global message rule ID.</param>
        /// <param name="cost">Specifies global message rule process oreder number.</param>
        /// <param name="enabled">Specifies if global message rule is enabled.</param>
        /// <param name="description">Global message rule description.</param>
        /// <param name="matchexpression">Global message rule match expression.</param>
        /// <param name="checkNext">Specifies when next rule is checked.</param>
        internal GlobalMessageRule(VirtualServer virtualServer,GlobalMessageRuleCollection owner,string id,long cost,bool enabled,string description,string matchexpression,GlobalMessageRule_CheckNextRule_enum checkNext)
        {
            m_pVirtualServer  = virtualServer;
            m_pOwner          = owner;
            m_ID              = id;
            m_Cost            = cost;
            m_Enabled         = enabled;
            m_Description     = description;
            m_MatchExpression = matchexpression;
            m_CheckNext       = checkNext;
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

            /* UpdateGlobalMessageRule <virtualServerID> "<ruleID>" <cost> <enabled> "<description>" "<matchExpression>" <checkNext>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP UpdateGlobalMessageRule
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateGlobalMessageRule " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_ID) + " " + 
                m_Cost + " " +
                m_Enabled + " " +
                TextUtils.QuoteString(m_Description) + " " + 
                TextUtils.QuoteString(m_MatchExpression) + " " + 
                (int)m_CheckNext
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this group.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets owner GlobalMessageRuleCollection that owns this rule.
        /// </summary>
        public GlobalMessageRuleCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets global message rule ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets or sets if user message rule cost. Cost specifies in what order rules are processes,
        /// rules with lower value are proecessed firts.
        /// </summary>
        public long Cost
        {
            get{ return m_Cost; }

            set{
                if(m_Cost != value){
                    m_Cost = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets if global message rule is enabled.
        /// </summary>
        public bool Enabled
        {
            get{ return m_Enabled; }

            set{
                if(m_Enabled != value){
                    m_Enabled = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets global message rule description.
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

        /// <summary>
        /// Gets or sets match expression.
        /// </summary>
        public string MatchExpression
        {
            get{ return m_MatchExpression; }

            set{
                if(m_MatchExpression != value){
                    m_MatchExpression = value;

                    m_ValuesChanged = true;
                }
            }
        }

        
        /// <summary>
        /// Gets or sets when next rule is checked.
        /// </summary>
        public GlobalMessageRule_CheckNextRule_enum CheckNextRule
        {
            get{ return m_CheckNext; }

            set{
                if(m_CheckNext != value){
                    m_CheckNext = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets global message rule actions.
        /// </summary>
        public GlobalMessageRuleActionCollection Actions
        {
            get{
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pActions == null){
                    m_pActions = new GlobalMessageRuleActionCollection(this);
                }

                return m_pActions;
            }
        }

        #endregion

    }
}
