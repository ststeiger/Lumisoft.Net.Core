using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Text database(CSV,TAB,SP delimited text database).
    /// </summary>
    public class TextDb : IDisposable
    {
        private char             m_FieldDelimiter  = '\t';
        private bool             m_Open            = false;
        private Stream           m_pDatabaseStream = null;
        private StreamLineReader m_pReader         = null;
        private string           m_CurrentRow      = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fieldDelimiter">Field value delimiter. Comma,Tab,Space, ... . Restricted chars are: '"' '!' '#'.</param>
        public TextDb(char fieldDelimiter)
        {
            m_FieldDelimiter = fieldDelimiter;
        }

        #region method Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion


        #region method Open

        /// <summary>
        /// Opens specified text database.
        /// </summary>
        /// <param name="file">Text database file name with optional path.</param>
        public void Open(string file)
        {
            if(!File.Exists(file)){
                throw new Exception("Specified database file doesn't exist !");
            }

            m_pDatabaseStream = OpenOrCreateDb(file);
            m_pReader         = new StreamLineReader(m_pDatabaseStream);

            m_Open = true;
        }

        #endregion

        #region method OpenOrCreate

        /// <summary>
        /// Opens or creates specified text database.
        /// </summary>
        /// <param name="file">Text database file name with optional path.</param>
        public void OpenOrCreate(string file)
        {
            m_pDatabaseStream = OpenOrCreateDb(file);
            m_pReader         = new StreamLineReader(m_pDatabaseStream);

            m_Open = true;
        }

        #endregion

        #region method OpenRead

        /// <summary>
        /// Opens specified text database.
        /// </summary>
        /// <param name="file">Text database file name with optional path.</param>
        public void OpenRead(string file)
        {
            if(!File.Exists(file)){
                throw new Exception("Specified database file doesn't exist !");
            }

            m_pDatabaseStream = OpenOrCreateDb(file);
            m_pReader         = new StreamLineReader(new BufferedStream(m_pDatabaseStream));

            m_Open = true;
        }

        #endregion

        #region method Close

        /// <summary>
        /// Closes active text database file.
        /// </summary>
        public void Close()
        {
            if(!m_Open){
                return;
            }
            
            m_pDatabaseStream.Dispose();
            m_pDatabaseStream = null;

            m_Open = false;
        }

        #endregion


        #region method MoveNext

        /// <summary>
        /// Moves current row to next row. Returns true if there is next row and current row moved.
        /// </summary>
        /// <returns>Returns true if there is next row and current row moved.</returns>
        public bool MoveNext()
        {
            if(!m_Open){
                throw new Exception("Database not open, please open or create database first !");
            }

            m_CurrentRow = m_pReader.ReadLineString();
            if(m_CurrentRow != null){
                return true;
            }
            else{
                return false;
            }
        }

        #endregion

        #region method Delete
        /*
        /// <summary>
        /// Deletes current record.
        /// </summary>
        public void Delete()
        {
            if(!m_Open){
                throw new Exception("Database not open, please open or create database first !");
            }
        }*/

        #endregion

        #region method Append

        /// <summary>
        /// Appends new record to the end of database file. 
        /// </summary>
        /// <param name="values">Row values.</param>
        public void Append(string[] values)
        {
            if(!m_Open){
                throw new Exception("Database not open, please open or create database first !");
            }
            if(values == null){
                throw new ArgumentException("Parameter value may not be null !");
            }

            // Move to end of file.
            m_pDatabaseStream.Position = m_pDatabaseStream.Length;

            StringBuilder row = new StringBuilder();
            for(int i=0;i<values.Length;i++){
                // Add field separator, not last field.
                if(i != (values.Length - 1)){
                    row.Append(TextUtils.QuoteString(values[i]) + m_FieldDelimiter);
                }
                // Last field, don't add field separator, add row ending CRLF.
                else{
                    row.Append(TextUtils.QuoteString(values[i]) + "\r\n");
                }
            }

            byte[] rowBytes = System.Text.Encoding.UTF8.GetBytes(row.ToString());
            m_pDatabaseStream.Write(rowBytes,0,rowBytes.Length);
        }
        
        #endregion

        #region method AppendComment

        /// <summary>
        /// Appends new comment text to the end of database file. 
        /// </summary>
        /// <param name="text">Comment text.</param>
        public void AppendComment(string text)
        {
            if(!m_Open){
                throw new Exception("Database not open, please open or create database first !");
            }
            if(text == null){
                throw new ArgumentException("Parameter text may not be null !");
            }

            // Move to end of file.
            m_pDatabaseStream.Position = m_pDatabaseStream.Length;

            byte[] rowBytes = System.Text.Encoding.UTF8.GetBytes("# " + text + "\r\n");
            m_pDatabaseStream.Write(rowBytes,0,rowBytes.Length);
        }
        
        #endregion


        #region static method OpenOrCreateDb

        /// <summary>
        /// Opens or creates db file.
        /// </summary>
        /// <param name="file">Database file name.</param>
        internal static FileStream OpenOrCreateDb(string file)
        {
            if(!Directory.Exists(Path.GetDirectoryName(file))){
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }

            // Try 20 seconds to open db file, it's locked.
            DateTime start = DateTime.Now;
            string   error = "";

            while(start.AddSeconds(20) > DateTime.Now){
                try{
                    FileStream fs = File.Open(file,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.Read);
                    fs.Position = 0;

                    return fs;
                }
                catch(Exception x){
                    error = x.Message;

                    // Wait here, otherwise takes 100% CPU
                    System.Threading.Thread.Sleep(5);
                }
            }

            // If we reach here, db file open failed.
            throw new Exception("Opening db file timed-out, failed with error: " + error);
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets if there is active database.
        /// </summary>
        public bool IsOpen
        {
            get{ return m_Open; }
        }

        /// <summary>
        /// Gets current row string line. Returns null if no current record.
        /// </summary>
        public string CurrentRowString
        {
            get{ 
                if(!m_Open){
                    throw new Exception("Database not open, please open or create database first !");
                }

                return m_CurrentRow;
            }
        }

        /// <summary>
        /// Gets current row values. Returns null if no current record.
        /// </summary>
        public string[] CurrentRow
        {
            get{ 
                if(!m_Open){
                    throw new Exception("Database not open, please open or create database first !");
                }

                if(m_CurrentRow != null){
                    return TextUtils.SplitQuotedString(m_CurrentRow,m_FieldDelimiter,true);
                }
                else{
                    return null; 
                }
            }
        }

        #endregion

    }
}
