using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Thsi class represents SIP Calls collection in virtual server.
    /// </summary>
    public class SIP_CallCollection : IEnumerable
    {
        private VirtualServer  m_pVirtualServer = null;
        private List<SIP_Call> m_pCalls         = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal SIP_CallCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pCalls         = new List<SIP_Call>();

            Bind();
        }


        #region method Refresh

        /// <summary>
        /// Refreshes collection.
        /// </summary>
        public void Refresh()
        {
            m_pCalls.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Binds SIP calls to this collection.
        /// </summary>
        private void Bind()
        {
            /* GetSipCalls <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */
           
            lock(m_pVirtualServer.Server.LockSynchronizer){
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetSipCalls " + m_pVirtualServer.VirtualServerID);
                            
                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("SipCalls")){
                    foreach(DataRow dr in ds.Tables["SipCalls"].Rows){
                        m_pCalls.Add(new SIP_Call(
                            this,
                            dr["CallID"].ToString(),
                            dr["Caller"].ToString(),
                            dr["Callee"].ToString(),
                            Convert.ToDateTime(dr["StartTime"])
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
			return m_pCalls.GetEnumerator();
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
        /// Gets number of items in the collection
        /// </summary>
        public int Count
        {
            get{ return m_pCalls.Count; }
        }

        #endregion

    }
}
