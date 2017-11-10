using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The IPSecurity object represents IP security entry in LumiSoft Mail Server virtual server.
    /// </summary>
    public class IPSecurity
    {
        private IPSecurityCollection  m_pOwner        = null;
        private string                m_ID            = "";
        private bool                  m_Enabled       = false;
        private string                m_Description   = "";
        private Service_enum          m_Service       = 0;
        private IPSecurityAction_enum m_Action        = 0;
        private IPAddress             m_pStartIP      = null;
        private IPAddress             m_pEndIP        = null;
        private bool                  m_ValuesChanged = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner IPSecurityCollection collection that owns this object.</param>
        /// <param name="id">Security entry ID.</param>
        /// <param name="enabled">Specifies if security entry is enabled.</param>
        /// <param name="description">Security entry description text.</param>
        /// <param name="service">Specifies service for what security entry applies.</param>
        /// <param name="action">Specifies what action done if IP matches to security entry range.</param>
        /// <param name="startIP">Range start IP.</param>
        /// <param name="endIP">Range end IP.</param>
        internal IPSecurity(IPSecurityCollection owner,string id,bool enabled,string description,Service_enum service,IPSecurityAction_enum action,IPAddress startIP,IPAddress endIP)
        {
            m_pOwner      = owner;
            m_ID          = id;
            m_Enabled     = enabled;
            m_Description = description;
            m_Service     = service;
            m_Action      = action;
            m_pStartIP    = startIP;
            m_pEndIP      = endIP;
        }


        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't chnaged, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }

            /* UpdateIPSecurityEntry <virtualServerID> "<securityEntryID>" enabled "<description>" <service> <action> "<startIP>" "<endIP>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            string id = Guid.NewGuid().ToString();

            // Call TCP UpdateIPSecurityEntry
            m_pOwner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateIPSecurityEntry " + 
                m_pOwner.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_ID) + " " + 
                m_Enabled + " " + 
                TextUtils.QuoteString(m_Description) + " " +
                (int)m_Service + " " +
                (int)m_Action + " " +
                TextUtils.QuoteString(m_pStartIP.ToString()) + " " +
                TextUtils.QuoteString(m_pEndIP.ToString())
            );
                        
            string response = m_pOwner.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner IPSecurityCollection that owns this object.
        /// </summary>
        public IPSecurityCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets if this object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets IP security entry ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets or sets if IP security entry is enabled.
        /// </summary>
        public bool Enabled
        {
            get{ return m_Enabled; }

            set{
                if(m_Enabled != value){
                    m_Enabled = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets description text.
        /// </summary>
        public string Description
        {
            get{ return m_Description; }

            set{
                if(m_Description != value){
                    m_Description = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets service.
        /// </summary>
        public Service_enum Service
        {
            get{ return m_Service; }

            set{
                if(m_Service != value){
                    m_Service = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets action.
        /// </summary>
        public IPSecurityAction_enum Action
        {
            get{ return m_Action; }

            set{
                if(m_Action != value){
                    m_Action = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets range start IP address.
        /// </summary>
        public IPAddress StartIP
        {
            get{ return m_pStartIP; }

            set{
                if(m_pStartIP != value){
                    m_pStartIP = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets range end IP address.
        /// </summary>
        public IPAddress EndIP
        {
            get{ return m_pEndIP; }

            set{
                if(m_pEndIP != value){
                    m_pEndIP = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
