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
    /// User or Group selector window.
    /// </summary>
    public class wfrm_se_UserOrGroup : Form
    {
        private Label     mt_Filter               = null;
        private TextBox   m_pFilter               = null;
        private ImageList m_pUsersAndGroupsImages = null;
        private ListView  m_pUsersAndGroups       = null;
        private GroupBox  m_pGroupBox1            = null;
        private Button    m_pCancel               = null;
        private Button    m_pOk                   = null;

        private VirtualServer m_pVirtualServer = null;
        private bool          m_ShowGroups     = true;
        private bool          m_ShowAnyOne     = false;
        private bool          m_ShowAuthUsers  = false;
        private List<string>  m_pExcludeList   = null;
        private string        m_UserOrGroup    = null;
        private bool          m_IsGruop        = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="showAnyOne">Specifies if user groups is shown.</param>
        /// <param name="showAnyOne">Specifies if built-in user 'anyone'is shown.</param>
        public wfrm_se_UserOrGroup(VirtualServer virtualServer,bool showGroups,bool showAnyOne) : this(virtualServer,showGroups,showAnyOne,false,new List<string>())
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="showAnyOne">Specifies if user groups is shown.</param>
        /// <param name="showAnyOne">Specifies if built-in user 'anyone'is shown.</param>
        /// <param name="excludeList">Specifies user and groups what are excluded from list. Items must be in lowecase.</param>
        public wfrm_se_UserOrGroup(VirtualServer virtualServer,bool showGroups,bool showAnyOne,List<string> excludeList) : this(virtualServer,showGroups,showAnyOne,false,excludeList)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="showAnyOne">Specifies if user groups is shown.</param>
        /// <param name="showAnyOne">Specifies if built-in user 'anyone'is shown.</param>
        /// <param name="showAuthUsers">Specifies if built-in group 'Authenticated Users'is shown.</param>
        /// <param name="excludeList">Specifies user and groups what are excluded from list. Items must be in lowecase.</param>
        public wfrm_se_UserOrGroup(VirtualServer virtualServer,bool showGroups,bool showAnyOne,bool showAuthUsers,List<string> excludeList)
        {
            m_pVirtualServer = virtualServer;
            m_ShowGroups     = showGroups;
            m_ShowAnyOne     = showAnyOne;
            m_ShowAuthUsers  = showAuthUsers;
            m_pExcludeList   = excludeList;

            InitUI();

            LoadUsersAndGroups();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,373);
            this.StartPosition = FormStartPosition.CenterScreen;
            if(m_ShowGroups){
                this.Text = "Select User or Group";
            }
            else{
                this.Text = "Select User";
            }

            mt_Filter = new Label();
            mt_Filter.Size = new Size(200,13);
            mt_Filter.Location = new Point(10,10);
            mt_Filter.Text = "Filter:";

            m_pFilter = new TextBox();
            m_pFilter.Size = new Size(200,20);
            m_pFilter.Location = new Point(10,25);
            m_pFilter.TextChanged += new EventHandler(m_pFilter_TextChanged);

            m_pUsersAndGroupsImages = new ImageList();
            m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("group.ico"));
            m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("group_disabled.ico"));
            m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("user.ico"));
            m_pUsersAndGroupsImages.Images.Add(ResManager.GetIcon("user_disabled.ico"));

            m_pUsersAndGroups = new ListView();
            m_pUsersAndGroups.Size = new Size(370,270);
            m_pUsersAndGroups.Location = new Point(10,50);
            m_pUsersAndGroups.View = View.Details;
            m_pUsersAndGroups.HideSelection = false;
            m_pUsersAndGroups.FullRowSelect = true;
            m_pUsersAndGroups.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pUsersAndGroups.SmallImageList = m_pUsersAndGroupsImages;
            m_pUsersAndGroups.DoubleClick += new EventHandler(m_pUsersAndGroups_DoubleClick);
            m_pUsersAndGroups.Columns.Add("User Name:",200,HorizontalAlignment.Left);

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

            this.Controls.Add(mt_Filter);
            this.Controls.Add(m_pFilter);
            this.Controls.Add(m_pUsersAndGroups);
            this.Controls.Add(m_pGroupBox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                                
        #endregion


        #region Events Handling

        #region method m_pFilter_TextChanged

        private void m_pFilter_TextChanged(object sender, EventArgs e)
        {
            LoadUsersAndGroups();
        }

        #endregion

        #region method m_pUsers_DoubleClick

        private void m_pUsersAndGroups_DoubleClick(object sender, EventArgs e)
        {
            if(m_pUsersAndGroups.SelectedItems.Count > 0){
                m_UserOrGroup = m_pUsersAndGroups.SelectedItems[0].Text;
                m_IsGruop     = m_pUsersAndGroups.SelectedItems[0].ImageIndex == 0;

                this.DialogResult = DialogResult.OK;
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
            if(m_pUsersAndGroups.SelectedItems.Count > 0){
                m_UserOrGroup = m_pUsersAndGroups.SelectedItems[0].Text;
                m_IsGruop     = m_pUsersAndGroups.SelectedItems[0].ImageIndex == 0;

                this.DialogResult = DialogResult.OK;
            }
            else{
                this.DialogResult = DialogResult.Cancel;
            }
        }

        #endregion

        #endregion

                
        #region method LoadUsersAndGroups

        /// <summary>
        /// Loads users and groups to UI.
        /// </summary>
        private void LoadUsersAndGroups()
        {
            m_pUsersAndGroups.Items.Clear();

            if(m_ShowGroups){
                foreach(Group group in m_pVirtualServer.Groups){
                    if(!m_pExcludeList.Contains(group.GroupName.ToLower()) && (m_pFilter.Text == "" || IsAstericMatch(m_pFilter.Text,group.GroupName.ToLower()))){
                        if(group.Enabled){
                            m_pUsersAndGroups.Items.Add(group.GroupName,0);
                        }
                        else{
                            m_pUsersAndGroups.Items.Add(group.GroupName,1);
                        }
                    }
                }

                if(m_ShowAuthUsers){
                    m_pUsersAndGroups.Items.Add("Authenticated Users",0);
                }
            }

            if(m_ShowAnyOne){
                m_pUsersAndGroups.Items.Add("anyone",2);
            }            
            foreach(User user in m_pVirtualServer.Users){
                if(!m_pExcludeList.Contains(user.UserName.ToLower()) && (m_pFilter.Text == "" || IsAstericMatch(m_pFilter.Text,user.UserName.ToLower()))){
                    if(user.Enabled){
                        m_pUsersAndGroups.Items.Add(user.UserName,2);
                    }
                    else{
                        m_pUsersAndGroups.Items.Add(user.UserName,3);
                    }
                }
            }
        }

        #endregion


        #region method IsAstericMatch

        /// <summary>
		/// Checks if text matches to search pattern.
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private bool IsAstericMatch(string pattern,string text)
		{
            pattern = pattern.ToLower();
			text = text.ToLower();

			if(pattern == ""){
				pattern = "*";
			}

			while(pattern.Length > 0){
				// *xxx[*xxx...]
				if(pattern.StartsWith("*")){
					// *xxx*xxx
					if(pattern.IndexOf("*",1) > -1){
						string indexOfPart = pattern.Substring(1,pattern.IndexOf("*",1) - 1);
						if(text.IndexOf(indexOfPart) == -1){
							return false;
						}

                        text = text.Substring(text.IndexOf(indexOfPart) + indexOfPart.Length);
                        pattern = pattern.Substring(pattern.IndexOf("*", 1));
					}
					// *xxx   This is last pattern	
					else{				
						return text.EndsWith(pattern.Substring(1));
					}
				}
				// xxx*[xxx...]
				else if(pattern.IndexOfAny(new char[]{'*'}) > -1){
					string startPart = pattern.Substring(0,pattern.IndexOfAny(new char[]{'*'}));
		
					// Text must startwith
					if(!text.StartsWith(startPart)){
						return false;
					}

					text = text.Substring(text.IndexOf(startPart) + startPart.Length);
					pattern = pattern.Substring(pattern.IndexOfAny(new char[]{'*'}));
				}
				// xxx
				else{
					return text == pattern;
				}
			}

            return true;
		}

		#endregion


        #region Properties Implementation

        /// <summary>
        /// Gets selected user or group name. Returns null if not selected.
        /// </summary>
        public string SelectedUserOrGroup
        {
            get{ return m_UserOrGroup; }
        }

        /// <summary>
        /// Gets if selected item is group.
        /// </summary>
        public bool IsGroup
        {
            get{ return m_IsGruop; }
        }

        #endregion

    }
}
