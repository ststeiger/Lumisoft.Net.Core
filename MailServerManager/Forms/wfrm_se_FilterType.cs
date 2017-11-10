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
    /// Filter type selector window.
    /// </summary>
    public class wfrm_se_FilterType : Form
    {
        private ListView m_pList      = null;
        private GroupBox m_pGroupbox1 = null;
        private Button   m_pCancel    = null;
        private Button   m_pOk        = null;

        private VirtualServer m_pVirtualServer = null;
        private string        m_Assembly       = "";
        private string        m_Type           = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_se_FilterType(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            LoadFilters();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,273);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Select Filter";

            m_pList = new ListView();
            m_pList.Size = new Size(375,220);
            m_pList.Location = new Point(10,10);
            m_pList.View = View.Details;
            m_pList.FullRowSelect = true;
            m_pList.HideSelection = false;
            m_pList.DoubleClick += new EventHandler(m_pList_DoubleClick);
            m_pList.Columns.Add("Assembly",150,HorizontalAlignment.Left);
            m_pList.Columns.Add("Type",200,HorizontalAlignment.Left);

            m_pGroupbox1 = new GroupBox();
            m_pGroupbox1.Size = new Size(390,3);
            m_pGroupbox1.Location = new Point(5,240);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(245,250);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(320,250);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pList);
            this.Controls.Add(m_pGroupbox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                
        #endregion


        #region Events Handling

        #region method m_pList_DoubleClick

        private void m_pList_DoubleClick(object sender, EventArgs e)
        {
            m_pOk_Click(sender,e);
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
            if(m_pList.SelectedItems.Count > 0){
                m_Assembly = m_pList.SelectedItems[0].Text;
                m_Type     = m_pList.SelectedItems[0].SubItems[1].Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region method LoadFilters

        /// <summary>
        /// Gets available virtual server filters and sets them to UI.
        /// </summary>
        private void LoadFilters()
        {
            DataSet ds = m_pVirtualServer.Filters.GetFilterTypes();
            if(ds.Tables.Contains("Filters")){
                foreach(DataRow dr in ds.Tables["Filters"].Rows){
                    ListViewItem it = new ListViewItem(dr["AssemblyName"].ToString());
                    it.SubItems.Add(dr["TypeName"].ToString());
                    m_pList.Items.Add(it);
                }
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets selected assembly name.
        /// </summary>
        public string AssemblyName
        {
           get{ return m_Assembly; }
        }

        /// <summary>
        /// Gets selected Type name.
        /// </summary>
        public string TypeName
        {
            get{ return m_Type; }
        }

        #endregion

    }
}
