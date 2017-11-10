using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Mail server info UI.
    /// </summary>
    public class wfrm_Server_Info : Form
    {
        private Label      mt_ServerInfo       = null;
        private GroupBox   m_pGroupbox1        = null;
        private Label      mt_OS               = null;
        private Label      m_pOS               = null;
        private Label      mt_MemUsage         = null;
        private Label      m_pMemUsage         = null;
        private Label      mt_ServerVersion    = null;
        private Label      m_pServerVersion    = null;
        private Label      mt_CpuUsage         = null;
        private WLineGraph m_pCpuUsage         = null;
        private Label      mt_BandwidthUsage   = null;
        private WLineGraph m_pBandwidthUsage   = null;
        private Label      mt_MaxBandwidth     = null;
        private Label      m_pMaxBandwidth     = null;
        private Panel      m_pReadColor        = null;
        private Label      mt_Read             = null;
        private Panel      m_pWriteColor       = null;
        private Label      mt_Write            = null;
        private Label      mt_ConnectionsUsage = null;
        private WLineGraph m_pConnectionsUsage = null;
        private Panel      m_pSmtpColor        = null;
        private Label      mt_Smtp             = null;
        private Panel      m_pPop3Color        = null;
        private Label      mt_Pop3             = null;
        private Panel      m_pImapColor        = null;
        private Label      mt_Imap             = null;
        private Panel      m_pRelayColor       = null;
        private Label      mt_Relay            = null;

        private Server     m_pServer     = null;
        private bool       m_Run         = false;
        private ServerInfo m_pServerInfo = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Reference to mail server.</param>
        public wfrm_Server_Info(Server server)
        {
            m_pServer = server;

            InitUI();
                       
            Start();
        }

        #region method Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            m_Run = false;
            base.Dispose(disposing);            
        }

        #endregion
                
        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(300,200);

            mt_ServerInfo = new Label();
            mt_ServerInfo.Size = new Size(70,12);
            mt_ServerInfo.Location = new Point(10,20);
            mt_ServerInfo.TextAlign = ContentAlignment.MiddleLeft;
            mt_ServerInfo.Text = "Server Info:";

            m_pGroupbox1 = new GroupBox();
            m_pGroupbox1.Size = new Size(200,3);
            m_pGroupbox1.Location = new Point(80,25);
            m_pGroupbox1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_OS = new Label();
            mt_OS.Size = new Size(100,20);
            mt_OS.Location = new Point(60,40);
            mt_OS.TextAlign = ContentAlignment.MiddleLeft;
            mt_OS.Text = "OS:";

            m_pOS = new Label();
            m_pOS.Size = new Size(200,20);
            m_pOS.Location = new Point(165,40);
            m_pOS.TextAlign = ContentAlignment.MiddleLeft;

            mt_MemUsage = new Label();
            mt_MemUsage.Size = new Size(100,20);
            mt_MemUsage.Location = new Point(60,65);
            mt_MemUsage.TextAlign = ContentAlignment.MiddleLeft;
            mt_MemUsage.Text = "Memory Usage:";

            m_pMemUsage = new Label();
            m_pMemUsage.Size = new Size(200,20);
            m_pMemUsage.Location = new Point(165,65);
            m_pMemUsage.TextAlign = ContentAlignment.MiddleLeft;
                        
            mt_ServerVersion = new Label();
            mt_ServerVersion.Size = new Size(100,20);
            mt_ServerVersion.Location = new Point(60,95);
            mt_ServerVersion.TextAlign = ContentAlignment.MiddleLeft;
            mt_ServerVersion.Text = "Version:";

            m_pServerVersion = new Label();
            m_pServerVersion.Size = new Size(200,20);
            m_pServerVersion.Location = new Point(165,95);
            m_pServerVersion.TextAlign = ContentAlignment.MiddleLeft;

            mt_CpuUsage = new Label();
            mt_CpuUsage.Size = new Size(100,20);
            mt_CpuUsage.Location = new Point(60,120);
            mt_CpuUsage.TextAlign = ContentAlignment.MiddleLeft;
            mt_CpuUsage.Text = "CPU Usage:";

            m_pCpuUsage = new WLineGraph();
            m_pCpuUsage.Size = new Size(110,100);
            m_pCpuUsage.Location = new Point(165,120);
            m_pCpuUsage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pCpuUsage.AddLine(Color.LightGreen);

            mt_BandwidthUsage = new Label();
            mt_BandwidthUsage.Size = new Size(100,20);
            mt_BandwidthUsage.Location = new Point(60,245);
            mt_BandwidthUsage.TextAlign = ContentAlignment.MiddleLeft;
            mt_BandwidthUsage.Text = "Bandwidth Usage:";

            m_pBandwidthUsage = new WLineGraph();
            m_pBandwidthUsage.Size = new Size(110,100);
            m_pBandwidthUsage.Location = new Point(165,245);
            m_pBandwidthUsage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pBandwidthUsage.AutoMaxValue = true;
            m_pBandwidthUsage.AddLine(Color.LightGreen);
            m_pBandwidthUsage.AddLine(Color.Red);

            mt_MaxBandwidth = new Label();
            mt_MaxBandwidth.Size = new Size(100,20);
            mt_MaxBandwidth.Location = new Point(60,355);
            mt_MaxBandwidth.TextAlign = ContentAlignment.MiddleLeft;
            mt_MaxBandwidth.Text = "Max bandwidth:";

            m_pMaxBandwidth = new Label();
            m_pMaxBandwidth.Size = new Size(80,20);
            m_pMaxBandwidth.Location = new Point(165,355);
            m_pMaxBandwidth.TextAlign = ContentAlignment.MiddleLeft;

            m_pReadColor = new Panel();
            m_pReadColor.Size = new Size(16,16);
            m_pReadColor.Location = new Point(250,357);
            m_pReadColor.BorderStyle = BorderStyle.FixedSingle;
            m_pReadColor.BackColor = Color.LightGreen;

            mt_Read = new Label();
            mt_Read.Size = new Size(100,20);
            mt_Read.Location = new Point(270,355);
            mt_Read.TextAlign = ContentAlignment.MiddleLeft;
            mt_Read.Text = "Reads";

            m_pWriteColor = new Panel();
            m_pWriteColor.Size = new Size(16,16);
            m_pWriteColor.Location = new Point(380,357);
            m_pWriteColor.BorderStyle = BorderStyle.FixedSingle;
            m_pWriteColor.BackColor = Color.Red;

            mt_Write = new Label();
            mt_Write.Size = new Size(120,20);
            mt_Write.Location = new Point(400,355);
            mt_Write.TextAlign = ContentAlignment.MiddleLeft;
            mt_Write.Text = "Writes";

            mt_ConnectionsUsage = new Label();
            mt_ConnectionsUsage.Size = new Size(160,20);
            mt_ConnectionsUsage.Location = new Point(0,400);
            mt_ConnectionsUsage.TextAlign = ContentAlignment.MiddleRight;
            mt_ConnectionsUsage.Text = "Connections Usage:";

            m_pConnectionsUsage = new WLineGraph();
            m_pConnectionsUsage.Size = new Size(110,80);
            m_pConnectionsUsage.Location = new Point(165,400);
            m_pConnectionsUsage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pConnectionsUsage.AutoMaxValue = true;
            m_pConnectionsUsage.AddLine(Color.LightGreen);
            m_pConnectionsUsage.AddLine(Color.Red);
            m_pConnectionsUsage.AddLine(Color.Yellow);
            m_pConnectionsUsage.AddLine(Color.DarkOrange);

            m_pSmtpColor = new Panel();
            m_pSmtpColor.Size = new Size(14,14);
            m_pSmtpColor.Location = new Point(165,490);
            m_pSmtpColor.BorderStyle = BorderStyle.FixedSingle;
            m_pSmtpColor.BackColor = Color.LightGreen;

            mt_Smtp = new Label();
            mt_Smtp.Size = new Size(80,14);
            mt_Smtp.Location = new Point(180,490);
            mt_Smtp.TextAlign = ContentAlignment.MiddleLeft;
            mt_Smtp.Text = "SMTP";

            m_pPop3Color = new Panel();
            m_pPop3Color.Size = new Size(14,14);
            m_pPop3Color.Location = new Point(260,490);
            m_pPop3Color.BorderStyle = BorderStyle.FixedSingle;
            m_pPop3Color.BackColor = Color.Red;

            mt_Pop3 = new Label();
            mt_Pop3.Size = new Size(75,14);
            mt_Pop3.Location = new Point(275,490);
            mt_Pop3.TextAlign = ContentAlignment.MiddleLeft;
            mt_Pop3.Text = "POP3";

            m_pImapColor = new Panel();
            m_pImapColor.Size = new Size(14,14);
            m_pImapColor.Location = new Point(350,490);
            m_pImapColor.BorderStyle = BorderStyle.FixedSingle;
            m_pImapColor.BackColor = Color.Yellow;

            mt_Imap = new Label();
            mt_Imap.Size = new Size(75,14);
            mt_Imap.Location = new Point(365,490);
            mt_Imap.TextAlign = ContentAlignment.MiddleLeft;
            mt_Imap.Text = "IMAP";

            m_pRelayColor = new Panel();
            m_pRelayColor.Size = new Size(14,14);
            m_pRelayColor.Location = new Point(440,490);
            m_pRelayColor.BorderStyle = BorderStyle.FixedSingle;
            m_pRelayColor.BackColor = Color.DarkOrange;

            mt_Relay = new Label();
            mt_Relay.Size = new Size(80,14);
            mt_Relay.Location = new Point(455,490);
            mt_Relay.TextAlign = ContentAlignment.MiddleLeft;
            mt_Relay.Text = "Relay";

            this.Controls.Add(mt_ServerInfo);
            this.Controls.Add(m_pGroupbox1);
            this.Controls.Add(mt_OS);
            this.Controls.Add(m_pOS);
            this.Controls.Add(mt_MemUsage);
            this.Controls.Add(m_pMemUsage);
            this.Controls.Add(mt_ServerVersion);
            this.Controls.Add(m_pServerVersion);
            this.Controls.Add(mt_CpuUsage);
            this.Controls.Add(m_pCpuUsage);
            this.Controls.Add(mt_BandwidthUsage);
            this.Controls.Add(m_pBandwidthUsage);
            this.Controls.Add(mt_MaxBandwidth);
            this.Controls.Add(m_pMaxBandwidth);
            this.Controls.Add(m_pReadColor);
            this.Controls.Add(mt_Read);
            this.Controls.Add(m_pWriteColor);
            this.Controls.Add(mt_Write);
            this.Controls.Add(mt_ConnectionsUsage);
            this.Controls.Add(m_pConnectionsUsage);
            this.Controls.Add(m_pSmtpColor);
            this.Controls.Add(mt_Smtp);
            this.Controls.Add(m_pPop3Color);
            this.Controls.Add(mt_Pop3);
            this.Controls.Add(m_pImapColor);
            this.Controls.Add(mt_Imap);
            this.Controls.Add(m_pRelayColor);
            this.Controls.Add(mt_Relay);
        }

        #endregion


        #region method Start

        /// <summary>
        /// Starts refreshing CPU,memory, ... usage.
        /// </summary>
        private void Start()
        {
            m_Run = true;
            Thread tr = new Thread(new ThreadStart(this.Run));
            tr.Start();
        }

        #endregion

        #region method Run

        /// <summary>
        /// Monitoring thread run loop.
        /// </summary>
        private void Run()
        {            
            while(m_Run){
                try{
                    // Get server info
                    m_pServerInfo = m_pServer.ServerInfo;
                    
                    // Force UI to refresh
                    this.Invoke(new ThreadStart(this.RefreshUI));
                }
                catch{
                }

                // Wait 1.5 sec 
                System.Threading.Thread.Sleep(1500);
            }
        }

        #endregion

        #region method RefreshUI

        /// <summary>
        /// Refreshes UI.
        /// </summary>
        private void RefreshUI()
        {
            m_pOS.Text            = m_pServerInfo.OS;
            m_pMemUsage.Text      = m_pServerInfo.MemoryUsage + " MB";
            m_pServerVersion.Text = m_pServerInfo.MailServerVersion;
            m_pCpuUsage.AddValue(new int[]{Math.Min(m_pServerInfo.CpuUsage,100)});
            m_pBandwidthUsage.AddValue(new int[]{m_pServerInfo.ReadsInSecond,m_pServerInfo.WritesInSecond});
            m_pMaxBandwidth.Text  = m_pBandwidthUsage.MaximumValue + " KB";
            mt_Read.Text          = "Reads (" + m_pServerInfo.ReadsInSecond + " KB)";
            mt_Write.Text         = "Writes (" + m_pServerInfo.WritesInSecond + " KB)";
            m_pConnectionsUsage.AddValue(new int[]{m_pServerInfo.TotalSmtpSessions,m_pServerInfo.TotalPop3Sessions,m_pServerInfo.TotalImapSessions,m_pServerInfo.TotalRelaySessions});
            mt_Smtp.Text          = "SMTP (" + m_pServerInfo.TotalSmtpSessions + ")";
            mt_Pop3.Text          = "POP3 (" + m_pServerInfo.TotalPop3Sessions + ")";
            mt_Imap.Text          = "IMAP (" + m_pServerInfo.TotalImapSessions + ")";
            mt_Relay.Text         = "Relay (" + m_pServerInfo.TotalRelaySessions + ")";
        }

        #endregion

    }
}
