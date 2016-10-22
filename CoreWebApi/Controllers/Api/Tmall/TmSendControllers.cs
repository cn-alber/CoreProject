using CoreDate.CoreApi;
using CoreModels;
using CoreModels.XyApi.Tmall;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Api.Tmall{    
    //天猫发货
    public class TmSendControllers : ControllBase
    {

        #region 在线订单发货处理（支持货到付款）
        [HttpGetAttribute("/core/Api/TmSend/onlineSend")]
        public ResponseResult onlineSend(string token="",string sub_tid="",string tid="",string is_split="",string out_sid="",string company_code="",string sender_id="",
                                        string cancel_id="",string feature="",string seller_ip=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(tid)){
                m.s = -5022;
            }else if(string.IsNullOrEmpty(company_code)){
                m.s = -5023;
            }else{
                m = TmallHaddle.onlineSend(token,sub_tid,tid,is_split, out_sid, company_code, sender_id,
                                         cancel_id, feature, seller_ip);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 取消物流订单接口
        [HttpGetAttribute("/core/Api/TmSend/onlineCancel")]
        public ResponseResult onlineCancel(string token="",string tid=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(tid)){
                m.s = -5022;
            }else{
                m = TmallHaddle.onlineCancel(token,tid);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 确认发货通知接口
        //  仅使用taobao.logistics.online.send 发货时，未输入运单号的情况下，需要使用该接口确认发货
        [HttpPostAttribute("/core/Api/TmSend/onlineConfirm")]
        public ResponseResult onlineConfirm([FromBodyAttribute]JObject obj){
            var oncancle = Newtonsoft.Json.JsonConvert.DeserializeObject<onlineConfirm>(obj.ToString());
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(oncancle.token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(oncancle.tid)){
                m.s = -5022;
            }else if(string.IsNullOrEmpty(oncancle.out_sid)){
                m.s = -5010;
            }else{
                m = TmallHaddle.onlineConfirm(oncancle.token, oncancle.tid, oncancle.sub_tid, oncancle.is_split,oncancle.out_sid,oncancle.seller_ip);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 自己联系物流（线下物流）发货
        [HttpGetAttribute("/core/Api/TmSend/offlineSend")]
        public ResponseResult offlineSend([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject<offlineSend>(obj.ToString());
            if(string.IsNullOrEmpty(o.token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(o.company_code)){
                m.s = -5023;
            }else{
                m = TmallHaddle.offlineSend(o.token,o.sub_tid,o.tid,o.is_split,o.out_sid,o.company_code,o.sender_id,
                                        o.cancel_id,o.feature,o.seller_ip);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 虚拟发货
        [HttpGetAttribute("/core/Api/TmSend/dummySend")]
        public ResponseResult dummySend([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject<dummySend>(obj.ToString());
            if(string.IsNullOrEmpty(o.token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(o.tid)){
                m.s = -5022;
            }else{
                m = TmallHaddle.dummySend(o.token,o.tid,o.feature,o.seller_ip);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

                #region
        [HttpGetAttribute("/core/Api/TmSend/orderCreateAndSend")]
        public ResponseResult orderCreateAndSend([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject<orderCreateAndSendRequest>(obj.ToString());
            o.order_source = "30";//订单来源，固定值
            o.order_type = "30";//订单类型，固定值
            o.logis_type = "2";//物流订单物流类型

            if(string.IsNullOrEmpty(o.token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(o.company_id)){
                m.s = -5023;
            }else if(string.IsNullOrEmpty(o.user_id)){
                m.s = -5024;
            }else if(string.IsNullOrEmpty(o.trade_id)){
                m.s = -5024;
            }else if(string.IsNullOrEmpty(o.s_name)){
                m.s = -5026;
            }else if(string.IsNullOrEmpty(o.s_area_id)){
                m.s = -5027;
            }else if(string.IsNullOrEmpty(o.s_address)){
                m.s = -5028;
            }else if(string.IsNullOrEmpty(o.s_zip_code)){
                m.s = -5029;
            }else if(string.IsNullOrEmpty(o.r_name)){
                m.s = -5030;
            }else if(string.IsNullOrEmpty(o.r_area_id)){
                m.s = -5031;
            }else if(string.IsNullOrEmpty(o.r_address)){
                m.s = -5032;
            }else if(string.IsNullOrEmpty(o.r_zip_code)){
                m.s = -5033;
            }else if(string.IsNullOrEmpty(o.r_prov_name)){
                m.s = -5034;
            }else if(string.IsNullOrEmpty(o.r_city_name)){
                m.s = -5035;
            }else if(o.item_json_string.Count == 0){
                m.s = -5036;
            }else{
                m = TmallHaddle.orderCreateAndSend(o);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion




        #region
        [HttpGetAttribute("/core/Api/TmSend/")]
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