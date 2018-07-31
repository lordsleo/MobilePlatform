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
    public partial class ShortMessage : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            var UserName = Request.Params["UserName"];
            var Password = Request.Params["Password"];
            var Code_User = Request.Params["Code_User"];
            var Tel = Request.Params["Tel"];
            var Message = Request.Params["Message"];

            Dictionary<string, string> info = new Dictionary<string, string>();
            try
            {
                if(UserName!=null & Password!=null)
                {
                    var sql = string.Format("select * from TB_SYS_USER t where t.logogram ='{0}'", UserName);
                    var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        if (Identity.VerifyText(Password, dt.Rows[0]["Password"] as string))
                        {
                            var sql1 = string.Format("select * from tb_msg_user where code_user = '{0}'", dt.Rows[0]["Code_User"] as string);
                            var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql1);
                            
                            info.Add("Code_User", dt.Rows[0]["Code_User"] as string);
                            if (dt1.Rows.Count == 1)
                            {
                                info.Add("MessageLeft", Convert.ToString(dt1.Rows[0][0]));
                            }
                            else
                            {
                                info.Add("MessageLeft", "0");
                            }
                            info.Add("Error", null);
                            info.Add("IsLogin", "True");
                        }
                        else
                        {
                            info.Add("Error", "密码错误");
                            info.Add("IsLogin", "False");
                        }
                    }
                    else
                    {
                        info.Add("Error", "用户名错误");
                        info.Add("IsLogin", "False");
                    }
                }

                if (Code_User != null & Tel != null & Message != null)
                {
                    var sql = string.Format("INSERT INTO TB_MSG_INFO (code_user,tel,message) VALUES ('{0}','{1}','{2}')", Code_User,Tel,Message);
                    var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql);

                    var sql1 = string.Format("select * from tb_msg_user where code_user = '{0}'",Code_User);
                    var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql1);
                    
                    var sql2 = string.Format("UPDATE tb_msg_user SET messageleft = '{0}' WHERE code_user = '{1}'",Convert.ToInt16(dt1.Rows[0]["messageleft"])-1, Code_User);
                    var dt2 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(sql2);
                    SendMessage.mobileSoapClient send = new SendMessage.mobileSoapClient();

                    var company =dt1.Rows[0]["company"] as string;
                    var department = dt1.Rows[0]["department"]as string;
                    send.sendmessage(Tel, Message, "短信系统", "短信系统");
                    
                }
                Json = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
                info.Add("Error", ex.Message);
                Json = JsonConvert.SerializeObject(info);
            }
        }
        protected string Json;
    }
}