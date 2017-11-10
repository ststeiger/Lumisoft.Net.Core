using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Virtual server add/edit window.
    /// </summary>
    public class wfrm_VirtualServers_VirtualServer : Form
    {
        #region class XML_InitString

        /// <summary>
        /// XML API init string.
        /// </summary>
        private class XML_InitString
        {
            private string m_SettingsPath  = "";
            private string m_MailStorePath = "";
                
            /// <summary>
            /// Default constructor.
            /// </summary>
            public XML_InitString()
            {
            }

            /// <summary>
            /// Parse constructor.
            /// </summary>
            /// <param name="initString">XML API init string.</param>
            public XML_InitString(string initString)
            {
                // datapath=
			    // mailstorepath=
			    string[] parameters = initString.Replace("\r\n","\n").Split('\n');
			    foreach(string param in parameters){
				    if(param.ToLower().IndexOf("datapath=") > -1){
					    m_SettingsPath = param.Substring(9);
				    }
				    else if(param.ToLower().IndexOf("mailstorepath=") > -1){
					    m_MailStorePath = param.Substring(14);
				    }
			    }
            }


            #region override method ToString

            /// <summary>
            /// Returns APi init string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "datapath=" + m_SettingsPath + "\r\nmailstorepath=" + m_MailStorePath;
            }

            #endregion


            #region Properties Implementation

            /// <summary>
            /// Gets or sets settings path.
            /// </summary>
            [Description("Settings location.")]
            public string SettingsPath
            {
                get{ return m_SettingsPath; }

                set{ m_SettingsPath = value; }
            }

            /// <summary>
            /// Gets or sets mail store path.
            /// </summary>
            [Description("Mail store location.")]
            public string MailStorePath
            {
                get{ return m_MailStorePath; }

                set{ m_MailStorePath = value; }
            }

            #endregion

        }

        #endregion

        #region class MSSQL_InitString

        /// <summary>
        /// MSSQL API init string.
        /// </summary>
        private class MSSQL_InitString
        {
            private string m_SqlConStr     = "";
            private string m_MailStorePath = "";
                        
            /// <summary>
            /// Default constructor.
            /// </summary>
            public MSSQL_InitString()
            {
                m_SqlConStr = "server=localhost;uid=sa;pwd=;database=lsMailServer";
            }

            /// <summary>
            /// Parse constructor.
            /// </summary>
            /// <param name="initString">MSSQL API init string.</param>
            public MSSQL_InitString(string initString)
            {
                // connectionstring=
			    string[] parameters = initString.Replace("\r\n","\n").Split('\n');
			    foreach(string param in parameters){
				    if(param.ToLower().IndexOf("connectionstring=") > -1){
					    m_SqlConStr = param.Substring(17);
				    }
				    else if(param.ToLower().IndexOf("mailstorepath=") > -1){
					    m_MailStorePath = param.Substring(14);
				    }
			    }
            }


            #region override method ToString

            /// <summary>
            /// Returns APi init string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "connectionstring=" + m_SqlConStr + "\r\nmailstorepath=" + m_MailStorePath;
            }

            #endregion


            #region Properties Implementation

            /// <summary>
            /// Gets or sets settings path.
            /// </summary>
            [Description("SQL connection string.")]
            public string SqlConnectionString
            {
                get{ return m_SqlConStr; }

                set{ m_SqlConStr = value; }
            }

            /// <summary>
            /// Gets or sets mail store path.
            /// </summary>
            [Description("Mail store location.")]
            public string MailStorePath
            {
                get{ return m_MailStorePath; }

                set{ m_MailStorePath = value; }
            }

            #endregion

        }

        #endregion

        #region class PGSQL_InitString

        /// <summary>
        /// Postgre SQL API init string.
        /// </summary>
        private class PGSQL_InitString
        {
            private string m_SqlConStr     = "";
            private string m_MailStorePath = "";
                        
            /// <summary>
            /// Default constructor.
            /// </summary>
            public PGSQL_InitString()
            {
                m_SqlConStr = "Server=127.0.0.1;User Id=user;Password=;Database=lsMailServer;";
            }

            /// <summary>
            /// Parse constructor.
            /// </summary>
            /// <param name="initString">MSSQL API init string.</param>
            public PGSQL_InitString(string initString)
            {
                // connectionstring=
			    string[] parameters = initString.Replace("\r\n","\n").Split('\n');
			    foreach(string param in parameters){
				    if(param.ToLower().IndexOf("connectionstring=") > -1){
					    m_SqlConStr = param.Substring(17);
				    }
				    else if(param.ToLower().IndexOf("mailstorepath=") > -1){
					    m_MailStorePath = param.Substring(14);
				    }
			    }
            }


            #region override method ToString

            /// <summary>
            /// Returns APi init string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "connectionstring=" + m_SqlConStr + "\r\nmailstorepath=" + m_MailStorePath;
            }

            #endregion


            #region Properties Implementation

            /// <summary>
            /// Gets or sets settings path.
            /// </summary>
            [Description("SQL connection string.")]
            public string SqlConnectionString
            {
                get{ return m_SqlConStr; }

                set{ m_SqlConStr = value; }
            }

            /// <summary>
            /// Gets or sets mail store path.
            /// </summary>
            [Description("Mail store location.")]
            public string MailStorePath
            {
                get{ return m_MailStorePath; }

                set{ m_MailStorePath = value; }
            }

            #endregion

        }

        #endregion

        private CheckBox     m_pEnabled      = null;
        private Label        mt_Name         = null;
        private TextBox      m_pName         = null;
        private Label        mt_Assembly     = null;
        private TextBox      m_pAssembly     = null;
        private Button       m_pGetAPI       = null;
        private Label        mt_Type         = null;
        private TextBox      m_pType         = null;
        private GroupBox     m_pGroupbox1    = null;
        private PropertyGrid m_pPropertyGrid = null;
        private Button       m_pCancel       = null;
        private Button       m_pOk           = null;

        private Server        m_pServer        = null;
        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Add new constructor.
        /// </summary>
        /// <param name="server">Mail server.</param>
        public wfrm_VirtualServers_VirtualServer(Server server)
        {
            m_pServer = server;

            InitUI();
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="server">Mail server.</param>
        /// <param name="virtualServer">Virtual server to update.</param>
        public wfrm_VirtualServers_VirtualServer(Server server,VirtualServer virtualServer)
        {
            m_pServer        = server;
            m_pVirtualServer = virtualServer;

            InitUI();

            m_pGetAPI.Enabled = false;
            m_pEnabled.Checked = virtualServer.Enabled;
            m_pName.Text       = virtualServer.Name;
            m_pAssembly.Text   = virtualServer.AssemblyName;
            m_pType.Text       = virtualServer.TypeName;
            if(m_pAssembly.Text.ToLower() == "xml_api.dll"){
                m_pPropertyGrid.SelectedObject = new XML_InitString(virtualServer.InitString);
            }
            else if(m_pAssembly.Text.ToLower() == "mssql_api.dll"){
                m_pPropertyGrid.SelectedObject = new MSSQL_InitString(virtualServer.InitString);
            }                
            else if(m_pAssembly.Text.ToLower() == "pgsql_api.dll"){
                m_pPropertyGrid.SelectedObject = new PGSQL_InitString(virtualServer.InitString);
            }
            else{
                MessageBox.Show("TODO:");
            }
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(442,373);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Add/Edit virtual server";

            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(280,20);
            m_pEnabled.Location = new Point(105,15);
            m_pEnabled.Text = "Enabled";
            m_pEnabled.Checked = true;

            mt_Name = new Label();
            mt_Name.Size = new Size(100,20);
            mt_Name.Location = new Point(0,45);
            mt_Name.TextAlign = ContentAlignment.MiddleRight;
            mt_Name.Text = "Name:";

            m_pName = new TextBox();
            m_pName.Size = new Size(330,20);
            m_pName.Location = new Point(105,45);

            mt_Assembly = new Label();
            mt_Assembly.Size = new Size(100,20);
            mt_Assembly.Location = new Point(0,70);
            mt_Assembly.TextAlign = ContentAlignment.MiddleRight;
            mt_Assembly.Text = "Assembly:";

            m_pAssembly = new TextBox();
            m_pAssembly.Size = new Size(300,20);
            m_pAssembly.Location = new Point(105,70);
            m_pAssembly.ReadOnly = true;

            m_pGetAPI = new Button();
            m_pGetAPI.Size = new Size(25,20);
            m_pGetAPI.Location = new Point(410,70);
            m_pGetAPI.Text = "...";
            m_pGetAPI.Click += new EventHandler(m_pGetAPI_Click);

            mt_Type = new Label();
            mt_Type.Size = new Size(100,20);
            mt_Type.Location = new Point(0,95);
            mt_Type.TextAlign = ContentAlignment.MiddleRight;
            mt_Type.Text = "Type:";

            m_pType = new TextBox();
            m_pType.Size = new Size(330,20);
            m_pType.Location = new Point(105,95);
            m_pType.ReadOnly = true;

            m_pGroupbox1 = new GroupBox();
            m_pGroupbox1.Size = new Size(435,3);
            m_pGroupbox1.Location = new Point(9,130);

            m_pPropertyGrid = new PropertyGrid();
            m_pPropertyGrid.Size = new Size(425,200);
            m_pPropertyGrid.Location = new Point(10,135);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(290,350);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(365,350);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pEnabled);
            this.Controls.Add(mt_Name);
            this.Controls.Add(m_pName);
            this.Controls.Add(mt_Assembly);
            this.Controls.Add(m_pAssembly);
            this.Controls.Add(m_pGetAPI);
            this.Controls.Add(mt_Type);
            this.Controls.Add(m_pType);
            this.Controls.Add(m_pGroupbox1);
            this.Controls.Add(m_pPropertyGrid);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                
        #endregion


        #region Events Handling

        #region method m_pGetAPI_Click

        private void m_pGetAPI_Click(object sender, EventArgs e)
        {
            wfrm_se_VirtualServerAPI frm = new wfrm_se_VirtualServerAPI(m_pServer);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pAssembly.Text = frm.AssemblyName;
                m_pType.Text     = frm.TypeName;

                if(m_pAssembly.Text.ToLower() == "xml_api.dll"){
                    m_pPropertyGrid.SelectedObject = new XML_InitString();
                }
                else if(m_pAssembly.Text.ToLower() == "mssql_api.dll"){
                    m_pPropertyGrid.SelectedObject = new MSSQL_InitString();
                }                
                else if(m_pAssembly.Text.ToLower() == "pgsql_api.dll"){
                    m_pPropertyGrid.SelectedObject = new PGSQL_InitString();
                }
                else{
                    MessageBox.Show("TODO:");
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
            //--- Validate values --------------------------------------------------------------------//
            if(m_pName.Text == ""){
                MessageBox.Show("Virtual server name cannot be empty !","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);					
			    return;
            }
            if(m_pAssembly.Text == ""){
                MessageBox.Show("Please select API assembly and Type !","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);					
			    return;
            }
            //----------------------------------------------------------------------------------------//

            this.Cursor = Cursors.WaitCursor;

            // Add new virtual server
            if(m_pVirtualServer == null){
                m_pServer.VirtualServers.Add(
                    m_pEnabled.Checked,
                    m_pName.Text,
                    m_pAssembly.Text,
                    m_pType.Text,
                    m_pPropertyGrid.SelectedObject.ToString()
                );
            }
            else{
                m_pVirtualServer.Enabled    = m_pEnabled.Checked;
                m_pVirtualServer.Name       = m_pName.Text;
                m_pVirtualServer.InitString = m_pPropertyGrid.SelectedObject.ToString();
                m_pVirtualServer.Commit();
            }

            this.Cursor = Cursors.Default;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion

    }
}
