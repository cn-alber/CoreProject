using System.Collections.Generic;
using CoreDate.CoreApi;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{    
    public class JdOrderControllers : ControllBase
    {
        #region 
        [HttpGetAttribute("/core/Api/JdOrder/download")]
        public ResponseResult orderDownload() //string start_date, string end_date, string order_state, string page, string page_size, string token
        {            
            string start_date="2016-08-15 11:29:19";
            string end_date ="2016-09-11 11:29:19";
            string   order_state = "WAIT_SELLER_STOCK_OUT,WAIT_GOODS_RECEIVE_CONFIRM,WAIT_SELLER_DELIVERY,FINISHED_L,TRADE_CANCELED,LOCKED,PAUSE";
            string   page = "1";
            string   page_size = "10";
            string   token="fc8e04d5-2d8d-458c-b188-d55852af94f3";

            var m = JingDHaddle.jdOrderDownload(start_date, end_date, order_state, page, page_size, token);
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/downByIds")]
        public ResponseResult downByIds(List<string> order_id,string optional_fields,string order_state,string token)
        {            
            List<string> ids = new List<string>();
            ids.Add("22919473317");
            ids.Add("23080116194");
            // ids.Add("22700014989");
            // ids.Add("22765138891");
            // ids.Add("22718110928");
            // ids.Add("22717671671");

            var m = JingDHaddle.orderDownByIds(ids, optional_fields, order_state, token);
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion























    }
}