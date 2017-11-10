using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Specifies virtual server backup restore options.
    /// </summary>
    public enum RestoreFlags_enum
    {
        /// <summary>
        /// All missing items added.
        /// </summary>
        Add = 2,

        /// <summary>
        /// All existing items will be overwritten.
        /// </summary>
        Replace = 4
    }
}
