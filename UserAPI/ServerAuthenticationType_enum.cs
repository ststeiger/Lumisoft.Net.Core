using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Specifies users authentication method.
    /// </summary>
    public enum ServerAuthenticationType_enum
    {
        /// <summary>
        /// Server uses local user names and passwords todo authentication.
        /// </summary>
        Integrated = 0,

        /// <summary>
        /// Server authenticates against windows.
        /// </summary>
        Windows = 1,

        /// <summary>
        /// LDAP authentication.
        /// </summary>
        Ldap = 2,
    }
}
