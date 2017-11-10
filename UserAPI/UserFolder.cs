using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;
using LumiSoft.Net.Mail;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The UserFolder object represents user folder in user.
    /// </summary>
    public class UserFolder
    {
        private UserFolderCollection    m_pOwner        = null;
        private User                    m_pUser         = null;
        private UserFolder              m_pParent       = null;
        private string                  m_FolderName    = "";
        private string                  m_Path          = "";
        private UserFolderAclCollection m_pAcl          = null;
        private UserFolderCollection    m_pChildFolders = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner UserFolderCollection collection that owns this object.</param>
        /// <param name="user">Owner user.</param>
        /// <param name="parent">Gets parent folder.</param>
        /// <param name="folderPath">Folder path where folder belongs to.</param>
        /// <param name="folderName">Folder name.</param>
        internal UserFolder(UserFolderCollection owner,User user,UserFolder parent,string folderPath,string folderName)
        {
            m_pOwner        = owner;
            m_pUser         = user;
            m_pParent       = parent;
            m_Path          = folderPath;
            m_FolderName    = folderName;
            m_pChildFolders = new UserFolderCollection(false,this,m_pUser);
        }


        #region method Rename

        /// <summary>
        /// Renames/moves folder name.
        /// </summary>
        /// <param name="newFolderName">Full folder path, path + folder name.</param>
        public void Rename(string newFolderName)
        {
            // Find root folder collection
            UserFolderCollection root = m_pOwner;
            while(root.Parent != null){
                root = root.Parent.Owner;
            }

            // Find new folderfolder collection            
            string folderName = "";
            string[] path_name = newFolderName.Replace('\\','/').Split(new char[]{'/'});
            if(path_name.Length == 1){
                folderName = newFolderName;
            }
            else{
                folderName = path_name[path_name.Length - 1];
            }
            UserFolderCollection newFolderCollection = root;
            for(int i=0;i<path_name.Length-1;i++){
                newFolderCollection = newFolderCollection[path_name[i]].ChildFolders;
            }

            /* RenameUserFolder <virtualServerID> "<folderOwnerUser>" "<folder>" "<newFolder>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();
            
            // Call TCP RenameUserFolder
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("RenameUserFolder " + 
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pUser.UserName) + " " + 
                TextUtils.QuoteString(this.FolderFullPath) + " " +
                TextUtils.QuoteString(newFolderName)
            );
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            // Move folder to right hierarchy
            m_pOwner.List.Remove(this);
            newFolderCollection.List.Add(this);
        }

        #endregion

        #region method GetMessagesInfo

        /// <summary>
        /// Gets folder messages info.
        /// </summary>
        /// <returns></returns>
        public DataSet GetMessagesInfo()
        {
            /* GetUserFolderMessagesInfo <virtualServerID> "<user>" "<folder>"
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    + ERR <errorText>
            */
    
            lock(m_pUser.VirtualServer.Server.LockSynchronizer){
                // Call TCP GetUserFolderMessagesInfo
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserFolderMessagesInfo " + 
                    m_pUser.VirtualServer.VirtualServerID + " " +
                    TextUtils.QuoteString(m_pUser.UserName) + " " + 
                    TextUtils.QuoteString(this.FolderFullPath)
                );

                string response = m_pUser.VirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pUser.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                return ds;
            }
        }

        #endregion

        #region method DeleteMessage

        /// <summary>
        /// Deletes specified message.
        /// </summary>
        /// <param name="messageID">Message ID.</param>
        /// <param name="uid">Message IMAP UID value.</param>
        public void DeleteMessage(string messageID,long uid)
        {
            /* DeleteUserFolderMessage <virtualServerID> "<folderOwnerUser>" "<folder>" "<ID>" "<UID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            // Call TCP DeleteUserFolderMessage
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteUserFolderMessage " + 
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pUser.UserName) + " " + 
                TextUtils.QuoteString(this.FolderFullPath) + " " +
                TextUtils.QuoteString(messageID) + " " +
                TextUtils.QuoteString(uid.ToString())
            );
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
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
            /* C: GetUserFolderMessage <virtualServerID> "<folderOwnerUser>" "<folder>" "<messageID>"
               S: +OK <sizeInBytes>
               S: <messageData>
                            
                  Responses:
                    +OK               
                    -ERR <errorText>
            */

            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserFolderMessage " +
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pUser.UserName) + " " + 
                TextUtils.QuoteString(this.FolderFullPath) + " " +
                TextUtils.QuoteString(messageID)
            );

            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            int size = Convert.ToInt32(response.Split(' ')[1]);
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(message,size);
        }

        #endregion

        #region method StoreMessage

        /// <summary>
        /// Stores specified message to this folder. Message storing begins from stream current position.
        /// </summary>
        /// <param name="message">Message to store.</param>
        public void StoreMessage(Stream message)
        {
            long pos = message.Position;
            try{                
                Mail_Message.ParseFromStream(message);
            }
            catch{
                throw new Exception("Specified message is not valid message or is corrupt !");
            }
            message.Position = pos;

            /* C: StoreUserFolderMessage <virtualServerID> "<folderOwnerUser>" "<folder>" <sizeInBytes>
               S: +OK Send message data
               C: <messageData>
               S: +OK
             
                  Responses:
                    +OK               
                    -ERR <errorText>
            */

            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("StoreUserFolderMessage " +
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pUser.UserName) + " " + 
                TextUtils.QuoteString(this.FolderFullPath) + " " +
                (message.Length - message.Position).ToString()
            );

            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteStream(message);

            response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }
        }

        #endregion


        #region method GetFolderInfo

        /// <summary>
        /// Gets folder info. Return structure: {creationDate,numberOfMessages,sizeUsed}.
        /// </summary>
        /// <returns></returns>
        private string[] GetFolderInfo()
        {
            /* GetUserFolderInfo <virtualServerID> "<folderOwnerUser>" "<folder>"
                  Responses:
                    +OK "<creationDate>" <numberOfMessages> <sizeUsed>                  
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();
            
            // Call TCP GetUserFolderInfo
            m_pUser.VirtualServer.Server.TcpClient.TcpStream.WriteLine("GetUserFolderInfo " + 
                m_pUser.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_pUser.UserName) + " " + 
                TextUtils.QuoteString(this.FolderFullPath)
            );
                        
            string response = m_pUser.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            return TextUtils.SplitQuotedString(response.Substring(4),' ',true);
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner UserFolderCollection that owns this object.
        /// </summary>
        public UserFolderCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets user who owns that folder.
        /// </summary>
        public User User
        {
            get{ return m_pUser; }
        }

        /// <summary>
        /// Gets parent folder collection. Returns null if this is root folders collection.
        /// </summary>
        public UserFolder Parent
        {
            get{ return m_pParent; }
        }

        /// <summary>
        /// Gets folder name. NOTE: Path isn't included !
        /// </summary>
        public string FolderName
        {
            get{ return m_FolderName; }
        }

        /// <summary>
        /// Gets folder path. NOTE: Folder name isn't included !
        /// </summary>
        public string FolderPath
        {
            get{ return m_Path; }
        }

        /// <summary>
        /// Gets folder path. NOTE: Folder name is included !
        /// </summary>
        public string FolderFullPath
        {
            get{ 
                if(m_Path != ""){
                    return m_Path + "/" + m_FolderName;
                }
                else{
                    return m_FolderName;
                }
            }
        }
        
        /// <summary>
        /// Gets specified folder specified ACL entires.
        /// </summary>
        public UserFolderAclCollection ACL
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pAcl == null){
                    m_pAcl = new UserFolderAclCollection(this);
                }

                return m_pAcl;
            }
        }
        
        /// <summary>
        /// Gets specified folder child folders.
        /// </summary>
        public UserFolderCollection ChildFolders
        {
            get{ return m_pChildFolders; }
        } 
       
        /// <summary>
        /// Gets number of messages in this folder.
        /// </summary>
        public int MessagesCount
        {
            get{ return Convert.ToInt32(GetFolderInfo()[1]); }
        }

        /// <summary>
        /// Gets number bytes this folder messages allocate.
        /// </summary>
        public long SizeUsed
        {
            get{ return Convert.ToInt32(GetFolderInfo()[2]); }
        }

        /// <summary>
        /// Gets date/time when folder was created.
        /// </summary>
        public DateTime CreationTime
        {
            get{ return DateTime.ParseExact(GetFolderInfo()[0],"yyyyMMdd HH:mm:ss",System.Globalization.DateTimeFormatInfo.InvariantInfo); }
        }

        #endregion

    }
}
