using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace MobilePlatform.Encryption
{
    public partial class DES_Encrypt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string password2 = DES_IV.IV;
                string password = Request["Key"];
                string cleartext = Request["Value"];
                //password = "gljsy";
                //cleartext = "AAA8N3ABfAAAxqAAAD";


                char[] key = new char[8];
                if (password.Length > 8)
                {
                    password = password.Remove(8);
                }
                password.CopyTo(0, key, 0, password.Length);

                char[] iv = new char[8];
                if (password2.Length > 8)
                {
                    password2 = password2.Remove(8);
                }
                password2.CopyTo(0, iv, 0, password2.Length);

                if (cleartext == null)
                {
                    Json = string.Empty;
                    return;
                }

                SymmetricAlgorithm serviceProvider = new DESCryptoServiceProvider();
                serviceProvider.Key = Encoding.ASCII.GetBytes(key);
                serviceProvider.IV = Encoding.ASCII.GetBytes(iv);

                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, serviceProvider.CreateEncryptor(), CryptoStreamMode.Write);
                StreamWriter streamWriter = new StreamWriter(cryptoStream);

                streamWriter.Write(cleartext);
                streamWriter.Dispose();
                cryptoStream.Dispose();

                byte[] signData = memoryStream.ToArray();
                memoryStream.Dispose();
                serviceProvider.Clear();
                Json = Convert.ToBase64String(signData);
                //Json = HttpUtility.UrlEncode(Json);
            }
            catch (Exception ex)
            {
                Json = ex.Message;
            }
        }
        protected string Json;
    }
}