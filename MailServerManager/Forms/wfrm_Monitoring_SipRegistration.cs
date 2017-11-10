using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Aadd/edit SIP registration info.
    /// </summary>
    public class wfrm_Monitoring_SipRegistration : Form
    {
        private PictureBox    m_pIcon          = null;
        private Label         mt_Info          = null;
        private GroupBox      m_pSeparator1    = null;
        private Label         mt_VirtualServer = null;
        private ComboBox      m_pVirtualServer = null;
        private Label         mt_AOR           = null;
        private TextBox       m_pAOR           = null;
        private Button        m_pGetAOR        = null;
        private Label         mt_Contact       = null;
        private TextBox       m_pContact       = null;
        private NumericUpDown m_pExpires       = null;
        private NumericUpDown m_pPriority      = null;
        private ToolStrip     m_pToolbar       = null;
        private ListView      m_pContacts      = null;
        private GroupBox      m_pSeparator2    = null;
        private Button        m_pCancel        = null;
        private Button        m_pOk            = null;

        private Server          m_pServer           = null;
        private SipRegistration m_pRegistration     = null;
        private List<string>    m_pContactsToRemove = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">Reference to contact owner server.</param>
        public wfrm_Monitoring_SipRegistration(Server server)
        {
            m_pServer = server;
            m_pContactsToRemove = new List<string>();

            InitUI();

            m_pToolbar.Items[3].Enabled = false;

            // Load virtual servers
            foreach(VirtualServer virtualServer in m_pServer.VirtualServers){
                m_pVirtualServer.Items.Add(new WComboBoxItem(virtualServer.Name,virtualServer));
            }
            if(m_pVirtualServer.Items.Count > 0){
                m_pVirtualServer.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="server">Reference to contact owner server.</param>
        /// <param name="registration">Registration what contacts to show.</param>
        public wfrm_Monitoring_SipRegistration(Server server,SipRegistration registration)
        {
            m_pServer = server;
            m_pRegistration = registration;
            m_pContactsToRemove = new List<string>();

            InitUI();

            // Load virtual servers
            foreach(VirtualServer virtualServer in m_pServer.VirtualServers){
                m_pVirtualServer.Items.Add(new WComboBoxItem(virtualServer.Name,virtualServer));
            }
            if(m_pVirtualServer.Items.Count > 0){
                m_pVirtualServer.SelectedIndex = 0;
            }

            m_pVirtualServer.Enabled = false;
            m_pAOR.Enabled = false;
            m_pAOR.Text = registration.AddressOfRecord;

            LoadContacts();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(500,320);
            this.Text = "Add/Update SIP registration.";
            this.Icon = ResManager.GetIcon("rule.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("rule.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(450,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "SIP registration info.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(485,3);
            m_pSeparator1.Location = new Point(7,50);
            m_pSeparator1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_VirtualServer = new Label();
            mt_VirtualServer.Size = new Size(140,20);
            mt_VirtualServer.Location = new Point(0,65);
            mt_VirtualServer.TextAlign = ContentAlignment.MiddleRight;
            mt_VirtualServer.Text = "Virtual Server:";

            m_pVirtualServer = new ComboBox();
            m_pVirtualServer.Size = new Size(315,20);
            m_pVirtualServer.Location = new Point(145,65);
            m_pVirtualServer.DropDownStyle = ComboBoxStyle.DropDownList;
            
            mt_AOR = new Label();
            mt_AOR.Size = new Size(140,20);
            mt_AOR.Location = new Point(0,90);
            mt_AOR.TextAlign = ContentAlignment.MiddleRight;
            mt_AOR.Text = "Address of Record:";

            m_pAOR = new TextBox();
            m_pAOR.Size = new Size(315,20);
            m_pAOR.Location = new Point(145,90);

            m_pGetAOR = new Button();
            m_pGetAOR.Size = new Size(25,20);
            m_pGetAOR.Location = new Point(465,90);
            m_pGetAOR.Text = "...";
            m_pGetAOR.Click += new EventHandler(m_pGetAOR_Click);
            m_pGetAOR.Enabled = false;

            mt_Contact = new Label();
            mt_Contact.Size = new Size(140,20);
            mt_Contact.Location = new Point(0,115);
            mt_Contact.TextAlign = ContentAlignment.MiddleRight;
            mt_Contact.Text = "Contact URI:";

            m_pContact = new TextBox();
            m_pContact.Size = new Size(160,20);
            m_pContact.Location = new Point(145,115);

            m_pExpires = new NumericUpDown();
            m_pExpires.Size = new Size(50,20);
            m_pExpires.Location = new Point(310,115);
            m_pExpires.Minimum = 60;
            m_pExpires.Maximum = 9999;

            m_pPriority = new NumericUpDown();
            m_pPriority.Size = new Size(45,20);
            m_pPriority.Location = new Point(365,115);
            m_pPriority.DecimalPlaces = 2;
            m_pPriority.Minimum = (decimal)0.1;
            m_pPriority.Maximum = (decimal)1.0;
            m_pPriority.Value = (decimal)1.0;
           
            m_pToolbar = new ToolStrip();
            m_pToolbar.Location = new Point(430,113);
            m_pToolbar.Size = new Size(60,25);
            m_pToolbar.Dock = DockStyle.None;
            m_pToolbar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pToolbar.BackColor = this.BackColor;
            m_pToolbar.Renderer = new ToolBarRendererEx();
            m_pToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pToolbar_ItemClicked);
            // Add button
            ToolStripButton button_Add = new ToolStripButton();
            button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            button_Add.Tag = "add";
            button_Add.ToolTipText = "Add";
            m_pToolbar.Items.Add(button_Add);
            // Delete button
            ToolStripButton button_Delete = new ToolStripButton();
            button_Delete.Enabled = false;
            button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            button_Delete.Tag = "delete";
            button_Delete.ToolTipText = "Delete";
            m_pToolbar.Items.Add(button_Delete);
            // Separator
            m_pToolbar.Items.Add(new ToolStripSeparator());
            // Refresh button
            ToolStripButton button_Refresh = new ToolStripButton();
            button_Refresh.Image = ResManager.GetIcon("refresh.ico").ToBitmap();
            button_Refresh.Tag = "refresh";
            button_Refresh.ToolTipText  = "Refresh";
            m_pToolbar.Items.Add(button_Refresh);

            m_pContacts = new ListView();
            m_pContacts.Size = new Size(480,130);
            m_pContacts.Location = new Point(10,145);
            m_pContacts.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pContacts.View = View.Details;
            m_pContacts.FullRowSelect = true;
            m_pContacts.HideSelection = false;
            m_pContacts.Columns.Add("Contact URI",340);
            m_pContacts.Columns.Add("Expires",60);
            m_pContacts.Columns.Add("Priority",50);
            m_pContacts.SelectedIndexChanged += new EventHandler(m_pContacts_SelectedIndexChanged);

            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(485,4);
            m_pSeparator2.Location = new Point(7,285);
            m_pSeparator2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(340,295);
            m_pCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(415,295);
            m_pOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_VirtualServer);
            this.Controls.Add(m_pVirtualServer);
            this.Controls.Add(mt_AOR);
            this.Controls.Add(m_pAOR);
            this.Controls.Add(m_pGetAOR);
            this.Controls.Add(mt_Contact);
            this.Controls.Add(m_pContact);
            this.Controls.Add(m_pExpires);
            this.Controls.Add(m_pPriority);
            this.Controls.Add(m_pToolbar);
            this.Controls.Add(m_pContacts);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                                                
        #endregion


        #region Events Handling

        #region method m_pGetAOR_Click

        private void m_pGetAOR_Click(object sender,EventArgs e)
        {
            //wfrm_se
        }

        #endregion

        #region method m_pToolbar_ItemClicked

        private void m_pToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }
            else if(e.ClickedItem.Tag.ToString() == "add"){
                if(m_pContact.Text.Length < 3){
                    MessageBox.Show(this,"Please specify Contact URI !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                ListViewItem it = new ListViewItem(m_pContact.Text);
                it.SubItems.Add(m_pExpires.Value.ToString());
                it.SubItems.Add(m_pPriority.Value.ToString("f2"));
                it.Tag = true;
                m_pContacts.Items.Add(it);

                m_pContact.Text = "";
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                ListViewItem selectedItem = m_pContacts.SelectedItems[0];
                // Add only existing contacts to remove list, don't add uncommited contacts.
                if(!((bool)selectedItem.Tag)){
                    m_pContactsToRemove.Add(selectedItem.Text);                    
                }
                selectedItem.Remove();
            }
            else if(e.ClickedItem.Tag.ToString() == "refresh"){
                LoadContacts();
            }
        }

        #endregion

        #region method m_pContacts_SelectedIndexChanged

        private void m_pContacts_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(m_pContacts.SelectedItems.Count > 0){
                m_pToolbar.Items[1].Enabled = true;
            }
            else{
                m_pToolbar.Items[1].Enabled = false;
            }
        }

        #endregion


        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender,EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender,EventArgs e)
        {
            if(m_pAOR.Text.Length < 3){
                MessageBox.Show(this,"Please specify Address of Record !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            List<string> contacts = new List<string>();
            // Add remove items
            foreach(string contact in m_pContactsToRemove){
                contacts.Add("<" + contact + ">;expires=0");
            }
            // Add new items
            foreach(ListViewItem item in m_pContacts.Items){
                // We need new items only, others we don't touch.
                if(((bool)item.Tag)){
                    contacts.Add("<" + item.Text + ">;expires=" + item.SubItems[1].Text + ";qvalue=" + item.SubItems[2].Text.Replace(',','.'));                    
                }
            }
            
            // Add or update registration info.
            ((VirtualServer)((WComboBoxItem)m_pVirtualServer.SelectedItem).Tag).SipRegistrations.Set(m_pAOR.Text,contacts.ToArray());

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #endregion


        #region method LoadContacts

        /// <summary>
        /// Loads contacts to UI.
        /// </summary>
        private void LoadContacts()
        {
            m_pContacts.Items.Clear();

            m_pRegistration.Refresh();
            foreach(SipRegistrationContact contact in m_pRegistration.Contacts){
                ListViewItem it = new ListViewItem(contact.ContactUri);
                it.SubItems.Add(contact.Expires.ToString());
                it.SubItems.Add(contact.Priority.ToString("f2"));
                it.Tag = false;
                m_pContacts.Items.Add(it);
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets current virtual server.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return (VirtualServer)((WComboBoxItem)m_pVirtualServer.SelectedItem).Tag; }
        }

        /// <summary>
        /// Gets current SIP registration address of record.
        /// </summary>
        public string AddressOfRecord
        {
            get{ return m_pAOR.Text; }
        }

        #endregion

    }
}
