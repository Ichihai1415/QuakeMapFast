using Newtonsoft.Json.Linq;
using QuakeMapFast.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuakeMapFast.Conv;
using static QuakeMapFast.CtrlForm;
using static QuakeMapFast.Func;

namespace QuakeMapFast
{
    internal class DataPro
    {
        public static JObject mapjson = new JObject();

        /// <summary>
        /// マップを描画します。塗りつぶしも実行します。
        /// </summary>
        /// <param name="areaColor">地区ごとの色</param>
        /// <param name="hypoLat">震源緯度</param>
        /// <param name="hypoLon">震源経度</param>
        /// <returns>マップの画像</returns>
        public static Bitmap DrawMap(Dictionary<string, SolidBrush> areaColor, double hypoLat = -200, double hypoLon = -200)
        {
            ConWrite("[DrawMap]座標計算開始");
            double latSta = 999;
            double latEnd = -999;
            double lonSta = 999;
            double lonEnd = -999;
            foreach (var features in mapjson["features"].Where(features => areaColor.ContainsKey((string)features["properties"]["name"])))
            {
                if ((string)features["geometry"]["type"] == "Polygon")
                {
                    foreach (var coordinate in features["geometry"]["coordinates"][0])
                    {
                        latSta = Math.Min(latSta, (double)coordinate[1]);
                        latEnd = Math.Max(latEnd, (double)coordinate[1]);
                        lonSta = Math.Min(lonSta, (double)coordinate[0]);
                        lonEnd = Math.Max(lonEnd, (double)coordinate[0]);
                    }
                }
                else
                {
                    foreach (var coordinate in features["geometry"]["coordinates"].SelectMany(coordinate => coordinate[0]))
                    {
                        latSta = Math.Min(latSta, (double)coordinate[1]);
                        latEnd = Math.Max(latEnd, (double)coordinate[1]);
                        lonSta = Math.Min(lonSta, (double)coordinate[0]);
                        lonEnd = Math.Max(lonEnd, (double)coordinate[0]);
                    }
                }
            }

            PointCorrect(ref latSta, ref latEnd, ref lonSta, ref lonEnd);
            double zoom = 1080 / (latEnd - latSta);
            ConWrite("[DrawMap]描画開始");
            var bitmap = new Bitmap(1920, 1080);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(30, 60, 90));
                var gPath = new GraphicsPath();
                foreach (var features in mapjson["features"])
                {
                    gPath.Reset();
                    gPath.StartFigure();
                    //if (features["geometry"]["coordinates"] == null)
                    if (features["geometry"].Count() == 0)
                        continue;
                    if ((string)features["geometry"]["type"] == "Polygon")
                    {
                        var points = features["geometry"]["coordinates"][0].Select(coordinate => new Point((int)(((double)coordinate[0] - lonSta) * zoom), (int)((latEnd - (double)coordinate[1]) * zoom)));
                        if (points.Count() > 2)
                            gPath.AddPolygon(points.ToArray());
                    }
                    else
                    {
                        foreach (var coordinates in features["geometry"]["coordinates"])
                        {
                            var points = coordinates[0].Select(coordinate => new Point((int)(((double)coordinate[0] - lonSta) * zoom), (int)((latEnd - (double)coordinate[1]) * zoom)));
                            if (points.Count() > 2)
                                gPath.AddPolygon(points.ToArray());
                        }
                    }
                    if (areaColor.ContainsKey((string)features["properties"]["name"]))
                        g.FillPath(areaColor[(string)features["properties"]["name"]], gPath);
                    else
                        g.FillPath(new SolidBrush(Color.FromArgb(60, 90, 120)), gPath);
                    g.DrawPath(new Pen(Color.FromArgb(200, 255, 255, 255), 2), gPath);

                }
                gPath.Dispose();

                if (hypoLat != -200)
                {
                    var center = new Point((int)((hypoLon - lonSta) * zoom), (int)((latEnd - hypoLat) * zoom));
                    g.DrawLine(new Pen(Color.Red, 11), center.X - 50, center.Y - 50, center.X + 50, center.Y + 50);
                    g.DrawLine(new Pen(Color.Red, 11), center.X + 50, center.Y - 50, center.X - 50, center.Y + 50);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// 震度速報
        /// </summary>
        /// <param name="json">描画するデータ</param>
        public static void ScalePrompt(JObject json)
        {
            DateTime time = DateTime.Parse((string)json["earthquake"]["time"]);
            int maxIntN = P2PScale2IntN((int)json["earthquake"]["maxScale"]);
            string maxIntS = P2PScale2IntS((int)json["earthquake"]["maxScale"]);
            Dictionary<string, int> areaInt = Points2Dic(json["points"], "addr");

            Bitmap bitmap = DrawMap(areaInt.ToDictionary(x => x.Key, x => IntN2Brush(x.Value)));
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
                g.DrawString("震度速報", new Font(font, 50), Brushes.White, 1090, 10);
                g.DrawString(time.ToString("yyyy/MM/dd HH:mm"), new Font(font, 30), Brushes.White, 1095, 85);

                Pen pen = new Pen(IntN2Brush(maxIntN), 51)
                {
                    LineJoin = LineJoin.Round
                };
                g.DrawRectangle(pen, 1125, 175, 750, 150);
                g.FillRectangle(IntN2Brush(maxIntN), 1150, 200, 700, 100);
                g.DrawString("最大震度", new Font(font, 50), IntN2TextBrush(maxIntN), 1150, 240);
                if (maxIntS.Contains("弱") || maxIntS.Contains("強"))
                    g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1550, 175);
                else
                    g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1600, 175);

                string maxIntAreas = string.Join(Environment.NewLine, areaInt.Where(x => x.Value == maxIntN).Select(x => x.Key));
                g.DrawString(maxIntAreas, new Font(font, 40), Brushes.White, 1100, 360);

                g.FillRectangle(Brushes.Black, 1080, 900, 840, 180);
                g.DrawString("日本地図データ:気象庁\n世界地図データ:Natural Earth\nそれぞれ加工して使用\nデータ:気象庁", new Font(font, 20), Brushes.White, 1090, 910);
                g.DrawImage(Resources.IntLegend, 1500, 906, 410, 164);
                if (debug || readJSON)
                    using (var textGP = new GraphicsPath())
                    {
                        textGP.AddString("《現在の情報ではありません》", font, 0, 140, new Point(-70, 400), StringFormat.GenericDefault);
                        g.FillPath(Brushes.White, textGP);
                        g.DrawPath(new Pen(Color.Black, 3), textGP);
                    }
            }
            ConWrite("[ScalePrompt]画像描画完了");
            DateTime saveTime = DateTime.Now;
            if (Settings.Default.Save_Image)
            {
                Directory.CreateDirectory($"output\\{saveTime:yyyyMM}\\{saveTime:dd}");
                bitmap.Save($"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png", ImageFormat.Png);
                ConWrite($"[Draw]output\\{saveTime:yyyyMM}\\{saveTime:dd}に保存しました");
            }

            string intsArea = Point2String(json, "addr");
            string intsArea_Max3 = Point2String(json, "addr", maxIntN - 2);//最大震度から3階級(Max6->6,5,4)
            string text = $"震度速報【最大震度{maxIntS}】{time:yyyy/MM/dd HH:mm}\n{intsArea}";
            ConWrite(text, ConsoleColor.Cyan);
            if (debug || readJSON)
                Bouyomichan($"QuakeMapFastの読み上げです。デバッグあるいはJSON読み込みモードのため無効です。");
            else
                Bouyomichan($"震度速報、{intsArea_Max3.Replace("\n", "").Replace("《", "、").Replace("》", "、").Replace(" ", "、")}");
            if (debug || readJSON)
                Telop($"0,《現在の情報ではありません》震度速報【最大震度{maxIntS}】,{intsArea.Replace("\n", "")},{Int2TelopColor(maxIntN)},False,10,1000");
            else
                Telop($"0,震度速報【最大震度{maxIntS}】,{intsArea.Replace("\n", "")},{Int2TelopColor(maxIntN)},False,60,1000");
            view_all.ImageChange(bitmap, text);
            if (Settings.Default.AutoCopy)
            {
                Clipboard.SetText(text);
                Task.Delay(100).ConfigureAwait(false);//片方がクリップボードに保存されないから仮
                Clipboard.SetImage(bitmap);
            }
            if (File.Exists("XPosterV2Host - Enable"))
                if (!debug && !readJSON)
                    XPost(text, $"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png");
        }


        public static void Destination(JObject json)
        {

        }

        public static void ScaleAndDestination(JObject json)
        {

        }

        public static void DetailScale(JObject json)
        {

        }

        public static void Foreign(JObject json)
        {

        }

        public static void Other(JObject json)
        {

        }

        public static void Tsunami(JObject json)
        {

        }

        public static void EEW(JObject json)
        {
            if ((bool)json["cancelled"])
                return;

            var earthquake = json["earthquake"];
            var hypocenter = earthquake["hypocenter"];

            DateTime time = DateTime.Parse((string)earthquake["originTime"]);
            Dictionary<string, SolidBrush> areaColor = json["areas"].ToDictionary(area => (string)area["name"], area => P2PScale2isOver6((int)area["scaleFrom"], (int)area["scaleTo"]) ? new SolidBrush(Color.FromArgb(180, 0, 0)) : new SolidBrush(Color.FromArgb(180, 180, 0)));
            List<string> areaWarn = areaColor.Keys.ToList();
            List<string> prefWarn = json["areas"].Select(n => (string)n["pref"]).Distinct().ToList();

            double hLat = (double)hypocenter["latitude"];
            double hLon = (double)hypocenter["longitude"];
            ConWrite("[EEW]画像描画開始");

            Bitmap bitmap = DrawMap(areaColor, hLat, hLon);

            string hypoName = (string)hypocenter["reduceName"];

            string warnAreaInfo1 = "";
            string warnAreaInfo2 = "";
            foreach (var area in json["areas"])
            {
                string minInt = P2PScale2IntS((int)area["scaleFrom"]);
                string maxInt = P2PScale2IntS((int)area["scaleTo"]);
                warnAreaInfo1 += $"{area["name"]}\n";
                if (maxInt == "-")
                    warnAreaInfo2 += $"震度{minInt}程度以上\n";
                else if (minInt == maxInt)
                    warnAreaInfo2 += $"震度{minInt}程度\n";
                else
                    warnAreaInfo2 += $"震度{minInt}～{maxInt}程度\n";
            }
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
                Brush warnTextColor = warnAreaInfo2.Contains("6") ? Brushes.Red : Brushes.Yellow;
                g.DrawString("■■■緊急地震速報■■■", new Font(font, 50), warnTextColor, 1085, 5);
                g.DrawString($"{time:yyyy/MM/dd HH:mm:ss} 警報第{json["issue"]["serial"]}報\n{hypoName} 深さ{hypocenter["depth"]}km  M{hypocenter["magnitude"]}", new Font(font, 30), warnTextColor, 1095, 90);

                g.DrawString(warnAreaInfo1, new Font(font, 30), Brushes.White, 1100, 240);
                g.DrawString(warnAreaInfo2, new Font(font, 30), Brushes.White, 1540, 240);

                g.FillRectangle(Brushes.Black, 1080, 900, 840, 180);
                g.DrawString("日本地図データ:気象庁\n世界地図データ:Natural Earth\nそれぞれ加工して使用\nデータ:気象庁", new Font(font, 20), Brushes.White, 1090, 910);
                //消す場合コメントアウト
                if (debug || readJSON)
                    using (var textGP = new GraphicsPath())
                    {
                        textGP.AddString("《現在の情報ではありません》", font, 0, 140, new Point(-70, 400), StringFormat.GenericDefault);
                        g.FillPath(Brushes.White, textGP);
                        g.DrawPath(new Pen(Color.Black, 3), textGP);
                    }
            }
            ConWrite("[EEW]画像描画完了");
            DateTime saveTime = DateTime.Now;
            if (Settings.Default.Save_Image)
            {
                Directory.CreateDirectory($"output\\{saveTime:yyyyMM}\\{saveTime:dd}");
                bitmap.Save($"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png", ImageFormat.Png);
                ConWrite($"[Draw]output\\{saveTime:yyyyMM}\\{saveTime:dd}に保存しました");
            }

            string text = $"■■緊急地震速報 強い揺れに警戒■■ {time:yyyy/MM/dd HH:mm}\n{string.Join(" ", prefWarn)}";
            ConWrite(text, ConsoleColor.Cyan);
            //Bouyomichan($"緊急地震速報です、次の地域では強い揺れに警戒してください。{string.Join("、", prefWarn)}");
            Telop($"0,緊急地震速報,強い揺れに警戒 {string.Join(" ", prefWarn)},200,0,0,White,255,0,0,White,False,60,1000");
            if (debug || readJSON)
                Telop($"0,《現在の情報ではありません》緊急地震速報,強い揺れに警戒 {string.Join(" ", prefWarn)},200,0,0,White,255,0,0,White,False,10,1000");
            view_all.ImageChange(bitmap, text);
            if (Settings.Default.AutoCopy)
            {
                Clipboard.SetText(text);
                Task.Delay(100).ConfigureAwait(false);//片方がクリップボードに保存されないから仮
                Clipboard.SetImage(bitmap);
            }
            if (File.Exists("XPosterV2Host - Enable"))
                if (!debug && !readJSON)
                    XPost(text, $"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png");
        }
    }
}