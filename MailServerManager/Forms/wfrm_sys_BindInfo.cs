using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;

using LumiSoft.Net;
using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Bind info window.
    /// </summary>
    public class wfrm_sys_BindInfo : Form
    {
        private PictureBox    m_pIcon       = null;
        private Label         mt_Info       = null;
        private GroupBox      m_pSeparator1 = null;
        private Label         mt_HostName   = null;
        private TextBox       m_pHostName   = null;
        private Label         mt_Protocol   = null;
        private ComboBox      m_pProtocol   = null;
        private Label         mt_IpEndPoint = null;
        private ComboBox      m_pIP         = null;
        private NumericUpDown m_pPort       = null;
        private Label         mt_SslMode    = null;
        private ComboBox      m_pSslMode    = null;
        private ToolStrip     m_pSslToolbar = null;
        private PictureBox    m_pSslIcon    = null;
        private GroupBox      m_pSeparator2 = null;
        private Button        m_pCancel     = null;
        private Button        m_pOk         = null;

        private bool             m_SslEnabled     = true;
        private int              m_DefaultPort    = 10000;
        private int              m_DefaultSSLPort = 10001;
        private X509Certificate2 m_pCert          = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Reference to server.</param> 
        /// <param name="defaultPort">Specifies default port.</param>
        /// <param name="defaultSSLPort">Specifies default SSL port.</param>
        public wfrm_sys_BindInfo(Server server,int defaultPort,int defaultSSLPort) : this(server,false,defaultPort,defaultSSLPort)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Reference to server.</param>
        /// <param name="allowUDP">Specifies if UDP protocol is enabled.</param>
        /// <param name="defaultPort">Specifies default port.</param>
        /// <param name="defaultSSLPort">Specifies default SSL port.</param>
        public wfrm_sys_BindInfo(Server server,bool allowUDP,int defaultPort,int defaultSSLPort) : this(server,allowUDP,defaultPort,defaultSSLPort,null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Reference to server.</param>
        /// <param name="allowUDP">Specifies if UDP protocol is enabled.</param>
        /// <param name="defaultPort">Specifies default port.</param>
        /// <param name="defaultSSLPort">Specifies default SSL port.</param>
        /// <param name="bindInfo">Bind info.</param>
        public wfrm_sys_BindInfo(Server server,bool allowUDP,int defaultPort,int defaultSSLPort,IPBindInfo bindInfo) : this(server,allowUDP,true,true,defaultPort,defaultSSLPort,bindInfo)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Reference to server.</param>
        /// <param name="allowUDP">Specifies if UDP protocol is enabled.</param>
        /// <param name="allowSSL">Specifies if SSL is enabled.</param>
        /// <param name="allowChangePort">Specifies if port can be changed.</param>
        /// <param name="defaultPort">Specifies default port.</param>
        /// <param name="defaultSSLPort">Specifies default SSL port.</param>
        /// <param name="bindInfo">Bind info.</param>
        public wfrm_sys_BindInfo(Server server,bool allowUDP,bool allowSSL,bool allowChangePort,int defaultPort,int defaultSSLPort,IPBindInfo bindInfo)
        {
            m_SslEnabled     = allowSSL;
            m_DefaultPort    = defaultPort;
            m_DefaultSSLPort = defaultSSLPort;

            InitUI();
                        
            m_pSslMode.SelectedIndex = 0;
            if(!allowSSL){
                m_pSslMode.Enabled = false;
            }
            if(!allowChangePort){
                m_pPort.Enabled = false;
            }

            if(bindInfo != null){
                m_pHostName.Text = bindInfo.HostName;
            }

            m_pProtocol.Items.Add("TCP");
            if(allowUDP){
                m_pProtocol.Items.Add("UDP");
            }
            if(bindInfo == null){
                m_pProtocol.SelectedIndex = 0;
            }
            else{
                m_pProtocol.Text = bindInfo.Protocol.ToString();
            }

            foreach(IPAddress ip in server.IPAddresses){
                m_pIP.Items.Add(new WComboBoxItem(IpToString(ip),ip));
            }

            if(bindInfo == null){
                m_pIP.SelectedIndex = 0;
                m_pPort.Value = defaultPort;
            }
            else{
                m_pCert = bindInfo.Certificate;

                m_pIP.Text = IpToString(bindInfo.IP);
                m_pPort.Value = bindInfo.Port;
                m_pSslMode.Text = bindInfo.SslMode.ToString();                           
            }

            UpdateCertStatus();
        }                

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(380,210);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add/Edit Bind info";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("server.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify IP binding information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(365,3);
            m_pSeparator1.Location = new Point(10,50);

            mt_HostName = new Label();
            mt_HostName.Size = new Size(100,20);
            mt_HostName.Location = new Point(0,60);
            mt_HostName.TextAlign = ContentAlignment.MiddleRight;
            mt_HostName.Text = "Host Name:";

            m_pHostName = new TextBox();
            m_pHostName.Size = new Size(270,20);
            m_pHostName.Location = new Point(105,60);

            mt_Protocol = new Label();
            mt_Protocol.Size = new Size(100,20);
            mt_Protocol.Location = new Point(0,85);
            mt_Protocol.TextAlign = ContentAlignment.MiddleRight;
            mt_Protocol.Text = "Protocol:";

            m_pProtocol = new ComboBox();
            m_pProtocol.Size = new Size(60,20);
            m_pProtocol.Location = new Point(105,85);
            m_pProtocol.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pProtocol.SelectedIndexChanged += new EventHandler(m_pProtocol_SelectedIndexChanged);

            mt_IpEndPoint = new Label();
            mt_IpEndPoint.Size = new Size(100,20);
            mt_IpEndPoint.Location = new Point(0,110);
            mt_IpEndPoint.TextAlign = ContentAlignment.MiddleRight;
            mt_IpEndPoint.Text = "IP EndPoint:";

            m_pIP = new ComboBox();
            m_pIP.Size = new Size(200,20);
            m_pIP.Location = new Point(105,110);
            m_pIP.DropDownStyle = ComboBoxStyle.DropDownList;

            m_pPort = new NumericUpDown();
            m_pPort.Size = new Size(63,20);
            m_pPort.Location = new Point(310,110);
            m_pPort.Minimum = 0;
            m_pPort.Maximum = 99999;
            
            mt_SslMode = new Label();
            mt_SslMode.Size = new Size(100,20);
            mt_SslMode.Location = new Point(0,140);
            mt_SslMode.TextAlign = ContentAlignment.MiddleRight;
            mt_SslMode.Text = "SSL Mode:";

            m_pSslMode = new ComboBox();
            m_pSslMode.Size = new Size(60,20);
            m_pSslMode.Location = new Point(105,140);
            m_pSslMode.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pSslMode.SelectedIndexChanged += new EventHandler(m_pSslMode_SelectedIndexChanged);
            m_pSslMode.Items.Add("None");
            m_pSslMode.Items.Add("SSL");
            m_pSslMode.Items.Add("TLS");

            m_pSslToolbar = new ToolStrip();
            m_pSslToolbar.Size = new Size(95,25);
            m_pSslToolbar.Location = new Point(210,140);
            m_pSslToolbar.Dock = DockStyle.None;
            m_pSslToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pSslToolbar.BackColor = this.BackColor;
            m_pSslToolbar.Renderer = new ToolBarRendererEx();
            m_pSslToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pSslToolbar_ItemClicked);
            // Create button
            ToolStripButton button_Create = new ToolStripButton();
            button_Create.Image = ResManager.GetIcon("write.ico").ToBitmap();
            button_Create.Name = "create";
            button_Create.ToolTipText = "Create SSL certificate.";
            m_pSslToolbar.Items.Add(button_Create); 
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Name = "add";
            button_Add.ToolTipText = "Add SSL certificate.";
            m_pSslToolbar.Items.Add(button_Add);            
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Name = "delete";
            button_Delete.ToolTipText = "Delete SSL certificate.";
            m_pSslToolbar.Items.Add(button_Delete);
            // Save button
            ToolStripButton button_Save = new ToolStripButton();
            button_Save.Enabled = false;
            button_Save.Image = ResManager.GetIcon("save.ico").ToBitmap();
            button_Save.Name = "save";
            button_Save.ToolTipText = "Export SSL certificate.";
            m_pSslToolbar.Items.Add(button_Save);

            m_pSslIcon = new PictureBox();
            m_pSslIcon.Size = new Size(32,32);
            m_pSslIcon.Location = new Point(180,135);
            m_pSslIcon.BorderStyle = BorderStyle.None;
            m_pSslIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            m_pSslIcon.Image = UI_Utils.GetGrayImage(ResManager.GetIcon("ssl.ico",new Size(32,32)).ToBitmap());

            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(365,2);
            m_pSeparator2.Location = new Point(5,175);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(225,185);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(300,185);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_HostName);
            this.Controls.Add(m_pHostName);
            this.Controls.Add(mt_Protocol);
            this.Controls.Add(m_pProtocol);
            this.Controls.Add(mt_IpEndPoint);
            this.Controls.Add(m_pIP);
            this.Controls.Add(m_pPort);
            this.Controls.Add(mt_SslMode);
            this.Controls.Add(m_pSslMode);
            this.Controls.Add(m_pSslToolbar);
            this.Controls.Add(m_pSslIcon);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                                                                                
        #endregion


        #region Events Handling

        #region method m_pProtocol_SelectedIndexChanged

        private void m_pProtocol_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_SslEnabled && m_pProtocol.SelectedItem.ToString() == "TCP"){
                m_pSslMode.Enabled = true;
            }
            else{
                m_pSslMode.Enabled = false;
            }
        }

        #endregion

        #region method m_pSslMode_SelectedIndexChanged

        private void m_pSslMode_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pSslMode.SelectedIndex > 0){
                m_pSslToolbar.Enabled = true;
                m_pSslIcon.Enabled = true;
            }
            else{
                m_pSslToolbar.Enabled = false;
                m_pSslIcon.Enabled = false;
            }

            if(m_pSslMode.SelectedItem.ToString() == "SSL"){
                m_pPort.Value = m_DefaultSSLPort;
            }
            else{
                m_pPort.Value = m_DefaultPort;
            }
        }

        #endregion

        #region method m_pSslToolbar_ItemClicked

        private void m_pSslToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Name == null){
            }
            else if(e.ClickedItem.Name == "create"){
                wfrm_sys_CreateCertificate dlgSsl = new wfrm_sys_CreateCertificate(m_pHostName.Text);
                if(dlgSsl.ShowDialog(this) == DialogResult.OK){
                    try{
                        X509Certificate2 cert = new X509Certificate2(dlgSsl.Certificate,"",X509KeyStorageFlags.Exportable);
                        if(!cert.HasPrivateKey){
                            MessageBox.Show(this,"Certificate is not server certificate, private key is missing !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            return;
                        }

                        m_pCert = cert;

                        UpdateCertStatus();
                    }
                    catch{
                        MessageBox.Show(this,"Invalid or not supported certificate file !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            else if(e.ClickedItem.Name == "add"){
                OpenFileDialog dlg = new OpenFileDialog();
                if(dlg.ShowDialog(this) == DialogResult.OK){
                    try{
                        X509Certificate2 cert = new X509Certificate2(dlg.FileName,"",X509KeyStorageFlags.Exportable);
                        if(!cert.HasPrivateKey){
                            MessageBox.Show(this,"Certificate is not server certificate, private key is missing !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            return;
                        }

                        m_pCert = cert;

                        UpdateCertStatus();
                    }
                    catch{
                        MessageBox.Show(this,"Invalid or not supported certificate file !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
            else if(e.ClickedItem.Name == "delete"){
                if(MessageBox.Show(this,"Are you sure you want to delete active SSL certificate ?","Confirm delete:",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes){
                    m_pCert = null;

                    UpdateCertStatus();
                }
            }
            else if(e.ClickedItem.Name == "save"){
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "*.pfx | *.p12";
                if(dlg.ShowDialog(this) == DialogResult.OK){
                    File.WriteAllBytes(dlg.FileName,m_pCert.Export(X509ContentType.Pfx));
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
            if(m_pSslMode.SelectedIndex > 0 && m_pCert == null){
                MessageBox.Show(this,"Please load certificate !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
    
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region method UpdateCertStatus

        /// <summary>
        /// Updates certificate status and toobar.
        /// </summary>
        private void UpdateCertStatus()
        {
            if(m_pCert == null){
                UI_Utils.GetGrayImage(ResManager.GetIcon("ssl.ico",new Size(32,32)).ToBitmap());
                m_pSslToolbar.Items["create"].Enabled = true;
                m_pSslToolbar.Items["add"].Enabled = true;
                m_pSslToolbar.Items["delete"].Enabled = false;
                m_pSslToolbar.Items["save"].Enabled = false;
            }
            else{
                m_pSslIcon.Image = ResManager.GetIcon("ssl.ico",new Size(48,48)).ToBitmap();
                m_pSslToolbar.Items["create"].Enabled = false;
                m_pSslToolbar.Items["add"].Enabled = false;
                m_pSslToolbar.Items["delete"].Enabled = true;
                m_pSslToolbar.Items["save"].Enabled = true;
            }
        }

        #endregion

        #region method IpToString

        /// <summary>
        /// Converts specified IP address for UI.
        /// </summary>
        /// <param name="ip">IP address to convert.</param>
        /// <returns>Returns IP as string.</returns>
        private string IpToString(IPAddress ip)
        {
            if(ip.Equals(IPAddress.Any)){
                return "any IPv4";
            }
            else if(ip.Equals(IPAddress.Loopback)){
                return "localhost IPv4";
            }
            else if(ip.Equals(IPAddress.IPv6Any)){
                return "Any IPv6";
            }
            else if(ip.Equals(IPAddress.IPv6Loopback)){
                return "localhost IPv6";
            }
            else{
                return ip.ToString();
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets host name.
        /// </summary>
        public string HostName
        {
            get{ return m_pHostName.Text; }
        }

        /// <summary>
        /// Gets selected protocol.
        /// </summary>
        public BindInfoProtocol Protocol
        {
            get{ return (BindInfoProtocol)Enum.Parse(typeof(BindInfoProtocol),m_pProtocol.SelectedItem.ToString()); }
        }

        /// <summary>
        /// Gets selected IP address.
        /// </summary>
        public IPAddress IP
        {
            get{ return (IPAddress)((WComboBoxItem)m_pIP.SelectedItem).Tag; }
        }

        /// <summary>
        /// Gets selected port.
        /// </summary>
        public int Port
        {
            get{ return (int)m_pPort.Value; }
        }

        /// <summary>
        /// Gets SSL mode.
        /// </summary>
        public SslMode SslMode
        {
            get{
                if(m_pSslMode.SelectedIndex == 1){
                    return SslMode.SSL;
                }
                else if(m_pSslMode.SelectedIndex == 2){
                    return SslMode.TLS;
                }
                else{
                    return SslMode.None;
                }
            }
        }

        /// <summary>
        /// Gets certificate or null if no certificate loaded.
        /// </summary>
        public X509Certificate2 Certificate
        {
            get{ return m_pCert; }
        }

        #endregion

    }
}
