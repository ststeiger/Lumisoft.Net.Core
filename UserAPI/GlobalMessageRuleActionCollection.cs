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
    /// The GlobalMessageRuleActionCollection object represents global message rule actions in global message rule.
    /// </summary>
    public class GlobalMessageRuleActionCollection : IEnumerable
    {
        private GlobalMessageRule                 m_pRule    = null;
        private List<GlobalMessageRuleActionBase> m_pActions = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Owner global message rule.</param>
        internal GlobalMessageRuleActionCollection(GlobalMessageRule rule)
        {
            m_pRule = rule;
            m_pActions = new List<GlobalMessageRuleActionBase>();

            Bind();
        }

        #region method Add

        #region method Add_AddHeaderField

        /// <summary>
        /// Creates and adds new 'Add Header Field' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="headerFieldName">Header field name.</param>
        /// <param name="headerFieldValue">Header field value.</param>
        /// <returns></returns>
        public GlobalMessageRuleAction_AddHeaderField Add_AddHeaderField(string description,string headerFieldName,string headerFieldValue)
        {
            GlobalMessageRuleAction_AddHeaderField action = new GlobalMessageRuleAction_AddHeaderField(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                headerFieldName,
                headerFieldValue
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_AutoResponse

        /// <summary>
        /// Creates and adds new 'Auto Response' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="from">MAIL FROM: what is reported to destination SMTP server when auto response is sent.</param>
        /// <param name="message">Full auto response message. This must be rfc 2822 defined message.
        /// You can use LumiSoft.Net.Mime class to parse and to construct valid message.</param>
        /// <returns></returns>
        public GlobalMessageRuleAction_AutoResponse Add_AutoResponse(string description,string from,byte[] message)
        {
            GlobalMessageRuleAction_AutoResponse action = new GlobalMessageRuleAction_AutoResponse(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                from,
                message
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_DeleteMessage

        /// <summary>
        /// Creates and adds new 'Delete Message' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        public GlobalMessageRuleAction_DeleteMessage Add_DeleteMessage(string description)
        {
            GlobalMessageRuleAction_DeleteMessage action = new GlobalMessageRuleAction_DeleteMessage(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_ExecuteProgram

        /// <summary>
        /// Creates and adds new 'Execute Program' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="program">Program to execute.</param>
        /// <param name="programArguments">Executable program arguments</param>
        public GlobalMessageRuleAction_ExecuteProgram Add_ExecuteProgram(string description,string program,string programArguments)
        {
            GlobalMessageRuleAction_ExecuteProgram action = new GlobalMessageRuleAction_ExecuteProgram(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                program,
                programArguments
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_ForwardToEmail

        /// <summary>
        /// Creates and adds new 'Forward To Email' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="email">Email address where to forward message.</param>
        public GlobalMessageRuleAction_ForwardToEmail Add_ForwardToEmail(string description,string email)
        {
            GlobalMessageRuleAction_ForwardToEmail action = new GlobalMessageRuleAction_ForwardToEmail(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                email
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_ForwardToHost

        /// <summary>
        /// Creates and adds new 'Forward To Host' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="host">Host name or IP where to forward message.</param>
        /// <param name="port">Destination host port.</param>
        public GlobalMessageRuleAction_ForwardToHost Add_ForwardToHost(string description,string host,int port)
        {
            GlobalMessageRuleAction_ForwardToHost action = new GlobalMessageRuleAction_ForwardToHost(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                host,
                port
            );

            
            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_MoveToImapFolder

        /// <summary>
        /// Creates and adds new 'Move To IMAP Folder' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="folder">IMAP folder where to move message. If specified folder doesn't exist, message is store users Inbox.</param>
        public GlobalMessageRuleAction_MoveToImapFolder Add_MoveToImapFolder(string description,string folder)
        {
            GlobalMessageRuleAction_MoveToImapFolder action = new GlobalMessageRuleAction_MoveToImapFolder(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                folder
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_PostToHttp

        /// <summary>
        /// Creates and adds new 'Post To HTTP' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="url">HTTP URL where to post message.</param>
        public GlobalMessageRuleAction_PostToHttp Add_PostToHttp(string description,string url)
        {
            GlobalMessageRuleAction_PostToHttp action = new GlobalMessageRuleAction_PostToHttp(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                url
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_PostToNntp

        /// <summary>
        /// Creates and adds new 'Post To NNTP Newsgroup' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="host">NTTP server where to post message.</param>
        /// <param name="port">NNTP server port.</param>
        /// <param name="newsgroup">NTTP newsgroup where to post message.</param>
        public GlobalMessageRuleAction_PostToNntpNewsgroup Add_PostToNntp(string description,string host,int port,string newsgroup)
        {
            GlobalMessageRuleAction_PostToNntpNewsgroup action = new GlobalMessageRuleAction_PostToNntpNewsgroup(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                host,
                port,
                newsgroup
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_RemoveHeaderField

        /// <summary>
        /// Creates and adds new 'Remove Header Field' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="headerField">Header field name what to remove.</param>
        public GlobalMessageRuleAction_RemoveHeaderField Add_RemoveHeaderField(string description,string headerField)
        {
            GlobalMessageRuleAction_RemoveHeaderField action = new GlobalMessageRuleAction_RemoveHeaderField(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                headerField
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_SendError

        /// <summary>
        /// Creates and adds new 'Send Error To Client' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="errorText">Error text..</param>
        public GlobalMessageRuleAction_SendError Add_SendError(string description,string errorText)
        {
            GlobalMessageRuleAction_SendError action = new GlobalMessageRuleAction_SendError(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                errorText
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_StoreToDisk

        /// <summary>
        /// Creates and adds new 'Store To Disk Folder' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="folder">Disk folder where to store message.</param>
        public GlobalMessageRuleAction_StoreToDiskFolder Add_StoreToDisk(string description,string folder)
        {
            GlobalMessageRuleAction_StoreToDiskFolder action = new GlobalMessageRuleAction_StoreToDiskFolder(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                folder
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion

        #region method Add_StoreToFtp

        /// <summary>
        /// Creates and adds new 'Store To FTP Folder' action to GlobalMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="host">FTP server where to store message.</param>
        /// <param name="port">FTP server port.</param>
        /// <param name="userName">FTP server user name.</param>
        /// <param name="password">FTP server user password.</param>
        /// <param name="folder">FTP folder where to store message.</param>
        public GlobalMessageRuleAction_StoreToFtp Add_StoreToFtp(string description,string host,int port,string userName,string password,string folder)
        {
            GlobalMessageRuleAction_StoreToFtp action = new GlobalMessageRuleAction_StoreToFtp(
                m_pRule,
                this,
                Guid.NewGuid().ToString(),
                description,
                host,
                port,
                userName,
                password,
                folder
            );

            // Add action to rule
            Add(action);

            m_pActions.Add(action);

            return action;
        }

        #endregion


        #region method Add

        /// <summary>
        /// Adds specified action to global message rule.
        /// </summary>
        /// <param name="action">Action to add.</param>
        private void Add(GlobalMessageRuleActionBase action)
        {
            Add(action.ID,action.Description,action.ActionType,action.Serialize(),false);
        }

        /// <summary>
        /// Adds specified action to connected server.
        /// </summary>
        /// <param name="actionID">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="type">Action type.</param>
        /// <param name="actionData">Action data.</param>
        /// <param name="addToCollection">Specifies if added action must be created locally and added to local collection.</param>
        internal void Add(string actionID,string description,GlobalMessageRuleAction_enum type,byte[] actionData,bool addToCollection)
        {
            /* AddGlobalMessageRuleAction <virtualServerID> "<messageRuleID>" "<actionID>" "<description>" <actionType> "<actionData>:base64"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddGlobalMessageRuleAction
            m_pRule.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddGlobalMessageRuleAction " +
                m_pRule.VirtualServer.VirtualServerID + " " +
                TextUtils.QuoteString(m_pRule.ID) + " " +
                TextUtils.QuoteString(actionID) + " " +
                TextUtils.QuoteString(description) + " " +
                ((int)type).ToString() + " " + 
                Convert.ToBase64String(actionData)
            );
                        
            string response = m_pRule.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            if(addToCollection){
                m_pActions.Add(GetAction(actionID,description,type,actionData));
            }
        }

        #endregion

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes specified action from global message rule.
        /// </summary>
        /// <param name="action">Action to remove.</param>
        public void Remove(GlobalMessageRuleActionBase action)
        {
            /* DeleteGlobalMessageRuleAction <virtualServerID> "<ruleID>" "<actionID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteGlobalMessageRuleAction
            m_pRule.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteGlobalMessageRuleAction " + 
                m_pRule.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pRule.ID) + " " + 
                TextUtils.QuoteString(action.ID)
            );
                        
            string response = m_pRule.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pActions.Remove(action);
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server global message rule actions and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetGlobalMessageRuleActions <virtualServerID> "<messageRuleID>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pRule.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetGlobalMessageRuleActions
                m_pRule.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetGlobalMessageRuleActions " + m_pRule.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pRule.ID));

                string response = m_pRule.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pRule.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
               
                if(ds.Tables.Contains("GlobalMessageRuleActions")){
                    foreach(DataRow dr in ds.Tables["GlobalMessageRuleActions"].Rows){
                        m_pActions.Add(GetAction(
                            dr["ActionID"].ToString(),
                            dr["Description"].ToString(),
                            (GlobalMessageRuleAction_enum)Convert.ToInt32(dr["ActionType"]),
                            Convert.FromBase64String(dr["ActionData"].ToString())
                        ));
                    }
                }
            }
        }

        #endregion

        #region method GetAction

        /// <summary>
        /// Gets global message rule action.
        /// </summary>
        /// <param name="actionID">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionType">Action type.</param>
        /// <param name="actionData">Action data.</param>
        /// <returns></returns>
        private GlobalMessageRuleActionBase GetAction(string actionID,string description,GlobalMessageRuleAction_enum actionType,byte[] actionData)
        {
            if(actionType == GlobalMessageRuleAction_enum.AddHeaderField){
                return new GlobalMessageRuleAction_AddHeaderField(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.AutoResponse){
                return new GlobalMessageRuleAction_AutoResponse(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.DeleteMessage){
                return new GlobalMessageRuleAction_DeleteMessage(m_pRule,this,actionID,description);
            }
            else if(actionType == GlobalMessageRuleAction_enum.ExecuteProgram){
                return new GlobalMessageRuleAction_ExecuteProgram(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.ForwardToEmail){
                return new GlobalMessageRuleAction_ForwardToEmail(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.ForwardToHost){
                return new GlobalMessageRuleAction_ForwardToHost(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.MoveToIMAPFolder){
                return new GlobalMessageRuleAction_MoveToImapFolder(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.PostToHTTP){
                return new GlobalMessageRuleAction_PostToHttp(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.PostToNNTPNewsGroup){
                return new GlobalMessageRuleAction_PostToNntpNewsgroup(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.RemoveHeaderField){
                return new GlobalMessageRuleAction_RemoveHeaderField(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.SendErrorToClient){
                return new GlobalMessageRuleAction_SendError(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.StoreToDiskFolder){
                return new GlobalMessageRuleAction_StoreToDiskFolder(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == GlobalMessageRuleAction_enum.StoreToFTPFolder){
                return new GlobalMessageRuleAction_StoreToFtp(m_pRule,this,actionID,description,actionData);
            }

            throw new Exception("Invalid action type !");
        }

        #endregion


        #region interface IEnumerator

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pActions.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner message rule where this actions apply.
        /// </summary>
        public GlobalMessageRule Rule
        {
            get{ return m_pRule; }
        }

        /// <summary>
        /// Gets number of global message rule actions.
        /// </summary>
        public int Count
        {
            get{ return m_pActions.Count; }
        }

        /// <summary>
        /// Gets a GlobalMessageRuleActionBase object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the GlobalMessageRuleActionBase object in the GlobalMessageRuleActionCollection collection.</param>
        /// <returns>A GlobalMessageRuleActionBase object value that represents the global message rule action in virtual server.</returns>
        public GlobalMessageRuleActionBase this[int index]
        {
            get{ return m_pActions[index]; }
        }

        /// <summary>
        /// Gets a GlobalMessageRuleActionBase object in the collection by global message rule ID.
        /// </summary>
        /// <param name="globalMessageRuleActionID">A String value that specifies the global message rule action ID of the GlobalMessageRuleActionBase object in the GlobalMessageRuleActionCollection collection.</param>
        /// <returns>A GlobalMessageRuleActionBase object value that represents the global message rule action in virtual server.</returns>
        public GlobalMessageRuleActionBase this[string globalMessageRuleActionID]
        {
            get{ 
                foreach(GlobalMessageRuleActionBase action in m_pActions){
                    if(action.ID.ToLower() == globalMessageRuleActionID.ToLower()){
                        return action;
                    }
                }

                throw new Exception("GlobalMessageRule action with specified ID '" + globalMessageRuleActionID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
