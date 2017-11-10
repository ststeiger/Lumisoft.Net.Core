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
    /// Users default folders Add folder UI.
    /// </summary>
    public class wfrm_Folders_UsersDefaultFolders_Folder : Form 
    {
        private PictureBox m_pIcon       = null;
        private Label      mt_Info       = null;
        private GroupBox   m_pSeparator1 = null;
        private Label      mt_FolderName = null;
        private TextBox    m_pFolderName = null;
        private CheckBox   m_pPermanent  = null;
        private GroupBox   m_pGroupbox1  = null;
        private Button     m_pCancel     = null;
        private Button     m_pOk         = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Add new constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_Folders_UsersDefaultFolders_Folder(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,153);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Add Users Default Folder";
            this.Icon = ResManager.GetIcon("folder32.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("folder32.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify default folder information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(383,3);
            m_pSeparator1.Location = new Point(7,50);

            mt_FolderName = new Label();
            mt_FolderName.Size = new Size(100,20);
            mt_FolderName.Location = new Point(0,65);
            mt_FolderName.TextAlign = ContentAlignment.MiddleRight;
            mt_FolderName.Text = "Folder:";

            m_pFolderName = new TextBox();
            m_pFolderName.Size = new Size(280,20);
            m_pFolderName.Location = new Point(105,65);

            m_pPermanent = new CheckBox();
            m_pPermanent.Size = new Size(250,20);
            m_pPermanent.Location = new Point(105,90);
            m_pPermanent.Text = "Folder is permanent, users can't delete it.";

            m_pGroupbox1 = new GroupBox();
            m_pGroupbox1.Size = new Size(383,3);
            m_pGroupbox1.Location = new Point(7,120);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(240,130);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(315,130);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_FolderName);
            this.Controls.Add(m_pFolderName);
            this.Controls.Add(m_pPermanent);
            this.Controls.Add(m_pGroupbox1);
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
            //--- Validate values ---------------------------------------------------//
            if(m_pFolderName.Text == ""){
                MessageBox.Show("Folder name cannot be empty!!!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);					
				return;
            }
            //-----------------------------------------------------------------------//

            try{
                m_pVirtualServer.UsersDefaultFolders.Add(
                    m_pFolderName.Text,
                    m_pPermanent.Checked
                );

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
        /// Gets current folder name.
        /// </summary>
        public string FolderName
        {
            get{ return m_pFolderName.Text; }
        }

        #endregion

    }
}
