using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Specifies shared folder root folder type.
    /// </summary>
    public enum SharedFolderRootType_enum
    {
        /// <summary>
        /// Root folder is bounded to some user account.
        /// </summary>
        BoundedRootFolder = 1,

        /// <summary>
        /// Users shared folders will be added into this root folder.
        /// </summary>
        UsersSharedFolder = 2,
    }
}
