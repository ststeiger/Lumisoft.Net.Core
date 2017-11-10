using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Provides virtual server logging related methods.
    /// </summary>
    public class Logs
    {
        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal Logs(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
        }


        #region method GetSmtpLogSessions

        /// <summary>
        /// Gets SMTP sessions logs.
        /// </summary>
        /// <param name="limit">Specifies maximum number of log entries to return.</param>
        /// <param name="date">Date what logs to get.</param>
        /// <param name="startTime">Start time filter.</param>
        /// <param name="endTime">End time filter.</param>
        /// <param name="containsText">Log record text filter. Pass null or "" if no text filter.</param>
        public LogSession[] GetSmtpLogSessions(int limit,DateTime date,DateTime startTime,DateTime endTime,string containsText)
        {
            return GetLogSessions("SMTP",limit,date,startTime,endTime,containsText);
        }

        #endregion

        #region method GetPop3LogSessions

        /// <summary>
        /// Gets POP3 sessions logs.
        /// </summary>
        /// <param name="limit">Specifies maximum number of log entries to return.</param>
        /// <param name="date">Date what logs to get.</param>
        /// <param name="startTime">Start time filter.</param>
        /// <param name="endTime">End time filter.</param>
        /// <param name="containsText">Log record text filter. Pass null or "" if no text filter.</param>
        public LogSession[] GetPop3LogSessions(int limit,DateTime date,DateTime startTime,DateTime endTime,string containsText)
        {
            return GetLogSessions("POP3",limit,date,startTime,endTime,containsText);
        }

        #endregion

        #region method GetImapLogSessions

        /// <summary>
        /// Gets IMAP sessions logs.
        /// </summary>
        /// <param name="limit">Specifies maximum number of log entries to return.</param>
        /// <param name="date">Date what logs to get.</param>
        /// <param name="startTime">Start time filter.</param>
        /// <param name="endTime">End time filter.</param>
        /// <param name="containsText">Log record text filter. Pass null or "" if no text filter.</param>
        public LogSession[] GetImapLogSessions(int limit,DateTime date,DateTime startTime,DateTime endTime,string containsText)
        {
            return GetLogSessions("IMAP",limit,date,startTime,endTime,containsText);
        }

        #endregion

        #region method GetRelayLogSessions

        /// <summary>
        /// Gets RELAY sessions logs.
        /// </summary>
        /// <param name="limit">Specifies maximum number of log entries to return.</param>
        /// <param name="date">Date what logs to get.</param>
        /// <param name="startTime">Start time filter.</param>
        /// <param name="endTime">End time filter.</param>
        /// <param name="containsText">Log record text filter. Pass null or "" if no text filter.</param>
        public LogSession[] GetRelayLogSessions(int limit,DateTime date,DateTime startTime,DateTime endTime,string containsText)
        {
            return GetLogSessions("RELAY",limit,date,startTime,endTime,containsText);
        }

        #endregion

        #region method GetFetchLogSessions

        /// <summary>
        /// Gets FETCH sessions logs.
        /// </summary>
        /// <param name="limit">Specifies maximum number of log entries to return.</param>
        /// <param name="date">Date what logs to get.</param>
        /// <param name="startTime">Start time filter.</param>
        /// <param name="endTime">End time filter.</param>
        /// <param name="containsText">Log record text filter. Pass null or "" if no text filter.</param>
        public LogSession[] GetFetchLogSessions(int limit,DateTime date,DateTime startTime,DateTime endTime,string containsText)
        {
            return GetLogSessions("FETCH",limit,date,startTime,endTime,containsText);
        }

        #endregion


        #region method GetLogSessions

        /// <summary>
        /// Gets specified service session logs.
        /// </summary>
        /// <param name="service">Service name. SMTP,POP3,IMAP,RELAY,FETCH.</param>
        /// <param name="limit">Specifies maximum number of log entries to return.</param>
        /// <param name="date">Date what logs to get.</param>
        /// <param name="startTime">Start time filter.</param>
        /// <param name="endTime">End time filter.</param>
        /// <param name="containsText">Log record text filter. Pass null or "" if no text filter.</param>
        /// <returns></returns>
        private LogSession[] GetLogSessions(string service,int limit,DateTime date,DateTime startTime,DateTime endTime,string containsText)
        {
            /* GetLogSessions <virtualServerID> <service> <limit> "<startTime>" "<endTime>" "containsText"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetLogSessions
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetLogSessions " + 
                    m_pVirtualServer.VirtualServerID + " " +
                    service + " " +
                    limit.ToString() + " " +
                    TextUtils.QuoteString(startTime.ToUniversalTime().ToString("u")) + " " +
                    TextUtils.QuoteString(endTime.ToUniversalTime().ToString("u")) + " " +
                    TextUtils.QuoteString(containsText)
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

                List<LogSession> retVal = new List<LogSession>();
                if(ds.Tables.Contains("LogSessions")){
                    foreach(DataRow dr in ds.Tables["LogSessions"].Rows){
                        retVal.Add(new LogSession(
                            m_pVirtualServer,
                            service,
                            dr["SessionID"].ToString(),
                            Convert.ToDateTime(dr["StartTime"]),
                            ConvertEx.ToIPEndPoint(dr["RemoteEndPoint"].ToString(),new IPEndPoint(IPAddress.None,0)),
                            dr["UserName"].ToString()
                        ));
                    }
                }

                return retVal.ToArray();
            }
        }

        #endregion

    }
}
