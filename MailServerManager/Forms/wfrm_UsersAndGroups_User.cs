using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Users and Groups User settings window.
    /// </summary>
    public class wfrm_UsersAndGroups_User : Form
    {
        //--- Common UI ----------------------------------
        private TabControl m_pTab                   = null;
        private TabPage    m_pTabPage_General       = null;
        private TabPage    m_pTabPage_Addressing    = null;
        private TabPage    m_pTabPage_Rules         = null;
        private TabPage    m_pTabPage_RemoteServers = null;
        private TabPage    m_pTabPage_Permissions   = null;
        private TabPage    m_pTabPage_Folders       = null;
        private Button     m_Cancel                 = null;
        private Button     m_pOk                    = null;
        //--- Tabpage General UI ------------------------------------
        private PictureBox    m_pTab_General_Icon             = null;
        private Label         mt_Tab_General_Info             = null;
        private GroupBox      m_pTab_General_Separator1       = null;
        private CheckBox      m_pGeneral_Enabled              = null;
        private Label         mt_General_FullName             = null;
        private TextBox       m_pGeneral_FullName             = null;        
        private Label         mt_General_Description          = null;
        private TextBox       m_pGeneral_Description          = null;
        private Label         mt_General_LoginName            = null;
        private TextBox       m_pGeneral_LoginName            = null;
        private Label         mt_General_Password             = null;
        private TextBox       m_pGeneral_Password             = null;
        private Button        m_pGeneral_GeneratePwd          = null;
        private Label         mt_General_MaxMailboxSize       = null;
        private NumericUpDown m_pGeneral_MaxMailboxSize       = null;
        private Label         mt_General_MaxMailboxMB         = null;
        private GroupBox      m_pTab_General_Separator2       = null;
        private Label         mt_Tab_General_MailboxSize      = null;
        private Label         m_pTab_General_MailboxSize      = null;
        private ProgressBar   m_pGeneral_MailboxSizeIndicator = null;
        private Label         mt_Tab_General_Created          = null;
        private Label         m_pTab_General_Created          = null;
        private Label         mt_Tab_General_LastLogin        = null;
        private Label         m_pTab_General_LastLogin        = null;
        private Button        m_pTab_General_Create           = null;
        //--- Tabpage Addressing UI ---------------------------------        
        private PictureBox m_pTab_Addressing_Icon       = null;
        private Label      mt_Tab_Addressing_Info       = null;
        private GroupBox   m_pTab_Addressing_Separator1 = null;
        private TextBox    m_pTab_Addressing_LocalPart  = null;
        private Label      mt_Tab_Addressing_At         = null;
        private ComboBox   m_pTab_Addressing_Domain     = null;
        private ToolStrip  m_pTab_Addressing_Toolbar    = null;
        private ListView   m_pTab_Addressing_Addresses  = null;
        //--- Tabpage Rules UI --------------------------------------
        private PictureBox m_pTab_Rules_Icon       = null;
        private Label      mt_Tab_Rules_Info       = null;
        private GroupBox   m_pTab_Rules_Separator1 = null;
        private ToolStrip  m_pTab_Rules_Toolbar    = null;
        private ListView   m_pRules_Rules          = null; 
        //--- Tabpage Remote Servers UI --------------------------
        private PictureBox m_pTab_RemoteServers_Icon       = null;
        private Label      mt_Tab_RemoteServers_Info       = null;
        private GroupBox   m_pTab_RemoteServers_Separator1 = null;
        private ToolStrip  m_pTab_RemoteServers_Toolbar    = null;
        private ListView   m_pRemoteServers_Servers        = null;
        //--- Tabpage Permissions UI ---------------------------
        private PictureBox m_pTab_Permissions_Icon           = null;
        private Label      mt_Tab_Permissions_Info           = null;
        private GroupBox   m_pTab_Permissions_Separator1     = null;
        private CheckBox   m_pPermissions_AllowPop3          = null;
        private CheckBox   m_pPermissions_AllowImap          = null;
        private CheckBox   m_pPermissions_AllowRelay         = null;
        private CheckBox   m_pPermissions_AllowSIP           = null;
        private Label      mt_Permissions_SipGwAccess        = null;
        private ToolStrip  m_pPermissions_SipGwAccessToolbar = null;
        private ListView   m_pPermissions_SipGwAccess        = null;
        //private ListView
        //--- Tabpage Folders UI ------------------------------
        private PictureBox m_pTab_Folders_Icon       = null;
        private Label      mt_Tab_Folders_Info       = null;
        private GroupBox   m_pTab_Folders_Separator1 = null;
        private ToolStrip  m_pTab_Folders_Toolbar    = null;
        private TreeView   m_pTab_Folders_Folders    = null;
        //-----------------------------------------------------

        private VirtualServer m_pVirtualServer = null;
        private User          m_pUser          = null;

        /// <summary>
        /// Add new user constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_UsersAndGroups_User(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadDomains();

            m_pTab.TabPages.Remove(m_pTabPage_Addressing);
            m_pTab.TabPages.Remove(m_pTabPage_Rules);
            m_pTab.TabPages.Remove(m_pTabPage_RemoteServers);
            m_pTab.TabPages.Remove(m_pTabPage_Permissions);
            m_pTab.TabPages.Remove(m_pTabPage_Folders);
            m_pTab_General_Create.Visible = true;
        }

        /// <summary>
        /// Edit existing user settings constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="user">User to update.</param>
        public wfrm_UsersAndGroups_User(VirtualServer virtualServer,User user)
        {
            m_pVirtualServer = virtualServer;
            m_pUser          = user;

            InitUI();
       
            LoadDomains();
            LoadSettings();
            LoadRules("");
            LoadRemoteServers("");
            LoadFolders("");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(492,373);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Add/Edit user settings";
            this.Icon = ResManager.GetIcon("user.ico");
            
            #region Common UI

            //--- Common UI -------------------------------------------------------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(493,335);
            m_pTab.Location = new Point(0,5);
            m_pTab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab.TabPages.Add(new TabPage("General"));
            m_pTab.TabPages.Add(new TabPage("Addressing"));
            m_pTab.TabPages.Add(new TabPage("Rules"));
            m_pTab.TabPages.Add(new TabPage("Remote Servers"));
            m_pTab.TabPages.Add(new TabPage("Permissions"));
            m_pTab.TabPages.Add(new TabPage("Folders"));
            m_pTab.TabPages[1].Size = new Size(487,311);
            m_pTab.TabPages[2].Size = new Size(487,311);
            m_pTabPage_General = m_pTab.TabPages[0];
            m_pTabPage_Addressing = m_pTab.TabPages[1];
            m_pTabPage_Rules = m_pTab.TabPages[2];
            m_pTabPage_RemoteServers = m_pTab.TabPages[3];
            m_pTabPage_Permissions = m_pTab.TabPages[4];
            m_pTabPage_Folders = m_pTab.TabPages[5];
            
            m_Cancel = new Button();
            m_Cancel.Size = new Size(70,20);
            m_Cancel.Location = new Point(340,350);
            m_Cancel.Text = "Cancel";
            m_Cancel.Click += new EventHandler(m_Cancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(415,350);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pTab);
            this.Controls.Add(m_Cancel);
            this.Controls.Add(m_pOk);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region General UI

            //--- Tabpage General UI ----------------------------------------------------------------------//
            m_pTab_General_Icon = new PictureBox();
            m_pTab_General_Icon.Size = new Size(32,32);
            m_pTab_General_Icon.Location = new Point(10,10);
            m_pTab_General_Icon.Image = ResManager.GetIcon("userinfo.ico").ToBitmap();

            mt_Tab_General_Info = new Label();
            mt_Tab_General_Info.Size = new Size(200,32);
            mt_Tab_General_Info.Location = new Point(50,10);
            mt_Tab_General_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_General_Info.Text = "Specify user info.";

            m_pTab_General_Separator1 = new GroupBox();
            m_pTab_General_Separator1.Size = new Size(475,3);
            m_pTab_General_Separator1.Location = new Point(7,50);

            m_pGeneral_Enabled = new CheckBox();            
            m_pGeneral_Enabled.Size = new Size(70,20);
            m_pGeneral_Enabled.Location = new Point(110,60);
            m_pGeneral_Enabled.Text = "Enabled";
            m_pGeneral_Enabled.Checked = true;

            mt_General_FullName = new Label();
            mt_General_FullName.Size = new Size(100,20);
            mt_General_FullName.Location = new Point(5,85);
            mt_General_FullName.TextAlign = ContentAlignment.MiddleRight;
            mt_General_FullName.Text = "Full Name:";

            m_pGeneral_FullName = new TextBox();
            m_pGeneral_FullName.Size = new Size(360,20);
            m_pGeneral_FullName.Location = new Point(110,85);

            mt_General_Description = new Label();
            mt_General_Description.Size = new Size(100,20);
            mt_General_Description.Location = new Point(5,110);
            mt_General_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_General_Description.Text = "Description:";

            m_pGeneral_Description = new TextBox();
            m_pGeneral_Description.Size = new Size(360,20);
            m_pGeneral_Description.Location = new Point(110,110);

            mt_General_LoginName = new Label();
            mt_General_LoginName.Size = new Size(100,20);
            mt_General_LoginName.Location = new Point(5,135);
            mt_General_LoginName.TextAlign = ContentAlignment.MiddleRight;
            mt_General_LoginName.Text = "Login Name:";

            m_pGeneral_LoginName = new TextBox();
            m_pGeneral_LoginName.Size = new Size(180,20);
            m_pGeneral_LoginName.Location = new Point(110,135);

            mt_General_Password = new Label();
            mt_General_Password.Size = new Size(100,20);
            mt_General_Password.Location = new Point(5,160);
            mt_General_Password.TextAlign = ContentAlignment.MiddleRight;
            mt_General_Password.Text = "Password:";

            m_pGeneral_Password = new TextBox();
            m_pGeneral_Password.Size = new Size(180,20);
            m_pGeneral_Password.Location = new Point(110,160);

            m_pGeneral_GeneratePwd = new Button();
            m_pGeneral_GeneratePwd.Size = new Size(170,20);
            m_pGeneral_GeneratePwd.Location = new Point(300,160);
            m_pGeneral_GeneratePwd.Text = "Generate Password";
            m_pGeneral_GeneratePwd.Click += new EventHandler(m_pGeneral_GeneratePwd_Click);

            mt_General_MaxMailboxSize = new Label();
            mt_General_MaxMailboxSize.Size = new Size(100,20);
            mt_General_MaxMailboxSize.Location = new Point(5,185);
            mt_General_MaxMailboxSize.TextAlign = ContentAlignment.MiddleRight;
            mt_General_MaxMailboxSize.Text = "Max mailbox size:";

            m_pGeneral_MaxMailboxSize = new NumericUpDown();
            m_pGeneral_MaxMailboxSize.Size = new Size(70,20);
            m_pGeneral_MaxMailboxSize.Location = new Point(110,185);
            m_pGeneral_MaxMailboxSize.Minimum = 0;
            m_pGeneral_MaxMailboxSize.Maximum = 999999;
            m_pGeneral_MaxMailboxSize.Value = 20;

            mt_General_MaxMailboxMB = new Label();
            mt_General_MaxMailboxMB.Size = new Size(150,20);
            mt_General_MaxMailboxMB.Location = new Point(180,185);
            mt_General_MaxMailboxMB.TextAlign = ContentAlignment.MiddleLeft;
            mt_General_MaxMailboxMB.Text = "MB (0 for unlimited)";
                        
            m_pTab_General_Separator2 = new GroupBox();
            m_pTab_General_Separator2.Size = new Size(475,3);
            m_pTab_General_Separator2.Location = new Point(7,215);

            mt_Tab_General_MailboxSize = new Label();
            mt_Tab_General_MailboxSize.Size = new Size(100,20);
            mt_Tab_General_MailboxSize.Location = new Point(5,225);
            mt_Tab_General_MailboxSize.TextAlign = ContentAlignment.MiddleRight;
            mt_Tab_General_MailboxSize.Text = "Mailbox size:";

            m_pTab_General_MailboxSize = new Label();
            m_pTab_General_MailboxSize.Size = new Size(170,20);
            m_pTab_General_MailboxSize.Location = new Point(110,225);
            m_pTab_General_MailboxSize.TextAlign = ContentAlignment.MiddleLeft;

            m_pGeneral_MailboxSizeIndicator = new ProgressBar();
            m_pGeneral_MailboxSizeIndicator.Size = new Size(170,20);
            m_pGeneral_MailboxSizeIndicator.Location = new Point(300,225);
            m_pGeneral_MailboxSizeIndicator.Style = ProgressBarStyle.Continuous;

            mt_Tab_General_Created = new Label();
            mt_Tab_General_Created.Size = new Size(100,20);
            mt_Tab_General_Created.Location = new Point(5,250);
            mt_Tab_General_Created.TextAlign = ContentAlignment.MiddleRight;
            mt_Tab_General_Created.Text = "Created:";

            m_pTab_General_Created = new Label();
            m_pTab_General_Created.Size = new Size(250,20);
            m_pTab_General_Created.Location = new Point(110,250);
            m_pTab_General_Created.TextAlign = ContentAlignment.MiddleLeft;

            mt_Tab_General_LastLogin = new Label();
            mt_Tab_General_LastLogin.Size = new Size(100,20);
            mt_Tab_General_LastLogin.Location = new Point(5,275);
            mt_Tab_General_LastLogin.TextAlign = ContentAlignment.MiddleRight;
            mt_Tab_General_LastLogin.Text = "Last login:";

            m_pTab_General_LastLogin = new Label();
            m_pTab_General_LastLogin.Size = new Size(250,20);
            m_pTab_General_LastLogin.Location = new Point(110,275);
            m_pTab_General_LastLogin.TextAlign = ContentAlignment.MiddleLeft;

            m_pTab_General_Create = new Button();
            m_pTab_General_Create.Size = new Size(70,20);
            m_pTab_General_Create.Location = new Point(410,280);
            m_pTab_General_Create.Text = "Create";
            m_pTab_General_Create.Visible = false;
            m_pTab_General_Create.Click += new EventHandler(m_pTab_General_Create_Click);
                                    
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Icon);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Info);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Separator1);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_Enabled);
            m_pTab.TabPages[0].Controls.Add(mt_General_FullName);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_FullName);
            m_pTab.TabPages[0].Controls.Add(mt_General_Description);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_Description);
            m_pTab.TabPages[0].Controls.Add(mt_General_LoginName);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_LoginName);
            m_pTab.TabPages[0].Controls.Add(mt_General_Password);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_Password);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_GeneratePwd);
            m_pTab.TabPages[0].Controls.Add(mt_General_MaxMailboxSize);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_MaxMailboxSize);
            m_pTab.TabPages[0].Controls.Add(mt_General_MaxMailboxMB);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Separator2);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_MailboxSize);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_MailboxSize);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_MailboxSizeIndicator);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Created);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Created);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_LastLogin);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_LastLogin);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Create);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region Addressing UI

            //--- Tabpage Addressing UI -------------------------------------------------------------------//
            m_pTab_Addressing_Icon = new PictureBox();
            m_pTab_Addressing_Icon.Size = new Size(32,32);
            m_pTab_Addressing_Icon.Location = new Point(10,10);
            m_pTab_Addressing_Icon.Image = ResManager.GetIcon("addressing.ico").ToBitmap();

            mt_Tab_Addressing_Info = new Label();
            mt_Tab_Addressing_Info.Size = new Size(200,32);
            mt_Tab_Addressing_Info.Location = new Point(50,10);
            mt_Tab_Addressing_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Addressing_Info.Text = "Specify user email addresses.";

            m_pTab_Addressing_Separator1 = new GroupBox();
            m_pTab_Addressing_Separator1.Size = new Size(475,3);
            m_pTab_Addressing_Separator1.Location = new Point(7,50);

            m_pTab_Addressing_LocalPart = new TextBox();
            m_pTab_Addressing_LocalPart.Size = new Size(190,20);
            m_pTab_Addressing_LocalPart.Location = new Point(8,55);

            mt_Tab_Addressing_At = new Label();
            mt_Tab_Addressing_At.Size = new Size(20,20);
            mt_Tab_Addressing_At.Location = new Point(205,55);
            mt_Tab_Addressing_At.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Addressing_At.Text = "@";

            m_pTab_Addressing_Domain = new ComboBox();
            m_pTab_Addressing_Domain.Size = new Size(190,20);
            m_pTab_Addressing_Domain.Location = new Point(230,55);
            m_pTab_Addressing_Domain.DropDownStyle = ComboBoxStyle.DropDownList;

            m_pTab_Addressing_Toolbar = new ToolStrip();
            m_pTab_Addressing_Toolbar.Location = new Point(430,55);
            m_pTab_Addressing_Toolbar.Dock = DockStyle.None;
            m_pTab_Addressing_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTab_Addressing_Toolbar.BackColor = this.BackColor;
            m_pTab_Addressing_Toolbar.Renderer = new ToolBarRendererEx();
            m_pTab_Addressing_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_Addressing_Toolbar_ItemClicked);
            // Add button
            ToolStripButton addressing_button_Add = new ToolStripButton();
            addressing_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            addressing_button_Add.Tag = "add";
            m_pTab_Addressing_Toolbar.Items.Add(addressing_button_Add);
            // Delete button
            ToolStripButton addressing_button_Delete = new ToolStripButton();
            addressing_button_Delete.Enabled = false;
            addressing_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            addressing_button_Delete.Tag = "delete";
            m_pTab_Addressing_Toolbar.Items.Add(addressing_button_Delete);

            m_pTab_Addressing_Addresses = new ListView();
            m_pTab_Addressing_Addresses.Size = new Size(475,220);
            m_pTab_Addressing_Addresses.Location = new Point(8,80);
            m_pTab_Addressing_Addresses.View = View.Details;
            m_pTab_Addressing_Addresses.HideSelection = false;
            m_pTab_Addressing_Addresses.FullRowSelect = true;
            m_pTab_Addressing_Addresses.Columns.Add("Email Address",450,HorizontalAlignment.Left);
            m_pTab_Addressing_Addresses.SelectedIndexChanged += new EventHandler(m_pTab_Addressing_Addresses_SelectedIndexChanged);

            m_pTab.TabPages[1].Controls.Add(m_pTab_Addressing_Icon);
            m_pTab.TabPages[1].Controls.Add(mt_Tab_Addressing_Info);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Addressing_Separator1);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Addressing_LocalPart);
            m_pTab.TabPages[1].Controls.Add(mt_Tab_Addressing_At);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Addressing_Domain);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Addressing_Toolbar);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Addressing_Addresses);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region Rules UI

            //--- Tabpage Rules UI ------------------------------------------------------------------------//
            m_pTab_Rules_Icon = new PictureBox();
            m_pTab_Rules_Icon.Size = new Size(32,32);
            m_pTab_Rules_Icon.Location = new Point(10,10);
            m_pTab_Rules_Icon.Image = ResManager.GetIcon("rule.ico").ToBitmap();

            mt_Tab_Rules_Info = new Label();
            mt_Tab_Rules_Info.Size = new Size(200,32);
            mt_Tab_Rules_Info.Location = new Point(50,10);
            mt_Tab_Rules_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Rules_Info.Text = "Specify user message rules.";

            m_pTab_Rules_Separator1 = new GroupBox();
            m_pTab_Rules_Separator1.Size = new Size(475,3);
            m_pTab_Rules_Separator1.Location = new Point(7,50);

            m_pTab_Rules_Toolbar = new ToolStrip();
            m_pTab_Rules_Toolbar.Size = new Size(125,25);
            m_pTab_Rules_Toolbar.Location = new Point(360,55);
            m_pTab_Rules_Toolbar.Dock = DockStyle.None;
            m_pTab_Rules_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTab_Rules_Toolbar.BackColor = this.BackColor;
            m_pTab_Rules_Toolbar.Renderer = new ToolBarRendererEx();
            m_pTab_Rules_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_Rules_Toolbar_ItemClicked);
            // Add button
            ToolStripButton rulesToolbar_button_Add = new ToolStripButton();
            rulesToolbar_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            rulesToolbar_button_Add.Tag = "add";
            m_pTab_Rules_Toolbar.Items.Add(rulesToolbar_button_Add);
            // Edit button
            ToolStripButton rulesToolbar_button_Edit = new ToolStripButton();
            rulesToolbar_button_Edit.Enabled = false;
            rulesToolbar_button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            rulesToolbar_button_Edit.Tag = "edit";
            m_pTab_Rules_Toolbar.Items.Add(rulesToolbar_button_Edit);
            // Delete button
            ToolStripButton rulesToolbar_button_Delete = new ToolStripButton();
            rulesToolbar_button_Delete.Enabled = false;
            rulesToolbar_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            rulesToolbar_button_Delete.Tag = "delete";
            m_pTab_Rules_Toolbar.Items.Add(rulesToolbar_button_Delete);
            // Separator
            m_pTab_Rules_Toolbar.Items.Add(new ToolStripSeparator());
            // Up button
            ToolStripButton rulesToolbar_button_Up = new ToolStripButton();
            rulesToolbar_button_Up.Enabled = false;
            rulesToolbar_button_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            rulesToolbar_button_Up.Tag = "up";
            m_pTab_Rules_Toolbar.Items.Add(rulesToolbar_button_Up);
            // Down button
            ToolStripButton rulesToolbar_button_down = new ToolStripButton();
            rulesToolbar_button_down.Enabled = false;
            rulesToolbar_button_down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            rulesToolbar_button_down.Tag = "down";
            m_pTab_Rules_Toolbar.Items.Add(rulesToolbar_button_down);

            m_pRules_Rules = new ListView();
            m_pRules_Rules.Size = new Size(475,220);
            m_pRules_Rules.Location = new Point(8,80);
            m_pRules_Rules.View = View.Details;
            m_pRules_Rules.HideSelection = false;
            m_pRules_Rules.FullRowSelect = true;
            m_pRules_Rules.Columns.Add("Description",450,HorizontalAlignment.Left);
            m_pRules_Rules.DoubleClick += new EventHandler(m_pRules_Rules_DoubleClick);
            m_pRules_Rules.SelectedIndexChanged += new EventHandler(m_pRules_Rules_SelectedIndexChanged);

            m_pTab.TabPages[2].Controls.Add(m_pTab_Rules_Icon);
            m_pTab.TabPages[2].Controls.Add(mt_Tab_Rules_Info);
            m_pTab.TabPages[2].Controls.Add(m_pTab_Rules_Separator1);
            m_pTab.TabPages[2].Controls.Add(m_pTab_Rules_Toolbar);
            m_pTab.TabPages[2].Controls.Add(m_pRules_Rules);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region Remote Servers

            //--- Tabpage Remote Servers UI ---------------------------------------------------------------//
            m_pTab_RemoteServers_Icon = new PictureBox();
            m_pTab_RemoteServers_Icon.Size = new Size(32,32);
            m_pTab_RemoteServers_Icon.Location = new Point(10,10);
            m_pTab_RemoteServers_Icon.Image = ResManager.GetIcon("remoteserver32.ico").ToBitmap();

            mt_Tab_RemoteServers_Info = new Label();
            mt_Tab_RemoteServers_Info.Size = new Size(200,32);
            mt_Tab_RemoteServers_Info.Location = new Point(50,10);
            mt_Tab_RemoteServers_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_RemoteServers_Info.Text = "Specify user remote mail servers.";

            m_pTab_RemoteServers_Separator1 = new GroupBox();
            m_pTab_RemoteServers_Separator1.Size = new Size(475,3);
            m_pTab_RemoteServers_Separator1.Location = new Point(7,50);

            m_pTab_RemoteServers_Toolbar = new ToolStrip();
            m_pTab_RemoteServers_Toolbar.Size = new Size(75,25);
            m_pTab_RemoteServers_Toolbar.Location = new Point(410,55);
            m_pTab_RemoteServers_Toolbar.Dock = DockStyle.None;
            m_pTab_RemoteServers_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTab_RemoteServers_Toolbar.BackColor = this.BackColor;
            m_pTab_RemoteServers_Toolbar.Renderer = new ToolBarRendererEx();
            m_pTab_RemoteServers_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_RemoteServers_Toolbar_ItemClicked); 
            // Add button
            ToolStripButton remsrv_button_Add = new ToolStripButton();
            remsrv_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            remsrv_button_Add.Tag = "add";
            m_pTab_RemoteServers_Toolbar.Items.Add(remsrv_button_Add);
            // Edit button
            ToolStripButton remsrv_button_Edit = new ToolStripButton();
            remsrv_button_Edit.Enabled = false;
            remsrv_button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            remsrv_button_Edit.Tag = "edit";
            m_pTab_RemoteServers_Toolbar.Items.Add(remsrv_button_Edit);
            // Delete button
            ToolStripButton remsrv_button_Delete = new ToolStripButton();
            remsrv_button_Delete.Enabled = false;
            remsrv_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            remsrv_button_Delete.Tag = "delete";
            m_pTab_RemoteServers_Toolbar.Items.Add(remsrv_button_Delete);

            m_pRemoteServers_Servers = new ListView();
            m_pRemoteServers_Servers.Size = new Size(475,220);
            m_pRemoteServers_Servers.Location = new Point(8,80);
            //m_pRemoteServers_Servers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pRemoteServers_Servers.View = View.Details;
            m_pRemoteServers_Servers.FullRowSelect = true;
            m_pRemoteServers_Servers.HideSelection = false;
            m_pRemoteServers_Servers.DoubleClick += new EventHandler(m_pRemoteServers_Servers_DoubleClick);
            m_pRemoteServers_Servers.SelectedIndexChanged += new EventHandler(m_pRemoteServers_Servers_SelectedIndexChanged);
            m_pRemoteServers_Servers.Columns.Add("Server",150,HorizontalAlignment.Left);
            m_pRemoteServers_Servers.Columns.Add("Description",300,HorizontalAlignment.Left);

            m_pTab.TabPages[3].Controls.Add(m_pTab_RemoteServers_Icon);
            m_pTab.TabPages[3].Controls.Add(mt_Tab_RemoteServers_Info);
            m_pTab.TabPages[3].Controls.Add(m_pTab_RemoteServers_Separator1);
            m_pTab.TabPages[3].Controls.Add(m_pTab_RemoteServers_Toolbar);
            m_pTab.TabPages[3].Controls.Add(m_pRemoteServers_Servers);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region Permissions UI

            //--- Tabpage Permissions UI ---------------------------------------------------------------//
            m_pTab_Permissions_Icon = new PictureBox();
            m_pTab_Permissions_Icon.Size = new Size(32,32);
            m_pTab_Permissions_Icon.Location = new Point(10,10);
            m_pTab_Permissions_Icon.Image = ResManager.GetIcon("security32.ico").ToBitmap();

            mt_Tab_Permissions_Info = new Label();
            mt_Tab_Permissions_Info.Size = new Size(200,32);
            mt_Tab_Permissions_Info.Location = new Point(50,10);
            mt_Tab_Permissions_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Permissions_Info.Text = "Specify user permissions.";

            m_pTab_Permissions_Separator1 = new GroupBox();
            m_pTab_Permissions_Separator1.Size = new Size(475,3);
            m_pTab_Permissions_Separator1.Location = new Point(7,50);

            m_pPermissions_AllowPop3 = new CheckBox();
            m_pPermissions_AllowPop3.Size = new Size(200,20);
            m_pPermissions_AllowPop3.Location = new Point(20,60);
            m_pPermissions_AllowPop3.Text = "Allow POP3";
            m_pPermissions_AllowPop3.Checked = true;

            m_pPermissions_AllowImap = new CheckBox();
            m_pPermissions_AllowImap.Size = new Size(200,20);
            m_pPermissions_AllowImap.Location = new Point(20,80);
            m_pPermissions_AllowImap.Text = "Allow IMAP";
            m_pPermissions_AllowImap.Checked = true;

            m_pPermissions_AllowRelay = new CheckBox();
            m_pPermissions_AllowRelay.Size = new Size(200,20);
            m_pPermissions_AllowRelay.Location = new Point(20,100);
            m_pPermissions_AllowRelay.Text = "Allow Relay";
            m_pPermissions_AllowRelay.Checked = true;

            m_pPermissions_AllowSIP = new CheckBox();
            m_pPermissions_AllowSIP.Size = new Size(200,20);
            m_pPermissions_AllowSIP.Location = new Point(20,120);
            m_pPermissions_AllowSIP.Text = "Allow SIP";
            m_pPermissions_AllowSIP.Checked = true;

            mt_Permissions_SipGwAccess = new Label();
            mt_Permissions_SipGwAccess.Size = new Size(200,20);
            mt_Permissions_SipGwAccess.Location = new Point(15,150);
            mt_Permissions_SipGwAccess.TextAlign = ContentAlignment.MiddleLeft;
            mt_Permissions_SipGwAccess.Text = "SIP Gateway access:";

            m_pPermissions_SipGwAccessToolbar = new ToolStrip();
            m_pPermissions_SipGwAccessToolbar.Size = new Size(75,25);
            m_pPermissions_SipGwAccessToolbar.Location = new Point(400,145);
            m_pPermissions_SipGwAccessToolbar.Dock = DockStyle.None;
            m_pPermissions_SipGwAccessToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pPermissions_SipGwAccessToolbar.BackColor = this.BackColor;
            m_pPermissions_SipGwAccessToolbar.Renderer = new ToolBarRendererEx();
            //m_pPermissions_SipGwAccessToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_RemoteServers_Toolbar_ItemClicked); 
            // Add button
            ToolStripButton gwAccess_button_Add = new ToolStripButton();
            gwAccess_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            gwAccess_button_Add.Tag = "add";
            m_pPermissions_SipGwAccessToolbar.Items.Add(gwAccess_button_Add);
            // Edit button
            ToolStripButton gwAccess_button_Edit = new ToolStripButton();
            gwAccess_button_Edit.Enabled = false;
            gwAccess_button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            gwAccess_button_Edit.Tag = "edit";
            m_pPermissions_SipGwAccessToolbar.Items.Add(gwAccess_button_Edit);
            // Delete button
            ToolStripButton gwAccess_button_Delete = new ToolStripButton();
            gwAccess_button_Delete.Enabled = false;
            gwAccess_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            gwAccess_button_Delete.Tag = "delete";
            m_pPermissions_SipGwAccessToolbar.Items.Add(gwAccess_button_Delete);

            m_pPermissions_SipGwAccess = new ListView();
            m_pPermissions_SipGwAccess.Size = new Size(455,125);
            m_pPermissions_SipGwAccess.Location = new Point(15,170);
            m_pPermissions_SipGwAccess.View = View.Details;
            m_pPermissions_SipGwAccess.HideSelection = false;
            m_pPermissions_SipGwAccess.FullRowSelect = true;
            m_pPermissions_SipGwAccess.Columns.Add("URI",60);
            m_pPermissions_SipGwAccess.Columns.Add("Access pattern",360);
            
            m_pTab.TabPages[4].Controls.Add(m_pTab_Permissions_Icon);
            m_pTab.TabPages[4].Controls.Add(mt_Tab_Permissions_Info);
            m_pTab.TabPages[4].Controls.Add(m_pTab_Permissions_Separator1);
            m_pTab.TabPages[4].Controls.Add(m_pPermissions_AllowPop3);
            m_pTab.TabPages[4].Controls.Add(m_pPermissions_AllowImap);
            m_pTab.TabPages[4].Controls.Add(m_pPermissions_AllowRelay);
            m_pTab.TabPages[4].Controls.Add(m_pPermissions_AllowSIP);
            m_pTab.TabPages[4].Controls.Add(mt_Permissions_SipGwAccess);
            m_pTab.TabPages[4].Controls.Add(m_pPermissions_SipGwAccessToolbar);
            m_pTab.TabPages[4].Controls.Add(m_pPermissions_SipGwAccess);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region Folders UI

            //--- Tabpage Folders UI -------------------------------------------------------------------//
            m_pTab_Folders_Icon = new PictureBox();
            m_pTab_Folders_Icon.Size = new Size(32,32);
            m_pTab_Folders_Icon.Location = new Point(10,10);
            m_pTab_Folders_Icon.Image = ResManager.GetIcon("folder32.ico").ToBitmap();
  
            mt_Tab_Folders_Info = new Label();
            mt_Tab_Folders_Info.Size = new Size(200,32);
            mt_Tab_Folders_Info.Location = new Point(50,10);
            mt_Tab_Folders_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Folders_Info.Text = "Manage user folders.";

            m_pTab_Folders_Separator1 = new GroupBox();
            m_pTab_Folders_Separator1.Size = new Size(475,3);
            m_pTab_Folders_Separator1.Location = new Point(7,50);

            m_pTab_Folders_Toolbar = new ToolStrip();
            m_pTab_Folders_Toolbar.Size = new Size(180,25);
            m_pTab_Folders_Toolbar.Location = new Point(305,55);
            m_pTab_Folders_Toolbar.Dock = DockStyle.None;
            m_pTab_Folders_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTab_Folders_Toolbar.BackColor = this.BackColor;
            m_pTab_Folders_Toolbar.Renderer = new ToolBarRendererEx();
            m_pTab_Folders_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_Folders_Toolbar_ItemClicked); 
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Tag = "add";
            button_Add.ToolTipText = "Create folder";
            m_pTab_Folders_Toolbar.Items.Add(button_Add);
            // Edit button
            ToolStripButton button_Edit = new ToolStripButton();
            button_Edit.Enabled = false;
            button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            button_Edit.Tag = "edit";
            button_Edit.ToolTipText = "Rename folder";
            m_pTab_Folders_Toolbar.Items.Add(button_Edit);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText = "Delete folder";
            m_pTab_Folders_Toolbar.Items.Add(button_Delete);
            // Separator
            m_pTab_Folders_Toolbar.Items.Add(new ToolStripSeparator());
            // Properties button
            ToolStripButton button_Properties = new ToolStripButton();
            button_Properties.Enabled = false;
            button_Properties.Image = ResManager.GetIcon("properties.ico").ToBitmap();
            button_Properties.Tag = "properties";
            button_Properties.ToolTipText = "Folder properties";
            m_pTab_Folders_Toolbar.Items.Add(button_Properties);
            // Separator
            m_pTab_Folders_Toolbar.Items.Add(new ToolStripSeparator());
            // View messages button
            ToolStripButton button_ViewMessages = new ToolStripButton();
            button_ViewMessages.Enabled = false;
            button_ViewMessages.Image = ResManager.GetIcon("viewmessages.ico").ToBitmap();
            button_ViewMessages.Tag = "viewmessages";
            button_ViewMessages.ToolTipText = "View folder messages";
            m_pTab_Folders_Toolbar.Items.Add(button_ViewMessages);
            // Recycle bin button
            ToolStripButton button_Recyclebin = new ToolStripButton();
            button_Recyclebin.Enabled = true;
            button_Recyclebin.Image = ResManager.GetIcon("recyclebin16.ico").ToBitmap();
            button_Recyclebin.Tag = "recyclebin";
            button_Recyclebin.ToolTipText = "Recycle bin";
            m_pTab_Folders_Toolbar.Items.Add(button_Recyclebin);
            // Transfer messages
            ToolStripButton button_Transfer = new ToolStripButton();
            button_Transfer.Enabled = true;
            button_Transfer.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();
            button_Transfer.Tag = "transfer";
            button_Transfer.ToolTipText = "Import / Export messages";
            m_pTab_Folders_Toolbar.Items.Add(button_Transfer);

            ImageList folders_ImageList = new ImageList();
            folders_ImageList.Images.Add(ResManager.GetIcon("folder.ico"));
            folders_ImageList.Images.Add(ResManager.GetIcon("share32.ico"));

            m_pTab_Folders_Folders = new TreeView();
            m_pTab_Folders_Folders.Size = new Size(475,220);
            m_pTab_Folders_Folders.Location = new Point(5,80);
            //m_pTab_Folders_Folders.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab_Folders_Folders.HideSelection = false;
            m_pTab_Folders_Folders.FullRowSelect = true;
            m_pTab_Folders_Folders.PathSeparator = "/";
            m_pTab_Folders_Folders.ImageList = folders_ImageList;
            m_pTab_Folders_Folders.AfterSelect += new TreeViewEventHandler(m_pTab_Folders_Folders_AfterSelect);

            m_pTabPage_Folders.Controls.Add(m_pTab_Folders_Icon);
            m_pTabPage_Folders.Controls.Add(mt_Tab_Folders_Info);
            m_pTabPage_Folders.Controls.Add(m_pTab_Folders_Separator1);
            m_pTabPage_Folders.Controls.Add(m_pTab_Folders_Toolbar);
            m_pTabPage_Folders.Controls.Add(m_pTab_Folders_Folders);
            //------------------------------------------------------------------------------------------//

            #endregion

        }
                                                                                                                                                                                                                                                                                                                                                                
        #endregion


        #region Events Handling

        #region method m_pGeneral_GeneratePwd_Click

        private void m_pGeneral_GeneratePwd_Click(object sender, EventArgs e)
        {
            m_pGeneral_Password.Text = Guid.NewGuid().ToString().Substring(0,8);
            m_pGeneral_Password.PasswordChar = '\0';
        }

        #endregion

        #region method m_pTab_General_Create_Click

        private void m_pTab_General_Create_Click(object sender, EventArgs e)
        {
            #region Validation

            if(m_pGeneral_LoginName.Text.Length == 0){
                MessageBox.Show("Login Name can't be empty !","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);				
				return;
            }

			#endregion
            
            //--- Construct user permissions ----------------------------//
            UserPermissions_enum permissions = UserPermissions_enum.None;
            if(m_pPermissions_AllowPop3.Checked){
                permissions |= UserPermissions_enum.POP3;
            }
            if(m_pPermissions_AllowImap.Checked){
                permissions |= UserPermissions_enum.IMAP;
            }
            if(m_pPermissions_AllowRelay.Checked){
                permissions |= UserPermissions_enum.Relay;
            }
            //----------------------------------------------------------//

            // Add user
            m_pUser = m_pVirtualServer.Users.Add(
                m_pGeneral_LoginName.Text,
                m_pGeneral_FullName.Text,
                m_pGeneral_Password.Text,
                m_pGeneral_Description.Text,
                (int)m_pGeneral_MaxMailboxSize.Value,
                m_pGeneral_Enabled.Checked,
                permissions
            );
                
            m_pTab.TabPages.Add(m_pTabPage_Addressing);
            m_pTab.TabPages.Add(m_pTabPage_Rules);
            m_pTab.TabPages.Add(m_pTabPage_RemoteServers);
            m_pTab.TabPages.Add(m_pTabPage_Permissions);
            m_pTab.TabPages.Add(m_pTabPage_Folders);
            m_pTab_General_Create.Visible = false;

            LoadFolders("");            
        }

        #endregion


        #region method m_pTab_Addressing_Toolbar_ItemClicked

        private void m_pTab_Addressing_Toolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                string address = m_pTab_Addressing_LocalPart.Text + "@" + m_pTab_Addressing_Domain.Text;

                #region Validation
                
			    if(m_pTab_Addressing_LocalPart.Text.Length == 0){
				    MessageBox.Show("Emails address can't be empty!!!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);				
				    return;
			    }

			    if(m_pTab_Addressing_Domain.Text.Length == 0){
				    MessageBox.Show("Domain must be selected !!!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				    return;
			    }
                /*
			    if(m_pServerAPI.MapUser(address) != null){
				    MessageBox.Show("Emails address already exist !!!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
				    return;
			    }*/

			    #endregion
                
                //--- Construct user permissions ----------------------------//
                UserPermissions_enum permissions = UserPermissions_enum.None;
                if(m_pPermissions_AllowPop3.Checked){
                    permissions |= UserPermissions_enum.POP3;
                }
                if(m_pPermissions_AllowImap.Checked){
                    permissions |= UserPermissions_enum.IMAP;
                }
                if(m_pPermissions_AllowRelay.Checked){
                    permissions |= UserPermissions_enum.Relay;
                }
                //----------------------------------------------------------//
                                            			            		
	            //--- Add email address -----------------------//
                m_pUser.EmailAddresses.Add(address);

			    ListViewItem it = new ListViewItem(address);
			    m_pTab_Addressing_Addresses.Items.Add(it);
                //--------------------------------------------//

			    m_pTab_Addressing_LocalPart.Text = "";
			    m_pTab_Addressing_LocalPart.Focus();
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                if(m_pTab_Addressing_Addresses.SelectedItems.Count > 0){
                    if(MessageBox.Show(this,"Are you sure you want to delete email address '" + m_pTab_Addressing_Addresses.SelectedItems[0].Text + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
			            m_pUser.EmailAddresses.Remove(m_pTab_Addressing_Addresses.SelectedItems[0].Text);
                        m_pTab_Addressing_Addresses.SelectedItems[0].Remove();
                    }
			    }
            }
        }

        #endregion

        #region method m_pTab_Addressing_Addresses_SelectedIndexChanged

        private void m_pTab_Addressing_Addresses_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pTab_Addressing_Addresses.SelectedItems.Count > 0){
                m_pTab_Addressing_Toolbar.Items[1].Enabled = true;
            }
            else{
                m_pTab_Addressing_Toolbar.Items[1].Enabled = false;
            }
        }

        #endregion


        #region method m_pTab_Rules_Toolbar_ItemClicked

        private void m_pTab_Rules_Toolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_User_MessageRule frm = new wfrm_User_MessageRule(m_pUser);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRules(frm.RuleID);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                UserMessageRule rule = (UserMessageRule)m_pRules_Rules.SelectedItems[0].Tag;
                wfrm_User_MessageRule frm = new wfrm_User_MessageRule(
                    m_pUser,
                    rule
                );
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRules(rule.ID);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                UserMessageRule rule = (UserMessageRule)m_pRules_Rules.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete Rule '" + rule.Description + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    rule.Owner.Remove(rule);
                    LoadRules("");
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "up"){
                if(m_pRules_Rules.SelectedItems.Count > 0 && m_pRules_Rules.SelectedItems[0].Index > 0){
                    SwapRules(m_pRules_Rules.SelectedItems[0],m_pRules_Rules.Items[m_pRules_Rules.SelectedItems[0].Index - 1]);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "down"){
                if(m_pRules_Rules.SelectedItems.Count > 0 && m_pRules_Rules.SelectedItems[0].Index < m_pRules_Rules.Items.Count - 1){
                    SwapRules(m_pRules_Rules.SelectedItems[0],m_pRules_Rules.Items[m_pRules_Rules.SelectedItems[0].Index + 1]);
                }
            }
        }

        #endregion

        #region method m_pRules_Rules_DoubleClick

        private void m_pRules_Rules_DoubleClick(object sender, EventArgs e)
        {
            if(m_pRules_Rules.SelectedItems.Count > 0){
                UserMessageRule rule = (UserMessageRule)m_pRules_Rules.SelectedItems[0].Tag;
                wfrm_User_MessageRule frm = new wfrm_User_MessageRule(
                    m_pUser,
                    rule
                );
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRules(rule.ID);
                }
            }
        }

        #endregion

        #region method m_pRules_Rules_SelectedIndexChanged

        private void m_pRules_Rules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pRules_Rules.SelectedItems.Count > 0){
                m_pTab_Rules_Toolbar.Items[1].Enabled = true;
                m_pTab_Rules_Toolbar.Items[2].Enabled = true;
                if(m_pRules_Rules.SelectedItems[0].Index > 0){
                    m_pTab_Rules_Toolbar.Items[4].Enabled = true;
                }
                if(m_pRules_Rules.SelectedItems[0].Index < (m_pRules_Rules.Items.Count - 1)){
                    m_pTab_Rules_Toolbar.Items[5].Enabled = true;
                }
            }
            else{
                m_pTab_Rules_Toolbar.Items[1].Enabled = false;
                m_pTab_Rules_Toolbar.Items[2].Enabled = false;
                m_pTab_Rules_Toolbar.Items[4].Enabled = false;
                m_pTab_Rules_Toolbar.Items[5].Enabled = false;
            }
        }

        #endregion


        #region method m_pTab_RemoteServers_Toolbar_ItemClicked

        private void m_pTab_RemoteServers_Toolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_User_RemoteServers_Server frm = new wfrm_User_RemoteServers_Server(m_pUser);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRemoteServers(frm.RemoteServerID);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                UserRemoteServer remoteServer = (UserRemoteServer)m_pRemoteServers_Servers.SelectedItems[0].Tag;
                wfrm_User_RemoteServers_Server frm = new wfrm_User_RemoteServers_Server(m_pUser,remoteServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRemoteServers(frm.RemoteServerID);
                }                
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                UserRemoteServer remoteServer = (UserRemoteServer)m_pRemoteServers_Servers.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete user Remote server '" + remoteServer.Host + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    remoteServer.Owner.Remove(remoteServer);
                    LoadRemoteServers("");
                }
            }
        }

        #endregion

        #region method m_pRemoteServers_Servers_DoubleClick

        private void m_pRemoteServers_Servers_DoubleClick(object sender, EventArgs e)
        {
            if(m_pRemoteServers_Servers.SelectedItems.Count > 0){
                UserRemoteServer remoteServer = (UserRemoteServer)m_pRemoteServers_Servers.SelectedItems[0].Tag;
                wfrm_User_RemoteServers_Server frm = new wfrm_User_RemoteServers_Server(m_pUser,remoteServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRemoteServers(frm.RemoteServerID);
                }
            }
        }

        #endregion

        #region method m_pRemoteServers_Servers_SelectedIndexChanged

        private void m_pRemoteServers_Servers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pRemoteServers_Servers.Items.Count > 0 && m_pRemoteServers_Servers.SelectedItems.Count > 0){
                m_pTab_RemoteServers_Toolbar.Items[1].Enabled = true;
                m_pTab_RemoteServers_Toolbar.Items[2].Enabled = true;
            }
            else{
                m_pTab_RemoteServers_Toolbar.Items[1].Enabled = false;
                m_pTab_RemoteServers_Toolbar.Items[2].Enabled = false;
            }
        }

        #endregion


        #region method m_pTab_Folders_Toolbar_ItemClicked

        private void m_pTab_Folders_Toolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }
                        
            if(e.ClickedItem.Tag.ToString() == "add"){
                if(m_pTab_Folders_Folders.SelectedNode == null){
                    wfrm_sys_Folder frm = new wfrm_sys_Folder(true,"",false);
                    if(frm.ShowDialog(this) == DialogResult.OK){
                        m_pUser.Folders.Add(frm.Folder);
                        LoadFolders(frm.Folder);
                    }
                }
                else{
                    wfrm_sys_Folder frm = new wfrm_sys_Folder(true,"",false);
                    if(frm.ShowDialog(this) == DialogResult.OK){
                        UserFolder folder = (UserFolder)m_pTab_Folders_Folders.SelectedNode.Tag;
                        folder.ChildFolders.Add(frm.Folder);
                        LoadFolders(frm.Folder);
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit" && m_pTab_Folders_Folders.SelectedNode != null){
                wfrm_sys_Folder frm = new wfrm_sys_Folder(false,m_pTab_Folders_Folders.SelectedNode.FullPath,true);
                if(frm.ShowDialog(this) == DialogResult.OK && m_pTab_Folders_Folders.SelectedNode.FullPath != frm.Folder){
                    UserFolder folder = (UserFolder)m_pTab_Folders_Folders.SelectedNode.Tag;
                    folder.Rename(frm.Folder);
                    LoadFolders(frm.Folder);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete" && m_pTab_Folders_Folders.SelectedNode != null){
                UserFolder folder = (UserFolder)m_pTab_Folders_Folders.SelectedNode.Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete Folder '" + folder.FolderFullPath + "' ?","Confirm delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                    folder.Owner.Remove(folder);
                    LoadFolders("");
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "properties" && m_pTab_Folders_Folders.SelectedNode != null){
                UserFolder folder = (UserFolder)m_pTab_Folders_Folders.SelectedNode.Tag;
                wfrm_User_FolderProperties frm = new wfrm_User_FolderProperties(m_pVirtualServer,folder);
                frm.ShowDialog(this);             
            }
            else if(e.ClickedItem.Tag.ToString() == "viewmessages" && m_pTab_Folders_Folders.SelectedNode != null){
                UserFolder folder = (UserFolder)m_pTab_Folders_Folders.SelectedNode.Tag;
                wfrm_User_FolderMessages frm = new wfrm_User_FolderMessages(m_pVirtualServer,folder);
                frm.ShowDialog(this);
            }
            else if(e.ClickedItem.Tag.ToString() == "recyclebin"){
                wfrm_User_Recyclebin frm = new wfrm_User_Recyclebin(m_pVirtualServer,m_pUser);
                frm.ShowDialog(this);
            }
            else if(e.ClickedItem.Tag.ToString() == "transfer"){
                wfrm_utils_MessagesTransferer frm = new wfrm_utils_MessagesTransferer(m_pUser);
                frm.ShowDialog();
            }
        }

        #endregion

        #region method m_pTab_Folders_Folders_AfterSelect

        private void m_pTab_Folders_Folders_AfterSelect(object sender,TreeViewEventArgs e)
        {
            if(e.Node != null){
                m_pTab_Folders_Toolbar.Items[1].Enabled = true;
                m_pTab_Folders_Toolbar.Items[2].Enabled = true;
                m_pTab_Folders_Toolbar.Items[4].Enabled = true;
                m_pTab_Folders_Toolbar.Items[6].Enabled = true;
                //m_pTab_Folders_Toolbar.Items[7].Enabled = true;
            }
            else{
                m_pTab_Folders_Toolbar.Items[1].Enabled = false;
                m_pTab_Folders_Toolbar.Items[2].Enabled = false;
                m_pTab_Folders_Toolbar.Items[4].Enabled = false;
                m_pTab_Folders_Toolbar.Items[6].Enabled = false;
                //m_pTab_Folders_Toolbar.Items[7].Enabled = false;
            }
        }

        #endregion


        #region ovveride method ProcessDialogKey

        /// <summary>
        /// Processes dialog box key.
        /// </summary>
        /// <param name="keyData">Key.</param>
        /// <returns>Returns true if key processed, otherwise false.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if(keyData == Keys.Escape){
                m_Cancel_Click(null,null);
            }

            return base.ProcessDialogKey(keyData);
        }

        #endregion

        #region method m_Cancel_Click

        private void m_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            #region Validation

            if (m_pGeneral_LoginName.Text.Length == 0){
                MessageBox.Show("Login Name can't be empty!!!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);				
				return;
            }

            #endregion

            //--- Construct user permissions ----------------------------//
            UserPermissions_enum permissions = UserPermissions_enum.None;
            if(m_pPermissions_AllowPop3.Checked){
                permissions |= UserPermissions_enum.POP3;
            }
            if(m_pPermissions_AllowImap.Checked){
                permissions |= UserPermissions_enum.IMAP;
            }
            if(m_pPermissions_AllowRelay.Checked){
                permissions |= UserPermissions_enum.Relay;
            }
            if(m_pPermissions_AllowSIP.Checked){
                permissions |= UserPermissions_enum.SIP;
            }
            //----------------------------------------------------------//

            // New user, add it
            if(m_pUser == null){
                m_pUser = m_pVirtualServer.Users.Add(
                   m_pGeneral_LoginName.Text,
                   m_pGeneral_FullName.Text,
                   m_pGeneral_Password.Text,
                   m_pGeneral_Description.Text,
                   (int)m_pGeneral_MaxMailboxSize.Value,
                   m_pGeneral_Enabled.Checked,
                   permissions
                );
            }
            // Update user
            else{
                m_pUser.UserName = m_pGeneral_LoginName.Text;
                m_pUser.Password = m_pGeneral_Password.Text;
                m_pUser.FullName = m_pGeneral_FullName.Text;
                m_pUser.Description = m_pGeneral_Description.Text;
                m_pUser.MaximumMailboxSize = (int)m_pGeneral_MaxMailboxSize.Value;
                m_pUser.Enabled = m_pGeneral_Enabled.Checked;
                m_pUser.Permissions = permissions;
                m_pUser.Commit();
            }

           this.DialogResult = DialogResult.OK;
           this.Close();
        }

        #endregion

        #endregion


        #region method LoadSettings

        /// <summary>
        /// Loads user settings to UI.
        /// </summary>
        private void LoadSettings()
        {
            //--- General tab ----------------------------------------------------------------------------------//
            m_pGeneral_Enabled.Checked = m_pUser.Enabled;
            m_pGeneral_FullName.Text = m_pUser.FullName;
            m_pGeneral_Description.Text = m_pUser.Description;
			m_pGeneral_LoginName.Text = m_pUser.UserName;
			m_pGeneral_Password.Text = m_pUser.Password;
			m_pGeneral_Password.PasswordChar = '*';
			m_pGeneral_MaxMailboxSize.Value = m_pUser.MaximumMailboxSize;
            			
            long currentMailboxSize = m_pUser.MailboxSize;
            m_pTab_General_MailboxSize.Text = Convert.ToDecimal(m_pUser.MailboxSize / (decimal)1000000).ToString("f2") + " MB";
            if(m_pGeneral_MaxMailboxSize.Value > 0){
                m_pTab_General_MailboxSize.Text += " of " + m_pGeneral_MaxMailboxSize.Value.ToString() + " MB";
            }
            m_pGeneral_MailboxSizeIndicator.Maximum = (int)m_pUser.MaximumMailboxSize;
			if((currentMailboxSize / 1000000) < m_pGeneral_MailboxSizeIndicator.Maximum){
				m_pGeneral_MailboxSizeIndicator.Value = (int)(currentMailboxSize / 1000000);
			}
			else{
				m_pGeneral_MailboxSizeIndicator.Value = m_pGeneral_MailboxSizeIndicator.Maximum;
			}
	        DateTime userCreationTime = m_pUser.CreationTime;
            m_pTab_General_Created.Text = userCreationTime.ToLongDateString() + " " + userCreationTime.ToLongTimeString();
            DateTime lastloginTime = m_pUser.LastLogin;
            m_pTab_General_LastLogin.Text = lastloginTime.ToLongDateString() + " " + lastloginTime.ToLongTimeString();
            //---------------------------------------------------------------------------------------------------//

            //--- Addressing tab --------------------------------------------------------------------------------//
            // Fill user addresses
            foreach(string emailAddress in m_pUser.EmailAddresses){
                ListViewItem it = new ListViewItem(emailAddress);                
			    m_pTab_Addressing_Addresses.Items.Add(it);
            }
            //---------------------------------------------------------------------------------------------------//
            
            //--- Permissions tab -------------------------------------------------------------------------------//
            m_pPermissions_AllowPop3.Checked  = (m_pUser.Permissions & UserPermissions_enum.POP3)  != 0;
            m_pPermissions_AllowImap.Checked  = (m_pUser.Permissions & UserPermissions_enum.IMAP)  != 0;
            m_pPermissions_AllowRelay.Checked = (m_pUser.Permissions & UserPermissions_enum.Relay) != 0;
            m_pPermissions_AllowSIP.Checked   = (m_pUser.Permissions & UserPermissions_enum.SIP) != 0;
            //---------------------------------------------------------------------------------------------------//            
        }

        #endregion

        #region method LoadRules

        /// <summary>
        /// Loads user message ruels to UI.
        /// </summary>
        /// <param name="ruleID">Selects specified rule, if exists.</param>
        private void LoadRules(string selectedRuleID)
        {
            m_pRules_Rules.Items.Clear();

            foreach(UserMessageRule rule in  m_pUser.MessageRules){
                ListViewItem it = new ListViewItem();
                // Make disabled rules red and striked out
                if(!rule.Enabled){
                    it.ForeColor = Color.Purple;
                    it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                    it.ImageIndex = 1;
                }
                else{
                    it.ImageIndex = 0;
                }
                it.Tag = rule;
                it.Text = rule.Description;
                m_pRules_Rules.Items.Add(it);

                if(rule.ID == selectedRuleID){
                    it.Selected = true;
                }
            }

            m_pRules_Rules_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

        #region method LoadRemoteServers

        /// <summary>
        /// Loads user remote servers to UI.
        /// </summary>
        /// <param name="selectedServerID">Selects specified server, if exists.</param>
        private void LoadRemoteServers(string selectedServerID)
        {
            m_pRemoteServers_Servers.Items.Clear();

            foreach(UserRemoteServer remoteServer in m_pUser.RemoteServers){
                ListViewItem it = new ListViewItem();
                // Make disabled red and striked out
                if(remoteServer.Enabled){
                    it.ImageIndex = 0;
                }
                else{
                    it.ForeColor = Color.Purple;
                    it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                    it.ImageIndex = 1;
                }
                it.Tag = remoteServer;
                it.Text = remoteServer.Host;
                it.SubItems.Add(remoteServer.Description);
                m_pRemoteServers_Servers.Items.Add(it);
                
                if(remoteServer.ID == selectedServerID){
                    it.Selected = true;
                }
            }

            m_pRemoteServers_Servers_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

        #region method LoadDomains

        private void LoadDomains()
        {
            // Fill domains combo
            foreach(Domain domain in m_pVirtualServer.Domains){
                m_pTab_Addressing_Domain.Items.Add(domain.DomainName);
            }
            if(m_pTab_Addressing_Domain.Items.Count > 0){
                m_pTab_Addressing_Domain.SelectedIndex = 0;
            }
        }

        #endregion

        #region method LoadFolders

        /// <summary>
        /// Load user folder to UI.
        /// </summary>
        /// <param name="selectedFolder">Selects specified folder, it it exists.</param>
        private void LoadFolders(string selectedFolder)
        {
            m_pTab_Folders_Folders.Nodes.Clear();
                        
            Queue<object> folders = new Queue<object>();
            foreach(UserFolder folder in m_pUser.Folders){                
                TreeNode n = new TreeNode(folder.FolderName);
                if(!IsFolderShared(folder.FolderName)){
                    n.ImageIndex = 0;
                    n.SelectedImageIndex = 0;
                }
                else{
                    n.ImageIndex = 1;
                    n.SelectedImageIndex = 1;
                }
                n.Tag = folder;
                m_pTab_Folders_Folders.Nodes.Add(n);

                folders.Enqueue(new object[]{folder,n});
            }

            while(folders.Count > 0){
                object[]   param  = (object[])folders.Dequeue();
                UserFolder folder = (UserFolder)param[0];
                TreeNode   node   = (TreeNode)param[1];
                foreach(UserFolder childFolder in folder.ChildFolders){                                        
                    TreeNode n = new TreeNode(childFolder.FolderName);
                    if(!IsFolderShared(childFolder.FolderName)){
                        n.ImageIndex = 0;
                        n.SelectedImageIndex = 0;
                    }
                    else{
                        n.ImageIndex = 1;
                        n.SelectedImageIndex = 1;
                    }
                    n.Tag = childFolder;
                    node.Nodes.Add(n);

                    folders.Enqueue(new object[]{childFolder,n});
                }                
            }

            m_pTab_Folders_Folders_AfterSelect(this,new TreeViewEventArgs(m_pTab_Folders_Folders.SelectedNode));
        }

        #endregion

        #region method IsFolderShared

        /// <summary>
        /// Gets if specified folder is shared as user bounded shared folder.
        /// </summary>
        /// <param name="folder">Folder name with path.</param>
        /// <returns></returns>
        private bool IsFolderShared(string folder)
        {
            foreach(SharedRootFolder root in m_pVirtualServer.RootFolders){
                if(root.Type == SharedFolderRootType_enum.BoundedRootFolder && root.BoundedUser == m_pUser.UserName && root.BoundedFolder == folder){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method SwapRules

        /// <summary>
        /// Swaps specified rules.
        /// </summary>
        /// <param name="item1">Item 1.</param>
        /// <param name="item2">Item 2.</param>
        private void SwapRules(ListViewItem item1,ListViewItem item2)
        {
            UserMessageRule rule_down = (UserMessageRule)item1.Tag;
            UserMessageRule rule_up   = (UserMessageRule)item2.Tag;

            string selectedID = "";
            if(item1.Selected){
                selectedID = rule_down.ID;
            }
            else if(item2.Selected){
                selectedID = rule_up.ID;
            }

            long up_Cost = rule_up.Cost;

            rule_up.Cost = rule_down.Cost;
            rule_up.Commit();

            rule_down.Cost = up_Cost;
            rule_down.Commit();

            m_pUser.MessageRules.Refresh();                        
            LoadRules(selectedID);
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets user ID who is active in that window or null if no active user.
        /// </summary>
        public string UserID
        {
            get{ 
                if(m_pUser != null){
                    return m_pUser.UserID;
                }
                else{
                    return ""; 
                }
            }
        }

        #endregion

    }
}
