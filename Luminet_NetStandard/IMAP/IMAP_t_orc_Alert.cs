using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>ALERT</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_Alert : IMAP_t_orc
    {
        private string m_AlertText = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="text">Alert text.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> is null reference.</exception>
        public IMAP_t_orc_Alert(string text)
        {
            if(text == null){
                throw new ArgumentNullException("text");
            }

            m_AlertText = text;
        }


        #region static method Parse

        /// <summary>
        /// Parses ALERT optional response from reader.
        /// </summary>
        /// <param name="r">ALERT optional response reader.</param>
        /// <returns>Returns ALERT optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public new static IMAP_t_orc_Alert Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string[] code_value = r.ReadParenthesized().Split(new char[]{' '},2);
            if(!string.Equals("ALERT",code_value[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid ALERT response value.","r");
            }

            return new IMAP_t_orc_Alert(code_value.Length == 2 ? code_value[1] : "");
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ALERT " + m_AlertText;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets alert text.
        /// </summary>
        public string AlertText
        {
            get{ return m_AlertText; }
        }

        #endregion
    }
}
