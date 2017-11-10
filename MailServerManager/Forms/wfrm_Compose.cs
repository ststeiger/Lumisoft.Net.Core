using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;

using LumiSoft.MailServer.API.UserAPI;
using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Compose new user mail message UI.
    /// </summary>
    public class wfrm_Compose : Form
    {
        private Label       mt_From        = null;
        private TextBox     m_pFrom        = null;
        private Label       mt_Subject     = null;
        private TextBox     m_pSubject     = null;
        private Label       mt_Attachments = null;
        private ListView    m_pAttachments = null;
        private WRichEditEx m_pText        = null;
		private Button      m_pSend        = null;
		private Button      m_pCancel      = null;

        private UserFolder m_pFolder = null;
        private byte[]     m_Message = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_Compose(UserFolder folder)
        {	
            m_pFolder = folder;
		
            InitUI();

            m_pFrom.Text = "\"" + m_pFolder.User.VirtualServer.Server.UserName + "\" <" + m_pFolder.User.VirtualServer.Server.UserName + "@localhost>";
        }
        
        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(435,390);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Compose:";
            this.Icon = ResManager.GetIcon("write.ico");
            this.MaximizeBox = false;

            mt_From = new Label();
            mt_From.Size = new Size(78,20);
            mt_From.Location = new Point(10,10);
            mt_From.TextAlign = ContentAlignment.MiddleRight;
            mt_From.Text = "From:";

            m_pFrom = new TextBox();
            m_pFrom.Size = new Size(336,20);
            m_pFrom.Location = new Point(90,10);
            m_pFrom.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_Subject = new Label();
            mt_Subject.Size = new Size(78,20);
            mt_Subject.Location = new Point(10,60);
            mt_Subject.TextAlign = ContentAlignment.MiddleRight;
            mt_Subject.Text = "Subject:";

            m_pSubject = new TextBox();
            m_pSubject.Size = new Size(336,20);
            m_pSubject.Location = new Point(90,60);
            m_pSubject.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            mt_Attachments = new Label();
            mt_Attachments.Size = new Size(78,20);
            mt_Attachments.Location = new Point(10,85);
            mt_Attachments.TextAlign = ContentAlignment.MiddleRight;
            mt_Attachments.Text = "Attachemnts:";

            m_pAttachments = new ListView();
            m_pAttachments.Size = new Size(336,54);
            m_pAttachments.Location = new Point(90,85);
            m_pAttachments.BorderStyle = BorderStyle.FixedSingle;
            m_pAttachments.View = View.SmallIcon;
            m_pAttachments.SmallImageList = new ImageList();
            m_pAttachments.MouseUp += new MouseEventHandler(m_pAttachments_MouseUp);
            m_pAttachments.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pText = new WRichEditEx();
            m_pText.Size = new Size(416,200);
            m_pText.Location = new Point(10,150);
            m_pText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            m_pSend = new Button();
            m_pSend.Size = new Size(70,20);
            m_pSend.Location = new Point(280,360);
            m_pSend.Text = "Send";
            m_pSend.Click += new EventHandler(m_pSend_ButtonPressed);
            m_pSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(355,360);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_ButtonPressed);
            m_pCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                                    
            this.Controls.Add(mt_From);
            this.Controls.Add(m_pFrom);
            this.Controls.Add(mt_Subject);
            this.Controls.Add(m_pSubject);
            this.Controls.Add(mt_Attachments);
            this.Controls.Add(m_pAttachments);
            this.Controls.Add(m_pText);
            this.Controls.Add(m_pSend);
            this.Controls.Add(m_pCancel);            
        }
                                                                
        #endregion


        #region Events Handling

        #region method attach_Click

        private void attach_Click(object sender, EventArgs e)
        {
            AddAttachment();           
        }

        #endregion


        #region method m_pAttachments_MouseUp

        private void m_pAttachments_MouseUp(object sender, MouseEventArgs e)
        {
			//-----------------------------------------------------
			
			string open   = "Open";
			string add    = "Add"; 
			string remove = "Remove";
			//-----------------------------------------------------
			
            if(e.Button == MouseButtons.Right && m_pAttachments.SelectedItems.Count > 0){
                ContextMenuStrip menu = new ContextMenuStrip();                
                menu.Items.Add(open,ResManager.GetIcon("open.ico").ToBitmap(),this.m_pAttachments_OpenClick);
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(add,ResManager.GetIcon("add.ico").ToBitmap(),this.m_pAttachments_AddClick);
                menu.Items.Add(remove,ResManager.GetIcon("delete.ico").ToBitmap(),this.m_pAttachments_RemoveClick);
                menu.Show(Control.MousePosition);
            }
            else if(e.Button == MouseButtons.Right){
                ContextMenuStrip menu = new ContextMenuStrip();                
                menu.Items.Add(add,ResManager.GetIcon("add.ico").ToBitmap(),this.m_pAttachments_AddClick);
                menu.Show(Control.MousePosition);
            }
        }

        #endregion

        #region method m_pAttachments_OpenClick

        private void m_pAttachments_OpenClick(object sender,EventArgs e)
        {
            if(m_pAttachments.SelectedItems.Count > 0){
                System.Diagnostics.Process.Start(m_pAttachments.SelectedItems[0].Tag.ToString());
            }
        }

        #endregion

        #region method m_pAttachments_AddClick

        private void m_pAttachments_AddClick(object sender,EventArgs e)
        {
            AddAttachment();
        }

        #endregion

        #region method m_pAttachments_RemoveClick

        private void m_pAttachments_RemoveClick(object sender,EventArgs e)
        {
            foreach(ListViewItem it in m_pAttachments.SelectedItems){
                it.Remove();
            }
        }

        #endregion


        
		#region method m_pSend_ButtonPressed

		private void m_pSend_ButtonPressed(object sender,EventArgs e)
		{
            //--- Validate values ----------//
            if(m_pFrom.Text == ""){
                MessageBox.Show(this,"Please fill From: !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if(m_pSubject.Text == ""){
                MessageBox.Show(this,"Please fill Subject: !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            //-----------------------------//
                        
            m_Message = CreateMessage().ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B,Encoding.UTF8),Encoding.UTF8);            

            this.DialogResult = DialogResult.OK;
            this.Close();
		}

        #endregion

		#region method m_pCancel_ButtonPressed

		private void m_pCancel_ButtonPressed(object sender,EventArgs e)
		{
			this.Close();
		}

        #endregion

        #endregion


        #region method AddAttachment

        /// <summary>
        /// Shows file dialog and adds specified attachment to attachemnts listview.
        /// </summary>
        private void AddAttachment()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if(dlg.ShowDialog(this) == DialogResult.OK){
                AddAttachment(dlg.FileName);
            }
        }

        /// <summary>
        /// Adds specified attachment to attachemnts listview.
        /// </summary>
        private void AddAttachment(string fileName)
        {
            string fileSize = "";
            using(FileStream fs = File.OpenRead(fileName)){
                if(fs.Length > 1000000){
                    fileSize = ((decimal)(fs.Length / (decimal)1000000)).ToString("f2") + " mb";
                }
                else if(fs.Length > 1000){
                    fileSize = ((decimal)(fs.Length / (decimal)1000)).ToString("f2") + " kb";
                }
                else{
                    fileSize = fs.Length.ToString() + " bytes";
                }
            }

            m_pAttachments.SmallImageList.Images.Add(GetFileIcon(fileName).ToBitmap());

            ListViewItem it = new ListViewItem(Path.GetFileName(fileName) + " (" + fileSize + ")");
            it.ImageIndex = m_pAttachments.SmallImageList.Images.Count - 1;
            it.Tag = fileName;
            m_pAttachments.Items.Add(it);            
        }

        #endregion

        #region method GetFileIcon

        /// <summary>
		/// Returns an icon for a given file - indicated by the name parameter.
		/// </summary>
		/// <param name="name">Pathname for file.</param>
		/// <returns>System.Drawing.Icon</returns>
		private Icon GetFileIcon(string name)
		{
            try{
			    Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
			    uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES | Shell32.SHGFI_SMALLICON;

			    Shell32.SHGetFileInfo(name, 
				    Shell32.FILE_ATTRIBUTE_NORMAL, 
				    ref shfi, 
				    (uint) System.Runtime.InteropServices.Marshal.SizeOf(shfi), 
				    flags );

			    // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
			    System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
			    User32.DestroyIcon( shfi.hIcon );		// Cleanup
			    return icon;
            }
            catch{
                return ResManager.GetIcon("attach");
            }
		}

        private class Shell32  
	    {
    		
		    public const int 	MAX_PATH = 256;
		    [StructLayout(LayoutKind.Sequential)]
			    public struct SHITEMID
		    {
			    public ushort cb;
			    [MarshalAs(UnmanagedType.LPArray)]
			    public byte[] abID;
		    }

		    [StructLayout(LayoutKind.Sequential)]
			    public struct ITEMIDLIST
		    {
			    public SHITEMID mkid;
		    }

		    [StructLayout(LayoutKind.Sequential)]
			    public struct BROWSEINFO 
		    { 
			    public IntPtr		hwndOwner; 
			    public IntPtr		pidlRoot; 
			    public IntPtr 		pszDisplayName;
			    [MarshalAs(UnmanagedType.LPTStr)] 
			    public string 		lpszTitle; 
			    public uint 		ulFlags; 
			    public IntPtr		lpfn; 
			    public int			lParam; 
			    public IntPtr 		iImage; 
		    } 

		    // Browsing for directory.
		    public const uint BIF_RETURNONLYFSDIRS   =	0x0001;
		    public const uint BIF_DONTGOBELOWDOMAIN  =	0x0002;
		    public const uint BIF_STATUSTEXT         =	0x0004;
		    public const uint BIF_RETURNFSANCESTORS  =	0x0008;
		    public const uint BIF_EDITBOX            =	0x0010;
		    public const uint BIF_VALIDATE           =	0x0020;
		    public const uint BIF_NEWDIALOGSTYLE     =	0x0040;
		    public const uint BIF_USENEWUI           =	(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
		    public const uint BIF_BROWSEINCLUDEURLS  =	0x0080;
		    public const uint BIF_BROWSEFORCOMPUTER  =	0x1000;
		    public const uint BIF_BROWSEFORPRINTER   =	0x2000;
		    public const uint BIF_BROWSEINCLUDEFILES =	0x4000;
		    public const uint BIF_SHAREABLE          =	0x8000;

		    [StructLayout(LayoutKind.Sequential)]
			    public struct SHFILEINFO
		    { 
			    public const int NAMESIZE = 80;
			    public IntPtr	hIcon; 
			    public int		iIcon; 
			    public uint	dwAttributes; 
			    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
			    public string szDisplayName; 
			    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=NAMESIZE)]
			    public string szTypeName; 
		    };

		    public const uint SHGFI_ICON				= 0x000000100;     // get icon
		    public const uint SHGFI_DISPLAYNAME			= 0x000000200;     // get display name
		    public const uint SHGFI_TYPENAME          	= 0x000000400;     // get type name
		    public const uint SHGFI_ATTRIBUTES        	= 0x000000800;     // get attributes
		    public const uint SHGFI_ICONLOCATION      	= 0x000001000;     // get icon location
		    public const uint SHGFI_EXETYPE           	= 0x000002000;     // return exe type
		    public const uint SHGFI_SYSICONINDEX      	= 0x000004000;     // get system icon index
		    public const uint SHGFI_LINKOVERLAY       	= 0x000008000;     // put a link overlay on icon
		    public const uint SHGFI_SELECTED          	= 0x000010000;     // show icon in selected state
		    public const uint SHGFI_ATTR_SPECIFIED    	= 0x000020000;     // get only specified attributes
		    public const uint SHGFI_LARGEICON         	= 0x000000000;     // get large icon
		    public const uint SHGFI_SMALLICON         	= 0x000000001;     // get small icon
		    public const uint SHGFI_OPENICON          	= 0x000000002;     // get open icon
		    public const uint SHGFI_SHELLICONSIZE     	= 0x000000004;     // get shell size icon
		    public const uint SHGFI_PIDL              	= 0x000000008;     // pszPath is a pidl
		    public const uint SHGFI_USEFILEATTRIBUTES 	= 0x000000010;     // use passed dwFileAttribute
		    public const uint SHGFI_ADDOVERLAYS       	= 0x000000020;     // apply the appropriate overlays
		    public const uint SHGFI_OVERLAYINDEX      	= 0x000000040;     // Get the index of the overlay

		    public const uint FILE_ATTRIBUTE_DIRECTORY  = 0x00000010;  
		    public const uint FILE_ATTRIBUTE_NORMAL     = 0x00000080;  

		    [DllImport("Shell32.dll")]
		    public static extern IntPtr SHGetFileInfo(
			    string pszPath,
			    uint dwFileAttributes,
			    ref SHFILEINFO psfi,
			    uint cbFileInfo,
			    uint uFlags
			    );
	    }

	    /// <summary>
	    /// Wraps necessary functions imported from User32.dll. Code courtesy of MSDN Cold Rooster Consulting example.
	    /// </summary>
	    private class User32
	    {
		    /// <summary>
		    /// Provides access to function required to delete handle. This method is used internally
		    /// and is not required to be called separately.
		    /// </summary>
		    /// <param name="hIcon">Pointer to icon handle.</param>
		    /// <returns>N/A</returns>
		    [DllImport("User32.dll")]
		    public static extern int DestroyIcon( IntPtr hIcon );
        }

        #endregion

        #region method CreateMessage

        /// <summary>
        /// Creates Mime message based on UI data.
        /// </summary>
        private Mail_Message CreateMessage()
        {
            Mail_Message msg = new Mail_Message();
            msg.MimeVersion = "1.0";
            msg.MessageID = MIME_Utils.CreateMessageID();
            msg.Date = DateTime.Now;
            msg.From = Mail_h_MailboxList.Parse("From: " + m_pFrom.Text).Addresses;
            msg.To = new Mail_t_AddressList();
            msg.To.Add(new Mail_t_Mailbox(m_pFolder.User.FullName,m_pFolder.User.FullName + "@localhost"));
            msg.Subject = m_pSubject.Text;

            //--- multipart/mixed -------------------------------------------------------------------------------------------------
            MIME_h_ContentType contentType_multipartMixed = new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed);
            contentType_multipartMixed.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
            MIME_b_MultipartMixed multipartMixed = new MIME_b_MultipartMixed(contentType_multipartMixed);
            msg.Body = multipartMixed;

                //--- multipart/alternative -----------------------------------------------------------------------------------------
                MIME_Entity entity_multipartAlternative = new MIME_Entity();
                MIME_h_ContentType contentType_multipartAlternative = new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative);
                contentType_multipartAlternative.Param_Boundary = Guid.NewGuid().ToString().Replace('-','.');
                MIME_b_MultipartAlternative multipartAlternative = new MIME_b_MultipartAlternative(contentType_multipartAlternative);
                entity_multipartAlternative.Body = multipartAlternative;
                multipartMixed.BodyParts.Add(entity_multipartAlternative);

                    //--- text/plain ----------------------------------------------------------------------------------------------------
                    MIME_Entity entity_text_plain = new MIME_Entity();
                    MIME_b_Text text_plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
                    entity_text_plain.Body = text_plain;
                    text_plain.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,m_pText.Text);
                    multipartAlternative.BodyParts.Add(entity_text_plain);

                    //--- text/html ------------------------------------------------------------------------------------------------------
                    MIME_Entity entity_text_html = new MIME_Entity();
                    MIME_b_Text text_html = new MIME_b_Text(MIME_MediaTypes.Text.html);
                    entity_text_html.Body = text_html;
                    text_html.SetText(MIME_TransferEncodings.QuotedPrintable,Encoding.UTF8,RtfToHtml());
                    multipartAlternative.BodyParts.Add(entity_text_html);
            
            //--- application/octet-stream -----------------------------------------------------------------------------------------------
            foreach(ListViewItem item in m_pAttachments.Items){
                multipartMixed.BodyParts.Add(Mail_Message.CreateAttachment(item.Tag.ToString()));
            }

            return msg;
        }

        #endregion

        #region method RtfToHtml

        /// <summary>
        /// Converts RTF text to HTML text.
        /// </summary>
        /// <returns></returns>
        private string RtfToHtml()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append("<html>\r\n");

            m_pText.SuspendPaint = true;
            // Go to text start.
            m_pText.RichTextBox.SelectionStart  = 0;
            m_pText.RichTextBox.SelectionLength = 1;

            Font  currentFont           = m_pText.RichTextBox.SelectionFont;
            Color currentSelectionColor = m_pText.RichTextBox.SelectionColor;
            Color currentBackColor      = m_pText.RichTextBox.SelectionBackColor;

            int numberOfSpans = 0;
            int startPos = 0;
            while(m_pText.RichTextBox.Text.Length > m_pText.RichTextBox.SelectionStart){  
                m_pText.RichTextBox.SelectionStart++;
                m_pText.RichTextBox.SelectionLength = 1;
                                
                // Font style or size or color or back color changed
                if(m_pText.RichTextBox.Text.Length == m_pText.RichTextBox.SelectionStart || (currentFont.Name != m_pText.RichTextBox.SelectionFont.Name || currentFont.Size != m_pText.RichTextBox.SelectionFont.Size || currentFont.Style != m_pText.RichTextBox.SelectionFont.Style || currentSelectionColor != m_pText.RichTextBox.SelectionColor || currentBackColor != m_pText.RichTextBox.SelectionBackColor)){
                    string currentTextBlock = m_pText.RichTextBox.Text.Substring(startPos,m_pText.RichTextBox.SelectionStart - startPos);
       
                    //--- Construct text bloxh html -----------------------------------------------------------------//
                    // Make colors to html color syntax: #hex(r)hex(g)hex(b)
                    string htmlSelectionColor = "#" + currentSelectionColor.R.ToString("X2") + currentSelectionColor.G.ToString("X2") + currentSelectionColor.B.ToString("X2");
                    string htmlBackColor      = "#" + currentBackColor.R.ToString("X2") + currentBackColor.G.ToString("X2") + currentBackColor.B.ToString("X2");
                    string textStyleStartTags = "";
                    string textStyleEndTags   = "";
                    if(currentFont.Bold){
                        textStyleStartTags += "<b>";
                        textStyleEndTags   += "</b>";
                    }
                    if(currentFont.Italic){
                        textStyleStartTags += "<i>";
                        textStyleEndTags   += "</i>";
                    }
                    if(currentFont.Underline){
                        textStyleStartTags += "<u>";
                        textStyleEndTags   += "</u>";
                    }           
                    retVal.Append("<span\n style=\"color:" + htmlSelectionColor + "; font-size:" + currentFont.Size + "pt; font-family:" + currentFont.FontFamily.Name + "; background-color:" + htmlBackColor + ";\">" + textStyleStartTags + currentTextBlock.Replace("\n","</br>") + textStyleEndTags);
                    //-----------------------------------------------------------------------------------------------//

                    startPos              = m_pText.RichTextBox.SelectionStart;
                    currentFont           = m_pText.RichTextBox.SelectionFont;
                    currentSelectionColor = m_pText.RichTextBox.SelectionColor;
                    currentBackColor      = m_pText.RichTextBox.SelectionBackColor;
                    numberOfSpans++;
                }
            }

            for(int i=0;i<numberOfSpans;i++){
                retVal.Append("</span>");
            }

            retVal.Append("\r\n</html>\r\n");
            m_pText.SuspendPaint = false;

            return retVal.ToString();
        }

        #endregion
                        

        #region Properties Implementation

        /// <summary>
        /// Gets composed message.
        /// </summary>
        public byte[] Message
        {
            get{ return m_Message; }
        }

        #endregion

    }
}
