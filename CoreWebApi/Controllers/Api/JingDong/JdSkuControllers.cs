using System;
using CoreData.CoreApi;
using CoreModels;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{    
    public class JdSkuControllers : ControllBase
    {
        #region  增加SKU信息（与正式环境相连，未做测试）
        [HttpGetAttribute("/core/Api/JdSku/SkuAdd")]
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
        [HttpGetAttribute("/core/Api/JdSku/SkuUpdate")]
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
        [HttpGetAttribute("/core/Api/JdSku/SkuDelete")]
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
        [HttpGetAttribute("/core/Api/JdSku/CustomGet")]
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
        [HttpGetAttribute("/core/Api/JdSku/SkusGet")]
        public ResponseResult SkusGet(string ware_ids,string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(ware_ids)){
                m.s = -5004;
            }else{
               m = JingDHaddle.jdSkusGet(ware_ids,token);
            }


            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 获取单个Sku信息
        [HttpGetAttribute("/core/Api/JdSku/SkuGet")]
        public ResponseResult SkuGet(string sku_id,string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(sku_id)){
                m.s = -5004;
            }else{
                m = JingDHaddle.jdSkuGet(sku_id,token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/JdSku/FindSkuById")]
        public ResponseResult FindSkuById(string skuId, string token){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else if(string.IsNullOrEmpty(skuId)){
                m.s = -5004;
            }else{
                m = JingDHaddle.jdFindSkuById(skuId, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 获取商品上架的商品信息
        [HttpGetAttribute("/core/Api/JdSku/ListingGet")]
        public ResponseResult ListingGet(string end_modified,string start_modified,string token,string cid="",int page=1,int page_size=100){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                page = Math.Max(page,1);
                page_size = Math.Min(page_size,100);
                m = JingDHaddle.jdListingGet( cid, page, page_size, end_modified, start_modified, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region 获取商品下架的商品信息
        [HttpGetAttribute("/core/Api/JdSku/DelistingGet")]
        public ResponseResult DelistingGet(string end_modified, string start_modified,string token,string cid="", int page=1, int page_size=100){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                page = Math.Max(page,1);
                page_size = Math.Min(page_size,100);
                m = JingDHaddle.jdDelistingGet(cid,page, page_size, end_modified, start_modified, token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region Sku搜索服务
        [HttpGetAttribute("/core/Api/JdSku/SearchSkuList")]
        public ResponseResult SearchSkuList(string token,string skuStatuValue="",string startCreatedTime="", string endCreatedTime="",int pageNo=1,string field=""){
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = JingDHaddle.jdSearchSkuList(token,skuStatuValue,startCreatedTime, endCreatedTime,pageNo,field);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

    }
}