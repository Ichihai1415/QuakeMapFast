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

        public static void WriteLog(Exception ex)
        {
            File.WriteAllText(@$"Log\Error\{DateTime.Now:now:yyyyMM\dd\yyyyMMddHHmmss.ffff}.txt", ex.ToString());
        }

    }
}
