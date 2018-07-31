//
//文件名：    GetPersonContactList.aspx.cs
//功能描述：  获取员工通讯列表
//创建时间：  2015/07/27
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
    public partial class GetPersonContactList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //应用名称
            var appName = Request.Params["AppName"];

            try
            {
                if (appName == null)
                {
                    string warning = "参数AppName不能为null！举例：http://218.92.115.55/MobilePlatform/Contacts/GetPersonContactList.aspx?AppName=WLKG";
                    Json = JsonConvert.SerializeObject(warning);
                    return;
                }

                //获取App应用对应的公司，公司对应的内部网公司ID
                string nbwCompanyId = DictionaryTool.GetNbwCompanyId(appName.ToUpper());
                if (nbwCompanyId == string.Empty)
                {
                    string warning = "应用名称错误！";
                    Json = JsonConvert.SerializeObject(warning);
                    return;
                }

                string sql = string.Format("select code,Description from gpms2000_nbw..department_sr");
                var dt0 = new Leo.SqlServer.DataAccess(RegistryKey.KeyPathNbwDept).ExecuteTable(sql);
                if (dt0.Rows.Count == 0)
                {
                    string error = "网络错误，请稍后再试！";
                    Json = JsonConvert.SerializeObject(error);
                    return;
                }

                sql =
                    string.Format(
                        "select a.id,a.username as login_name,a.truename as user_name,a.tel,a.email,a.companyid as company_id,b.shortname as company_name,a.bumenid as dept_id,a.tel1,a.duty,a.tel2,a.pemail,a.phone1,a.head_pic from user_info a, company_name b where a.companyid='{0}' and substr(a.bumenid,1,4)=b.companyid  order by user_name",
                        nbwCompanyId);
                var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathNbw).ExecuteTable(sql);

                var infoArray = new List<Dictionary<string, string>>();
                for (int iRow = 0; iRow < dt1.Rows.Count; iRow++)
                {
                    string departmentName = "";
                    bool markWeibo = true;
                    var rows = dt0.Select(string.Format("CODE='{0}'", Convert.ToString(dt1.Rows[iRow]["dept_id"])));
                    if (rows.Length > 0)
                        departmentName = rows[0]["DESCRIPTION"] as string;
                    departmentName = departmentName == null ? string.Empty : departmentName;

                    markWeibo =
                        !(dt1.Rows[iRow]["EMAIL"] is DBNull) && !string.IsNullOrEmpty(dt1.Rows[iRow]["EMAIL"] as string) &&
                        !string.IsNullOrWhiteSpace(dt1.Rows[iRow]["EMAIL"] as string);

                    Dictionary<string, string> info = new Dictionary<string,string>();
                    info.Add("userID", Convert.ToString(dt1.Rows[iRow]["ID"]));
                    info.Add("name", Convert.ToString(dt1.Rows[iRow]["USER_NAME"]));
                    info.Add("company", Convert.ToString(dt1.Rows[iRow]["COMPANY_NAME"]));
                    info.Add("department", departmentName);
                    info.Add("duty", Convert.ToString(dt1.Rows[iRow]["DUTY"]));
                    info.Add("mobilephone", Convert.ToString(dt1.Rows[iRow]["TEL"]));
                    info.Add("backupmobilephone", Convert.ToString(dt1.Rows[iRow]["PHONE1"]));
                    info.Add("telephone", Convert.ToString(dt1.Rows[iRow]["TEL1"]));
                    info.Add("backuptelephone", Convert.ToString(dt1.Rows[iRow]["TEL2"]));
                    info.Add("workemail", Convert.ToString(dt1.Rows[iRow]["EMAIL"]));
                    info.Add("personalemail", Convert.ToString(dt1.Rows[iRow]["PEMAIL"]));
                    info.Add("weibo", markWeibo == true ? "已开通" : "未开通");
                    infoArray.Add(info);
                }

                Json = JsonConvert.SerializeObject(infoArray);
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(ex.Message);
            }

        }
        protected string Json;
    }
}