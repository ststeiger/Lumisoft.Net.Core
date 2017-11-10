using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.IMAP.Server;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserFolderAclCollection object represents user folder ACL entries in UserFolder.
    /// </summary>
    public class UserFolderAclCollection : IEnumerable
    {
        private UserFolder          m_pFolder     = null;
        private List<UserFolderAcl> m_pAclEntries = null;

        /// <summary>
        /// Default consturctor.
        /// </summary>
        internal UserFolderAclCollection(UserFolder folder)
        {
            m_pFolder     = folder;
            m_pAclEntries = new List<UserFolderAcl>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds specified user or group permissions to active folder.
        /// </summary>
        /// <param name="userOrGroup">User or group.</param>
        /// <param name="permissions">Permissions to allow.</param>
        /// <returns>Returns added UserFolderAcl.</returns>
        public UserFolderAcl Add(string userOrGroup,IMAP_ACL_Flags permissions)
        {
            /* SetUserFolderAcl <virtualServerID> "<folderOwnerUser>" "<folder>" "<userOrGroup>" <flags:int32>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP SetUserFolderAcl
            m_pFolder.User.VirtualServer.Server.TcpClient.TcpStream.WriteLine("SetUserFolderAcl " + m_pFolder.User.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pFolder.User.UserName) + " " + TextUtils.QuoteString(m_pFolder.FolderFullPath) + " " + TextUtils.QuoteString(userOrGroup) + " " + (int)permissions);
                        
            string response = m_pFolder.User.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            UserFolderAcl acl = new UserFolderAcl(this,m_pFolder,userOrGroup,permissions);
            m_pAclEntries.Add(acl);
            return acl;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified user or group permissions from active folder.
        /// </summary>
        /// <param name="aclEntry">User or group.</param>
        public void Remove(UserFolderAcl aclEntry)
        {
            /* DeleteUserFolderAcl <virtualServerID> "<folderOwnerUser>" "<folder>" "<userOrGroup>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteUserFolderAcl
            m_pFolder.User.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUserFolderAcl " + 
                m_pFolder.User.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pFolder.User.UserName) + " " + 
                TextUtils.QuoteString(m_pFolder.FolderFullPath) + " " + 
                TextUtils.QuoteString(aclEntry.UserOrGroup)
            );
                        
            string response = m_pFolder.User.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pAclEntries.Remove(aclEntry);
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server user folders and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetUserFolderAcl <virtualServerID> <userID> "<folderName>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pFolder.User.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetUserFolderAcl
                m_pFolder.User.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserFolderAcl " + 
                    m_pFolder.User.VirtualServer.VirtualServerID + " " + 
                    m_pFolder.User.UserID + " \"" + 
                    m_pFolder.FolderFullPath
                );

                string response = m_pFolder.User.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pFolder.User.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("ACL")){
                    foreach(DataRow dr in ds.Tables["ACL"].Rows){
                        m_pAclEntries.Add(new UserFolderAcl(
                            this,
                            m_pFolder,
                            dr["User"].ToString(),
                            IMAP_Utils.ACL_From_String(dr["Permissions"].ToString())
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
			return m_pAclEntries.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets folder for what this ACL apply.
        /// </summary>
        public UserFolder Folder
        {
            get{ return m_pFolder; }
        }

        /// <summary>
        /// Gets number of ACL entries for that folder.
        /// </summary>
        public int Count
        {
            get{ return m_pAclEntries.Count; }
        }

        /// <summary>
        /// Gets a UserFolderAcl object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the UserFolderAcl object in the UserFolderAclCollection collection.</param>
        /// <returns>A UserFolderAcl object value that represents the user folder ACL in virtual server.</returns>
        public UserFolderAcl this[int index]
        {
            get{ return m_pAclEntries[index]; }
        }

        /// <summary>
        /// Gets a UserFolderAcl object in the collection by user or group name.
        /// </summary>
        /// <param name="userOrGroup">A String value that specifies the user or group name of the UserFolderAcl object in the UserFolderAclCollection collection.</param>
        /// <returns>A UserFolderAcl object value that represents the user folder ACL in virtual server.</returns>
        public UserFolderAcl this[string userOrGroup]
        {
            get{   
                foreach(UserFolderAcl acl in m_pAclEntries){
                    if(acl.UserOrGroup.ToLower() == userOrGroup.ToLower()){
                        return acl;
                    }
                }

                throw new Exception("User or group with specified name '" + userOrGroup + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
