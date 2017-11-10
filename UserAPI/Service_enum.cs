using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Specifies service
    /// </summary>
    public enum Service_enum
    {
        /// <summary>
        /// SMTP.
        /// </summary>
        SMTP = 1,

        /// <summary>
        /// POP3.
        /// </summary>
        POP3 = 2,

        /// <summary>
        /// IMAP.
        /// </summary>
        IMAP = 3,

        /// <summary>
        /// SMTP Relay.
        /// </summary>
        Relay = 4,
    }
}
