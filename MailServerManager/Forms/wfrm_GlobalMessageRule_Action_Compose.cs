using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Global message rule Auto Response action Compose message window.
    /// </summary>
    public class wfrm_GlobalMessageRule_Action_Compose : Form
    {
        private TextBox  m_pFrom      = null;
        private Label    mt_From      = null;
        private Label    mt_To        = null;
        private TextBox  m_pTo        = null;
        private Label    mt_Subject   = null;
        private TextBox  m_pSubject   = null;
        private TextBox  m_pBodyText  = null;
        private GroupBox m_pGroupBox1 = null;
        private Button   m_pCancel    = null;
        private Button   m_pOk        = null;

        private string m_Message = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_GlobalMessageRule_Action_Compose()
        {
            InitUI();

            m_pSubject.Text = "Auto Response: #SUBJECT";
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(498,372);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Compose Message";

            mt_From = new Label();
            mt_From.Size = new Size(86,23);
            mt_From.Location = new Point(9,13);
            mt_From.Text = "From:";

            m_pFrom = new TextBox();
            m_pFrom.Size = new Size(385,20);
            m_pFrom.Location = new Point(101,16);

            mt_To = new Label();
            mt_To.Size = new Size(86,23);
            mt_To.Location = new Point(12,40);
            mt_To.Text = "To:";

            m_pTo = new TextBox();
            m_pTo.Size = new Size(385,20);
            m_pTo.Location = new Point(101,42);

            mt_Subject = new Label();
            mt_Subject.Size = new Size(86,23);
            mt_Subject.Location = new Point(9,68);
            mt_Subject.Text = "Subject:";

            m_pSubject = new TextBox();
            m_pSubject.Size = new Size(385,20);
            m_pSubject.Location = new Point(101,68);
           
            m_pBodyText = new TextBox();
            m_pBodyText.Size = new Size(474,207);
            m_pBodyText.Location = new Point(12,104);
            m_pBodyText.AcceptsReturn = true;
            m_pBodyText.AcceptsTab = true;
            m_pBodyText.Multiline = true;

            m_pGroupBox1 = new GroupBox();
            m_pGroupBox1.Size = new Size(505,4);
            m_pGroupBox1.Location = new Point(1,327);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(67,20);
            m_pCancel.Location = new Point(346,340);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(67,20);
            m_pOk.Location = new Point(419,340);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(mt_From);
            this.Controls.Add(m_pFrom);
            this.Controls.Add(mt_To);
            this.Controls.Add(m_pTo);
            this.Controls.Add(mt_Subject);
            this.Controls.Add(m_pSubject);
            this.Controls.Add(m_pBodyText);
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
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            Mail_Message msg = new Mail_Message();
            msg.MimeVersion = "1.0";
            msg.MessageID = MIME_Utils.CreateMessageID();
            msg.Date = DateTime.Now;
            msg.From = Mail_h_MailboxList.Parse("From: " + m_pFrom.Text).Addresses;
            if(!string.IsNullOrEmpty(m_pTo.Text)){
                msg.To = Mail_h_AddressList.Parse("To: " + m_pTo.Text).Addresses;
            }
            msg.Subject = m_pSubject.Text;
            
            MIME_b_Text text_plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
            msg.Body = text_plain;
            text_plain.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,m_pBodyText.Text);

            m_Message = msg.ToString(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B,Encoding.UTF8),Encoding.UTF8);

            this.DialogResult = DialogResult.OK;
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets composed message.
        /// </summary>
        public string Message
        {
            get{ return m_Message; }
        }

        #endregion

    }
}
