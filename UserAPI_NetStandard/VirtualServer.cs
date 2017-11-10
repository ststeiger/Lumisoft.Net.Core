using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The VirtualServer object represents virtual server in LumiSoft Mail Server.
    /// </summary>
    public class VirtualServer
    {
        private Server                       m_pServer             = null;
        private VirtualServerCollection      m_pOwner              = null;
        private string                       m_VirtualServerID     = "";
        private bool                         m_Enabled             = false;
        private string                       m_Name                = "";
        private string                       m_Assembly            = "";
        private string                       m_Type                = "";
        private string                       m_InitString          = "";
        private System_Settings              m_pSystemSettings     = null;
        private DomainCollection             m_pDomains            = null;
        private UserCollection               m_pUsers              = null;
        private GroupCollection              m_pGroups             = null;
        private MailingListCollection        m_pMailingLists       = null;
        private GlobalMessageRuleCollection  m_pGlobalMsgRules     = null;
        private RouteCollection              m_pRoutes             = null;
        private SharedRootFolderCollection   m_pRootFolders        = null;
        private UsersDefaultFolderCollection m_UsersDefaultFolders = null;
        private FilterCollection             m_pFilters            = null;
        private IPSecurityCollection         m_pIpSecurity         = null;
        private Queues                       m_pQueues             = null;
        private RecycleBin                   m_pRecycleBin         = null;
        private Logs                         m_pLogs               = null;
        private SipRegistrationCollection    m_pSipRegistrations   = null;
        private SIP_CallCollection           m_pSipCalls           = null;
        private bool                         m_ValuesChanged       = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Owner server.</param>
        /// <param name="owner">Owner VirtualServerCollection collection that owns this object.</param>
        /// <param name="id">Virtual server ID.</param>
        /// <param name="enabled">Specifies if virtual server is enabled.</param>
        /// <param name="name">Virtual server name.</param>
        /// <param name="assembly">API assembly name.</param>
        /// <param name="type">API Type name.</param>
        /// <param name="initString">API init string.</param>
        internal VirtualServer(Server server,VirtualServerCollection owner,string id,bool enabled,string name,string assembly,string type,string initString)
        {
            m_pServer         = server;
            m_pOwner          = owner;
            m_VirtualServerID = id;
            m_Enabled         = enabled;
            m_Name            = name;
            m_Assembly        = assembly;
            m_Type            = type;
            m_InitString      = initString;
        }


        #region method Backup

        /// <summary>
        /// Backsup all virtual server settings to the specified file.
        /// </summary>
        /// <param name="fileName">File name where to store backup.</param>
        public void Backup(string fileName)
        {
            using(FileStream fs = File.Create(fileName)){
                Backup(fs);
            }
        }

        /// <summary>
        /// Backsup all virtual server settings to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store backup.</param>
        public void Backup(Stream stream)
        {
            DataSet ds = new DataSet("dsVirtualServerBackup");

            #region Settings

            ds.Merge(this.SystemSettings.ToDataSet());

            #endregion


            #region Domains

            ds.Tables.Add("Domains");
            ds.Tables["Domains"].Columns.Add("DomainID");
            ds.Tables["Domains"].Columns.Add("DomainName");
            ds.Tables["Domains"].Columns.Add("Description");
            foreach(Domain domain in this.Domains){
                DataRow dr = ds.Tables["Domains"].NewRow();
                dr["DomainID"]    = domain.DomainID;
                dr["DomainName"]  = domain.DomainName;
                dr["Description"] = domain.Description;
                ds.Tables["Domains"].Rows.Add(dr);
            }


            #endregion


            #region Users

            ds.Tables.Add("Users");
            ds.Tables["Users"].Columns.Add("UserID");
            ds.Tables["Users"].Columns.Add("FullName");
            ds.Tables["Users"].Columns.Add("UserName");
            ds.Tables["Users"].Columns.Add("Password");
            ds.Tables["Users"].Columns.Add("Description");
            ds.Tables["Users"].Columns.Add("Mailbox_Size");
            ds.Tables["Users"].Columns.Add("Enabled");
            ds.Tables["Users"].Columns.Add("Permissions");
            foreach(User user in this.Users){
                DataRow dr = ds.Tables["Users"].NewRow();
                dr["UserID"]       = user.UserID;
                dr["FullName"]     = user.FullName;
                dr["UserName"]     = user.UserName;
                dr["Password"]     = user.Password;
                dr["Description"]  = user.Description;
                dr["Mailbox_Size"] = user.MaximumMailboxSize;
                dr["Enabled"]      = user.Enabled;
                dr["Permissions"]  = (int)user.Permissions;
                ds.Tables["Users"].Rows.Add(dr);
            }

            #endregion

            #region User email addresses

            ds.Tables.Add("User_EmailAddresses");
            ds.Tables["User_EmailAddresses"].Columns.Add("UserID");
            ds.Tables["User_EmailAddresses"].Columns.Add("EmailAddress");
            foreach(User user in this.Users){
                foreach(string emailAddress in user.EmailAddresses){
                    DataRow dr = ds.Tables["User_EmailAddresses"].NewRow();
                    dr["UserID"]         = user.UserID;
                    dr["EmailAddress"]   = emailAddress;
                    ds.Tables["User_EmailAddresses"].Rows.Add(dr);
                }
            }

            #endregion

            #region User remote servers

            ds.Tables.Add("User_RemoteServers");
            ds.Tables["User_RemoteServers"].Columns.Add("UserID");
            ds.Tables["User_RemoteServers"].Columns.Add("ServerID");
            ds.Tables["User_RemoteServers"].Columns.Add("Description");
            ds.Tables["User_RemoteServers"].Columns.Add("RemoteServer");
            ds.Tables["User_RemoteServers"].Columns.Add("RemotePort");
            ds.Tables["User_RemoteServers"].Columns.Add("RemoteUserName");
            ds.Tables["User_RemoteServers"].Columns.Add("RemotePassword");
            ds.Tables["User_RemoteServers"].Columns.Add("UseSSL");
            ds.Tables["User_RemoteServers"].Columns.Add("Enabled");
            foreach(User user in this.Users){
                foreach(UserRemoteServer userRemServer in user.RemoteServers){
                    DataRow dr = ds.Tables["User_RemoteServers"].NewRow();
                    dr["UserID"]         = user.UserID;
                    dr["ServerID"]       = userRemServer.ID;
                    dr["Description"]    = userRemServer.Description;
                    dr["RemoteServer"]   = userRemServer.Host;
                    dr["RemotePort"]     = userRemServer.Port;
                    dr["RemoteUserName"] = userRemServer.UserName;
                    dr["RemotePassword"] = userRemServer.Password;
                    dr["UseSSL"]         = userRemServer.SSL;
                    dr["Enabled"]        = userRemServer.Enabled;
                    ds.Tables["User_RemoteServers"].Rows.Add(dr);
                }
            }

            #endregion

            #region User message rules

            ds.Tables.Add("User_MessageRules");
            ds.Tables["User_MessageRules"].Columns.Add("UserID");
            ds.Tables["User_MessageRules"].Columns.Add("RuleID");
            ds.Tables["User_MessageRules"].Columns.Add("Enabled");
            ds.Tables["User_MessageRules"].Columns.Add("CheckNextRuleIf");
            ds.Tables["User_MessageRules"].Columns.Add("Description");
            ds.Tables["User_MessageRules"].Columns.Add("MatchExpression");
            foreach(User user in this.Users){
                foreach(UserMessageRule rule in user.MessageRules){
                    DataRow dr = ds.Tables["User_MessageRules"].NewRow();
                    dr["UserID"]          = user.UserID;
                    dr["RuleID"]          = rule.ID;
                    dr["Enabled"]         = rule.Enabled;
                    dr["CheckNextRuleIf"] = (int)rule.CheckNextRule;
                    dr["Description"]     = rule.Description;
                    dr["MatchExpression"] = rule.MatchExpression;
                    ds.Tables["User_MessageRules"].Rows.Add(dr);
                }
            }

            #endregion

            #region User message rule actions

            ds.Tables.Add("User_MessageRuleActions");
            ds.Tables["User_MessageRuleActions"].Columns.Add("UserID");
            ds.Tables["User_MessageRuleActions"].Columns.Add("RuleID");
            ds.Tables["User_MessageRuleActions"].Columns.Add("ActionID");
            ds.Tables["User_MessageRuleActions"].Columns.Add("Description");
            ds.Tables["User_MessageRuleActions"].Columns.Add("ActionType");
            ds.Tables["User_MessageRuleActions"].Columns.Add("ActionData",typeof(byte[]));
            foreach(User user in this.Users){
                foreach(UserMessageRule rule in user.MessageRules){
                    foreach(UserMessageRuleActionBase ruleAction in rule.Actions){
                        DataRow dr = ds.Tables["User_MessageRuleActions"].NewRow();
                        dr["UserID"]      = user.UserID;
                        dr["RuleID"]      = rule.ID;
                        dr["ActionID"]    = ruleAction.ID;
                        dr["Description"] = ruleAction.Description;
                        dr["ActionType"]  = (int)ruleAction.ActionType;
                        dr["ActionData"]  = ruleAction.Serialize();
                        ds.Tables["User_MessageRuleActions"].Rows.Add(dr);
                    }
                }
            }

            #endregion

            #region Groups

            ds.Tables.Add("Groups");
            ds.Tables["Groups"].Columns.Add("GroupID");
            ds.Tables["Groups"].Columns.Add("GroupName");
            ds.Tables["Groups"].Columns.Add("Description");
            ds.Tables["Groups"].Columns.Add("Enabled");
            foreach(Group group in this.Groups){
                DataRow dr = ds.Tables["Groups"].NewRow();
                dr["GroupID"]     = group.GroupID;
                dr["GroupName"]   = group.GroupName;
                dr["Description"] = group.Description;
                dr["Enabled"]     = group.Enabled;
                ds.Tables["Groups"].Rows.Add(dr);
            }

            #endregion

            #region Group members

            ds.Tables.Add("Group_Members");
            ds.Tables["Group_Members"].Columns.Add("GroupID");
            ds.Tables["Group_Members"].Columns.Add("UserOrGroup");
            foreach(Group group in this.Groups){
                foreach(string member in group.Members){
                    DataRow dr = ds.Tables["Group_Members"].NewRow();
                    dr["GroupID"]     = group.GroupID;
                    dr["UserOrGroup"] = member;
                    ds.Tables["Group_Members"].Rows.Add(dr);
                }
            }

            #endregion


            #region Mailing lists

            ds.Tables.Add("MailingLists");
            ds.Tables["MailingLists"].Columns.Add("MailingListID");
            ds.Tables["MailingLists"].Columns.Add("MailingListName");
            ds.Tables["MailingLists"].Columns.Add("Description");
            ds.Tables["MailingLists"].Columns.Add("Enabled");
            foreach(MailingList list in this.MailingLists){
                DataRow dr = ds.Tables["MailingLists"].NewRow();
                dr["MailingListID"]   = list.ID;
                dr["MailingListName"] = list.Name;
                dr["Description"]     = list.Description;
                dr["Enabled"]         = list.Enabled;
                ds.Tables["MailingLists"].Rows.Add(dr);
            }

            #endregion

            #region Mailing list members

            ds.Tables.Add("MailingList_Members");
            ds.Tables["MailingList_Members"].Columns.Add("MailingListID");
            ds.Tables["MailingList_Members"].Columns.Add("Address");
            foreach(MailingList list in this.MailingLists){
                foreach(string member in list.Members){
                    DataRow dr = ds.Tables["MailingList_Members"].NewRow();
                    dr["MailingListID"] = list.ID;
                    dr["Address"]       = member;
                    ds.Tables["MailingList_Members"].Rows.Add(dr);
                }
            }

            #endregion

            #region Mailing list ACL

            ds.Tables.Add("MailingList_ACL");
            ds.Tables["MailingList_ACL"].Columns.Add("MailingListID");
            ds.Tables["MailingList_ACL"].Columns.Add("UserOrGroup");
            foreach(MailingList list in this.MailingLists){
                foreach(string acl in list.ACL){
                    DataRow dr = ds.Tables["MailingList_ACL"].NewRow();
                    dr["MailingListID"] = list.ID;
                    dr["UserOrGroup"]   = acl;
                    ds.Tables["MailingList_ACL"].Rows.Add(dr);
                }
            }

            #endregion


            #region Routes

            ds.Tables.Add("Routing");
            ds.Tables["Routing"].Columns.Add("RouteID");
            ds.Tables["Routing"].Columns.Add("Enabled");
            ds.Tables["Routing"].Columns.Add("Description");
            ds.Tables["Routing"].Columns.Add("Pattern");
            ds.Tables["Routing"].Columns.Add("Action");
            ds.Tables["Routing"].Columns.Add("ActionData",typeof(byte[]));
            foreach(Route route in this.Routes){
                DataRow dr = ds.Tables["Routing"].NewRow();
                dr["RouteID"]     = route.ID;
                dr["Enabled"]     = route.Enabled;
                dr["Description"] = route.Description;
                dr["Pattern"]     = route.Pattern;
                dr["Action"]      = (int)route.Action.ActionType;
                dr["ActionData"]  = route.Action.Serialize();
                ds.Tables["Routing"].Rows.Add(dr);
            }

            #endregion


            #region Global message rules

            ds.Tables.Add("GlobalMessageRules");
            ds.Tables["GlobalMessageRules"].Columns.Add("RuleID");
            ds.Tables["GlobalMessageRules"].Columns.Add("Enabled");
            ds.Tables["GlobalMessageRules"].Columns.Add("CheckNextRuleIf");
            ds.Tables["GlobalMessageRules"].Columns.Add("Description");
            ds.Tables["GlobalMessageRules"].Columns.Add("MatchExpression");
            foreach(GlobalMessageRule rule in this.GlobalMessageRules){
                DataRow dr = ds.Tables["GlobalMessageRules"].NewRow();
                dr["RuleID"]          = rule.ID;
                dr["Enabled"]         = rule.Enabled;
                dr["CheckNextRuleIf"] = (int)rule.CheckNextRule;
                dr["Description"]     = rule.Description;
                dr["MatchExpression"] = rule.MatchExpression;
                ds.Tables["GlobalMessageRules"].Rows.Add(dr);
            }

            #endregion

            #region Global message rule actions

            ds.Tables.Add("GlobalMessageRuleActions");
            ds.Tables["GlobalMessageRuleActions"].Columns.Add("RuleID");
            ds.Tables["GlobalMessageRuleActions"].Columns.Add("ActionID");
            ds.Tables["GlobalMessageRuleActions"].Columns.Add("Description");
            ds.Tables["GlobalMessageRuleActions"].Columns.Add("ActionType");
            ds.Tables["GlobalMessageRuleActions"].Columns.Add("ActionData",typeof(byte[]));
            foreach(GlobalMessageRule rule in this.GlobalMessageRules){
                foreach(GlobalMessageRuleActionBase ruleAction in rule.Actions){
                    DataRow dr = ds.Tables["GlobalMessageRuleActions"].NewRow();
                    dr["RuleID"]      = rule.ID;
                    dr["ActionID"]    = ruleAction.ID;
                    dr["Description"] = ruleAction.Description;
                    dr["ActionType"]  = (int)ruleAction.ActionType;
                    dr["ActionData"]  = ruleAction.Serialize();
                    ds.Tables["GlobalMessageRuleActions"].Rows.Add(dr);
                }                
            }

            #endregion


            #region IP Security

            ds.Tables.Add("IP_Security");
            ds.Tables["IP_Security"].Columns.Add("ID");
            ds.Tables["IP_Security"].Columns.Add("Enabled");
            ds.Tables["IP_Security"].Columns.Add("Description");
            ds.Tables["IP_Security"].Columns.Add("Service");
            ds.Tables["IP_Security"].Columns.Add("Action");
            ds.Tables["IP_Security"].Columns.Add("StartIP");
            ds.Tables["IP_Security"].Columns.Add("EndIP");
            foreach(IPSecurity entry in this.IpSecurity){
                DataRow dr = ds.Tables["IP_Security"].NewRow();
                dr["ID"]          = entry.ID;
                dr["Enabled"]     = entry.Enabled;
                dr["Description"] = entry.Description;
                dr["Service"]     = (int)entry.Service;
                dr["Action"]      = (int)entry.Action;
                dr["StartIP"]     = entry.StartIP;
                dr["EndIP"]       = entry.EndIP;
                ds.Tables["IP_Security"].Rows.Add(dr);
            }

            #endregion


            #region Filters

            ds.Tables.Add("Filters");
            ds.Tables["Filters"].Columns.Add("FilterID");
            ds.Tables["Filters"].Columns.Add("Assembly");
            ds.Tables["Filters"].Columns.Add("ClassName");
            ds.Tables["Filters"].Columns.Add("Enabled");
            ds.Tables["Filters"].Columns.Add("Description");
            foreach(Filter filter in this.Filters){
                DataRow dr = ds.Tables["Filters"].NewRow();
                dr["FilterID"]    = filter.ID;
                dr["Assembly"]    = filter.AssemblyName;
                dr["ClassName"]   = filter.Class;
                dr["Enabled"]     = filter.Enabled;
                dr["Description"] = filter.Description;
                ds.Tables["Filters"].Rows.Add(dr);
            }

            #endregion


            #region Users Default Folders

            ds.Tables.Add("UsersDefaultFolders");
            ds.Tables["UsersDefaultFolders"].Columns.Add("FolderName");
            ds.Tables["UsersDefaultFolders"].Columns.Add("Permanent");
            foreach(UsersDefaultFolder folder in this.UsersDefaultFolders){
                DataRow dr = ds.Tables["UsersDefaultFolders"].NewRow();
                dr["FolderName"] = folder.FolderName;
                dr["Permanent"]  = folder.Permanent;
                ds.Tables["UsersDefaultFolders"].Rows.Add(dr);
            }

            #endregion

            ds.WriteXml(stream);
        }

        #endregion

        #region method Restore
                
        /// <summary>
        /// Restores all virtual server settings from the specified file.
        /// </summary>
        /// <param name="fileName">File what conatins backup.</param>
        /// <param name="restoreFlags">Specifies restore options.</param>
        public void Restore(string fileName,RestoreFlags_enum restoreFlags)
        {
            using(FileStream fs = File.OpenRead(fileName)){
                Restore(fs,restoreFlags);
            }
        }

        /// <summary>
        /// Restores all virtual server settings from the specified stream.
        /// </summary>
        /// <param name="stream">Stream what conatins backup.</param>
        /// <param name="restoreFlags">Specifies restore options.</param>
        public void Restore(Stream stream,RestoreFlags_enum restoreFlags)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(stream);

            #region Settings

            /* NOTE: Settings need special handling, we can't comapre settings, so just always overwrite it. 
            */

            if(ds.Tables.Contains("Settings")){
                this.SystemSettings.LoadSettings(ds);
                this.SystemSettings.Commit();
            }

            #endregion


            #region Domains

            /* NOTE: Domains must act differenly compared other object, because deleting domain looses mailboxes.
                     We need to update domain, if ResoreFlags_enum.Replace, we can't delete domain !!! 
            */

            if(ds.Tables.Contains("Domains")){
                foreach(DataRow dr in ds.Tables["Domains"].Rows){
                    // Update exisiting domain, if ResoreFlags_enum.Replace
                    if(this.Domains.Contains(dr["DomainName"].ToString())){
                        if((restoreFlags & RestoreFlags_enum.Replace) != 0){
                            // TODO:
                        }
                    }
                    // Add domain list, if ResoreFlags_enum.Add
                    else if((restoreFlags & RestoreFlags_enum.Add) != 0){
                        Domain domain = this.Domains.Add(
                            dr["DomainName"].ToString(),
                            dr["Description"].ToString()
                        );
                    }
                }
            }

            #endregion


            #region Users

            /* NOTE: Users must act differenly compared other object, because deleting user looses mailbox.
                     We need to update user, if ResoreFlags_enum.Replace, we can't delete user !!! 
            */

            if(ds.Tables.Contains("Users")){
                foreach(DataRow dr in ds.Tables["Users"].Rows){
                    User user        = null;
                    bool userChanged = false;
                    // Update exisiting domain, if ResoreFlags_enum.Replace
                    if(this.Users.Contains(dr["UserName"].ToString())){
                        if((restoreFlags & RestoreFlags_enum.Replace) != 0){
                            user = this.Users.GetUserByName(dr["UserName"].ToString());
                            user.UserName           = dr["UserName"].ToString();
                            user.FullName           = dr["FullName"].ToString();
                            user.Password           = dr["Password"].ToString();
                            user.Description        = dr["Description"].ToString();
                            user.MaximumMailboxSize = ConvertEx.ToInt32(dr["Mailbox_Size"]);
                            user.Enabled            = ConvertEx.ToBoolean(dr["Enabled"]);
                            user.Permissions        = (UserPermissions_enum)ConvertEx.ToInt32(dr["Permissions"]);
                            user.Commit();

                            // Delete user email addresses
                            foreach(string emailAddress in user.EmailAddresses.ToArray()){
                                user.EmailAddresses.Remove(emailAddress);
                            }

                            // Delete user remote servers
                            foreach(UserRemoteServer server in user.RemoteServers.ToArray()){
                                user.RemoteServers.Remove(server);
                            }

                            // Delete user message rules
                            foreach(UserMessageRule rule in user.MessageRules.ToArray()){
                                user.MessageRules.Remove(rule);
                            }

                            userChanged = true;
                        }
                    }
                    // Add domain list, if ResoreFlags_enum.Add
                    else if((restoreFlags & RestoreFlags_enum.Add) != 0){
                        user = this.Users.Add(
                            dr["UserName"].ToString(),
                            dr["FullName"].ToString(),
                            dr["Password"].ToString(),
                            dr["Description"].ToString(),
                            ConvertEx.ToInt32(dr["Mailbox_Size"]),
                            ConvertEx.ToBoolean(dr["Enabled"]),
                            (UserPermissions_enum)ConvertEx.ToInt32(dr["Permissions"])
                        );
                        userChanged = true;
                    }

                    if(userChanged){
                        // Add user remote servers
                        if(ds.Tables.Contains("User_EmailAddresses")){
                            DataView dv = new DataView(ds.Tables["User_EmailAddresses"]);
                            dv.RowFilter = "UserID='" + dr["UserID"].ToString() + "'";
                            foreach(DataRowView drV in dv){
                                user.EmailAddresses.Add(
                                    drV["EmailAddress"].ToString()
                                );                    
                            }
                        }

                        // Add user remote servers
                        if(ds.Tables.Contains("User_RemoteServers")){
                            DataView dv = new DataView(ds.Tables["User_RemoteServers"]);
                            dv.RowFilter = "UserID='" + dr["UserID"].ToString() + "'";
                            foreach(DataRowView drV in dv){
                                user.RemoteServers.Add(
                                    drV["Description"].ToString(),
                                    drV["RemoteServer"].ToString(),
                                    ConvertEx.ToInt32(drV["RemotePort"]),
                                    ConvertEx.ToBoolean(drV["UseSSL"]),
                                    drV["RemoteUserName"].ToString(),
                                    drV["RemotePassword"].ToString(),
                                    ConvertEx.ToBoolean(drV["Enabled"])
                                );                    
                            }
                        }

                        // Add user message rules
                        if(ds.Tables.Contains("User_MessageRules")){
                            DataView dv = new DataView(ds.Tables["User_MessageRules"]);
                            dv.RowFilter = "UserID='" + dr["UserID"].ToString() + "'";
                            foreach(DataRowView drV in dv){
                                UserMessageRule rule = user.MessageRules.Add(
                                    ConvertEx.ToBoolean(drV["Enabled"]),
                                    drV["Description"].ToString(),
                                    drV["MatchExpression"].ToString(),
                                    (GlobalMessageRule_CheckNextRule_enum)ConvertEx.ToInt32(drV["CheckNextRuleIf"])                   
                                );
                   
                                // Add rule actions
                                if(ds.Tables.Contains("User_MessageRuleActions")){
                                    DataView dvRuleActions = new DataView(ds.Tables["User_MessageRuleActions"]);
                                    dvRuleActions.RowFilter = "UserID='" + dr["UserID"].ToString() + "' AND RuleID='" + drV["RuleID"].ToString() + "'";
                                    foreach(DataRowView drvAction in dvRuleActions){                                        
                                        rule.Actions.Add(
                                            drvAction["ActionID"].ToString(),
                                            drvAction["Description"].ToString(),
                                            (UserMessageRuleAction_enum)Convert.ToInt32(drvAction["ActionType"]),
                                            Convert.FromBase64String(drvAction["ActionData"].ToString()),
                                            true
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Groups

            if(ds.Tables.Contains("Groups")){
                foreach(DataRow dr in ds.Tables["Groups"].Rows){
                    // Delete exisiting group, if ResoreFlags_enum.Replace
                    bool replaceAdd = false;
                    if(this.Groups.Contains(dr["GroupName"].ToString()) && (restoreFlags & RestoreFlags_enum.Replace) != 0){
                        this.Groups.Remove(this.Groups.GetGroupByName(dr["GroupName"].ToString()));
                        replaceAdd = true;
                    }

                    // Add group, if ResoreFlags_enum.Add or Replace group add
                    if(!this.Groups.Contains(dr["GroupName"].ToString()) && (replaceAdd || (restoreFlags & RestoreFlags_enum.Add) != 0)){
                        Group group = this.Groups.Add(
                            dr["GroupName"].ToString(),
                            dr["Description"].ToString(),
                            ConvertEx.ToBoolean(dr["Enabled"])
                        );

                        // Add group members
                        if(ds.Tables.Contains("Group_Members")){
                            DataView dv = new DataView(ds.Tables["Group_Members"]);
                            dv.RowFilter = "GroupID='" + dr["GroupID"].ToString() + "'";
                            foreach(DataRowView drV in dv){
                                group.Members.Add(drV["UserOrGroup"].ToString());                     
                            }
                        }
                    }
                }
            }

            #endregion


            #region Mailing lists

            if(ds.Tables.Contains("MailingLists")){
                foreach(DataRow dr in ds.Tables["MailingLists"].Rows){
                    // Delete exisiting mailing list, if ResoreFlags_enum.Replace
                    bool replaceAdd = false;
                    if(this.MailingLists.Contains(dr["MailingListName"].ToString()) && (restoreFlags & RestoreFlags_enum.Replace) != 0){
                        this.MailingLists.Remove(this.MailingLists.GetMailingListByName(dr["MailingListName"].ToString()));
                        replaceAdd = true;
                    }

                    // Add mailing list, if ResoreFlags_enum.Add or Replace mailing list add
                    if(!this.MailingLists.Contains(dr["MailingListName"].ToString()) && (replaceAdd || (restoreFlags & RestoreFlags_enum.Add) != 0)){
                        MailingList list = this.MailingLists.Add(
                            dr["MailingListName"].ToString(),
                            dr["Description"].ToString(),
                            ConvertEx.ToBoolean(dr["Enabled"])
                        );
                        
                        // Add mailing list members
                        if(ds.Tables.Contains("MailingList_Members")){
                            DataView dv = new DataView(ds.Tables["MailingList_Members"]);
                            dv.RowFilter = "MailingListID='" + dr["MailingListID"].ToString() + "'";
                            foreach(DataRowView drV in dv){
                                list.Members.Add(drV["Address"].ToString());                     
                            }
                        }

                        // Add mailing list ACL
                        if(ds.Tables.Contains("MailingList_ACL")){
                            DataView dv = new DataView(ds.Tables["MailingList_ACL"]);
                            dv.RowFilter = "MailingListID='" + dr["MailingListID"].ToString() + "'";
                            foreach(DataRowView drV in dv){
                                list.ACL.Add(drV["UserOrGroup"].ToString());                     
                            }
                        }
                    }
                }
            }

            #endregion


            #region Routes

            if(ds.Tables.Contains("Routing")){
                foreach(DataRow dr in ds.Tables["Routing"].Rows){
                    // Delete exisiting route, if ResoreFlags_enum.Replace
                    bool replaceAdd = false;
                    if(this.Routes.ContainsPattern(dr["Pattern"].ToString()) && (restoreFlags & RestoreFlags_enum.Replace) != 0){
                        this.Routes.Remove(this.Routes.GetRouteByPattern(dr["Pattern"].ToString()));
                        replaceAdd = true;
                    }
                    
                    // Add route, if ResoreFlags_enum.Add or Replace route add
                    if(!this.Routes.ContainsPattern(dr["Pattern"].ToString()) && (replaceAdd || (restoreFlags & RestoreFlags_enum.Add) != 0)){
                        RouteAction_enum actionType = (RouteAction_enum)Convert.ToInt32(dr["Action"]);
                        RouteActionBase action = null;
                        if(actionType == RouteAction_enum.RouteToEmail){
                            action = new RouteAction_RouteToEmail(Convert.FromBase64String(dr["ActionData"].ToString()));
                        }
                        else if(actionType == RouteAction_enum.RouteToHost){
                            action = new RouteAction_RouteToHost(Convert.FromBase64String(dr["ActionData"].ToString()));
                        }
                        else if(actionType == RouteAction_enum.RouteToMailbox){
                            action = new RouteAction_RouteToMailbox(Convert.FromBase64String(dr["ActionData"].ToString()));
                        }

                        Route route = this.Routes.Add(
                            dr["Description"].ToString(),
                            dr["Pattern"].ToString(),
                            ConvertEx.ToBoolean(dr["Enabled"]),
                            action
                        );
                    }
                }
             }

            #endregion


            #region Global message rules

            // Add user message rules
            if(ds.Tables.Contains("GlobalMessageRules")){
                // Delete exisiting global message rules, if ResoreFlags_enum.Replace
                if((restoreFlags & RestoreFlags_enum.Replace) != 0){
                    foreach(GlobalMessageRule rule in this.GlobalMessageRules){
                        this.GlobalMessageRules.Remove(rule);
                    }
                }
                                
                DataView dv = new DataView(ds.Tables["GlobalMessageRules"]);
                foreach(DataRowView drV in dv){
                    GlobalMessageRule rule = this.GlobalMessageRules.Add(
                        ConvertEx.ToBoolean(drV["Enabled"]),
                        drV["Description"].ToString(),
                        drV["MatchExpression"].ToString(),
                        (GlobalMessageRule_CheckNextRule_enum)ConvertEx.ToInt32(drV["CheckNextRuleIf"])                   
                     );
                   
                     // Add rule actions
                     if(ds.Tables.Contains("GlobalMessageRuleActions")){
                         DataView dvRuleActions = new DataView(ds.Tables["GlobalMessageRuleActions"]);
                         dvRuleActions.RowFilter = "RuleID='" + drV["RuleID"].ToString() + "'";
                         foreach(DataRowView drvAction in dvRuleActions){                                        
                             rule.Actions.Add(
                                 drvAction["ActionID"].ToString(),
                                 drvAction["Description"].ToString(),
                                 (GlobalMessageRuleAction_enum)Convert.ToInt32(drvAction["ActionType"]),
                                 Convert.FromBase64String(drvAction["ActionData"].ToString()),
                                 true
                             );
                         }
                     }
                 }
             }

            #endregion


            #region IP Security

            /* NOTE: IP security need special handling, we can't comapre values, so just always overwrite it. 
            */

            if(ds.Tables.Contains("IP_Security")){
                // Delete filters
                foreach(IPSecurity entry in this.IpSecurity.ToArray()){
                    this.IpSecurity.Remove(entry);
                }
                                
                foreach(DataRow dr in ds.Tables["IP_Security"].Rows){
                    this.IpSecurity.Add(
                        ConvertEx.ToBoolean(dr["Enabled"]),
                        dr["Description"].ToString(),
                        (Service_enum)ConvertEx.ToInt32(dr["Service"]),
                        (IPSecurityAction_enum)ConvertEx.ToInt32(dr["Action"]),
                        IPAddress.Parse(dr["StartIP"].ToString()),
                        IPAddress.Parse(dr["EndIP"].ToString())
                    );
                }
             }

            #endregion


            #region Filters

            /* NOTE: Filters need special handling, we can't comapre values, so just always overwrite it. 
            */

            if(ds.Tables.Contains("Filters")){
                // Delete filters
                foreach(Filter filter in this.Filters.ToArray()){
                    this.Filters.Remove(filter);
                }
                                
                foreach(DataRow dr in ds.Tables["Filters"].Rows){
                    this.Filters.Add(
                        ConvertEx.ToBoolean(dr["Enabled"]),
                        dr["Description"].ToString(),
                        dr["Assembly"].ToString(),
                        dr["ClassName"].ToString()
                    );
                }
             }

            #endregion


            #region Users Default Folders

            if(ds.Tables.Contains("UsersDefaultFolders")){
                foreach(DataRow dr in ds.Tables["UsersDefaultFolders"].Rows){
                    // Delete exisiting folder and add new, if ResoreFlags_enum.Replace
                    if(this.UsersDefaultFolders.Contains(dr["FolderName"].ToString())){
                        if((restoreFlags & RestoreFlags_enum.Replace) != 0){
                            this.UsersDefaultFolders.Remove(this.UsersDefaultFolders.GetFolderByName(dr["FolderName"].ToString()));

                            UsersDefaultFolder folder = this.UsersDefaultFolders.Add(
                                dr["FolderName"].ToString(),
                                ConvertEx.ToBoolean(dr["Permanent"])
                            );
                        }
                    }
                    // Add domain list, if ResoreFlags_enum.Add
                    else if((restoreFlags & RestoreFlags_enum.Add) != 0){
                        UsersDefaultFolder folder = this.UsersDefaultFolders.Add(
                            dr["FolderName"].ToString(),
                            ConvertEx.ToBoolean(dr["Permanent"])
                        );
                    }
                }
            }

            #endregion

         }

        #endregion

        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't changed, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }

            /* UpdateVirtualServer <virtualServerID> enabled "<name>" "<initString>:base64"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP UpdateVirtualServer
            m_pServer.TcpClient.TcpStream.WriteLine("UpdateVirtualServer " + 
                m_VirtualServerID + " " +
                m_Enabled + " " +
                TextUtils.QuoteString(m_Name) + " " +
                TextUtils.QuoteString(Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(m_InitString)))
            );
                        
            string response = m_pServer.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }
        }

        #endregion


        #region method DomainChanged

        /// <summary>
        /// Is called when domain has changed.
        /// </summary>
        internal void DomainChanged()
        {
             m_pUsers = null;
            m_pMailingLists = null;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the Server object that is the owner of this collection.
        /// </summary>
        public Server Server
        {
            get{ return m_pServer; }
        }

        /// <summary>
        /// Gets onwer VirtualServerCollection that owns this object.
        /// </summary>
        public VirtualServerCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets if this user object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets virtual server ID.
        /// </summary>
        public string VirtualServerID
        {
            get{ return m_VirtualServerID; }
        }

        /// <summary>
        /// Gets or sets if virtual server is enabled.
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
        /// Gets or sets virtual server name.
        /// </summary>
        public string Name
        {
            get{ return m_Name; }

            set{
                if(m_Name != value){
                    m_Name = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets virtual server assembly name what implements virtual server API.
        /// </summary>
        public string AssemblyName
        {
            get{ return m_Assembly; }
        }

        /// <summary>
        /// Gets virtual server Type name what implements virtual server API.
        /// </summary>
        public string TypeName
        {
            get{ return m_Type; }
        }

        /// <summary>
        /// Gets or sets init string used to configure virtual server API.
        /// </summary>
        public string InitString
        {
            get{ return m_InitString; }

            set{
                if(m_InitString != value){
                    m_InitString = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets virtual server sytem settings.
        /// </summary>
        public System_Settings SystemSettings
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pSystemSettings == null){
                    m_pSystemSettings = new System_Settings(this);
                }

                return m_pSystemSettings; 
            }
        }

        /// <summary>
        /// Gets virtual server domains.
        /// </summary>
        public DomainCollection Domains
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pDomains == null){
                    m_pDomains = new DomainCollection(this);
                }

                return m_pDomains; 
            }
        }

        /// <summary>
        /// Gets virtual server users.
        /// </summary>
        public UserCollection Users
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pUsers == null){
                    m_pUsers = new UserCollection(this);
                }

                return m_pUsers; 
            }
        }

        /// <summary>
        /// Gets virtual server user groups.
        /// </summary>
        public GroupCollection Groups
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pGroups == null){
                    m_pGroups = new GroupCollection(this);
                }

                return m_pGroups; 
            }
        }

        /// <summary>
        /// Gets virtual server mailing lists.
        /// </summary>
        public MailingListCollection MailingLists
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pMailingLists == null){
                    m_pMailingLists = new MailingListCollection(this);
                }

                return m_pMailingLists; 
            }
        }

        /// <summary>
        /// Gets virtual server global message rules.
        /// </summary>
        public GlobalMessageRuleCollection GlobalMessageRules
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pGlobalMsgRules == null){
                    m_pGlobalMsgRules = new GlobalMessageRuleCollection(this);
                }

                return m_pGlobalMsgRules; 
            }
        }

        /// <summary>
        /// Gets virtual server routes.
        /// </summary>
        public RouteCollection Routes
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pRoutes == null){
                    m_pRoutes = new RouteCollection(this);
                }

                return m_pRoutes; 
            }
        }

        /// <summary>
        /// Gets virtual server shared root folders.
        /// </summary>
        public SharedRootFolderCollection RootFolders
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pRootFolders == null){
                    m_pRootFolders = new SharedRootFolderCollection(this);
                }

                return m_pRootFolders; 
            }
        }

        /// <summary>
        /// Gets virtual server shared root folders.
        /// </summary>
        public UsersDefaultFolderCollection UsersDefaultFolders
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_UsersDefaultFolders == null){
                    m_UsersDefaultFolders = new UsersDefaultFolderCollection(this);
                }

                return m_UsersDefaultFolders; 
            }
        }

        /// <summary>
        /// Gets virtual server filters.
        /// </summary>
        public FilterCollection Filters
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pFilters == null){
                    m_pFilters = new FilterCollection(this);
                }

                return m_pFilters; 
            }
        }

        /// <summary>
        /// Gets virtual server IP security entries collection.
        /// </summary>
        public IPSecurityCollection IpSecurity
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pIpSecurity == null){
                    m_pIpSecurity = new IPSecurityCollection(this);
                }

                return m_pIpSecurity; 
            }
        }

        /// <summary>
        /// Gets virtual server queues.
        /// </summary>
        public Queues Queues
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pQueues == null){
                    m_pQueues = new Queues(this);
                }

                return m_pQueues; 
            }
        }

        /// <summary>
        /// Gets virtual server recycle bin.
        /// </summary>
        public RecycleBin RecycleBin
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pRecycleBin == null){
                    m_pRecycleBin = new RecycleBin(this);
                }

                return m_pRecycleBin; 
            }
        }

        /// <summary>
        /// Gets virtual server logs.
        /// </summary>
        public Logs Logs
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pLogs == null){
                    m_pLogs = new Logs(this);
                }

                return m_pLogs; 
            }
        }

        /// <summary>
        /// Gets virtual server SIP registrations.
        /// </summary>
        public SipRegistrationCollection SipRegistrations
        {
            get{
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pSipRegistrations == null){
                    m_pSipRegistrations = new SipRegistrationCollection(this);
                }

                return m_pSipRegistrations; 
            } 
        }

        /// <summary>
        /// Gets virtual server SIP calls.
        /// </summary>
        public SIP_CallCollection SipCalls
        {
            get{
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pSipCalls == null){
                    m_pSipCalls = new SIP_CallCollection(this);
                }

                return m_pSipCalls; 
            } 
        }

        #endregion

    }
}
