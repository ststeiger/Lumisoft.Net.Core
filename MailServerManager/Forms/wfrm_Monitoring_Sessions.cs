using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Monitoring -> Sessions window.
    /// </summary>
    public class wfrm_Monitoring_Sessions : Form
    {
        private Label     mt_Show           = null;
        private ComboBox  m_pShow           = null;
        private Button    m_pKill           = null;
        private Button    m_pViewSession    = null;
        private ImageList m_pSessionsImages = null;
        private WListView m_pSessions       = null;

        private Server m_pServer = null;
        private bool   m_Run     = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Mail server.</param>
        public wfrm_Monitoring_Sessions(Server server)
        {
            m_pServer = server;

            InitUI();

            m_pShow.SelectedIndex = 0;

            StartMonitoring();
        }

        #region method Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_Run = false;
        }

        #endregion

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(472,357);

            mt_Show = new Label();
            mt_Show.Size = new Size(70,20);
            mt_Show.Location = new Point(10,20);
            mt_Show.TextAlign = ContentAlignment.MiddleRight;
            mt_Show.Text = "Show:";

            m_pShow = new ComboBox();
            m_pShow.Size = new Size(100,20);
            m_pShow.Location = new Point(85,20);
            m_pShow.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pShow.SelectedIndexChanged += new EventHandler(m_pShow_SelectedIndexChanged);
            m_pShow.Items.Add("ALL");
            m_pShow.Items.Add("SMTP");
            m_pShow.Items.Add("POP3");
            m_pShow.Items.Add("IMAP");
            m_pShow.Items.Add("RELAY");
            m_pShow.Items.Add("ADMIN");
            
            m_pKill = new Button();
            m_pKill.Size = new Size(70,20);
            m_pKill.Location = new Point(307,20);
            m_pKill.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pKill.Text = "Kill";
            m_pKill.Enabled = false;
            m_pKill.Click += new EventHandler(m_pKill_Click);

            m_pViewSession = new Button();
            m_pViewSession.Size = new Size(70,20);
            m_pViewSession.Location = new Point(382,20);
            m_pViewSession.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pViewSession.Text = "View";
            m_pViewSession.Enabled = false;
            m_pViewSession.Click += new EventHandler(m_pViewSession_Click);

            m_pSessionsImages = new ImageList();            
            m_pSessionsImages.Images.Add(ResManager.GetIcon("user.ico"));
             
            m_pSessions = new WListView();
            m_pSessions.Size = new Size(445,265);
            m_pSessions.Location = new Point(9,47);
            m_pSessions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pSessions.View = View.Details;
            m_pSessions.FullRowSelect = true;
            m_pSessions.HideSelection = false;
            m_pSessions.SmallImageList = m_pSessionsImages;
            m_pSessions.SelectedIndexChanged += new EventHandler(m_pSessions_SelectedIndexChanged);
            m_pSessions.MouseClick += new MouseEventHandler(m_pSessions_MouseClick);
            m_pSessions.Columns.Add("Type",60,HorizontalAlignment.Left);
            m_pSessions.Columns.Add("UserName",80,HorizontalAlignment.Left);
            m_pSessions.Columns.Add("LocalEndPoint",100,HorizontalAlignment.Left);
            m_pSessions.Columns.Add("RemoteEndPoint",100,HorizontalAlignment.Left);
            m_pSessions.Columns.Add("R KB/S",55,HorizontalAlignment.Left);
            m_pSessions.Columns.Add("W KB/S",55,HorizontalAlignment.Left);
            m_pSessions.Columns.Add("Session Start",100,HorizontalAlignment.Left);
            m_pSessions.Columns.Add("Timeout after sec.",100,HorizontalAlignment.Left);
            
            this.Controls.Add(mt_Show);
            this.Controls.Add(m_pShow);
            this.Controls.Add(m_pKill);
            this.Controls.Add(m_pViewSession);
            this.Controls.Add(m_pSessions);
        }
                                                                                
        #endregion


        #region Events Handling

        #region method m_pShow_SelectedIndexChanged

        private void m_pShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_pSessions.Items.Clear();
        }

        #endregion

        #region method m_pKill_Click

        private void m_pKill_Click(object sender, EventArgs e)
        {
            SwitchToolBarTask("kill");
        }

        #endregion

        #region method m_pViewSession_Click

        private void m_pViewSession_Click(object sender, EventArgs e)
        {   
            SwitchToolBarTask("view");           
        }

        #endregion

        #region method m_pSessions_SelectedIndexChanged

        private void m_pSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pSessions.SelectedItems.Count > 0){
                m_pKill.Enabled = true;
                m_pViewSession.Enabled = true;
            }
            else{
                m_pKill.Enabled = false;
                m_pViewSession.Enabled = false;
            }
        }

        #endregion

        #region method m_pSessions_MouseClick

        private void m_pSessions_MouseClick(object sender,MouseEventArgs e)
        {
            // We want right click only.
            if(e.Button != MouseButtons.Right){
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.ItemClicked += new ToolStripItemClickedEventHandler(m_pSessions_ContextMenuItem_Clicked);
            //--- MenuItem View 
            ToolStripMenuItem menuItem_View = new ToolStripMenuItem("View");
            menuItem_View.Image = ResManager.GetIcon("viewmessages.ico").ToBitmap();
            menuItem_View.Tag = "view";
            menu.Items.Add(menuItem_View);
            //--- MenuItem Kill
            ToolStripMenuItem menuItem_Kill = new ToolStripMenuItem("Kill");
            menuItem_Kill.Tag = "kill";
            menuItem_Kill.Image = ResManager.GetIcon("exit.ico").ToBitmap();
            menu.Items.Add(menuItem_Kill);
            //---
            menu.Show(Control.MousePosition);
        }       
                
        #endregion

        #region method m_pSessions_ContextMenuItem_Clicked

        private void m_pSessions_ContextMenuItem_Clicked(object sender,ToolStripItemClickedEventArgs e)
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
            if(taskID == "view"){
                if(m_pSessions.SelectedItems.Count > 0){
                    Form frm = new Form();
                    frm.Size = new Size(800,600);
                    frm.StartPosition = FormStartPosition.CenterScreen;

                    TextBox textBox = new TextBox();
                    textBox.Dock = DockStyle.Fill;
                    textBox.Multiline = true;
                    textBox.ReadOnly = true;
                    textBox.Text = ((Session)m_pSessions.SelectedItems[0].Tag).SessionLog;                
                    textBox.SelectionStart = 0;
                    textBox.SelectionLength = 0;

                    frm.Controls.Add(textBox);
                    frm.Visible = true;
                }
            }
            else if(taskID == "kill"){
                if(m_pSessions.SelectedItems.Count > 0){
                    ((Session)m_pSessions.SelectedItems[0].Tag).Kill();
                    m_pSessions.SelectedItems[0].Remove();
                }
            }
        }

        #endregion


        #region method StartMonitoring

        /// <summary>
        /// Starts monitoring sessions on dedicated thread.
        /// </summary>
        private void StartMonitoring()
        {
            m_Run = true;
            Thread tr = new Thread(new ThreadStart(this.Run));
            tr.Start();
        }

        #endregion

        #region method Run

        /// <summary>
        /// Monitoring thread run loop.
        /// </summary>
        private void Run()
        {            
            while(m_Run){
                try{
                    // Force to refresh sessions
                    m_pServer.Sessions.Refresh();
                    
                    // Force UI to refresh
                    this.Invoke(new ThreadStart(this.RefreshSessions));
                }
                catch{
                }

                // Wait 3 sec 
                System.Threading.Thread.Sleep(3000);
            }
        }

        #endregion

        #region method RefreshSessions

        /// <summary>
        /// Refreshes sessions.
        /// </summary>
        private void RefreshSessions()
        {
            m_pSessions.BeginUpdate();

            try{
                // Update and remove exisitng sessions
                for(int i=0;i<m_pSessions.Items.Count;i++){
                    ListViewItem it = m_pSessions.Items[i];
                    
                    // Update session info
                    if(m_pServer.Sessions.ConatainsID(((Session)it.Tag).ID)){
                        Session session = m_pServer.Sessions.GetSessionByID(((Session)it.Tag).ID);

                        it.Tag = session;
                        it.SubItems[1].Text = session.UserName;
                        it.SubItems[2].Text = session.LocalEndPoint;
                        it.SubItems[3].Text = session.RemoteEndPoint;
                        it.SubItems[4].Text = (session.ReadKbInSecond / (decimal)1000).ToString("f2");
                        it.SubItems[5].Text = (session.WriteKbInSecond / (decimal)1000).ToString("f2");
                        it.SubItems[6].Text = session.SartTime.ToString("yy.MM.dd HH:mm");
                        it.SubItems[7].Text = session.IdleTimeOutSeconds.ToString();
                    }
                    // Remove session
                    else{
                        it.Remove();
                        i++;
                    }
                }

                // Add new sessions
                foreach(Session session in m_pServer.Sessions){
                    //--- Show only sessions what wanted to see --------------------//
                    // ALL
                    if(m_pShow.SelectedIndex == 0){
                        // Do nothing
                    }
                    // SMTP
                    else if(m_pShow.SelectedIndex == 1 && session.Type != "SMTP"){
                        continue;
                    }
                    // POP3
                    else if(m_pShow.SelectedIndex == 2 && session.Type != "POP3"){
                        continue;
                    }
                    // IMAP
                    else if(m_pShow.SelectedIndex == 3 && session.Type != "IMAP"){
                        continue;
                    }
                    // REALY
                    else if(m_pShow.SelectedIndex == 4 && session.Type != "RELAY"){
                        continue;
                    }
                    //---------------------------------------------------------------//

                    bool contains = false;
                    foreach(ListViewItem item in m_pSessions.Items){
                        if(session.ID == ((Session)item.Tag).ID){
                            contains = true;
                            break;
                        }
                    }

                    if(!contains){ 
                        ListViewItem it = new ListViewItem();
                        it.ImageIndex = 0;
                        it.Text = session.Type;
                        it.SubItems.Add(session.UserName);
                        it.SubItems.Add(session.LocalEndPoint);
                        it.SubItems.Add(session.RemoteEndPoint);
                        it.SubItems.Add((session.ReadKbInSecond / (decimal)1000).ToString("f2"));
                        it.SubItems.Add((session.WriteKbInSecond / (decimal)1000).ToString("f2"));
                        it.SubItems.Add(session.SartTime.ToString("yy.MM.dd HH:mm"));
                        it.SubItems.Add(session.IdleTimeOutSeconds.ToString());
                        it.Tag = session;
                        m_pSessions.Items.Add(it);
                    }
                }
            }
            catch{
            }

            m_pSessions.EndUpdate();
        }

        #endregion

    }
}
