using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// User remote server Add/Edit windows.
    /// </summary>
    public class wfrm_User_RemoteServers_Server :Form
    {
        private PictureBox    m_pIcon        = null;
        private Label         mt_Info        = null;
        private GroupBox      m_pSeparator1  = null;
        private CheckBox      m_pEnabled     = null;
        private Label         mt_Description = null;
        private TextBox       m_pDescription = null;
        private Label         mt_Server      = null;
        private TextBox       m_pServer      = null;
        private NumericUpDown m_pPort        = null;
        private CheckBox      m_UseSSL       = null;
        private Label         mt_User        = null;
        private TextBox       m_pUser        = null;
        private Label         mt_Password    = null;
        private TextBox       m_pPassword    = null;
        private GroupBox      m_pSeparator2  = null;
        private Button        m_pCancel      = null;
        private Button        m_pOk          = null;

        private User             m_pOwnerUser    = null;
        private UserRemoteServer m_pRemoteServer = null;

        /// <summary>
        /// Add new constructor.
        /// </summary>
        /// <param name="user">Owner user.</param>
        public wfrm_User_RemoteServers_Server(User user)
        {
            m_pOwnerUser  = user;

            InitUI();
        }

        //// <summary>
        /// Edit new constructor.
        /// </summary>
        /// <param name="user">Owner user.</param>
        /// <param name="remoteServer">User remote server to update.</param>
        public wfrm_User_RemoteServers_Server(User user,UserRemoteServer remoteServer)
        {
            m_pRemoteServer = remoteServer;

            InitUI();

            m_pDescription.Text = remoteServer.Description;
            m_pServer.Text = remoteServer.Host;
            m_pPort.Value = remoteServer.Port;
            m_pUser.Text = remoteServer.UserName;
            m_pPassword.Text = remoteServer.Password;
            m_UseSSL.Checked = remoteServer.SSL;
            m_pEnabled.Checked = remoteServer.Enabled;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,258);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Add/Edit User Remote Server";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("remoteserver32.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify remote mail server information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(383,3);
            m_pSeparator1.Location = new Point(7,50);

            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(200,20);
            m_pEnabled.Location = new Point(105,65);
            m_pEnabled.Checked = true;
            m_pEnabled.Text = "Enabled";

            mt_Description = new Label();
            mt_Description.Size = new Size(100,20);
            mt_Description.Location = new Point(0,90);
            mt_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(280,20);
            m_pDescription.Location = new Point(105,90);

            mt_Server = new Label();
            mt_Server.Size = new Size(100,20);
            mt_Server.Location = new Point(0,115);
            mt_Server.TextAlign = ContentAlignment.MiddleRight;
            mt_Server.Text = "Server:";

            m_pServer = new TextBox();
            m_pServer.Size = new Size(215,20);
            m_pServer.Location = new Point(105,115);

            m_pPort = new NumericUpDown();
            m_pPort.Size = new Size(60,20);
            m_pPort.Location = new Point(325,115);
            m_pPort.Minimum = 1;
            m_pPort.Maximum = 99999;
            m_pPort.Value = 110;

            m_UseSSL = new CheckBox();
            m_UseSSL.Size = new Size(200,20);
            m_UseSSL.Location = new Point(105,140);
            m_UseSSL.Text = "Connect via SSL";
            m_UseSSL.CheckedChanged += new EventHandler(m_UseSSL_CheckedChanged);

            mt_User = new Label();
            mt_User.Size = new Size(100,20);
            mt_User.Location = new Point(0,165);
            mt_User.TextAlign = ContentAlignment.MiddleRight;
            mt_User.Text = "User:";

            m_pUser = new TextBox();
            m_pUser.Size = new Size(280,20);
            m_pUser.Location = new Point(105,165);

            mt_Password = new Label();
            mt_Password.Size = new Size(100,20);
            mt_Password.Location = new Point(0,190);
            mt_Password.TextAlign = ContentAlignment.MiddleRight;
            mt_Password.Text = "Password:";

            m_pPassword = new TextBox();
            m_pPassword.Size = new Size(280,20);
            m_pPassword.Location = new Point(105,190);
            m_pPassword.PasswordChar = '*';
                                                
            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(383,3);
            m_pSeparator2.Location = new Point(7,225);

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
            this.Controls.Add(m_pEnabled);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pDescription);
            this.Controls.Add(mt_Server);
            this.Controls.Add(m_pServer);
            this.Controls.Add(m_pPort);
            this.Controls.Add(m_UseSSL);
            this.Controls.Add(mt_User);
            this.Controls.Add(m_pUser);
            this.Controls.Add(mt_Password);
            this.Controls.Add(m_pPassword);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                
        #endregion


        #region Events Handling

        #region method m_UseSSL_CheckedChanged

        private void m_UseSSL_CheckedChanged(object sender, EventArgs e)
        {
            if(m_UseSSL.Checked){
                if(m_pPort.Value == 110){
                    m_pPort.Value = 995;
                }
            }
            else{
                if(m_pPort.Value == 995){
                    m_pPort.Value = 110;
                }
            }
        }

        #endregion



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
            //--- Validate values ---------------------------//
            if(m_pServer.Text == ""){
                MessageBox.Show(this,"Please fill Server !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if(m_pUser.Text == ""){
                MessageBox.Show(this,"Please fill User !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //----------------------------------------------//
            
            // Add new user reomte server
            if(m_pRemoteServer == null){
                m_pRemoteServer = m_pOwnerUser.RemoteServers.Add(
                    m_pDescription.Text,
                    m_pServer.Text,
                    (int)m_pPort.Value,
                    m_UseSSL.Checked,
                    m_pUser.Text,
                    m_pPassword.Text,
                    m_pEnabled.Checked                    
               );
            }
            // Update user remote server
            else{
                m_pRemoteServer.Enabled     = m_pEnabled.Checked;
                m_pRemoteServer.Description = m_pDescription.Text;                
                m_pRemoteServer.Host        = m_pServer.Text;
                m_pRemoteServer.Port        = (int)m_pPort.Value;
                m_pRemoteServer.SSL         = m_UseSSL.Checked;
                m_pRemoteServer.UserName    = m_pUser.Text;
                m_pRemoteServer.Password    = m_pPassword.Text;
                m_pRemoteServer.Commit();
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets remote server ID.
        /// </summary>
        public string RemoteServerID
        {
            get{ 
                if(m_pRemoteServer != null){
                    return m_pRemoteServer.ID;
                }
                else{
                    return ""; 
                }
            }
        }

        #endregion

    }
}
