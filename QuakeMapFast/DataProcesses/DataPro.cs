using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using static QuakeMapFast.Utils.Utils;
using static QuakeMapFast.Utils.JSONClasses;

namespace QuakeMapFast
{
    [SupportedOSPlatform("windows7.0")]
    internal partial class DataPro
    {
        public DataPro(GeoJSON_JMA_Map json)
        {
            json_map_AreaForecastLocalE = json;
        }

        internal static GeoJSON_JMA_Map? json_map_AreaForecastLocalE;

        public static Bitmap DrawMap(float latSta, float latEnd, float lonSta, float lonEnd)
        {
            if (json_map_AreaForecastLocalE == null)
                throw new Exception("地図データが読み込まれていません。");

            var zoom = 1080f / (latEnd - latSta);

            var bitmap = new Bitmap(1920, 1080);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.FromArgb(20, 40, 60));
            using var gp = new GraphicsPath();





            foreach (var feature in json_map_AreaForecastLocalE.Features)
            {
                if (feature.Geometry == null)
                    continue;
                if (feature.Geometry.Type == "Polygon")
                {
                    gp.StartFigure();
                    var points = feature.Geometry.Coordinates.Objects[0].MainPoints.Select(coordinate => new PointF((coordinate.Lon - lonSta) * zoom, (latEnd - coordinate.Lat) * zoom));
                    if (points.Count() > 2)
                        gp.AddPolygon(points.ToArray());
                }
                else
                {

                    foreach (var singleObject in feature.Geometry.Coordinates.Objects)
                    {
                        gp.StartFigure();
                        var points = singleObject.MainPoints.Select(coordinate => new PointF((coordinate.Lon - lonSta) * zoom, (latEnd - coordinate.Lat) * zoom));
                        if (points.Count() > 2)
                            gp.AddPolygon(points.ToArray());
                    }
                }
            }
            var lineWidth = Math.Max(1f, zoom / 216f);
            if (CtrlForm.debug)
                ConWrite($"<debug>[DrawMap]zoom: {zoom}, lineWidth: {lineWidth}");
            g.FillPath(new SolidBrush(Color.FromArgb(100, 100, 150)), gp);
            g.DrawPath(new Pen(Color.FromArgb(255, 200, 200, 200), lineWidth) { LineJoin = LineJoin.Round }, gp);//zoom > 200 ? 2 : 1
            return bitmap;
        }


        /*
        /// <summary>
        /// マップを描画します。塗りつぶしも実行します。
        /// </summary>
        /// <param name="areaColor">地区ごとの色</param>
        /// <param name="hypoLat">震源緯度</param>
        /// <param name="hypoLon">震源経度</param>
        /// <returns>マップの画像</returns>
        public static Bitmap DrawMap(Dictionary<string, SolidBrush> areaColor, double hypoLat = -200, double hypoLon = -200)
        {
            ConWrite("[DrawMap]座標計算開始");
            double latSta = 999;
            double latEnd = -999;
            double lonSta = 999;
            double lonEnd = -999;
            foreach (var features in mapjson["features"].AsArray().Where(features => areaColor.ContainsKey((string)features["properties"]["name"])))
            {
                if ((string)features["geometry"]["type"] == "Polygon")
                {
                    foreach (var coordinate in features["geometry"]["coordinates"][0].AsArray())
                    {
                        latSta = Math.Min(latSta, (double)coordinate[1]);
                        latEnd = Math.Max(latEnd, (double)coordinate[1]);
                        lonSta = Math.Min(lonSta, (double)coordinate[0]);
                        lonEnd = Math.Max(lonEnd, (double)coordinate[0]);
                    }
                }
                else
                {
                    foreach (var coordinate in features["geometry"]["coordinates"].AsArray().SelectMany((JsonNode coordinate) => coordinate.AsArray()))
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
            ConWrite("[DrawMap]描画開始");
            var bitmap = new Bitmap(1920, 1080);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(20, 40, 60));
                var gPath = new GraphicsPath();
                foreach (var features in mapjson["features"].AsArray())
                {
                    gPath.Reset();
                    gPath.StartFigure();
                    //if (features["geometry"]["coordinates"] == null)
                    if (features["geometry"].AsArray().Count() == 0)
                        continue;
                    if ((string)features["geometry"]["type"] == "Polygon")
                    {
                        var points = features["geometry"]["coordinates"][0].AsArray().Select(coordinate => new Point((int)(((double)coordinate[0] - lonSta) * zoom), (int)((latEnd - (double)coordinate[1]) * zoom)));
                        if (points.Count() > 2)
                            gPath.AddPolygon(points.ToArray());
                    }
                    else
                    {
                        foreach (var coordinates in features["geometry"]["coordinates"].AsArray())
                        {
                            var points = coordinates[0].AsArray().Select(coordinate => new Point((int)(((double)coordinate[0] - lonSta) * zoom), (int)((latEnd - (double)coordinate[1]) * zoom)));
                            if (points.Count() > 2)
                                gPath.AddPolygon(points.ToArray());
                        }
                    }
                    if (areaColor.ContainsKey((string)features["properties"]["name"]))
                        g.FillPath(areaColor[(string)features["properties"]["name"]], gPath);
                    else
                        g.FillPath(new SolidBrush(Color.FromArgb(100, 100, 150)), gPath);
                    g.DrawPath(new Pen(Color.FromArgb(255, 200, 200, 200), 2), gPath);//zoom > 200 ? 2 : 1

                }
                gPath.Dispose();

                if (hypoLat != -200)
                {
                    var center = new Point((int)((hypoLon - lonSta) * zoom), (int)((latEnd - hypoLat) * zoom));
                    g.DrawLine(new Pen(Color.Red, 11), center.X - 50, center.Y - 50, center.X + 50, center.Y + 50);
                    g.DrawLine(new Pen(Color.Red, 11), center.X + 50, center.Y - 50, center.X - 50, center.Y + 50);
                }
            }
            return bitmap;
        }*/

    }
}