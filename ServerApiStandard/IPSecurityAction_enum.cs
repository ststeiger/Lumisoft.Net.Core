using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Specifiess IP security action.
    /// </summary>
    public enum IPSecurityAction_enum
    {
        /// <summary>
        /// Allow access.
        /// </summary>
        Allow = 1,

        /// <summary>
        /// Deny access.
        /// </summary>
        Deny = 2,
    }
}
