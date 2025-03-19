using QuakeMapFast.Properties;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.Json.Nodes;
using static QuakeMapFast.Conv;
using static QuakeMapFast.CtrlForm;
using static QuakeMapFast.Func;

namespace QuakeMapFast
{
    internal partial class DataPro
    {

        /// <summary>
        /// 震度速報
        /// </summary>
        /// <param name="json">描画するデータ</param>
        public static void ScalePrompt(JsonNode json)
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
            {
                Telop($"0,《現在の情報ではありません》震度速報【最大震度{maxIntS}】,{intsArea.Replace("\n", "")},{Int2TelopColor(maxIntN)},False,10,1000");
                Bouyomichan($"QuakeMapFastの読み上げです。デバッグあるいはJSON読み込みモードのため無効です。");
            }
            else
            {
                Telop($"0,震度速報【最大震度{maxIntS}】,{intsArea.Replace("\n", "")},{Int2TelopColor(maxIntN)},False,60,1000");
                Bouyomichan($"震度速報、{intsArea_Max3.Replace("\n", "").Replace("《", "、").Replace("》", "、").Replace(" ", "、")}");
            }
            view_all.ImageChange(bitmap, text);
            if (Settings.Default.AutoCopy)
            {
                Clipboard.SetText(text);
                Task.Delay(100).ConfigureAwait(false);//片方がクリップボードに保存されないから仮
                Clipboard.SetImage(bitmap);
            }

            string soundFile;
            switch (maxIntN)
            {
                case 0:
                    soundFile = "scale\\0.wav";
                    break;
                case 1:
                    soundFile = "scale\\1.wav";
                    break;
                case 2:
                    soundFile = "scale\\2.wav";
                    break;
                case 3:
                    soundFile = "scale\\3.wav";
                    break;
                case 4:
                    soundFile = "scale\\4.wav";
                    break;
                case 5:
                    soundFile = "scale\\5-.wav";
                    break;
                case 6:
                    soundFile = "scale\\5+.wav";
                    break;
                case 7:
                    soundFile = "scale\\6-.wav";
                    break;
                case 8:
                    soundFile = "scale\\6+.wav";
                    break;
                case 9:
                    soundFile = "scale\\7.wav";
                    break;
                default:
                    soundFile = string.Empty;
                    break;
            }
            PlaySound(soundFile);

            if (File.Exists("XPosterV2Host - Enable"))
                if (!debug && !readJSON)
                    XPost(text, $"output\\{saveTime:yyyyMM}\\{saveTime:dd}\\{saveTime:yyyyMMddHHmmss.ff}.png");
        }


    }
}
