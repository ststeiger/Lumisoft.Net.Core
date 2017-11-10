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
    /// Routing add/edit route window.
    /// </summary>
    public class wfrm_Routing_Route : Form
    {
        //--- Common UI -------------------------
        private PictureBox m_pIcon        = null;
        private Label      mt_Info        = null;
        private GroupBox   m_pSeparator1  = null;
        private CheckBox   m_pEnabled     = null;
        private Label      mt_Description = null;
        private TextBox    m_pDescription = null;
        private Label      mt_Pattern     = null;
        private TextBox    m_pPattern     = null;
        private Label      mt_Action      = null;
        private ComboBox   m_pAction      = null;        
        private GroupBox   m_pGroubBox1   = null;
        private Button     m_pCancel      = null;
        private Button     m_pOk          = null;
        //--- Action -> Route To Mailbox UI
        private Label    mt_RouteToMailbox_Mailbox    = null;
        private TextBox  m_pRouteToMailbox_Mailbox    = null;
        private Button   m_pRouteToMailbox_GetMailbox = null;
        //--- Action -> Route To Email UI
        private Label    mt_RouteToEmail_Email = null;
        private TextBox  m_pRouteToEmail_Email = null;
        //--- Action -> Route To Host
        private Label         mt_RouteToHost_Host = null;
        private TextBox       m_pRouteToHost_Host = null;
        private NumericUpDown m_pRouteToHost_Port = null;

        private VirtualServer m_pVirtualServer = null;
        private Route         m_pRoute         = null;

        /// <summary>
        /// Add constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        public wfrm_Routing_Route(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();
                        
            m_pAction.SelectedIndex = 0;
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="virtualServer">Virtual server.</param>
        /// <param name="route">Route to update.</param>
        public wfrm_Routing_Route(VirtualServer virtualServer,Route route)
        {
            m_pVirtualServer = virtualServer;
            m_pRoute         = route;

            InitUI();

            m_pEnabled.Checked = route.Enabled;
            m_pDescription.Text = route.Description;
            m_pPattern.Text = route.Pattern;
            
            #region RouteToMailbox

            if(route.Action.ActionType == RouteAction_enum.RouteToMailbox){
                m_pRouteToMailbox_Mailbox.Text = ((RouteAction_RouteToMailbox)route.Action).Mailbox;

                m_pAction.SelectedIndex = 0;
            }

            #endregion

            #region RouteToEmail

            else if(route.Action.ActionType == RouteAction_enum.RouteToEmail){
                m_pRouteToEmail_Email.Text = ((RouteAction_RouteToEmail)route.Action).EmailAddress;

                m_pAction.SelectedIndex = 1;
            }

            #endregion

            #region RouteToHost

            else if(route.Action.ActionType == RouteAction_enum.RouteToHost){
                m_pRouteToHost_Host.Text  = ((RouteAction_RouteToHost)route.Action).Host;
                m_pRouteToHost_Port.Value = ((RouteAction_RouteToHost)route.Action).Port;

                m_pAction.SelectedIndex = 2;
            }

            #endregion             
             
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
            this.Text = "Add/Edit route";
            this.Icon = ResManager.GetIcon("ruleaction.ico");

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify route information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(383,3);
            m_pSeparator1.Location = new Point(7,50);
                        
            m_pEnabled = new CheckBox();
            m_pEnabled.Size = new Size(150,20);
            m_pEnabled.Location = new Point(105,65);
            m_pEnabled.Text = "Enabled";
            m_pEnabled.Checked = true;

            mt_Description = new Label();
            mt_Description.Size = new Size(100,20);
            mt_Description.Location = new Point(0,90);
            mt_Description.TextAlign = ContentAlignment.BottomRight;
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(275,20);
            m_pDescription.Location = new Point(105,90);

            mt_Pattern = new Label();
            mt_Pattern.Size = new Size(100,20);
            mt_Pattern.Location = new Point(0,115);
            mt_Pattern.TextAlign = ContentAlignment.BottomRight;
            mt_Pattern.Text = "Pattern:";

            m_pPattern = new TextBox();
            m_pPattern.Size = new Size(200,20);
            m_pPattern.Location = new Point(105,115);

            mt_Action = new Label();
            mt_Action.Size = new Size(100,20);
            mt_Action.Location = new Point(0,140);
            mt_Action.TextAlign = ContentAlignment.BottomRight;
            mt_Action.Text = "Action:";

            m_pAction = new ComboBox();
            m_pAction.Size = new Size(200,20);
            m_pAction.Location = new Point(105,140);
            m_pAction.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pAction.SelectedIndexChanged += new EventHandler(m_pAction_SelectedIndexChanged);
            m_pAction.Items.Add(new WComboBoxItem("Route To Mailbox",RouteAction_enum.RouteToMailbox));
            m_pAction.Items.Add(new WComboBoxItem("Route To Email",RouteAction_enum.RouteToEmail));
            m_pAction.Items.Add(new WComboBoxItem("Route To Host",RouteAction_enum.RouteToHost));
            
            #region Route To Mailbox UI

            //--- Action -> Route To Mailbox UI ------------------------//
            mt_RouteToMailbox_Mailbox = new Label();
            mt_RouteToMailbox_Mailbox.Size = new Size(100,18);
            mt_RouteToMailbox_Mailbox.Location = new Point(0,175);
            mt_RouteToMailbox_Mailbox.Visible = false;
            mt_RouteToMailbox_Mailbox.TextAlign = ContentAlignment.MiddleRight;
            mt_RouteToMailbox_Mailbox.Text = "Mailbox:";

            m_pRouteToMailbox_Mailbox = new TextBox();
            m_pRouteToMailbox_Mailbox.Size = new Size(200,20);
            m_pRouteToMailbox_Mailbox.Location = new Point(105,175);
            m_pRouteToMailbox_Mailbox.Visible = false;
            m_pRouteToMailbox_Mailbox.ReadOnly = true;

            m_pRouteToMailbox_GetMailbox = new Button();
            m_pRouteToMailbox_GetMailbox.Size = new Size(25,20);
            m_pRouteToMailbox_GetMailbox.Location = new Point(315,175);
            m_pRouteToMailbox_GetMailbox.Visible = false;
            m_pRouteToMailbox_GetMailbox.Text = "...";
            m_pRouteToMailbox_GetMailbox.Click += new EventHandler(m_pRouteToMailbox_GetMailbox_Click);
            //---------------------------------------------------------//

            #endregion

            #region Route To Email UI

            //--- Action -> Route To Email UI -------------------------//
            mt_RouteToEmail_Email = new Label();
            mt_RouteToEmail_Email.Size = new Size(100,18);
            mt_RouteToEmail_Email.Location = new Point(0,175);
            mt_RouteToEmail_Email.Visible = false;
            mt_RouteToEmail_Email.TextAlign = ContentAlignment.MiddleRight;
            mt_RouteToEmail_Email.Text = "Email:";

            m_pRouteToEmail_Email = new TextBox();
            m_pRouteToEmail_Email.Size = new Size(200,20);
            m_pRouteToEmail_Email.Location = new Point(105,175);
            m_pRouteToEmail_Email.Visible = false;
            //---------------------------------------------------------//

            #endregion

            #region Route To Host

            //--- Action -> Route To Host -----------------------------//
            mt_RouteToHost_Host = new Label();
            mt_RouteToHost_Host.Size = new Size(100,18);
            mt_RouteToHost_Host.Location = new Point(0,175);
            mt_RouteToHost_Host.Visible = false;
            mt_RouteToHost_Host.TextAlign = ContentAlignment.MiddleRight;
            mt_RouteToHost_Host.Text = "Host:";

            m_pRouteToHost_Host = new TextBox();
            m_pRouteToHost_Host.Size = new Size(200,20);
            m_pRouteToHost_Host.Location = new Point(105,175);
            m_pRouteToHost_Host.Visible = false;

            m_pRouteToHost_Port = new NumericUpDown();
            m_pRouteToHost_Port.Size = new Size(70,20);
            m_pRouteToHost_Port.Location = new Point(315,175);
            m_pRouteToHost_Port.Visible = false;
            m_pRouteToHost_Port.Minimum = 1;
            m_pRouteToHost_Port.Maximum = 99999;
            m_pRouteToHost_Port.Value = 25;
            //---------------------------------------------------------//

            #endregion

            m_pGroubBox1 = new GroupBox();
            m_pGroubBox1.Size = new Size(387,3);
            m_pGroubBox1.Location = new Point(5,235);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(240,245);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_pOk = new Button();
            m_pOk.Size = new Size(70,20);
            m_pOk.Location = new Point(315,245);
            m_pOk.Text = "Ok";
            m_pOk.Click += new EventHandler(m_pOk_Click);

            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(m_pEnabled);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pDescription);
            this.Controls.Add(mt_Pattern);
            this.Controls.Add(m_pPattern);
            this.Controls.Add(mt_Action);
            this.Controls.Add(m_pAction);
            //--- Action -> Route To Mailbox UI
            this.Controls.Add(mt_RouteToMailbox_Mailbox);
            this.Controls.Add(m_pRouteToMailbox_Mailbox);
            this.Controls.Add(m_pRouteToMailbox_GetMailbox);
            //------------------------------
            //--- Action -> Route To Email UI
            this.Controls.Add(mt_RouteToEmail_Email);
            this.Controls.Add(m_pRouteToEmail_Email);
            //------------------------------
            //--- Action -> Route To Email UI
            this.Controls.Add(mt_RouteToHost_Host);
            this.Controls.Add(m_pRouteToHost_Host);
            this.Controls.Add(m_pRouteToHost_Port);
            //------------------------------
            this.Controls.Add(m_pGroubBox1);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
        }
                                                                
        #endregion


        #region Events Handling

        #region method m_pAction_SelectedIndexChanged

        private void m_pAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            mt_RouteToMailbox_Mailbox.Visible = false;
            m_pRouteToMailbox_Mailbox.Visible = false;
            m_pRouteToMailbox_GetMailbox.Visible = false;
            mt_RouteToEmail_Email.Visible = false;
            m_pRouteToEmail_Email.Visible = false;
            mt_RouteToHost_Host.Visible = false;
            m_pRouteToHost_Host.Visible = false;
            m_pRouteToHost_Port.Visible = false;

            if(m_pAction.SelectedIndex == 0){
                mt_RouteToMailbox_Mailbox.Visible = true;
                m_pRouteToMailbox_Mailbox.Visible = true;
                m_pRouteToMailbox_GetMailbox.Visible = true;
            }
            else if(m_pAction.SelectedIndex == 1){
                mt_RouteToEmail_Email.Visible = true;
                m_pRouteToEmail_Email.Visible = true;
            }
            else if(m_pAction.SelectedIndex == 2){
                mt_RouteToHost_Host.Visible = true;
                m_pRouteToHost_Host.Visible = true;
                m_pRouteToHost_Port.Visible = true;
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
            if(m_pPattern.Text.Length == 0){
                MessageBox.Show(this,"Please specify match pattern !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            RouteAction_enum action     = (RouteAction_enum)((WComboBoxItem)m_pAction.SelectedItem).Tag;
            RouteActionBase  actionData = null;

            #region Route To Mailbox

            if(action == RouteAction_enum.RouteToMailbox){
                //--- Validate values ------------------------------------------------//
                if(m_pRouteToMailbox_Mailbox.Text == ""){
                    MessageBox.Show(this,"Mailbox: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                actionData = new RouteAction_RouteToMailbox(m_pRouteToMailbox_Mailbox.Text);
            }

            #endregion

            #region Route To Email

            else if(action == RouteAction_enum.RouteToEmail){
                //--- Validate values ------------------------------------------------//
                if(m_pRouteToEmail_Email.Text == ""){
                    MessageBox.Show(this,"Email: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                actionData = new RouteAction_RouteToEmail(m_pRouteToEmail_Email.Text);
            }

            #endregion

            #region Route To Host

            else if(action == RouteAction_enum.RouteToHost){
                //--- Validate values ------------------------------------------------//
                if(m_pRouteToHost_Host.Text == ""){
                    MessageBox.Show(this,"Host: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                actionData = new RouteAction_RouteToHost(m_pRouteToHost_Host.Text,(int)m_pRouteToHost_Port.Value);
            }

            #endregion
            
            if(m_pRoute == null){
                m_pRoute = m_pVirtualServer.Routes.Add(
                    m_pDescription.Text,
                    m_pPattern.Text,
                    m_pEnabled.Checked,
                    actionData
                );
            }
            else{
                m_pRoute.Description = m_pDescription.Text;
                m_pRoute.Pattern = m_pPattern.Text;
                m_pRoute.Enabled = m_pEnabled.Checked;
                m_pRoute.Action = actionData;
                m_pRoute.Commit();
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion


        #region method m_pRouteToMailbox_GetMailbox_Click

        private void m_pRouteToMailbox_GetMailbox_Click(object sender, EventArgs e)
        {
            wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pVirtualServer,false,false);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pRouteToMailbox_Mailbox.Text = frm.SelectedUserOrGroup;
            }
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets active route ID.
        /// </summary>
        public string RouteID
        {
            get{ 
                if(m_pRoute != null){
                    return m_pRoute.ID;
                }
                else{
                    return ""; 
                }
            }
        }

        #endregion

    }
}
