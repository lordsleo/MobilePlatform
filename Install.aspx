<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Install.aspx.cs" Inherits="MobilePlatform.Install" %>
  <script type="text/javascript">
      

      function getQueryString(name) {
          var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
          var r = window.location.search.substr(1).match(reg);
          if (r != null) return unescape(r[2]); return null;
      }

      function getDeviceType() {

				if (/android/i.test(navigator.userAgent)){
					return "Android";
				}
				if (/iphone/i.test(navigator.userAgent)||/ipad/i.test(navigator.userAgent)){
					return "iOS";
				}
				else{
				    return "PC";
				}
      }

          if (getQueryString("DeviceType") == null & getQueryString("AppName") != null) {

              self.location = "Install.aspx?AppName=" + getQueryString("Appname") + "&DeviceType=" + getDeviceType();
          }
          if (getDeviceType() == "PC") {
              alert("请使用Android/iOS设备或联系技术支持。")
          }

    </script>
<%=Json %>