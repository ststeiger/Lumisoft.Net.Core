using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>PERMANENTFLAGS</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_PermanentFlags : IMAP_t_orc
    {
        private string[] m_pFlags = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="flags">List of supported permanent flags.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>flags</b> is null reference.</exception>
        public IMAP_t_orc_PermanentFlags(string[] flags)
        {
            if(flags == null){
                throw new ArgumentNullException("flags");
            }

            m_pFlags = flags;
        }


        #region static method Parse

        /// <summary>
        /// Parses PERMANENTFLAGS optional response from string.
        /// </summary>
        /// <param name="r">PERMANENTFLAGS optional response reader.</param>
        /// <returns>Returns PERMANENTFLAGS optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public new static IMAP_t_orc_PermanentFlags Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            if(!r.StartsWith("[PERMANENTFLAGS",false)){
                throw new ArgumentException("Invalid PERMANENTFLAGS response value.","r");
            }

            // Read [
            r.ReadSpecifiedLength(1);
            // Read PERMANENTFLAGS
            r.ReadWord();
            r.ReadToFirstChar();
            string[] flags = r.ReadParenthesized().Split(' ');
            // Read ]
            r.ReadSpecifiedLength(1);

            return new IMAP_t_orc_PermanentFlags(flags);
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PERMANENTFLAGS (" + Net_Utils.ArrayToString(m_pFlags," ") + ")";
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets list of supported permanent flags.
        /// </summary>
        public string[] Flags
        {
            get{ return m_pFlags; }
        }

        #endregion
    }
}
