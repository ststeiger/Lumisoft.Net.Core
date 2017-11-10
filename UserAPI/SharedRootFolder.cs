using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The SharedRootFolder object represents shared root folder in LumiSoft Mail Server virtual server.
    /// </summary>
    public class SharedRootFolder
    {
        private VirtualServer              m_pVirtualServer = null;
        private SharedRootFolderCollection m_pOwner         = null;
        private string                     m_ID             = "";
        private bool                       m_Enabled        = false;
        private string                     m_Name           = "";
        private string                     m_Description    = "";
        private SharedFolderRootType_enum  m_FolderType     = SharedFolderRootType_enum.BoundedRootFolder;
        private string                     m_BoundedUser    = "";
        private string                     m_BoundedFolder  = "";
        private bool                       m_ValuesChanged  = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <param name="owner">Owner SharedRootFolderCollection collection that owns this root folder.</param>
        /// <param name="id">Root folder ID.</param>
        /// <param name="enabled">Specifies if root folder is enabled.</param>
        /// <param name="name">Root folder name.</param>
        /// <param name="description">Root folder description.</param>
        /// <param name="type">Root folder type.</param>
        /// <param name="boundedUser">Root folder bounded user.</param>
        /// <param name="boundedFolder">Root folder bounded folder.</param>
        internal SharedRootFolder(VirtualServer virtualServer,SharedRootFolderCollection owner,string id,bool enabled,string name,string description,SharedFolderRootType_enum type,string boundedUser,string boundedFolder)
        {
            m_pVirtualServer = virtualServer;
            m_pOwner         = owner;
            m_ID             = id;
            m_Enabled        = enabled;
            m_Name           = name;
            m_Description    = description;
            m_FolderType     = type;
            m_BoundedUser    = boundedUser;
            m_BoundedFolder  = boundedFolder;
        }


        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't chnaged, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }

            /* UpdateSharedRootFolder <virtualServerID> "<rootFolderID>" "<rootFolderName>" "<description>" <type> "<boundedUser>" "boundedFolder" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP AddSharedRootFolder
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateSharedRootFolder " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_ID) + " " + 
                TextUtils.QuoteString(m_Name) + " " + 
                TextUtils.QuoteString(m_Description) + " " + 
                (int)m_FolderType + " " +
                TextUtils.QuoteString(m_BoundedUser) + " " + 
                TextUtils.QuoteString(m_BoundedFolder) + " " + 
                m_Enabled
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner SharedRootFolderCollection that owns this root folder.
        /// </summary>
        public SharedRootFolderCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets if this group object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets root folder ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets or sets if root folder is enabled.
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
        /// Gets or sets root folder name.
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
        /// Gets or sets root folder description.
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
        /// Gets or sets root folder type.
        /// </summary>
        public SharedFolderRootType_enum Type
        {
            get{ return m_FolderType; }

            set{
                if(m_FolderType != value){
                    m_FolderType = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets bounded user.
        /// </summary>
        public string BoundedUser
        {
            get{ return m_BoundedUser; }

            set{
                if(m_BoundedUser != value){
                    m_BoundedUser = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets bounded folder.
        /// </summary>
        public string BoundedFolder
        {
            get{ return m_BoundedFolder; }

            set{
                if(m_BoundedFolder != value){
                    m_BoundedFolder = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
