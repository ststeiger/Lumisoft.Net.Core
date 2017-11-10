using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.Net;
using LumiSoft.Net.MIME;
using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Queues Incoming SMTP queues window.
    /// </summary>
    public class wfrm_Queues_IncomingSMTP : Form
    {
        private Button   m_pGet   = null;
        private ListView m_pQueue = null;

        private VirtualServer m_pVirtualServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_Queues_IncomingSMTP(VirtualServer virtualServer)
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
            this.Size = new Size(470,350);

            m_pGet = new Button();
            m_pGet.Size = new Size(70,20);
            m_pGet.Location = new Point(385,30);
            m_pGet.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pGet.Text = "Get";
            m_pGet.Click += new EventHandler(m_pGet_Click);

            m_pQueue = new ListView();
            m_pQueue.Size = new Size(445,240);
            m_pQueue.Location = new Point(10,60);
            m_pQueue.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pQueue.View = View.Details;
            m_pQueue.HideSelection = false;
            m_pQueue.FullRowSelect = true;
            m_pQueue.Columns.Add("Created",115,HorizontalAlignment.Left);
            m_pQueue.Columns.Add("From",100,HorizontalAlignment.Left);
            m_pQueue.Columns.Add("To",100,HorizontalAlignment.Left);
            m_pQueue.Columns.Add("Subject",150,HorizontalAlignment.Left);

            this.Controls.Add(m_pGet);
            this.Controls.Add(m_pQueue);
        }
                
        #endregion


        #region Events Handling

        #region method m_pGet_Click

        private void m_pGet_Click(object sender, EventArgs e)
        {
            m_pQueue.Items.Clear();
            m_pVirtualServer.Queues.SMTP.Refresh();

            foreach(QueueItem item in m_pVirtualServer.Queues.SMTP){
                MIME_h_Collection header = new MIME_h_Collection(new MIME_h_Provider());
                header.Parse(item.Header);
                     
                string from = "";
                if(header.GetFirst("From") != null){
                    from = header.GetFirst("From").ToString().Split(new char[]{':'},2)[1];
                }

                string to = "";
                if(header.GetFirst("To") != null){
                    to = header.GetFirst("To").ToString().Split(new char[]{':'},2)[1];
                }

                string subject = "";
                if(header.GetFirst("Subject") != null){
                    subject = header.GetFirst("Subject").ToString().Split(new char[]{':'},2)[1];
                }

                ListViewItem it = new ListViewItem();
                it.Text = item.CreateTime.ToString();
                it.SubItems.Add(from);
                it.SubItems.Add(to);
                it.SubItems.Add(subject);
                m_pQueue.Items.Add(it);
            }
        }

        #endregion

        #endregion

    }
}
