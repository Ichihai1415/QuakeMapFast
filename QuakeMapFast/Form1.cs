using Newtonsoft.Json.Linq;
using QuakeMapFast.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QuakeMapFast
{
    public partial class Form1 : Form
    {
        readonly int[] ignoreID = { 554, 555, 561, 9611 };//表示しない
        string LatestID = "";

        public Form1()
        {
            InitializeComponent();
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            //""
            //ScalePrompt(JObject.Parse(File.ReadAllText("C:\\Users\\proje\\source\\repos\\QuakeMapFast\\QuakeMapFast\\bin\\Debug\\Log\\202305\\26\\19\\20230526190603.3438.txt")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2023hukushima-scale-last.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2016kumamoto-scale-0414.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\2015ogasawara-scale.json")));
            //ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-ogasawara-only.json")));
            ScalePrompt(JObject.Parse(File.ReadAllText("F:\\色々\\json\\P2Pquake\\scale-tokyo23-only.json")));
            return;

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
        public void DrawMap(string MapKind)
        {





        }
        public void ScalePrompt(JObject json)
        {

            DateTime Time = Convert.ToDateTime((string)json.SelectToken("earthquake.time"));
            int MaxIntN = Converter.P2PScale2IntN((int)json.SelectToken("earthquake.maxScale"));
            string MaxIntS = Converter.P2PScale2IntS((int)json.SelectToken("earthquake.maxScale"));
            Dictionary<string, int> AreaInt = Converter.Points2Dic(json, "addr");


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
            LatSta -= (LatEnd - LatSta) / 20;//差の1/20余白追加
            LatEnd += (LatEnd - LatSta) / 20;
            LonSta -= (LonEnd - LonSta) / 20;
            LonEnd += (LonEnd - LonSta) / 20;
            if (LatEnd - LatSta < 3)//緯度差を最大3に
            {
                double correction = (3 - (LatEnd - LatSta)) / 2d;
                LatSta -= correction;
                LatEnd += correction;
            }
            if (LonEnd - LonSta > LatEnd - LatSta)//大きいほうに合わせる
            {
                double correction = ((LonEnd - LonSta) - (LatEnd - LatSta)) / 2d;
                LatSta -= correction;
                LatEnd += correction;
            }
            else if (LonEnd - LonSta < LatEnd - LatSta)
            {
                double correction = ((LatEnd - LatSta) - (LonEnd - LonSta)) / 2d;
                LonSta -= correction;
                LonEnd += correction;
            }
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
                    g.FillPath(Converter.IntN2Brush(AreaInt[(string)mapjson_1.SelectToken("properties.name")]), Maps);
                else
                    g.FillPath(new SolidBrush(Color.FromArgb(60, 90, 120)), Maps);
                g.DrawPath(new Pen(Color.FromArgb(255, 255, 255), 2), Maps);
            }

            g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);

            g.DrawString("震度速報", new Font("Koruri Regular", 50), Brushes.White, 1090, 10);
            g.DrawString(Time.ToString("yyyy/MM/dd HH:mm"), new Font("Koruri Regular", 30), Brushes.White, 1095, 85);


            Pen pen = new Pen(Converter.IntN2Brush(MaxIntN), 51)
            {
                LineJoin = LineJoin.Round
            };
            g.DrawRectangle(pen, 1125, 175, 750, 150);
            g.FillRectangle(Converter.IntN2Brush(MaxIntN), 1150, 200, 700, 100);
            g.DrawString("最大震度", new Font("Koruri Regular", 50), Converter.IntN2TextBrush(MaxIntN), 1150, 240);
            if (MaxIntS.Contains("弱") || MaxIntS.Contains("強"))
                g.DrawString(MaxIntS, new Font("Koruri Regular", 90, FontStyle.Bold), Converter.IntN2TextBrush(MaxIntN), 1550, 175);
            else
                g.DrawString(MaxIntS, new Font("Koruri Regular", 90, FontStyle.Bold), Converter.IntN2TextBrush(MaxIntN), 1600, 175);

            string MaxIntAreas = "";
            foreach (KeyValuePair<string, int> area in AreaInt)
            {
                if (area.Value == MaxIntN)
                    MaxIntAreas += $"{area.Key}\n";
            }
            g.DrawString(MaxIntAreas, new Font("Koruri Regular", 40), Brushes.White, 1100, 360);

            g.FillRectangle(Brushes.Black, 1080, 900, 840, 180);
            g.DrawString("日本地図データ:気象庁\n世界地図データ:National Earth\nそれぞれ加工して使用", new Font("Koruri Regular", 20), Brushes.White, 1090, 910);
            g.DrawImage(Resources.IntLegend, 1500, 906, 410, 164);

            g.Dispose();
            BackgroundImage = canvas;
            canvas.Save("img.png", ImageFormat.Png);
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
    }
}
