using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The MailingList object represents mailing list in LumiSoft Mail Server virtual server.
    /// </summary>
    public class MailingList
    {
        private VirtualServer               m_pVirtualServer = null;
        private MailingListCollection       m_pOwner         = null;
        private string                      m_ID             = null;
        private string                      m_Name           = null;
        private string                      m_Description    = null;
        private bool                        m_Enabled        = false;
        private MailingListMemberCollection m_pMembers       = null;
        private MailingListAclCollection    m_pAcl           = null;
        private bool                        m_ValuesChanged  = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <param name="owner">Owner MailingListCollection collection that owns this mailing list.</param>
        /// <param name="id">Mailing list ID.</param>
        /// <param name="name">Mailing list name.</param>
        /// <param name="description">Mailing list description.</param>
        /// <param name="enabled">Specifies if mailing list is enabled.</param>
        internal MailingList(VirtualServer virtualServer,MailingListCollection owner,string id,string name,string description,bool enabled)
        {
            m_pVirtualServer = virtualServer;
            m_pOwner         = owner;
            m_ID             = id;
            m_Name           = name;
            m_Description    = description;
            m_Enabled        = enabled;
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

            /* UpdateMailingList <virtualServerID> "<mailingListID>" "<mailingListName>" "<description>" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP UpdateMailingList
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateMailingList " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_ID) + " " + TextUtils.QuoteString(m_Name) + " " + TextUtils.QuoteString(m_Description) + " " + m_Enabled);
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this mailing list.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets onwer MailingListCollection that owns this mailing list.
        /// </summary>
        public MailingListCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets mailing list ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets or sets mailing list name.
        /// </summary>
        public string Name
        {
            get{ return m_Name; }

            set{
                if(m_Name != value){
                    m_Name = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets mailing list description.
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
        /// Gets or sets if mailing list enabled.
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
        /// Gets mailing list members.
        /// </summary>
        public MailingListMemberCollection Members
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pMembers == null){
                    m_pMembers = new MailingListMemberCollection(this);
                }

                return m_pMembers;
            }
        }

        /// <summary>
        /// Gets mailing list ACL.
        /// </summary>
        public MailingListAclCollection ACL
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pAcl == null){
                    m_pAcl = new MailingListAclCollection(this);
                }

                return m_pAcl;
            }
        }

        #endregion

    }
}
