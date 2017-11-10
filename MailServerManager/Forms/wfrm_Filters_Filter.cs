using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Filters Add/Edit filter window.
    /// </summary>
    public class wfrm_Filters_Filter : Form
    {
        private PictureBox m_pIcon        = null;
        private Label      mt_Info        = null;
        private GroupBox   m_pSeparator1  = null;
        private CheckBox   m_pEnabled     = null;
        private Label      mt_Description = null;
        private TextBox    m_pDescription = null;
        private Label      mt_Assembly    = null;
        private TextBox    m_pAssembly    = null;
        private Button     m_pGetAssembly = null;
        private Label      mt_Class       = null;
        private TextBox    m_pClass       = null;
        private GroupBox   m_pSeparator2  = null;
        private Button     m_Cancel       = null;
        private Button     m_pOk          = null;

        private VirtualServer m_pVirtualServer = null;
        private Filter        m_pFilter        = null;

        /// <summary>
        /// Add new constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_Filters_Filter(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="filter">Filter to update.</param>
        public wfrm_Filters_Filter(VirtualServer virtualServer,Filter filter)
        {
            m_pVirtualServer = virtualServer;
            m_pFilter        = filter;

            InitUI();
                        
			m_pEnabled.Checked  = filter.Enabled;
            m_pDescription.Text = filter.Description;
		    m_pAssembly.Text    = filter.AssemblyName;
			m_pClass.Text       = filter.Class;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(392,208);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Add/Edit filter";
            this.Icon = ResManager.GetIcon("filter.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("filter32.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify filter information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(383,3);
            m_pSeparator1.Location = new Point(7,50);
            
            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(100,20);
            m_pEnabled.Location = new Point(105,60);
            m_pEnabled.Text = "Enabled";
            m_pEnabled.Checked = true;

            mt_Description = new Label();
            mt_Description.Size = new Size(100,20);
            mt_Description.Location = new Point(0,85);
            mt_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(280,20);
            m_pDescription.Location = new Point(105,85);

            mt_Assembly = new Label();
            mt_Assembly.Size = new Size(100,20);
            mt_Assembly.Location = new Point(0,110);
            mt_Assembly.TextAlign = ContentAlignment.MiddleRight;
            mt_Assembly.Text = "Assembly:";

            m_pAssembly = new TextBox();
            m_pAssembly.Size = new Size(250,20);
            m_pAssembly.Location = new Point(105,110);
            m_pAssembly.ReadOnly = true;

            m_pGetAssembly = new Button();
            m_pGetAssembly.Size = new Size(25,20);
            m_pGetAssembly.Location = new Point(360,110);
            m_pGetAssembly.Text = "...";
            m_pGetAssembly.Click += new EventHandler(m_pGetAssembly_Click);

            mt_Class = new Label();
            mt_Class.Size = new Size(100,20);
            mt_Class.Location = new Point(0,135); 
            mt_Class.TextAlign = ContentAlignment.MiddleRight;
            mt_Class.Text = "Class:";

            m_pClass = new TextBox();
            m_pClass.Size = new Size(280,20);
            m_pClass.Location = new Point(105,135);
            m_pClass.ReadOnly = true;

            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(383,3);
            m_pSeparator2.Location = new Point(7,165);

            m_Cancel = new Button();
            m_Cancel.Size = new Size(70,20);
            m_Cancel.Location = new Point(240,180);
            m_Cancel.Text = "Cancel";
            m_Cancel.Click += new EventHandler(m_Cancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(315,180);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(m_pEnabled);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pDescription);
            this.Controls.Add(mt_Assembly);
            this.Controls.Add(m_pAssembly);
            this.Controls.Add(m_pGetAssembly);
            this.Controls.Add(mt_Class);
            this.Controls.Add(m_pClass);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_Cancel);
            this.Controls.Add(m_pOk);
        }
                                                                
        #endregion


        #region Events Handling

        #region method m_pGetAssembly_Click

        private void m_pGetAssembly_Click(object sender, EventArgs e)
        {
            wfrm_se_FilterType frm = new wfrm_se_FilterType(m_pVirtualServer);
			if(frm.ShowDialog() == DialogResult.OK){
                m_pAssembly.Text = frm.AssemblyName;
				m_pClass.Text    = frm.TypeName;
			}
        }

        #endregion


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
			if(m_pFilter == null){
                m_pFilter = m_pVirtualServer.Filters.Add(
                    m_pEnabled.Checked,
                    m_pDescription.Text,
                    m_pAssembly.Text,
                    m_pClass.Text
                );
			}
			else{
				m_pFilter.Enabled = m_pEnabled.Checked;
                m_pFilter.Description = m_pDescription.Text;
                m_pFilter.AssemblyName = m_pAssembly.Text;
                m_pFilter.Class = m_pClass.Text;
                m_pFilter.Commit();
			}

			this.DialogResult = DialogResult.OK;
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets filter ID.
        /// </summary>
        public string FilterID
        {
            get{
                if(m_pFilter != null){
                    return m_pFilter.ID;
                }
                else{
                    return ""; 
                }
            }

        }

        #endregion

    }
}
