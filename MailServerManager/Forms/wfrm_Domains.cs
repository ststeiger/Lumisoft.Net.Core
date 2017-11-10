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
    /// Domains window.
    /// </summary>
    public class wfrm_Domains : Form
    {
        private ToolStrip m_pToolbar       = null;
        private ImageList m_pDomainsImages = null;
        private WListView m_pDomains       = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_Domains(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadDomains();
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

            m_pDomainsImages = new ImageList();            
            m_pDomainsImages.Images.Add(ResManager.GetIcon("domain.ico"));
 
            m_pDomains = new WListView();
            m_pDomains.Size = new Size(445,265);
            m_pDomains.Location = new Point(9,47);
            m_pDomains.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pDomains.View = View.Details;
            m_pDomains.FullRowSelect = true;
            m_pDomains.HideSelection = false;
            m_pDomains.SmallImageList = m_pDomainsImages;
            m_pDomains.SelectedIndexChanged += new EventHandler(m_pDomains_SelectedIndexChanged);
            m_pDomains.DoubleClick += new EventHandler(m_pDomains_DoubleClick);
            m_pDomains.MouseUp += new MouseEventHandler(m_pDomains_MouseUp);
            m_pDomains.Columns.Add("Name",190,HorizontalAlignment.Left);
            m_pDomains.Columns.Add("Description",290,HorizontalAlignment.Left);
            
            this.Controls.Add(m_pToolbar);
            this.Controls.Add(m_pDomains);
        }
                
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            SwitchToolBarTask(e.ClickedItem.Tag.ToString());
        }

        #endregion


        #region method m_pDomains_SelectedIndexChanged

        private void m_pDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pDomains.Items.Count > 0 && m_pDomains.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
            }
        }

        #endregion

        #region method m_pDomains_DoubleClick

        private void m_pDomains_DoubleClick(object sender, EventArgs e)
        {
            if(m_pDomains.SelectedItems.Count > 0){
                Domain domain = (Domain)m_pDomains.SelectedItems[0].Tag;

                wfrm_Domains_Domain frm = new wfrm_Domains_Domain(m_pVirtualServer,domain);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadDomains();
                }
            }
        }

        #endregion

        #region method m_pDomains_MouseUp

        private void m_pDomains_MouseUp(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pDomains_ContextMenuItem_Clicked);                       
            //--- MenuItem Add 
            ToolStripMenuItem menuItem_Add = new ToolStripMenuItem("Add");
            menuItem_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            menuItem_Add.Tag = "add";
            menu.Items.Add(menuItem_Add);
            //--- MenuItem Edit
            ToolStripMenuItem menuItem_Edit = new ToolStripMenuItem("Edit");
            menuItem_Edit.Enabled = m_pDomains.SelectedItems.Count > 0;
            menuItem_Edit.Tag = "edit";
            menuItem_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            menu.Items.Add(menuItem_Edit);
            //--- MenuItem Delete
            ToolStripMenuItem menuItem_Delete = new ToolStripMenuItem("Delete");
            menuItem_Delete.Enabled = m_pDomains.SelectedItems.Count > 0;
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

        #region method m_pDomains_ContextMenuItem_Clicked

        private void m_pDomains_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
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
                wfrm_Domains_Domain frm = new wfrm_Domains_Domain(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadDomains();
                }
            }
            else if(taskID == "edit"){
                Domain domain = (Domain)m_pDomains.SelectedItems[0].Tag;
                wfrm_Domains_Domain frm = new wfrm_Domains_Domain(m_pVirtualServer,domain);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadDomains();
                }
            }
            else if(taskID == "delete"){
                Domain domain = (Domain)m_pDomains.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Warning: Deleting domain '" + domain.DomainName + "', deletes all domain users and mailing lists,...!!!\nDo you want to continue?","Delete confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    domain.Owner.Remove(domain);
                    m_pDomains.SelectedItems[0].Remove();
				}
            }
            else if(taskID == "refresh"){                
                LoadDomains();
            }
        }

        #endregion


        #region method LoadDomains

        /// <summary>
        /// Loads domains to UI.
        /// </summary>
        private void LoadDomains()
        {
            m_pDomains.Items.Clear();
            m_pVirtualServer.Domains.Refresh();
            foreach(Domain domain in m_pVirtualServer.Domains){
                ListViewItem it = new ListViewItem();
                it.ImageIndex = 0;                
                it.Tag = domain;
                it.Text = domain.DomainName;
                it.SubItems.Add(domain.Description);
                m_pDomains.Items.Add(it);
            }

            m_pDomains.SortItems();
            m_pDomains_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

    }
}
