using System.ComponentModel;

namespace QuakeMapFast.Utils
{
    public static partial class Utils
    {
        /// <summary>
        /// P2p地震情報 JSON API v2 - 情報コード
        /// </summary>
        /// <remarks>GitHub(<see href="https://github.com/p2pquake/epsp-peer-cs/blob/master/JsonApi/Client.cs"/>)も参考にしています。 値は551(地震情報)、552(津波予報)、554(緊急地震速報 発表検出)、555(各地域ピア数)、556(緊急地震速報（警報）)、561(地震感知情報)、9611(地震感知情報 解析結果)です。</remarks>
        public enum P2PQ_JMAQuake_code
        {
            /// <summary>
            /// (未設定)
            /// </summary>
            [Description("<設定されていません>")]
            Undefined = -9,
            /// <summary>
            /// (未実装)
            /// </summary>
            [Description("<未実装です>")]
            NotImplemented = -8,
            /// <summary>
            /// 地震情報
            /// </summary>
            [Description("地震情報")]
            Earthquake = 551,
            /// <summary>
            /// 津波予報
            /// </summary>
            [Description("津波予報")]
            Tsunami = 552,
            /// <summary>
            /// 津波予報
            /// </summary>
            /// <remarks>以前のバージョンで受信あり？</remarks>
            [Description("津波予報")]
            Tsunami2 = 5520,
            /// <summary>
            /// 緊急地震速報 発表検出
            /// </summary>
            [Description("緊急地震速報 発表検出")]
            EEWTest = 554,
            /// <summary>
            /// 各地域ピア数
            /// </summary>
            [Description("各地域ピア数")]
            PeerCount = 555,
            /// <summary>
            /// 緊急地震速報（警報）
            /// </summary>
            [Description("緊急地震速報（警報）")]
            EEW = 556,
            /// <summary>
            /// 地震感知情報
            /// </summary>
            [Description("地震感知情報")]
            Userquake = 561,
            /// <summary>
            /// 地震感知情報 解析結果
            /// </summary>
            [Description("地震感知情報 解析結果")]
            UserquakeEvaluation = 9611
        }


        /// <summary>
        /// P2P地震情報 JSON API v2 - JMAQuake - 発表種類
        /// </summary>
        ///<remarks>[ ScalePrompt(震度速報), Destination(震源に関する情報), ScaleAndDestination(震度・震源に関する情報), DetailScale(各地の震度に関する情報), Foreign(遠地地震に関する情報), Other(その他の情報) ]</remarks>
        public enum P2PQ_JMAQuake_type
        {
            /// <summary>
            /// (未設定)
            /// </summary>
            [Description("<設定されていません>")]
            Undefined = -9,
            /// <summary>
            /// (未実装)
            /// </summary>
            [Description("<未実装です>")]
            NotImplemented = -8,
            /// <summary>
            /// ScalePrompt(震度速報)
            /// </summary>
            [Description("震度速報")]
            ScalePrompt = 1,
            /// <summary>
            /// Destination(震源に関する情報)
            /// </summary>
            [Description("震源に関する情報")]
            Destination = 2,
            /// <summary>
            /// ScaleAndDestination(震度・震源に関する情報)
            /// </summary>
            /// <remarks>気象庁表記: 不明</remarks>
            [Description("(震度・震源に関する情報)")]
            ScaleAndDestination = 3,
            /// <summary>
            /// DetailScale(各地の震度に関する情報)
            /// </summary>
            /// <remarks>気象庁表記: 震源・震度情報</remarks>
            [Description("震源・震度情報")]
            DetailScale = 4,
            /// <summary>
            /// Foreign(遠地地震に関する情報)
            /// </summary>
            [Description("遠地地震に関する情報")]
            Foreign = 5,
            /// <summary>
            /// Other(その他の情報)
            /// </summary>
            [Description("その他の情報")]
            Other = 6
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 - 震度
        /// </summary>
        /// <remarks>[ -1(震度情報なし), 10(震度1), 20(震度2), 30(震度3), 40(震度4), 45(震度5弱), 50(震度5強), 55(震度6弱), 60(震度6強), 70(震度7) ]</remarks>
        public enum P2PQ_Scales
        {
            /// <summary>
            /// (未設定)
            /// </summary>
            [Description("<設定されていません>")]
            Undefined = -9,
            /// <summary>
            /// (未実装)
            /// </summary>
            [Description("<未実装です>")]
            NotImplemented = -8,
            /// <summary>
            /// 震度情報なし
            /// </summary>
            [Description("震度情報なし")]
            None = -1,
            /// <summary>
            /// 震度1
            /// </summary>
            [Description("震度1")]
            S1 = 10,
            /// <summary>
            /// 震度2
            /// </summary>
            [Description("震度2")]
            S2 = 20,
            /// <summary>
            /// 震度3
            /// </summary>
            [Description("震度3")]
            S3 = 30,
            /// <summary>
            /// 震度4
            /// </summary>
            [Description("震度4")]
            S4 = 40,
            /// <summary>
            /// 震度5弱
            /// </summary>
            [Description("震度5弱")]
            S5m = 45,
            /// <summary>
            /// 未入電(震度5弱以上推定)
            /// </summary>
            /// <remarks>震度５弱以上と考えられるが現在震度を入手していない市</remarks>
            [Description("未入電(震度5弱以上推定)")]
            SUnknown = 46,
            /// <summary>
            /// 震度5強
            /// </summary>
            [Description("震度5強")]
            S5p = 50,
            /// <summary>
            /// 震度6弱
            /// </summary>
            [Description("震度6弱")]
            S6m = 55,
            /// <summary>
            /// 震度6強
            /// </summary>
            [Description("震度6強")]
            S6p = 60,
            /// <summary>
            /// 震度7
            /// </summary>
            [Description("震度7")]
            S7 = 70
        }



        /// <summary>
        /// P2P地震情報 JSON API v2 - JMAQuake - 訂正の有無
        /// </summary>
        ///<remarks>[ None(なし), Unknown(不明), ScaleOnly(震度), DestinationOnly(震源), ScaleAndDestination(震度・震源) ]</remarks>
        public enum P2PQ_JMAQuake_correct
        {
            /// <summary>
            /// (未設定)
            /// </summary>
            [Description("<設定されていません>")]
            Undefined = -9,
            /// <summary>
            /// (未実装)
            /// </summary>
            [Description("<未実装です>")]
            NotImplemented = -8,
            /// <summary>
            /// None(なし)
            /// </summary>
            [Description("なし")]
            None = 0,
            /// <summary>
            /// Unknown(不明)
            /// </summary>
            [Description("不明")]
            Unknown = 1,
            /// <summary>
            /// ScaleOnly(震度)
            /// </summary>
            [Description("震度")]
            ScaleOnly = 11,
            /// <summary>
            /// DestinationOnly(震源)
            /// </summary>
            [Description("震源")]
            DestinationOnly = 12,
            /// <summary>
            /// ScaleAndDestination(震度・震源)
            /// </summary>
            [Description("震度・震源")]
            ScaleAndDestination = 13
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 - 国内への津波の有無
        /// </summary>
        /// <remarks>[ None(なし), Unknown(不明), Checking(調査中), NonEffective(若干の海面変動が予想されるが、被害の心配なし), Watch(津波注意報), Warning(津波予報(種類不明)) ]</remarks>
        public enum P2PQ_domesticTsunami
        {
            /// <summary>
            /// (未設定)
            /// </summary>
            [Description("<設定されていません>")]
            Undefined = -9,
            /// <summary>
            /// (未実装)
            /// </summary>
            [Description("<未実装です>")]
            NotImplemented = -8,
            /// <summary>
            /// None(なし), この地震による津波の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 215</remarks>
            [Description("この地震による津波の心配はありません。")]
            None = 0,
            /// <summary>
            /// Unknown(不明), (不明)
            /// </summary>
            [Description("<国内への津波について不明です>")]
            Unknown = 1,
            /// <summary>
            /// Checking(調査中), 今後の情報に注意してください。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 217</remarks>
            [Description("今後の情報に注意してください。")]
            Checking = 2,
            /// <summary>
            /// NonEffective(若干の海面変動が予想されるが、被害の心配なし), この地震により、日本の沿岸では若干の海面変動があるかもしれませんが、被害の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 212</remarks>
            [Description("この地震により、日本の沿岸では若干の海面変動があるかもしれませんが、被害の心配はありません。")]
            NonEffective = 3,
            /// <summary>
            /// Watch(津波注意報), (津波注意報)
            /// </summary>
            [Description("津波注意報を発表中です。")]
            Watch = 4,
            /// <summary>
            /// Warning(津波予報(種類不明)), 津波警報等（大津波警報・津波警報あるいは津波注意報）を発表中です。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 211</remarks>
            [Description("津波警報等（大津波警報・津波警報あるいは津波注意報）を発表中です。")]
            Warning = 5
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 - 海外での津波の有無
        /// </summary>
        /// <remarks>[ None(なし), Unknown(不明), Checking(調査中), NonEffectiveNearby(震源の近傍で小さな津波の可能性があるが、被害の心配なし), WarningNearby(震源の近傍で津波の可能性がある), WarningPacific(太平洋で津波の可能性がある), WarningPacificWide(太平洋の広域で津波の可能性がある), WarningIndian(インド洋で津波の可能性がある), WarningIndianWide(インド洋の広域で津波の可能性がある), Potential(一般にこの規模では津波の可能性がある) ]</remarks>
        public enum P2PQ_foreignTsunami
        {
            /// <summary>
            /// (未設定)
            /// </summary>
            [Description("<設定されていません>")]
            Undefined = -9,
            /// <summary>
            /// (未実装)
            /// </summary>
            [Description("<未実装です>")]
            NotImplemented = -8,
            /// <summary>
            /// None(なし), この地震による津波の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 215</remarks>
            [Description("この地震による津波の心配はありません。")]
            None = 0,
            /// <summary>
            /// Unknown(不明), (不明)
            /// </summary>
            [Description("<海外での津波について不明です>")]
            Unknown = 1,
            /// <summary>
            /// Checking(調査中), 今後の情報に注意してください。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 217</remarks>
            [Description("今後の情報に注意してください。")]
            Checking = 2,
            /// <summary>
            /// NonEffectiveNearby(震源の近傍で小さな津波の可能性があるが、被害の心配なし), 震源の近傍で小さな津波発生の可能性がありますが、被害をもたらす津波の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 227</remarks>
            [Description("震源の近傍で小さな津波発生の可能性がありますが、被害をもたらす津波の心配はありません。")]
            NonEffectiveNearby = 3,
            /// <summary>
            /// WarningNearby(震源の近傍で津波の可能性がある), 震源の近傍で津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 226</remarks>
            [Description("震源の近傍で津波発生の可能性があります。")]
            WarningNearby = 4,
            /// <summary>
            /// WarningPacific(太平洋で津波の可能性がある), 太平洋で津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 222</remarks>
            [Description("太平洋で津波発生の可能性があります。")]
            WarningPacific = 5,
            /// <summary>
            /// WarningPacificWide(太平洋の広域で津波の可能性がある), 太平洋の広域に津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 221</remarks>
            [Description("太平洋の広域に津波発生の可能性があります。")]
            WarningPacificWide = 6,
            /// <summary>
            /// WarningIndian(インド洋で津波の可能性がある), インド洋で津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 225</remarks>
            [Description("インド洋で津波発生の可能性があります。")]
            WarningIndian = 7,
            /// <summary>
            /// WarningIndianWide(インド洋の広域で津波の可能性がある), インド洋の広域に津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 224</remarks>
            [Description("インド洋の広域に津波発生の可能性があります。")]
            WarningIndianWide = 8,
            /// <summary>
            /// Potential(一般にこの規模では津波の可能性がある), 一般的に、この規模の地震が海域の浅い領域で発生すると、津波が発生することがあります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 228</remarks>
            [Description("一般的に、この規模の地震が海域の浅い領域で発生すると、津波が発生することがあります。")]
            Potential = 9
        }

    }
}
