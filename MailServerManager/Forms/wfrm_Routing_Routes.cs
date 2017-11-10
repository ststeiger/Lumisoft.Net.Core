using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Routing routes window.
    /// </summary>
    public class wfrm_Routing_Routes : Form
    {
        private ToolStrip m_pToolbar      = null;
        private ImageList m_pRoutesImages = null;
        private ListView  m_pRoutes       = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_Routing_Routes(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadRoutes("");
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

            m_pRoutesImages = new ImageList();
            m_pRoutesImages.Images.Add(ResManager.GetIcon("filter.ico"));
            m_pRoutesImages.Images.Add(ResManager.GetIcon("filter_disabled.ico"));
            
            m_pRoutes = new ListView();
            m_pRoutes.Size = new Size(445,290);
            m_pRoutes.Location = new Point(10,20);
            m_pRoutes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pRoutes.View = View.Details;
            m_pRoutes.FullRowSelect = true;
            m_pRoutes.HideSelection = false;
            m_pRoutes.SmallImageList = m_pRoutesImages;
            m_pRoutes.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            m_pRoutes.DoubleClick += new EventHandler(m_pRoutes_DoubleClick);
            m_pRoutes.SelectedIndexChanged += new EventHandler(m_pRoutes_SelectedIndexChanged);
            m_pRoutes.MouseUp += new MouseEventHandler(m_pRoutes_MouseUp);
            m_pRoutes.Columns.Add("Pattern",190,HorizontalAlignment.Left);
            m_pRoutes.Columns.Add("Description",280,HorizontalAlignment.Left);

            this.Controls.Add(m_pRoutes);
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


        #region method m_pRoutes_DoubleClick

        private void m_pRoutes_DoubleClick(object sender, EventArgs e)
        {
            if(m_pRoutes.SelectedItems.Count > 0){
                Route route = (Route)m_pRoutes.SelectedItems[0].Tag;
                wfrm_Routing_Route frm = new wfrm_Routing_Route(m_pVirtualServer,route);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRoutes(route.ID);
                }
            }
        }

        #endregion

        #region method m_pRoutes_SelectedIndexChanged

        private void m_pRoutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pRoutes.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
                if(m_pRoutes.SelectedItems[0].Index > 0){
                    m_pToolbar.Items[6].Enabled = true;
                }
                if(m_pRoutes.SelectedItems[0].Index < (m_pRoutes.Items.Count - 1)){
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

        #region method m_pRoutes_MouseUp

        private void m_pRoutes_MouseUp(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pRoutes_ContextMenuItem_Clicked);                       
            //--- MenuItem Add 
            ToolStripMenuItem menuItem_Add = new ToolStripMenuItem("Add");
            menuItem_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            menuItem_Add.Tag = "add";
            menu.Items.Add(menuItem_Add);
            //--- MenuItem Edit
            ToolStripMenuItem menuItem_Edit = new ToolStripMenuItem("Edit");
            menuItem_Edit.Enabled = m_pRoutes.SelectedItems.Count > 0;
            menuItem_Edit.Tag = "edit";
            menuItem_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            menu.Items.Add(menuItem_Edit);
            //--- MenuItem Delete
            ToolStripMenuItem menuItem_Delete = new ToolStripMenuItem("Delete");
            menuItem_Delete.Enabled = m_pRoutes.SelectedItems.Count > 0;
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
            if(!(m_pRoutes.SelectedItems.Count > 0 && m_pRoutes.SelectedItems[0].Index > 0)){
                menuItem_Up.Enabled = false;
            }
            menuItem_Up.Image = ResManager.GetIcon("up.ico").ToBitmap();
            menuItem_Up.Tag = "up";
            menu.Items.Add(menuItem_Up);
            //--- MenuItem Down
            ToolStripMenuItem menuItem_Down = new ToolStripMenuItem("Move Down");
            if(!(m_pRoutes.SelectedItems.Count > 0 && m_pRoutes.SelectedItems[0].Index < (m_pRoutes.Items.Count - 1))){
                menuItem_Down.Enabled = false;
            }
            menuItem_Down.Image = ResManager.GetIcon("down.ico").ToBitmap();
            menuItem_Down.Tag = "down";
            menu.Items.Add(menuItem_Down);
            //---
            menu.Show(Control.MousePosition);
        }

        #endregion

        #region method m_pRoutes_ContextMenuItem_Clicked

        private void m_pRoutes_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
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
                wfrm_Routing_Route frm = new wfrm_Routing_Route(m_pVirtualServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRoutes(frm.RouteID);
                }
            }
            else if(taskID == "edit"){
                Route route = (Route)m_pRoutes.SelectedItems[0].Tag;
                wfrm_Routing_Route frm = new wfrm_Routing_Route(m_pVirtualServer,route);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadRoutes(route.ID);
                }
            }
            else if(taskID == "delete"){
                Route route = (Route)m_pRoutes.SelectedItems[0].Tag;
                if(MessageBox.Show(this,"Are you sure you want to delete Route '" + route.Pattern + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    route.Owner.Remove(route);
                    m_pRoutes.SelectedItems[0].Remove();
                }
            }
            else if(taskID == "refresh"){                
                LoadRoutes("");
            }
            else if(taskID == "up"){
                if(m_pRoutes.SelectedItems.Count > 0 && m_pRoutes.SelectedItems[0].Index > 0){
                    SwapRoutes(m_pRoutes.SelectedItems[0],m_pRoutes.Items[m_pRoutes.SelectedItems[0].Index - 1]);
                }
            }
            else if(taskID == "down"){
                if(m_pRoutes.SelectedItems.Count > 0 && m_pRoutes.SelectedItems[0].Index < m_pRoutes.Items.Count - 1){
                    SwapRoutes(m_pRoutes.SelectedItems[0],m_pRoutes.Items[m_pRoutes.SelectedItems[0].Index + 1]);
                }
            }
        }

        #endregion


        #region method LoadRoutes

        /// <summary>
        /// Loads routes to UI.
        /// </summary>
        /// <param name="selectedRouteID">Selects specified route, if route exists.</param>
        private void LoadRoutes(string selectedRouteID)
        {
            m_pRoutes.Items.Clear();
            m_pVirtualServer.Routes.Refresh();
            foreach(Route route in m_pVirtualServer.Routes){
                ListViewItem it = new ListViewItem();
                // Make disabled rules red and striked out
                if(!route.Enabled){
                    it.ForeColor = Color.Purple;
                    it.Font = new Font(it.Font.FontFamily,it.Font.Size,FontStyle.Strikeout);
                    it.ImageIndex = 1;
                }
                else{
                    it.ImageIndex = 0;
                }
                it.Tag = route;
                it.Text = route.Pattern;
                it.SubItems.Add(route.Description);
                m_pRoutes.Items.Add(it);
                
                if(route.ID == selectedRouteID){
                    it.Selected = true;
                }
            }

            m_pRoutes_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

        #region method SwapRules

        /// <summary>
        /// Swaps specified routes.
        /// </summary>
        /// <param name="item1">Item 1.</param>
        /// <param name="item2">Item 2.</param>
        private void SwapRoutes(ListViewItem item1,ListViewItem item2)
        {/*
            DataRowView drV_Down = (DataRowView)item1.Tag;
            DataRowView drV_Up   = (DataRowView)item2.Tag;
                        
            m_pServerAPI.UpdateRoute(                    
                drV_Down["RouteID"].ToString(),
                (long)drV_Up["Cost"],
                (bool)drV_Down["Enabled"],
                drV_Down["Description"].ToString(),
                drV_Down["Pattern"].ToString(),
                (RouteAction_enum)Convert.ToInt32(drV_Down["Action"]),
                (byte[])drV_Down["ActionData"]
            );

            m_pServerAPI.UpdateRoute(                    
                drV_Up["RouteID"].ToString(),
                (long)drV_Down["Cost"],
                (bool)drV_Up["Enabled"],
                drV_Up["Description"].ToString(),
                drV_Up["Pattern"].ToString(),
                (RouteAction_enum)Convert.ToInt32(drV_Up["Action"]),
                (byte[])drV_Up["ActionData"]
            );

            string selectedRouteID = "";
            if(item1.Selected){
                selectedRouteID = drV_Up["RouteID"].ToString();
            }
            else if(item2.Selected){
                selectedRouteID = drV_Down["RouteID"].ToString();
            }
            LoadRoutes(selectedRouteID);*/
        }

        #endregion

    }
}
