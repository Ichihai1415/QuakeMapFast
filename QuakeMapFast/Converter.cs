using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static SolidBrush IntS2Brush(string Int)
        {
            switch(Int)
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
            switch(Int)
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
    }
}
