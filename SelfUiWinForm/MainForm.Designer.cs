﻿namespace SelfUiWinForm
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            NotifyIcon = new NotifyIcon(components);
            HiddenTimer = new Timer(components);
            StatusLabel = new Label();
            SuspendLayout();
            // 
            // NotifyIcon
            // 
            NotifyIcon.Icon = (System.Drawing.Icon)resources.GetObject("NotifyIcon.Icon");
            NotifyIcon.Text = "SelfUI";
            NotifyIcon.Visible = true;
            // 
            // HiddenTimer
            // 
            HiddenTimer.Enabled = true;
            HiddenTimer.Tick += HiddenTimer_Tick;
            // 
            // StatusLabel
            // 
            StatusLabel.AutoSize = true;
            StatusLabel.Location = new System.Drawing.Point(10, 8);
            StatusLabel.Margin = new Padding(2, 0, 2, 0);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new System.Drawing.Size(14, 20);
            StatusLabel.TabIndex = 0;
            StatusLabel.Text = "!";
            StatusLabel.Click += StatusLabel_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1288, 700);
            Controls.Add(StatusLabel);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            ImeMode = ImeMode.Disable;
            Margin = new Padding(2, 2, 2, 2);
            MaximizeBox = false;
            Name = "MainForm";
            ShowIcon = false;
            Text = "SelfUI";
            Load += MainForm_Load;
            LocationChanged += MainForm_LocationChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private Timer HiddenTimer;
        public Label StatusLabel;
    }
}

