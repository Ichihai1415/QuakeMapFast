namespace QuakeMapFast
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CMS = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSMI_Setting = new System.Windows.Forms.ToolStripMenuItem();
            this.TSS1 = new System.Windows.Forms.ToolStripSeparator();
            this.TSMI_TextCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_ImageCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.GBTimer = new System.Windows.Forms.Timer(this.components);
            this.TSMI_ReadJSON = new System.Windows.Forms.ToolStripMenuItem();
            this.TSS2 = new System.Windows.Forms.ToolStripSeparator();
            this.CMS.SuspendLayout();
            this.SuspendLayout();
            // 
            // CMS
            // 
            this.CMS.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.CMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Setting,
            this.TSS1,
            this.TSMI_ReadJSON,
            this.TSS2,
            this.TSMI_TextCopy,
            this.TSMI_ImageCopy});
            this.CMS.Name = "CMS";
            this.CMS.Size = new System.Drawing.Size(270, 140);
            // 
            // TSMI_Setting
            // 
            this.TSMI_Setting.Name = "TSMI_Setting";
            this.TSMI_Setting.Size = new System.Drawing.Size(269, 24);
            this.TSMI_Setting.Text = "設定";
            this.TSMI_Setting.Click += new System.EventHandler(this.TSMI_Setting_Click);
            // 
            // TSS1
            // 
            this.TSS1.Name = "TSS1";
            this.TSS1.Size = new System.Drawing.Size(266, 6);
            // 
            // TSMI_TextCopy
            // 
            this.TSMI_TextCopy.Name = "TSMI_TextCopy";
            this.TSMI_TextCopy.Size = new System.Drawing.Size(269, 24);
            this.TSMI_TextCopy.Text = "テキストコピー";
            this.TSMI_TextCopy.Click += new System.EventHandler(this.TSMI_TextCopy_Click);
            // 
            // TSMI_ImageCopy
            // 
            this.TSMI_ImageCopy.Name = "TSMI_ImageCopy";
            this.TSMI_ImageCopy.Size = new System.Drawing.Size(269, 24);
            this.TSMI_ImageCopy.Text = "画像コピー";
            this.TSMI_ImageCopy.Click += new System.EventHandler(this.TSMI_ImageCopy_Click);
            // 
            // GBTimer
            // 
            this.GBTimer.Interval = 30000;
            this.GBTimer.Tick += new System.EventHandler(this.GBTimer_Tick);
            // 
            // TSMI_ReadJSON
            // 
            this.TSMI_ReadJSON.Name = "TSMI_ReadJSON";
            this.TSMI_ReadJSON.Size = new System.Drawing.Size(269, 24);
            this.TSMI_ReadJSON.Text = "JSON読み込みモードをオンにする";
            this.TSMI_ReadJSON.Click += new System.EventHandler(this.TSMI_ReadJSON_Click);
            // 
            // TSS2
            // 
            this.TSS2.Name = "TSS2";
            this.TSS2.Size = new System.Drawing.Size(266, 6);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Lime;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.ContextMenuStrip = this.CMS;
            this.Name = "MainForm";
            this.Text = "QuakeMapFast";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.BackgroundImageChanged += new System.EventHandler(this.Form1_BackgroundImageChanged);
            this.CMS.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip CMS;
        private System.Windows.Forms.ToolStripMenuItem TSMI_TextCopy;
        private System.Windows.Forms.ToolStripMenuItem TSMI_ImageCopy;
        private System.Windows.Forms.Timer GBTimer;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Setting;
        private System.Windows.Forms.ToolStripSeparator TSS1;
        private System.Windows.Forms.ToolStripMenuItem TSMI_ReadJSON;
        private System.Windows.Forms.ToolStripSeparator TSS2;
    }
}

