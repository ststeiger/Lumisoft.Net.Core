using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Group object represents user group in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Group
    {
        private VirtualServer         m_pVirtualServer = null;
        private GroupCollection       m_pOwner         = null;
        private string                m_GroupID        = "";
        private string                m_GroupName      = "";
        private string                m_Description    = "";
        private bool                  m_Enabled        = false;
        private GroupMemberCollection m_pMembers       = null;
        private bool                  m_ValuesChanged  = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <param name="owner">Owner GroupCollection collection that owns this group.</param>
        /// <param name="id">Group ID.</param>
        /// <param name="name">Group name.</param>
        /// <param name="descritpion">Group description.</param>
        /// <param name="enabled">Specifies if group is enabled.</param>
        internal Group(VirtualServer virtualServer,GroupCollection owner,string id,string name,string descritpion,bool enabled)
        {
            m_pVirtualServer = virtualServer;
            m_pOwner         = owner;
            m_GroupID        = id;
            m_GroupName      = name;
            m_Description    = descritpion;
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

            /* UpdateGroup <virtualServerID> "<groupID>" "<groupName>" "<description>" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP UpdateGroup
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateGroup " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(m_GroupID) + " " + TextUtils.QuoteString(m_GroupName) + " " + TextUtils.QuoteString(m_Description) + " " + m_Enabled);
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets the VirtualServer object that is the owner of this group.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets onwer GroupCollection that owns this group.
        /// </summary>
        public GroupCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets if this group object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets group ID.
        /// </summary>
        public string GroupID
        {
            get{ return m_GroupID; }
        }

        /// <summary>
        /// Gets or sets group name.
        /// </summary>
        public string GroupName
        {
            get{ return m_GroupName; }

            set{
                if(m_GroupName != value){
                    m_GroupName = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets group description.
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
        /// Gets or sets if group is enabled.
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
        /// Gets group members.
        /// </summary>
        public GroupMemberCollection Members
        {
            get{ 
                // Not created yet, create it. We use late binding here, 
                // create objects and bind them when we need them.
                if(m_pMembers == null){
                    m_pMembers = new GroupMemberCollection(this);
                }

                return m_pMembers; 
            }
        }

        #endregion

    }
}
