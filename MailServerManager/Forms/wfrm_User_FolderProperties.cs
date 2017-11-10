using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;
using LumiSoft.Net.IMAP;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// User folder properties window.
    /// </summary>
    public class wfrm_User_FolderProperties : Form
    {
        //--- Common UI
        private TabControl m_pTab   = null;
        private Button     m_pClose = null;
        //--- Tabpage General UI
        private PictureBox m_pTab_General_Icon       = null;
        private TextBox    m_pTab_General_FolderName = null;
        private GroupBox   m_pTab_General_Separator1 = null;
        private Label      mt_Tab_General_Path       = null;
        private Label      m_pTab_General_Path       = null;
        private Label      mt_Tab_General_Size       = null;
        private Label      m_pTab_General_Size       = null;
        private Label      mt_Tab_General_Contains   = null;
        private Label      m_pTab_General_Contains   = null;
        private GroupBox   m_pTab_General_Separator2 = null;
        private Label      mt_Tab_General_Created    = null;
        private Label      m_pTab_General_Created    = null;
        private GroupBox   m_pTab_General_Separator3 = null;
        //--- Tabpage Sharing UI        
        private PictureBox  m_pTab_Sharing_Icon       = null;
        private Label       mt_Tab_Sharing_Info       = null;
        private GroupBox    m_pTab_Sharing_Separator1 = null;
        private RadioButton m_pTab_Sharing_DontShare  = null;
        private RadioButton m_pTab_Sharing_Share      = null;
        private TextBox     m_pTab_Sharing_ShareName  = null;
        //--- Tabpage Security UI
        private PictureBox m_pTab_Security_Icon                 = null;
        private Label      mt_Tab_Security_Info                 = null;
        private GroupBox   m_pTab_Security_Separator1           = null;
        private CheckBox   m_pTab_Security_InheritAcl           = null;
        private ToolStrip  mt_Tab_Security_UsersOrGroupsToolbar = null;
        private Label      mt_Tab_Security_UsersOrGroups        = null;
        private ListView   m_pTab_Security_UsersOrGroups        = null;
        private Label      mt_Tab_Security_Permissions          = null;
        private ListView   m_pTab_Security_Permissions          = null;

        private VirtualServer m_pVirtualServer = null;
        private UserFolder    m_pFolder        = null;
        private string        m_ShareName      = "";
        private bool          m_EvensLocked    = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="folder">Folder what properties to show.</param>
        public wfrm_User_FolderProperties(VirtualServer virtualServer,UserFolder folder)
        {
            m_pVirtualServer = virtualServer;
            m_pFolder        = folder;

            InitUI();

            LoadData();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,423);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Folder '' Properties";
            this.Icon = ResManager.GetIcon("folder.ico");
            // MONO won't support it !!
            //this.FormClosing += new FormClosingEventHandler(wfrm_User_FolderProperties_FormClosing);
            this.VisibleChanged += new EventHandler(wfrm_User_FolderProperties_VisibleChanged);

            #region Common UI

            //--- Common UI ----------------------
            m_pTab = new TabControl();
            m_pTab.Size = new Size(385,385);
            m_pTab.Location = new Point(5,5);
            m_pTab.TabPages.Add(new TabPage("General"));
            m_pTab.TabPages.Add(new TabPage("Sharing"));
            m_pTab.TabPages.Add(new TabPage("Security"));

            m_pClose = new Button();
            m_pClose.Size = new Size(70,20);
            m_pClose.Location = new Point(320,400);
            m_pClose.Text = "Close";
            m_pClose.Click += new EventHandler(m_pClose_Click);

            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pClose);
            //----------------------------------

            #endregion

            #region General UI

            //--- General UI ---------------------
            m_pTab_General_Icon = new PictureBox();
            m_pTab_General_Icon.Size = new Size(32,32);            
            m_pTab_General_Icon.Location = new Point(10,10);
            m_pTab_General_Icon.Image = ResManager.GetIcon("folder32.ico").ToBitmap();

            m_pTab_General_FolderName = new TextBox();
            m_pTab_General_FolderName.Size = new Size(270,20);
            m_pTab_General_FolderName.Location = new Point(100,17);
            m_pTab_General_FolderName.ReadOnly = true;

            m_pTab_General_Separator1 = new GroupBox();
            m_pTab_General_Separator1.Size = new Size(365,3);
            m_pTab_General_Separator1.Location = new Point(7,50);

            mt_Tab_General_Path = new Label();
            mt_Tab_General_Path.Size = new Size(95,20);
            mt_Tab_General_Path.Location = new Point(10,65);
            mt_Tab_General_Path.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_General_Path.Text = "Path:";

            m_pTab_General_Path = new Label();
            m_pTab_General_Path.Size = new Size(200,20);
            m_pTab_General_Path.Location = new Point(105,65);
            m_pTab_General_Path.TextAlign = ContentAlignment.MiddleLeft;

            mt_Tab_General_Size = new Label();
            mt_Tab_General_Size.Size = new Size(95,20);
            mt_Tab_General_Size.Location = new Point(10,90);
            mt_Tab_General_Size.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_General_Size.Text = "Size:";

            m_pTab_General_Size = new Label();
            m_pTab_General_Size.Size = new Size(200,20);
            m_pTab_General_Size.Location = new Point(105,90);
            m_pTab_General_Size.TextAlign = ContentAlignment.MiddleLeft;

            mt_Tab_General_Contains = new Label();
            mt_Tab_General_Contains.Size = new Size(95,20);
            mt_Tab_General_Contains.Location = new Point(10,115);
            mt_Tab_General_Contains.Text = "Contains:";

            m_pTab_General_Contains = new Label();
            m_pTab_General_Contains.Size = new Size(270,20);
            m_pTab_General_Contains.Location = new Point(105,115);
            m_pTab_General_Contains.TextAlign = ContentAlignment.MiddleLeft;

            m_pTab_General_Separator2 = new GroupBox();
            m_pTab_General_Separator2.Size = new Size(365,3);
            m_pTab_General_Separator2.Location = new Point(7,140);

            mt_Tab_General_Created = new Label();
            mt_Tab_General_Created.Size = new Size(95,20);
            mt_Tab_General_Created.Location = new Point(10,150);
            mt_Tab_General_Created.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_General_Created.Text = "Created:";

            m_pTab_General_Created = new Label();
            m_pTab_General_Created.Size = new Size(270,20);
            m_pTab_General_Created.Location = new Point(105,150);
            m_pTab_General_Created.TextAlign = ContentAlignment.MiddleLeft;

            m_pTab_General_Separator3 = new GroupBox();
            m_pTab_General_Separator3.Size = new Size(365,3);
            m_pTab_General_Separator3.Location = new Point(7,180);

            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Icon);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_FolderName);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Separator1);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Path);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Path);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Size);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Size);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Contains);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Contains);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Separator2);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Created);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Created);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Separator3);
            //---------------------------------------------------------

            #endregion

            #region Sharing UI

            //--- Security UI -----------------------------------------
            m_pTab_Sharing_Icon = new PictureBox();
            m_pTab_Sharing_Icon.Size = new Size(32,32);
            m_pTab_Sharing_Icon.Location = new Point(10,10);
            m_pTab_Sharing_Icon.Image = ResManager.GetIcon("share32.ico").ToBitmap();

            mt_Tab_Sharing_Info = new Label();
            mt_Tab_Sharing_Info.Size = new Size(200,32);
            mt_Tab_Sharing_Info.Location = new Point(50,10);
            mt_Tab_Sharing_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Sharing_Info.Text = "Specify sharing information.";

            m_pTab_Sharing_Separator1 = new GroupBox();
            m_pTab_Sharing_Separator1.Size = new Size(365,3);
            m_pTab_Sharing_Separator1.Location = new Point(7,50);

            m_pTab_Sharing_DontShare = new RadioButton();
            m_pTab_Sharing_DontShare.Size = new Size(200,20);
            m_pTab_Sharing_DontShare.Location = new Point(20,65);
            m_pTab_Sharing_DontShare.Text = "Do not share this folder";
            m_pTab_Sharing_DontShare.CheckedChanged += new EventHandler(m_pTab_Sharing_Share_CheckedChanged);

            m_pTab_Sharing_Share = new RadioButton();
            m_pTab_Sharing_Share.Size = new Size(200,20);
            m_pTab_Sharing_Share.Location = new Point(20,85);
            m_pTab_Sharing_Share.Text = "Share this folder";
            m_pTab_Sharing_Share.CheckedChanged += new EventHandler(m_pTab_Sharing_Share_CheckedChanged);

            m_pTab_Sharing_ShareName = new TextBox();
            m_pTab_Sharing_ShareName.Size = new Size(330,20);
            m_pTab_Sharing_ShareName.Location = new Point(40,110);

            m_pTab.TabPages[1].Controls.Add(m_pTab_Sharing_Icon);
            m_pTab.TabPages[1].Controls.Add(mt_Tab_Sharing_Info);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Sharing_Separator1);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Sharing_DontShare);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Sharing_Share);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Sharing_ShareName);
            //---------------------------------------------------------

            #endregion

            #region Security UI

            //--- Security UI -----------------------------------------
            m_pTab_Security_Icon = new PictureBox();
            m_pTab_Security_Icon.Size = new Size(32,32);
            m_pTab_Security_Icon.Location = new Point(10,10);
            m_pTab_Security_Icon.Image = ResManager.GetIcon("security32.ico").ToBitmap();

            mt_Tab_Security_Info = new Label();
            mt_Tab_Security_Info.Size = new Size(200,32);
            mt_Tab_Security_Info.Location = new Point(50,10);
            mt_Tab_Security_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Security_Info.Text = "Specify folder permissions.";

            m_pTab_Security_Separator1 = new GroupBox();
            m_pTab_Security_Separator1.Size = new Size(365,3);
            m_pTab_Security_Separator1.Location = new Point(7,50);

            m_pTab_Security_InheritAcl = new CheckBox();
            m_pTab_Security_InheritAcl.Size = new Size(300,20);
            m_pTab_Security_InheritAcl.Location = new Point(10,60);
            m_pTab_Security_InheritAcl.Text = "Inherit permissions from parent folder.";
            m_pTab_Security_InheritAcl.CheckStateChanged += new EventHandler(m_pTab_Security_InheritAcl_CheckStateChanged);

            mt_Tab_Security_UsersOrGroups = new Label();
            mt_Tab_Security_UsersOrGroups.Size = new Size(200,20);
            mt_Tab_Security_UsersOrGroups.Location = new Point(10,85);
            mt_Tab_Security_UsersOrGroups.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Security_UsersOrGroups.Text = "Group or user names:";

            mt_Tab_Security_UsersOrGroupsToolbar = new ToolStrip();
            mt_Tab_Security_UsersOrGroupsToolbar.Location = new Point(315,80);
            mt_Tab_Security_UsersOrGroupsToolbar.Dock = DockStyle.None;
            mt_Tab_Security_UsersOrGroupsToolbar.GripStyle = ToolStripGripStyle.Hidden;
            mt_Tab_Security_UsersOrGroupsToolbar.BackColor = this.BackColor;
            mt_Tab_Security_UsersOrGroupsToolbar.Renderer = new ToolBarRendererEx();
            ToolStripButton tab_Security_UsersOrGroupsToolbarAdd = new ToolStripButton(ResManager.GetIcon("add.ico").ToBitmap());
            tab_Security_UsersOrGroupsToolbarAdd.Tag = "add";
            mt_Tab_Security_UsersOrGroupsToolbar.Items.Add(tab_Security_UsersOrGroupsToolbarAdd);            
            ToolStripButton tab_Security_UsersOrGroupsToolbarDel = new ToolStripButton(ResManager.GetIcon("delete.ico").ToBitmap());
            tab_Security_UsersOrGroupsToolbarDel.Tag = "delete";
            mt_Tab_Security_UsersOrGroupsToolbar.Items.Add(tab_Security_UsersOrGroupsToolbarDel);            
            mt_Tab_Security_UsersOrGroupsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(mt_Tab_Security_UsersOrGroupsToolbar_ItemClicked);

            ImageList tab_Security_UsersOrGroups_imagelist = new ImageList();
            tab_Security_UsersOrGroups_imagelist.Images.Add(ResManager.GetIcon("user.ico"));
            tab_Security_UsersOrGroups_imagelist.Images.Add(ResManager.GetIcon("group.ico"));

            m_pTab_Security_UsersOrGroups = new ListView();
            m_pTab_Security_UsersOrGroups.Size = new Size(355,125);
            m_pTab_Security_UsersOrGroups.Location = new Point(10,105);
            m_pTab_Security_UsersOrGroups.View = View.List;
            m_pTab_Security_UsersOrGroups.HideSelection = false;
            m_pTab_Security_UsersOrGroups.FullRowSelect = true;
            m_pTab_Security_UsersOrGroups.SmallImageList = tab_Security_UsersOrGroups_imagelist;
            m_pTab_Security_UsersOrGroups.Columns.Add("",300,HorizontalAlignment.Left);
            m_pTab_Security_UsersOrGroups.SelectedIndexChanged += new EventHandler(m_pTab_Security_UsersOrGroups_SelectedIndexChanged);

            mt_Tab_Security_Permissions = new Label();
            mt_Tab_Security_Permissions.Size = new Size(195,20);
            mt_Tab_Security_Permissions.Location = new Point(10,235);
            mt_Tab_Security_Permissions.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Security_Permissions.Text = "Permissions:";

            m_pTab_Security_Permissions = new ListView();
            m_pTab_Security_Permissions.Size = new Size(355,95);
            m_pTab_Security_Permissions.Location = new Point(10,255);
            m_pTab_Security_Permissions.View = View.List;
            m_pTab_Security_Permissions.HideSelection = false;
            m_pTab_Security_Permissions.FullRowSelect = true;
            m_pTab_Security_Permissions.CheckBoxes = true;
            m_pTab_Security_Permissions.Columns.Add("",145,HorizontalAlignment.Left);
            m_pTab_Security_Permissions.Items.Add("Administer");
            m_pTab_Security_Permissions.Items.Add("List Folders");
            m_pTab_Security_Permissions.Items.Add("Create Folder");
            m_pTab_Security_Permissions.Items.Add("Read");
            m_pTab_Security_Permissions.Items.Add("Write");
            m_pTab_Security_Permissions.Items.Add("Delete Messages");
            m_pTab_Security_Permissions.Items.Add("Store Flags");
            //m_pTab_Security_Permissions.ItemChecked += new ItemCheckedEventHandler(m_pTab_Security_Permissions_ItemChecked);
            m_pTab_Security_Permissions.ItemCheck += new ItemCheckEventHandler(m_pTab_Security_Permissions_ItemCheck);
                                   
            m_pTab.TabPages[2].Controls.Add(m_pTab_Security_Icon);
            m_pTab.TabPages[2].Controls.Add(mt_Tab_Security_Info);
            m_pTab.TabPages[2].Controls.Add(m_pTab_Security_Separator1);
            m_pTab.TabPages[2].Controls.Add(m_pTab_Security_InheritAcl);
            m_pTab.TabPages[2].Controls.Add(mt_Tab_Security_UsersOrGroupsToolbar);
            m_pTab.TabPages[2].Controls.Add(mt_Tab_Security_UsersOrGroups);
            m_pTab.TabPages[2].Controls.Add(m_pTab_Security_UsersOrGroups);
            m_pTab.TabPages[2].Controls.Add(mt_Tab_Security_Permissions);
            m_pTab.TabPages[2].Controls.Add(m_pTab_Security_Permissions);
            //---------------------------------------------------------

            #endregion

        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_pTab_Security_Permissions_ItemCheck(object sender,ItemCheckEventArgs e)
        {
            if(m_EvensLocked){
                return;
            }

            // See if permissions has changed, if so update them.
            if(m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0){
                //--- Construct ACl flags ---------------//
                IMAP_ACL_Flags acl = IMAP_ACL_Flags.None;
                if((e.Index == 0 && e.NewValue == CheckState.Checked) || (e.Index != 0 && m_pTab_Security_Permissions.Items[0].Checked)){
                    acl |= IMAP_ACL_Flags.a;
                }
                if((e.Index == 1 && e.NewValue == CheckState.Checked) || (e.Index != 1 && m_pTab_Security_Permissions.Items[1].Checked)){
                    acl |= IMAP_ACL_Flags.l;
                }
                if((e.Index == 2 && e.NewValue == CheckState.Checked) || (e.Index != 2 && m_pTab_Security_Permissions.Items[2].Checked)){
                    acl |= IMAP_ACL_Flags.c;
                }
                if((e.Index == 3 && e.NewValue == CheckState.Checked) || (e.Index != 3 && m_pTab_Security_Permissions.Items[3].Checked)){
                    acl |= IMAP_ACL_Flags.r;
                }
                if((e.Index == 4 && e.NewValue == CheckState.Checked) || (e.Index != 4 && m_pTab_Security_Permissions.Items[4].Checked)){
                    acl |= IMAP_ACL_Flags.i | IMAP_ACL_Flags.p;
                }
                if((e.Index == 5 && e.NewValue == CheckState.Checked) || (e.Index != 5 && m_pTab_Security_Permissions.Items[5].Checked)){
                    acl |= IMAP_ACL_Flags.d;
                }
                if((e.Index == 6 && e.NewValue == CheckState.Checked) || (e.Index != 6 && m_pTab_Security_Permissions.Items[6].Checked)){
                    acl |= IMAP_ACL_Flags.w | IMAP_ACL_Flags.s;
                }
                //--------------------------------------//

                UserFolderAcl aclEntry = (UserFolderAcl)m_pTab_Security_UsersOrGroups.SelectedItems[0].Tag;
                if(aclEntry.Permissions != acl){
                    aclEntry.Permissions = acl;
                }
            }
        }
                                                                                                                                                                                       
        #endregion


        #region Events Handling
                
        #region method wfrm_User_FolderProperties_VisibleChanged

        private void wfrm_User_FolderProperties_VisibleChanged(object sender,EventArgs e)
        {
            if(this.Visible){
                return;
            }

            try{
                // Apply changes
                foreach(UserFolderAcl acl in m_pFolder.ACL){
                    acl.Commit();
                }

                //--- See if sharing data changed ------------------------------------------------------------
                // Sharing removed
                if(m_pTab_Sharing_DontShare.Checked && m_ShareName != ""){
                    SharedRootFolder root = m_pVirtualServer.RootFolders.GetRootFolderByName(m_ShareName);
                    root.Owner.Remove(root);
                }
                else if(m_pTab_Sharing_Share.Checked && m_ShareName != m_pTab_Sharing_ShareName.Text){
                    // Update shared root folder info
                    if(m_pVirtualServer.RootFolders.Contains(m_ShareName)){
                        SharedRootFolder root = m_pVirtualServer.RootFolders.GetRootFolderByName(m_ShareName);
                        root.Enabled = true;
                        root.Name = m_pTab_Sharing_ShareName.Text;
                        root.Commit();
                    }
                    // Add new shared root folder
                    else{
                        m_pVirtualServer.RootFolders.Add(
                            true,
                            m_pTab_Sharing_ShareName.Text,
                            "",
                            SharedFolderRootType_enum.BoundedRootFolder,
                            m_pFolder.User.UserName,
                            m_pFolder.FolderFullPath
                        );
                    }
                }
                //-------------------------------------------------------------------------------------------
            }
            catch(Exception x){
                wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
			    frm.ShowDialog(null);
            }
        }

        #endregion

        #region method wfrm_User_FolderProperties_FormClosing

        private void wfrm_User_FolderProperties_FormClosing(object sender, FormClosingEventArgs e)
        {
            try{
                // See if permissions has chanhed, if so update them,
                if(m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0){
                    //--- Construct ACl flags ---------------//
                    IMAP_ACL_Flags acl = IMAP_ACL_Flags.None;
                    if(m_pTab_Security_Permissions.Items[0].Checked){
                        acl |= IMAP_ACL_Flags.a;
                    }
                    if(m_pTab_Security_Permissions.Items[1].Checked){
                        acl |= IMAP_ACL_Flags.l;
                    }
                    if(m_pTab_Security_Permissions.Items[2].Checked){
                        acl |= IMAP_ACL_Flags.c;
                    }
                    if(m_pTab_Security_Permissions.Items[3].Checked){
                        acl |= IMAP_ACL_Flags.r;
                    }
                    if(m_pTab_Security_Permissions.Items[4].Checked){
                        acl |= IMAP_ACL_Flags.i | IMAP_ACL_Flags.p;
                    }
                    if(m_pTab_Security_Permissions.Items[5].Checked){
                        acl |= IMAP_ACL_Flags.d;
                    }
                    if(m_pTab_Security_Permissions.Items[6].Checked){
                        acl |= IMAP_ACL_Flags.w | IMAP_ACL_Flags.s;
                    }
                    //--------------------------------------//

                    UserFolderAcl aclEntry = (UserFolderAcl)m_pTab_Security_UsersOrGroups.SelectedItems[0].Tag;
                    if(aclEntry.Permissions != acl){
                        aclEntry.Permissions = acl;
                        aclEntry.Commit();
                    }
                }

                //--- See if sharing data changed ------------------------------------------------------------
                // Sharing removed
                if(m_pTab_Sharing_DontShare.Checked && m_ShareName != ""){
                    SharedRootFolder root = m_pVirtualServer.RootFolders.GetRootFolderByName(m_ShareName);
                    root.Owner.Remove(root);
                }
                else if(m_pTab_Sharing_Share.Checked && m_ShareName != m_pTab_Sharing_ShareName.Text){
                    // Update shared root folder info
                    if(m_pVirtualServer.RootFolders.Contains(m_ShareName)){
                        SharedRootFolder root = m_pVirtualServer.RootFolders.GetRootFolderByName(m_ShareName);
                        root.Enabled = true;
                        root.Name = m_pTab_Sharing_ShareName.Text;
                        root.Commit();
                    }
                    // Add new shared root folder
                    else{
                        m_pVirtualServer.RootFolders.Add(
                            true,
                            m_pTab_Sharing_ShareName.Text,
                            "",
                            SharedFolderRootType_enum.BoundedRootFolder,
                            m_pFolder.User.UserName,
                            m_pFolder.FolderFullPath
                        );
                    }
                }
                //-------------------------------------------------------------------------------------------
            }
            catch(Exception x){
                wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
			    frm.ShowDialog(null);

                if(MessageBox.Show(this,"Do you want to reconfigure ?","Confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region method m_pClose_Click

        private void m_pClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        
        #region method m_pTab_Sharing_Share_CheckedChanged

        private void m_pTab_Sharing_Share_CheckedChanged(object sender,EventArgs e)
        {
            if(m_pTab_Sharing_Share.Checked){
                m_pTab_Sharing_ShareName.Enabled = true;
            }
            else{
                m_pTab_Sharing_ShareName.Enabled = false;
            }
        }

        #endregion


        #region method m_pTab_Security_InheritAcl_CheckStateChanged

        private void m_pTab_Security_InheritAcl_CheckStateChanged(object sender, EventArgs e)
        {
            // Events are locked, just do nothing
            if(m_EvensLocked){
                return;
            }
            m_EvensLocked = true;

            if(m_pTab_Security_InheritAcl.Checked){
                string text = "Do you want to inherit all permissions from parent folder and loose current permissions from current folder ?";
                if(MessageBox.Show(this,text,"Warning:",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes){
                    mt_Tab_Security_UsersOrGroupsToolbar.Enabled = false;
                    m_pTab_Security_UsersOrGroups.Enabled = false;
                    m_pTab_Security_Permissions.Enabled = false;
                                       
                    foreach(ListViewItem it in m_pTab_Security_UsersOrGroups.Items){
                        UserFolderAcl acl = (UserFolderAcl)it.Tag;
                        acl.Owner.Remove(acl);
                    }
                                        
                    LoadACL();
                }
                else{
                    // Undo check (make checkbox unchecked)
                    m_pTab_Security_InheritAcl.Checked = false;
                    mt_Tab_Security_UsersOrGroupsToolbar.Enabled = true;
                    m_pTab_Security_UsersOrGroups.Enabled = true;
                    m_pTab_Security_Permissions.Enabled = true;                    
                }
            }
            else{
                mt_Tab_Security_UsersOrGroupsToolbar.Enabled = true;
                m_pTab_Security_UsersOrGroups.Enabled = true;
                m_pTab_Security_Permissions.Enabled = true;

                m_pTab_Security_UsersOrGroups.Items.Clear();
                m_pTab_Security_UsersOrGroups_SelectedIndexChanged(null,null);
            }

            m_EvensLocked = false;            
        }

        #endregion

        #region method mt_Tab_Security_UsersOrGroupsToolbar_ItemClicked

        private void mt_Tab_Security_UsersOrGroupsToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag.ToString() == "add"){
                List<string> excludeList = new List<string>();
                foreach(ListViewItem item in m_pTab_Security_UsersOrGroups.Items){
                    excludeList.Add(item.Text.ToLower());
                }
                wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pVirtualServer,false,false,excludeList);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    ListViewItem item = new ListViewItem(frm.SelectedUserOrGroup);
                    item.Tag = m_pFolder.ACL.Add(frm.SelectedUserOrGroup,LumiSoft.Net.IMAP.IMAP_ACL_Flags.None);
                    if(frm.IsGroup){
                        item.ImageIndex = 1;
                    }
                    else{
                        item.ImageIndex =  0;
                    }
                    item.Selected = true;
                    m_pTab_Security_UsersOrGroups.Items.Add(item);                    
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                ListViewItem item = m_pTab_Security_UsersOrGroups.SelectedItems[0];
                if(MessageBox.Show(this,"Are you sure you want to remove '" + item.Text + "' permissions on current folder ?","Confirm Delete:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                    UserFolderAcl acl = (UserFolderAcl)item.Tag;
                    acl.Owner.Remove(acl);
                    item.Remove();
                }
            }
        }

        #endregion

        #region method m_pTab_Security_UsersOrGroups_SelectedIndexChanged

        private void m_pTab_Security_UsersOrGroups_SelectedIndexChanged(object sender,EventArgs e)
        {
            m_EvensLocked = true;

            if(m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0){
                mt_Tab_Security_UsersOrGroupsToolbar.Items[1].Enabled = true;
                if(!m_pTab_Security_InheritAcl.Checked){
                    m_pTab_Security_Permissions.Enabled = true;
                }

                ListViewItem item = m_pTab_Security_UsersOrGroups.SelectedItems[0];
                UserFolderAcl acl = (UserFolderAcl)item.Tag;

                if((acl.Permissions & IMAP_ACL_Flags.l) != 0){
                    m_pTab_Security_Permissions.Items[1].Checked = true;                
                }
                if((acl.Permissions & IMAP_ACL_Flags.r) != 0){
                    m_pTab_Security_Permissions.Items[3].Checked = true;                
                }
                if((acl.Permissions & IMAP_ACL_Flags.s) != 0){
                    m_pTab_Security_Permissions.Items[6].Checked = true;                
                }
                if((acl.Permissions & IMAP_ACL_Flags.w) != 0){
                    m_pTab_Security_Permissions.Items[6].Checked = true;
                }
                if((acl.Permissions & IMAP_ACL_Flags.i) != 0){
                    m_pTab_Security_Permissions.Items[4].Checked = true;
                }
                if((acl.Permissions & IMAP_ACL_Flags.p) != 0){
                    m_pTab_Security_Permissions.Items[4].Checked = true;                
                }
                if((acl.Permissions & IMAP_ACL_Flags.c) != 0){
                    m_pTab_Security_Permissions.Items[2].Checked = true;
                }
                if((acl.Permissions & IMAP_ACL_Flags.d) != 0){
                    m_pTab_Security_Permissions.Items[5].Checked = true;
                }
                if((acl.Permissions & IMAP_ACL_Flags.a) != 0){
                    m_pTab_Security_Permissions.Items[0].Checked = true;
                }
            }
            else{
                mt_Tab_Security_UsersOrGroupsToolbar.Items[1].Enabled = false;
                m_pTab_Security_Permissions.Enabled = false;

                foreach(ListViewItem item in m_pTab_Security_Permissions.Items){
                    item.Checked = false;
                }
            }

            m_EvensLocked = false;
        }

        #endregion

        #region method m_pTab_Security_Permissions_ItemChecked

        private void m_pTab_Security_Permissions_ItemChecked(object sender,ItemCheckedEventArgs e)
        {     
            if(m_EvensLocked){
                return;
            }

            // See if permissions has changed, if so update them.
            if(m_pTab_Security_UsersOrGroups.SelectedItems.Count > 0){
                //--- Construct ACl flags ---------------//
                IMAP_ACL_Flags acl = IMAP_ACL_Flags.None;
                if(m_pTab_Security_Permissions.Items[0].Checked){
                    acl |= IMAP_ACL_Flags.a;
                }
                if(m_pTab_Security_Permissions.Items[1].Checked){
                    acl |= IMAP_ACL_Flags.l;
                }
                if(m_pTab_Security_Permissions.Items[2].Checked){
                    acl |= IMAP_ACL_Flags.c;
                }
                if(m_pTab_Security_Permissions.Items[3].Checked){
                    acl |= IMAP_ACL_Flags.r;
                }
                if(m_pTab_Security_Permissions.Items[4].Checked){
                    acl |= IMAP_ACL_Flags.i | IMAP_ACL_Flags.p;
                }
                if(m_pTab_Security_Permissions.Items[5].Checked){
                    acl |= IMAP_ACL_Flags.d;
                }
                if(m_pTab_Security_Permissions.Items[6].Checked){
                    acl |= IMAP_ACL_Flags.w | IMAP_ACL_Flags.s;
                }
                //--------------------------------------//

                UserFolderAcl aclEntry = (UserFolderAcl)m_pTab_Security_UsersOrGroups.SelectedItems[0].Tag;
                if(aclEntry.Permissions != acl){
                    aclEntry.Permissions = acl;
                }
            }
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads folder data to UI.
        /// </summary>
        private void LoadData()
        {
            //--- General UI -----------------------------------------------------------------------
            this.Text                      = "Folder '" + m_pFolder.FolderName + "' Properties";
            m_pTab_General_FolderName.Text = m_pFolder.FolderName;
            m_pTab_General_Path.Text       = m_pFolder.FolderFullPath;
            m_pTab_General_Size.Text       = ((decimal)(m_pFolder.SizeUsed / (decimal)1000000)).ToString("f2") + " MB";
            m_pTab_General_Contains.Text   = m_pFolder.MessagesCount + " Messages, " + m_pFolder.ChildFolders.Count + " Folders";
            m_pTab_General_Created.Text    = m_pFolder.CreationTime.ToLongDateString() + " " + m_pFolder.CreationTime.ToLongTimeString();             
            //--------------------------------------------------------------------------------------

            //--- Security UI ----------------------------------------------------------------------
            m_pTab_Sharing_DontShare.Checked = true;
            // See if folder shared
            foreach(SharedRootFolder root in m_pVirtualServer.RootFolders){
                if(root.Type == SharedFolderRootType_enum.BoundedRootFolder && root.BoundedUser == m_pFolder.User.UserName && root.BoundedFolder == m_pFolder.FolderFullPath){
                    m_ShareName = root.Name;
                    m_pTab_Sharing_ShareName.Text = root.Name;
                    m_pTab_Sharing_Share.Checked = true;
                    break;
                }
            }
            //--------------------------------------------------------------------------------------

            //--- Security UI ----------------------------------------------------------------------            
            LoadACL();
            //--------------------------------------------------------------------------------------
        }

        #endregion

        #region method LoadACL

        /// <summary>
        /// Load ACL to UI.
        /// </summary>
        private void LoadACL()
        {           
            m_EvensLocked = true;
            m_pTab_Security_UsersOrGroups.Items.Clear();

            // See if ACL is set to this folder, if not show inhereted ACL
            bool aclSetToFolder = false;            
            if(m_pFolder.ACL.Count > 0){
                foreach(UserFolderAcl acl in m_pFolder.ACL){
                    ListViewItem it = new ListViewItem(acl.UserOrGroup);
                    it.SubItems.Add(IMAP_Utils.ACL_to_String(acl.Permissions));
                    if(!m_pVirtualServer.Groups.Contains(acl.UserOrGroup)){
                        it.ImageIndex = 0;
                    }
                    else{
                        it.ImageIndex = 1;
                    }
                    it.Tag = acl;
                    m_pTab_Security_UsersOrGroups.Items.Add(it);
                }

                 aclSetToFolder = true;
            }
            else{
                UserFolder folder = m_pFolder;
                // Try to inherit ACL from parent folder(s)
                // Move right to left in path.
                while(folder.Parent != null){
                    // Move 1 level to right in path
                    folder = folder.Parent;

                    if(folder.ACL.Count > 0){
                        foreach(UserFolderAcl acl in folder.ACL){
                            ListViewItem it = new ListViewItem(acl.UserOrGroup);
                            it.SubItems.Add(IMAP_Utils.ACL_to_String(acl.Permissions));                    
                            if(!m_pVirtualServer.Groups.Contains(acl.UserOrGroup)){
                                it.ImageIndex = 0;
                            }
                            else{
                                it.ImageIndex = 1;
                            }
                            it.Tag = acl;
                            m_pTab_Security_UsersOrGroups.Items.Add(it);
                        }

                        // We inhereted all permission, don't look other parent folders anymore
                        break;
                    }
                }
            }
                        
            // ACL isn't set to this folder, disable users ListView
            if(!aclSetToFolder){
                m_pTab_Security_InheritAcl.Checked = true;
                mt_Tab_Security_UsersOrGroupsToolbar.Enabled = false;
                //m_pTab_Security_UsersOrGroups.Enabled = false;
                m_pTab_Security_Permissions.Enabled = false;
            }
            else{
                m_pTab_Security_InheritAcl.Checked = false;
                mt_Tab_Security_UsersOrGroupsToolbar.Enabled = true;
                //m_pTab_Security_UsersOrGroups.Enabled = true;
                m_pTab_Security_Permissions.Enabled = true;

                m_pTab_Security_UsersOrGroups_SelectedIndexChanged(null,null);
            }

            m_EvensLocked = false;
        }

        #endregion

    }
}
