using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Specified when next rule is checked.
    /// </summary>
    public enum GlobalMessageRule_CheckNextRule_enum
    {
        /// <summary>
        /// Next rules is always checked.
        /// </summary>
        Always = 0,

        /// <summary>
        /// Next rule is checked if last rule matches.
        /// </summary>
        IfMatches = 1,

        /// <summary>
        /// Next rule is checked is last rule don't match.
        /// </summary>
        IfNotMatches = 2,
    }
}
