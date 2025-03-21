using System.Text.Json.Serialization;

namespace QuakeMapFast.Utils
{
    /// <summary>
    /// JSONのクラス。
    /// </summary>
    /// <remarks>
    /// P2P地震情報 JSON API v2(P2PQuake_...)のものはP2P地震情報 API仕様書(<see href="https://www.p2pquake.net/develop/json_api_v2/"/>)より作成しました。
    /// *表記のものは<c>required</c>、それ以外はnull許容値型(<c>T?</c>)です。
    /// </remarks>
    public static class JSONClasses
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