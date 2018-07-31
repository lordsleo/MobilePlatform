using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Leo;
using Leo.Json;
using YGSoft.IPort.Data;
using ServiceInterface.Common;

namespace MobilePlatform
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var Logogram = Request.Params["Logogram"];
            var Password = Request.Params["Password"];
            var DeviceToken = Request.Params["DeviceToken"];
            var DeviceType = Request.Params["DeviceType"];
            var AppName = Request.Params["AppName"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (Logogram == null || Password == null || DeviceToken == null || DeviceType == null || AppName == null)
                {
                    info.Add("参数Logogram，Password，DeviceToken，DeviceType，AppName不能为空！", "举例：http://218.92.115.55/MobilePlatform/Login.aspx?Logogram=zhangsan&Password=123&DeviceToken=aaaaaa&DeviceType=iOS&AppName=HMW");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }
                
                //获取应用名称对应的用户类型
                string UserType = DictionaryTool.GetUserType(AppName.ToUpper());
                if (UserType == string.Empty)
                {
                    return;
                }

                string Code_User;
                string Code_Company;
                string Code_Department;
                string UserName;
                string Mobile = string.Empty;
                //string Department;

                if (UserType == "nbw")
                {
                    string sql = string.Format("select * from USER_INFO where username='{0}'", Logogram);
                    var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathNbw).ExecuteTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "用户名错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    if (Password==dt.Rows[0]["PASSWORD"] as string)
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "密码错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    Code_User = dt.Rows[0]["GONGHAO"] as string;
                    Code_Company = dt.Rows[0]["Code_Company"] as string;
                    Code_Department = dt.Rows[0]["Code_Department"] as string;
                    UserName = dt.Rows[0]["TrueName"] as string;
                    //Department = dt.Rows[0]["Duty"] as string;
                }
                else if (UserType == "iPort")
                {
                    //获取应用名称与Iport关键字对应字典表
                    string iportKeyDataBase = DictionaryTool.GetIportKeyDataBase(AppName.ToUpper());
                    string sql =
                        string.Format(@"select a.code_user,a.code_company,a.code_department,a.username,a.password,b.mobile 
                                        from TB_SYS_USER a,TB_SYS_USERINFO b 
                                        where a.code_user=b.code_user(+) and  upper(logogram)='{0}' and mark_audit='1'", 
                                        Logogram.ToUpper());
                    var dt = new Leo.Oracle.DataAccess(iportKeyDataBase).ExecuteTable(sql);
                    if (dt.Rows.Count <= 0)
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "用户名错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }
                    if (!Identity.VerifyText(Format.Trim(Password), dt.Rows[0]["PASSWORD"] as string))
                    {
                        info.Add("IsLogin", "No");
                        info.Add("Message", "密码错误！");
                        Json = JsonConvert.SerializeObject(info);
                        return;
                    }

                    Code_User = dt.Rows[0]["Code_User"] as string;
                    Code_Company = dt.Rows[0]["Code_Company"] as string;
                    Code_Department = dt.Rows[0]["Code_Department"] as string;
                    UserName = dt.Rows[0]["UserName"] as string;
                    Mobile = dt.Rows[0]["Mobile"] as string;
                    
                    //Department = dt.Rows[0]["Duty"] as string;
                }
                else
                {
                    return;
                }

                Mobile = (Mobile == null ? "" : Mobile);
                //获取应用名称与移动设备关键字对应字典表
                string deviceKeyDataBase = DictionaryTool.GetDeviceKeyDataBase(AppName.ToUpper());
                string sql1 = string.Format("select * from tb_app_device where mobile='{0}' and appname='{1}'", Mobile, AppName);
                var dt1 = new Leo.Oracle.DataAccess(deviceKeyDataBase).ExecuteTable(sql1);
                string isBinding = "0";

                if (dt1.Rows.Count == 0)
                {
                    //sql1 = string.Format("insert into tb_app_device (mobile,devicetoken,devicetype,appname) values ('{0}','{1}','{2}','{3}',to_date('{4}','YYYY-MM-DD HH24:MI:SS'))", Mobile, DeviceToken, DeviceType, AppName);
                    //dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql1);
                    //isBinding = "0";
                }
                else
                {
                    if ("1" == Convert.ToString(dt1.Rows[0]["isbinding"]))
                    {
                        if (DeviceToken != dt1.Rows[0]["DEVICETOKEN"] as string)
                        {
                            info.Add("IsLogin", "No");
                            info.Add("Error", "登录失败，已绑定到其他设备！");
                            Json = JsonConvert.SerializeObject(info);
                            return;
                        }
                    }
                    isBinding = Convert.ToString(dt1.Rows[0]["isbinding"]);
                }
                info.Add("IsLogin", "Yes");
                info.Add("Code_User", Code_User);
                info.Add("Code_Department", Code_Department);
                info.Add("Code_Company", Code_Company);
                info.Add("UserName", UserName);
                info.Add("IsBinding", isBinding);
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsLogin", "No");
                info.Add("Message", ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}