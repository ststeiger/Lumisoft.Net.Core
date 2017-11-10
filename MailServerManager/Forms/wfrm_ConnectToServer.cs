using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Connect to mail server settings window.
    /// </summary>
    public class wfrm_ConnectToServer : Form
    {
        private PictureBox m_pIcon           = null;
        private Label      mt_Info           = null;
        private GroupBox   m_pSeparator1     = null;
        private Label      mt_Server         = null;
        private ComboBox   m_pServer         = null;
        private Label      mt_UserName       = null;
        private TextBox    m_pUserName       = null;
        private Label      mt_Password       = null;
        private TextBox    m_pPassword       = null;
        private CheckBox   m_pSaveConnection = null;
        private GroupBox   m_pGroupbox1      = null;
        private Button     m_pCancel         = null;
        private Button     m_pOk             = null;

        private Server m_pApiServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_ConnectToServer()
        {
            InitUI();

            LoadRecentConnections();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="host">Host name or IP.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="saveConnectionEnabled">Specifies if save connection checkbox enabled.</param>
        public wfrm_ConnectToServer(string host,string userName,string password,bool saveConnectionEnabled)
        {
            InitUI();

            m_pServer.Text            = host;
            m_pUserName.Text          = userName;
            m_pPassword.Text          = password;
            m_pSaveConnection.Enabled = saveConnectionEnabled;

            LoadRecentConnections();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,203);
            this.StartPosition = FormStartPosition.CenterScreen;            
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Connect to Server";
            this.Icon = ResManager.GetIcon("connect32.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("connect32.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify connection parameters.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(380,3);
            m_pSeparator1.Location = new Point(7,50);

            mt_Server = new Label();
            mt_Server.Size = new Size(100,20);
            mt_Server.Location = new Point(5,60);
            mt_Server.TextAlign = ContentAlignment.MiddleLeft;
            mt_Server.Text = "Server:";

            m_pServer = new ComboBox();
            m_pServer.Size = new Size(225,20);
            m_pServer.Location = new Point(155,60);

            mt_UserName = new Label();
            mt_UserName.Size = new Size(100,20);
            mt_UserName.Location = new Point(5,85);
            mt_UserName.TextAlign = ContentAlignment.MiddleLeft;
            mt_UserName.Text = "User Name:";

            m_pUserName = new TextBox();
            m_pUserName.Size = new Size(225,20);
            m_pUserName.Location = new Point(155,85);
            m_pUserName.Text = "Administrator";

            mt_Password = new Label();
            mt_Password.Size = new Size(100,20);
            mt_Password.Location = new Point(5,110);
            mt_Password.TextAlign = ContentAlignment.MiddleLeft;
            mt_Password.Text = "Password:";

            m_pPassword = new TextBox();
            m_pPassword.Size = new Size(225,20);
            m_pPassword.Location = new Point(155,110);
            m_pPassword.PasswordChar = '*';

            m_pSaveConnection = new CheckBox();            
            m_pSaveConnection.Size = new Size(220,20);
            m_pSaveConnection.Location = new Point(155,135);
            m_pSaveConnection.Text = "Save Connection";

            m_pGroupbox1 = new GroupBox();
            m_pGroupbox1.Size = new Size(380,3);
            m_pGroupbox1.Location = new Point(7,160);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(235,175);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(310,175);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_Server);
            this.Controls.Add(m_pServer);
            this.Controls.Add(mt_UserName);
            this.Controls.Add(m_pUserName);
            this.Controls.Add(mt_Password);
            this.Controls.Add(m_pPassword);
            this.Controls.Add(m_pSaveConnection);
            this.Controls.Add(m_pGroupbox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                
        #endregion


        #region Events Handling

        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
        
        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            try{
                string host = m_pServer.Text;
                if(host == ""){
                    host = "localhost";
                }

                Server server = new Server();
                server.Connect(host,m_pUserName.Text,m_pPassword.Text);
                m_pApiServer = server;

                SaveRecentConnections();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch(Exception x){
                MessageBox.Show(this,"Error connecting to server:\r\n\t" + x.Message,"Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        #endregion

        #endregion


        #region method LoadRecentConnections

        /// <summary>
        /// Loads recent connections to UI.
        /// </summary>
        private void LoadRecentConnections()
        {
            if(File.Exists(Application.StartupPath + "/Settings/RecentConnections.txt")){                
                string[] connections =  File.ReadAllText(Application.StartupPath + "/Settings/RecentConnections.txt",System.Text.Encoding.UTF8).Replace("\r\n","\n").Split('\n');
                foreach(string connection in connections){
                    if(connection != ""){
                        m_pServer.Items.Add(connection);
                    }
                }
            }
        }

        #endregion

        #region method SaveRecentConnections

        /// <summary>
        /// Saves recent connections to txt file, if new recent connection.
        /// </summary>
        private void SaveRecentConnections()
        {
            if(!m_pServer.Items.Contains(m_pServer.Text)){
                if(m_pServer.Items.Count > 25){
                    m_pServer.Items.RemoveAt(m_pServer.Items.Count - 1);
                }

                m_pServer.Items.Insert(0,m_pServer.Text);

                string data = "";
                foreach(string item in m_pServer.Items){
                    data += item + "\r\n";
                }
                File.WriteAllText(Application.StartupPath + "/Settings/RecentConnections.txt",data,System.Text.Encoding.UTF8);
            }
        }

        #endregion


        #region Properteis Implementation

        /// <summary>
        /// Gets connected server or null if not connected.
        /// </summary>
        public Server Server
        {
            get{ return m_pApiServer; }
        }

        /// <summary>
        /// Gets current server in UI.
        /// </summary>
        public string Host
        {
            get{ return m_pServer.Text; }
        }

        /// <summary>
        /// Gets current user name in UI.
        /// </summary>
        public string UserName
        {
            get{ return m_pUserName.Text; }
        }

        /// <summary>
        /// Gets current password in UI.
        /// </summary>
        public string Password
        {
            get{ return m_pPassword.Text; }
        }

        /// <summary>
        /// Gets current save connection value in UI.
        /// </summary>
        public bool SaveConnection
        {
            get{ return m_pSaveConnection.Checked; }
        }

        #endregion

    }
}
