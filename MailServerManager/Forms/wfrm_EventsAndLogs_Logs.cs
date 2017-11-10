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
    /// Events and logs Logs window.
    /// </summary>
    public class wfrm_EventsAndLogs_Logs : Form
    {
        private Label          mt_VirtualServer = null;
        private ComboBox       m_pVirtualServer = null;
        private Button         m_pGet           = null;
        private Label          mt_Service       = null;
        private ComboBox       m_pService       = null;
        private Label          mt_Limit         = null;
        private NumericUpDown  m_pLimit         = null;
        private Label          mt_Date          = null;
        private DateTimePicker m_pDate          = null;
        private Label          mt_Between       = null;
        private DateTimePicker m_pStartTime     = null;
        private DateTimePicker m_pEndTime       = null;
        private Label          mt_ContainsText  = null;
        private TextBox        m_pContainsText  = null;
        private ListView       m_pLogSessions   = null;

        private Server m_pServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Mail server.</param>
        /// <param name="frame"></param>
        public wfrm_EventsAndLogs_Logs(Server server,WFrame frame)
        {
            m_pServer = server;

            InitUI();

            LoadVirtualServers();
            if(m_pVirtualServer.Items.Count > 0){
                m_pVirtualServer.SelectedIndex = 0;
            }
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(450,300);

            mt_VirtualServer = new Label();
            mt_VirtualServer.Size = new Size(80,20);
            mt_VirtualServer.Location = new Point(0,20);
            mt_VirtualServer.TextAlign = ContentAlignment.MiddleRight;
            mt_VirtualServer.Text = "Virtual Server:";

            m_pVirtualServer = new ComboBox();
            m_pVirtualServer.Size = new Size(305,20);
            m_pVirtualServer.Location = new Point(85,20);
            m_pVirtualServer.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pVirtualServer.SelectedIndexChanged += new EventHandler(m_pVirtualServer_SelectedIndexChanged);

            m_pGet = new Button();
            m_pGet.Size = new Size(70,20);
            m_pGet.Location = new Point(400,20);
            m_pGet.Text = "Get";
            m_pGet.Click += new EventHandler(m_pGet_Click);

            mt_Service = new Label();
            mt_Service.Size = new Size(80,20);
            mt_Service.Location = new Point(0,45);
            mt_Service.TextAlign = ContentAlignment.MiddleRight;
            mt_Service.Text = "Service:";

            m_pService = new ComboBox();
            m_pService.Size = new Size(120,20);
            m_pService.Location = new Point(85,45);
            m_pService.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pService.SelectedIndexChanged += new EventHandler(m_pService_SelectedIndexChanged);
            m_pService.Items.Add("SMTP");
            m_pService.Items.Add("POP3");
            m_pService.Items.Add("IMAP");
            m_pService.Items.Add("RELAY");
            m_pService.Items.Add("FETCH");
            m_pService.SelectedIndex = 0;

            mt_Limit = new Label();
            mt_Limit.Size = new Size(60,20);
            mt_Limit.Location = new Point(210,45);
            mt_Limit.TextAlign = ContentAlignment.MiddleRight;
            mt_Limit.Text = "Limit To:";

            m_pLimit = new NumericUpDown();
            m_pLimit.Size = new Size(55,20);
            m_pLimit.Location = new Point(275,45);
            m_pLimit.Minimum = 0;
            m_pLimit.Maximum = 99999;
            m_pLimit.Value = 1000;

            mt_Date = new Label();
            mt_Date.Size = new Size(80,20);
            mt_Date.Location = new Point(0,70);
            mt_Date.TextAlign = ContentAlignment.MiddleRight;
            mt_Date.Text = "Date:";

            m_pDate = new DateTimePicker();
            m_pDate.Size = new Size(120,20);
            m_pDate.Location = new Point(85,70);
            m_pDate.Format = DateTimePickerFormat.Short;
            m_pDate.ValueChanged += new EventHandler(m_pDate_ValueChanged);

            mt_Between = new Label();
            mt_Between.Size = new Size(70,20);
            mt_Between.Location = new Point(200,70);
            mt_Between.TextAlign = ContentAlignment.MiddleRight;
            mt_Between.Text = "Between:";

            m_pStartTime = new DateTimePicker();
            m_pStartTime.Size = new Size(55,20);
            m_pStartTime.Location = new Point(275,70);
            m_pStartTime.CustomFormat = "HH:mm";
            m_pStartTime.Format = DateTimePickerFormat.Custom;
            m_pStartTime.ShowUpDown = true;
            m_pStartTime.Value = new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day,0,0,0);
            m_pStartTime.ValueChanged += new EventHandler(m_pStartTime_ValueChanged);

            m_pEndTime = new DateTimePicker();
            m_pEndTime.Size = new Size(55,20);
            m_pEndTime.Location = new Point(335,70);
            m_pEndTime.CustomFormat = "HH:mm";
            m_pEndTime.Format = DateTimePickerFormat.Custom;
            m_pEndTime.ShowUpDown = true;
            m_pEndTime.Value = new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day,23,59,59);
            m_pEndTime.ValueChanged += new EventHandler(m_pEndTime_ValueChanged);

            mt_ContainsText = new Label();
            mt_ContainsText.Size = new Size(80,20);
            mt_ContainsText.Location = new Point(0,95);
            mt_ContainsText.TextAlign = ContentAlignment.MiddleRight;
            mt_ContainsText.Text = "Contains Text:";

            m_pContainsText = new TextBox();
            m_pContainsText.Size = new Size(305,20);
            m_pContainsText.Location = new Point(85,95);
            m_pContainsText.TextChanged += new EventHandler(m_pContainsText_TextChanged);

            m_pLogSessions = new ListView();
            m_pLogSessions.Size = new Size(422,135);
            m_pLogSessions.Location = new Point(10,125);
            m_pLogSessions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pLogSessions.View = View.Details;
            m_pLogSessions.FullRowSelect = true;
            m_pLogSessions.HideSelection = false;
            m_pLogSessions.DoubleClick += new EventHandler(m_pLogSessions_DoubleClick);
            m_pLogSessions.Columns.Add("Start Time",110,HorizontalAlignment.Left);
            m_pLogSessions.Columns.Add("User",100,HorizontalAlignment.Left);
            m_pLogSessions.Columns.Add("Remote End Point",150,HorizontalAlignment.Left);
            m_pLogSessions.Columns.Add("Session ID",100,HorizontalAlignment.Left);
                                                
            this.Controls.Add(mt_VirtualServer);
            this.Controls.Add(m_pVirtualServer);
            this.Controls.Add(m_pGet);
            this.Controls.Add(mt_Service);
            this.Controls.Add(m_pService);
            this.Controls.Add(mt_Limit);
            this.Controls.Add(m_pLimit);
            this.Controls.Add(mt_Date);
            this.Controls.Add(mt_Date);
            this.Controls.Add(m_pDate);
            this.Controls.Add(mt_Between);
            this.Controls.Add(m_pStartTime);
            this.Controls.Add(m_pEndTime);
            this.Controls.Add(mt_ContainsText);
            this.Controls.Add(m_pContainsText);
            this.Controls.Add(m_pLogSessions);
        }
                                                                                                                               
        #endregion


        #region Events Handling

        #region method m_pVirtualServer_SelectedIndexChanged

        private void m_pVirtualServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pLogSessions != null){
                m_pLogSessions.Items.Clear();
            }
        }

        #endregion

        #region method m_pGet_Click

        private void m_pGet_Click(object sender, EventArgs e)
        {
            if(m_pVirtualServer.Items.Count == 0){
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            m_pLogSessions.BeginUpdate();
            m_pLogSessions.Items.Clear();

            DateTime startTime = new DateTime(m_pDate.Value.Year,m_pDate.Value.Month,m_pDate.Value.Day,m_pStartTime.Value.Hour,m_pStartTime.Value.Minute,0);
            DateTime endTime   = new DateTime(m_pDate.Value.Year,m_pDate.Value.Month,m_pDate.Value.Day,m_pEndTime.Value.Hour,m_pEndTime.Value.Minute,0);

            LogSession[] logSessions = null;
            VirtualServer virtualServer = ((VirtualServer)((WComboBoxItem)m_pVirtualServer.SelectedItem).Tag);
            if(m_pService.Text == "SMTP"){
                logSessions = virtualServer.Logs.GetSmtpLogSessions((int)m_pLimit.Value,m_pDate.Value,startTime,endTime,m_pContainsText.Text);
            }
            else if(m_pService.Text == "POP3"){
                logSessions = virtualServer.Logs.GetPop3LogSessions((int)m_pLimit.Value,m_pDate.Value,startTime,endTime,m_pContainsText.Text);
            }
            else if(m_pService.Text == "IMAP"){
                logSessions = virtualServer.Logs.GetImapLogSessions((int)m_pLimit.Value,m_pDate.Value,startTime,endTime,m_pContainsText.Text);
            }
            else if(m_pService.Text == "RELAY"){
                logSessions = virtualServer.Logs.GetRelayLogSessions((int)m_pLimit.Value,m_pDate.Value,startTime,endTime,m_pContainsText.Text);
            }
            else if(m_pService.Text == "FETCH"){
                logSessions = virtualServer.Logs.GetFetchLogSessions((int)m_pLimit.Value,m_pDate.Value,startTime,endTime,m_pContainsText.Text);
            }

            foreach(LogSession logSession in logSessions){
                ListViewItem it = new ListViewItem(logSession.StartTime.ToString());
                it.SubItems.Add(logSession.UserName);
                it.SubItems.Add(logSession.RemoteEndPoint.ToString());
                it.SubItems.Add(logSession.SessionID);
                it.Tag = logSession;
                m_pLogSessions.Items.Add(it);
            }

            m_pLogSessions.EndUpdate();
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region method m_pService_SelectedIndexChanged

        private void m_pService_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pLogSessions != null){
                m_pLogSessions.Items.Clear();
            }
        }

        #endregion

        #region method m_pDate_ValueChanged

        private void m_pDate_ValueChanged(object sender, EventArgs e)
        {
            if(m_pLogSessions != null){
                m_pLogSessions.Items.Clear();
            }
        }

        #endregion

        #region method m_pStartTime_ValueChanged

        private void m_pStartTime_ValueChanged(object sender, EventArgs e)
        {
            if(m_pLogSessions != null){
                m_pLogSessions.Items.Clear();
            }
        }

        #endregion

        #region method m_pEndTime_ValueChanged

        private void m_pEndTime_ValueChanged(object sender, EventArgs e)
        {
            if(m_pLogSessions != null){
                m_pLogSessions.Items.Clear();
            }
        }

        #endregion

        #region method m_pContainsText_TextChanged

        private void m_pContainsText_TextChanged(object sender, EventArgs e)
        {
            if(m_pLogSessions != null){
                m_pLogSessions.Items.Clear();
            }
        }

        #endregion

        #region method m_pLogSessions_DoubleClick

        private void m_pLogSessions_DoubleClick(object sender,EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if(m_pLogSessions.SelectedItems.Count > 0){
                LogSession logSession = (LogSession)m_pLogSessions.SelectedItems[0].Tag;

                wfrm_LogViewer frm = new wfrm_LogViewer(logSession.LogText,m_pContainsText.Text);
                frm.ShowDialog(this);
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

        #endregion


        #region method LoadVirtualServers

        /// <summary>
        /// Loads virtual servers to virtual servers combobox.
        /// </summary>
        private void LoadVirtualServers()
        {
            foreach(VirtualServer vServer in m_pServer.VirtualServers){
                m_pVirtualServer.Items.Add(new WComboBoxItem(vServer.Name,vServer));
            }
        }

        #endregion

    }
}
