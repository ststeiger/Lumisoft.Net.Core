using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net.IMAP.Server;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Provides data to IMailServerApi.GetMessageItems method.
    /// </summary>
    public class EmailMessageItems
    {
        private string                 m_MessageID          = "";
        private IMAP_MessageItems_enum m_MessageItems       = IMAP_MessageItems_enum.Message;
        private Stream                 m_MessageStream      = null;
        private long                   m_MessageStartOffset = 0;
        private byte[]                 m_Header             = null;
        private string                 m_Envelope           = null;
        private string                 m_BodyStructure      = null;
        private bool                   m_MessageExists      = true;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="messageID">Message ID what message items to get.</param>
        /// <param name="messageItems">Specifies message items what must be filled.</param>
        public EmailMessageItems(string messageID,IMAP_MessageItems_enum messageItems)
        {
            m_MessageID    = messageID;
            m_MessageItems = messageItems;
        }


        #region method CopyTo

        /// <summary>
        /// Copies EmailMessageItems info to the specified IMAP_eArgs_MessageItems object.
        /// </summary>
        /// <param name="e"></param>
        public void CopyTo(IMAP_eArgs_MessageItems e)
        {
            if(this.BodyStructure != null){
                e.BodyStructure = this.BodyStructure;
            }
            if(this.Envelope != null){
                e.Envelope = this.Envelope;
            }
            if(this.Header != null){
                e.Header = this.Header;
            }
            e.MessageExists = this.MessageExists;            
            if(this.MessageStream != null){
                e.MessageStream = this.MessageStream;
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets message ID what message items to get.
        /// </summary>
        public string MessageID
        {
            get{ return m_MessageID; }
        }

        /// <summary>
        /// Gets what message items must be filled.
        /// </summary>
        public IMAP_MessageItems_enum MessageItems
        {
            get{ return m_MessageItems; }
        }

        /// <summary>
        /// Gets or sets message stream. When setting this property Stream position must be where message begins.
        /// Fill this property only if IMAP_MessageItems_enum.Message flag is specified.
        /// Note: Stream must be closed by caller !
        /// </summary>
        public Stream MessageStream
        {
            get{
                if(m_MessageStream != null){
                    m_MessageStream.Position = m_MessageStartOffset;
                }
                return m_MessageStream; 
            }

            set{
                if(value == null){
                    throw new ArgumentNullException("Property MessageStream value can't be null !");
                }
                if(!value.CanSeek){
                    throw new Exception("Stream must support seeking !");
                }

                m_MessageStream = value;
                m_MessageStartOffset = m_MessageStream.Position;
            }
        }
        
        /// <summary>
        /// Gets or sets message main header.
        /// Fill this property only if IMAP_MessageItems_enum.Header flag is specified.
        /// </summary>
        public byte[] Header
        {
            get{ return m_Header; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Property Header value can't be null !");
                }

                m_Header = value;
            }
        }

        /// <summary>
        /// Gets or sets IMAP ENVELOPE string.
        /// Fill this property only if IMAP_MessageItems_enum.Envelope flag is specified.
        /// </summary>
        public string Envelope
        {
            get{ return m_Envelope; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Property Envelope value can't be null !");
                }

                m_Envelope = value;
            }
        }

        /// <summary>
        /// Gets or sets IMAP BODYSTRUCTURE string.
        /// Fill this property only if IMAP_MessageItems_enum.BodyStructure flag is specified.
        /// </summary>
        public string BodyStructure
        {
            get{ return m_BodyStructure; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Property BodyStructure value can't be null !");
                }

                m_BodyStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets if message exists. Set this false, if message actually doesn't exist any more.
        /// </summary>
        public bool MessageExists
        {
            get{ return m_MessageExists; }

            set{ m_MessageExists = value; }
        }

        #endregion
    }
}
