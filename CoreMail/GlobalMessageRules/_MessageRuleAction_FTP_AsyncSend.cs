using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using LumiSoft.Net.FTP.Client;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// FTP asynchronous file uploader. This class is used by internally by 'Store To FTP Folder' message rule action.
    /// </summary>
    internal class _MessageRuleAction_FTP_AsyncSend
    {
        private string m_Server     = "";
        private int    m_Port       = 21;
        private string m_User       = "";
        private string m_Password   = "";
        private string m_Folder     = "";
        private Stream m_DataStream = null;
        private string m_FileName   = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">FTP server.</param>
        /// <param name="port">FTP server port.</param>
        /// <param name="user">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="folder">FTP folder.</param>
        /// <param name="data">Data to store to server.</param>
        /// <param name="fileName">File name to add to stored data.</param>
        public _MessageRuleAction_FTP_AsyncSend(string server,int port,string user,string password,string folder,Stream data,string fileName)
        {
            m_Server     = server;
            m_Port       = port;
            m_User       = user;
            m_Password   = password;
            m_Folder     = folder;            
            m_FileName   = fileName;

            m_DataStream = new MemoryStream(); // We need to use copy of message
            SCore.StreamCopy(data,m_DataStream);
            m_DataStream.Position = 0;

            Thread tr = new Thread(new ThreadStart(this.Send));
            tr.Start();
        }


        #region method Send

        /// <summary>
        /// Sends message.
        /// </summary>
        private void Send()
        {
            try{
                using(FTP_Client ftp = new FTP_Client()){
                    ftp.Connect(m_Server,m_Port);
                    ftp.Authenticate(m_User,m_Password);
                    ftp.SetCurrentDir(m_Folder);
                    ftp.StoreFile(m_FileName,m_DataStream);
                }
            }
            catch(Exception x){
                Error.DumpError(x,new System.Diagnostics.StackTrace());
            }
        }

        #endregion

    }
}
