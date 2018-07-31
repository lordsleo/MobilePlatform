//
//文件名：    GetDevice.aspx.cs
//功能描述：  获取移动用户设备
//创建时间：  2015/07/08
//作者：      
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Leo;
using Leo.Json;
using YGSoft.IPort.Data;

namespace MobilePlatform.Device
{
    public partial class GetDevice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var codeUser = Request.Params["CodeUser"];
            var deviceType = Request.Params["DeviceType"];
            var appName = Request.Params["AppName"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || deviceType == null || appName == null)
                {
                    info.Add("参数CodeUser，DeviceType，AppName不能为null！", "举例：http://218.92.115.55/MobilePlatform/Device/GetDevice.aspx?CodeUser=safdfaDAS&DeviceType=iOS&AppName=SPHSJ");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string deviceToken = "";
                string sql =
                    string.Format("select * from tb_app_device where code_user='{0}' and deviceType='{1}' and appname='{2}'", codeUser, deviceType, appName);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsGet", "No");
                    info.Add("Message", "无此用户设备信息！");
                }
                else
                {
                    deviceToken = dt.Rows[0]["devicetoken"].ToString();
                    info.Add("IsGet", "Yes");
                    info.Add("DeviceToken", deviceToken);
                }
                         
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsGet", "No");
                info.Add("Message", ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}