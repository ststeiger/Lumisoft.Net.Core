using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.Net;
using LumiSoft.Net.SMTP.Relay;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Relay smart host UI.
    /// </summary>
    public class wfrm_System_SmartHost : Form
    {
        private PictureBox    m_pIcon       = null;
        private Label         mt_Info       = null;
        private GroupBox      m_pSeparator1 = null;
        private Label         mt_Server     = null;
        private TextBox       m_pServer     = null;
        private NumericUpDown m_pPort       = null;
        private Label         mt_SslMode    = null;
        private ComboBox      m_pSslMode    = null;
        private Label         mt_UserName   = null;
        private TextBox       m_pUserName   = null;
        private Label         mt_Password   = null;
        private TextBox       m_pPassword   = null;
        private GroupBox      m_pSeparator2 = null;
        private Button        m_pCancel     = null;
        private Button        m_pOk         = null;

        /// <summary>
        /// Add new constructor.
        /// </summary>
        public wfrm_System_SmartHost()
        {
            InitUI();
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="host">Smart host name or IP address.</param>
        /// <param name="port">Smart host port.</param>
        /// <param name="sslMode">SSL mode.</param>
        /// <param name="userName">Smart host user name.</param>
        /// <param name="password">Smart host password.</param>
        public wfrm_System_SmartHost(string host,int port,SslMode sslMode,string userName,string password)
        {
            InitUI();

            m_pServer.Text = host;
            m_pPort.Value = port;
            if(sslMode == SslMode.None){
                m_pSslMode.SelectedIndex = 0;
            }
            else if(sslMode == SslMode.SSL){
                m_pSslMode.SelectedIndex = 1;
            }
            else if(sslMode == SslMode.TLS){
                m_pSslMode.SelectedIndex = 2;
            }
            m_pUserName.Text = userName;
            m_pPassword.Text = password;
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="smartHost">Smart host.</param>
        public wfrm_System_SmartHost(Relay_SmartHost smartHost)
        {
            InitUI();

            m_pServer.Text = smartHost.Host;
            m_pPort.Value = smartHost.Port;
            if(smartHost.SslMode == SslMode.None){
                m_pSslMode.SelectedIndex = 0;
            }
            else if(smartHost.SslMode == SslMode.SSL){
                m_pSslMode.SelectedIndex = 1;
            }
            else if(smartHost.SslMode == SslMode.TLS){
                m_pSslMode.SelectedIndex = 2;
            }
            m_pUserName.Text = smartHost.UserName;
            m_pPassword.Text = smartHost.Password;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(400,230);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Add/Edit smart host";

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("server.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify smart host information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(385,3);
            m_pSeparator1.Location = new Point(10,50);

            mt_Server = new Label();
            mt_Server.Size = new Size(100,20);
            mt_Server.Location = new Point(0,70);
            mt_Server.TextAlign = ContentAlignment.MiddleRight;
            mt_Server.Text = "Host:";

            m_pServer = new TextBox();
            m_pServer.Size = new Size(200,20);
            m_pServer.Location = new Point(105,70);

            m_pPort = new NumericUpDown();
            m_pPort.Size = new Size(75,20);
            m_pPort.Location = new Point(310,70);
            m_pPort.Minimum = 1;
            m_pPort.Maximum = 99999;
            m_pPort.Value = WellKnownPorts.SMTP;

            mt_SslMode = new Label();
            mt_SslMode.Size = new Size(100,20);
            mt_SslMode.Location = new Point(0,95);
            mt_SslMode.TextAlign = ContentAlignment.MiddleRight;
            mt_SslMode.Text = "SSL Mode:";

            m_pSslMode = new ComboBox();
            m_pSslMode.Size = new Size(100,20);
            m_pSslMode.Location = new Point(105,95);
            m_pSslMode.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pSslMode.SelectedIndexChanged += new EventHandler(m_pSslMode_SelectedIndexChanged);
            m_pSslMode.Items.Add("None");
            m_pSslMode.Items.Add("SSL");
            m_pSslMode.Items.Add("TLS");
            m_pSslMode.SelectedIndex = 0;

            mt_UserName = new Label();
            mt_UserName.Size = new Size(100,20);
            mt_UserName.Location = new Point(0,120);
            mt_UserName.TextAlign = ContentAlignment.MiddleRight;
            mt_UserName.Text = "User Name:";

            m_pUserName = new TextBox();
            m_pUserName.Size = new Size(200,20);
            m_pUserName.Location = new Point(105,120);

            mt_Password = new Label();
            mt_Password.Size = new Size(100,20);
            mt_Password.Location = new Point(0,145);
            mt_Password.TextAlign = ContentAlignment.MiddleRight;
            mt_Password.Text = "Password:";

            m_pPassword = new TextBox();
            m_pPassword.Size = new Size(200,20);
            m_pPassword.Location = new Point(105,145);
            m_pPassword.PasswordChar = '*';

            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(383,4);
            m_pSeparator2.Location = new Point(7,180);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(240,200);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(315,200);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_Server);
            this.Controls.Add(m_pServer);
            this.Controls.Add(m_pPort);
            this.Controls.Add(mt_SslMode);
            this.Controls.Add(m_pSslMode);
            this.Controls.Add(mt_UserName);
            this.Controls.Add(m_pUserName);
            this.Controls.Add(mt_Password);
            this.Controls.Add(m_pPassword);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                
        #endregion


        #region Events Handling

        #region method m_pSslMode_SelectedIndexChanged

        private void m_pSslMode_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pSslMode.Text == "SSL"){
                m_pPort.Value = WellKnownPorts.SMTP_SSL;
            }
            else{
                m_pPort.Value = WellKnownPorts.SMTP;
            }
        }

        #endregion


        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender,EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender,EventArgs e)
        {
            if(m_pServer.Text.Trim() == ""){
                MessageBox.Show(this,"Please specify Host value !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets smart host name.
        /// </summary>
        public string Host
        {
            get{ return m_pServer.Text; }
        }

        /// <summary>
        /// Gets smart host port.
        /// </summary>
        public int Port
        {
            get{ return (int)m_pPort.Value; }
        }

        /// <summary>
        /// Gets smart host SSL mode.
        /// </summary>
        public SslMode SslMode
        {
            get{
                if(m_pSslMode.Text.ToLower() == "ssl"){
                    return SslMode.SSL;
                }
                else if(m_pSslMode.Text.ToLower() == "tls"){
                    return SslMode.TLS;
                }
                else{
                    return SslMode.None; 
                }
            }
        }

        /// <summary>
        /// Gets smart host user name.
        /// </summary>
        public string UserName
        {
            get{ return m_pUserName.Text; }
        }

        /// <summary>
        /// Gets smart host password.
        /// </summary>
        public string Password
        {
            get{ return m_pPassword.Text; }
        }

        #endregion

    }
}
