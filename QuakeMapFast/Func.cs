using QuakeMapFast.Properties;
using System;
using System.IO;
using System.Media;
using System.Net.Sockets;
using System.Text;

namespace QuakeMapFast
{
    public class Func
    {
        /// <summary>
        /// コンソールのデフォルトの色
        /// </summary>
        public static readonly ConsoleColor defaultColor = Console.ForegroundColor;

        /// <summary>
        /// コンソールにデフォルトの色で出力します。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="withLine">改行するか</param>
        public static void ConWrite(string text, bool withLine = true)
        {
            ConWrite(text, defaultColor, withLine);
        }

        /// <summary>
        /// 例外のテキストを赤色で出力します。
        /// </summary>
        /// <param name="loc">場所([ConWrite]など)</param>
        /// <param name="ex">出力する例外</param>
        public static void ConWrite(string loc, Exception ex)
        {
            ConWrite(loc + ex.ToString(), ConsoleColor.Red);
        }

        /// <summary>
        /// コンソールに色付きで出力します。色は変わったままとなります。
        /// </summary>
        /// <param name="text">出力するテキスト</param>
        /// <param name="color">表示する色</param>
        /// <param name="withLine">改行するか</param>
        public static void ConWrite(string text, ConsoleColor color, bool withLine = true)
        {
            Console.ForegroundColor = color;
            Console.Write(DateTime.Now.ToString("HH:mm:ss.ffff "));
            if (withLine)
                Console.WriteLine(text);
            else
                Console.Write(text);
        }

        /// <summary>
        /// 棒読みちゃんに読み上げ指令を送ります。
        /// </summary>
        /// <remarks>事前に有効確認が必要です。</remarks>
        /// <param name="text">読み上げさせる文</param>
        public static void Bouyomichan(string text)
        {
            if (!Settings.Default.Bouyomi_Enable)
                return;
            try
            {
                ConWrite("[Bouyomichan]棒読みちゃん処理開始");
                byte[] message = Encoding.UTF8.GetBytes(text);
                ConWrite($"[Bouyomichan]棒読みちゃん送信中...");
                using (TcpClient tcpClient = new TcpClient("127.0.0.1", 50001))
                using (NetworkStream networkStream = tcpClient.GetStream())
                using (BinaryWriter binaryWriter = new BinaryWriter(networkStream))
                {
                    binaryWriter.Write((short)1);
                    binaryWriter.Write(Settings.Default.Bouyomi_Speed);
                    binaryWriter.Write(Settings.Default.Bouyomi_Tone);
                    binaryWriter.Write(Settings.Default.Bouyomi_Volume);
                    binaryWriter.Write(Settings.Default.Bouyomi_Voice);
                    binaryWriter.Write((byte)0);
                    binaryWriter.Write(message.Length);
                    binaryWriter.Write(message);
                }
                ConWrite($"[Bouyomichan]棒読みちゃん送信完了");
            }
            catch (Exception ex)
            {
                ConWrite($"[Bouyomichan]", ex);
            }
        }

        /// <summary>
        /// TelopにSocket送信します
        /// </summary>
        /// <param name="text">Telopに送信するテキスト(Telop方式)</param>
        public static void Telop(string text)
        {
            if (!Settings.Default.Telop_Enable)
                return;
            ConWrite("[Telop]テロップ送信開始");
            ConWrite("[Telop]Text:" + text);
            try
            {
                byte[] message = new byte[4096];
                message = Encoding.UTF8.GetBytes(text);
                using (TcpClient tcpClient = new TcpClient("127.0.0.1", 31401))
                using (NetworkStream networkStream = tcpClient.GetStream())
                    networkStream.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                ConWrite("[Telop]", ex);
            }
            ConWrite("[Telop]テロップ送信終了");
        }

        /// <summary>
        /// XPosterV2Hostに送信します。
        /// </summary>
        /// <remarks><c>if (!debug && !readJSON) </c>をしておくこと</remarks>
        /// <param name="text"></param>
        /// <param name="path"></param>
        public static void XPost(string text, string path)
        {
            if (File.Exists("XPosterV2Host - Enable"))//念のため
                try
                {
                    ConWrite("[XPost]X送信開始");
                    string sendText = $"{{ \"text\" : \"{text.Replace("\n", "\\\\n")}\", \"images\" : \"{Path.GetFullPath(path).Replace("\\", "\\\\")}\" }}";
                    ConWrite("[XPost]Text:" + sendText);
                    byte[] message = new byte[16 * 1024];
                    message = Encoding.UTF8.GetBytes(sendText);
                    using (TcpClient tcpClient = new TcpClient("127.0.0.1", 31403))
                    using (NetworkStream networkStream = tcpClient.GetStream())
                        networkStream.Write(message, 0, message.Length);
                }
                catch (Exception ex)
                {
                    ConWrite("[XPost]", ex);
                }
                finally
                {
                    ConWrite("[XPost]X送信終了");
                }
        }

        //共通プレイヤー
        public static SoundPlayer player;

        /// <summary>
        /// 音声を再生します。
        /// </summary>
        /// <remarks>音声ファイルがなければ無効です。</remarks>
        /// <param name="fileName">再生するファイル名(sound\\)</param>
        public static void PlaySound(string fileName)
        {
            if (!fileName.StartsWith("Sound\\"))
                fileName = "Sound\\" + fileName;
            if (!File.Exists(fileName))
            {
                ConWrite("[PlaySound]音声ファイルがないため再生しません。");
                return;
            }
            ConWrite($"[PlaySound]音声再生開始(\"{fileName}\")");
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
            player = new SoundPlayer(fileName);
            player.Play();
        }
    }
}
