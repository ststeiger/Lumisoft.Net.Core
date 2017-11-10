using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Server return messages.
    /// </summary>
    public class wfrm_System_ReturnMessages : Form
    {
        private Label       mt_MessageType = null;
        private ComboBox    m_pMessageType = null;
        private Label       mt_Subject     = null;
        private TextBox     m_pSubject     = null;
        private WRichEditEx m_pText        = null;
        private Button      m_pHelp        = null;
        private Button      m_pSave        = null;

        private VirtualServer m_pVirtualServer      = null;
        private WComboBoxItem m_pCurrentMessageType = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_System_ReturnMessages(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            m_pMessageType.SelectedIndex = 0;
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(400,400);

            mt_MessageType = new Label();
            mt_MessageType.Size = new Size(100,20);
            mt_MessageType.Location = new Point(0,20);
            mt_MessageType.TextAlign = ContentAlignment.MiddleRight;
            mt_MessageType.Text = "Message Type:";
            
            m_pMessageType = new ComboBox();
            m_pMessageType.Size = new Size(200,20);
            m_pMessageType.Location = new Point(105,20);
            m_pMessageType.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pMessageType.Items.Add(new WComboBoxItem("Delayed delivery warning","delayed_delivery_warning"));
            m_pMessageType.Items.Add(new WComboBoxItem("Undelivered notice","undelivered"));
            m_pMessageType.SelectedIndexChanged += new EventHandler(m_pMessageType_SelectedIndexChanged);

            mt_Subject = new Label();
            mt_Subject.Size = new Size(100,20);
            mt_Subject.Location = new Point(0,65);
            mt_Subject.TextAlign = ContentAlignment.MiddleRight;
            mt_Subject.Text = "Subject:";

            m_pSubject = new TextBox();
            m_pSubject.Size = new Size(280,20);
            m_pSubject.Location = new Point(105,65);
            m_pSubject.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pText = new WRichEditEx();
            m_pText.Size = new Size(375,240);
            m_pText.Location = new Point(10,90);
            m_pText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pHelp = new Button();
            m_pHelp.Size = new Size(70,20);
            m_pHelp.Location = new Point(10,340);
            m_pHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            m_pHelp.Text = "Help";
            m_pHelp.Click += new EventHandler(m_pHelp_Click);

            m_pSave = new Button();
            m_pSave.Size = new Size(70,20);
            m_pSave.Location = new Point(315,340);
            m_pSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pSave.Text = "Save";
            m_pSave.Click += new EventHandler(m_pSave_Click);

            this.Controls.Add(mt_MessageType);
            this.Controls.Add(m_pMessageType);
            this.Controls.Add(mt_Subject);
            this.Controls.Add(m_pSubject);
            this.Controls.Add(m_pText);
            this.Controls.Add(m_pHelp);
            this.Controls.Add(m_pSave);
        }
                
        #endregion


        #region Events Handling

        #region method m_pMessageType_SelectedIndexChanged

        private void m_pMessageType_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pMessageType.SelectedItem == null){
                return;
            }

            // If selected item changed, store old item value.
            if(m_pCurrentMessageType != null){
                if(m_pCurrentMessageType.Tag.ToString() == "delayed_delivery_warning"){
                    m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning = new ServerReturnMessage(m_pSubject.Text,m_pText.Rtf);                    
                }
                else if(m_pCurrentMessageType.Tag.ToString() == "undelivered"){
                    m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered = new ServerReturnMessage(m_pSubject.Text,m_pText.Rtf);
                }
            }

            m_pCurrentMessageType = (WComboBoxItem)m_pMessageType.SelectedItem;
            if(m_pCurrentMessageType.Tag.ToString() == "delayed_delivery_warning"){
                m_pSubject.Text = m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning.Subject;
                m_pText.Rtf     = m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning.BodyTextRtf;
            }
            else if(m_pCurrentMessageType.Tag.ToString() == "undelivered"){
                m_pSubject.Text = m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered.Subject;
                m_pText.Rtf     = m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered.BodyTextRtf;
            }
        }

        #endregion

        #region method m_pHelp_Click

        private void m_pHelp_Click(object sender,EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo pInf = new System.Diagnostics.ProcessStartInfo("explorer",Application.StartupPath + "\\Help\\System.ReturnMessages.txt");
			System.Diagnostics.Process.Start(pInf);
        }

        #endregion

        #region method m_pSave_Click

        private void m_pSave_Click(object sender,EventArgs e)
        {
            if(m_pCurrentMessageType.Tag.ToString() == "delayed_delivery_warning"){
                m_pVirtualServer.SystemSettings.ReturnMessages.DelayedDeliveryWarning = new ServerReturnMessage(m_pSubject.Text,m_pText.Rtf);
            }
            else if(m_pCurrentMessageType.Tag.ToString() == "undelivered"){
                m_pVirtualServer.SystemSettings.ReturnMessages.Undelivered = new ServerReturnMessage(m_pSubject.Text,m_pText.Rtf);
            }

            m_pVirtualServer.SystemSettings.Commit();
        }

        #endregion

        #endregion

    }
}
