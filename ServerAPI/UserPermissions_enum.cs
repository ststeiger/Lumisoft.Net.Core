using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// User permission flags.
    /// </summary>
    public enum UserPermissions_enum
    {
        /// <summary>
        /// None of the permissions.
        /// </summary>
        None = 0,

        /// <summary>
        /// All permissions.
        /// </summary>
        All = 0xFFFF,

        /// <summary>
        /// POP3 access.
        /// </summary>
        POP3 = 2,

        /// <summary>
        /// IMAP access.
        /// </summary>
        IMAP = 4,

        /// <summary>
        /// Can relay. This permission is used only if connected user IP doesn't have relay permission.
        /// </summary>
        Relay = 8,

        /// <summary>
        /// SIP access.
        /// </summary>
        SIP = 16,
    }
}
