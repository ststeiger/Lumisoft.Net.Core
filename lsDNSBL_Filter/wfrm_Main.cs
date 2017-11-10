using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace LumiSoft.MailServer.Filters
{
    /// <summary>
    /// Filter settings UI window.
    /// </summary>
    public class wfrm_Main : Form
    {
        //--- Common UI
        private TabControl m_pTab       = null;
        private GroupBox   m_pGroupBox1 = null;
        private Button     m_pCancel    = null;
        private Button     m_pOk        = null;
        //--- General UI
        private CheckBox m_pGeneral_CheckHelo     = null;
        private CheckBox m_pGeneral_LogRejections = null;
        //--- Black list UI
        private Label   mt_BlackList_ErrorText = null;
        private TextBox m_pBlackList_ErrorText = null;
        private TextBox m_pBlackList_IP        = null;
        private Button  m_pBlackList_Add       = null;
        private ListBox m_pBlackList_IPs       = null;
        private Button  m_pBlackList_Remove    = null;
        //--- DNSBL UI
        private ListView m_pServers   = null;
        private Button   m_pAdd       = null;
        private Button   m_pDelete    = null;
        private Button   m_pMoveUp    = null;
        private Button   m_pMoveDown  = null;
        
        private DataSet m_pDsSettings = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_Main()
        {
            InitUI();

            LoadSettings();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(470,300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "LumiSoft DNSBL Filter Settings";

            //--- Common UI ----------------------------------------------------------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(457,220);
            m_pTab.Location = new Point(3,3);
            m_pTab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab.TabPages.Add("General");
            m_pTab.TabPages.Add("Black List");
            m_pTab.TabPages.Add("DNSBL");

            m_pGroupBox1 = new GroupBox();
            m_pGroupBox1.Size = new Size(455,4);
            m_pGroupBox1.Location = new Point(5,235);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(305,245);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();            
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(380,245);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pGroupBox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
            //-----------------------------------------------------------------------------------------------//

            //--- Tabpage General UI ------------------------------------------------------------------------//
            m_pGeneral_CheckHelo = new CheckBox();
            m_pGeneral_CheckHelo.Size = new Size(350,20);
            m_pGeneral_CheckHelo.Location = new Point(20,20);
            m_pGeneral_CheckHelo.Text = "Check HELO/EHLO name for non authenticated users";

            m_pGeneral_LogRejections = new CheckBox();
            m_pGeneral_LogRejections.Size = new Size(350,20);
            m_pGeneral_LogRejections.Location = new Point(20,40);
            m_pGeneral_LogRejections.Text = "Log rejections";

            m_pTab.TabPages[0].Controls.Add(m_pGeneral_CheckHelo);
            m_pTab.TabPages[0].Controls.Add(m_pGeneral_LogRejections);
            //-----------------------------------------------------------------------------------------------//

            //--- Tabpage Black List UI ---------------------------------------------------------------------//
            mt_BlackList_ErrorText = new Label();
            mt_BlackList_ErrorText.Size = new Size(70,20);
            mt_BlackList_ErrorText.Location = new Point(0,20);
            mt_BlackList_ErrorText.TextAlign = ContentAlignment.MiddleRight;
            mt_BlackList_ErrorText.Text = "Error Text:";

            m_pBlackList_ErrorText = new TextBox();
            m_pBlackList_ErrorText.Size = new Size(370,20);
            m_pBlackList_ErrorText.Location = new Point(75,20);

            m_pBlackList_IP = new TextBox();
            m_pBlackList_IP.Size = new Size(200,20);
            m_pBlackList_IP.Location = new Point(75,45);

            m_pBlackList_Add = new Button();
            m_pBlackList_Add.Size = new Size(70,20);
            m_pBlackList_Add.Location = new Point(280,45);
            m_pBlackList_Add.Text = "Add";
            m_pBlackList_Add.Click += new EventHandler(m_pBlackList_Add_Click);
  
            m_pBlackList_IPs = new ListBox();
            m_pBlackList_IPs.Size = new Size(200,130);
            m_pBlackList_IPs.Location = new Point(75,70);
            m_pBlackList_IPs.SelectedIndexChanged += new EventHandler(m_pBlackList_IPs_SelectedIndexChanged);

            m_pBlackList_Remove = new Button();
            m_pBlackList_Remove.Size = new Size(70,20);
            m_pBlackList_Remove.Location = new Point(280,70);
            m_pBlackList_Remove.Text = "Remove";
            m_pBlackList_Remove.Click += new EventHandler(m_pBlackList_Remove_Click);

            m_pTab.TabPages[1].Controls.Add(mt_BlackList_ErrorText);
            m_pTab.TabPages[1].Controls.Add(m_pBlackList_ErrorText);
            m_pTab.TabPages[1].Controls.Add(m_pBlackList_IP);
            m_pTab.TabPages[1].Controls.Add(m_pBlackList_Add);
            m_pTab.TabPages[1].Controls.Add(m_pBlackList_IPs);
            m_pTab.TabPages[1].Controls.Add(m_pBlackList_Remove);
            //-----------------------------------------------------------------------------------------------//

            //--- Tabpage DNSBL UI --------------------------------------------------------------------------//
            m_pServers = new ListView();
            m_pServers.Size = new Size(350,150);
            m_pServers.Location = new Point(10,20);
            m_pServers.View = View.Details;
            m_pServers.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            m_pServers.MultiSelect = false;
            m_pServers.FullRowSelect = true;
            m_pServers.HideSelection = false;
            m_pServers.SelectedIndexChanged += new EventHandler(m_pServers_SelectedIndexChanged);
            m_pServers.Columns.Add("Server",150);
            m_pServers.Columns.Add("Default Rejection Text",150);

            m_pAdd = new Button();
            m_pAdd.Size = new Size(70,20);
            m_pAdd.Location = new Point(370,20);
            m_pAdd.Text = "Add";
            m_pAdd.Click += new EventHandler(m_pAdd_Click);

            m_pDelete = new Button();
            m_pDelete.Size = new Size(70,20);
            m_pDelete.Location = new Point(370,45);
            m_pDelete.Text = "Delete";
            m_pDelete.Click += new EventHandler(m_pDelete_Click);

            m_pMoveUp = new Button();
            m_pMoveUp.Size = new Size(70,20);
            m_pMoveUp.Location = new Point(370,85);
            m_pMoveUp.Text = "Up";
            m_pMoveUp.Click += new EventHandler(m_pMoveUp_Click);

            m_pMoveDown = new Button();
            m_pMoveDown.Size = new Size(70,20);
            m_pMoveDown.Location = new Point(370,110);
            m_pMoveDown.Text = "Down";
            m_pMoveDown.Click += new EventHandler(m_pMoveDown_Click);

            m_pTab.TabPages[2].Controls.Add(m_pServers);
            m_pTab.TabPages[2].Controls.Add(m_pAdd);
            m_pTab.TabPages[2].Controls.Add(m_pDelete);
            m_pTab.TabPages[2].Controls.Add(m_pMoveUp);
            m_pTab.TabPages[2].Controls.Add(m_pMoveDown);
            //--------------------------------------------------------------------------------------------//           
        }
                                                                                                                                                                
        #endregion


        #region Events Handling

        #region method m_pBlackList_Add_Click

        private void m_pBlackList_Add_Click(object sender, EventArgs e)
        {
            // Check that specified item already doesn't exist.
            foreach(string item in m_pBlackList_IPs.Items){
                if(item.ToLower() == m_pBlackList_IP.Text.ToLower()){
                    m_pBlackList_IP.Text = "";
                    return;
                }
            }

            m_pBlackList_IPs.Items.Add(m_pBlackList_IP.Text);
            m_pBlackList_IP.Text = "";
        }

        #endregion

        #region method m_pBlackList_IPs_SelectedIndexChanged

        private void m_pBlackList_IPs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pBlackList_IPs.SelectedItems.Count > 0){
                m_pBlackList_Remove.Enabled = true;
            }
            else{
                m_pBlackList_Remove.Enabled = false;
            }
        }

        #endregion

        #region method m_pBlackList_Remove_Click

        private void m_pBlackList_Remove_Click(object sender, EventArgs e)
        {
            while(m_pBlackList_IPs.SelectedItems.Count > 0){
                m_pBlackList_IPs.Items.Remove(m_pBlackList_IPs.SelectedItems[0]);
            }
        }

        #endregion


        #region method m_pServers_SelectedIndexChanged

        private void m_pServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pServers.SelectedItems.Count > 0){
                m_pDelete.Enabled = true;
                if(m_pServers.Items.Count > 0){
                    if(m_pServers.SelectedItems[0].Index > 0){
                        m_pMoveUp.Enabled = true;
                    }
                    if(m_pServers.SelectedItems[0].Index < (m_pServers.Items.Count - 1)){
                        m_pMoveDown.Enabled = true;
                    }
                }
            }
            else{
                m_pDelete.Enabled   = false;
                m_pMoveUp.Enabled   = false;
                m_pMoveDown.Enabled = false;
            }
        }

        #endregion

        #region method m_pAdd_Click

        private void m_pAdd_Click(object sender, EventArgs e)
        {
            wfrm_DNSBL_Entry frm = new wfrm_DNSBL_Entry();
            if(frm.ShowDialog() == DialogResult.OK){
                DataRow dr = m_pDsSettings.Tables["Servers"].NewRow();
                dr["Cost"]                 = DateTime.Now.Ticks;
                dr["Server"]               = frm.Server;
                dr["DefaultRejectionText"] = frm.DefaultRejectionText;
                m_pDsSettings.Tables["Servers"].Rows.Add(dr);

                ListViewItem it = new ListViewItem();
                it.Tag = dr;
                it.Text = frm.Server;
                it.SubItems.Add(frm.DefaultRejectionText);
                m_pServers.Items.Add(it);
            }
        }

        #endregion

        #region method m_pDelete_Click

        private void m_pDelete_Click(object sender, EventArgs e)
        {
            if(m_pServers.SelectedItems.Count > 0){
                ((DataRow)m_pServers.SelectedItems[0].Tag).Delete();
                m_pServers.SelectedItems[0].Remove();
            }
        }

        #endregion

        #region method m_pMoveUp_Click

        private void m_pMoveUp_Click(object sender, EventArgs e)
        {
            if(m_pServers.SelectedItems.Count > 0 && m_pServers.SelectedItems[0].Index > 0){
                SwapItems(m_pServers.SelectedItems[0],m_pServers.Items[m_pServers.SelectedItems[0].Index - 1]);
            }
        }

        #endregion

        #region method m_pMoveDown_Click

        private void m_pMoveDown_Click(object sender, EventArgs e)
        {
            if(m_pServers.SelectedItems.Count > 0 && m_pServers.SelectedItems[0].Index < m_pServers.Items.Count - 1){
                SwapItems(m_pServers.SelectedItems[0],m_pServers.Items[m_pServers.SelectedItems[0].Index + 1]);
            }
        }

        #endregion


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
            SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region method LoadSettings

        /// <summary>
        /// Loads settings from xml file to UI.
        /// </summary>
        private void LoadSettings()
        {
            m_pDsSettings = new DataSet();
            m_pDsSettings.Tables.Add("General");
            m_pDsSettings.Tables["General"].Columns.Add("CheckHelo");
            m_pDsSettings.Tables["General"].Columns.Add("LogRejections");
            m_pDsSettings.Tables.Add("BlackListSettings");
            m_pDsSettings.Tables["BlackListSettings"].Columns.Add("ErrorText");
            m_pDsSettings.Tables.Add("BlackList");
            m_pDsSettings.Tables["BlackList"].Columns.Add("IP");
            m_pDsSettings.Tables.Add("Servers");
            m_pDsSettings.Tables["Servers"].Columns.Add("Cost");
            m_pDsSettings.Tables["Servers"].Columns.Add("Server");
            m_pDsSettings.Tables["Servers"].Columns.Add("DefaultRejectionText");

            if(File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\lsDNSBL_Filter_db.xml")){
                m_pDsSettings.ReadXml(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\lsDNSBL_Filter_db.xml");
            }

            //--- Load general ----------------------------------------------------------------------------------//
            // If no default settings, create them.
            if(m_pDsSettings.Tables["General"].Rows.Count == 0){
                DataRow dr = m_pDsSettings.Tables["General"].NewRow();
                dr["CheckHelo"]     = false;
                dr["LogRejections"] = false;
                m_pDsSettings.Tables["General"].Rows.Add(dr);
            }

            m_pGeneral_CheckHelo.Checked     = ConvertEx.ToBoolean(m_pDsSettings.Tables["General"].Rows[0]["CheckHelo"]);
            m_pGeneral_LogRejections.Checked = ConvertEx.ToBoolean(m_pDsSettings.Tables["General"].Rows[0]["LogRejections"]);
            //----------------------------------------------------------------------------------------------------//

            //--- Load black list -------------------------------------------------------------------------------//
            // If no default settings, create them.
            if(m_pDsSettings.Tables["BlackListSettings"].Rows.Count == 0){
                DataRow dr = m_pDsSettings.Tables["BlackListSettings"].NewRow();
                dr["ErrorText"] = "Your IP is in server black list !";
                m_pDsSettings.Tables["BlackListSettings"].Rows.Add(dr);
            }
                       
            m_pBlackList_ErrorText.Text = m_pDsSettings.Tables["BlackListSettings"].Rows[0]["ErrorText"].ToString();

            foreach(DataRow dr in m_pDsSettings.Tables["BlackList"].Rows){
                m_pBlackList_IPs.Items.Add(dr["IP"].ToString());
            }
            //----------------------------------------------------------------------------------------------------//

            //--- Load DNSBL servers ------------------------------------//
            foreach(DataRow dr in m_pDsSettings.Tables["Servers"].Rows){
                ListViewItem it = new ListViewItem();
                it.Tag = dr;
                it.Text = dr["Server"].ToString();
                it.SubItems.Add(dr["DefaultRejectionText"].ToString());
                m_pServers.Items.Add(it);
            }

            m_pServers_SelectedIndexChanged(this,new EventArgs());
            //----------------------------------------------------------//
        }

        #endregion

        #region method SaveSettings

        /// <summary>
        /// Saves settings to xml file.
        /// </summary>
        private void SaveSettings()
        {
            m_pDsSettings.Tables["General"].Rows[0]["CheckHelo"]     = m_pGeneral_CheckHelo.Checked;
            m_pDsSettings.Tables["General"].Rows[0]["LogRejections"] = m_pGeneral_LogRejections.Checked;

            m_pDsSettings.WriteXml(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\lsDNSBL_Filter_db.xml");
        }

        #endregion
                

        #region method SwapItems

        /// <summary>
        /// Swaps specified items.
        /// </summary>
        /// <param name="item1">Item 1.</param>
        /// <param name="item2">Item 2.</param>
        private void SwapItems(ListViewItem item1,ListViewItem item2)
        {
            DataRow drV_Down = (DataRow)item1.Tag;
            DataRow drV_Up   = (DataRow)item2.Tag;

            string down_Cost          = drV_Down["Cost"].ToString();
            string down_Server        = drV_Down["Server"].ToString();
            string down_RejectionText = drV_Down["DefaultRejectionText"].ToString();

            drV_Down["Cost"]                 = drV_Up["Cost"];
            drV_Down["Server"]               = drV_Up["Server"];
            drV_Down["DefaultRejectionText"] = drV_Up["DefaultRejectionText"];
            drV_Up["Cost"]                   = down_Cost;
            drV_Up["Server"]                 = down_Server;
            drV_Up["DefaultRejectionText"]   = down_RejectionText;

            item1.Text = item2.Text;
            item1.SubItems[1].Text = item2.SubItems[1].Text;
            item2.Text = down_Server;
            item2.SubItems[1].Text = down_RejectionText;
                       
            if(item1.Selected){
                item2.Selected = true;
            }
            else if(item2.Selected){
                item1.Selected = true;
            }
        }

        #endregion
    }
}
