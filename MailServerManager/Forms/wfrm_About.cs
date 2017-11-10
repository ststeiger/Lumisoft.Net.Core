using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// About box window.
    /// </summary>
    public class wfrm_About : Form
    {
        private Label  mt_Name = null;
        private Button m_pOk   = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_About()
        {
            InitUI();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(292,233);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "About";

            mt_Name = new Label();
            mt_Name.Size = new Size(250,60);
            mt_Name.Location = new Point(40,40);
            mt_Name.Font = new Font(mt_Name.Font.FontFamily,10);
            mt_Name.Text = "LumiSoft Mail Server Manager 0.99";

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(210,195);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(mt_Name);
            this.Controls.Add(m_pOk);
        }
                
        #endregion


        #region Events Handling

        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion

    }
}
