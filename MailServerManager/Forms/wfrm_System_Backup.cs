using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// System backup/restore window.
    /// </summary>
    public class wfrm_System_Backup : Form
    {
        // Backup UI
        private GroupBox m_pGroubBox_Backup  = null;
        private Button   m_pBackup           = null;
        private Button   m_pBackupMessages   = null;
        // Restore UI
        private GroupBox m_pGroupBox_Restore    = null;        
        private Button   m_pRestore             = null;
        private CheckBox m_pRestoreFlagsAdd     = null;
        private CheckBox m_pRestoreFlagsReplace = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_System_Backup(VirtualServer virtualServer)
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
            this.Size = new Size(472,357);

            //--- Backup UI -------------------------------------//
            m_pGroubBox_Backup = new GroupBox();
            m_pGroubBox_Backup.Size = new Size(400,150);
            m_pGroubBox_Backup.Location = new Point(20,20);
            m_pGroubBox_Backup.Text = "Backup:";

            m_pBackup = new Button();
            m_pBackup.Size = new Size(110,20);
            m_pBackup.Location = new Point(20,40);
            m_pBackup.Text = "Backup";
            m_pBackup.Click += new EventHandler(m_pBackup_Click);

            m_pBackupMessages = new Button();
            m_pBackupMessages.Size = new Size(110,20);
            m_pBackupMessages.Location = new Point(20,80);
            m_pBackupMessages.Text = "Backup Messages";
            m_pBackupMessages.Click += new EventHandler(m_pBackupMessages_Click);

            m_pGroubBox_Backup.Controls.Add(m_pBackup);
            m_pGroubBox_Backup.Controls.Add(m_pBackupMessages);
            //---------------------------------------------------//
            
            //--- Restore UI ------------------------------------//
            m_pGroupBox_Restore = new GroupBox();
            m_pGroupBox_Restore.Size = new Size(400,150);
            m_pGroupBox_Restore.Location = new Point(20,200);
            m_pGroupBox_Restore.Text = "Restore:";

            m_pRestore = new Button();
            m_pRestore.Size = new Size(70,20);
            m_pRestore.Location = new Point(20,40);
            m_pRestore.Text = "Restore";
            m_pRestore.Click += new EventHandler(m_pRestore_Click);

            m_pRestoreFlagsAdd = new CheckBox();
            m_pRestoreFlagsAdd.Size = new Size(200,20);
            m_pRestoreFlagsAdd.Location = new Point(20,70);
            m_pRestoreFlagsAdd.Text = "Add non existent items";
            m_pRestoreFlagsAdd.Checked = true;
            
            m_pRestoreFlagsReplace = new CheckBox();
            m_pRestoreFlagsReplace.Size = new Size(200,20);
            m_pRestoreFlagsReplace.Location = new Point(20,95);
            m_pRestoreFlagsReplace.Text = "Replace existing items";
            m_pRestoreFlagsReplace.Checked = false;
            
            m_pGroupBox_Restore.Controls.Add(m_pRestore);
            m_pGroupBox_Restore.Controls.Add(m_pRestoreFlagsAdd);
            m_pGroupBox_Restore.Controls.Add(m_pRestoreFlagsReplace);
            //--------------------------------------------------//

            //--- Common UI
            this.Controls.Add(m_pGroubBox_Backup);
            this.Controls.Add(m_pGroupBox_Restore);
        }
                                                
        #endregion


        #region Events Handling

        #region method m_pBackup_Click

        private void m_pBackup_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "LS Mail Server backup (*.lsmbck)|*.lsmbck";
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pVirtualServer.Backup(dlg.FileName);
            }
        }

        #endregion

        #region method m_pBackupMessages_Click

        private void m_pBackupMessages_Click(object sender,EventArgs e)
        {
            wfrm_utils_BackupMessages frm = new wfrm_utils_BackupMessages(m_pVirtualServer);
            frm.ShowDialog(this);
        }

        #endregion


        #region method m_pRestore_Click

        private void m_pRestore_Click(object sender, EventArgs e)
        {
            RestoreFlags_enum restoreFlags = 0;
            if(m_pRestoreFlagsAdd.Checked){
                restoreFlags |= RestoreFlags_enum.Add;
            }
            if(m_pRestoreFlagsReplace.Checked){
                restoreFlags |= RestoreFlags_enum.Replace;
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "LS Mail Server backup (*.lsmbck)|*.lsmbck";
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pVirtualServer.Restore(dlg.FileName,restoreFlags);
            }
        }

        #endregion

        #endregion

    }
}
