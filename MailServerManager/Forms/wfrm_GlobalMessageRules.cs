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
    /// Global message rules window.
    /// </summary>
    public class wfrm_GlobalMessageRules : Form
    {
        private ToolStrip m_pToolbar     = null;
        private ImageList m_pRulesImages = null;
        private ListView  m_pRules       = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_GlobalMessageRules(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadRules("");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(472,357);

            ImageList toobarImageList = new ImageList();
            toobarImageList.Images.Add(ResManager.GetIcon("add.ico"));
            toobarImageList.Images.Add(ResManager.GetIcon("edit.ico"));
            toobarImageList.Images.Add(ResManager.GetIcon("delete.ico"));

            m_pToolbar = new ToolStrip();
            m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pToolbar.BackColor = this.BackColor;
            m_pToolbar.Renderer = new ToolBarRendererEx();
            m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pToolbar_ItemClicked);
            // Add button
            ToolStripButton rulesToolbar_button_Add = new ToolStripButton();
            rulesToolbar_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            rulesToolbar_button_Add.Tag = "add";
            m_pToolbar.Items.Add(rulesToolbar_button_Add);
            // Edit button
            ToolStripButton rulesToolbar_button_Edit = new ToolStripButton();
            rulesToolbar_button_Edit.Enabled = false;
            rulesToolbar_button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            rulesToolbar_button_Edit.Tag = "edit";
            m_pToolbar.Items.Add(rulesToolbar_button_Edit);
            // Delete button
            ToolStripButton rulesToolbar_button_Delete = new ToolStripButton();
            rulesToolbar_button_Delete.Enabled = false;
            rulesToolbar_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            rulesToolbar_button_Delete.Tag = "delete";
            m_pToolbar.Items.Add(rulesToolbar_button_Delete);
            // Separator
            m_pToolbar.Items.Add(new ToolStripSeparator());
            // Refresh button
            ToolStripButton button_Refresh = new ToolStripButton();
            button_Refresh.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
            button_Refresh.Tag = "refresh";
            button_Refresh.ToolTipText  = "Refresh";
            m_pToolbar.Items.Add(button_Refresh);
            // Separator
            m_pToolbar.Items.Add(new ToolStripSeparator());
            // Up button
            ToolStripButton rulesToolbar_button_Up = new ToolStripButton();
            rulesToolbar_button_Up.Enabled = false;
            rulesToolbar_button_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            rulesToolbar_button_Up.Tag = "up";
            m_pToolbar.Items.Add(rulesToolbar_button_Up);
            // Down button
            ToolStripButton rulesToolbar_button_down = new ToolStripButton();
            rulesToolbar_button_down.Enabled = false;
            rulesToolbar_button_down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            rulesToolbar_button_down.Tag = "down";
            m_pToolbar.Items.Add(rulesToolbar_button_down);

            m_pRulesImages = new ImageList();
            m_pRulesImages.Images.Add(ResManager.GetIcon("messagerule.ico"));
            m_pRulesImages.Images.Add(ResManager.GetIcon("messagerule_disabled.ico"));
            
            m_pRules = new ListView();
            m_pRules.Size = new Size(445,290);
            m_pRules.Location = new Point(10,20);
            m_pRules.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pRules.View = View.Details;
            m_pRules.FullRowSelect = true;
            m_pRules.HideSelection = false;
            m_pRules.SmallImageList = m_pRulesImages;
            m_pRules.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            m_pRules.DoubleClick += new EventHandler(m_pRules_DoubleClick);
            m_pRules.SelectedIndexChanged += new EventHandler(m_pRules_SelectedIndexChanged);
            m_pRules.MouseUp += new MouseEventHandler(m_pRules_MouseUp);
            m_pRules.Columns.Add("Description",460,HorizontalAlignment.Left);

            this.Controls.Add(m_pRules);
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


        #region method m_pRules_DoubleClick

        private void m_pRules_DoubleClick(object sender, EventArgs e)
        {
            if(m_pRules.SelectedItems.Count > 0){
                GlobalMessageRule rule = (GlobalMessageRule)m_pRules.SelectedItems[0].Tag;
                wfrm_GlobalMessageRule frm = new wfrm_GlobalMessageRule(m_pVirtualServer,rule);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRules(rule.ID);
                }
            }
        }

        #endregion

        #region method m_pRules_SelectedIndexChanged

        private void m_pRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pRules.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
                if(m_pRules.SelectedItems[0].Index > 0){
                    m_pToolbar.Items[6].Enabled = true;
                }
                if(m_pRules.SelectedItems[0].Index < (m_pRules.Items.Count - 1)){
                    m_pToolbar.Items[7].Enabled = true;
                }
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
                m_pToolbar.Items[6].Enabled = false;
                m_pToolbar.Items[7].Enabled = false;
            }
        }

        #endregion

        #region method m_pRules_MouseUp

        private void m_pRules_MouseUp(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pRules_ContextMenuItem_Clicked);                       
            //--- MenuItem Add 
            ToolStripMenuItem menuItem_Add = new ToolStripMenuItem("Add");
            menuItem_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            menuItem_Add.Tag = "add";
            menu.Items.Add(menuItem_Add);
            //--- MenuItem Edit
            ToolStripMenuItem menuItem_Edit = new ToolStripMenuItem("Edit");
            menuItem_Edit.Enabled = m_pRules.SelectedItems.Count > 0;
            menuItem_Edit.Tag = "edit";
            menuItem_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            menu.Items.Add(menuItem_Edit);
            //--- MenuItem Delete
            ToolStripMenuItem menuItem_Delete = new ToolStripMenuItem("Delete");
            menuItem_Delete.Enabled = m_pRules.SelectedItems.Count > 0;
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
            //--- Separator
            menu.Items.Add(new ToolStripSeparator());
            //--- MenuItem Up
            ToolStripMenuItem menuItem_Up = new ToolStripMenuItem("Move Up");
            if(!(m_pRules.SelectedItems.Count > 0 && m_pRules.SelectedItems[0].Index > 0)){
                menuItem_Up.Enabled = false;
            }
            menuItem_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            menuItem_Up.Tag = "up";
            menu.Items.Add(menuItem_Up);
            //--- MenuItem Down
            ToolStripMenuItem menuItem_Down = new ToolStripMenuItem("Move Down");
            if(!(m_pRules.SelectedItems.Count > 0 && m_pRules.SelectedItems[0].Index < (m_pRules.Items.Count - 1))){
                menuItem_Down.Enabled = false;
            }
            menuItem_Down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            menuItem_Down.Tag = "down";
            menu.Items.Add(menuItem_Down);
            //---
            menu.Show(Control.MousePosition);
        }

        #endregion

        #region method m_pRules_ContextMenuItem_Clicked

        private void m_pRules_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
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
                wfrm_GlobalMessageRule frm = new wfrm_GlobalMessageRule(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRules("");
                }
            }
            else if(taskID == "edit"){
                GlobalMessageRule rule = (GlobalMessageRule)m_pRules.SelectedItems[0].Tag;
                wfrm_GlobalMessageRule frm = new wfrm_GlobalMessageRule(m_pVirtualServer,rule);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRules(rule.ID);
                }
            }
            else if(taskID == "delete"){
                GlobalMessageRule rule = (GlobalMessageRule)m_pRules.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete Rule '" + rule.Description + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    rule.Owner.Remove(rule);
                    m_pRules.SelectedItems[0].Remove();
                }
            }
            else if(taskID == "refresh"){                
                LoadRules("");
            }
            else if(taskID == "up"){
                if(m_pRules.SelectedItems.Count > 0 && m_pRules.SelectedItems[0].Index > 0){
                    SwapRules(m_pRules.SelectedItems[0],m_pRules.Items[m_pRules.SelectedItems[0].Index - 1]);
                }
            }
            else if(taskID == "down"){
                if(m_pRules.SelectedItems.Count > 0 && m_pRules.SelectedItems[0].Index < m_pRules.Items.Count - 1){
                    SwapRules(m_pRules.SelectedItems[0],m_pRules.Items[m_pRules.SelectedItems[0].Index + 1]);
                }
            }
        }

        #endregion


        #region method LoadRules

        /// <summary>
        /// Loads rules to UI.
        /// </summary>
        /// <param name="selectedRuleID">Selects specified rule, if rule exists.</param>
        private void LoadRules(string selectedRuleID)
        {
            m_pRules.Items.Clear();
            m_pVirtualServer.GlobalMessageRules.Refresh();
            foreach(GlobalMessageRule gmRule in m_pVirtualServer.GlobalMessageRules){
                ListViewItem it = new ListViewItem();
                // Make disabled rules red and striked out
                if(!gmRule.Enabled){
                    it.ForeColor = Color.Purple;
                    it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                    it.ImageIndex = 1;
                }
                else{
                    it.ImageIndex = 0;
                }
                it.Tag = gmRule;
                it.Text = gmRule.Description;
                m_pRules.Items.Add(it);

                if(gmRule.ID == selectedRuleID){
                    it.Selected = true;
                }
            }

            m_pRules_SelectedIndexChanged(this,new EventArgs());
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
            GlobalMessageRule rule_down = (GlobalMessageRule)item1.Tag;
            GlobalMessageRule rule_up   = (GlobalMessageRule)item2.Tag;

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

            m_pVirtualServer.GlobalMessageRules.Refresh();                        
            LoadRules(selectedID);
        }

        #endregion

    }
}
