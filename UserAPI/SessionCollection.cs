using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The SessionCollection object represents sessions in LumiSoft Mail Server server.
    /// </summary>
    public class SessionCollection : IEnumerable
    {
        private Server        m_pOwner    = null;
        private List<Session> m_pSessions = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner Server object that owns this collection.</param>
        internal SessionCollection(Server owner)
        {
            m_pOwner    = owner;
            m_pSessions = new List<Session>();

            Bind();
        }


        #region method Refresh

        /// <summary>
        /// Refreshes sessions.
        /// </summary>
        public void Refresh()
        {
            lock(m_pOwner.LockSynchronizer){
                m_pSessions.Clear();
                Bind();
            }
        }

        #endregion

        #region method ConatainsID

        /// <summary>
        /// Gets if collection contains session with specified ID.
        /// </summary>
        /// <param name="sessionID">Session ID.</param>
        /// <returns></returns>
        public bool ConatainsID(string sessionID)
        {
            foreach(Session session in m_pSessions){
                if(session.ID == sessionID){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method GetSessionByID

        /// <summary>
        /// Gets a Session object in the collection by session ID.
        /// </summary>
        /// <returns></returns>
        public Session GetSessionByID(string sessionID)
        {
            foreach(Session session in m_pSessions){
                if(session.ID == sessionID){
                    return session;
                }
            }

            throw new Exception("Session with specified session ID '" + sessionID + "' doesn't exist !");
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server events and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetSessions
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            lock(m_pOwner.LockSynchronizer){
                // Call TCP GetEvents
                m_pOwner.TcpClient.TcpStream.WriteLine("GetSessions");

                string response = m_pOwner.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pOwner.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);
                
                if(ds.Tables.Contains("Sessions")){
                    foreach(DataRow dr in ds.Tables["Sessions"].Rows){
                        m_pSessions.Add(new Session(
                            this,
                            dr["SessionID"].ToString(),
                            dr["SessionType"].ToString(),
                            Convert.ToDateTime(dr["SessionStartTime"]),
                            Convert.ToInt32(dr["ExpectedTimeout"]),
                            dr["UserName"].ToString(),
                            dr["LocalEndPoint"].ToString(),
                            dr["RemoteEndPoint"].ToString(),
                            Convert.ToInt32(dr["ReadTransferRate"]),
                            Convert.ToInt32(dr["WriteTransferRate"]),
                            dr["SessionLog"].ToString()
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
			return m_pSessions.GetEnumerator();
		}

		#endregion


        #region Properties Implementation
        
        /// <summary>
        /// Gets the Server object that is the owner of this collection.
        /// </summary>
        public Server Server
        {
            get{ return m_pOwner; }
        }
      
        /// <summary>
        /// Gets number of sessions in server.
        /// </summary>
        public int Count
        {
            get{ return m_pSessions.Count; }
        }

        /// <summary>
        /// Gets a Session object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the Session object in the SessionCollection collection.</param>
        /// <returns>A Session object value that represents the session in server.</returns>
        public Session this[int index]
        {
            get{ return m_pSessions[index]; }
        }

        /// <summary>
        /// Gets a Session object in the collection by filter ID.
        /// </summary>
        /// <param name="sessionID">A String value that specifies the session ID of the Session object in the SessionCollection collection.</param>
        /// <returns>A Session object value that represents the session in server.</returns>
        public Session this[string sessionID]
        {
            get{ 
                foreach(Session session in m_pSessions){
                    if(sessionID.ToLower() == sessionID.ToLower()){
                        return session;
                    }
                }

                throw new Exception("Session with specified ID '" + sessionID + "' doesn't exist !"); 
            }
        }


        /// <summary>
        /// Gets direct access to sessions collection.
        /// </summary>
        internal List<Session> List
        {
            get{ return m_pSessions; }
        }

        #endregion

    }
}
