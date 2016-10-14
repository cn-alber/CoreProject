using System;
using CoreDate.CoreApi;
using CoreModels;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.Tmall{    
    //天猫退款
    public class TmRefundControllers : ControllBase
    {

        public static string REFUND_APPLY_GET = "refund_id, tid, title, buyer_nick, seller_nick, total_fee, status, created, refund_fee";
        public static string REFUND_RECEIVE_GET = "refund_id, tid, title, buyer_nick, seller_nick, total_fee, status, created, refund_fee, oid, good_status, company_name, sid, payment, reason, desc, has_good_return, modified, order_status,refund_phase";

        public static string RFUND_ONE ="refund_id, alipay_no, tid, oid, buyer_nick, seller_nick, total_fee, status, created, refund_fee, good_status, has_good_return, payment, reason, desc, num_iid, title, price, num, good_return_time, company_name, sid, address, shipping_type, refund_remind_timeout, refund_phase, refund_version, operation_contraint, attribute, outer_id, sku"; 
        
        public static string REFUND_MESSAGE = "id, refund_id,owner_id,owner_nick,content,pic_urls,created,message_type,refund_phase,owner_role";
        #region 查询买家申请的退款列表
        [HttpGetAttribute("/core/Api/TmRefund/ApplyGet")]
        public ResponseResult ApplyGet(string token="",string fields="",string status="",string seller_nick="",string type="",int page=1,int pageSize=100){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = REFUND_APPLY_GET;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                page = Math.Max(page,1);
                pageSize = Math.Min(pageSize,100);
                m = TmallHaddle.refundsApplyGet(token,fields,status,seller_nick, type, page, pageSize);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 
        
        #region 查询卖家收到的退款列表
        [HttpGetAttribute("/core/Api/TmRefund/ReceiveGet")]
        public ResponseResult ReceiveGet(string token="",string fields="",string status="",string buyer_nick="",string type="",string start_modified="",string end_modified="",int page=1,int pageSize=100){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = REFUND_RECEIVE_GET;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                page = Math.Max(page,1);
                pageSize = Math.Min(pageSize,100);
                m = TmallHaddle.refundsReceiveGet(token,fields,status,buyer_nick,type,start_modified,end_modified,page,pageSize);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 获取单笔退款详情
        [HttpGetAttribute("/core/Api/TmRefund/OneGet")]
        public ResponseResult OneGet(string token,string fields,string refund_id){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = RFUND_ONE;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(refund_id)){  
                m.s = -5037;
            }else{
                m = TmallHaddle.refundOneGet(token,fields,refund_id);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 查询退款留言/凭证列表
        [HttpGetAttribute("/core/Api/TmRefund/MessagesGet")]
        public ResponseResult MessagesGet(string token="",string fields="" ,string refund_id="",int page=1,int pageSize=100,string refund_phase=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = REFUND_MESSAGE;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(refund_id)){
                m.s = -5037;
            }else if(string.IsNullOrEmpty(refund_phase)){
                m.s = -5038;
            }else{
                page = Math.Max(page,1);
                pageSize = Math.Min(pageSize,100);
                m = TmallHaddle.refundMessagesGet(token,fields ,refund_id,page,pageSize,refund_phase);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion


        
        
        
        
        
        
        #region
        [HttpGetAttribute("/core/Api/TmRefund/")]
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