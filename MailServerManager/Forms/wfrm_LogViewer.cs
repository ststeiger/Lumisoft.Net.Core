using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.Net;
using LumiSoft.MailServer.UI.Resources;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Session log viewer window.
    /// </summary>
    public class wfrm_LogViewer : Form
    {
        private Label       mt_SessionStartTime = null;
        private TextBox     m_pSessionStartTime = null;
        private Label       mt_UserName         = null;
        private TextBox     m_pUserName         = null;
        private Label       mt_SessionID        = null;
        private TextBox     m_pSessionID        = null;
        private Label       mt_RemoteEndPoint   = null;
        private TextBox     m_pRemoteEndPoint   = null;
        private RichTextBox m_pLogText          = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logText">Log text.</param>
        /// <param name="highlightWord">Word that will be highlighted in log text.</param>
        public wfrm_LogViewer(string logText,string highlightWord)
        {
            InitUI();

            LoadLogText(logText,highlightWord);
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes window UI.
        /// </summary>
        private void InitUI()
        {
            this.Size = new Size(500,400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Log Viewer";
            this.Icon = ResManager.GetIcon("logging.ico");

            mt_SessionStartTime = new Label();
            mt_SessionStartTime.Size = new Size(120,20);
            mt_SessionStartTime.Location = new Point(0,20);
            mt_SessionStartTime.TextAlign = ContentAlignment.MiddleRight;
            mt_SessionStartTime.Text = "Session Start:";

            m_pSessionStartTime = new TextBox();
            m_pSessionStartTime.Size = new Size(250,20);
            m_pSessionStartTime.Location = new Point(125,20);
            m_pSessionStartTime.ReadOnly = true;

            mt_UserName = new Label();
            mt_UserName.Size = new Size(120,20);
            mt_UserName.Location = new Point(0,45);
            mt_UserName.TextAlign = ContentAlignment.MiddleRight;
            mt_UserName.Text = "Authenticated User:";

            m_pUserName = new TextBox();
            m_pUserName.Size = new Size(250,20);
            m_pUserName.Location = new Point(125,45);
            m_pUserName.ReadOnly = true;

            mt_SessionID = new Label();
            mt_SessionID.Size = new Size(120,20);
            mt_SessionID.Location = new Point(0,70);
            mt_SessionID.TextAlign = ContentAlignment.MiddleRight;
            mt_SessionID.Text = "Session ID:";

            m_pSessionID = new TextBox();
            m_pSessionID.Size = new Size(250,20);
            m_pSessionID.Location = new Point(125,70);
            m_pSessionID.ReadOnly = true;

            mt_RemoteEndPoint = new Label();
            mt_RemoteEndPoint.Size = new Size(120,20);
            mt_RemoteEndPoint.Location = new Point(0,95);
            mt_RemoteEndPoint.TextAlign = ContentAlignment.MiddleRight;
            mt_RemoteEndPoint.Text = "Remote End Point:";

            m_pRemoteEndPoint = new TextBox();
            m_pRemoteEndPoint.Size = new Size(250,20);
            m_pRemoteEndPoint.Location = new Point(125,95);
            m_pRemoteEndPoint.ReadOnly = true;

            m_pLogText = new RichTextBox();
            m_pLogText.Size = new Size(483,225);
            m_pLogText.Location = new Point(5,125);
            m_pLogText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            m_pLogText.ReadOnly = true;

            this.Controls.Add(mt_SessionStartTime);
            this.Controls.Add(m_pSessionStartTime);
            this.Controls.Add(mt_UserName);
            this.Controls.Add(m_pUserName);
            this.Controls.Add(mt_SessionID);
            this.Controls.Add(m_pSessionID);
            this.Controls.Add(mt_RemoteEndPoint);
            this.Controls.Add(m_pRemoteEndPoint);
            this.Controls.Add(m_pLogText);
        }

        #endregion


        #region method LoadLogText

        /// <summary>
        /// Loads log text to UI.
        /// </summary>
        /// <param name="logText">Log text.</param>
        /// <param name="highlightWord">Word that will be highlighted in log text.</param>
        private void LoadLogText(string logText,string highlightWord)
        {            
            string[] logLines = logText.Replace("\r","").Split('\n');
            foreach(string logLine in logLines){
                string[] fields = TextUtils.SplitQuotedString(logLine,'\t',true);
                if(fields.Length == 6){
                    if(m_pSessionStartTime.Text.Length == 0){
                        m_pSessionStartTime.Text = fields[1];
                    }
                    if(m_pUserName.Text.Length == 0){
                        m_pUserName.Text = fields[3];
                    }
                    if(m_pSessionID.Text.Length == 0){
                        m_pSessionID.Text = fields[0];
                    }
                    if(m_pRemoteEndPoint.Text.Length == 0){
                        m_pRemoteEndPoint.Text = fields[2];
                    }
                    string logType     = fields[4];
                    string logLineText = "\"" + fields[5] + "\"";

                    // Add log type and paint it green. <<< >>> xxx
                    m_pLogText.AppendText(logType);
                    // REMOVE ME: Mono 1.2 throws exception here
                    if(Environment.OSVersion.Platform != PlatformID.Unix){
                        m_pLogText.SelectionStart  = m_pLogText.Text.Length - logType.Length;
                        m_pLogText.SelectionLength = logType.Length;
                        m_pLogText.SelectionColor  = Color.DarkGreen;
                    }
                    
                    m_pLogText.AppendText("  ");

                    // Add log line text and paint it magneta.
                    m_pLogText.AppendText(logLineText);
                    if(Environment.OSVersion.Platform != PlatformID.Unix){
                        m_pLogText.SelectionStart  = m_pLogText.Text.Length - logLineText.Length;
                        m_pLogText.SelectionLength = logLineText.Length;
                        m_pLogText.SelectionColor  = Color.DarkMagenta;
                        // Highlight requested word
                        if(highlightWord.Length > 0 && logLineText.IndexOf(highlightWord) > -1){
                            m_pLogText.SelectionStart  = m_pLogText.Text.Length - logLineText.Length + logLineText.IndexOf(highlightWord);
                            m_pLogText.SelectionLength = highlightWord.Length;
                            m_pLogText.SelectionFont   = new Font(m_pLogText.Font,FontStyle.Bold);
                            m_pLogText.SelectionColor  = Color.Red;
                        }
                    }

                    m_pLogText.AppendText("\n");
                }
            }
        }

        #endregion

    }
}
