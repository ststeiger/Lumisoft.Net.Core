using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Create/ rename folder window.
    /// </summary>
    public class wfrm_sys_Folder : Form
    {
        private Label   mt_Folder = null;
        private TextBox m_pFolder = null;
        private Button  m_pOk     = null;
        private Button  m_Cancel  = null;

        private bool m_MayContainPath = true;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="create_rename">If true then create folder window, otherwise rename folder window.</param>
        /// <param name="folder">Initial folder full path.</param>
        /// <param name="mayContainPath">Specifies if folder name can contain full apth or folder name only.</param>
        public wfrm_sys_Folder(bool create_rename,string folder,bool mayContainPath)
        {
            m_MayContainPath = mayContainPath;

            InitUI();

            if(create_rename){
                this.Text = "Add new Folder";
            }
            else{
                this.Text = "Rename Folder";
            }

            m_pFolder.Text = folder;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(292,103);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            
            mt_Folder = new Label();
            mt_Folder.Size = new Size(200,13);
            mt_Folder.Location = new Point(9,20);
            mt_Folder.Text = "Folder Name:";

            m_pFolder = new TextBox();
            m_pFolder.Size = new Size(270,13);
            m_pFolder.Location = new Point(9,35);
            
            m_Cancel = new Button();
            m_Cancel.Size = new Size(70,20);
            m_Cancel.Location = new Point(135,70);
            m_Cancel.Text = "Cancel";
            m_Cancel.Click += new EventHandler(m_Cancel_Click);

            m_pOk = new Button(); 
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(210,70);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(mt_Folder);
            this.Controls.Add(m_pFolder);
            this.Controls.Add(m_Cancel);
            this.Controls.Add(m_pOk);
        }
                                
        #endregion


        #region Events Handling

        #region method m_Cancel_Click

        private void m_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            //--- Validate values ---------------------------//
            if(m_pFolder.Text == ""){
                MessageBox.Show(this,"Folder name can't be empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if(!m_MayContainPath && m_pFolder.Text.IndexOfAny(new char[]{'/','\\'}) > -1 ){
                MessageBox.Show(this,"Path in folder name not allowed, please specify folder name only !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //----------------------------------------------//

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets folder name.
        /// </summary>
        public string Folder
        {
            get{ return m_pFolder.Text; }
        }

        #endregion

    }
}
