using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UsersDefaultFolder object represents users default folder in LumiSoft Mail Server virtual server.
    /// </summary>
    public class UsersDefaultFolder
    {
        private UsersDefaultFolderCollection m_pOwner     = null;
        private string                       m_FolderName = "";
        private bool                         m_Permanent  = true;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner UsersDefaultFolderCollection collection that owns this.</param>
        /// <param name="folderName">Folder name.</param>
        /// <param name="permanent">Specifies if folder is permanent (User can't delete it).</param>
        internal UsersDefaultFolder(UsersDefaultFolderCollection owner,string folderName,bool permanent)
        {
            m_pOwner     = owner;
            m_FolderName = folderName;
            m_Permanent  = permanent;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets owner UsersDefaultFolderCollection that owns this.
        /// </summary>
        public UsersDefaultFolderCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets folder name.
        /// </summary>
        public string FolderName
        {
            get{ return m_FolderName; }
        }

        /// <summary>
        /// Gets if folder is permanent (User can't delete it).
        /// </summary>
        public bool Permanent
        {
            get{ return m_Permanent; }
        }

        #endregion

    }
}
