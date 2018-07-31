//
//文件名：    GetUserPermissions.aspx.cs
//功能描述：  获取一二级界面用户权限
//创建时间：  2015/07/16
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

namespace MobilePlatform.UserPermission
{
    public partial class GetUserPermissions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //用户ID
            var codeUser = Request.Params["CodeUser"];
            //应用名称
            var appName = Request.Params["AppName"];

            Dictionary<string, Array> info = new Dictionary<string,Array>();
            try
            {
                if (codeUser == null || appName == null)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "举例：http://218.92.115.55/MobilePlatform/UserPermission/GetUserPermissions.aspx?CodeUser=121907&AppName=ZSLB";
                    info.Add("参加CodeUser，AppName不能为null！", arry0);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                //获取应用名称对应的iport数据库构建注册项目---对应的code_assembly
                string codeAssembly = DictionaryTool.GetCodeAssembly(appName.ToUpper());

                //获取用户一级界面权限
                string sql =
                    string.Format("select key from vw_userpermission where isenable=1 and supertype=1 and code_user='{0}' and code_assembly='{1}' order by code_sort", codeUser, codeAssembly);
                var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                if (dt1.Rows.Count == 0)
                {
                    string[] arry0 = new string[1];
                    arry0[0] = "No";
                    info.Add("IsGet", arry0);
                    string[] arry1 = new string[1];
                    arry1[0] = "网络错误，请稍后再试！";
                    info.Add("Message", arry1);
                    Json = JsonConvert.SerializeObject(info);
                    return;
                }

                string[] key1Arry = new string[dt1.Rows.Count];
                //获取用户二级界面权限
                for (int iRow = 0; iRow < dt1.Rows.Count; iRow++)
                {                
                    string key1 = Convert.ToString(dt1.Rows[iRow]["key"]);
                    key1Arry[iRow] = key1;
                    sql =
                        string.Format("select key from vw_userpermission where isenable=1 and supertype=2 and code_user='{0}' and code_assembly='{1}' and superkey='{2}' order by code_sort", codeUser, codeAssembly, key1);
                    var dt2 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                    string[] key2Arry = new string[dt2.Rows.Count];
                    for (int jRow = 0; jRow < dt2.Rows.Count; jRow++)
                    {
                        key2Arry[jRow] = Convert.ToString(dt2.Rows[jRow]["key"]);
                    }
                    info.Add(key1, key2Arry);
                }
                info.Add("Order", key1Arry);
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                string[] arry0 = new string[1];
                arry0[0] = "No";
                info.Add("IsGet", arry0);
                string[] arry1 = new string[1];
                arry1[0] = ex.Message;
                info.Add("Message", arry1);
                Json = JsonConvert.SerializeObject(info);
                return;
            }
        }
        protected string Json;
    }
}