using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Public Folders window.
    /// </summary>
    public class wfrm_SharedFolders_RootFolders : Form
    {
        private ToolStrip m_pToolbar           = null;
        private ImageList m_pRootFoldersImages = null;
        private ListView  m_pRootFolders       = null;

        private VirtualServer m_pVirtualServer = null;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_SharedFolders_RootFolders(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();
            
            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadRoots("");
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
            m_pToolbar.Items.Add(button_Add);
            // Edit button
            ToolStripButton button_Edit = new ToolStripButton();
            button_Edit.Enabled = false;
            button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            button_Edit.Tag = "edit";
            m_pToolbar.Items.Add(button_Edit);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            m_pToolbar.Items.Add(button_Delete);

            m_pRootFoldersImages = new ImageList();
            m_pRootFoldersImages.Images.Add(ResManager.GetIcon("rootfolder.ico"));
            m_pRootFoldersImages.Images.Add(ResManager.GetIcon("rootfolder_disabled.ico"));

            m_pRootFolders = new ListView();
            m_pRootFolders.Size = new Size(445,265);
            m_pRootFolders.Location = new Point(10,50);
            m_pRootFolders.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pRootFolders.View = View.Details;
      //    m_pRootFolders.CheckBoxes = true;
            m_pRootFolders.HideSelection = false;
            m_pRootFolders.FullRowSelect = true;
            m_pRootFolders.SmallImageList = m_pRootFoldersImages;
            m_pRootFolders.SelectedIndexChanged += new EventHandler(m_pRootFolders_SelectedIndexChanged);
            m_pRootFolders.DoubleClick += new EventHandler(m_pRootFolders_DoubleClick);
            m_pRootFolders.Columns.Add("Folder Name",170,HorizontalAlignment.Left);
            m_pRootFolders.Columns.Add("Description",180,HorizontalAlignment.Left);
            m_pRootFolders.Columns.Add("Root Type",120,HorizontalAlignment.Left);

            this.Controls.Add(m_pRootFolders);
        }
                                                
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_SharedFolders_RootFolder frm = new wfrm_SharedFolders_RootFolder(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRoots(frm.RootID);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                SharedRootFolder root = (SharedRootFolder)m_pRootFolders.SelectedItems[0].Tag;
                wfrm_SharedFolders_RootFolder frm = new wfrm_SharedFolders_RootFolder(m_pVirtualServer,root);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRoots(root.ID);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                SharedRootFolder root = (SharedRootFolder)m_pRootFolders.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete Root folder '" + root.Name + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    root.Owner.Remove(root);
                    LoadRoots("");
                }
            }
        }

        #endregion


        #region method m_pRootFolders_SelectedIndexChanged

        private void m_pRootFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pRootFolders.Items.Count > 0 && m_pRootFolders.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
            }
        }

        #endregion

        #region method m_pRootFolders_DoubleClick

        private void m_pRootFolders_DoubleClick(object sender, EventArgs e)
        {
            if(m_pRootFolders.SelectedItems.Count > 0){
                SharedRootFolder root = (SharedRootFolder)m_pRootFolders.SelectedItems[0].Tag;
                wfrm_SharedFolders_RootFolder frm = new wfrm_SharedFolders_RootFolder(m_pVirtualServer,root);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRoots(root.ID);
                }
            }
        }

        #endregion

        #endregion


        #region method LoadRoots

        /// <summary>
        /// Loads root folders to UI.
        /// </summary>
        /// <param name="selectedRootID">Selects specified rule if exists.</param>
        private void LoadRoots(string selectedRootID)
        {
            m_pRootFolders.Items.Clear();

            foreach(SharedRootFolder rootFolder in  m_pVirtualServer.RootFolders){
                ListViewItem it = new ListViewItem();
                it.Text = rootFolder.Enabled.ToString();
                // Make disabled rules red and striked out
                if(!rootFolder.Enabled){
                    it.ForeColor = Color.Purple;
                    it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                    it.ImageIndex = 1;
                }
                else{
                    it.ImageIndex = 0;
                }
                it.Tag = rootFolder;
                it.Text = rootFolder.Name;
                it.SubItems.Add(rootFolder.Description);
                it.SubItems.Add(rootFolder.Type.ToString());
                m_pRootFolders.Items.Add(it);

                if(rootFolder.ID == selectedRootID){
                    it.Selected = true;
                }
            }

            m_pRootFolders_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

    }
}
