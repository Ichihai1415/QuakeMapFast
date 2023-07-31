using QuakeMapFast.Properties;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace QuakeMapFast
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            MainForm.ConsoleWrite("[Setting]設定読み込み開始");
            Settings.Default.Reload();

            WindowSize_Width.Value = Settings.Default.Window_Size.Width;
            WindowSize_Height.Value = Settings.Default.Window_Size.Height;
            WindowLocation_X.Value = Settings.Default.Window_Location.X;
            WindowLocation_Y.Value = Settings.Default.Window_Location.Y;
            Save_JSON.Checked = Settings.Default.Save_JSON;
            Save_Image.Checked = Settings.Default.Save_Image;
            BackGreenTime.Value = Settings.Default.BackGreenTime;

            Bouyomi_Enable.Checked = Settings.Default.Bouyomi_Enable;
            Bouyomi_Voice.Value = Settings.Default.Bouyomi_Voice;
            Bouyomi_Volume.Value = Settings.Default.Bouyomi_Volume;
            Bouyomi_Speed.Value = Settings.Default.Bouyomi_Speed;
            Bouyomi_Tone.Value = Settings.Default.Bouyomi_Tone;

            Telop_Enable.Checked = Settings.Default.Telop_Enable;
            MainForm.ConsoleWrite("[Setting]設定読み込み終了");
        }

        private void SettingSave_Click(object sender, EventArgs e)
        {
            MainForm.ConsoleWrite("[Setting]設定保存開始");
            Settings.Default.Window_Size = new Size((int)WindowSize_Width.Value, (int)WindowSize_Height.Value);
            Settings.Default.Window_Location = new Point((int)WindowLocation_X.Value, (int)WindowLocation_Y.Value);
            Settings.Default.Save_JSON = Save_JSON.Checked;
            Settings.Default.Save_Image = Save_Image.Checked;
            Settings.Default.BackGreenTime = (int)BackGreenTime.Value;

            Settings.Default.Bouyomi_Enable = Bouyomi_Enable.Checked;
            Settings.Default.Bouyomi_Voice = (short)Bouyomi_Voice.Value;
            Settings.Default.Bouyomi_Volume = (short)Bouyomi_Volume.Value;
            Settings.Default.Bouyomi_Speed = (short)Bouyomi_Speed.Value;
            Settings.Default.Bouyomi_Tone = (short)Bouyomi_Tone.Value;

            Settings.Default.Telop_Enable = Telop_Enable.Checked;
            Settings.Default.Save();
            File.Copy(MainForm.config.FilePath, "UserSetting.xml", true);
            File.WriteAllText("AppDataPath.txt", MainForm.config.FilePath);
            MainForm.ConsoleWrite("[Setting]設定保存終了");
        }

        private void SettingReset_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("リセットしてもよろしいですか？\nリセットすると設定画面を開き直します。", "WQV - setting", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (Result == DialogResult.Yes)
            {
                MainForm.ConsoleWrite("[Setting]設定をリセットします");
                Settings.Default.Reset();
                SettingForm Setting = new SettingForm();
                Configuration Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                if (File.Exists("UserSetting.xml"))
                    File.Delete("UserSetting.xml");
                if (File.Exists(Config.FilePath))
                    File.Delete(Config.FilePath);
                Setting.Show();
                Close();
            }
        }

        private void BouyomiTest_Click(object sender, EventArgs e)
        {
            MainForm.ConsoleWrite("[Setting]棒読みちゃん送信テスト開始");
            string Text = "QuakeMapFastの棒読みちゃん送信テストです。";
            MainForm.ConsoleWrite("[Bouyomi]Text:" + Text);
            try
            {
                byte[] Message = Encoding.UTF8.GetBytes(Text);
                int Length = Message.Length;
                byte Code = 0;
                short Command = 0x0001;
                short Speed = (short)Bouyomi_Speed.Value;
                short Tone = (short)Bouyomi_Tone.Value;
                short Volume = (short)Bouyomi_Volume.Value;
                short Voice = (short)Bouyomi_Voice.Value;
                using (TcpClient tcpClient = new TcpClient("127.0.0.1", 50001))
                using (NetworkStream networkStream = tcpClient.GetStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(networkStream))
                {
                    binaryWriter.Write(Command);
                    binaryWriter.Write(Speed);
                    binaryWriter.Write(Tone);
                    binaryWriter.Write(Volume);
                    binaryWriter.Write(Voice);
                    binaryWriter.Write(Code);
                    binaryWriter.Write(Length);
                    binaryWriter.Write(Message);
                }
            }
            catch (Exception ex)
            {
                MainForm.ConsoleWrite($"[Bouyomi]{ex}");
            }
            MainForm.ConsoleWrite("[Setting]棒読みちゃん送信テスト完了");
        }

        private void TelopTest_Click(object sender, EventArgs e)
        {
            
            MainForm.ConsoleWrite("[Setting]テロップ送信テスト開始");
            string Text = "0, - テスト - ,これはQuakeMapFastのTelop送信テストです。,60,70,80,White,80,90,100,White,False,30,1000";
            MainForm.ConsoleWrite("[Telop]Text:" + Text);
            IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31401);
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(IPEndPoint);
                    using (NetworkStream networkStream = tcpClient.GetStream())
                    {
                        byte[] Bytes = new byte[4096];
                        Bytes = Encoding.UTF8.GetBytes(Text);
                        networkStream.Write(Bytes, 0, Bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm.ConsoleWrite("[Telop]" + ex.ToString());
            }
            MainForm.ConsoleWrite("[Setting]テロップ送信テスト完了"); ;
        }
    }
}
