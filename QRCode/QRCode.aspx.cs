using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.Text;
using System.IO;

namespace MobilePlatform.QRCode
{
    public partial class QRCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string info = Request["info"];
                //info = "http://www.baidu.com";

                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                Bitmap bt = qrCodeEncoder.Encode(info, Encoding.UTF8);

                MemoryStream ms = new MemoryStream();
                bt.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                Response.ContentType = "image/bmp";
                ms.WriteTo(Response.OutputStream);
                ms.Close();
            }
            catch (Exception ex)
            {
                string a = ex.Message;
            }
        }
    }
}