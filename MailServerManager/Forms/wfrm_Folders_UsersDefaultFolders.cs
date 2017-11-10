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
    /// User default folders UI.
    /// </summary>
    public class wfrm_Folders_UsersDefaultFolders : Form
    {
        private ToolStrip m_pToolbar = null;
        private ListView  m_pFolders = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Reference to virtual server.</param>
        /// <param name="frame">Reference to WFrame, where to set toolbar.</param>
        public wfrm_Folders_UsersDefaultFolders(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadFolders("");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(400,300);

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
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            m_pToolbar.Items.Add(button_Delete);

            ImageList foldersImages = new ImageList();            
            foldersImages.Images.Add(ResManager.GetIcon("folder.ico"));

            m_pFolders = new ListView();
            m_pFolders.Size = new Size(375,230);
            m_pFolders.Location = new Point(10,20);
            m_pFolders.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pFolders.View = View.Details;
            m_pFolders.HideSelection = false;
            m_pFolders.FullRowSelect = true;
            m_pFolders.SmallImageList = foldersImages;
            m_pFolders.SelectedIndexChanged += new EventHandler(m_pFolders_SelectedIndexChanged);
            m_pFolders.Columns.Add("Folder",200,HorizontalAlignment.Left);
            m_pFolders.Columns.Add("Permanent",65,HorizontalAlignment.Left);

            this.Controls.Add(m_pFolders);
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
                wfrm_Folders_UsersDefaultFolders_Folder frm = new wfrm_Folders_UsersDefaultFolders_Folder(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadFolders(frm.FolderName);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                UsersDefaultFolder folder = (UsersDefaultFolder)m_pFolders.SelectedItems[0].Tag;

                if(folder.FolderName.ToLower() == "inbox"){
                    MessageBox.Show(this,"Inbox is permanent system folder and can't be deleted ! '","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                if(MessageBox.Show(this,"Are you sure you want to delete Users Default Folder '" + folder.FolderName + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    folder.Owner.Remove(folder);
                    LoadFolders("");
                }
            }
        }

        #endregion


        #region method m_pFolders_SelectedIndexChanged

        private void m_pFolders_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pFolders.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
            }
        }

        #endregion

        #endregion


        #region method LoadFolders

        /// <summary>
        /// Loads users default folders to UI.
        /// </summary>
        /// <param name="selectedFolderName">Selects specified folder, if exists.</param>
        private void LoadFolders(string selectedFolderName)
        {
            m_pFolders.Items.Clear();

            foreach(UsersDefaultFolder folder in m_pVirtualServer.UsersDefaultFolders){
                ListViewItem it = new ListViewItem(folder.FolderName);
                it.ImageIndex = 0;
                it.SubItems.Add(folder.Permanent.ToString());
                it.Tag = folder;
                m_pFolders.Items.Add(it);

                if(folder.FolderName.ToLower() == selectedFolderName.ToLower()){
                    it.Selected = true;
                }
            }

            m_pFolders_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

    }
}
