using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// User folder selector window.
    /// </summary>
    public class wfrm_se_UserFolder : Form
    {
        private ImageList m_pFoldersImages = null;
        private TreeView  m_pFolders       = null;
        private GroupBox  m_pGroupBox1     = null;
        private Button    m_pCancel        = null;
        private Button    m_pOk            = null;

        private VirtualServer m_pVirtualServer = null;
        private string        m_User           = "";
        private string        m_SelectedFolder = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="user">User who's folders to show.</param>
        public wfrm_se_UserFolder(VirtualServer virtualServer,string user)
        {
            m_pVirtualServer = virtualServer;
            m_User           = user;

            InitUI();

            LoadFolders();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,373);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Select User Folder";

            m_pFoldersImages = new ImageList();
            m_pFoldersImages.Images.Add(ResManager.GetIcon("folder.ico"));

            m_pFolders = new TreeView();
            m_pFolders.Size = new Size(370,270);
            m_pFolders.Location = new Point(10,50);
            m_pFolders.HideSelection = false;
            m_pFolders.FullRowSelect = true;
            m_pFolders.ImageList = m_pFoldersImages;
            m_pFolders.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pFolders.DoubleClick += new EventHandler(m_pFolders_DoubleClick);

            m_pGroupBox1 = new GroupBox();
            m_pGroupBox1.Size = new Size(386,4);
            m_pGroupBox1.Location = new Point(4,332);
            m_pGroupBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(235,345);
            m_pCancel.Text = "Cancel";
            m_pCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            m_pCancel.Click += new EventHandler(m_pCancel_Click);
            
            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(310,345);
            m_pOk.Text = "Ok";
            m_pOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pFolders);
            this.Controls.Add(m_pGroupBox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                
        #endregion


        #region Events Handling

        #region method m_pFolders_DoubleClick

        private void m_pFolders_DoubleClick(object sender,EventArgs e)
        {/*
            if(m_pFolders.SelectedNode != null){
                m_SelectedFolder = m_pFolders.SelectedNode.Text;

                this.DialogResult = DialogResult.OK;
            }*/
        }

        #endregion


        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            if(m_pFolders.SelectedNode != null){
                m_SelectedFolder = m_pFolders.SelectedNode.Tag.ToString();

                this.DialogResult = DialogResult.OK;
            }
            else{
                this.DialogResult = DialogResult.Cancel;
            }
        }

        #endregion

        #endregion


        #region method LoadFolders

        /// <summary>
        /// Loads folders to UI.
        /// </summary>
        private void LoadFolders()
        {
            m_pFolders.Nodes.Clear();
                        
            Queue<object> folders = new Queue<object>();
            foreach(UserFolder folder in m_pVirtualServer.Users.GetUserByName(m_User).Folders){                
                TreeNode n = new TreeNode(folder.FolderName);
                n.ImageIndex = 0;
                n.Tag = folder.FolderFullPath;
                m_pFolders.Nodes.Add(n);

                folders.Enqueue(new object[]{folder,n});
            }

            while(folders.Count > 0){
                object[]   param  = (object[])folders.Dequeue();
                UserFolder folder = (UserFolder)param[0];
                TreeNode   node   = (TreeNode)param[1];
                foreach(UserFolder childFolder in folder.ChildFolders){                                        
                    TreeNode n = new TreeNode(childFolder.FolderName);
                    n.ImageIndex = 0;
                    n.Tag = childFolder.FolderFullPath;
                    node.Nodes.Add(n);

                    folders.Enqueue(new object[]{childFolder,n});
                }                
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets selected folder. Returns null if no selected folder.
        /// </summary>
        public string SelectedFolder
        {
            get{ return m_SelectedFolder; }
        }

        #endregion

    }
}
