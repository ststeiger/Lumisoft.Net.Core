using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.Mail;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// User Recyclebin window.
    /// </summary>
    public class wfrm_User_Recyclebin : Form
    {
        private PictureBox     m_pIcon       = null;
        private Label          mt_Info       = null;
        private GroupBox       m_pSeparator1 = null;
        private Label          mt_Between    = null;
        private DateTimePicker m_pStartDate  = null;
        private DateTimePicker m_pEndDate    = null;
        private ToolStrip      m_pToolbar    = null;
        private WListView      m_pMessages   = null;

        private VirtualServer m_pVirtualServer = null;
        private User          m_pUser          = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="user">User who's recyclebin to show.</param>
        public wfrm_User_Recyclebin(VirtualServer virtualServer,User user)
        {
            m_pVirtualServer = virtualServer;
            m_pUser          = user;

            InitUI();

            LoadData();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(692,473);
            this.MinimumSize = new Size(700,500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "User '" + m_pUser.UserName + "' Recyclebin messages";
            this.Icon = ResManager.GetIcon("recyclebin16.ico");
            
            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("recyclebin.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(400,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "User '" + m_pUser.UserName + "' Recyclebin messages";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(682,3);
            m_pSeparator1.Location = new Point(7,50);
            m_pSeparator1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_Between = new Label();
            mt_Between.Size = new Size(100,20);
            mt_Between.Location = new Point(295,57);
            mt_Between.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            mt_Between.TextAlign = ContentAlignment.MiddleRight;
            mt_Between.Text = "Between:";

            m_pStartDate = new DateTimePicker();
            m_pStartDate.Size = new Size(100,20);
            m_pStartDate.Location = new Point(400,57);
            m_pStartDate.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pStartDate.Format = DateTimePickerFormat.Short;
            m_pStartDate.Value = DateTime.Today.AddDays(-1);

            m_pEndDate = new DateTimePicker();
            m_pEndDate.Size = new Size(100,20);
            m_pEndDate.Location = new Point(505,57);
            m_pEndDate.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pEndDate.Format = DateTimePickerFormat.Short;

            m_pToolbar = new ToolStrip();            
            m_pToolbar.AutoSize = false;
            m_pToolbar.Size = new Size(100,25);
            m_pToolbar.Location = new Point(617,55);
            m_pToolbar.Dock = DockStyle.None;
            m_pToolbar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pToolbar.BackColor = this.BackColor;
            m_pToolbar.Renderer = new ToolBarRendererEx();
            m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pToolbar_ItemClicked);
            // Refresh button
            ToolStripButton button_refresh = new ToolStripButton();
            button_refresh.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
            button_refresh.Tag = "refresh";
            button_refresh.ToolTipText = "Refresh";
            m_pToolbar.Items.Add(button_refresh);
            // Restore button
            ToolStripButton button_Restore = new ToolStripButton();
            button_Restore.Image = ResManager.GetIcon("restore.ico").ToBitmap();
            button_Restore.Tag = "restore";
            button_Restore.Enabled = false;
            button_Restore.ToolTipText = "Restore Message";
            m_pToolbar.Items.Add(button_Restore);
            // Save button
            ToolStripButton button_Save = new ToolStripButton();
            button_Save.Image = ResManager.GetIcon("save.ico").ToBitmap();
            button_Save.Tag = "save";
            button_Save.Enabled = false;
            button_Save.ToolTipText = "Save Message";
            m_pToolbar.Items.Add(button_Save);

            ImageList messagesImgList = new ImageList();
            messagesImgList.Images.Add(ResManager.GetIcon("message16.ico"));

            m_pMessages = new WListView();
            m_pMessages.Size = new Size(682,390);
            m_pMessages.Location = new Point(5,80);
            m_pMessages.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pMessages.View = View.Details;
            m_pMessages.FullRowSelect = true;
            m_pMessages.HideSelection = false;
            m_pMessages.SmallImageList = messagesImgList;
            m_pMessages.Columns.Add("Folder",120,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Subject",250,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Sender",170,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Date",120,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Date Deleted",120,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Size KB",60,HorizontalAlignment.Right);
            m_pMessages.SelectedIndexChanged += new EventHandler(m_pMessages_SelectedIndexChanged);
   
            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_Between);
            this.Controls.Add(m_pStartDate);
            this.Controls.Add(m_pEndDate);
            this.Controls.Add(m_pToolbar);
            this.Controls.Add(m_pMessages);
        }
                                                                
        #endregion


        #region Events Handling

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            if(e.ClickedItem.Tag.ToString() == "refresh"){
                LoadData();
            }
            else if(e.ClickedItem.Tag.ToString() == "save"){
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Email Message (*.eml)|*.eml";
                dlg.FileName = m_pMessages.SelectedItems[0].SubItems[1].Text.Replace("\\"," ").Replace("/"," ").Replace(":"," ").Replace("*"," ").Replace("?"," ").Replace("<"," ").Replace(">"," ");
                if(dlg.ShowDialog(this) == DialogResult.OK){                    
                    using(FileStream fs = File.Create(dlg.FileName)){
                        ListViewItem item = m_pMessages.SelectedItems[0];
                        m_pVirtualServer.RecycleBin.GetMessage(((DataRow)item.Tag)["MessageID"].ToString(),fs);
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "restore"){
                this.Cursor = Cursors.WaitCursor;

                for(int i=0;i<m_pMessages.SelectedItems.Count;i++){
                    ListViewItem it = m_pMessages.SelectedItems[i];
                    m_pVirtualServer.RecycleBin.RestoreRecycleBinMessage(((DataRow)it.Tag)["MessageID"].ToString());                
                    it.Remove();
                    i--;
                }
            }

            this.Cursor = Cursors.Default;
        }

        #endregion


        #region method m_pMessages_SelectedIndexChanged

        private void m_pMessages_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pMessages.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
                m_pToolbar.Items[2].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
                m_pToolbar.Items[2].Enabled = false;
            }
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads user recyclebin messages to UI.
        /// </summary>
        private void LoadData()
        {
            try{
                this.Cursor = Cursors.WaitCursor;
                m_pMessages.Items.Clear();
                                
                DataTable dtMessages = m_pVirtualServer.RecycleBin.GetMessagesInfo(m_pUser.UserName,m_pStartDate.Value.Date,m_pEndDate.Value.Date);
                foreach(DataRow dr in dtMessages.Rows){
                    string   user       = dr["User"].ToString();
                    DateTime deleteTime = Convert.ToDateTime(dr["DeleteTime"]).Date;

                    IMAP_Envelope envelope = null;
                    try{
                        envelope = IMAP_Envelope.Parse(new LumiSoft.Net.StringReader(dr["Envelope"].ToString()));
                    }
                    catch{
                    }

                    ListViewItem it = new ListViewItem(dr["Folder"].ToString());
                    it.SubItems.Add(envelope.Subject);
                    if(envelope.Sender != null && ((Mail_t_Mailbox)envelope.Sender[0]).DisplayName != null && ((Mail_t_Mailbox)envelope.Sender[0]).DisplayName != ""){
                        it.SubItems.Add(((Mail_t_Mailbox)envelope.Sender[0]).DisplayName);
                    }
                    else{
                        if(envelope.Sender != null){
                            it.SubItems.Add(envelope.Sender.ToString());
                        }
                        else{
                            it.SubItems.Add("<none>");
                        }
                    }
                    it.SubItems.Add(envelope.Date.ToString());
                    it.SubItems.Add(Convert.ToDateTime(dr["DeleteTime"]).ToString());
                    it.SubItems.Add(((decimal)(Convert.ToDecimal(dr["Size"]) / 1000)).ToString("f2"));
                    it.ImageIndex = 0;
                    it.Tag = dr;
                    m_pMessages.Items.Add(it); 
                }

                mt_Info.Text = "User '" + m_pUser.UserName + "' Recyclebin messages (" + dtMessages.Rows.Count + ")";
            }
            finally{
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

    }
}
