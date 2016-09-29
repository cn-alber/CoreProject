using System.Collections.Generic;
using CoreDate.CoreApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{
    [AllowAnonymous]
    public class JdOrderControllers : ControllBase
    {

        

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
            List<string> ids = new List<string>();
            ids.Add("22919473317");
            
            var m = JingDHaddle.orderDownByIds(order_id, optional_fields, order_state, token);
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion























    }
}