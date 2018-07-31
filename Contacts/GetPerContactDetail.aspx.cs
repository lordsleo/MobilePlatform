//
//文件名：    GetPerContactDetail.aspx.cs
//功能描述：  获取员工通讯明细
//创建时间：  2015/07/20
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
using ServiceInterface.Common;

namespace MobilePlatform.Contacts
{
    public partial class GetPerContactDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户ID
            var codeUser = Request.Params["CodeUser"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if (codeUser == null)
                {
                    info.Add("参加CodeUser", "http://218.92.115.55/MobilePlatform/UserPermission/GetPerContactDetail.aspx?CodeUser=67545");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string sql = string.Format("select code,Description from gpms2000_nbw..department_sr");
                var dt0 = new Leo.SqlServer.DataAccess(RegistryKey.KeyPathNbwDept).ExecuteTable(sql);
                if (dt0.Rows.Count == 0)
                {
                    info.Add("IsGet", "No");
                    info.Add("Message", "网络错误，请稍后再试！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                sql =
                    string.Format(
                        "select t.user_id,t.login_name,t.user_name,t.tel,t.email,t.company_id,t.company_name,t.dept_id,t.tel1,t.duty,t.tel2,t.pemail,t.phone1,t.head_pic from VW_SYS_USER t where t.user_id='{0}'",
                        codeUser);
                var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt1.Rows.Count == 0)
                {
                    info.Add("IsGet", "No");
                    info.Add("Message", "用户ID不存在！");
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string departmentName = "";
                bool markWeibo = true;
                var rows = dt0.Select(string.Format("CODE='{0}'", Convert.ToString(dt1.Rows[0]["dept_id"])));
                if (rows.Length > 0)
                    departmentName = rows[0]["DESCRIPTION"] as string;
                markWeibo =
                    !(dt1.Rows[0]["EMAIL"] is DBNull) && !string.IsNullOrEmpty(dt1.Rows[0]["EMAIL"] as string) &&
                    !string.IsNullOrWhiteSpace(dt1.Rows[0]["EMAIL"] as string);

                info.Add("姓名", Convert.ToString(dt1.Rows[0]["USER_NAME"]));
                info.Add("机构", Convert.ToString(dt1.Rows[0]["COMPANY_NAME"]));
                info.Add("部门", departmentName);
                info.Add("职务", Convert.ToString(dt1.Rows[0]["DUTY"]));
                info.Add("工作手机", Convert.ToString(dt1.Rows[0]["TEL"]));
                info.Add("备用手机", Convert.ToString(dt1.Rows[0]["PHONE1"]));
                info.Add("办公室电话", Convert.ToString(dt1.Rows[0]["TEL1"]));
                info.Add("备用电话", Convert.ToString(dt1.Rows[0]["TEL2"]));
                info.Add("集团邮箱", Convert.ToString(dt1.Rows[0]["EMAIL"]));
                info.Add("个人邮箱", Convert.ToString(dt1.Rows[0]["PEMAIL"]));
                info.Add("云之家微博", markWeibo == true ? "已开通" : "未开通");

                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("IsGet", "No");
                info.Add("Message", ex.Message);
                Json = JsonConvert.SerializeObject(info);
                return;
            }
        }
        protected string Json;
    }
}