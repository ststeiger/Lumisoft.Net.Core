using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Domain object represents domain in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Domain
    {
        private DomainCollection m_pOwner        = null;
        private string           m_DomainID      = "";
        private string           m_DomainName    = "";
        private string           m_Description   = "";
        private bool             m_ValuesChanged = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner DomainCollection that owns this domain.</param>
        /// <param name="id">Domain ID.</param>
        /// <param name="name">Domain name.</param>
        /// <param name="description">Domain decription.</param>
        internal Domain(DomainCollection owner,string id,string name,string description)
        {
            m_pOwner      = owner;
            m_DomainID    = id;
            m_DomainName  = name;
            m_Description = description; 
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

            /* UpdateDomain <virtualServerID> "<domainID>" "<domainName>" "<description>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP UpdateDomain
            m_pOwner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateDomain " + 
                m_pOwner.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_DomainID) + " " + 
                TextUtils.QuoteString(m_DomainName) + " " + 
                TextUtils.QuoteString(m_Description)
            );
                        
            string response = m_pOwner.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
            m_pOwner.VirtualServer.DomainChanged();
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets if this domain object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets onwer DomainCollection that owns this domain.
        /// </summary>
        public DomainCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets domain ID.
        /// </summary>
        public string DomainID
        {
            get{ return m_DomainID; }
        }

        /// <summary>
        /// Gets or sets domain name.
        /// </summary>
        public string DomainName
        {
            get{ return m_DomainName; }

            set{
                if(m_DomainName != value){
                    m_DomainName = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets domain description.
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

        #endregion

    }
}
