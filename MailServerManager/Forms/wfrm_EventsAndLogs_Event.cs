using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Events and logs Event window.
    /// </summary>
    public class wfrm_EventsAndLogs_Event : Form
    {
        private PictureBox m_pImage       = null;
        private Label      mt_CreateDate  = null;
        private Label      m_pCreateDate  = null;
        private Label      mt_Type        = null;
        private Label      m_pType        = null;
        private Label      mt_Description = null;
        private TextBox    m_pText        = null;
        private Button     m_pClose       = null;
                
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="date">Event create date.</param>
        /// <param name="text">Event text.</param>
        public wfrm_EventsAndLogs_Event(DateTime date,string text)
        {
            InitUI();

            this.Icon = ResManager.GetIcon("error.ico");
            m_pImage.Image = ResManager.GetIcon("error.ico").ToBitmap();
            m_pCreateDate.Text = date.ToString();
            m_pText.Text       = text;
            m_pText.SelectionStart = 0;
            m_pText.SelectionLength = 0;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(492,373);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Event info:";

            m_pImage = new PictureBox();            
            m_pImage.Size = new Size(36,36);
            m_pImage.Location = new Point(9,15);

            mt_CreateDate = new Label();     
            mt_CreateDate.Size = new Size(100,20);
            mt_CreateDate.Location = new Point(9,15);
            mt_CreateDate.TextAlign = ContentAlignment.MiddleRight;
            mt_CreateDate.Text = "Create Date:";

            m_pCreateDate = new Label();     
            m_pCreateDate.Size = new Size(150,20);
            m_pCreateDate.Location = new Point(115,15);
            m_pCreateDate.TextAlign = ContentAlignment.MiddleLeft;

            mt_Type = new Label();     
            mt_Type.Size = new Size(100,20);
            mt_Type.Location = new Point(9,35);
            mt_Type.TextAlign = ContentAlignment.MiddleRight;
            mt_Type.Text = "Type:";

            m_pType = new Label();     
            m_pType.Size = new Size(100,20);
            m_pType.Location = new Point(115,35);
            m_pType.TextAlign = ContentAlignment.MiddleLeft;
            m_pType.Text = "Error";

            mt_Description = new Label();     
            mt_Description.Size = new Size(100,20);
            mt_Description.Location = new Point(9,65);
            mt_Description.Text = "Desciption:";

            m_pText = new TextBox();     
            m_pText.Size = new Size(475,240);
            m_pText.Location = new Point(9,85);
            m_pText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pText.Multiline = true;
            m_pText.ReadOnly = true;

            m_pClose = new Button();   
            m_pClose.Size = new Size(70,20);
            m_pClose.Location = new Point(412,340);
            m_pClose.Text = "Close";
            m_pClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pClose.Click += new EventHandler(m_pClose_Click);

            this.Controls.Add(m_pImage);
            this.Controls.Add(mt_CreateDate);
            this.Controls.Add(m_pCreateDate);
            this.Controls.Add(mt_Type);
            this.Controls.Add(m_pType);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pText);
            this.Controls.Add(m_pClose);
        }
                
        #endregion


        #region Events Handling

        #region method m_pClose_Click

        private void m_pClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

    }
}
