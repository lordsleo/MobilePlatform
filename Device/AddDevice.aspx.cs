//
//文件名：    AddDevice.aspx.cs
//功能描述：  添加移动用户设备
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
    public partial class AddDevice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var deviceToken = Request.Params["DeviceToken"];
            var deviceType = Request.Params["DeviceType"];
            var appName = Request.Params["AppName"];
            var codeUser = Request.Params["CodeUser"];
            var mobile = Request.Params["Mobile"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (mobile == null || deviceToken == null || deviceType == null || appName == null || codeUser == null)
                {
                    info.Add("参数Mobile，DeviceToken，DeviceType，AppName，CodeUser不能为null！", "举例：http://218.92.115.55/MobilePlatform/Device/AddDevice.aspx?Mobile=18000000000&DeviceToken=aaaaaa&DeviceType=iOS&AppName=SPHSJ&CodeUser=ASFDFEWGWGFEW");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string sql =
                    string.Format("select * from tb_app_device where devicetoken='{0}' and devicetype='{1}' and appname='{2}'", deviceToken, deviceType, appName);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    sql =
                        string.Format("insert into tb_app_device (mobile,devicetoken,devicetype,appname,code_user) values ('{0}','{1}','{2}','{3}','{4}')", mobile, deviceToken, deviceType, appName, codeUser);

                }
                else
                {
                    sql =
                        string.Format("update tb_app_device set code_user='{0}',mobile='{1}',modifytime=to_date('{2}','YYYY-MM-DD HH24:MI:SS') where devicetoken='{3}' and devicetype='{4}' and appname='{5}'", codeUser, mobile, DateTime.Now, deviceToken, deviceType, appName);
                }

                dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                info.Add("IsAdd", "Yes");
                info.Add("Message", "添加或更细成功！");
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsAdd", "No");
                info.Add("Message", ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}