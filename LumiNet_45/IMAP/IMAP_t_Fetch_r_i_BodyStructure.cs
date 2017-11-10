using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP FETCH response BODYSTRUCTURE data-item. Defined in RFC 3501 7.4.2.
    /// </summary>
    public class IMAP_t_Fetch_r_i_BodyStructure : IMAP_t_Fetch_r_i
    {
        private IMAP_t_Fetch_r_i_BodyStructure_e m_pMessage = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private IMAP_t_Fetch_r_i_BodyStructure()
        {
        }
                

        #region static method Parse

        /// <summary>
        /// Parses IMAP FETCH BODYSTRUCTURE from reader.
        /// </summary>
        /// <param name="r">Fetch reader.</param>
        /// <returns>Returns parsed bodystructure.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        public static IMAP_t_Fetch_r_i_BodyStructure Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            IMAP_t_Fetch_r_i_BodyStructure retVal = new IMAP_t_Fetch_r_i_BodyStructure();
            r.ReadToFirstChar();
            // We have multipart message
            if(r.StartsWith("(")){
                retVal.m_pMessage = IMAP_t_Fetch_r_i_BodyStructure_e_Multipart.Parse(r);
            }
            // We have single-part message.
            else{
                 retVal.m_pMessage = IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart.Parse(r);
            }

            return retVal;
        }

        #endregion


        #region method GetAttachments

        /// <summary>
        /// Gets message attachments.
        /// </summary>
        /// <param name="includeInline">Specifies if 'inline' entities are included.</param>
        /// <returns>Returns message attachments.</returns>
        public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart[] GetAttachments(bool includeInline)
        {
            List<IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart> retVal = new List<IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart>();
            foreach(IMAP_t_Fetch_r_i_BodyStructure_e entity in this.AllEntities){
                MIME_h_ContentType        contentType = entity.ContentType;
                MIME_h_ContentDisposition disposition = entity.ContentDisposition;

                if(entity is IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart){
                    if(disposition != null && string.Equals(disposition.DispositionType,"attachment",StringComparison.InvariantCultureIgnoreCase)){
                        retVal.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)entity);
                    }
                    else if(disposition != null && string.Equals(disposition.DispositionType,"inline",StringComparison.InvariantCultureIgnoreCase)){
                        if(includeInline){
                            retVal.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)entity);
                        }
                    }
                    else if(contentType != null && string.Equals(contentType.Type,"application",StringComparison.InvariantCultureIgnoreCase)){
                        retVal.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)entity);
                    }
                    else if(contentType != null && string.Equals(contentType.Type,"image",StringComparison.InvariantCultureIgnoreCase)){
                        retVal.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)entity);
                    }
                    else if(contentType != null && string.Equals(contentType.Type,"video",StringComparison.InvariantCultureIgnoreCase)){
                        retVal.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)entity);
                    }
                    else if(contentType != null && string.Equals(contentType.Type,"audio",StringComparison.InvariantCultureIgnoreCase)){
                        retVal.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)entity);
                    }
                    else if(contentType != null && string.Equals(contentType.Type,"message",StringComparison.InvariantCultureIgnoreCase)){
                        retVal.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)entity);
                    }
                }
            }

            return retVal.ToArray();
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets if message contains signed data.
        /// </summary>
        public bool IsSigned
        {
            get{
                foreach(IMAP_t_Fetch_r_i_BodyStructure_e entity in this.AllEntities){
                    if(string.Equals(entity.ContentType.TypeWithSubtype,MIME_MediaTypes.Application.pkcs7_mime,StringComparison.InvariantCultureIgnoreCase)){
                        return true;
                    }
                    else if(string.Equals(entity.ContentType.TypeWithSubtype,MIME_MediaTypes.Multipart.signed,StringComparison.InvariantCultureIgnoreCase)){
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets message entity.
        /// </summary>
        public IMAP_t_Fetch_r_i_BodyStructure_e Message
        {
            get{ return m_pMessage; }
        }
        
        /// <summary>
        /// Gets all MIME entities as list.
        /// </summary>
        public IMAP_t_Fetch_r_i_BodyStructure_e[] AllEntities
        {
            get{
                List<IMAP_t_Fetch_r_i_BodyStructure_e> retVal        = new List<IMAP_t_Fetch_r_i_BodyStructure_e>();
                List<IMAP_t_Fetch_r_i_BodyStructure_e> entitiesQueue = new List<IMAP_t_Fetch_r_i_BodyStructure_e>();
                entitiesQueue.Add(m_pMessage);
            
                while(entitiesQueue.Count > 0){
                    IMAP_t_Fetch_r_i_BodyStructure_e currentEntity = entitiesQueue[0];
                    entitiesQueue.RemoveAt(0);
        
                    retVal.Add(currentEntity);

                    // Current entity is multipart entity, add it's body-parts for processing.
                    if(currentEntity is IMAP_t_Fetch_r_i_BodyStructure_e_Multipart){
                        IMAP_t_Fetch_r_i_BodyStructure_e[] bodyParts = ((IMAP_t_Fetch_r_i_BodyStructure_e_Multipart)currentEntity).BodyParts;
                        for(int i=0;i<bodyParts.Length;i++){
                            entitiesQueue.Insert(i,bodyParts[i]);
                        }
                    }
                }

                return retVal.ToArray();
            }
        }

        /// <summary>
        /// Gets attachment entities. Content-Disposition "inline" not included, use GetAttachments method which allows to include "inline".
        /// </summary>
        public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart[] Attachments
        {
            get{ return GetAttachments(false); }
        }

        /// <summary>
        /// Gets first text/plain body entity, returns null if no such entity.
        /// </summary>
        public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart BodyTextEntity
        {
            get{ 
                foreach(IMAP_t_Fetch_r_i_BodyStructure_e e in this.AllEntities){
                    if(string.Equals(e.ContentType.TypeWithSubtype,MIME_MediaTypes.Text.plain,StringComparison.InvariantCultureIgnoreCase)){
                        return (IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)e;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets first text/html body entity, returns null if no such entity.
        /// </summary>
        public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart BodyTextHtmlEntity
        {
            get{ 
                foreach(IMAP_t_Fetch_r_i_BodyStructure_e e in this.AllEntities){
                    if(string.Equals(e.ContentType.TypeWithSubtype,MIME_MediaTypes.Text.html,StringComparison.InvariantCultureIgnoreCase)){
                        return (IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)e;
                    }
                }

                return null;
            }
        }

        #endregion
    }
}
