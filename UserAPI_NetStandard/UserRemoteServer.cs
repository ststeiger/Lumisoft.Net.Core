using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserRemoteServer object represents remote server in user.
    /// </summary>
    public class UserRemoteServer
    {
        private User                       m_pUser         = null;
        private UserRemoteServerCollection m_pOwner        = null;
        private string                     m_ID            = "";
        private string                     m_Description   = "";
        private string                     m_Host          = "";
        private int                        m_Port          = 110;
        private bool                       m_SSL           = false;
        private string                     m_UserName      = "";
        private string                     m_Password      = "";
        private bool                       m_Enabled       = true;
        private bool                       m_ValuesChanged = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ownerUser">User that owns specified remote server.</param>
        /// <param name="owner">Owner UserRemoteServerCollection collection that owns this remote server.</param>
        /// <param name="id">Remote server ID.</param>
        /// <param name="description">Remote server description.</param>
        /// <param name="host">Remote server name or IP.</param>
        /// <param name="port">Remote server port.</param>
        /// <param name="ssl">Specifies if connected to remote server via SSL.</param>
        /// <param name="userName">Remote server user name.</param>
        /// <param name="password">Remote server password.</param>
        /// <param name="enabled">Specifies if remote server is enabled.</param>
        internal UserRemoteServer(User ownerUser,UserRemoteServerCollection owner,string id,string description,string host,int port,bool ssl,string userName,string password,bool enabled)
        {
            m_pUser       = ownerUser;
            m_pOwner      = owner;
            m_ID          = id;
            m_Description = description;
            m_Host        = host;
            m_Port        = port;
            m_SSL         = ssl;
            m_UserName    = userName;
            m_Password    = password;
            m_Enabled     = enabled;
        }

        
        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't chnaged, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }

            /* UpdateUserRemoteServer <virtualServerID> "<remoteServerID>" "<userName>" "<description>" "<remoteHost>" <remoteHostPort> "<remoteHostUserName>" "<remoteHostPassword>" <ssl> <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP UpdateUserRemoteServer
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateUserRemoteServer " + 
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_ID) + " " + 
                TextUtils.QuoteString(m_pUser.UserName) + " " + 
                TextUtils.QuoteString(m_Description) + " " + 
                TextUtils.QuoteString(m_Host) + " " + 
                m_Port + " " + 
                TextUtils.QuoteString(m_UserName) + " " + 
                TextUtils.QuoteString(m_Password) + " " +
                m_SSL + " " +
                m_Enabled
            );
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets onwer UserRemoteServerCollection that owns this remote server.
        /// </summary>
        public UserRemoteServerCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets remote server id.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets or sets remote server description.
        /// </summary>
        public string Description
        {
            get{ return m_Description; }

            set{
                if(m_Description != value){
                    m_Description = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets remote server name or IP.
        /// </summary>
        public string Host
        {
            get{ return m_Host; }

            set{
                if(m_Host != value){
                    m_Host = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets remote server port.
        /// </summary>
        public int Port
        {
            get{ return m_Port; }

            set{
                if(m_Port != value){
                    m_Port = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets if connection is madet to remote server via SSL.
        /// </summary>
        public bool SSL
        {
            get{ return m_SSL; }

            set{
                if(m_SSL != value){
                    m_SSL = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets remote server user name.
        /// </summary>
        public string UserName
        {
            get{ return m_UserName; }

            set{
                if(m_UserName != value){
                    m_UserName = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets remote server password.
        /// </summary>
        public string Password
        {
            get{ return m_Password; }

            set{
                if(m_Password != value){
                    m_Password = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets if remote server is enabled.
        /// </summary>
        public bool Enabled
        {
            get{ return m_Enabled; }

            set{
                if(m_Enabled != value){
                    m_Enabled = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
