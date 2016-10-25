using System;
using CoreData.CoreApi;
using CoreModels;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{    
    public class JdRefundControllers : ControllBase
    {
        #region 退款审核单列表查询
        [HttpGetAttribute("/core/Api/JdRefund/RefundList")]
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
        [HttpGetAttribute("/core/Api/JdRefund/RefundById")]
        public ResponseResult RefundById(string token,long id=0){
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
        [HttpGetAttribute("/core/Api/JdRefund/ReplyRefund")]
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
        [HttpGetAttribute("/core/Api/JdRefund/GetWaitRefundNum")]
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



    }
}