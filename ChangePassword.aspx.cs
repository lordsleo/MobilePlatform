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
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var codeUser = Request.Params["CodeUser"];
            var oldPassword = Request.Params["OldPassword"];
            var newPassword = Request.Params["NewPassword"];
            var appName = Request.Params["AppName"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null || oldPassword == null || newPassword == null || appName == null)
                {
                    info.Add("参数CodeUser，OldPassword，NewPassword，AppName不能为空！", "举例：http://218.92.115.55/MobilePlatform/ChangePassword.aspx?CodeUser=0000&OldPassword=123&NewPassword=456&AppName=HMW");
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

                if (UserType == "nbw")
                {
                    string sql = string.Format("select PWD,PASSWORD from USER_INFO t where t.GONGHAO ='{0}'", codeUser);
                    var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathNbw).ExecuteTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        info.Add("IsChange", "No");
                        info.Add("Message", "用户名错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    if (!Identity.VerifyText(oldPassword, dt.Rows[0]["PASSWORD"] as string))
                    {
                        info.Add("IsChange", "No");
                        info.Add("Message", "密码错误错误！");
                    }
                    else
                    {
                        sql = string.Format(
                           "update USER_INFO set PWD='{0}',PASSWORD='{1}' where GONGHAO='{2}'",
                           newPassword, Identity.EncodeText(newPassword), codeUser);
                        dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathNbw).ExecuteTable(sql);
                        info.Add("IsChange", "Yes");
                        info.Add("Message", "修改成功！");
                    }
                }
                else if (UserType == "iPort")
                {
                    string sql = string.Format("select PASSWORD from TB_SYS_USER t where t.CODE_USER ='{0}'", codeUser);
                    var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        info.Add("IsChange", "No");
                        info.Add("Message", "用户名错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }

                    if (dt.Rows[0]["PASSWORD"] as string != null)
                    {
                        if (!Identity.VerifyText(oldPassword, dt.Rows[0]["PASSWORD"] as string))
                        {
                            info.Add("IsChange", "No");
                            info.Add("Message", "密码错误！");
                        }
                        else
                        {
                            sql = string.Format(
                               "update TB_SYS_USER set DPBEGINTIME=null,DYNAMICPASSWORD=null,PASSWORD='{0}' where CODE_USER='{1}'",
                               Identity.EncodeText(newPassword), codeUser);
                            dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                            info.Add("IsChange", "Yes");
                            info.Add("Message", "修改成功！");
                        }

                    }
                    else if (dt.Rows[0]["PASSWORD"] as string == null && Convert.ToString(oldPassword) == "")
                    {
                        sql = string.Format(
                           "update TB_SYS_USER set DPBEGINTIME=null,DYNAMICPASSWORD=null,PASSWORD='{0}' where CODE_USER='{1}'",
                           Identity.EncodeText(newPassword), codeUser);
                        dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                        info.Add("IsChange", "Yes");
                        info.Add("Message", "修改成功！");
                    }
                    else
                    {
                        info.Add("IsChange", "No");
                        info.Add("Message", "密码错误！");
                    }
                }
                else
                {
                    return;
                }

                Json = JsonConvert.SerializeObject(info);

            }
            catch (Exception ex)
            {
                info.Add("IsChange", "No");
                info.Add("Message", ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}