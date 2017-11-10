using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Users and Groups Groups Add/Edit group window.
    /// </summary>
    public class wfrm_UsersAndGroups_Group : Form
    {
        //--- Common UI -------------------------------
        private TabControl m_pTab    = null;
        private Button     m_pCancel = null;
        private Button     m_pOk     = null;
        //--- Tabpage General UI ---------------------------------
        private CheckBox  m_pTab_General_Enabled            = null;
        private Label     mt_Tab_General_Name               = null;
        private TextBox   m_pTab_General_Name               = null;
        private Label     mt_Tab_General_Description        = null;
        private TextBox   m_pTab_General_Description        = null;
        private ImageList m_pTab_General_GroupMembersImages = null;
        private Label     mt_Tab_General_GroupMembers       = null;
        private ToolStrip m_pTab_General_Toolbar            = null;
        private ListView  m_pTab_General_GroupMembers       = null;

        private VirtualServer m_pVirtualServer = null;
        private Group         m_pGroup         = null;

        /// <summary>
        /// Add new constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_UsersAndGroups_Group(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="group">Group to update.</param>
        public wfrm_UsersAndGroups_Group(VirtualServer virtualServer,Group group)
        {
            m_pVirtualServer = virtualServer;
            m_pGroup         = group;      

            InitUI();

            m_pTab_General_Name.Text = group.GroupName;
            m_pTab_General_Description.Text = group.Description;
            m_pTab_General_Enabled.Checked = group.Enabled;

            LoadMembers();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,373);
            this.MinimumSize = new Size(400,400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Add/Edit Group";
            this.Icon = ResManager.GetIcon("group.ico");

            #region Common UI

            //--- Common UI -------------------------------------------------------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(393,330);
            m_pTab.Location = new Point(0,5);
            m_pTab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab.TabPages.Add(new TabPage("General"));
            m_pTab.TabPages[0].Size = new Size(387,304);
            
            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(240,345);
            m_pCancel.Text = "Cancel";
            m_pCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(315,345);
            m_pOk.Text = "Ok";
            m_pOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region General UI

            //--- General UI ------------------------------------------------------------------------------//
            m_pTab_General_Enabled = new CheckBox();
            m_pTab_General_Enabled.Size = new Size(100,13);
            m_pTab_General_Enabled.Location = new Point(115,15);
            m_pTab_General_Enabled.Text = "Enabled";
            m_pTab_General_Enabled.Checked = true;

            mt_Tab_General_Name = new Label();
            mt_Tab_General_Name.Size = new Size(100,13);
            mt_Tab_General_Name.Location = new Point(9,35);
            mt_Tab_General_Name.Text = "Name:";

            m_pTab_General_Name = new TextBox();
            m_pTab_General_Name.Size = new Size(265,13);
            m_pTab_General_Name.Location = new Point(115,35);

            mt_Tab_General_Description = new Label();
            mt_Tab_General_Description.Size = new Size(100,13);
            mt_Tab_General_Description.Location = new Point(9,60);
            mt_Tab_General_Description.Text = "Description:";

            m_pTab_General_Description = new TextBox();
            m_pTab_General_Description.Size = new Size(265,13);
            m_pTab_General_Description.Location = new Point(115,60);

            m_pTab_General_GroupMembersImages = new ImageList();
            m_pTab_General_GroupMembersImages.Images.Add(ResManager.GetIcon("group.ico"));
            m_pTab_General_GroupMembersImages.Images.Add(ResManager.GetIcon("user.ico"));

            mt_Tab_General_GroupMembers = new Label();
            mt_Tab_General_GroupMembers.Size = new Size(100,13);
            mt_Tab_General_GroupMembers.Location = new Point(9,90);
            mt_Tab_General_GroupMembers.Text = "Members:";

            m_pTab_General_Toolbar = new ToolStrip();
            m_pTab_General_Toolbar.AutoSize = false;
            m_pTab_General_Toolbar.Size = new Size(100,25);
            m_pTab_General_Toolbar.Location = new Point(330,85);
            m_pTab_General_Toolbar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pTab_General_Toolbar.Dock = DockStyle.None;            
            m_pTab_General_Toolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTab_General_Toolbar.BackColor = this.BackColor;
            m_pTab_General_Toolbar.Renderer = new ToolBarRendererEx();
            m_pTab_General_Toolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_General_Toolbar_ItemClicked);
            // Add button
            ToolStripButton general_button_Add = new ToolStripButton();
            general_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            general_button_Add.Tag = "add";
            m_pTab_General_Toolbar.Items.Add(general_button_Add);
            // Delete button
            ToolStripButton general_button_Delete = new ToolStripButton();
            general_button_Delete.Enabled = false;
            general_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            general_button_Delete.Tag = "delete";
            m_pTab_General_Toolbar.Items.Add(general_button_Delete);

            m_pTab_General_GroupMembers = new ListView();
            m_pTab_General_GroupMembers.Size = new Size(370,180);
            m_pTab_General_GroupMembers.Location = new Point(9,110);
            m_pTab_General_GroupMembers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab_General_GroupMembers.View = View.Details;
            m_pTab_General_GroupMembers.HideSelection = false;
            m_pTab_General_GroupMembers.FullRowSelect = true;
            m_pTab_General_GroupMembers.SmallImageList = m_pTab_General_GroupMembersImages;
            m_pTab_General_GroupMembers.SelectedIndexChanged += new EventHandler(m_pTab_General_GroupMembers_SelectedIndexChanged);
            m_pTab_General_GroupMembers.Columns.Add("",345,HorizontalAlignment.Left);
             
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Enabled);           
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Name);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Name);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Description);            
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Description);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_GroupMembers);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Toolbar);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_GroupMembers);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Enabled);
            //---------------------------------------------------------------------------------------------//

            #endregion
        }
                
        #endregion


        #region Events Handling

        #region method m_pTab_General_Toolbar_ItemClicked

        private void m_pTab_General_Toolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                //--- Validate values ---------------------------//
                if(m_pTab_General_Name.Text == ""){
                    MessageBox.Show(this,"Please fill Group name !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //----------------------------------------------//

                // This is new not yet added group, add it
                if(m_pGroup == null){
                    m_pGroup = m_pVirtualServer.Groups.Add(m_pTab_General_Name.Text,m_pTab_General_Description.Text,m_pTab_General_Enabled.Checked);
                }

                //--- Exclude existing members + group itself ---//
                List<string> excludeList = new List<string>();
                excludeList.Add(m_pTab_General_Name.Text.ToLower());
                foreach(ListViewItem it in m_pTab_General_GroupMembers.Items){
                    excludeList.Add(it.Text.ToLower());
                }
                //-----------------------------------------------//

                wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pVirtualServer,true,false,excludeList);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    m_pGroup.Members.Add(frm.SelectedUserOrGroup);

                    m_pTab_General_GroupMembers.SelectedItems.Clear();
                    ListViewItem it = new ListViewItem(frm.SelectedUserOrGroup);
                    it.Selected = true;
                    // Member is group
                    if(m_pVirtualServer.Groups.Contains(frm.SelectedUserOrGroup)){
                        it.ImageIndex = 0;
                    }
                    // Member is user
                    else{
                        it.ImageIndex = 1;
                    }
                    m_pTab_General_GroupMembers.Items.Add(it);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                m_pGroup.Members.Remove(m_pTab_General_GroupMembers.SelectedItems[0].Text);
                m_pTab_General_GroupMembers.SelectedItems[0].Remove();
            }
        }

        #endregion

        #region method m_pTab_General_GroupMembers_SelectedIndexChanged

        private void m_pTab_General_GroupMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pTab_General_GroupMembers.SelectedItems.Count > 0){
                m_pTab_General_Toolbar.Items[1].Enabled = true;
            }
            else{
                m_pTab_General_Toolbar.Items[1].Enabled = false;
            }
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
            //--- Validate values ---------------------------//
            if(m_pTab_General_Name.Text == ""){
                MessageBox.Show(this,"Please fill Group name !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //----------------------------------------------//

            // Add new group
            if(m_pGroup == null){
                m_pGroup = m_pVirtualServer.Groups.Add(m_pTab_General_Name.Text,m_pTab_General_Description.Text,m_pTab_General_Enabled.Checked);
            }
            // Update group
            else{
                m_pGroup.GroupName   = m_pTab_General_Name.Text;
                m_pGroup.Description = m_pTab_General_Description.Text;
                m_pGroup.Enabled     = m_pTab_General_Enabled.Checked; 
                m_pGroup.Commit();
            }

            this.DialogResult = DialogResult.OK;
        }

        #endregion

        #endregion


        #region mehtod LoadMembers

        /// <summary>
        /// Loads user group members to UI.
        /// </summary>
        private void LoadMembers()
        {
            foreach(string member in m_pGroup.Members){
                // Member is group
                if(m_pGroup.Owner.Contains(member)){
                    m_pTab_General_GroupMembers.Items.Add(member,0);
                }
                // Member is user
                else{
                    m_pTab_General_GroupMembers.Items.Add(member,1);
                }
            }

            m_pTab_General_GroupMembers_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets active group ID.
        /// </summary>
        public string GroupID
        {
            get{ 
                if(m_pGroup != null){
                    return m_pGroup.GroupID;
                }
                else{
                    return ""; 
                }
            }
        }

        #endregion

    }
}
