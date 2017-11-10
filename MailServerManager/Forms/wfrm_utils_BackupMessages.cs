using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Virtual server messages backup wizard.
    /// </summary>
    public class wfrm_utils_BackupMessages : Form
    {
        private PictureBox m_pIcon       = null;
        private Label      m_pTitle      = null;
        private GroupBox   m_pSeparator1 = null;
        private GroupBox   m_pSeparator2 = null;
        private Button     m_pBack       = null;
        private Button     m_pNext       = null;
        private Button     m_pCancel     = null;
        //--- Select Destination Folder
        private Panel   m_pDestination           = null;
        private Label   mt_Destination_Folder    = null;
        private TextBox m_pDestionation_Folder   = null;
        private Button  m_pDestination_GetFolder = null;
        //--- Select Users
        private Panel     m_pUsers           = null;
        private Button    m_pUsers_SelectAll = null;
        private WTreeView m_pUsers_Users     = null;
        //--- Finish
        private Panel       m_pFinish               = null;
        private ProgressBar m_pFinish_ProgressTotal = null;
        private ProgressBar m_pFinish_Progress      = null;
        private ListView    m_pFinish_Completed     = null;

        private VirtualServer m_pVirtualServer = null;
        private string        m_Step           = "";
        private string        m_Folder         = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Reference to virtual server.</param>       
        public wfrm_utils_BackupMessages(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;

            InitUI();

            SwitchStep("destination");
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(500,400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Virtual Server Messages Backup Wizard";
            this.Icon = ResManager.GetIcon("ruleaction.ico");

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

            #region Destination

            m_pDestination = new Panel();
            m_pDestination.Size = new Size(500,300);
            m_pDestination.Location = new Point(0,75);
            m_pDestination.Visible = false;

            mt_Destination_Folder = new Label();
            mt_Destination_Folder.Size = new Size(100,20);
            mt_Destination_Folder.Location = new Point(0,0);
            mt_Destination_Folder.TextAlign = ContentAlignment.MiddleRight;
            mt_Destination_Folder.Text = "Folder:";

            m_pDestionation_Folder = new TextBox();
            m_pDestionation_Folder.Size = new Size(270,20);
            m_pDestionation_Folder.Location = new Point(105,0);
            m_pDestionation_Folder.ReadOnly = true;

            m_pDestination_GetFolder = new Button();
            m_pDestination_GetFolder.Size = new Size(25,20);
            m_pDestination_GetFolder.Location = new Point(380,0);
            m_pDestination_GetFolder.Text = "...";
            m_pDestination_GetFolder.Click += new EventHandler(m_pDestination_GetFolder_Click);

            m_pDestination.Controls.Add(mt_Destination_Folder);
            m_pDestination.Controls.Add(m_pDestionation_Folder);
            m_pDestination.Controls.Add(m_pDestination_GetFolder);

            this.Controls.Add(m_pDestination);

            #endregion

            #region Users
  
            m_pUsers = new Panel();
            m_pUsers.Size = new Size(500,300);
            m_pUsers.Location = new Point(0,50);
            m_pUsers.Visible = false;

            m_pUsers_SelectAll = new Button();
            m_pUsers_SelectAll.Size = new Size(70,20);
            m_pUsers_SelectAll.Location = new Point(10,0);
            m_pUsers_SelectAll.Text = "Select All";
            m_pUsers_SelectAll.Click += new EventHandler(m_pUsers_SelectAll_Click);

            ImageList imageList_m_pUsers_Users = new ImageList();
            imageList_m_pUsers_Users.Images.Add(ResManager.GetIcon("user.ico"));

            m_pUsers_Users = new WTreeView();
            m_pUsers_Users.Size = new Size(480,265);
            m_pUsers_Users.Location = new Point(10,25);
            m_pUsers_Users.CheckBoxes = true;
            m_pUsers_Users.ImageList = imageList_m_pUsers_Users;

            m_pUsers.Controls.Add(m_pUsers_SelectAll);
            m_pUsers.Controls.Add(m_pUsers_Users);

            this.Controls.Add(m_pUsers);

            #endregion

            #region Finish

            m_pFinish = new Panel();
            m_pFinish.Size = new Size(500,300);
            m_pFinish.Location = new Point(0,50);
            m_pFinish.Visible = false;

            m_pFinish_ProgressTotal = new ProgressBar();
            m_pFinish_ProgressTotal.Size = new Size(480,20);
            m_pFinish_ProgressTotal.Location = new Point(10,0);

            m_pFinish_Progress = new ProgressBar();
            m_pFinish_Progress.Size = new Size(480,20);
            m_pFinish_Progress.Location = new Point(10,25);

            ImageList imgList_m_pFinish_Completed = new ImageList();
            imgList_m_pFinish_Completed.Images.Add(ResManager.GetIcon("folder.ico"));

            m_pFinish_Completed = new ListView();
            m_pFinish_Completed.Size = new Size(480,250);
            m_pFinish_Completed.Location = new Point(10,50);
            m_pFinish_Completed.View = View.Details;
            m_pFinish_Completed.FullRowSelect = true;
            m_pFinish_Completed.HideSelection = false;
            m_pFinish_Completed.SmallImageList = imgList_m_pFinish_Completed;
            m_pFinish_Completed.Columns.Add("Folder",340);
            m_pFinish_Completed.Columns.Add("Count",70);
            m_pFinish_Completed.Columns.Add("Errors",50);
            m_pFinish_Completed.DoubleClick += new EventHandler(m_pFinish_Completed_DoubleClick);

            m_pFinish.Controls.Add(m_pFinish_ProgressTotal);
            m_pFinish.Controls.Add(m_pFinish_Progress);
            m_pFinish.Controls.Add(m_pFinish_Completed);

            this.Controls.Add(m_pFinish);

            #endregion
        }
                                                                                               
        #endregion


        #region Events Handling

        #region method m_pDestination_GetFolder_Click

        private void m_pDestination_GetFolder_Click(object sender,EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pDestionation_Folder.Text = dlg.SelectedPath;
            }
        }

        #endregion


        #region method m_pUsers_SelectAll_Click

        private void m_pUsers_SelectAll_Click(object sender,EventArgs e)
        {
            foreach(TreeNode node in m_pUsers_Users.Nodes){
                node.Checked = true;
            }
        }

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


        #region method m_pBack_Click

        private void m_pBack_Click(object sender,EventArgs e)
        {
            if(m_Step == "users"){
                SwitchStep("destination");
            }
            else if(m_Step == "finish"){
                SwitchStep("users");
            }
        }

        #endregion

        #region method m_pNext_Click

        private void m_pNext_Click(object sender,EventArgs e)
        {
            if(m_Step == "destination"){
                if(m_pDestionation_Folder.Text == ""){
                    MessageBox.Show(this,"Please select backup destination !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                m_Folder = m_pDestionation_Folder.Text;

                foreach(User user in m_pVirtualServer.Users){
                    TreeNode node = new TreeNode(user.UserName);
                    node.ImageIndex = 0; 
                    node.Tag = user;
                    m_pUsers_Users.Nodes.Add(node);
                }

                SwitchStep("users");
            }
            else if(m_Step == "users"){
                SwitchStep("finish");
            }
            else if(m_Step == "finish"){
                m_pTitle.Text = "Backingup messages ...";

                m_pBack.Enabled = false;
                m_pNext.Enabled = false;

                System.Threading.Thread tr = new System.Threading.Thread(new System.Threading.ThreadStart(this.Start));
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
            if(step == "destination"){
                m_pTitle.Text = "Please select backup location.";

                m_pDestination.Visible = true;
                m_pUsers.Visible = false;
                m_pFinish.Visible = false;
                // Common UI
                m_pBack.Enabled = false;
                m_pNext.Enabled = true;
                m_pCancel.Enabled = true;
            }
            else if(step == "users"){
                m_pTitle.Text = "Please select user who's messages to backup.";

                m_pDestination.Visible = false;
                m_pUsers.Visible = true;
                m_pFinish.Visible = false;
                // Common UI
                m_pBack.Enabled = true;
                m_pNext.Enabled = true;
                m_pCancel.Enabled = true;
            }
            else if(step == "finish"){
                m_pTitle.Text = "Click start to begin.";

                m_pDestination.Visible = false;
                m_pUsers.Visible = false;
                m_pFinish.Visible = true;
                // Common UI
                m_pBack.Enabled = true;
                m_pNext.Enabled = true;
                m_pCancel.Enabled = true;

                m_pNext.Text = "Start";
            }

            m_Step = step;
        }

        #endregion


        #region method Start

        /// <summary>
        /// Starts backingup.
        /// </summary>
        private void Start()
        {   
            List<User> users = new List<User>();
            foreach(TreeNode node in m_pUsers_Users.Nodes){
                if(node.Checked){
                    users.Add((User)node.Tag);
                }
            }
     
            // Set total progress max value
            this.Invoke(new SetTotalProgressDelegate(this.SetTotalProgress),new object[]{users.Count});

            foreach(User user in users){                
                //--- Get folders what must be transfered -------------------------
                List<UserFolder> folders = new List<UserFolder>();

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

                    folders.Add(folder);
                        
                    // There are sibling child folders, store to stack.
                    if(folder.ChildFolders.Count > 0){
                        Queue<UserFolder> siblingChildNodes = new Queue<UserFolder>();
                        foreach(UserFolder n in folder.ChildFolders){
                            siblingChildNodes.Enqueue(n);
                        }
                        foldersStack.Push(siblingChildNodes);
                    }
                }
                //---------------------------------------------------------------
                                        
                try{
                    // Create ZIP file
                    ZipArchive zipFile = ZipFile.Open(m_Folder + "/" + user.UserName + ".zip",ZipArchiveMode.Create);

                    // Loop user folders
                    foreach(UserFolder folder in folders){
                        DataSet dsMessages = folder.GetMessagesInfo();

                        if(!dsMessages.Tables.Contains("MessagesInfo")){
                            dsMessages.Tables.Add("MessagesInfo");
                        }
                            
                        this.Invoke(new StartNewFolderDelegate(this.StartNewFolder),new object[]{user.UserName + "/" + folder.FolderFullPath,dsMessages.Tables["MessagesInfo"].Rows.Count});

                        try{    
                            // Loop user folder messages
                            foreach(DataRow dr in dsMessages.Tables["MessagesInfo"].Rows){
                                try{
                                    // Add new file to zip
                                    ZipArchiveEntry entry = zipFile.CreateEntry(folder.FolderFullPath.Replace("/","\\") + "\\" + Guid.NewGuid().ToString() + ".eml",CompressionLevel.Optimal);
                               
                                    using(Stream zipStream = entry.Open()){
                                        folder.GetMessage(dr["ID"].ToString(),zipStream);
                                    }
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

                    zipFile.Dispose();
                }
                catch(Exception x){
                    this.Invoke(new AddErrorDelegate(this.AddError),new object[]{x});
                }

                this.Invoke(new MethodInvoker(this.IncreaseTotal));
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
            item.EnsureVisible();
        }

        #endregion

        #region method SetTotalProgress

        private delegate void SetTotalProgressDelegate(int maxValue);

        private void SetTotalProgress(int maxValue)
        {
             m_pFinish_ProgressTotal.Maximum = maxValue;
        }

        #endregion

        #region method IncreaseTotal

        private void IncreaseTotal()
        {
            m_pFinish_ProgressTotal.Value++;
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
            if(m_pFinish_Completed.Items.Count == 0){
                MessageBox.Show("Error:" + x.ToString(),"Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else{
                ListViewItem currentItem = m_pFinish_Completed.Items[m_pFinish_Completed.Items.Count - 1];
                List<Exception> errors = (List<Exception>)currentItem.Tag;
                errors.Add(x);

                currentItem.SubItems[2].Text = errors.Count.ToString();
            }           
        }

        #endregion

        #region method Finish

        private void Finish()
        {
            m_pTitle.Text = "Completed.";
            m_pFinish_ProgressTotal.Value = 0;
            m_pFinish_Progress.Value = 0;
            m_pCancel.Text = "Finish";
        }

        #endregion

        #endregion

    }
}
