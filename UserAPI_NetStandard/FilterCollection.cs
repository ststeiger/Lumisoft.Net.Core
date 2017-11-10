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
    /// The FilterCollection object represents filters in LumiSoft Mail Server virtual server.
    /// </summary>
    public class FilterCollection : IEnumerable
    {
        private VirtualServer m_pVirtualServer = null;
        private List<Filter>  m_pFilters       = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal FilterCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pFilters       = new List<Filter>();

            Bind();
        }


        #region method GetFilterTypes

        /// <summary>
        /// Gets available filters info.
        /// </summary>
        /// <returns></returns>
        public DataSet GetFilterTypes()
        {
            /* GetFilterTypes 
                Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */

            lock(m_pVirtualServer.Server){
                // Call TCP GetFilterTypes
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetFilterTypes " + m_pVirtualServer.VirtualServerID);

                string response =  m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                return ds;
            }
        }

        #endregion

        #region method Add

        /// <summary>
        /// Adds new filter to virtual server.
        /// </summary>
        /// <param name="enabled">Specifies if filter is enabled.</param>
        /// <param name="description">Filter description.</param>
        /// <param name="assembly">Filter assembly.</param>
        /// <param name="filterClass">Filter class.</param>
        /// <returns></returns>
        public Filter Add(bool enabled,string description,string assembly,string filterClass)
        {
            /* AddFilter <virtualServerID> "<filterID>" <cost> "<description>" "<assembly>" "<filterClass>" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id   = Guid.NewGuid().ToString();
            long   cost = DateTime.Now.Ticks;
                       
            // Call TCP AddFilter
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddFilter " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(id) + " " + 
                cost + " " +
                TextUtils.QuoteString(description) + " " + 
                TextUtils.QuoteString(assembly) + " " + 
                TextUtils.QuoteString(filterClass) + " " + 
                enabled
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            Filter filter = new Filter(m_pVirtualServer,this,id,cost,enabled,description,assembly,filterClass);
            m_pFilters.Add(filter);
            return filter;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes specified filter from virtual server.
        /// </summary>
        /// <param name="filter">Filter to remove.</param>
        public void Remove(Filter filter)
        {
            /* DeleteFilter <virtualServerID> "<filterID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteFilter
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteFilter " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(filter.ID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pFilters.Remove(filter);
        }

        #endregion

        #region method ToArray

        /// <summary>
        /// Copies collection to array.
        /// </summary>
        /// <returns></returns>
        public Filter[] ToArray()
        {
            return m_pFilters.ToArray();
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes filters.
        /// </summary>
        public void Refresh()
        {
            m_pFilters.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server filters and binds them to this, if not binded already.
        /// </summary>
        private void Bind()
        {
            /* GetFilters <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetFilters
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetFilters " + m_pVirtualServer.VirtualServerID);
                            
                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("SmtpFilters")){
                    foreach(DataRow dr in ds.Tables["SmtpFilters"].Rows){
                        m_pFilters.Add(new Filter(
                            m_pVirtualServer,
                            this,
                            dr["FilterID"].ToString(),
                            Convert.ToInt64(dr["Cost"]),
                            Convert.ToBoolean(dr["Enabled"].ToString()),
                            dr["Description"].ToString(),
                            dr["Assembly"].ToString(),
                            dr["ClassName"].ToString()
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
			return m_pFilters.GetEnumerator();
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
        /// Gets number of filters in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pFilters.Count; }
        }

        /// <summary>
        /// Gets a Filter object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the Filter object in the FilterCollection collection.</param>
        /// <returns>A Filter object value that represents the filter in virtual server.</returns>
        public Filter this[int index]
        {
            get{ return m_pFilters[index]; }
        }

        /// <summary>
        /// Gets a Filter object in the collection by filter ID.
        /// </summary>
        /// <param name="filterID">A String value that specifies the filter ID of the Filter object in the FilterCollection collection.</param>
        /// <returns>A Filter object value that represents the filter in virtual server.</returns>
        public Filter this[string filterID]
        {
            get{ 
                foreach(Filter filter in m_pFilters){
                    if(filter.ID.ToLower() == filterID.ToLower()){
                        return filter;
                    }
                }

                throw new Exception("Filter with specified ID '" + filterID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
