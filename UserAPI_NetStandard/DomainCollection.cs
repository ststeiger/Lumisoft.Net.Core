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
    /// The DomainCollection object represents domains in LumiSoft Mail Server virtual server.
    /// </summary>
    public class DomainCollection : IEnumerable
    {
        private VirtualServer m_pVirtualServer = null;
        private List<Domain>  m_pDomains       = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        public DomainCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pDomains       = new List<Domain>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Creates and adds new domain to domain collection.
        /// </summary>
        /// <param name="name">Domain name.</param>
        /// <param name="description">Domain description text.</param>
        public Domain Add(string name,string description)
        {
            /* AddDomain <virtualServerID> "<domainID>" "<domainName>" "<description>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddGroup
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddDomain " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(id) + " " + TextUtils.QuoteString(name) + " " + TextUtils.QuoteString(description));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            Domain domain = new Domain(this,id,name,description);
            m_pDomains.Add(domain);
            return domain;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes and removes specified domain from domain collection.
        /// </summary>
        /// <param name="domain">Domain to delete.</param>
        public void Remove(Domain domain)
        {
            /* DeleteDomain <virtualServerID> "<domainID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteDomain
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteDomain " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(domain.DomainID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pDomains.Remove(domain);
        }

        #endregion

        #region method Contains

        /// <summary>
        /// Check if collection contains domain with specified name.
        /// </summary>
        /// <param name="domainName">Domain name.</param>
        /// <returns></returns>
        public bool Contains(string domainName)
        {
            foreach(Domain domain in m_pDomains){
                if(domain.DomainName.ToLower() == domainName.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes domains.
        /// </summary>
        public void Refresh()
        {
            m_pDomains.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server domains and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetDomains <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetDomains
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetDomains " + m_pVirtualServer.VirtualServerID);
                            
                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("Domains")){
                    foreach(DataRow dr in ds.Tables["Domains"].Rows){
                        m_pDomains.Add(new Domain(
                            this,
                            dr["DomainID"].ToString(),
                            dr["DomainName"].ToString(),
                            dr["Description"].ToString()
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
			return m_pDomains.GetEnumerator();
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets number of domains in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pDomains.Count; }
        }

        /// <summary>
        /// Gets a Domain object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the Domain object in the DomainCollection collection.</param>
        /// <returns>A Domain object value that represents the virtual server domain.</returns>
        public Domain this[int index]
        {
            get{ return m_pDomains[index]; }
        }

        /// <summary>
        /// Gets a Domain object in the collection by domain ID.
        /// </summary>
        /// <param name="domainID">A String value that specifies the domain ID of the Domain object in the DomainCollection collection.</param>
        /// <returns>A Domain object value that represents the virtual server domain.</returns>
        public Domain this[string domainID]
        {
            get{ 
                foreach(Domain domain in m_pDomains){
                    if(domain.DomainID.ToLower() == domainID.ToLower()){
                        return domain;
                    }
                }

                throw new Exception("Domain with specified ID '" + domainID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
