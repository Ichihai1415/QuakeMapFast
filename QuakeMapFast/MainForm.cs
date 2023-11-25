using CoreTweet;
using Newtonsoft.Json.Linq;
using QuakeMapFast.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuakeMapFast.Converter;

namespace QuakeMapFast
{
    public partial class MainForm : Form
    {
        /*
         * 更新時確認
         * 
         * ↓のバージョン
         * アセンブリ
         * README.md
         * JSON-sample.zip(F:\色々\json\P2Pquake) ResourceのCommentにバージョンを書いておく
         */
        public static readonly string Version = "0.1.2";//こことアセンブリを変える
        readonly int[] ignoreCode = { 554, 555, 561, 9611 };//表示しない
        public static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
        string LatestID = "";
        long LastTweetID = 0;
        public static bool ReadJSON = false;
        public static bool debug = false;
        DateTime LastTime = DateTime.MinValue;
        public FontFamily font;
        Tokens tokens = null;

        string LastText = "";
        Bitmap LastCanvas = new Bitmap(1920, 1080);

        public MainForm()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //ConsoleWrite("");
            ConsoleWrite("起動しました");
            Console.WriteLine($"/////QuakeMapFast v{Version}/////");

            string AppDataReadmePath = config.FilePath.Replace("user.config", "readme.txt");
            if (!File.Exists(AppDataReadmePath))//初回/更新時
            {
                ConsoleWrite("初回または更新等を検知しました");
                Settings.Default.Window_Size = new Size(0, 0);//変えないとSaveで作られない
                Settings.Default.Save();//ディレクトリとか作成
                Settings.Default.Reset();//一応直しとく
                Settings.Default.Save();//一応保存
                Console.WriteLine("設定は右クリックメニューからできます。");
                File.WriteAllText(AppDataReadmePath, Resources.AppData_README);
                ConsoleWrite($"[Main]AppData-readmeファイル(\"{AppDataReadmePath}\")をコピーしました");
            }
            if (File.Exists("UserSetting.xml"))//AppDataに保存
            {
                File.Copy("UserSetting.xml", config.FilePath, true);
                ConsoleWrite($"[Main]設定ファイルをAppDataにコピー完了");
                Console.WriteLine("以前の設定を復元しました。");
            }
            File.WriteAllText("AppDataPath.txt", config.FilePath);
            ConsoleWrite($"[Main]\"AppDataPath.txt\"に設定ファイルのパスを保存しました");

            SettingReload();

            if (!Directory.Exists("Font"))
            {
                Directory.CreateDirectory("Font");
                ConsoleWrite($"[Main]Fontフォルダを作成しました");
            }
            if (!File.Exists("Font\\Koruri-Regular.ttf"))
            {
                File.WriteAllBytes("Font\\Koruri-Regular.ttf", Resources.Koruri_Regular);
                ConsoleWrite($"[Main]フォントファイル(\"Font\\Koruri-Regular.ttf\")をコピーしました");
            }
            if (!File.Exists("Font\\LICENSE"))
            {
                File.WriteAllText("Font\\LICENSE", Resources.Koruri_LICENSE);
                ConsoleWrite($"[Main]フォントライセンスファイル(\"Font\\LICENSE\")をコピーしました");
            }
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile("Font\\Koruri-Regular.ttf");
            font = pfc.Families[0];
            ConsoleWrite($"[Main]フォント確認完了");

            if (File.Exists("Token.txt"))
            {
                string[] tokens_ = File.ReadAllText("Token.txt").Split(',');
                if (tokens_.Length == 4)
                {
                    tokens = Tokens.Create(tokens_[0], tokens_[1], tokens_[2], tokens_[3]);
                    ConsoleWrite($"[Main]tokenを確認完了");
                }
            }

            ReadJSON = File.Exists("ReadPath.txt");
            if (ReadJSON)
            {
                TSMI_ReadJSON.Text = "JSON読み込みモードをオフにする";
                ConsoleWrite($"[ReadJSON]JSON読み込みモードです。");
                try
                {
                    string path = File.ReadAllText("ReadPath.txt").Replace("\"", "");
                    string jsonText = File.ReadAllText(path);
                    Console.WriteLine($"path:{path}");
                    Console.WriteLine(jsonText);
                    JObject json = JObject.Parse(jsonText);
                    ConsoleWrite($"[ReadJSON]処理開始 code:{json.SelectToken("code")}{P2PInfoCodeName[(int)json.SelectToken("code")]} type:{json.SelectToken("issue.type")}{P2PInfoTypeName[(string)json.SelectToken("issue.type") ?? ""]} id:{json.SelectToken("_id")}");
                    if (ignoreCode.Contains((int)json.SelectToken("code")))
                        goto ReadJSONEnd;
                    if ((string)json.SelectToken("issue.type") == "ScalePrompt")
                        ScalePrompt(json);
                }
                catch (Exception ex)
                {
                    ConsoleWrite($"[ReadJSON]{ex}");
                }
            ReadJSONEnd:;
                ConsoleWrite($"[ReadJSON]処理が終了しました。通常の取得を行います。");
                ReadJSON = false;
                Console.WriteLine($"次回から通常モードにする場合、右クリックメニューの\"JSON読み込みモードをオフにする\"を押すか\"ReadPath.txt\"を削除して再起動してください。。");
            }

            //Debug();//デバッグ時
            //return;//ここの2行をつける(ここ以降行かせない)

            while (true)
                try
                {
                    using (ClientWebSocket client = new ClientWebSocket())
                    {
                        await client.ConnectAsync(new Uri("wss://api.p2pquake.net/v2/ws"), CancellationToken.None);
                        ConsoleWrite("[Main]接続しました");
                        while (client.State == WebSocketState.Open)
                        {
                            byte[] buffer = new byte[1024 * 1024];//分割されるからこんなに要らないかも
                            int bytesRead = 0;
                            while (true)
                            {
                                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, bytesRead, buffer.Length - bytesRead);
                                WebSocketReceiveResult result = await client.ReceiveAsync(segment, CancellationToken.None);
                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                    break;
                                }
                                bytesRead += result.Count;
                                if (result.EndOfMessage)
                                    break;
                            }
                            string jsonText = Encoding.UTF8.GetString(buffer);
                            if (Settings.Default.Save_JSON)
                            {
                                if (!Directory.Exists($"Log"))
                                    Directory.CreateDirectory($"Log");
                                if (!Directory.Exists($"Log\\{DateTime.Now:yyyyMM}"))
                                    Directory.CreateDirectory($"Log\\{DateTime.Now:yyyyMM}");
                                if (!Directory.Exists($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}"))
                                    Directory.CreateDirectory($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}");
                                if (!Directory.Exists($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}"))
                                    Directory.CreateDirectory($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}");
                                File.WriteAllText($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.txt", jsonText);
                            }
                            JObject json;
                            try
                            {
                                json = JObject.Parse(jsonText);
                            }
                            catch (Exception ex)
                            {
                                ConsoleWrite($"[Main](JSON変換失敗){ex}");
                                if (!Directory.Exists($"Log"))
                                    Directory.CreateDirectory($"Log");
                                if (!Directory.Exists($"Log\\Error"))
                                    Directory.CreateDirectory($"Log\\Error");
                                if (!Directory.Exists($"Log\\Error\\{DateTime.Now:yyyyMM}"))
                                    Directory.CreateDirectory($"Log\\Error\\{DateTime.Now:yyyyMM}");
                                if (!Directory.Exists($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}"))
                                    Directory.CreateDirectory($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}");
                                File.WriteAllText($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.txt", $"{ex}");
                                continue;
                            }
                            ConsoleWrite($"[Main]受信 code:{json.SelectToken("code")}{P2PInfoCodeName[(int)json.SelectToken("code")]} type:{json.SelectToken("issue.type")}{P2PInfoTypeName[(string)json.SelectToken("issue.type") ?? ""]} id:{json.SelectToken("_id")}");
                            if (LatestID == (string)json.SelectToken("_id"))
                                continue;
                            LatestID = (string)json.SelectToken("_id");
                            if (ignoreCode.Contains((int)json.SelectToken("code")))
                                continue;
                            ConsoleWrite(jsonText);
                            if ((string)json.SelectToken("issue.type") == "ScalePrompt")
                                ScalePrompt(json);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleWrite($"[Main]{ex}");
                    if (!(ex.Message.Contains("リモート サーバーに接続できません。") || ex.Message.Contains("内部 WebSocket エラーが発生しました。")))
                    {
                        if (!Directory.Exists($"Log"))
                            Directory.CreateDirectory($"Log");
                        if (!Directory.Exists($"Log\\Error"))
                            Directory.CreateDirectory($"Log\\Error");
                        if (!Directory.Exists($"Log\\Error\\{DateTime.Now:yyyyMM}"))
                            Directory.CreateDirectory($"Log\\Error\\{DateTime.Now:yyyyMM}");
                        if (!Directory.Exists($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}"))
                            Directory.CreateDirectory($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}");
                        File.WriteAllText($"Log\\Error\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.txt", $"{ex}");
                    }
                    await Task.Delay(1000);
                    ConsoleWrite("[Main]再接続します");
                }
        }

        /// <summary>
        /// デバッグはここでやるように
        /// </summary>
        /// <remarks>パスは開発者のものです。変える場合、APIの情報リストから<b><u>情報は一つ、最初と最後の[]は付けない</u></b>ようにして抜き出してください。</remarks>
        public void Debug()
        {
            ConsoleWrite("[Main]デバッグモードです");
            debug = true;
            //""
            //ScalePrompt(JObject.Parse(File.ReadAllText("C:\\Users\\proje\\source\\repos\\QuakeMapFast\\QuakeMapFast\\bin\\Debug\\Log\\202305\\26\\19\\20230526190603.3438.txt")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2023hukushima-scale-last.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2016kumamoto-scale-0414.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2015ogasawara-scale.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-ogasawara-only.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-tokyo23-only.json")));
            ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2018oosakahokubu-scale-last.json")));

        }

        /// <summary>
        /// 設定を読み込みます。
        /// </summary>
        /// <remarks>ReloadしてUserSetting.xmlに保存します。</remarks>
        public void SettingReload()
        {
            ConsoleWrite($"[Main]設定読み込み開始");
            Settings.Default.Reload();
            if (File.Exists(config.FilePath))
                File.Copy(config.FilePath, "UserSetting.xml", true);
            ClientSize = Settings.Default.Window_Size;
            Location = Settings.Default.Window_Location;
            ConsoleWrite($"[Main]設定読み込み終了");
        }

        /// <summary>
        /// 震度速報
        /// </summary>
        /// <param name="json"></param>
        public async void ScalePrompt(JObject json)
        {
            DateTime StartTime = DateTime.Now;

            DateTime Time = Convert.ToDateTime((string)json.SelectToken("earthquake.time"));
            int MaxIntN = P2PScale2IntN((int)json.SelectToken("earthquake.maxScale"));
            string MaxIntS = P2PScale2IntS((int)json.SelectToken("earthquake.maxScale"));
            Dictionary<string, int> AreaInt = Points2Dic(json, "addr");

            ConsoleWrite("[ScalePrompt]座標計算開始");
            JObject mapjson = JObject.Parse(Resources._20190125_AreaForecastLocalE_GIS_name_0_1);
            double LatSta = 999;
            double LatEnd = -999;
            double LonSta = 999;
            double LonEnd = -999;
            foreach (JToken mapjson_1 in mapjson.SelectToken("features"))
            {
                if (AreaInt.ContainsKey((string)mapjson_1.SelectToken("properties.name")))
                    if ((string)mapjson_1.SelectToken("geometry.type") == "Polygon")
                    {
                        foreach (JToken mapjson_2 in mapjson_1.SelectToken($"geometry.coordinates[0]"))
                        {
                            LatSta = Math.Min(LatSta, (double)mapjson_2.SelectToken("[1]"));
                            LatEnd = Math.Max(LatEnd, (double)mapjson_2.SelectToken("[1]"));
                            LonSta = Math.Min(LonSta, (double)mapjson_2.SelectToken("[0]"));
                            LonEnd = Math.Max(LonEnd, (double)mapjson_2.SelectToken("[0]"));
                        }
                    }
                    else
                    {
                        foreach (JToken mapjson_2 in mapjson_1.SelectToken($"geometry.coordinates"))
                        {
                            foreach (JToken mapjson_3 in mapjson_2.SelectToken("[0]"))
                            {
                                LatSta = Math.Min(LatSta, (double)mapjson_3.SelectToken("[1]"));
                                LatEnd = Math.Max(LatEnd, (double)mapjson_3.SelectToken("[1]"));
                                LonSta = Math.Min(LonSta, (double)mapjson_3.SelectToken("[0]"));
                                LonEnd = Math.Max(LonEnd, (double)mapjson_3.SelectToken("[0]"));
                            }
                        }
                    }
            }
            PointCorrect(ref LatSta, ref LatEnd, ref LonSta, ref LonEnd);//補正
            double Zoom = 1080d / (LatEnd - LatSta);

            ConsoleWrite("[ScalePrompt]画像描画開始");//todo:都道府県線を太く
            Bitmap canvas = new Bitmap(1920, 1080);
            Graphics g = Graphics.FromImage(canvas);
            g.Clear(Color.FromArgb(30, 60, 90));
            foreach (JToken mapjson_1 in mapjson.SelectToken("features"))
            {
                GraphicsPath Maps = new GraphicsPath();
                Maps.Reset();
                Maps.StartFigure();
                if (mapjson_1.SelectToken($"geometry.coordinates") == null)
                    continue;
                if ((string)mapjson_1.SelectToken("geometry.type") == "Polygon")
                {
                    List<Point> points = new List<Point>();
                    foreach (JToken mapjson_2 in mapjson_1.SelectToken($"geometry.coordinates[0]"))
                        points.Add(new Point((int)(((double)mapjson_2.SelectToken("[0]") - LonSta) * Zoom), (int)((LatEnd - (double)mapjson_2.SelectToken("[1]")) * Zoom)));
                    if (points.Count > 2)
                        Maps.AddPolygon(points.ToArray());
                }
                else
                {
                    foreach (JToken mapjson_2 in mapjson_1.SelectToken($"geometry.coordinates"))
                    {
                        List<Point> points = new List<Point>();
                        foreach (JToken mapjson_3 in mapjson_2.SelectToken("[0]"))
                            points.Add(new Point((int)(((double)mapjson_3.SelectToken("[0]") - LonSta) * Zoom), (int)((LatEnd - (double)mapjson_3.SelectToken("[1]")) * Zoom)));
                        if (points.Count > 2)
                            Maps.AddPolygon(points.ToArray());
                    }
                }
                if (AreaInt.ContainsKey((string)mapjson_1.SelectToken("properties.name")))
                    g.FillPath(IntN2Brush(AreaInt[(string)mapjson_1.SelectToken("properties.name")]), Maps);
                else
                    g.FillPath(new SolidBrush(Color.FromArgb(60, 90, 120)), Maps);
                g.DrawPath(new Pen(Color.FromArgb(255, 255, 255), 2), Maps);
            }
            ConsoleWrite("[ScalePrompt]情報描画開始");

            g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);

            g.DrawString("震度速報", new Font(font, 50), Brushes.White, 1090, 10);
            g.DrawString(Time.ToString("yyyy/MM/dd HH:mm"), new Font(font, 30), Brushes.White, 1095, 85);


            Pen pen = new Pen(IntN2Brush(MaxIntN), 51)
            {
                LineJoin = LineJoin.Round
            };
            g.DrawRectangle(pen, 1125, 175, 750, 150);
            g.FillRectangle(IntN2Brush(MaxIntN), 1150, 200, 700, 100);
            g.DrawString("最大震度", new Font(font, 50), IntN2TextBrush(MaxIntN), 1150, 240);
            if (MaxIntS.Contains("弱") || MaxIntS.Contains("強"))
                g.DrawString(MaxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(MaxIntN), 1550, 175);
            else
                g.DrawString(MaxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(MaxIntN), 1600, 175);

            string MaxIntAreas = "";
            foreach (KeyValuePair<string, int> area in AreaInt)
            {
                if (area.Value == MaxIntN)
                    MaxIntAreas += $"{area.Key}\n";
            }
            g.DrawString(MaxIntAreas, new Font(font, 40), Brushes.White, 1100, 360);

            g.FillRectangle(Brushes.Black, 1080, 900, 840, 180);
            g.DrawString("日本地図データ:気象庁\n世界地図データ:National Earth\nそれぞれ加工して使用\nデータ:気象庁", new Font(font, 20), Brushes.White, 1090, 910);
            g.DrawImage(Resources.IntLegend, 1500, 906, 410, 164);
            if (debug || ReadJSON)
                g.DrawString("《現在の情報ではありません》", new Font(font, 50), Brushes.White, 0, 0);

            g.Dispose();
            BackgroundImage = canvas;
            DateTime SaveTime = DateTime.Now;
            if (Settings.Default.Save_Image)
            {
                if (!Directory.Exists($"output"))
                    Directory.CreateDirectory($"output");
                if (!Directory.Exists($"output\\{SaveTime:yyyyMM}"))
                    Directory.CreateDirectory($"output\\{SaveTime:yyyyMM}");
                if (!Directory.Exists($"output\\{SaveTime:yyyyMM}\\{SaveTime:dd}"))
                    Directory.CreateDirectory($"output\\{SaveTime:yyyyMM}\\{SaveTime:dd}");
                canvas.Save($"output\\{SaveTime:yyyyMM}\\{SaveTime:dd}\\{SaveTime:yyyyMMddHHmmss.ff}.png", ImageFormat.Png);
                ConsoleWrite($"[ScalePrompt]output\\{SaveTime:yyyyMM}\\{SaveTime:dd}に保存しました");
            }
            string IntsArea = Point2String(json, "addr");
            string IntsArea_Max3 = Point2String(json, "addr", MaxIntN - 2);//最大震度から3階級(Max6->6,5,4)
            string Text = $"震度速報【最大震度{MaxIntS}】{Time:yyyy/MM/dd HH:mm}\n{IntsArea}";
            if (Text.Length > 120)
                Text = Text.Remove(120, Text.Length - 120) + "…";
            BouyomiChanSocketSend($"震度速報、{IntsArea_Max3.Replace("\n", "").Replace("《", "、").Replace("》", "、").Replace(" ", "、")}");
            TelopSocketSend($"0,震度速報【最大震度{MaxIntS}】,{IntsArea.Replace("\n", "")},{Int2TelopColor(MaxIntN)},False,60,1000");
            if (debug || ReadJSON)
                TelopSocketSend($"0,《現在の情報ではありません》震度速報【最大震度{MaxIntS}】,{IntsArea.Replace("\n", "")},{Int2TelopColor(MaxIntN)},False,60,1000");
            Clipboard.SetText(Text);
            await Task.Delay(1);//テキストがクリップボードに保存されないから仮
            Clipboard.SetImage(canvas);

            LastText = Text;
            LastCanvas = (Bitmap)canvas.Clone();
            ConsoleWrite($"[ScalePrompt]処理時間:{(DateTime.Now - StartTime).TotalMilliseconds}ms");
            //throw new Exception("aa");
        }

        public void Destination(JObject json)
        {

        }

        public void ScaleAndDestination(JObject json)
        {

        }

        public void DetailScale(JObject json)
        {

        }

        public void Foreign(JObject json)
        {

        }

        public void Other(JObject json)
        {

        }

        public void Tsunami(JObject json)
        {

        }

        public void EEW(JObject json)
        {

        }

        /// <summary>
        /// ツイートします
        /// </summary>
        /// <param name="Text">ツイートするテキスト</param>
        /// <param name="Time">リプライ判別用発生時刻</param>
        /// <param name="ImagePath">画像を送信する場合の画像のパス</param>
        /// <remarks>Timeが最後に送信した情報の発生時刻と一致する場合リプライします</remarks>
        public async void Tweet(string Text, DateTime Time, string ImagePath = "")
        {
            /*
            if (tokens == null)
                return;
            if (debug)
            {
                ConsoleWrite("[Tweet]デバッグモードのためツイートはしません。");
                return;
            }
            if (ReadJSON)
            {
                ConsoleWrite("[Tweet]JSON読み込みモードのためツイートはしません。");
                return;
            }
            ConsoleWrite("[Tweet]ツイート送信開始");
            ConsoleWrite("[Tweet]Text:" + Text);
            bool Reply = Time == LastTime;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                Status status;
                if (ImagePath == "")
                    if (Reply)
                        status = await tokens.Statuses.UpdateAsync(new
                        {
                            status = Text,
                            in_reply_to_status_id = LastTweetID
                        });
                    else
                        status = await tokens.Statuses.UpdateAsync(new
                        {
                            status = Text
                        });
                else
                {
                    ConsoleWrite("[Tweet]Image:" + ImagePath);
                    ConsoleWrite("[Tweet]画像送信開始");
                    MediaUploadResult mur = await tokens.Media.UploadAsync(media: new FileInfo(ImagePath));
                    ConsoleWrite("[Tweet]画像送信終了　ツイート開始");
                    if (Reply)
                        status = await tokens.Statuses.UpdateAsync(new
                        {
                            status = Text,
                            media_ids = mur.MediaId,
                            in_reply_to_status_id = LastTweetID
                        });
                    else
                        status = await tokens.Statuses.UpdateAsync(new
                        {
                            status = Text,
                            media_ids = mur.MediaId
                        });
                }
                LastTweetID = status.Id;
            }
            catch (Exception ex)
            {
                ConsoleWrite($"[Tweet]{ex}");
            }
            LastTime = Time;
            ConsoleWrite("[Tweet]ツイート送信終了");
            */
        }

        /// <summary>
        /// TelopにSocket送信します
        /// </summary>
        /// <param name="Text">Telopに送信するテキスト(Telop方式)</param>
        public static void TelopSocketSend(string Text)
        {
            if (!Settings.Default.Telop_Enable)
                return;
            ConsoleWrite("[Telop]テロップ送信開始");
            ConsoleWrite("[Telop]Text:" + Text);
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
                ConsoleWrite("[Telop]" + ex.ToString());
            }
            ConsoleWrite("[Telop]テロップ送信終了");
        }
        /// <summary>
        /// 棒読みちゃんにSocket送信します
        /// </summary>
        /// <param name="Text">読み上げさせるテキスト</param>
        public void BouyomiChanSocketSend(string Text)
        {
            if (!Settings.Default.Bouyomi_Enable)
                return;
            ConsoleWrite("[Bouyomi]棒読みちゃん送信開始");
            if (debug)
                Text = "デバッグ中です。現在の情報ではありません。" + Text;
            if (ReadJSON)
                Text = "JSON読み込みモード中です。現在の情報ではありません。" + Text;
            ConsoleWrite("[Bouyomi]Text:" + Text);
            try
            {
                byte[] Message = Encoding.UTF8.GetBytes(Text);
                int Length = Message.Length;
                byte Code = 0;
                short Command = 0x0001;
                short Speed = Settings.Default.Bouyomi_Speed;
                short Tone = Settings.Default.Bouyomi_Tone;
                short Volume = Settings.Default.Bouyomi_Volume;
                short Voice = Settings.Default.Bouyomi_Voice;
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
                ConsoleWrite($"[Bouyomi]{ex}");
            }
            ConsoleWrite("[Bouyomi]棒読みちゃん送信完了");
        }

        /// <summary>
        /// コンソールにタイムスタンプ付きで出力します
        /// </summary>
        /// <param name="Text">表示するテキスト</param>
        public static void ConsoleWrite(string Text)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.ffff} {Text}");
        }

        private void TSMI_TextCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(LastText);
        }

        private void TSMI_ImageCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(LastCanvas);
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

        private void TSMI_Setting_Click(object sender, EventArgs e)
        {
            SettingForm setting = new SettingForm();
            setting.FormClosed += SettingForm_MainForm_Closed;
            setting.Show();
        }

        public void SettingForm_MainForm_Closed(object sender, FormClosedEventArgs e)
        {
            SettingReload();
        }

        private void TSMI_ReadJSON_Click(object sender, EventArgs e)
        {
            if (TSMI_ReadJSON.Text == "JSON読み込みモードをオフにする")
            {
                ConsoleWrite("[Main]JSON読み込みモードをオフにします");
                File.Delete("ReadPath.txt");
                ConsoleWrite("[Main]再起動します");
                Application.Restart();
            }
            else
            {
                ConsoleWrite("[Main]JSON読み込みモードの準備をします");
                File.WriteAllBytes("JSON-sample.zip", Resources.JSON_sample);
                if (Directory.Exists("JSON-sample"))
                    Directory.Delete("JSON-sample", true);
                ZipFile.ExtractToDirectory("JSON-sample.zip", ".");
                File.Delete("JSON-sample.zip");
                ConsoleWrite("[Main]JSONのサンプルをコピーしました(\"JSON-sample\"フォルダ)");

                File.WriteAllText("ReadPath.txt", "");
                Console.WriteLine("\"ReadPath.txt\"にJSONデータのパスを入力してください。メモ帳を起動しています…");
                Process.Start("notepad.exe", "ReadPath.txt");
                Console.WriteLine("パスは実行ファイルからの相対パス(\"JSON-sample\\sample.json\"など)か絶対パスで入力してください。");
                Console.WriteLine("入力し終わったら保存してソフトを再起動してください。");
            }
        }
    }
}
