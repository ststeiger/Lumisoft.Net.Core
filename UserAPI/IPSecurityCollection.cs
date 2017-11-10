using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Data;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The IPSecurityCollection object represents IP security entries in LumiSoft Mail Server virtual server.
    /// </summary>
    public class IPSecurityCollection : IEnumerable
    {
        private VirtualServer    m_pVirtualServer = null;
        private List<IPSecurity> m_pEntries       = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal IPSecurityCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pEntries       = new List<IPSecurity>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Adds new security enrty to collection.
        /// </summary>
        /// <param name="enabled">Specifies if security entry is enabled.</param>
        /// <param name="description">Security entry description text.</param>
        /// <param name="service">Specifies service for what security entry applies.</param>
        /// <param name="action">Specifies what action done if IP matches to security entry range.</param>
        /// <param name="startIP">Range start IP.</param>
        /// <param name="endIP">Range end IP.</param>
        /// <returns></returns>
        public IPSecurity Add(bool enabled,string description,Service_enum service,IPSecurityAction_enum action,IPAddress startIP,IPAddress endIP)
        {
            /* AddIPSecurityEntry <virtualServerID> "<securityEntryID>" enabled "<description>" <service> <action> "<startIP>" "<endIP>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP AddIPSecurityEntry
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddIPSecurityEntry " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(id) + " " + 
                enabled + " " + 
                TextUtils.QuoteString(description) + " " +
                (int)service + " " +
                (int)action + " " +
                TextUtils.QuoteString(startIP.ToString()) + " " +
                TextUtils.QuoteString(endIP.ToString())
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            IPSecurity entry = new IPSecurity(
                this,
                id,
                enabled,
                description,
                service,
                action,
                startIP,
                endIP
            );
            m_pEntries.Add(entry);

            return entry;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes and removes specified security entry from collection.
        /// </summary>
        /// <param name="entry"></param>
        public void Remove(IPSecurity entry)
        {
            /* DeleteIPSecurityEntry <virtualServerID> "<securityEntryID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteIPSecurityEntry
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteIPSecurityEntry " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(entry.ID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pEntries.Remove(entry);
        }

        #endregion

        #region method ToArray

        /// <summary>
        /// Copies collection to array.
        /// </summary>
        /// <returns></returns>
        public IPSecurity[] ToArray()
        {
            return m_pEntries.ToArray();
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes IP security entries.
        /// </summary>
        public void Refresh()
        {
            m_pEntries.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server mailing list members and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetIPSecurity <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetIPSecurity
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetIPSecurity " + m_pVirtualServer.VirtualServerID);
                            
                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("IPSecurity")){
                    foreach(DataRow dr in ds.Tables["IPSecurity"].Rows){
                        m_pEntries.Add(new IPSecurity(
                            this,
                            dr["ID"].ToString(),
                            Convert.ToBoolean(dr["Enabled"]),
                            dr["Description"].ToString(),
                            (Service_enum)Convert.ToInt32(dr["Service"]),
                            (IPSecurityAction_enum)Convert.ToInt32(dr["Action"]),
                            IPAddress.Parse(dr["StartIP"].ToString()),
                            IPAddress.Parse(dr["EndIP"].ToString())
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
			return m_pEntries.GetEnumerator();
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
        /// Gets number of IP security entries in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pEntries.Count; }
        }

        /// <summary>
        /// Gets a IPSecurity object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the IPSecurity object in the IPSecurityCollection collection.</param>
        /// <returns>A IPSecurity object value that represents the filter in virtual server.</returns>
        public IPSecurity this[int index]
        {
            get{ return m_pEntries[index]; }
        }

        /// <summary>
        /// Gets a IPSecurity object in the collection by filter ID.
        /// </summary>
        /// <param name="securityEntryID">A String value that specifies the IP security entry ID of the IPSecurity object in the IPSecurityCollection collection.</param>
        /// <returns>A IPSecurity object value that represents the filter in virtual server.</returns>
        public IPSecurity this[string securityEntryID]
        {
            get{ 
                foreach(IPSecurity securityEntry in m_pEntries){
                    if(securityEntry.ID.ToLower() == securityEntryID.ToLower()){
                        return securityEntry;
                    }
                }

                throw new Exception("IPSecurity with specified ID '" + securityEntryID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
