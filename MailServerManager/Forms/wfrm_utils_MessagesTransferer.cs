using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Data;

using LumiSoft.Net;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.IMAP.Client;
using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Messages import/export window.
    /// </summary>
    public class wfrm_utils_MessagesTransferer : Form
    {     
        //--- Common UI
        private PictureBox m_pIcon       = null;
        private Label      m_pTitle      = null;
        private GroupBox   m_pSeparator1 = null;
        private GroupBox   m_pSeparator2 = null;
        private Button     m_pBack       = null;
        private Button     m_pNext       = null;
        private Button     m_pCancel     = null;
        //--- Source UI
        private Panel         m_pSource                    = null;
        private Label         mt_Source_Type               = null;
        private ComboBox      m_pSource_Type               = null;
        private Label         mt_Source_TypeLSUser_User    = null;
        private TextBox       m_pSource_TypeLSUser_User    = null;
        private Button        m_pSource_TypeLSUser_UserGet = null;
        private Label         mt_Source_TypeIMAP_Host      = null;
        private TextBox       m_pSource_TypeIMAP_Host      = null;
        private NumericUpDown m_pSource_TypeIMAP_Port      = null;
        private CheckBox      m_pSource_TypeIMAP_UseSSL    = null;
        private Label         mt_Source_TypeIMAP_User      = null;
        private TextBox       m_pSource_TypeIMAP_User      = null;
        private Label         mt_Source_TypeIMAP_Password  = null;
        private TextBox       m_pSource_TypeIMAP_Password  = null;
        private Label         mt_Source_TypeZIP_File       = null;
        private TextBox       m_pSource_TypeZIP_File       = null;
        private Button        m_pSource_TypeZIP_FileGet    = null;
        //--- Folder UI
        private Panel     m_pFolders           = null;
        private Button    m_pFolders_SelectAll = null;
        private WTreeView m_pFolders_Folders   = null;
        //--- Destination UI
        private Panel         m_pDestination                    = null;
        private Label         mt_Destination_Type               = null;
        private ComboBox      m_pDestination_Type               = null;
        private Label         mt_Destination_TypeLSUser_User    = null;
        private TextBox       m_pDestination_TypeLSUser_User    = null;
        private Button        m_pDestination_TypeLSUser_UserGet = null;
        private Label         mt_Destination_TypeIMAP_Host      = null;
        private TextBox       m_pDestination_TypeIMAP_Host      = null;
        private NumericUpDown m_pDestination_TypeIMAP_Port      = null;
        private CheckBox      m_pDestination_TypeIMAP_UseSSL    = null;
        private Label         mt_Destination_TypeIMAP_User      = null;
        private TextBox       m_pDestination_TypeIMAP_User      = null;
        private Label         mt_Destination_TypeIMAP_Password  = null;
        private TextBox       m_pDestination_TypeIMAP_Password  = null;
        private Label         mt_Destination_TypeZIP_File       = null;
        private TextBox       m_pDestination_TypeZIP_File       = null;
        private Button        m_pDestination_TypeZIP_FileGet    = null;
        //--- Finish UI
        private Panel       m_pFinish           = null;
        private ProgressBar m_pFinish_Progress  = null;
        private ListView    m_pFinish_Completed = null;

        private User    m_pUser              = null;
        private string  m_Step               = "source";
        private int     m_SourceType         = -1;
        private object  m_pSourceObject      = null;
        private int     m_DestinationType    = -1;
        private object  m_pDestinationObject = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">User.</param>
        public wfrm_utils_MessagesTransferer(User user)
        {
            m_pUser = user;

            InitUI();

            SwitchStep("source");
        }
                
        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(500,400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Transfer Messages";
            this.Icon = ResManager.GetIcon("ruleaction.ico");
            this.FormClosed += new FormClosedEventHandler(wfrm_utils_MessagesTransferer_FormClosed);

            #region Common UI

            m_pIcon = new PictureBox();
            m_pIcon.Size = new Size(36,36);
            m_pIcon.Location = new Point(10,5);
            m_pIcon.Image = ResManager.GetIcon("ruleaction.ico").ToBitmap();

            m_pTitle = new Label();
            m_pTitle.Size = new Size(300,30);
            m_pTitle.Location = new Point(50,10);
            m_pTitle.TextAlign = ContentAlignment.MiddleLeft;

            m_pSeparator1 = new GroupBox();
            m_pSeparator1.Size = new Size(490,3);
            m_pSeparator1.Location = new Point(5,44);
            
            m_pSeparator2 = new GroupBox();
            m_pSeparator2.Size = new Size(490,3);
            m_pSeparator2.Location = new Point(5,360);

            m_pBack = new Button();
            m_pBack.Size = new Size(70,20);
            m_pBack.Location = new Point(265,375);
            m_pBack.Text = "Back";
            m_pBack.Click += new EventHandler(m_pBack_Click);

            m_pNext = new Button();
            m_pNext.Size = new Size(70,20);
            m_pNext.Location = new Point(340,375);
            m_pNext.Text = "Next";
            m_pNext.Click += new EventHandler(m_pNext_Click);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(420,375);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);
            
            this.Controls.Add(m_pIcon);
            this.Controls.Add(m_pTitle);
            this.Controls.Add(m_pSeparator1);
            this.Controls.Add(m_pSeparator2);
            this.Controls.Add(m_pBack);
            this.Controls.Add(m_pNext);
            this.Controls.Add(m_pCancel);

            #endregion

            #region Source UI

            m_pSource = new Panel();
            m_pSource.Size = new Size(500,300);
            m_pSource.Location = new Point(0,75);
            m_pSource.Visible = false;

            mt_Source_Type = new Label();
            mt_Source_Type.Size = new Size(100,20);
            mt_Source_Type.Location = new Point(0,0);
            mt_Source_Type.TextAlign = ContentAlignment.MiddleRight;
            mt_Source_Type.Text = "Source:";

            m_pSource_Type = new ComboBox();
            m_pSource_Type.Size = new Size(300,20);
            m_pSource_Type.Location = new Point(105,0);
            m_pSource_Type.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pSource_Type.Items.Add("LumiSoft Mail Server User");
            m_pSource_Type.Items.Add("IMAP");
            m_pSource_Type.Items.Add("ZIP Messages Archive");
            m_pSource_Type.SelectedIndexChanged += new EventHandler(m_pSource_Type_SelectedIndexChanged);

            mt_Source_TypeLSUser_User = new Label();
            mt_Source_TypeLSUser_User.Size = new Size(100,20);
            mt_Source_TypeLSUser_User.Location = new Point(0,25);
            mt_Source_TypeLSUser_User.TextAlign = ContentAlignment.MiddleRight;
            mt_Source_TypeLSUser_User.Text = "User:";
            mt_Source_TypeLSUser_User.Visible = false;

            m_pSource_TypeLSUser_User = new TextBox();
            m_pSource_TypeLSUser_User.Size = new Size(270,20);
            m_pSource_TypeLSUser_User.Location = new Point(105,25);
            m_pSource_TypeLSUser_User.ReadOnly = true;
            m_pSource_TypeLSUser_User.Visible = false;

            m_pSource_TypeLSUser_UserGet = new Button();
            m_pSource_TypeLSUser_UserGet.Size = new Size(25,20);
            m_pSource_TypeLSUser_UserGet.Location = new Point(380,25);
            m_pSource_TypeLSUser_UserGet.Text = "...";
            m_pSource_TypeLSUser_UserGet.Visible = false;
            m_pSource_TypeLSUser_UserGet.Click += new EventHandler(m_pSource_TypeLSUser_UserGet_Click);

            mt_Source_TypeIMAP_Host = new Label();
            mt_Source_TypeIMAP_Host.Size = new Size(100,20);
            mt_Source_TypeIMAP_Host.Location = new Point(0,25);
            mt_Source_TypeIMAP_Host.TextAlign = ContentAlignment.MiddleRight;
            mt_Source_TypeIMAP_Host.Text = "Host:";
            mt_Source_TypeIMAP_Host.Visible = false;

            m_pSource_TypeIMAP_Host = new TextBox();
            m_pSource_TypeIMAP_Host.Size = new Size(150,20);
            m_pSource_TypeIMAP_Host.Location = new Point(105,25);
            m_pSource_TypeIMAP_Host.Visible = false;

            m_pSource_TypeIMAP_Port = new NumericUpDown();
            m_pSource_TypeIMAP_Port.Size = new Size(45,20);
            m_pSource_TypeIMAP_Port.Location = new Point(260,25);
            m_pSource_TypeIMAP_Port.Minimum = 1;
            m_pSource_TypeIMAP_Port.Maximum = 99999;
            m_pSource_TypeIMAP_Port.Value = 143;
            m_pSource_TypeIMAP_Port.Visible = false;

            m_pSource_TypeIMAP_UseSSL = new CheckBox();
            m_pSource_TypeIMAP_UseSSL.Size = new Size(200,20);
            m_pSource_TypeIMAP_UseSSL.Location = new Point(315,25);
            m_pSource_TypeIMAP_UseSSL.Text = "Use SSL";
            m_pSource_TypeIMAP_UseSSL.Visible = false;

            mt_Source_TypeIMAP_User = new Label();
            mt_Source_TypeIMAP_User.Size = new Size(100,20);
            mt_Source_TypeIMAP_User.Location = new Point(0,50);
            mt_Source_TypeIMAP_User.TextAlign = ContentAlignment.MiddleRight;
            mt_Source_TypeIMAP_User.Text = "User:";
            mt_Source_TypeIMAP_User.Visible = false;

            m_pSource_TypeIMAP_User = new TextBox();
            m_pSource_TypeIMAP_User.Size = new Size(150,20);
            m_pSource_TypeIMAP_User.Location = new Point(105,50);
            m_pSource_TypeIMAP_User.Visible = false;

            mt_Source_TypeIMAP_Password = new Label();
            mt_Source_TypeIMAP_Password.Size = new Size(100,20);
            mt_Source_TypeIMAP_Password.Location = new Point(0,75);
            mt_Source_TypeIMAP_Password.TextAlign = ContentAlignment.MiddleRight;
            mt_Source_TypeIMAP_Password.Text = "Password:";
            mt_Source_TypeIMAP_Password.Visible = false;

            m_pSource_TypeIMAP_Password = new TextBox();
            m_pSource_TypeIMAP_Password.Size = new Size(150,20);
            m_pSource_TypeIMAP_Password.Location = new Point(105,75);
            m_pSource_TypeIMAP_Password.PasswordChar = '*';
            m_pSource_TypeIMAP_Password.Visible = false;

            mt_Source_TypeZIP_File = new Label();
            mt_Source_TypeZIP_File.Size = new Size(100,20);
            mt_Source_TypeZIP_File.Location = new Point(0,25);
            mt_Source_TypeZIP_File.TextAlign = ContentAlignment.MiddleRight;
            mt_Source_TypeZIP_File.Text = "Zip File:";
            mt_Source_TypeZIP_File.Visible = false;

            m_pSource_TypeZIP_File = new TextBox();
            m_pSource_TypeZIP_File.Size = new Size(270,20);
            m_pSource_TypeZIP_File.Location = new Point(105,25);
            m_pSource_TypeZIP_File.ReadOnly = true;
            m_pSource_TypeZIP_File.Visible = false;

            m_pSource_TypeZIP_FileGet = new Button();
            m_pSource_TypeZIP_FileGet.Size = new Size(25,20);
            m_pSource_TypeZIP_FileGet.Location = new Point(380,25);
            m_pSource_TypeZIP_FileGet.Text = "...";
            m_pSource_TypeZIP_FileGet.Visible = false;
            m_pSource_TypeZIP_FileGet.Click += new EventHandler(m_pSource_TypeZIP_FileGet_Click);

            m_pSource.Controls.Add(mt_Source_Type);
            m_pSource.Controls.Add(m_pSource_Type);
            m_pSource.Controls.Add(mt_Source_TypeLSUser_User);
            m_pSource.Controls.Add(m_pSource_TypeLSUser_User);
            m_pSource.Controls.Add(m_pSource_TypeLSUser_UserGet);
            m_pSource.Controls.Add(mt_Source_TypeIMAP_Host);
            m_pSource.Controls.Add(m_pSource_TypeIMAP_Host);
            m_pSource.Controls.Add(m_pSource_TypeIMAP_Port);
            m_pSource.Controls.Add(m_pSource_TypeIMAP_UseSSL);
            m_pSource.Controls.Add(mt_Source_TypeIMAP_User);
            m_pSource.Controls.Add(m_pSource_TypeIMAP_User);
            m_pSource.Controls.Add(mt_Source_TypeIMAP_Password);
            m_pSource.Controls.Add(m_pSource_TypeIMAP_Password);
            m_pSource.Controls.Add(mt_Source_TypeZIP_File);
            m_pSource.Controls.Add(m_pSource_TypeZIP_File);
            m_pSource.Controls.Add(m_pSource_TypeZIP_FileGet);
            
            this.Controls.Add(m_pSource);
            
            #endregion

            #region Folders UI

            m_pFolders = new Panel();
            m_pFolders.Size = new Size(500,300);
            m_pFolders.Location = new Point(0,50);
            m_pFolders.Visible = false;

            m_pFolders_SelectAll = new Button();
            m_pFolders_SelectAll.Size = new Size(70,20);
            m_pFolders_SelectAll.Location = new Point(10,0);
            m_pFolders_SelectAll.Text = "Select All";
            m_pFolders_SelectAll.Click += new EventHandler(m_pFolders_SelectAll_Click);

            ImageList imageList_m_pFolders_Folders = new ImageList();
            imageList_m_pFolders_Folders.Images.Add(ResManager.GetIcon("folder.ico"));

            m_pFolders_Folders = new WTreeView();
            m_pFolders_Folders.Size = new Size(480,265);
            m_pFolders_Folders.Location = new Point(10,25);
            m_pFolders_Folders.CheckBoxes = true;
            m_pFolders_Folders.ImageList = imageList_m_pFolders_Folders;

            m_pFolders.Controls.Add(m_pFolders_SelectAll);
            m_pFolders.Controls.Add(m_pFolders_Folders);

            this.Controls.Add(m_pFolders);

            #endregion

            #region Destination UI

            m_pDestination = new Panel();
            m_pDestination.Size = new Size(500,300);
            m_pDestination.Location = new Point(0,50);
            m_pDestination.Visible = false;

            mt_Destination_Type = new Label();
            mt_Destination_Type.Size = new Size(100,20);
            mt_Destination_Type.Location = new Point(0,25);
            mt_Destination_Type.TextAlign = ContentAlignment.MiddleRight;
            mt_Destination_Type.Text = "Destination:";

            m_pDestination_Type = new ComboBox();
            m_pDestination_Type.Size = new Size(300,20);
            m_pDestination_Type.Location = new Point(105,25);
            m_pDestination_Type.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pDestination_Type.Items.Add("LumiSoft Mail Server User");
            m_pDestination_Type.Items.Add("IMAP");
            m_pDestination_Type.Items.Add("ZIP Messages Archive");
            m_pDestination_Type.SelectedIndexChanged += new EventHandler(m_pDestination_Type_SelectedIndexChanged);

            mt_Destination_TypeLSUser_User = new Label();
            mt_Destination_TypeLSUser_User.Size = new Size(100,20);
            mt_Destination_TypeLSUser_User.Location = new Point(0,50);
            mt_Destination_TypeLSUser_User.TextAlign = ContentAlignment.MiddleRight;
            mt_Destination_TypeLSUser_User.Text = "User:";
            mt_Destination_TypeLSUser_User.Visible = false;

            m_pDestination_TypeLSUser_User = new TextBox();
            m_pDestination_TypeLSUser_User.Size = new Size(270,20);
            m_pDestination_TypeLSUser_User.Location = new Point(105,50);
            m_pDestination_TypeLSUser_User.ReadOnly = true;
            m_pDestination_TypeLSUser_User.Visible = false;

            m_pDestination_TypeLSUser_UserGet = new Button();
            m_pDestination_TypeLSUser_UserGet.Size = new Size(25,20);
            m_pDestination_TypeLSUser_UserGet.Location = new Point(380,50);
            m_pDestination_TypeLSUser_UserGet.Text = "...";
            m_pDestination_TypeLSUser_UserGet.Visible = false;
            m_pDestination_TypeLSUser_UserGet.Click += new EventHandler(m_pDestination_TypeLSUser_UserGet_Click);

            mt_Destination_TypeIMAP_Host = new Label();
            mt_Destination_TypeIMAP_Host.Size = new Size(100,20);
            mt_Destination_TypeIMAP_Host.Location = new Point(0,50);
            mt_Destination_TypeIMAP_Host.TextAlign = ContentAlignment.MiddleRight;
            mt_Destination_TypeIMAP_Host.Text = "Host:";
            mt_Destination_TypeIMAP_Host.Visible = false;

            m_pDestination_TypeIMAP_Host = new TextBox();
            m_pDestination_TypeIMAP_Host.Size = new Size(150,20);
            m_pDestination_TypeIMAP_Host.Location = new Point(105,50);
            m_pDestination_TypeIMAP_Host.Visible = false;

            m_pDestination_TypeIMAP_Port = new NumericUpDown();
            m_pDestination_TypeIMAP_Port.Size = new Size(45,20);
            m_pDestination_TypeIMAP_Port.Location = new Point(260,50);
            m_pDestination_TypeIMAP_Port.Minimum = 1;
            m_pDestination_TypeIMAP_Port.Maximum = 99999;
            m_pDestination_TypeIMAP_Port.Value = 143;
            m_pDestination_TypeIMAP_Port.Visible = false;

            m_pDestination_TypeIMAP_UseSSL = new CheckBox();
            m_pDestination_TypeIMAP_UseSSL.Size = new Size(200,20);
            m_pDestination_TypeIMAP_UseSSL.Location = new Point(315,50);
            m_pDestination_TypeIMAP_UseSSL.Text = "Use SSL";
            m_pDestination_TypeIMAP_UseSSL.Visible = false;

            mt_Destination_TypeIMAP_User = new Label();
            mt_Destination_TypeIMAP_User.Size = new Size(100,20);
            mt_Destination_TypeIMAP_User.Location = new Point(0,75);
            mt_Destination_TypeIMAP_User.TextAlign = ContentAlignment.MiddleRight;
            mt_Destination_TypeIMAP_User.Text = "User:";
            mt_Destination_TypeIMAP_User.Visible = false;

            m_pDestination_TypeIMAP_User = new TextBox();
            m_pDestination_TypeIMAP_User.Size = new Size(150,20);
            m_pDestination_TypeIMAP_User.Location = new Point(105,75);
            m_pDestination_TypeIMAP_User.Visible = false;

            mt_Destination_TypeIMAP_Password = new Label();
            mt_Destination_TypeIMAP_Password.Size = new Size(100,20);
            mt_Destination_TypeIMAP_Password.Location = new Point(0,100);
            mt_Destination_TypeIMAP_Password.TextAlign = ContentAlignment.MiddleRight;
            mt_Destination_TypeIMAP_Password.Text = "Password:";
            mt_Destination_TypeIMAP_Password.Visible = false;

            m_pDestination_TypeIMAP_Password = new TextBox();
            m_pDestination_TypeIMAP_Password.Size = new Size(150,20);
            m_pDestination_TypeIMAP_Password.Location = new Point(105,100);
            m_pDestination_TypeIMAP_Password.PasswordChar = '*';
            m_pDestination_TypeIMAP_Password.Visible = false;

            mt_Destination_TypeZIP_File = new Label();
            mt_Destination_TypeZIP_File.Size = new Size(100,20);
            mt_Destination_TypeZIP_File.Location = new Point(0,50);
            mt_Destination_TypeZIP_File.TextAlign = ContentAlignment.MiddleRight;
            mt_Destination_TypeZIP_File.Text = "Zip File:";
            mt_Destination_TypeZIP_File.Visible = false;

            m_pDestination_TypeZIP_File = new TextBox();
            m_pDestination_TypeZIP_File.Size = new Size(270,20);
            m_pDestination_TypeZIP_File.Location = new Point(105,50);
            m_pDestination_TypeZIP_File.ReadOnly = true;
            m_pDestination_TypeZIP_File.Visible = false;

            m_pDestination_TypeZIP_FileGet = new Button();
            m_pDestination_TypeZIP_FileGet.Size = new Size(25,20);
            m_pDestination_TypeZIP_FileGet.Location = new Point(380,50);
            m_pDestination_TypeZIP_FileGet.Text = "...";
            m_pDestination_TypeZIP_FileGet.Visible = false;
            m_pDestination_TypeZIP_FileGet.Click += new EventHandler(m_pDestination_TypeZIP_FileGet_Click);

            m_pDestination.Controls.Add(mt_Destination_Type);
            m_pDestination.Controls.Add(m_pDestination_Type);
            m_pDestination.Controls.Add(mt_Destination_TypeLSUser_User);
            m_pDestination.Controls.Add(m_pDestination_TypeLSUser_User);
            m_pDestination.Controls.Add(m_pDestination_TypeLSUser_UserGet);
            m_pDestination.Controls.Add(mt_Destination_TypeIMAP_Host);
            m_pDestination.Controls.Add(m_pDestination_TypeIMAP_Host);
            m_pDestination.Controls.Add(m_pDestination_TypeIMAP_Port);
            m_pDestination.Controls.Add(m_pDestination_TypeIMAP_UseSSL);
            m_pDestination.Controls.Add(mt_Destination_TypeIMAP_User);
            m_pDestination.Controls.Add(m_pDestination_TypeIMAP_User);
            m_pDestination.Controls.Add(mt_Destination_TypeIMAP_Password);
            m_pDestination.Controls.Add(m_pDestination_TypeIMAP_Password);
            m_pDestination.Controls.Add(mt_Destination_TypeZIP_File);
            m_pDestination.Controls.Add(m_pDestination_TypeZIP_File);
            m_pDestination.Controls.Add(m_pDestination_TypeZIP_FileGet);

            this.Controls.Add(m_pDestination);

            #endregion

            #region Finish UI

            m_pFinish = new Panel();
            m_pFinish.Size = new Size(500,300);
            m_pFinish.Location = new Point(0,50);
            m_pFinish.Visible = false;

            m_pFinish_Progress = new ProgressBar();
            m_pFinish_Progress.Size = new Size(480,20);
            m_pFinish_Progress.Location = new Point(10,0);

            ImageList imgList_m_pFinish_Completed = new ImageList();
            imgList_m_pFinish_Completed.Images.Add(ResManager.GetIcon("folder.ico"));

            m_pFinish_Completed = new ListView();
            m_pFinish_Completed.Size = new Size(480,275);
            m_pFinish_Completed.Location = new Point(10,25);
            m_pFinish_Completed.View = View.Details;
            m_pFinish_Completed.FullRowSelect = true;
            m_pFinish_Completed.HideSelection = false;
            m_pFinish_Completed.SmallImageList = imgList_m_pFinish_Completed;
            m_pFinish_Completed.Columns.Add("Folder",340);
            m_pFinish_Completed.Columns.Add("Count",70);
            m_pFinish_Completed.Columns.Add("Errors",50);
            m_pFinish_Completed.DoubleClick += new EventHandler(m_pFinish_Completed_DoubleClick);

            m_pFinish.Controls.Add(m_pFinish_Progress);
            m_pFinish.Controls.Add(m_pFinish_Completed);

            this.Controls.Add(m_pFinish);

            #endregion
        }
                                
        #endregion


        #region Events Handling

        #region Source

        #region method m_pSource_Type_SelectedIndexChanged

        private void m_pSource_Type_SelectedIndexChanged(object sender,EventArgs e)
        {
            // Hide controls
            mt_Source_TypeLSUser_User.Visible = false;
            m_pSource_TypeLSUser_User.Visible = false;
            m_pSource_TypeLSUser_UserGet.Visible = false;
            mt_Source_TypeIMAP_Host.Visible = false;
            m_pSource_TypeIMAP_Host.Visible = false;
            m_pSource_TypeIMAP_Port.Visible = false;
            m_pSource_TypeIMAP_UseSSL.Visible = false;
            mt_Source_TypeIMAP_User.Visible = false;
            m_pSource_TypeIMAP_User.Visible = false;
            mt_Source_TypeIMAP_Password.Visible = false;
            m_pSource_TypeIMAP_Password.Visible = false;
            mt_Source_TypeZIP_File.Visible = false;
            m_pSource_TypeZIP_File.Visible = false;
            m_pSource_TypeZIP_FileGet.Visible = false;

            //--- Show right Type: controls ---------------------
            // LumiSoft Mail Server User
            if(m_pSource_Type.SelectedIndex == 0){
                mt_Source_TypeLSUser_User.Visible = true;
                m_pSource_TypeLSUser_User.Visible = true;
                m_pSource_TypeLSUser_UserGet.Visible = true;
            }
            // IMAP
            else if(m_pSource_Type.SelectedIndex == 1){
                mt_Source_TypeIMAP_Host.Visible = true;
                m_pSource_TypeIMAP_Host.Visible = true;
                m_pSource_TypeIMAP_Port.Visible = true;
                m_pSource_TypeIMAP_UseSSL.Visible = true;
                mt_Source_TypeIMAP_User.Visible = true;
                m_pSource_TypeIMAP_User.Visible = true;
                mt_Source_TypeIMAP_Password.Visible = true;
                m_pSource_TypeIMAP_Password.Visible = true;
            }
            // ZIP Messages Archive
            else if(m_pSource_Type.SelectedIndex == 2){
                mt_Source_TypeZIP_File.Visible = true;
                m_pSource_TypeZIP_File.Visible = true;
                m_pSource_TypeZIP_FileGet.Visible = true;
            }
            //---------------------------------------------------
        }

        #endregion


        #region method m_pSource_TypeLSUser_UserGet_Click

        private void m_pSource_TypeLSUser_UserGet_Click(object sender,EventArgs e)
        {
            wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pUser.VirtualServer,false,false);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pSource_TypeLSUser_User.Text = frm.SelectedUserOrGroup;
            }
        }

        #endregion

        #region method m_pSource_TypeZIP_FileGet_Click

        private void m_pSource_TypeZIP_FileGet_Click(object sender,EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Zipped Email Archive (*.zip)|*.zip";
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pSource_TypeZIP_File.Text = dlg.FileName;
            }
        }

        #endregion

        #endregion

        #region Folders

        #region method m_pFolders_SelectAll_Click

        private void m_pFolders_SelectAll_Click(object sender,EventArgs e)
        {            
            foreach(TreeNode node in m_pFolders_Folders.Nodes){
                node.Checked = true;
            }            
        }

        #endregion

        #endregion

        #region Destination

        #region method m_pDestination_Type_SelectedIndexChanged

        private void m_pDestination_Type_SelectedIndexChanged(object sender,EventArgs e)
        {
            // Hide controls
            mt_Destination_TypeLSUser_User.Visible = false;
            m_pDestination_TypeLSUser_User.Visible = false;
            m_pDestination_TypeLSUser_UserGet.Visible = false;
            mt_Destination_TypeIMAP_Host.Visible = false;
            m_pDestination_TypeIMAP_Host.Visible = false;
            m_pDestination_TypeIMAP_Port.Visible = false;
            m_pDestination_TypeIMAP_UseSSL.Visible = false;
            mt_Destination_TypeIMAP_User.Visible = false;
            m_pDestination_TypeIMAP_User.Visible = false;
            mt_Destination_TypeIMAP_Password.Visible = false;
            m_pDestination_TypeIMAP_Password.Visible = false;
            mt_Destination_TypeZIP_File.Visible = false;
            m_pDestination_TypeZIP_File.Visible = false;
            m_pDestination_TypeZIP_FileGet.Visible = false;

            //--- Show right Type: controls ---------------------
            // LumiSoft Mail Server User
            if(m_pDestination_Type.SelectedIndex == 0){
                mt_Destination_TypeLSUser_User.Visible = true;
                m_pDestination_TypeLSUser_User.Visible = true;
                m_pDestination_TypeLSUser_UserGet.Visible = true;
            }
            // IMAP
            else if(m_pDestination_Type.SelectedIndex == 1){
                mt_Destination_TypeIMAP_Host.Visible = true;
                m_pDestination_TypeIMAP_Host.Visible = true;
                m_pDestination_TypeIMAP_Port.Visible = true;
                m_pDestination_TypeIMAP_UseSSL.Visible = true;
                mt_Destination_TypeIMAP_User.Visible = true;
                m_pDestination_TypeIMAP_User.Visible = true;
                mt_Destination_TypeIMAP_Password.Visible = true;
                m_pDestination_TypeIMAP_Password.Visible = true;
            }
            // ZIP Messages Archive
            else if(m_pDestination_Type.SelectedIndex == 2){
                mt_Destination_TypeZIP_File.Visible = true;
                m_pDestination_TypeZIP_File.Visible = true;
                m_pDestination_TypeZIP_FileGet.Visible = true;
            }
            //---------------------------------------------------
        }

        #endregion


        #region method m_pDestination_TypeLSUser_UserGet_Click

        private void m_pDestination_TypeLSUser_UserGet_Click(object sender,EventArgs e)
        {
            wfrm_se_UserOrGroup frm = new wfrm_se_UserOrGroup(m_pUser.VirtualServer,false,false);
            if(frm.ShowDialog(this) == DialogResult.OK){
                m_pDestination_TypeLSUser_User.Text = frm.SelectedUserOrGroup;
            }
        }

        #endregion

        #region method m_pDestination_TypeZIP_FileGet_Click

        private void m_pDestination_TypeZIP_FileGet_Click(object sender,EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Zipped Email Archive (*.zip)|*.zip";
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pDestination_TypeZIP_File.Text = dlg.FileName;
            }
        }

        #endregion

        #endregion


        #region method m_pFinish_Completed_DoubleClick

        private void m_pFinish_Completed_DoubleClick(object sender,EventArgs e)
        {
            if(m_pFinish_Completed.SelectedItems.Count > 0){
                List<Exception> errors = (List<Exception>)m_pFinish_Completed.SelectedItems[0].Tag;

                Form frm = new Form();
                frm.Size = new Size(400,300);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Text = "Folder: '" +  m_pFinish_Completed.SelectedItems[0].Text + "' Errors:";
                frm.Icon = ResManager.GetIcon("error.ico");
                
                TextBox textbox = new TextBox();
                textbox.Dock = DockStyle.Fill;
                textbox.Multiline = true;

                StringBuilder errorText = new StringBuilder();
                foreach(Exception x in errors){
                    errorText.Append(x.Message + "\n\n");
                }
                textbox.Lines = errorText.ToString().Split('\n');
                textbox.SelectionStart = 0;
                textbox.SelectionLength = 0;                
                
                frm.Controls.Add(textbox);

                frm.Show();
            }
        }

        #endregion


        #region method wfrm_utils_MessagesTransferer_FormClosed

        private void wfrm_utils_MessagesTransferer_FormClosed(object sender,FormClosedEventArgs e)
        {
            DisposeSource();
            DisposeDestination();
        }

        #endregion

        #region method m_pBack_Click

        private void m_pBack_Click(object sender,EventArgs e)
        {
            if(m_Step == "folders"){
                SwitchStep("source");
            }
            else if(m_Step == "destination"){
                SwitchStep("folders");
            }
            else if(m_Step == "finish"){
                SwitchStep("destination");
            }
        }

        #endregion

        #region method m_pNext_Click

        private void m_pNext_Click(object sender,EventArgs e)
        {
            if(m_Step == "source"){
                // Get folders
                try{
                    if(m_pSource_Type.SelectedIndex == -1){
                        MessageBox.Show(this,"Please select messages source !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }

                    DisposeSource();
                    m_SourceType = m_pSource_Type.SelectedIndex;
                    InitSource();

                    m_pFolders_Folders.Nodes.Clear();
                    string[] folders = GetSourceFolders();
                    foreach(string folder in folders){
                        TreeNodeCollection currentNodes = m_pFolders_Folders.Nodes;
                        string[] pathParts = folder.Split('/','\\');
                        foreach(string pathPart in pathParts){
                            bool contains = false;
                            foreach(TreeNode node in currentNodes){
                                if(node.Text == pathPart){
                                    currentNodes = node.Nodes;
                                    contains = true;
                                }
                            }
                            if(!contains){
                                TreeNode node = new TreeNode(pathPart);
                                node.ImageIndex = 0; 
                                node.Tag = folder;
                                currentNodes.Add(node);
                            }
                        }                   
                    }
                }
                catch(Exception x){
                    // We didn't get folders, show error,stay on source step.
                    MessageBox.Show(this,"Error: " + x.Message,"Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                                                                
                SwitchStep("folders");
            }
            else if(m_Step == "folders"){
                SwitchStep("destination");
            }
            else if(m_Step == "destination"){
                if(m_pDestination_Type.SelectedIndex == -1){
                    MessageBox.Show(this,"Please select messages destination !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                DisposeDestination();
                m_DestinationType = m_pDestination_Type.SelectedIndex;
                InitDestination();

                SwitchStep("finish");
            }
            else if(m_Step == "finish"){
                m_pTitle.Text = "Transfering messages ...";

                m_pBack.Enabled = false;
                m_pNext.Enabled = false;

                Thread tr = new Thread(new ThreadStart(this.Start));
                tr.Start();
            }
        }

        #endregion

        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender,EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion


        #region method SwitchStep

        /// <summary>
        /// Switchs UI wizard step.
        /// </summary>
        /// <param name="step"></param>
        private void SwitchStep(string step)
        {
            if(step == "source"){
                m_pTitle.Text = "Please select messages source.";
                                
                m_pSource.Visible = true;
                m_pFolders.Visible = false;
                m_pDestination.Visible = false;
                m_pFinish.Visible = false;
                // Common UI
                m_pBack.Enabled = false;
                m_pNext.Text = "Next";
            }
            else if(step == "folders"){
                m_pTitle.Text = "Please select folders which messages to transfer.";

                m_pSource.Visible = false;
                m_pFolders.Visible = true;
                m_pDestination.Visible = false;
                m_pFinish.Visible = false;
                // Common UI
                m_pBack.Enabled = true;
                m_pNext.Text = "Next";
            }
            else if(step == "destination"){
                m_pTitle.Text = "Please select messages destination.";

                m_pSource.Visible = false;
                m_pFolders.Visible = false;
                m_pDestination.Visible = true;
                m_pFinish.Visible = false;
                // Common UI
                m_pBack.Enabled = true;
                m_pNext.Text = "Next";
            }
            else if(step == "finish"){
                m_pTitle.Text = "Click start to begin messages transfer.";

                m_pSource.Visible = false;
                m_pFolders.Visible = false;
                m_pDestination.Visible = false;
                m_pFinish.Visible = true;
                // Common UI
                m_pNext.Text = "Start";
            }

            m_Step = step;
        }

        #endregion
                        

        #region method InitSource

        /// <summary>
        /// Intializes source object. Makes it ready to get messages info and messages.
        /// </summary>
        private void InitSource()
        {
            #region LumiSoft Mail Server User

            // LumiSoft Mail Server User
            if(m_SourceType == 0){
                m_pSourceObject = m_pUser.VirtualServer.Users.GetUserByName(m_pSource_TypeLSUser_User.Text);
            }

            #endregion

            #region IMAP

            // IMAP
            else if(m_SourceType == 1){
                IMAP_Client imap = new IMAP_Client();
                imap.Connect(m_pSource_TypeIMAP_Host.Text,(int)m_pSource_TypeIMAP_Port.Value,m_pSource_TypeIMAP_UseSSL.Checked);
                imap.Login(m_pSource_TypeIMAP_User.Text,m_pSource_TypeIMAP_Password.Text);
                m_pSourceObject = imap;
            }

            #endregion

            #region ZIP

            // ZIP
            else if(m_SourceType == 2){
                m_pSourceObject = ZipFile.OpenRead(m_pSource_TypeZIP_File.Text);
            }

            #endregion

            else{
                throw new Exception("Invalid source type '" + m_SourceType.ToString() + "' !");
            }
        }

        #endregion

        #region method DisposeSource

        /// <summary>
        /// Releases all resources useb by messages source.
        /// </summary>
        private void DisposeSource()
        {
            if(m_pSourceObject == null){
                return;
            }

            if(m_pSourceObject is IMAP_Client){
                ((IMAP_Client)m_pSourceObject).Dispose();
            }
            else if(m_pSourceObject is ZipArchive){
                ((ZipArchive)m_pSourceObject).Dispose();
            }

            m_pSourceObject = null;
        }

        #endregion

        #region method InitDestination

        /// <summary>
        /// Intializes destination object. Makes it ready to store messages messages into it.
        /// </summary>
        private void InitDestination()
        {
            #region LumiSoft Mail Server User

            // LumiSoft Mail Server User
            if(m_DestinationType == 0){
                m_pDestinationObject = m_pUser.VirtualServer.Users.GetUserByName(m_pDestination_TypeLSUser_User.Text);
            }

            #endregion

            #region IMAP

            // IMAP
            else if(m_DestinationType == 1){
                IMAP_Client imap = new IMAP_Client();
                imap.Connect(m_pDestination_TypeIMAP_Host.Text,(int)m_pDestination_TypeIMAP_Port.Value,m_pDestination_TypeIMAP_UseSSL.Checked);
                imap.Login(m_pDestination_TypeIMAP_User.Text,m_pDestination_TypeIMAP_Password.Text);
                m_pDestinationObject = imap;
            }

            #endregion

            #region ZIP

            // ZIP
            else if(m_DestinationType == 2){
                m_pDestinationObject = ZipFile.Open(m_pDestination_TypeZIP_File.Text,ZipArchiveMode.Create);
            }

            #endregion

            else{
                throw new Exception("Invalid destination type '" + m_DestinationType.ToString() + "' !");
            }
        }

        #endregion

        #region method DisposeDestination

        /// <summary>
        /// Releases all resources useb by messages destination.
        /// </summary>
        private void DisposeDestination()
        {
            if(m_pDestinationObject == null){
                return;
            }

            if(m_pDestinationObject is IMAP_Client){
                ((IMAP_Client)m_pDestinationObject).Dispose();
            }
            else if(m_pDestinationObject is ZipArchive){
                ((ZipArchive)m_pDestinationObject).Dispose();
            }

            m_pDestinationObject = null;
        }

        #endregion

        #region mehtod GetSourceFolders

        /// <summary>
        /// Gets messages source available folders.
        /// </summary>
        /// <returns></returns>
        private string[] GetSourceFolders()
        {
            #region LumiSoft Mail Server User

            // LumiSoft Mail Server User
            if(m_pSourceObject is User){
                User user = (User)m_pSourceObject;

                //--- Get folders what must be transfered -------------------------
                List<string> folders = new List<string>();

                // Add initial root sibling nodes to stack.
                Stack<Queue<UserFolder>> foldersStack = new Stack<Queue<UserFolder>>();
                Queue<UserFolder> q = new Queue<UserFolder>();
                foreach(UserFolder n in user.Folders){
                    q.Enqueue(n);
                }
                foldersStack.Push(q);

                // Process while there are not processed nodes.
                while(foldersStack.Count > 0){
                    // Get next sibling folder in current stack.
                    UserFolder folder = foldersStack.Peek().Dequeue();
                    // No more nodes, remove from processing stack.
                    if(foldersStack.Peek().Count == 0){
                        foldersStack.Pop();
                    }

                    folders.Add(folder.FolderFullPath);
                    
                    // There are sibling child folders, store to stack.
                    if(folder.ChildFolders.Count > 0){
                        Queue<UserFolder> siblingChildNodes = new Queue<UserFolder>();
                        foreach(UserFolder n in folder.ChildFolders){
                            siblingChildNodes.Enqueue(n);
                        }
                        foldersStack.Push(siblingChildNodes);
                    }
                }

                return folders.ToArray();
                //---------------------------------------------------------------
            }

            #endregion

            #region IMAP

            // IMAP
            else if(m_pSourceObject is IMAP_Client){
                IMAP_Client imap = ((IMAP_Client)m_pSourceObject);
                            
                // Make exlucde list, show local folders only.
                List<string> excludeFolders = new List<string>();
                try{
                    foreach(IMAP_r_u_Namespace namespaceInfo in imap.GetNamespaces()){
                        if(namespaceInfo.OtherUsersNamespaces != null){
                            foreach(IMAP_Namespace_Entry n in namespaceInfo.OtherUsersNamespaces){                                    
                                excludeFolders.Add(n.NamespaceName);
                            }
                        }
                        if(namespaceInfo.SharedNamespaces != null){
                            foreach(IMAP_Namespace_Entry n in namespaceInfo.SharedNamespaces){ 
                                excludeFolders.Add(n.NamespaceName);
                            }
                        }
                    }
                }
                catch{                            
                }

                List<string> retVal = new List<string>();
                foreach(IMAP_r_u_List folder in imap.GetFolders(null)){
                    // See if this folder must be excluded.
                    bool exclude = false;
                    foreach(string excludeFolder in excludeFolders){                                
                        if(folder.FolderName.ToLower().StartsWith(excludeFolder.ToLower())){                                    
                            exclude = true;
                            break;
                        }
                    }
                    if(exclude){
                        continue;
                    }

                    retVal.Add(folder.FolderName);                    
                }
                         
                return retVal.ToArray();                
            }

            #endregion

            #region ZIP

            // ZIP
            else if(m_pSourceObject is ZipArchive){
                ZipArchive zipFile = (ZipArchive)m_pSourceObject;
        
                List<string> folders = new List<string>();
                foreach(ZipArchiveEntry entry in zipFile.Entries){
                    string folder = Path.GetDirectoryName(entry.FullName);
                    if(folder.Length > 0 && !folders.Contains(folder)){
                        folders.Add(folder);
                    }
                }
                
                return folders.ToArray();
            }

            #endregion

            throw new Exception("Invalid source");
        }

        #endregion

        #region method GetSourceMessages

        /// <summary>
        /// Gets source messages ID's.
        /// </summary>
        /// <param name="folder">Folder which messages ID's to get.</param>
        /// <returns></returns>
        private string[] GetSourceMessages(string folder)
        {
            if(m_pSource == null){
                throw new Exception("Source not inited !");
            }

            #region LumiSoft Mail Server User

            // LumiSoft Mail Server User
            if(m_pSourceObject is User){
                User user = (User)m_pSourceObject;
                // Get folder from path                         
                string[] pathParts = folder.Split('/','\\');
                UserFolderCollection currentFolders = user.Folders;
                foreach(string pathPart in pathParts){                                
                    if(!currentFolders.Contains(pathPart)){
                        throw new Exception("Source folder '" + folder + "' doesn't exist !");
                    }
                    else{
                        currentFolders = currentFolders[pathPart].ChildFolders;
                    }
                }
                // Root folder            
                UserFolder sourceFolder = currentFolders.Parent;
                if(sourceFolder == null){                                
                    sourceFolder = m_pUser.Folders[folder];
                }

                List<string> retVal = new List<string>();
                DataSet ds = sourceFolder.GetMessagesInfo();
                if(ds.Tables.Contains("MessagesInfo")){
                    foreach(DataRow dr in ds.Tables["MessagesInfo"].Rows){
                        retVal.Add(dr["ID"].ToString());
                    }
                }
                return retVal.ToArray();
            }

            #endregion

            #region IMAP

            // IMAP
            else if(m_pSourceObject is IMAP_Client){
                IMAP_Client imap = ((IMAP_Client)m_pSourceObject);
                imap.SelectFolder(folder);

                List<string> retVal = new List<string>();
                IMAP_Client_FetchHandler fetchHandler = new IMAP_Client_FetchHandler();
                fetchHandler.UID += new EventHandler<EventArgs<long>>(delegate(object s,EventArgs<long> e){
                    retVal.Add(e.Value.ToString());
                });

                IMAP_SequenceSet seqSet = new IMAP_SequenceSet();
                seqSet.Parse("1:*");
                imap.Fetch(
                    false,
                    seqSet,
                    new IMAP_Fetch_DataItem[]{
                        new IMAP_Fetch_DataItem_Uid()
                    },
                    fetchHandler
                );

                return retVal.ToArray();
            }

            #endregion

            #region ZIP

            // ZIP
            else if(m_pSourceObject is ZipArchive){
                ZipArchive zipFile = (ZipArchive)m_pSourceObject;
                List<string> retVal = new List<string>();
                foreach(ZipArchiveEntry entry in zipFile.Entries){
                    if(Path.GetDirectoryName(entry.FullName).Replace("\\","/").ToLower() == folder.Replace("\\","/").ToLower() && entry.FullName.EndsWith(".eml")){
                        retVal.Add(entry.FullName);
                    }
                }
                return retVal.ToArray();
            }

            #endregion

            throw new Exception("Invalid source, never should reach here !");
        }

        #endregion

        #region method GetSourceMessage

        /// <summary>
        /// Gets source message with specified ID and stores it to storeStream.
        /// </summary>
        /// <param name="folder">Folder which message to get.</param>
        /// <param name="messageID">ID of message with to get.</param>
        /// <param name="storeStream">Stream where to store message.</param>
        private void GetSourceMessage(string folder,string messageID,Stream storeStream)
        {
            if(m_pSource == null){
                throw new Exception("Source not inited !");
            }

            #region LumiSoft Mail Server User

            // LumiSoft Mail Server User
            if(m_pSourceObject is User){
                User user = (User)m_pSourceObject;
                // Get folder from path                         
                string[] pathParts = folder.Split('/','\\');
                UserFolderCollection currentFolders = user.Folders;
                foreach(string pathPart in pathParts){                                
                    if(!currentFolders.Contains(pathPart)){
                        throw new Exception("Source folder '" + folder + "' doesn't exist !");
                    }
                    else{
                        currentFolders = currentFolders[pathPart].ChildFolders;
                    }
                }
                // Root folder            
                UserFolder sourceFolder = currentFolders.Parent;
                if(sourceFolder == null){                                
                    sourceFolder = m_pUser.Folders[folder];
                }

                sourceFolder.GetMessage(messageID,storeStream);
            }

            #endregion

            #region IMAP

            // IMAP
            else if(m_pSourceObject is IMAP_Client){
                IMAP_Client imap = ((IMAP_Client)m_pSourceObject);

                List<string> retVal = new List<string>();
                IMAP_Client_FetchHandler fetchHandler = new IMAP_Client_FetchHandler();
                fetchHandler.Rfc822 += new EventHandler<IMAP_Client_Fetch_Rfc822_EArgs>(delegate(object s,IMAP_Client_Fetch_Rfc822_EArgs e){ 
                    e.Stream = storeStream;
                    e.StoringCompleted += new EventHandler(delegate(object s1,EventArgs e1){
                    });
                });

                IMAP_SequenceSet seqSet = new IMAP_SequenceSet();
                seqSet.Parse(messageID);
                imap.Fetch(
                    true,
                    seqSet,
                    new IMAP_Fetch_DataItem[]{
                        new IMAP_Fetch_DataItem_Rfc822()
                    },
                    fetchHandler
                );
            }

            #endregion

            #region ZIP

            // ZIP
            else if(m_pSourceObject is ZipArchive){
                ZipArchive zipFile = (ZipArchive)m_pSourceObject;
                foreach(ZipArchiveEntry entry in zipFile.Entries){
                    if(entry.FullName == messageID){
                        using(Stream zipStream = entry.Open()){
                            SCore.StreamCopy(zipStream,storeStream);
                        }

                        return;
                    }
                }

                throw new Exception("Folder '" + folder + "' message with ID '" + messageID + "', not found !");
            }

            #endregion
        }

        #endregion

        #region method StoreDestinationMessage

        /// <summary>
        /// Stores specified message to specified source folder.
        /// </summary>
        /// <param name="folder">Folder where to store message.</param>
        /// <param name="messageStream">Streeam what contains message.</param>
        private void StoreDestinationMessage(string folder,Stream messageStream)
        {       
            #region LumiSoft Mail Server User

            // LumiSoft Mail Server User
            if(m_pDestinationObject is User){
                User user = (User)m_pDestinationObject;

                // Create destination folder if it doesn't exist.                          
                string[] pathParts = folder.Split('/','\\');
                UserFolderCollection currentFolders = user.Folders;
                foreach(string pathPart in pathParts){                                
                    if(!currentFolders.Contains(pathPart)){
                        currentFolders = currentFolders.Add(pathPart).ChildFolders;
                    }
                    else{
                        currentFolders = currentFolders[pathPart].ChildFolders;
                    }
                }
                // Root folder            
                UserFolder destinationFolder = currentFolders.Parent;
                if(destinationFolder == null){                                
                    destinationFolder = m_pUser.Folders[folder];
                }

                destinationFolder.StoreMessage(messageStream);
            }

            #endregion

            #region IMAP

            // IMAP
            else if(m_pDestinationObject is IMAP_Client){
                IMAP_Client imap = (IMAP_Client)m_pDestinationObject;
                try{
                    imap.CreateFolder(folder);
                }
                catch{
                    // Just skip error, probably folder already exists error
                }
                messageStream.Position = 0;
                imap.StoreMessage(folder,IMAP_MessageFlags.None,DateTime.MinValue,messageStream,(int)messageStream.Length);
            }

            #endregion

            #region ZIP

            // ZIP
            else if(m_pDestinationObject is ZipArchive){
                ZipArchive zipFile = (ZipArchive)m_pDestinationObject;

                ZipArchiveEntry entry = zipFile.CreateEntry(folder.Replace("/","\\") + "\\" + Guid.NewGuid().ToString() + ".eml",CompressionLevel.Optimal);

                using(Stream zipStream = entry.Open()){
                    SCore.StreamCopy(messageStream,zipStream);
                }
            }

            #endregion
        }

        #endregion


        #region method Start

        private void Start()
        {
            try{
                //--- Get folders what must be transfered -------------------------
                List<string> folders = new List<string>();

                // Add initial root sibling nodes to stack.
                Stack<Queue<TreeNode>> nodesStack = new Stack<Queue<TreeNode>>();
                Queue<TreeNode> q = new Queue<TreeNode>();
                foreach(TreeNode n in m_pFolders_Folders.Nodes){
                    q.Enqueue(n);
                }
                nodesStack.Push(q);
                
                // Process while there are not processed nodes.
                while(nodesStack.Count > 0){
                    // No more nodes it active stack item, remove from processing stack.
                    if(nodesStack.Peek().Count == 0){
                        nodesStack.Pop();                        
                    }
                    // Process node.
                    else{
                        // Get next sibling node in current stack.
                        TreeNode node = nodesStack.Peek().Dequeue();
                        
                        // Checked(wanted) folder.
                        if(node.Checked){
                            folders.Add(node.Tag.ToString());
                        }

                        // There are sibling child nodes, store to stack.
                        if(node.Nodes.Count > 0){
                            Queue<TreeNode> siblingChildNodes = new Queue<TreeNode>();
                            foreach(TreeNode n in node.Nodes){
                                siblingChildNodes.Enqueue(n);
                            }
                            nodesStack.Push(siblingChildNodes);
                        }
                    }
                }
                //---------------------------------------------------------------

                foreach(string folder in folders){
                    string[] messageIDs = GetSourceMessages(folder);

                    this.Invoke(new StartNewFolderDelegate(this.StartNewFolder),new object[]{folder,messageIDs.Length});

                    try{
                        foreach(string messageID in messageIDs){
                            try{
                                MemoryStream messageStream = new MemoryStream();
                                GetSourceMessage(folder,messageID,messageStream);
                                messageStream.Position = 0;
                                StoreDestinationMessage(folder,messageStream);
                            }
                            catch(Exception x){
                                this.Invoke(new AddErrorDelegate(this.AddError),new object[]{x});
                            }

                            this.Invoke(new MethodInvoker(this.IncreaseMessages));
                        }
                    }
                    catch(Exception x){
                        this.Invoke(new AddErrorDelegate(this.AddError),new object[]{x});
                    }
                }
            }
            finally{
                DisposeSource();
                DisposeDestination();
            }

            this.Invoke(new MethodInvoker(this.Finish));
        }


        //--- Threading Helper methods

        #region method StartNewFolder

        private delegate void StartNewFolderDelegate(string folderName,int messagesCount);

        private void StartNewFolder(string folderName,int messagesCount)
        {
            m_pFinish_Progress.Value = 0;
            m_pFinish_Progress.Maximum = messagesCount;

            ListViewItem item = new ListViewItem(folderName);
            item.ImageIndex = 0;
            item.Tag = new List<Exception>();
            item.SubItems.Add("0");
            item.SubItems.Add("0");
            m_pFinish_Completed.Items.Add(item);
        }

        #endregion

        #region method IncreaseMessages

        private void IncreaseMessages()
        {
            m_pFinish_Progress.Value++;
            m_pFinish_Completed.Items[m_pFinish_Completed.Items.Count - 1].SubItems[1].Text = m_pFinish_Progress.Value.ToString() + "/" + m_pFinish_Progress.Maximum.ToString();
        }

        #endregion

        #region method AddError

        private delegate void AddErrorDelegate(Exception x);

        private void AddError(Exception x)
        {
            ListViewItem currentItem = m_pFinish_Completed.Items[m_pFinish_Completed.Items.Count - 1];
            List<Exception> errors = (List<Exception>)currentItem.Tag;
            errors.Add(x);

            currentItem.SubItems[2].Text = errors.Count.ToString();
        }

        #endregion

        #region method Finish

        private void Finish()
        {
            m_pTitle.Text = "Completed.";
            m_pFinish_Progress.Value = 0;
            m_pCancel.Text = "Finish";
        }

        #endregion

        #endregion

    }
}
