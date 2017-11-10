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
    /// The UserMessageRuleActionCollection object represents user message rule actions in global message rule.
    /// </summary>
    public class UserMessageRuleActionCollection
    {
        private UserMessageRule                 m_pRule    = null;
        private List<UserMessageRuleActionBase> m_pActions = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rule">Owner user message rule.</param>
        internal UserMessageRuleActionCollection(UserMessageRule rule)
        {
            m_pRule    = rule;
            m_pActions = new List<UserMessageRuleActionBase>();

            Bind();
        }

        #region method Add

        #region method Add_AddHeaderField

        /// <summary>
        /// Creates and adds new 'Add Header Field' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="headerFieldName">Header field name.</param>
        /// <param name="headerFieldValue">Header field value.</param>
        /// <returns></returns>
        public UserMessageRuleAction_AddHeaderField Add_AddHeaderField(string description,string headerFieldName,string headerFieldValue)
        {
            UserMessageRuleAction_AddHeaderField action = new UserMessageRuleAction_AddHeaderField(
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
        /// Creates and adds new 'Auto Response' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="from">MAIL FROM: what is reported to destination SMTP server when auto response is sent.</param>
        /// <param name="message">Full auto response message. This must be rfc 2822 defined message.
        /// You can use LumiSoft.Net.Mime class to parse and to construct valid message.</param>
        /// <returns></returns>
        public UserMessageRuleAction_AutoResponse Add_AutoResponse(string description,string from,byte[] message)
        {
            UserMessageRuleAction_AutoResponse action = new UserMessageRuleAction_AutoResponse(
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
        /// Creates and adds new 'Delete Message' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        public UserMessageRuleAction_DeleteMessage Add_DeleteMessage(string description)
        {
            UserMessageRuleAction_DeleteMessage action = new UserMessageRuleAction_DeleteMessage(
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
        /// Creates and adds new 'Execute Program' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="program">Program to execute.</param>
        /// <param name="programArguments">Executable program arguments</param>
        public UserMessageRuleAction_ExecuteProgram Add_ExecuteProgram(string description,string program,string programArguments)
        {
            UserMessageRuleAction_ExecuteProgram action = new UserMessageRuleAction_ExecuteProgram(
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
        /// Creates and adds new 'Forward To Email' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="email">Email address where to forward message.</param>
        public UserMessageRuleAction_ForwardToEmail Add_ForwardToEmail(string description,string email)
        {
            UserMessageRuleAction_ForwardToEmail action = new UserMessageRuleAction_ForwardToEmail(
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
        /// Creates and adds new 'Forward To Host' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="host">Host name or IP where to forward message.</param>
        /// <param name="port">Destination host port.</param>
        public UserMessageRuleAction_ForwardToHost Add_ForwardToHost(string description,string host,int port)
        {
            UserMessageRuleAction_ForwardToHost action = new UserMessageRuleAction_ForwardToHost(
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
        /// Creates and adds new 'Move To IMAP Folder' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="folder">IMAP folder where to move message. If specified folder doesn't exist, message is store users Inbox.</param>
        public UserMessageRuleAction_MoveToImapFolder Add_MoveToImapFolder(string description,string folder)
        {
            UserMessageRuleAction_MoveToImapFolder action = new UserMessageRuleAction_MoveToImapFolder(
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
        /// Creates and adds new 'Post To HTTP' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="url">HTTP URL where to post message.</param>
        public UserMessageRuleAction_PostToHttp Add_PostToHttp(string description,string url)
        {
            UserMessageRuleAction_PostToHttp action = new UserMessageRuleAction_PostToHttp(
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
        /// Creates and adds new 'Post To NNTP Newsgroup' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="host">NTTP server where to post message.</param>
        /// <param name="port">NNTP server port.</param>
        /// <param name="newsgroup">NTTP newsgroup where to post message.</param>
        public UserMessageRuleAction_PostToNntpNewsgroup Add_PostToNntp(string description,string host,int port,string newsgroup)
        {
            UserMessageRuleAction_PostToNntpNewsgroup action = new UserMessageRuleAction_PostToNntpNewsgroup(
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
        /// Creates and adds new 'Remove Header Field' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="headerField">Header field name what to remove.</param>
        public UserMessageRuleAction_RemoveHeaderField Add_RemoveHeaderField(string description,string headerField)
        {
            UserMessageRuleAction_RemoveHeaderField action = new UserMessageRuleAction_RemoveHeaderField(
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

        #region method Add_StoreToDisk

        /// <summary>
        /// Creates and adds new 'Store To Disk Folder' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="folder">Disk folder where to store message.</param>
        public UserMessageRuleAction_StoreToDiskFolder Add_StoreToDisk(string description,string folder)
        {
            UserMessageRuleAction_StoreToDiskFolder action = new UserMessageRuleAction_StoreToDiskFolder(
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
        /// Creates and adds new 'Store To FTP Folder' action to UserMessageRuleActionCollection collection.
        /// </summary>
        /// <param name="description">Action description text.</param>
        /// <param name="host">FTP server where to store message.</param>
        /// <param name="port">FTP server port.</param>
        /// <param name="userName">FTP server user name.</param>
        /// <param name="password">FTP server user password.</param>
        /// <param name="folder">FTP folder where to store message.</param>
        public UserMessageRuleAction_StoreToFtp Add_StoreToFtp(string description,string host,int port,string userName,string password,string folder)
        {
            UserMessageRuleAction_StoreToFtp action = new UserMessageRuleAction_StoreToFtp(
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
        /// Adds specified action to user message rule.
        /// </summary>
        /// <param name="action">Action to add.</param>
        private void Add(UserMessageRuleActionBase action)
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
        internal void Add(string actionID,string description,UserMessageRuleAction_enum type,byte[] actionData,bool addToCollection)
        {
            /* AddUserMessageRuleAction <virtualServerID> "<userID>" "<messageRuleID>" "<actionID>" "<description>" <actionType> "<actionData>:base64"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            string id = Guid.NewGuid().ToString();

            // Call TCP AddUserMessageRuleAction
            m_pRule.Owner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddUserMessageRuleAction " +
                m_pRule.Owner.VirtualServer.VirtualServerID + " " +
                TextUtils.QuoteString(m_pRule.Owner.Owner.UserID) + " " +
                TextUtils.QuoteString(m_pRule.ID) + " " +
                TextUtils.QuoteString(actionID) + " " +
                TextUtils.QuoteString(description) + " " +
                ((int)type).ToString() + " " + 
                Convert.ToBase64String(actionData)
            );
                        
            string response = m_pRule.Owner.VirtualServer.Server.ReadLine();
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
        /// Deletes specified action from user message rule.
        /// </summary>
        /// <param name="action">Action to remove.</param>
        public void Remove(UserMessageRuleActionBase action)
        {
            /* DeleteUserMessageRuleAction <virtualServerID> "<ruleID>" "<actionID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteUserMessageRuleAction
            m_pRule.Owner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUserMessageRuleAction " + 
                m_pRule.Owner.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pRule.Owner.Owner.UserID) + " " + 
                TextUtils.QuoteString(m_pRule.ID) + " " + 
                TextUtils.QuoteString(action.ID)
            );
                        
            string response = m_pRule.Owner.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pActions.Remove(action);
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets user  message rule actions and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetUserMessageRuleActions <virtualServerID> "<userID>" "<messageRuleID>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            lock(m_pRule.Owner.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetUserMessageRuleActions
                m_pRule.Owner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserMessageRuleActions " + 
                    m_pRule.Owner.VirtualServer.VirtualServerID + " " + 
                    TextUtils.QuoteString(m_pRule.Owner.Owner.UserID) + " " + 
                    TextUtils.QuoteString(m_pRule.ID)
                );

                string response = m_pRule.Owner.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pRule.Owner.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
               
                if(ds.Tables.Contains("UserMessageRuleActions")){
                    foreach(DataRow dr in ds.Tables["UserMessageRuleActions"].Rows){
                        m_pActions.Add(GetAction(
                            dr["ActionID"].ToString(),
                            dr["Description"].ToString(),
                            (UserMessageRuleAction_enum)Convert.ToInt32(dr["ActionType"]),
                            Convert.FromBase64String(dr["ActionData"].ToString())
                        ));
                    }
                }
            }
        }

        #endregion

        #region method GetAction

        /// <summary>
        /// Gets user message rule action.
        /// </summary>
        /// <param name="actionID">Action ID.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionType">Action type.</param>
        /// <param name="actionData">Action data.</param>
        /// <returns></returns>
        private UserMessageRuleActionBase GetAction(string actionID,string description,UserMessageRuleAction_enum actionType,byte[] actionData)
        {
            if(actionType == UserMessageRuleAction_enum.AddHeaderField){
                return new UserMessageRuleAction_AddHeaderField(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.AutoResponse){
                return new UserMessageRuleAction_AutoResponse(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.DeleteMessage){
                return new UserMessageRuleAction_DeleteMessage(m_pRule,this,actionID,description);
            }
            else if(actionType == UserMessageRuleAction_enum.ExecuteProgram){
                return new UserMessageRuleAction_ExecuteProgram(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.ForwardToEmail){
                return new UserMessageRuleAction_ForwardToEmail(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.ForwardToHost){
                return new UserMessageRuleAction_ForwardToHost(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.MoveToIMAPFolder){
                return new UserMessageRuleAction_MoveToImapFolder(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.PostToHTTP){
                return new UserMessageRuleAction_PostToHttp(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.PostToNNTPNewsGroup){
                return new UserMessageRuleAction_PostToNntpNewsgroup(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.RemoveHeaderField){
                return new UserMessageRuleAction_RemoveHeaderField(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.StoreToDiskFolder){
                return new UserMessageRuleAction_StoreToDiskFolder(m_pRule,this,actionID,description,actionData);
            }
            else if(actionType == UserMessageRuleAction_enum.StoreToFTPFolder){
                return new UserMessageRuleAction_StoreToFtp(m_pRule,this,actionID,description,actionData);
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
        public UserMessageRule Rule
        {
            get{ return m_pRule; }
        }

        /// <summary>
        /// Gets number of user message rule actions.
        /// </summary>
        public int Count
        {
            get{ return m_pActions.Count; }
        }

        /// <summary>
        /// Gets a UserMessageRuleActionBase object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the UserMessageRuleActionBase object in the UserMessageRuleActionCollection collection.</param>
        /// <returns>A UserMessageRuleActionBase object value that represents the user message rule action in virtual server.</returns>
        public UserMessageRuleActionBase this[int index]
        {
            get{ return m_pActions[index]; }
        }

        /// <summary>
        /// Gets a UserMessageRuleActionBase object in the collection by user message rule ID.
        /// </summary>
        /// <param name="userMessageRuleActionID">A String value that specifies the user message rule action ID of the GlobalMessageRuleActionBase object in the GlobalMessageRuleActionCollection collection.</param>
        /// <returns>A UserMessageRuleActionBase object value that represents the user message rule action in virtual server.</returns>
        public UserMessageRuleActionBase this[string userMessageRuleActionID]
        {
            get{ 
                foreach(UserMessageRuleActionBase action in m_pActions){
                    if(action.ID.ToLower() == userMessageRuleActionID.ToLower()){
                        return action;
                    }
                }

                throw new Exception("UserMessageRule action with specified ID '" + userMessageRuleActionID + "' doesn't exist !"); 
            }
        }

        #endregion
    }
}
