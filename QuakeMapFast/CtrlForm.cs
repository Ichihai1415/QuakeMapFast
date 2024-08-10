using Newtonsoft.Json.Linq;
using QuakeMapFast.Properties;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuakeMapFast.Conv;
using static QuakeMapFast.DataPro;
using static QuakeMapFast.Func;

namespace QuakeMapFast
{
    public partial class CtrlForm : Form
    {
        public CtrlForm()
        {
            InitializeComponent();
        }

        /*
         更新時確認
         
         ↓のバージョン
         アセンブリ
         README.md
         (JSON-sample.zip(...\json\P2Pquake)更新時にResourceのCommentにバージョンを書いておく
         */
        public static readonly string version = "0.2.3";
        readonly int[] ignoreCode = { 554, 555, 561, 9611 };//表示しない
        public static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
        string latestID = "";
        public static bool debug = false;
        public static bool readJSON = false;
        public static FontFamily font;

        public static DataView view_all = new DataView();


        private async void CtrlForm_Load(object sender, EventArgs e)
        {
            //ConWrite($"");
            ConWrite($"[CtrlForm_Load]起動しました");
            ConWrite($"/////QuakeMapFast v{version}/////\nhttps://github.com/Ichihai1415/QuakeMapFast\nこのコンソールを閉じるとQuakeMapFastが終了します。\nその他READMEを参照してください。", ConsoleColor.Cyan);

            string appDataReadmePath = config.FilePath.Replace("user.config", "readme.txt");
            if (!File.Exists(appDataReadmePath))//初回/更新時
            {
                ConWrite("[CtrlForm_Load]初回または更新等を検知しました");
                Settings.Default.Window_Size = new Size(0, 0);//変えないとSaveで作られない
                Settings.Default.Save();//ディレクトリとか作成
                Settings.Default.Reset();//一応直しとく(初期化して保存するため)
                Settings.Default.Save();//一応保存(初期化したのを保存、UserSetting.xmlがあれば後で上書き)
                ConWrite("[CtrlForm_Load]設定は右クリックメニューからできます。");
                File.WriteAllText(appDataReadmePath, Resources.AppData_README);
                ConWrite($"[CtrlForm_Load]AppData-readmeファイル(\"{appDataReadmePath}\")をコピーしました");
            }
            if (File.Exists("UserSetting.xml"))//AppDataに保存
            {
                File.Copy("UserSetting.xml", config.FilePath, true);
                ConWrite($"[CtrlForm_Load]以前の設定を復元しました。");
            }
            File.WriteAllText("AppDataPath.txt", config.FilePath);
            ConWrite($"[CtrlForm_Load]\"AppDataPath.txt\"に設定ファイルのパスを保存しました");

            if (!Directory.Exists("Font"))
            {
                Directory.CreateDirectory("Font");
                ConWrite($"[CtrlForm_Load]Fontフォルダを作成しました");
            }
            if (!File.Exists("Font\\Koruri-Regular.ttf"))
            {
                File.WriteAllBytes("Font\\Koruri-Regular.ttf", Resources.Koruri_Regular);
                ConWrite($"[CtrlForm_Load]フォントファイル(\"Font\\Koruri-Regular.ttf\")をコピーしました");
            }
            if (!File.Exists("Font\\LICENSE"))
            {
                File.WriteAllText("Font\\LICENSE", Resources.Koruri_LICENSE);
                ConWrite($"[CtrlForm_Load]フォントライセンスファイル(\"Font\\LICENSE\")をコピーしました");
            }
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile("Font\\Koruri-Regular.ttf");
            font = pfc.Families[0];
            ConWrite($"[CtrlForm_Load]フォント確認完了");


            if (!File.Exists("AreaForecastLocalE_GIS_20240520_1.geojson"))
            {
                File.WriteAllText("AreaForecastLocalE_GIS_20240520_1.geojson", Resources.AreaForecastLocalE_GIS_20240520_1);
                ConWrite($"[CtrlForm_Load]マップファイル(AreaForecastLocalE_GIS_20240520_1.geojson)をコピーしました");
            }

            ConWrite($"[CtrlForm_Load]マップファイル確認完了");
            mapjson = JObject.Parse(File.ReadAllText("AreaForecastLocalE_GIS_20240520_1.geojson"));
            ConWrite($"[CtrlForm_Load]マップファイル読み込み完了");

            view_all.Show();

            SettingReload();
            //Debug(); return;//デバッグ時ここをつける(ここ以降行かせない)

            //XPost("test from QuakeMapFast (2)", "D:\\Ichihai1415\\image\\icon\\new - bot.png");
            await Get();
        }

        /// <summary>
        /// WebSocketでデータを受信します。
        /// </summary>
        /// <returns>(なし)</returns>
        public async Task Get()
        {
            while (true)
                try
                {
                    using (ClientWebSocket client = new ClientWebSocket())
                    {
                    connect:
                        await client.ConnectAsync(new Uri("wss://api.p2pquake.net/v2/ws"), CancellationToken.None);
                        ConWrite("[Get]接続しました");
                        while (client.State == WebSocketState.Open)
                        {
                            byte[] buffer = new byte[256 * 1024];//分割されるからこんなに要らないかも
                            int bytesRead = 0;
                            while (true)//受信
                            {
                                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, bytesRead, buffer.Length - bytesRead);
                                WebSocketReceiveResult result = await client.ReceiveAsync(segment, CancellationToken.None);
                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                    ConWrite("[Get]切断されました。再接続します。");
                                    goto connect;
                                }
                                else if (result.MessageType != WebSocketMessageType.Text)//あとBinaryしかない
                                {
                                    ConWrite($"[Get]未対応のタイプです(WebSocketMessageType.{result.MessageType})", ConsoleColor.Red);
                                    continue;
                                }
                                bytesRead += result.Count;
                                if (result.EndOfMessage)
                                    break;
                            }
                            string jsonText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            if (jsonText == string.Empty)
                                continue;
                            if (Settings.Default.Save_JSON)
                            {
                                Directory.CreateDirectory($"output\\json\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}");
                                File.WriteAllText($"output\\json\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.json", jsonText);
                            }
                            JObject json;
                            try
                            {
                                json = JObject.Parse(jsonText);

                                int code = (int)json["code"];
                                string id = (string)json["_id"];
                                string type = (string)json.SelectToken("issue.type");//ないときあるからこれで
                                string codeInfo = P2PInfoCodeName.Keys.Contains(code) ? P2PInfoCodeName[code] : "-";
                                string issueInfo = P2PInfoTypeName.Keys.Contains(type ?? "") ? P2PInfoTypeName[type ?? ""] : "-";
                                ConWrite($"[Get]受信 id:{id} code:{code}{codeInfo} type:{type}{issueInfo}");
                                if (latestID == id)
                                    continue;
                                latestID = id;
                                if (ignoreCode.Contains(code))
                                    continue;
                                ConWrite($"[Get]{jsonText}", ConsoleColor.Green);
                                switch (code)
                                {
                                    case 551:
                                        switch (type)
                                        {
                                            case "ScalePrompt":
                                                ScalePrompt(json);
                                                break;
                                        }
                                        break;
                                    case 556:
                                        EEW(json);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                ConWrite($"[Get](JSON分析失敗)", ex);
                                Directory.CreateDirectory($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}");
                                File.WriteAllText($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.txt", $"{ex}");
                                continue;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConWrite($"[Get]", ex);
                    if (!(ex.Message.Contains("リモート サーバーに接続できません。") || ex.Message.Contains("内部 WebSocket エラーが発生しました。")))
                    {
                        Directory.CreateDirectory($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}");
                        File.WriteAllText($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.txt", $"{ex}");
                    }
                    await Task.Delay(1000);
                    ConWrite("[Get]切断されました。再接続します。");
                }
        }



        /// <summary>
        /// デバッグはここでやるように
        /// </summary>
        /// <remarks>パスは開発者のものです。変える場合、APIの情報リストから<b><u>情報は一つ、最初と最後の[]は付けない</u></b>ようにして抜き出してください。</remarks>
        public void Debug()
        {
            ConWrite("[Debug]デバッグモードです", ConsoleColor.Cyan);
            debug = true;
            //""
            //ScalePrompt(JObject.Parse(File.ReadAllText("C:\\Users\\proje\\source\\repos\\QuakeMapFast\\QuakeMapFast\\bin\\Debug\\Log\\202305\\26\\19\\20230526190603.3438.txt")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2023hukushima-scale-last.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2016kumamoto-scale-0414.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2015ogasawara-scale.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-ogasawara-only.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-tokyo23-only.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2018oosakahokubu-scale-last.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("C:\\Ichihai1415\\source\\vs\\QuakeMapFast\\QuakeMapFast\\bin\\x64\\Debug\\Log\\202401\\01\\16\\20240101161457.8494.txt")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("D:\\Ichihai1415\\data\\json\\P2Pquake\\sc-2023kushiro.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("D:\\Ichihai1415\\data\\json\\P2Pquake\\sc-2023kushiro-edit.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("D:\\Ichihai1415\\data\\json\\P2Pquake\\2024-r6noto-last.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("D:\\Ichihai1415\\data\\json\\P2Pquake\\2024-r6noto-last-edit.json")));
            //EEW(JObject.Parse(File.ReadAllText("C:\\Ichihai1415\\source\\vs\\QuakeMapFast\\QuakeMapFast\\bin\\x64\\Debug\\Log\\202401\\01\\16\\20240101161107.3056.txt")));
        }

        private void SettingReload()
        {
            Settings.Default.Reload();

            WindowSize_Width.Value = Settings.Default.Window_Size.Width;
            WindowSize_Height.Value = Settings.Default.Window_Size.Height;
            WindowLocation_X.Value = Settings.Default.Window_Location.X;
            WindowLocation_Y.Value = Settings.Default.Window_Location.Y;
            Save_JSON.Checked = Settings.Default.Save_JSON;
            Save_Image.Checked = Settings.Default.Save_Image;
            BackGreenTime.Value = Settings.Default.BackGreenTime;
            AutoCopy.Checked = Settings.Default.AutoCopy;

            Bouyomi_Enable.Checked = Settings.Default.Bouyomi_Enable;
            Bouyomi_Voice.Value = Settings.Default.Bouyomi_Voice;
            Bouyomi_Volume.Value = Settings.Default.Bouyomi_Volume;
            Bouyomi_Speed.Value = Settings.Default.Bouyomi_Speed;
            Bouyomi_Tone.Value = Settings.Default.Bouyomi_Tone;

            Telop_Enable.Checked = Settings.Default.Telop_Enable;

            view_all.SettingReload();
            ConWrite("[SettingReload]設定読み込み完了");
        }

        private void SettingSave_Click(object sender, EventArgs e)
        {
            ConWrite("[Setting]設定保存開始");
            Settings.Default.Window_Size = new Size((int)WindowSize_Width.Value, (int)WindowSize_Height.Value);
            Settings.Default.Window_Location = new Point((int)WindowLocation_X.Value, (int)WindowLocation_Y.Value);
            Settings.Default.Save_JSON = Save_JSON.Checked;
            Settings.Default.Save_Image = Save_Image.Checked;
            Settings.Default.BackGreenTime = (int)BackGreenTime.Value;
            Settings.Default.AutoCopy = AutoCopy.Checked;

            Settings.Default.Bouyomi_Enable = Bouyomi_Enable.Checked;
            Settings.Default.Bouyomi_Voice = (short)Bouyomi_Voice.Value;
            Settings.Default.Bouyomi_Volume = (short)Bouyomi_Volume.Value;
            Settings.Default.Bouyomi_Speed = (short)Bouyomi_Speed.Value;
            Settings.Default.Bouyomi_Tone = (short)Bouyomi_Tone.Value;

            Settings.Default.Telop_Enable = Telop_Enable.Checked;
            Settings.Default.Save();
            File.Copy(config.FilePath, "UserSetting.xml", true);
            File.WriteAllText("AppDataPath.txt", config.FilePath);
            ConWrite("[Setting]設定保存終了");
            SettingReload();
        }

        private void SettingReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("リセットしてもよろしいですか？\nリセットすると再起動します。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                ConWrite("[Setting]設定をリセットします");
                Settings.Default.Reset();
                if (File.Exists("UserSetting.xml"))
                    File.Delete("UserSetting.xml");
                if (File.Exists(config.FilePath))
                    File.Delete(config.FilePath);
                Process.Start("QuakeMapFast.exe");
                Application.Exit();
            }
        }

        private void BouyomiTest_Click(object sender, EventArgs e)
        {
            ConWrite("[Setting]棒読みちゃん送信テスト開始");
            Bouyomichan("QuakeMapFast棒読みちゃん読み上げテスト");
            ConWrite("[Setting]棒読みちゃん送信テスト完了");
        }

        private void TelopTest_Click(object sender, EventArgs e)
        {

            ConWrite("[TelopTest_Click]テロップ送信テスト開始");
            Telop("0, - テスト - ,これはQuakeMapFastのTelop送信テストです。,60,70,80,White,80,90,100,White,False,30,1000");
            ConWrite("[TelopTest_Click]テロップ送信テスト完了");
        }

        private void JSONread_Click(object sender, EventArgs e)
        {
            readJSON = true;
            try
            {
                ConWrite("[JSONread_Click]JSON読み込みモードの準備をします");
                File.WriteAllBytes("JSON-sample.zip", Resources.JSON_sample);
                if (Directory.Exists("JSON-sample"))
                    Directory.Delete("JSON-sample", true);
                ZipFile.ExtractToDirectory("JSON-sample.zip", ".");
                File.Delete("JSON-sample.zip");
                ConWrite("[JSONread_Click]JSONのサンプルをコピーしました(\"JSON-sample\"フォルダ)");

                ConWrite("[JSONread_Click]ファイルのパスを入力してください。");
                string jsonText = File.ReadAllText(Console.ReadLine().Replace("\"", ""));
                var json = JObject.Parse(jsonText);

                int code = (int)json["code"];
                string id = (string)json["_id"];
                string type = (string)json["issue"]["type"];
                string codeInfo = P2PInfoCodeName.Keys.Contains(code) ? P2PInfoCodeName[code] : "-";
                string issueInfo = P2PInfoTypeName.Keys.Contains(type ?? "") ? P2PInfoTypeName[type ?? ""] : "-";
                ConWrite($"[JSONreadCk]読み込み code:{code}{codeInfo} type:{type}{issueInfo} id:{id}");
                latestID = id;
                ConWrite(jsonText, ConsoleColor.Green);
                switch (code)
                {
                    case 551:
                        switch (type)
                        {
                            case "ScalePrompt":
                                ScalePrompt(json);
                                break;
                        }
                        break;
                    case 556:
                        EEW(json);
                        break;
                }
            }
            catch (Exception ex)
            {
                ConWrite("[JSONreadCk]", ex);
                Directory.CreateDirectory($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}");
                File.WriteAllText($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.txt", $"{ex}");
            }
            readJSON = false;
        }
    }
}
