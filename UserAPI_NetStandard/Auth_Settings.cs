using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Auth_Settings object represents Authentication settings in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Auth_Settings
    {
        private System_Settings               m_pSysSettings = null;
        private ServerAuthenticationType_enum m_AuthType     = ServerAuthenticationType_enum.Integrated;
        private string                        m_WinDomain    = "";
        private string                        m_LdapServer   = "";
        private string                        m_LdapDn       = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sysSettings">Reference to system settings.</param>
        /// <param name="authType">Specifies authentication type.</param>
        /// <param name="winDomain">Windows domain.</param>
        /// <param name="ldapServer">LDAP server.</param>
        /// <param name="ldapDN">LDAP base DN value.</param>
        internal Auth_Settings(System_Settings sysSettings,ServerAuthenticationType_enum authType,string winDomain,string ldapServer,string ldapDN)
        {
            m_pSysSettings = sysSettings;
            m_AuthType     = authType;
            m_WinDomain    = winDomain;
            m_LdapServer   = ldapServer;
            m_LdapDn       = ldapDN;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets or sets authentication type.
        /// </summary>
        public ServerAuthenticationType_enum AuthenticationType
        {
            get{ return m_AuthType; }

            set{
                if(m_AuthType != value){
                    m_AuthType = value; 

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or set windows domain to against what to do authentication. 
        /// This property is used only if ServerAuthenticationType_enum.Windows. 
        /// Value "" means that local computer is used.
        /// </summary>
        public string WinDomain
        {
            get{ return m_WinDomain; }

            set{                 
                if(m_WinDomain != value){
                    m_WinDomain = value; 

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or set LDAP server to use for authentication. 
        /// This property is used only if ServerAuthenticationType_enum.Ldap. 
        /// Value "" means that local computer is used.
        /// </summary>
        public string LdapServer
        {
            get{ return m_LdapServer; }

            set{                 
                if(m_LdapServer != value){
                    m_LdapServer = value; 

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or set LDAP server base DN value. 
        /// This property is used only if ServerAuthenticationType_enum.Ldap. 
        /// </summary>
        public string LdapDn
        {
            get{ return m_LdapDn; }

            set{                 
                if(m_LdapDn != value){
                    m_LdapDn = value; 

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        #endregion

    }
}
