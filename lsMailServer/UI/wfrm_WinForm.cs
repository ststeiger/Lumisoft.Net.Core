using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LumiSoft.MailServer;


namespace LumiSoft.MailServer.UI
{
    public partial class wfrm_WinForm : Form
    {

        private Server m_pServer = null;

        public wfrm_WinForm()
        {
            InitializeComponent();

            m_pServer = new Server();
        }

        private void m_pStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_pServer.Start();
                m_pStart.Enabled = false;
                m_pStop.Enabled = true;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        private void m_pStop_Click(object sender, EventArgs e)
        {
            try
            {
                m_pServer.Stop();
                m_pStart.Enabled = true;
                m_pStop.Enabled = false;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        private void wfrm_WinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.m_pServer.Stop();
        }
    }
}
