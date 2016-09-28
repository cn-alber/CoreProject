using CoreDate.CoreApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{
    [AllowAnonymous]
    public class JdOrderControllers : ControllBase
    {

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/getToken")]
        public ResponseResult getToken()
        {
            string code = "132456";
            var m = JingDHaddle.GetToken("https://oauth.jd.com/oauth/token", "authorization_code", code, "http://localhost:8080/AppWeb/admin/shop_token.aspx", "7888CE9C0F3AAD424FEE8EEAAC99E10E", "a88b70e6c629465785088f9a9151701a");
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion




    }
}