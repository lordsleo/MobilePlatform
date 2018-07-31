using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Net.Sockets;
using Leo;
using Leo.Json;
using System.Text;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates; 


namespace MobilePlatform
{
    public partial class PushMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //参数
            var AppName = "MYLYGPORT";
            var DeviceToken = "d7a3d1492159e256e117c061a47398f966cc242ddd51fadd31d074cecae78de9";
            var Message ="12gfsdfs3";
            //证书检索
            string certificatepath;
            switch (AppName)
            {
                case "MYLYGPORT": certificatepath = "APNsCert/MyLygport.p12"; break;
                default: return;
            }
            //创建证书集
            var certificates = new X509CertificateCollection();
            certificates.Add(new X509Certificate2(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificatepath))));
            //创建TCP连接
            TcpClient apnsClient = new TcpClient();
            apnsClient.Connect("gateway.push.apple.com", 2195);
            //使用创建SSL加密连接
            SslStream apnsStream = new SslStream(apnsClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate),null);
            //SslProtocols支持类型在将来可能会改变
            try
            {
                apnsStream.AuthenticateAsClient("gateway.push.apple.com", certificates, SslProtocols.Default, true);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", ex.InnerException.Message);
                }
                apnsClient.Close();
                return;
            }

            byte[] Notification = ToBytes(DeviceToken, Message);
            //写入流
            apnsStream.Write(Notification);
            apnsStream.Flush();
            //读取从服务器的消息。
            string serverMessage = ReadMessage(apnsStream);
            Console.WriteLine("Server says: {0}", serverMessage);
            apnsClient.Close();

        }
        //证书校验。
        public static bool ValidateServerCertificate(object sender,X509Certificate certificate,X509Chain chain,SslPolicyErrors sslPolicyErrors)
            {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
            }
        //读取流
        static string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("") != -1)
                {
                    break;
                }
            }
            while (bytes != 0);
            return messageData.ToString();
        }
        //二进制数据
        public static byte[] ToBytes(string DeviceToken,string Message)
        {
            //消息标识
            int identifier = 0;
            byte[] identifierBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(identifier));
            
            //过期时间戳
            byte[] expiry = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32)86400));

            //推送设备token
            byte[] deviceToken = new byte[DeviceToken.Length / 2];
            for (int i = 0; i < deviceToken.Length; i++)
            {
                deviceToken[i] = byte.Parse(DeviceToken.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            //推送token长度
            byte[] deviceTokenSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(deviceToken.Length)));

            //推送内容
            Dictionary<string,Dictionary<string,string>> payloadstr = new Dictionary<string,Dictionary<string,string>>();
            Dictionary<string,string> aps = new Dictionary<string,string>();
            aps.Add("alert",Message);
            aps.Add("sound", "default");
            payloadstr.Add("aps", aps);

            string str = JsonConvert.SerializeObject(payloadstr);//"{\"aps\":{\"alert\":\"这是测试消息！！\",\"badge\":1,\"sound\":\"anke.mp3\"}}";
            byte[] payload = Encoding.UTF8.GetBytes(str);

            //内容长度
            byte[] payloadSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(payload.Length)));
            List<byte[]> notificationParts = new List<byte[]>();

            //1 Command
            //报文起始
            notificationParts.Add(new byte[] { 0x01 }); // Enhanced notification format command
            notificationParts.Add(identifierBytes);
            notificationParts.Add(expiry);
            notificationParts.Add(deviceTokenSize);
            notificationParts.Add(deviceToken);
            notificationParts.Add(payloadSize);
            notificationParts.Add(payload);

            return BuildBufferFrom(notificationParts);
        }
        //拼接二进制数组
        private static byte[] BuildBufferFrom(IList<byte[]> bufferParts)
        {
            int bufferSize = 0;
            for (int i = 0; i < bufferParts.Count; i++)
            {
                bufferSize += bufferParts[i].Length;
            }
            byte[] buffer = new byte[bufferSize];
            int position = 0;
            for (int i = 0; i < bufferParts.Count; i++)
            {
                byte[] part = bufferParts[i];
                System.Buffer.BlockCopy(bufferParts[i], 0, buffer, position, part.Length);
                position += part.Length;
            }
            return buffer;
        }

        protected string Json;
    }
}