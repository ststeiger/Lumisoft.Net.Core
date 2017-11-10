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
    /// Root folder Add/Edit window.
    /// </summary>
    public class wfrm_SharedFolders_RootFolder : Form
    {
        private PictureBox m_pIcon             = null;
        private Label      mt_Info             = null;
        private GroupBox   m_pSeparator1       = null;
        private CheckBox   m_pEnabled          = null;
        private Label      mt_RootFolderName   = null;
        private TextBox    m_pRootFolderName   = null;
        private Label      mt_Description      = null;
        private TextBox    m_pDescription      = null;
        private Label      mt_RootFolderType   = null;
        private ComboBox   m_pRootFolderType   = null;
        private Label      mt_BoundedUser      = null;
        private TextBox    m_pBoundedUser      = null;
        private Button     m_pGetBoundedUser   = null;
        private Label      mt_BoundedFolder    = null;
        private TextBox    m_pBoundedFolder    = null;
        private Button     m_pGetBoundedFolder = null;
        private GroupBox   m_pSeparator2       = null;
        private Button     m_pCancel           = null;
        private Button     m_Ok                = null;

        private VirtualServer    m_pVirtualServer = null;
        private SharedRootFolder m_pRootFolder    = null;

        /// <summary>
        /// Add constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_SharedFolders_RootFolder(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            m_pRootFolderType.SelectedIndex = 0;
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="root">Root Folder to update.</param>
        public wfrm_SharedFolders_RootFolder(VirtualServer virtualServer,SharedRootFolder root)
        {
            m_pVirtualServer = virtualServer;
            m_pRootFolder    = root;

            InitUI();

            m_pEnabled.Checked = root.Enabled;
            if(root.Type == SharedFolderRootType_enum.BoundedRootFolder){
                m_pRootFolderType.SelectedIndex = 0;
            }
            else if(root.Type == SharedFolderRootType_enum.UsersSharedFolder){
                m_pRootFolderType.SelectedIndex = 1;
            }
            m_pRootFolderName.Text = root.Name;
            m_pDescription.Text = root.Description;
            m_pBoundedUser.Text = root.BoundedUser;
            m_pBoundedFolder.Text = root.BoundedFolder;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(442,273);
            this.Text = "Shared Folders Add/Edit Root folder";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Icon = ResManager.GetIcon("share32.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("share32.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify share information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(432,3);
            m_pSeparator1.Location = new Point(7,50);

            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(300,20);
            m_pEnabled.Location = new Point(125,65);
            m_pEnabled.Text = "Enabled";
            m_pEnabled.Checked = true;

            mt_RootFolderName = new Label();
            mt_RootFolderName.Size = new Size(120,20);
            mt_RootFolderName.Location = new Point(0,90);
            mt_RootFolderName.TextAlign = ContentAlignment.MiddleRight;
            mt_RootFolderName.Text = "Root Folder Name:";

            m_pRootFolderName = new TextBox();
            m_pRootFolderName.Size = new Size(310,20);
            m_pRootFolderName.Location = new Point(125,90);

            mt_Description = new Label();
            mt_Description.Size = new Size(120,20);
            mt_Description.Location = new Point(0,115);
            mt_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(310,20);
            m_pDescription.Location = new Point(125,115);

            mt_RootFolderType = new Label();
            mt_RootFolderType.Size = new Size(120,20);
            mt_RootFolderType.Location = new Point(0,140);
            mt_RootFolderType.TextAlign = ContentAlignment.MiddleRight;
            mt_RootFolderType.Text = "Root Folder Type:";

            m_pRootFolderType = new ComboBox();
            m_pRootFolderType.Size = new Size(200,20);
            m_pRootFolderType.Location = new Point(125,140);
            m_pRootFolderType.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pRootFolderType.SelectedIndexChanged += new EventHandler(m_pRootFolderType_SelectedIndexChanged);
            m_pRootFolderType.Items.Add(new WComboBoxItem("Bounded Root Folder",SharedFolderRootType_enum.BoundedRootFolder));
            m_pRootFolderType.Items.Add(new WComboBoxItem("Users Shared Folder",SharedFolderRootType_enum.UsersSharedFolder));

            mt_BoundedUser = new Label();
            mt_BoundedUser.Size = new Size(120,20);
            mt_BoundedUser.Location = new Point(0,170);
            mt_BoundedUser.TextAlign = ContentAlignment.MiddleRight;
            mt_BoundedUser.Text = "Bounded User:";

            m_pBoundedUser = new TextBox();
            m_pBoundedUser.Size = new Size(280,20);
            m_pBoundedUser.Location = new Point(125,170);
            m_pBoundedUser.ReadOnly = true;

            m_pGetBoundedUser = new Button();
            m_pGetBoundedUser.Size = new Size(25,20);
            m_pGetBoundedUser.Location = new Point(410,170);
            m_pGetBoundedUser.Text = "...";
            m_pGetBoundedUser.Click += new EventHandler(m_pGetBoundedUser_Click);

            mt_BoundedFolder = new Label();
            mt_BoundedFolder.Size = new Size(120,20);
            mt_BoundedFolder.Location = new Point(0,195);
            mt_BoundedFolder.TextAlign = ContentAlignment.MiddleRight;
            mt_BoundedFolder.Text = "Bounded User Folder:";

            m_pBoundedFolder = new TextBox();
            m_pBoundedFolder.Size = new Size(280,20);
            m_pBoundedFolder.Location = new Point(125,195);
            m_pBoundedFolder.ReadOnly = true;

            m_pGetBoundedFolder = new Button();
            m_pGetBoundedFolder.Size = new Size(25,20);
            m_pGetBoundedFolder.Location = new Point(410,195);
            m_pGetBoundedFolder.Text = "...";
            m_pGetBoundedFolder.Click += new EventHandler(m_pGetBoundedFolder_Click);
                        
            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(432,2);
            m_pSeparator2.Location = new Point(7,235);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,21);
            m_pCancel.Location = new Point(290,248);
            m_pCancel.Click += new EventHandler(m_pCancel_Click);
            m_pCancel.Text = "Cancel";

            m_Ok = new Button();
            m_Ok.Size = new Size(70,21);
            m_Ok.Location = new Point(365,248);
            m_Ok.Click += new EventHandler(m_Ok_Click);
            m_Ok.Text = "Ok";

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(m_pEnabled);
            this.Controls.Add(mt_RootFolderName);
            this.Controls.Add(m_pRootFolderName);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pDescription);
            this.Controls.Add(mt_RootFolderType);
            this.Controls.Add(m_pRootFolderType);
            this.Controls.Add(mt_BoundedUser);
            this.Controls.Add(m_pBoundedUser);
            this.Controls.Add(m_pGetBoundedUser);
            this.Controls.Add(mt_BoundedFolder);
            this.Controls.Add(m_pBoundedFolder);
            this.Controls.Add(m_pGetBoundedFolder);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_Ok);
        }
                                                                                
        #endregion


        #region Events Handling

        #region method m_pRootFolderType_SelectedIndexChanged

        private void m_pRootFolderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pRootFolderType.SelectedItem.ToString() == "Bounded Root Folder"){
                mt_BoundedUser.Visible = true;
                m_pBoundedUser.Visible = true;
                m_pGetBoundedUser.Visible = true;
                mt_BoundedFolder.Visible = true;
                m_pBoundedFolder.Visible = true;
                m_pGetBoundedFolder.Visible = true;
            }
            else if(m_pRootFolderType.SelectedItem.ToString() == "Users Shared Folder"){
                mt_BoundedUser.Visible = false;
                m_pBoundedUser.Visible = false;
                m_pGetBoundedUser.Visible = false;
                mt_BoundedFolder.Visible = false;
                m_pBoundedFolder.Visible = false;
                m_pGetBoundedFolder.Visible = false;
            }
        }

        #endregion

        #region method m_pGetBoundedUser_Click

        private void m_pGetBoundedUser_Click(object sender, EventArgs e)
        {
            wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pVirtualServer,false,false);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pBoundedUser.Text = frm.SelectedUserOrGroup;
                m_pBoundedFolder.Text = "";
            }
        }

        #endregion

        #region method m_pGetBoundedFolder_Click

        private void m_pGetBoundedFolder_Click(object sender, EventArgs e)
        {
            if(m_pBoundedUser.Text == ""){
                MessageBox.Show(this,"Please select bounded user !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            wfrm_se_UserFolder frm = new wfrm_se_UserFolder(m_pVirtualServer,m_pBoundedUser.Text);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pBoundedFolder.Text = frm.SelectedFolder;
            }
        }

        #endregion


        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
                
        #endregion

        #region method m_Ok_Click

        private void m_Ok_Click(object sender, EventArgs e)
        {
            SharedFolderRootType_enum rootType = (SharedFolderRootType_enum)((WComboBoxItem)m_pRootFolderType.SelectedItem).Tag;

            //--- Validate values ------------------------//
            if(rootType == SharedFolderRootType_enum.BoundedRootFolder && m_pBoundedUser.Text == ""){
                MessageBox.Show(this,"Please select bounded user !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if(rootType == SharedFolderRootType_enum.BoundedRootFolder && m_pBoundedFolder.Text == ""){
                MessageBox.Show(this,"Please select bounded folder !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //--------------------------------------------//


            // Add new 
            if(m_pRootFolder == null){
                m_pRootFolder = m_pVirtualServer.RootFolders.Add(
                    m_pEnabled.Checked,
                    m_pRootFolderName.Text,
                    m_pDescription.Text,
                    (SharedFolderRootType_enum)((WComboBoxItem)m_pRootFolderType.SelectedItem).Tag,
                    m_pBoundedUser.Text,
                    m_pBoundedFolder.Text
                );
            }
            // Update
            else{
                m_pRootFolder.Enabled = m_pEnabled.Checked;
                m_pRootFolder.Name = m_pRootFolderName.Text;
                m_pRootFolder.Description = m_pDescription.Text;
                m_pRootFolder.Type = (SharedFolderRootType_enum)((WComboBoxItem)m_pRootFolderType.SelectedItem).Tag;
                m_pRootFolder.BoundedUser = m_pBoundedUser.Text;
                m_pRootFolder.BoundedFolder = m_pBoundedFolder.Text;
                m_pRootFolder.Commit();
            }

            this.DialogResult = DialogResult.OK;
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets root id.
        /// </summary>
        public string RootID
        {
            get{ 
                if(m_pRootFolder != null){
                    return m_pRootFolder.ID;
                }
                else{
                    return ""; 
                }
            }
        }

        #endregion

    }
}
