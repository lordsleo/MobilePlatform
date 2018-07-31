using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using AndroidPush;
using Leo;
using ServiceInterface.Common;

namespace MobilePlatform
{
    /// <summary>
    /// ServiceMobile 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceMobile : System.Web.Services.WebService
    {
        #region 推送

        //#region 消息推送（以iport UserId推送）
        //[WebMethod]
        //public String Push(string token, string UserId, string appName, string Message)
        //{
        //    if (!TokenTool.VerifyToken(token))
        //    {
        //        return new Leo.Xml.Message("Token未通过校验。").FalseXml();
        //    }
        //    var sql = string.Format("select * from tb_app_device where code_user='{0}' and appname='{1}'", UserId, appName);
        //    var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
        //    if (dt.Rows.Count == 0)
        //    {
        //        return "ERROR";
        //    }
        //    var devicetoken = dt.Rows[0]["devicetoken"].ToString();
        //    var devicetype = dt.Rows[0]["devicetype"].ToString();
        //    switch (devicetype)
        //    {
        //        case "iOS":
        //            {
        //                //IOS测试devicetoken  7a6f8056710252ff8561ed1611b1b82bc398eab45b8882c12fdb143a7fb15e46
        //                string URL = "http://168.100.1.218/test.php";
        //                System.Collections.Specialized.NameValueCollection PostVars = new System.Collections.Specialized.NameValueCollection(); //参数类
        //                Message = HttpUtility.UrlEncode(Message); //中文UTF8编码转化
        //                PostVars.Add("devicetoken", devicetoken);//参数USERid  
        //                PostVars.Add("message", Message);//参数msg
        //                System.Net.WebClient wb = new System.Net.WebClient();
        //                byte[] byRemoteInfo = wb.UploadValues(URL, "POST", PostVars); //HTTP地址，POST请求，参数类
        //                string sRemoteInfo = System.Text.Encoding.Default.GetString(byRemoteInfo);//获取返回值
        //                if (sRemoteInfo == "Connected to APNS\r\nMessage successfully delivered\r\n")
        //                {
        //                    return "True";
        //                }
        //                else
        //                {
        //                    return string.Format("False");
        //                }
        //            }

        //        case "Android":
        //            {
        //                // 推送服务器地址
        //                const String url = "http://127.0.0.1:8080/androidpn/notification.do?action=send";

        //                // androidpn推送工具
        //                AndroidPn androidPush = new AndroidPn(url);

        //                try
        //                {
        //                    // 单用户推送
        //                    androidPush.PushToSingle(devicetoken, null, Message);

        //                    return "True";
        //                }
        //                catch (Exception e)
        //                {
        //                    // 失败原因
        //                    return string.Format("False");
        //                }
        //            }
        //        default:
        //            return "ERROR";
        //    }

        //    try
        //    {
        //        sql = string.Format("insert into tb_hmw_messagepush (userid,message) values ('{0}','{1}')", UserId, Message);
        //        new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
        //        return "True";
        //    }
        //    catch (Exception ex)
        //    {
        //        LogTool.WriteLog(typeof(ServiceMobile), ex);
        //        return new Leo.Xml.Message(string.Format("获取数据异常。{0}", ex.Message)).FalseXml();
        //    }
        //}
        //#endregion

        #region 消息推送
        #region 消息推送（按手机号推送）
        /// <summary>
        /// 消息推送（按手机号推送）
        /// </summary>
        /// <param name="token">校验码</param>
        /// <param name="mobile">接收者手机号码</param>
        /// <param name="SenderId">发送人用户编码</param>
        /// <param name="appName">应用程序名</param>
        /// <param name="Message">消息</param>
        /// <returns></returns>
        [WebMethod]
        public String PushByMobile(string token, string mobile, string SenderId, string appName, string Message)
        {
            try
            {
                if (!TokenTool.VerifyToken(token))
                {
                    return new Leo.Xml.Message("Token未通过校验。").FalseXml();
                }

                //mobile = "18026600293";

                SenderId = (SenderId == "" ? "0" : SenderId);
                string sql =
                    string.Format("select * from tb_app_device where mobile='{0}' and appname='{1}'", mobile, appName);
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    return new Leo.Xml.Message("无此用户设备信息！").FalseXml();
                }
                var devicetoken = dt.Rows[0]["devicetoken"].ToString();
                var devicetype = dt.Rows[0]["devicetype"].ToString();
                switch (devicetype)
                {
                    case "iOS":
                        {
                            //IOS测试devicetoken  7a6f8056710252ff8561ed1611b1b82bc398eab45b8882c12fdb143a7fb15e46
                            string URL = "http://168.100.1.218/test.php";
                            System.Collections.Specialized.NameValueCollection PostVars = new System.Collections.Specialized.NameValueCollection(); //参数类
                            Message = HttpUtility.UrlEncode(Message); //中文UTF8编码转化
                            PostVars.Add("devicetoken", devicetoken);//参数USERid  
                            PostVars.Add("message", Message);//参数msg
                            System.Net.WebClient wb = new System.Net.WebClient();
                            byte[] byRemoteInfo = wb.UploadValues(URL, "POST", PostVars); //HTTP地址，POST请求，参数类
                            string sRemoteInfo = System.Text.Encoding.Default.GetString(byRemoteInfo);//获取返回值
                            if (sRemoteInfo == "Connected to APNS\r\nMessage successfully delivered\r\n")
                            {
                                break;
                            }
                            else
                            {
                                return new Leo.Xml.Message("推送失败！").FalseXml();
                            }
                        }

                    case "Android":
                        {
                            // 推送服务器地址
                            const String url = "http://127.0.0.1:8080/androidpn/notification.do?action=send";

                            // androidpn推送工具
                            AndroidPn androidPush = new AndroidPn(url);

                            // 单用户推送
                            androidPush.PushToSingle(devicetoken, null, Message);
                            break;
                        }
                    default:
                        return new Leo.Xml.Message("暂不支持iOS,Android以外设备推送！").FalseXml();
                }

                //插入推送数据库
                //sql = string.Format("insert into tb_hmw_messagepush (userid,message) values ('{0}','{1}')", UserId, Message);
                //new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
                return new Leo.Xml.Message("推送成功！").TrueXml();
            }
            catch (Exception ex)
            {
                LogTool.WriteLog(typeof(ServiceMobile), ex);
                return new Leo.Xml.Message(string.Format("获取数据异常。{0}", ex.Message)).FalseXml();
            }
        }
        #endregion

        #region 消息推送（按用户编码号推送）
        /// <summary>
        /// 消息推送（按手机号推送）
        /// </summary>
        /// <param name="token">校验码</param>
        /// <param name="mobile">接收者用户编码</param>
        /// <param name="SenderId">发送者用户编码</param>
        /// <param name="appName">应用程序名</param>
        /// <param name="Message">消息</param>
        /// <returns></returns>
        [WebMethod]
        public String PushByCodeUser(string token, string ReceiverId, string SenderId, string appName, string Message)
        {
            try
            {
                if (!TokenTool.VerifyToken(token))
                {
                    return new Leo.Xml.Message("Token未通过校验。").FalseXml();
                }

                //mobile = "18026600293";

                SenderId = (SenderId == "" ? "0" : SenderId);
                string sql =
                    string.Format("select * from tb_app_device where code_user='{0}' and appname='{1}'", ReceiverId, appName);
                var dt = new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
                if (dt.Rows.Count == 0)
                {
                    return new Leo.Xml.Message("无此用户设备信息！").FalseXml();
                }
                var devicetoken = dt.Rows[0]["devicetoken"].ToString();
                var devicetype = dt.Rows[0]["devicetype"].ToString();
                switch (devicetype)
                {
                    case "iOS":
                        {
                            //IOS测试devicetoken  7a6f8056710252ff8561ed1611b1b82bc398eab45b8882c12fdb143a7fb15e46
                            string URL = "http://168.100.1.218/test.php";
                            System.Collections.Specialized.NameValueCollection PostVars = new System.Collections.Specialized.NameValueCollection(); //参数类
                            Message = HttpUtility.UrlEncode(Message); //中文UTF8编码转化
                            PostVars.Add("devicetoken", devicetoken);//参数USERid  
                            PostVars.Add("message", Message);//参数msg
                            System.Net.WebClient wb = new System.Net.WebClient();
                            byte[] byRemoteInfo = wb.UploadValues(URL, "POST", PostVars); //HTTP地址，POST请求，参数类
                            string sRemoteInfo = System.Text.Encoding.Default.GetString(byRemoteInfo);//获取返回值
                            if (sRemoteInfo == "Connected to APNS\r\nMessage successfully delivered\r\n")
                            {
                                break;
                            }
                            else
                            {
                                return new Leo.Xml.Message("推送失败！").FalseXml();
                            }
                        }

                    case "Android":
                        {
                            // 推送服务器地址
                            const String url = "http://127.0.0.1:8080/androidpn/notification.do?action=send";

                            // androidpn推送工具
                            AndroidPn androidPush = new AndroidPn(url);

                            // 单用户推送
                            androidPush.PushToSingle(devicetoken, null, Message);
                            break;
                        }
                    default:
                        return new Leo.Xml.Message("暂不支持iOS,Android以外设备推送！").FalseXml();
                }

                //插入推送数据库
                //sql = string.Format("insert into tb_hmw_messagepush (userid,message) values ('{0}','{1}')", UserId, Message);
                //new Leo.Oracle.DataAccess(Leo.RegistryKey.KeyPathMa).ExecuteTable(sql);
                return new Leo.Xml.Message("推送成功！").TrueXml();
            }
            catch (Exception ex)
            {
                LogTool.WriteLog(typeof(ServiceMobile), ex);
                return new Leo.Xml.Message(string.Format("获取数据异常。{0}", ex.Message)).FalseXml();
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
