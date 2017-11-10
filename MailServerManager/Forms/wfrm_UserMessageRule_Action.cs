using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.IO;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// User message rule action Add/Edit window.
    /// </summary>
    public class wfrm_UserMessageRule_Action : Form
    {        
        //--- Common UI ------------------------
        private PictureBox m_pIcon        = null;
        private Label      mt_Info        = null;
        private GroupBox   m_pSeparator1  = null;
        private Label      mt_Description = null;
        private TextBox    m_pDescription = null;
        private Label      mt_Action      = null;
        private ComboBox   m_pAction      = null;
        private GroupBox   m_pSeparator2  = null;
        private GroupBox   m_pSeparator3  = null;
        private Button     m_pHelp        = null;
        private Button     m_pCancel      = null;
        private Button     m_Ok           = null;
        //--- Auto Response UI  
        private Label   mt_AutoResponse_From        = null;      
        private TextBox m_pAutoResponse_From        = null;
        private Label   mt_AutoResponse_FullMEssage = null;
        private TextBox m_pAutoResponse_FullMEssage = null;
        private Button  m_pAutoResponse_Compose     = null;
        private Button  mt_AutoResponse_Load        = null;
        //--- Execute Program UI
        private Label    mt_ExecuteProgram_ProgramToExecute       = null;
        private TextBox  m_pExecuteProgram_ProgramToExecute       = null;
        private Button   m_pExecuteProgram_BrowseProgramToExecute = null;        
        private Label    mt_ExecuteProgram_ProgramArguments       = null;
        private TextBox  m_pExecuteProgram_ProgramArguments       = null;
        //--- Forward To Email UI
        private Label   mt_ForwardToEmail_Email = null;
        private TextBox m_pForwardToEmail_Email = null;
        //--- Forward To Host UI
        private Label         mt_ForwardToHost_Host     = null;
        private TextBox       m_pForwardToHost_Host     = null;
        private Label         mt_ForwardToHost_HostPort = null;
        private NumericUpDown m_pForwardToHost_HostPort = null;
        //--- Store To Disk Folder
        private Label   mt_StoreToDiskFolder_Folder       = null;
        private TextBox m_pStoreToDiskFolder_Folder       = null;
        private Button  m_pStoreToDiskFolder_BrowseFolder = null;       
        //--- Move To IMAP Folder
        private Label   mt_MoveToIMAPFolder_Folder = null;
        private TextBox m_pMoveToIMAPFolder_Folder = null;
        //--- Add Header Field
        private Label   mt_AddHeaderField_FieldName  = null;
        private TextBox m_pAddHeaderField_FieldName  = null;
        private Label   mt_AddHeaderField_FieldValue = null;
        private TextBox m_pAddHeaderField_FieldValue = null;
        //--- Remove Header Field
        private Label   mt_RemoveHeaderField_FieldName = null;
        private TextBox m_pRemoveHeaderField_FieldName = null;
        //--- Send Error To Client
        private Label   mt_SendErrorToClient_ErrorText = null;
        private TextBox m_pSendErrorToClient_ErrorText = null;
        //--- Store To FTP Folder
        private Label         mt_StoreToFTPFolder_Server   = null;
        private TextBox       m_pStoreToFTPFolder_Server   = null;
        private NumericUpDown m_pStoreToFTPFolder_Port     = null;
        private Label         mt_StoreToFTPFolder_User     = null;
        private TextBox       m_pStoreToFTPFolder_User     = null;
        private Label         mt_StoreToFTPFolder_Password = null;
        private TextBox       m_pStoreToFTPFolder_Password = null;
        private Label         mt_StoreToFTPFolder_Folder   = null;
        private TextBox       m_pStoreToFTPFolder_Folder   = null;
        //--- Post To NNTP Newsgroup
        private Label         mt_PostToNNTPNewsgroup_Server    = null;
        private TextBox       m_pPostToNNTPNewsgroup_Server    = null;
        private NumericUpDown m_pPostToNNTPNewsgroup_Port      = null;
        private Label         mt_PostToNNTPNewsgroup_Newsgroup = null;
        private TextBox       m_pPostToNNTPNewsgroup_Newsgroup = null;
        //--- Post TO HTTP
        private Label   mt_PostToHTTP_URL = null;
        private TextBox m_pPostToHTTP_URL = null;

        private UserMessageRule           m_pRule       = null;
        private UserMessageRuleActionBase m_pActionData = null;
        
        /// <summary>
        /// Add constructor.
        /// </summary>
        /// <param name="rule">Owner rule.</param>
        public wfrm_UserMessageRule_Action(UserMessageRule rule)
        {
            m_pRule = rule;

            InitUI();

            m_pAction.SelectedIndex = 0;
        }

        /// <summary>
        /// Edit constructor.
        /// </summary>
        /// <param name="rule">Owner rule.</param>
        /// <param name="action">User messgae rule action action.</param>
        public wfrm_UserMessageRule_Action(UserMessageRule rule,UserMessageRuleActionBase action)
        {
            m_pRule       = rule;
            m_pActionData = action;

            InitUI();

            m_pDescription.Text = action.Description;
            m_pAction.Enabled = false;

            //--- Pase action data -------------------------------------------------//

            #region AutoResponse

            if(action.ActionType == UserMessageRuleAction_enum.AutoResponse){
                UserMessageRuleAction_AutoResponse a = (UserMessageRuleAction_AutoResponse)action;
                m_pAutoResponse_From.Text        = a.From;
                m_pAutoResponse_FullMEssage.Text = System.Text.Encoding.Default.GetString(a.Message);
                
                m_pAction.SelectedIndex = 0;
            }

            #endregion

            #region DeleteMessage

            else if(action.ActionType == UserMessageRuleAction_enum.DeleteMessage){
                UserMessageRuleAction_DeleteMessage a = (UserMessageRuleAction_DeleteMessage)action;

                m_pAction.SelectedIndex = 1;
            }

            #endregion

            #region ExecuteProgram

            else if(action.ActionType == UserMessageRuleAction_enum.ExecuteProgram){
                UserMessageRuleAction_ExecuteProgram a = (UserMessageRuleAction_ExecuteProgram)action;
                m_pExecuteProgram_ProgramToExecute.Text = a.Program;
                m_pExecuteProgram_ProgramArguments.Text = a.ProgramArguments;

                m_pAction.SelectedIndex = 2;
            }

            #endregion

            #region ForwardToEmail

            else if(action.ActionType == UserMessageRuleAction_enum.ForwardToEmail){
                UserMessageRuleAction_ForwardToEmail a = (UserMessageRuleAction_ForwardToEmail)action;
                m_pForwardToEmail_Email.Text = a.EmailAddress;

                m_pAction.SelectedIndex = 3;
            }

            #endregion

            #region ForwardToHost

            else if(action.ActionType == UserMessageRuleAction_enum.ForwardToHost){
                UserMessageRuleAction_ForwardToHost a = (UserMessageRuleAction_ForwardToHost)action;
                m_pForwardToHost_Host.Text      = a.Host;
                m_pForwardToHost_HostPort.Value = a.Port;

                m_pAction.SelectedIndex = 4;
            }

            #endregion

            #region StoreToDiskFolder

            else if(action.ActionType == UserMessageRuleAction_enum.StoreToDiskFolder){
                UserMessageRuleAction_StoreToDiskFolder a = (UserMessageRuleAction_StoreToDiskFolder)action;
                m_pStoreToDiskFolder_Folder.Text = a.Folder;

                m_pAction.SelectedIndex = 5;
            }

            #endregion

            #region StoreToIMAPFolder

            else if(action.ActionType == UserMessageRuleAction_enum.MoveToIMAPFolder){
                UserMessageRuleAction_MoveToImapFolder a = (UserMessageRuleAction_MoveToImapFolder)action;
                m_pMoveToIMAPFolder_Folder.Text = a.Folder;

                m_pAction.SelectedIndex = 6;
            }

            #endregion

            #region AddHeaderField

            else if(action.ActionType == UserMessageRuleAction_enum.AddHeaderField){
                UserMessageRuleAction_AddHeaderField a = (UserMessageRuleAction_AddHeaderField)action;
                m_pAddHeaderField_FieldName.Text  = a.HeaderFieldName;
                m_pAddHeaderField_FieldValue.Text = a.HeaderFieldValue;

                m_pAction.SelectedIndex = 7;
            }

            #endregion

            #region RemoveHeaderField

            else if(action.ActionType == UserMessageRuleAction_enum.RemoveHeaderField){
                UserMessageRuleAction_RemoveHeaderField a = (UserMessageRuleAction_RemoveHeaderField)action;
                m_pRemoveHeaderField_FieldName.Text = a.HeaderFieldName;

                m_pAction.SelectedIndex = 8;
            }

            #endregion

            #region StoreToFTPFolder

            else if(action.ActionType == UserMessageRuleAction_enum.StoreToFTPFolder){
                UserMessageRuleAction_StoreToFtp a = (UserMessageRuleAction_StoreToFtp)action;
                m_pStoreToFTPFolder_Server.Text   = a.Server;
                m_pStoreToFTPFolder_Port.Value    = a.Port;
                m_pStoreToFTPFolder_User.Text     = a.UserName;
                m_pStoreToFTPFolder_Password.Text = a.Password;
                m_pStoreToFTPFolder_Folder.Text   = a.Folder;

                m_pAction.SelectedIndex = 10;
            }

            #endregion

            #region PostToNNTPNewsGroup

            else if(action.ActionType == UserMessageRuleAction_enum.PostToNNTPNewsGroup){
                UserMessageRuleAction_PostToNntpNewsgroup a = (UserMessageRuleAction_PostToNntpNewsgroup)action;
                m_pPostToNNTPNewsgroup_Server.Text = a.Server;
                m_pPostToNNTPNewsgroup_Port.Value  = a.Port;
                // table.Add("User","");
                // table.Add("Password","");
                m_pPostToNNTPNewsgroup_Newsgroup.Text = a.Newsgroup;

                m_pAction.SelectedIndex = 11;
            }

            #endregion

            #region PostToHTTP

            else if(action.ActionType == UserMessageRuleAction_enum.PostToHTTP){
                UserMessageRuleAction_PostToHttp a = (UserMessageRuleAction_PostToHttp)action;
                m_pPostToHTTP_URL.Text = a.Url;
                // table.GetValue("FileName");

                m_pAction.SelectedIndex = 12;
            }

            #endregion

            //---------------------------------------------------------------------//
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {            
            this.ClientSize = new Size(492,373);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(500,400);
            this.MinimizeBox = false;
            this.Text = "User Message Rule Add/Edit Action";
            this.Icon = ResManager.GetIcon("ruleaction.ico");

            #region Common UI

            //--- Common UI -------------------------------------------------------------------//
            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(32,32);
            m_pIcon.Location = new Point(10,10);
            m_pIcon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();

            mt_Info = new Label();
            mt_Info.Size = new Size(200,32);
            mt_Info.Location = new Point(50,10);
            mt_Info.TextAlign = ContentAlignment.MiddleLeft;
            mt_Info.Text = "Specify action information.";

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(483,3);
            m_pSeparator1.Location = new Point(7,50);
            m_pSeparator1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_Description = new Label();
            mt_Description.Size = new Size(100,20);
            mt_Description.Location = new Point(0,60);
            mt_Description.TextAlign = ContentAlignment.MiddleRight;
            mt_Description.Text = "Description:";

            m_pDescription = new TextBox();
            m_pDescription.Size = new Size(380,20);
            m_pDescription.Location = new Point(105,60);

            mt_Action = new Label();
            mt_Action.Size = new Size(100,20);
            mt_Action.Location = new Point(0,85);
            mt_Action.TextAlign = ContentAlignment.MiddleRight;
            mt_Action.Text = "Action:";

            m_pAction = new ComboBox();
            m_pAction.Size = new Size(160,21);
            m_pAction.Location = new Point(105,85);
            m_pAction.DropDownStyle = ComboBoxStyle.DropDownList;
            try{
                // FIX ME: Mono throws exception
                // m_pAction.DropDownHeight = 200;
                m_pAction.GetType().GetProperty("DropDownHeight").GetSetMethod(true).Invoke(m_pAction,new object[]{200});
            }
            catch{
            }
            m_pAction.SelectedIndexChanged += new EventHandler(m_pAction_SelectedIndexChanged);
            m_pAction.Items.Add(new WComboBoxItem("Auto Response",GlobalMessageRuleAction_enum.AutoResponse));
            m_pAction.Items.Add(new WComboBoxItem("Delete Message",GlobalMessageRuleAction_enum.DeleteMessage));
            m_pAction.Items.Add(new WComboBoxItem("Execute Program",GlobalMessageRuleAction_enum.ExecuteProgram));
            m_pAction.Items.Add(new WComboBoxItem("Forward To Email",GlobalMessageRuleAction_enum.ForwardToEmail));
            m_pAction.Items.Add(new WComboBoxItem("Forward To Host",GlobalMessageRuleAction_enum.ForwardToHost));
            m_pAction.Items.Add(new WComboBoxItem("Store To Disk Folder",GlobalMessageRuleAction_enum.StoreToDiskFolder));
            m_pAction.Items.Add(new WComboBoxItem("Move To IMAP Folder",GlobalMessageRuleAction_enum.MoveToIMAPFolder));
            m_pAction.Items.Add(new WComboBoxItem("Add Header Field",GlobalMessageRuleAction_enum.AddHeaderField));
            m_pAction.Items.Add(new WComboBoxItem("Remove Header Field",GlobalMessageRuleAction_enum.RemoveHeaderField));
            //m_pAction.Items.Add(new WComboBoxItem("Send Error To Client",GlobalMessageRuleAction_enum.SendErrorToClient));
            m_pAction.Items.Add(new WComboBoxItem("Store To FTP Folder",GlobalMessageRuleAction_enum.StoreToFTPFolder));
            m_pAction.Items.Add(new WComboBoxItem("Post To NNTP Newsgroup",GlobalMessageRuleAction_enum.PostToNNTPNewsGroup));
            m_pAction.Items.Add(new WComboBoxItem("Post To HTTP",GlobalMessageRuleAction_enum.PostToHTTP));

            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(483,3);
            m_pSeparator2.Location = new Point(7,115);
            m_pSeparator2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pSeparator3 = new GroupBox();
            m_pSeparator3.Size = new Size(483,3);
            m_pSeparator3.Location = new Point(7,335);
            m_pSeparator3.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            m_pHelp = new Button();
            m_pHelp.Size = new Size(70,20);
            m_pHelp.Location = new Point(10,350);
            m_pHelp.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            m_pHelp.Text = "Help";
            m_pHelp.Click += new EventHandler(m_pHelp_Click);

            m_pCancel = new Button();            
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(340,350);
            m_pCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);

            m_Ok = new Button();
            m_Ok.Size = new Size(70,20);
            m_Ok.Location = new Point(415,350);
            m_Ok.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            m_Ok.Text = "Ok";
            m_Ok.Click += new EventHandler(m_Ok_Click);
            //--------------------------------------------------------------------------------//

            #endregion


            #region Auto Response

            //--- Auto Response UI -----------------------------------------------------------//
            mt_AutoResponse_From = new Label();
            mt_AutoResponse_From.Size = new Size(100,20);
            mt_AutoResponse_From.Location = new Point(0,125);
            mt_AutoResponse_From.TextAlign = ContentAlignment.MiddleRight;
            mt_AutoResponse_From.Text = "From:";
            mt_AutoResponse_From.Visible = false;
       
            m_pAutoResponse_From = new TextBox();
            m_pAutoResponse_From.Size = new Size(380,20);
            m_pAutoResponse_From.Location = new Point(105,125);
            m_pAutoResponse_From.Visible = false;

            mt_AutoResponse_FullMEssage = new Label();
            mt_AutoResponse_FullMEssage.Size = new Size(100,13);
            mt_AutoResponse_FullMEssage.Location = new Point(10,155);
            mt_AutoResponse_FullMEssage.TextAlign = ContentAlignment.MiddleLeft;
            mt_AutoResponse_FullMEssage.Text = "Full Message:";
            mt_AutoResponse_FullMEssage.Visible = false;

            m_pAutoResponse_FullMEssage = new TextBox();
            m_pAutoResponse_FullMEssage.Size = new Size(395,145);
            m_pAutoResponse_FullMEssage.Location = new Point(12,175);
            m_pAutoResponse_FullMEssage.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            m_pAutoResponse_FullMEssage.AcceptsReturn = true;
            m_pAutoResponse_FullMEssage.AcceptsTab = true;
            m_pAutoResponse_FullMEssage.Multiline = true;
            m_pAutoResponse_FullMEssage.ScrollBars = ScrollBars.Both;
            m_pAutoResponse_FullMEssage.Visible = false;

            m_pAutoResponse_Compose = new Button();
            m_pAutoResponse_Compose.Size = new Size(70,21);
            m_pAutoResponse_Compose.Location = new Point(415,175);
            m_pAutoResponse_Compose.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            m_pAutoResponse_Compose.Text = "Compose";
            m_pAutoResponse_Compose.Visible = false;
            m_pAutoResponse_Compose.Click += new EventHandler(m_pAutoResponse_Compose_Click);

            mt_AutoResponse_Load = new Button();
            mt_AutoResponse_Load.Size = new Size(70,40);
            mt_AutoResponse_Load.Location = new Point(415,200);
            mt_AutoResponse_Load.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            mt_AutoResponse_Load.Text = "Load Form File";
            mt_AutoResponse_Load.Visible = false;
            mt_AutoResponse_Load.Click += new EventHandler(mt_AutoResponse_Load_Click);
            //-------------------------------------------------------------------------------//

            #endregion

            #region Execute Program

            //--- Execute Program UI --------------------------------------------------------//
            mt_ExecuteProgram_ProgramToExecute = new Label();
            mt_ExecuteProgram_ProgramToExecute.Size = new Size(100,20);
            mt_ExecuteProgram_ProgramToExecute.Location = new Point(0,125);
            mt_ExecuteProgram_ProgramToExecute.Visible = false;
            mt_ExecuteProgram_ProgramToExecute.TextAlign = ContentAlignment.MiddleRight;
            mt_ExecuteProgram_ProgramToExecute.Text = "Program:";

            m_pExecuteProgram_ProgramToExecute = new TextBox();
            m_pExecuteProgram_ProgramToExecute.Size = new Size(350,20);
            m_pExecuteProgram_ProgramToExecute.Location = new Point(105,125);
            m_pExecuteProgram_ProgramToExecute.Visible = false;

            m_pExecuteProgram_BrowseProgramToExecute = new Button();
            m_pExecuteProgram_BrowseProgramToExecute.Size = new Size(25,20);
            m_pExecuteProgram_BrowseProgramToExecute.Location = new Point(460,125);
            m_pExecuteProgram_BrowseProgramToExecute.Visible = false;
            m_pExecuteProgram_BrowseProgramToExecute.Click += new EventHandler(m_pExecuteProgram_BrowseProgramToExecute_Click);
            m_pExecuteProgram_BrowseProgramToExecute.Text = "...";

            mt_ExecuteProgram_ProgramArguments = new Label();
            mt_ExecuteProgram_ProgramArguments.Size = new Size(100,20);
            mt_ExecuteProgram_ProgramArguments.Location = new Point(0,150);
            mt_ExecuteProgram_ProgramArguments.Visible = false;
            mt_ExecuteProgram_ProgramArguments.TextAlign = ContentAlignment.MiddleRight;
            mt_ExecuteProgram_ProgramArguments.Text = "Arguments:";

            m_pExecuteProgram_ProgramArguments = new TextBox();
            m_pExecuteProgram_ProgramArguments.Size = new Size(380,20);
            m_pExecuteProgram_ProgramArguments.Location = new Point(105,150);
            m_pExecuteProgram_ProgramArguments.Visible = false;
            //------------------------------------------------------------------------------//

            #endregion

            #region Forward To Email

            //--- Forward To Email UI ------------------------------------------------------//
            mt_ForwardToEmail_Email = new Label();
            mt_ForwardToEmail_Email.Size = new Size(100,20);
            mt_ForwardToEmail_Email.Location = new Point(0,125);
            mt_ForwardToEmail_Email.Visible = false;
            mt_ForwardToEmail_Email.TextAlign = ContentAlignment.MiddleRight;
            mt_ForwardToEmail_Email.Text = "Email:";

            m_pForwardToEmail_Email = new TextBox();
            m_pForwardToEmail_Email.Size = new Size(380,20);
            m_pForwardToEmail_Email.Location = new Point(105,125);
            m_pForwardToEmail_Email.Visible = false;
            //------------------------------------------------------------------------------//

            #endregion

            #region Forward To Host

            //--- Forward To Host UI -------------------------------------------------------//
            mt_ForwardToHost_Host = new Label();
            mt_ForwardToHost_Host.Size = new Size(100,20);
            mt_ForwardToHost_Host.Location = new Point(0,125);
            mt_ForwardToHost_Host.Visible = false;
            mt_ForwardToHost_Host.TextAlign = ContentAlignment.MiddleRight;
            mt_ForwardToHost_Host.Text = "Host:";

            m_pForwardToHost_Host = new TextBox();
            m_pForwardToHost_Host.Size = new Size(380,20);
            m_pForwardToHost_Host.Location = new Point(105,125);
            m_pForwardToHost_Host.Visible = false;

            mt_ForwardToHost_HostPort = new Label();
            mt_ForwardToHost_HostPort.Size = new Size(100,20);
            mt_ForwardToHost_HostPort.Location = new Point(0,150);
            mt_ForwardToHost_HostPort.Visible = false;
            mt_ForwardToHost_HostPort.TextAlign = ContentAlignment.MiddleRight;
            mt_ForwardToHost_HostPort.Text = "Port:";

            m_pForwardToHost_HostPort = new NumericUpDown();
            m_pForwardToHost_HostPort.Size = new Size(60,20);
            m_pForwardToHost_HostPort.Location = new Point(105,150);
            m_pForwardToHost_HostPort.Maximum = 99999;
            m_pForwardToHost_HostPort.Minimum = 1;
            m_pForwardToHost_HostPort.Value = 25;
            m_pForwardToHost_HostPort.Visible = false;
            //-----------------------------------------------------------------------------//

            #endregion

            #region Store To Disk Folder

            //--- Store To Disk Folder ----------------------------------------------------//
            mt_StoreToDiskFolder_Folder = new Label();
            mt_StoreToDiskFolder_Folder.Size = new Size(100,20);
            mt_StoreToDiskFolder_Folder.Location = new Point(0,125);
            mt_StoreToDiskFolder_Folder.Visible = false;
            mt_StoreToDiskFolder_Folder.TextAlign = ContentAlignment.MiddleRight;
            mt_StoreToDiskFolder_Folder.Text = "Disk Folder:";

            m_pStoreToDiskFolder_Folder = new TextBox();
            m_pStoreToDiskFolder_Folder.Size = new Size(350,120);
            m_pStoreToDiskFolder_Folder.Location = new Point(105,125);
            m_pStoreToDiskFolder_Folder.Visible = false;

            m_pStoreToDiskFolder_BrowseFolder = new Button();
            m_pStoreToDiskFolder_BrowseFolder.Size = new Size(25,20);
            m_pStoreToDiskFolder_BrowseFolder.Location = new Point(460,125);
            m_pStoreToDiskFolder_BrowseFolder.Visible = false;
            m_pStoreToDiskFolder_BrowseFolder.Click += new EventHandler(m_pStoreToDiskFolder_BrowseFolder_Click);
            m_pStoreToDiskFolder_BrowseFolder.Text = "...";
            //-----------------------------------------------------------------------------//

            #endregion

            #region Store To IMAP Folder

            //--- Store To IMAP Folder ----------------------------------------------------//
            mt_MoveToIMAPFolder_Folder = new Label();
            mt_MoveToIMAPFolder_Folder.Size = new Size(100,20);
            mt_MoveToIMAPFolder_Folder.Location = new Point(0,125);
            mt_MoveToIMAPFolder_Folder.Visible = false;
            mt_MoveToIMAPFolder_Folder.TextAlign = ContentAlignment.MiddleRight;
            mt_MoveToIMAPFolder_Folder.Text = "IMAP Folder:";

            m_pMoveToIMAPFolder_Folder = new TextBox();
            m_pMoveToIMAPFolder_Folder.Size = new Size(380,20);
            m_pMoveToIMAPFolder_Folder.Location = new Point(105,125);
            m_pMoveToIMAPFolder_Folder.Visible = false;
            //-----------------------------------------------------------------------------//

            #endregion

            #region Add Header Field

            //--- Add Header Field --------------------------------------------------------//
            mt_AddHeaderField_FieldName = new Label();
            mt_AddHeaderField_FieldName.Size = new Size(100,20);
            mt_AddHeaderField_FieldName.Location = new Point(0,125);
            mt_AddHeaderField_FieldName.Visible = false;
            mt_AddHeaderField_FieldName.TextAlign = ContentAlignment.MiddleRight;
            mt_AddHeaderField_FieldName.Text = "Name:";

            m_pAddHeaderField_FieldName = new TextBox();
            m_pAddHeaderField_FieldName.Size = new Size(380,120);
            m_pAddHeaderField_FieldName.Location = new Point(105,125);
            m_pAddHeaderField_FieldName.Visible = false;

            mt_AddHeaderField_FieldValue = new Label();
            mt_AddHeaderField_FieldValue.Size = new Size(100,20);
            mt_AddHeaderField_FieldValue.Location = new Point(0,150);
            mt_AddHeaderField_FieldValue.Visible = false;
            mt_AddHeaderField_FieldValue.TextAlign = ContentAlignment.MiddleRight;
            mt_AddHeaderField_FieldValue.Text = "Value:";

            m_pAddHeaderField_FieldValue = new TextBox();
            m_pAddHeaderField_FieldValue.Size = new Size(380,20);
            m_pAddHeaderField_FieldValue.Location = new Point(105,150);
            m_pAddHeaderField_FieldValue.Visible = false;
            //-----------------------------------------------------------------------------//

            #endregion

            #region Remove Header Field

            //--- Remove Header Field -----------------------------------------------------//
            mt_RemoveHeaderField_FieldName = new Label();
            mt_RemoveHeaderField_FieldName.Size = new Size(100,20);
            mt_RemoveHeaderField_FieldName.Location = new Point(0,125);
            mt_RemoveHeaderField_FieldName.Visible = false;
            mt_RemoveHeaderField_FieldName.TextAlign = ContentAlignment.MiddleRight;
            mt_RemoveHeaderField_FieldName.Text = "Name:";

            m_pRemoveHeaderField_FieldName = new TextBox();
            m_pRemoveHeaderField_FieldName.Size = new Size(380,20);
            m_pRemoveHeaderField_FieldName.Location = new Point(105,125);
            m_pRemoveHeaderField_FieldName.Visible = false;
            //----------------------------------------------------------------------------//

            #endregion

            #region Send Error To Client

            //--- Send Error To Client ---------------------------------------------------//
            mt_SendErrorToClient_ErrorText = new Label();            
            mt_SendErrorToClient_ErrorText.Size = new Size(100,20);
            mt_SendErrorToClient_ErrorText.Location = new Point(0,125);
            mt_SendErrorToClient_ErrorText.Text = "Error Text:";
            mt_SendErrorToClient_ErrorText.TextAlign = ContentAlignment.MiddleRight;
            mt_SendErrorToClient_ErrorText.Visible = false;

            m_pSendErrorToClient_ErrorText = new TextBox();            
            m_pSendErrorToClient_ErrorText.Size = new Size(380,20);
            m_pSendErrorToClient_ErrorText.Location = new Point(105,125);
            m_pSendErrorToClient_ErrorText.Visible = false;
            //----------------------------------------------------------------------------//

            #endregion

            #region Store To FTP Folder

            //--- Store To FTP Folder ----------------------------------------------------//
            mt_StoreToFTPFolder_Server = new Label();           
            mt_StoreToFTPFolder_Server.Size = new Size(100,20);
            mt_StoreToFTPFolder_Server.Location = new Point(0,125);
            mt_StoreToFTPFolder_Server.TextAlign = ContentAlignment.MiddleRight;
            mt_StoreToFTPFolder_Server.Text = "Host:";
            mt_StoreToFTPFolder_Server.Visible = false;

            m_pStoreToFTPFolder_Server = new TextBox();
            m_pStoreToFTPFolder_Server.Size = new Size(310,20);
            m_pStoreToFTPFolder_Server.Location = new Point(105,125);
            m_pStoreToFTPFolder_Server.Visible = false;

            m_pStoreToFTPFolder_Port = new NumericUpDown();
            m_pStoreToFTPFolder_Port.Size = new Size(65,13);
            m_pStoreToFTPFolder_Port.Location = new Point(420,125);
            m_pStoreToFTPFolder_Port.Minimum = 1;
            m_pStoreToFTPFolder_Port.Maximum = 99999;
            m_pStoreToFTPFolder_Port.Visible = false;

            mt_StoreToFTPFolder_User = new Label();         
            mt_StoreToFTPFolder_User.Size = new Size(100,20);
            mt_StoreToFTPFolder_User.Location = new Point(0,150);
            mt_StoreToFTPFolder_User.TextAlign = ContentAlignment.MiddleRight;
            mt_StoreToFTPFolder_User.Text = "User:";
            mt_StoreToFTPFolder_User.Visible = false;

            m_pStoreToFTPFolder_User = new TextBox();         
            m_pStoreToFTPFolder_User.Size = new Size(200,13);
            m_pStoreToFTPFolder_User.Location = new Point(105,150);
            m_pStoreToFTPFolder_User.Visible = false;

            mt_StoreToFTPFolder_Password = new Label();         
            mt_StoreToFTPFolder_Password.Size = new Size(100,20);
            mt_StoreToFTPFolder_Password.Location = new Point(0,175);
            mt_StoreToFTPFolder_Password.TextAlign = ContentAlignment.MiddleRight;
            mt_StoreToFTPFolder_Password.Text = "Password:";
            mt_StoreToFTPFolder_Password.Visible = false;

            m_pStoreToFTPFolder_Password = new TextBox();         
            m_pStoreToFTPFolder_Password.Size = new Size(200,20);
            m_pStoreToFTPFolder_Password.Location = new Point(105,175);
            m_pStoreToFTPFolder_Password.Visible = false;

            mt_StoreToFTPFolder_Folder = new Label();         
            mt_StoreToFTPFolder_Folder.Size = new Size(100,20);
            mt_StoreToFTPFolder_Folder.Location = new Point(0,200);
            mt_StoreToFTPFolder_Folder.TextAlign = ContentAlignment.MiddleRight;
            mt_StoreToFTPFolder_Folder.Text = "Folder:";
            mt_StoreToFTPFolder_Folder.Visible = false;

            m_pStoreToFTPFolder_Folder = new TextBox();         
            m_pStoreToFTPFolder_Folder.Size = new Size(380,20);
            m_pStoreToFTPFolder_Folder.Location = new Point(105,200);
            m_pStoreToFTPFolder_Folder.Visible = false;
            //----------------------------------------------------------------------------//

            #endregion

            #region Post To NNTP Newsgroup

            //--- Post To NNTP Newsgroup -------------------------------------------------//
            mt_PostToNNTPNewsgroup_Server = new Label();        
            mt_PostToNNTPNewsgroup_Server.Size = new Size(100,20);
            mt_PostToNNTPNewsgroup_Server.Location = new Point(0,125);
            mt_PostToNNTPNewsgroup_Server.TextAlign = ContentAlignment.MiddleRight;
            mt_PostToNNTPNewsgroup_Server.Text = "Host:";
            mt_PostToNNTPNewsgroup_Server.Visible = false;
   
            m_pPostToNNTPNewsgroup_Server = new TextBox();        
            m_pPostToNNTPNewsgroup_Server.Size = new Size(310,20);
            m_pPostToNNTPNewsgroup_Server.Location = new Point(105,125);
            m_pPostToNNTPNewsgroup_Server.Visible = false;

            m_pPostToNNTPNewsgroup_Port = new NumericUpDown();        
            m_pPostToNNTPNewsgroup_Port.Size = new Size(65,20);
            m_pPostToNNTPNewsgroup_Port.Location = new Point(420,125);
            m_pPostToNNTPNewsgroup_Port.Minimum = 1;
            m_pPostToNNTPNewsgroup_Port.Maximum = 99999;
            m_pPostToNNTPNewsgroup_Port.Value = 119;
            m_pPostToNNTPNewsgroup_Port.Visible = false;

            mt_PostToNNTPNewsgroup_Newsgroup = new Label();        
            mt_PostToNNTPNewsgroup_Newsgroup.Size = new Size(100,20);
            mt_PostToNNTPNewsgroup_Newsgroup.Location = new Point(0,150);
            mt_PostToNNTPNewsgroup_Newsgroup.TextAlign = ContentAlignment.MiddleRight;
            mt_PostToNNTPNewsgroup_Newsgroup.Text = "Newsgroup:";
            mt_PostToNNTPNewsgroup_Newsgroup.Visible = false;

            m_pPostToNNTPNewsgroup_Newsgroup = new TextBox();        
            m_pPostToNNTPNewsgroup_Newsgroup.Size = new Size(380,13);
            m_pPostToNNTPNewsgroup_Newsgroup.Location = new Point(105,150);
            m_pPostToNNTPNewsgroup_Newsgroup.Visible = false;
            //----------------------------------------------------------------------------//

            #endregion

            #region Post To HTTP

            //--- Post To HTTP -----------------------------------------------------------//
            mt_PostToHTTP_URL = new Label();    
            mt_PostToHTTP_URL.Size = new Size(100,20);
            mt_PostToHTTP_URL.Location = new Point(0,125);
            mt_PostToHTTP_URL.TextAlign = ContentAlignment.MiddleRight;
            mt_PostToHTTP_URL.Text = "URL:";
            mt_PostToHTTP_URL.Visible = false;

            m_pPostToHTTP_URL = new TextBox();    
            m_pPostToHTTP_URL.Size = new Size(380,20);
            m_pPostToHTTP_URL.Location = new Point(105,125);
            m_pPostToHTTP_URL.Visible = false;
            //----------------------------------------------------------------------------//

            #endregion


            // Common
            this.Controls.Add(m_pIcon);
            this.Controls.Add(mt_Info);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(mt_Description);
            this.Controls.Add(m_pDescription);
            this.Controls.Add(mt_Action);
            this.Controls.Add(m_pAction);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_pSeparator3);
            this.Controls.Add(m_pHelp);
            this.Controls.Add(m_pCancel);
            this.Controls.Add(m_Ok);
            // Auto Response
            this.Controls.Add(mt_AutoResponse_From);
            this.Controls.Add(m_pAutoResponse_From);
            this.Controls.Add(mt_AutoResponse_FullMEssage);
            this.Controls.Add(m_pAutoResponse_FullMEssage);
            this.Controls.Add(m_pAutoResponse_Compose);
            this.Controls.Add(mt_AutoResponse_Load);
            // Execute Program
            this.Controls.Add(mt_ExecuteProgram_ProgramToExecute);
            this.Controls.Add(m_pExecuteProgram_ProgramToExecute);
            this.Controls.Add(m_pExecuteProgram_BrowseProgramToExecute);
            this.Controls.Add(mt_ExecuteProgram_ProgramArguments);
            this.Controls.Add(m_pExecuteProgram_ProgramArguments);
            // Forward To Email
            this.Controls.Add(mt_ForwardToEmail_Email);
            this.Controls.Add(m_pForwardToEmail_Email);
            // Forward To Host
            this.Controls.Add(mt_ForwardToHost_Host);
            this.Controls.Add(m_pForwardToHost_Host);
            this.Controls.Add(mt_ForwardToHost_HostPort);
            this.Controls.Add(m_pForwardToHost_HostPort);
            // Store To Disk Folder
            this.Controls.Add(m_pStoreToDiskFolder_Folder);
            this.Controls.Add(m_pStoreToDiskFolder_BrowseFolder);
            this.Controls.Add(mt_StoreToDiskFolder_Folder);
            // Move To IMAP Folder
            this.Controls.Add(mt_MoveToIMAPFolder_Folder);
            this.Controls.Add(m_pMoveToIMAPFolder_Folder);
            // Add Header Field
            this.Controls.Add(mt_AddHeaderField_FieldName);
            this.Controls.Add(m_pAddHeaderField_FieldName);
            this.Controls.Add(mt_AddHeaderField_FieldValue);
            this.Controls.Add(m_pAddHeaderField_FieldValue);
            // Remove Header Field
            this.Controls.Add(mt_RemoveHeaderField_FieldName);
            this.Controls.Add(m_pRemoveHeaderField_FieldName);
            // Send Error To Client
            this.Controls.Add(mt_SendErrorToClient_ErrorText);
            this.Controls.Add(m_pSendErrorToClient_ErrorText);
            // Store To FTP Folder
            this.Controls.Add(mt_StoreToFTPFolder_Server);
            this.Controls.Add(m_pStoreToFTPFolder_Server);
            this.Controls.Add(m_pStoreToFTPFolder_Port);
            this.Controls.Add(mt_StoreToFTPFolder_User);
            this.Controls.Add(m_pAddHeaderField_FieldValue);
            this.Controls.Add(m_pStoreToFTPFolder_User);
            this.Controls.Add(mt_StoreToFTPFolder_Password);
            this.Controls.Add(m_pStoreToFTPFolder_Password);
            this.Controls.Add(mt_StoreToFTPFolder_Folder);
            this.Controls.Add(m_pStoreToFTPFolder_Folder);
            // Post To NNTP Newsgroup
            this.Controls.Add(mt_PostToNNTPNewsgroup_Server);
            this.Controls.Add(m_pPostToNNTPNewsgroup_Server);
            this.Controls.Add(m_pPostToNNTPNewsgroup_Port);
            this.Controls.Add(mt_PostToNNTPNewsgroup_Newsgroup);
            this.Controls.Add(m_pPostToNNTPNewsgroup_Newsgroup);
            // Post To HTTP
            this.Controls.Add(mt_PostToHTTP_URL);
            this.Controls.Add(m_pPostToHTTP_URL);
        }
                                                                                                
        #endregion


        #region Events Handling

        #region method m_pAction_SelectedIndexChanged

        private void m_pAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide all action data controls
            // Auto Response
            mt_AutoResponse_From.Visible = false;
            m_pAutoResponse_From.Visible = false;
            mt_AutoResponse_FullMEssage.Visible = false;
            m_pAutoResponse_FullMEssage.Visible = false;
            m_pAutoResponse_Compose.Visible = false;
            mt_AutoResponse_Load.Visible = false;
            // Execute Program
            mt_ExecuteProgram_ProgramToExecute.Visible = false;  
            m_pExecuteProgram_ProgramToExecute.Visible = false;
            m_pExecuteProgram_BrowseProgramToExecute.Visible = false;
            mt_ExecuteProgram_ProgramArguments.Visible = false;
            m_pExecuteProgram_ProgramArguments.Visible = false;
            // Forward To Email
            mt_ForwardToEmail_Email.Visible = false;
            m_pForwardToEmail_Email.Visible = false;
            // Forward To Host
            mt_ForwardToHost_Host.Visible = false;
            m_pForwardToHost_Host.Visible = false;   
            mt_ForwardToHost_HostPort.Visible = false;
            m_pForwardToHost_HostPort.Visible = false;
            // Store To Disk Folder           
            mt_StoreToDiskFolder_Folder.Visible = false;
            m_pStoreToDiskFolder_Folder.Visible = false;
            m_pStoreToDiskFolder_BrowseFolder.Visible = false;
            // Move To IMAP Folder
            mt_MoveToIMAPFolder_Folder.Visible = false;
            m_pMoveToIMAPFolder_Folder.Visible = false;
            // Add Header Field
            mt_AddHeaderField_FieldName.Visible = false;
            m_pAddHeaderField_FieldName.Visible = false;
            mt_AddHeaderField_FieldValue.Visible = false;
            m_pAddHeaderField_FieldValue.Visible = false;
            // Remove Header Field
            mt_RemoveHeaderField_FieldName.Visible = false;
            m_pRemoveHeaderField_FieldName.Visible = false;
            // Send Error To Client
            mt_SendErrorToClient_ErrorText.Visible = false;
            m_pSendErrorToClient_ErrorText.Visible = false;
            // Store To FTP Folder
            mt_StoreToFTPFolder_Server.Visible = false; 
            m_pStoreToFTPFolder_Server.Visible = false;
            m_pStoreToFTPFolder_Port.Visible = false;
            mt_StoreToFTPFolder_User.Visible = false;
            m_pStoreToFTPFolder_User.Visible = false;
            mt_StoreToFTPFolder_Password.Visible = false;
            m_pStoreToFTPFolder_Password.Visible = false;
            mt_StoreToFTPFolder_Folder.Visible = false;
            m_pStoreToFTPFolder_Folder.Visible = false;
            // Post To NNTP Newsgroup
            mt_PostToNNTPNewsgroup_Server.Visible = false;
            m_pPostToNNTPNewsgroup_Server.Visible = false;
            m_pPostToNNTPNewsgroup_Port.Visible = false;
            mt_PostToNNTPNewsgroup_Newsgroup.Visible = false;
            m_pPostToNNTPNewsgroup_Newsgroup.Visible = false;
            // Post To HTTP
            mt_PostToHTTP_URL.Visible = false;
            m_pPostToHTTP_URL.Visible = false;

                       
            if(m_pAction.SelectedItem.ToString() == "Auto Response"){
                mt_AutoResponse_From.Visible = true;
                m_pAutoResponse_From.Visible = true;
                mt_AutoResponse_FullMEssage.Visible = true;
                m_pAutoResponse_FullMEssage.Visible = true;
                m_pAutoResponse_Compose.Visible = true;
                mt_AutoResponse_Load.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Delete Message"){
            }
            else if(m_pAction.SelectedItem.ToString() == "Execute Program"){
                mt_ExecuteProgram_ProgramToExecute.Visible = true;  
                m_pExecuteProgram_ProgramToExecute.Visible = true;
                m_pExecuteProgram_BrowseProgramToExecute.Visible = true;
                mt_ExecuteProgram_ProgramArguments.Visible = true;
                m_pExecuteProgram_ProgramArguments.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Forward To Email"){
                mt_ForwardToEmail_Email.Visible = true;
                m_pForwardToEmail_Email.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Forward To Host"){
                mt_ForwardToHost_Host.Visible = true;
                m_pForwardToHost_Host.Visible = true;   
                mt_ForwardToHost_HostPort.Visible = true;
                m_pForwardToHost_HostPort.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Store To Disk Folder"){
                mt_StoreToDiskFolder_Folder.Visible = true;
                m_pStoreToDiskFolder_Folder.Visible = true;      
                m_pStoreToDiskFolder_BrowseFolder.Visible = true;                
            }
            else if(m_pAction.SelectedItem.ToString() == "Move To IMAP Folder"){
                mt_MoveToIMAPFolder_Folder.Visible = true;
                m_pMoveToIMAPFolder_Folder.Visible = true;
            }            
            else if(m_pAction.SelectedItem.ToString() == "Add Header Field"){
                mt_AddHeaderField_FieldName.Visible = true;
                m_pAddHeaderField_FieldName.Visible = true;
                mt_AddHeaderField_FieldValue.Visible = true;
                m_pAddHeaderField_FieldValue.Visible = true;
            }            
            else if(m_pAction.SelectedItem.ToString() == "Remove Header Field"){
                mt_RemoveHeaderField_FieldName.Visible = true;
                m_pRemoveHeaderField_FieldName.Visible = true;
            }            
            else if(m_pAction.SelectedItem.ToString() == "Remove Header Field"){
                mt_RemoveHeaderField_FieldName.Visible = true;
                m_pRemoveHeaderField_FieldName.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Send Error To Client"){
                mt_SendErrorToClient_ErrorText.Visible = true;
                m_pSendErrorToClient_ErrorText.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Store To FTP Folder"){
                mt_StoreToFTPFolder_Server.Visible = true; 
                m_pStoreToFTPFolder_Server.Visible = true;
                m_pStoreToFTPFolder_Port.Visible = true;
                mt_StoreToFTPFolder_User.Visible = true;
                m_pStoreToFTPFolder_User.Visible = true;
                mt_StoreToFTPFolder_Password.Visible = true;
                m_pStoreToFTPFolder_Password.Visible = true;
                mt_StoreToFTPFolder_Folder.Visible = true;
                m_pStoreToFTPFolder_Folder.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Post To NNTP Newsgroup"){
                mt_PostToNNTPNewsgroup_Server.Visible = true;
                m_pPostToNNTPNewsgroup_Server.Visible = true;
                m_pPostToNNTPNewsgroup_Port.Visible = true;
                mt_PostToNNTPNewsgroup_Newsgroup.Visible = true;
                m_pPostToNNTPNewsgroup_Newsgroup.Visible = true;
            }
            else if(m_pAction.SelectedItem.ToString() == "Post To HTTP"){
                mt_PostToHTTP_URL.Visible = true;
                m_pPostToHTTP_URL.Visible = true;
            }
        }

        #endregion


        #region method m_pHelp_Click

        private void m_pHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo pInf = new System.Diagnostics.ProcessStartInfo("explorer",Application.StartupPath + "\\Help\\Grules-Actions.htm");
			System.Diagnostics.Process.Start(pInf);
        }

        #endregion

        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        #region method m_Ok_Click

        private void m_Ok_Click(object sender, EventArgs e)
        {
            #region AutoResponse

            if(m_pAction.SelectedItem.ToString() == "Auto Response"){
                //--- Validate values ------------------------------------------------//
                if(m_pAutoResponse_FullMEssage.Text == ""){
                    MessageBox.Show(this,"Full Message: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_AutoResponse(
                        m_pDescription.Text,
                        m_pAutoResponse_From.Text,
                        System.Text.Encoding.Default.GetBytes(m_pAutoResponse_FullMEssage.Text)
                    );
                }
                else{
                    UserMessageRuleAction_AutoResponse a = (UserMessageRuleAction_AutoResponse)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.From        = m_pAutoResponse_From.Text;
                    a.Message     = System.Text.Encoding.Default.GetBytes(m_pAutoResponse_FullMEssage.Text);
                    a.Commit();
                }
            }

            #endregion

            #region DeleteMessage

            else if(m_pAction.SelectedItem.ToString() == "Delete Message"){
                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_DeleteMessage(m_pDescription.Text);
                }
                else{
                    UserMessageRuleAction_DeleteMessage a = (UserMessageRuleAction_DeleteMessage)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.Commit();
                }
            }

            #endregion

            #region ExecuteProgram

            else if(m_pAction.SelectedItem.ToString() == "Execute Program"){
                //--- Validate values ------------------------------------------------//
                if(m_pExecuteProgram_ProgramToExecute.Text == ""){
                    MessageBox.Show(this,"Program to Execute: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_ExecuteProgram(
                        m_pDescription.Text,
                        m_pExecuteProgram_ProgramToExecute.Text,
                        m_pExecuteProgram_ProgramArguments.Text
                    );
                }
                else{
                    UserMessageRuleAction_ExecuteProgram a = (UserMessageRuleAction_ExecuteProgram)m_pActionData;
                    a.Description      = m_pDescription.Text;
                    a.Program          = m_pExecuteProgram_ProgramToExecute.Text;
                    a.ProgramArguments = m_pExecuteProgram_ProgramArguments.Text;
                    a.Commit();
                }
            }

            #endregion

            #region ForwardToEmail

            else if(m_pAction.SelectedItem.ToString() == "Forward To Email"){
                //--- Validate values ------------------------------------------------//
                if(m_pForwardToEmail_Email.Text == ""){
                    MessageBox.Show(this,"Forward to Email: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_ForwardToEmail(
                        m_pDescription.Text,
                        m_pForwardToEmail_Email.Text
                    );
                }
                else{
                    UserMessageRuleAction_ForwardToEmail a = (UserMessageRuleAction_ForwardToEmail)m_pActionData;
                    a.Description  = m_pDescription.Text;
                    a.EmailAddress = m_pForwardToEmail_Email.Text;
                    a.Commit();
                }
            }

            #endregion

            #region ForwartToHost

            else if(m_pAction.SelectedItem.ToString() == "Forward To Host"){
                //--- Validate values ------------------------------------------------//
                if(m_pForwardToHost_Host.Text == ""){
                    MessageBox.Show(this,"Forward to Host: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_ForwardToHost(
                        m_pDescription.Text,
                        m_pForwardToHost_Host.Text,
                        (int)m_pForwardToHost_HostPort.Value
                    );
                }
                else{
                    UserMessageRuleAction_ForwardToHost a = (UserMessageRuleAction_ForwardToHost)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.Host        = m_pForwardToHost_Host.Text;
                    a.Port        = (int)m_pForwardToHost_HostPort.Value;
                    a.Commit();
                }
            }

            #endregion

            #region StoreToDiskFolder

            else if(m_pAction.SelectedItem.ToString() == "Store To Disk Folder"){
                //--- Validate values ------------------------------------------------//
                if(m_pStoreToDiskFolder_Folder.Text == ""){
                    MessageBox.Show(this,"Store to Disk Folder: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_StoreToDisk(
                        m_pDescription.Text,
                        m_pStoreToDiskFolder_Folder.Text
                    );
                }
                else{
                    UserMessageRuleAction_StoreToDiskFolder a = (UserMessageRuleAction_StoreToDiskFolder)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.Folder      = m_pStoreToDiskFolder_Folder.Text;
                    a.Commit();
                }
            }

            #endregion

            #region MoveToIMAPFolder

            else if(m_pAction.SelectedItem.ToString() == "Move To IMAP Folder"){
                //--- Validate values ------------------------------------------------//
                if(m_pMoveToIMAPFolder_Folder.Text == ""){
                    MessageBox.Show(this,"Move to IMAP Folder: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_MoveToImapFolder(
                        m_pDescription.Text,
                        m_pMoveToIMAPFolder_Folder.Text
                    );
                }
                else{
                    UserMessageRuleAction_MoveToImapFolder a = (UserMessageRuleAction_MoveToImapFolder)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.Folder      = m_pMoveToIMAPFolder_Folder.Text;
                    a.Commit();
                }
            }

            #endregion

            #region AddHeaderField

            else if(m_pAction.SelectedItem.ToString() == "Add Header Field"){
                //--- Validate values ------------------------------------------------//
                if(m_pAddHeaderField_FieldName.Text == ""){
                    MessageBox.Show(this,"Header Field Name: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_AddHeaderField(
                        m_pDescription.Text,
                        m_pAddHeaderField_FieldName.Text,
                        m_pAddHeaderField_FieldValue.Text
                    );
                }
                else{
                    UserMessageRuleAction_AddHeaderField a = (UserMessageRuleAction_AddHeaderField)m_pActionData;
                    a.Description      = m_pDescription.Text;
                    a.HeaderFieldName  = m_pAddHeaderField_FieldName.Text;
                    a.HeaderFieldValue = m_pAddHeaderField_FieldValue.Text;
                    a.Commit();
                }
            }

            #endregion

            #region RemoveHeaderField

            else if(m_pAction.SelectedItem.ToString() == "Remove Header Field"){
                //--- Validate values ------------------------------------------------//
                if(m_pRemoveHeaderField_FieldName.Text == ""){
                    MessageBox.Show(this,"Header Field Name: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_RemoveHeaderField(
                        m_pDescription.Text,
                        m_pAddHeaderField_FieldName.Text
                    );
                }
                else{
                    UserMessageRuleAction_RemoveHeaderField a = (UserMessageRuleAction_RemoveHeaderField)m_pActionData;
                    a.Description      = m_pDescription.Text;
                    a.HeaderFieldName  = m_pAddHeaderField_FieldName.Text;
                    a.Commit();
                }
            }

            #endregion

            #region StoreToFTPFolder

            else if(m_pAction.SelectedItem.ToString() == "Store To FTP Folder"){
                //--- Validate values ------------------------------------------------//
                if(m_pStoreToFTPFolder_Server.Text == ""){
                    MessageBox.Show(this,"FTP Server: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_StoreToFtp(
                        m_pDescription.Text,
                        m_pStoreToFTPFolder_Server.Text,
                        (int)m_pStoreToFTPFolder_Port.Value,
                        m_pStoreToFTPFolder_User.Text,
                        m_pStoreToFTPFolder_Password.Text,
                        m_pStoreToFTPFolder_Folder.Text
                    );
                }
                else{
                    UserMessageRuleAction_StoreToFtp a = (UserMessageRuleAction_StoreToFtp)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.Server      = m_pStoreToFTPFolder_Server.Text;
                    a.Port        = (int)m_pStoreToFTPFolder_Port.Value;
                    a.UserName    = m_pStoreToFTPFolder_User.Text;
                    a.Password    = m_pStoreToFTPFolder_Password.Text;
                    a.Folder      = m_pStoreToFTPFolder_Folder.Text;
                    a.Commit();
                }
            }

            #endregion

            #region PostToNNTPNewsgroup

            else if(m_pAction.SelectedItem.ToString() == "Post To NNTP Newsgroup"){
                //--- Validate values ------------------------------------------------//
                if(m_pPostToNNTPNewsgroup_Server.Text == ""){
                    MessageBox.Show(this,"NNTP Server: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                if(m_pPostToNNTPNewsgroup_Newsgroup.Text == ""){
                    MessageBox.Show(this,"Newsgroup: value can't empty !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                //--------------------------------------------------------------------//

                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_PostToNntp(
                        m_pDescription.Text,
                        m_pPostToNNTPNewsgroup_Server.Text,
                        (int)m_pPostToNNTPNewsgroup_Port.Value,
                        m_pPostToNNTPNewsgroup_Newsgroup.Text
                    );
                }
                else{
                    UserMessageRuleAction_PostToNntpNewsgroup a = (UserMessageRuleAction_PostToNntpNewsgroup)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.Server      = m_pPostToNNTPNewsgroup_Server.Text;
                    a.Port        = (int)m_pPostToNNTPNewsgroup_Port.Value;
                    a.Newsgroup   = m_pPostToNNTPNewsgroup_Newsgroup.Text;
                    a.Commit();
                }
            }

            #endregion

            #region PostToHTTP

            else if(m_pAction.SelectedItem.ToString() == "Post To HTTP"){
                if(m_pActionData == null){
                    m_pActionData = m_pRule.Actions.Add_PostToHttp(
                        m_pDescription.Text,
                        m_pPostToHTTP_URL.Text
                    );
                }
                else{
                    UserMessageRuleAction_PostToHttp a = (UserMessageRuleAction_PostToHttp)m_pActionData;
                    a.Description = m_pDescription.Text;
                    a.Url         = m_pPostToHTTP_URL.Text;
                    a.Commit();
                }
            }

            #endregion
                       
            this.DialogResult = DialogResult.OK;
        }

        #endregion


        #region method m_pAutoResponse_Compose_Click

        private void m_pAutoResponse_Compose_Click(object sender, EventArgs e)
        {
            wfrm_GlobalMessageRule_Action_Compose frm = new wfrm_GlobalMessageRule_Action_Compose();
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pAutoResponse_FullMEssage.Text = frm.Message;
            }
        }

        #endregion

        #region method mt_AutoResponse_Load_Click

        private void mt_AutoResponse_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pAutoResponse_FullMEssage.Text = File.ReadAllText(dlg.FileName);
            }
        }

        #endregion


        #region method m_pExecuteProgram_BrowseProgramToExecute_Click

        private void m_pExecuteProgram_BrowseProgramToExecute_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pExecuteProgram_ProgramToExecute.Text = dlg.FileName;
            }
        }

        #endregion


        #region method m_pStoreToDiskFolder_BrowseFolder_Click

        private void m_pStoreToDiskFolder_BrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pStoreToDiskFolder_Folder.Text = dlg.SelectedPath;
            }
        }

        #endregion

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets actionID.
        /// </summary>
        public string ActionID
        {
            get{ 
                if(m_pActionData != null){
                    return m_pActionData.ID;
                }
                else{
                    return ""; 
                }
            }
        }

        /// <summary>
        /// Gets action description.
        /// </summary>
        public string Description
        {
            get{ return m_pDescription.Text; }
        }

        /// <summary>
        /// Gets action type.
        /// </summary>
        public GlobalMessageRuleAction_enum ActionType
        {
            get{ return ((GlobalMessageRuleAction_enum)((WComboBoxItem)m_pAction.SelectedItem).Tag); }
        }

        #endregion

    }
}
