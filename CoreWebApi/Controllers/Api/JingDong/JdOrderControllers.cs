using System;
using System.Collections.Generic;
using CoreDate.CoreApi;
using CoreModels;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{    

    public class JdOrderControllers : ControllBase
    {
        #region 
        [HttpGetAttribute("/core/Api/JdOrder/download")]
        public ResponseResult orderDownload(string start_date="", string end_date="", string order_state="", int page=1, int pageSize=100, string token="")
        {            
            // string start_date="2016-08-15 11:29:19";
            // string end_date ="2016-09-11 11:29:19";
            // string   order_state = "WAIT_SELLER_STOCK_OUT,WAIT_GOODS_RECEIVE_CONFIRM,WAIT_SELLER_DELIVERY,FINISHED_L,TRADE_CANCELED,LOCKED,PAUSE";
            // string   page = "1";
            // string   page_size = "10";
            // string   token="fc8e04d5-2d8d-458c-b188-d55852af94f3";
            page = Math.Max(page,1);
            pageSize = Math.Min(pageSize,100);

            var m = JingDHaddle.jdOrderDownload(start_date, end_date, page, pageSize, token,order_state);
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/downByIds")]
        public ResponseResult downByIds(List<string> order_ids,string token="",string optional_fields = "",string order_state="")
        {                  
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(order_ids.Count == 0){
                m.s = -5004;
            }else{
                m = JingDHaddle.orderDownByIds(order_ids, optional_fields, token,order_state);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/JdOrder/")]
        public ResponseResult demo(string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                
            }


            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

















    }
}