using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.ServiceProcess;
using LumiSoft.MailServer.Resources;


namespace LumiSoft.MailServer.UI
{
    public partial class wfrm_Install : Form
    {
        public wfrm_Install()
        {
            InitializeComponent();
            this.Icon = ResManager.GetIcon("trayicon.ico");

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                m_pRunAsTryApp.Enabled = true;
                m_pRunAsWindowsForm.Enabled = true;
            }
            else
            {
                if (!IsServiceInstalled())
                {
                    m_pInstallAsService.Enabled = true;
                    m_pUninstallService.Enabled = false;
                    m_pRunAsTryApp.Enabled = true;
                    m_pRunAsWindowsForm.Enabled = true;
                }
                else
                {
                    m_pInstallAsService.Enabled = false;
                    m_pUninstallService.Enabled = true;
                }
            }
        }

        private void m_pInstallAsService_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(Application.StartupPath + "/MailServerService.exe", "-install");
            p.WaitForExit();
            if (p.ExitCode == 0)
            {
                m_pInstallAsService.Enabled = false;
                m_pUninstallService.Enabled = true;
                m_pRunAsTryApp.Enabled = false;
                m_pRunAsWindowsForm.Enabled = false;
            }
        }

        private void m_pUninstallService_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(Application.StartupPath + "/MailServerService.exe", "-uninstall");
            p.WaitForExit();
            if (p.ExitCode == 0)
            {
                m_pInstallAsService.Enabled = true;
                m_pUninstallService.Enabled = false;
                m_pRunAsTryApp.Enabled = true;
                m_pRunAsWindowsForm.Enabled = true;
            }
        }

        private void m_pRunAsTryApp_Click(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                System.Diagnostics.Process.Start("mono", Application.StartupPath + "/lsMailServer.exe -trayapp");
            }
            else
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "/lsMailServer.exe", "-trayapp");
            }

            m_pInstallAsService.Enabled = false;
            m_pUninstallService.Enabled = false;
            m_pRunAsTryApp.Enabled = false;
            m_pRunAsWindowsForm.Enabled = false;
        }

        private void m_pRunAsWindowsForm_Click(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                System.Diagnostics.Process.Start("mono", Application.StartupPath + "/lsMailServer.exe -winform");
            }
            else
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "/lsMailServer.exe", "-winform");
            }

            m_pInstallAsService.Enabled = false;
            m_pUninstallService.Enabled = false;
            m_pRunAsTryApp.Enabled = false;
            m_pRunAsWindowsForm.Enabled = false;
        }


        /// <summary>
        /// Gets if mail server service is installed.
        /// </summary>
        /// <returns></returns>
        private bool IsServiceInstalled()
        {
            foreach (ServiceController service in ServiceController.GetServices())
            {
                if (service.ServiceName == "LumiSoft Mail Server")
                {
                    return true;
                }
            }

            return false;
        }


    } // End Class 
}
