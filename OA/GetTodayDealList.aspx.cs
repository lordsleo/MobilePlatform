//
//文件名：    GetTodayDealList.aspx.cs
//功能描述：  获取今日待办列表
//创建时间：  2015/07/29
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
using ServiceInterface.Common;

namespace MobilePlatform.OA
{
    public partial class GetTodayDealList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var logogram = Request.Params["Logogram"];

            try
            {
                if (logogram == null)
                {
                    string warning = "参数Logogram不能为null！举例：http://218.92.115.55/MobilePlatform/OA/GetTodayDealList.aspx?Logogram=xuehui";
                    Json = JsonConvert.SerializeObject(warning);
                    return;
                }

                string sql =
                    string.Format("select title from awsprod.wf_task where target='{0}'", logogram);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);        

                if (dt.Rows.Count == 0)
                {
                    string warning = "暂无今日待办！";
                    Json = JsonConvert.SerializeObject(warning);
                }
                else
                {
                    string[] arrays = new string[dt.Rows.Count];
                    for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                    {
                        arrays[iRow] = Convert.ToString(dt.Rows[iRow]["title"]);
                    }
                    Json = JsonConvert.SerializeObject(arrays);
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