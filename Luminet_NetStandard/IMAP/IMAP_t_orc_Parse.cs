using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>PARSE</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_Parse : IMAP_t_orc
    {
        private string m_ErrorText = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="text">Parse error text.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> is null reference.</exception>
        public IMAP_t_orc_Parse(string text)
        {
            if(text == null){
                throw new ArgumentNullException("text");
            }

            m_ErrorText = text;
        }


        #region static method Parse

        /// <summary>
        /// Parses PARSE optional response from reader.
        /// </summary>
        /// <param name="r">PARSE optional response reader.</param>
        /// <returns>Returns PARSE optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public new static IMAP_t_orc_Parse Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string[] code_value = r.ReadParenthesized().Split(new char[]{' '},2);
            if(!string.Equals("PARSE",code_value[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid PARSE response value.","r");
            }

            return new IMAP_t_orc_Parse(code_value.Length == 2 ? code_value[1] : "");
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PARSE " + m_ErrorText;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets parse error text.
        /// </summary>
        public string ErrorText
        {
            get{ return m_ErrorText; }
        }

        #endregion
    }
}
