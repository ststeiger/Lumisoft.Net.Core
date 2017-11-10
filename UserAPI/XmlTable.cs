using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Represents simple xml serializable/deserializable name/value table.
    /// </summary>
    public class XmlTable
    {
        private string    m_TableName = "";
        private Hashtable m_pValues   = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public XmlTable(string tableName)
        {
            if(tableName == null || tableName == ""){
                throw new Exception("Table name can't be empty !");
            }

            m_TableName = tableName;

            m_pValues = new Hashtable();
        }


        #region method Add

        /// <summary>
        /// Adds name/value to table.
        /// </summary>
        /// <param name="name">Name of the value pair.</param>
        /// <param name="value">Value.</param>
        public void Add(string name,string value)
        {
            if(m_pValues.ContainsKey(name)){
                throw new Exception("Specified name '" + name + "' already exists !");
            }

            m_pValues.Add(name,value);
        }

        #endregion

        #region method GetValue

        /// <summary>
        /// Gets value from name/value table.
        /// </summary>
        /// <param name="name">Name of value to get.</param>
        /// <returns></returns>
        public string GetValue(string name)
        {
            if(!m_pValues.ContainsKey(name)){
                throw new Exception("Specified name '" + name + "' doesn't exists !");
            }

            return m_pValues[name].ToString();
        }

        #endregion


        #region method Parse

        /// <summary>
        /// Parses table from byte[] xml data.
        /// </summary>
        /// <param name="data">Table data.</param>
        public void Parse(byte[] data)
        {
            m_pValues.Clear();

            MemoryStream ms = new MemoryStream(data);
            XmlTextReader wr = new XmlTextReader(ms);
            
            // Read start element
            wr.Read();

            m_TableName = wr.LocalName;
                      
            // Read Name/Values
            while(wr.Read()){            
                if(wr.NodeType == XmlNodeType.Element){
                    this.Add(wr.LocalName,wr.ReadElementString());
                }
            }
        }

        #endregion


        #region method ToString

        /// <summary>
        /// Returns string representation of xml table.
        /// </summary>
        /// <returns>Returns string representation of xml table.</returns>
        public string ToStringData()
        {
            return Encoding.Default.GetString(ToByteData());
        }

        #endregion

        #region method ToByteData

        /// <summary>
        /// Returns byte[] representation of xml table.
        /// </summary>
        /// <returns>Returns byte[] representation of xml table.</returns>
        public byte[] ToByteData()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter wr = new XmlTextWriter(ms,Encoding.UTF8);

            // Write table start
            wr.WriteStartElement(m_TableName);
            wr.WriteRaw("\r\n");

            // Write elements
            foreach(DictionaryEntry entry in m_pValues){
                wr.WriteRaw("\t");
                wr.WriteStartElement(entry.Key.ToString());

                wr.WriteValue(entry.Value.ToString());

                wr.WriteEndElement();
                wr.WriteRaw("\r\n");
            }

            // Write table end
            wr.WriteEndElement();
            wr.Flush();

            return ms.ToArray();
        }

        #endregion


        #region Porperties Implementation

        /// <summary>
        /// Gets or sets table name.
        /// </summary>
        public string TableName
        {
            get{ return m_TableName; }

            set{ m_TableName = value; }
        }

        #endregion
    }
}
