using System.ComponentModel;
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

        public static Bitmap[] DrawScaleIcons(float size)
        {
            var iconList = new List<Bitmap>();
            var penW = Math.Max(1, (float)Math.Floor(size / 10d));
            for (int i = 0; i <= 9; i++)
            {
                var icon = new Bitmap((int)size, (int)size);
                using var g = Graphics.FromImage(icon);

                var brush = IntN2Brush(i == 0 ? 5 : i);
                g.FillRectangle(brush, 0, 0, size, size);
                g.DrawRectangle(new Pen(Color.FromArgb(63, 0, 0, 0), penW * 2 - 1), 0, 0, size - 1, size - 1);

                iconList.Add(icon);
            }
            return [.. iconList];
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
            public required int Scale { get; set; }
        }


    }
}
