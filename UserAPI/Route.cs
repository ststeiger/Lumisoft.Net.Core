using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Route object represents route in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Route
    {
        private RouteCollection m_pOwner        = null;
        private string          m_ID            = "";
        private long            m_Cost          = 0;
        private string          m_Description   = "";
        private string          m_Pattern       = "";
        private bool            m_Enabled       = false;
        private RouteActionBase m_pAction       = null;
        private bool            m_ValuesChanged = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner RouteCollection collection that owns this route.</param>
        /// <param name="id">Route ID.</param>
        /// <param name="cost">Rule cost.</param>
        /// <param name="descritpion">Route description text.</param>
        /// <param name="pattern">Routing pattern.</param>
        /// <param name="enabled">Specifies if route is enabled.</param>
        /// <param name="action">Route action.</param>
        internal Route(RouteCollection owner,string id,long cost,string descritpion,string pattern,bool enabled,RouteActionBase action)
        {
            m_pOwner      = owner;
            m_ID          = id;
            m_Cost        = cost;
            m_Description = descritpion;
            m_Pattern     = pattern;
            m_Enabled     = enabled;
            m_pAction     = action;
        }

        
        #region method Commit

        /// <summary>
        /// Tries to save all changed values to server. Throws Exception if fails.
        /// </summary>
        public void Commit()
        {
            // Values haven't changed, so just skip saving.
            if(!m_ValuesChanged){
                return;
            }

            /* UpdateRoute <virtualServerID> "<routeID>" <cost> "<description>" "<pattern>" <enabled> <actionType> "<actionData>:base64"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            // Call TCP UpdateRoute
            m_pOwner.VirtualServer.Server.TcpClient.TcpStream.WriteLine("UpdateRoute " + 
                m_pOwner.VirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(m_ID) + " " + 
                m_Cost + " " +
                TextUtils.QuoteString(m_Description) + " " + 
                TextUtils.QuoteString(m_Pattern) + " " + 
                m_Enabled + " " +
                (int)m_pAction.ActionType + " " +
                Convert.ToBase64String(m_pAction.Serialize())
            );
                        
            string response = m_pOwner.VirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_ValuesChanged = false;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets owner RouteCollection that owns this route.
        /// </summary>
        public RouteCollection Owner
        {
            get{ return m_pOwner; }
        }

        /// <summary>
        /// Gets if this route object has changes what isn't stored to mail server by calling Commit().
        /// </summary>
        public bool HasChanges
        {
            get{ return m_ValuesChanged; }
        }

        /// <summary>
        /// Gets route ID.
        /// </summary>
        public string ID
        {
            get{ return m_ID; }
        }

        /// <summary>
        /// Gets or sets route description text.
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
        /// Gets or sets pattern text.
        /// </summary>
        public string Pattern
        {
            get{ return m_Pattern; }

            set{
                if(m_Pattern != value){
                    m_Pattern = value;

                    m_ValuesChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets if route is enabled.
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
        /// Gets or sets route action.
        /// </summary>
        public RouteActionBase Action
        {
            get{ return m_pAction; }

            set{
                if(value == null){
                    throw new NullReferenceException("Action value can't be null !");
                }

                // See if action changed
                bool changed = false;
                if(m_pAction.ActionType != value.ActionType){
                    changed = true;
                }
                else{
                    if(value.ActionType == RouteAction_enum.RouteToEmail){
                        if(((RouteAction_RouteToEmail)m_pAction).EmailAddress != ((RouteAction_RouteToEmail)value).EmailAddress){
                            changed = true;
                        }
                    }
                    else if(value.ActionType == RouteAction_enum.RouteToHost){
                        if(((RouteAction_RouteToHost)m_pAction).Host != ((RouteAction_RouteToHost)value).Host){
                            changed = true;
                        }
                        if(((RouteAction_RouteToHost)m_pAction).Port != ((RouteAction_RouteToHost)value).Port){
                            changed = true;
                        }
                    }
                    else if(value.ActionType == RouteAction_enum.RouteToMailbox){
                        if(((RouteAction_RouteToMailbox)m_pAction).Mailbox != ((RouteAction_RouteToMailbox)value).Mailbox){
                            changed = true;
                        }
                    }
                }

                if(changed){
                    m_pAction = value;

                    m_ValuesChanged = true;
                }
            }
        }

        #endregion

    }
}
