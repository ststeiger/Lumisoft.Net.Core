using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LumiSoft.Net
{
    /// <summary>
    /// This class specifies TCP client based connection security.
    /// </summary>
    public enum TcpClientSecurity
    {
        /// <summary>
        /// No security(connection not encrypted).
        /// </summary>
        None = 0,

        /// <summary>
        /// Use SSL for connection security.
        /// </summary>
        SSL = 1,

        /// <summary>
        /// Use TLS for connection security.
        /// </summary>
        TLS = 2,

        /// <summary>
        /// Use TLS for connection security, if remote server supports it.
        /// </summary>
        UseTlsIfSupported = 3,
    }
}
