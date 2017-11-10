using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The User object represents user in LumiSoft Mail Server virtual server.
    /// </summary>
    public class User
    {
        private VirtualServer              m_pVirtualServer  = null;
        private UserCollection             m_pOwner          = null;
        private string                     m_UserID          = "";
        private bool                       m_Enabled         = false;
        private string                     m_UserName        = "";
        private string                     m_Password        = "";
        private string                     m_FullName        = "";
        private string                     m_Description     = "";
        private int                        m_MailboxSize     = 0;
        private UserPermissions_enum       m_Permissions     = UserPermissions_enum.None;
        private UserEmailAddressCollection m_pEmailAddresses = null;
        private UserFolderCollection       m_pFolders        = null;
        private UserMessageRuleCollection  m_pMessageRules   = null;
        private UserRemoteServerCollection m_pRemoteServers  = null;
        private DateTime                   m_CreationTime;
        private bool                       m_ValuesChanged   = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <param name="owner">Owner UserCollection collection that owns this user.</param>
        /// <param name="id">User ID.</param>
        /// <param name="enabled">Specifies if user is enabled.</param>
        /// <param name="userName">User login name.</param>
        /// <param name="password">User password.</param>
        /// <param name="fullName">User full name.</param>
        /// <param name="description">User description.</param>
        /// <param name="mailboxSize">Mailbox size in MB.</param>
        /// <param name="permissions">Specifies user permissions.</param>
        /// <param name="creationTime">Time when user was created.</param>
        internal User(VirtualServer virtualServer,UserCollection owner,string id,bool enabled,string userName,string password,string fullName,string description,int mailboxSize,UserPermissions_enum permissions,DateTime creationTime)
        {
            m_pVirtualServer  = virtualServer;
            m_pOwner          = owner;
            m_UserID          = id;
            m_Enabled         = enabled;
            m_UserName        = userName;
            m_Password        = password;
            m_FullName        = fullName;
            m_Description     = description;
            m_MailboxSize     = mailboxSize;
            m_Permissions     = permissions;
            m_CreationTime    = creationTime;
        }


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

            /* UpdateUser <virtualServerID> "<userID>" "<userName>" "<fullName>" "<password>" "<description>" <mailboxSize> <enabled> <allowRelay>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP AddUser
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateUser " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_UserID) + " " + 
                TextUtils.QuoteString(m_UserName) + " " + 
                TextUtils.QuoteString(m_FullName) + " " + 
                TextUtils.QuoteString(m_Password) + " " + 
                TextUtils.QuoteString(m_Description) + " " + 
                m_MailboxSize + " " +
                m_Enabled + " " +
                (int)m_Permissions
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }
        }

        #endregion


        #region method SetPassword
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPassword"></param>
        public void SetPassword(string newPassword)
        {
        }*/

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets virtual server to where this collection belongs to.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets owner UserCollection that owns this user.
        /// </summary>
        public UserCollection Owner
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
        /// Gets user ID.
        /// </summary>
        public string UserID
        {
            get{ return m_UserID; }
        }

        /// <summary>
        /// Gets or sets if user is enabled.
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
        /// Gets or sets user login name.
        /// </summary>
        public string UserName
        {
            get{ return m_UserName; }

            set{
                if(m_UserName != value){
                    m_UserName = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        public string Password
        {
            get{ return m_Password; }

            set{
                if(m_Password != value){
                    m_Password = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets user full name.
        /// </summary>
        public string FullName
        {
            get{ return m_FullName; }

            set{
                if(m_FullName != value){
                    m_FullName = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets user description.
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
        /// Gets or sets user maximum mailbox size in MB.
        /// </summary>
        public int MaximumMailboxSize
        {
            get{ return m_MailboxSize; }

            set{
                if(m_MailboxSize != value){
                    m_MailboxSize = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets user mailbox size in bytes.
        /// </summary>
        public long MailboxSize
        {
            get{ 
                /* GetUserMailboxSize <virtualServerID> "<userID>"
                      Responses:
                        +OK                     
                        -ERR <errorText>
                */

                // Call TCP GetUserMailboxSize
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserMailboxSize " + 
                    m_pVirtualServer.VirtualServerID + " " + 
                    TextUtils.QuoteString(m_UserID)
                );
                            
                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                return Convert.ToInt64(response.Substring(4).Trim());
            }
        }
                
        /// <summary>
        /// Gets or sets user permissions.
        /// </summary>
        public UserPermissions_enum Permissions
        {
            get{ return m_Permissions; }

            set{
                if(m_Permissions != value){
                    m_Permissions = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets user email addresses.
        /// </summary>
        public UserEmailAddressCollection EmailAddresses
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pEmailAddresses == null){
                    m_pEmailAddresses = new UserEmailAddressCollection(this);
                }

                return m_pEmailAddresses;
            }
        }

        
        /// <summary>
        /// Gets user folders.
        /// </summary>
        public UserFolderCollection Folders
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pFolders == null){
                    m_pFolders = new UserFolderCollection(true,null,this);
                }

                return m_pFolders;
            }
        }

        /// <summary>
        /// Gets user remote servers.
        /// </summary>
        public UserMessageRuleCollection MessageRules
        {
            get{
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pMessageRules == null){
                    m_pMessageRules = new UserMessageRuleCollection(this);
                }

                return m_pMessageRules;
            }
        }

        /// <summary>
        /// Gets user remote servers.
        /// </summary>
        public UserRemoteServerCollection RemoteServers
        {
            get{
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pRemoteServers == null){
                    m_pRemoteServers = new UserRemoteServerCollection(this);
                }

                return m_pRemoteServers;
            }
        }

        /// <summary>
        /// Gets date/time when user was created.
        /// </summary>
        public DateTime CreationTime
        {
            get{ return m_CreationTime; }
        }

        /// <summary>
        /// Gets user last login time.
        /// </summary>
        public DateTime LastLogin
        {
            get{
                /* GetUserLastLoginTime <virtualServerID> "<userID>"
                      Responses:
                        +OK <lastLoginTime>                    
                        -ERR <errorText>
                */

                // Call TCP GetUserLastLoginTime
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserLastLoginTime " + 
                    m_pVirtualServer.VirtualServerID + " " + 
                    TextUtils.QuoteString(m_UserID)
                );
                            
                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                return Convert.ToDateTime(response.Substring(4).Trim());
            }
        }
        
        #endregion

    }
}
