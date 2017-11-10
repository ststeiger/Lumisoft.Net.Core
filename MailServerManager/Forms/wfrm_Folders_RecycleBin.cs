using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.Net;
using LumiSoft.Net.Mail;
using LumiSoft.Net.IMAP;
using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Folders management Recycle Bin UI.
    /// </summary>
    public class wfrm_Folders_RecycleBin : Form
    {
        private CheckBox       m_pDeleteToRecycleBin = null;
        private NumericUpDown  m_pDeleteAfterDays    = null;
        private Label          mt_DeleteAfterDays    = null;
        private Button         m_pApply              = null;
        private GroupBox       m_pGroupBox1          = null;
        private Label          mt_User               = null;
        private TextBox        m_pUser               = null;
        private Button         m_pGetUser            = null;
        private Label          mt_Between            = null;
        private DateTimePicker m_pSince              = null;
        private DateTimePicker m_pTo                 = null;
        private Button         m_pGet                = null;
        private Button         m_pRestore            = null;
        private WListView      m_pMessages           = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="frame"></param>
        public wfrm_Folders_RecycleBin(VirtualServer virtualServer,WFrame frame)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadSettings();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(492,373);

            m_pDeleteToRecycleBin = new CheckBox();
            m_pDeleteToRecycleBin.Size = new Size(300,20);
            m_pDeleteToRecycleBin.Location = new Point(10,15);
            m_pDeleteToRecycleBin.Text = "Delete all messages to recycle bin";
            m_pDeleteToRecycleBin.CheckedChanged += new EventHandler(m_pDeleteToRecycleBin_CheckedChanged);

            m_pDeleteAfterDays = new NumericUpDown();
            m_pDeleteAfterDays.Size = new Size(50,20);
            m_pDeleteAfterDays.Location = new Point(65,45);
            m_pDeleteAfterDays.Minimum = 1;
            m_pDeleteAfterDays.Maximum = 365;
            m_pDeleteAfterDays.Value = 1;

            mt_DeleteAfterDays = new Label();
            mt_DeleteAfterDays.Size = new Size(300,20);
            mt_DeleteAfterDays.Location = new Point(125,45);
            mt_DeleteAfterDays.TextAlign = ContentAlignment.MiddleLeft;
            mt_DeleteAfterDays.Text = "Delete messages permanently after specified days";

            m_pApply = new Button();
            m_pApply.Size = new Size(70,20);
            m_pApply.Location = new Point(415,45);
            m_pApply.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pApply.Text = "Apply";
            m_pApply.Click += new EventHandler(m_pApply_Click);

            m_pGroupBox1 = new GroupBox();            
            m_pGroupBox1.Size = new Size(480,3);
            m_pGroupBox1.Location = new Point(5,75);            
            m_pGroupBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_User = new Label();          
            mt_User.Size = new Size(50,20);
            mt_User.Location = new Point(10,95);
            mt_User.TextAlign = ContentAlignment.MiddleRight;
            mt_User.Text = "User:";

            m_pUser = new TextBox();      
            m_pUser.Size = new Size(165,20);
            m_pUser.Location = new Point(65,95);

            m_pGetUser = new Button();
            m_pGetUser.Size = new Size(25,20);
            m_pGetUser.Location = new Point(235,95);
            m_pGetUser.Text = "...";
            m_pGetUser.Click += new EventHandler(m_pGetUser_Click);

            mt_Between = new Label();  
            mt_Between.Size = new Size(60,20);
            mt_Between.Location = new Point(0,120);
            mt_Between.TextAlign = ContentAlignment.MiddleRight;
            mt_Between.Text = "Between:";

            m_pSince = new DateTimePicker();
            m_pSince.Size = new Size(80,20);
            m_pSince.Location = new Point(65,120);
            m_pSince.Format = DateTimePickerFormat.Short;

            m_pTo = new DateTimePicker();
            m_pTo.Size = new Size(80,20);
            m_pTo.Location = new Point(150,120);
            m_pTo.Format = DateTimePickerFormat.Short;

            m_pGet = new Button();
            m_pGet.Size = new Size(50,20);
            m_pGet.Location = new Point(355,120);
            m_pGet.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pGet.Text = "Get";
            m_pGet.Click += new EventHandler(m_pGet_Click);

            m_pRestore = new Button();
            m_pRestore.Size = new Size(70,20);
            m_pRestore.Location = new Point(415,120);
            m_pRestore.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pRestore.Text = "Restore";
            m_pRestore.Click += new EventHandler(m_pRestore_Click);

            m_pMessages = new WListView();
            m_pMessages.Size = new Size(475,215);
            m_pMessages.Location = new Point(10,150);
            m_pMessages.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pMessages.View = View.Details;
            m_pMessages.HideSelection = false;
            m_pMessages.FullRowSelect = true;
            m_pMessages.Columns.Add("User",100,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Folder",120,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Subject",250,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Sender",170,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Date",120,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Date Deleted",120,HorizontalAlignment.Left);
            m_pMessages.Columns.Add("Size KB",60,HorizontalAlignment.Right);
            m_pMessages.SelectedIndexChanged += new EventHandler(m_pMessages_SelectedIndexChanged);

            this.Controls.Add(m_pDeleteToRecycleBin);
            this.Controls.Add(m_pDeleteAfterDays);
            this.Controls.Add(mt_DeleteAfterDays);
            this.Controls.Add(m_pApply);
            this.Controls.Add(m_pGroupBox1);
            this.Controls.Add(mt_User);
            this.Controls.Add(m_pUser);
            this.Controls.Add(m_pGetUser);
            this.Controls.Add(mt_Between);
            this.Controls.Add(m_pSince);
            this.Controls.Add(m_pTo);
            this.Controls.Add(m_pGet);
            this.Controls.Add(m_pRestore);
            this.Controls.Add(m_pMessages);
        }
                                                                                                
        #endregion


        #region Events Handling

        #region method m_pDeleteToRecycleBin_CheckedChanged

        private void m_pDeleteToRecycleBin_CheckedChanged(object sender, EventArgs e)
        {
            if(m_pDeleteToRecycleBin.Checked){
                foreach(Control c in this.Controls){
                    c.Enabled = true;
                }
            }
            else{
                foreach(Control c in this.Controls){
                    if(!c.Equals(m_pDeleteToRecycleBin) && !c.Equals(m_pApply)){
                        c.Enabled = false;
                    }
                }
            }

            m_pMessages_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

        #region method m_pApply_Click

        private void m_pApply_Click(object sender, EventArgs e)
        {
            m_pVirtualServer.RecycleBin.DeleteToRecycleBin  = m_pDeleteToRecycleBin.Checked;
            m_pVirtualServer.RecycleBin.DeleteMessagesAfter = (int)m_pDeleteAfterDays.Value;
            m_pVirtualServer.RecycleBin.Commit();
        }

        #endregion


        #region method m_pGetUser_Click

        private void m_pGetUser_Click(object sender, EventArgs e)
        {
            wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pVirtualServer,false,false);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pUser.Text = frm.SelectedUserOrGroup;
            }
        }

        #endregion

        #region method m_pGet_Click

        private void m_pGet_Click(object sender, EventArgs e)
        {
            try{
                this.Cursor = Cursors.WaitCursor;
                m_pMessages.Items.Clear();
                                
                foreach(DataRow dr in m_pVirtualServer.RecycleBin.GetMessagesInfo(m_pUser.Text,m_pSince.Value.Date,m_pTo.Value.Date).Rows){
                    string   user       = dr["User"].ToString();
                    DateTime deleteTime = Convert.ToDateTime(dr["DeleteTime"]).Date;
                                       
                    IMAP_Envelope envelope = null;
                    try{
                        envelope = IMAP_Envelope.Parse(new LumiSoft.Net.StringReader(dr["Envelope"].ToString()));
                    }
                    catch{
                    }

                    ListViewItem it = new ListViewItem(user);
                    it.SubItems.Add(dr["Folder"].ToString());
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
            }
            finally{
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region method m_pRestore_Click

        private void m_pRestore_Click(object sender, EventArgs e)
        {
            try{
                this.Cursor = Cursors.WaitCursor;

                for(int i=0;i<m_pMessages.SelectedItems.Count;i++){
                    ListViewItem it = m_pMessages.SelectedItems[i];
                    m_pVirtualServer.RecycleBin.RestoreRecycleBinMessage(((DataRow)it.Tag)["MessageID"].ToString());                
                    it.Remove();
                    i--;
                }
            }
            finally{
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region method m_pMessages_SelectedIndexChanged

        private void m_pMessages_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pMessages.SelectedItems.Count > 0){
                m_pRestore.Enabled = true;
            }
            else{
                m_pRestore.Enabled = false;
            }
        }

        #endregion

        #endregion


        #region method LoadSettings

        /// <summary>
        /// Loads Recycle Bin settings to UI.
        /// </summary>
        private void LoadSettings()
        {
            m_pDeleteToRecycleBin.Checked = m_pVirtualServer.RecycleBin.DeleteToRecycleBin;
            m_pDeleteAfterDays.Value      = m_pVirtualServer.RecycleBin.DeleteMessagesAfter;

            m_pDeleteToRecycleBin_CheckedChanged(this,new EventArgs());
        }

        #endregion

    }
}
