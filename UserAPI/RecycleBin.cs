using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The RecycleBin object represents recycle bin in LumiSoft Mail Server virtual server.
    /// </summary>
    public class RecycleBin
    {
        private VirtualServer m_pVirtualServer      = null;
        private bool          m_deleteToRecycleBin  = false;
        private int           m_deleteMessagesAfter = 1;
        private bool          m_ValuesChanged       = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal RecycleBin(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            Bind();
        }


        #region method GetMessagesInfo

        /// <summary>
        /// Gets recycle bin messages info.
        /// </summary>
        /// <param name="user">User who's recyclebin messages to get or null if all users messages.</param>
        /// <param name="startDate">Messages from specified date. Pass DateTime.MinValue if not used.</param>
        /// <param name="endDate">Messages to specified date. Pass DateTime.MinValue if not used.</param>
        public DataTable GetMessagesInfo(string user,DateTime startDate,DateTime endDate)
        {
            /* GetRecycleBinMessagesInfo <virtualServerID> "<user>" "<startDate>" "<endDate>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */

            if(user == null){
                user = "";
            }

            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetRecycleBinMessagesInfo
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetRecycleBinMessagesInfo " + 
                    m_pVirtualServer.VirtualServerID + " " +
                    TextUtils.QuoteString(user) + " " +
                    TextUtils.QuoteString(startDate.ToUniversalTime().ToString("u")) + " " +
                    TextUtils.QuoteString(endDate.ToUniversalTime().ToString("u"))
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

                if(ds.Tables.Count == 0){
                    ds.Tables.Add("MessagesInfo");
                }

                return ds.Tables["MessagesInfo"];
            }
        }

        #endregion

        #region method GetMessage
        
        /// <summary>
        /// Gets specified message and stores it to specified stream.
        /// </summary>
        /// <param name="messageID">Message ID.</param>
        /// <param name="message">Stream where to store message.</param>
        public void GetMessage(string messageID,Stream message)
        {
            /* C: GetRecycleBinMessage <virtualServerID> "<messageID>"
               S: +OK <sizeInBytes>
               S: <messageData>
                            
                  Responses:
                    +OK               
                    -ERR <errorText>
            */

            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetRecycleBinMessage " +
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(messageID)
            );

            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            int size = Convert.ToInt32(response.Split(' ')[1]);
            m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(message,size);
        }

        #endregion

        #region method RestoreRecycleBinMessage

        /// <summary>
        /// Restores specified recycle bin message.
        /// </summary>
        /// <param name="messageID">Messge ID which to restore.</param>
        public void RestoreRecycleBinMessage(string messageID)
        {
            /* RestoreRecycleBinMessage <virtualServerID> <messageID>
                  Responses:
                    +OK <sizeOfData>
                    + ERR <errorText>
            */

            // Call TCP RestoreRecycleBinMessage
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("RestoreRecycleBinMessage " + 
                m_pVirtualServer.VirtualServerID + " " +
                messageID
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }
        }

        #endregion
                

        #region method Bind

        /// <summary>
        /// Gets recycle bin settings and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetRecycleBinSettings <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            // Call TCP GetRecycleBinSettings
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetRecycleBinSettings " + m_pVirtualServer.VirtualServerID);

            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
            MemoryStream ms = new MemoryStream();
            m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
            ms.Position = 0;

            DataSet ds = new DataSet();
            ds.ReadXml(ms);
         
            if(ds.Tables.Contains("RecycleBinSettings")){
                m_deleteToRecycleBin  = Convert.ToBoolean(ds.Tables["RecycleBinSettings"].Rows[0]["DeleteToRecycleBin"]);
                m_deleteMessagesAfter = Convert.ToInt32(ds.Tables["RecycleBinSettings"].Rows[0]["DeleteMessagesAfter"]);
            }
        }

        #endregion

        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't chnaged, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }

            /* UpdateRecycleBinSettings <virtualServerID> <deleteToRecycleBin> <deleteMessagesAfter>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            // Call TCP UpdateRecycleBinSettings
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateRecycleBinSettings " + 
                m_pVirtualServer.VirtualServerID + " " + 
                m_deleteToRecycleBin + " " + 
                m_deleteMessagesAfter
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets if this domain object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets or sets if deleted messages are store to recycle bin.
        /// </summary>
        public bool DeleteToRecycleBin
        {
            get{ return m_deleteToRecycleBin; }
            
            set{
                if(m_deleteToRecycleBin != value){
                    m_deleteToRecycleBin = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gest or sets how old messages will be deleted.
        /// </summary>
        public int DeleteMessagesAfter
        {
            get{ return m_deleteMessagesAfter; }

            set{
                if(value < 1 || value > 365){
                    throw new ArgumentException("DeleteMessagesAfter value must be between 1 and 365 !");
                }
                
                if(m_deleteMessagesAfter != value){
                    m_deleteMessagesAfter = value;

                    m_ValuesChanged = true;
                }            
            }
        }

        #endregion

    }
}
