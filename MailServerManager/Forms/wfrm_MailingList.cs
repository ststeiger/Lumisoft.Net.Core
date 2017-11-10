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
    /// Add/Edit mailing list window.
    /// </summary>
    public class wfrm_MailingList : Form
    {
        // Common UI
        private TabControl m_pTab       = null;
        private GroupBox   m_pGroupBox1 = null;
        private Button     m_pCancel    = null;
        private Button     m_pOk        = null;
        // General tab UI
        private Label     mt_MailingListName = null;
        private TextBox   m_pMailingListName = null;
        private Label     mt_At              = null;
        private ComboBox  m_pDomains         = null; 
        private Label     mt_Description     = null;
        private TextBox   m_pDescription     = null;
        private CheckBox  m_pEnabled         = null;
        // Members tab UI
        private Label    mt_Member         = null;
        private TextBox  m_pMember         = null;
        private Button   m_pGetUserOrGroup = null;
        private ListView m_pMembers        = null;
        private Button   m_pMembers_Add    = null;
        private Button   m_pMembers_Remove = null;
        // Access tab UI
        private ImageList m_pAccessImages  = null;
        private ListView  m_pAccess        = null;
        private Button    m_pAccess_Add    = null;
        private Button    m_pAccess_Remove = null;
        
        private VirtualServer m_pVirtualServer = null;
        private MailingList   m_pMailingList   = null;
        
        /// <summary>
        /// Add new constructor.
        /// </summary>
        /// <param name="serverAPI">Reference to server API.</param>
        public wfrm_MailingList(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadDomains();
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="serverAPI">Reference to server API.</param>
        /// <param name="mailingList">Mailing list to update.</param>
        public wfrm_MailingList(VirtualServer virtualServer,MailingList mailingList)
        {
            m_pVirtualServer = virtualServer;
            m_pMailingList   = mailingList;

            InitUI();

            LoadDomains();

            m_pMailingListName.Text = mailingList.Name.Split(new char[]{'@'})[0];
            m_pDomains.SelectedText = mailingList.Name.Split(new char[]{'@'})[1];
            m_pDescription.Text = mailingList.Description;
            m_pEnabled.Checked = mailingList.Enabled;

            LoadMembers("");
            LoadAccess("");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,323);
            this.MinimumSize = new Size(400,350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Add/Edit Mailing List";

            //--- Common UI ----------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(393,280);
            m_pTab.Location = new Point(0,5);
            m_pTab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab.TabPages.Add(new TabPage("General"));
            m_pTab.TabPages.Add(new TabPage("Members"));
            m_pTab.TabPages.Add(new TabPage("Access"));

            m_pGroupBox1 = new GroupBox();
            m_pGroupBox1.Size = new Size(385,4);
            m_pGroupBox1.Location = new Point(5,290);
            m_pGroupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(245,300);
            m_pCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(320,300);
            m_pOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pGroupBox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
            //-----------------------------------------------//

            //--- General tab UI ----------------------------//
            mt_MailingListName = new Label();
            mt_MailingListName.Size = new Size(200,13);
            mt_MailingListName.Location = new Point(5,10);
            mt_MailingListName.Text = "Name:";

            m_pMailingListName = new TextBox();
            m_pMailingListName.Size = new Size(190,20);
            m_pMailingListName.Location = new Point(5,25);

            mt_At = new Label();
            mt_At.Size = new Size(13,13);
            mt_At.Location = new Point(195,28);
            mt_At.Text = "@";

            m_pDomains = new ComboBox();
            m_pDomains.Size = new Size(170,20);
            m_pDomains.Location = new Point(210,25);
            m_pDomains.DropDownStyle = ComboBoxStyle.DropDownList;

            mt_Description = new Label();
            mt_Description.Size = new Size(200,13);
            mt_Description.Location = new Point(5,55);
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(375,20);
            m_pDescription.Location = new Point(5,70);
                        
            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(265,13);
            m_pEnabled.Location = new Point(5,105);
            m_pEnabled.Checked = true;
            m_pEnabled.Text = "Enabled";

            m_pTab.TabPages[0].Controls.Add(mt_MailingListName);
            m_pTab.TabPages[0].Controls.Add(m_pMailingListName);
            m_pTab.TabPages[0].Controls.Add(mt_At);
            m_pTab.TabPages[0].Controls.Add(m_pDomains);
            m_pTab.TabPages[0].Controls.Add(mt_Description);
            m_pTab.TabPages[0].Controls.Add(m_pDescription);
            m_pTab.TabPages[0].Controls.Add(m_pEnabled);
            //-----------------------------------------------//  
        
            //--- Members tab UI ----------------------------//
            mt_Member = new Label();          
            mt_Member.Size = new Size(50,13);
            mt_Member.Location = new Point(5,22);
            mt_Member.TextAlign = ContentAlignment.MiddleRight;
            mt_Member.Text = "Member:";

            m_pMember = new TextBox();            
            m_pMember.Size = new Size(215,20);
            m_pMember.Location = new Point(65,20);

            m_pGetUserOrGroup = new Button();                 
            m_pGetUserOrGroup.Size = new Size(20,20);
            m_pGetUserOrGroup.Location = new Point(285,20);
            m_pGetUserOrGroup.Image = ResManager.GetIcon("group.ico").ToBitmap();
            m_pGetUserOrGroup.Click += new EventHandler(m_pGetUserOrGroup_Click);
            
            m_pMembers = new ListView();
            m_pMembers.Size = new Size(300,200);
            m_pMembers.Location = new Point(5,45);
            m_pMembers.View = View.List;
            m_pMembers.FullRowSelect = true;
            m_pMembers.HideSelection = false;
            m_pMembers.SelectedIndexChanged += new EventHandler(m_pMembers_SelectedIndexChanged);
               
            m_pMembers_Add = new Button();
            m_pMembers_Add.Size = new Size(70,20);
            m_pMembers_Add.Location = new Point(310,20);
            m_pMembers_Add.Text = "Add";
            m_pMembers_Add.Click += new EventHandler(m_pMembers_Add_Click);

            m_pMembers_Remove = new Button();
            m_pMembers_Remove.Size = new Size(70,20);
            m_pMembers_Remove.Location = new Point(310,45);
            m_pMembers_Remove.Text = "Remove";
            m_pMembers_Remove.Click += new EventHandler(m_pMembers_Remove_Click);

            m_pTab.TabPages[1].Controls.Add(mt_Member);
            m_pTab.TabPages[1].Controls.Add(m_pMember);
            m_pTab.TabPages[1].Controls.Add(m_pGetUserOrGroup);
            m_pTab.TabPages[1].Controls.Add(m_pMembers);
            m_pTab.TabPages[1].Controls.Add(m_pMembers_Add);
            m_pTab.TabPages[1].Controls.Add(m_pMembers_Remove);
            //-----------------------------------------------//

            //--- Access tab UI -----------------------------//
            m_pAccessImages = new ImageList();
            m_pAccessImages.Images.Add(ResManager.GetIcon("user.ico"));
            m_pAccessImages.Images.Add(ResManager.GetIcon("group.ico"));

            m_pAccess = new ListView();
            m_pAccess.Size = new Size(300,220);
            m_pAccess.Location = new Point(5,20);
            m_pAccess.View = View.List;
            m_pAccess.FullRowSelect = true;
            m_pAccess.HideSelection = false;
            m_pAccess.SmallImageList = m_pAccessImages;
            m_pAccess.SelectedIndexChanged += new EventHandler(m_pAccess_SelectedIndexChanged);

            m_pAccess_Add = new Button();
            m_pAccess_Add.Size = new Size(70,20);
            m_pAccess_Add.Location = new Point(310,20);
            m_pAccess_Add.Text = "Add";
            m_pAccess_Add.Click += new EventHandler(m_pAccess_Add_Click);

            m_pAccess_Remove = new Button();
            m_pAccess_Remove.Size = new Size(70,20);
            m_pAccess_Remove.Location = new Point(310,45);
            m_pAccess_Remove.Text = "Remove";
            m_pAccess_Remove.Click += new EventHandler(m_pAccess_Remove_Click);

            m_pTab.TabPages[2].Controls.Add(m_pAccess);
            m_pTab.TabPages[2].Controls.Add(m_pAccess_Add);
            m_pTab.TabPages[2].Controls.Add(m_pAccess_Remove);
            //-----------------------------------------------//
        }
                                                                                                                                                
        #endregion


        #region Events Handling

        #region method m_pGetUserOrGroup_Click

        private void m_pGetUserOrGroup_Click(object sender, EventArgs e)
        {
            wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pVirtualServer,true,false);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pMember.Text = frm.SelectedUserOrGroup;
            }
        }

        #endregion

        #region method m_pMembers_SelectedIndexChanged

        private void m_pMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pMembers.SelectedItems.Count > 0){
                m_pMembers_Remove.Enabled = true;
            }
            else{
                m_pMembers_Remove.Enabled = false;
            }
        }

        #endregion

        #region method m_pMembers_Add_Click

        private void m_pMembers_Add_Click(object sender, EventArgs e)
        {
            // This is new not yet added mailing list, add it.
            if(m_pMailingList == null){
                if(!AddOrUpdate()){
                    return;
                }
            }

            //--- Validate values ----------------------------------------------//
            if(m_pMember.Text == ""){
                MessageBox.Show(this,"Please fill member name !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //------------------------------------------------------------------//

            m_pMailingList.Members.Add(m_pMember.Text);

            LoadMembers(m_pMember.Text);
                        
            m_pMember.Text = "";
        }

        #endregion

        #region method m_pMembers_Remove_Click

        private void m_pMembers_Remove_Click(object sender, EventArgs e)
        {
            if(m_pMembers.SelectedItems.Count > 0){
                m_pMailingList.Members.Remove(m_pMembers.SelectedItems[0].Text);
                LoadMembers("");
            }
        }

        #endregion


        #region method m_pAccess_SelectedIndexChanged

        private void m_pAccess_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pAccess.SelectedItems.Count > 0){
                m_pAccess_Remove.Enabled = true;
            }
            else{
                m_pAccess_Remove.Enabled = false;
            }
        }

        #endregion

        #region method m_pAccess_Add_Click

        private void m_pAccess_Add_Click(object sender, EventArgs e)
        {
            //--- Exclude existing members + group itself ---//
            List<string> excludeList = new List<string>();
            foreach(ListViewItem it in m_pAccess.Items){
                excludeList.Add(it.Text.ToLower());
            }
            //-----------------------------------------------//

            wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pVirtualServer,true,true,true,excludeList);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pMailingList.ACL.Add(frm.SelectedUserOrGroup);
                LoadAccess(frm.SelectedUserOrGroup);
            }
        }

        #endregion

        #region method m_pAccess_Remove_Click

        private void m_pAccess_Remove_Click(object sender, EventArgs e)
        {
            if(m_pAccess.SelectedItems.Count > 0){
                m_pMailingList.ACL.Remove(m_pAccess.SelectedItems[0].Text);
                LoadAccess("");
            }
        }

        #endregion


        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            AddOrUpdate();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region method AddOrUpdate

        /// <summary>
        /// Adds or updates mailing list as needed. Returns true if operation was successful.
        /// </summary>
        private bool AddOrUpdate()
        {
            //--- Validate values ----------------------------------------------//
            if(m_pMailingListName.Text == ""){
                MessageBox.Show(this,"Please fill mailing list name !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
            if(m_pDomains.SelectedIndex == -1){
                MessageBox.Show(this,"Please choose domain !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
            //------------------------------------------------------------------//
            
            // Add new mailing list
            if(m_pMailingList == null){
                m_pMailingList = m_pVirtualServer.MailingLists.Add(
                    m_pMailingListName.Text + "@" + m_pDomains.SelectedItem.ToString(),
                    m_pDescription.Text,
                    m_pEnabled.Checked                  
                );
            }
            // Update mailing list
            else{
                m_pMailingList.Enabled = m_pEnabled.Checked;
                m_pMailingList.Name = m_pMailingListName.Text + "@" + m_pDomains.SelectedItem.ToString();
                m_pMailingList.Description = m_pDescription.Text;
                m_pMailingList.Commit();
            }

            return true;
        }

        #endregion

        #region method LoadDomains

        /// <summary>
        /// Fills domains combo.
        /// </summary>
        private void LoadDomains()
        {
            foreach(Domain domain in m_pVirtualServer.Domains){
                m_pDomains.Items.Add(domain.DomainName);
            }

			if(m_pDomains.Items.Count > 0){
				m_pDomains.SelectedIndex = 0;
			}
        }

        #endregion

        #region method LoadMembers

        /// <summary>
        /// Load mailing list members to UI.
        /// </summary>
        /// <param name="selectedMember">Selects specified member, if it exists.</param>
        private void LoadMembers(string selectedMember)
        {   
            m_pMembers.Items.Clear();
    
            foreach(string member in m_pMailingList.Members){
                ListViewItem it = new ListViewItem(member);
				m_pMembers.Items.Add(it);

                if(selectedMember == member){
                    it.Selected = true;
                }
            }

            m_pMembers_SelectedIndexChanged(this,null);
        }

        #endregion

        #region method LoadAccess

        /// <summary>
        /// Loads mailing list accessing user and groups to UI.
        /// </summary>
        /// <param name="selectedUserOrGroup">Selects specified user or group, if it exists.</param>
        private void LoadAccess(string selectedUserOrGroup)
        {
            m_pAccess.Items.Clear();
    
            foreach(string userOrGroup in m_pMailingList.ACL){
                ListViewItem it = new ListViewItem(userOrGroup);
                if(m_pVirtualServer.Domains.Contains(userOrGroup)){
                    it.ImageIndex = 1;
                }
                else{
                    it.ImageIndex = 0;
                }
				m_pAccess.Items.Add(it);
                    
                if(selectedUserOrGroup == userOrGroup){
                    it.Selected = true;
                }
            }

            m_pAccess_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets active mailing list ID.
        /// </summary>
        public string MailingListID
        {
            get{ 
                if(m_pMailingList != null){
                    return m_pMailingList.ID;
                }
                else{
                    return "";
                }
            }
        }

        #endregion

    }
}
