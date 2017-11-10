using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Virtual servers window.
    /// </summary>
    public class wfrm_VirtualServers : Form
    {
        private ToolStrip m_pToolbar = null;
        private WListView m_pServers = null;

        private wfrm_Main m_pFrmMain            = null;
        private TreeNode  m_pVirtualServersNode = null;
        private Server    m_pServer             = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mainFrm">Reference to main UI window.</param>
        /// <param name="virtualServersNode">Reference to owner virtual servers tree node.</param>
        /// <param name="server">Mail server.</param>
        /// <param name="frame"></param>
        public wfrm_VirtualServers(wfrm_Main mainFrm,TreeNode virtualServersNode,Server server,WFrame frame)
        {
            m_pFrmMain            = mainFrm;
            m_pVirtualServersNode = virtualServersNode;
            m_pServer             = server;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadVirtualServers();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(450,300);

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

            ImageList imgListServers = new ImageList();
            imgListServers.Images.Add(ResManager.GetIcon("server_running.ico"));
            imgListServers.Images.Add(ResManager.GetIcon("server_stopped.ico"));

            m_pServers = new WListView();
            m_pServers.Size = new Size(425,210);
            m_pServers.Location = new Point(9,47);
            m_pServers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pServers.View = View.Details;
            m_pServers.FullRowSelect = true;
            m_pServers.HideSelection = false;
            m_pServers.SmallImageList = imgListServers;
            m_pServers.SelectedIndexChanged += new EventHandler(m_pServers_SelectedIndexChanged);
            m_pServers.DoubleClick += new EventHandler(m_pServers_DoubleClick);
            m_pServers.Columns.Add("Name",400,HorizontalAlignment.Left);

            this.Controls.Add(m_pServers);
        }
                                                                
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){
                wfrm_VirtualServers_VirtualServer frm = new wfrm_VirtualServers_VirtualServer(m_pServer);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadVirtualServers();

                    // Refresh Tree "Virtual Servers" node.
                    m_pFrmMain.LoadVirtualServers(m_pVirtualServersNode,m_pServer);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                VirtualServer server = (VirtualServer)m_pServers.SelectedItems[0].Tag;                
                wfrm_VirtualServers_VirtualServer frm = new wfrm_VirtualServers_VirtualServer(m_pServer,server);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadVirtualServers();

                    // Refresh Tree "Virtual Servers" node.
                    m_pFrmMain.LoadVirtualServers(m_pVirtualServersNode,m_pServer);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                VirtualServer server = (VirtualServer)m_pServers.SelectedItems[0].Tag; 
                if(MessageBox.Show(this,"Are you sure you want to delete Virtual server '" + server.Name + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    server.Owner.Remove(server);
                    LoadVirtualServers();

                    // Refresh Tree "Virtual Servers" node.
                    m_pFrmMain.LoadVirtualServers(m_pVirtualServersNode,m_pServer);
                }
            }
        }

        #endregion


        #region method m_pServers_SelectedIndexChanged

        private void m_pServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pServers.SelectedItems.Count == 0){
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
            }
            else{
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
            }
        }

        #endregion

        #region method m_pServers_DoubleClick

        private void m_pServers_DoubleClick(object sender,EventArgs e)
        {
            if(m_pServers.SelectedItems.Count > 0){
                VirtualServer server = (VirtualServer)m_pServers.SelectedItems[0].Tag;                
                wfrm_VirtualServers_VirtualServer frm = new wfrm_VirtualServers_VirtualServer(m_pServer,server);
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadVirtualServers();
                    // Refresh Tree "Virtual Servers" node.
                    m_pFrmMain.LoadVirtualServers(m_pVirtualServersNode,m_pServer);
                }
            }
        }

        #endregion

        #endregion


        #region method LoadVirtualServers

        /// <summary>
        /// Gets virtual servers and loads them to UI.
        /// </summary>
        private void LoadVirtualServers()
        {
            m_pServers.Items.Clear();

            foreach(VirtualServer vServer in m_pServer.VirtualServers){
                ListViewItem it = new ListViewItem(vServer.Name);
                if(vServer.Enabled){
                    it.ImageIndex = 0;
                }
                else{
                    it.ImageIndex = 1;
                }
                it.Tag = vServer;
                m_pServers.Items.Add(it);
            }

            m_pServers_SelectedIndexChanged(this,null);
        }

        #endregion

    }
}
