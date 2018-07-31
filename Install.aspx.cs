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
    public partial class Install : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var AppName = Request.Params["AppName"];
            var DeviceType = Request.Params["DeviceType"];

            try
            {

                if (DeviceType == null  & AppName == null)
                {
                    Json = "参数<br/>AppName  应用名称<br/>DeviceType  设备类型<br/><br/>测试参数<br/>?AppName=YGHY&DeviceType=Android";
                    return;
                }
                var sql = string.Format("select * from tb_app_update where devicetype='{0}' and  appName='{1}'", DeviceType, AppName);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    return;
                }
                else
                {
                    Json = string.Format("<body onload=\"javascript:self.location = '{0}'\">", dt.Rows[0]["url"] as string);   
                }
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(ex.Message);
            }
        }
        protected string Json;
        
    }
}