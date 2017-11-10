namespace LumiSoft.MailServer.UI
{
    partial class wfrm_Install
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
            this.m_pInstallAsService = new System.Windows.Forms.Button();
            this.m_pUninstallService = new System.Windows.Forms.Button();
            this.m_pRunAsTryApp = new System.Windows.Forms.Button();
            this.m_pRunAsWindowsForm = new System.Windows.Forms.Button();
            this.m_pService = new System.Windows.Forms.GroupBox();
            this.m_pRun = new System.Windows.Forms.GroupBox();
            this.m_pService.SuspendLayout();
            this.m_pRun.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_pInstallAsService
            // 
            this.m_pInstallAsService.Location = new System.Drawing.Point(6, 19);
            this.m_pInstallAsService.Name = "m_pInstallAsService";
            this.m_pInstallAsService.Size = new System.Drawing.Size(210, 23);
            this.m_pInstallAsService.TabIndex = 0;
            this.m_pInstallAsService.Text = "Install as Service";
            this.m_pInstallAsService.UseVisualStyleBackColor = true;
            this.m_pInstallAsService.Click += new System.EventHandler(this.m_pInstallAsService_Click);
            // 
            // m_pUninstallService
            // 
            this.m_pUninstallService.Enabled = false;
            this.m_pUninstallService.Location = new System.Drawing.Point(6, 48);
            this.m_pUninstallService.Name = "m_pUninstallService";
            this.m_pUninstallService.Size = new System.Drawing.Size(210, 23);
            this.m_pUninstallService.TabIndex = 1;
            this.m_pUninstallService.Text = "Uninstall";
            this.m_pUninstallService.UseVisualStyleBackColor = true;
            this.m_pUninstallService.Click += new System.EventHandler(this.m_pUninstallService_Click);
            // 
            // m_pRunAsTryApp
            // 
            this.m_pRunAsTryApp.Location = new System.Drawing.Point(6, 19);
            this.m_pRunAsTryApp.Name = "m_pRunAsTryApp";
            this.m_pRunAsTryApp.Size = new System.Drawing.Size(210, 23);
            this.m_pRunAsTryApp.TabIndex = 2;
            this.m_pRunAsTryApp.Text = "Run as tray application";
            this.m_pRunAsTryApp.UseVisualStyleBackColor = true;
            this.m_pRunAsTryApp.Click += new System.EventHandler(this.m_pRunAsTryApp_Click);
            // 
            // m_pRunAsWindowsForm
            // 
            this.m_pRunAsWindowsForm.Location = new System.Drawing.Point(6, 48);
            this.m_pRunAsWindowsForm.Name = "m_pRunAsWindowsForm";
            this.m_pRunAsWindowsForm.Size = new System.Drawing.Size(210, 23);
            this.m_pRunAsWindowsForm.TabIndex = 3;
            this.m_pRunAsWindowsForm.Text = "Run as windows form application";
            this.m_pRunAsWindowsForm.UseVisualStyleBackColor = true;
            this.m_pRunAsWindowsForm.Click += new System.EventHandler(this.m_pRunAsWindowsForm_Click);
            // 
            // m_pService
            // 
            this.m_pService.Controls.Add(this.m_pInstallAsService);
            this.m_pService.Controls.Add(this.m_pUninstallService);
            this.m_pService.Location = new System.Drawing.Point(10, 10);
            this.m_pService.Name = "m_pService";
            this.m_pService.Size = new System.Drawing.Size(222, 80);
            this.m_pService.TabIndex = 4;
            this.m_pService.TabStop = false;
            this.m_pService.Text = "Service";
            // 
            // m_pRun
            // 
            this.m_pRun.Controls.Add(this.m_pRunAsTryApp);
            this.m_pRun.Controls.Add(this.m_pRunAsWindowsForm);
            this.m_pRun.Location = new System.Drawing.Point(10, 100);
            this.m_pRun.Name = "m_pRun";
            this.m_pRun.Size = new System.Drawing.Size(222, 80);
            this.m_pRun.TabIndex = 5;
            this.m_pRun.TabStop = false;
            this.m_pRun.Text = "Run";
            // 
            // wfrm_Install
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 191);
            this.Controls.Add(this.m_pRun);
            this.Controls.Add(this.m_pService);
            this.Name = "wfrm_Install";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mail Server Installer";
            this.m_pService.ResumeLayout(false);
            this.m_pRun.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_pInstallAsService;
        private System.Windows.Forms.Button m_pUninstallService;
        private System.Windows.Forms.Button m_pRunAsTryApp;
        private System.Windows.Forms.Button m_pRunAsWindowsForm;
        private System.Windows.Forms.GroupBox m_pService;
        private System.Windows.Forms.GroupBox m_pRun;
    }
}