using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace QuakeMapFast.Utils
{
    /// <summary>
    /// 各処理とデータのクラス
    /// </summary>
    [SupportedOSPlatform("windows7.0")]
    public partial class Utils
    {

        /// <summary>
        /// enumのDescriptionを取得します。
        /// </summary>
        /// <param name="value">取得するenum</param>
        /// <returns>description、なければ<c>value.ToString()</c></returns>
        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return value.ToString();
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// フォントリスト
        /// </summary>
        private static readonly Dictionary<float, Font> _fontDict = [];

        /// <summary>
        /// フォントリストからフォントを取り出します。無ければ作成します。
        /// </summary>
        /// <param name="size">フォントサイズ</param>
        /// <returns>指定されたサイズのフォント</returns>
        /// <exception cref="Exception"><see cref="CtrlForm.font"/>が読み込まれていない場合</exception>
        internal static Font FontProvide(float size)
        {
            if (_fontDict.TryGetValue(size, out var rFont))
                return rFont;
            else
            {
                if (CtrlForm.font == null)
                    throw new Exception("フォントが読み込まれていません。");
                rFont = new Font(CtrlForm.font, size);
                _fontDict.Add(size, rFont);
                return rFont;
            }
        }

        /// <summary>
        /// 震度アイコンを描画します。
        /// </summary>
        /// <param name="size">アイコンのサイズ</param>
        /// <returns>描画されたアイコン(11個([10]:未入電))</returns>
        public static Bitmap[] DrawScaleIcons(float size)
        {
            var iconList = new List<Bitmap>();
            var penW = Math.Max(1, (float)Math.Floor(size / 10d));
            var fontSize = size / 1.5f;
            for (int i = 0; i <= 10; i++)
            {
                var icon = new Bitmap((int)size, (int)size);
                using var g = Graphics.FromImage(icon);
                //ほぼ変わらない
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                var brush = IntN2Brush(i);
                g.FillRectangle(brush, 0, 0, size, size);
                g.DrawRectangle(new Pen(Color.FromArgb(63, 0, 0, 0), penW * 2 - 1), 0, 0, size - 1, size - 1);
                //特に意味のない表
                // 2^-x  :  0   -1    -2     -3      -4       -5        -6         -7          -8           -9           -10
                // 1/2^x :  0    2     4      8      16       32        64        128         256          512          1024
                // float :  1  0.5  0.25  0.125  0.0625  0.03125  0.015625  0.0078125  0.00390625  0.001953125  0.0009765625
                // m:-, p:+ 1:数字, 2:+-
                var x = size * 0.09375f;//1/16+1/32
                var x_m1 = size * -0.0625f;//1/16
                var x_m2 = size * 0.375f;//1/4+1/8
                var x_p1 = x_m1;
                var x_p2 = x_m2;
                var y = size * -0.140625f;//1/8+1/64
                var y_m1 = y;
                var y_m2 = size * -0.35625f;//1/4+1/8+1/32
                var y_p1 = y;
                var y_p2 = size * -0.125f;//1/8
                var fontSize_p = fontSize * 0.75f;
                switch (i)
                {
                    case 0:
                        g.DrawString("0", FontProvide(fontSize), Brushes.White, x, y);
                        break;
                    case 1:
                        g.DrawString("1", FontProvide(fontSize), Brushes.White, x, y);
                        break;
                    case 2:
                        g.DrawString("2", FontProvide(fontSize), Brushes.White, x, y);
                        break;
                    case 3:
                        g.DrawString("3", FontProvide(fontSize), Brushes.Black, x, y);
                        break;
                    case 4:
                        g.DrawString("4", FontProvide(fontSize), Brushes.Black, x, y);
                        break;
                    case 5:
                        g.DrawString("5", FontProvide(fontSize), Brushes.Black, x_m1, y_m1);
                        g.DrawString("-", FontProvide(fontSize), Brushes.Black, x_m2, y_m2);
                        break;
                    case 6:
                        g.DrawString("5", FontProvide(fontSize), Brushes.Black, x_p1, y_p1);
                        g.DrawString("+", FontProvide(fontSize_p), Brushes.Black, x_p2, y_p2);
                        break;
                    case 7:
                        g.DrawString("6", FontProvide(fontSize), Brushes.White, x_m1, y_m1);
                        g.DrawString("-", FontProvide(fontSize), Brushes.White, x_m2, y_m2);
                        break;
                    case 8:
                        g.DrawString("6", FontProvide(fontSize), Brushes.White, x_p1, y_p1);
                        g.DrawString("+", FontProvide(fontSize_p), Brushes.White, x_p2, y_p2);
                        break;
                    case 9:
                        g.DrawString("7", FontProvide(fontSize), Brushes.White, x, y);
                        break;
                    case 10:
                        g.DrawString("未", FontProvide(fontSize_p), Brushes.Black, size * 0.0625f, size * 0.03125f);//1/16+1/32
                        break;
                }
                iconList.Add(icon);
            }
            return [.. iconList];
        }

        /// <summary>
        /// 震度アイコン描画時の透明度を変更します。設定反映時等に呼び出してください。
        /// </summary>
        /// <param name="alpha">透明度(0~1)</param>
        private static void ChangeAlpha(float alpha)
        {
            var colorMatrix = new ColorMatrix() { Matrix33 = alpha };
            CtrlForm.IA_ScaleIcon = new ImageAttributes();
            CtrlForm.IA_ScaleIcon.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        /// <summary>
        /// 震度アイコン描画時の透明度を変更します。設定反映時等に呼び出してください。
        /// </summary>
        /// <param name="alpha">透明度(0~255)</param>
        private static void ChangeAlpha(int alpha)
        {
            ChangeAlpha(alpha / 255f);
        }



        /// <summary>
        /// 観測点データの格納用
        /// </summary>
        public class PointData
        {
            /// <summary>
            /// 名称
            /// </summary>
            public required string Name { get; set; }

            /// <summary>
            /// 緯度
            /// </summary>
            public required float Lat { get; set; }

            /// <summary>
            /// 経度
            /// </summary>
            public required float Lon { get; set; }

            /// <summary>
            /// 震度
            /// </summary>
            public required P2PQ_Scales Scale { get; set; }
        }


    }
}
