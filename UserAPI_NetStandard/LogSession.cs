using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Data;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The LogSession object represents user session log in LumiSoft Mail Server virtual server.
    /// </summary>
    public class LogSession
    {
        private VirtualServer m_pVirtualServer  = null;
        private string        m_Service         = "";
        private string        m_SessionID       = "";
        private DateTime      m_StartTime;
        private IPEndPoint    m_pRemoteEndPoint = null;
        private string        m_UserName        = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Reference to owner virtual server.</param>
        /// <param name="service">Service. SMTP,POP3,IMAP,RELAY,FETCH.</param>
        /// <param name="sessionID">Session ID.</param>
        /// <param name="startTime">Session start time.</param>
        /// <param name="remoteEndPoint">Remote end point.</param>
        /// <param name="userName">Authenticated user name.</param>
        internal LogSession(VirtualServer virtualServer,string service,string sessionID,DateTime startTime,IPEndPoint remoteEndPoint,string userName)
        {
            m_pVirtualServer  = virtualServer;
            m_Service         = service;
            m_SessionID       = sessionID;
            m_StartTime       = startTime;
            m_pRemoteEndPoint = remoteEndPoint;
            m_UserName        = userName;
        }


        #region method GetLogText

        /// <summary>
        /// Gets log text (TAB delimited log fields).
        /// </summary>
        /// <returns></returns>
        private string GetLogText()
        {
            /* GetSessionLog <virtualServerID> <service> "<sessionID>" "<sessionStartDate>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            lock(m_pVirtualServer.Server){
                // Call TCP GetSessionLog
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetSessionLog " + 
                    m_pVirtualServer.VirtualServerID + " " +
                    m_Service + " " +
                    TextUtils.QuoteString(m_SessionID) + " " +
                    TextUtils.QuoteString(m_StartTime.ToUniversalTime().ToString("u"))
                );

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                 DataSet ds = Utils.DecompressDataSet(ms);
                        
                if(ds.Tables.Contains("SessionLog")){
                    if(ds.Tables["SessionLog"].Rows.Count > 0){
                        return ds.Tables["SessionLog"].Rows[0]["LogText"].ToString();
                    }
                }

                return "";
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets session ID.
        /// </summary>
        public string SessionID
        {
            get{ return m_SessionID; }
        }

        /// <summary>
        /// Gets session start time.
        /// </summary>
        public DateTime StartTime
        {
            get{ return m_StartTime; }
        }

        /// <summary>
        /// Gets remote end point.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get{ return m_pRemoteEndPoint; }
        }

        /// <summary>
        /// Gets session authenticated user name.
        /// </summary>
        public string UserName
        {
            get{ return m_UserName; }
        }

        /// <summary>
        /// Gets log text (TAB delimited log fields).
        /// </summary>
        public string LogText
        {
            get{ return GetLogText(); }
        }

        #endregion

    }
}
