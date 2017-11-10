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
    /// The SharedRootFolderCollection object represents shared root folders in LumiSoft Mail Server virtual server.
    /// </summary>
    public class SharedRootFolderCollection : IEnumerable
    {
        private VirtualServer          m_pVirtualServer = null;
        private List<SharedRootFolder> m_pRootFolders   = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal SharedRootFolderCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pRootFolders   = new List<SharedRootFolder>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds new shared root folder to virtual server.
        /// </summary>
        /// <param name="enabled">Specifies if shared root folder is enabled.</param>
        /// <param name="name">Shared root folder name.</param>
        /// <param name="description">Shared root folder description.</param>
        /// <param name="type">Shared root folder type.</param>
        /// <param name="boundedUser">Bounded user.</param>
        /// <param name="boundedFolder">Bounded folder.</param>
        public SharedRootFolder Add(bool enabled,string name,string description,SharedFolderRootType_enum type,string boundedUser,string boundedFolder)
        {
            /* AddSharedRootFolder <virtualServerID> "<rootFolderID>" "<rootFolderName>" "<description>" <type> "<boundedUser>" "boundedFolder" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddSharedRootFolder
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddSharedRootFolder " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(id) + " " + 
                TextUtils.QuoteString(name) + " " + 
                TextUtils.QuoteString(description) + " " + 
                (int)type + " " +
                TextUtils.QuoteString(boundedUser) + " " + 
                TextUtils.QuoteString(boundedFolder) + " " + 
                enabled
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            SharedRootFolder rootFolder = new SharedRootFolder(m_pVirtualServer,this,id,enabled,name,description,type,boundedUser,boundedFolder);
            m_pRootFolders.Add(rootFolder);
            return rootFolder;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified shared root folder from virtual server.
        /// </summary>
        /// <param name="sharedFolder">Shared folder to delete.</param>
        public void Remove(SharedRootFolder sharedFolder)
        {
            /* DeleteSharedRootFolder <virtualServerID> "<rootFolderID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteGroup
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteSharedRootFolder " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(sharedFolder.ID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pRootFolders.Remove(sharedFolder);
        }

        #endregion

        #region method Contains

        /// <summary>
        /// Check if collection contains root folder with specified name.
        /// </summary>
        /// <param name="rootFolderName">Root folder name.</param>
        /// <returns></returns>
        public bool Contains(string rootFolderName)
        {
            foreach(SharedRootFolder root in m_pRootFolders){
                if(root.Name.ToLower() == rootFolderName.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method GetRootFolderByName

        /// <summary>
        /// Gets a SharedRootFolder object in the collection by root folder name.
        /// </summary>
        /// <param name="rootFolderName">Root folder name.</param>
        /// <returns>A SharedRootFolder object value that represents the shared root folder in virtual server.</returns>
        public SharedRootFolder GetRootFolderByName(string rootFolderName)
        {
            foreach(SharedRootFolder root in m_pRootFolders){
                if(root.Name.ToLower() == rootFolderName.ToLower()){
                    return root;
                }
            }

            throw new Exception("SharedRootFolder with specified name '" + rootFolderName + "' doesn't exist !");
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server shared root folders and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetSharedRootFolders <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetSharedRootFolders
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetSharedRootFolders " + m_pVirtualServer.VirtualServerID);

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("SharedFoldersRoots")){
                    foreach(DataRow dr in ds.Tables["SharedFoldersRoots"].Rows){
                        m_pRootFolders.Add(new SharedRootFolder(
                            m_pVirtualServer,
                            this,
                            dr["RootID"].ToString(),
                            Convert.ToBoolean(dr["Enabled"].ToString()),
                            dr["Folder"].ToString(),
                            dr["Description"].ToString(),
                            (SharedFolderRootType_enum)Convert.ToInt32(dr["RootType"]),
                            dr["BoundedUser"].ToString(),
                            dr["BoundedFolder"].ToString()
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
			return m_pRootFolders.GetEnumerator();
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
        /// Gets number of shared root folders in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pRootFolders.Count; }
        }

        /// <summary>
        /// Gets a SharedRootFolder object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the SharedRootFolder object in the SharedRootFolderCollection collection.</param>
        /// <returns>A SharedRootFolder object value that represents the shared root folder in virtual server.</returns>
        public SharedRootFolder this[int index]
        {
            get{ return m_pRootFolders[index]; }
        }

        /// <summary>
        /// Gets a SharedRootFolder object in the collection by group ID.
        /// </summary>
        /// <param name="rootFolderID">A String value that specifies the shared root folder ID of the SharedRootFolder object in the SharedRootFolderCollection collection.</param>
        /// <returns>A SharedRootFolder object value that represents the shared root folder in virtual server.</returns>
        public SharedRootFolder this[string rootFolderID]
        {
            get{ 
                foreach(SharedRootFolder rootFolder in m_pRootFolders){
                    if(rootFolder.ID.ToLower() == rootFolderID.ToLower()){
                        return rootFolder;
                    }
                }

                throw new Exception("SharedRootFolder with specified ID '" + rootFolderID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
