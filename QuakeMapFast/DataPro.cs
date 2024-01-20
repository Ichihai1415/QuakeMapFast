using Newtonsoft.Json.Linq;
using QuakeMapFast.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static QuakeMapFast.Conv;
using static QuakeMapFast.CtrlForm;
using static QuakeMapFast.Func;

namespace QuakeMapFast
{
    internal class DataPro
    {

        public static Bitmap DrawMap(double latEnd, double lonSta, double zoom, Dictionary<string, SolidBrush> areaColor)
        {
            var mapjson = JObject.Parse(Resources._20190125_AreaForecastLocalE_GIS_name_0_1);
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
            }
            return bitmap;
        }

        /// <summary>
        /// 震度速報
        /// </summary>
        /// <param name="json">描画するデータ</param>
        /// <returns>震度速報の画像</returns>
        public static void ScalePrompt(JObject json)
        {
            DateTime startTime = DateTime.Now;

            DateTime time = DateTime.Parse((string)json["earthquake"]["time"]);
            int maxIntN = P2PScale2IntN((int)json["earthquake"]["maxScale"]);
            string maxIntS = P2PScale2IntS((int)json["earthquake"]["maxScale"]);
            Dictionary<string, int> areaInt = Points2Dic(json["points"], "addr");

            ConWrite("[ScalePrompt]座標計算開始");
            JObject mapjson = JObject.Parse(Resources._20190125_AreaForecastLocalE_GIS_name_0_1);
            double latSta = 999;
            double latEnd = -999;
            double lonSta = 999;
            double lonEnd = -999;
            foreach (JToken features in mapjson["features"])
            {
                if (areaInt.ContainsKey((string)features["properties"]["name"]))
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
            ConWrite("[ScalePrompt]画像描画開始");
            Bitmap bitmap = DrawMap(latEnd, lonSta, zoom, areaInt.ToDictionary(x => x.Key, x => IntN2Brush(x.Value)));
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
                g.DrawString("日本地図データ:気象庁\n世界地図データ:National Earth\nそれぞれ加工して使用\nデータ:気象庁", new Font(font, 20), Brushes.White, 1090, 910);
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
            if (Settings.Default.Save_Image)
            {
                DateTime saveTime = DateTime.Now;
                Directory.CreateDirectory($"output\\{saveTime:yyyyMM}\\{saveTime:dd}");
                bitmap.Save($"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png", ImageFormat.Png);
                ConWrite($"[Draw]output\\{saveTime:yyyyMM}\\{saveTime:dd}に保存しました");
            }

            string intsArea = Point2String(json, "addr");
            string intsArea_Max3 = Point2String(json, "addr", maxIntN - 2);//最大震度から3階級(Max6->6,5,4)
            string text = $"震度速報【最大震度{maxIntS}】{time:yyyy/MM/dd HH:mm}\n{intsArea}";
            ConWrite(text, ConsoleColor.Cyan);
            Bouyomichan($"震度速報、{intsArea_Max3.Replace("\n", "").Replace("《", "、").Replace("》", "、").Replace(" ", "、")}");
            Telop($"0,震度速報【最大震度{maxIntS}】,{intsArea.Replace("\n", "")},{Int2TelopColor(maxIntN)},False,60,1000");
            if (debug || readJSON)
                Telop($"0,《現在の情報ではありません》震度速報【最大震度{maxIntS}】,{intsArea.Replace("\n", "")},{Int2TelopColor(maxIntN)},False,60,1000");
            if (Settings.Default.AutoCopy)
            {
                Clipboard.SetImage(bitmap);
                Clipboard.SetText(text);
            }
            view_all.ImageChange(bitmap, text);
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