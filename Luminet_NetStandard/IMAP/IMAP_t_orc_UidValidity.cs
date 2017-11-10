using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>UIDVALIDITY</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_UidValidity : IMAP_t_orc
    {
        private long m_Uid = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uid">Mailbox UID value.</param>
        public IMAP_t_orc_UidValidity(long uid)
        {
            m_Uid = uid;
        }


        #region static method Parse

        /// <summary>
        /// Parses UIDVALIDITY optional response from reader.
        /// </summary>
        /// <param name="r">UIDVALIDITY optional response reader.</param>
        /// <returns>Returns UIDVALIDITY optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public new static IMAP_t_orc_UidValidity Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string[] code_value = r.ReadParenthesized().Split(new char[]{' '},2);
            if(!string.Equals("UIDVALIDITY",code_value[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid UIDVALIDITY response value.","r");
            }
            if(code_value.Length != 2){
                throw new ArgumentException("Invalid UIDVALIDITY response value.","r");
            }

            return new IMAP_t_orc_UidValidity(Convert.ToInt64(code_value[1]));
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "UIDVALIDITY " + m_Uid;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets mailbox(folder) UID value.
        /// </summary>
        public long Uid
        {
            get{ return m_Uid; }
        }

        #endregion
    }
}
