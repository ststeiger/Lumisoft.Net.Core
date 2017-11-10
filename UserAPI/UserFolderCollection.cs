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
    /// The UserFolderCollection object represents user folders in user.
    /// </summary>
    public class UserFolderCollection : IEnumerable
    {
        private UserFolder       m_pFolder  = null;
        private User             m_pUser    = null;
        private List<UserFolder> m_pFolders = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="bind"></param>
        /// <param name="folder">Owner folder or null if root.</param>
        /// <param name="user">Owner user.</param>
        internal UserFolderCollection(bool bind,UserFolder folder,User user)
        {
            m_pFolder  = folder;
            m_pUser    = user;
            m_pFolders = new List<UserFolder>();

            if(bind){
                Bind();
            }
        }


        #region method Add

        /// <summary>
        /// Adds new folder to user.
        /// </summary>
        /// <param name="newFolder">Folder to add. NOTE: Folder may not conatian path parts like folder/subfolder !</param>
        public UserFolder Add(string newFolder)
        {
            /* AddUserFolder <virtualServerID> "<folderOwnerUser>" "<folder>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();
            string path = "";
            string folderFullPath = newFolder;
            if(m_pFolder != null){
                path = m_pFolder.FolderFullPath;
                folderFullPath = path + "/" + newFolder; 
            }

            // Call TCP AddUserFolder
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddUserFolder " + m_pUser.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pUser.UserName) + " " + TextUtils.QuoteString(folderFullPath));
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            UserFolder folder = new UserFolder(this,m_pUser,m_pFolder,path,newFolder);
            m_pFolders.Add(folder);
            return folder;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified folder.
        /// </summary>
        /// <param name="folder">Folder to remove.</param>
        public void Remove(UserFolder folder)
        {
            /* DeleteUserFolder <virtualServerID> "<folderOwnerUser>" "<folder>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteUserFolder
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUserFolder " + m_pUser.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_pUser.UserName) + " " + TextUtils.QuoteString(folder.FolderFullPath));
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pFolders.Remove(folder);
        }

        #endregion


        #region method Contains

        /// <summary>
        /// Checks if specified folder name exists in this collection.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        /// <returns></returns>
        public bool Contains(string folderName)
        {
            foreach(UserFolder folder in m_pFolders){
                if(folder.FolderName.ToLower() == folderName.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server user folders and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {            
            /* GetUserFolders <virtualServerID> <userID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pUser.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetUserEmailAddresses
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserFolders " + m_pUser.VirtualServer.VirtualServerID + " " + m_pUser.UserID);

                string response = m_pUser.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("Folders")){
                    foreach(DataRow dr in ds.Tables["Folders"].Rows){
                        string[] folderPathParts = dr["Folder"].ToString().Split('/');
                        UserFolderCollection current = this;
                        string currentPath = "";
                        foreach(string pathPart in folderPathParts){
                            if(!current.Contains(pathPart)){                            
                                UserFolder f = new UserFolder(current,m_pUser,current.Parent,currentPath,pathPart);
                                current.List.Add(f);
                            }
                         
                            current = current[pathPart].ChildFolders;
                            if(currentPath == ""){
                                currentPath = pathPart;
                            }
                            else{
                                currentPath += "/" + pathPart;
                            }
                        }
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
			return m_pFolders.GetEnumerator();
		}

		#endregion


        #region Properties Implementation
        
        /// <summary>
        /// Gets parent folder. Returns null if this is root folders collection.
        /// </summary>
        public UserFolder Parent
        {
            get{ return m_pFolder; }
        }

        /// <summary>
        /// Gets number of folders in that collection.
        /// </summary>
        public int Count
        {
            get{ return m_pFolders.Count; }
        }

        /// <summary>
        /// Gets a folder in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the folder object in the UserFolderCollection collection.</param>
        /// <returns></returns>
        public UserFolder this[int index]
        {
            get{ return m_pFolders[index]; }
        }

        /// <summary>
        /// Gets a folder in the collection by folder name.
        /// </summary>
        /// <param name="folderName">A String value that specifies the folder name in the UserFolderCollection collection.</param>
        /// <returns></returns>
        public UserFolder this[string folderName]
        {
            get{   
                foreach(UserFolder folder in m_pFolders){
                    if(folder.FolderName.ToLower() == folderName.ToLower()){
                        return folder;
                    }
                }

                throw new Exception("Folder with specified name '" + folderName + "' doesn't exist !"); 
            }
        }


        /// <summary>
        /// Gets direct access to internal collection.
        /// </summary>
        internal List<UserFolder> List
        {
            get{ return m_pFolders; }
        }

        #endregion

    }
}
