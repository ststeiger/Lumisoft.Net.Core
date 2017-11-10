using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Error window.
    /// </summary>
    public class wfrm_sys_Error : Form
    {
        private TextBox    m_pMessage         = null;
        private PictureBox m_pImage           = null;
        private GroupBox   m_pGroupbox1       = null;
        private Button     m_pToggleExtended  = null;
        private Button     m_pClose           = null;
        private TextBox    m_pExtendedMessage = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="x">Error exception.</param>
        /// <param name="stack">Current stack.</param>
        public wfrm_sys_Error(Exception x,System.Diagnostics.StackTrace stack)
        {
            InitUI();

            this.ClientSize = new Size(492,168);

            m_pMessage.Text = x.Message;
            string extenedMessage  = "Message: " + x.Message + "\r\n";
                   extenedMessage += "Method: " + stack.GetFrame(0).GetMethod().DeclaringType.FullName + "." + stack.GetFrame(0).GetMethod().Name + "()" + "\r\n\r\n";
                   extenedMessage += "Stack:\r\n" + x.StackTrace;
			m_pExtendedMessage.Text = extenedMessage;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(492,373);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Error:";

            m_pMessage = new TextBox();
            m_pMessage.Size = new Size(240,100);
            m_pMessage.Location = new Point(10,10);
            m_pMessage.Multiline = true;
            m_pMessage.ReadOnly = true;
            m_pMessage.BorderStyle = BorderStyle.FixedSingle;
            m_pMessage.ScrollBars = ScrollBars.Horizontal;

            m_pImage = new PictureBox();
            m_pImage.Size = new Size(200,100);
            m_pImage.Location = new Point(280,10);
            m_pImage.SizeMode = PictureBoxSizeMode.StretchImage;
            m_pImage.Image = ResManager.GetImage("error.jpg");

            m_pGroupbox1 = new GroupBox();
            m_pGroupbox1.Size = new Size(485,3);
            m_pGroupbox1.Location = new Point(5,125);
                        
            m_pToggleExtended = new Button();
            m_pToggleExtended.Size = new Size(70,20);
            m_pToggleExtended.Location = new Point(335,140);
            m_pToggleExtended.Text = "More";
            m_pToggleExtended.Click += new EventHandler(m_pToggleExtended_Click);

            m_pClose = new Button();
            m_pClose.Size = new Size(70,20);
            m_pClose.Location = new Point(410,140);
            m_pClose.Text = "Close";
            m_pClose.Click += new EventHandler(m_pClose_Click);

            m_pExtendedMessage = new TextBox();
            m_pExtendedMessage.Size = new Size(470,185);
            m_pExtendedMessage.Location = new Point(10,170);
            m_pExtendedMessage.Multiline = true;
            m_pExtendedMessage.ReadOnly = true;
            m_pExtendedMessage.BorderStyle = BorderStyle.FixedSingle;
            m_pExtendedMessage.ScrollBars = ScrollBars.Both;            

            this.Controls.Add(m_pMessage);
            this.Controls.Add(m_pImage);
            this.Controls.Add(m_pGroupbox1);
            this.Controls.Add(m_pToggleExtended);
            this.Controls.Add(m_pClose);
            this.Controls.Add(m_pExtendedMessage);

        }
                                
        #endregion


        #region Events Handling

        #region method m_pToggleExtended_Click

        private void m_pToggleExtended_Click(object sender, EventArgs e)
        {
            if(m_pToggleExtended.Text == "More"){
                m_pToggleExtended.Text = "Less";
                this.ClientSize = new Size(492,373);
            }
            else{
                m_pToggleExtended.Text = "More";
                this.ClientSize = new Size(492,168);
            }
        }

        #endregion

        #region method m_pClose_Click

        private void m_pClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

    }
}
