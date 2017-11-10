using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.UI.Controls;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Events and logs Events window.
    /// </summary>
    public class wfrm_EventsAndLogs_Events : Form
    {
        private Button    m_pClearAllEvents = null;
        private ImageList m_pEventsImages   = null;
        private WListView m_pEvents         = null;

        private Server m_pServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Mail server.</param>
        /// <param name="frame"></param>
        public wfrm_EventsAndLogs_Events(Server server,WFrame frame)
        {
            m_pServer = server;

            InitUI();

            LoadEvents();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(450,300);

            m_pClearAllEvents = new Button();            
            m_pClearAllEvents.Size = new Size(100,20);
            m_pClearAllEvents.Location = new Point(9,15);
            m_pClearAllEvents.Text = "Clear all Events";
            m_pClearAllEvents.Click += new EventHandler(m_pClearAllEvents_Click);

            m_pEventsImages = new ImageList();            
            m_pEventsImages.Images.Add(ResManager.GetIcon("error.ico"));

            m_pEvents = new WListView();
            m_pEvents.Size = new Size(425,210);
            m_pEvents.Location = new Point(9,47);
            m_pEvents.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pEvents.View = View.Details;
            m_pEvents.FullRowSelect = true;
            m_pEvents.HideSelection = false;
            m_pEvents.SmallImageList = m_pEventsImages;
            m_pEvents.DoubleClick += new EventHandler(m_pEvents_DoubleClick);
            m_pEvents.Columns.Add("",20,HorizontalAlignment.Left);
            m_pEvents.Columns.Add("Virtual Server",120,HorizontalAlignment.Left);
            m_pEvents.Columns.Add("Date",130,HorizontalAlignment.Left);
            m_pEvents.Columns.Add("Text",200,HorizontalAlignment.Left);

            this.Controls.Add(m_pClearAllEvents);
            this.Controls.Add(m_pEvents);
        }
                                
        #endregion


        #region Events Handling

        #region method m_pClearAllEvents_Click

        private void m_pClearAllEvents_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show(this,"Are you sure you want to delete all events","Confirmation:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){
                m_pServer.Events.Clear();
                m_pEvents.Items.Clear();
            }
        }

        #endregion

        #region method m_pEvents_DoubleClick

        private void m_pEvents_DoubleClick(object sender, EventArgs e)
        {
            if(m_pEvents.SelectedItems.Count > 0){
                Event evnt = (Event)m_pEvents.SelectedItems[0].Tag;
                wfrm_EventsAndLogs_Event frm = new wfrm_EventsAndLogs_Event(
                    evnt.CreateDate,
                    evnt.Text
                );
                frm.ShowDialog(this);

            }
        }

        #endregion

        #endregion


        #region method LoadEvents

        /// <summary>
        /// Loads events to UI.
        /// </summary>
        private void LoadEvents()
        {
            m_pServer.Events.Refresh();
            foreach(Event evnt in m_pServer.Events){
                ListViewItem it = new ListViewItem();
                it.ImageIndex = 0;
                it.SubItems.Add(evnt.VirtualServer);
                it.SubItems.Add(evnt.CreateDate.ToString());
                it.SubItems.Add(evnt.Text.ToString().Split(new char[]{'\n'},2)[0]);
                it.Tag = evnt;
                m_pEvents.Items.Add(it);
            }
        }

        #endregion

    }
}
