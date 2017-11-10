using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.Mail;
using LumiSoft.Net.IMAP;

namespace LumiSoft.MailServer
{
    #region class RecycleBinMessageInfo

    /// <summary>
    /// This class holds recycle bin message info.
    /// </summary>
    internal class RecycleBinMessageInfo
    {
        private string            m_MessageID  = "";
        private DateTime          m_DeleteTime;
        private string            m_User       = "";
        private string            m_Folder     = "";
        private int               m_Size       = 0;
        private string            m_Envelope   = "";
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="messageID">Message ID.</param>
        /// <param name="deleteTime">Message delete time.</param>
        /// <param name="user">User name.</param>
        /// <param name="folder">Folder name with path. This is folder what originally contained message.</param>
        /// <param name="size">Message size in bytes.</param>
        /// <param name="envelope">Message IMAP Envelope string.</param>
        public RecycleBinMessageInfo(string messageID,DateTime deleteTime,string user,string folder,int size,string envelope)
        {
            m_MessageID    = messageID;
            m_DeleteTime   = deleteTime;
            m_User         = user;
            m_Folder       = folder;
            m_Size         = size;
            m_Envelope     = envelope;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets message ID.
        /// </summary>
        public string MessageID
        {
            get{ return m_MessageID; }
        }

        /// <summary>
        /// Gets message delete time.
        /// </summary>
        public DateTime DeleteTime
        {
            get{ return m_DeleteTime; }
        }

        /// <summary>
        /// Gets user name whos message it is.
        /// </summary>
        public string User
        {
            get{ return m_User; }
        }

        /// <summary>
        /// Gets name with path. This is folder what originally contained message.
        /// </summary>
        public string Folder
        {
            get{ return m_Folder; }
        }

        /// <summary>
        /// Gets message size in bytes,
        /// </summary>
        public int Size
        {
            get{ return m_Size; }
        }

        /// <summary>
        /// Gets message IMAP Envelope string.
        /// </summary>
        public string Envelope
        {
            get{ return m_Envelope; }
        }
        
        #endregion

    }

    #endregion

    /// <summary>
    /// Recycle Bin manager.
    /// </summary>
    internal class RecycleBinManager
    {
        private static string m_RecycleBinPath = "";


        #region static method GetMessagesInfo

        /// <summary>
        /// Gets specified folder messages info.
        /// </summary>
        /// <param name="user">User who's recyclebin messages to get or null if all users messages.</param>
        /// <param name="startDate">Messages from specified date. Pass DateTime.MinValue if not used.</param>
        /// <param name="endDate">Messages to specified date. Pass DateTime.MinValue if not used.</param>
        /// <returns></returns>
        public static List<RecycleBinMessageInfo> GetMessagesInfo(string user,DateTime startDate,DateTime endDate)
        {
            List<RecycleBinMessageInfo> retVal = new List<RecycleBinMessageInfo>();

            using(FileStream fs = GetFile()){
                TextReader r = new StreamReader(fs);
                string line = r.ReadLine();
                while(line != null){
                    // Skip comment lines and deleted rows
                    if(!(line.StartsWith("#") || line.StartsWith("\0"))){                        
                        string[] row = TextUtils.SplitQuotedString(line,' ',true);

                        bool skip = false;
                        // Not wanted user message
                        if(user != null && user != "" && row[2].ToLower() != user.ToLower()){                       
                            skip = true;
                        }                       
                        // Not wanted start date
                        if(startDate != DateTime.MinValue && startDate > DateTime.ParseExact(row[1],"yyyyMMddHHmmss",System.Globalization.DateTimeFormatInfo.CurrentInfo).Date){
                            skip = true;
                        }
                        // Too new messages, break
                        if(endDate != DateTime.MinValue && endDate < DateTime.ParseExact(row[1],"yyyyMMddHHmmss",System.Globalization.DateTimeFormatInfo.CurrentInfo).Date){
                            break;
                        }
                    
                        if(!skip){
                            // Old deleted messages. REMOVE ME: in version >= 0.93
                            if(row.Length == 5){
                                retVal.Add(new RecycleBinMessageInfo(
                                    row[0],
                                    DateTime.ParseExact(row[1],"yyyyMMddHHmmss",System.Globalization.DateTimeFormatInfo.CurrentInfo),
                                    row[2],
                                    row[3],
                                    0,
                                    ""                         
                                ));
                            }
                            else if(row.Length == 6){
                                retVal.Add(new RecycleBinMessageInfo(
                                    row[0],
                                    DateTime.ParseExact(row[1],"yyyyMMddHHmmss",System.Globalization.DateTimeFormatInfo.CurrentInfo),
                                    row[2],
                                    row[3],
                                    Convert.ToInt32(row[4]),
                                    row[5]                           
                                ));
                            }
                        }
                    }

                    line = r.ReadLine();
                }
            }

            return retVal;
        }

        #endregion

        #region static method StoreToRecycleBin

        /// <summary>
        /// Stores specified message to recycle bin.
        /// </summary>
        /// <param name="folderOwner">Folder woner user.</param>
        /// <param name="folder">Folder what message it is.</param>
        /// <param name="messageFile">Message file name with path.</param>
        public static void StoreToRecycleBin(string folderOwner,string folder,string messageFile)
        {            
            string messageID = Guid.NewGuid().ToString().Replace("-","");
            string envelope  = "";
            int    size      = 0;
            try{
                // Parse message
                Mail_Message m = Mail_Message.ParseFromFile(messageFile);
                
                // Construct envelope
                envelope = IMAP_Envelope.ConstructEnvelope(m);

                size = (int)new FileInfo(messageFile).Length;
            }
            catch{
            }

            if(!Directory.Exists(m_RecycleBinPath)){
                try{
                    Directory.CreateDirectory(m_RecycleBinPath);
                }
                catch{
                }
            }

            // Store recycle bin copy of message
            File.Copy(messageFile,m_RecycleBinPath + messageID + ".eml");
            
            // Update index file
            using(FileStream fs = GetFile()){
                fs.Position = fs.Length;

                // MessageID DeleteDate UserName Folder InternalDate Flags Subject
                byte[] record = System.Text.Encoding.ASCII.GetBytes(
                    messageID + " " + 
                    DateTime.Now.ToString("yyyyMMddHHmmss") + " " + 
                    folderOwner + " " + 
                    TextUtils.QuoteString(folder) + " " +
                    size + " " +
                    TextUtils.QuoteString(envelope) + "\r\n"
                );
                fs.Write(record,0,record.Length);
            }
        }

        #endregion

        #region static method GetRecycleBinMessage

        /// <summary>
        /// Gets recycle bin message stream. NOTE: This method caller must take care of closing stream.
        /// </summary>
        /// <param name="messageID">Message ID if of message what to get.</param>
        /// <returns></returns>
        public static Stream GetRecycleBinMessage(string messageID)
        {
            using(FileStream fs = GetFile()){
                int              delRowCount = 0;
                StreamLineReader r           = new StreamLineReader(fs);
                long             pos         = fs.Position;
                string           line        = r.ReadLineString();
                while(line != null){
                    // Skip comment lines
                    if(!line.StartsWith("#")){
                        // Skip deleted row
                        if(line.StartsWith("\0")){
                            delRowCount++;
                        }
                        else{
                            string[] row = TextUtils.SplitQuotedString(line,' ');
                            // Delete row
                            if(row[0] == messageID){
                                string            user   = row[2];
                                string            folder = TextUtils.UnQuoteString(row[3]);

                                // Store message back to original user folder
                                FileStream stream = File.OpenRead(m_RecycleBinPath + messageID + ".eml");
                                return stream;                                                                
                            }
                        }
                    }

                    pos  = fs.Position;
                    line = r.ReadLineString();
                }
            }

            throw new Exception("Specified message doesn't exist !");
        }

        #endregion

        #region static method DeleteRecycleBinMessage

        /// <summary>
        /// Deletes specified message from recycle bin.
        /// </summary>
        /// <param name="messageID">Message ID which to restore.</param>
        public static void DeleteRecycleBinMessage(string messageID)
        {
            using(FileStream fs = GetFile()){
                int              delRowCount = 0;
                StreamLineReader r           = new StreamLineReader(fs);
                long             pos         = fs.Position;
                string           line        = r.ReadLineString();
                while(line != null){
                    // Skip comment lines
                    if(!line.StartsWith("#")){
                        // Skip deleted row
                        if(line.StartsWith("\0")){
                            delRowCount++;
                        }
                        else{
                            string[] row = TextUtils.SplitQuotedString(line,' ');
                            // Delete row
                            if(row[0] == messageID){
                                string            user   = row[2];
                                string            folder = TextUtils.UnQuoteString(row[3]);

                                // Delete message
                                File.Delete(m_RecycleBinPath + messageID + ".eml");

                                // Delete row
                                byte[] linebytes = new byte[fs.Position - pos - 2];
                                fs.Position = pos;
                                fs.Write(linebytes,0,linebytes.Length);
                                fs.Position += 2; // CRLF
                                delRowCount++;
                                break;
                            }
                        }
                    }

                    pos  = fs.Position;
                    line = r.ReadLineString();
                }

                // There are many deleted rows, vacuum(remove deleted rows) flags database.
                if(delRowCount > 500){
                    Vacuum(fs);
                }
            }
        }

        #endregion

        #region static method RestoreFromRecycleBin

        /// <summary>
        /// Restores specified message from recycle bin.
        /// </summary>
        /// <param name="messageID">Message ID which to restore.</param>
        /// <param name="api">Reference to API.</param>
        public static void RestoreFromRecycleBin(string messageID,IMailServerApi api)
        {
            using(FileStream fs = GetFile()){
                int              delRowCount = 0;
                StreamLineReader r           = new StreamLineReader(fs);
                long             pos         = fs.Position;
                string           line        = r.ReadLineString();
                while(line != null){
                    // Skip comment lines
                    if(!line.StartsWith("#")){
                        // Skip deleted row
                        if(line.StartsWith("\0")){
                            delRowCount++;
                        }
                        else{
                            string[] row = TextUtils.SplitQuotedString(line,' ');
                            // Delete row
                            if(row[0] == messageID){
                                string            user   = row[2];
                                string            folder = TextUtils.UnQuoteString(row[3]);

                                // Store message back to original user folder
                                using(FileStream stream = File.OpenRead(m_RecycleBinPath + messageID + ".eml")){
                                    // If folder doesn't exist, create it
                                    if(!api.FolderExists(user + "/" + folder)){
                                        api.CreateFolder("system",user,folder);
                                    }

                                    api.StoreMessage("system",user,folder,stream,DateTime.Now,new string[]{"Recent"});
                                }

                                // Delete row
                                byte[] linebytes = new byte[fs.Position - pos - 2];
                                fs.Position = pos;
                                fs.Write(linebytes,0,linebytes.Length);
                                fs.Position += 2; // CRLF
                                delRowCount++;

                                // Delete recycle bin message
                                File.Delete(m_RecycleBinPath + messageID + ".eml");
                                break;
                            }
                        }
                    }

                    pos  = fs.Position;
                    line = r.ReadLineString();
                }

                // There are many deleted rows, vacuum(remove deleted rows) flags database.
                if(delRowCount > 500){
                    Vacuum(fs);
                }
            }
        }

        #endregion


        #region static method GetFile

        /// <summary>
        /// Gets messages info file.
        /// </summary>
        internal static FileStream GetFile()
        {
            // Try 20 seconds to open flags file, it's locked.
            DateTime start = DateTime.Now;
            string   error = "";
            
            while(start.AddSeconds(20) > DateTime.Now){
                try{
                    FileStream fs = File.Open(m_RecycleBinPath + "_index.txt",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None);

                    // Messages info file just created.
                    if(fs.Length == 0){                                     
                        byte[] fileCommnet = System.Text.Encoding.ASCII.GetBytes("#\r\n# This file holds messages info, don't delete this file !\r\n#\r\n");
                        fs.Write(fileCommnet,0,fileCommnet.Length);
                    }
                    fs.Position = 0;

                    return fs;
                }
                catch(Exception x){
                    error = x.Message;

                    // Wait here, otherwise takes 100% CPU
                    System.Threading.Thread.Sleep(5);
                }
            }

            // If we reach here, flags file open failed.
            throw new Exception("Opening messages info file timed-out, failed with error: " + error);
        }

        #endregion

        #region static method Vacuum

        /// <summary>
        /// Vacuums flags database, deletes deleted rows empty used space from file.
        /// </summary>
        /// <param name="fs">Database file stream.</param>
        private static void Vacuum(FileStream fs)
        {
            MemoryStream buffer = new MemoryStream();
            fs.Position = 0;

            StreamLineReader r    = new StreamLineReader(fs);
            string           line = r.ReadLineString();
            while(line != null){
                // Skip deleted rows
                if(!line.StartsWith("\0")){
                    byte[] lineBytes = System.Text.Encoding.ASCII.GetBytes(line + "\r\n");
                    buffer.Write(lineBytes,0,lineBytes.Length);
                }                

                line = r.ReadLineString();
            }

            fs.SetLength(buffer.Length);
            fs.Position = 0;
            buffer.WriteTo(fs);
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets recycle bin path.
        /// </summary>
        public static string RecycleBinPath
        {
            get{ return m_RecycleBinPath; }

            set{ m_RecycleBinPath = value; }
        }

        #endregion

    }
}
