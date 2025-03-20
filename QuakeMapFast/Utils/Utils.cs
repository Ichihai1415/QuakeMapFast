using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using static QuakeMapFast.Utils.JSONClasses;

namespace QuakeMapFast.Utils
{
    /// <summary>
    /// 各処理とデータのクラス
    /// </summary>
    [SupportedOSPlatform("windows7.0")]
    public partial class Utils
    {
        /// <summary>
        /// Geometry独自クラスへの読み込みに必要です。書き込みは未実装です。
        /// </summary>
        public class OriginalGeometryConverter : JsonConverter<OriginalGeometry?>
        {
            public override OriginalGeometry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using var jsonDoc = JsonDocument.ParseValue(ref reader);
                var root = jsonDoc.RootElement;
                if (root.TryGetProperty("type", out JsonElement type))
                    if (root.TryGetProperty("coordinates", out JsonElement coordinates))//lineSt:[[ ]],multiLineSt,[[[ ]]],poly:[[[ ]]],multiPoly:[[[[ ]]]]
                        switch (type.ToString())
                        {
                            case "Polygon":
                                return PolygonList2OriginalGeometry(coordinates, "Polygon");
                            case "MultiPolygon":
                                return PolygonList2OriginalGeometry([.. coordinates.EnumerateArray()], "MultiPolygon");
                            case "LineString":
                                break;
                            case "MultiLineString":
                                break;
                            default:
                                throw new JsonException("JSONの解析に失敗しました。", new NotImplementedException("geometryのtypeが対応外です。値:" + type));
                        }
                    else
                        throw new JsonException("JSONの解析に失敗しました。", new ArgumentException("geometryのcoordinatesの取得に失敗しました。"));
                else
                    throw new JsonException("JSONの解析に失敗しました。", new ArgumentException("geometryのtypeの取得に失敗しました。"));
                return null;
            }

            /// <summary>
            /// オブジェクトから各座標を取得します。
            /// </summary>
            /// <param name="singleObject">1つのオブジェクト(<c>[[x1,y1],[x2,y2,], ...]</c>)</param>
            /// <returns>座標の配列</returns>
            /// <exception cref="JsonException">変換に失敗したとき</exception>
            private static OriginalGeometry.Point[] GetPoints(JsonElement singleObject)
            {
                var pointsList = new List<OriginalGeometry.Point>();
                foreach (var point in singleObject.EnumerateArray())
                {
                    if (point.GetArrayLength() == 2)
                    {
                        var lon = point[0].GetDouble();
                        var lat = point[1].GetDouble();
                        pointsList.Add(new OriginalGeometry.Point { Lat = (float)lat, Lon = (float)lon });
                    }
                    else
                        throw new JsonException("JSONの解析に失敗しました。", new Exception("構造が想定外です。"));
                }
                return [.. pointsList];
            }

            /// <summary>
            /// Polygonの配列を<see cref="OriginalGeometry"/>に変換します。
            /// </summary>
            /// <param name="polygon">Polygon(type=Polygonのみ MultiPolygonは<c>(JsonElement[] polygons, string type)</c>を参照。)</param>
            /// <param name="type">種類(<c>Polygon</c>) ※指定ミス防止のため参照の代入推奨</param>
            /// <returns>変換された値</returns>
            /// <exception cref="JsonException">変換に失敗したとき</exception>
            private static OriginalGeometry PolygonList2OriginalGeometry(JsonElement polygon, string type)
            {
                if (type != "Polygon")
                    throw new JsonException("JSONの解析に失敗しました。", new Exception("typeが不正です。"));
                return PolygonList2OriginalGeometry([polygon], type);
            }

            /// <summary>
            /// Polygonの配列を<see cref="OriginalGeometry"/>に変換します。
            /// </summary>
            /// <param name="polygons">Polygon配列(type=Polygon: <c>[coordinates]</c>、MultiPolygon: <c>[.. coordinates.EnumerateArray()]</c>)</param>
            /// <param name="type">種類(<c>Polygon</c>または<c>MultiPolygon</c>) ※指定ミス防止のため参照の代入推奨</param>
            /// <returns>変換された値</returns>
            /// <exception cref="JsonException">変換に失敗したとき</exception>
            private static OriginalGeometry PolygonList2OriginalGeometry(JsonElement[] polygons, string type)
            {
                if (type != "Polygon" && type != "MultiPolygon")
                    throw new JsonException("JSONの解析に失敗しました。", new Exception("typeが不正です。"));
                var polygonsPointsList = new List<List<OriginalGeometry.Point[]>>();
                foreach (var singleObject in polygons)
                {
                    var polygonsPoints = new List<OriginalGeometry.Point[]>();//mainはpolygonsPoints[0]、subはあるときpolygonsPoints[1]として
                    if (singleObject.ValueKind == JsonValueKind.Array)
                        foreach (var element_pts in singleObject.EnumerateArray())
                            if (element_pts.ValueKind == JsonValueKind.Array)
                            {
                                var points = GetPoints(element_pts);
                                polygonsPoints.Add(points);
                            }
                            else
                                throw new JsonException("JSONの解析に失敗しました。", new Exception("構造が想定外です。"));
                    else
                        throw new JsonException("JSONの解析に失敗しました。", new Exception("構造が想定外です。"));
                    polygonsPointsList.Add(polygonsPoints);
                }
                return new OriginalGeometry
                {
                    Type = type,
                    Coordinates = new OriginalGeometry.OriginalCoordinates
                    {
                        Objects = polygonsPointsList.Select(p => new OriginalGeometry.OriginalCoordinates.SingleObject
                        {
                            MainPoints = p[0],
                            HolePoints = p.Count == 2 ? p[1] : null
                        }).ToArray()
                    }
                };
            }

            public override void Write(Utf8JsonWriter writer, OriginalGeometry? value, JsonSerializerOptions options)
            {
                throw new NotImplementedException("書き込みは未実装です。");
            }
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
