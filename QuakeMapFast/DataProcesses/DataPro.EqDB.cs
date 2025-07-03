using QuakeMapFast.Properties;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        public static void EqDB(JMA_EqDB? json)
        {
            if (font == null)
                throw new Exception("フォントが読み込まれていません。");
            if (json == null)
            {
                ConWrite("[EqDB]データがありません。", ConsoleColor.Red);
                return;
            }
            ConWrite("[EqDB]データ処理開始");

            //
            var maxIntS = json.Res.Hyp[0].MaxI ?? json.Res.Hyp[1].MaxI ?? json.Res.Hyp[2].MaxI ?? json.Res.Hyp[3].MaxI ?? json.Res.Hyp[4].MaxI ?? json.Res.Hyp[5].MaxI;
            if (maxIntS == null)
            {
                ConWrite("[EqDB]最大震度の取得に失敗しました。[開発者向け]コードを確認してください。", ConsoleColor.Red);
                return;
            }
            var maxIntN = JMAintSt2int(maxIntS);

            var latSta = float.Parse(json.Res.Hyp[0].Lat);
            var latEnd = float.Parse(json.Res.Hyp[0].Lat);
            var lonSta = float.Parse(json.Res.Hyp[0].Lon);
            var lonEnd = float.Parse(json.Res.Hyp[0].Lon);


            var points = new List<PointData>();
            var pointDict = new Dictionary<P2PQ_Scales, List<(float Lat, float Lon)>>();


            var startScale_map = Math.Max(1, maxIntN - 4);
            //startScale_map = 0;//debug
            foreach (var a in json.Res.Int)
            {
                var scaleE = JMAintSt2P2PQEnum(a.Int);
                var lat = float.Parse(a.Lat);
                var lon = float.Parse(a.Lon);
                if (Math.Abs(JMAintSt2int(a.Int)) >= startScale_map)//absは未実装のp2p震度の旧震度用
                {
                    latSta = Math.Min(latSta, lat);
                    latEnd = Math.Max(latEnd, lat);
                    lonSta = Math.Min(lonSta, lon);
                    lonEnd = Math.Max(lonEnd, lon);
                }

                points.Add(new PointData { Name = a.Name.Replace("＊", ""), Lat = lat, Lon = lon, Scale = scaleE });

                if (pointDict.TryGetValue(scaleE, out var values))
                    values.Add((lat, lon));
                else
                    pointDict.Add(scaleE, [(lat, lon)]);
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

            g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
            g.DrawString("震度データベース", new Font(font, 50), Brushes.White, 1090, 10);
            g.DrawString(json.Res.Hyp[0].Ot, new Font(font, 30), Brushes.White, 1095, 85);

            var pen = new Pen(IntN2Brush(maxIntN), 51) { LineJoin = LineJoin.Round };
            g.DrawRectangle(pen, 1125, 175, 750, 150);
            g.FillRectangle(IntN2Brush(maxIntN), 1150, 200, 700, 100);
            g.DrawString("最大震度", new Font(font, 50), IntN2TextBrush(maxIntN), 1150, 240);
            maxIntS = maxIntS.Replace("震度", "");//todo:不明等考慮を追加
            if (maxIntS.Contains('弱') || maxIntS.Contains('強'))
                g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1550, 175);
            else
                g.DrawString(maxIntS, new Font(font, 90, FontStyle.Bold), IntN2TextBrush(maxIntN), 1600, 175);

            var maxIntAreas = "テストテスト";//string.Join(Environment.NewLine, areaInt.Where(x => x.Value == maxIntN).Select(x => x.Key));
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
