using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Security IP security window.
    /// </summary>
    public class wfrm_Security_IPSecurity : Form
    {
        private ToolStrip m_pToolbar          = null;
        private ImageList m_pIPSecurityImages = null;
        private ListView  m_pIPSecurity       = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_Security_IPSecurity(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadSecurity("");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(472,357);

            ImageList toobarImageList = new ImageList();
            toobarImageList.Images.Add(ResManager.GetIcon("add.ico"));
            toobarImageList.Images.Add(ResManager.GetIcon("edit.ico"));
            toobarImageList.Images.Add(ResManager.GetIcon("delete.ico"));

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
            // Edit button
            ToolStripButton button_Edit = new ToolStripButton();
            button_Edit.Enabled = false;
            button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            button_Edit.Tag = "edit";
            button_Edit.ToolTipText = "Edit";
            m_pToolbar.Items.Add(button_Edit);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText = "Delete";
            m_pToolbar.Items.Add(button_Delete);
            // Separator
            m_pToolbar.Items.Add(new ToolStripSeparator());
            // Refresh button
            ToolStripButton button_Refresh = new ToolStripButton();
            button_Refresh.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
            button_Refresh.Tag = "refresh";
            button_Refresh.ToolTipText  = "Refresh";
            m_pToolbar.Items.Add(button_Refresh);

            m_pIPSecurityImages = new ImageList();            
            m_pIPSecurityImages.Images.Add(ResManager.GetIcon("security.ico"));
            m_pIPSecurityImages.Images.Add(ResManager.GetIcon("security_disabled.ico"));
 
            m_pIPSecurity = new ListView();
            m_pIPSecurity.Size = new Size(445,265);
            m_pIPSecurity.Location = new Point(9,47);
            m_pIPSecurity.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pIPSecurity.View = View.Details;
            m_pIPSecurity.FullRowSelect = true;
            m_pIPSecurity.HideSelection = false;
            m_pIPSecurity.SmallImageList = m_pIPSecurityImages;
            m_pIPSecurity.SelectedIndexChanged += new EventHandler(m_pIPSecurity_SelectedIndexChanged);
            m_pIPSecurity.DoubleClick += new EventHandler(m_pIPSecurity_DoubleClick);
            m_pIPSecurity.MouseUp += new MouseEventHandler(m_pIPSecurity_MouseUp);
            m_pIPSecurity.Columns.Add("Description",180,HorizontalAlignment.Left);
            m_pIPSecurity.Columns.Add("Service",55,HorizontalAlignment.Left);
            m_pIPSecurity.Columns.Add("Action",55,HorizontalAlignment.Left);
            m_pIPSecurity.Columns.Add("StartIP",120,HorizontalAlignment.Left);
            m_pIPSecurity.Columns.Add("EndIP",120,HorizontalAlignment.Left);
            
            this.Controls.Add(m_pToolbar);
            this.Controls.Add(m_pIPSecurity);
        }
                                
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            SwitchToolBarTask(e.ClickedItem.Tag.ToString());
        }

        #endregion


        #region method m_pIPSecurity_SelectedIndexChanged

        private void m_pIPSecurity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pIPSecurity.Items.Count > 0 && m_pIPSecurity.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
            }
        }

        #endregion

        #region method m_pIPSecurity_DoubleClick

        private void m_pIPSecurity_DoubleClick(object sender, EventArgs e)
        {             
            if(m_pIPSecurity.Items.Count > 0 && m_pIPSecurity.SelectedItems.Count > 0){
                IPSecurity securityEntry = (IPSecurity)m_pIPSecurity.SelectedItems[0].Tag;
                wfrm_Security_IPSecurityEntry frm = new wfrm_Security_IPSecurityEntry(m_pVirtualServer,securityEntry);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadSecurity(frm.SecurityEntryID);
                }
            }
        }

        #endregion

        #region method m_pIPSecurity_MouseUp

        private void m_pIPSecurity_MouseUp(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pIPSecurity_ContextMenuItem_Clicked);                       
            //--- MenuItem Add 
            ToolStripMenuItem menuItem_Add = new ToolStripMenuItem("Add");
            menuItem_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            menuItem_Add.Tag = "add";
            menu.Items.Add(menuItem_Add);
            //--- MenuItem Edit
            ToolStripMenuItem menuItem_Edit = new ToolStripMenuItem("Edit");
            menuItem_Edit.Enabled = m_pIPSecurity.SelectedItems.Count > 0;
            menuItem_Edit.Tag = "edit";
            menuItem_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            menu.Items.Add(menuItem_Edit);
            //--- MenuItem Delete
            ToolStripMenuItem menuItem_Delete = new ToolStripMenuItem("Delete");
            menuItem_Delete.Enabled = m_pIPSecurity.SelectedItems.Count > 0;
            menuItem_Delete.Tag = "delete";
            menuItem_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            menu.Items.Add(menuItem_Delete);
            //--- Separator
            menu.Items.Add(new ToolStripSeparator());
            //--- MenuItem Refresh
            ToolStripMenuItem menuItem_Refresh = new ToolStripMenuItem("Refresh");
            menuItem_Refresh.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
            menuItem_Refresh.Tag = "refresh";
            menu.Items.Add(menuItem_Refresh); 
            //---
            menu.Show(Control.MousePosition);
        }

        #endregion

        #region method m_pIPSecurity_ContextMenuItem_Clicked

        private void m_pIPSecurity_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            SwitchToolBarTask(e.ClickedItem.Tag.ToString());
        }

        #endregion

        #endregion


        #region method SwitchToolBarTask

        /// <summary>
        /// Executes specified tool bar task.
        /// </summary>
        /// <param name="taskID">Task ID.</param>
        private void SwitchToolBarTask(string taskID)
        {
            if(taskID == "add"){
                wfrm_Security_IPSecurityEntry frm = new wfrm_Security_IPSecurityEntry(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadSecurity(frm.SecurityEntryID);
                }
            }
            else if(taskID == "edit"){
                IPSecurity securityEntry = (IPSecurity)m_pIPSecurity.SelectedItems[0].Tag;
                wfrm_Security_IPSecurityEntry frm = new wfrm_Security_IPSecurityEntry(m_pVirtualServer,securityEntry);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadSecurity(frm.SecurityEntryID);
                }
            }
            else if(taskID == "delete"){
                IPSecurity securityEntry = (IPSecurity)m_pIPSecurity.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete IP Security entry '" + securityEntry.Description + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    securityEntry.Owner.Remove(securityEntry);
                    m_pIPSecurity.SelectedItems[0].Remove();
                }
            }
            else if(taskID == "refresh"){                
                LoadSecurity("");
            }
        }

        #endregion


        #region method LoadSecurity

        /// <summary>
        /// Loads IP security to UI.
        /// </summary>
        /// <param name="selectedSecurityEntry">Selects specified security entry, if exists.</param>
        private void LoadSecurity(string selectedSecurityEntry)
        {
            m_pIPSecurity.Items.Clear();
            m_pVirtualServer.IpSecurity.Refresh();
            foreach(IPSecurity securityEntry in m_pVirtualServer.IpSecurity){
                ListViewItem it = new ListViewItem();
                // Make disabled rules red and striked out
                if(!securityEntry.Enabled){
                    it.ForeColor = Color.Purple;
                    it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                    it.ImageIndex = 1;
                }
                else{
                    it.ImageIndex = 0;
                }              
                it.Tag = securityEntry;
                it.Text = securityEntry.Description;
                it.SubItems.Add(securityEntry.Service.ToString());
                it.SubItems.Add(securityEntry.Action.ToString());
                it.SubItems.Add(securityEntry.StartIP.ToString());
                it.SubItems.Add(securityEntry.EndIP.ToString());
                m_pIPSecurity.Items.Add(it);

                if(securityEntry.ID == selectedSecurityEntry){
                    it.Selected = true;
                }
            }

            m_pIPSecurity_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

    }
}
