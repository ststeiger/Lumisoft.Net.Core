using System;
using System.IO;
using System.Data;
using System.Net;
using System.Collections.Generic;

using LumiSoft.Net;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.IMAP.Server;

namespace LumiSoft.MailServer
{	
	/// <summary>
	/// Mailserver API interface.
	/// </summary>
	public interface IMailServerApi
	{	

		#region Domain related

		#region method GetDomains

		/// <summary>
		/// Gets domain list.
		/// </summary>
		/// <returns></returns>
		DataView GetDomains();		

		#endregion


		#region method AddDomain

		/// <summary>
		/// Adds new domain.
		/// </summary>
		/// <param name="domainID">Domain ID. Suggested value is Guid.NewGuid() .</param>
		/// <param name="domainName">Domain name. Eg. yourDomain.com .</param>
		/// <param name="description">Domain description.</param>
		/// <remarks>Throws exception if specified domain already exists.</remarks>
		void AddDomain(string domainID,string domainName,string description);

		#endregion

		#region method DeleteDomain

		/// <summary>
		/// Deletes specified domain.
		/// </summary>
		/// <param name="domainID">Domain name. Use <see cref="IMailServerApi.GetDomains">GetDomains()</see> to get valid values.</param>
		/// <remarks>Deletes specified domain and all domain related data (users,mailing lists,routes).</remarks>
		void DeleteDomain(string domainID);

		#endregion

        #region method UpdateDomain

        /// <summary>
        /// Updates specified domain data.
        /// </summary>
        /// <param name="domainID">Domain ID which to update.</param>
        /// <param name="domainName">Domain name.</param>
        /// <param name="description">Domain description.</param>
        void UpdateDomain(string domainID,string domainName,string description);

        #endregion

        #region method DomainExists

        /// <summary>
		/// Checks if specified domain exists.
		/// </summary>
		/// <param name="source">Domain name or email address.</param>
		/// <returns>Returns true if domain exists.</returns>
		bool DomainExists(string source);

		#endregion

		#endregion


		#region User and Groups related

		#region method GetUsers

		/// <summary>
		/// Gets user list in specified domain.
		/// </summary>
		/// <param name="domainName">Domain which user list to retrieve.To get all use value 'ALL'.</param>
		/// <returns></returns>
		DataView GetUsers(string domainName);

		#endregion


        #region method GetUserID

        /// <summary>
        /// Gets user ID from user name. Returns null if user doesn't exist.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns>Returns user ID or null if user doesn't exist.</returns>
        string GetUserID(string userName);

        #endregion

        #region method AddUser

        /// <summary>
		/// Adds new user to specified domain.
		/// </summary>
		/// <param name="userID">User ID. Suggested value is Guid.NewGuid() .</param>
		/// <param name="userName">User login name.</param>
		/// <param name="fullName">User full name.</param> 
		/// <param name="password">User login password.</param>
		/// <param name="description">User description.</param>
		/// <param name="domainName">Domain where to add user. Use <see cref="IMailServerApi.GetDomains">GetDomains()</see> to get valid values.</param>
		/// <param name="mailboxSize">Maximum mailbox size.</param>
		/// <param name="enabled">Sepcifies if user is enabled.</param>
		/// <param name="permissions">Specifies user permissions.</param>
		/// <remarks>Throws exception if specified user already exists.</remarks>
		void AddUser(string userID,string userName,string fullName,string password,string description,string domainName,int mailboxSize,bool enabled,UserPermissions_enum permissions);
	
		#endregion

		#region method DeleteUser

		/// <summary>
		/// Deletes user.
		/// </summary>
		/// <param name="userID">User ID of the user which to delete. Use <see cref="IMailServerApi.GetUsers">>GetUsers()</see> to get valid values.</param>
		void DeleteUser(string userID);
		
		#endregion

		#region method UpdateUser

		/// <summary>
		/// Updates new user to specified domain.
		/// </summary>
		/// <param name="userID">User id of the user which to update. Use <see cref="IMailServerApi.GetUsers">>GetUsers()</see> to get valid values.</param>
		/// <param name="userName">User login name.</param>
		/// <param name="fullName">User full name.</param>
		/// <param name="password">User login password.</param>
		/// <param name="description">User description.</param>
		/// <param name="domainName">Domain where to add user. Use <see cref="IMailServerApi.GetDomains">>GetDomains()</see> to get valid values.</param>
		/// <param name="mailboxSize">Maximum mailbox size.</param>
		/// <param name="enabled">Sepcifies if user is enabled.</param>
		/// <param name="permissions">Specifies user permissions.</param>
		void UpdateUser(string userID,string userName,string fullName,string password,string description,string domainName,int mailboxSize,bool enabled,UserPermissions_enum permissions);
		
		#endregion

		#region method AddUserAddress

		/// <summary>
		/// Add new email address to user.
		/// </summary>
		/// <param name="userName">User name. Use <see cref="IMailServerApi.GetUsers">>GetUsers()</see> to get valid values.</param>
		/// <param name="emailAddress">Email address to add.</param>
		/// <remarks>Throws exception if specified user email address exists.</remarks>
		void AddUserAddress(string userName,string emailAddress);

		#endregion

		#region method DeleteUserAddress

		/// <summary>
		/// Deletes specified email address from user. 
		/// </summary>
		/// <param name="emailAddress">Email address to delete.</param>
		void DeleteUserAddress(string emailAddress);

		#endregion

		#region method GetUserAddresses

		/// <summary>
		/// Gets user email addresses.
		/// </summary>
		/// <param name="userName"> Use <see cref="IMailServerApi.GetUsers">GetUsers()</see> to get valid values.</param>
		DataView GetUserAddresses(string userName);

		#endregion

		#region method UserExists

		/// <summary>
		/// Checks if user exists.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <returns>Returns true if user exists.</returns>
		bool UserExists(string userName);
		
		#endregion

		#region method MapUser

		/// <summary>
		/// Maps email address to mailbox.
		/// </summary>
		/// <param name="emailAddress"></param>
		/// <returns>Returns mailbox or null if such email address won't exist.</returns>
		string MapUser(string emailAddress);
		
		#endregion

		#region method ValidateMailboxSize

		/// <summary>
		/// Checks if specified mailbox size is exceeded.
		/// </summary>
		/// <param name="userName">User name. Use <see cref="IMailServerApi.GetUsers">GetUsers()</see> to get valid values.</param>
		/// <returns>Returns true if exceeded.</returns>
		bool ValidateMailboxSize(string userName);
		
		#endregion

        #region method GetUserPermissions

        /// <summary>
        /// Gets specified user permissions.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <returns></returns>
        UserPermissions_enum GetUserPermissions(string userName);

        #endregion

        #region method GetUserLastLoginTime

        /// <summary>
        /// Gets user last login time.
        /// </summary>
        /// <param name="userName">User name who's last login time to get.</param>
        /// <returns>User last login time.</returns>
        DateTime GetUserLastLoginTime(string userName);

        #endregion

        #region method UpdateUserLastLoginTime

        /// <summary>
        /// Updates user last login time.
        /// </summary>
        /// <param name="userName">User name who's last login time to update.</param>
        void UpdateUserLastLoginTime(string userName);

        #endregion


        #region method GetUserRemoteServers

        /// <summary>
		/// Gets user pop3 remote accounts.
		/// </summary>
		/// <param name="userName">User name. Use <see cref="IMailServerApi.GetUsers">GetUsers()</see> to get valid values.</param>
		/// <returns></returns>
		DataView GetUserRemoteServers(string userName);

		#endregion

		#region method AddUserRemoteServer

		/// <summary>
		/// Adds new remote pop3 server to user.
		/// </summary>
		/// <param name="serverID">Server ID. Suggested value is Guid.NewGuid() .</param>
		/// <param name="userName">User name. Use <see cref="IMailServerApi.GetUsers">GetUsers()</see> to get valid values.</param>
		/// <param name="description">Remote server description.</param>
		/// <param name="remoteServer">Remote server name.</param>
		/// <param name="remotePort">Remote server port.</param>
		/// <param name="remoteUser">Remote server user name.</param>
		/// <param name="remotePassword">Remote server password.</param>
		/// <param name="useSSL">Specifies if SSL must be used to connect to remote server.</param>
		/// <param name="enabled">Specifies if remote server is enabled.</param>
		/// <remarks>Throws exception if specified user remote server already exists.</remarks>
		void AddUserRemoteServer(string serverID,string userName,string description,string remoteServer,int remotePort,string remoteUser,string remotePassword,bool useSSL,bool enabled);

		#endregion

		#region method DeleteUserRemoteServer

		/// <summary>
		/// Deletes specified pop3 remote account from user.
		/// </summary>
		/// <param name="serverID">Remote server ID. Use <see cref="IMailServerApi.GetUserRemoteServers">GetUserRemoteServers()</see> to get valid values.</param>
		void DeleteUserRemoteServer(string serverID);

		#endregion

        #region mehtod UpdateUserRemoteServer

        /// <summary>
		/// Updates user remote pop3 server.
		/// </summary>
		/// <param name="serverID">Server ID. Suggested value is Guid.NewGuid() .</param>
		/// <param name="userName">User name. Use <see cref="IMailServerApi.GetUsers">GetUsers()</see> to get valid values.</param>
		/// <param name="description">Remote server description.</param>
		/// <param name="remoteServer">Remote server name.</param>
		/// <param name="remotePort">Remote server port.</param>
		/// <param name="remoteUser">Remote server user name.</param>
		/// <param name="remotePassword">Remote server password.</param>
		/// <param name="useSSL">Specifies if SSL must be used to connect to remote server.</param>
		/// <param name="enabled">Specifies if remote server is enabled.</param>
		/// <remarks>Throws exception if specified user remote server already exists.</remarks>
		void UpdateUserRemoteServer(string serverID,string userName,string description,string remoteServer,int remotePort,string remoteUser,string remotePassword,bool useSSL,bool enabled);

        #endregion


        #region method GetUserMessageRules

        /// <summary>
		/// Gets user message  rules.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <returns></returns>
		DataView GetUserMessageRules(string userName);

		#endregion

		#region method AddUserMessageRule

		/// <summary>
        /// Adds new user message rule.
        /// </summary>
        /// <param name="userID">User who owns specified rule.</param>
        /// <param name="ruleID">Rule ID. Guid.NewID().ToString() is suggested.</param>
        /// <param name="cost">Cost specifies in what order rules are processed. Costs with lower values are processed first.</param>
        /// <param name="enabled">Specifies if rule is enabled.</param>
        /// <param name="checkNextRule">Specifies when next rule is checked.</param>
        /// <param name="description">Rule description.</param>
        /// <param name="matchExpression">Rule match expression.</param>
        void AddUserMessageRule(string userID,string ruleID,long cost,bool enabled,GlobalMessageRule_CheckNextRule_enum checkNextRule,string description,string matchExpression);

		#endregion

		#region method DeleteUserMessageRule

		/// <summary>
		/// Deletes specified user message rule.
		/// </summary>
        /// <param name="userID">User who owns specified rule.</param>
		/// <param name="ruleID">Rule ID.</param>
		void DeleteUserMessageRule(string userID,string ruleID);

		#endregion

		#region method UpdateUserMessageRule

		/// <summary>
        /// Updates specified user message rule.
        /// </summary>
        /// <param name="userID">User who owns specified rule.</param>
        /// <param name="ruleID">Rule ID.</param>
        /// <param name="cost">Cost specifies in what order rules are processed. Costs with lower values are processed first.</param>
        /// <param name="enabled">Specifies if rule is enabled.</param>
        /// <param name="checkNextRule">Specifies when next rule is checked.</param>
        /// <param name="description">Rule description.</param>
        /// <param name="matchExpression">Rule match expression.</param>
        void UpdateUserMessageRule(string userID,string ruleID,long cost,bool enabled,GlobalMessageRule_CheckNextRule_enum checkNextRule,string description,string matchExpression);

		#endregion

        #region method GetUserMessageRuleActions

        /// <summary>
        /// Gets specified user message rule actions.
        /// </summary>
        /// <param name="userID">User who owns specified rule.</param>
        /// <param name="ruleID">Rule ID of rule which actions to get.</param>
        DataView GetUserMessageRuleActions(string userID,string ruleID);

        #endregion

        #region method AddUserMessageRuleAction

        /// <summary>
        /// Adds action to specified user message rule.
        /// </summary>
        /// <param name="userID">User who owns specified rule.</param>
        /// <param name="ruleID">Rule ID to which to add this action.</param>
        /// <param name="actionID">Action ID. Guid.NewID().ToString() is suggested.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionType">Action type.</param>
        /// <param name="actionData">Action data. Data structure depends on action type.</param>
        void AddUserMessageRuleAction(string userID,string ruleID,string actionID,string description,GlobalMessageRuleAction_enum actionType,byte[] actionData);

        #endregion

        #region method DeleteUserMessageRuleAction

        /// <summary>
        /// Deletes specified action from specified user message rule.
        /// </summary>
        /// <param name="userID">User who owns specified rule.</param>
        /// <param name="ruleID">Rule ID which action to delete.</param>
        /// <param name="actionID">Action ID of action which to delete.</param>
        void DeleteUserMessageRuleAction(string userID,string ruleID,string actionID);

        #endregion

        #region method UpdateUserMessageRuleAction

        /// <summary>
        /// Updates specified rule action.
        /// </summary>
        /// <param name="userID">User who owns specified rule.</param>
        /// <param name="ruleID">Rule ID which action to update.</param>
        /// <param name="actionID">Action ID of action which to update.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionType">Action type.</param>
        /// <param name="actionData">Action data. Data structure depends on action type.</param>
        void UpdateUserMessageRuleAction(string userID,string ruleID,string actionID,string description,GlobalMessageRuleAction_enum actionType,byte[] actionData);

        #endregion


        #region method AuthUser

        /// <summary>
		/// Authenticates user.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="passwData">Password data.</param>
		/// <param name="authData">Authentication specific data(as tag).</param>
		/// <param name="authType">Authentication type.</param>
		/// <returns></returns>
		DataSet AuthUser(string userName,string passwData,string authData,AuthType authType);
		
		#endregion


        #region method GroupExists

        /// <summary>
        /// Gets if specified user group exists.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <returns>Returns true, if user group exists.</returns>
        bool GroupExists(string groupName);

        #endregion

        #region method GetGroups

        /// <summary>
        /// Gets user groups.
        /// </summary>
        /// <returns></returns>
        DataView GetGroups();

        #endregion

        #region method AddGroup

        /// <summary>
        /// Adds new user group.
        /// </summary>
        /// <param name="groupID">Group ID. Guid.NewGuid().ToString() is suggested.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="description">Group description.</param>
        /// <param name="enabled">Specifies if group is enabled.</param>
        void AddGroup(string groupID,string groupName,string description,bool enabled);
       
        #endregion

        #region method DeleteGroup

        /// <summary>
        /// Deletes specified user group.
        /// </summary>
        /// <param name="groupID">Group ID.</param>
        void DeleteGroup(string groupID);

        #endregion

        #region method UpdateGroup

        /// <summary>
        /// Updates user group info.
        /// </summary>
        /// <param name="groupID">Group ID.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="description">Group description.</param>
        /// <param name="enabled">Specifies if group is enabled.</param>
        void UpdateGroup(string groupID,string groupName,string description,bool enabled);
        
        #endregion

        #region method GroupMemberExists

        /// <summary>
        /// Gets if specified group member exists in specified user group members list.
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="userOrGroup">User or group.</param>
        /// <returns></returns>
        bool GroupMemberExists(string groupName,string userOrGroup);

        #endregion

        #region method GetGroupMembers

        /// <summary>
        /// Gets useer group members who belong to specified group.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <returns></returns>
        string[] GetGroupMembers(string groupName);

        #endregion

        #region method AddGroupMember

        /// <summary>
        /// Add specified user or group to specified goup members list.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="userOrGroup">User or group.</param>
        void AddGroupMember(string groupName,string userOrGroup);

        #endregion

        #region method DeleteGroupMember

        /// <summary>
        /// Deletes specified user or group from specified group members list.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="userOrGroup">User or group.</param>
        void DeleteGroupMember(string groupName,string userOrGroup);

        #endregion

        #region method GetGroupUsers

        /// <summary>
        /// Gets specified group users. All nested group members are replaced by actual users.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <returns></returns>
        string[] GetGroupUsers(string groupName);

        #endregion

        #endregion


        #region MailingList related

        #region method GetMailingLists

        /// <summary>
		/// Gets mailing lists.
		/// </summary>
		/// <param name="domainName">Domain name. Use <see cref="IMailServerApi.GetDomains">GetDomains()</see> to get valid values.</param>
		/// <returns></returns>
		DataView GetMailingLists(string domainName);
		
		#endregion
		

		#region method AddMailingList

		/// <summary>
		/// Adds new mailing list.
		/// </summary>
		/// <param name="mailingListID">Mailing list ID. Suggested value is Guid.NewGuid() .</param>
		/// <param name="mailingListName">Mailing list name name. Eg. all@lumisoft.ee .</param>
		/// <param name="description">Mailing list description.</param>
		/// <param name="domainName">Domain name. Use <see cref="IMailServerApi.GetDomains">GetDomains()</see> to get valid values.</param>
		/// <param name="enabled">Specifies if mailing list is enabled.</param>
		/// <remarks>Throws exception if specified mailing list already exists.</remarks>
		void AddMailingList(string mailingListID,string mailingListName,string description,string domainName,bool enabled);
		
		#endregion

		#region method DeleteMailingList

		/// <summary>
		/// Deletes specified mailing list.
		/// </summary>
		/// <param name="mailingListID"> Use <see cref="IMailServerApi.GetMailingLists">GetMailingLists()</see> to get valid values.</param>
		/// <returns></returns>
		void DeleteMailingList(string mailingListID);
		
		#endregion

		#region method UpdateMailingList

		/// <summary>
		/// Updates specified mailing list.
		/// </summary>
		/// <param name="mailingListID">Mailing list ID.</param>
		/// <param name="mailingListName">Mailing list name name. Use <see cref="IMailServerApi.GetMailingLists">GetMailingLists()</see> to get valid values.</param>
		/// <param name="description">Mailing list description.</param>
		/// <param name="domainName">Domain name. Use <see cref="IMailServerApi.GetDomains">>GetUsers()</see> to get valid values.</param>
		/// <param name="enabled">Specifies if mailing list is enabled.</param>
		void UpdateMailingList(string mailingListID,string mailingListName,string description,string domainName,bool enabled);
		
		#endregion

		#region method AddMailingListAddress

		/// <summary>
		/// Add new email address to specified mailing list.
		/// </summary>
		/// <param name="addressID">Address ID. Suggested value is Guid.NewGuid() .</param>
		/// <param name="mailingListName">Mailing list name name. Use <see cref="IMailServerApi.GetMailingLists">GetMailingLists()</see> to get valid values.</param>
		/// <param name="address">Mailing list member address.</param>
		/// <remarks>Throws exception if specified mailing list member already exists.</remarks>
		void AddMailingListAddress(string addressID,string mailingListName,string address);

		#endregion

		#region method DeleteMailingListAddress

		/// <summary>
		/// Deletes specified email address from mailing list. 
		/// </summary>
		/// <param name="addressID">Mailing list member address ID. Use <see cref="IMailServerApi.GetMailingListAddresses">GetMailingListMembers()</see> to get valid values.</param>
		void DeleteMailingListAddress(string addressID);

		#endregion

		#region method GetMailingListAddresses

		/// <summary>
		/// Gets mailing list members.
		/// </summary>
		/// <param name="mailingListName">Mailing list name name. Use <see cref="IMailServerApi.GetMailingLists">GetMailingLists()</see> to get valid values.</param>
		DataView GetMailingListAddresses(string mailingListName);

		#endregion


        #region method GetMailingListACL

        /// <summary>
        /// Gets mailing list ACL list.
        /// </summary>
        /// <param name="mailingListName">Mailing list name.</param>
        DataView GetMailingListACL(string mailingListName);

        #endregion

        #region method AddMailingListACL

        /// <summary>
        /// Adds specified user or group to mailing list ACL list (specified user can send messages to the specified mailing list).
        /// </summary>
        /// <param name="mailingListName">Mailing list name.</param>
        /// <param name="userOrGroup">User or group name.</param>
        void AddMailingListACL(string mailingListName,string userOrGroup);

        #endregion

        #region method DeleteMailingListACL

        /// <summary>
        /// Deletes specified user or group from mailing list ACL list.
        /// </summary>
        /// <param name="mailingListName">Mailing list name.</param>
        /// <param name="userOrGroup">User or group name.</param>
        void DeleteMailingListACL(string mailingListName,string userOrGroup);

        #endregion

        #region method CanAccessMailingList

        /// <summary>
        /// Checks if specified user can access specified mailing list.
        /// There is one built-in user anyone, that represent all users (including anonymous).
        /// </summary>
        /// <param name="mailingListName">Mailing list name.</param>
        /// <param name="user">User name.</param>
        /// <returns></returns>
        bool CanAccessMailingList(string mailingListName,string user);

        #endregion


        #region method MailingListExists

        /// <summary>
		/// Checks if user exists.
		/// </summary>
		/// <param name="mailingListName">Mailing list name.</param>
		/// <returns>Returns true if mailing list exists.</returns>
		bool MailingListExists(string mailingListName);
		
		#endregion

		#endregion


        #region Rules

        #region method GetGlobalMessageRules

        /// <summary>
        /// Gets global message rules.
        /// </summary>
        /// <returns></returns>
        DataView GetGlobalMessageRules();

        #endregion

        #region method AddGlobalMessageRule

        /// <summary>
        /// Adds new global message rule.
        /// </summary>
        /// <param name="ruleID">Rule ID. Guid.NewID().ToString() is suggested.</param>
        /// <param name="cost">Cost specifies in what order rules are processed. Costs with lower values are processed first.</param>
        /// <param name="enabled">Specifies if rule is enabled.</param>
        /// <param name="checkNextRule">Specifies when next rule is checked.</param>
        /// <param name="description">Rule description.</param>
        /// <param name="matchExpression">Rule match expression.</param>
        void AddGlobalMessageRule(string ruleID,long cost,bool enabled,GlobalMessageRule_CheckNextRule_enum checkNextRule,string description,string matchExpression);
        
        #endregion

        #region method DeleteGlobalMessageRule

        /// <summary>
        /// Deletes specified global message rule.
        /// </summary>
        /// <param name="ruleID">Rule ID of rule which to delete.</param>
        void DeleteGlobalMessageRule(string ruleID);
       
        #endregion

        #region method UpdateGlobalMessageRule

        /// <summary>
        /// Updates specified global message rule.
        /// </summary>
        /// <param name="ruleID">Rule ID.</param>
        /// <param name="cost">Cost specifies in what order rules are processed. Costs with lower values are processed first.</param>
        /// <param name="enabled">Specifies if rule is enabled.</param>
        /// <param name="checkNextRule">Specifies when next rule is checked.</param>
        /// <param name="description">Rule description.</param>
        /// <param name="matchExpression">Rule match expression.</param>
        void UpdateGlobalMessageRule(string ruleID,long cost,bool enabled,GlobalMessageRule_CheckNextRule_enum checkNextRule,string description,string matchExpression);
        
        #endregion

        #region method GetGlobalMessageRuleActions

        /// <summary>
        /// Gets specified global message rule actions.
        /// </summary>
        /// <param name="ruleID">Rule ID of rule which actions to get.</param>
        DataView GetGlobalMessageRuleActions(string ruleID);

        #endregion

        #region method AddGlobalMessageRuleAction

        /// <summary>
        /// Adds action to specified global message rule.
        /// </summary>
        /// <param name="ruleID">Rule ID to which to add this action.</param>
        /// <param name="actionID">Action ID. Guid.NewID().ToString() is suggested.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionType">Action type.</param>
        /// <param name="actionData">Action data. Data structure depends on action type.</param>
        void AddGlobalMessageRuleAction(string ruleID,string actionID,string description,GlobalMessageRuleAction_enum actionType,byte[] actionData);

        #endregion

        #region method DeleteGlobalMessageRuleAction

        /// <summary>
        /// Deletes specified action from specified global message rule.
        /// </summary>
        /// <param name="ruleID">Rule ID which action to delete.</param>
        /// <param name="actionID">Action ID of action which to delete.</param>
        void DeleteGlobalMessageRuleAction(string ruleID,string actionID);

        #endregion

        #region method UpdateGlobalMessageRuleAction

        /// <summary>
        /// Updates specified rule action.
        /// </summary>
        /// <param name="ruleID">Rule ID which action to update.</param>
        /// <param name="actionID">Action ID of action which to update.</param>
        /// <param name="description">Action description.</param>
        /// <param name="actionType">Action type.</param>
        /// <param name="actionData">Action data. Data structure depends on action type.</param>
        void UpdateGlobalMessageRuleAction(string ruleID,string actionID,string description,GlobalMessageRuleAction_enum actionType,byte[] actionData);

        #endregion

        #endregion


        #region Routing related

        #region method GetRoutes

        /// <summary>
		/// Gets email address routes.
		/// </summary>
		/// <returns></returns>
		DataView GetRoutes();
		
		#endregion


		#region method AddRoute

		/// <summary>
		/// Adds new route.
		/// </summary>
		/// <param name="routeID">Route ID.</param>
        /// <param name="cost">Cost specifies in what order roues are processed. Costs with lower values are processed first.</param>
		/// <param name="enabled">Specifies if route is enabled.</param>
		/// <param name="description">Route description text.</param>
		/// <param name="pattern">Match pattern. For example: *,*@domain.com,*sales@domain.com.</param>
		/// <param name="action">Specifies route action.</param>
		/// <param name="actionData">Route action data.</param>
		void AddRoute(string routeID,long cost,bool enabled,string description,string pattern,RouteAction_enum action,byte[] actionData);
		
		#endregion

		#region method DeleteRoute

		/// <summary>
		/// Deletes route.
		/// </summary>
		/// <param name="routeID">Route ID.</param>
		void DeleteRoute(string routeID);
		
		#endregion

		#region method UpdateRoute

		/// <summary>
		/// Updates route.
		/// </summary>
		/// <param name="routeID">Route ID.</param>
        /// <param name="cost">Cost specifies in what order roues are processed. Costs with lower values are processed first.</param>
		/// <param name="enabled">Specifies if route is enabled.</param>
		/// <param name="description">Route description text.</param>
		/// <param name="pattern">Match pattern. For example: *,*@domain.com,*sales@domain.com.</param>
		/// <param name="action">Specifies route action.</param>
		/// <param name="actionData">Route action data.</param>
		void UpdateRoute(string routeID,long cost,bool enabled,string description,string pattern,RouteAction_enum action,byte[] actionData);
		
		#endregion

		#endregion


		#region MailStore related

		#region method GetMessagesInfo

		/// <summary>
        /// Gets specified IMAP folder messages info. 
        /// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what messages info to get. For example: Inbox,Public Folders/Documnets .</param>
        /// <param name="messageInfos">List where to store folder messages info.</param>
        void GetMessagesInfo(string accessingUser,string folderOwnerUser,string folder,List<IMAP_MessageInfo> messageInfos);
				
		#endregion

        
		#region method StoreMessage

		/// <summary>
		/// Stores message to specified folder.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder where to store message. For example: Inbox,Public Folders/Documnets .</param>
		/// <param name="msgStream">Stream where message has stored. Stream position must be at the beginning of the message.</param>
		/// <param name="date">Recieve date.</param>
		/// <param name="flags">Message flags.</param>
		void StoreMessage(string accessingUser,string folderOwnerUser,string folder,Stream msgStream,DateTime date,string[] flags);
	
		#endregion

		#region method StoreMessageFlags

		/// <summary>
		/// Stores IMAP message flags (\seen,\draft, ...).
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder which message flags to store. For example: Inbox,Public Folders/Documnets .</param>
		/// <param name="messageInfo">IMAP message info.</param>
		/// <param name="flags">Message flags.</param>
		void StoreMessageFlags(string accessingUser,string folderOwnerUser,string folder,IMAP_MessageInfo messageInfo,string[] flags);
		
		#endregion

		#region method DeleteMessage

		/// <summary>
		/// Deletes message from mailbox.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what message to delete. For example: Inbox,Public Folders/Documnets .</param>
		/// <param name="messageID">Message ID.</param>
		/// <param name="uid">Message UID value.</param>
		void DeleteMessage(string accessingUser,string folderOwnerUser,string folder,string messageID,int uid);
				
		#endregion

        #region method GetMessageItems

        /// <summary>
        /// Gets specified message specified items.
        /// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what message to delete. For example: Inbox,Public Folders/Documnets .</param>
        /// <param name="e">MessageItems info.</param>
        void GetMessageItems(string accessingUser,string folderOwnerUser,string folder,EmailMessageItems e);

        #endregion

		#region method GetMessageTopLines

		/// <summary>
		/// Gets message header + number of specified lines.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what message top lines to get. For example: Inbox,Public Folders/Documnets .</param>
		/// <param name="msgID">MessageID.</param>
		/// <param name="nrLines">Number of lines to retrieve. NOTE: line counting starts at the end of header.</param>
		/// <returns>Returns message header + number of specified lines.</returns>
		byte[] GetMessageTopLines(string accessingUser,string folderOwnerUser,string folder,string msgID,int nrLines);
			
		#endregion

		#region method CopyMessage

		/// <summary>
		/// Creates copy of message to destination IMAP folder.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what contains message to copy. For example: Inbox,Public Folders/Documnets .</param>
		/// <param name="destFolderUser">Destination IMAP folder owner user name.</param>
		/// <param name="destFolder">Destination IMAP folder name.</param>
		/// <param name="messageInfo">IMAP message info.</param>
		void CopyMessage(string accessingUser,string folderOwnerUser,string folder,string destFolderUser,string destFolder,IMAP_MessageInfo messageInfo);
	
		#endregion

        #region method Search

        /// <summary>
        /// Searhes specified folder messages which match to search criteria.
        /// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what messages info to get. For example: Inbox,Public Folders/Documnets .</param>
        /// <param name="e">IMAP search event data.</param>
        void Search(string accessingUser,string folderOwnerUser,string folder,IMAP_e_Search e);

        #endregion


        #region method GetFolders

        /// <summary>
		/// Gets all available IMAP folders.
		/// </summary>
		/// <param name="userName">User name who's folders to get.</param>
        /// <param name="includeSharedFolders">If true, shared folders are included.</param>
		string[] GetFolders(string userName,bool includeSharedFolders);
				
		#endregion

		#region method GetSubscribedFolders

		/// <summary>
		/// Gets subscribed IMAP folders.
		/// </summary>
		/// <param name="userName"></param>
		string[] GetSubscribedFolders(string userName);
		
		#endregion

		#region method SubscribeFolder

		/// <summary>
		/// Subscribes new IMAP folder.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="folder"></param>
		void SubscribeFolder(string userName,string folder);
		
		#endregion

		#region method UnSubscribeFolder

		/// <summary>
		/// UnSubscribes IMAP folder.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="folder"></param>
		void UnSubscribeFolder(string userName,string folder);
		
		#endregion

		#region method CreateFolder

		/// <summary>
		/// Creates new IMAP folder.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what contains message to copy. For example: Inbox,Public Folders/Documnets .</param>
		void CreateFolder(string accessingUser,string folderOwnerUser,string folder);
				
		#endregion

		#region method DeleteFolder

		/// <summary>
		/// Deletes IMAP folder.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what to delete. For example: Inbox,Public Folders/Documnets .</param>
		void DeleteFolder(string accessingUser,string folderOwnerUser,string folder);
				
		#endregion

		#region method RenameFolder

		/// <summary>
		/// Renames IMAP folder.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what to delete. For example: Trash,Public Folders/Documnets .</param>
		/// <param name="newFolder">New folder name.</param>
		void RenameFolder(string accessingUser,string folderOwnerUser,string folder,string newFolder);
				
		#endregion

		#region method FolderExists

		/// <summary>
		/// Gets if specified folder exists.
		/// </summary>
		/// <param name="folderName">Folder name which to check. Eg. UserName/Inbox,UserName/Inbox/subfolder</param>
		/// <returns>Returns true if folder exists, otherwise false.</returns>
		bool FolderExists(string folderName);

		#endregion

        #region method FolderCreationTime

        /// <summary>
		/// Gets time when specified folder was created.
		/// </summary>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what creation time to get. For example: Inbox,Public Folders/Documnets .</param>
		DateTime FolderCreationTime(string folderOwnerUser,string folder);

		#endregion


        #region method GetSharedFolderRoots

        /// <summary>
        /// Gets shared folder root folders.
        /// </summary>
        /// <returns></returns>
        SharedFolderRoot[] GetSharedFolderRoots();

        #endregion

        #region method AddSharedFolderRoot

        /// <summary>
        /// Add shared folder root.
        /// </summary>
        /// <param name="rootID">Root folder ID. Guid.NewID().ToString() is suggested.</param>
        /// <param name="enabled">Specifies if root folder is enabled.</param>
        /// <param name="folder">Folder name which will be visible to public.</param>
        /// <param name="description">Description text.</param>
        /// <param name="rootType">Specifies what type root folder is.</param>
        /// <param name="boundedUser">User which to bound root folder.</param>
        /// <param name="boundedFolder">Folder which to bound to public folder.</param>
        void AddSharedFolderRoot(string rootID,bool enabled,string folder,string description,SharedFolderRootType_enum rootType,string boundedUser,string boundedFolder);

        #endregion

        #region method DeleteSharedFolderRoot

        /// <summary>
        /// Deletes shard folders root folder.
        /// </summary>
        /// <param name="rootID">Root folder ID which to delete.</param>
        void DeleteSharedFolderRoot(string rootID);

        #endregion

        #region method UpdateSharedFolderRoot

        /// <summary>
        /// Updates shared folder root.
        /// </summary>
        /// <param name="rootID">Root Folder IF which to update.</param>
        /// <param name="enabled">Specifies if root folder is enabled.</param>
        /// <param name="folder">Folder name which will be visible to public.</param>
        /// <param name="description">Description text.</param>
        /// <param name="rootType">Specifies what type root folder is.</param>
        /// <param name="boundedUser">User which to bound root folder.</param>
        /// <param name="boundedFolder">Folder which to bound to public folder.</param>
        void UpdateSharedFolderRoot(string rootID,bool enabled,string folder,string description,SharedFolderRootType_enum rootType,string boundedUser,string boundedFolder);

        #endregion


        #region method GetFolderACL

        /// <summary>
		/// Gets specified folder ACL.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what ACL to get. For example: Inbox,Public Folders/Documnets .</param>
		/// <returns></returns>
		DataView GetFolderACL(string accessingUser,string folderOwnerUser,string folder);
		
        #endregion

        #region method DeleteFolderACL

        /// <summary>
		/// Deletes specified folder ACL for specified user.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what ACL to delete. For example: Inbox,Public Folders/Documnets .</param>
        /// <param name="userOrGroup">User or user group which ACL on specified folder to delete.</param>
		void DeleteFolderACL(string accessingUser,string folderOwnerUser,string folder,string userOrGroup);
		
        #endregion

        #region method SetFolderACL

        /// <summary>
		/// Sets specified folder ACL for specified user.
		/// </summary>
        /// <param name="accessingUser">User who accesses this method. 
        /// User needs r permission to call this method or Exception is thrown. 
        /// There is special user 'system' for which permission check is skipped.</param>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
        /// <param name="folder">Folder what ACL to set. For example: Inbox,Public Folders/Documnets .</param>
        /// <param name="userOrGroup">>User or user which group ACL set to specified folder.</param>
		/// <param name="setType">Specifies how ACL flags must be stored (ADD,REMOVE,REPLACE).</param>
		/// <param name="aclFlags">ACL flags.</param>
		void SetFolderACL(string accessingUser,string folderOwnerUser,string folder,string userOrGroup,IMAP_Flags_SetType setType,IMAP_ACL_Flags aclFlags);
		
        #endregion

        #region method GetUserACL

        /// <summary>
		/// Gets what permissions specified user has to specified folder.
		/// </summary>
        /// <param name="folderOwnerUser">User who's folder it is.</param>
		/// <param name="folder">Folder which ACL to get. For example Inbox,Public Folders.</param>
		/// <param name="user">User name which ACL to get.</param>
		/// <returns></returns>
		IMAP_ACL_Flags GetUserACL(string folderOwnerUser,string folder,string user);

        #endregion


        #region method CreateUserDefaultFolders

        /// <summary>
        /// Creates specified user default folders, if they don't exist already.
        /// </summary>
        /// <param name="userName">User name to who's default folders to create.</param>
        void CreateUserDefaultFolders(string userName);
        
        #endregion

        #region method GetUsersDefaultFolders

        /// <summary>
        /// Gets users default folders.
        /// </summary>
        /// <returns></returns>
        DataView GetUsersDefaultFolders();

        #endregion

        #region method AddUsersDefaultFolder

        /// <summary>
        /// Adds users default folder.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        /// <param name="permanent">Spcifies if folder is permanent, user can't delete it.</param>
        void AddUsersDefaultFolder(string folderName,bool permanent);

        #endregion

        #region method DeleteUsersDefaultFolder

        /// <summary>
        /// Deletes specified users default folder.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        void DeleteUsersDefaultFolder(string folderName);

        #endregion


        #region method GetMailboxSize

        /// <summary>
		/// Gets specified user mailbox size.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <returns>Returns mailbox size.</returns>
		long GetMailboxSize(string userName);

		#endregion


        #region method GetRecycleBinSettings

        /// <summary>
        /// Gets recycle bin settings.
        /// </summary>
        /// <returns></returns>
        DataTable GetRecycleBinSettings();

        #endregion

        #region method UpdateRecycleBinSettings

        /// <summary>
        /// Updates recycle bin settings.
        /// </summary>
        /// <param name="deleteToRecycleBin">Specifies if deleted messages are store to recycle bin.</param>
        /// <param name="deleteMessagesAfter">Specifies how old messages will be deleted.</param>
        void UpdateRecycleBinSettings(bool deleteToRecycleBin,int deleteMessagesAfter);

        #endregion

        #region method GetRecycleBinMessagesInfo

        /// <summary>
        /// Gets recycle bin messages info. 
        /// </summary>
        /// <param name="user">User who's recyclebin messages to get or null if all users messages.</param>
        /// <param name="startDate">Messages from specified date. Pass DateTime.MinValue if not used.</param>
        /// <param name="endDate">Messages to specified date. Pass DateTime.MinValue if not used.</param>
        /// <returns></returns>
        DataView GetRecycleBinMessagesInfo(string user,DateTime startDate,DateTime endDate);
				
		#endregion

        #region method GetRecycleBinMessages

        /// <summary>
        /// Gets recycle bin message stream. NOTE: This method caller must take care of closing stream. 
        /// </summary>
        /// <param name="messageID">Message ID if of message what to get.</param>
        /// <returns></returns>
        Stream GetRecycleBinMessage(string messageID);
				
		#endregion

        #region method DeleteRecycleBinMessage

        /// <summary>
        /// Deletes specified recycle bin message.
        /// </summary>
        /// <param name="messageID">Message ID.</param>
        void DeleteRecycleBinMessage(string messageID);

        #endregion

        #region method RestoreRecycleBinMessage

        /// <summary>
        /// Restores specified recycle bin message.
        /// </summary>
        /// <param name="messageID">Message ID.</param>
        void RestoreRecycleBinMessage(string messageID);

        #endregion

        #endregion


        #region Security related

        #region method GetSecurityList

        /// <summary>
		/// Gets security entries list.
		/// </summary>
		DataView GetSecurityList();
		
		#endregion


		#region method AddSecurityEntry

		/// <summary>
		/// Adds new IP security entry.
		/// </summary>
		/// <param name="id">IP security entry ID.</param>
		/// <param name="enabled">Specifies if IP security entry is enabled.</param>
		/// <param name="description">IP security entry description text.</param>
		/// <param name="service">Specifies service for what security entry applies.</param>
		/// <param name="action">Specifies what action done if IP matches to security entry range.</param>
		/// <param name="startIP">Range start IP.</param>
		/// <param name="endIP">Range end IP.</param>
		void AddSecurityEntry(string id,bool enabled,string description,Service_enum service,IPSecurityAction_enum action,IPAddress startIP,IPAddress endIP);
		
		#endregion

		#region method DeleteSecurityEntry

		/// <summary>
		/// Deletes security entry.
		/// </summary>
		/// <param name="id">IP security entry ID.</param>
		void DeleteSecurityEntry(string id);
		
		#endregion

		#region method UpdateSecurityEntry

		/// <summary>
		/// Updates IP security entry.
		/// </summary>
		/// <param name="id">IP security entry ID.</param>
		/// <param name="enabled">Specifies if IP security entry is enabled.</param>
		/// <param name="description">IP security entry description text.</param>
		/// <param name="service">Specifies service for what security entry applies.</param>
		/// <param name="action">Specifies what action done if IP matches to security entry range.</param>
		/// <param name="startIP">Range start IP.</param>
		/// <param name="endIP">Range end IP.</param>
		void UpdateSecurityEntry(string id,bool enabled,string description,Service_enum service,IPSecurityAction_enum action,IPAddress startIP,IPAddress endIP);
		
		#endregion

		#endregion


		#region Filters related

		#region method GetFilters

		/// <summary>
		/// Gets filter list.
		/// </summary>
		/// <returns></returns>
		DataView GetFilters();
		
		#endregion


		#region method AddFilter

		/// <summary>
		/// Adds new filter.
		/// </summary>
		/// <param name="filterID">Filter ID. Suggested value is Guid.NewGuid() .</param>
		/// <param name="description">Filter description</param>
		/// <param name="type">Filter type. Eg. ISmtpMessageFilter.</param>
		/// <param name="assembly">Assembly with full location. Eg. C:\MailServer\Filters\filter.dll .</param>
		/// <param name="className">Filter full class name, wih namespace. Eg. LumiSoft.MailServer.Fileters.Filter1 .</param>
		/// <param name="cost">Filters are sorted by cost and proccessed with cost value. Smallest cost is proccessed first.</param>
		/// <param name="enabled">Specifies if filter is enabled.</param>
		/// <remarks>Throws exception if specified filter entry already exists.</remarks>
		void AddFilter(string filterID,string description,string type,string assembly,string className,long cost,bool enabled);
		
		#endregion

		#region method DeleteFilter

		/// <summary>
		/// Deletes specified filter.
		/// </summary>
		/// <param name="filterID">FilterID of the filter which to delete.</param>
		void DeleteFilter(string filterID);
		
		#endregion

		#region method UpdateFilter

		/// <summary>
		/// Updates specified filter.
		/// </summary>		 
		/// <param name="filterID">FilterID which to update.</param>
		/// <param name="description">Filter description</param>
		/// <param name="type">Filter type. Eg. ISmtpMessageFilter.</param>
		/// <param name="assembly">Assembly with full location. Eg. C:\MailServer\Filters\filter.dll .</param>
		/// <param name="className">Filter full class name, wih namespace. Eg. LumiSoft.MailServer.Fileters.Filter1 .</param>
		/// <param name="cost">Filters are sorted by cost and proccessed with cost value. Smallest cost is proccessed first.</param>
		/// <param name="enabled">Specifies if filter is enabled.</param>
		/// <returns></returns>
		void UpdateFilter(string filterID,string description,string type,string assembly,string className,long cost,bool enabled);
		
		#endregion

		#endregion


		#region Settings related

		/// <summary>
		/// Gets server settings.
		/// </summary>
		/// <returns></returns>
		DataRow GetSettings();

		/// <summary>
		/// Updates server settings.
		/// </summary>
		/// <returns></returns>
		void UpdateSettings(DataRow settings);

		#endregion
	
	}
}
