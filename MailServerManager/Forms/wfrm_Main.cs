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
    /// Application main UI window.
    /// </summary>
    public class wfrm_Main : Form
    {
        #region enum NodeType

        /// <summary>
        /// Specifies treenode type.
        /// </summary>
	    private enum NodeType
	    {
		    Dummy,
		    LocalMachine,
		    LocalMachineMonitoring,
            Monitoring_SipRegistrations,
            Monitoring_SipCalls,
		    VirtualServers,
		    Server,
		    System,
            System_General,
            System_Authentication,
            System_Services,
            System_Services_SMTP,
            System_Services_POP3,
            System_Services_IMAP,
            System_Services_Relay,
            System_Services_FetchPOP3,
            System_Services_SIP,
            System_Logging,
            System_Backup,
            System_ReturnMessages,
		    Domains,
            UsersAndGroups,
		    UsersAndGroups_Users,
            UsersAndGroups_Groups,
		    MailingLists,
            Rules,
            Rules_Message_Global,
		    Routing,
		    Security,
		    Filters, 
            SharedFolders_Users,
            SharedFolders_RootFolders,
            Folders_UsersDefaultFolders,
            Folders_RecycleBin,
            Queues_IncomingSMTP,
            Queues_OutgoingSMTP,
            EventsAndLogs_Events,
            EventsAndLogs_Logs,
	    }

	    #endregion

        #region class TreeNodeTag

        /// <summary>
        /// Provides user data for tree node.
        /// </summary>
        private class TreeNodeTag
        {
            private TreeNode m_pNode    = null;
            private Server   m_pServer  = null;
            private NodeType m_NodeType = NodeType.Dummy;
            private object   m_Tag      = null;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="server">Mail server object.</param>
            /// <param name="type">Treenode type.</param>
            /// <param name="tag">User data.</param>
            public TreeNodeTag(Server server,NodeType type,object tag)
            {
                m_pServer  = server;
                m_NodeType = type;
                m_Tag      = tag;
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="node">Tree node that owns this object.</param>
            /// <param name="server">Mail server object.</param>
            /// <param name="type">Treenode type.</param>
            /// <param name="tag">User data.</param>
            public TreeNodeTag(TreeNode node,Server server,NodeType type,object tag)
            {
                m_pNode    = node;
                m_pServer  = server;
                m_NodeType = type;
                m_Tag      = tag;
            }


            #region Properties Implementation

            /// <summary>
            /// Gets tree node that owns this object.
            /// </summary>
            public TreeNode OwnerNode
            {
                get{ return m_pNode; }
            }

            /// <summary>
            /// Gets mail server oject.
            /// </summary>
            public Server Server
            {
                get{ return m_pServer; }
            }

            /// <summary>
            /// Gets treenode type.
            /// </summary>
            public NodeType TreeNodeType
            {
                get{ return m_NodeType; }
            }

            /// <summary>
            /// Gets or sets user data.
            /// </summary>
            public object Tag
            {
                get{ return m_Tag; }

                set{ m_Tag = value; }
            }

            #endregion
        }

        #endregion

        private MenuStrip m_pMenu  = null;
        private WFrame    m_pFrame = null;
        private TreeView  m_pTree  = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_Main()
        {
            InitUI();

            LoadServers();
        }        

        #region method Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        { 
            base.Dispose(disposing); 
           
            try{
                m_pFrame.Frame_Form = null;
                m_pFrame.Dispose();
            }
            catch{
            }
        }

        #endregion

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(760,610);
            this.MinimumSize = new Size(780,650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "LumiSoft Mail Server Manager";
            this.Icon = ResManager.GetIcon("server.ico");

            m_pMenu = new MenuStrip();
            m_pMenu.BackColor = this.BackColor;
            // File menu
            m_pMenu.Items.Add("File");
            ToolStripMenuItem file_Connect = new ToolStripMenuItem("Connect");
            file_Connect.Tag = "file_connect";
            file_Connect.Image = ResManager.GetIcon("connect.ico").ToBitmap();
            file_Connect.Click += new EventHandler(file_Connect_Click);
            ((ToolStripMenuItem)m_pMenu.Items[0]).DropDownItems.Add(file_Connect);
            ((ToolStripMenuItem)m_pMenu.Items[0]).DropDownItems.Add("-");
            ToolStripMenuItem file_Exit = new ToolStripMenuItem("Exit");
            file_Exit.Tag = "file_exit";
            file_Exit.Image = ResManager.GetIcon("exit.ico").ToBitmap();
            file_Exit.Click += new EventHandler(file_Exit_Click);
            ((ToolStripMenuItem)m_pMenu.Items[0]).DropDownItems.Add(file_Exit);
            // About menu
            m_pMenu.Items.Add("Help");
            ToolStripMenuItem file_Help = new ToolStripMenuItem("Help");
            file_Help.Image = ResManager.GetIcon("help.ico").ToBitmap();
            ((ToolStripMenuItem)m_pMenu.Items[1]).DropDownItems.Add(file_Help);
            ((ToolStripMenuItem)m_pMenu.Items[1]).DropDownItems.Add("-");
            ToolStripMenuItem help_Forum = new ToolStripMenuItem("Forum");
            help_Forum.Tag = "help_forum";
            help_Forum.Image = ResManager.GetIcon("forum.ico").ToBitmap();
            help_Forum.Click += new EventHandler(help_Forum_Click);
            ((ToolStripMenuItem)m_pMenu.Items[1]).DropDownItems.Add(help_Forum);
            ((ToolStripMenuItem)m_pMenu.Items[1]).DropDownItems.Add("-");
            ToolStripMenuItem help_About = new ToolStripMenuItem("About");
            help_About.Tag = "help_about";
            help_About.Image = ResManager.GetIcon("about.ico").ToBitmap();
            help_About.Click += new EventHandler(help_About_Click);
            ((ToolStripMenuItem)m_pMenu.Items[1]).DropDownItems.Add(help_About);

            m_pFrame = new WFrame();
            m_pFrame.Size = new Size(764,589);
            m_pFrame.Location = new Point(0,22);
            m_pFrame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pFrame.ControlPaneWidth = 230;

            ImageList treeImageList = new ImageList();
            treeImageList.Images.Add(ResManager.GetIcon("server.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("servers.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("services.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("domain.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("user.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("mailinglist.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("system.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("acl.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("filter.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("messagerule.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("rootfolder.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("folders.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("queue.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("logging.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("services.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("system.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("authentication.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("backup.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("server_running.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("server_stopped.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("recyclebin.ico"));
            treeImageList.Images.Add(ResManager.GetIcon("message16.ico"));

            m_pTree = new TreeView();
            m_pTree.BorderStyle = BorderStyle.None;
			m_pTree.HideSelection = false;
			m_pTree.HotTracking = true;
			m_pTree.ImageList = treeImageList;
            m_pTree.DoubleClick += new EventHandler(m_pTree_DoubleClick);
            m_pTree.AfterSelect += new TreeViewEventHandler(m_pTree_AfterSelect);
            m_pTree.MouseUp += new MouseEventHandler(m_pTree_MouseUp);
            m_pFrame.Frame_BarControl = m_pTree;

            this.Controls.Add(m_pFrame);
            this.Controls.Add(m_pMenu);
        }
                                                                                                                                
        #endregion


        #region Events Handling

        #region method m_pMenu_ItemClicked

        #region method file_Connect_Click

        private void file_Connect_Click(object sender, EventArgs e)
        {
            wfrm_ConnectToServer frm = new wfrm_ConnectToServer();
            if(frm.ShowDialog(this) == DialogResult.OK){
                string serverID = "";         

                // Save connection to XML
                if(frm.SaveConnection && frm.Host != ""){
                    serverID = Guid.NewGuid().ToString();

                    // Ensure that settings folder exists
                    if(!Directory.Exists(Application.StartupPath + "/Settings")){
                        Directory.CreateDirectory(Application.StartupPath + "/Settings");
                    }

                    DataSet ds = LoadRegisteredServers();
                    DataRow dr = ds.Tables["Servers"].NewRow();
                    dr["ID"]       = serverID;
                    dr["Host"]     = frm.Host;
                    dr["UserName"] = frm.UserName;
                    dr["Password"] = frm.Password;
                    ds.Tables["Servers"].Rows.Add(dr);
                    ds.WriteXml(Application.StartupPath + "/Settings/managerServers.xml");
                }

                LoadServer(frm.Server,serverID);
            }
        }

        #endregion

        #region method file_Exit_Click

        private void file_Exit_Click(object sender, EventArgs e)
        {
            this.Close();

            // TODO: Close all active user API connections
        }

        #endregion


        #region method help_Forum_Click

        private void help_Forum_Click(object sender, EventArgs e)
        {
            if(Environment.OSVersion.Platform != PlatformID.Unix){
                System.Diagnostics.Process.Start("explorer","http://www.lumisoft.ee/Forum");
            }
            else{
                // TODO: Make this works with default browser
                try{
                    System.Diagnostics.Process.Start("firefox","http://www.lumisoft.ee/Forum");
                }
                catch{
                }
            }
        }

        #endregion

        #region method help_About_Click

        private void help_About_Click(object sender, EventArgs e)
        {
            wfrm_About frm = new wfrm_About();
            frm.ShowDialog(this);
        }

        #endregion

        #endregion


        #region method m_pTree_DoubleClick

        private void m_pTree_DoubleClick(object sender,EventArgs e)
        {
            if(m_pTree.SelectedNode == null){
                return;
            }

            // If root node and server not connected yet, connect
            if(m_pTree.SelectedNode.Parent == null && m_pTree.SelectedNode.Nodes.Count == 0){
                TreeNodeTag nodeTag = (TreeNodeTag)m_pTree.SelectedNode.Tag;
                DataRow dr = (DataRow)nodeTag.Tag;
                try{
                    nodeTag.Server.Connect(dr["Host"].ToString(),dr["UserName"].ToString(),dr["Password"].ToString());
                    LoadServer(m_pTree.SelectedNode,nodeTag.Server,dr["ID"].ToString());
                }
                // Show connect UI
                catch{
                    wfrm_ConnectToServer frm = new wfrm_ConnectToServer(
                        dr["Host"].ToString(),
                        dr["UserName"].ToString(),
                        dr["Password"].ToString(),
                        false
                    );
                    if(frm.ShowDialog(this) == DialogResult.OK){
                        // Save new connection info
                        DataSet ds = LoadRegisteredServers();
                        // Find server with specified ID
                        foreach(DataRow drServer in ds.Tables["Servers"].Rows){                            
                            if(drServer["ID"].ToString() == dr["ID"].ToString()){
                                drServer["Host"]     = frm.Host;
                                drServer["UserName"] = frm.UserName;
                                drServer["Password"] = frm.Password;
                                ds.WriteXml(Application.StartupPath + "/Settings/managerServers.xml");
                                break;
                            }
                        }

                        LoadServer(m_pTree.SelectedNode,frm.Server,dr["ID"].ToString());
                    }
                    else{                    
                        return;
                    }
                }
            }
        }

        #endregion

        #region method m_pTree_AfterSelect

        private void m_pTree_AfterSelect(object sender,TreeViewEventArgs e)
        {
            m_pFrame.FormFrameBorder = BorderStyle.FixedSingle;
            m_pFrame.Frame_ToolStrip = null;
            if(e.Node == null || e.Node.Tag == null){
                m_pFrame.Frame_Form = new Form();
				return;
			}
            // If root node and server not connected yet, skip it
            if(e.Node.Parent == null && e.Node.Nodes.Count == 0){
                return;
            }

            TreeNodeTag nodeTag = (TreeNodeTag)e.Node.Tag;  
            if(nodeTag.TreeNodeType == NodeType.Dummy){
                m_pFrame.Frame_Form = new Form();
            }
            else if(nodeTag.TreeNodeType == NodeType.Server){
                m_pFrame.Frame_Form = new wfrm_Server_Info(nodeTag.Server);
            }
            else if(nodeTag.TreeNodeType == NodeType.LocalMachineMonitoring){
                m_pFrame.Frame_Form = new wfrm_Monitoring_Sessions(nodeTag.Server);
            }
            else if(nodeTag.TreeNodeType == NodeType.Monitoring_SipRegistrations){
                m_pFrame.Frame_Form = new wfrm_Monitoring_SipRegistrations(nodeTag.Server,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.Monitoring_SipCalls){
                m_pFrame.Frame_Form = new wfrm_Monitoring_SIP_Calls(nodeTag.Server,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.EventsAndLogs_Events){
                m_pFrame.Frame_Form = new wfrm_EventsAndLogs_Events(nodeTag.Server,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.EventsAndLogs_Logs){
                m_pFrame.Frame_Form = new wfrm_EventsAndLogs_Logs(nodeTag.Server,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.VirtualServers){
                m_pFrame.Frame_Form = new wfrm_VirtualServers(this,nodeTag.OwnerNode,nodeTag.Server,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_General){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_General((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Authentication){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Authentication((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Services_SMTP){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Services_SMTP((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Services_POP3){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Services_POP3((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Services_IMAP){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Services_IMAP((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Services_Relay){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Services_Relay((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Services_FetchPOP3){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Services_FetchPOP3((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Services_SIP){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Services_SIP((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Logging){
                m_pFrame.FormFrameBorder = BorderStyle.None;
                m_pFrame.Frame_Form = new wfrm_System_Logging((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_Backup){
                m_pFrame.Frame_Form = new wfrm_System_Backup((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.System_ReturnMessages){
                m_pFrame.Frame_Form = new wfrm_System_ReturnMessages((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.Domains){
                m_pFrame.Frame_Form = new wfrm_Domains((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.UsersAndGroups_Users){
                m_pFrame.Frame_Form = new wfrm_UsersAndGroups_Users((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.UsersAndGroups_Groups){
                m_pFrame.Frame_Form = new wfrm_UsersAndGroups_Groups((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.MailingLists){
                m_pFrame.Frame_Form = new wfrm_MailingLists((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.Routing){
                m_pFrame.Frame_Form = new wfrm_Routing_Routes((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.Rules_Message_Global){
                m_pFrame.Frame_Form = new wfrm_GlobalMessageRules((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.SharedFolders_RootFolders){
                m_pFrame.Frame_Form = new wfrm_SharedFolders_RootFolders((VirtualServer)nodeTag.Tag,m_pFrame);
            }/*
            else if(nodeTag.TreeNodeType == NodeType.SharedFolders_Users){
                m_pFrame.Frame_Form = new wfrm_SharedFolders_UserFolders((VirtualServer)nodeTag.Tag,m_pFrame);
            }*/
            else if(nodeTag.TreeNodeType == NodeType.Folders_UsersDefaultFolders){
                m_pFrame.Frame_Form = new wfrm_Folders_UsersDefaultFolders((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.Folders_RecycleBin){
                m_pFrame.Frame_Form = new wfrm_Folders_RecycleBin((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.Filters){
                m_pFrame.Frame_Form = new wfrm_Filters((VirtualServer)nodeTag.Tag,m_pFrame);
            }
            else if(nodeTag.TreeNodeType == NodeType.Queues_IncomingSMTP){
                m_pFrame.Frame_Form = new wfrm_Queues_IncomingSMTP((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.Queues_OutgoingSMTP){
                m_pFrame.Frame_Form = new wfrm_Queues_OutgoingSMTP((VirtualServer)nodeTag.Tag);
            }
            else if(nodeTag.TreeNodeType == NodeType.Security){
                m_pFrame.Frame_Form = new wfrm_Security_IPSecurity((VirtualServer)nodeTag.Tag,m_pFrame);
            }
        }

        #endregion

        #region method m_pTree_MouseUp

        private void m_pTree_MouseUp(object sender, MouseEventArgs e)
        {
            if(m_pTree.SelectedNode == null || e.Button != MouseButtons.Right){
                return;
            }
            
            // If root node and registered server, show delete menu
            if(m_pTree.SelectedNode.Parent == null && m_pTree.SelectedNode.Tag != null){
                TreeNodeTag nodeTag = (TreeNodeTag)m_pTree.SelectedNode.Tag;
                if(nodeTag.Tag.GetType() == typeof(string) && nodeTag.Tag.ToString().Length > 0){;
                    /* Mono no support for ContextMenuStrip
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripMenuItem deleteServer = new ToolStripMenuItem("Delete");
                    deleteServer.Tag = m_pTree.SelectedNode;
                    deleteServer.Click += new EventHandler(deleteServer_Click);
                    menu.Items.Add(deleteServer);
                    menu.Show(m_pTree.PointToScreen(e.Location));*/

                    // Don't show this menu now.
                }
            }
        }


        #region method deleteServer_Click

        private void deleteServer_Click(object sender,EventArgs e)
        {
            TreeNode    node    = ((TreeNode)((ToolStripMenuItem)sender).Tag);
            TreeNodeTag nodeTag = (TreeNodeTag)m_pTree.SelectedNode.Tag;
            if(MessageBox.Show(this,"Are you sure you want to remove server '" + node.Text + "' from UI ?","Confirm delete:",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes){                
                // Save new connection info
                DataSet ds = LoadRegisteredServers();
                // Find server with specified ID                
                foreach(DataRow drServer in ds.Tables["Servers"].Rows){                            
                    if(drServer["ID"].ToString() == nodeTag.Tag.ToString()){
                        drServer.Delete();
                        ds.WriteXml(Application.StartupPath + "/Settings/managerServers.xml");
                        break;
                    }
                }
                nodeTag.Server.Disconnect();
                node.Remove();
            }
        }

        #endregion

        #endregion

        #endregion


        #region method LoadRegisteredServers

        /// <summary>
        /// Loads registered servers from XML.
        /// </summary>
        /// <returns></returns>
        private DataSet LoadRegisteredServers()
        {
            DataSet ds = new DataSet("dsRegisteredServers");
            ds.Tables.Add("Servers");
            ds.Tables["Servers"].Columns.Add("ID");
            ds.Tables["Servers"].Columns.Add("Host");
            ds.Tables["Servers"].Columns.Add("UserName");
            ds.Tables["Servers"].Columns.Add("Password");

            if(File.Exists(Application.StartupPath + "/Settings/managerServers.xml")){
                ds.ReadXml(Application.StartupPath + "/Settings/managerServers.xml");
            }

            return ds;
        }

        #endregion

        #region method LoadServers

        /// <summary>
        /// Loads registered mail servers list to UI treeview.
        /// </summary>
        private void LoadServers()
        {            
            foreach(DataRow dr in LoadRegisteredServers().Tables["Servers"].Rows){
                // Create machine root tree node
                TreeNode node_machine = new TreeNode(dr["Host"].ToString());
                node_machine.Tag = new TreeNodeTag(new Server(),NodeType.Dummy,dr);
                m_pTree.Nodes.Add(node_machine);
            }            
        }

        #endregion

        #region method LoadServer

        /// <summary>
        /// Loads specified mail server to UI treeview.
        /// </summary>
        /// <param name="server">Server to load.</param>
        /// <param name="serverID">Server ID.</param>
        private void LoadServer(Server server,string serverID)
        {
            LoadServer(null,server,serverID);
        }

        /// <summary>
        /// Loads specified mail server to UI treeview.
        /// </summary>
        /// <param name="serverNode">Node to where to load server noders.</param>
        /// <param name="server">Server to load.</param>
        /// <param name="serverID">Server ID.</param>
        private void LoadServer(TreeNode serverNode,Server server,string serverID)
        {
            /* Tree node struct:
                machineName
                    Virtual Servers
                        ...
            */

            TreeNode node_machine = serverNode;
            if(serverNode == null){
                // Create machine root tree node
                node_machine = new TreeNode(server.Host);            
                node_machine.Tag = new TreeNodeTag(server,NodeType.Server,serverID);
                m_pTree.Nodes.Add(node_machine);
            }
            else{
                node_machine.Tag = new TreeNodeTag(server,NodeType.Server,serverID);
            }
                TreeNode node_Monitoring = new TreeNode("Monitoring",0,0);
				//node_Monitoring.Tag = new NodeData(NodeType.LocalMachineMonitoring);
				node_machine.Nodes.Add(node_Monitoring);
                    
                    TreeNode node_Monitoring_Sessions = new TreeNode("Sessions",0,0);
				    node_Monitoring_Sessions.Tag = new TreeNodeTag(server,NodeType.LocalMachineMonitoring,null);
				    node_Monitoring.Nodes.Add(node_Monitoring_Sessions);

                    TreeNode node_Monitoring_SIP = new TreeNode("SIP",0,0);
				    //node_Monitoring_SIP.Tag = new TreeNodeTag(server,NodeType.LocalMachineMonitoring,null);
				    node_Monitoring.Nodes.Add(node_Monitoring_SIP);

                        TreeNode node_Monitoring_SIP_Registrations = new TreeNode("Registrations",0,0);
				        node_Monitoring_SIP_Registrations.Tag = new TreeNodeTag(server,NodeType.Monitoring_SipRegistrations,null);
				        node_Monitoring_SIP.Nodes.Add(node_Monitoring_SIP_Registrations);

                        TreeNode node_Monitoring_SIP_Calls = new TreeNode("Calls",0,0);
				        node_Monitoring_SIP_Calls.Tag = new TreeNodeTag(server,NodeType.Monitoring_SipCalls,null);
				        node_Monitoring_SIP.Nodes.Add(node_Monitoring_SIP_Calls);

                TreeNode node_LogsAndEvents = new TreeNode("Logs and Events",13,13);
				//node_LogsAndEvents.Tag = new NodeData(NodeType.Dummy);
				node_machine.Nodes.Add(node_LogsAndEvents);

                    TreeNode node_LogsAndEvents_Events = new TreeNode("Events",12,12);
				    node_LogsAndEvents_Events.Tag = new TreeNodeTag(server,NodeType.EventsAndLogs_Events,null);
				    node_LogsAndEvents.Nodes.Add(node_LogsAndEvents_Events);

                    TreeNode node_LogsAndEvents_Logs = new TreeNode("Logs",13,13);
				    node_LogsAndEvents_Logs.Tag = new TreeNodeTag(server,NodeType.EventsAndLogs_Logs,null);
				    node_LogsAndEvents.Nodes.Add(node_LogsAndEvents_Logs);

                // Create machine node virtual servers
                TreeNode node_virtualServers = new TreeNode("Virtual Servers",1,1);
                node_virtualServers.Tag = new TreeNodeTag(node_virtualServers,server,NodeType.VirtualServers,null);
                node_machine.Nodes.Add(node_virtualServers);
                    LoadVirtualServers(node_virtualServers,server);
                    
        }

        #endregion

        #region method LoadVirtualServers

        /// <summary>
        /// Loads virtual servers to specified tree node.
        /// </summary>
        /// <param name="virtualServersNode">Tree node where to load virtual servers.</param>
        /// <param name="server">Server what virtual servers to load.</param>
        internal void LoadVirtualServers(TreeNode virtualServersNode,Server server)
        {
            virtualServersNode.Nodes.Clear();

            // Add all virtual servers
            foreach(VirtualServer vServer in server.VirtualServers){
                TreeNode node_virtualServers_vServer = new TreeNode(vServer.Name,0,0);
                if(vServer.Enabled){
                    node_virtualServers_vServer.ImageIndex = 18;
                    node_virtualServers_vServer.SelectedImageIndex = 18;
                }
                else{
                    node_virtualServers_vServer.ImageIndex = 19;
                    node_virtualServers_vServer.SelectedImageIndex = 19;
                }
                virtualServersNode.Nodes.Add(node_virtualServers_vServer);
                    TreeNode node_Sys = new TreeNode("System",15,15);
				    //node_Sys.Tag = new NodeData(NodeType.System,null);
				    node_virtualServers_vServer.Nodes.Add(node_Sys);

                        TreeNode node_General = new TreeNode("General",15,15);
				        node_General.Tag = new TreeNodeTag(server,NodeType.System_General,vServer);
				        node_Sys.Nodes.Add(node_General);

                        TreeNode node_Authentication = new TreeNode("Authentication",16,16);
				        node_Authentication.Tag = new TreeNodeTag(server,NodeType.System_Authentication,vServer);
				        node_Sys.Nodes.Add(node_Authentication);

                        TreeNode node_Services = new TreeNode("Services",14,14);
				        //node_Services.Tag = new NodeData(NodeType.System_Services,null);
				        node_Sys.Nodes.Add(node_Services);

                            TreeNode node_Services_SMTP = new TreeNode("SMTP",14,14);
				            node_Services_SMTP.Tag = new TreeNodeTag(server,NodeType.System_Services_SMTP,vServer);
				            node_Services.Nodes.Add(node_Services_SMTP);

                            TreeNode node_Services_POP3 = new TreeNode("POP3",14,14);
				            node_Services_POP3.Tag = new TreeNodeTag(server,NodeType.System_Services_POP3,vServer);
				            node_Services.Nodes.Add(node_Services_POP3);

                            TreeNode node_Services_IMAP = new TreeNode("IMAP",14,14);
				            node_Services_IMAP.Tag = new TreeNodeTag(server,NodeType.System_Services_IMAP,vServer);
				            node_Services.Nodes.Add(node_Services_IMAP);

                            TreeNode node_Services_Relay = new TreeNode("Relay",14,14);
				            node_Services_Relay.Tag = new TreeNodeTag(server,NodeType.System_Services_Relay,vServer);
				            node_Services.Nodes.Add(node_Services_Relay);

                            TreeNode node_Services_FetchPOP3 = new TreeNode("Fetch POP3",14,14);
				            node_Services_FetchPOP3.Tag = new TreeNodeTag(server,NodeType.System_Services_FetchPOP3,vServer);
				            node_Services.Nodes.Add(node_Services_FetchPOP3);

                            TreeNode node_Services_SIP = new TreeNode("SIP",14,14);
				            node_Services_SIP.Tag = new TreeNodeTag(server,NodeType.System_Services_SIP,vServer);
				            node_Services.Nodes.Add(node_Services_SIP);

                        TreeNode node_Sys_Logging = new TreeNode("Logging",13,13);
				        node_Sys_Logging.Tag = new TreeNodeTag(server,NodeType.System_Logging,vServer);
				        node_Sys.Nodes.Add(node_Sys_Logging);

                        TreeNode node_Sys_Backup = new TreeNode("Backup",17,17);
				        node_Sys_Backup.Tag = new TreeNodeTag(server,NodeType.System_Backup,vServer);
				        node_Sys.Nodes.Add(node_Sys_Backup);

                        TreeNode node_Sys_ReturnMessages = new TreeNode("Return Messages",21,21);
				        node_Sys_ReturnMessages.Tag = new TreeNodeTag(server,NodeType.System_ReturnMessages,vServer);
				        node_Sys.Nodes.Add(node_Sys_ReturnMessages);

                    TreeNode node_Domains = new TreeNode("Domains",3,3);
				    node_Domains.Tag = new TreeNodeTag(server,NodeType.Domains,vServer);
				    node_virtualServers_vServer.Nodes.Add(node_Domains);

				    TreeNode node_UsersAndGroups = new TreeNode("Users and Groups",4,4);
				    //node_UsersAndGroups.Tag = new NodeData(NodeType.Dummy,api);
				    node_virtualServers_vServer.Nodes.Add(node_UsersAndGroups);

                        TreeNode node_UsersAndGroups_Users = new TreeNode("Users",4,4);
				        node_UsersAndGroups_Users.Tag = new TreeNodeTag(server,NodeType.UsersAndGroups_Users,vServer);
				        node_UsersAndGroups.Nodes.Add(node_UsersAndGroups_Users);

                        TreeNode node_UsersAndGroups_Groups = new TreeNode("Groups",5,5);
				        node_UsersAndGroups_Groups.Tag = new TreeNodeTag(server,NodeType.UsersAndGroups_Groups,vServer);
				        node_UsersAndGroups.Nodes.Add(node_UsersAndGroups_Groups);

                    TreeNode node_Rules = new TreeNode("Rules",9,9);
				    //node_Rules.Tag = new NodeData(NodeType.Rules,api);
				    node_virtualServers_vServer.Nodes.Add(node_Rules);

                    TreeNode node_MessageRules = new TreeNode("Message Rules",9,9);
				    //node_MessageRules.Tag = new NodeData(NodeType.Dummy,api);
				    node_Rules.Nodes.Add(node_MessageRules);
                        
                        TreeNode node_GlobalMessageRules = new TreeNode("Global",9,9);
				        node_GlobalMessageRules.Tag = new TreeNodeTag(server,NodeType.Rules_Message_Global,vServer);
				        node_MessageRules.Nodes.Add(node_GlobalMessageRules);
                        /*
                        TreeNode node_UserMessageRules = new TreeNode("User",8,8);
				        node_UserMessageRules.Tag = new NodeData(NodeType.Rules,api);
				        node_MessageRules.Nodes.Add(node_UserMessageRules);
                            */

				    TreeNode node_MailingLists = new TreeNode("Mailing Lists",5,5);
				    node_MailingLists.Tag = new TreeNodeTag(server,NodeType.MailingLists,vServer);
				    node_virtualServers_vServer.Nodes.Add(node_MailingLists);

				    TreeNode node_Routing = new TreeNode("Routing",6,6);
				    node_Routing.Tag = new TreeNodeTag(server,NodeType.Routing,vServer);
				    node_virtualServers_vServer.Nodes.Add(node_Routing);

                    TreeNode node_FoldersManagement = new TreeNode("Folders Management",11,11);
				    //node_FoldersManagement.Tag = new NodeData(NodeType.Dummy,api);
				    node_virtualServers_vServer.Nodes.Add(node_FoldersManagement);

                        TreeNode node_FoldersManagement_RootFolders = new TreeNode("Shared Root Folders",10,10);
				        node_FoldersManagement_RootFolders.Tag = new TreeNodeTag(server,NodeType.SharedFolders_RootFolders,vServer);
				        node_FoldersManagement.Nodes.Add(node_FoldersManagement_RootFolders);

                        //TreeNode node_FoldersManagement_MangeUserFolders = new TreeNode("Manage User Folders",7,7);
				        //node_FoldersManagement_MangeUserFolders.Tag = new TreeNodeTag(server,NodeType.SharedFolders_Users,vServer);
				        //node_FoldersManagement.Nodes.Add(node_FoldersManagement_MangeUserFolders);

                        TreeNode node_FoldersManagement_UsersDefaultFolders = new TreeNode("Users Default Folders",11,11);
				        node_FoldersManagement_UsersDefaultFolders.Tag = new TreeNodeTag(server,NodeType.Folders_UsersDefaultFolders,vServer);
				        node_FoldersManagement.Nodes.Add(node_FoldersManagement_UsersDefaultFolders);

                        TreeNode node_FoldersManagement_RecycelBin = new TreeNode("Recycle Bin",20,20);
				        node_FoldersManagement_RecycelBin.Tag = new TreeNodeTag(server,NodeType.Folders_RecycleBin,vServer);
				        node_FoldersManagement.Nodes.Add(node_FoldersManagement_RecycelBin);
                        
                    TreeNode node_Queues = new TreeNode("Queues",12,12);
				    //node_Queues.Tag = new NodeData(NodeType.Dummy,api);
				    node_virtualServers_vServer.Nodes.Add(node_Queues);
                        
                        TreeNode node_Queues_IncomingSmtp = new TreeNode("Incoming SMTP",12,12);
				        node_Queues_IncomingSmtp.Tag = new TreeNodeTag(server,NodeType.Queues_IncomingSMTP,vServer);
				        node_Queues.Nodes.Add(node_Queues_IncomingSmtp);
                        
                        TreeNode node_Queues_OutgoingSmtp = new TreeNode("Outgoing SMTP",12,12);
				        node_Queues_OutgoingSmtp.Tag = new TreeNodeTag(server,NodeType.Queues_OutgoingSMTP,vServer);
				        node_Queues.Nodes.Add(node_Queues_OutgoingSmtp);
                        
				    TreeNode node_Security = new TreeNode("Security",7,7);
				    node_Security.Tag = new TreeNodeTag(server,NodeType.Security,vServer);
				    node_virtualServers_vServer.Nodes.Add(node_Security);

				    TreeNode node_Filters = new TreeNode("Filters",8,8);
				    node_Filters.Tag = new TreeNodeTag(server,NodeType.Filters,vServer);
				    node_virtualServers_vServer.Nodes.Add(node_Filters);
                                        
                        /*
                    TreeNode node_Utils = new TreeNode("Utils",8,8);
				    node_Utils.Tag = new NodeData(NodeType.Dummy,api);
				    node_Server.Nodes.Add(node_Utils);

                       TreeNode node_DnsQuery = new TreeNode("Dns Query",8,8);
				       node_DnsQuery.Tag = new NodeData(NodeType.Dummy,api);
				       node_Utils.Nodes.Add(node_DnsQuery);

                       TreeNode node_SmtpStress = new TreeNode("Smtp Stress",8,8);
				       node_SmtpStress.Tag = new NodeData(NodeType.Dummy,api);
				       node_Utils.Nodes.Add(node_SmtpStress);*/
            }
        }

        #endregion

    }
}
