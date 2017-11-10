using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Mailing lists window.
    /// </summary>
    public class wfrm_MailingLists : Form
    {
        private ToolStrip m_pToolbar            = null;
        private Label     mt_Filter             = null;
        private TextBox   m_pFilter             = null;
        private Button    m_pGetMailingLists    = null;
        private ImageList m_pMailingListsImages = null;
        private WListView m_pMailingLists       = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_MailingLists(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadMailingLists("");
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
            m_pFilter.Size = new Size(150,13);
            m_pFilter.Location = new Point(115,20);
            m_pFilter.Text = "*";

            m_pGetMailingLists = new Button();            
            m_pGetMailingLists.Size = new Size(70,20);
            m_pGetMailingLists.Location = new Point(280,20);
            m_pGetMailingLists.Text = "Get";
            m_pGetMailingLists.Click += new EventHandler(m_pGetMailingLists_Click);

            m_pMailingListsImages = new ImageList();
            m_pMailingListsImages.Images.Add(ResManager.GetIcon("mailinglist.ico"));
            m_pMailingListsImages.Images.Add(ResManager.GetIcon("mailinglist_disabled.ico"));

            m_pMailingLists = new WListView();
            m_pMailingLists.Size = new Size(270,200);
            m_pMailingLists.Location = new Point(10,50);
            m_pMailingLists.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pMailingLists.View = View.Details;
            m_pMailingLists.FullRowSelect = true;
            m_pMailingLists.HideSelection = false;
            m_pMailingLists.SmallImageList = m_pMailingListsImages;
            m_pMailingLists.SelectedIndexChanged += new EventHandler(m_pMailingLists_SelectedIndexChanged);
            m_pMailingLists.DoubleClick += new EventHandler(m_pMailingLists_DoubleClick);
            m_pMailingLists.MouseUp += new MouseEventHandler(m_pMailingLists_MouseUp);
            m_pMailingLists.Columns.Add("Name",190,HorizontalAlignment.Left);
            m_pMailingLists.Columns.Add("Description",290,HorizontalAlignment.Left);

            this.Controls.Add(m_pToolbar);
            this.Controls.Add(mt_Filter);
            this.Controls.Add(m_pFilter);
            this.Controls.Add(m_pGetMailingLists);
            this.Controls.Add(m_pMailingLists);
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


        #region method m_pGetMailingLists_Click

        private void m_pGetMailingLists_Click(object sender, EventArgs e)
        {
            LoadMailingLists("");
        }

        #endregion

        #region method m_pMailingLists_DoubleClick

        private void m_pMailingLists_DoubleClick(object sender, EventArgs e)
        {
            if(m_pMailingLists.SelectedItems.Count > 0){
                MailingList mailingList = (MailingList)m_pMailingLists.SelectedItems[0].Tag;
                wfrm_MailingList frm = new wfrm_MailingList(m_pVirtualServer,mailingList);
				if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadMailingLists(frm.MailingListID);
                }
            }
        }

        #endregion

        #region method m_pMailingLists_SelectedIndexChanged

        private void m_pMailingLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pMailingLists.Items.Count > 0 && m_pMailingLists.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
            }
        }

        #endregion

        #region method m_pMailingLists_MouseUp

        private void m_pMailingLists_MouseUp(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pMailingLists_ContextMenuItem_Clicked);                       
            //--- MenuItem Add 
            ToolStripMenuItem menuItem_Add = new ToolStripMenuItem("Add");
            menuItem_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            menuItem_Add.Tag = "add";
            menu.Items.Add(menuItem_Add);
            //--- MenuItem Edit
            ToolStripMenuItem menuItem_Edit = new ToolStripMenuItem("Edit");
            menuItem_Edit.Enabled = m_pMailingLists.SelectedItems.Count > 0;
            menuItem_Edit.Tag = "edit";
            menuItem_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            menu.Items.Add(menuItem_Edit);
            //--- MenuItem Delete
            ToolStripMenuItem menuItem_Delete = new ToolStripMenuItem("Delete");
            menuItem_Delete.Enabled = m_pMailingLists.SelectedItems.Count > 0;
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

        #region method m_pMailingLists_ContextMenuItem_Clicked

        private void m_pMailingLists_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
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
                wfrm_MailingList frm = new wfrm_MailingList(m_pVirtualServer);
				if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadMailingLists(frm.MailingListID);
                }
            }
            else if(taskID == "edit"){
                MailingList mailingList = (MailingList)m_pMailingLists.SelectedItems[0].Tag;
                wfrm_MailingList frm = new wfrm_MailingList(m_pVirtualServer,mailingList);
				if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadMailingLists(frm.MailingListID);
                }
            }
            else if(taskID == "delete"){
                MailingList mailingList = (MailingList)m_pMailingLists.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete Mailing List '" + mailingList.Name + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    mailingList.Owner.Remove(mailingList);
                    m_pMailingLists.SelectedItems[0].Remove();
                }
            }
            else if(taskID == "refresh"){                
                LoadMailingLists("");
            }
        }

        #endregion


        #region method LoadMailingLists

        /// <summary>
        /// Load mailing lists to UI.
        /// </summary>
        /// <param name="selectedMailingListID">Selects specified mailing list if mailing list exists.</param>
        private void LoadMailingLists(string selectedMailingListID)
        {
            m_pMailingLists.Items.Clear();
            m_pVirtualServer.MailingLists.Refresh();
            foreach(MailingList mailingList in m_pVirtualServer.MailingLists){
                if(m_pFilter.Text == "" || IsAstericMatch(m_pFilter.Text,mailingList.Name.ToLower())){
                    ListViewItem it = new ListViewItem();
                    it.ImageIndex = 0;
                    // Make disabled red and striked out
                    if(mailingList.Enabled){
                        it.ImageIndex = 0;
                    }
                    else{
                        it.ForeColor = Color.Purple;
                        it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                        it.ImageIndex = 1;
                    }
                    it.Tag = mailingList;
                    it.Text = mailingList.Name;
                    it.SubItems.Add(mailingList.Description);
                    m_pMailingLists.Items.Add(it);

                    if(mailingList.ID == selectedMailingListID){
                        it.Selected = true;
                    }
                }
            }

            m_pMailingLists.SortItems();
            m_pMailingLists_SelectedIndexChanged(this,new EventArgs());
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
