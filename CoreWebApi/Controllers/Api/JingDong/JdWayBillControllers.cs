using System;
using CoreDate.CoreApi;
using CoreModels;
using CoreModels.XyApi.JingDong;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Api.JingDong{    
    public class JdWayBillControllers : ControllBase
    {
        #region
        [HttpGetAttribute("/core/Api/JdWayBill/WayBillCodeget")]
        public ResponseResult WayBillCodeget(string customerCode ,string orderType,string token,int preNum=1){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(customerCode)){
                m.s = -5011;
            }else{
                preNum = preNum>100 ? 100 : preNum;
                preNum = preNum<0? 1: preNum;
                m = JingDHaddle.jdWayBillCodeget(preNum,customerCode , orderType, token);
            }

            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
        
        #region 是否可以京配
        [HttpGetAttribute("/core/Api/JdWayBill/RangeCheck")]
        public ResponseResult RangeCheck(string customerCode,string orderId,string wareHouseCode, string receiveAddress,string token,int goodsType=1){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(customerCode)){
                m.s = -5011;
            }else if(string.IsNullOrEmpty(orderId)){
                m.s = -5009;
            }else if(string.IsNullOrEmpty(wareHouseCode)){
                m.s = -5012;
            }else if(string.IsNullOrEmpty(receiveAddress)){
                m.s = -5013;
            }else{
                m = JingDHaddle.jdRangeCheck(customerCode,orderId,goodsType,wareHouseCode, receiveAddress,token);
            }


            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 京东快递提交运单信息接口（青龙接单接口）
        [HttpPostAttribute("/core/Api/JdWayBill/WaybillSend")]
        public ResponseResult WaybillSend([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            var w = Newtonsoft.Json.JsonConvert.DeserializeObject<WaybillSend>(obj.ToString());
            if(string.IsNullOrEmpty(w.token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(w.deliveryId)){
                m.s = -5010;
            }else if(string.IsNullOrEmpty(w.salePlat)){
                m.s = -5014;
            }else if(string.IsNullOrEmpty(w.customerCode)){
                m.s = -5011;
            }else if(string.IsNullOrEmpty(w.orderId)){
                m.s = -5015;
            }else if(w.salePlat == "0010001" && w.thrOrderIds.Count == 0){
                m.s = -5009;
            }else if(string.IsNullOrEmpty(w.senderName)||w.senderName.Length>25){
                m.s = -5016;
            }else if(string.IsNullOrEmpty(w.senderAddress)||w.senderAddress.Length>128){
                m.s = -5017;
            }else if(string.IsNullOrEmpty(w.receiveName)||w.receiveName.Length>25){
                m.s = -5018;
            }else if(string.IsNullOrEmpty(w.receiveAddress)||w.receiveAddress.Length>128){
                m.s = -5019;
            }else if(w.packageCount<0 || w.packageCount>1000){
                m.s = -5020;
            }else{
                w.weight = Math.Round(w.weight,2);
                w.vloumn = Math.Round(w.vloumn,2);
                m = JingDHaddle.jdWaybillSend(w.deliveryId,w.salePlat,w.customerCode,w.orderId, w.thrOrderIds,
                    w.senderName,w.senderAddress,w.receiveName,w.receiveAddress,w.packageCount,w.weight, w.vloumn,w.token); 
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/JdWayBill/OrderPrint")]
        public ResponseResult OrderPrint(string customerCode, string deliveryId, string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(customerCode)){
                m.s = -5011;
            }else if(string.IsNullOrEmpty(deliveryId)){
                m.s = -5010;
            }else{
                m = JingDHaddle.jdOrderPrint(customerCode, deliveryId, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 查询京东快递物流跟踪信息 
        [HttpGetAttribute("/core/Api/JdWayBill/TraceGet")]
        public ResponseResult TraceGet(string waybillCode,string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(waybillCode)){
                m.s = -5010;
            }else{
                m =  JingDHaddle.jdTraceGet(waybillCode,token); 
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 修改京东快递包裹数
        [HttpGetAttribute("/core/Api/JdWayBill/PackageUpdate")]
        public ResponseResult PackageUpdate(string customerCode="",string deliveryId="",int packageCount=0,string token=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(customerCode)){
                m.s = -5011;
            }else if(string.IsNullOrEmpty(deliveryId)){
                m.s = -5010;
            }else if(packageCount<0 || packageCount>1000){
                m.s = -5020;
            }else{
                m = JingDHaddle.jdPackageUpdate(customerCode,deliveryId,packageCount,token);    
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/JdWayBill/OrderIntercept")]
        public ResponseResult OrderIntercept(string vendorCode="",string deliveryId="",string interceptReason="",string token=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(vendorCode)){
                m.s = -5011;
            }else if(string.IsNullOrEmpty(deliveryId)){
                m.s = -5010;
            }else if(string.IsNullOrEmpty(interceptReason)){
                m.s = -5021; 
            }else{
                m = JingDHaddle.jdOrderIntercept(vendorCode,deliveryId,interceptReason,token);
            }


            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion


    }
}