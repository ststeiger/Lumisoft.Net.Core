using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Domains domain settings window.
    /// </summary>
    public class wfrm_Domains_Domain : Form
    {       
        private PictureBox m_pIcon        = null;
        private Label      mt_Info        = null;
        private GroupBox   m_pSeparator1  = null;
        private Label      mt_Domain      = null;
        private TextBox    m_pDomainName  = null;
        private Label      mt_Description = null;
        private TextBox    m_pDescription = null;
        private GroupBox   m_pSeparator2  = null;
        private Button     m_pCancel      = null;
        private Button     m_pOk          = null;

        private VirtualServer m_pVirtualServer = null;
        private Domain        m_pDomain        = null;  

        /// <summary>
        /// Add constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_Domains_Domain(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="domain">Domain to update.</param>
        public wfrm_Domains_Domain(VirtualServer virtualServer,Domain domain)
        {
            m_pVirtualServer = virtualServer;
            m_pDomain        = domain;

            InitUI();

            m_pDomainName.Text  = domain.DomainName;
            m_pDescription.Text = domain.Description;
            m_pDomainName.SelectionStart = 0;
            m_pDomainName.SelectionLength = 0;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,173);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Add/Edit domain";

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("domain32.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify domain information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(383,3);
            m_pSeparator1.Location = new Point(7,50);

            mt_Domain = new Label();
            mt_Domain.Size = new Size(100,20);
            mt_Domain.Location = new Point(0,70);
            mt_Domain.TextAlign = ContentAlignment.MiddleRight;
            mt_Domain.Text = "Domain Name:";
  
            m_pDomainName = new TextBox();
            m_pDomainName.Size = new Size(280,20);
            m_pDomainName.Location = new Point(105,70);

            mt_Description = new Label();
            mt_Description.Size = new Size(100,20);
            mt_Description.Location = new Point(0,95);
            mt_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(280,20);
            m_pDescription.Location = new Point(105,95);

            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(383,4);
            m_pSeparator2.Location = new Point(7,130);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(240,150);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(315,150);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_Domain);
            this.Controls.Add(m_pDomainName);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pDescription);
            this.Controls.Add(m_pSeparator2);
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
				if(m_pDomainName.Text.Length <= 0){
					MessageBox.Show("Domain name cannot be empty!!!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);					
					return;
				}

                // Add new domain.
                if(m_pDomain == null){
				    m_pDomain = m_pVirtualServer.Domains.Add(
                        m_pDomainName.Text,
                        m_pDescription.Text
                    );
                }
                // Update domain.
                else{
                    m_pDomain.DomainName  = m_pDomainName.Text;
                    m_pDomain.Description = m_pDescription.Text;
                    m_pDomain.Commit();
                }

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch(Exception x){
				wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
				frm.ShowDialog(this);
			}
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets domain ID.
        /// </summary>
        public string DomainID
        {
            get{ 
                if(m_pDomain != null){
                    return m_pDomain.DomainID;
                }
                else{
                    return ""; 
                } }
        }

        #endregion

    }
}
