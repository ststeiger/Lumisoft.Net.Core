using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using LumiSoft.Net;
using LumiSoft.Net.SMTP.Relay;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The Relay_Settings object represents Relay settings in LumiSoft Mail Server virtual server.
    /// </summary>
    public class Relay_Settings
    {
        private System_Settings   m_pSysSettings          = null;
        private Relay_Mode        m_RelayMode             = Relay_Mode.Dns;
        private BalanceMode       m_SmartHostsBalanceMode = BalanceMode.LoadBalance;
        private Relay_SmartHost[] m_pSmartHosts           = null;
        private int               m_IdleTimeout           = 0;
        private int               m_MaxConnections        = 0;
        private int               m_MaxConnectionsPerIP   = 0;
        private int               m_RelayInterval         = 0;
        private int               m_RelayRetryInterval    = 0;
        private int               m_SendUndelWaringAfter  = 0;
        private int               m_SendUndeliveredAfter  = 0;
        private bool              m_StoreUndeliveredMsgs  = false;
        private bool              m_UseTlsIfPossible      = false;
        private IPBindInfo[]      m_pBinds                = null;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sysSettings">Reference to system settings.</param>
        /// <param name="relayMode">Relay mode.</param>
        /// <param name="smartHostBalanceMode">Specifies how smart hosts will balance.</param>
        /// <param name="smartHosts">Smart hosts.</param>
        /// <param name="idleTimeout">Session idle timeout seconds.</param>
        /// <param name="maxConnections">Maximum conncurent conncetions.</param>
        /// <param name="maxConnectionsPerIP">Maximum conncurent conncetions to one IP address.</param>
        /// <param name="relayInterval">Relay messages interval seconds.</param>
        /// <param name="relayRetryInterval">Relay retry messages interval seconds.</param>
        /// <param name="undeliveredWarning">Specifies after how many minutes delayed delivery message is sent.</param>
        /// <param name="undelivered">Specifies after how many hours undelivered notification message is sent.</param>
        /// <param name="storeUndelivered">Specifies if undelivered messages are stored to "Undelivered" folder in mail store.</param>
        /// <param name="useTlsIfPossible">Specifies if TLS is used when remote server supports it.</param>
        /// <param name="bindings">Specifies SMTP listening info.</param>
        internal Relay_Settings(System_Settings sysSettings,Relay_Mode relayMode,BalanceMode smartHostBalanceMode,Relay_SmartHost[] smartHosts,int idleTimeout,int maxConnections,int maxConnectionsPerIP,int relayInterval,int relayRetryInterval,int undeliveredWarning,int undelivered,bool storeUndelivered,bool useTlsIfPossible,IPBindInfo[] bindings)
        {
            m_pSysSettings          = sysSettings;
            m_RelayMode             = relayMode;
            m_SmartHostsBalanceMode = smartHostBalanceMode;
            m_pSmartHosts           = smartHosts;
            m_IdleTimeout           = idleTimeout;
            m_MaxConnections        = maxConnections;
            m_MaxConnectionsPerIP   = maxConnectionsPerIP;
            m_RelayInterval         = relayInterval;
            m_RelayRetryInterval    = relayRetryInterval;
            m_SendUndelWaringAfter  = undeliveredWarning;
            m_SendUndeliveredAfter  = undelivered;
            m_StoreUndeliveredMsgs  = storeUndelivered;
            m_UseTlsIfPossible      = useTlsIfPossible;
            m_pBinds                = bindings;
        }


        #region Properties Implementation
             
        /// <summary>
        /// Gets or sets relay mode.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public Relay_Mode RelayMode
        {
            get{ 
                //if(m_IsDisposed){
                //    throw new ObjectDisposedException(this.GetType().Name);
                //}

                return m_RelayMode; 
            }

            set{
                //if(m_IsDisposed){
                //    throw new ObjectDisposedException(this.GetType().Name);
                //}

                if(m_RelayMode != value){
                    m_RelayMode = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets how smart hosts will be balanced.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public BalanceMode SmartHostsBalanceMode
        {
            get{ 
                //if(m_IsDisposed){
                //    throw new ObjectDisposedException(this.GetType().Name);
                //}

                return m_SmartHostsBalanceMode; 
            }

            set{
                //if(m_IsDisposed){
                //    throw new ObjectDisposedException(this.GetType().Name);
                //}

                if(m_SmartHostsBalanceMode != value){
                    m_SmartHostsBalanceMode = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets smart hosts. Smart hosts must be in priority order, lower index number means higher priority.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public Relay_SmartHost[] SmartHosts
        {
            get{
                //if(m_IsDisposed){
                //    throw new ObjectDisposedException(this.GetType().Name);
                //}

                return m_pSmartHosts;
            }

            set{
                //if(m_IsDisposed){
                //    throw new ObjectDisposedException(this.GetType().Name);
                //}
                if(value == null){
                    throw new ArgumentNullException("SmartHosts");
                }
                
                bool changed = false;
                if(value.Length != m_pSmartHosts.Length){
                    changed = true;
                }
                else{
                    for(int i=0;i<m_pSmartHosts.Length;i++){
                        if(!value[i].Equals(m_pSmartHosts[i])){
                            changed = true;
                            break;
                        }
                    }
                }

                if(changed){
                    m_pSmartHosts = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets how many seconds session can idle before times out.
        /// </summary>
        public int SessionIdleTimeOut
        {
            get{ return m_IdleTimeout; }

            set{
                if(m_IdleTimeout != value){
                    m_IdleTimeout = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum concurent connections allowed to send out messages.
        /// </summary>
        public int MaximumConnections
        {
            get{ return m_MaxConnections; }

            set{
                if(m_MaxConnections != value){
                    m_MaxConnections = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum concurent connections allowed to one destination IP.
        /// </summary>
        public int MaximumConnectionsPerIP
        {
            get{ return m_MaxConnectionsPerIP; }

            set{
                if(m_MaxConnectionsPerIP != value){
                    m_MaxConnectionsPerIP = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets interval seconds.
        /// </summary>
        public int RelayInterval
        {
            get{ return m_RelayInterval; }

            set{
                if(m_RelayInterval != value){
                    m_RelayInterval = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets relay retry messages interval seconds.
        /// </summary>
        public int RelayRetryInterval
        {
            get{ return m_RelayRetryInterval; }

            set{
                if(m_RelayRetryInterval != value){
                    m_RelayRetryInterval = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets after how many minutes delayed delivery message is sent.
        /// </summary>
        public int SendUndeliveredWarningAfter
        {
            get{ return m_SendUndelWaringAfter; }

            set{
                if(m_SendUndelWaringAfter != value){
                    m_SendUndelWaringAfter = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets after how many hours undelivered notification is sent.
        /// </summary>
        public int SendUndeliveredAfter
        {
            get{ return m_SendUndeliveredAfter; }

            set{
                if(m_SendUndeliveredAfter != value){
                    m_SendUndeliveredAfter = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets if undelivered messages are stored to "Undelivered" folder in mail store.
        /// </summary>
        public bool StoreUndeliveredMessages
        {
            get{ return m_StoreUndeliveredMsgs; }

            set{
                if(m_StoreUndeliveredMsgs != value){
                    m_StoreUndeliveredMsgs = value;

                    m_pSysSettings.SetValuesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets if TLS is used when remote server supports it.
        /// </summary>
        public bool UseTlsIfPossible
        {
            get{ return m_UseTlsIfPossible; }

            set{
                if(m_UseTlsIfPossible != value){
                    m_UseTlsIfPossible = value;

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

        #endregion

    }
}
