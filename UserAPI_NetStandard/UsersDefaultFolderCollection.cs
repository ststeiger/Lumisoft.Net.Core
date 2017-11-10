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
    /// The UsersDefaultFolderCollection object represents users default folders in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UsersDefaultFolderCollection : IEnumerable
    {
        private VirtualServer            m_pVirtualServer = null;
        private List<UsersDefaultFolder> m_pFolders       = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        public UsersDefaultFolderCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pFolders       = new List<UsersDefaultFolder>();

            Bind();            
        }


        #region method Add

        /// <summary>
        /// Creates and adds new users default folder into collection.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        /// <param name="permanent">Specifies if folder is permanent (User can't delete it).</param>
        public UsersDefaultFolder Add(string folderName,bool permanent)
        {
            /* AddUsersDefaultFolder <virtualServerID> "<folderName>" <permanent>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            if(folderName.IndexOfAny(new char[]{'\\','/'}) > -1){
                throw new Exception("Folders with path not allowed !");
            }

            string id = Guid.NewGuid().ToString();

            // Call TCP AddUsersDefaultFolder
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddUsersDefaultFolder " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(folderName) + " " +
                permanent
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            UsersDefaultFolder folder = new UsersDefaultFolder(this,folderName,permanent);
            m_pFolders.Add(folder);
            return folder;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes and removes specified users default folder from collection.
        /// </summary>
        /// <param name="folder">UsersDefaultFolder to delete.</param>
        public void Remove(UsersDefaultFolder folder)
        {
            /* DeleteUsersDefaultFolder <virtualServerID> "<folderName>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            if(folder.FolderName.ToLower() == "inbox"){
                throw new Exception("Inbox is permanent system folder and can't be deleted ! '");                
            }

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteGroup
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUsersDefaultFolder " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(folder.FolderName)
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pFolders.Remove(folder);
        }

        #endregion        

        #region method Contains

        /// <summary>
        /// Check if collection contains users default folder with specified name.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        /// <returns></returns>
        public bool Contains(string folderName)
        {
            foreach(UsersDefaultFolder folder in m_pFolders){
                if(folder.FolderName.ToLower() == folderName.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method GetFolderByName

        /// <summary>
        /// Gets a UsersDefaultFolder object in the collection by folder name.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        /// <returns>A UsersDefaultFolder object value that represents the users default folder in virtual server.</returns>
        public UsersDefaultFolder GetFolderByName(string folderName)
        {
            foreach(UsersDefaultFolder folder in m_pFolders){
                if(folder.FolderName.ToLower() == folderName.ToLower()){
                    return folder;
                }
            }

            throw new Exception("Folder with specified name '" + folderName + "' doesn't exist !");
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server users default folders and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetUsersDefaultFolders <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetUsersDefaultFolders
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUsersDefaultFolders " + m_pVirtualServer.VirtualServerID);

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("UsersDefaultFolders")){
                    foreach(DataRow dr in ds.Tables["UsersDefaultFolders"].Rows){
                        m_pFolders.Add(new UsersDefaultFolder(
                            this,
                            dr["FolderName"].ToString(),
                            ConvertEx.ToBoolean(dr["Permanent"])
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
			return m_pFolders.GetEnumerator();
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
        /// Gets number of users default folders in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pFolders.Count; }
        }

        /// <summary>
        /// Gets a UsersDefaultFolder object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the UsersDefaultFolder object in the UsersDefaultFolderCollection collection.</param>
        /// <returns>A UsersDefaultFolder object value that represents the virtual server users default folder.</returns>
        public UsersDefaultFolder this[int index]
        {
            get{ return m_pFolders[index]; }
        }

        /// <summary>
        /// Gets a UsersDefaultFolder object in the collection by domain ID.
        /// </summary>
        /// <param name="folderName">A String value that specifies the users default folder name of the UsersDefaultFolder object in the UsersDefaultFolderCollection collection.</param>
        /// <returns> UsersDefaultFolder object value that represents the virtual server users default folder.</returns>
        public UsersDefaultFolder this[string folderName]
        {
            get{ 
                foreach(UsersDefaultFolder folder in m_pFolders){
                    if(folder.FolderName.ToLower() == folderName.ToLower()){
                        return folder;
                    }
                }

                throw new Exception("Users default folder with specified name '" + folderName + "' doesn't exist !"); 
            }
        }

        #endregion
    }
}
