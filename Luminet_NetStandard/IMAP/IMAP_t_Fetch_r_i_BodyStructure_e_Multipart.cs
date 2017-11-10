using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.MIME;
using LumiSoft.Net.IMAP;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP server FETCH BODYSTRUCTURE multipart entity. Defined in RFC 3501 7.4.2.
    /// </summary>
    public class IMAP_t_Fetch_r_i_BodyStructure_e_Multipart : IMAP_t_Fetch_r_i_BodyStructure_e
    {
        private MIME_h_ContentType                     m_pContentType        = null;
        private MIME_h_ContentDisposition              m_pContentDisposition = null;
        private string                                 m_Language            = null;
        private string                                 m_Location            = null;
        private List<IMAP_t_Fetch_r_i_BodyStructure_e> m_pBodyParts          = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private IMAP_t_Fetch_r_i_BodyStructure_e_Multipart()
        {
            m_pBodyParts = new List<IMAP_t_Fetch_r_i_BodyStructure_e>();
        }


        #region static method Parse

        /// <summary>
        /// Parses IMAP FETCH BODYSTRUCTURE multipart entity from reader.
        /// </summary>
        /// <param name="r">Fetch reader.</param>
        /// <returns>Returns parsed bodystructure entity.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public static IMAP_t_Fetch_r_i_BodyStructure_e_Multipart Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            IMAP_t_Fetch_r_i_BodyStructure_e_Multipart retVal = new IMAP_t_Fetch_r_i_BodyStructure_e_Multipart();

            /* RFC 3501 7.4.2.
            
                Normal fields.
                    1*(body-parts)  - parenthesized list of body parts
                    subtype
                    --- extention fields
              
                Extention fields.
                    body parameter parenthesized list
                    body disposition
                    body language
                    body location
            */

            #region Normal fields

            // Read child entities.
            while(r.Available > 0){
                r.ReadToFirstChar();
                if(r.StartsWith("(")){
                    StringReader bodyPartReader = new StringReader(r.ReadParenthesized());
                    bodyPartReader.ReadToFirstChar();

                    IMAP_t_Fetch_r_i_BodyStructure_e part = null;
                    // multipart
                    if(bodyPartReader.StartsWith("(")){
                        part = IMAP_t_Fetch_r_i_BodyStructure_e_Multipart.Parse(bodyPartReader);
                    }
                    // single-part
                    else{
                        part = IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart.Parse(bodyPartReader);
                       
                    }
                    part.SetParent(retVal);
                    retVal.m_pBodyParts.Add(part);
                }
                else{
                    break;
                }
            }

            // subtype
            string subtype = IMAP_Utils.ReadString(r);
            if(!string.IsNullOrEmpty(subtype)){
                retVal.m_pContentType = new MIME_h_ContentType("multipart/" + subtype);
            }

            #endregion

            #region Extention field

            // body parameter parenthesized list
            r.ReadToFirstChar();
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


        #region method IndexOfBodyPart

        /// <summary>
        /// Gets the specified body part zero-based index number.
        /// </summary>
        /// <param name="bodyPart">Body part.</param>
        /// <returns>Return specified body part zero-based index number.</returns>
        internal int IndexOfBodyPart(IMAP_t_Fetch_r_i_BodyStructure_e bodyPart)
        {
            return m_pBodyParts.IndexOf(bodyPart);
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

        /// <summary>
        /// Gets multipart child body parts.
        /// </summary>
        public IMAP_t_Fetch_r_i_BodyStructure_e[] BodyParts
        {
            get{ return m_pBodyParts.ToArray(); }
        }

        #endregion
    }
}
