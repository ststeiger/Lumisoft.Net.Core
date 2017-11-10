using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;
using LumiSoft.UI.Controls;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Monitoring -> SIP -> Calls window.
    /// </summary>
    public class wfrm_Monitoring_SIP_Calls : Form
    {
        private ToolStrip m_pToolbar = null;
        private WListView m_pCalls   = null;

        private Server m_pServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Mail server.</param>
        /// <param name="frame"></param>
        public wfrm_Monitoring_SIP_Calls(Server server,WFrame frame)
        {
            m_pServer = server;

            InitUI();

            // Move toolbar to Frame
            frame.Frame_ToolStrip = m_pToolbar;

            LoadData();
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
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText  = "Delete";
            m_pToolbar.Items.Add(button_Delete);
            // Refresh button
            ToolStripButton button_Refresh = new ToolStripButton();
            button_Refresh.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
            button_Refresh.Tag = "refresh";
            button_Refresh.ToolTipText  = "Refresh";
            m_pToolbar.Items.Add(button_Refresh);

            m_pCalls = new WListView();
            m_pCalls.Size = new Size(445,265);
            m_pCalls.Location = new Point(9,47);
            m_pCalls.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pCalls.View = View.Details;
            m_pCalls.FullRowSelect = true;
            m_pCalls.HideSelection = false;
            m_pCalls.Columns.Add("Caller",180,HorizontalAlignment.Left);
            m_pCalls.Columns.Add("Callee",180,HorizontalAlignment.Left);
            m_pCalls.Columns.Add("Start Time",80,HorizontalAlignment.Left);
            m_pCalls.SelectedIndexChanged += new EventHandler(m_pCalls_SelectedIndexChanged);

            this.Controls.Add(m_pCalls);
        }
                                                       
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                SIP_Call call = (SIP_Call)m_pCalls.SelectedItems[0].Tag;

                if(MessageBox.Show(this,"Are you sure you want to terminate call '" + call.Caller + "->" + call.Callee + "' ?","Remove Registration",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                    call.Terminate();
                    m_pCalls.SelectedItems[0].Remove();
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "refresh"){
                LoadData();
            }
        }

        #endregion

        #region method m_pCalls_SelectedIndexChanged

        private void m_pCalls_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pCalls.SelectedItems.Count > 0){
                m_pToolbar.Items[0].Enabled = true;
                m_pToolbar.Items[0].Enabled = true;
            }
            else{
                m_pToolbar.Items[0].Enabled = false;
                m_pToolbar.Items[0].Enabled = false;
            }
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads SIP calls to UI.
        /// </summary>
        private void LoadData()
        {
            m_pCalls.Items.Clear();

            foreach(VirtualServer virtualServer in m_pServer.VirtualServers){
                virtualServer.SipCalls.Refresh();            
                foreach(SIP_Call call in virtualServer.SipCalls){
                    ListViewItem it = new ListViewItem(call.Caller);                
                    it.SubItems.Add(call.Callee);
                    it.SubItems.Add(call.StartTime.ToString("HH:mm:ss"));
                    it.Tag = call;
                    m_pCalls.Items.Add(it);
                }
            }
        }

        #endregion

    }
}
