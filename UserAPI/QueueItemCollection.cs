using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Queues object represents SMTP/Relay queue in LumiSoft Mail Server virtual server.
    /// </summary>
    public class QueueItemCollection : IEnumerable
    {
        private VirtualServer   m_pVirtualServer = null;
        private List<QueueItem> m_pCollection    = null;
        private bool            m_smtp_relay     = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <param name="smtp_relay">Specifies if smtp or relay queue collection.</param>
        internal QueueItemCollection(VirtualServer virtualServer,bool smtp_relay)
        {
            m_pVirtualServer = virtualServer;
            m_smtp_relay     = smtp_relay;
            m_pCollection    = new List<QueueItem>();

            Bind();
        }


        #region method Refresh

        /// <summary>
        /// Refreshes sessions.
        /// </summary>
        public void Refresh()
        {
            m_pCollection.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server groups and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetQueue <virtualServerID> <queueType>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetQueue
                if(m_smtp_relay){
                    m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetQueue " + m_pVirtualServer.VirtualServerID + " 1");
                }
                else{
                    m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetQueue " + m_pVirtualServer.VirtualServerID + " 0");
                }

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("Queue")){
                    foreach(DataRow dr in ds.Tables["Queue"].Rows){
                        m_pCollection.Add(new QueueItem(                        
                            Convert.ToDateTime(dr["CreateTime"]),
                            dr["Header"].ToString()
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
			return m_pCollection.GetEnumerator();
		}

		#endregion


        #region Properties Implementaion

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets number of queue itmes in collection.
        /// </summary>
        public int Count
        {
            get{ return m_pCollection.Count; }
        }

        /// <summary>
        /// Gets a QueueItem object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the QueueItem object in the QueueCollection collection.</param>
        /// <returns>A QueueItem object value that represents the queue item in virtual server.</returns>
        public QueueItem this[int index]
        {
            get{ return m_pCollection[index]; }
        }

        #endregion
    }
}
