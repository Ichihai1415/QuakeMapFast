using QuakeMapFast.Properties;
using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using static QuakeMapFast.CtrlForm;
using static QuakeMapFast.Utils.JSONClasses;
using static QuakeMapFast.Utils.Utils;

namespace QuakeMapFast
{
    internal partial class DataPro
    {
        /// <summary>
        /// 各地の震度に関する情報
        /// </summary>
        /// <param name="json"></param>
        public static void DetailScale(P2PQuake_JMAQuake? json)
        {
            if (font == null)
                throw new Exception("フォントが読み込まれていません。");
            if (json == null)
            {
                ConWrite("[DetailScale]データがありません。", ConsoleColor.Red);
                return;
            }
            if (json.Points == null)//todo:つくる
            {
                ConWrite("[DetailScale]震度観測データがありません。", ConsoleColor.Red);
                return;
            }

            ConWrite("[DetailScale]データ処理開始");

            var maxIntN = P2PQScale2Int(json.Earthquake.MaxScale);
            var maxIntS = GetEnumDescription((P2PQ_Scales)json.Earthquake.MaxScale);

            float latSta = 200, latEnd = -200, lonSta = 200, lonEnd = -200;
            if (json.Earthquake.Hypocenter != null)//震央を範囲に含めるか設定にする
            {
                latSta = json.Earthquake.Hypocenter.Latitude;
                latEnd = json.Earthquake.Hypocenter.Latitude;
                lonSta = json.Earthquake.Hypocenter.Longitude;
                lonEnd = json.Earthquake.Hypocenter.Longitude;

            }

            var points = new List<PointData>();
            var pointDict = new Dictionary<P2PQ_Scales, List<(float Lat, float Lon)>>();

            var startScale_map = Math.Max(1, maxIntN - 4);
            foreach (var a in json.Points)
            {
                if (obsPt2LatLon.TryGetValue(a.Addr, out var latLon))
                {
                    if (P2PQScale2Int(a.Scale) >= startScale_map)
                    {
                        latSta = Math.Min(latSta, latLon.Lat);
                        latEnd = Math.Max(latEnd, latLon.Lat);
                        lonSta = Math.Min(lonSta, latLon.Lon);
                        lonEnd = Math.Max(lonEnd, latLon.Lon);
                    }

                    var scaleE = (P2PQ_Scales)a.Scale;

                    points.Add(new PointData { Name = a.Addr, Lat = latLon.Lat, Lon = latLon.Lon, Scale = scaleE });

                    if (pointDict.TryGetValue(scaleE, out var values))
                        values.Add((latLon.Lat, latLon.Lon));
                    else
                        pointDict.Add(scaleE, [(latLon.Lat, latLon.Lon)]);
                }
                else
                    ConWrite($"座標不明: {a.Addr}");
            }

            PointCorrect(ref latSta, ref latEnd, ref lonSta, ref lonEnd);
            var zoom = 1080f / (latEnd - latSta);//1度あたりのピクセル
            var bitmap = DrawMap(latSta, latEnd, lonSta, lonEnd);
            using var g = Graphics.FromImage(bitmap);


            var size = Math.Min(108f, Math.Max(10f, zoom / 7.5f));//size=>(範囲(単位:度))
            if (debug)
                ConWrite($"<debug>[]zoom: {zoom}, size: {size}");
            var iconList = DrawScaleIcons(size);

            var startScale = Math.Max(1, maxIntN - 10);
            int[] scaleOrder = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            scaleOrder = [1, 2, 3, 4, 10, 5, 6, 7, 8, 9];

            for (var i = 0; i < 10; i++)
            {
                var s = scaleOrder[i];
                if (s < startScale)
                    continue;
                var scale = P2PQScaleInt2Enum(s);
                if (pointDict.TryGetValue(scale, out var values))
                {
                    var icon = iconList[s];
                    foreach (var (lat, lon) in values)
                    {
                        var x = (lon - lonSta) * zoom - size / 2f;
                        var y = (latEnd - lat) * zoom - size / 2f;
                        g.DrawImage(icon, new Rectangle((int)x, (int)y, (int)size, (int)size), 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, IA_ScaleIcon);

                    }
                }
            }

            if (json.Earthquake.Hypocenter.Latitude != -200)
            {
                var center = new Point((int)((json.Earthquake.Hypocenter.Longitude - lonSta) * zoom), (int)((latEnd - json.Earthquake.Hypocenter.Latitude) * zoom));
                g.DrawLine(new Pen(Color.FromArgb(127, 255, 0, 0), 11), center.X - 50, center.Y - 50, center.X + 50, center.Y + 50);
                g.DrawLine(new Pen(Color.FromArgb(127, 255, 0, 0), 11), center.X + 50, center.Y - 50, center.X - 50, center.Y + 50);
            }

            g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
            g.DrawString("震源・震度情報", new Font(font, 50), Brushes.White, 1090, 10);
            g.DrawString(json.Earthquake.Time[..16], new Font(font, 30), Brushes.White, 1580, 45);
            var dep = json.Earthquake.Hypocenter.Depth == -1 ? "不明" :
                json.Earthquake.Hypocenter.Depth == 0 ? "ごく浅い" :
                (json.Earthquake.Hypocenter.Depth + "km");
            var mag = json.Earthquake.Hypocenter.Magnitude == -1 ?
                "不明" : json.Earthquake.Hypocenter.Magnitude.ToString("0.0");
            g.DrawString(json.Earthquake.Hypocenter.Name + " 深さ" + dep + "  M" + mag, new Font(font, 30), Brushes.White, 1095, 95);
            //g.DrawString("山梨県東部・富士五湖 深さごく浅い  M不明", new Font(font, 30), Brushes.White, 1095, 95);

            var pen = new Pen(IntN2Brush(maxIntN), 51) { LineJoin = LineJoin.Round };
            g.DrawRectangle(pen, 1125, 175, 750, 150);
            g.FillRectangle(IntN2Brush(maxIntN), 1150, 200, 700, 100);
            g.DrawString("最大震度", new Font(font, 50), IntN2TextBrush(maxIntN), 1150, 240);
            maxIntS = maxIntS.Replace("震度", "").Replace("情報なし", "-");//todo:不明等考慮を追加
            if (maxIntS.Contains('弱') || maxIntS.Contains('強'))
                g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1550, 175);
            else
                g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1600, 175);

            var maxIntAreas = string.Join(Environment.NewLine, json.Points.Where(x => x.Scale == json.Earthquake.MaxScale).Select(x => x.Addr));
            //maxIntAreas = "1\n2\n3\n4\n5\n6\n7\n8\n9\n10";
            g.DrawString(maxIntAreas, new Font(font, 40), Brushes.White, 1100, 360);

            g.FillRectangle(Brushes.Black, 1080, 900, 840, 180);
            g.DrawString("日本地図データ:気象庁\n世界地図データ:Natural Earth\nそれぞれ加工して使用\nデータ:気象庁", new Font(font, 20), Brushes.White, 1090, 910);
            g.DrawImage(Resources.IntLegend, 1500, 906, 410, 164);
            if ((debug || readJSON) && oldDataDraw)
                using (var textGP = new GraphicsPath())
                {
                    textGP.AddString("《現在の情報ではありません》", font, 0, 140, new Point(-70, 400), StringFormat.GenericDefault);
                    g.FillPath(Brushes.White, textGP);
                    g.DrawPath(new Pen(Color.Black, 3), textGP);
                }

            view_all.ImageChange(bitmap, "");

            var saveTime = DateTime.Now;
            if (Settings.Default.Save_Image)
            {
                Directory.CreateDirectory($"output\\{saveTime:yyyyMM}\\{saveTime:dd}");
                bitmap.Save($"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png", ImageFormat.Png);
                ConWrite($"[Draw]output\\{saveTime:yyyyMM}\\{saveTime:dd}に保存しました");
            }


            //震度大きい順前提
            var intsSt = new StringBuilder();
            var lastScale = -99;
            var count = 0;
            foreach (var pts in json.Points.OrderByDescending(x => x.Scale))
            {
                if (lastScale != pts.Scale)
                {
                    intsSt.Append("《震度");
                    intsSt.Append(P2PScale2IntS(pts.Scale));
                    intsSt.Append('》');
                    lastScale = pts.Scale;
                }
                intsSt.Append(pts.Addr);
                intsSt.Append(' ');

                count++;
                if (count == 50)
                    break;
            }

            //string text = $"震度速報【最大震度{maxIntS}】{time:yyyy/MM/dd HH:mm}\n{intsArea}";
            //ConWrite(text, ConsoleColor.Cyan);
            if (debug || readJSON)//todo:
            {
                Telop($"0,《現在の情報ではありません》震源・震度情報【最大震度{maxIntS}】,{json.Earthquake.Time[..16]} 震源: {json.Earthquake.Hypocenter.Name} 深さ{dep} M{mag}  {intsSt},{Int2TelopColor(maxIntN)},False,10,1000");
                //BouyomiChan($"QuakeMapFastの読み上げです。デバッグあるいはJSON読み込みモードのため無効です。");
            }
            else
            {
                Telop($"0,震源・震度情報【最大震度{maxIntS}】,{json.Earthquake.Time[..16]} 震源: {json.Earthquake.Hypocenter.Name} 深さ{dep} M{mag}  {intsSt},{Int2TelopColor(maxIntN)},False,60,1000");
                //BouyomiChan($"震度速報、{intsArea_Max3.Replace("\n", "").Replace("《", "、").Replace("》", "、").Replace(" ", "、")}");
            }

            string soundFile = maxIntN switch
            {
                0 => "scale\\0.wav",
                1 => "scale\\1.wav",
                2 => "scale\\2.wav",
                3 => "scale\\3.wav",
                4 => "scale\\4.wav",
                5 => "scale\\5-.wav",
                6 => "scale\\5+.wav",
                7 => "scale\\6-.wav",
                8 => "scale\\6+.wav",
                9 => "scale\\7.wav",
                _ => string.Empty,
            };
            PlaySound(soundFile);

            var dt = DateTime.Parse(json.Earthquake.Time);
            if (File.Exists("XPosterV2Host - Enable"))
                if (!debug && !readJSON && maxIntN >= 5)
                    XPost($"{dt.Day}日{dt.Hour}時{dt.Minute}分ごろ、最大震度{maxIntS}を観測する地震がありました。{GetTsunamiMessege(json.Earthquake.DomesticTsunami)}震源地は{json.Earthquake.Hypocenter.Name}、震源の深さは{dep}、地震の規模を示すマグニチュードは{mag}と推定されています。", $"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png");
        }

    }
}
