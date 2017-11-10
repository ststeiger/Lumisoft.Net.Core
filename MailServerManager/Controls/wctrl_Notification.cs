using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// This class implements notification area control.
    /// </summary>
    public class wctrl_Notification : Control
    {
        private Panel       m_pPanel = null;
        private PictureBox  m_pIcon  = null;
        private RichTextBox m_pText  = null;

        /// <summary>
        /// Default control.
        /// </summary>
        public wctrl_Notification()
        {
            InitUI();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(100,38);
            this.MinimumSize = new Size(100,38);

            m_pPanel = new Panel();
            m_pPanel.Size = this.ClientSize;
            m_pPanel.Location = new Point(0,0);
            m_pPanel.Dock = DockStyle.Fill;
            m_pPanel.BorderStyle = BorderStyle.FixedSingle;
                        
            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(36,36);
            m_pIcon.Location = new Point(4,1);

            m_pText = new RichTextBox();
            m_pText.Size = new Size(55,36);
            m_pText.Location = new Point(45,1);
            m_pText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pText.ReadOnly = true;
            m_pText.ForeColor = Color.Gray;
            m_pText.ScrollBars = RichTextBoxScrollBars.Vertical;
            m_pText.BorderStyle = BorderStyle.None;

            m_pPanel.Controls.Add(m_pIcon);
            m_pPanel.Controls.Add(m_pText);            

            this.Controls.Add(m_pPanel);
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets notification icon.
        /// </summary>
        public Image Icon
        {
            get{ return m_pIcon.Image; }

            set{ m_pIcon.Image = value; }
        }

        /// <summary>
        /// Gets or sets notification text.
        /// </summary>
        public override string Text
        {
            get{ return m_pText.Text; }

            set{ m_pText.Text = value; }
        }

        #endregion
        
    }
}
