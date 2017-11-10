using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// IP security add/edit IP security entry window.
    /// </summary>
    public class wfrm_Security_IPSecurityEntry : Form
    {
        private CheckBox m_pEnabled     = null;
        private Label    mt_Description = null;
        private TextBox  m_pDescription = null;
        private Label    mt_Service     = null;
        private ComboBox m_pService     = null;
        private Label    mt_Action      = null;
        private ComboBox m_pAction      = null;
        private Label    mt_Type        = null;
        private ComboBox m_pType        = null;
        private Label    mt_StartIP     = null;
        private TextBox  m_pStartIP     = null;
        private Label    mt_EndIP       = null;
        private TextBox  m_pEndIP       = null;
        private GroupBox m_pGroupbox1   = null;
        private Button   m_pCancel      = null;
        private Button   m_pOk          = null;
        
        private VirtualServer m_pVirtualServer = null;
        private IPSecurity    m_pSecurityEntry = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_Security_IPSecurityEntry(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            m_pService.SelectedIndex = 0;
            m_pAction.SelectedIndex = 0;
            m_pType.SelectedIndex = 0;
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="securityEntry">Security entry to update.</param>
        public wfrm_Security_IPSecurityEntry(VirtualServer virtualServer,IPSecurity securityEntry)
        {
            m_pVirtualServer = virtualServer;
            m_pSecurityEntry = securityEntry;

            InitUI();

            m_pEnabled.Checked = securityEntry.Enabled;
            m_pDescription.Text = securityEntry.Description;
            m_pService.SelectedIndex = (int)securityEntry.Service - 1;
            m_pAction.SelectedIndex = (int)securityEntry.Action - 1;
            if(securityEntry.StartIP.Equals(securityEntry.EndIP)){
                m_pType.SelectedIndex = 0;
            }
            else{
                m_pType.SelectedIndex = 1;
            }
            m_pStartIP.Text = securityEntry.StartIP.ToString();
            m_pEndIP.Text = securityEntry.EndIP.ToString();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,243);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Add/Edit IP security entry";

            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(100,20);
            m_pEnabled.Location = new Point(115,20);
            m_pEnabled.Text = "Enabled";
            m_pEnabled.Checked = true;

            mt_Description = new Label();
            mt_Description.Size = new Size(100,18);
            mt_Description.Location = new Point(10,45);
            mt_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(270,20);
            m_pDescription.Location = new Point(115,45);

            mt_Service = new Label();
            mt_Service.Size = new Size(100,18);
            mt_Service.Location = new Point(10,70);
            mt_Service.TextAlign = ContentAlignment.MiddleRight;
            mt_Service.Text = "Service:";

            m_pService = new ComboBox();
            m_pService.Size = new Size(100,20);
            m_pService.Location = new Point(115,70);
            m_pService.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pService.Items.Add(new WComboBoxItem("SMTP",Service_enum.SMTP));
            m_pService.Items.Add(new WComboBoxItem("POP3",Service_enum.POP3));
            m_pService.Items.Add(new WComboBoxItem("IMAP",Service_enum.IMAP));
            m_pService.Items.Add(new WComboBoxItem("SMTP Relay",Service_enum.Relay));

            mt_Action = new Label();
            mt_Action.Size = new Size(100,18);
            mt_Action.Location = new Point(10,95);
            mt_Action.TextAlign = ContentAlignment.MiddleRight;
            mt_Action.Text = "Action:";

            m_pAction = new ComboBox();
            m_pAction.Size = new Size(100,20);
            m_pAction.Location = new Point(115,95);
            m_pAction.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pAction.Items.Add(new WComboBoxItem("Allow",IPSecurityAction_enum.Allow));
            m_pAction.Items.Add(new WComboBoxItem("Deny",IPSecurityAction_enum.Deny));

            mt_Type = new Label();
            mt_Type.Size = new Size(100,18);
            mt_Type.Location = new Point(10,120);
            mt_Type.TextAlign = ContentAlignment.MiddleRight;
            mt_Type.Text = "Type:";

            m_pType = new ComboBox();
            m_pType.Size = new Size(100,20);
            m_pType.Location = new Point(115,120);
            m_pType.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pType.SelectedIndexChanged += new EventHandler(m_pType_SelectedIndexChanged);
            m_pType.Items.Add("IP");
            m_pType.Items.Add("IP Range");

            mt_StartIP = new Label();
            mt_StartIP.Size = new Size(100,18);
            mt_StartIP.Location = new Point(10,145);
            mt_StartIP.TextAlign = ContentAlignment.MiddleRight;
            mt_StartIP.Text = "Start IP:";

            m_pStartIP = new TextBox();
            m_pStartIP.Size = new Size(270,20);
            m_pStartIP.Location = new Point(115,145);

            mt_EndIP = new Label();
            mt_EndIP.Size = new Size(100,18);
            mt_EndIP.Location = new Point(10,170);
            mt_EndIP.TextAlign = ContentAlignment.MiddleRight;
            mt_EndIP.Text = "End IP:";

            m_pEndIP = new TextBox();
            m_pEndIP.Size = new Size(270,20);
            m_pEndIP.Location = new Point(115,170);

            m_pGroupbox1 = new GroupBox();
            m_pGroupbox1.Size = new Size(390,3);
            m_pGroupbox1.Location = new Point(3,205);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(235,220);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(310,220);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pEnabled);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pDescription);
            this.Controls.Add(mt_Service);
            this.Controls.Add(m_pService);
            this.Controls.Add(mt_Action);
            this.Controls.Add(m_pAction);
            this.Controls.Add(mt_Type);
            this.Controls.Add(m_pType);
            this.Controls.Add(mt_StartIP);
            this.Controls.Add(m_pStartIP);
            this.Controls.Add(mt_EndIP);
            this.Controls.Add(m_pEndIP);
            this.Controls.Add(m_pGroupbox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                
        #endregion


        #region Events Handling

        #region method m_pType_SelectedIndexChanged

        private void m_pType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pType.SelectedIndex == 0){
                m_pEndIP.Enabled = false;
            }
            else if(m_pType.SelectedIndex == 1){
                m_pEndIP.Enabled = true;
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
            IPAddress startIP = null;
            IPAddress endIP   = null;

            //--- Validate values --------------------------------------------------------------------------------//
            if(m_pDescription.Text == ""){
                MessageBox.Show(this,"Please fill description !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            try{
                startIP = IPAddress.Parse(m_pStartIP.Text);

                if(m_pType.SelectedIndex == 0){
                    endIP = startIP;
                }
                else{
                    try{
                        endIP = IPAddress.Parse(m_pEndIP.Text);
                    }
                    catch{
                        MessageBox.Show(this,"Invalid end IP value !","Invalid IP value",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch{
                MessageBox.Show(this,"Invalid start IP value !","Invalid IP value",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            if(startIP.AddressFamily != endIP.AddressFamily){
                MessageBox.Show(this,"Start IP and End IP must be from same address familily, you can't mix IPv4 and IPv6 addresses !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //-----------------------------------------------------------------------------------------------------//
            
            // Add new security entry
            if(m_pSecurityEntry == null){
                m_pSecurityEntry = m_pVirtualServer.IpSecurity.Add(
                    m_pEnabled.Checked,
                    m_pDescription.Text,
                    (Service_enum)((WComboBoxItem)m_pService.SelectedItem).Tag,
                    (IPSecurityAction_enum)((WComboBoxItem)m_pAction.SelectedItem).Tag,
                    startIP,
                    endIP
                );
            }
            // Update security entry
            else{
                m_pSecurityEntry.Enabled     = m_pEnabled.Checked;
                m_pSecurityEntry.Description = m_pDescription.Text;
                m_pSecurityEntry.Service     = (Service_enum)((WComboBoxItem)m_pService.SelectedItem).Tag;
                m_pSecurityEntry.Action      = (IPSecurityAction_enum)((WComboBoxItem)m_pAction.SelectedItem).Tag;
                m_pSecurityEntry.StartIP     = startIP;
                m_pSecurityEntry.EndIP       = endIP;
                m_pSecurityEntry.Commit();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets active security entry ID.
        /// </summary>
        public string SecurityEntryID
        {
            get{ 
                if(m_pSecurityEntry != null){
                    return m_pSecurityEntry.ID;
                }
                else{
                    return "";
                }
            }
        }

        #endregion

    }
}
