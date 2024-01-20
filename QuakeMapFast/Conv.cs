using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace QuakeMapFast
{
    public static class Conv
    {
        /// <summary>
        /// P2Pのcodeと説明
        /// </summary>
        public static Dictionary<int, string> P2PInfoCodeName = new Dictionary<int, string>
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
        public static Dictionary<string, string> P2PInfoTypeName = new Dictionary<string, string>
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
            public List<string> S1 { get; set; }
            public List<string> S2 { get; set; }
            public List<string> S3 { get; set; }
            public List<string> S4 { get; set; }
            public List<string> S5 { get; set; }
            public List<string> S6 { get; set; }
            public List<string> S7 { get; set; }
            public List<string> S8 { get; set; }
            public List<string> S9 { get; set; }
        }

        /// <summary>
        /// P2Pjsonの震度をint形式(0,1,..,8,9)に変換します。
        /// </summary>
        /// <param name="Scale">P2Pjsonの震度</param>
        /// <returns>int形式の震度</returns>
        public static int P2PScale2IntN(int Scale)
        {
            switch (Scale)
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
        /// <param name="Scale">P2Pjsonの震度</param>
        /// <returns>int形式の震度</returns>
        public static string P2PScale2IntS(int Scale)
        {
            switch (Scale)
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
                default:
                    return "-";
            }
        }

        /// <summary>
        /// jsonから指定した区分の地区ごとの震度のDictionaryを返します。
        /// </summary>
        /// <param name="points">jsonのpoints</param>
        /// <param name="token">addr(地点・区分)/pref(県)</param>
        /// <returns>Dictionary<地区, int形式の震度></returns>
        public static Dictionary<string, int> Points2Dic(JToken points, string token)
        {
            return points.ToDictionary(pt => (string)pt[token], pt => P2PScale2IntN((int)pt["scale"]));
        }

        /// <summary>
        /// jsonから震度別の指定した区分の地区を返します。
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="token">addr(地点・区分)/pref(県)</param>
        /// <returns>IntList</returns>
        public static IntList Point2IntList(JObject json, string token)
        {
            IntList intList = new IntList
            {
                S1 = new List<string>(),
                S2 = new List<string>(),
                S3 = new List<string>(),
                S4 = new List<string>(),
                S5 = new List<string>(),
                S6 = new List<string>(),
                S7 = new List<string>(),
                S8 = new List<string>(),
                S9 = new List<string>()
            };
            foreach (JToken json_ in json["points"])
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
        public static string Point2String(JObject json, string Token, int LowestInt = 0)
        {
            return IntList2String(Point2IntList(json, Token), LowestInt);
        }

        /// <summary>
        /// 画像描画用に緯度・経度を補正します
        /// </summary>
        /// <param name="LatSta">緯度の始点</param>
        /// <param name="LatEnd">緯度の終点</param>
        /// <param name="LonSta">経度の始点</param>
        /// <param name="LonEnd">経度の終点</param>
        public static void PointCorrect(ref double LatSta, ref double LatEnd, ref double LonSta, ref double LonEnd)
        {
            LatSta -= (LatEnd - LatSta) / 20;//差の1/20余白追加
            LatEnd += (LatEnd - LatSta) / 20;
            LonSta -= (LonEnd - LonSta) / 20;
            LonEnd += (LonEnd - LonSta) / 20;
            if (LatEnd - LatSta < 3)//緯度差を最小3に
            {
                double correction = (3 - (LatEnd - LatSta)) / 2d;
                LatSta -= correction;
                LatEnd += correction;
            }
            if (LonEnd - LonSta > LatEnd - LatSta)//大きいほうに合わせる
            {
                double correction = ((LonEnd - LonSta) - (LatEnd - LatSta)) / 2d;
                LatSta -= correction;
                LatEnd += correction;
            }
            else// if (LonEnd - LonSta < LatEnd - LatSta)
            {
                double correction = ((LatEnd - LatSta) - (LonEnd - LonSta)) / 2d;
                LonSta -= correction;
                LonEnd += correction;
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

    }
}
