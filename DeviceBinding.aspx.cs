using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Leo;
using Leo.Json;
using YGSoft.IPort.Data;

namespace MobilePlatform
{
    public partial class DeviceBinding : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var codeUser = Request.Params["CodeUser"];
            var deviceToken = Request.Params["DeviceToken"];
            var deviceType = Request.Params["DeviceType"];                     
            var appName = Request.Params["AppName"];
            var isBinding = Request.Params["IsBinding"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || deviceToken == null || deviceType == null || appName == null)
                {
                    info.Add("参数CodeUser，DeviceToken，DeviceType，AppName不能为空！", "举例：http://218.92.115.55/MobilePlatform/DeviceBinding.aspx?CodeUser=0000&DeviceToken=aaaaaa&DeviceType=iOS&AppName=HMW&IsBinding=0");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string sql = string.Format("select * from tb_app_device where Code_User='{0}' and appname='{1}'", codeUser, appName);
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsBinding", "No");
                    info.Add("Message", "登陆出错！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                else
                {
                    if (dt.Rows[0]["isbinding"].ToString() == "1" && isBinding == "1")
                    {
                        info.Add("IsBinding", "No");
                        info.Add("Message", "当前用户已被绑定！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    else
                    {
                        sql = 
                            string.Format("update tb_app_device set devicetoken='{0}',devicetype='{1}',isbinding='{2}',modifytime=to_date('{5}','YYYY-MM-DD HH24:MI:SS') where code_user='{3}' and appname='{4}'", deviceToken, deviceType, isBinding, codeUser, appName, DateTime.Now);
                    }
                }
                dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
                sql = string.Format("select devicetoken,isbinding from tb_app_device where Code_User='{0}' and appname='{1}'", codeUser, appName);
                dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    info.Add("IsBinding", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                }
                else{
                    if (dt.Rows[0]["devicetoken"].ToString() == deviceToken)
                    {
                        if (dt.Rows[0]["isbinding"].ToString() == isBinding)
                        {
                            switch (isBinding)
                            {
                                case "0":
                                    info.Add("IsBinding", "Yes");
                                    info.Add("Message", "解绑成功！");
                                    break;
                                case "1": 
                                    info.Add("IsBinding", "Yes");
                                    info.Add("Message", "绑定成功！");
                                    break;
                                default: 
                                    info.Add("IsBinding", "No");
                                    info.Add("Message", "网络错误，请稍后再试！！");
                                    break;
                            }
                        }
                    }       
                }
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsBinding", "No");
                info.Add("Message", ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}