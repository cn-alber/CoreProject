using CoreDate.CoreApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{
    [AllowAnonymous]
    public class JdOrderControllers : ControllBase
    {

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/hello")]
        public ResponseResult taskdata()
        {
            string code = "LK0EZY";
            var m = JingDHaddle.GetToken("https://oauth.jd.com/oauth/token", "authorization_code", code, "http://localhost:8080/AppWeb/admin/shop_token.aspx", "7888CE9C0F3AAD424FEE8EEAAC99E10E", "a88b70e6c629465785088f9a9151701a");
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/download")]
        public ResponseResult orderDownload(string start_date, string end_date, string order_state, string page, string page_size, string token)
        {            
            var m = JingDHaddle.jdOrderDownload(start_date, end_date, order_state, page, page_size, token);
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/downByIds")]
        public ResponseResult downByIds(string order_id,string optional_fields,string order_state,string token)
        {            
            var m = JingDHaddle.orderDownByIds(order_id, optional_fields, order_state, token);
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion























    }
}