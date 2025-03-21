using QuakeMapFast.Properties;
using System.Drawing.Drawing2D;
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
            if(font==null)
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

            float latSta = 200, latEnd = -200, lonSta = 200, lonEnd = -200;
            var points = new List<PointData>();
            var maxIntN = 0;
            var maxIntS = "";
            foreach (var a in json.Points)
            {
                if (obsPt2LatLon.TryGetValue(a.Addr, out var latLon))
                {
                    latSta = Math.Min(latSta, latLon.Lat);
                    latEnd = Math.Max(latEnd, latLon.Lat);
                    lonSta = Math.Min(lonSta, latLon.Lon);
                    lonEnd = Math.Max(lonEnd, latLon.Lon);
                    var intN = P2PScale2IntN(a.Scale);
                    if (intN > maxIntN)
                    {
                        maxIntN = intN;
                        maxIntS = P2PScale2IntS(a.Scale);
                    }
                    points.Add(new PointData { Name = a.Addr, Lat = latLon.Lat, Lon = latLon.Lon, Scale = intN });
                }
                else
                    ConWrite($"座標不明: {a.Addr}");
            }
            PointCorrect(ref latSta, ref latEnd, ref lonSta, ref lonEnd);
            var zoom = 1080f / (latEnd - latSta);//1度あたりのピクセル
            var bitmap = DrawMap(latSta, latEnd, lonSta, lonEnd);
            using var g = Graphics.FromImage(bitmap);

            var size = Math.Min(36, Math.Max(10.8f, zoom / 10f));//size=>(範囲(単位:度))　36=>3 10.8=>10 3.6=>30
            var penW = size / 10f;
            if (debug)
                ConWrite($"zoom: {zoom}, size: {size}");
            foreach (var point in points)
            {
                var brush = IntN2Brush(point.Scale);
                g.FillRectangle(brush, (float)((point.Lon - lonSta) * zoom) - size / 2f, (float)((latEnd - point.Lat) * zoom) - size / 2f, size, size);
                g.DrawRectangle(new Pen(Color.FromArgb(63, 0, 0, 0), penW), (float)((point.Lon - lonSta) * zoom) - size / 2f, (float)((latEnd - point.Lat) * zoom) - size / 2f, size, size);
            }



            g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
            g.DrawString("震源・震度情報", new Font(font, 50), Brushes.White, 1090, 10);
            //g.DrawString(time.ToString("yyyy/MM/dd HH:mm"), new Font(font, 30), Brushes.White, 1095, 85);

            var pen = new Pen(IntN2Brush(maxIntN), 51) { LineJoin = LineJoin.Round };
            g.DrawRectangle(pen, 1125, 175, 750, 150);
            g.FillRectangle(IntN2Brush(maxIntN), 1150, 200, 700, 100);
            g.DrawString("最大震度", new Font(font, 50), IntN2TextBrush(maxIntN), 1150, 240);
            if (maxIntS.Contains('弱') || maxIntS.Contains('強'))
                g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1550, 175);
            else
                g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1600, 175);

            string maxIntAreas = "テストテスト";//string.Join(Environment.NewLine, areaInt.Where(x => x.Value == maxIntN).Select(x => x.Key));
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

            view_all.ImageChange(bitmap, "");



        }

    }
}
