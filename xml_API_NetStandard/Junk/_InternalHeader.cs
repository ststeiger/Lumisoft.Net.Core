using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.Mime;

namespace LumiSoft.MailServer
{
    #pragma warning disable 

    /// <summary>
    /// Internal .eml file header. Obsolete, get rid of in new versions.
    /// </summary>
    internal class _InternalHeader
    {
        private FileStream        m_pFile        = null;
        private IMAP_MessageFlags m_MessageFlags = IMAP_MessageFlags.Recent; 
        private string            m_Envelope     = "";
        private string            m_Body         = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fs">Message file stream.</param>
        public _InternalHeader(FileStream fs)
        {
            m_pFile = fs;
            
            StreamLineReader r = new StreamLineReader(fs);
            string line = r.ReadLineString();
            if(line!= null && line.ToLower() == "<internalheader>"){                
                line = r.ReadLineString();
                while(line.ToLower() != "</internalheader>"){
                    if(line.ToLower().StartsWith("#-messageflags:")){                        
                        m_MessageFlags = (IMAP_MessageFlags)Enum.Parse(typeof(IMAP_MessageFlags),line.Substring(15).Trim());
                    }
                    else if(line.ToLower().StartsWith("#-envelope:")){
                        m_Envelope = line.Substring(11).Trim();
                    }
                    else if(line.ToLower().StartsWith("#-body:")){
                        m_Body = line.Substring(7).Trim();
                    }

                    line = r.ReadLineString();
                }

                // Remove internal header
                if(fs.CanWrite){
                    byte[] data = new byte[fs.Length - fs.Position];
                    fs.Read(data,0,data.Length);
                    fs.Position = 0;
                    fs.Write(data,0,data.Length);
                    fs.SetLength(data.Length);
                    fs.Position = 0;
                }
            }
            // Internal header doesn't exist
            else{
                fs.Position = 0;
            }
        }


        #region Properties Implementation

        /// <summary>
        /// Gets message flags.
        /// </summary>
        public IMAP_MessageFlags MessageFlags
        {
            get{
                return m_MessageFlags;
            }
        }

        /// <summary>
        /// Gets messgae IMAP ENVELOPE.
        /// </summary>
        public string Envelope
        {
            get{ return m_Envelope; }
        }

        /// <summary>
        /// Gets message IMAP BODY.
        /// </summary>
        public string Body
        {
            get{ return m_Body; }
        }

        #endregion

    }

    #pragma warning enable 
}
