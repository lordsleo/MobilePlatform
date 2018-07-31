//
//文件名：    GetTodayDealedList.aspx.cs
//功能描述：  获取今日已办列表
//创建时间：  2015/09/25
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
    public partial class GetTodayDealedList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var logogram = Request.Params["Logogram"];

            try
            {
                if (logogram == null)
                {
                    string warning = "参数Logogram不能为null！举例：http://218.92.115.55/MobilePlatform/OA/GetTodayDealedList.aspx?Logogram=xuehui";
                    Json = JsonConvert.SerializeObject(warning);
                    return;
                }

                DateTime curTime = DateTime.Now;
                string startTime = curTime.ToShortDateString() + " 00:00:00";
                
                string sql =
                    string.Format("select title from awsprod.wf_task_log where owner='{0}' and endtime >= to_date('{1}', 'yyyy-MM-dd hh24:mi:ss')", logogram, startTime);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);       

                if (dt.Rows.Count == 0)
                {
                    string warning = "暂无今日已办！";
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