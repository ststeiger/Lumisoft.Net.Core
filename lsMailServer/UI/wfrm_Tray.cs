
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer;
using LumiSoft.MailServer.Resources;


namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Adds tray app support to mail server.
    /// </summary>
    public class wfrm_Tray : Form
    {
        private ContextMenuStrip m_pMenu = null;
        private NotifyIcon m_pNotyfyIcon = null;

        private Server m_pMailServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_Tray()
        {
            InitUI();

            m_pMailServer = new Server();

            Start();
        }

        #region method Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (m_pMailServer != null)
            {
                m_pMailServer.Dispose();
                m_pMailServer = null;
            }
            if (m_pNotyfyIcon != null)
            {
                m_pNotyfyIcon.Dispose();
                m_pNotyfyIcon = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region method InitUI

        /// <summary>
        /// Creates and intializes UI.
        /// </summary>
        private void InitUI()
        {
            this.ShowInTaskbar = false;
            this.ControlBox = false;
            this.WindowState = FormWindowState.Minimized;

            m_pMenu = new ContextMenuStrip();
            ToolStripMenuItem manager = new ToolStripMenuItem("Open Manager");
            manager.Tag = "manager";
            m_pMenu.Items.Add(manager);
            m_pMenu.Items.Add(new ToolStripSeparator());
            ToolStripMenuItem start = new ToolStripMenuItem("Start");
            start.Tag = "start";
            m_pMenu.Items.Add(start);
            ToolStripMenuItem stop = new ToolStripMenuItem("Stop");
            stop.Tag = "stop";
            m_pMenu.Items.Add(stop);
            m_pMenu.Items.Add(new ToolStripSeparator());
            ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
            exit.Image = ResManager.GetIcon("exit.ico").ToBitmap();
            exit.Tag = "exit";
            m_pMenu.Items.Add(exit);
            m_pMenu.ItemClicked += new ToolStripItemClickedEventHandler(m_pMenu_ItemClicked);

            m_pNotyfyIcon = new NotifyIcon();
            m_pNotyfyIcon.Icon = ResManager.GetIcon("trayicon.ico");
            m_pNotyfyIcon.ContextMenuStrip = m_pMenu;
            m_pNotyfyIcon.Text = "LumiSoft Mail Server";
            m_pNotyfyIcon.Visible = true;
        }

        #endregion


        #region Events Handling

        #region method m_pMenu_ItemClicked

        private void m_pMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null)
            {
                return;
            }

            if (e.ClickedItem.Tag.ToString() == "manager")
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "/mailservermanager.exe");
            }
            else if (e.ClickedItem.Tag.ToString() == "start")
            {
                Start();
            }
            else if (e.ClickedItem.Tag.ToString() == "stop")
            {
                Stop();
            }
            else if (e.ClickedItem.Tag.ToString() == "exit")
            {
                Exit();
            }
        }

        #endregion

        #endregion


        #region method Start

        /// <summary>
        /// Starts mail server.
        /// </summary>
        private void Start()
        {
            try
            {
                m_pMailServer.Start();
                m_pMenu.Items[2].Enabled = false;
                m_pMenu.Items[3].Enabled = true;
            }
            catch (Exception x)
            {
                Error.DumpError(x, new System.Diagnostics.StackTrace());
            }
        }

        #endregion

        #region method Stop

        /// <summary>
        /// Stops server.
        /// </summary>
        private void Stop()
        {
            try
            {
                m_pMailServer.Stop();
                m_pMenu.Items[2].Enabled = true;
                m_pMenu.Items[3].Enabled = false;
            }
            catch (Exception x)
            {
                Error.DumpError(x, new System.Diagnostics.StackTrace());
            }
        }

        #endregion

        #region method Exit

        /// <summary>
        /// Exits tray application, stops server.
        /// </summary>
        private void Exit()
        {
            try
            {
                this.Close();
            }
            catch (Exception x)
            {
                Error.DumpError(x, new System.Diagnostics.StackTrace());
            }
        }

        #endregion

    }
}
