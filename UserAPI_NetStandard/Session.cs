using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Session object represents sessions(SMTP,POP3,...) event in LumiSoft Mail Server server.
    /// </summary>
    public class Session
    {
        private SessionCollection m_pOwner         = null;
        private string            m_ID             = "";
        private string            m_Type           = "";
        private DateTime          m_SartTime;
        private int               m_TimeoutSeconds = 0;
        private string            m_UserName       = "";
        private string            m_LocalEndPoint  = "";
        private string            m_RemoteEndPoint = "";
        private int               m_ReadKbSec      = 0;
        private int               m_WriteKbSec     = 0;
        private string            m_SessionLog     = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner SessionCollection collection that owns this object.</param>
        /// <param name="id">Session ID.</param>
        /// <param name="type">Session type.</param>
        /// <param name="startTime">Session start time.</param>
        /// <param name="timeoutSec">Session timeout seconds after no activity.</param>
        /// <param name="userName">Session authenticated user.</param>
        /// <param name="localEndPoint">Session local end point.</param>
        /// <param name="remoteEndPoint">Session remote end point.</param>sessionLog
        /// <param name="readKbSec">Session read KB in second.</param>
        /// <param name="writeKbSec">Session write KB in second.</param>
        /// <param name="sessionLog">Session log part.</param>
        internal Session(SessionCollection owner,string id,string type,DateTime startTime,int timeoutSec,string userName,string localEndPoint,string remoteEndPoint,int readKbSec,int writeKbSec,string sessionLog)
        {
            m_pOwner         = owner;
            m_ID             = id;
            m_Type           = type;
            m_SartTime       = startTime;
            m_TimeoutSeconds = timeoutSec;
            m_UserName       = userName;
            m_LocalEndPoint  = localEndPoint;
            m_RemoteEndPoint = remoteEndPoint;
            m_ReadKbSec      = readKbSec;
            m_WriteKbSec     = writeKbSec;
            m_SessionLog     = sessionLog;
        }


        #region method Kill

        /// <summary>
        /// Kills specified session.
        /// </summary>
        public void Kill()
        {
            /* KillSession "<sessionID>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            // Call TCP KillSession
            m_pOwner.Server.TcpClient.TcpStream.WriteLine("KillSession " + TextUtils.QuoteString(m_ID));

            string response = m_pOwner.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pOwner.List.Remove(this);
        }

        #endregion


        #region Proeprties Implementation

        /// <summary>
        /// Gets session ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets session type (SMTP,POP3,...).
        /// </summary>
        public string Type
        {
            get{ return m_Type; }
        }

        /// <summary>
        /// Gets session start time.
        /// </summary>
        public DateTime SartTime
        {
            get{ return m_SartTime; }
        }

        /// <summary>
        /// Gets after how many seconds session will time out if no activity.
        /// </summary>
        public int IdleTimeOutSeconds
        {
            get{ return m_TimeoutSeconds; }
        }

        /// <summary>
        /// Gets session authenticated user name.
        /// </summary>
        public string UserName
        {
            get{ return m_UserName; }
        }

        /// <summary>
        /// Gets session local endpoint.
        /// </summary>
        public string LocalEndPoint
        {
            get{ return m_LocalEndPoint; }
        }

        /// <summary>
        /// Gets session remote endpoint.
        /// </summary>
        public string RemoteEndPoint
        {
            get{ return m_RemoteEndPoint; }
        }

        /// <summary>
        /// Gets session read KB in second.
        /// </summary>
        public int ReadKbInSecond
        {
            get{ return m_ReadKbSec; }
        }

        /// <summary>
        /// Gets session write KB in second.
        /// </summary>
        public int WriteKbInSecond
        {
            get{ return m_WriteKbSec; }
        }

        /// <summary>
        /// Gets session active log part.
        /// </summary>
        public string SessionLog
        {
            get{ return m_SessionLog; }
        }

        #endregion

    }
}
