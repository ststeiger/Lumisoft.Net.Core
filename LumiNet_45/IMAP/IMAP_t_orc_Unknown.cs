using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents unknwon IMAP server optional response.
    /// </summary>
    public class IMAP_t_orc_Unknown : IMAP_t_orc
    {
        private string m_Value = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Optional response value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public IMAP_t_orc_Unknown(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            m_Value = value;
        }


        #region static method Parse

        /// <summary>
        /// Parses unknown optional response from reader.
        /// </summary>
        /// <param name="r">Unknown optional response reader.</param>
        /// <returns>Returns unknown optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public new static IMAP_t_orc_Unknown Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            return new IMAP_t_orc_Unknown(r.ReadParenthesized());
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_Value;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Optional response value.
        /// </summary>
        public string Value
        {
            get{ return m_Value; }
        }

        #endregion
    }
}
