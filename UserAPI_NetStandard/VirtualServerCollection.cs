using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;
using LumiSoft.Net.IO;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The VirtualServerCollection object represents LumiSoft Mail Server virtual servers.
    /// </summary>
    public class VirtualServerCollection : IEnumerable
    {
        private Server              m_pParent         = null;
        private List<VirtualServer> m_pVirtualServers = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="parent">Server object that is the parent of this object.</param>
        internal VirtualServerCollection(Server parent)
        {
            m_pParent = parent;
            m_pVirtualServers = new List<VirtualServer>();

            Bind();
        }


        #region method GetVirtualServerAPIs

        /// <summary>
        /// Gets available virtual server API infos.
        /// </summary>
        /// <returns></returns>
        public DataSet GetVirtualServerAPIs()
        {
            /* GetVirtualServerAPIs Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pParent){
                // Call TCP GetVirtualServerAPIs
                m_pParent.TcpClient.TcpStream.WriteLine("GetVirtualServerAPIs");
                                
                string response = m_pParent.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pParent.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
            
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                return ds;
            }
        }

        #endregion

        #region method Add

        /// <summary>
        /// Adds new virtual server to virtual server collection.
        /// </summary>
        /// <param name="enabled">Specified if virtual server is enabled.</param>
        /// <param name="name">Virtual server name.</param>
        /// <param name="assembly">Assembly name what contains virtual server API.</param>
        /// <param name="type">Full virtual server API type name.</param>
        /// <param name="initString">Virtual server API init string. This value depends on API implementation.</param>
        /// <returns>Returns new virtual server.</returns>
        public VirtualServer Add(bool enabled,string name,string assembly,string type,string initString)
        {
            /* AddVirtualServer <ID> enabled "<name>" "<assembly>" "<type>" "<initString>:base64"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddVirtualServer
            m_pParent.TcpClient.TcpStream.WriteLine("AddVirtualServer " + 
                id + " " +
                enabled + " " +
                TextUtils.QuoteString(name) + " " +
                TextUtils.QuoteString(assembly) + " " +
                TextUtils.QuoteString(type) + " " +
                TextUtils.QuoteString(Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(initString)))
            );
                        
            string response = m_pParent.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            VirtualServer retVal = new VirtualServer(                
                m_pParent,
                this,
                id,
                enabled,
                name,
                assembly,
                type,
                initString
            );

            m_pVirtualServers.Add(retVal);

            return retVal;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified virtual server from mail server.
        /// </summary>
        /// <param name="server">Virtual server to remove.</param>
        public void Remove(VirtualServer server)
        {
            /* DeleteVirtualServer <virtualServerID>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            // Call TCP DeleteVirtualServer
            m_pParent.TcpClient.TcpStream.WriteLine("DeleteVirtualServer " + server.VirtualServerID);
                        
            string response = m_pParent.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }
                        
            m_pVirtualServers.Remove(server);
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server virtual servers and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {            
            /* GetVirtualServers Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pParent){
                // Call TCP GetVirtualServers
                m_pParent.TcpClient.TcpStream.WriteLine("GetVirtualServers");

                string response = m_pParent.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pParent.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("Servers")){
                    foreach(DataRow dr in ds.Tables["Servers"].Rows){
                        m_pVirtualServers.Add(new VirtualServer(
                            m_pParent,
                            this,
                            dr["ID"].ToString(),
                            ConvertEx.ToBoolean(dr["Enabled"],true),
                            dr["Name"].ToString(),
                            dr["API_assembly"].ToString(),
                            dr["API_class"].ToString(),
                            dr["API_initstring"].ToString()
                        ));
                    }
                }
            }
        }

        #endregion


        #region interface IEnumerator

		/// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pVirtualServers.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the Server object that is the owner of this collection.
        /// </summary>
        public Server Server
        {
            get{ return m_pParent; }
        }

        /// <summary>
        /// Gets number of virtual servers in server.
        /// </summary>
        public int Count
        {
            get{ return m_pVirtualServers.Count; }
        }

        /// <summary>
        /// Gets a VirtualServer object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the VirtualServer object in the VirtualServerCollection collection.</param>
        /// <returns>A VirtualServer object value that represents the virtual server.</returns>
        public VirtualServer this[int index]
        {
            get{ return m_pVirtualServers[index]; }
        }

        /// <summary>
        /// Gets a VirtualServer object in the collection by virtual server ID.
        /// </summary>
        /// <param name="virtualServerID">A String value that specifies the virtual server ID of the VirtualServer object in the VirtualServerCollection collection.</param>
        /// <returns>A VirtualServer object value that represents the virtual server.</returns>
        public VirtualServer this[string virtualServerID]
        {
            get{ 
                foreach(VirtualServer virtualServer in m_pVirtualServers){
                    if(virtualServer.VirtualServerID.ToLower() == virtualServerID.ToLower()){
                        return virtualServer;
                    }
                }

                throw new Exception("Virtual server with specified ID '" + virtualServerID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
