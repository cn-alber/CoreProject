using System;
using CoreDate.CoreApi;
using CoreModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.Tmall{    
    //天猫订单（order）名称为交易(trade)
    [AllowAnonymous]
    public class TmTradesControllers : ControllBase
    {
        //订单需要返回的字段列表
        public static string OREDER_FIELDS = "seller_nick,buyer_nick,title,type,created,sid,tid,seller_rate,buyer_rate,status,payment,discount_fee,adjust_fee,post_fee,total_fee,pay_time,end_time,modified,consign_time,buyer_obtain_point_fee,point_fee,real_point_fee,received_payment,commission_fee,pic_path,num_iid,num_iid,num,price,cod_fee,cod_status,shipping_type,receiver_name,receiver_state,receiver_city,receiver_district,receiver_address,receiver_zip,receiver_mobile,receiver_phone,orders.title,orders.pic_path,orders.price,orders.num,orders.iid,orders.num_iid,orders.sku_id,orders.refund_status,orders.status,orders.oid,orders.total_fee,orders.payment,orders.discount_fee,orders.adjust_fee,orders.sku_properties_name,orders.item_meal_name,orders.buyer_rate,orders.seller_rate,orders.outer_iid,orders.outer_sku_id,orders.refund_id,orders.seller_type";

        public static string ITEM_PROPS = @"pid,name,must,multi,prop_values,features,is_color_prop,is_sale_prop,is_key_prop,is_enum_prop,is_item_prop, features,status,sort_order,
                                            is_allow_alias,is_input_prop,taosir_do,is_material,material_do,expr_el_list"; 


        #region 天猫订单下载
        [HttpGetAttribute("/core/Api/TmTrades/soldget")]
        public ResponseResult soldget(string fields = "",string start_created="", string end_created="", string status="",string buyer_nick="",string type="", string ext_type="",string rate_status="",string tag="",int page=1, int pageSize=100, bool use_has_next = false,string token="")
        {                       
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = OREDER_FIELDS;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000; 
            }else{
                page = Math.Max(page,1);
                pageSize = Math.Min(pageSize,100);
                m = TmallHaddle.OrderDownload( fields, start_created,  end_created,  status, buyer_nick,type,  ext_type, rate_status, tag, page,  pageSize,  use_has_next, token);
            }            
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/TmTrades/oneget")]
        public ResponseResult oneget(string fields="",string tids="",string token=""){                    
            var m = new DataResult(1,null);
            if(string.IsNullOrEmpty(fields)){
                fields = OREDER_FIELDS;
            }
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }if(string.IsNullOrEmpty(tids)){
                m.s = -5022;
            }else{
                m = TmallHaddle.getbytid(fields,tids,token);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion


        #region
        [HttpGetAttribute("/core/Api/TmTrades/itempropsGet")]
        public ResponseResult itempropsGet(string token,string cid){
            string fields= ITEM_PROPS;
            var m = new DataResult(1,null);
            //string coid =GetCoid();
            if(string.IsNullOrEmpty(token)){
                m.s = -5000;
            }else{
                m = TmallHaddle.itempropsGet(token,fields,cid);
            }
            return CoreResult.NewResponse(m.s, m.d, "Api");
        }
        #endregion

        #region
        [HttpGetAttribute("/core/Api/TmTrades/")]
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