using CoreTweet;
using Newtonsoft.Json.Linq;
using QuakeMapFast.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static QuakeMapFast.Converter;

namespace QuakeMapFast
{
    public partial class Form1 : Form
    {
        readonly int[] ignoreID = { 554, 555, 561, 9611 };//表示しない
        string LatestID = "";
        long LastTweetID = 0;
        bool debug = false;
        DateTime LastTime = DateTime.MinValue;
        Tokens tokens;
        public Form1()
        {
            InitializeComponent();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("Token.txt"))
            {
                string[] tokens_ = File.ReadAllText("Token.txt").Split(',');
                if (tokens_.Length == 4)
                    tokens = Tokens.Create(tokens_[0], tokens_[1], tokens_[2], tokens_[3]);
            }
            else
                File.WriteAllText("Token.txt", "");

            Debug();//デバッグ時
            return;//ここの2行をつける(ここ以降行かせない)

            while (true)
                try
                {
                    using (ClientWebSocket client = new ClientWebSocket())
                    {
                        await client.ConnectAsync(new Uri("wss://api.p2pquake.net/v2/ws"), CancellationToken.None);
                        while (client.State == WebSocketState.Open)
                        {
                            byte[] buffer = new byte[1024 * 1024];
                            WebSocketReceiveResult result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            if (result.MessageType == WebSocketMessageType.Text)
                            {
                                string jsonText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                if (!Directory.Exists($"Log"))
                                    Directory.CreateDirectory($"Log");
                                if (!Directory.Exists($"Log\\{DateTime.Now:yyyyMM}"))
                                    Directory.CreateDirectory($"Log\\{DateTime.Now:yyyyMM}");
                                if (!Directory.Exists($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}"))
                                    Directory.CreateDirectory($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}");
                                if (!Directory.Exists($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}"))
                                    Directory.CreateDirectory($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}");
                                File.WriteAllText($"Log\\{DateTime.Now:yyyyMM}\\{DateTime.Now:dd}\\{DateTime.Now:HH}\\{DateTime.Now:yyyyMMddHHmmss.ffff}.txt", jsonText);
                                JObject json;
                                try
                                {
                                    json = JObject.Parse(jsonText);
                                }
                                catch (Exception ex)
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
                                    continue;
                                }
                                if (LatestID == (string)json.SelectToken("_id"))
                                    continue;
                                LatestID = (string)json.SelectToken("_id");
                                if (ignoreID.Contains((int)json.SelectToken("code")))
                                    continue;
                                if ((string)json.SelectToken("issue.type") == "ScalePrompt")
                                {
                                    ScalePrompt(json);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("リモート サーバーに接続できません。") || ex.Message.Contains("内部 WebSocket エラーが発生しました。"))
                        continue;
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
        }
        /// <summary>
        /// デバッグはここでやるように
        /// </summary>
        public void Debug()
        {
            debug = true;
            //""
            //ScalePrompt(JObject.Parse(File.ReadAllText("C:\\Users\\proje\\source\\repos\\QuakeMapFast\\QuakeMapFast\\bin\\Debug\\Log\\202305\\26\\19\\20230526190603.3438.txt")));
            ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2023hukushima-scale-last.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2016kumamoto-scale-0414.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2015ogasawara-scale.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-ogasawara-only.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-tokyo23-only.json")));

        }
        public void ScalePrompt(JObject json)
        {

            DateTime Time = Convert.ToDateTime((string)json.SelectToken("earthquake.time"));
            int MaxIntN = P2PScale2IntN((int)json.SelectToken("earthquake.maxScale"));
            string MaxIntS = P2PScale2IntS((int)json.SelectToken("earthquake.maxScale"));
            Dictionary<string, int> AreaInt = Points2Dic(json, "addr");

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

            g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);

            g.DrawString("震度速報", new Font("Koruri Regular", 50), Brushes.White, 1090, 10);
            g.DrawString(Time.ToString("yyyy/MM/dd HH:mm"), new Font("Koruri Regular", 30), Brushes.White, 1095, 85);


            Pen pen = new Pen(IntN2Brush(MaxIntN), 51)
            {
                LineJoin = LineJoin.Round
            };
            g.DrawRectangle(pen, 1125, 175, 750, 150);
            g.FillRectangle(IntN2Brush(MaxIntN), 1150, 200, 700, 100);
            g.DrawString("最大震度", new Font("Koruri Regular", 50), IntN2TextBrush(MaxIntN), 1150, 240);
            if (MaxIntS.Contains("弱") || MaxIntS.Contains("強"))
                g.DrawString(MaxIntS, new Font("Koruri Regular", 90, FontStyle.Bold), IntN2TextBrush(MaxIntN), 1550, 175);
            else
                g.DrawString(MaxIntS, new Font("Koruri Regular", 90, FontStyle.Bold), IntN2TextBrush(MaxIntN), 1600, 175);

            string MaxIntAreas = "";
            foreach (KeyValuePair<string, int> area in AreaInt)
            {
                if (area.Value == MaxIntN)
                    MaxIntAreas += $"{area.Key}\n";
            }
            g.DrawString(MaxIntAreas, new Font("Koruri Regular", 40), Brushes.White, 1100, 360);

            g.FillRectangle(Brushes.Black, 1080, 900, 840, 180);
            g.DrawString("日本地図データ:気象庁\n世界地図データ:National Earth\nそれぞれ加工して使用\nデータ:気象庁", new Font("Koruri Regular", 20), Brushes.White, 1090, 910);
            g.DrawImage(Resources.IntLegend, 1500, 906, 410, 164);

            g.Dispose();
            BackgroundImage = canvas;
            DateTime SaveTime = DateTime.Now;
            if (!Directory.Exists($"output"))
                Directory.CreateDirectory($"output");
            if (!Directory.Exists($"output\\{SaveTime:yyyyMM}"))
                Directory.CreateDirectory($"output\\{SaveTime:yyyyMM}");
            if (!Directory.Exists($"output\\{SaveTime:yyyyMM}\\{SaveTime:dd}"))
                Directory.CreateDirectory($"output\\{SaveTime:yyyyMM}\\{SaveTime:dd}");
            canvas.Save($"output\\{SaveTime:yyyyMM}\\{SaveTime:dd}\\{SaveTime:yyyyMMddHHmmss.f}.png", ImageFormat.Png);
            string IntsArea = Point2String(json, "addr");
            string IntsArea_Max3 = Point2String(json, "addr",MaxIntN-2);//最大震度から3階級(Max6->6,5,4)
            string Text = $"震度速報【最大震度{MaxIntS}】{Time:yyyy/MM/dd HH:mm}\n{IntsArea}";
            if (Text.Length > 120)
                Text = Text.Remove(120, Text.Length - 120) + "…";
            BouyomiChanSocketSend($"震度速報、{IntsArea_Max3.Replace("《", "、").Replace("》", "、").Replace(" ", "、")}");
            TelopSocketSend($"0,震度速報【最大震度{MaxIntS}】,{IntsArea},{Int2TelopColor(MaxIntN)},False,60,1000");
            if(debug)//デバッグ時は早く消す//長さに応じて調整
                TelopSocketSend($"0, - テスト - 震度速報【最大震度{MaxIntS}】,{IntsArea},{Int2TelopColor(MaxIntN)},False,30,1000");
            Tweet(Text, Time, $"output\\{SaveTime:yyyyMM}\\{SaveTime:dd}\\{SaveTime:yyyyMMddHHmmss.f}.png");
            Console.WriteLine(Text);

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
        public async void Tweet(string Text, DateTime Time, string ImagePath = "")
        {
            bool Reply = Time == LastTime;
            if (!debug)
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
                        MediaUploadResult mur = await tokens.Media.UploadAsync(media: new FileInfo(ImagePath));
                        if (Reply)
                            status = await tokens.Statuses.UpdateAsync(new
                            {
                                status = Text,
                                media_id = mur.MediaId,
                                in_reply_to_status_id = LastTweetID
                            });
                        else
                            status = await tokens.Statuses.UpdateAsync(new
                            {
                                status = Text,
                                media_id = mur.MediaId
                            });
                    }
                    LastTweetID = status.Id;
                }
                catch
                {

                }
            LastTime = Time;
        }

        public void TelopSocketSend(string Text)
        {
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
                Console.WriteLine(ex);
            }
        }
        public void BouyomiChanSocketSend(string Text)
        {
            try
            {
                byte[] Message = Encoding.UTF8.GetBytes(Text);
                int Length = Message.Length;
                byte Code = 0;
                short Command = 0x0001;
                short Speed = 100;
                short Tone = 150;
                short Volume = 100;
                short Voice = 2;
                using (TcpClient TcpClient = new TcpClient("127.0.0.1", 50001))
                using (NetworkStream NetworkStream = TcpClient.GetStream())
                using (BinaryWriter BinaryWriter = new BinaryWriter(NetworkStream))
                {
                    BinaryWriter.Write(Command);
                    BinaryWriter.Write(Speed);
                    BinaryWriter.Write(Tone);
                    BinaryWriter.Write(Volume);
                    BinaryWriter.Write(Voice);
                    BinaryWriter.Write(Code);
                    BinaryWriter.Write(Length);
                    BinaryWriter.Write(Message);
                }
            }
            catch
            {

            }
        }
    }
}
