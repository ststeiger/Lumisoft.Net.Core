using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.IMAP;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserFolderAcl object represents user folder ACL in UserFolder.
    /// </summary>
    public class UserFolderAcl
    {
        private UserFolderAclCollection m_pOwner        = null;
        private UserFolder              m_pFolder       = null;
        private string                  m_UserOrGroup   = "";
        private IMAP_ACL_Flags          m_Permissions   = IMAP_ACL_Flags.None;
        private bool                    m_ValuesChanged = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner UserFolderAclCollection collection that owns this object.</param>
        /// <param name="folder">Folder to what that ACl entry applies.</param>
        /// <param name="userOrGroup">User or group to who this acl apply.</param>
        /// <param name="permissions">Permissions to owner folder.</param>
        internal UserFolderAcl(UserFolderAclCollection owner,UserFolder folder,string userOrGroup,IMAP_ACL_Flags permissions)
        {
            m_pOwner      = owner;
            m_pFolder     = folder;
            m_UserOrGroup = userOrGroup;
            m_Permissions = permissions;
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

            /* SetUserFolderAcl <virtualServerID> "<folderOwnerUser>" "<folder>" "<userOrGroup>" <flags:int32>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP SetUserFolderAcl
            m_pFolder.User.VirtualServer.Server.TcpClient.TcpStream.WriteLine("SetUserFolderAcl " + 
                m_pFolder.User.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pFolder.User.UserName) + " " + 
                TextUtils.QuoteString(m_pFolder.FolderFullPath) + " " + 
                TextUtils.QuoteString(m_UserOrGroup) + " " + 
                (int)m_Permissions
            );
                        
            string response = m_pFolder.User.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation
        
        /// <summary>
        /// Gets owner UserFolderAclCollection that owns this object.
        /// </summary>
        public UserFolderAclCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets folder where this ACL entry applies.
        /// </summary>
        public UserFolder Folder
        {
            get{ return m_pFolder; }
        }

        /// <summary>
        /// Gets user or Group name to who that ACL will apply.
        /// </summary>
        public string UserOrGroup
        {
            get{ return m_UserOrGroup; }
        }

        /// <summary>
        /// Gets or sets user/group permissions on that folder.
        /// </summary>
        public IMAP_ACL_Flags Permissions
        {
            get{ return m_Permissions; }

            set{
                if(m_Permissions != value){
                    m_Permissions = value;

                    m_ValuesChanged = true;
                }
            }
        }
        

        #endregion

    }
}
