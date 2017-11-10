using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// System Services Fetch POP3 settings window.
    /// </summary>
    public class wfrm_System_Services_FetchPOP3 : Form
    {
        //--- Common UI ------------------------------
        private TabControl    m_pTab           = null;
        private Button        m_pApply         = null;
        //--- Tabpage General UI ---------------------
        private CheckBox      m_pEnabled       = null;
        private Label         mt_FetchInterval = null;
        private NumericUpDown m_pFetchInterval = null;
        private Label         mt_Seconds       = null;
        //--------------------------------------------

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_System_Services_FetchPOP3(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadData();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            //--- Common UI -------------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(515,520);
            m_pTab.Location = new Point(5,0);
            m_pTab.TabPages.Add(new TabPage("General"));

            m_pApply = new Button();
            m_pApply.Size = new Size(70,20);
            m_pApply.Location = new Point(450,530);
            m_pApply.Text = "Apply";
            m_pApply.Click += new EventHandler(m_pApply_Click);
            //---------------------------------------------------//

            //--- Tabpage General UI ----------------------------//
            m_pEnabled = new CheckBox();            
            m_pEnabled.Size = new Size(70,20);
            m_pEnabled.Location = new Point(105,30);
            m_pEnabled.Text = "Enabled";

            mt_FetchInterval = new Label();
            mt_FetchInterval.Size = new Size(80,13);
            mt_FetchInterval.Location = new Point(20,63);
            mt_FetchInterval.Text = "Fetch Interval:";

            m_pFetchInterval = new NumericUpDown();
            m_pFetchInterval.Size = new Size(70,20);
            m_pFetchInterval.Location = new Point(105,60);
            m_pFetchInterval.Minimum = 30;
            m_pFetchInterval.Maximum = 9999;

            mt_Seconds = new Label();
            mt_Seconds.Size = new Size(30,13);
            mt_Seconds.Location = new Point(180,63);
            mt_Seconds.Text = "sec.";

            m_pTab.TabPages[0].Controls.Add(m_pEnabled);
            m_pTab.TabPages[0].Controls.Add(mt_FetchInterval);
            m_pTab.TabPages[0].Controls.Add(m_pFetchInterval);
            m_pTab.TabPages[0].Controls.Add(mt_Seconds);
            //-------------------------------------------------//

            // Common UI
            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pApply);
        }

        #endregion


        #region Events Handling

        #region method OnVisibleChanged

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if(!this.Visible){
                SaveData(true); 
            }
        }

        #endregion


        #region method m_pApply_Click

        private void m_pApply_Click(object sender, EventArgs e)
        {
            SaveData(false); 
        }

        #endregion

        #endregion


        #region method LoadData

        /// <summary>
        /// Loads data to UI.
        /// </summary>
        private void LoadData()
        {
            try{
                FetchMessages_Settings settings = m_pVirtualServer.SystemSettings.FetchMessages;

                m_pEnabled.Checked     = settings.Enabled;
				m_pFetchInterval.Value = settings.FetchInterval;
			}
			catch(Exception x){
				wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
				frm.ShowDialog(this);
			}
        }

        #endregion

        #region method SaveData

        /// <summary>
        /// Saves data.
        /// </summary>
        /// <param name="confirmSave">Specifies is save confirmation UI is showed.</param>
        private void SaveData(bool confirmSave)
        {
            try{
                FetchMessages_Settings settings = m_pVirtualServer.SystemSettings.FetchMessages;

                settings.Enabled       = m_pEnabled.Checked;
				settings.FetchInterval = (int)m_pFetchInterval.Value;

                if(m_pVirtualServer.SystemSettings.HasChanges){
                    if(!confirmSave || MessageBox.Show(this,"You have changes settings, do you want to save them ?","Confirm:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                        m_pVirtualServer.SystemSettings.Commit();
                    }
                }
            }
			catch(Exception x){
				wfrm_sys_Error frm = new wfrm_sys_Error(x,new System.Diagnostics.StackTrace());
				frm.ShowDialog(this);
			}
        }

        #endregion

    }
}
