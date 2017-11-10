using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{    
    /// <summary>
    /// Extended richedit control.
    /// </summary>
    public class WRichEditEx : UserControl
    {
        #region class RickTextBoxEx

        /// <summary>
        /// Extened rich textbox, add suspend paint property.
        /// </summary>
        private class RickTextBoxEx : RichTextBox
        {
            private bool m_SuspendPaint = false;

            /// <summary>
            /// Default constructor.
            /// </summary>
            public RickTextBoxEx()
            {
            }

            


            #region method override WndProc

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // WM_PAINT = 0x00f
            if(m.Msg == 0x00f){
                if(m_SuspendPaint){
                    m.Result = IntPtr.Zero;
                }
                else{
                    base.WndProc(ref m);
                }
            }
            else{
                base.WndProc(ref m);
            }
        }

        #endregion


            #region Properties Implementation

        /// <summary>
        /// Gets or sets if painting is suspended.
        /// </summary>
        public bool SuspendPaint
        {
            get{ return m_SuspendPaint; }

            set{ 
                m_SuspendPaint = value; 

                if(!value){
                    this.Refresh();
                }
            }
        }

        #endregion
        }

        #endregion

        private ToolStrip     m_pToolbar = null;
        private RickTextBoxEx m_pTextbox = null;
        private bool          m_Lock     = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WRichEditEx()
        {
            InitUI();


            SetSelectionFont();
            
            Font newFont = (Font)m_pTextbox.SelectionFont.Clone();
            
            m_pTextbox.Font = (Font)newFont.Clone();
            m_pTextbox.SelectionFont = (Font)newFont.Clone();
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(100,100);

            m_pToolbar = new ToolStrip();            
            m_pToolbar.GripStyle = ToolStripGripStyle.Hidden;
            m_pToolbar.BackColor = SystemColors.Control;
            m_pToolbar.Renderer = new ToolBarRendererEx();
            // Toolbar item font
            ToolStripComboBox font = new ToolStripComboBox();
            font.Size = new Size(150,20);
            font.DropDownStyle = ComboBoxStyle.DropDownList;
            font.SelectedIndexChanged += new EventHandler(font_SelectedIndexChanged);
//            foreach(FontFamily fontFamily in FontFamily.Families){
//                font.Items.Add(fontFamily.Name);
//            }
			font.Items.Add("Arial");
			font.Items.Add("Courier New");
			font.Items.Add("Times New Roman");
			font.Items.Add("Verdana");
            if(font.Items.Count > 0){
                font.SelectedIndex = 0;
            }
            m_pToolbar.Items.Add(font);
            // Toolbar item font size
            ToolStripComboBox fontSize = new ToolStripComboBox();
            fontSize.AutoSize = false;
            fontSize.Size = new Size(50,20);
            fontSize.DropDownStyle = ComboBoxStyle.DropDownList;
            fontSize.Items.Add("8");
            fontSize.Items.Add("10");
            fontSize.Items.Add("12");
            fontSize.Items.Add("14");
            fontSize.Items.Add("18");
            fontSize.Items.Add("24");
            fontSize.Items.Add("32");
            fontSize.SelectedIndex = 1;
            fontSize.SelectedIndexChanged += new EventHandler(fontSize_SelectedIndexChanged);
            m_pToolbar.Items.Add(fontSize);
            m_pToolbar.Items.Add(new ToolStripSeparator());
            // Toolbar item bold
            ToolStripButton bold = new ToolStripButton();
            bold.Image = ResManager.GetIcon("bold.ico").ToBitmap();
            bold.Click += new EventHandler(bold_Click);
            m_pToolbar.Items.Add(bold);
            // Toolbar item italic
            ToolStripButton italic = new ToolStripButton();
            italic.Image = ResManager.GetIcon("italic.ico").ToBitmap();
            italic.Click += new EventHandler(italic_Click);
            m_pToolbar.Items.Add(italic);
            // Toolbar item underline
            ToolStripButton underline = new ToolStripButton();            
            underline.Image = ResManager.GetIcon("underline.ico").ToBitmap();
            underline.Click += new EventHandler(underline_Click);
            m_pToolbar.Items.Add(underline);
            // Separator
            m_pToolbar.Items.Add(new ToolStripSeparator());
            // Toolbar item font color
            ToolStripButton fontColor = new ToolStripButton();            
            fontColor.Image = CreateFontColorIcon(Color.Black);
            fontColor.Click += new EventHandler(fontColor_Click);
            m_pToolbar.Items.Add(fontColor);
            // Toolbar item font background color
            ToolStripButton fontBackColor = new ToolStripButton();            
            fontBackColor.Image = CreateFontBackColorIcon(Color.White);
            fontBackColor.Click += new EventHandler(fontBackColor_Click);
            m_pToolbar.Items.Add(fontBackColor);

            m_pTextbox = new RickTextBoxEx();
            m_pTextbox.Size = new Size(97,73);
            m_pTextbox.Location = new Point(1,25);
            m_pTextbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pTextbox.BorderStyle = BorderStyle.None;
            m_pTextbox.HideSelection = false;
            m_pTextbox.SelectionChanged += new EventHandler(m_pTextbox_SelectionChanged);

            this.Controls.Add(m_pToolbar);
            this.Controls.Add(m_pTextbox);
        }
                                                                                                                                
        #endregion


        #region Events Handling

		#region method SetText

		/// <summary>
		/// Set text.
		/// </summary>
		/// <param name="text"></param>
		public void SetText(string text)
		{
            m_pTextbox.AppendText(text);
		}
		
		#endregion


        #region method font_SelectedIndexChanged

        private void font_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pToolbar.Items.Count == 0){
                return;
            }

			SetSelectionFont();
        }

        #endregion

        #region method fontSize_SelectedIndexChanged

        private void fontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_pToolbar.Items.Count == 0){
                return;
            }

             SetSelectionFont();
         }

        #endregion

        #region method bold_Click

        private void bold_Click(object sender, EventArgs e)
        {
            if(((ToolStripButton)m_pToolbar.Items[3]).Checked){
                ((ToolStripButton)m_pToolbar.Items[3]).Checked = false;
            }
            else{
                ((ToolStripButton)m_pToolbar.Items[3]).Checked = true;
            }

            SetSelectionFont();
        }

        #endregion

        #region method italic_Click

        private void italic_Click(object sender, EventArgs e)
        {
            if(((ToolStripButton)m_pToolbar.Items[4]).Checked){
                ((ToolStripButton)m_pToolbar.Items[4]).Checked = false;
            }
            else{
                ((ToolStripButton)m_pToolbar.Items[4]).Checked = true;                
            }

            SetSelectionFont();
        }

        #endregion

        #region method underline_Click

        private void underline_Click(object sender, EventArgs e)
        {
            if(((ToolStripButton)m_pToolbar.Items[5]).Checked){
                ((ToolStripButton)m_pToolbar.Items[5]).Checked = false;
            }
            else{
                ((ToolStripButton)m_pToolbar.Items[5]).Checked = true;                
            }

            SetSelectionFont();
        }

        #endregion

        #region method fontColor_Click

        private void fontColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pTextbox.SelectionColor = dlg.Color;

                ((ToolStripButton)m_pToolbar.Items[7]).Image = CreateFontColorIcon(dlg.Color);
            }
        }

        #endregion

        #region method fontBackColor_Click

        private void fontBackColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if(dlg.ShowDialog(this) == DialogResult.OK){
                m_pTextbox.SelectionBackColor = dlg.Color;

                ((ToolStripButton)m_pToolbar.Items[8]).Image = CreateFontBackColorIcon(dlg.Color);                
            }
        }

        #endregion


        #region method m_pTextbox_SelectionChanged

        private void m_pTextbox_SelectionChanged(object sender,EventArgs e)
        {
			if(m_pTextbox.SelectionFont == null){
				return;
			}
			
			m_Lock = true;
			
            ((ToolStripComboBox)m_pToolbar.Items[0]).Text  = m_pTextbox.SelectionFont.Name;
            ((ToolStripComboBox)m_pToolbar.Items[1]).Text  = m_pTextbox.SelectionFont.Size.ToString();
            ((ToolStripButton)m_pToolbar.Items[3]).Checked = m_pTextbox.SelectionFont.Bold;
            ((ToolStripButton)m_pToolbar.Items[4]).Checked = m_pTextbox.SelectionFont.Italic;
            ((ToolStripButton)m_pToolbar.Items[5]).Checked = m_pTextbox.SelectionFont.Underline;
            ((ToolStripButton)m_pToolbar.Items[7]).Image   = CreateFontColorIcon(m_pTextbox.SelectionColor);
            ((ToolStripButton)m_pToolbar.Items[8]).Image   = CreateFontBackColorIcon(m_pTextbox.SelectionBackColor);
            
            m_Lock = false;
        }

        #endregion

        #endregion


        #region method SetSelectionFont

        /// <summary>
        /// Sets selected text properties.
        /// </summary>
        private void SetSelectionFont()
        {
			if(m_Lock){
				return;
			}
			
            FontStyle style = FontStyle.Regular;

            if(((ToolStripButton)m_pToolbar.Items[3]).Checked){
                style |= FontStyle.Bold;
            }
            if(((ToolStripButton)m_pToolbar.Items[4]).Checked){
                style |= FontStyle.Italic;
            }
            if(((ToolStripButton)m_pToolbar.Items[5]).Checked){
                style |= FontStyle.Underline;
            }
            
            m_pTextbox.SelectionFont = new Font(((ToolStripComboBox)m_pToolbar.Items[0]).Text,Convert.ToInt32(((ToolStripComboBox)m_pToolbar.Items[1]).Text),style);
            m_pTextbox.Focus();
        }

        #endregion

        #region method CreateFontColorIcon

        /// <summary>
        /// Creates font color icon for specified color.
        /// </summary>
        /// <param name="color">Color for what to create icon.</param>
        /// <returns></returns>
        private Bitmap CreateFontColorIcon(Color color)
        {
            Bitmap bmp = ResManager.GetIcon("fontcolor.ico").ToBitmap();
            for(int x=0;x<bmp.Width;x++){
                for(int y=12;y<bmp.Height;y++){
                   bmp.SetPixel(x,y,color);
                }
            }

            return bmp;
        }

        #endregion

        #region method CreateFontBackColorIcon

        /// <summary>
        /// Creates font back ground color icon for specified color.
        /// </summary>
        /// <param name="color">Color for what to create icon.</param>
        /// <returns></returns>
        private Bitmap CreateFontBackColorIcon(Color color)
        {
            Bitmap bmp = ResManager.GetIcon("fontbackcolor.ico").ToBitmap();
            for(int x=0;x<bmp.Width;x++){
                for(int y=12;y<bmp.Height;y++){
                   bmp.SetPixel(x,y,color);
                }
            }

            return bmp;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets or sets the current text in textbox.
        /// </summary>
        public new string Text
        {
            get{ return m_pTextbox.Text; }

            set{ m_pTextbox.Text = value; }
        }

        /// <summary>
        /// Gets or sets the current RTF in textbox.
        /// </summary>
        public string Rtf
        {
            get{ return m_pTextbox.Rtf; }

            set{ m_pTextbox.Rtf = value; }
        }

        /// <summary>
        /// Gets reference to subclassed RichTextBox.
        /// </summary>
        internal RichTextBox RichTextBox
        {
            get{ return m_pTextbox; }
        }

        /// <summary>
        /// Gets or sets if painting is suspended.
        /// </summary>
        internal bool SuspendPaint
        {
            get{ return m_pTextbox.SuspendPaint; }

            set{ m_pTextbox.SuspendPaint = value; }
        }

        #endregion

    }
}
