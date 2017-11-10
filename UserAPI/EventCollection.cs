using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The EventCollection object represents events in LumiSoft Mail Server server.
    /// </summary>
    public class EventCollection : IEnumerable
    {
        private Server      m_pOwner  = null;
        private List<Event> m_pEvents = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner Server object that owns this collection.</param>
        internal EventCollection(Server owner)
        {
            m_pOwner  = owner;
            m_pEvents = new List<Event>();

            Bind();
        }


        #region method Bind

        /// <summary>
        /// Gets server events and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetEvents
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pOwner){
                // Call TCP GetEvents
                m_pOwner.TcpClient.TcpStream.WriteLine("GetEvents");

                string response = m_pOwner.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pOwner.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("Events")){
                    foreach(DataRow dr in ds.Tables["Events"].Rows){
                        m_pEvents.Add(new Event(
                            dr["ID"].ToString(),
                            (EventType_enum)Convert.ToInt32(dr["Type"]),
                            dr["VirtualServer"].ToString(),
                            Convert.ToDateTime(dr["CreateDate"]),
                            dr["Text"].ToString(),
                            dr["Text"].ToString()
                        ));
                    }
                }
            }
        }

        #endregion


        #region method Refresh

        /// <summary>
        /// Refreshes sessions.
        /// </summary>
        public void Refresh()
        {
            m_pEvents.Clear();
            Bind();
        }

        #endregion

        #region method Clear

        /// <summary>
        /// Deletes all events.
        /// </summary>
        public void Clear()
        {
            /* ClearEvents
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            // Call TCP GetEvents
            m_pOwner.TcpClient.TcpStream.WriteLine("ClearEvents");

            string response = m_pOwner.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pEvents.Clear();
        }

        #endregion


        #region interface IEnumerator

		/// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pEvents.GetEnumerator();
		}

		#endregion


        #region Properties Implementation
                
        /// <summary>
        /// Gets number of events in server.
        /// </summary>
        public int Count
        {
            get{ return m_pEvents.Count; }
        }

        /// <summary>
        /// Gets a Event object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the Event object in the EventCollection collection.</param>
        /// <returns>A Event object value that represents the event in server.</returns>
        public Event this[int index]
        {
            get{ return m_pEvents[index]; }
        }

        /// <summary>
        /// Gets a Event object in the collection by filter ID.
        /// </summary>
        /// <param name="eventID">A String value that specifies the event ID of the Event object in the EventCollection collection.</param>
        /// <returns>A Event object value that represents the event in server.</returns>
        public Event this[string eventID]
        {
            get{ 
                foreach(Event e in m_pEvents){
                    if(e.ID.ToLower() == eventID.ToLower()){
                        return e;
                    }
                }

                throw new Exception("Event with specified ID '" + eventID + "' doesn't exist !"); 
            }
        }

        #endregion

    }
}
