using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace QuakeMapFast
{
    public static class Converter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="Token">addr/pref</param>
        /// <returns></returns>
        public static Dictionary<string, int> Points2Dic(JObject json, string Token)
        {
            Dictionary<string, int> PointInt = new Dictionary<string, int>();
            foreach (JToken json_ in json.SelectToken("points"))
                PointInt.Add((string)json_.SelectToken(Token), P2PScale2IntN((int)json_.SelectToken("scale")));
            return PointInt;
        }
        public static IntList Point2IntList(JObject json, string Token)
        {
            IntList intList = new IntList();
            foreach (JToken json_ in json.SelectToken("points"))
            {
                switch ((int)json_.SelectToken("scale"))
                {
                    case 10:
                        intList.S1.Add((string)json_.SelectToken(Token));
                        break;
                    case 20:
                        intList.S2.Add((string)json_.SelectToken(Token));
                        break;
                    case 30:
                        intList.S3.Add((string)json_.SelectToken(Token));
                        break;
                    case 40:
                        intList.S4.Add((string)json_.SelectToken(Token));
                        break;
                    case 45:
                        intList.S5.Add((string)json_.SelectToken(Token));
                        break;
                    case 50:
                        intList.S6.Add((string)json_.SelectToken(Token));
                        break;
                    case 55:
                        intList.S7.Add((string)json_.SelectToken(Token));
                        break;
                    case 60:
                        intList.S8.Add((string)json_.SelectToken(Token));
                        break;
                    case 70:
                        intList.S9.Add((string)json_.SelectToken(Token));
                        break;
                }
            }
            return intList;
        }
        public static string IntList2String(IntList intList)
        {
            string output = "";
            if(intList.S9.Count!=0)
            {
                output += "《震度7》";
                foreach(string Area in intList.S9)
                    output += Area+" ";
            }
            if (intList.S8.Count != 0)
            {
                output += "《震度6強》";
                foreach (string Area in intList.S8)
                    output += Area + " ";
            }
            if (intList.S7.Count != 0)
            {
                output += "《震度6弱》";
                foreach (string Area in intList.S7)
                    output += Area + " ";
            }
            if (intList.S6.Count != 0)
            {
                output += "《震度5強》";
                foreach (string Area in intList.S6)
                    output += Area + " ";
            }
            if (intList.S5.Count != 0)
            {
                output += "《震度5弱》";
                foreach (string Area in intList.S5)
                    output += Area + " ";
            }
            if (intList.S4.Count != 0)
            {
                output += "《震度4》";
                foreach (string Area in intList.S4)
                    output += Area + " ";
            }
            if (intList.S3.Count != 0)
            {
                output += "《震度3》";
                foreach (string Area in intList.S3)
                    output += Area + " ";
            }
            if (intList.S2.Count != 0)
            {
                output += "《震度2》";
                foreach (string Area in intList.S2)
                    output += Area + " ";
            }
            if (intList.S1.Count != 0)
            {
                output += "《震度1》";
                foreach (string Area in intList.S1)
                    output += Area + " ";
            }
            return output;
        }
        public static string Point2String(JObject json, string Token)
        {
            return IntList2String(Point2IntList(json, Token));
        }
        public static void PointCorrect(ref double LatSta, ref double LatEnd, ref double LonSta, ref double LonEnd)
        {
            LatSta -= (LatEnd - LatSta) / 20;//差の1/20余白追加
            LatEnd += (LatEnd - LatSta) / 20;
            LonSta -= (LonEnd - LonSta) / 20;
            LonEnd += (LonEnd - LonSta) / 20;
            if (LatEnd - LatSta < 3)//緯度差を最大3に
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
            else if (LonEnd - LonSta < LatEnd - LatSta)
            {
                double correction = ((LatEnd - LatSta) - (LonEnd - LonSta)) / 2d;
                LonSta -= correction;
                LonEnd += correction;
            }
        }
        public static SolidBrush IntS2Brush(string Int)
        {
            switch (Int)
            {
                case "-":
                    return new SolidBrush(Color.FromArgb(80, 90, 100));
                case "1":
                    return new SolidBrush(Color.FromArgb(60, 80, 100));
                case "2":
                    return new SolidBrush(Color.FromArgb(45, 90, 180));
                case "3":
                    return new SolidBrush(Color.FromArgb(50, 175, 175));
                case "4":
                    return new SolidBrush(Color.FromArgb(240, 240, 60));
                case "5弱":
                    return new SolidBrush(Color.FromArgb(250, 150, 0));
                case "5強":
                    return new SolidBrush(Color.FromArgb(250, 75, 0));
                case "6弱":
                    return new SolidBrush(Color.FromArgb(200, 0, 0));
                case "6強":
                    return new SolidBrush(Color.FromArgb(100, 0, 0));
                case "7":
                    return new SolidBrush(Color.FromArgb(100, 0, 100));
                default:
                    return new SolidBrush(Color.FromArgb(30, 60, 90));
            }
        }
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
        public static Brush IntN2TextBrush(int Int)
        {
            if (Int >= 3 && Int <= 6)
                return Brushes.Black;
            else
                return Brushes.White;
        }
        public static string Int2TelopColor(int Int)
        {
            switch (Int)
            {
                case 0:
                    return "80,90,100,White,60,70,80,White";
                case 1:
                    return "60,80,100,White,40,60,80,White";
                case 2:
                    return "45,90,180,White,30,60,150,White";
                case 3:
                    return "50,175,175,Black,25,150,150,Black";
                case 4:
                    return "240,240,60,Black,200,200,40,Black";
                case 5:
                    return "250,150,0,Black,200,130,0,Black";
                case 6:
                    return "250,75,0,Black,200,50,0,Black";
                case 7:
                    return "200,0,0,White,180,0,0,White";
                case 8:
                    return "100,0,0,White,80,0,0,White";
                case 9:
                    return "100,0,100,White,80,0,80,White";
                default:
                    return "30,60,90,White,15,30,45,White";
            }
        }

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

    }
}
