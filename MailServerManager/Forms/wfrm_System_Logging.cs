using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// System Logging settings window.
    /// </summary>
    public class wfrm_System_Logging : Form
    {
        //--- Common UI -------------------------------
        private TabControl    m_pTab            = null;
        private Button        m_pApply          = null;
        //--- Tabpage General UI ----------------------
        private CheckBox m_pLogSMTP             = null;
        private Label    mt_SMTPLogPath         = null;
        private TextBox  m_pSMTPLogPath         = null;
        private Button   m_pGetSMTPLogPath      = null;
        private CheckBox m_pLogPOP3             = null;
        private Label    mt_POP3LogPath         = null;
        private TextBox  m_pPOP3LogPath         = null;
        private Button   m_pGetPOP3LogPath      = null;
        private CheckBox m_pLogIMAP             = null;
        private Label    mt_IMAPLogPath         = null;
        private TextBox  m_pIMAPLogPath         = null;
        private Button   m_pGetIMAPLogPath      = null;
        private CheckBox m_pLogRelay            = null;
        private Label    mt_RelayLogPath        = null;
        private TextBox  m_pRelayLogPath        = null;
        private Button   m_pGetRelayLogPath     = null;
        private CheckBox m_pLogFetchPOP3        = null;
        private Label    mt_FetchPOP3LogPath    = null;
        private TextBox  m_pFetchPOP3LogPath    = null;
        private Button   m_pGetFetchPOP3LogPath = null;
        private CheckBox m_pLogServer           = null;
        private Label    mt_ServerLogPath       = null;
        private TextBox  m_pServerLogPath       = null;
        private Button   m_pGetServerLogPath    = null;
        //---------------------------------------------

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_System_Logging(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadData();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            //--- Common UI -------------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(515,490);
            m_pTab.Location = new Point(5,0);
            m_pTab.TabPages.Add(new TabPage("General"));

            m_pApply = new Button();
            m_pApply.Size = new Size(70,20);
            m_pApply.Location = new Point(450,500);
            m_pApply.Text = "Apply";
            m_pApply.Click += new EventHandler(m_pApply_Click);
            //---------------------------------------------------//

            //--- Tabpage General UI ----------------------------//
            m_pLogSMTP = new CheckBox();
            m_pLogSMTP.Size = new Size(150,20);
            m_pLogSMTP.Location = new Point(10,20);
            m_pLogSMTP.Text = "Log SMTP";

            mt_SMTPLogPath = new Label();
            mt_SMTPLogPath.Size = new Size(50,20);
            mt_SMTPLogPath.Location = new Point(10,45);
            mt_SMTPLogPath.TextAlign = ContentAlignment.MiddleRight;
            mt_SMTPLogPath.Text = "Path:";

            m_pSMTPLogPath = new TextBox();
            m_pSMTPLogPath.Size = new Size(400,20);
            m_pSMTPLogPath.Location = new Point(65,45);
            m_pSMTPLogPath.ReadOnly = true;

            m_pGetSMTPLogPath = new Button();            
            m_pGetSMTPLogPath.Size = new Size(25,20);
            m_pGetSMTPLogPath.Location = new Point(470,45);
            m_pGetSMTPLogPath.Text = "...";
            m_pGetSMTPLogPath.Click += new EventHandler(m_pGetSMTPLogPath_Click);

            m_pLogPOP3 = new CheckBox();
            m_pLogPOP3.Size = new Size(150,20);
            m_pLogPOP3.Location = new Point(10,70);
            m_pLogPOP3.Text = "Log POP3";

            mt_POP3LogPath = new Label();
            mt_POP3LogPath.Size = new Size(50,20);
            mt_POP3LogPath.Location = new Point(10,90);
            mt_POP3LogPath.TextAlign = ContentAlignment.MiddleRight;
            mt_POP3LogPath.Text = "Path:";

            m_pPOP3LogPath = new TextBox();
            m_pPOP3LogPath.Size = new Size(400,20);
            m_pPOP3LogPath.Location = new Point(65,90);
            m_pPOP3LogPath.ReadOnly = true;

            m_pGetPOP3LogPath = new Button();            
            m_pGetPOP3LogPath.Size = new Size(25,20);
            m_pGetPOP3LogPath.Location = new Point(470,90);
            m_pGetPOP3LogPath.Text = "...";
            m_pGetPOP3LogPath.Click += new EventHandler(m_pGetPOP3LogPath_Click);

            m_pLogIMAP = new CheckBox();
            m_pLogIMAP.Size = new Size(150,20);
            m_pLogIMAP.Location = new Point(10,120);
            m_pLogIMAP.Text = "Log IMAP";

            mt_IMAPLogPath = new Label();
            mt_IMAPLogPath.Size = new Size(50,20);
            mt_IMAPLogPath.Location = new Point(10,140);
            mt_IMAPLogPath.TextAlign = ContentAlignment.MiddleRight;
            mt_IMAPLogPath.Text = "Path:";

            m_pIMAPLogPath = new TextBox();
            m_pIMAPLogPath.Size = new Size(400,20);
            m_pIMAPLogPath.Location = new Point(65,140);
            m_pIMAPLogPath.ReadOnly = true;

            m_pGetIMAPLogPath = new Button();            
            m_pGetIMAPLogPath.Size = new Size(25,20);
            m_pGetIMAPLogPath.Location = new Point(470,140);
            m_pGetIMAPLogPath.Text = "...";
            m_pGetIMAPLogPath.Click += new EventHandler(m_pGetIMAPLogPath_Click);

            m_pLogRelay = new CheckBox();
            m_pLogRelay.Size = new Size(150,20);
            m_pLogRelay.Location = new Point(10,170);
            m_pLogRelay.Text = "Log Relay";

            mt_RelayLogPath = new Label();
            mt_RelayLogPath.Size = new Size(50,20);
            mt_RelayLogPath.Location = new Point(10,190);
            mt_RelayLogPath.TextAlign = ContentAlignment.MiddleRight;
            mt_RelayLogPath.Text = "Path:";

            m_pRelayLogPath = new TextBox();
            m_pRelayLogPath.Size = new Size(400,20);
            m_pRelayLogPath.Location = new Point(65,190);
            m_pRelayLogPath.ReadOnly = true;

            m_pGetRelayLogPath = new Button();            
            m_pGetRelayLogPath.Size = new Size(25,20);
            m_pGetRelayLogPath.Location = new Point(470,190);
            m_pGetRelayLogPath.Text = "...";
            m_pGetRelayLogPath.Click += new EventHandler(m_pGetRelayLogPath_Click);

            m_pLogFetchPOP3 = new CheckBox();
            m_pLogFetchPOP3.Size = new Size(150,20);
            m_pLogFetchPOP3.Location = new Point(10,220);
            m_pLogFetchPOP3.Text = "Log Fetch POP3";

            mt_FetchPOP3LogPath = new Label();
            mt_FetchPOP3LogPath.Size = new Size(50,20);
            mt_FetchPOP3LogPath.Location = new Point(10,240);
            mt_FetchPOP3LogPath.TextAlign = ContentAlignment.MiddleRight;
            mt_FetchPOP3LogPath.Text = "Path:";

            m_pFetchPOP3LogPath = new TextBox();
            m_pFetchPOP3LogPath.Size = new Size(400,20);
            m_pFetchPOP3LogPath.Location = new Point(65,240);
            m_pFetchPOP3LogPath.ReadOnly = true;

            m_pGetFetchPOP3LogPath = new Button();            
            m_pGetFetchPOP3LogPath.Size = new Size(25,20);
            m_pGetFetchPOP3LogPath.Location = new Point(470,240);
            m_pGetFetchPOP3LogPath.Text = "...";
            m_pGetFetchPOP3LogPath.Click += new EventHandler(m_pGetFetchPOP3LogPath_Click);

            m_pLogServer = new CheckBox();
            m_pLogServer.Size = new Size(150,20);
            m_pLogServer.Location = new Point(10,270);
            m_pLogServer.Text = "Log Server";
            m_pLogServer.Checked = true;
            m_pLogServer.Enabled = false;

            mt_ServerLogPath = new Label();
            mt_ServerLogPath.Size = new Size(50,20);
            mt_ServerLogPath.Location = new Point(10,290);
            mt_ServerLogPath.TextAlign = ContentAlignment.MiddleRight;
            mt_ServerLogPath.Text = "Path:";

            m_pServerLogPath = new TextBox();
            m_pServerLogPath.Size = new Size(400,20);
            m_pServerLogPath.Location = new Point(65,290);
            m_pServerLogPath.ReadOnly = true;

            m_pGetServerLogPath = new Button();         
            m_pGetServerLogPath.Size = new Size(25,20);
            m_pGetServerLogPath.Location = new Point(470,290);
            m_pGetServerLogPath.Text = "...";

            // Tabpage General UI
            m_pTab.TabPages[0].Controls.Add(m_pLogSMTP);
            m_pTab.TabPages[0].Controls.Add(mt_SMTPLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pSMTPLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pGetSMTPLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pLogPOP3);
            m_pTab.TabPages[0].Controls.Add(mt_POP3LogPath);
            m_pTab.TabPages[0].Controls.Add(m_pPOP3LogPath);
            m_pTab.TabPages[0].Controls.Add(m_pGetPOP3LogPath);
            m_pTab.TabPages[0].Controls.Add(m_pLogIMAP);
            m_pTab.TabPages[0].Controls.Add(mt_IMAPLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pIMAPLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pGetIMAPLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pLogRelay);
            m_pTab.TabPages[0].Controls.Add(mt_RelayLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pRelayLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pGetRelayLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pLogFetchPOP3);
            m_pTab.TabPages[0].Controls.Add(mt_FetchPOP3LogPath);
            m_pTab.TabPages[0].Controls.Add(m_pFetchPOP3LogPath);
            m_pTab.TabPages[0].Controls.Add(m_pGetFetchPOP3LogPath);
            m_pTab.TabPages[0].Controls.Add(m_pLogServer);
            m_pTab.TabPages[0].Controls.Add(mt_ServerLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pServerLogPath);
            m_pTab.TabPages[0].Controls.Add(m_pGetServerLogPath);
            //-------------------------------------------------//

            // Common UI
            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pApply);
        }
                                                                                
        #endregion


        #region Events Handling

        #region method OnVisibleChanged

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if(!this.Visible){
                SaveData(true); 
            }
        }

        #endregion


        #region method m_pGetSMTPLogPath_Click

        private void m_pGetSMTPLogPath_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog dlg = new FolderBrowserDialog()){
				dlg.SelectedPath = m_pSMTPLogPath.Text;
				if(dlg.ShowDialog() == DialogResult.OK){
					m_pSMTPLogPath.Text = dlg.SelectedPath;
					if(m_pSMTPLogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\Smtp").ToLower()){
						m_pSMTPLogPath.Text = "";
					}
				}
			}
        }

        #endregion

        #region method m_pGetPOP3LogPath_Click

        private void m_pGetPOP3LogPath_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog dlg = new FolderBrowserDialog()){
				dlg.SelectedPath = m_pPOP3LogPath.Text;
				if(dlg.ShowDialog() == DialogResult.OK){
					m_pPOP3LogPath.Text = dlg.SelectedPath;
					if(m_pPOP3LogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\Pop3").ToLower()){
						m_pPOP3LogPath.Text = "";
					}
				}
			}
        }

        #endregion

        #region method m_pGetIMAPLogPath_Click

        private void m_pGetIMAPLogPath_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog dlg = new FolderBrowserDialog()){
				dlg.SelectedPath = m_pIMAPLogPath.Text;
				if(dlg.ShowDialog() == DialogResult.OK){
					m_pIMAPLogPath.Text = dlg.SelectedPath;
					if(m_pIMAPLogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\IMAP").ToLower()){
						m_pIMAPLogPath.Text = "";
					}
				}
			}
        }

        #endregion

        #region method m_pGetRelayLogPath_Click

        private void m_pGetRelayLogPath_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog dlg = new FolderBrowserDialog()){
				dlg.SelectedPath = m_pRelayLogPath.Text;
				if(dlg.ShowDialog() == DialogResult.OK){
					m_pRelayLogPath.Text = dlg.SelectedPath;
					if(m_pRelayLogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\Relay").ToLower()){
						m_pRelayLogPath.Text = "";
					}
				}
			}
        }

        #endregion

        #region method m_pGetFetchPOP3LogPath_Click

        private void m_pGetFetchPOP3LogPath_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog dlg = new FolderBrowserDialog()){
				dlg.SelectedPath = m_pFetchPOP3LogPath.Text;
				if(dlg.ShowDialog() == DialogResult.OK){
					m_pFetchPOP3LogPath.Text = dlg.SelectedPath;
					if(m_pFetchPOP3LogPath.Text.ToLower() == (Application.StartupPath + "\\Logs\\FetchPOP3").ToLower()){
						m_pFetchPOP3LogPath.Text = "";
					}
				}
			}
        }

        #endregion


        #region method m_pApply_Click

        private void m_pApply_Click(object sender, EventArgs e)
        {
            SaveData(false); 
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads data to UI.
        /// </summary>
        private void LoadData()
        {            
            try{
				Logging_Settings settings = m_pVirtualServer.SystemSettings.Logging;

				m_pLogSMTP.Checked       = settings.LogSMTP;
				m_pSMTPLogPath.Text      = settings.SmtpLogsPath;
				m_pLogPOP3.Checked       = settings.LogPOP3;
				m_pPOP3LogPath.Text      = settings.Pop3LogsPath;
				m_pLogIMAP.Checked       = settings.LogIMAP;
				m_pIMAPLogPath.Text      = settings.ImapLogsPath;
				m_pLogRelay.Checked      = settings.LogRelay;
				m_pRelayLogPath.Text     = settings.RelayLogsPath;
				m_pLogFetchPOP3.Checked  = settings.LogFetchMessages;
				m_pFetchPOP3LogPath.Text = settings.FetchMessagesLogsPath;
				//m_pServerLogPath.Text    = dr["Server_LogPath"].ToString();
			}
			catch(Exception x){
				wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
				frm.ShowDialog(this);
			}
        }

        #endregion

        #region method SaveData

        /// <summary>
        /// Saves data.
        /// </summary>
        /// <param name="confirmSave">Specifies is save confirmation UI is showed.</param>
        private void SaveData(bool confirmSave)
        {
            try{
                Logging_Settings settings = m_pVirtualServer.SystemSettings.Logging;

				settings.LogSMTP               = m_pLogSMTP.Checked;
				settings.SmtpLogsPath          = m_pSMTPLogPath.Text;
				settings.LogPOP3               = m_pLogPOP3.Checked;
				settings.Pop3LogsPath          = m_pPOP3LogPath.Text;
				settings.LogIMAP               = m_pLogIMAP.Checked;
				settings.ImapLogsPath          = m_pIMAPLogPath.Text;
				settings.LogRelay              = m_pLogRelay.Checked;
				settings.RelayLogsPath         = m_pRelayLogPath.Text;
				settings.LogFetchMessages      = m_pLogFetchPOP3.Checked;
				settings.FetchMessagesLogsPath = m_pFetchPOP3LogPath.Text;
                
                if(m_pVirtualServer.SystemSettings.HasChanges){
                    if(!confirmSave || MessageBox.Show(this,"You have changes settings, do you want to save them ?","Confirm:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                        m_pVirtualServer.SystemSettings.Commit();
                    }
                }
            }
			catch(Exception x){
				wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
				frm.ShowDialog(this);
			}
        }

        #endregion

    }
}
