using System.Runtime.Versioning;
using System.Text.Json.Nodes;

namespace QuakeMapFast
{
    [SupportedOSPlatform("windows7.0")]
    public static class Conv
    {
        /// <summary>
        /// P2Pのcodeと説明
        /// </summary>
        public static readonly Dictionary<int, string> P2PInfoCodeName = new()
        {
            { 551, "(地震情報)" },
            { 552, "(津波予報)" },
            { 5520, "(津波予報)" },
            { 554, "(緊急地震速報 発表検出)" },
            { 555, "(各地域ピア数)" },
            { 556, "(緊急地震速報(警報))" },
            { 561, "(地震感知情報)" },
            { 9611, "(地震感知情報 解析結果)" }
        };

        /// <summary>
        /// P2Pの地震情報のtypeと説明
        /// </summary>
        public static readonly Dictionary<string, string> P2PInfoTypeName = new()
        {
            { "", "-" },
            { "ScalePrompt", "(震度速報)" },
            { "Destination", "(震源に関する情報)" },
            { "ScaleAndDestination", "(震度・震源に関する情報)" },
            { "DetailScale", "(各地の震度に関する情報)" },
            { "Foreign", "(遠地地震に関する情報)" },
            { "Other", "(その他の情報)" }
        };

        /// <summary>
        /// 震度別のList<string>
        /// </summary>
        public class IntList
        {
            public List<string> S1 { get; set; } = [];
            public List<string> S2 { get; set; } = [];
            public List<string> S3 { get; set; } = [];
            public List<string> S4 { get; set; } = [];
            public List<string> S5 { get; set; } = [];
            public List<string> S6 { get; set; } = [];
            public List<string> S7 { get; set; } = [];
            public List<string> S8 { get; set; } = [];
            public List<string> S9 { get; set; } = [];
        }

        /// <summary>
        /// P2Pjsonの震度をint形式(0,1,..,8,9)に変換します。
        /// </summary>
        /// <param name="scale">P2Pjsonの震度</param>
        /// <returns>int形式の震度</returns>
        public static int P2PScale2IntN(int scale)
        {
            switch (scale)
            {
                case 10:
                    return 1;
                case 20:
                    return 2;
                case 30:
                    return 3;
                case 40:
                    return 4;
                case 45:
                    return 5;
                case 50:
                    return 6;
                case 55:
                    return 7;
                case 60:
                    return 8;
                case 70:
                    return 9;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// P2Pjsonの震度をstring形式(-,1,..,6強,7)に変換します。
        /// </summary>
        /// <param name="scale">P2Pjsonの震度</param>
        /// <returns>int形式の震度</returns>
        public static string P2PScale2IntS(int scale)
        {
            switch (scale)
            {
                case 10:
                    return "1";
                case 20:
                    return "2";
                case 30:
                    return "3";
                case 40:
                    return "4";
                case 45:
                    return "5弱";
                case 50:
                    return "5強";
                case 55:
                    return "6弱";
                case 60:
                    return "6強";
                case 70:
                    return "7";
                default://-1,99
                    return "-";
            }
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 の震度が6弱以上か判定します。
        /// </summary>
        /// <param name="scaleFrom">震度の下限</param>
        /// <param name="scaleTo">震度の上限</param>
        /// <returns>6弱以上の場合<c>true</c></returns>
        public static bool P2PQScale2isOver6(int scale)
        {
            return scale <= 70 && scale >= 55;//55<=scale<=70 
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 の震度が6弱以上か判定します。
        /// </summary>
        /// <param name="scaleFrom">震度の下限</param>
        /// <param name="scaleTo">震度の上限</param>
        /// <returns>6弱以上の場合<c>true</c></returns>
        public static bool P2PQScale2isOver6(int scaleFrom, int scaleTo)
        {
            return scaleTo <= 70 ?
                scaleTo >= 55 :
                scaleFrom <= 70 && scaleFrom >= 55;//55<=scaleTo<=70 
        }

        /// <summary>
        /// jsonから指定した区分の地区ごとの震度のDictionaryを返します。
        /// </summary>
        /// <param name="points">jsonのpoints</param>
        /// <param name="token">addr(地点・区分)/pref(県)</param>
        /// <returns>Dictionary<地区, int形式の震度></returns>
        public static Dictionary<string, int> Points2Dic(JsonNode points, string token)
        {
            return points.AsArray().ToDictionary(pt => (string)pt![token]!, pt => P2PScale2IntN((int)pt!["scale"]!));
        }

        /// <summary>
        /// jsonから震度別の指定した区分の地区を返します。
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="token">addr(地点・区分)/pref(県)</param>
        /// <returns>IntList</returns>
        public static IntList Point2IntList(JsonNode json, string token)
        {
            var intList = new IntList();
            foreach (JsonNode json_ in json["points"].AsArray())
            {
                switch ((int)json_["scale"])
                {
                    case 10:
                        intList.S1.Add((string)json_[token]);
                        break;
                    case 20:
                        intList.S2.Add((string)json_[token]);
                        break;
                    case 30:
                        intList.S3.Add((string)json_[token]);
                        break;
                    case 40:
                        intList.S4.Add((string)json_[token]);
                        break;
                    case 45:
                        intList.S5.Add((string)json_[token]);
                        break;
                    case 50:
                        intList.S6.Add((string)json_[token]);
                        break;
                    case 55:
                        intList.S7.Add((string)json_[token]);
                        break;
                    case 60:
                        intList.S8.Add((string)json_[token]);
                        break;
                    case 70:
                        intList.S9.Add((string)json_[token]);
                        break;
                }
            }
            return intList;
        }

        /// <summary>
        /// IntListから震度別の文字列を返します。
        /// </summary>
        /// <param name="intList">IntList</param>
        /// <param name="mininumInt">文字列にする最小の震度</param>
        /// <returns>震度別の文字列</returns>
        /// <remarks>例:《震度4》○○ ○○ \n《震度3》○○ ○○ </remarks>
        public static string IntList2String(IntList intList, int mininumInt = 0)
        {
            string output = "\n";
            if (intList.S9.Count != 0 && mininumInt <= 9)
                output += "\n《震度7》" + string.Join(" ", intList.S9);
            if (intList.S8.Count != 0 && mininumInt <= 8)
                output += "\n《震度6強》" + string.Join(" ", intList.S8);
            if (intList.S7.Count != 0 && mininumInt <= 7)
                output += "\n《震度6弱》" + string.Join(" ", intList.S7);
            if (intList.S6.Count != 0 && mininumInt <= 6)
                output += "\n《震度5強》" + string.Join(" ", intList.S6);
            if (intList.S5.Count != 0 && mininumInt <= 5)
                output += "\n《震度5弱》" + string.Join(" ", intList.S5);
            if (intList.S4.Count != 0 && mininumInt <= 4)
                output += "\n《震度4》" + string.Join(" ", intList.S4);
            if (intList.S3.Count != 0 && mininumInt <= 3)
                output += "\n《震度3》" + string.Join(" ", intList.S3);
            if (intList.S2.Count != 0 && mininumInt <= 2)
                output += "\n《震度2》" + string.Join(" ", intList.S2);
            if (intList.S1.Count != 0 && mininumInt <= 1)
                output += "\n《震度1》" + string.Join(" ", intList.S1);
            return output.Replace("\n\n", "");
        }

        /// <summary>
        /// jsonから震度別の文字列を返します。
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="Token">addr(地点・区分)/pref(県)</param>
        /// <param name="MininumInt">文字列にする最小のint形式の震度</param>
        /// <returns></returns>
        public static string Point2String(JsonNode json, string Token, int LowestInt = 0)
        {
            return IntList2String(Point2IntList(json, Token), LowestInt);
        }

        /// <summary>
        /// 画像描画用に緯度・経度を補正します
        /// </summary>
        /// <param name="latSta">緯度の始点</param>
        /// <param name="latEnd">緯度の終点</param>
        /// <param name="lonSta">経度の始点</param>
        /// <param name="lonEnd">経度の終点</param>
        /// <param name="enableCorrectMax">最大値を補正するか</param>
        public static void PointCorrect(ref float latSta, ref float latEnd, ref float lonSta, ref float lonEnd, bool enableCorrectMax = false)
        {
            latSta -= (latEnd - latSta) / 20f;//差の1/20余白追加
            latEnd += (latEnd - latSta) / 20f;
            lonSta -= (lonEnd - lonSta) / 20f;
            lonEnd += (lonEnd - lonSta) / 20f;
            if (latEnd - latSta < 3f)//緯度差を最小3に
            {
                var correction = (3f - (latEnd - latSta)) / 2f;
                latSta -= correction;
                latEnd += correction;
            }
            if (latEnd - latSta < 3f)//経度差を最小3に
            {
                var correction = (3f - (lonEnd - lonSta)) / 2f;
                lonSta -= correction;
                lonEnd += correction;
            }
            if (enableCorrectMax)
            {
                if (latEnd - latSta > 10f) //緯度差を最大10に
                {
                    var correction = ((latEnd - latSta) - 10f) / 2f;
                    latSta += correction;
                    latEnd -= correction;
                }
                if (lonEnd - lonSta > 10f) //経度差を最大10に
                {
                    var correction = ((lonEnd - lonSta) - 10f) / 2f;
                    lonSta += correction;
                    lonEnd -= correction;
                }
            }

            if (lonEnd - lonSta > latEnd - latSta)//大きいほうに合わせる
            {
                var correction = ((lonEnd - lonSta) - (latEnd - latSta)) / 2f;
                latSta -= correction;
                latEnd += correction;
            }
            else// if (LonEnd - LonSta < LatEnd - LatSta)
            {
                var correction = ((latEnd - latSta) - (lonEnd - lonSta)) / 2f;
                lonSta -= correction;
                lonEnd += correction;
            }
        }

        /// <summary>
        /// 震度から震度色を返します。
        /// </summary>
        /// <param name="Int">int形式の震度</param>
        /// <returns>SolidBrush形式の震度色</returns>
        /// <remarks>配色はKiwi Monitor カラースキーム第2版を改変したものです。</remarks>
        public static SolidBrush IntN2Brush(int Int)
        {
            switch (Int)
            {
                case 0:
                    return new SolidBrush(Color.FromArgb(80, 90, 100));
                case 1:
                    return new SolidBrush(Color.FromArgb(60, 80, 100));
                case 2:
                    return new SolidBrush(Color.FromArgb(45, 90, 180));
                case 3:
                    return new SolidBrush(Color.FromArgb(50, 175, 175));
                case 4:
                    return new SolidBrush(Color.FromArgb(240, 240, 60));
                case 5:
                    return new SolidBrush(Color.FromArgb(250, 150, 0));
                case 6:
                    return new SolidBrush(Color.FromArgb(250, 75, 0));
                case 7:
                    return new SolidBrush(Color.FromArgb(200, 0, 0));
                case 8:
                    return new SolidBrush(Color.FromArgb(100, 0, 0));
                case 9:
                    return new SolidBrush(Color.FromArgb(100, 0, 100));
                default:
                    return new SolidBrush(Color.FromArgb(30, 60, 90));
            }
        }

        /// <summary>
        /// 震度から文字色を返します。
        /// </summary>
        /// <param name="Int">int形式の震度</param>
        /// <returns>Brush形式の文字色</returns>
        public static Brush IntN2TextBrush(int Int)
        {
            if (Int >= 3 && Int <= 6)
                return Brushes.Black;
            else
                return Brushes.White;
        }

        /// <summary>
        /// 震度からTelop用の色を返します。
        /// </summary>
        /// <param name="Int">int形式の震度</param>
        /// <returns>Telop形式の文字列</returns>
        /// <remarks>例:60,70,80,White,80,90,100,White</remarks>
        public static string Int2TelopColor(int Int)
        {
            switch (Int)
            {
                case 0:
                    return "60,70,80,White,80,90,100,White";
                case 1:
                    return "40,60,80,White,60,80,100,White";
                case 2:
                    return "30,60,150,White,45,90,180,White";
                case 3:
                    return "25,150,150,Black,50,175,175,Black";
                case 4:
                    return "200,200,40,Black,240,240,60,Black";
                case 5:
                    return "200,130,0,Black,250,150,0,Black";
                case 6:
                    return "200,50,0,Black,250,75,0,Black";
                case 7:
                    return "180,0,0,White,200,0,0,White";
                case 8:
                    return "80,0,0,White,100,0,0,White";
                case 9:
                    return "80,0,80,White,100,0,100,White";
                default:
                    return "15,30,45,White,30,60,90,White";
            }
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 - JMAQuake - 発表種類
        /// </summary>
        ///<remarks>[ ScalePrompt(震度速報), Destination(震源に関する情報), ScaleAndDestination(震度・震源に関する情報), DetailScale(各地の震度に関する情報), Foreign(遠地地震に関する情報), Other(その他の情報) ]</remarks>
        public enum P2PQ_JMAQuake_type
        {
            /// <summary>
            /// (未実装)
            /// </summary>
            NotImplemented = -9,
            /// <summary>
            /// ScalePrompt(震度速報)
            /// </summary>
            ScalePrompt = 1,
            /// <summary>
            /// Destination(震源に関する情報)
            /// </summary>
            Destination = 2,
            /// <summary>
            /// ScaleAndDestination(震度・震源に関する情報)
            /// </summary>
            ScaleAndDestination = 3,
            /// <summary>
            /// DetailScale(各地の震度に関する情報)
            /// </summary>
            DetailScale = 4,
            /// <summary>
            /// Foreign(遠地地震に関する情報)
            /// </summary>
            Foreign = 5,
            /// <summary>
            /// Other(その他の情報)
            /// </summary>
            Other = 6
        }

        /// <summary>
        /// JMAQuake - 発表種類 - String2Enum
        /// </summary>
        /// <param name="str">変換する文字列</param>
        /// <returns>対応する<see cref="P2PQ_JMAQuake_type"/></returns>
        public static P2PQ_JMAQuake_type P2PQ_JMAQuake_type_String2Enum(string str)
        {
            return str switch
            {
                "ScalePrompt" => P2PQ_JMAQuake_type.ScalePrompt,
                "Destination" => P2PQ_JMAQuake_type.Destination,
                "ScaleAndDestination" => P2PQ_JMAQuake_type.ScaleAndDestination,
                "DetailScale" => P2PQ_JMAQuake_type.DetailScale,
                "Foreign" => P2PQ_JMAQuake_type.Foreign,
                "Other" => P2PQ_JMAQuake_type.Other,
                _ => P2PQ_JMAQuake_type.NotImplemented
            };
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 - JMAQuake - 訂正の有無
        /// </summary>
        ///<remarks>[ None(なし), Unknown(不明), ScaleOnly(震度), DestinationOnly(震源), ScaleAndDestination(震度・震源) ]</remarks>
        public enum P2PQ_JMAQuake_correct
        {
            /// <summary>
            /// (未実装)
            /// </summary>
            NotImplemented = -9,
            /// <summary>
            /// None(なし)
            /// </summary>
            None = 0,
            /// <summary>
            /// Unknown(不明)
            /// </summary>
            Unknown = 1,
            /// <summary>
            /// ScaleOnly(震度)
            /// </summary>
            ScaleOnly = 2,
            /// <summary>
            /// DestinationOnly(震源)
            /// </summary>
            DestinationOnly = 3,
            /// <summary>
            /// ScaleAndDestination(震度・震源)
            /// </summary>
            ScaleAndDestination = 4
        }

        /// <summary>
        /// JMAQuake - 訂正の有無 - String2Enum
        /// </summary>
        /// <param name="str">変換する文字列</param>
        /// <returns>対応する<see cref="P2PQ_JMAQuake_correct"/></returns>
        public static P2PQ_JMAQuake_correct P2PQ_JMAQuake_correct_String2Enum(string str)
        {
            return str switch
            {
                "None" => P2PQ_JMAQuake_correct.None,
                "Unknown" => P2PQ_JMAQuake_correct.Unknown,
                "ScaleOnly" => P2PQ_JMAQuake_correct.ScaleOnly,
                "DestinationOnly" => P2PQ_JMAQuake_correct.DestinationOnly,
                "ScaleAndDestination" => P2PQ_JMAQuake_correct.ScaleAndDestination,
                _ => P2PQ_JMAQuake_correct.NotImplemented
            };
        }

        /// <summary>
        /// P2P地震情報 JSON API v2 - 国内への津波の有無
        /// </summary>
        /// <remarks>[ None(なし), Unknown(不明), Checking(調査中), NonEffective(若干の海面変動が予想されるが、被害の心配なし), Watch(津波注意報), Warning(津波予報(種類不明)) ]</remarks>
        public enum P2PQ_domesticTsunami
        {
            /// <summary>
            /// (未実装)
            /// </summary>
            NotImplemented = -9,
            /// <summary>
            /// None(なし), この地震による津波の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 215</remarks>
            None = 0,
            /// <summary>
            /// Unknown(不明), (不明)
            /// </summary>
            Unknown = 1,
            /// <summary>
            /// Checking(調査中), 今後の情報に注意してください。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 217</remarks>
            Checking = 2,
            /// <summary>
            /// NonEffective(若干の海面変動が予想されるが、被害の心配なし), この地震により、日本の沿岸では若干の海面変動があるかもしれませんが、被害の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 212</remarks>
            NonEffective = 3,
            /// <summary>
            /// Watch(津波注意報), (津波注意報)
            /// </summary>
            Watch = 4,
            /// <summary>
            /// Warning(津波予報(種類不明)), 津波警報等（大津波警報・津波警報あるいは津波注意報）を発表中です。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 211</remarks>
            Warning = 5
        }

        /// <summary>
        /// 国内への津波の有無 - String2Enum
        /// </summary>
        public static P2PQ_domesticTsunami P2PQ_domesticTsunami_String2Enum(string str)
        {
            return str switch
            {
                "None" => P2PQ_domesticTsunami.None,
                "Unknown" => P2PQ_domesticTsunami.Unknown,
                "Checking" => P2PQ_domesticTsunami.Checking,
                "NonEffective" => P2PQ_domesticTsunami.NonEffective,
                "Watch" => P2PQ_domesticTsunami.Watch,
                "Warning" => P2PQ_domesticTsunami.Warning,
                _ => P2PQ_domesticTsunami.NotImplemented
            };
        }

        /// <summary>
        /// 国内への津波の有無 - Enum2String
        /// </summary>
        public static readonly Dictionary<P2PQ_domesticTsunami, string> P2PQ_domesticTsunami_Dict = new()
        {
            { P2PQ_domesticTsunami.NonEffective, "(未実装データです)" },
            { P2PQ_domesticTsunami.None, "この地震による津波の心配はありません。" },
            { P2PQ_domesticTsunami.Unknown, "(不明)" },
            { P2PQ_domesticTsunami.Checking, "今後の情報に注意してください。" },
            { P2PQ_domesticTsunami.NonEffective, "この地震により、日本の沿岸では若干の海面変動があるかもしれませんが、被害の心配はありません。" },
            { P2PQ_domesticTsunami.Watch, "(津波注意報)" },
            { P2PQ_domesticTsunami.Warning, "津波警報等（大津波警報・津波警報あるいは津波注意報）を発表中です。" }
        };

        /// <summary>
        /// P2P地震情報 JSON API v2 - 海外での津波の有無
        /// </summary>
        /// <remarks>[ None(なし), Unknown(不明), Checking(調査中), NonEffectiveNearby(震源の近傍で小さな津波の可能性があるが、被害の心配なし), WarningNearby(震源の近傍で津波の可能性がある), WarningPacific(太平洋で津波の可能性がある), WarningPacificWide(太平洋の広域で津波の可能性がある), WarningIndian(インド洋で津波の可能性がある), WarningIndianWide(インド洋の広域で津波の可能性がある), Potential(一般にこの規模では津波の可能性がある) ]</remarks>
        public enum P2PQ_foreignTsunami
        {
            /// <summary>
            /// (未実装)
            /// </summary>
            NotImplemented = -9,
            /// <summary>
            /// None(なし), この地震による津波の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 215</remarks>
            None = 0,
            /// <summary>
            /// Unknown(不明), (不明)
            /// </summary>
            Unknown = 1,
            /// <summary>
            /// Checking(調査中), 今後の情報に注意してください。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 217</remarks>
            Checking = 2,
            /// <summary>
            /// NonEffectiveNearby(震源の近傍で小さな津波の可能性があるが、被害の心配なし), 震源の近傍で小さな津波発生の可能性がありますが、被害をもたらす津波の心配はありません。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 227</remarks>
            NonEffectiveNearby = 3,
            /// <summary>
            /// WarningNearby(震源の近傍で津波の可能性がある), 震源の近傍で津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 226</remarks>
            WarningNearby = 4,
            /// <summary>
            /// WarningPacific(太平洋で津波の可能性がある), 太平洋で津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 222</remarks>
            WarningPacific = 5,
            /// <summary>
            /// WarningPacificWide(太平洋の広域で津波の可能性がある), 太平洋の広域に津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 221</remarks>
            WarningPacificWide = 6,
            /// <summary>
            /// WarningIndian(インド洋で津波の可能性がある), インド洋で津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 225</remarks>
            WarningIndian = 7,
            /// <summary>
            /// WarningIndianWide(インド洋の広域で津波の可能性がある), インド洋の広域に津波発生の可能性があります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 224</remarks>
            WarningIndianWide = 8,
            /// <summary>
            /// Potential(一般にこの規模では津波の可能性がある), 一般的に、この規模の地震が海域の浅い領域で発生すると、津波が発生することがあります。
            /// </summary>
            /// <remarks>AdditionalCommentEarthquake code: 228</remarks>
            Potential = 9
        }

        /// <summary>
        /// 海外の津波の有無 - String2Enum
        /// </summary>
        /// <param name="str">変換する文字列</param>
        /// <returns>対応する<see cref="P2PQ_foreignTsunami"/></returns>
        public static P2PQ_foreignTsunami P2PQ_foreignTsunami_String2Enum(string str)
        {
            return str switch
            {
                "None" => P2PQ_foreignTsunami.None,
                "Unknown" => P2PQ_foreignTsunami.Unknown,
                "Checking" => P2PQ_foreignTsunami.Checking,
                "NonEffectiveNearby" => P2PQ_foreignTsunami.NonEffectiveNearby,
                "WarningNearby" => P2PQ_foreignTsunami.WarningNearby,
                "WarningPacific" => P2PQ_foreignTsunami.WarningPacific,
                "WarningPacificWide" => P2PQ_foreignTsunami.WarningPacificWide,
                "WarningIndian" => P2PQ_foreignTsunami.WarningIndian,
                "WarningIndianWide" => P2PQ_foreignTsunami.WarningIndianWide,
                "Potential" => P2PQ_foreignTsunami.Potential,
                _ => P2PQ_foreignTsunami.NotImplemented
            };
        }

        /// <summary>
        /// 海外の津波の有無 - Enum2String
        /// </summary>
        public static readonly Dictionary<P2PQ_foreignTsunami, string> P2PQ_foreignTsunami_Dict = new()
        {
            { P2PQ_foreignTsunami.None, "この地震による津波の心配はありません。" },
            { P2PQ_foreignTsunami.Unknown, "(不明)" },
            { P2PQ_foreignTsunami.Checking, "今後の情報に注意してください。" },
            { P2PQ_foreignTsunami.NonEffectiveNearby, "震源の近傍で小さな津波発生の可能性がありますが、被害をもたらす津波の心配はありません。" },
            { P2PQ_foreignTsunami.WarningNearby, "震源の近傍で津波発生の可能性があります。" },
            { P2PQ_foreignTsunami.WarningPacific, "太平洋で津波発生の可能性があります。" },
            { P2PQ_foreignTsunami.WarningPacificWide, "太平洋の広域に津波発生の可能性があります。" },
            { P2PQ_foreignTsunami.WarningIndian, "インド洋で津波発生の可能性があります。" },
            { P2PQ_foreignTsunami.WarningIndianWide, "インド洋の広域に津波発生の可能性があります。" },
            { P2PQ_foreignTsunami.Potential, "一般的に、この規模の地震が海域の浅い領域で発生すると、津波が発生することがあります。" }
        };

    }
}
