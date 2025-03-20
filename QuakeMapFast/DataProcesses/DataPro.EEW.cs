using QuakeMapFast.Properties;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.Json.Nodes;
using static QuakeMapFast.CtrlForm;
using static QuakeMapFast.Utils.Utils;
using static QuakeMapFast.Utils.JSONClasses;

namespace QuakeMapFast
{
    internal partial class DataPro
    {/*
        public static void EEW(JsonNode json)
        {
            if ((bool)json["cancelled"])
                return;

            var earthquake = json["earthquake"];
            var hypocenter = earthquake["hypocenter"];

            DateTime time = DateTime.Parse((string)earthquake["originTime"]);
            Dictionary<string, SolidBrush> areaColor = json["areas"].AsArray().ToDictionary(area => (string)area["name"], area => P2PQScale2isOver6((int)area["scaleFrom"], (int)area["scaleTo"])
            ? new SolidBrush(Color.FromArgb(180, 0, 0)) : new SolidBrush(Color.FromArgb(180, 180, 0)));
            List<string> areaWarn = areaColor.Keys.ToList();
            List<string> prefWarn = json["areas"].AsArray().Select(n => (string)n["pref"]).Distinct().ToList();

            double hLat = (double)hypocenter["latitude"];
            double hLon = (double)hypocenter["longitude"];
            ConWrite("[EEW]画像描画開始");

            Bitmap bitmap = DrawMap(areaColor, hLat, hLon);

            string hypoName = (string)hypocenter["reduceName"];

            string warnAreaInfo1 = "";
            string warnAreaInfo2 = "";
            foreach (var area in json["areas"].AsArray())
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
            var isMajorWarn = warnAreaInfo2.Contains("6") || warnAreaInfo2.Contains("7");
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
                Brush warnTextColor = isMajorWarn ? Brushes.Red : Brushes.Yellow;
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
            if (debug || readJSON)
            {
                Telop($"0,《現在の情報ではありません》緊急地震速報,強い揺れに警戒 {string.Join(" ", prefWarn)},200,0,0,White,255,0,0,White,False,10,1000");
                Bouyomichan($"QuakeMapFastの読み上げです。デバッグあるいはJSON読み込みモードのため無効です。");
            }
            else
            {
                Telop($"0,緊急地震速報,強い揺れに警戒 {string.Join(" ", prefWarn)},200,0,0,White,255,0,0,White,False,60,1000");
                Bouyomichan($"緊急地震速報です、次の地域では強い揺れに警戒してください。{string.Join("、", prefWarn)}");
            }
            view_all.ImageChange(bitmap, text);
            if (Settings.Default.AutoCopy)
            {
                Clipboard.SetText(text);
                Task.Delay(100).ConfigureAwait(false);//片方がクリップボードに保存されないから仮
                Clipboard.SetImage(bitmap);
            }

            PlaySound(isMajorWarn ? "eew\\warn2.wav" : "eew\\warn1.wav");

            if (File.Exists("XPosterV2Host - Enable"))
                if (!debug && !readJSON)
                    XPost(text, $"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png");
        }*/

    }
}
