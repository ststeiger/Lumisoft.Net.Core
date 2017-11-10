using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is base class for any IMAP server optional response codes. Defined in RFC 3501 7.1.
    /// </summary>
    public abstract class IMAP_t_orc
    {
        #region static method Parse

        /// <summary>
        /// Parses IMAP optional response from reader.
        /// </summary>
        /// <param name="r">Optional response reader.</param>
        /// <returns>Returns parsed optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public static IMAP_t_orc Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }
            
            r.ReadToFirstChar();
                        
            if(r.StartsWith("[ALERT",false)){
                return IMAP_t_orc_Alert.Parse(r);
            }
            else if(r.StartsWith("[BADCHARSET",false)){
                return IMAP_t_orc_BadCharset.Parse(r);
            }
            else if(r.StartsWith("[CAPABILITY",false)){
                return IMAP_t_orc_Capability.Parse(r);
            }
            else if(r.StartsWith("[PARSE",false)){
                return IMAP_t_orc_Parse.Parse(r);
            }
            else if(r.StartsWith("[PERMANENTFLAGS",false)){
                return IMAP_t_orc_PermanentFlags.Parse(r);
            }
            else if(r.StartsWith("[READ-ONLY",false)){
                return IMAP_t_orc_ReadOnly.Parse(r);
            }
            else if(r.StartsWith("[READ-WRITE",false)){
                return IMAP_t_orc_ReadWrite.Parse(r);
            }
            else if(r.StartsWith("[TRYCREATE",false)){
                return IMAP_t_orc_TryCreate.Parse(r);
            }
            else if(r.StartsWith("[UIDNEXT",false)){
                return IMAP_t_orc_UidNext.Parse(r);
            }
            else if(r.StartsWith("[UIDVALIDITY",false)){
                return IMAP_t_orc_UidValidity.Parse(r);
            }
            else if(r.StartsWith("[UNSEEN",false)){
                return IMAP_t_orc_Unseen.Parse(r);
            }
            //---------------------
            else if(r.StartsWith("[APPENDUID",false)){
                return IMAP_t_orc_AppendUid.Parse(r);
            }
            else if(r.StartsWith("[COPYUID",false)){
                return IMAP_t_orc_CopyUid.Parse(r);
            }
            else{
                return IMAP_t_orc_Unknown.Parse(r);
            }
        }

        #endregion
    }
}
