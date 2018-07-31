using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Leo;
using Leo.Json;

namespace MobilePlatform
{
    public partial class Update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var DeviceType = Request.Params["DeviceType"];
            var Build = Request.Params["Build"];
            var AppName = Request.Params["AppName"];
            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {

                if (DeviceType == null & Build == null & AppName == null)
                {
                    Json = "参数<br/>AppName  应用名称<br/>DeviceType  设备类型<br/>Build  内部版本号<br/><br/>测试参数<br/>?AppName=YGHY&DeviceType=Android&Build=0";
                    return;
                }
                if (DeviceType == "iOS" & Build == "0" & AppName == "YGHY")
                {
                    info.Add("Update", "Yes");
                    info.Add("Version", "1.0.1");
                    info.Add("Url", "http://www.baidu.com");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                var sql = string.Format("select * from tb_app_update where devicetype='{0}' and  appName='{1}'", DeviceType, AppName);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("Update", "No");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                if ((Convert.ToInt32(dt.Rows[0]["Build"]))!= (Convert.ToInt32(Build)))
                {
                    info.Add("Update", "Yes");
                    info.Add("Version", dt.Rows[0]["version"] as string); 
                    info.Add("Url", dt.Rows[0]["url"] as string);   
                }
                else
                {
                    info.Add("Update", "No");
                }
                 
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("Update", "No");
                info.Add("Error",ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
        
    }
}