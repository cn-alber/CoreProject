using CoreDate.CoreApi;
using CoreModels;
using Microsoft.AspNetCore.Mvc;


namespace CoreWebApi.Api.JingDong{    
    public class JdSupplierControllers : ControllBase
    {
        #region 厂商直送出库
        [HttpGetAttribute("/core/Api/JdOrder/DpsOutbound")]
        public ResponseResult DpsOutbound(string token,string customOrderId,string memoByVendor="",string isJdexpress=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(customOrderId)){
                m.s = -5004;
            }else{
                m = JingDHaddle.jdDpsOutbound(customOrderId,memoByVendor,isJdexpress,token);
            }

            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 厂商直送发货
        [HttpGetAttribute("/core/Api/JdOrder/DpsDelivery")]
        public ResponseResult DpsDelivery(string token,string customOrderId,string carrierId,string carrierBusinessName,string shipNo,string estimateDate){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = JingDHaddle.jdDpsDelivery(customOrderId,carrierId,carrierBusinessName,shipNo,estimateDate,token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 订单发货
        [HttpGetAttribute("/core/Api/JdOrder/EptDeliveryOrder")]
        public ResponseResult EptDeliveryOrder(string orderId,string expressNo,string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(orderId)){
                m.s = -5009;
            }else if(string.IsNullOrEmpty(expressNo)){
                m.s = -5010;
            }else{
                m = JingDHaddle.jdEptDeliveryOrder(orderId, expressNo,token);
            }

            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion



    }
}