using CoreDate.CoreApi;
using CoreModels;
using CoreModels.XyApi.Tmall;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Api.Tmall{    
    //天猫发货
    public class TmSkuControllers : ControllBase
    {
        public static string SKU_GET = "sku_id ,iid,num_iid ,properties ,quantity ,price,created ,modified ,status,extra_id,memo,properties_name,sku_spec_id,with_hold_quantity	,sku_delivery_time,change_prop,outer_id ,barcode";

        #region 添加SKU
        [HttpPostAttribute("/core/Api/TmSku/Add")]
        public ResponseResult add([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            var sku = Newtonsoft.Json.JsonConvert.DeserializeObject<skuAddRequest>(obj.ToString());
            if(string.IsNullOrEmpty(sku.token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(sku.num_iid)){
                m.s = -5039;
            }else if(string.IsNullOrEmpty(sku.properties)){
                m.s = -5040;
            }else if(string.IsNullOrEmpty(sku.quantity)){
                m.s = -5041;
            }else if(string.IsNullOrEmpty(sku.price)){
                m.s = -5042;
            }else{
                m = TmallHaddle.itemSkuAdd(sku);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
        
        #region 获取SKU
        [HttpGetAttribute("/core/Api/TmSku/Get")]
        public ResponseResult SkuGet(string token="",string fields="", string sku_id="",string num_iid="",string nick=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = SKU_GET;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(sku_id)){
                m.s = -5043;
            }else if(string.IsNullOrEmpty(num_iid)&&string.IsNullOrEmpty(nick)){
                m.s = -5044;
            }else{
                m = TmallHaddle.itemSkuGet(token,fields, sku_id,num_iid,nick);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
        
        #region
        [HttpPostAttribute("/core/Api/TmSku/Update")]
        public ResponseResult Update([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            var sku = Newtonsoft.Json.JsonConvert.DeserializeObject<skuUpdateRequest>(obj.ToString());
            if(string.IsNullOrEmpty(sku.token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(sku.num_iid)){
                m.s = -5045;
            }else if(string.IsNullOrEmpty(sku.properties)){
                m.s = -5046;
            }else{
                m = TmallHaddle.itemSkuUpdate(sku);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
        
        #region
        [HttpGetAttribute("/core/Api/TmSku/SkusGet")]
        public ResponseResult SkusGet(string token,string fields,string num_iids){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = SKU_GET;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(num_iids)){
                m.s = -5047;
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion
                    
        #region
        [HttpGetAttribute("/core/Api/TmSku/")]
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