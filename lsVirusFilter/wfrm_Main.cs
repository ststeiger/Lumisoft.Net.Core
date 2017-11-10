using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Reflection;

namespace LumiSoft.MailServer.Filters
{
    /// <summary>
    /// Application main form.
    /// </summary>
    public class wfrm_Main : Form
    {
        private Label         mt_ScanProgram     = null;
        private ComboBox      m_pScanProgram     = null;
        private Button        m_pGetScanProgram  = null;
        private Label         mt_ScanProgramArgs = null;
        private TextBox       m_pScanProgramArgs = null;        
        private Label         mt_VirusIfExitCode = null;
        private NumericUpDown m_pVirusIfExitCode = null;
        private GroupBox      m_pGroupBox1       = null;
        private Button        m_pCancel          = null;
        private Button        m_pOk              = null;

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
            this.Size = new Size(450,200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "LumiSoft Virus Filter Settings";

            mt_ScanProgram = new Label();
            mt_ScanProgram.Size = new Size(400,20);
            mt_ScanProgram.Location = new Point(10,10);
            mt_ScanProgram.Text = "Scan program:";

            m_pScanProgram = new ComboBox();
            m_pScanProgram.Size = new Size(400,20);
            m_pScanProgram.Location = new Point(10,30);
            m_pScanProgram.SelectedIndexChanged += new EventHandler(m_pScanProgram_SelectedIndexChanged);
            m_pScanProgram.Items.Add("C:\\Program Files\\ClamWin\\bin\\clamscan.exe");

            m_pGetScanProgram = new Button();
            m_pGetScanProgram.Size = new Size(25,20);
            m_pGetScanProgram.Location = new Point(415,30);
            m_pGetScanProgram.Text = "...";
            m_pGetScanProgram.Click += new EventHandler(m_pGetScanProgram_Click);
            
            mt_ScanProgramArgs = new Label();
            mt_ScanProgramArgs.Size = new Size(400,20);
            mt_ScanProgramArgs.Location = new Point(10,60);
            mt_ScanProgramArgs.Text = "Scan program command line arguments:";

            m_pScanProgramArgs = new TextBox();
            m_pScanProgramArgs.Size = new Size(400,20);
            m_pScanProgramArgs.Location = new Point(10,80);

            mt_VirusIfExitCode = new Label();
            mt_VirusIfExitCode.Size = new Size(100,20);
            mt_VirusIfExitCode.Location = new Point(5,110);
            mt_VirusIfExitCode.TextAlign = ContentAlignment.MiddleRight;
            mt_VirusIfExitCode.Text = "Virus if exit code:";

            m_pVirusIfExitCode = new NumericUpDown();
            m_pVirusIfExitCode.Size = new Size(60,20);
            m_pVirusIfExitCode.Location = new Point(110,110);

            m_pGroupBox1 = new GroupBox();
            m_pGroupBox1.Size = new Size(435,4);
            m_pGroupBox1.Location = new Point(5,135);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(295,145);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();            
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(370,145);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(mt_ScanProgram);
            this.Controls.Add(m_pScanProgram);
            this.Controls.Add(m_pGetScanProgram);
            this.Controls.Add(mt_ScanProgramArgs);
            this.Controls.Add(m_pScanProgramArgs);
            this.Controls.Add(mt_VirusIfExitCode);
            this.Controls.Add(m_pVirusIfExitCode);
            this.Controls.Add(m_pGroupBox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }

        #endregion


        #region Events Handling

        #region method m_pGetScanProgram_Click

        private void m_pGetScanProgram_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
			dlg.RestoreDirectory = true;
			dlg.Filter = "Executable (*.exe)|*.exe";
			if(dlg.ShowDialog(this) == DialogResult.OK){
				m_pScanProgram.Text = dlg.FileName;
			}
        }

        #endregion

        #region method m_pScanProgram_SelectedIndexChanged

        private void m_pScanProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pScanProgram.Text.ToLower().IndexOf("clamscan") > -1){
                m_pScanProgramArgs.Text = "--database=\"C:\\Documents and Settings\\All Users\\.clamwin\\db\" \"#FileName\"";
                m_pVirusIfExitCode.Value = 1;
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
            this.Cursor = Cursors.WaitCursor;

            bool scannerOk = false;
            // Test that virus scanner block "Eicar" test.
            try{
                // Create test eicar file
                Stream rs = Assembly.GetExecutingAssembly().GetManifestResourceStream("LumiSoft.MailServer.Filters.Resources.eicar_message.eml.base64");
                byte[] data = new byte[rs.Length];
                rs.Read(data,0,data.Length);
                data = Convert.FromBase64String(System.Text.Encoding.Default.GetString(data));
                File.WriteAllBytes(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml",data);

                //--- Scan test file ------------------------------------------------------------------------------
                string virusSoft     = m_pScanProgram.Text;
				string virusSoftArgs = m_pScanProgramArgs.Text.Replace("#FileName",Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml");
                int    virusExitCode = (int)m_pVirusIfExitCode.Value;

                int exitCode = 0;
				System.Diagnostics.ProcessStartInfo sInf = new System.Diagnostics.ProcessStartInfo(virusSoft,virusSoftArgs);
				sInf.CreateNoWindow = true;
				sInf.UseShellExecute = false;
                sInf.RedirectStandardError = true;
				System.Diagnostics.Process p = System.Diagnostics.Process.Start(sInf);
				if(p != null){
					p.WaitForExit(60000);
                    exitCode = p.ExitCode;
				}
                                                					
                // Do exit code mapping, we must get virus reported or scanner won' work.
                if(virusExitCode != exitCode){
                    string response = p.StandardError.ReadToEnd();
			    	if(response == null){
					    response = "";
				    }

                    MessageBox.Show(this,"Error your scanner don't block test 'Eicar' virus file !\r\n\r\nScanner returned:\r\n" + response,"Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);                    
                }
                else{
                    scannerOk = true;
                }
                //-------------------------------------------------------------------------------------------------

                // Delete eicar test file
                if(File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml")){
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\eicar_virus_test.eml");
                }
            }
            catch(Exception x){
                MessageBox.Show("Error: " + x.Message);
            }

            if(scannerOk){
                SaveSettings();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

        #endregion


        #region method LoadSettings

        /// <summary>
        /// Loads settings from xml to UI.
        /// </summary>
        private void LoadSettings()
        {
            m_pDsSettings = new DataSet();
            m_pDsSettings.Tables.Add("Settings");
            m_pDsSettings.Tables["Settings"].Columns.Add("Program");
			m_pDsSettings.Tables["Settings"].Columns.Add("Arguments");
            m_pDsSettings.Tables["Settings"].Columns.Add("VirusExitCode");

            if(File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\lsVirusFilter_db.xml")){
                m_pDsSettings.ReadXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\lsVirusFilter_db.xml");
            }
            else{
                DataRow dr = m_pDsSettings.Tables["Settings"].NewRow();
                dr["Program"]       = "";
                dr["Arguments"]     = "";
                dr["VirusExitCode"] = 1;
                m_pDsSettings.Tables["Settings"].Rows.Add();
            }

            m_pScanProgram.Text      = m_pDsSettings.Tables["Settings"].Rows[0]["Program"].ToString();
            m_pScanProgramArgs.Text  = m_pDsSettings.Tables["Settings"].Rows[0]["Arguments"].ToString();
            m_pVirusIfExitCode.Value = ConvertEx.ToInt32(m_pDsSettings.Tables["Settings"].Rows[0]["VirusExitCode"],1);
        }

        #endregion

        #region method SaveSettings

        /// <summary>
        /// Saves settings to xml file.
        /// </summary>
        private void SaveSettings()
        {
            m_pDsSettings.Tables["Settings"].Rows[0]["Program"]       = m_pScanProgram.Text;
            m_pDsSettings.Tables["Settings"].Rows[0]["Arguments"]     = m_pScanProgramArgs.Text;
            m_pDsSettings.Tables["Settings"].Rows[0]["VirusExitCode"] = m_pVirusIfExitCode.Value;

            m_pDsSettings.WriteXml(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\lsVirusFilter_db.xml");
        }

        #endregion

    }
}
