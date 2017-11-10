using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.Net;
using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// System Services SMTP settings window.
    /// </summary>
    public class wfrm_System_Services_SMTP : Form
    {
        //--- Common UI ------------------------------
        private TabControl    m_pTab            = null;
        private Button        m_pApply          = null;
        //--- Tabpage General UI ----------------------------------
        private CheckBox      m_pEnabled                    = null;
        private Label         mt_GreetingText               = null;
        private TextBox       m_pGreetingText               = null;
        private Label         mt_DefaultDomain              = null;
        private TextBox       m_pDefaultDomain              = null;
        private Label         mt_SessionTimeout             = null;
        private NumericUpDown m_pSessionTimeout             = null;
        private Label         mt_SessTimeoutSec             = null;
        private Label         mt_MaxConnections             = null;
        private NumericUpDown m_pMaxConnections             = null;
        private Label         mt_MaxConnsPerIP              = null;
        private NumericUpDown m_pMaxConnsPerIP              = null;
        private Label         mt_MaxConnsPerIP0             = null;
        private Label         mt_MaxBadCommands             = null;
        private NumericUpDown m_pMaxBadCommands             = null;
        private Label         mt_MaxRecipients              = null;
        private NumericUpDown m_pMaxRecipients              = null;
        private Label         mt_MaxMessageSize             = null;
        private NumericUpDown m_pMaxMessageSize             = null;
        private Label         m_pMaxMsgSizeMB               = null;
        private Label         mt_MaxTransactions            = null;
        private NumericUpDown m_pMaxTransactions            = null;
        private Label         mt_MaxTransactions0           = null;
        private CheckBox      m_pRequireAuth                = null;
        private Label         mt_TabGeneral_Bindings        = null;
        private ToolStrip     m_pTabGeneral_BindingsToolbar = null;
        private ListView      m_pTabGeneral_Bindings        = null;               
        //---------------------------------------------------------

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_System_Services_SMTP(VirtualServer virtualServer)
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

            m_pApply = new Button();
            m_pApply.Size = new Size(70,20);
            m_pApply.Location = new Point(450,530);
            m_pApply.Text = "Apply";
            m_pApply.Click += new EventHandler(m_pApply_Click);
            //---------------------------------------------------//

            //--- Tabpage General UI ----------------------------//
            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(70,20);
            m_pEnabled.Location = new Point(170,10);
            m_pEnabled.Text = "Enabled";

            mt_GreetingText = new Label();
            mt_GreetingText.Size = new Size(164,20);
            mt_GreetingText.Location = new Point(1,40);
            mt_GreetingText.TextAlign = ContentAlignment.MiddleRight;
            mt_GreetingText.Text = "Greeting Text:";

            m_pGreetingText = new TextBox();
            m_pGreetingText.Size = new Size(250,20);
            m_pGreetingText.Location = new Point(170,40);

            mt_DefaultDomain = new Label();
            mt_DefaultDomain.Size = new Size(164,20);
            mt_DefaultDomain.Location = new Point(1,65);
            mt_DefaultDomain.TextAlign = ContentAlignment.MiddleRight;
            mt_DefaultDomain.Text = "Default Domain:";

            m_pDefaultDomain = new TextBox();
            m_pDefaultDomain.Size = new Size(250,20);
            m_pDefaultDomain.Location = new Point(170,65);

            mt_SessionTimeout = new Label();
            mt_SessionTimeout.Size = new Size(164,20);
            mt_SessionTimeout.Location = new Point(1,105);
            mt_SessionTimeout.TextAlign = ContentAlignment.MiddleRight;
            mt_SessionTimeout.Text = "Session Idle Timeout:";

            m_pSessionTimeout = new NumericUpDown();
            m_pSessionTimeout.Size = new Size(70,20);
            m_pSessionTimeout.Location = new Point(170,105);
            m_pSessionTimeout.Minimum = 10;
            m_pSessionTimeout.Maximum = 99999;

            mt_SessTimeoutSec = new Label();
            mt_SessTimeoutSec.Size = new Size(25,20);
            mt_SessTimeoutSec.Location = new Point(245,105);
            mt_SessTimeoutSec.TextAlign = ContentAlignment.MiddleLeft;
            mt_SessTimeoutSec.Text = "sec.";

            mt_MaxConnections = new Label();
            mt_MaxConnections.Size = new Size(164,20);
            mt_MaxConnections.Location = new Point(1,145);
            mt_MaxConnections.TextAlign = ContentAlignment.MiddleRight;
            mt_MaxConnections.Text = "Maximum Connections:";

            m_pMaxConnections = new NumericUpDown();
            m_pMaxConnections.Size = new Size(70,20);
            m_pMaxConnections.Location = new Point(170,145);
            m_pMaxConnections.Minimum = 1;
            m_pMaxConnections.Maximum = 99999;
                        
            mt_MaxConnsPerIP = new Label();
            mt_MaxConnsPerIP.Size = new Size(164,20);
            mt_MaxConnsPerIP.Location = new Point(1,170);
            mt_MaxConnsPerIP.TextAlign = ContentAlignment.MiddleRight;
            mt_MaxConnsPerIP.Text = "Maximum Connections per IP:";

            m_pMaxConnsPerIP = new NumericUpDown();
            m_pMaxConnsPerIP.Size = new Size(70,20);
            m_pMaxConnsPerIP.Location = new Point(170,170);
            m_pMaxConnsPerIP.Minimum = 0;
            m_pMaxConnsPerIP.Maximum = 99999;

            mt_MaxConnsPerIP0 = new Label();
            mt_MaxConnsPerIP0.Size = new Size(164,20);
            mt_MaxConnsPerIP0.Location = new Point(245,170);
            mt_MaxConnsPerIP0.TextAlign = ContentAlignment.MiddleLeft;
            mt_MaxConnsPerIP0.Text = "(0 for unlimited)";
                                    
            mt_MaxBadCommands = new Label();
            mt_MaxBadCommands.Size = new Size(164,20);
            mt_MaxBadCommands.Location = new Point(1,195);
            mt_MaxBadCommands.TextAlign = ContentAlignment.MiddleRight;
            mt_MaxBadCommands.Text = "Maximum Bad Commands:";

            m_pMaxBadCommands = new NumericUpDown();
            m_pMaxBadCommands.Size = new Size(70,20);
            m_pMaxBadCommands.Location = new Point(170,195);
            m_pMaxBadCommands.Minimum = 1;
            m_pMaxBadCommands.Maximum = 99999;

            mt_MaxRecipients = new Label();
            mt_MaxRecipients.Size = new Size(164,20);
            mt_MaxRecipients.Location = new Point(1,220);
            mt_MaxRecipients.TextAlign = ContentAlignment.MiddleRight;
            mt_MaxRecipients.Text = "Max Recipients per Message:";

            m_pMaxRecipients = new NumericUpDown();
            m_pMaxRecipients.Size = new Size(70,20);
            m_pMaxRecipients.Location = new Point(170,220);
            m_pMaxRecipients.Minimum = 1;
            m_pMaxRecipients.Maximum = 99999;

            mt_MaxMessageSize = new Label();
            mt_MaxMessageSize.Size = new Size(164,20);
            mt_MaxMessageSize.Location = new Point(1,245);
            mt_MaxMessageSize.TextAlign = ContentAlignment.MiddleRight;
            mt_MaxMessageSize.Text = "Maximum Message Size:";

            m_pMaxMessageSize = new NumericUpDown();
            m_pMaxMessageSize.Size = new Size(70,20);
            m_pMaxMessageSize.Location = new Point(170,245);
            m_pMaxMessageSize.Minimum = 1;
            m_pMaxMessageSize.Maximum = 99999;

            m_pMaxMsgSizeMB = new Label();
            m_pMaxMsgSizeMB.Size = new Size(25,20);
            m_pMaxMsgSizeMB.Location = new Point(245,245);
            m_pMaxMsgSizeMB.TextAlign = ContentAlignment.MiddleLeft;
            m_pMaxMsgSizeMB.Text = "MB";

            mt_MaxTransactions = new Label();
            mt_MaxTransactions.Size = new Size(164,20);
            mt_MaxTransactions.Location = new Point(1,270);
            mt_MaxTransactions.TextAlign = ContentAlignment.MiddleRight;
            mt_MaxTransactions.Text = "Max transactions per session:";

            m_pMaxTransactions = new NumericUpDown();
            m_pMaxTransactions.Size = new Size(70,20);
            m_pMaxTransactions.Location = new Point(170,270);
            m_pMaxTransactions.Minimum = 0;
            m_pMaxTransactions.Maximum = 99999;

            mt_MaxTransactions0 = new Label();
            mt_MaxTransactions0.Size = new Size(164,20);
            mt_MaxTransactions0.Location = new Point(245,270);
            mt_MaxTransactions0.TextAlign = ContentAlignment.MiddleLeft;
            mt_MaxTransactions0.Text = "(0 for unlimited)";

            m_pRequireAuth = new CheckBox();
            m_pRequireAuth.Size = new Size(300,20);
            m_pRequireAuth.Location = new Point(170,295);
            m_pRequireAuth.Text = "Require authentication (private SMTP server)";

            mt_TabGeneral_Bindings = new Label();
            mt_TabGeneral_Bindings.Size = new Size(70,20);
            mt_TabGeneral_Bindings.Location = new Point(10,325);
            mt_TabGeneral_Bindings.Text = "IP Bindings:";

            m_pTabGeneral_BindingsToolbar = new ToolStrip();
            m_pTabGeneral_BindingsToolbar.Size = new Size(95,25);
            m_pTabGeneral_BindingsToolbar.Location = new Point(425,325);
            m_pTabGeneral_BindingsToolbar.Dock = DockStyle.None;
            m_pTabGeneral_BindingsToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTabGeneral_BindingsToolbar.BackColor = this.BackColor;
            m_pTabGeneral_BindingsToolbar.Renderer = new ToolBarRendererEx();
            m_pTabGeneral_BindingsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTabGeneral_BindingsToolbar_ItemClicked);
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Tag = "add";
            button_Add.ToolTipText = "Add";
            m_pTabGeneral_BindingsToolbar.Items.Add(button_Add);
            // Edit button
            ToolStripButton button_Edit = new ToolStripButton();
            button_Edit.Enabled = false;
            button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            button_Edit.Tag = "edit";
            button_Edit.ToolTipText = "edit";
            m_pTabGeneral_BindingsToolbar.Items.Add(button_Edit);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText = "Delete";
            m_pTabGeneral_BindingsToolbar.Items.Add(button_Delete);

            m_pTabGeneral_Bindings = new ListView();
            m_pTabGeneral_Bindings.Size = new Size(485,100);
            m_pTabGeneral_Bindings.Location = new Point(10,350);
            m_pTabGeneral_Bindings.View = View.Details;
            m_pTabGeneral_Bindings.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            m_pTabGeneral_Bindings.HideSelection = false;
            m_pTabGeneral_Bindings.FullRowSelect = true;
            m_pTabGeneral_Bindings.MultiSelect = false;
            m_pTabGeneral_Bindings.SelectedIndexChanged += new EventHandler(m_pTabGeneral_Bindings_SelectedIndexChanged);
            m_pTabGeneral_Bindings.Columns.Add("Host Name",130,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("IP",140,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("Port",50,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("SSL",50,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("Certificate",60,HorizontalAlignment.Left);
                        
            // Tabpage General UI
            m_pTab.TabPages[0].Controls.Add(m_pEnabled);
            m_pTab.TabPages[0].Controls.Add(mt_GreetingText);
            m_pTab.TabPages[0].Controls.Add(m_pGreetingText);
            m_pTab.TabPages[0].Controls.Add(mt_DefaultDomain);
            m_pTab.TabPages[0].Controls.Add(m_pDefaultDomain);
            m_pTab.TabPages[0].Controls.Add(mt_SessionTimeout);
            m_pTab.TabPages[0].Controls.Add(m_pSessionTimeout);
            m_pTab.TabPages[0].Controls.Add(mt_SessTimeoutSec);
            m_pTab.TabPages[0].Controls.Add(mt_MaxConnections);
            m_pTab.TabPages[0].Controls.Add(m_pMaxConnections);
            m_pTab.TabPages[0].Controls.Add(mt_MaxConnsPerIP);
            m_pTab.TabPages[0].Controls.Add(m_pMaxConnsPerIP);
            m_pTab.TabPages[0].Controls.Add(mt_MaxConnsPerIP0);
            m_pTab.TabPages[0].Controls.Add(mt_MaxBadCommands);
            m_pTab.TabPages[0].Controls.Add(m_pMaxBadCommands);
            m_pTab.TabPages[0].Controls.Add(mt_MaxRecipients);
            m_pTab.TabPages[0].Controls.Add(m_pMaxRecipients);
            m_pTab.TabPages[0].Controls.Add(mt_MaxMessageSize);
            m_pTab.TabPages[0].Controls.Add(m_pMaxMessageSize);
            m_pTab.TabPages[0].Controls.Add(m_pMaxMsgSizeMB);
            m_pTab.TabPages[0].Controls.Add(mt_MaxTransactions);
            m_pTab.TabPages[0].Controls.Add(m_pMaxTransactions);
            m_pTab.TabPages[0].Controls.Add(mt_MaxTransactions0);
            m_pTab.TabPages[0].Controls.Add(m_pRequireAuth);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_Bindings);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_BindingsToolbar);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_Bindings);
            //-------------------------------------------------//
                        
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


        #region method m_pTabGeneral_BindingsToolbar_ItemClicked

        private void m_pTabGeneral_BindingsToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_sys_BindInfo frm = new wfrm_sys_BindInfo(m_pVirtualServer.Server,false,WellKnownPorts.SMTP,WellKnownPorts.SMTP_SSL);
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
                    m_pTabGeneral_Bindings.Items.Add(it);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                if(m_pTabGeneral_Bindings.SelectedItems.Count > 0){
                    ListViewItem it = m_pTabGeneral_Bindings.SelectedItems[0];
                    wfrm_sys_BindInfo frm = new wfrm_sys_BindInfo(m_pVirtualServer.Server,false,WellKnownPorts.SMTP,WellKnownPorts.SMTP_SSL,(IPBindInfo)it.Tag);
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
                if(m_pTabGeneral_Bindings.SelectedItems.Count > 0){
                    if(MessageBox.Show(this,"Are you sure you want to delete binding '" + m_pTabGeneral_Bindings.SelectedItems[0].SubItems[0].Text + ":" + m_pTabGeneral_Bindings.SelectedItems[0].SubItems[1].Text + "' ?","Confirm:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){                    
                        m_pTabGeneral_Bindings.SelectedItems[0].Remove();
                    }
                }
            }
        }

        #endregion

        #region method m_pTabGeneral_Bindings_SelectedIndexChanged

        private void m_pTabGeneral_Bindings_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pTabGeneral_Bindings.SelectedItems.Count > 0){
                m_pTabGeneral_BindingsToolbar.Items[1].Enabled = true;
                m_pTabGeneral_BindingsToolbar.Items[2].Enabled = true;
            }
            else{
                m_pTabGeneral_BindingsToolbar.Items[1].Enabled = false;
                m_pTabGeneral_BindingsToolbar.Items[2].Enabled = false;
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
                SMTP_Settings settings =  m_pVirtualServer.SystemSettings.SMTP;
 		
				m_pEnabled.Checked       = settings.Enabled; 
                m_pGreetingText.Text     = settings.GreetingText;
                m_pDefaultDomain.Text    = settings.DefaultDomain;
				m_pSessionTimeout.Value  = settings.SessionIdleTimeOut;
				m_pMaxConnections.Value  = settings.MaximumConnections;
                m_pMaxConnsPerIP.Value   = settings.MaximumConnectionsPerIP;
				m_pMaxBadCommands.Value  = settings.MaximumBadCommands;
				m_pMaxMessageSize.Value  = settings.MaximumMessageSize;
				m_pMaxRecipients.Value   = settings.MaximumRecipientsPerMessage;
                m_pMaxTransactions.Value = settings.MaximumTransactions;
				m_pRequireAuth.Checked   = settings.RequireAuthentication;
                                
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
                    m_pTabGeneral_Bindings.Items.Add(it);
                }

                m_pTabGeneral_Bindings_SelectedIndexChanged(this,new EventArgs());
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
                SMTP_Settings settings =  m_pVirtualServer.SystemSettings.SMTP;
 		
				settings.Enabled                     = m_pEnabled.Checked; 
                settings.GreetingText                = m_pGreetingText.Text;
                settings.DefaultDomain               = m_pDefaultDomain.Text;
				settings.SessionIdleTimeOut          = (int)m_pSessionTimeout.Value;
				settings.MaximumConnections          = (int)m_pMaxConnections.Value;
                settings.MaximumConnectionsPerIP     = (int)m_pMaxConnsPerIP.Value;
				settings.MaximumBadCommands          = (int)m_pMaxBadCommands.Value;
				settings.MaximumMessageSize          = (int)m_pMaxMessageSize.Value;
				settings.MaximumRecipientsPerMessage = (int)m_pMaxRecipients.Value;
                settings.MaximumTransactions         = (int)m_pMaxTransactions.Value;
				settings.RequireAuthentication       = m_pRequireAuth.Checked;
                // IP binds
                List<IPBindInfo> binds = new List<IPBindInfo>();
                foreach(ListViewItem it in m_pTabGeneral_Bindings.Items){
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

    }
}
