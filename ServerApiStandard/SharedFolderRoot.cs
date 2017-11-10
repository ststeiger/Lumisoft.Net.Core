using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Shared Folders root folder.
    /// </summary>
    public class SharedFolderRoot
    {
        private string                    m_RootID        = "";
        private bool                      m_Enabled       = true;
        private string                    m_FolderName    = "";
        private string                    m_Description   = "";
        private SharedFolderRootType_enum m_RootType      = SharedFolderRootType_enum.BoundedRootFolder;
        private string                    m_BoundedUser   = "";
        private string                    m_BoundedFolder = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rootID">Root Folder ID.</param>
        /// <param name="enabled">Specifies if root folder is enabled.</param>
        /// <param name="folderName">Root folder visible name.</param>
        /// <param name="description">Description text.</param>
        /// <param name="rootType">Specifies what type root folder is.</param>
        /// <param name="boundedUser">User which to bound root folder.</param>
        /// <param name="boundedFolder">BoundingUser folder name which to bound root folder.</param>
        public SharedFolderRoot(string rootID,bool enabled,string folderName,string description,SharedFolderRootType_enum rootType,string boundedUser,string boundedFolder)
        {
            m_RootID        = rootID;
            m_Enabled       = enabled;
            m_FolderName    = folderName;
            m_Description   = description;
            m_RootType      = rootType;
            m_BoundedUser   = boundedUser;
            m_BoundedFolder = boundedFolder;
        }


        #region method Properites Implementation

        /// <summary>
        /// Gets Root Folder ID.
        /// </summary>
        public string RootID
        {
            get{ return m_RootID; }
        }

        /// <summary>
        /// Gets if root folder is enabled.
        /// </summary>
        public bool Enabled
        {
            get{ return m_Enabled; }
        }

        /// <summary>
        /// Gets root folder name. For example "Public Folders".
        /// </summary>
        public string FolderName
        {
            get{ return m_FolderName; }
        }


        /// <summary>
        /// Gets root folder description.
        /// </summary>
        public string Description
        {
            get{ return m_Description; }
        }

        /// <summary>
        /// Gets what type root folder it.
        /// </summary>
        public SharedFolderRootType_enum RootType
        {
            get{ return m_RootType; }
        }

        /// <summary>
        /// Gets user name which to bound root folder.
        /// </summary>
        public string BoundedUser
        {
            get{ return m_BoundedUser; }
        }

        /// <summary>
        /// Gets BoundedUser folder name which to bound root folder.
        /// </summary>
        public string BoundedFolder
        {
            get{ return m_BoundedFolder; }
        }

        #endregion

    }
}
