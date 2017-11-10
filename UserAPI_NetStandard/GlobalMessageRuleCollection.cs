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
    /// The GlobalMessageRuleCollection object represents global message rules in LumiSoft Mail Server virtual server.
    /// </summary>
    public class GlobalMessageRuleCollection : IEnumerable
    {
        private VirtualServer           m_pVirtualServer      = null;
        private List<GlobalMessageRule> m_pGlobalMessageRules = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal GlobalMessageRuleCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer      = virtualServer;
            m_pGlobalMessageRules = new List<GlobalMessageRule>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds new global message rule to virtual server.
        /// </summary>
        /// <param name="enabled">Specifies if global message rules is enabled.</param>
        /// <param name="description">Global message rule description.</param>
        /// <param name="matchExpression">Match expression.</param>
        /// <param name="checkNext">Specifies when next rule is checked.</param>
        /// <returns></returns>
        public GlobalMessageRule Add(bool enabled,string description,string matchExpression,GlobalMessageRule_CheckNextRule_enum checkNext)
        {
            /* AddGlobalMessageRule <virtualServerID> "<ruleID>" <cost> <enabled> "<description>" "<matchExpression>" <checkNext>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id   = Guid.NewGuid().ToString();
            long   cost = DateTime.Now.Ticks;

            // Call TCP AddGlopbalMessageRule
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddGlobalMessageRule " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(id) + " " + 
                cost + " " +
                enabled + " " +
                TextUtils.QuoteString(description) + " " + 
                TextUtils.QuoteString(matchExpression) + " " + 
                (int)checkNext
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            GlobalMessageRule rule = new GlobalMessageRule(m_pVirtualServer,this,id,cost,enabled,description,matchExpression,checkNext);
            m_pGlobalMessageRules.Add(rule);
            return rule;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes specified global message rule from virtual server.
        /// </summary>
        /// <param name="rule">Global message rule to delete.</param>
        public void Remove(GlobalMessageRule rule)
        {
            /* DeleteGlobalMessageRule <virtualServerID> "<ruleID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteGlobalMessageRule
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteGlobalMessageRule " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(rule.ID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pGlobalMessageRules.Remove(rule);
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes rules.
        /// </summary>
        public void Refresh()
        {
            m_pGlobalMessageRules.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server global message rules and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetGlobalMessageRules <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetGlobalMessageRules
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetGlobalMessageRules " + m_pVirtualServer.VirtualServerID);

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("GlobalMessageRules")){
                    foreach(DataRow dr in ds.Tables["GlobalMessageRules"].Rows){
                        m_pGlobalMessageRules.Add(new GlobalMessageRule(
                            m_pVirtualServer,
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
			return m_pGlobalMessageRules.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets number of global message rules in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pGlobalMessageRules.Count; }
        }

        /// <summary>
        /// Gets a GlobalMessageRule object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the GlobalMessageRule object in the GlobalMessageRuleCollection collection.</param>
        /// <returns>A GlobalMessageRule object value that represents the global message rule in virtual server.</returns>
        public GlobalMessageRule this[int index]
        {
            get{ return m_pGlobalMessageRules[index]; }
        }

        /// <summary>
        /// Gets a GlobalMessageRule object in the collection by global message rule ID.
        /// </summary>
        /// <param name="globalMessageRuleID">A String value that specifies the global message rule ID of the GlobalMessageRule object in the GlobalMessageRuleCollection collection.</param>
        /// <returns>A GlobalMessageRule object value that represents the global message rule in virtual server.</returns>
        public GlobalMessageRule this[string globalMessageRuleID]
        {
            get{ 
                foreach(GlobalMessageRule rule in m_pGlobalMessageRules){
                    if(rule.ID.ToLower() == globalMessageRuleID.ToLower()){
                        return rule;
                    }
                }

                throw new Exception("GlobalMessageRule with specified ID '" + globalMessageRuleID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
