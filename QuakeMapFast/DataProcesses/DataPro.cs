using Ichihai1415.GeoJSON;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using static QuakeMapFast.Utils.Utils;

namespace QuakeMapFast
{
    [SupportedOSPlatform("windows7.0")]
    internal partial class DataPro
    {
        public DataPro(GeoJSONScheme.GeoJSON_JMA_Map json)
        {
            json_map_AreaForecastLocalE = json;
        }

        internal static GeoJSONScheme.GeoJSON_JMA_Map? json_map_AreaForecastLocalE;

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
                    {
                        gp.AddPolygon(points.ToArray());
                        g.FillPolygon(new SolidBrush(Color.FromArgb(100, 100, 150)), points.ToArray());
                    }
                }
                else
                {

                    foreach (var singleObject in feature.Geometry.Coordinates.Objects)
                    {
                        gp.StartFigure();
                        var points = singleObject.MainPoints.Select(coordinate => new PointF((coordinate.Lon - lonSta) * zoom, (latEnd - coordinate.Lat) * zoom));
                        if (points.Count() > 2)
                        {
                            gp.AddPolygon(points.ToArray());
                            g.FillPolygon(new SolidBrush(Color.FromArgb(100, 100, 150)), points.ToArray());
                        }
                    }
                }
            }
            var lineWidth = Math.Max(1f, zoom / 216f);
            if (CtrlForm.debug)
                ConWrite($"<debug>[DrawMap]zoom: {zoom}, lineWidth: {lineWidth}");
            //g.FillPath(new SolidBrush(Color.FromArgb(100, 100, 150)), gp);
            g.DrawPath(new Pen(Color.FromArgb(255, 200, 200, 200), lineWidth) { LineJoin = LineJoin.Round }, gp);//zoom > 200 ? 2 : 1
            return bitmap;
        }



        /// <summary>
        /// マップを描画します。塗りつぶしも実行します。
        /// </summary>
        /// <param name="areaColor">地区ごとの色</param>
        /// <param name="hypoLat">震源緯度</param>
        /// <param name="hypoLon">震源経度</param>
        /// <returns>マップの画像</returns>
        public static Bitmap DrawMap_Old(Dictionary<string, SolidBrush> areaColor, double hypoLat = -200, double hypoLon = -200)
        {
            ConWrite("[DrawMap]座標計算開始");
            var latSta = 999f;
            var latEnd = -999f;
            var lonSta = 999f;
            var lonEnd = -999f;
            foreach (var features in json_map_AreaForecastLocalE.Features.Where(features => areaColor.ContainsKey(features.Properties.Name)))
            {
                foreach (var coordinate in features.Geometry.Coordinates.Objects.SelectMany(x => x.MainPoints))
                {
                    latSta = Math.Min(latSta, coordinate.Lat);
                    latEnd = Math.Max(latEnd, coordinate.Lat);
                    lonSta = Math.Min(lonSta, coordinate.Lon);
                    lonEnd = Math.Max(lonEnd, coordinate.Lon);
                }
            }

            PointCorrect(ref latSta, ref latEnd, ref lonSta, ref lonEnd);
            var zoom = 1080f / (latEnd - latSta);
            ConWrite("[DrawMap]描画開始");
            var bitmap = new Bitmap(1920, 1080);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(20, 40, 60));
                var gPath = new GraphicsPath();
                foreach (var features in json_map_AreaForecastLocalE.Features)
                {
                    gPath.Reset();
                    gPath.StartFigure();
                    //if (features["geometry"]["coordinates"] == null)
                    if (features.Geometry == null)
                        continue;
                    if (features.Geometry.Type == "Polygon")
                    {
                        var points = features.Geometry.Coordinates.Objects[0].MainPoints.Select(coordinate => new PointF((coordinate.Lon - lonSta) * zoom, (latEnd - coordinate.Lat) * zoom));
                        if (points.Count() > 2)
                            gPath.AddPolygon(points.ToArray());
                    }
                    else
                    {
                        foreach (var objects in features.Geometry.Coordinates.Objects)
                        {
                            var points = objects.MainPoints.Select(coordinate => new PointF((coordinate.Lon - lonSta) * zoom, (latEnd - coordinate.Lat) * zoom));
                            if (points.Count() > 2)
                                gPath.AddPolygon(points.ToArray());
                        }
                    }
                    if (areaColor.ContainsKey(features.Properties.Name))
                        g.FillPath(areaColor[features.Properties.Name], gPath);
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
        }

    }
}