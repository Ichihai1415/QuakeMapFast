namespace QuakeMapFast
{
    partial class CtrlForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlForm));
            this.GroupBox_Display = new System.Windows.Forms.GroupBox();
            this.AutoCopy = new System.Windows.Forms.CheckBox();
            this.BackGreenTime = new System.Windows.Forms.NumericUpDown();
            this.WindowLocation_Y = new System.Windows.Forms.NumericUpDown();
            this.Save_Image = new System.Windows.Forms.CheckBox();
            this.Save_JSON = new System.Windows.Forms.CheckBox();
            this.WindowLocation_X = new System.Windows.Forms.NumericUpDown();
            this.WindowSize_Height = new System.Windows.Forms.NumericUpDown();
            this.WindowSize_Width = new System.Windows.Forms.NumericUpDown();
            this.GroupBox_Display_Text = new System.Windows.Forms.Label();
            this.GroupBox_Bouyomi = new System.Windows.Forms.GroupBox();
            this.BouyomiTest = new System.Windows.Forms.Button();
            this.Bouyomi_Enable = new System.Windows.Forms.CheckBox();
            this.Bouyomi_Tone = new System.Windows.Forms.NumericUpDown();
            this.Bouyomi_Speed = new System.Windows.Forms.NumericUpDown();
            this.Bouyomi_Volume = new System.Windows.Forms.NumericUpDown();
            this.Bouyomi_Voice = new System.Windows.Forms.NumericUpDown();
            this.GroupBox_Bouyomi_Text = new System.Windows.Forms.Label();
            this.GroupBox_Telop = new System.Windows.Forms.GroupBox();
            this.TelopTest = new System.Windows.Forms.Button();
            this.Telop_Enable = new System.Windows.Forms.CheckBox();
            this.GroupBox_Telop_Text = new System.Windows.Forms.Label();
            this.TextEnd = new System.Windows.Forms.Label();
            this.SettingSave = new System.Windows.Forms.Button();
            this.SettingReset = new System.Windows.Forms.Button();
            this.JSONread = new System.Windows.Forms.Button();
            this.GroupBox_Display.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BackGreenTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowLocation_Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowLocation_X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowSize_Height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowSize_Width)).BeginInit();
            this.GroupBox_Bouyomi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Tone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Speed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Volume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Voice)).BeginInit();
            this.GroupBox_Telop.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox_Display
            // 
            this.GroupBox_Display.Controls.Add(this.AutoCopy);
            this.GroupBox_Display.Controls.Add(this.BackGreenTime);
            this.GroupBox_Display.Controls.Add(this.WindowLocation_Y);
            this.GroupBox_Display.Controls.Add(this.Save_Image);
            this.GroupBox_Display.Controls.Add(this.Save_JSON);
            this.GroupBox_Display.Controls.Add(this.WindowLocation_X);
            this.GroupBox_Display.Controls.Add(this.WindowSize_Height);
            this.GroupBox_Display.Controls.Add(this.WindowSize_Width);
            this.GroupBox_Display.Controls.Add(this.GroupBox_Display_Text);
            this.GroupBox_Display.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.GroupBox_Display.Location = new System.Drawing.Point(13, 13);
            this.GroupBox_Display.Margin = new System.Windows.Forms.Padding(4);
            this.GroupBox_Display.Name = "GroupBox_Display";
            this.GroupBox_Display.Padding = new System.Windows.Forms.Padding(4);
            this.GroupBox_Display.Size = new System.Drawing.Size(384, 196);
            this.GroupBox_Display.TabIndex = 0;
            this.GroupBox_Display.TabStop = false;
            this.GroupBox_Display.Text = "表示・処理関連";
            // 
            // AutoCopy
            // 
            this.AutoCopy.AutoSize = true;
            this.AutoCopy.Location = new System.Drawing.Point(144, 169);
            this.AutoCopy.Name = "AutoCopy";
            this.AutoCopy.Size = new System.Drawing.Size(144, 19);
            this.AutoCopy.TabIndex = 8;
            this.AutoCopy.Text = "(されない場合があります)";
            this.AutoCopy.UseVisualStyleBackColor = true;
            // 
            // BackGreenTime
            // 
            this.BackGreenTime.Location = new System.Drawing.Point(153, 137);
            this.BackGreenTime.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.BackGreenTime.Name = "BackGreenTime";
            this.BackGreenTime.Size = new System.Drawing.Size(45, 23);
            this.BackGreenTime.TabIndex = 7;
            // 
            // WindowLocation_Y
            // 
            this.WindowLocation_Y.Location = new System.Drawing.Point(153, 48);
            this.WindowLocation_Y.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.WindowLocation_Y.Name = "WindowLocation_Y";
            this.WindowLocation_Y.Size = new System.Drawing.Size(45, 23);
            this.WindowLocation_Y.TabIndex = 6;
            this.WindowLocation_Y.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // Save_Image
            // 
            this.Save_Image.AutoSize = true;
            this.Save_Image.Checked = true;
            this.Save_Image.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Save_Image.Location = new System.Drawing.Point(74, 112);
            this.Save_Image.Name = "Save_Image";
            this.Save_Image.Size = new System.Drawing.Size(15, 14);
            this.Save_Image.TabIndex = 5;
            this.Save_Image.UseVisualStyleBackColor = true;
            // 
            // Save_JSON
            // 
            this.Save_JSON.AutoSize = true;
            this.Save_JSON.Location = new System.Drawing.Point(117, 80);
            this.Save_JSON.Name = "Save_JSON";
            this.Save_JSON.Size = new System.Drawing.Size(176, 19);
            this.Save_JSON.TabIndex = 4;
            this.Save_JSON.Text = "(処理しないものも保存されます)";
            this.Save_JSON.UseVisualStyleBackColor = true;
            // 
            // WindowLocation_X
            // 
            this.WindowLocation_X.Location = new System.Drawing.Point(90, 48);
            this.WindowLocation_X.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.WindowLocation_X.Name = "WindowLocation_X";
            this.WindowLocation_X.Size = new System.Drawing.Size(45, 23);
            this.WindowLocation_X.TabIndex = 3;
            this.WindowLocation_X.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // WindowSize_Height
            // 
            this.WindowSize_Height.Location = new System.Drawing.Point(153, 18);
            this.WindowSize_Height.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.WindowSize_Height.Name = "WindowSize_Height";
            this.WindowSize_Height.Size = new System.Drawing.Size(45, 23);
            this.WindowSize_Height.TabIndex = 2;
            this.WindowSize_Height.Value = new decimal(new int[] {
            720,
            0,
            0,
            0});
            // 
            // WindowSize_Width
            // 
            this.WindowSize_Width.Location = new System.Drawing.Point(90, 18);
            this.WindowSize_Width.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.WindowSize_Width.Name = "WindowSize_Width";
            this.WindowSize_Width.Size = new System.Drawing.Size(45, 23);
            this.WindowSize_Width.TabIndex = 1;
            this.WindowSize_Width.Value = new decimal(new int[] {
            1280,
            0,
            0,
            0});
            // 
            // GroupBox_Display_Text
            // 
            this.GroupBox_Display_Text.AutoSize = true;
            this.GroupBox_Display_Text.Location = new System.Drawing.Point(8, 20);
            this.GroupBox_Display_Text.Name = "GroupBox_Display_Text";
            this.GroupBox_Display_Text.Size = new System.Drawing.Size(250, 165);
            this.GroupBox_Display_Text.TabIndex = 0;
            this.GroupBox_Display_Text.Text = "ウィンドウサイズ:                   x\r\n\r\nウィンドウ位置:                    ,\r\n\r\n受信したデータの保存:\r\n\r\n" +
    "画像の保存:\r\n\r\n受信してから真緑になる時間:                 s(0で無効)\r\n\r\nクリップボードへの自動コピー:\r\n";
            // 
            // GroupBox_Bouyomi
            // 
            this.GroupBox_Bouyomi.Controls.Add(this.BouyomiTest);
            this.GroupBox_Bouyomi.Controls.Add(this.Bouyomi_Enable);
            this.GroupBox_Bouyomi.Controls.Add(this.Bouyomi_Tone);
            this.GroupBox_Bouyomi.Controls.Add(this.Bouyomi_Speed);
            this.GroupBox_Bouyomi.Controls.Add(this.Bouyomi_Volume);
            this.GroupBox_Bouyomi.Controls.Add(this.Bouyomi_Voice);
            this.GroupBox_Bouyomi.Controls.Add(this.GroupBox_Bouyomi_Text);
            this.GroupBox_Bouyomi.Location = new System.Drawing.Point(404, 13);
            this.GroupBox_Bouyomi.Name = "GroupBox_Bouyomi";
            this.GroupBox_Bouyomi.Size = new System.Drawing.Size(384, 126);
            this.GroupBox_Bouyomi.TabIndex = 1;
            this.GroupBox_Bouyomi.TabStop = false;
            this.GroupBox_Bouyomi.Text = "棒読みちゃん送信";
            // 
            // BouyomiTest
            // 
            this.BouyomiTest.Location = new System.Drawing.Point(58, 17);
            this.BouyomiTest.Name = "BouyomiTest";
            this.BouyomiTest.Size = new System.Drawing.Size(75, 23);
            this.BouyomiTest.TabIndex = 12;
            this.BouyomiTest.Text = "送信テスト";
            this.BouyomiTest.UseVisualStyleBackColor = true;
            this.BouyomiTest.Click += new System.EventHandler(this.BouyomiTest_Click);
            // 
            // Bouyomi_Enable
            // 
            this.Bouyomi_Enable.AutoSize = true;
            this.Bouyomi_Enable.Location = new System.Drawing.Point(37, 21);
            this.Bouyomi_Enable.Name = "Bouyomi_Enable";
            this.Bouyomi_Enable.Size = new System.Drawing.Size(15, 14);
            this.Bouyomi_Enable.TabIndex = 11;
            this.Bouyomi_Enable.UseVisualStyleBackColor = true;
            // 
            // Bouyomi_Tone
            // 
            this.Bouyomi_Tone.Location = new System.Drawing.Point(196, 92);
            this.Bouyomi_Tone.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.Bouyomi_Tone.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.Bouyomi_Tone.Name = "Bouyomi_Tone";
            this.Bouyomi_Tone.Size = new System.Drawing.Size(39, 23);
            this.Bouyomi_Tone.TabIndex = 10;
            this.Bouyomi_Tone.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // Bouyomi_Speed
            // 
            this.Bouyomi_Speed.Location = new System.Drawing.Point(118, 92);
            this.Bouyomi_Speed.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.Bouyomi_Speed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.Bouyomi_Speed.Name = "Bouyomi_Speed";
            this.Bouyomi_Speed.Size = new System.Drawing.Size(39, 23);
            this.Bouyomi_Speed.TabIndex = 9;
            this.Bouyomi_Speed.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // Bouyomi_Volume
            // 
            this.Bouyomi_Volume.Location = new System.Drawing.Point(40, 92);
            this.Bouyomi_Volume.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.Bouyomi_Volume.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.Bouyomi_Volume.Name = "Bouyomi_Volume";
            this.Bouyomi_Volume.Size = new System.Drawing.Size(39, 23);
            this.Bouyomi_Volume.TabIndex = 8;
            this.Bouyomi_Volume.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // Bouyomi_Voice
            // 
            this.Bouyomi_Voice.Location = new System.Drawing.Point(40, 47);
            this.Bouyomi_Voice.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.Bouyomi_Voice.Name = "Bouyomi_Voice";
            this.Bouyomi_Voice.Size = new System.Drawing.Size(52, 23);
            this.Bouyomi_Voice.TabIndex = 8;
            // 
            // GroupBox_Bouyomi_Text
            // 
            this.GroupBox_Bouyomi_Text.AutoSize = true;
            this.GroupBox_Bouyomi_Text.Location = new System.Drawing.Point(6, 19);
            this.GroupBox_Bouyomi_Text.Name = "GroupBox_Bouyomi_Text";
            this.GroupBox_Bouyomi_Text.Size = new System.Drawing.Size(369, 90);
            this.GroupBox_Bouyomi_Text.TabIndex = 0;
            this.GroupBox_Bouyomi_Text.Text = resources.GetString("GroupBox_Bouyomi_Text.Text");
            // 
            // GroupBox_Telop
            // 
            this.GroupBox_Telop.Controls.Add(this.TelopTest);
            this.GroupBox_Telop.Controls.Add(this.Telop_Enable);
            this.GroupBox_Telop.Controls.Add(this.GroupBox_Telop_Text);
            this.GroupBox_Telop.Location = new System.Drawing.Point(404, 145);
            this.GroupBox_Telop.Name = "GroupBox_Telop";
            this.GroupBox_Telop.Size = new System.Drawing.Size(384, 55);
            this.GroupBox_Telop.TabIndex = 2;
            this.GroupBox_Telop.TabStop = false;
            this.GroupBox_Telop.Text = "Telop送信";
            // 
            // TelopTest
            // 
            this.TelopTest.Location = new System.Drawing.Point(58, 22);
            this.TelopTest.Name = "TelopTest";
            this.TelopTest.Size = new System.Drawing.Size(75, 23);
            this.TelopTest.TabIndex = 2;
            this.TelopTest.Text = "送信テスト";
            this.TelopTest.UseVisualStyleBackColor = true;
            this.TelopTest.Click += new System.EventHandler(this.TelopTest_Click);
            // 
            // Telop_Enable
            // 
            this.Telop_Enable.AutoSize = true;
            this.Telop_Enable.Location = new System.Drawing.Point(37, 26);
            this.Telop_Enable.Name = "Telop_Enable";
            this.Telop_Enable.Size = new System.Drawing.Size(15, 14);
            this.Telop_Enable.TabIndex = 1;
            this.Telop_Enable.UseVisualStyleBackColor = true;
            // 
            // GroupBox_Telop_Text
            // 
            this.GroupBox_Telop_Text.AutoSize = true;
            this.GroupBox_Telop_Text.Location = new System.Drawing.Point(6, 24);
            this.GroupBox_Telop_Text.Name = "GroupBox_Telop_Text";
            this.GroupBox_Telop_Text.Size = new System.Drawing.Size(34, 15);
            this.GroupBox_Telop_Text.TabIndex = 0;
            this.GroupBox_Telop_Text.Text = "有効:";
            // 
            // TextEnd
            // 
            this.TextEnd.AutoSize = true;
            this.TextEnd.Location = new System.Drawing.Point(12, 426);
            this.TextEnd.Name = "TextEnd";
            this.TextEnd.Size = new System.Drawing.Size(218, 15);
            this.TextEnd.TabIndex = 3;
            this.TextEnd.Text = "詳細はREADME.mdなどを確認してください。";
            // 
            // SettingSave
            // 
            this.SettingSave.Location = new System.Drawing.Point(632, 415);
            this.SettingSave.Name = "SettingSave";
            this.SettingSave.Size = new System.Drawing.Size(75, 23);
            this.SettingSave.TabIndex = 4;
            this.SettingSave.Text = "保存";
            this.SettingSave.UseVisualStyleBackColor = true;
            this.SettingSave.Click += new System.EventHandler(this.SettingSave_Click);
            // 
            // SettingReset
            // 
            this.SettingReset.Location = new System.Drawing.Point(713, 415);
            this.SettingReset.Name = "SettingReset";
            this.SettingReset.Size = new System.Drawing.Size(75, 23);
            this.SettingReset.TabIndex = 5;
            this.SettingReset.Text = "リセット";
            this.SettingReset.UseVisualStyleBackColor = true;
            this.SettingReset.Click += new System.EventHandler(this.SettingReset_Click);
            // 
            // JSONread
            // 
            this.JSONread.Location = new System.Drawing.Point(276, 415);
            this.JSONread.Name = "JSONread";
            this.JSONread.Size = new System.Drawing.Size(261, 23);
            this.JSONread.TabIndex = 6;
            this.JSONread.Text = "JSON読み込み(コンソールにパスを入力してください)";
            this.JSONread.UseVisualStyleBackColor = true;
            this.JSONread.Click += new System.EventHandler(this.JSONread_Click);
            // 
            // CtrlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.JSONread);
            this.Controls.Add(this.SettingReset);
            this.Controls.Add(this.SettingSave);
            this.Controls.Add(this.TextEnd);
            this.Controls.Add(this.GroupBox_Telop);
            this.Controls.Add(this.GroupBox_Bouyomi);
            this.Controls.Add(this.GroupBox_Display);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "CtrlForm";
            this.Text = "QuMast - コントロール画面";
            this.Load += new System.EventHandler(this.CtrlForm_Load);
            this.GroupBox_Display.ResumeLayout(false);
            this.GroupBox_Display.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BackGreenTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowLocation_Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowLocation_X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowSize_Height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WindowSize_Width)).EndInit();
            this.GroupBox_Bouyomi.ResumeLayout(false);
            this.GroupBox_Bouyomi.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Tone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Speed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Volume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bouyomi_Voice)).EndInit();
            this.GroupBox_Telop.ResumeLayout(false);
            this.GroupBox_Telop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox_Display;
        private System.Windows.Forms.Label GroupBox_Display_Text;
        private System.Windows.Forms.NumericUpDown WindowSize_Width;
        private System.Windows.Forms.NumericUpDown WindowSize_Height;
        private System.Windows.Forms.NumericUpDown WindowLocation_X;
        private System.Windows.Forms.CheckBox Save_Image;
        private System.Windows.Forms.CheckBox Save_JSON;
        private System.Windows.Forms.NumericUpDown WindowLocation_Y;
        private System.Windows.Forms.NumericUpDown BackGreenTime;
        private System.Windows.Forms.GroupBox GroupBox_Bouyomi;
        private System.Windows.Forms.GroupBox GroupBox_Telop;
        private System.Windows.Forms.Label TextEnd;
        private System.Windows.Forms.Label GroupBox_Bouyomi_Text;
        private System.Windows.Forms.NumericUpDown Bouyomi_Voice;
        private System.Windows.Forms.NumericUpDown Bouyomi_Volume;
        private System.Windows.Forms.NumericUpDown Bouyomi_Tone;
        private System.Windows.Forms.NumericUpDown Bouyomi_Speed;
        private System.Windows.Forms.Label GroupBox_Telop_Text;
        private System.Windows.Forms.CheckBox Bouyomi_Enable;
        private System.Windows.Forms.CheckBox Telop_Enable;
        private System.Windows.Forms.Button BouyomiTest;
        private System.Windows.Forms.Button TelopTest;
        private System.Windows.Forms.Button SettingSave;
        private System.Windows.Forms.Button SettingReset;
        private System.Windows.Forms.CheckBox AutoCopy;
        private System.Windows.Forms.Button JSONread;
    }
}