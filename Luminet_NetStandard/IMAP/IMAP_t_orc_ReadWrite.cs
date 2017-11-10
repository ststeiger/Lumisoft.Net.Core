using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>READ-WRITE</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_ReadWrite : IMAP_t_orc
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IMAP_t_orc_ReadWrite()
        {
        }


        #region static method Parse

        /// <summary>
        /// Parses READ-WRITE optional response from reader.
        /// </summary>
        /// <param name="r">READ-WRITE optional response reader.</param>
        /// <returns>Returns READ-WRITE optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public new static IMAP_t_orc_ReadWrite Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string[] code_value = r.ReadParenthesized().Split(new char[]{' '},2);
            if(!string.Equals("READ-WRITE",code_value[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid READ-WRITE response value.","r");
            }

            return new IMAP_t_orc_ReadWrite();
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "READ-WRITE";
        }

        #endregion
    }
}
