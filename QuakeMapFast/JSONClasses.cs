using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuakeMapFast
{
    /// <summary>
    /// JSONのクラス。
    /// </summary>
    /// <remarks>
    /// P2P地震情報 JSON API v2(P2PQuake_...)のものはP2P地震情報 API仕様書(<see href="https://www.p2pquake.net/develop/json_api_v2/"/>)より作成しました。
    /// *表記のものは<c>required</c>、それ以外はnull許容値型(<c>T?</c>)です。
    /// </remarks>
    public class JSONClasses
    {
        /// <summary>
        /// 独自観測点と座標対応データ用クラス
        /// </summary>
        /// <remarks>レポジトリ: <see href="https://github.com/Ichihai1415/JmaEqPointsConverter"/> データ: <see href="https://raw.githubusercontent.com/Ichihai1415/JmaEqPointsConverter/refs/heads/main/JmaEqPointsConverter/output/PointSeismicIntensity.json"/></remarks>
        public class ObsPoints
        {
            [JsonPropertyName("updateDate")]
            public required string UpdateDate { get; set; }

            [JsonPropertyName("pref")]
            public required C_Pref[] Pref { get; set; }


            public class C_Pref
            {
                [JsonPropertyName("name")]
                public required string Name { get; set; }

                [JsonPropertyName("code")]
                public required string Code { get; set; }

                [JsonPropertyName("area")]
                public required C_Area[] Area { get; set; }
            }

            public class C_Area
            {
                [JsonPropertyName("name")]
                public required string Name { get; set; }

                [JsonPropertyName("code")]
                public required string Code { get; set; }

                [JsonPropertyName("city")]
                public required C_City[] City { get; set; }
            }

            public class C_City
            {

                [JsonPropertyName("name")]
                public required string Name { get; set; }

                [JsonPropertyName("code")]
                public required string Code { get; set; }

                [JsonPropertyName("point")]
                public required C_Point[] Point { get; set; }
            }

            public class C_Point
            {
                [JsonPropertyName("name")]
                public required string Name { get; set; }

                [JsonPropertyName("code")]
                public required string Code { get; set; }

                [JsonPropertyName("lat")]
                public required float Lat { get; set; }

                [JsonPropertyName("lon")]
                public required float Lon { get; set; }
            }
        }

        /// <summary>
        /// 気象庁GISデータのGeoJSONの地図用クラス
        /// </summary>
        public class GeoJSON_JMA_Map
        {
            /// <summary>
            /// 常にFeatureCollection
            /// </summary>
            [JsonPropertyName("type")]
            public required string Type { get; set; }

            /// <summary>
            /// 地理要素の配列
            /// </summary>
            [JsonPropertyName("features")]
            public required C_Feature[] Features { get; set; }

            /// <summary>
            /// 地理要素
            /// </summary>
            public class C_Feature
            {
                /// <summary>
                /// 常にFeature
                /// </summary>
                [JsonPropertyName("type")]
                public required string Type { get; set; }

                /// <summary>
                /// 地物(nullの可能性あり)
                /// </summary>
                /// <remarks>自作クラス(<see cref="OriginalGeometry"/>?)に格納します。nullの場合のfeature例: <c>{"type":"Feature","geometry":null,"properties":{"code":"","name":"鷹島(甑島南方)","namekana":""}}</c></remarks>
                [JsonPropertyName("geometry")]
                public OriginalGeometry? Geometry { get; set; }

                [JsonPropertyName("properties")]
                public required C_Properties Properties { get; set; }
            }
            /*//旧クラス
            public class C_Geometry
            {
                /// <summary>
                /// Polygon/MultiPolygon
                /// </summary>
                [JsonPropertyName("type")]
                public required string Type { get; set; }

                /// <summary>
                /// 座標の配列
                /// </summary>
                /// <remarks><c>"type":"Polygon"</c>の場合、<c>double</c>(<c>double[外側(,内側)][点配列][(経度,緯度)配列]</c>)、<c>"type":"MultiPolygon"</c>の場合、<c>double[]</c>(<c>double[ポリゴン配列][外側(,内側)][点配列][(経度,緯度)配列]</c>)</remarks>
                [JsonPropertyName("coordinates")]
                public required OriginalGeometry Coordinates { get; set; }
            }
            */
            /// <summary>
            /// 地物の詳細
            /// </summary>
            public class C_Properties
            {
                /// <summary>
                /// 気象庁コード
                /// </summary>
                [JsonPropertyName("code")]
                public required string Code { get; set; }

                /// <summary>
                /// 名称
                /// </summary>
                [JsonPropertyName("name")]
                public required string Name { get; set; }

                /// <summary>
                /// 名称(かな)
                /// </summary>
                [JsonPropertyName("namekana")]
                public required string Namekana { get; set; }
            }
        }


        /// <summary>
        /// 独自クラスのGeometry
        /// </summary>
        public class OriginalGeometry
        {
            /// <summary>
            /// Polygon/MultiPolygon
            /// </summary>
            public required string Type { get; set; }

            /// <summary>
            /// 独自クラスのcoordinates
            /// </summary>
            public required OriginalCoordinates Coordinates { get; set; }

            /// <summary>
            /// 独自クラスのcoordinates
            /// </summary>
            public class OriginalCoordinates
            {
                /// <summary>
                /// 1オブジェクトの配列(Geometryではない)
                /// </summary>
                public required SingleObject[] Objects { get; set; }

                /// <summary>
                ///  1オブジェクト(Geometryではない)
                /// </summary>
                public class SingleObject
                {
                    /// <summary>
                    /// 通常の座標の配列
                    /// </summary>
                    public required Point[] MainPoints { get; set; }

                    /// <summary>
                    /// Polygonの中空き用の座標の配列
                    /// </summary>
                    public Point[]? HolePoints { get; set; }
                }
            }

            /// <summary>
            /// 座標
            /// </summary>
            public class Point
            {
                /// <summary>
                /// 経度(x座標)
                /// </summary>
                public required float Lon { get; set; }

                /// <summary>
                /// 緯度(y座標)
                /// </summary>
                public required float Lat { get; set; }

                /// <summary>
                /// 標高(y座標)
                /// </summary>
                /// <remarks>必須ではない</remarks>
                //public float? height { get; set; }//軽量化のためOFF
            }
        }

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
                                return PolygonList2OriginalGeometry([coordinates], "Polygon");
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
        /// P2P地震情報 JSON API v2 - 地震情報
        /// </summary>
        public class P2PQuake_JMAQuake
        {
            /// <summary>
            /// 情報を一意に識別するID
            /// </summary>
            [JsonPropertyName("id")]
            public required string Id { get; set; }

            /// <summary>
            /// 受信日時。形式は 2006/01/02 15:04:05.999 です。
            /// </summary>
            [JsonPropertyName("time")]
            public required string Time { get; set; }

            /// <summary>
            /// 情報コード。常に551です。
            /// </summary>
            [JsonPropertyName("code")]
            public required int Code { get; set; }

            /// <summary>
            /// 発表元の情報
            /// </summary>
            [JsonPropertyName("issue")]
            public required C_Issue Issue { get; set; }

            /// <summary>
            /// (元説明なし)
            /// </summary>
            [JsonPropertyName("earthquake")]
            public required C_Earthquake Earthquake { get; set; }

            /// <summary>
            /// 震度観測点の情報
            /// </summary>
            [JsonPropertyName("points")]
            public C_Point[]? Points { get; set; }

            /// <summary>
            /// 付加文（2024年8月下旬から提供予定）
            /// </summary>
            /// <remarks>以前のものにはないため*だがnullable</remarks>
            [JsonPropertyName("comments")]
            public C_Comments? Comments { get; set; }

            /// <summary>
            /// 発表元の情報
            /// </summary>
            public class C_Issue
            {
                /// <summary>
                /// 発表元
                /// </summary>
                [JsonPropertyName("source")]
                public string? Source { get; set; }

                /// <summary>
                /// 発表日時
                /// </summary>
                [JsonPropertyName("time")]
                public required string Time { get; set; }

                /// <summary>
                /// 訂正の有無
                /// </summary>
                /// <remarks>Enum: [None(なし), Unknown(不明), ScaleOnly(震度), DestinationOnly(震源), ScaleAndDestination(震度・震源)]</remarks>
                [JsonPropertyName("correct")]
                public string? Correct { get; set; }

                /// <summary>
                /// 発表種類
                /// </summary>
                /// <remarks>Enum: [ScalePrompt(震度速報), Destination(震源に関する情報), ScaleAndDestination(震度・震源に関する情報), DetailScale(各地の震度に関する情報), Foreign(遠地地震に関する情報), Other(その他の情報)]</remarks>
                [JsonPropertyName("type")]
                public required string Type { get; set; }
            }

            /// <summary>
            /// (元説明なし)
            /// </summary>
            public class C_Earthquake
            {
                /// <summary>
                /// 国内への津波の有無
                /// </summary>
                /// <remarks>Enum: [None(なし), Unknown(不明), Checking(調査中), NonEffective(若干の海面変動が予想されるが、被害の心配なし), Watch(津波注意報), Warning(津波予報(種類不明))]</remarks>
                [JsonPropertyName("domesticTsunami")]
                public string? DomesticTsunami { get; set; }

                /// <summary>
                /// 海外での津波の有無
                /// </summary>
                /// <remarks>Enum: [None(なし), Unknown(不明), Checking(調査中), NonEffectiveNearby(震源の近傍で小さな津波の可能性があるが、被害の心配なし), WarningNearby(震源の近傍で津波の可能性がある), WarningPacific(太平洋で津波の可能性がある), WarningPacificWide(太平洋の広域で津波の可能性がある), WarningIndian(インド洋で津波の可能性がある), WarningIndianWide(インド洋の広域で津波の可能性がある), Potential(一般にこの規模では津波の可能性がある)]</remarks>
                [JsonPropertyName("foreignTsunami")]
                public string? ForeignTsunami { get; set; }

                /// <summary>
                /// 震源情報
                /// </summary>
                [JsonPropertyName("hypocenter")]
                public C_Hypocenter? Hypocenter { get; set; }

                /// <summary>
                /// 最大震度。震度情報が存在しない場合は-1となります。
                /// </summary>
                /// <remarks>Enum: [ -1(震度情報なし), 10(震度1), 20(震度2), 30(震度3), 40(震度4), 45(震度5弱), 50(震度5強), 55(震度6弱), 60(震度6強), 70(震度7) ]</remarks>
                [JsonPropertyName("maxScale")]
                public int? MaxScale { get; set; }

                /// <summary>
                /// 発生日時
                /// </summary>
                [JsonPropertyName("time")]
                public required string Time { get; set; }
            }

            /// <summary>
            /// 震源情報
            /// </summary>
            public class C_Hypocenter
            {
                /// <summary>
                /// 深さ(km)。「ごく浅い」は0、震源情報が存在しない場合は-1となります。
                /// </summary>
                [JsonPropertyName("depth")]
                public int? Depth { get; set; }

                /// <summary>
                /// 緯度。震源情報が存在しない場合は-200となります。
                /// </summary>
                [JsonPropertyName("latitude")]
                public double? Latitude { get; set; }

                /// <summary>
                /// 経度。震源情報が存在しない場合は-200となります。
                /// </summary>
                [JsonPropertyName("longitude")]
                public double? Longitude { get; set; }

                /// <summary>
                /// マグニチュード。震源情報が存在しない場合は-1となります。
                /// </summary>
                [JsonPropertyName("magnitude")]
                public double? Magnitude { get; set; }

                /// <summary>
                /// 名称
                /// </summary>
                [JsonPropertyName("name")]
                public string? Name { get; set; }
            }

            /// <summary>
            /// 付加文（2024年8月下旬から提供予定）
            /// </summary>
            public class C_Comments
            {
                /// <summary>
                /// 自由付加文。ない場合は空文字列となります。気象庁の発表電文に含まれる自由付加文をそのまま提供しており、火山噴火に伴って発表される遠地地震に関する情報では、「大規模な噴火が発生しました」という文言が含まれます（2024年7月現在）。
                /// </summary>
                [JsonPropertyName("freeFormComment")]
                public string? FreeFormComment { get; set; }
            }

            /// <summary>
            /// 震度観測点の情報
            /// </summary>
            public class C_Point
            {
                /// <summary>
                /// 震度観測点名称（震度速報の場合は 気象庁 | 緊急地震速報や震度情報で用いる区域の名称 に記載のある区域名）
                /// </summary>
                [JsonPropertyName("addr")]
                public required string Addr { get; set; }

                /// <summary>
                /// 区域名かどうか
                /// </summary>
                [JsonPropertyName("isArea")]
                public required bool IsArea { get; set; }

                /// <summary>
                /// 都道府県
                /// </summary>
                [JsonPropertyName("pref")]
                public required string Pref { get; set; }

                /// <summary>
                /// 震度
                /// </summary>
                /// <remarks>Enum: [10(震度1), 20(震度2), 30(震度3), 40(震度4), 45(震度5弱), 46(震度5弱以上と推定されるが震度情報を入手していない), 50(震度5強), 55(震度6弱), 60(震度6強), 70(震度7)]</remarks>
                [JsonPropertyName("scale")]
                public required int Scale { get; set; }
            }
        }


    }
}
