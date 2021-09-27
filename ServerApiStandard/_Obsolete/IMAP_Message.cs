
namespace LumiSoft.Net.IMAP.Server
{


    /// <summary>
    /// IMAP message info.
    /// </summary>
    public class IMAP_Message
    {
        private IMAP_MessageCollection m_pOwner = null;
        private string m_ID = "";
        private long m_UID = 0;
        private System.DateTime m_InternalDate = System.DateTime.Now;
        private long m_Size = 0;
        private IMAP_MessageFlags m_Flags = IMAP_MessageFlags.None;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="onwer">Owner collection.</param>
        /// <param name="id">Message ID.</param>
        /// <param name="uid">Message IMAP UID value.</param>
        /// <param name="internalDate">Message store date.</param>
        /// <param name="size">Message size in bytes.</param>
        /// <param name="flags">Message flags.</param>
        public IMAP_Message(IMAP_MessageCollection onwer, string id, long uid, System.DateTime internalDate, long size, IMAP_MessageFlags flags)
        {
            m_pOwner = onwer;
            m_ID = id;
            m_UID = uid;
            m_InternalDate = internalDate;
            m_Size = size;
            m_Flags = flags;
        }


        /// <summary>
        /// Sets message flags.
        /// </summary>
        /// <param name="flags">Message flags.</param>
        internal void SetFlags(IMAP_MessageFlags flags)
        {
            m_Flags = flags;
        }


        /// <summary>
        /// Gets message 1 based sequence number in the collection. This property is slow, use with care, never use in big for loops !
        /// </summary>
        public int SequenceNo
        {
            get { return m_pOwner.IndexOf(this) + 1; }
        }

        /// <summary>
        /// Gets message ID.
        /// </summary>
        public string ID
        {
            get { return m_ID; }
        }

        /// <summary>
        /// Gets message IMAP UID value.
        /// </summary>
        public long UID
        {
            get { return m_UID; }
        }

        /// <summary>
        /// Gets message store date.
        /// </summary>
        public System.DateTime InternalDate
        {
            get { return m_InternalDate; }
        }

        /// <summary>
        /// Gets message size in bytes.
        /// </summary>
        public long Size
        {
            get { return m_Size; }
        }

        /// <summary>
        /// Gets message flags.
        /// </summary>
        public IMAP_MessageFlags Flags
        {
            get { return m_Flags; }
        }

        /// <summary>
        /// Gets message flags string. For example: "\DELETES \SEEN".
        /// </summary>
        public string FlagsString
        {
            get { return IMAP_Utils.MessageFlagsToString(m_Flags); }
        }


    }


}
