using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserMessageRuleCollection object represents user message rules in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UserMessageRuleCollection
    {
        private User                  m_pUser  = null;
        private List<UserMessageRule> m_pRules = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="user">Owner user that own this collection.</param>
        internal UserMessageRuleCollection(User user)
        {
            m_pUser  = user;
            m_pRules = new List<UserMessageRule>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds new user message rule to virtual server.
        /// </summary>
        /// <param name="enabled">Specifies if user message rules is enabled.</param>
        /// <param name="description">User message rule description.</param>
        /// <param name="matchExpression">Match expression.</param>
        /// <param name="checkNext">Specifies when next rule is checked.</param>
        /// <returns></returns>
        public UserMessageRule Add(bool enabled,string description,string matchExpression,GlobalMessageRule_CheckNextRule_enum checkNext)
        {
            /* AddUserMessageRule <virtualServerID> "<userID>" "<ruleID>" <cost> <enabled> "<description>" "<matchExpression>" <checkNext>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            string id   = Guid.NewGuid().ToString();
            long   cost = DateTime.Now.Ticks;

            // Call TCP AddUserMessageRule
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddUserMessageRule " + 
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pUser.UserID) + " " + 
                TextUtils.QuoteString(id) + " " + 
                cost + " " +
                enabled + " " +
                TextUtils.QuoteString(description) + " " + 
                TextUtils.QuoteString(matchExpression.TrimEnd()) + " " + 
                (int)checkNext
            );
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            UserMessageRule rule = new UserMessageRule(this,id,cost,enabled,description,matchExpression,checkNext);
            m_pRules.Add(rule);
            return rule;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes specified user message rule from virtual server.
        /// </summary>
        /// <param name="rule">User message rule to delete.</param>
        public void Remove(UserMessageRule rule)
        {
            /* DeleteUserMessageRule <virtualServerID> "<userID>" "<ruleID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteUserMessageRule
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUserMessageRule " + 
                m_pUser.VirtualServer.VirtualServerID + " " +
                TextUtils.QuoteString(m_pUser.UserID) + " " +
                TextUtils.QuoteString(rule.ID)
            );
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pRules.Remove(rule);
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes rules.
        /// </summary>
        public void Refresh()
        {
            m_pRules.Clear();
            Bind();
        }

        #endregion

        #region method ToArray

        /// <summary>
        /// Copies collection to array.
        /// </summary>
        /// <returns></returns>
        public UserMessageRule[] ToArray()
        {
            return m_pRules.ToArray();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server global message rules and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetUserMessageRules <virtualServerID> "<userID>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pUser.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetUserMessageRules
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserMessageRules " + 
                    m_pUser.VirtualServer.VirtualServerID + " " + 
                    TextUtils.QuoteString(m_pUser.UserID)
                );

                string response = m_pUser.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("UserMessageRules")){
                    foreach(DataRow dr in ds.Tables["UserMessageRules"].Rows){               
                        m_pRules.Add(new UserMessageRule(
                            this,
                            dr["RuleID"].ToString(),
                            Convert.ToInt64(dr["Cost"]),
                            Convert.ToBoolean(dr["Enabled"]),
                            dr["Description"].ToString(),
                            dr["MatchExpression"].ToString(),
                            (GlobalMessageRule_CheckNextRule_enum)Convert.ToInt32(dr["CheckNextRuleIf"])
                        ));
                    }
                }
            }
        }

        #endregion


        #region interface IEnumerator

		/// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pRules.GetEnumerator();
		}

		#endregion


        #region Properties Implementation
        
        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pUser.VirtualServer; }
        }

        /// <summary>
        /// Gets onwer user that ownt this UserMessageRuleCollection.
        /// </summary>
        public User Owner
        {
            get{ return m_pUser; }
        }

        /// <summary>
        /// Gets number of user message rules in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pRules.Count; }
        }

        /// <summary>
        /// Gets a UserMessageRule object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the UserMessageRule object in the UserMessageRuleCollection collection.</param>
        /// <returns>A UserMessageRule object value that represents the user message rule in virtual server.</returns>
        public UserMessageRule this[int index]
        {
            get{ return m_pRules[index]; }
        }

        /// <summary>
        /// Gets a UserMessageRule object in the collection by user message rule ID.
        /// </summary>
        /// <param name="ruleID">A String value that specifies the user message rule ID of the UserMessageRule object in the UserMessageRuleCollection collection.</param>
        /// <returns>A UserMessageRule object value that represents the user message rule in virtual server.</returns>
        public UserMessageRule this[string ruleID]
        {
            get{ 
                foreach(UserMessageRule rule in m_pRules){
                    if(rule.ID.ToLower() == ruleID.ToLower()){
                        return rule;
                    }
                }

                throw new Exception("UserMessageRule with specified ID '" + ruleID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
