//
//文件名：    GetPerContactList.aspx.cs
//功能描述：  获取员工通讯列表
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
    public partial class GetPerContactList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //应用名称
            var appName = Request.Params["AppName"];
            //用户名筛选
            var filter = Request.Params["Filter"];

            Dictionary<string, Array> info = new Dictionary<string, Array>();
            try
            {
                if (appName == null)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "http://218.92.115.55/MobilePlatform/Contacts/GetPerContactList.aspx?AppName=WLKG&filter=";
                    info.Add("参数AppName，Filter不能为null", arry0);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取App应用对应的公司，公司对应的内部网公司ID
                string nbwCompanyId = DictionaryTool.GetNbwCompanyId(appName.ToUpper());
                if (nbwCompanyId == string.Empty)
                {
                    return;
                }

                string sql;
                if (string.IsNullOrEmpty(filter) || string.IsNullOrWhiteSpace(filter))
                {
                    sql =
                        string.Format(
                            "select t.user_id,t.login_name,t.user_name,t.tel,t.email,t.company_id,t.company_name,t.dept_id,t.tel1,t.duty,t.tel2,t.pemail,t.phone1,t.head_pic from VW_SYS_USER t where t.company_id='{0}' order by t.user_name",
                            nbwCompanyId);
                }
                else
                {
                    sql =
                        string.Format(
                            "select t.user_id,t.login_name,t.user_name,t.tel,t.email,t.company_id,t.company_name,t.dept_id,t.tel1,t.duty,t.tel2,t.pemail,t.phone1,t.head_pic from VW_SYS_USER t where t.company_id='{0}' and t.user_name like '%{1}%' order by t.user_name",
                            nbwCompanyId, filter);
                }

                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);
                string[,] arrys = new string[dt.Rows.Count, 4];

                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    arrys[iRow, 0] = Convert.ToString(dt.Rows[iRow]["user_id"]);
                    arrys[iRow, 1] = Convert.ToString(dt.Rows[iRow]["user_name"]);
                    arrys[iRow, 2] = Convert.ToString(dt.Rows[iRow]["tel1"]);
                    arrys[iRow, 3] = Convert.ToString(dt.Rows[iRow]["email"]);
                }

                string[] arry2 = new string[1];
                arry2[0] = "Yes";
                info.Add("IsGet", arry2);
                info.Add("PerContactList", arrys);
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                string[] arry0 = new string[1];
                arry0[0] = "NO";
                info.Add("IsGet", arry0);
                string[] arry1 = new string[1];
                arry1[0] = ex.Message;
                info.Add("Message", arry1);
                Json = JsonConvert.SerializeObject(info);
            }

        }
        protected string Json;
    }
}