using System;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net;
using LumiSoft.Net.SIP.Proxy;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The SIP_Settings object represents SIP settings in LumiSoft Mail Server virtual server.
    /// </summary>
    public class SIP_Settings
    {
        private System_Settings       m_pSysSettings = null;
        private bool                  m_Enabled      = false;
        private SIP_ProxyMode         m_ProxyMode    = SIP_ProxyMode.Registrar | SIP_ProxyMode.B2BUA;
        private int                   m_MinExpires   = 60;
        private IPBindInfo[]          m_pBinds       = null;
        private SIP_GatewayCollection m_pGateways    = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sysSettings">Reference to system settings.</param>
        /// <param name="enabled">Specifies if SIP service is enabled.</param>
        /// <param name="proxyMode">Specifies SIP proxy server opearion mode.</param>
        /// <param name="minExpires">SIP minimum content expire time in seconds.</param>
        /// <param name="bindings">Specifies SIP listening info.</param>
        /// <param name="gateways">SIP gateways.</param>
        internal SIP_Settings(System_Settings sysSettings,bool enabled,SIP_ProxyMode proxyMode,int minExpires,IPBindInfo[] bindings,SIP_GatewayCollection gateways)
        {
            m_pSysSettings = sysSettings;
            m_Enabled      = enabled;
            m_ProxyMode    = proxyMode;
            m_MinExpires   = minExpires;
            m_pBinds       = bindings;
            m_pGateways    = gateways;
        }

        #region Properties Implementation

        /// <summary>
        /// Gets or sets if IMAP server is enabled.
        /// </summary>
        public bool Enabled
        {
            get{ return m_Enabled; }

            set{
                if(m_Enabled != value){
                    m_Enabled = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets SIP proxy mode.
        /// </summary>
        public SIP_ProxyMode ProxyMode
        {
            get{ return m_ProxyMode; }

            set{
                if(m_ProxyMode != value){
                    m_ProxyMode = value;

                    m_pSysSettings.SetValuesChanged();
                }  
            }
        }

        /// <summary>
        /// Gets or set SIP minimum allowed content expire time in seconds.
        /// </summary>
        public int MinimumExpires
        {
            get{ return m_MinExpires; }

            set{
                if(value < 60){
                    throw new ArgumentException("Argument MinimumExpires value must be >= 60 !");
                }

                if(m_MinExpires != value){
                    m_MinExpires = value;

                    m_pSysSettings.SetValuesChanged();
                }                
            }
        }

        /// <summary>
        /// Gets IP bindings.
        /// </summary>
        public IPBindInfo[] Binds
        {
            get{ return m_pBinds; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Binds");
                }

                if(!Net_Utils.CompareArray(m_pBinds,value)){
                    m_pBinds = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets SIP gateways collection.
        /// </summary>
        public SIP_GatewayCollection Gateways
        {
            get{ return m_pGateways; }
        }

        #endregion

    }
}
