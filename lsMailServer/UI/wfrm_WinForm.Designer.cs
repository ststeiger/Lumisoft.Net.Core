namespace LumiSoft.MailServer.UI
{
    partial class wfrm_WinForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_pStart = new System.Windows.Forms.Button();
            this.m_pStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_pStart
            // 
            this.m_pStart.Location = new System.Drawing.Point(12, 23);
            this.m_pStart.Name = "m_pStart";
            this.m_pStart.Size = new System.Drawing.Size(75, 23);
            this.m_pStart.TabIndex = 0;
            this.m_pStart.Text = "Start";
            this.m_pStart.UseVisualStyleBackColor = true;
            this.m_pStart.Click += new System.EventHandler(this.m_pStart_Click);
            // 
            // m_pStop
            // 
            this.m_pStop.Location = new System.Drawing.Point(182, 23);
            this.m_pStop.Name = "m_pStop";
            this.m_pStop.Size = new System.Drawing.Size(75, 23);
            this.m_pStop.TabIndex = 1;
            this.m_pStop.Text = "Stop";
            this.m_pStop.UseVisualStyleBackColor = true;
            this.m_pStop.Click += new System.EventHandler(this.m_pStop_Click);
            // 
            // wfrm_WinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 64);
            this.Controls.Add(this.m_pStop);
            this.Controls.Add(this.m_pStart);
            this.Name = "wfrm_WinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LumiSoft Mail Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.wfrm_WinForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_pStart;
        private System.Windows.Forms.Button m_pStop;
    }
}