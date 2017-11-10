using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.Net;
using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// System Services SIP settings window.
    /// </summary>
    public class wfrm_System_Services_SIP : Form
    {
        //--- Common UI ------------------------------
        private TabControl    m_pTab      = null;
        private Button        m_pApply    = null;
        //--- Tabpage General UI ---------------------------------
        private CheckBox      m_pTabGeneral_Enabled         = null;
        private Label         mt_TabGeneral_ProxyType       = null;
        private ComboBox      m_pTabGeneral_ProxyType       = null;
        private Label         mt_TabGeneral_MinExpires      = null;
        private NumericUpDown m_pTabGeneral_MinExpires      = null;
        private Label         mt_TabGeneral_Bindings        = null;
        private ToolStrip     m_pTabGeneral_BindingsToolbar = null;
        private ListView      m_pTabGeneral_Bindings        = null;
        //--- Tabpage Gateways UI ---------------------------------
        private ToolStrip     m_pTabGateways_GatewaysToolbar = null;
        private ListView      m_pTabGateways_Gateways        = null;
        //---------------------------------------------------------

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_System_Services_SIP(VirtualServer virtualServer)
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
            m_pTab.TabPages.Add(new TabPage("Gateways"));

            m_pApply = new Button();
            m_pApply.Size = new Size(70,20);
            m_pApply.Location = new Point(450,530);
            m_pApply.Text = "Apply";
            m_pApply.Click += new EventHandler(m_pApply_Click);
                        
            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pApply);
            //---------------------------------------------------//

            #region General

            //--- Tabpage General UI ----------------------------//
            m_pTabGeneral_Enabled = new CheckBox();
            m_pTabGeneral_Enabled.Size = new Size(70,20);
            m_pTabGeneral_Enabled.Location = new Point(170,10);
            m_pTabGeneral_Enabled.Text = "Enabled";

            mt_TabGeneral_ProxyType = new Label();
            mt_TabGeneral_ProxyType.Size = new Size(155,20);
            mt_TabGeneral_ProxyType.Location = new Point(10,40);
            mt_TabGeneral_ProxyType.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_ProxyType.Text = "Proxy Type:";

            m_pTabGeneral_ProxyType = new ComboBox();
            m_pTabGeneral_ProxyType.Size = new Size(100,20);
            m_pTabGeneral_ProxyType.Location = new Point(170,40);
            m_pTabGeneral_ProxyType.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pTabGeneral_ProxyType.Items.Add("B2BUA");
            m_pTabGeneral_ProxyType.Items.Add("Statefull");

            mt_TabGeneral_MinExpires = new Label();
            mt_TabGeneral_MinExpires.Size = new Size(155,20);
            mt_TabGeneral_MinExpires.Location = new Point(10,65);
            mt_TabGeneral_MinExpires.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_MinExpires.Text = "Minimum Expires:";

            m_pTabGeneral_MinExpires = new NumericUpDown();
            m_pTabGeneral_MinExpires.Size = new Size(70,20);
            m_pTabGeneral_MinExpires.Location = new Point(170,65);
            m_pTabGeneral_MinExpires.Minimum = 60;
            m_pTabGeneral_MinExpires.Maximum = 9999;

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
            m_pTabGeneral_Bindings.Columns.Add("Host Name",100,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("IP",140,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("Protocol",60,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("Port",50,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("SSL",50,HorizontalAlignment.Left);
            m_pTabGeneral_Bindings.Columns.Add("Certificate",60,HorizontalAlignment.Left);
                                    
            // Tabpage General UI
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_Enabled);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_ProxyType);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_ProxyType);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_MinExpires);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_MinExpires);
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_Bindings);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_BindingsToolbar);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_Bindings);
            //-------------------------------------------------//

            #endregion

            #region Gateways

            m_pTabGateways_GatewaysToolbar = new ToolStrip();         
            m_pTabGateways_GatewaysToolbar.Dock = DockStyle.None;
            m_pTabGateways_GatewaysToolbar.Location = new Point(430,15);
            //m_pTabGateways_GatewaysToolbar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pTabGateways_GatewaysToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTabGateways_GatewaysToolbar.BackColor = this.BackColor;
            m_pTabGateways_GatewaysToolbar.Renderer = new ToolBarRendererEx();
            m_pTabGateways_GatewaysToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTabGateways_GatewaysToolbar_ItemClicked);
            // Add button
            ToolStripButton gwButton_Add = new ToolStripButton();
            gwButton_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            gwButton_Add.Tag = "add";
            gwButton_Add.ToolTipText = "Add";
            m_pTabGateways_GatewaysToolbar.Items.Add(gwButton_Add);
            // Edit button
            ToolStripButton gwButton_Edit = new ToolStripButton();
            gwButton_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            gwButton_Edit.Tag = "edit";
            gwButton_Edit.ToolTipText = "Edit";
            m_pTabGateways_GatewaysToolbar.Items.Add(gwButton_Edit);
            // Delete button
            ToolStripButton gwButton_Delete = new ToolStripButton();
            gwButton_Delete.Enabled = false;
            gwButton_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            gwButton_Delete.Tag = "delete";
            gwButton_Delete.ToolTipText  = "Delete";
            m_pTabGateways_GatewaysToolbar.Items.Add(gwButton_Delete);

            m_pTabGateways_Gateways = new ListView();
            m_pTabGateways_Gateways.Size = new Size(495,415);
            m_pTabGateways_Gateways.Location = new Point(5,40);
            //m_pTabGateways_Gateways.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTabGateways_Gateways.View = View.Details;
            m_pTabGateways_Gateways.FullRowSelect = true;
            m_pTabGateways_Gateways.HideSelection = false;
            m_pTabGateways_Gateways.Columns.Add("URI scheme",80);
            m_pTabGateways_Gateways.Columns.Add("Transport",60);
            m_pTabGateways_Gateways.Columns.Add("Host",240);
            m_pTabGateways_Gateways.Columns.Add("Port",60);
            m_pTabGateways_Gateways.SelectedIndexChanged += new EventHandler(m_pTabGateways_Gateways_SelectedIndexChanged);
            m_pTabGateways_Gateways.DoubleClick += new EventHandler(m_pTabGateways_Gateways_DoubleClick);

            // Add controls to tab
            m_pTab.TabPages[1].Controls.Add(m_pTabGateways_GatewaysToolbar);
            m_pTab.TabPages[1].Controls.Add(m_pTabGateways_Gateways);

            #endregion
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
                wfrm_sys_BindInfo frm = new wfrm_sys_BindInfo(m_pVirtualServer.Server,true,5060,5061);
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
                    wfrm_sys_BindInfo frm = new wfrm_sys_BindInfo(m_pVirtualServer.Server,true,5060,5061,(IPBindInfo)it.Tag);
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
                        it.SubItems[2].Text = frm.Protocol.ToString();
                        it.SubItems[3].Text = frm.Port.ToString();
                        it.SubItems[4].Text = frm.SslMode.ToString();
                        it.SubItems[5].Text = Convert.ToString(frm.Certificate != null);
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


        #region method m_pTabGateways_GatewaysToolbar_ItemClicked

        private void m_pTabGateways_GatewaysToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_System_Services_SIP_Gateway frm = new wfrm_System_Services_SIP_Gateway();
                if(frm.ShowDialog(this) == DialogResult.OK){
                    SIP_Gateway gateway = m_pVirtualServer.SystemSettings.SIP.Gateways.Add(
                        frm.UriScheme,
                        frm.Transport,
                        frm.Host,
                        frm.Port,
                        frm.Realm,
                        frm.UserName,
                        frm.Password
                    );

                    ListViewItem item = new ListViewItem(frm.UriScheme);
                    item.SubItems.Add(frm.Transport);
                    item.SubItems.Add(frm.Host);
                    item.SubItems.Add(frm.Port.ToString());
                    item.Tag = gateway;
                    m_pTabGateways_Gateways.Items.Add(item);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                SIP_Gateway gw = (SIP_Gateway)m_pTabGateways_Gateways.SelectedItems[0].Tag;
                wfrm_System_Services_SIP_Gateway frm = new wfrm_System_Services_SIP_Gateway(
                    gw.UriScheme,
                    gw.Transport,
                    gw.Host,
                    gw.Port,
                    gw.Realm,
                    gw.UserName,
                    gw.Password
                );
                if(frm.ShowDialog(this) == DialogResult.OK){
                    gw.UriScheme = frm.UriScheme;
                    gw.Transport = frm.Transport;
                    gw.Host      = frm.Host;
                    gw.Port      = frm.Port;
                    gw.Realm     = frm.Realm;
                    gw.UserName  = frm.UserName;
                    gw.Password  = frm.Password;

                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.UriScheme;
                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.Transport;
                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.Host;
                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.Port.ToString();
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                SIP_Gateway gw = (SIP_Gateway)m_pTabGateways_Gateways.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to remove SIP selected gateway ?","Remove Gateway",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){                
                    gw.Remove();
                    m_pTabGateways_Gateways.SelectedItems[0].Remove();
                }
            }
        }

        #endregion

        #region method m_pTabGateways_Gateways_SelectedIndexChanged

        private void m_pTabGateways_Gateways_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pTabGateways_Gateways.SelectedItems.Count > 0){
                m_pTabGateways_GatewaysToolbar.Items[1].Enabled = true;
                m_pTabGateways_GatewaysToolbar.Items[2].Enabled = true;
            }
            else{
                m_pTabGateways_GatewaysToolbar.Items[1].Enabled = false;
                m_pTabGateways_GatewaysToolbar.Items[2].Enabled = false;
            }
        }

        #endregion

        #region method m_pTabGateways_Gateways_DoubleClick

        private void m_pTabGateways_Gateways_DoubleClick(object sender,EventArgs e)
        {
            if(m_pTabGateways_Gateways.SelectedItems.Count > 0){
                SIP_Gateway gw = (SIP_Gateway)m_pTabGateways_Gateways.SelectedItems[0].Tag;
                wfrm_System_Services_SIP_Gateway frm = new wfrm_System_Services_SIP_Gateway(
                    gw.UriScheme,
                    gw.Transport,
                    gw.Host,
                    gw.Port,
                    gw.Realm,
                    gw.UserName,
                    gw.Password
                );
                if(frm.ShowDialog(this) == DialogResult.OK){
                    gw.UriScheme = frm.UriScheme;
                    gw.Transport = frm.Transport;
                    gw.Host      = frm.Host;
                    gw.Port      = frm.Port;
                    gw.Realm     = frm.Realm;
                    gw.UserName  = frm.UserName;
                    gw.Password  = frm.Password;

                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.UriScheme;
                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.Transport;
                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.Host;
                    m_pTabGateways_Gateways.SelectedItems[0].SubItems[0].Text = frm.Port.ToString();
                }
            }
        }

        #endregion


        #region method m_pApply_Click

        private void m_pApply_Click(object sender,EventArgs e)
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
				SIP_Settings settings = m_pVirtualServer.SystemSettings.SIP;
            
				m_pTabGeneral_Enabled.Checked = settings.Enabled;
                if((settings.ProxyMode & LumiSoft.Net.SIP.Proxy.SIP_ProxyMode.B2BUA) != 0){
                    m_pTabGeneral_ProxyType.SelectedIndex = 0;
                }
                else{
                    m_pTabGeneral_ProxyType.SelectedIndex = 1;
                }
                m_pTabGeneral_MinExpires.Value = Convert.ToDecimal(settings.MinimumExpires);

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
                    it.SubItems.Add(binding.Protocol.ToString());
                    it.SubItems.Add(binding.Port.ToString());
                    it.SubItems.Add(binding.SslMode.ToString());
                    it.SubItems.Add(Convert.ToString(binding.Certificate != null));
                    it.Tag = binding;
                    m_pTabGeneral_Bindings.Items.Add(it);
                }

                foreach(SIP_Gateway gw in m_pVirtualServer.SystemSettings.SIP.Gateways){
                    ListViewItem item = new ListViewItem(gw.UriScheme);
                    item.SubItems.Add(gw.Transport);
                    item.SubItems.Add(gw.Host);
                    item.SubItems.Add(gw.Port.ToString());
                    item.Tag = gw;
                    m_pTabGateways_Gateways.Items.Add(item);
                }

                m_pTabGateways_Gateways_SelectedIndexChanged(null,null);
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
                SIP_Settings settings = m_pVirtualServer.SystemSettings.SIP;

                settings.Enabled = m_pTabGeneral_Enabled.Checked;
                if(m_pTabGeneral_ProxyType.SelectedIndex == 0){
                    settings.ProxyMode = LumiSoft.Net.SIP.Proxy.SIP_ProxyMode.Registrar | LumiSoft.Net.SIP.Proxy.SIP_ProxyMode.B2BUA;
                }
                else{
                    settings.ProxyMode = LumiSoft.Net.SIP.Proxy.SIP_ProxyMode.Registrar | LumiSoft.Net.SIP.Proxy.SIP_ProxyMode.Statefull;
                }
                settings.MinimumExpires = (int)m_pTabGeneral_MinExpires.Value;
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
