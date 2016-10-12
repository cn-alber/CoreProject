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
        public ResponseResult orderDownload(string start_date="", string end_date="", string order_state="", int page=1, int page_size=100, string token="")
        {            
            // string start_date="2016-08-15 11:29:19";
            // string end_date ="2016-09-11 11:29:19";
            // string   order_state = "WAIT_SELLER_STOCK_OUT,WAIT_GOODS_RECEIVE_CONFIRM,WAIT_SELLER_DELIVERY,FINISHED_L,TRADE_CANCELED,LOCKED,PAUSE";
            // string   page = "1";
            // string   page_size = "10";
            // string   token="fc8e04d5-2d8d-458c-b188-d55852af94f3";
            page = Math.Max(page,1);
            page_size = Math.Min(page_size,100);

            var m = JingDHaddle.jdOrderDownload(start_date, end_date, order_state, page, page_size, token);
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/downByIds")]
        public ResponseResult downByIds(List<string> order_id,string token,string optional_fields = "",string order_state="")
        {                  
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(order_id.Count == 0){
                m.s = -5004;
            }else{
                m = JingDHaddle.orderDownByIds(order_id, optional_fields, order_state, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 退款审核单列表查询
        [HttpGetAttribute("/core/Api/JdOrder/RefundList")]
        public ResponseResult RefundList(string token,string ids="", string status="", string orderId="", string buyerId="", string buyerName="", string applyTimeStart="", string applyTimeEnd="", string checkTimeStart="", string checkTimeEnd="", int pageIndex=1, int pageSize=50){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                pageIndex = Math.Max(pageIndex,1);
                pageSize = Math.Min(pageSize,50);
                m = JingDHaddle.jdRefundList( ids,  status,  orderId,  buyerId,  buyerName,  applyTimeStart,  applyTimeEnd,  checkTimeStart,  checkTimeEnd,  pageIndex,  pageSize, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/JdOrder/RefundById")]
        public ResponseResult RefundById(string token,int id=0){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(id == 0){
                m.s = -5004;
            }else{
                m = JingDHaddle.jdRefundById(id,token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
        
        #region 商家审核退款单
        [HttpGetAttribute("/core/Api/JdOrder/ReplyRefund")]
        public ResponseResult ReplyRefund(string token,int status, string id , string checkUserName,string outWareStatus="",string remark="",string rejectType=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(id)){
                m.s = -5004;
            }else if(string.IsNullOrEmpty(checkUserName)){
                m.s = -5005;
            }else if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if (status != 9){
                if (string.IsNullOrEmpty(remark))                          
                    m.s = -5002;                         
            }
            else if (status == 2)
            {   
                if(string.IsNullOrEmpty(rejectType))             
                    m.s = -5003;
            }else{
                m = JingDHaddle.jdReplyRefund(status, id ,checkUserName,remark,rejectType, outWareStatus, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 待处理退款单数查询
        [HttpGetAttribute("/core/Api/JdOrder/GetWaitRefundNum")]
        public ResponseResult GetWaitRefundNum(string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = JingDHaddle.jdGetWaitRefundNum(token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region  增加SKU信息（与正式环境相连，未做测试）
        [HttpGetAttribute("/core/Api/JdOrder/SkuAdd")]
        public ResponseResult SkuAdd(string token,string ware_id,string attributes,string jd_price,int stock_num=0,string trade_no="",string outer_id=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(ware_id)){
                m.s = -5004;
            }else if(string.IsNullOrEmpty(attributes)){
                m.s = -5006;
            }else if(string.IsNullOrEmpty(jd_price)){
                m.s = -5007;
            }else{
                m = JingDHaddle.jdSkuAdd(ware_id, attributes, jd_price, stock_num, trade_no, outer_id, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 修改SKU库存信息
        [HttpGetAttribute("/core/Api/JdOrder/SkuUpdate")]
        public ResponseResult SkuUpdate(string token,string sku_id,string ware_id,string jd_price="",string stock_num="",string trade_no="",string outer_id=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(sku_id)&&string.IsNullOrEmpty(ware_id)){
                m.s = -5008;
            }else{
                m = JingDHaddle.jdSkuUpdate(sku_id,ware_id,outer_id,jd_price,stock_num,trade_no,token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
        
        #region 删除Sku信息
        [HttpGetAttribute("/core/Api/JdOrder/SkuDelete")]
        public ResponseResult SkuDelete(string sku_id,string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(sku_id)){
                m.s = -5004;
            }else{
                m = JingDHaddle.jdSkuDelete(sku_id,token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 根据外部ID获取商品SKU
        [HttpGetAttribute("/core/Api/JdOrder/CustomGet")]
        public ResponseResult CustomGet(string outer_id,string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(outer_id)){
                m.s = -5004;
            }else{
                m = JingDHaddle.jdCustomGet(outer_id,token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/JdOrder/SkusGet")]
        public ResponseResult SkusGet(string ware_ids,string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
               //m = JingDHaddle.jdSkusGet();
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