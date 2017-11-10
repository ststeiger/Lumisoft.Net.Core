using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// System general settings window.
    /// </summary>
    public class wfrm_System_General : Form
    {
        private TabControl m_pTab   = null;
        private Button     m_pApply = null;
        // Tabpage General
        private Label              mt_TabGeneral_DnsServers       = null;
        private TextBox            m_pTabGeneral_DnsServer        = null;
        private ToolStrip          m_pTabGeneral_DnsServerToolbar = null;
        private ListView           m_pTabGeneral_DnsServers       = null;
        private wctrl_Notification m_pTabGeneral_Notification     = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_System_General(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadData();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes UI.
        /// </summary>
        private void InitUI()
        {
            m_pTab = new TabControl();
            m_pTab.Size = new Size(515,490);
            m_pTab.Location = new Point(5,0);

            m_pApply = new Button();
            m_pApply.Size = new Size(70,20);
            m_pApply.Location = new Point(450,500);
            m_pApply.Text = "Apply";
            m_pApply.Click += new EventHandler(m_pApply_Click);

            #region Tabpage General

            m_pTab.TabPages.Add("General");
                        
            mt_TabGeneral_DnsServers = new Label();
            mt_TabGeneral_DnsServers.Size = new Size(80,20);
            mt_TabGeneral_DnsServers.Location = new Point(0,10);
            mt_TabGeneral_DnsServers.TextAlign = ContentAlignment.MiddleRight;
            mt_TabGeneral_DnsServers.Text = "DNS servers:";

            m_pTabGeneral_DnsServer = new TextBox();
            m_pTabGeneral_DnsServer.Size = new Size(180,20);
            m_pTabGeneral_DnsServer.Location = new Point(85,10);

            m_pTabGeneral_DnsServerToolbar = new ToolStrip();
            m_pTabGeneral_DnsServerToolbar.Size = new Size(60,25);
            m_pTabGeneral_DnsServerToolbar.Location = new Point(270,8);
            m_pTabGeneral_DnsServerToolbar.Dock = DockStyle.None;
            m_pTabGeneral_DnsServerToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTabGeneral_DnsServerToolbar.BackColor = this.BackColor;
            m_pTabGeneral_DnsServerToolbar.Renderer = new ToolBarRendererEx();
            m_pTabGeneral_DnsServerToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTabGeneral_DnsServerToolbar_ItemClicked);
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Tag = "add";
            button_Add.ToolTipText = "Add";
            m_pTabGeneral_DnsServerToolbar.Items.Add(button_Add);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText = "Delete";
            m_pTabGeneral_DnsServerToolbar.Items.Add(button_Delete);
            // Separator
            m_pTabGeneral_DnsServerToolbar.Items.Add(new ToolStripSeparator());
            // Up button
            ToolStripButton button_Up = new ToolStripButton();
            button_Up.Enabled = false;
            button_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            button_Up.Tag = "up";
            m_pTabGeneral_DnsServerToolbar.Items.Add(button_Up);
            // Down button
            ToolStripButton button_down = new ToolStripButton();
            button_down.Enabled = false;
            button_down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            button_down.Tag = "down";
            m_pTabGeneral_DnsServerToolbar.Items.Add(button_down);

            m_pTabGeneral_DnsServers = new ListView();
            m_pTabGeneral_DnsServers.Size = new Size(355,100);
            m_pTabGeneral_DnsServers.Location = new Point(10,35);
            m_pTabGeneral_DnsServers.View = View.Details;
            m_pTabGeneral_DnsServers.FullRowSelect = true;
            m_pTabGeneral_DnsServers.HideSelection = false;
            m_pTabGeneral_DnsServers.SelectedIndexChanged += new EventHandler(m_pTabGeneral_DnsServers_SelectedIndexChanged);
            m_pTabGeneral_DnsServers.Columns.Add("IP",325);

            m_pTabGeneral_Notification = new wctrl_Notification();
            m_pTabGeneral_Notification.Size = new Size(485,38);
            m_pTabGeneral_Notification.Location = new Point(10,421);
            m_pTabGeneral_Notification.Icon = ResManager.GetIcon("warning.ico").ToBitmap();
            m_pTabGeneral_Notification.Visible = false;
            
            m_pTab.TabPages[0].Controls.Add(mt_TabGeneral_DnsServers);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_DnsServer);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_DnsServerToolbar);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_DnsServers);
            m_pTab.TabPages[0].Controls.Add(m_pTabGeneral_Notification);

            #endregion

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


        #region method m_pTabGeneral_DnsServerToolbar_ItemClicked

        private void m_pTabGeneral_DnsServerToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
            }
            else if(e.ClickedItem.Tag.ToString() == "add"){
                try{
                    IPAddress.Parse(m_pTabGeneral_DnsServer.Text);
                }
                catch{
                    MessageBox.Show(this,"Invalid DNS server IP address !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                ListViewItem item = new ListViewItem(m_pTabGeneral_DnsServer.Text);
                m_pTabGeneral_DnsServers.Items.Add(item);

                m_pTabGeneral_DnsServer.Text = "";
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                m_pTabGeneral_DnsServers.SelectedItems[0].Remove();
            }
            else if(e.ClickedItem.Tag.ToString() == "up"){
                ListViewItem it1 = m_pTabGeneral_DnsServers.SelectedItems[0];
                ListViewItem it2 = m_pTabGeneral_DnsServers.Items[it1.Index - 1];
                m_pTabGeneral_DnsServers.Items.Remove(it2);
                m_pTabGeneral_DnsServers.Items.Insert(it1.Index + 1,it2);
            }
            else if(e.ClickedItem.Tag.ToString() == "down"){
                ListViewItem it1 = m_pTabGeneral_DnsServers.SelectedItems[0];
                ListViewItem it2 = m_pTabGeneral_DnsServers.Items[it1.Index + 1];
                m_pTabGeneral_DnsServers.Items.Remove(it2);
                m_pTabGeneral_DnsServers.Items.Insert(it1.Index,it2);
            }

            m_pTabGeneral_DnsServers_SelectedIndexChanged(this,new EventArgs());
            AddNotifications();
        }

        #endregion

        #region method m_pTabGeneral_DnsServers_SelectedIndexChanged

        private void m_pTabGeneral_DnsServers_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pTabGeneral_DnsServers.SelectedItems.Count > 0){
                m_pTabGeneral_DnsServerToolbar.Items[1].Enabled = true;
                if(m_pTabGeneral_DnsServers.SelectedItems[0].Index > 0){
                    m_pTabGeneral_DnsServerToolbar.Items[3].Enabled = true;
                }
                else{
                    m_pTabGeneral_DnsServerToolbar.Items[3].Enabled = false;
                }
                if(m_pTabGeneral_DnsServers.SelectedItems[0].Index < (m_pTabGeneral_DnsServers.Items.Count - 1)){
                    m_pTabGeneral_DnsServerToolbar.Items[4].Enabled = true;
                }
                else{
                    m_pTabGeneral_DnsServerToolbar.Items[4].Enabled = false;
                }
            }
            else{
                m_pTabGeneral_DnsServerToolbar.Items[1].Enabled = false;
                m_pTabGeneral_DnsServerToolbar.Items[3].Enabled = false;
                m_pTabGeneral_DnsServerToolbar.Items[4].Enabled = false;
            }
        }

        #endregion


        #region method m_pApply_Click

        private void m_pApply_Click(object sender,EventArgs e)
        {
            SaveData(false);
        }

        #endregion


        #region method m_pTestDns_Click

        private void m_pTestDns_Click(object sender, EventArgs e)
        {
            /*
            Dns_Client.DnsServers  = new string[]{m_pDns1.Text};
			Dns_Client.UseDnsCache = false;
			Dns_Client dns = new Dns_Client();

			DnsServerResponse response = dns.Query("lumisoft.ee",QTYPE.MX);
			if(!response.ConnectionOk || response.ResponseCode != RCODE.NO_ERROR){
				MessageBox.Show(this,"Invalid dns server(" + m_pDns1.Text + "), can't resolve lumisoft.ee","Info",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}

			Dns_Client.DnsServers  = new string[]{m_pDns2.Text};
			Dns_Client.UseDnsCache = false;
			Dns_Client dns2 = new Dns_Client();

			response = dns2.Query("lumisoft.ee",QTYPE.MX);
			if(!response.ConnectionOk || response.ResponseCode != RCODE.NO_ERROR){
				MessageBox.Show(this,"Invalid dns server(" + m_pDns2.Text + "), can't resolve lumisoft.ee","Info",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				return;
			}

			MessageBox.Show(this,"Ok.","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);*/
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads system general settings to UI.
        /// </summary>
        private void LoadData()
        {
            try{
                System_Settings settings = m_pVirtualServer.SystemSettings;
                
                foreach(IPAddress ip in settings.DnsServers){
                    ListViewItem it = new ListViewItem(ip.ToString());
                    m_pTabGeneral_DnsServers.Items.Add(it);
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
        /// Savves system general settings.
        /// </summary>
        /// <param name="confirmSave">Specifies is save confirmation UI is showed.</param>
        private void SaveData(bool confirmSave)
        {
            try{
                System_Settings settings = m_pVirtualServer.SystemSettings;

                List<IPAddress> dnsServers = new List<IPAddress>();
                foreach(ListViewItem item in m_pTabGeneral_DnsServers.Items){
                    dnsServers.Add(IPAddress.Parse(item.Text));
                }
                settings.DnsServers = dnsServers.ToArray();

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
            if(m_pTabGeneral_DnsServers.Items.Count == 0){
                m_pTabGeneral_Notification.Visible = true;
                m_pTabGeneral_Notification.Text    = "You need to specifiy at least 1 DNS server.\n";
            }
        }

        #endregion

    }
}
