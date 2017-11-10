using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

using LumiSoft.Net;
using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// User message rule Add/Edit window.
    /// </summary>
    public class wfrm_User_MessageRule : Form
    {
        //--- Common UI -------------------------
        private TabControl m_pTab         = null;        
        private TabPage    m_pTab_General = null;
        private TabPage    m_pTab_Actions = null;
        private Button     m_pHelp        = null;
        private Button     m_pCancel      = null;
        private Button     m_pOk          = null;
        //--- Tabpage General UI -----------------------------------
        private PictureBox   m_pTab_General_Icon             = null;
        private Label        mt_Tab_General_Info             = null;
        private GroupBox     m_pTab_General_Separator1       = null;
        private CheckBox     m_pTab_General_Enabled          = null;  
        private Label        mt_Tab_General_Description      = null;        
        private TextBox      m_pTab_General_Description      = null;              
        private Label        mt_Tab_General_CheckNextRule    = null;
        private ComboBox     m_pTab_General_CheckNextRule    = null;        
        private Label        mt_Tab_General_MatchExpression  = null;
        private ToolStrip    m_pTab_General_MatchExprToolbar = null;
        private WRichTextBox m_pTab_General_MatchExpression  = null;
        private Button       m_pTab_General_Create           = null;
        //--- Tabpage Actions UI ---------------------------------
        private PictureBox   m_pTab_Actions_Icon           = null;
        private Label        mt_Tab_Actions_Info           = null;
        private GroupBox     m_pTab_Actions_Separator1     = null;
        private ToolStrip    m_pTab_Actions_ActionsToolbar = null;
        private ListView     m_pTab_Actions_Actions        = null;
        
        private User            m_pUser = null;
        private UserMessageRule m_pRule = null;

        /// <summary>
        /// Add constructor.
        /// </summary>
        /// <param name="user">Message rule owner user</param>
        public wfrm_User_MessageRule(User user)
        {
            m_pUser = user;

            InitUI();

            m_pTab.TabPages.Remove(m_pTab_Actions);
            m_pTab_General_MatchExpression.Height -= 25;
            m_pTab_General_Create.Visible = true;
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="user">Message rule owner user</param>
        /// <param name="rule">Rule to update.</param>
        public wfrm_User_MessageRule(User user,UserMessageRule rule)
        {
            m_pUser = user;
            m_pRule = rule;

            InitUI();
        
            m_pTab_General_Enabled.Checked = rule.Enabled;
            if(rule.CheckNextRule == GlobalMessageRule_CheckNextRule_enum.Always){
                m_pTab_General_CheckNextRule.SelectedIndex = 0;
            }
            else if(rule.CheckNextRule == GlobalMessageRule_CheckNextRule_enum.IfMatches){
                m_pTab_General_CheckNextRule.SelectedIndex = 1;
            }
            else if(rule.CheckNextRule == GlobalMessageRule_CheckNextRule_enum.IfNotMatches){
                m_pTab_General_CheckNextRule.SelectedIndex = 2;
            }
            m_pTab_General_Description.Text = rule.Description;
            m_pTab_General_MatchExpression.Text = rule.MatchExpression;
            this.m_pTab_General_MatchExpression_TextChanged(this,new EventArgs());

            LoadActions();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(492,373);
            this.MinimumSize = new Size(500,400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add/Edit User Message Rule";
            this.Icon = ResManager.GetIcon("rule.ico");

            #region Common UI

            //--- Common UI -------------------------------------------------------------------------------//
            m_pTab = new TabControl();
            m_pTab.Size = new Size(493,335);
            m_pTab.Location = new Point(0,5);
            m_pTab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab.TabPages.Add(new TabPage("General"));
            m_pTab_General = m_pTab.TabPages[0]; 
            m_pTab_General.Size = new Size(485,309);
            m_pTab.TabPages.Add(new TabPage("Actions"));
            m_pTab_Actions = m_pTab.TabPages[1];
            m_pTab_Actions.Size = new Size(485,309);

            m_pHelp = new Button();
            m_pHelp.Size = new Size(70,21);
            m_pHelp.Location = new Point(10,350);
            m_pHelp.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            m_pHelp.Click += new EventHandler(m_pHelp_Click);
            m_pHelp.Text = "Help";

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,21);
            m_pCancel.Location = new Point(335,350);
            m_pCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            m_pCancel.Click += new EventHandler(m_pCancel_Click);
            m_pCancel.Text = "Cancel";

            m_pOk = new Button();
            m_pOk.Size = new Size(71,21);
            m_pOk.Location = new Point(410,350);
            m_pOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            m_pOk.Click += new EventHandler(m_pOk_Click);
            m_pOk.Text = "Ok";
                        
            this.Controls.Add(m_pTab);
            this.Controls.Add(m_pHelp);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_pOk);
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region General UI

            //--- General UI ------------------------------------------------------------------------------//
            m_pTab_General_Icon = new PictureBox();
            m_pTab_General_Icon.Size = new Size(32,32);
            m_pTab_General_Icon.Location = new Point(10,10);
            m_pTab_General_Icon.Image = ResManager.GetIcon("rule.ico").ToBitmap();

            mt_Tab_General_Info = new Label();
            mt_Tab_General_Info.Size = new Size(200,32);
            mt_Tab_General_Info.Location = new Point(50,10);
            mt_Tab_General_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_General_Info.Text = "Specify user message rule info.";

            m_pTab_General_Separator1 = new GroupBox();
            m_pTab_General_Separator1.Size = new Size(475,3);
            m_pTab_General_Separator1.Location = new Point(7,50);
            m_pTab_General_Separator1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pTab_General_Enabled = new CheckBox();
            m_pTab_General_Enabled.Size = new Size(100,20);
            m_pTab_General_Enabled.Location = new Point(105,60);
            m_pTab_General_Enabled.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            m_pTab_General_Enabled.Text = "Enabled";
            m_pTab_General_Enabled.Checked = true;

            mt_Tab_General_Description = new Label();
            mt_Tab_General_Description.Size = new Size(100,20);
            mt_Tab_General_Description.Location = new Point(0,85);
            mt_Tab_General_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_Tab_General_Description.Text = "Description:";

            m_pTab_General_Description = new TextBox();
            m_pTab_General_Description.Size = new Size(365,20);
            m_pTab_General_Description.Location = new Point(105,85);
            m_pTab_General_Description.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_Tab_General_CheckNextRule = new Label();
            mt_Tab_General_CheckNextRule.Size = new Size(100,20);
            mt_Tab_General_CheckNextRule.Location = new Point(0,110);
            mt_Tab_General_CheckNextRule.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            mt_Tab_General_CheckNextRule.TextAlign = ContentAlignment.MiddleRight;
            mt_Tab_General_CheckNextRule.Text = "Check Next Rule:";
            
            m_pTab_General_CheckNextRule = new ComboBox();
            m_pTab_General_CheckNextRule.Size = new Size(160,20);
            m_pTab_General_CheckNextRule.Location = new Point(105,110);
            m_pTab_General_CheckNextRule.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            m_pTab_General_CheckNextRule.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pTab_General_CheckNextRule.Items.Add(new WComboBoxItem("Always",GlobalMessageRule_CheckNextRule_enum.Always));
            m_pTab_General_CheckNextRule.Items.Add(new WComboBoxItem("If this rule matches",GlobalMessageRule_CheckNextRule_enum.IfMatches));
            m_pTab_General_CheckNextRule.Items.Add(new WComboBoxItem("If this rule does not match",GlobalMessageRule_CheckNextRule_enum.IfNotMatches));
            m_pTab_General_CheckNextRule.SelectedIndex = 0;
            
            mt_Tab_General_MatchExpression = new Label();
            mt_Tab_General_MatchExpression.Size = new Size(100,20);
            mt_Tab_General_MatchExpression.Location = new Point(10,140);
            mt_Tab_General_MatchExpression.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            mt_Tab_General_MatchExpression.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_General_MatchExpression.Text = "Match Expression:";

            m_pTab_General_MatchExprToolbar = new ToolStrip();
            m_pTab_General_MatchExprToolbar.AutoSize = false;
            m_pTab_General_MatchExprToolbar.Size = new Size(26,25);
            m_pTab_General_MatchExprToolbar.Location = new Point(450,135);
            m_pTab_General_MatchExprToolbar.Dock = DockStyle.None;
            m_pTab_General_MatchExprToolbar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pTab_General_MatchExprToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTab_General_MatchExprToolbar.BackColor = this.BackColor;
            m_pTab_General_MatchExprToolbar.Renderer = new ToolBarRendererEx();
            m_pTab_General_MatchExprToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_General_MatchExprToolbar_ItemClicked);
            // Check syntax button
            ToolStripButton matchexpr_button_CheckSyntax = new ToolStripButton();
            matchexpr_button_CheckSyntax.Image = ResManager.GetIcon("checksyntax.ico").ToBitmap();
            matchexpr_button_CheckSyntax.Tag = "checksyntax";
            m_pTab_General_MatchExprToolbar.Items.Add(matchexpr_button_CheckSyntax);

            m_pTab_General_MatchExpression = new WRichTextBox();
            m_pTab_General_MatchExpression.Size = new Size(465,140);
            m_pTab_General_MatchExpression.Location = new Point(10,160);
            m_pTab_General_MatchExpression.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;            
            // FIX ME: In mono 1.2, if that specified, RichTextbox max height = Text.Height.
            if(Environment.OSVersion.Platform != PlatformID.Unix){
                m_pTab_General_MatchExpression.Font = new System.Drawing.Font("Courier New",10,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
            }
            m_pTab_General_MatchExpression.TextChanged += new EventHandler(m_pTab_General_MatchExpression_TextChanged);

            m_pTab_General_Create = new Button();
            m_pTab_General_Create.Size = new Size(70,20);
            m_pTab_General_Create.Location = new Point(405,285);
            m_pTab_General_Create.Text = "Create";
            m_pTab_General_Create.Visible = false;
            m_pTab_General_Create.Click += new EventHandler(m_pTab_General_Create_Click);
                                                
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Icon);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Info);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Separator1);           
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Enabled);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_Description);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Description);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_CheckNextRule);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_CheckNextRule);
            m_pTab.TabPages[0].Controls.Add(mt_Tab_General_MatchExpression);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_MatchExprToolbar);
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_MatchExpression); 
            m_pTab.TabPages[0].Controls.Add(m_pTab_General_Create); 
            //---------------------------------------------------------------------------------------------//

            #endregion

            #region Actions UI

            //--- Actions UI ------------------------------------------------------------------------------//
            m_pTab_Actions_Icon = new PictureBox();
            m_pTab_Actions_Icon.Size = new Size(32,32);
            m_pTab_Actions_Icon.Location = new Point(10,10);
            m_pTab_Actions_Icon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();

            mt_Tab_Actions_Info = new Label();
            mt_Tab_Actions_Info.Size = new Size(200,32);
            mt_Tab_Actions_Info.Location = new Point(50,10);
            mt_Tab_Actions_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Tab_Actions_Info.Text = "Specify user message rule actions.";

            m_pTab_Actions_Separator1 = new GroupBox();
            m_pTab_Actions_Separator1.Size = new Size(475,3);
            m_pTab_Actions_Separator1.Location = new Point(7,50);
            m_pTab_Actions_Separator1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pTab_Actions_ActionsToolbar = new ToolStrip();
            m_pTab_Actions_ActionsToolbar.AutoSize = false;
            m_pTab_Actions_ActionsToolbar.Size = new Size(72,25);
            m_pTab_Actions_ActionsToolbar.Location = new Point(405,55);
            m_pTab_Actions_ActionsToolbar.Dock = DockStyle.None;
            m_pTab_Actions_ActionsToolbar.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pTab_Actions_ActionsToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pTab_Actions_ActionsToolbar.BackColor = this.BackColor;
            m_pTab_Actions_ActionsToolbar.Renderer = new ToolBarRendererEx();
            m_pTab_Actions_ActionsToolbar.ItemClicked += new ToolStripItemClickedEventHandler(m_pTab_Actions_ActionsToolbar_ItemClicked); 
            // Add button
            ToolStripButton actions_button_Add = new ToolStripButton();
            actions_button_Add.Image = ResManager.GetIcon("add.ico").ToBitmap();
            actions_button_Add.Tag = "add";
            m_pTab_Actions_ActionsToolbar.Items.Add(actions_button_Add);
            // Edit button
            ToolStripButton actions_button_Edit = new ToolStripButton();
            actions_button_Edit.Enabled = false;
            actions_button_Edit.Image = ResManager.GetIcon("edit.ico").ToBitmap();
            actions_button_Edit.Tag = "edit";
            m_pTab_Actions_ActionsToolbar.Items.Add(actions_button_Edit);
            // Delete button
            ToolStripButton actions_button_Delete = new ToolStripButton();
            actions_button_Delete.Enabled = false;
            actions_button_Delete.Image = ResManager.GetIcon("delete.ico").ToBitmap();
            actions_button_Delete.Tag = "delete";
            m_pTab_Actions_ActionsToolbar.Items.Add(actions_button_Delete);

            m_pTab_Actions_Actions = new ListView();
            m_pTab_Actions_Actions.Size = new Size(465,220);
            m_pTab_Actions_Actions.Location = new Point(10,80);
            m_pTab_Actions_Actions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTab_Actions_Actions.View = View.Details;
            m_pTab_Actions_Actions.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            m_pTab_Actions_Actions.BorderStyle = BorderStyle.FixedSingle;
            m_pTab_Actions_Actions.FullRowSelect = true;
            m_pTab_Actions_Actions.HideSelection = false;
            m_pTab_Actions_Actions.DoubleClick += new EventHandler(m_pActions_DoubleClick);
            m_pTab_Actions_Actions.SelectedIndexChanged += new EventHandler(m_pActions_SelectedIndexChanged);
            m_pTab_Actions_Actions.Columns.Add("Action",160,HorizontalAlignment.Left);
            m_pTab_Actions_Actions.Columns.Add("Description",280,HorizontalAlignment.Left);

            m_pTab.TabPages[1].Controls.Add(m_pTab_Actions_Icon);
            m_pTab.TabPages[1].Controls.Add(mt_Tab_Actions_Info);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Actions_Separator1);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Actions_ActionsToolbar);
            m_pTab.TabPages[1].Controls.Add(m_pTab_Actions_Actions);
            //---------------------------------------------------------------------------------------------//

            #endregion
        }
                                                
        #endregion


        #region Events handling

        #region method m_pTab_General_MatchExprToolbar_ItemClicked

        private void m_pTab_General_MatchExprToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "checksyntax"){
                CheckSyntax(true);
            }
        }

        #endregion

        #region method m_pTab_General_MatchExpression_TextChanged

        private void m_pTab_General_MatchExpression_TextChanged(object sender,EventArgs e)
        {
            m_pTab_General_MatchExpression.SuspendPaint = true;

            string text = m_pTab_General_MatchExpression.Text;
            int selectionStart = m_pTab_General_MatchExpression.SelectionStart;
            int startPos = 0;

            StringReader r = new StringReader(text);
            while(r.Available > 0){
                r.ReadToFirstChar();
                startPos = r.Position;
                                                
                string word = r.ReadWord(false);
                if(word == null){
                    break;
                }
                // We must have ()[]{}<>
                if(word == ""){
                    word = r.ReadSpecifiedLength(1);
                }
                                
                if(word.StartsWith("\"") && word.EndsWith("\"")){
                    m_pTab_General_MatchExpression.SelectionStart = startPos;
                    m_pTab_General_MatchExpression.SelectionLength = word.Length;
                    m_pTab_General_MatchExpression.SelectionColor = Color.Brown;
                    continue;
                }

                bool isKeyWord = false;
                string[] keyWords = new string[]{
                    "and",
                    "or",
                    "not"                
                };                
                foreach(string keyWord in keyWords){
                    if(word.ToLower() == keyWord.ToLower()){
                        isKeyWord = true;
                        break;
                    }
                }

                bool isMatcher = false;
                string[] matchers = new string[]{
                    "smtp.mail_from",
                    "smtp.rcpt_to",
                    "smtp.ehlo",
                    "smtp.authenticated",
                    "smtp.user",
                    "smtp.remote_ip",
                    "message.size",
                    "message.header",
                    "message.all_headers",
                    "message.body_text",
                    "message.body_html",
                    "message.content_md5",
                    "sys.date_time",
                    "sys.date",
                    "sys.time",
                    "sys.day_of_week",
                    "sys.day_of_month",
                    "sys.day_of_year"
                };
                foreach(string keyWord in matchers){
                    if(word.ToLower() == keyWord.ToLower()){
                        isMatcher = true;
                        break;
                    }
                }

                if(isKeyWord){
                    m_pTab_General_MatchExpression.SelectionStart = startPos;
                    m_pTab_General_MatchExpression.SelectionLength = word.Length;
                    m_pTab_General_MatchExpression.SelectionColor = Color.Blue;
                }
                else if(isMatcher){
                    m_pTab_General_MatchExpression.SelectionStart = startPos;
                    m_pTab_General_MatchExpression.SelectionLength = word.Length;
                    m_pTab_General_MatchExpression.SelectionColor = Color.DarkMagenta;
                }
                else{
                    m_pTab_General_MatchExpression.SelectionStart = startPos;
                    m_pTab_General_MatchExpression.SelectionLength = word.Length;
                    m_pTab_General_MatchExpression.SelectionColor = Color.Black;
                }
            }

            m_pTab_General_MatchExpression.SelectionStart = selectionStart;
            m_pTab_General_MatchExpression.SelectionLength = 0;

            m_pTab_General_MatchExpression.SuspendPaint = false;
        }

        #endregion

        #region method m_pTab_General_Create_Click

        private void m_pTab_General_Create_Click(object sender,EventArgs e)
        {
            if(!CheckSyntax(false)){
                return;
            }

            m_pRule = m_pUser.MessageRules.Add(
                m_pTab_General_Enabled.Checked,
                m_pTab_General_Description.Text,
                m_pTab_General_MatchExpression.Text,
                (GlobalMessageRule_CheckNextRule_enum)((WComboBoxItem)m_pTab_General_CheckNextRule.SelectedItem).Tag
            );

            m_pTab_General_Create.Visible = false;
            m_pTab_General_MatchExpression.Height += 25;
            m_pTab.TabPages.Add(m_pTab_Actions);
        }

        #endregion


        #region method m_pTab_Actions_ActionsToolbar_ItemClicked

        private void m_pTab_Actions_ActionsToolbar_ItemClicked(object sender,ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem.Tag == null){
                return;
            }

            if(e.ClickedItem.Tag.ToString() == "add"){                
                wfrm_UserMessageRule_Action frm = new wfrm_UserMessageRule_Action(m_pRule);
                frm.Text = "Add/Edit User Message Rule Action";
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadActions();
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "edit"){
                if(m_pTab_Actions_Actions.SelectedItems.Count > 0){
                    UserMessageRuleActionBase action = (UserMessageRuleActionBase)m_pTab_Actions_Actions.SelectedItems[0].Tag;
                    wfrm_UserMessageRule_Action frm = new wfrm_UserMessageRule_Action(m_pRule,action);
                    frm.Text = "Add/Edit User Message Rule Action";
                    if(frm.ShowDialog(this) == DialogResult.OK){
                        LoadActions();
                    }
                }
            }
            else if(e.ClickedItem.Tag.ToString() == "delete"){
                if(m_pTab_Actions_Actions.SelectedItems.Count > 0){
                    UserMessageRuleActionBase action = (UserMessageRuleActionBase)m_pTab_Actions_Actions.SelectedItems[0].Tag;
                    if(MessageBox.Show(this,"Are you sure you want to delete Action '" + action.Description + "' !","Confirm Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                        action.Owner.Remove(action);
                        LoadActions();
                    }
                }
            }
        }

        #endregion

        #region method m_pActions_DoubleClick

        private void m_pActions_DoubleClick(object sender, EventArgs e)
        {
            if(m_pTab_Actions_Actions.SelectedItems.Count > 0){
                UserMessageRuleActionBase action = (UserMessageRuleActionBase)m_pTab_Actions_Actions.SelectedItems[0].Tag;
                wfrm_UserMessageRule_Action frm = new wfrm_UserMessageRule_Action(m_pRule,action);
                frm.Text = "Add/Edit User Message Rule Action";
                if(frm.ShowDialog(this) == DialogResult.OK){
                    LoadActions();
                }
            }
        }

        #endregion

        #region method m_pActions_SelectedIndexChanged

        private void m_pActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pTab_Actions_Actions.Items.Count > 0 && m_pTab_Actions_Actions.SelectedItems.Count > 0){
                m_pTab_Actions_ActionsToolbar.Items[1].Enabled = true;
                m_pTab_Actions_ActionsToolbar.Items[2].Enabled = true;
            }
            else{
                m_pTab_Actions_ActionsToolbar.Items[1].Enabled = false;
                m_pTab_Actions_ActionsToolbar.Items[2].Enabled = false;
            }
        }

        #endregion


        #region method m_pHelp_Click

        private void m_pHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo pInf = new System.Diagnostics.ProcessStartInfo("explorer",Application.StartupPath + "\\Help\\Grules.htm");
			System.Diagnostics.Process.Start(pInf);
        }

        #endregion

        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        #region method m_pOk_Click

        private void m_pOk_Click(object sender, EventArgs e)
        {
            if(!CheckSyntax(false)){
                return;
            }

            if(m_pTab_General_Description.Text == ""){
                MessageBox.Show(this,"Please fill description !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }            
                 
            // Add new rule
            if(m_pRule == null){                
                m_pRule = m_pUser.MessageRules.Add(
                    m_pTab_General_Enabled.Checked,
                    m_pTab_General_Description.Text,
                    m_pTab_General_MatchExpression.Text,
                    (GlobalMessageRule_CheckNextRule_enum)((WComboBoxItem)m_pTab_General_CheckNextRule.SelectedItem).Tag
                );
            }
            // Edit rule
            else{
                m_pRule.Enabled         = m_pTab_General_Enabled.Checked;
                m_pRule.Description     = m_pTab_General_Description.Text;
                m_pRule.MatchExpression = m_pTab_General_MatchExpression.Text;
                m_pRule.CheckNextRule   = (GlobalMessageRule_CheckNextRule_enum)((WComboBoxItem)m_pTab_General_CheckNextRule.SelectedItem).Tag;
                m_pRule.Commit();
            }
            
            this.DialogResult = DialogResult.OK;
        }

        #endregion

        #endregion


        #region method LoadActions

        /// <summary>
        /// Load actions to UI.
        /// </summary>
        private void LoadActions()
        {
            m_pTab_Actions_Actions.Items.Clear();

            foreach(UserMessageRuleActionBase action in  m_pRule.Actions){
                ListViewItem it = new ListViewItem();
                //--- Get action human readable text -------//
                if(action.ActionType == UserMessageRuleAction_enum.AutoResponse){
                    it.Text = "Auto Response";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.DeleteMessage){
                    it.Text = "Delete Message";
                }                
                else if(action.ActionType == UserMessageRuleAction_enum.ForwardToEmail){
                    it.Text = "Forward To Email";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.ForwardToHost){
                    it.Text = "Forward To Host";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.StoreToDiskFolder){
                    it.Text = "Store To Disk Folder";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.ExecuteProgram){
                    it.Text = "Execute Program";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.MoveToIMAPFolder){
                    it.Text = "Move To IMAP Folder";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.AddHeaderField){
                    it.Text = "Add Header Field";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.RemoveHeaderField){
                    it.Text = "Remove Header Field";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.StoreToFTPFolder){
                    it.Text = "Store To FTP Folder";
                }
                else if(action.ActionType == UserMessageRuleAction_enum.PostToNNTPNewsGroup){
                    it.Text = "Post To NNTP Newsgroup";
                }                    
                else if(action.ActionType == UserMessageRuleAction_enum.PostToHTTP){
                    it.Text = "Post To HTTP";
                }
                else{
                    it.Text = action.ActionType.ToString();
                }
                //--------------------------------------------//
                it.Tag = action;
                it.SubItems.Add(action.Description);
                m_pTab_Actions_Actions.Items.Add(it);
            }

            m_pActions_SelectedIndexChanged(this,new EventArgs());
        }

        #endregion

        #region method CheckSyntax

        /// <summary>
        /// Checks if match expression is valid. Show Ok(if showOkMessageBox = true) or Error message box accordingly.Returns true if syntax ok.
        /// </summary>
        /// <param name="showOkMessageBox">Specifies if Ok message box is shown.</param>
        /// <returns>Returns true if syntax ok.</returns>
        private bool CheckSyntax(bool showOkMessageBox)
        {
            try{
                LumiSoft.MailServer.GlobalMessageRuleProcessor.CheckMatchExpressionSyntax(m_pTab_General_MatchExpression.Text);

                if(showOkMessageBox){
                    MessageBox.Show(this,"Syntax Ok !","Syntax Check:",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                return true;
            }
            catch(Exception x){
                MessageBox.Show(this,"Syntax Error !\r\n\r\n" + x.Message,"Syntax Check:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets active rule ID.
        /// </summary>
        public string RuleID
        {
            get{
                if(m_pRule != null){
                    return m_pRule.ID;
                }
                else{
                    return "";
                }
            }
        }

        #endregion

    }
}
