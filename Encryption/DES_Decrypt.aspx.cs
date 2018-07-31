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
    public partial class DES_Decrypt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string password2 = DES_IV.IV;
            string password = Request["Key"];
            string ciphertext = HttpUtility.UrlDecode(Request["Value"]);
            //ciphertext = "C1hstQ2Erq3P2w4hJzr71%2fVqu%2biOq5WF";
            string cipher = string.Empty;
            //password = "gljsy";

            try
            {
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

                if (ciphertext == null)
                {
                    Json = "null";
                    return;
                }
                ciphertext = ciphertext.Replace(" ", "+");
                SymmetricAlgorithm serviceProvider = new DESCryptoServiceProvider();
                serviceProvider.Key = Encoding.ASCII.GetBytes(key);
                serviceProvider.IV = Encoding.ASCII.GetBytes(iv);

                byte[] contentArray = Convert.FromBase64String(ciphertext);
                MemoryStream memoryStream = new MemoryStream(contentArray);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, serviceProvider.CreateDecryptor(), CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);

                Json = streamReader.ReadToEnd();

                streamReader.Dispose();
                cryptoStream.Dispose();
                memoryStream.Dispose();
                serviceProvider.Clear();

            }
            catch (Exception ex)
            {
                Json = ex.Message;
            }
        }
        protected string Json;
    }
}