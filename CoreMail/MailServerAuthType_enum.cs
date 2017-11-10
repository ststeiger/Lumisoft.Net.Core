using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Specifies mailserver authentication type.
    /// </summary>
    public enum MailServerAuthType_enum
    {
        /// <summary>
        /// Mail server integrated authentication. Uses mailserver user name and password.
        /// </summary>
        Integrated = 1,

        /// <summary>
        /// Windows authentication. Uses mail server user name and windows password to do authentication.
        /// </summary>
        Windows = 2,

        /// <summary>
        /// LDAP authentication.
        /// </summary>
        Ldap = 3,
    }
}
