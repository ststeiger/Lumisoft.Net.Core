using System;
using System.IO;
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
    /// Filters window.
    /// </summary>
    public class wfrm_Filters : Form
    {
        private ToolStrip m_pToolbar       = null;
        private ImageList m_pFiltersImages = null;
        private ListView  m_pFilters       = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_Filters(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadFilters("");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(472,357);

            m_pToolbar = new ToolStrip();
            m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pToolbar.BackColor = this.BackColor;
            m_pToolbar.Renderer = new ToolBarRendererEx();
            m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pToolbar_ItemClicked);
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Tag = "add";
            m_pToolbar.Items.Add(button_Add);
            // Edit button
            ToolStripButton button_Edit = new ToolStripButton();
            button_Edit.Enabled = false;
            button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            button_Edit.Tag = "edit";
            m_pToolbar.Items.Add(button_Edit);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            m_pToolbar.Items.Add(button_Delete);
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
            ToolStripButton button_Up = new ToolStripButton();
            button_Up.Enabled = false;
            button_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            button_Up.Tag = "up";
            m_pToolbar.Items.Add(button_Up);
            // Down button
            ToolStripButton button_down = new ToolStripButton();
            button_down.Enabled = false;
            button_down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            button_down.Tag = "down";
            m_pToolbar.Items.Add(button_down);

            m_pFiltersImages = new ImageList();
            m_pFiltersImages.Images.Add(ResManager.GetIcon("filter.ico"));
            m_pFiltersImages.Images.Add(ResManager.GetIcon("filter_disabled.ico"));
            
            m_pFilters = new ListView();
            m_pFilters.Size = new Size(445,290);
            m_pFilters.Location = new Point(10,20);
            m_pFilters.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pFilters.View = View.Details;
            m_pFilters.FullRowSelect = true;
            m_pFilters.HideSelection = false;
            m_pFilters.SmallImageList = m_pFiltersImages;
            m_pFilters.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            m_pFilters.DoubleClick += new EventHandler(m_pFilters_DoubleClick);
            m_pFilters.SelectedIndexChanged += new EventHandler(m_pFilters_SelectedIndexChanged);
            m_pFilters.MouseUp += new MouseEventHandler(m_pFilters_MouseUp);
            m_pFilters.Columns.Add("Description",460,HorizontalAlignment.Left);

            this.Controls.Add(m_pFilters);
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


        #region method m_pFilters_DoubleClick

        private void m_pFilters_DoubleClick(object sender,EventArgs e)
        {
            if(m_pFilters.SelectedItems.Count > 0){
                Filter filter = (Filter)m_pFilters.SelectedItems[0].Tag;
                wfrm_Filters_Filter frm = new wfrm_Filters_Filter(m_pVirtualServer,filter);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadFilters(frm.FilterID);
                }
            }
        }

        #endregion

        #region method m_pFilters_SelectedIndexChanged

        private void m_pFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pFilters.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
                if(m_pFilters.SelectedItems[0].Index > 0){
                    m_pToolbar.Items[6].Enabled = true;
                }
                if(m_pFilters.SelectedItems[0].Index < (m_pFilters.Items.Count - 1)){
                    m_pToolbar.Items[7].Enabled = true;
                }
                //m_pToolbar.Items[7].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
                m_pToolbar.Items[6].Enabled = false;
                m_pToolbar.Items[7].Enabled = false;
                //m_pToolbar.Items[7].Enabled = false;
            }
        }

        #endregion

        #region method m_pFilters_MouseUp

        private void m_pFilters_MouseUp(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pFilters_ContextMenuItem_Clicked);                       
            //--- MenuItem Add 
            ToolStripMenuItem menuItem_Add = new ToolStripMenuItem("Add");
            menuItem_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            menuItem_Add.Tag = "add";
            menu.Items.Add(menuItem_Add);
            //--- MenuItem Edit
            ToolStripMenuItem menuItem_Edit = new ToolStripMenuItem("Edit");
            menuItem_Edit.Enabled = m_pFilters.SelectedItems.Count > 0;
            menuItem_Edit.Tag = "edit";
            menuItem_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            menu.Items.Add(menuItem_Edit);
            //--- MenuItem Delete
            ToolStripMenuItem menuItem_Delete = new ToolStripMenuItem("Delete");
            menuItem_Delete.Enabled = m_pFilters.SelectedItems.Count > 0;
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
            if(!(m_pFilters.SelectedItems.Count > 0 && m_pFilters.SelectedItems[0].Index > 0)){
                menuItem_Up.Enabled = false;
            }
            menuItem_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            menuItem_Up.Tag = "up";
            menu.Items.Add(menuItem_Up);
            //--- MenuItem Down
            ToolStripMenuItem menuItem_Down = new ToolStripMenuItem("Move Down");
            if(!(m_pFilters.SelectedItems.Count > 0 && m_pFilters.SelectedItems[0].Index < (m_pFilters.Items.Count - 1))){
                menuItem_Down.Enabled = false;
            }
            menuItem_Down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            menuItem_Down.Tag = "down";
            menu.Items.Add(menuItem_Down);
            //---
            menu.Show(Control.MousePosition);
        }

        #endregion

        #region method m_pFilters_ContextMenuItem_Clicked

        private void m_pFilters_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
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
                wfrm_Filters_Filter frm = new wfrm_Filters_Filter(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadFilters(frm.FilterID);
                }
            }
            else if(taskID == "edit"){
                Filter filter = (Filter)m_pFilters.SelectedItems[0].Tag;
                wfrm_Filters_Filter frm = new wfrm_Filters_Filter(m_pVirtualServer,filter);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadFilters(frm.FilterID);
                }
            }
            else if(taskID == "delete"){
                Filter filter = (Filter)m_pFilters.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete Filter '" + filter.Description + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    filter.Owner.Remove(filter);
                    m_pFilters.SelectedItems[0].Remove();
                }
            }
            else if(taskID == "refresh"){                
                LoadFilters("");
            }
            else if(taskID == "up"){
                if(m_pFilters.SelectedItems.Count > 0 && m_pFilters.SelectedItems[0].Index > 0){
                    SwapFilters(m_pFilters.SelectedItems[0],m_pFilters.Items[m_pFilters.SelectedItems[0].Index - 1]);
                }
            }
            else if(taskID == "down"){
                if(m_pFilters.SelectedItems.Count > 0 && m_pFilters.SelectedItems[0].Index < m_pFilters.Items.Count - 1){
                    SwapFilters(m_pFilters.SelectedItems[0],m_pFilters.Items[m_pFilters.SelectedItems[0].Index + 1]);
                }
            }/*
            else if(taskID == "properties"){
                Form filterUI = GetFilterSettingsUI();
                if(filterUI != null){
                    filterUI.ShowDialog(this);
                }
            }*/
        }

        #endregion


        #region method LoadFilters

        /// <summary>
        /// Loads filters to UI.
        /// </summary>
        /// <param name="selectedFilterID">Selects specified filter, if filter exists.</param>
        private void LoadFilters(string selectedFilterID)
        {
            m_pFilters.Items.Clear();
            m_pVirtualServer.Filters.Refresh();
            foreach(Filter filter in m_pVirtualServer.Filters){
                ListViewItem it = new ListViewItem();
                // Make disabled rules red and striked out
                if(!filter.Enabled){
                    it.ForeColor = Color.Purple;
                    it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                    it.ImageIndex = 1;
                }
                else{
                    it.ImageIndex = 0;
                }
                it.Tag = filter;
                it.Text = filter.Description;
                m_pFilters.Items.Add(it);

                if(filter.ID == selectedFilterID){
                    it.Selected = true;
                }
            }

            m_pFilters_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

        #region method SwapFilters

        /// <summary>
        /// Swaps specified filters.
        /// </summary>
        /// <param name="item1">Item 1.</param>
        /// <param name="item2">Item 2.</param>
        private void SwapFilters(ListViewItem item1,ListViewItem item2)
        {
            Filter filter_down = (Filter)item1.Tag;
            Filter filter_up   = (Filter)item2.Tag;

            string selectedFilterID = "";
            if(item1.Selected){
                selectedFilterID = filter_up.ID;
            }
            else if(item2.Selected){
                selectedFilterID = filter_down.ID;
            }

            bool   up_Enabled      = filter_up.Enabled;
            string up_Description  = filter_up.Description;
            string up_AssemblyName = filter_up.AssemblyName;
            string up_Class        = filter_up.Class;

            filter_up.Enabled      = filter_down.Enabled;
            filter_up.Description  = filter_down.Description;
            filter_up.AssemblyName = filter_down.AssemblyName;
            filter_up.Class        = filter_down.Class;
            filter_up.Commit();

            filter_down.Enabled      = up_Enabled;
            filter_down.Description  = up_Description;
            filter_down.AssemblyName = up_AssemblyName;
            filter_down.Class        = up_Class;
            filter_down.Commit();
                        
            LoadFilters(selectedFilterID);
        }

        #endregion

        #region method GetFilterSettingsUI

        /// <summary>
        /// Gets selectd filter settings UI. Returns null is filter isn't selected or filter doesn't support getting settings UI.
        /// </summary>
        /// <returns>Returns null is filter isn't selected or filter doesn't support getting settings UI.</returns>
        private Form GetFilterSettingsUI()
        {
            try{
                if(m_pFilters.SelectedItems.Count > 0){
                    Filter filter = (Filter)m_pFilters.SelectedItems[0].Tag;

                    string assemblyFile = filter.AssemblyName;
				    // File is without path probably, try to load it from filters folder
				    if(!File.Exists(assemblyFile)){
					    assemblyFile = Application.StartupPath + "\\Filters\\" + assemblyFile;
				    }

                    System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFrom(assemblyFile);
                    Type filterType = ass.GetType(filter.Class);
                    object filterInstance = Activator.CreateInstance(filterType);
                    return ((LumiSoft.MailServer.Filters.ISettingsUI)filterInstance).GetUI();  
                }
            }
            catch{
                // If reaches here, probably filter doesn't implement settings UI. Anyway we can't show UI.
            }

            return null;
        }

        #endregion

    }
}
