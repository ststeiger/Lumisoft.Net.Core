using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;
using LumiSoft.UI.Controls;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Monitoring -> SIP Registration window.
    /// </summary>
    public class wfrm_Monitoring_SipRegistrations : Form
    {
        private ToolStrip m_pToolbar       = null;
        private WListView m_pRegistrations = null;

        private Server m_pServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Mail server.</param>
        /// <param name="frame"></param>
        public wfrm_Monitoring_SipRegistrations(Server server,WFrame frame)
        {
            m_pServer = server;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadData();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(472,357);

            m_pToolbar = new ToolStrip();            
            m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pToolbar.BackColor = this.BackColor;
            m_pToolbar.Renderer = new ToolBarRendererEx();
            m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pToolbar_ItemClicked);
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Tag = "add";
            button_Add.ToolTipText = "Add";
            m_pToolbar.Items.Add(button_Add);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText  = "Delete";
            m_pToolbar.Items.Add(button_Delete);
            // Separator
            m_pToolbar.Items.Add(new ToolStripSeparator());
            // Refresh button
            ToolStripButton button_Refresh = new ToolStripButton();
            button_Refresh.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
            button_Refresh.Tag = "refresh";
            button_Refresh.ToolTipText  = "Refresh";
            m_pToolbar.Items.Add(button_Refresh);
            // View button
            ToolStripButton button_View = new ToolStripButton();
            button_View.Enabled = false;
            button_View.Image = ResManager.GetIcon("viewmessages.ico").ToBitmap();
            button_View.Tag = "view";
            button_View.ToolTipText  = "View Contacts";
            m_pToolbar.Items.Add(button_View);

            m_pRegistrations = new WListView();
            m_pRegistrations.Size = new Size(445,265);
            m_pRegistrations.Location = new Point(9,47);
            m_pRegistrations.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pRegistrations.View = View.Details;
            m_pRegistrations.FullRowSelect = true;
            m_pRegistrations.HideSelection = false;
            m_pRegistrations.Columns.Add("User",120,HorizontalAlignment.Left);
            m_pRegistrations.Columns.Add("Address of Record",360,HorizontalAlignment.Left);
            m_pRegistrations.SelectedIndexChanged += new EventHandler(m_pRegistrations_SelectedIndexChanged);
            m_pRegistrations.DoubleClick += new EventHandler(m_pRegistrations_DoubleClick);

            this.Controls.Add(m_pRegistrations);
        }
                                       
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_Monitoring_SipRegistration frm = new wfrm_Monitoring_SipRegistration(m_pServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    SipRegistration registration = frm.VirtualServer.SipRegistrations[frm.AddressOfRecord];
                    if(registration != null){
                        ListViewItem it = new ListViewItem(registration.UserName);
                        it.SubItems.Add(registration.AddressOfRecord);
                        it.Tag = registration;
                        m_pRegistrations.Items.Add(it);
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                SipRegistration registration = (SipRegistration)m_pRegistrations.SelectedItems[0].Tag;

                if(MessageBox.Show(this,"Are you sure you want to remove SIP registration '" + registration.AddressOfRecord + "' ?","Remove Registration",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){                    
                    registration.Owner.Remove(registration);
                    m_pRegistrations.SelectedItems[0].Remove();
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "refresh"){
                LoadData();
            }
            else if(e.ClickedItem.Tag.ToString() == "view"){
                ViewContacts();
            }
        }

        #endregion

        #region method m_pRegistrations_SelectedIndexChanged

        private void m_pRegistrations_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pRegistrations.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[4].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[4].Enabled = false;
            }
        }

        #endregion

        #region method m_pRegistrations_DoubleClick

        private void m_pRegistrations_DoubleClick(object sender,EventArgs e)
        {
            ViewContacts();
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads SIP registrations to UI.
        /// </summary>
        private void LoadData()
        {
            m_pRegistrations.Items.Clear();

            foreach(VirtualServer virtualServer in m_pServer.VirtualServers){
                virtualServer.SipRegistrations.Refresh();            
                foreach(SipRegistration registration in virtualServer.SipRegistrations){
                    ListViewItem it = new ListViewItem(registration.UserName);                
                    it.SubItems.Add(registration.AddressOfRecord);
                    it.Tag = registration;
                    m_pRegistrations.Items.Add(it);
                }
            }
        }

        #endregion

        #region method ViewContacts

        /// <summary>
        /// Show selected registration contacts window.
        /// </summary>
        private void ViewContacts()
        {
            if(m_pRegistrations.SelectedItems.Count > 0){
                SipRegistration registration = (SipRegistration)m_pRegistrations.SelectedItems[0].Tag;

                wfrm_Monitoring_SipRegistration frm = new wfrm_Monitoring_SipRegistration(m_pServer,registration);
                frm.ShowDialog(this);
            }
        }

        #endregion

    }
}
