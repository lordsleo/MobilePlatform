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
    public partial class PersonInformation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var codeUser = Request.Params["CodeUser"];
            var appName = Request.Params["AppName"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || appName == null)
                {
                    info.Add("参数CodeUser，AppName不能为空！", "举例：http://218.92.115.55/MobilePlatform/PersonInformation.aspx?CodeUser=0000&AppName=HMW");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string UserType;
                switch (appName)
                {
                    case "HMW": UserType = "iPort"; break;
                    case "YGHY": UserType = "nbw"; break;
                    case "IPORT": UserType = "iPort"; break;
                    case "WLKG": UserType = "iPort"; break;
                    default: return;
                }

                string phone;
                string mobile;
                string email;

                if (UserType == "nbw")
                {
                    string sql = string.Format("select TEL,TEL1,EMAIL from USER_INFO t where t.GONGHAO='{0}'", codeUser);
                    var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathNbw).ExecuteTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        info.Add("IsGet", "No");
                        info.Add("Message", "用户名错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    phone = dt.Rows[0]["TEL"] as string;
                    mobile = dt.Rows[0]["TEL1"] as string;
                    email = dt.Rows[0]["EMAIL"] as string; 
                }
                else if (UserType == "iPort")
                {
                    string sql = string.Format("select PHONE,MOBILE,EMAIL from tb_sys_userinfo t where t.CODE_USER='{0}'", codeUser);
                    var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        info.Add("IsGet", "No");
                        info.Add("Message", "用户名错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    phone = dt.Rows[0]["PHONE"] as string;
                    mobile = dt.Rows[0]["MOBILE"] as string;
                    email = dt.Rows[0]["EMAIL"] as string;         
                }
                else
                {
                    return;
                }

                info.Add("IsGet", "Yes");
                info.Add("Phone", phone);
                info.Add("Mobile", mobile);
                info.Add("Email", email);
                Json = JsonConvert.SerializeObject(info);

            }
            catch (Exception ex)
            {
                info.Add("IsGet", "No");
                info.Add("Error", ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}