using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.MIME;
using LumiSoft.Net.IMAP;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP server FETCH BODYSTRUCTURE single-part entity. Defined in RFC 3501 7.4.2.
    /// </summary>
    public class IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart : IMAP_t_Fetch_r_i_BodyStructure_e
    {
        private MIME_h_ContentType        m_pContentType            = null;
        private string                    m_ContentID               = null;
        private string                    m_ContentDescription      = null;
        private string                    m_ContentTransferEncoding = null;
        private long                      m_ContentSize             = -1;
        private int                       m_LinesCount              = -1;
        private string                    m_Md5                     = null;
        private MIME_h_ContentDisposition m_pContentDisposition     = null;
        private string                    m_Language                = null;
        private string                    m_Location                = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart()
        {
        }


        #region static method Parse

        /// <summary>
        /// Parses IMAP FETCH BODYSTRUCTURE single part entity from reader.
        /// </summary>
        /// <param name="r">Fetch reader.</param>
        /// <returns>Returns parsed bodystructure entity.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public static IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart retVal = new IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart();

            /* RFC 3501 7.4.2.
                
                Normal single part entity fields.
                    body type
                    body subtype
                    body parameter parenthesized list
                    body id
                    body description
                    body encoding
                    body size - Encoded bytes count.
                    --- extention fields
            
                Message/xxx type entity fields.
                    body type
                    body subtype
                    body parameter parenthesized list
                    body id
                    body description
                    body encoding
                    body size - Encoded bytes count.
                    --- message special fields
                    envelope structure
                    body structure
                    body encoded text lines count
                    --- extention fields
            
                Text/xxx type entity fields.
                    body type
                    body subtype
                    body parameter parenthesized list
                    body id
                    body description
                    body encoding
                    body size - Encoded bytes count.
                    --- text special fields
                    body encoded text lines count
                    --- extention fields
             
                Extention fields.
                    body MD5
                    body disposition
                    body language
                    body location
            */

            #region Common fields

            // body type
            // body subtype
            // body parameter parenthesized list
            string type    = IMAP_Utils.ReadString(r);
            string subtype = IMAP_Utils.ReadString(r);
            if(!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(subtype)){
                retVal.m_pContentType = new MIME_h_ContentType(type + "/" + subtype);
            }
            r.ReadToFirstChar();
            // Parse Content-Type parameters
            if(r.StartsWith("(")){
                StringReader pramsReader = new StringReader(r.ReadParenthesized());
                if(retVal.m_pContentType != null){
                    while(pramsReader.Available > 0){
                        string name = IMAP_Utils.ReadString(pramsReader);
                        if(string.IsNullOrEmpty(name)){
                            break;
                        }
                        string value = IMAP_Utils.ReadString(pramsReader);
                        if(value == null){
                            value = "";
                        }
                        retVal.m_pContentType.Parameters[name] = MIME_Encoding_EncodedWord.DecodeTextS(value);
                    }
                }
            }
            // NIL
            else{
                IMAP_Utils.ReadString(r);
            }

            // body id - nstring
            retVal.m_ContentID = IMAP_Utils.ReadString(r);

            // body description - nstring
            retVal.m_ContentDescription = IMAP_Utils.ReadString(r);

            // body encoding - string
            retVal.m_ContentTransferEncoding = IMAP_Utils.ReadString(r);

            // body size - Encoded bytes count.
            string size = IMAP_Utils.ReadString(r);
            if(string.IsNullOrEmpty(size)){
                retVal.m_ContentSize = -1;
            }
            else{
                retVal.m_ContentSize = Convert.ToInt64(size);
            }

            #endregion

            #region Text/xxx fields

            if(string.Equals("text",type,StringComparison.InvariantCultureIgnoreCase)){
                // body encoded text lines count
                string linesCount = IMAP_Utils.ReadString(r);
                if(string.IsNullOrEmpty(size)){
                    retVal.m_LinesCount = -1;
                }
                else{
                    retVal.m_LinesCount = Convert.ToInt32(linesCount);
                }
            }

            #endregion

            #region Message/xxx fields

            if(string.Equals("message",type,StringComparison.InvariantCultureIgnoreCase)){
                // envelope structure
                r.ReadToFirstChar();
                // Read ENVELOPE
                if(r.StartsWith("(")){
                    string prams = r.ReadParenthesized();
                }
                // NIL
                else{
                    IMAP_Utils.ReadString(r);
                }
                
                // body structure
                r.ReadToFirstChar();
                // Read BODYSTRUCTURE
                if(r.StartsWith("(")){
                    string prams = r.ReadParenthesized();
                }
                // NIL
                else{
                    IMAP_Utils.ReadString(r);
                }

                // body encoded text lines count
                string linesCount = IMAP_Utils.ReadString(r);
                if(string.IsNullOrEmpty(size)){
                    retVal.m_LinesCount = -1;
                }
                else{
                    retVal.m_LinesCount = Convert.ToInt32(linesCount);
                }
            }

            #endregion

            #region Extention fields

            // body MD5 - nstring
            retVal.m_Md5 = IMAP_Utils.ReadString(r);

            // body disposition - "(" string SP body-fld-param ")" / nil
            //                    body-fld-param  = "(" string SP string *(SP string SP string) ")" / nil            
            if(r.StartsWith("(")){
                string disposition = IMAP_Utils.ReadString(r);
                if(!string.IsNullOrEmpty(disposition)){
                    retVal.m_pContentDisposition = new MIME_h_ContentDisposition(disposition);
                }
                r.ReadToFirstChar();

                // Parse Content-Dispostion parameters.
                if(r.StartsWith("(")){
                    StringReader pramsReader = new StringReader(r.ReadParenthesized());
                    if(retVal.m_pContentDisposition != null){
                        while(pramsReader.Available > 0){
                            string name = IMAP_Utils.ReadString(pramsReader);
                            if(string.IsNullOrEmpty(name)){
                                break;
                            }
                            string value = IMAP_Utils.ReadString(pramsReader);
                            if(value == null){
                                value = "";
                            }
                            retVal.m_pContentDisposition.Parameters[name] = MIME_Encoding_EncodedWord.DecodeTextS(value);                            
                        }
                    }
                }
                // NIL
                else{
                    IMAP_Utils.ReadString(r);
                }               
            }
            // NIL
            else{
                IMAP_Utils.ReadString(r);
            }

            // body language - nstring / "(" string *(SP string) ")"
            r.ReadToFirstChar();
            if(r.StartsWith("(")){
                retVal.m_Language = r.ReadParenthesized();
            }
            else{
                retVal.m_Language = IMAP_Utils.ReadString(r);
            }            

            // body location - nstring
            retVal.m_Location = IMAP_Utils.ReadString(r);

            #endregion

            return retVal;
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets Content-Type header value. Value null means not specified.
        /// </summary>
        public override MIME_h_ContentType ContentType
        {
            get{ return m_pContentType; }
        }

        /// <summary>
        /// Gets Content-ID header value. Value null means not specified.
        /// </summary>
        public string ContentID
        {
            get{ return m_ContentID; }
        }

        /// <summary>
        /// Gets Content-Description header value. Value null means not specified.
        /// </summary>
        public string ContentDescription
        {
            get{ return m_ContentDescription; }
        }

        /// <summary>
        /// Gets Content-Transfer-Encoding header value. Value null means not specified.
        /// </summary>
        public string ContentTransferEncoding
        {
            get{ return m_ContentTransferEncoding; }
        }

        /// <summary>
        /// Gets encoded content size in bytes. Value -1 means not specified.
        /// </summary>
        public long ContentSize
        {
            get{ return m_ContentSize; }
        }

        /// <summary>
        /// Gets encoded content lines count. This property is available only for Content-Type: text/xxx and message/xx. Value -1 means not specified.
        /// </summary>
        public int LinesCount
        {
            get{ return m_LinesCount; }
        }

        /// <summary>
        /// Gets content MD5 hash. Value null means not specified.
        /// </summary>
        public string Md5
        {
            get{ return m_Md5; }
        }

        /// <summary>
        /// Gets Content-Disposition header value. Value null means not specified.
        /// </summary>
        public override MIME_h_ContentDisposition ContentDisposition
        {
            get{ return m_pContentDisposition; }
        }

        /// <summary>
        /// Gets content language. Value null means not specified.
        /// </summary>
        public override string Language
        {
            get{ return m_Language; }
        }

        /// <summary>
        /// Gets content location. Value null means not specified.
        /// </summary>
        public override string Location
        {
            get{ return m_Location; }
        }

        #endregion
    }
}
