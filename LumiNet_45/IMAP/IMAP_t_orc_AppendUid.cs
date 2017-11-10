using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>APPENDUID</b> optional response code. Defined in RFC 4315.
    /// </summary>
    public class IMAP_t_orc_AppendUid : IMAP_t_orc
    {
        private long m_MailboxUid = 0;
        private int  m_MessageUid = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailboxUid">Mailbox UID value.</param>
        /// <param name="msgUid">Message UID value.</param>
        public IMAP_t_orc_AppendUid(long mailboxUid,int msgUid)
        {
            m_MailboxUid = mailboxUid;
            m_MessageUid = msgUid;
        }


        #region static method Parse

        /// <summary>
        /// Parses APPENDUID optional response from reader.
        /// </summary>
        /// <param name="r">APPENDUID optional response reader.</param>
        /// <returns>Returns APPENDUID optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public new static IMAP_t_orc_AppendUid Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            /* RFC 4315 3.
                APPENDUID
                  Followed by the UIDVALIDITY of the destination mailbox and the UID
                  assigned to the appended message in the destination mailbox,
                  indicates that the message has been appended to the destination
                  mailbox with that UID.
            */

            string[] code_mailboxUid_msgUid = r.ReadParenthesized().Split(new char[]{' '},3);
            if(!string.Equals("APPENDUID",code_mailboxUid_msgUid[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid APPENDUID response value.","r");
            }
            if(code_mailboxUid_msgUid.Length != 3){
                throw new ArgumentException("Invalid APPENDUID response value.","r");
            }

            return new IMAP_t_orc_AppendUid(Convert.ToInt64(code_mailboxUid_msgUid[1]),Convert.ToInt32(code_mailboxUid_msgUid[2]));
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "APPENDUID " + m_MailboxUid + " " + m_MessageUid;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets mailbox(folder) UID value.
        /// </summary>
        public long MailboxUid
        {
            get{ return m_MailboxUid; }
        }

        /// <summary>
        /// Gets message UID value.
        /// </summary>
        public int MessageUid
        {
            get{ return m_MessageUid; }
        }

        #endregion
    }
}
