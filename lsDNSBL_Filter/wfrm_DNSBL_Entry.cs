using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.Filters
{
    /// <summary>
    /// DNSBL server entry window.
    /// </summary>
    public class wfrm_DNSBL_Entry : Form
    {
        private Label    mt_Server        = null;
        private ComboBox m_pServer        = null;
        private Label    mt_RejectionText = null;
        private TextBox  m_pRejectionText = null;
        private GroupBox m_pGroupBox1     = null;
        private Button   m_pCancel        = null;
        private Button   m_pOk            = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_DNSBL_Entry()
        {
            InitUI();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(400,200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "DNSBL Entry Settings";

            mt_Server = new Label();
            mt_Server.Size = new Size(100,20);
            mt_Server.Location = new Point(10,10);
            mt_Server.Text = "Server:";

            m_pServer = new ComboBox();
            m_pServer.Size = new Size(370,20);
            m_pServer.Location = new Point(10,30);
            m_pServer.Items.Add("sbl-xbl.spamhaus.org");            
            m_pServer.Items.Add("bl.spamcop.net");
            m_pServer.Items.Add("dnsbl.sorbs.net");
            m_pServer.Items.Add("relays.ordb.org");

            mt_RejectionText = new Label();
            mt_RejectionText.Size = new Size(300,20);
            mt_RejectionText.Location = new Point(10,60);
            mt_RejectionText.Text = "Default rejection text: (if server won't provide TXT record)";

            m_pRejectionText = new TextBox();
            m_pRejectionText.Size = new Size(370,20);
            m_pRejectionText.Location = new Point(10,80);

            m_pGroupBox1 = new GroupBox();
            m_pGroupBox1.Size = new Size(435,4);
            m_pGroupBox1.Location = new Point(5,135);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(245,145);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();            
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(320,145);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);
            
            this.Controls.Add(mt_Server);
            this.Controls.Add(m_pServer);
            this.Controls.Add(mt_RejectionText);
            this.Controls.Add(m_pRejectionText);
            this.Controls.Add(m_pGroupBox1);
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
            if(m_pServer.Text == ""){
                MessageBox.Show(this,"Server can't be empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets DNSBL server.
        /// </summary>
        public string Server
        {
            get{ return m_pServer.Text; }
        }

        /// <summary>
        /// Gets default rejection text.
        /// </summary>
        public string DefaultRejectionText
        {
            get{ return m_pRejectionText.Text; }
        }

        #endregion

    }
}
