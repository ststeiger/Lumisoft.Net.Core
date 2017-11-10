using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserRemoteServerCollection object represents remote servers in user.
    /// </summary>
    public class UserRemoteServerCollection : IEnumerable
    {
        private User                   m_pUser    = null;
        private List<UserRemoteServer> m_pServers = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="user">Owner user.</param>
        internal UserRemoteServerCollection(User user)
        {
            m_pUser    = user;
            m_pServers = new List<UserRemoteServer>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds new remote server for owner user.
        /// </summary>
        /// <param name="description">Remote server description.</param>
        /// <param name="host">Remote server host name or IP.</param>
        /// <param name="port">Remote server port.</param>
        /// <param name="ssl">Specifies if connected to remote server via SSL.</param>
        /// <param name="userName">Remote server user name.</param>
        /// <param name="password">Remote server password.</param>
        /// <param name="enabled">Specifies if remote server is enabled.</param>
        /// <returns></returns>
        public UserRemoteServer Add(string description,string host,int port,bool ssl,string userName,string password,bool enabled)
        {
            /* AddUserRemoteServer <virtualServerID> "<remoteServerID>" "<userName>" "<description>" "<remoteHost>" <remoteHostPort> "<remoteHostUserName>" "<remoteHostPassword>" <ssl> <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddUserRemoteServer
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("AddUserRemoteServer " + 
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(id) + " " + 
                TextUtils.QuoteString(m_pUser.UserName) + " " + 
                TextUtils.QuoteString(description) + " " + 
                TextUtils.QuoteString(host) + " " + 
                port + " " + 
                TextUtils.QuoteString(userName) + " " + 
                TextUtils.QuoteString(password) + " " +
                ssl + " " +
                enabled
            );
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            UserRemoteServer server = new UserRemoteServer(m_pUser,this,id,description,host,port,ssl,userName,password,enabled);
            m_pServers.Add(server);
            return server;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified remote server.
        /// </summary>
        /// <param name="remoteServer">Remote server to remove.</param>
        public void Remove(UserRemoteServer remoteServer)
        {
            /* DeleteUserRemoteServer <virtualServerID> "<remoteServerID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteUserRemoteServer
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUserRemoteServer " + m_pUser.VirtualServer.VirtualServerID + " " + TextUtils.QuoteString(remoteServer.ID));
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pServers.Remove(remoteServer);
        }

        #endregion

        #region method ToArray

        /// <summary>
        /// Copies collection to array.
        /// </summary>
        /// <returns></returns>
        public UserRemoteServer[] ToArray()
        {
            return m_pServers.ToArray();
        }

        #endregion


        #region interface IEnumerator

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pServers.GetEnumerator();
		}

		#endregion


        #region method Bind

        /// <summary>
        /// Gets user remote servers and binds them to this.
        /// </summary>
        private void Bind()
        {            
            /* GetUserRemoteServers <virtualServerID> <userID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pUser.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetUserRemoteServers
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserRemoteServers " + m_pUser.VirtualServer.VirtualServerID + " " + m_pUser.UserID);

                string response = m_pUser.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
               
                if(ds.Tables.Contains("UserRemoteServers")){
                    foreach(DataRow dr in ds.Tables["UserRemoteServers"].Rows){
                        m_pServers.Add( new UserRemoteServer(
                            m_pUser,
                            this,
                            dr["ServerID"].ToString(),
                            dr["Description"].ToString(),
                            dr["RemoteServer"].ToString(),
                            Convert.ToInt32(dr["RemotePort"]),
                            Convert.ToBoolean(dr["UseSSL"]),
                            dr["RemoteUserName"].ToString(),
                            dr["RemotePassword"].ToString(),
                            Convert.ToBoolean(dr["Enabled"])
                        ));
                    }
                }
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets number of remote servers on that user.
        /// </summary>
        public int Count
        {
            get{ return m_pServers.Count; }
        }
        
        /// <summary>
        /// Gets a UserRemoteServer object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the UserRemoteServer object in the UserRemoteServerCollection collection.</param>
        /// <returns>A UserRemoteServer object value that represents the user remote server in virtual server.</returns>
        public UserRemoteServer this[int index]
        {
            get{ return m_pServers[index]; }
        }

        /// <summary>
        /// Gets remote server with specified ID.
        /// </summary>
        /// <param name="remoteServerID">A String value that specifies the remote server ID of the UserRemoteServer object in the UserRemoteServerCollection collection.</param>
        /// <returns>A UserRemoteServer object value that represents the user remote server in virtual server.</returns>
        public UserRemoteServer this[string remoteServerID]
        {
            get{   
                foreach(UserRemoteServer server in m_pServers){
                    if(server.ID.ToLower() == remoteServerID.ToLower()){
                        return server;
                    }
                }

                throw new Exception("Remote server '" + remoteServerID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
