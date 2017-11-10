using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Net;

using LumiSoft.Net;
using LumiSoft.Net.SMTP.Relay;
using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// System Services Relay settings window.
    /// </summary>
    public class wfrm_System_Services_Relay : Form
    {
        //--- Common UI -------------------------------------
        private TabControl    m_pTab                  = null;
        private Button        m_pApply                = null;
        //--- Tabpage General UI ----------------------------
        private RadioButton        m_pTabGeneral_SendSmartHost         = null;
        private Label              mt_TabGeneral_SmartHostsBalanceMode = null;
        private ComboBox           m_pTabGeneral_SmartHostsBalanceMode = null;
        private ToolStrip          m_pTabGeneral_SmartHosts_Toolbar    = null;
        private ListView           m_pTabGeneral_SmartHosts            = null;
        private RadioButton        m_pTabGeneral_SendDns               = null;
        private Label              mt_TabGeneral_SessionTimeout        = null;
        private NumericUpDown      m_pTabGeneral_SessionTimeout        = null;
        private Label              mt_TabGeneral_SessTimeoutSec        = null;
        private Label              mt_TabGeneral_MaxConnections        = null;
        private NumericUpDown      m_pTabGeneral_MaxConnections        = null;
        private Label              mt_TabGeneral_MaxConnsPerIP         = null;
        private NumericUpDown      m_pTabGeneral_MaxConnsPerIP         = null;
        private Label              mt_TabGeneral_MaxConnsPerIP0        = null;
        private Label              mt_TabGeneral_RelayInterval         = null;
        private NumericUpDown      m_pTabGeneral_RelayInterval         = null;
        private Label              mt_TabGeneral_RelayIntervalSeconds  = null;
        private Label              mt_TabGeneral_RelayRetryInterval    = null;
        private NumericUpDown      m_pTabGeneral_RelayRetryInterval    = null;
        private Label              mt_TabGeneral_RelayRetryIntervSec   = null;
        private Label              mt_TabGeneral_SendUndelWarning      = null;
        private NumericUpDown      m_pTabGeneral_SendUndelWarning      = null;
        private Label              mt_TabGeneral_SendUndelWarnMinutes  = null;
        private Label              mt_TabGeneral_SendUndelivered       = null;
        private NumericUpDown      m_pTabGeneral_SendUndelivered       = null;
        private Label              mt_TabGeneral_SendUndeliveredHours  = null;
        private CheckBox           m_pTabGeneral_StoreUndeliveredMsgs  = null;        
        private CheckBox           m_pTabGeneral_UseTlsIfPossible      = null;
        private wctrl_Notification m_pTabGeneral_Notification          = null;
        //--- Tabpage IP Bindings --------------------------------------
        private ToolStrip          m_pTabBindings_BindingsToolbar = null;
        private ListView           m_pTabBindings_Bindings        = null;
        private wctrl_Notification m_pTabBindings_Notification    = null;
        //---------------------------------------------------------------
        
        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_System_Services_Relay(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadData();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            //--- Common UI -------------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(515,520);
            m_pTab.Location = new Point(5,0);
            m_pTab.TabPages.Add(new TabPage("General"));
            m_pTab.TabPages.Add(new TabPage("IP Bindings"));

            m_pApply = new Button();
            m_pApply.Size = new Size(70,20);
            m_pApply.Location = new Point(450,530);
            m_pApply.Text = "Apply";
            m_pApply.Click += new EventHandler(m_pApply_Click);
            //---------------------------------------------------//

            #region Tabpage General

            //--- Tabpage General UI ----------------------------//
            m_pTabGeneral_SendSmartHost = new RadioButton();
            m_pTabGeneral_SendSmartHost.Size = new Size(250,20);
            m_pTabGeneral_SendSmartHost.Location = new Point(10,15);
            m_pTabGeneral_SendSmartHost.Text = "Send mails through SmartHost";
            m_pTabGeneral_SendSmartHost.CheckedChanged += new EventHandler(m_pTabGeneral_SendSmartHost_CheckedChanged);

            mt_TabGeneral_SmartHostsBalanceMode = new Label();
            mt_TabGeneral_SmartHostsBalanceMode.Size = new Size(160,20);
            mt_TabGeneral_SmartHostsBalanceMode.Location = new Point(10,40);
            mt_TabGeneral_SmartHostsBalanceMode.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_SmartHostsBalanceMode.Text = "Smart hosts balance mode:";

            m_pTabGeneral_SmartHostsBalanceMode = new ComboBox();
            m_pTabGeneral_SmartHostsBalanceMode.Size = new Size(100,20);
            m_pTabGeneral_SmartHostsBalanceMode.Location = new Point(180,40);
            m_pTabGeneral_SmartHostsBalanceMode.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pTabGeneral_SmartHostsBalanceMode.Items.Add("Load-balance");
            m_pTabGeneral_SmartHostsBalanceMode.Items.Add("Fail-over");
            m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex = 0;

            m_pTabGeneral_SmartHosts_Toolbar = new ToolStrip();
            m_pTabGeneral_SmartHosts_Toolbar.Size = new Size(95,25);
            m_pTabGeneral_SmartHosts_Toolbar.Location = new Point(373,40);
            m_pTabGeneral_SmartHosts_Toolbar.Dock = DockStyle.None;
            m_pTabGeneral_SmartHosts_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTabGeneral_SmartHosts_Toolbar.BackColor = this.BackColor;
            m_pTabGeneral_SmartHosts_Toolbar.Renderer = new ToolBarRendererEx();
            m_pTabGeneral_SmartHosts_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTabGeneral_SmartHosts_Toolbar_ItemClicked);
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Tag = "add";
            button_Add.ToolTipText = "Add";
            m_pTabGeneral_SmartHosts_Toolbar.Items.Add(button_Add);
            // Edit button
            ToolStripButton button_Edit = new ToolStripButton();
            button_Edit.Enabled = false;
            button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            button_Edit.Tag = "edit";
            button_Edit.ToolTipText = "edit";
            m_pTabGeneral_SmartHosts_Toolbar.Items.Add(button_Edit);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText = "Delete";
            m_pTabGeneral_SmartHosts_Toolbar.Items.Add(button_Delete);
            // Separator
            m_pTabGeneral_SmartHosts_Toolbar.Items.Add(new ToolStripSeparator());
            // Up button
            ToolStripButton rulesToolbar_button_Up = new ToolStripButton();
            rulesToolbar_button_Up.Enabled = false;
            rulesToolbar_button_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            rulesToolbar_button_Up.Tag = "up";
            m_pTabGeneral_SmartHosts_Toolbar.Items.Add(rulesToolbar_button_Up);
            // Down button
            ToolStripButton rulesToolbar_button_down = new ToolStripButton();
            rulesToolbar_button_down.Enabled = false;
            rulesToolbar_button_down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            rulesToolbar_button_down.Tag = "down";
            m_pTabGeneral_SmartHosts_Toolbar.Items.Add(rulesToolbar_button_down);

            m_pTabGeneral_SmartHosts = new ListView();
            m_pTabGeneral_SmartHosts.Size = new Size(465,100);
            m_pTabGeneral_SmartHosts.Location = new Point(30,65);
            m_pTabGeneral_SmartHosts.View = View.Details;
            m_pTabGeneral_SmartHosts.FullRowSelect = true;
            m_pTabGeneral_SmartHosts.HideSelection = false;
            m_pTabGeneral_SmartHosts.SelectedIndexChanged += new EventHandler(m_pTabGeneral_SmartHosts_SelectedIndexChanged);
            m_pTabGeneral_SmartHosts.Columns.Add("Host",200);
            m_pTabGeneral_SmartHosts.Columns.Add("Port",60);
            m_pTabGeneral_SmartHosts.Columns.Add("SSL Mode",80);
            m_pTabGeneral_SmartHosts.Columns.Add("User Name",100);

            m_pTabGeneral_SendDns = new RadioButton();
            m_pTabGeneral_SendDns.Size = new Size(250,20);
            m_pTabGeneral_SendDns.Location = new Point(10,180);
            m_pTabGeneral_SendDns.Text = "Send mails directly using DNS";
            m_pTabGeneral_SendDns.CheckedChanged += new EventHandler(m_pTabGeneral_SendSmartHost_CheckedChanged);
            
            mt_TabGeneral_SessionTimeout = new Label();
            mt_TabGeneral_SessionTimeout.Size = new Size(200,20);
            mt_TabGeneral_SessionTimeout.Location = new Point(10,210);
            mt_TabGeneral_SessionTimeout.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_SessionTimeout.Text = "Session Idle Timeout:";

            m_pTabGeneral_SessionTimeout = new NumericUpDown();
            m_pTabGeneral_SessionTimeout.Size = new Size(70,20);
            m_pTabGeneral_SessionTimeout.Location = new Point(215,210);
            m_pTabGeneral_SessionTimeout.Minimum = 10;
            m_pTabGeneral_SessionTimeout.Maximum = 99999;

            mt_TabGeneral_SessTimeoutSec = new Label();
            mt_TabGeneral_SessTimeoutSec.Size = new Size(70,20);
            mt_TabGeneral_SessTimeoutSec.Location = new Point(290,210);
            mt_TabGeneral_SessTimeoutSec.TextAlign = ContentAlignment.MiddleLeft;
            mt_TabGeneral_SessTimeoutSec.Text = "seconds";

            mt_TabGeneral_MaxConnections = new Label();
            mt_TabGeneral_MaxConnections.Size = new Size(200,20);
            mt_TabGeneral_MaxConnections.Location = new Point(10,240);
            mt_TabGeneral_MaxConnections.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_MaxConnections.Text = "Maximum Connections:";
                        
            m_pTabGeneral_MaxConnections = new NumericUpDown();
            m_pTabGeneral_MaxConnections.Size = new Size(70,20);
            m_pTabGeneral_MaxConnections.Location = new Point(215,240);
            m_pTabGeneral_MaxConnections.Minimum = 1;
            m_pTabGeneral_MaxConnections.Maximum = 99999;

            mt_TabGeneral_MaxConnsPerIP = new Label();
            mt_TabGeneral_MaxConnsPerIP.Size = new Size(200,20);
            mt_TabGeneral_MaxConnsPerIP.Location = new Point(1,265);
            mt_TabGeneral_MaxConnsPerIP.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_MaxConnsPerIP.Text = "Maximum Connections per IP:";

            m_pTabGeneral_MaxConnsPerIP = new NumericUpDown();
            m_pTabGeneral_MaxConnsPerIP.Size = new Size(70,20);
            m_pTabGeneral_MaxConnsPerIP.Location = new Point(215,265);
            m_pTabGeneral_MaxConnsPerIP.Minimum = 0;
            m_pTabGeneral_MaxConnsPerIP.Maximum = 99999;

            mt_TabGeneral_MaxConnsPerIP0 = new Label();
            mt_TabGeneral_MaxConnsPerIP0.Size = new Size(164,20);
            mt_TabGeneral_MaxConnsPerIP0.Location = new Point(290,265);
            mt_TabGeneral_MaxConnsPerIP0.TextAlign = ContentAlignment.MiddleLeft;
            mt_TabGeneral_MaxConnsPerIP0.Text = "(0 for unlimited)";

            mt_TabGeneral_RelayInterval = new Label();
            mt_TabGeneral_RelayInterval.Size = new Size(200,20);
            mt_TabGeneral_RelayInterval.Location = new Point(10,290);
            mt_TabGeneral_RelayInterval.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_RelayInterval.Text = "Relay Interval:";

            m_pTabGeneral_RelayInterval = new NumericUpDown();
            m_pTabGeneral_RelayInterval.Size = new Size(70,20);
            m_pTabGeneral_RelayInterval.Location = new Point(215,290);
            m_pTabGeneral_RelayInterval.Minimum = 1;
            m_pTabGeneral_RelayInterval.Maximum = 9999;

            mt_TabGeneral_RelayIntervalSeconds = new Label();
            mt_TabGeneral_RelayIntervalSeconds.Size = new Size(50,20);
            mt_TabGeneral_RelayIntervalSeconds.Location = new Point(290,290);
            mt_TabGeneral_RelayIntervalSeconds.TextAlign = ContentAlignment.MiddleLeft;
            mt_TabGeneral_RelayIntervalSeconds.Text = "seconds";

            mt_TabGeneral_RelayRetryInterval = new Label();
            mt_TabGeneral_RelayRetryInterval.Size = new Size(200,20);
            mt_TabGeneral_RelayRetryInterval.Location = new Point(10,315);
            mt_TabGeneral_RelayRetryInterval.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_RelayRetryInterval.Text = "Relay Retry Interval:";

            m_pTabGeneral_RelayRetryInterval = new NumericUpDown();
            m_pTabGeneral_RelayRetryInterval.Size = new Size(70,20);
            m_pTabGeneral_RelayRetryInterval.Location = new Point(215,315);
            m_pTabGeneral_RelayRetryInterval.Minimum = 1;
            m_pTabGeneral_RelayRetryInterval.Maximum = 9999;

            mt_TabGeneral_RelayRetryIntervSec = new Label();
            mt_TabGeneral_RelayRetryIntervSec.Size = new Size(50,20);
            mt_TabGeneral_RelayRetryIntervSec.Location = new Point(290,315);
            mt_TabGeneral_RelayRetryIntervSec.TextAlign = ContentAlignment.MiddleLeft;
            mt_TabGeneral_RelayRetryIntervSec.Text = "seconds";

            mt_TabGeneral_SendUndelWarning = new Label();
            mt_TabGeneral_SendUndelWarning.Size = new Size(200,20);
            mt_TabGeneral_SendUndelWarning.Location = new Point(10,345);
            mt_TabGeneral_SendUndelWarning.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_SendUndelWarning.Text = "Send undelivered warning after:";

            m_pTabGeneral_SendUndelWarning = new NumericUpDown();
            m_pTabGeneral_SendUndelWarning.Size = new Size(70,20);
            m_pTabGeneral_SendUndelWarning.Location = new Point(215,345);
            m_pTabGeneral_SendUndelWarning.Minimum = 1;
            m_pTabGeneral_SendUndelWarning.Maximum = 9999;

            mt_TabGeneral_SendUndelWarnMinutes = new Label();
            mt_TabGeneral_SendUndelWarnMinutes.Size = new Size(50,20);
            mt_TabGeneral_SendUndelWarnMinutes.Location = new Point(290,345);
            mt_TabGeneral_SendUndelWarnMinutes.TextAlign = ContentAlignment.MiddleLeft;
            mt_TabGeneral_SendUndelWarnMinutes.Text = "minutes";

            mt_TabGeneral_SendUndelivered = new Label();
            mt_TabGeneral_SendUndelivered.Size = new Size(200,20);
            mt_TabGeneral_SendUndelivered.Location = new Point(10,370);
            mt_TabGeneral_SendUndelivered.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_SendUndelivered.Text = "Send undelivered after:";

            m_pTabGeneral_SendUndelivered = new NumericUpDown();
            m_pTabGeneral_SendUndelivered.Size = new Size(70,20);
            m_pTabGeneral_SendUndelivered.Location = new Point(215,370);
            m_pTabGeneral_SendUndelivered.Minimum = 1;
            m_pTabGeneral_SendUndelivered.Maximum = 999;

            mt_TabGeneral_SendUndeliveredHours = new Label();
            mt_TabGeneral_SendUndeliveredHours.Size = new Size(50,20);
            mt_TabGeneral_SendUndeliveredHours.Location = new Point(290,370);
            mt_TabGeneral_SendUndeliveredHours.TextAlign = ContentAlignment.MiddleLeft;
            mt_TabGeneral_SendUndeliveredHours.Text = "hours";

            m_pTabGeneral_StoreUndeliveredMsgs = new CheckBox();
            m_pTabGeneral_StoreUndeliveredMsgs.Size = new Size(250,20);
            m_pTabGeneral_StoreUndeliveredMsgs.Location = new Point(215,395);
            m_pTabGeneral_StoreUndeliveredMsgs.Text = "Store undelivered messages";

            m_pTabGeneral_UseTlsIfPossible = new CheckBox();
            m_pTabGeneral_UseTlsIfPossible.Size = new Size(250,20);
            m_pTabGeneral_UseTlsIfPossible.Location = new Point(215,415);
            m_pTabGeneral_UseTlsIfPossible.Text = "Use TLS if possible";

            m_pTabGeneral_Notification = new wctrl_Notification();
            m_pTabGeneral_Notification.Size = new Size(485,38);
            m_pTabGeneral_Notification.Location = new Point(10,440);
            m_pTabGeneral_Notification.Icon = ResManager.GetIcon("warning.ico").ToBitmap();
            m_pTabGeneral_Notification.Visible = false;
                                    
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SendSmartHost);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_SmartHostsBalanceMode);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SmartHostsBalanceMode);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SmartHosts_Toolbar);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SmartHosts);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SendDns);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_SessionTimeout);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SessionTimeout);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_SessTimeoutSec);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_MaxConnections);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_MaxConnections);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_MaxConnsPerIP);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_MaxConnsPerIP);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_MaxConnsPerIP0);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_RelayInterval);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_RelayInterval);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_RelayIntervalSeconds);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_RelayRetryInterval);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_RelayRetryInterval);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_RelayRetryIntervSec);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_SendUndelWarning);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SendUndelWarning);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_SendUndelWarnMinutes);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_SendUndelivered);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_SendUndelivered);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_SendUndeliveredHours);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_StoreUndeliveredMsgs);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_UseTlsIfPossible);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_Notification);
            //-------------------------------------------------//

            #endregion

            #region Tabpage IP Bindings
                        
            m_pTabBindings_BindingsToolbar = new ToolStrip();
            m_pTabBindings_BindingsToolbar.Size = new Size(95,25);
            m_pTabBindings_BindingsToolbar.Location = new Point(425,13);
            m_pTabBindings_BindingsToolbar.Dock = DockStyle.None;
            m_pTabBindings_BindingsToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTabBindings_BindingsToolbar.BackColor = this.BackColor;
            m_pTabBindings_BindingsToolbar.Renderer = new ToolBarRendererEx();
            m_pTabBindings_BindingsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTabBindings_BindingsToolbar_ItemClicked);
            // Add button
            ToolStripButton bindings_button_Add = new ToolStripButton();
            bindings_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            bindings_button_Add.Tag = "add";
            bindings_button_Add.ToolTipText = "Add";
            m_pTabBindings_BindingsToolbar.Items.Add(bindings_button_Add);
            // Edit button
            ToolStripButton bindings_button_Edit = new ToolStripButton();
            bindings_button_Edit.Enabled = false;
            bindings_button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            bindings_button_Edit.Tag = "edit";
            bindings_button_Edit.ToolTipText = "edit";
            m_pTabBindings_BindingsToolbar.Items.Add(bindings_button_Edit);
            // Delete button
            ToolStripButton bindings_button_Delete = new ToolStripButton();
            bindings_button_Delete.Enabled = false;
            bindings_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            bindings_button_Delete.Tag = "delete";
            bindings_button_Delete.ToolTipText = "Delete";
            m_pTabBindings_BindingsToolbar.Items.Add(bindings_button_Delete);

            m_pTabBindings_Bindings = new ListView();
            m_pTabBindings_Bindings.Size = new Size(485,375);
            m_pTabBindings_Bindings.Location = new Point(10,40);
            m_pTabBindings_Bindings.View = View.Details;
            m_pTabBindings_Bindings.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            m_pTabBindings_Bindings.HideSelection = false;
            m_pTabBindings_Bindings.FullRowSelect = true;
            m_pTabBindings_Bindings.MultiSelect = false;
            m_pTabBindings_Bindings.SelectedIndexChanged += new EventHandler(m_pTabBindings_Bindings_SelectedIndexChanged);
            m_pTabBindings_Bindings.Columns.Add("Host Name",250,HorizontalAlignment.Left);
            m_pTabBindings_Bindings.Columns.Add("IP",190,HorizontalAlignment.Left);
                        
            m_pTabBindings_Notification = new wctrl_Notification();
            m_pTabBindings_Notification.Size = new Size(485,38);
            m_pTabBindings_Notification.Location = new Point(10,421);
            m_pTabBindings_Notification.Icon = ResManager.GetIcon("warning.ico").ToBitmap();
            m_pTabBindings_Notification.Visible = false;

            m_pTab.TabPages[1].Controls.Add(m_pTabBindings_BindingsToolbar);
            m_pTab.TabPages[1].Controls.Add(m_pTabBindings_Bindings);
            m_pTab.TabPages[1].Controls.Add(m_pTabBindings_Notification);

            #endregion

            // Common UI
            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pApply);
        }
                                                                                                                                                
        #endregion


        #region Events Handling

        #region method OnVisibleChanged

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if(!this.Visible){
                SaveData(true); 
            }
        }

        #endregion


        #region method m_pTabGeneral_SendSmartHost_CheckedChanged

        private void m_pTabGeneral_SendSmartHost_CheckedChanged(object sender, EventArgs e)
        {
            if(m_pTabGeneral_SendSmartHost.Checked){
                m_pTabGeneral_SmartHostsBalanceMode.Enabled = true;
                m_pTabGeneral_SmartHosts_Toolbar.Enabled    = true;
                m_pTabGeneral_SmartHosts.Enabled            = true;
			}
			else{
                m_pTabGeneral_SmartHostsBalanceMode.Enabled = false;
                m_pTabGeneral_SmartHosts_Toolbar.Enabled    = false;
                m_pTabGeneral_SmartHosts.Enabled            = false;
			}

            AddNotifications();
        }

        #endregion

        #region method m_pTabGeneral_SmartHosts_Toolbar_ItemClicked

        private void m_pTabGeneral_SmartHosts_Toolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
            }
            else if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_System_SmartHost frm = new wfrm_System_SmartHost();
                if(frm.ShowDialog(this) == DialogResult.OK){
                    ListViewItem it = new ListViewItem();
                    it.Text = frm.Host;
                    it.SubItems.Add(frm.Port.ToString());
                    it.SubItems.Add(frm.SslMode.ToString());
                    it.SubItems.Add(frm.UserName);
                    it.Tag = new Relay_SmartHost(frm.Host,frm.Port,frm.SslMode,frm.UserName,frm.Password);
                    m_pTabGeneral_SmartHosts.Items.Add(it);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                if(m_pTabGeneral_SmartHosts.SelectedItems.Count > 0){
                    ListViewItem it = m_pTabGeneral_SmartHosts.SelectedItems[0];
                    wfrm_System_SmartHost frm = new wfrm_System_SmartHost((Relay_SmartHost)it.Tag);
                    if(frm.ShowDialog(this) == DialogResult.OK){
                        it.Text             = frm.Host;
                        it.SubItems[1].Text = frm.Port.ToString();
                        it.SubItems[2].Text = frm.SslMode.ToString();
                        it.SubItems[3].Text = frm.UserName;
                        it.Tag = new Relay_SmartHost(frm.Host,frm.Port,frm.SslMode,frm.UserName,frm.Password);
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                if(m_pTabGeneral_SmartHosts.SelectedItems.Count > 0){
                    if(MessageBox.Show(this,"Are you sure you want to delete smart host '" + m_pTabGeneral_SmartHosts.SelectedItems[0].Text + "' ?","Confirm:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){                    
                        m_pTabGeneral_SmartHosts.SelectedItems[0].Remove();
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "up"){
                if(m_pTabGeneral_SmartHosts.SelectedItems.Count > 0 && m_pTabGeneral_SmartHosts.SelectedItems[0].Index > 0){
                    ListViewItem it1 = m_pTabGeneral_SmartHosts.SelectedItems[0];
                    ListViewItem it2 = m_pTabGeneral_SmartHosts.Items[it1.Index - 1];
                    m_pTabGeneral_SmartHosts.Items.Remove(it2);
                    m_pTabGeneral_SmartHosts.Items.Insert(it1.Index + 1,it2);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "down"){
                if(m_pTabGeneral_SmartHosts.SelectedItems.Count > 0 && m_pTabGeneral_SmartHosts.SelectedItems[0].Index < m_pTabGeneral_SmartHosts.Items.Count - 1){
                    ListViewItem it1 = m_pTabGeneral_SmartHosts.SelectedItems[0];
                    ListViewItem it2 = m_pTabGeneral_SmartHosts.Items[it1.Index + 1];
                    m_pTabGeneral_SmartHosts.Items.Remove(it2);
                    m_pTabGeneral_SmartHosts.Items.Insert(it1.Index,it2);
                }
            }

            m_pTabGeneral_SmartHosts_SelectedIndexChanged(this,new EventArgs());
            AddNotifications();
        }

        #endregion

        #region method m_pTabGeneral_SmartHosts_SelectedIndexChanged

        private void m_pTabGeneral_SmartHosts_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pTabGeneral_SmartHosts.SelectedItems.Count > 0){
                m_pTabGeneral_SmartHosts_Toolbar.Items[1].Enabled = true;
                m_pTabGeneral_SmartHosts_Toolbar.Items[2].Enabled = true;
                if(m_pTabGeneral_SmartHosts.SelectedItems[0].Index > 0){
                    m_pTabGeneral_SmartHosts_Toolbar.Items[4].Enabled = true;
                }
                else{
                    m_pTabGeneral_SmartHosts_Toolbar.Items[4].Enabled = false;
                }
                if(m_pTabGeneral_SmartHosts.SelectedItems[0].Index < (m_pTabGeneral_SmartHosts.Items.Count - 1)){
                    m_pTabGeneral_SmartHosts_Toolbar.Items[5].Enabled = true;
                }
                else{
                    m_pTabGeneral_SmartHosts_Toolbar.Items[5].Enabled = false;
                }
            }
            else{
                m_pTabGeneral_SmartHosts_Toolbar.Items[1].Enabled = false;
                m_pTabGeneral_SmartHosts_Toolbar.Items[2].Enabled = false;
                m_pTabGeneral_SmartHosts_Toolbar.Items[4].Enabled = false;
                m_pTabGeneral_SmartHosts_Toolbar.Items[5].Enabled = false;
            }
        }

        #endregion
               

        #region method m_pTabBindings_BindingsToolbar_ItemClicked

        private void m_pTabBindings_BindingsToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_sys_BindInfo frm = new wfrm_sys_BindInfo(m_pVirtualServer.Server,false,false,false,0,0,null);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    ListViewItem it = new ListViewItem();
                    it.Text = frm.HostName;
                    if(frm.IP.ToString() == "0.0.0.0"){
                        it.SubItems.Add("Any IPv4");
                    }
                    else if(frm.IP.ToString() == "0:0:0:0:0:0:0:0"){
                        it.SubItems.Add("Any IPv6");
                    }
                    else{
                        it.SubItems.Add(frm.IP.ToString());
                    }
                    it.SubItems.Add(frm.Protocol.ToString());
                    it.SubItems.Add(frm.Port.ToString());
                    it.SubItems.Add(frm.SslMode.ToString());
                    it.SubItems.Add(Convert.ToString(frm.Certificate != null));
                    it.Tag = new IPBindInfo(frm.HostName,frm.Protocol,frm.IP,frm.Port,frm.SslMode,frm.Certificate);
                    it.Selected = true;
                    m_pTabBindings_Bindings.Items.Add(it);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                if(m_pTabBindings_Bindings.SelectedItems.Count > 0){
                    ListViewItem it = m_pTabBindings_Bindings.SelectedItems[0];
                    wfrm_sys_BindInfo frm = new wfrm_sys_BindInfo(m_pVirtualServer.Server,false,false,false,0,0,(IPBindInfo)it.Tag);
                    if(frm.ShowDialog(this) == DialogResult.OK){
                        it.Text = frm.HostName;
                        if(frm.IP.ToString() == "0.0.0.0"){
                            it.SubItems[1].Text = "Any IPv4";
                        }
                        else if(frm.IP.ToString() == "0:0:0:0:0:0:0:0"){
                            it.SubItems[1].Text = "Any IPv6";
                        }
                        else{
                            it.SubItems[1].Text = frm.IP.ToString();
                        }
                        it.SubItems[2].Text = frm.Port.ToString();
                        it.SubItems[3].Text = frm.SslMode.ToString();
                        it.SubItems[4].Text = Convert.ToString(frm.Certificate != null);
                        it.Tag = new IPBindInfo(frm.HostName,frm.Protocol,frm.IP,frm.Port,frm.SslMode,frm.Certificate);
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                if(m_pTabBindings_Bindings.SelectedItems.Count > 0){
                    if(MessageBox.Show(this,"Are you sure you want to delete binding '" + m_pTabBindings_Bindings.SelectedItems[0].SubItems[0].Text + ":" + m_pTabBindings_Bindings.SelectedItems[0].SubItems[1].Text + "' ?","Confirm:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){                    
                        m_pTabBindings_Bindings.SelectedItems[0].Remove();
                    }
                }
            }

            AddNotifications();
        }

        #endregion

        #region method m_pTabBindings_Bindings_SelectedIndexChanged

        private void m_pTabBindings_Bindings_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pTabBindings_Bindings.SelectedItems.Count > 0){
                m_pTabBindings_BindingsToolbar.Items[1].Enabled = true;
                m_pTabBindings_BindingsToolbar.Items[2].Enabled = true;
            }
            else{
                m_pTabBindings_BindingsToolbar.Items[1].Enabled = false;
                m_pTabBindings_BindingsToolbar.Items[2].Enabled = false;
            }
        }

        #endregion


        #region method m_pApply_Click

        private void m_pApply_Click(object sender, EventArgs e)
        {
            SaveData(false); 
        }

        #endregion

        #endregion

        
        #region method LoadData

        /// <summary>
        /// Loads data to UI.
        /// </summary>
        private void LoadData()
        {            
            try{
                Relay_Settings settings =  m_pVirtualServer.SystemSettings.Relay;
 		
				if(settings.RelayMode == Relay_Mode.SmartHost){
					m_pTabGeneral_SendSmartHost.Checked = true;
				}
				else{
					m_pTabGeneral_SendDns.Checked = true;
				}
                if(settings.SmartHostsBalanceMode == BalanceMode.LoadBalance){
                    m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex = 0;
                }
                else{
                    m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex = 1;
                }
                m_pTabGeneral_SessionTimeout.Value         = settings.SessionIdleTimeOut;
				m_pTabGeneral_MaxConnections.Value         = settings.MaximumConnections;
                m_pTabGeneral_MaxConnsPerIP.Value          = settings.MaximumConnectionsPerIP;
				m_pTabGeneral_RelayInterval.Value          = settings.RelayInterval;
				m_pTabGeneral_RelayRetryInterval.Value     = settings.RelayRetryInterval;
				m_pTabGeneral_SendUndelWarning.Value       = settings.SendUndeliveredWarningAfter;
				m_pTabGeneral_SendUndelivered.Value        = settings.SendUndeliveredAfter;
				m_pTabGeneral_StoreUndeliveredMsgs.Checked = settings.StoreUndeliveredMessages;
				m_pTabGeneral_UseTlsIfPossible.Checked     = settings.UseTlsIfPossible;

                foreach(Relay_SmartHost smartHost in settings.SmartHosts){
                    ListViewItem it = new ListViewItem();
                    it.Text = smartHost.Host;
                    it.SubItems.Add(smartHost.Port.ToString());
                    it.SubItems.Add(smartHost.SslMode.ToString());
                    it.SubItems.Add(smartHost.UserName);
                    it.Tag = smartHost;
                    m_pTabGeneral_SmartHosts.Items.Add(it);
                }

                foreach(IPBindInfo binding in settings.Binds){
                    ListViewItem it = new ListViewItem();
                    it.Text = binding.HostName;
                    if(binding.IP.ToString() == "0.0.0.0"){
                        it.SubItems.Add("Any IPv4");
                    }
                    else if(binding.IP.ToString() == "0:0:0:0:0:0:0:0"){
                        it.SubItems.Add("Any IPv6");
                    }
                    else{
                        it.SubItems.Add(binding.IP.ToString());
                    }
                    it.SubItems.Add(binding.Port.ToString());
                    it.SubItems.Add(binding.SslMode.ToString());
                    it.SubItems.Add(Convert.ToString(binding.Certificate != null));
                    it.Tag = binding;
                    m_pTabBindings_Bindings.Items.Add(it);
                }

                AddNotifications();
			}
			catch(Exception x){
				wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
				frm.ShowDialog(this);
			}
        }

        #endregion

        #region method SaveData

        /// <summary>
        /// Saves data.
        /// </summary>
        /// <param name="confirmSave">Specifies is save confirmation UI is showed.</param>
        private void SaveData(bool confirmSave)
        {
            try{
                Relay_Settings settings =  m_pVirtualServer.SystemSettings.Relay;
 		
                if(m_pTabGeneral_SendSmartHost.Checked){
				    settings.RelayMode               = Relay_Mode.SmartHost;
                }
                else{
                    settings.RelayMode               = Relay_Mode.Dns;
                }
                if(m_pTabGeneral_SmartHostsBalanceMode.SelectedIndex == 0){
                    settings.SmartHostsBalanceMode   = BalanceMode.LoadBalance;
                }
                else{
                    settings.SmartHostsBalanceMode   = BalanceMode.FailOver;
                }
                settings.SessionIdleTimeOut          = (int)m_pTabGeneral_SessionTimeout.Value;
				settings.MaximumConnections          = (int)m_pTabGeneral_MaxConnections.Value;
                settings.MaximumConnectionsPerIP     = (int)m_pTabGeneral_MaxConnsPerIP.Value;
				settings.RelayInterval               = (int)m_pTabGeneral_RelayInterval.Value;
				settings.RelayRetryInterval          = (int)m_pTabGeneral_RelayRetryInterval.Value;
				settings.SendUndeliveredWarningAfter = (int)m_pTabGeneral_SendUndelWarning.Value;
				settings.SendUndeliveredAfter        = (int)m_pTabGeneral_SendUndelivered.Value;
				settings.StoreUndeliveredMessages    = m_pTabGeneral_StoreUndeliveredMsgs.Checked;                
				settings.UseTlsIfPossible            = m_pTabGeneral_UseTlsIfPossible.Checked;
                // Smart hosts
                List<Relay_SmartHost> smartHosts = new List<Relay_SmartHost>();
                foreach(ListViewItem it in m_pTabGeneral_SmartHosts.Items){
                    smartHosts.Add((Relay_SmartHost)it.Tag);
                }
                settings.SmartHosts = smartHosts.ToArray();
                // IP binds
                List<IPBindInfo> binds = new List<IPBindInfo>();
                foreach(ListViewItem it in m_pTabBindings_Bindings.Items){
                    binds.Add((IPBindInfo)it.Tag);
                }
                settings.Binds = binds.ToArray();
                               
                if(m_pVirtualServer.SystemSettings.HasChanges){
                    if(!confirmSave || MessageBox.Show(this,"You have changes settings, do you want to save them ?","Confirm:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                        m_pVirtualServer.SystemSettings.Commit();
                    }
                }
            }
			catch(Exception x){
				wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
				frm.ShowDialog(this);
			}
        }

        #endregion

        #region method AddNotifications

        /// <summary>
        /// Checks for misconfigured settings and adds notifications about it to UI.
        /// </summary>
        private void AddNotifications()
        {
            m_pTabGeneral_Notification.Visible = false;
            if(m_pTabGeneral_SendSmartHost.Checked && m_pTabGeneral_SmartHosts.Items.Count == 0){
                m_pTabGeneral_Notification.Visible = true;
                m_pTabGeneral_Notification.Text    = "Relay needs at least 1 Smart Host to function.\n";
            }

            m_pTabBindings_Notification.Visible = false;
            m_pTabBindings_Notification.Text    = "";
            if(m_pTabBindings_Bindings.Items.Count == 0){
                m_pTabBindings_Notification.Visible = true;
                m_pTabBindings_Notification.Text    = "Relay needs at least 1 IP Binding(hostname, ip address) to function.\n";
            }
            foreach(ListViewItem it in m_pTabBindings_Bindings.Items){
                if(it.Text == ""){
                    m_pTabBindings_Notification.Visible  = true;
                    m_pTabBindings_Notification.Text    += "Host Name is missing one or more binding(s).\n";
                    break;
                }
            }
        }

        #endregion

    }
}
