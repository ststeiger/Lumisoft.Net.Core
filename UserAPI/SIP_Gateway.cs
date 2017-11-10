using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The SIP_Gateway object represents SIP Gateway in LumiSoft Mail Server virtual server.
    /// </summary>
    public class SIP_Gateway
    {
        private SIP_GatewayCollection m_pCollection = null;
        private string                m_UriScheme   = "";
        private string                m_Transport   = SIP_Transport.UDP;
        private string                m_Host        = "";
        private int                   m_Port        = 5060;
        private string                m_Realm       = "";
        private string                m_UserName    = "";
        private string                m_Password    = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner collection.</param>
        /// <param name="uriScheme">URI scheme.</param>
        /// <param name="transport">Transport to use.</param>
        /// <param name="host">Remote gateway host name or IP address.</param>
        /// <param name="port">Remote gateway port.</param>
        /// <param name="realm">Remote gateway realm.</param>
        /// <param name="userName">Remote gateway user name.</param>
        /// <param name="password">Remote gateway password.</param>
        internal SIP_Gateway(SIP_GatewayCollection owner,string uriScheme,string transport,string host,int port,string realm,string userName,string password)
        {
            m_pCollection = owner;
            m_UriScheme   = uriScheme;
            m_Transport   = transport;
            m_Host        = host;
            m_Port        = port;
            m_Realm       = realm;
            m_UserName    = userName;
            m_Password    = password;
        }


        #region method Remove

        /// <summary>
        /// Removes this item for the collection.
        /// </summary>
        public void Remove()
        {
            m_pCollection.Remove(this);
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets URI scheme.
        /// </summary>
        public string UriScheme
        {
            get{ return m_UriScheme; }

            set{
                if(string.IsNullOrEmpty(value)){
                    throw new ArgumentException("Value cant be null or empty !");
                }

                if(m_UriScheme != value){
                    m_UriScheme = value;
                    m_pCollection.Owner.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets transport.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value passed.</exception>
        public string Transport
        {
            get{ return m_Transport; }

            set{
                if(string.IsNullOrEmpty(value)){
                    throw new ArgumentException("Value cant be null or empty !");
                }

                if(m_Transport != value){
                    m_Transport = value;
                    m_pCollection.Owner.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets remote gateway host name or IP address.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value passed.</exception>
        public string Host
        {
            get{ return m_Host; }

            set{
                if(string.IsNullOrEmpty(value)){
                    throw new ArgumentException("Value cant be null or empty !");
                }

                if(m_Host != value){
                    m_Host = value;
                    m_pCollection.Owner.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets remote gateway port.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value passed.</exception>
        public int Port
        {
            get{ return m_Port; }
            
            set{
                if(value < 1){
                    throw new ArgumentException("Value must be >= 1 !");
                }

                if(m_Port != value){
                    m_Port = value;
                    m_pCollection.Owner.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets remote gateway realm(domain).
        /// </summary>
        public string Realm
        {
            get{ return m_Realm; }

            set{
                if(value == null){
                    m_Realm = "";
                }

                if(m_Realm != value){
                    m_Realm = value;
                    m_pCollection.Owner.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets remote gateway user name.
        /// </summary>
        public string UserName
        {
            get{ return m_UserName; }

            set{
                if(value == null){
                    m_UserName = "";
                }

                if(m_UserName != value){
                    m_UserName = value;
                    m_pCollection.Owner.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets remote gateway password.
        /// </summary>
        public string Password
        {
            get{ return m_Password; }

            set{
                if(value == null){
                    m_Password = "";
                }

                if(m_Password != value){
                    m_Password = value;
                    m_pCollection.Owner.SetValuesChanged();
                }
            }
        }

        #endregion

    }
}
