using System.Text.Json.Nodes;
using static QuakeMapFast.Utils.JSONClasses;

namespace QuakeMapFast.Utils
{
    public static partial class Utils
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
        /// 震度別の観測点・区域名のリスト
        /// </summary>
        public class IntList
        {
            /// <summary>
            /// 震度1
            /// </summary>
            public List<string> S1 { get; set; } = [];
            public List<string> S2 { get; set; } = [];
            public List<string> S3 { get; set; } = [];
            public List<string> S4 { get; set; } = [];
            public List<string> S5 { get; set; } = [];
            public List<string> S6 { get; set; } = [];
            public List<string> S7 { get; set; } = [];
            public List<string> S8 { get; set; } = [];
            public List<string> S9 { get; set; } = [];
            public List<string> S10 { get; set; } = [];
        }

        /// <summary>
        /// P2Pjsonの震度をint形式(0,1,..,9,10(未入電))に変換します。
        /// </summary>
        /// <param name="scale">P2Pjsonの震度</param>
        /// <returns>int形式の震度</returns>
        public static int P2PScale2IntN(int? scale) => scale switch
        {
            10 => 1,
            20 => 2,
            30 => 3,
            40 => 4,
            45 => 5,
            50 => 6,
            55 => 7,
            60 => 8,
            70 => 9,
            46 => 10,
            _ => 0,
        };

        /// <summary>
        ///  P2P地震情報 JSON API v2 の震度をstring形式(-,1,..,6強,7)に変換します。
        /// </summary>
        /// <param name="scale">P2P地震情報 JSON API v2 の震度</param>
        /// <returns>string形式(-,1,..,6強,7)の震度</returns>
        public static string P2PScale2IntS(int scale) => scale switch
        {
            10 => "1",
            20 => "2",
            30 => "3",
            40 => "4",
            45 => "5弱",
            46 => "5弱以上推定未入電",
            50 => "5強",
            55 => "6弱",
            60 => "6強",
            70 => "7",
            _ => "-"//-1(なし),99(上限なし(EEW))
        };

        /// <summary>
        /// P2P地震情報 JSON API v2 の震度が6弱以上か判定します。
        /// </summary>
        /// <param name="scale">震度</param>
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
                scaleFrom <= 70 && scaleFrom >= 55;//x程度以上 55<=scaleTo<=70 
        }

        /// <summary>
        /// jsonから指定した区分の地区・地点ごとの震度のDictionaryを返します。
        /// </summary>
        /// <param name="points">jsonのpoints</param>
        /// <param name="isPref">addr(地点・区分)/pref(県)</param>
        /// <returns>Dictionary<地区, int形式の震度></returns>
        public static Dictionary<string, int> P2PQPoints2Dic(JSONClasses.P2PQuake_JMAQuake.C_Point[] points, bool isPref)
            => points.ToDictionary(pt => isPref ? pt.Pref : pt.Addr, pt => P2PScale2IntN(pt.Scale));

        /// <summary>
        /// jsonから震度別の指定した区分の地区を返します。
        /// </summary>
        /// <param name="points">震度観測点の情報</param>
        /// <param name="token">addr(地点・区分)/pref(県)</param>
        /// <returns>IntList</returns>
        public static IntList Point2IntList(P2PQuake_JMAQuake.C_Point[] points, PointToken token)
        {
            var intList = new IntList();
            foreach (var point in points)
                (point.Scale switch
                {
                    10 => intList.S1,
                    20 => intList.S2,
                    30 => intList.S3,
                    40 => intList.S4,
                    45 => intList.S5,
                    50 => intList.S6,
                    55 => intList.S7,
                    60 => intList.S8,
                    70 => intList.S9,
                    46 => intList.S10,
                    _ => throw new Exception("未知の震度です。")
                }).Add(token == PointToken.Pref ? point.Pref : point.Addr);
            return intList;
        }

        /// <summary>
        /// IntListから震度別の文字列を返します。
        /// </summary>
        /// <param name="intList">IntList</param>
        /// <param name="minimumInt">文字列にする最小の震度</param>
        /// <returns>震度別の文字列</returns>
        /// <remarks>例:《震度4》○○ ○○ \n《震度3》○○ ○○ </remarks>
        public static string IntList2String(IntList intList, int minimumInt = 0)
        {
            string output = "\n";
            if (intList.S9.Count != 0 && minimumInt <= 9)
                output += "\n《震度7》" + string.Join(" ", intList.S9);
            if (intList.S8.Count != 0 && minimumInt <= 8)
                output += "\n《震度6強》" + string.Join(" ", intList.S8);
            if (intList.S7.Count != 0 && minimumInt <= 7)
                output += "\n《震度6弱》" + string.Join(" ", intList.S7);
            if (intList.S6.Count != 0 && minimumInt <= 6)
                output += "\n《震度5強》" + string.Join(" ", intList.S6);
            if (intList.S5.Count != 0 && minimumInt <= 5)
                output += "\n《震度5弱》" + string.Join(" ", intList.S5);
            if (intList.S4.Count != 0 && minimumInt <= 4)
                output += "\n《震度4》" + string.Join(" ", intList.S4);
            if (intList.S3.Count != 0 && minimumInt <= 3)
                output += "\n《震度3》" + string.Join(" ", intList.S3);
            if (intList.S2.Count != 0 && minimumInt <= 2)
                output += "\n《震度2》" + string.Join(" ", intList.S2);
            if (intList.S1.Count != 0 && minimumInt <= 1)
                output += "\n《震度1》" + string.Join(" ", intList.S1);
            return output.Replace("\n\n", "");
        }

        /// <summary>
        /// jsonから震度別の文字列を返します。
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="Token">addr(地点・区分)/pref(県)</param>
        /// <param name="minimumInt">文字列にする最小のint形式の震度</param>
        /// <returns></returns>
        public static string Point2String(JsonNode json, PointToken token, int minimumInt = 0)
        {
            return IntList2String(Point2IntList(json, token),minimumInt);
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
                case 10:
                    return new SolidBrush(Color.FromArgb(250, 150, 0));
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
            return Int switch
            {
                0 => "60,70,80,White,80,90,100,White",
                1 => "40,60,80,White,60,80,100,White",
                2 => "30,60,150,White,45,90,180,White",
                3 => "25,150,150,Black,50,175,175,Black",
                4 => "200,200,40,Black,240,240,60,Black",
                5 => "200,130,0,Black,250,150,0,Black",
                6 => "200,50,0,Black,250,75,0,Black",
                7 => "180,0,0,White,200,0,0,White",
                8 => "80,0,0,White,100,0,0,White",
                9 => "80,0,80,White,100,0,100,White",
                _ => "15,30,45,White,30,60,90,White",
            };
        }

        public static int P2PQScale2Int(int? scale)
        {
            return scale switch
            {
                null => -1,
                -1 => -1,
                10 => 1,
                20 => 2,
                30 => 3,
                40 => 4,
                45 => 5,
                46 => 10,
                50 => 6,
                55 => 7,
                60 => 8,
                70 => 9,
                _ => -8,
            };
        }


        public static int P2PQScaleEnum2Int(P2PQ_Scales scale)
        {
            return scale switch
            {
                P2PQ_Scales.None => -1,
                P2PQ_Scales.NotImplemented => -8,
                P2PQ_Scales.S0 => 0,
                P2PQ_Scales.S1 => 1,
                P2PQ_Scales.S2 => 2,
                P2PQ_Scales.S3 => 3,
                P2PQ_Scales.S4 => 4,
                P2PQ_Scales.S5m => 5,
                P2PQ_Scales.S5p => 6,
                P2PQ_Scales.S6m => 7,
                P2PQ_Scales.S6p => 8,
                P2PQ_Scales.S7 => 9,
                P2PQ_Scales.SUnknown => 10,
                _ => -8
            };
        }


        public static P2PQ_Scales P2PQScaleInt2Enum(int scale)
        {
            return scale switch
            {
                -1 => P2PQ_Scales.None,
                -8 => P2PQ_Scales.NotImplemented,
                0 => P2PQ_Scales.S0,
                1 => P2PQ_Scales.S1,
                2 => P2PQ_Scales.S2,
                3 => P2PQ_Scales.S3,
                4 => P2PQ_Scales.S4,
                5 => P2PQ_Scales.S5m,
                6 => P2PQ_Scales.S5p,
                7 => P2PQ_Scales.S6m,
                8 => P2PQ_Scales.S6p,
                9 => P2PQ_Scales.S7,
                10 => P2PQ_Scales.SUnknown,
                _ => P2PQ_Scales.NotImplemented
            };
        }

        public static int JMAintSt2int(string intSt)//todo:旧震度56
        {
            return intSt switch
            {
                "震度１" => 1,
                "震度２" => 2,
                "震度３" => 3,
                "震度４" => 4,
                "震度５" => -5,
                "震度５弱" => 5,
                "震度５強" => 6,
                "震度６" => -7,
                "震度６弱" => 7,
                "震度６強" => 8,
                "震度７" => 9,
                _ => -8
            };
        }

        public static P2PQ_Scales JMAintSt2P2PQEnum(string intSt)//todo:旧震度56
        {
            return intSt switch
            {
                "震度１" => P2PQ_Scales.S1,
                "震度２" => P2PQ_Scales.S2,
                "震度３" => P2PQ_Scales.S3,
                "震度４" => P2PQ_Scales.S4,
                "震度５" => P2PQ_Scales.S5m,
                "震度５弱" => P2PQ_Scales.S5m,
                "震度５強" => P2PQ_Scales.S5p,
                "震度６" => P2PQ_Scales.S6m,
                "震度６弱" => P2PQ_Scales.S6m,
                "震度６強" => P2PQ_Scales.S6p,
                "震度７" => P2PQ_Scales.S7,
                _ => P2PQ_Scales.NotImplemented
            };
        }

        public static string GetTsunamiMessege(string domesticTsunami) => domesticTsunami switch
        {
            //[ None(なし), Unknown(不明), Checking(調査中), NonEffective(若干の海面変動が予想されるが、被害の心配なし), Watch(津波注意報), Warning(津波予報(種類不明)) ]
            "None" => "この地震による津波の心配はありません。",
            "Unknown" => "日本への津波の有無については不明です。",
            "Checking" => "今後の情報に注意してください。",
            "NonEffective" => "この地震により、日本の沿岸では若干の海面変動があるかもしれませんが、被害の心配はありません。",
            "Watch" => "津波警報等（大津波警報・津波警報あるいは津波注意報）を発表中です。",
            "Warning" => "津波警報等（大津波警報・津波警報あるいは津波注意報）を発表中です。",
            _ => ""
        };



    }
}
