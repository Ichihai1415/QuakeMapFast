using QuakeMapFast.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuakeMapFast
{
    /// <summary>
    /// 画像表示画面
    /// </summary>
    public partial class DataView : Form
    {
        /// <summary>
        /// 最新の情報のテキスト
        /// </summary>
        string lastText = "";
        /// <summary>
        /// 最新の情報の画像
        /// </summary>
        Bitmap lastBitmap = new Bitmap(1920, 1080);

        public DataView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// サイズ・位置設定を反映します。
        /// </summary>
        public void SettingReload()
        {
            ClientSize = Settings.Default.Window_Size;
            Location = Settings.Default.Window_Location;
        }

        private void TSMI_TextCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lastText);
        }

        private void TSMI_ImageCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(lastBitmap);
        }

        private void Form1_BackgroundImageChanged(object sender, EventArgs e)
        {
            if (Settings.Default.BackGreenTime == 0)
                return;
            if (BackgroundImage != null)
            {
                GBTimer.Enabled = false;//切り替わり前に再更新した場合タイマーのリセット
                GBTimer.Interval = Settings.Default.BackGreenTime * 1000;
                GBTimer.Enabled = true;
            }
        }

        private void GBTimer_Tick(object sender, EventArgs e)
        {
            BackgroundImage = null;
            GBTimer.Enabled = false;
        }

        /// <summary>
        /// 画像を表示します。
        /// </summary>
        /// <param name="newImage">新しい画像</param>
        /// <param name="newText">コピー用テキスト</param>
        public void ImageChange(Bitmap newImage, string newText)
        {
            BackgroundImage = null;
            BackgroundImage = newImage;
            lastBitmap = (Bitmap)newImage.Clone();
            lastText = newText;
        }

        private void DataView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("終了してもよろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                Environment.Exit(0);//こうじゃないと1回では閉じないっぽい
            else
                e.Cancel = true;
        }
    }
}
