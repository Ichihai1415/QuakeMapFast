using System.ComponentModel;
using System.Text.Json.Nodes;

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


    }
}
