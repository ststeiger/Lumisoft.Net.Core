using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Add/edit SIP gateway window.
    /// </summary>
    public class wfrm_System_Services_SIP_Gateway : Form
    {
        private PictureBox    m_pIcon       = null;
        private Label         mt_Info       = null;
        private GroupBox      m_pSeparator1 = null;
        private Label         mt_UriScheme  = null;
        private ComboBox      m_pUriScheme  = null;
        private Label         mt_Transport  = null;
        private ComboBox      m_pTransport  = null;
        private Label         mt_Host       = null;
        private TextBox       m_pHost       = null;
        private NumericUpDown m_pPort       = null;
        private Label         mt_Realm      = null;
        private TextBox       m_pRealm      = null;
        private Label         mt_UserName   = null;
        private TextBox       m_pUserName   = null;
        private Label         mt_Password   = null;
        private TextBox       m_pPassword   = null;
        private GroupBox      m_pSeparator2 = null;
        private Button        m_pCancel     = null;
        private Button        m_pOk         = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_System_Services_SIP_Gateway()
        {
            InitUI();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uriScheme">URI scheme.</param>
        /// <param name="transport">Transport.</param>
        /// <param name="host">Host name or IP.</param>
        /// <param name="port">Host port.</param>
        /// <param name="realm">Realm(domain).</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public wfrm_System_Services_SIP_Gateway(string uriScheme,string transport,string host,int port,string realm,string userName,string password)
        {
            InitUI();

            m_pUriScheme.Text = uriScheme;
            m_pTransport.Text = transport;
            m_pHost.Text      = host;
            m_pPort.Value     = port;
            m_pRealm.Text     = realm;
            m_pUserName.Text  = userName;
            m_pPassword.Text  = password;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(400,260);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "SIP Gateway";
            this.Icon = ResManager.GetIcon("rule.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("rule.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(350,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "SIP gateway info.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(385,3);
            m_pSeparator1.Location = new Point(7,50);
            m_pSeparator1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_UriScheme = new Label();
            mt_UriScheme.Size = new Size(100,20);
            mt_UriScheme.Location = new Point(0,65);
            mt_UriScheme.TextAlign = ContentAlignment.MiddleRight;
            mt_UriScheme.Text = "URI scheme:";

            m_pUriScheme = new ComboBox();
            m_pUriScheme.Size = new Size(80,20);
            m_pUriScheme.Location = new Point(105,65);
            m_pUriScheme.Items.Add("tel");
            m_pUriScheme.Text = "tel";

            mt_Transport = new Label();
            mt_Transport.Size = new Size(100,20);
            mt_Transport.Location = new Point(0,90);
            mt_Transport.TextAlign = ContentAlignment.MiddleRight;
            mt_Transport.Text = "Transport:";

            m_pTransport = new ComboBox();
            m_pTransport.Size = new Size(80,20);
            m_pTransport.Location = new Point(105,90);
            m_pTransport.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pTransport.Items.Add("UDP");
            m_pTransport.Items.Add("TCP");
            m_pTransport.Items.Add("TLS");
            m_pTransport.Text = "UDP";

            mt_Host = new Label();
            mt_Host.Size = new Size(100,20);
            mt_Host.Location = new Point(0,115);
            mt_Host.TextAlign = ContentAlignment.MiddleRight;
            mt_Host.Text = "Host:";

            m_pHost = new TextBox();
            m_pHost.Size = new Size(205,20);
            m_pHost.Location = new Point(105,115);
            m_pHost.TabIndex = 1;

            m_pPort = new NumericUpDown();
            m_pPort.Size = new Size(70,20);
            m_pPort.Location = new Point(315,115);
            m_pPort.Minimum = 1;
            m_pPort.Maximum = 99999;
            m_pPort.Value = 5060;

            mt_Realm = new Label();
            mt_Realm.Size = new Size(100,20);
            mt_Realm.Location = new Point(0,140);
            mt_Realm.TextAlign = ContentAlignment.MiddleRight;
            mt_Realm.Text = "Realm:";

            m_pRealm = new TextBox();
            m_pRealm.Size = new Size(205,20);
            m_pRealm.Location = new Point(105,140);
                        
            mt_UserName = new Label();
            mt_UserName.Size = new Size(100,20);
            mt_UserName.Location = new Point(0,165);
            mt_UserName.TextAlign = ContentAlignment.MiddleRight;
            mt_UserName.Text = "User:";

            m_pUserName = new TextBox();
            m_pUserName.Size = new Size(205,20);
            m_pUserName.Location = new Point(105,165);

            mt_Password = new Label();
            mt_Password.Size = new Size(100,20);
            mt_Password.Location = new Point(0,190);
            mt_Password.TextAlign = ContentAlignment.MiddleRight;
            mt_Password.Text = "Password:";

            m_pPassword = new TextBox();
            m_pPassword.Size = new Size(205,20);
            m_pPassword.Location = new Point(105,190);
            m_pPassword.PasswordChar = '*';

            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(385,3);
            m_pSeparator2.Location = new Point(7,225);
            m_pSeparator2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(240,235);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(315,235);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_UriScheme);
            this.Controls.Add(m_pUriScheme);
            this.Controls.Add(mt_Transport);
            this.Controls.Add(m_pTransport);
            this.Controls.Add(mt_Host);
            this.Controls.Add(m_pHost);
            this.Controls.Add(m_pPort);
            this.Controls.Add(mt_Realm);
            this.Controls.Add(m_pRealm);
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

        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender,EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender,EventArgs e)
        {
            if(m_pUriScheme.Text == ""){
                MessageBox.Show(this,"Please fill URI scheme !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if(m_pHost.Text == ""){
                MessageBox.Show(this,"Please fill Host !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets current URI scheme value.
        /// </summary>
        public string UriScheme
        {
            get{ return m_pUriScheme.Text; }
        }

        /// <summary>
        /// Gets current Transport value.
        /// </summary>
        public string Transport
        {
            get{ return m_pTransport.Text; }
        }

        /// <summary>
        /// Gets current Host value.
        /// </summary>
        public string Host
        {
            get{ return m_pHost.Text; }
        }

        /// <summary>
        /// Gets current Port value.
        /// </summary>
        public int Port
        {
            get{ return (int)m_pPort.Value; }
        }

        /// <summary>
        /// Gets current Realm(Domain) value.
        /// </summary>
        public string Realm
        {
            get{ return m_pRealm.Text; }
        }

        /// <summary>
        /// Gets current User Name value.
        /// </summary>
        public string UserName
        {
            get{ return m_pUserName.Text; }
        }

        /// <summary>
        /// Gets current Password value.
        /// </summary>
        public string Password
        {
            get{ return m_pPassword.Text; }
        }

        #endregion
    }
}
