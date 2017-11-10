using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Filter object represents user filter in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Filter
    {
        private VirtualServer    m_pVirtualServer = null;
        private FilterCollection m_pOwner         = null;
        private string           m_ID             = "";
        private long             m_Cost           = 0;
        private bool             m_Enabled        = false;
        private string           m_Description    = "";
        private string           m_Assembly       = "";
        private string           m_Class          = "";
        private bool             m_ValuesChanged  = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        /// <param name="owner">Owner FilterCollection collection that owns this filter.</param>
        /// <param name="id">Filter ID.</param>
        /// <param name="cost">Filter process order, lower values are processed first.</param>
        /// <param name="enabled">Specifies if filter is enabled.</param>
        /// <param name="description">Filter description.</param>
        /// <param name="assembly">Filter assebly file.</param>
        /// <param name="filterClass">Filter class name.</param>
        internal Filter(VirtualServer virtualServer,FilterCollection owner,string id,long cost,bool enabled,string description,string assembly,string filterClass)
        {
            m_pVirtualServer = virtualServer;
            m_pOwner         = owner;
            m_ID             = id;
            m_Cost           = cost;
            m_Enabled        = enabled;
            m_Description    = description;
            m_Assembly       = assembly;
            m_Class          = filterClass;
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

            /* UpdateFilter <virtualServerID> "<filterID>" <cost> "<description>" "<assembly>" "<filterClass>" <enabled>
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */

            // Call TCP UpdateFilter
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateFilter " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_ID) + " " + 
                m_Cost + " " +
                TextUtils.QuoteString(m_Description) + " " + 
                TextUtils.QuoteString(m_Assembly) + " " + 
                TextUtils.QuoteString(m_Class) + " " + 
                m_Enabled
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner FilterCollection that owns this filter.
        /// </summary>
        public FilterCollection Owner
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
        /// Gets filter ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets or sets if filter is enabled.
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
        /// Gets or sets filter description.
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
        /// Gets or sets filer assembly name.
        /// </summary>
        public string AssemblyName
        {
            get{ return m_Assembly; }

            set{
                if(m_Assembly != value){
                    m_Assembly = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets filter class.
        /// </summary>
        public string Class
        {
            get{ return m_Class; }

            set{
                if(m_Class != value){
                    m_Class = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
