using System.Collections.Generic;
using CoreData.CoreApi;
using CoreModels;
using CoreModels.XyApi.Tmall;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Api.Tmall{   
    [AllowAnonymous]
    public class TmCaiNiaoControllers : ControllBase{
        
        #region  菜鸟电子面单的云打印申请电子面单号
        [HttpPostAttribute("/core/Api/TmCaiNiao/WaybillIIGet")]
        public ResponseResult WaybillIIGet([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            if(obj["ApplyNew"]!=null && obj["token"]!=null){
                var waybill = Newtonsoft.Json.JsonConvert.DeserializeObject<WaybillCloudPrintApplyNewRequest>(obj["ApplyNew"].ToString());                       
                string token = obj["token"].ToString();
                m = CaiNiaoHaddle.WaybillIIGet(waybill);
            }else{
                m.s = -5048;
            }
                        
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 通过面单号查询电子面单信息
        [HttpPostAttribute("/core/Api/TmCaiNiao/waybillIIQueryByCode")]
        public ResponseResult waybillIIQueryByCode([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);
            if(obj["DetailList"]!=null &&　obj["token"]!=null){
                var waybill = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WaybillDetailQueryByWaybillCodeRequest>>(obj["DetailList"].ToString());
                m = CaiNiaoHaddle.waybillIIQueryByCode(waybill);
            }else{
                m.s = -5048;
            }            
                        
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 更新电子面单
        [HttpPostAttribute("/core/Api/TmCaiNiao/waybillIIUpdate")]
        public ResponseResult waybillIIUpdate([FromBodyAttribute]JObject obj){
            var m = new DataResult(1,null);            
            if(obj["Update"]!=null &&　obj["token"]!=null){
                var waybill = Newtonsoft.Json.JsonConvert.DeserializeObject<WaybillCloudPrintUpdateRequest>(obj["Update"].ToString());
                m = CaiNiaoHaddle.waybillIIUpdate(waybill);       
            }else{
                m.s = -5048;
            }     
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        
        #region 商家查询物流商产品类型接口
        [HttpGetAttribute("/core/Api/TmCaiNiao/waybillIIProduct")]
        public ResponseResult waybillIIProduct(string cp_code="",string token=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(cp_code)){
                m.s = -5049;
            }else{
                m = CaiNiaoHaddle.waybillIIProduct(cp_code,token);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        
        #region 查询面单服务订购及面单使用情况
        [HttpGetAttribute("/core/Api/TmCaiNiao/waybillIISearch")]
        public ResponseResult waybillIISearch(string cp_code="",string token=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(cp_code)){
                m.s = -5049;
            }else{
                m = CaiNiaoHaddle.waybillIISearch(cp_code,token);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        #region 商家取消获取的电子面单号
        [HttpGetAttribute("/core/Api/TmCaiNiao/waybillIICancel")]
        public ResponseResult waybillIICancel(string cp_code="",string waybill_code="",string token=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(cp_code)){
                m.s = -5049;
            }else if(string.IsNullOrEmpty(waybill_code)){
                m.s = -5050;
            }else{
                m = CaiNiaoHaddle.waybillIICancel(cp_code,waybill_code,token);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        #region 获取所有的菜鸟标准电子面单模板
        [HttpGetAttribute("/core/Api/TmCaiNiao/cloudTempGet")]
        public ResponseResult cloudTempGet(string token=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = CaiNiaoHaddle.cloudTempGet(token);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        #region 获取所有的菜鸟标准电子面单模板
        [HttpGetAttribute("/core/Api/TmCaiNiao/cloudMyTempGet")]
        public ResponseResult cloudMyTempGet(string token=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = CaiNiaoHaddle.cloudMyTempGet(token);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        #region 获取所有的菜鸟标准电子面单模板
        [HttpGetAttribute("/core/Api/TmCaiNiao/cloudCustomGet")]
        public ResponseResult cloudCustomGet(string token="",int template_id=0){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(template_id == 0){
                m.s = -5051;
            }else{
                m = CaiNiaoHaddle.cloudCustomGet(token,template_id.ToString());
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        #region 获取商家使用的标准模板
        [HttpGetAttribute("/core/Api/TmCaiNiao/cloudIsvTempGet")]
        public ResponseResult cloudIsvTempGet(string token=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = CaiNiaoHaddle.cloudIsvTempGet(token);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        #region 云打印模板迁移接口
        [HttpGetAttribute("/core/Api/TmCaiNiao/cloudTempMigrate")]
        public ResponseResult cloudTempMigrate(string token="",int tempalte_id=0,string custom_area_name="",string custom_area_content=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = CaiNiaoHaddle.cloudTempMigrate(token,tempalte_id.ToString(),custom_area_name,custom_area_content);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

         #region 客户端更新回调
        [HttpGetAttribute("/core/Api/TmCaiNiao/clientUpdate")]
        public ResponseResult clientUpdate(string token="",string mac="",string version="",string update_typa_name=""){
            var m = new DataResult(1,null);                                
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(mac)){
                m.s = -5052;
            }else if(string.IsNullOrEmpty(version)){
                m.s=-5053;
            }else if(string.IsNullOrEmpty(update_typa_name)){
                m.s=-5054;
            }else{ 
                m = CaiNiaoHaddle.clientupdate( token, mac, version, update_typa_name);
            }                       
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion 

        
    }

}