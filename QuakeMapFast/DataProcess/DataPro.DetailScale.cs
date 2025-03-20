using System.Drawing;
using static QuakeMapFast.Func;
using static QuakeMapFast.JSONClasses;

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
            if (json == null)
            {
                ConWrite("[DetailScale]データがありません。", ConsoleColor.Red);
                return;
            }

            ConWrite("[DetailScale]データ処理開始");

            float latSta = 200, latEnd = -200, lonSta = 200, lonEnd = -200;
            foreach (var a in json.Points)
            {
                if (CtrlForm.obsPt2LatLon.TryGetValue(a.Addr, out var latLon))
                {
                    latSta = Math.Min(latSta, latLon.Lat);
                    latEnd = Math.Max(latEnd, latLon.Lat);
                    lonSta = Math.Min(lonSta, latLon.Lon);
                    lonEnd = Math.Max(lonEnd, latLon.Lon);
                }
                else
                    ConWrite($"座標不明: {a.Addr}");
            }
            var bitmap = DrawMap(latSta, latEnd, lonSta, lonEnd);

            CtrlForm.view_all.ImageChange(bitmap, "");


        }
    }
}
