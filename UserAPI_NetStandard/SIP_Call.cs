using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// This class represents SIP Call in virtual server.
    /// </summary>
    public class SIP_Call
    {
        private SIP_CallCollection m_pOwner = null;
        private string             m_CallID = "";
        private string             m_Caller = "";
        private string             m_Callee = "";
        private DateTime           m_StartTime;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner collection.</param>
        /// <param name="callID">Call ID.</param>
        /// <param name="caller">Caller URI.</param>
        /// <param name="callee">Callee URI.</param>
        /// <param name="startTime">Call start time.</param>
        internal SIP_Call(SIP_CallCollection owner,string callID,string caller,string callee,DateTime startTime)
        {
            m_pOwner    = owner;
            m_CallID    = callID;
            m_Caller    = caller;
            m_Callee    = callee;
            m_StartTime = startTime;
        }


        #region method Terminate

        /// <summary>
        /// Terminates call.
        /// </summary>
        public void Terminate()
        {
            /* TerminateSipCall "<virtualServerID>" "<callID>"
                  Responses:
                    +OK 
                    + ERR <errorText>
            */

            lock(m_pOwner.VirtualServer.Server.LockSynchronizer){
                m_pOwner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("TerminateSipCall " + 
                    TextUtils.QuoteString(m_pOwner.VirtualServer.VirtualServerID) + " " +
                    TextUtils.QuoteString(this.CallID)
                );
                            
                string response = m_pOwner.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets call ID.
        /// </summary>
        public string CallID
        {
            get{ return m_CallID; }
        }

        /// <summary>
        /// Gets caller.
        /// </summary>
        public string Caller
        {
            get{ return m_Caller; }
        }

        /// <summary>
        /// Gets callee.
        /// </summary>
        public string Callee
        {
            get{ return m_Callee; }
        }

        /// <summary>
        /// Gets call start time.
        /// </summary>
        public DateTime StartTime
        {
            get{ return m_StartTime; }
        }

        #endregion

    }
}
