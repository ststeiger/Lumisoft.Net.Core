using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Users and Groups Groups window.
    /// </summary>
    public class wfrm_UsersAndGroups_Groups : Form
    {
        private ToolStrip m_pToolbar      = null;
        private Label     mt_Filter       = null;
        private TextBox   m_pFilter       = null;
        private Button    m_pGetGroups    = null;
        private ImageList m_pGroupsImages = null;
        private WListView m_pGroups       = null;

        private VirtualServer m_pVirtualServer = null;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_UsersAndGroups_Groups(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadGroups("");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(300,300);

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

            mt_Filter = new Label();
            mt_Filter.Size = new Size(100,20);
            mt_Filter.Location = new Point(9,20);
            mt_Filter.Text = "Filter:";
            mt_Filter.TextAlign = ContentAlignment.MiddleRight;

            m_pFilter = new TextBox();
            m_pFilter.Size = new Size(150,20);
            m_pFilter.Location = new Point(115,20);
            m_pFilter.Text = "*";

            m_pGetGroups = new Button();            
            m_pGetGroups.Size = new Size(70,20);
            m_pGetGroups.Location = new Point(280,20);
            m_pGetGroups.Text = "Get";
            m_pGetGroups.Click += new EventHandler(m_pGetGroups_Click);

            m_pGroupsImages = new ImageList();
            m_pGroupsImages.Images.Add(ResManager.GetIcon("group.ico"));
            m_pGroupsImages.Images.Add(ResManager.GetIcon("group_disabled.ico"));

            m_pGroups = new WListView();
            m_pGroups.Size = new Size(270,200);
            m_pGroups.Location = new Point(10,50);
            m_pGroups.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pGroups.View = View.Details;
            m_pGroups.FullRowSelect = true;
            m_pGroups.HideSelection = false;
            m_pGroups.SmallImageList = m_pGroupsImages;
            m_pGroups.SelectedIndexChanged += new EventHandler(m_pGroups_SelectedIndexChanged);
            m_pGroups.DoubleClick += new EventHandler(m_pGroups_DoubleClick);
            m_pGroups.MouseUp += new MouseEventHandler(m_pGroups_MouseUp);
            m_pGroups.Columns.Add("Name",190,HorizontalAlignment.Left);
            m_pGroups.Columns.Add("Description",290,HorizontalAlignment.Left);

            this.Controls.Add(m_pToolbar);
            this.Controls.Add(mt_Filter);
            this.Controls.Add(m_pFilter);
            this.Controls.Add(m_pGetGroups);
            this.Controls.Add(m_pGroups);
        }
                                                                                
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            SwitchToolBarTask(e.ClickedItem.Tag.ToString());
        }

        #endregion


        #region method m_pGetGroups_Click

        private void m_pGetGroups_Click(object sender, EventArgs e)
        {
            LoadGroups("");
        }

        #endregion
                
        #region method m_pGroups_DoubleClick

        private void m_pGroups_DoubleClick(object sender,EventArgs e)
        {
            if(m_pGroups.SelectedItems.Count > 0){
                Group group = (Group)m_pGroups.SelectedItems[0].Tag;                
                wfrm_UsersAndGroups_Group frm = new wfrm_UsersAndGroups_Group(m_pVirtualServer,group);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadGroups(frm.GroupID);
                }
            }
        }

        #endregion

        #region method m_pGroups_SelectedIndexChanged

        private void m_pGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pGroups.Items.Count > 0 && m_pGroups.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
            }
        }

        #endregion

        #region method m_pGroups_MouseUp

        private void m_pGroups_MouseUp(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pGroups_ContextMenuItem_Clicked);                       
            //--- MenuItem Add 
            ToolStripMenuItem menuItem_Add = new ToolStripMenuItem("Add");
            menuItem_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            menuItem_Add.Tag = "add";
            menu.Items.Add(menuItem_Add);
            //--- MenuItem Edit
            ToolStripMenuItem menuItem_Edit = new ToolStripMenuItem("Edit");
            menuItem_Edit.Enabled = m_pGroups.SelectedItems.Count > 0;
            menuItem_Edit.Tag = "edit";
            menuItem_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            menu.Items.Add(menuItem_Edit);
            //--- MenuItem Delete
            ToolStripMenuItem menuItem_Delete = new ToolStripMenuItem("Delete");
            menuItem_Delete.Enabled = m_pGroups.SelectedItems.Count > 0;
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

        #region method m_pGroups_ContextMenuItem_Clicked

        private void m_pGroups_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
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
                wfrm_UsersAndGroups_Group frm = new wfrm_UsersAndGroups_Group(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadGroups("");
                }
            }
            else if(taskID == "edit"){
                Group group = (Group)m_pGroups.SelectedItems[0].Tag;                
                wfrm_UsersAndGroups_Group frm = new wfrm_UsersAndGroups_Group(m_pVirtualServer,group);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadGroups(frm.GroupID);
                }
            }
            else if(taskID == "delete"){
                Group group = (Group)m_pGroups.SelectedItems[0].Tag; 
                if(MessageBox.Show(this,"Are you sure you want to delete Group '" + group.GroupName + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    group.Owner.Remove(group);
                    LoadGroups("");
                }
            }
            else if(taskID == "refresh"){                
                LoadGroups("");
            }
        }

        #endregion


        #region method LoadGroups

        /// <summary>
        /// Loads groups to UI.
        /// </summary>
        /// <param name="selectedGroupID">Selects specified gruop, if exists.</param>
        private void LoadGroups(string selectedGroupID)
        {
            m_pGroups.Items.Clear();
            m_pVirtualServer.Groups.Refresh();
            foreach(Group group in m_pVirtualServer.Groups){
                if(m_pFilter.Text == "" || IsAstericMatch(m_pFilter.Text,group.GroupName.ToLower())){
                    ListViewItem it = new ListViewItem();
                    // Make disabled rules red and striked out
                    if(group.Enabled){
                        it.ImageIndex = 0;
                    }
                    else{
                        it.ForeColor = Color.Purple;
                        it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                        it.ImageIndex = 1;
                    }
                    it.Tag = group;
                    it.Text = group.GroupName;
                    it.SubItems.Add(group.Description);
                    m_pGroups.Items.Add(it);

                    if(group.GroupID == selectedGroupID){
                        it.Selected = true;
                    }
                }
            }

            m_pGroups.SortItems();
            m_pGroups_SelectedIndexChanged(this,new EventArgs());
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

    }
}
