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
    /// User folder messages window.
    /// </summary>
    public class wfrm_User_FolderMessages : Form
    {
        private PictureBox m_pIcon        = null;
        private Label      mt_Info        = null;
        private GroupBox   m_pSeparator1  = null;
        private ToolStrip  m_pToolbar     = null;
        private WListView  m_pMessages    = null;

        private VirtualServer m_pVirtualServer = null;
        private UserFolder    m_pFolder        = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="folder">Folder what messages to show.</param>
        public wfrm_User_FolderMessages(VirtualServer virtualServer,UserFolder folder)
        {
            m_pVirtualServer = virtualServer;
            m_pFolder        = folder;

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
            this.Text = "User '" + m_pFolder.User.UserName + "' Folder '" + m_pFolder.FolderFullPath + "' Messages";
            this.Icon = ResManager.GetIcon("message.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("message.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(400,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "User '" + m_pFolder.User.UserName + "' Folder '" + m_pFolder.FolderFullPath + "' Messages";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(682,3);
            m_pSeparator1.Location = new Point(7,50);
            m_pSeparator1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pToolbar = new ToolStrip();            
            m_pToolbar.AutoSize = false;
            m_pToolbar.Size = new Size(100,25);
            m_pToolbar.Location = new Point(595,55);
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
            // Write
            ToolStripButton button_Write = new ToolStripButton();
            button_Write.Image = ResManager.GetIcon("write.ico").ToBitmap();
            button_Write.Tag = "write";
            button_Write.ToolTipText = "Write Message";
            m_pToolbar.Items.Add(button_Write);
            // Save button
            ToolStripButton button_Save = new ToolStripButton();
            button_Save.Image = ResManager.GetIcon("save.ico").ToBitmap();
            button_Save.Tag = "save";
            button_Save.Enabled = false;
            button_Save.ToolTipText = "Save Message";
            m_pToolbar.Items.Add(button_Save);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.Enabled = false;
            button_Delete.ToolTipText = "Delete Message";
            m_pToolbar.Items.Add(button_Delete);

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
            m_pMessages.Columns.Add("Subject",250,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Sender",170,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Date",120,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Size KB",60,HorizontalAlignment.Right);
            m_pMessages.SelectedIndexChanged += new EventHandler(m_pMessages_SelectedIndexChanged);
   
            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
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
                dlg.FileName = m_pMessages.SelectedItems[0].Text.Replace("\\"," ").Replace("/"," ").Replace(":"," ").Replace("*"," ").Replace("?"," ").Replace("<"," ").Replace(">"," ");
                if(dlg.ShowDialog(this) == DialogResult.OK){                    
                    using(FileStream fs = File.Create(dlg.FileName)){
                        ListViewItem item = m_pMessages.SelectedItems[0];
                        m_pFolder.GetMessage((string)((object[])item.Tag)[0],fs);
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "write"){
                wfrm_Compose compose = new wfrm_Compose(m_pFolder);
                if(compose.ShowDialog(this) == DialogResult.OK){
                    MemoryStream ms = new MemoryStream(compose.Message);
                    ms.Position = 0;                    
                    m_pFolder.StoreMessage(ms);
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                if(MessageBox.Show(this,"Are you sure you want to delete selected messages !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                    for(int i=0;i<m_pMessages.SelectedItems.Count;i++){
                        ListViewItem item = m_pMessages.SelectedItems[i];
                        m_pFolder.DeleteMessage((string)((object[])item.Tag)[0],(int)((object[])item.Tag)[1]);
                        item.Remove();
                        i--;
                    }
                }
            }

            this.Cursor = Cursors.Default;
        }

        #endregion


        #region method m_pMessages_SelectedIndexChanged

        private void m_pMessages_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pMessages.SelectedItems.Count > 0){
                m_pToolbar.Items[2].Enabled = true;
                m_pToolbar.Items[3].Enabled = true;
            }
            else{
                m_pToolbar.Items[2].Enabled = false;
                m_pToolbar.Items[3].Enabled = false;
            }
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads messages to UI.
        /// </summary>
        private void LoadData()
        {
            m_pMessages.BeginUpdate();
            m_pMessages.Items.Clear();

            DataSet ds = m_pFolder.GetMessagesInfo();
            if(ds.Tables.Contains("MessagesInfo")){
                foreach(DataRow dr in ds.Tables["MessagesInfo"].Rows){
                    IMAP_t_Fetch_r_i_Envelope envelope = null;
                    try{
                        LumiSoft.Net.StringReader r = new LumiSoft.Net.StringReader(dr["Envelope"].ToString());
                        // Remove ENVELOPE and read content ().
                        r.ReadWord();
                        r =  new LumiSoft.Net.StringReader(r.ReadParenthesized());

                        envelope = IMAP_t_Fetch_r_i_Envelope.Parse(r);
                    }
                    catch{
                        envelope = new IMAP_t_Fetch_r_i_Envelope(DateTime.Now,"Mailserver parse error",null,null,null,null,null,null,null,null);
                    }

                    ListViewItem item = new ListViewItem(envelope.Subject);
                    item.ImageIndex = 0;
                    item.Tag = new object[]{dr["ID"].ToString(),Convert.ToInt32(dr["UID"])}; 
                    if(envelope.Sender != null && ((Mail_t_Mailbox)envelope.Sender[0]).DisplayName != null && ((Mail_t_Mailbox)envelope.Sender[0]).DisplayName != ""){
                        item.SubItems.Add(((Mail_t_Mailbox)envelope.Sender[0]).DisplayName);
                    }
                    else{
                        if(envelope.From != null && envelope.From.Length > 0){
                            item.SubItems.Add(envelope.From[0].ToString());
                        }
                        else{
                            item.SubItems.Add("<none>");
                        }
                    }
                    item.SubItems.Add(envelope.Date.ToString());
                    item.SubItems.Add(((decimal)(Convert.ToDecimal(dr["Size"]) / 1000)).ToString("f2"));
                    m_pMessages.Items.Add(item);
                }

                mt_Info.Text = "User '" + m_pFolder.User.UserName + "' Folder '" + m_pFolder.FolderFullPath + "' Messages (" + ds.Tables["MessagesInfo"].Rows.Count + ")";
            }            

            m_pMessages.EndUpdate();
            m_pMessages_SelectedIndexChanged(null,null);
        }

        #endregion
    }
}
