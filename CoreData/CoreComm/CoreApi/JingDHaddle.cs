using System;
using System.Collections.Generic;
using CoreData;
//using CoreData.CoreComm;
using CoreModels;
using CoreModels.XyComm;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CoreData.CoreApi
{
    
    public  class JingDHaddle{

        private static string SERVER_URL = "https://api.jd.com/routerjson";
        public  static string ORDER_STATE = "WAIT_SELLER_STOCK_OUT,WAIT_GOODS_RECEIVE_CONFIRM,WAIT_SELLER_DELIVERY,FINISHED_L,TRADE_CANCELED,LOCKED,PAUSE";                                                        
        private static IDictionary<string, string> jdparam = new Dictionary<string, string>{
            {"app_key", "7888CE9C0F3AAD424FEE8EEAAC99E10E"},
            {"sign", "123"},
            {"timestamp", System.DateTime.Now.AddMinutes(6).ToString()},
            {"v", "2.0"}
        };                                                 

        public static void cleanParam(){
            jdparam.Remove("method");
            jdparam.Remove("access_token");
            jdparam.Remove("360buy_param_json");
        }
        
        public static DataResult getAipLog(string shopid,string coid)
        {
            var res = new DataResult(1, null);  
            //ShopHaddle.ShopQuery(coid,shopid);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {                    
                    string sql ="SELECT job_id,enabled , shop_id, api_name,api_key ,api_interval ,run_eof ,run_times ,"+
                                "(run_total+err_total) as total ,run_total ,err_total ,err_timestamp , err_message FROM api_job WHERE shop_id="+shopid;
                    var data = conn.Query<apilog>(sql).AsList();
                    res.d = data;
                }
                catch (Exception e)
                {
                    res.s = -1;
                    res.d = e.Message;
                    conn.Dispose();
                }
            }
            return res;
        }



        /// <summary>
		/// 获取token
		/// </summary>
        public static DataResult GetToken(string url, string grant_type, string code, string redirect_uri, string client_id, string client_secret){
            var result = new DataResult(1,null);            
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", grant_type);
            parameters.Add("code", code);
            parameters.Add("redirect_uri", redirect_uri);
            parameters.Add("client_id", client_id);
            parameters.Add("client_secret", client_secret);
            var obj = new {
                grant_type = grant_type,
                code = code,
                redirect_uri = redirect_uri,
                client_id = client_id,
                client_secret = client_secret
            };
            var response = JsonResponse.CreatePostHttpResponse(url, parameters);
            result.d = JsonConvert.DeserializeObject(response.Result.ToString().Replace("\"","\'")+"}");//JsonConvert.DeserializeObject(response.Result);            
            return result;
        }
        
        #region Order 订单
        public static DataResult jdOrderDownload(string start_date, string end_date, int page, int page_size, string token, string order_state=""){
            var result = new DataResult(1,null);
            if(string.IsNullOrEmpty(order_state))
                order_state = ORDER_STATE; 
            try{              
                jdparam.Add("method", "360buy.order.search");
                jdparam.Add("access_token", token);
                jdparam.Add("360buy_param_json", "{\"start_date\":\"" + start_date + "\",\"end_date\":\"" + end_date + "\",\"order_state\":\"" + order_state + "\",\"page\":\"" + page + "\",\"page_size\":\"" + page_size + "\" }");
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                                
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.order_search_response.order_search;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        public static DataResult orderDownByIds(List<string> order_ids,string optional_fields,string token,string order_state=""){
            var result = new DataResult(1,null);
            if(string.IsNullOrEmpty(order_state))
                order_state = ORDER_STATE;
            try{
                List<dynamic> orderinfolist = new List<dynamic>();                
                jdparam.Add("method", "360buy.order.get");
                jdparam.Add("access_token", token);
                foreach(string order_id in order_ids){
                    jdparam.Add("360buy_param_json", "{\"order_id\":\"" + order_id + "\",\"optional_fields\":\"" + optional_fields + "\",\"order_state\":\"" + order_state + "\"}");                
                    var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);
                    var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                    
                    if(response.Result.ToString().IndexOf("error") > 0){
                        result.s = -1;
                        result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                        break;
                    }else{
                        orderinfolist.Add(res.order_get_response.order.orderInfo);                        
                    }
                    jdparam.Remove("360buy_param_json");
                }
                if(result.s == 1)
                    result.d = new {
                        order_info_list = orderinfolist,
                        order_total = orderinfolist.Count
                    };
                
            }catch(Exception ex){                
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }
            return result;
        }
        #endregion

        #region Refund 退款 
         /// <summary>
        ///  退款审核单列表查询
        /// </summary>
        /// <param name="ids">可为空，批量传入退款单id，格式为'id,id'，传入id数不能超过pageSize </param>
        /// <param name="status">可为空，退款申请单状态 0、未审核 1、审核通过2、审核不通过 3、京东财务审核通过 4、京东财务审核不通过 5、人工审核通过 6、拦截并退款 7、青龙拦截成功 8、青龙拦截失败 9、强制关单并退款。不传是查询全部状态 </param>
        /// <param name="orderId">可为空，订单id </param>
        /// <param name="buyerId">可为空，客户帐号 </param>
        /// <param name="buyerName">可为空，	客户姓名 </param>
        /// <param name="applyTimeStart">可为空，	申请日期（start）格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）</param>
        /// <param name="applyTimeEnd">可为空，申请日期（end）格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）</param>
        /// <param name="checkTimeStart">可为空，	审核日期(start) 格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07） </param>
        /// <param name="checkTimeEnd">可为空，审核日期(END) 格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）</param>
        /// <param name="pageIndex">页码(显示多少页，区间为1-1000) </param>
        /// <param name="pageSize">	每页显示多少条（区间为1-50）</param>
        public static DataResult jdRefundList(string ids, string status, string orderId, string buyerId, string buyerName, string applyTimeStart, string applyTimeEnd, string checkTimeStart, string checkTimeEnd, int pageIndex, int pageSize,string token){
            var result = new DataResult(1,null);
            try{
                jdparam.Add("method", "jingdong.pop.afs.soa.refundapply.queryPageList");                              
                jdparam.Add("access_token", token);
                jdparam.Add("360buy_param_json", "{\"ids\":\"" + ids + "\",\"status\":\"" + status + "\",\"orderId\":\"" + orderId + "\",\"buyerId\":\"" + buyerId + "\",\"buyerName\":\"" + buyerName + "\",\"applyTimeStart\":\"" + applyTimeStart + "\",\"applyTimeEnd\":\"" + applyTimeEnd + "\",\"checkTimeStart\":\"" + checkTimeStart + "\",\"checkTimeEnd\":\"" + checkTimeEnd + "\",\"pageIndex\":\"" + pageIndex + "\",\"pageSize\":\"" + pageSize + "\"  }");                
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_pop_afs_soa_refundapply_queryPageList_responce.queryResult;
                }
                
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
                    
        public static DataResult jdRefundById(long id,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.pop.afs.soa.refundapply.queryById");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"id\":\"" + id + "\"  }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{                    
                    result.d = res.jingdong_pop_afs_soa_refundapply_queryById_responce.queryResult.result;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        ///  商家审核退款单
        /// </summary>
        /// <param name="status">退款申请单状态：1审核通过 2审核不通过 6拦截并退款 9强制关单)</param>
        /// <param name="id">退款单ID</param>
        /// <param name="checkUserName">审核人 </param>
        /// <param name="remark">	审核备注,除强制关单外的其它审核状态下必填 </param>
        /// <param name="rejectType">仅在审核不通过时填写该值。1: 商品已在配送途中,无法取消,烦请谅解；2：商品已签收,无法取消,烦请谅解；3：国际站保税区订单，已报关</param>
        /// <param name="outWareStatus">实际是否已出库:1已出库 2未出</param>
        public static DataResult jdReplyRefund(int status, string id , string checkUserName,string remark,string rejectType,string outWareStatus,string token){
            var result = new DataResult(1,null);
 
            try{                             
                jdparam.Add("method", "jingdong.pop.afs.soa.refundapply.replyRefund");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"status\":\"" + status + "\",\"id\":\"" + id + "\",\"checkUserName\":\"" + checkUserName + "\",\"remark\":\"" + remark + "\",\"rejectType\":\"" + rejectType + "\" ,\"outWareStatus\":\"" + outWareStatus + "\"  }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_pop_afs_soa_refundapply_replyRefund_responce.replyResult;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 待处理退款单数查询
        /// </summary>
        /// <returns></returns>
        public static DataResult jdGetWaitRefundNum(string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.pop.afs.soa.refundapply.getWaitRefundNum");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{}");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_pop_afs_soa_refundapply_getWaitRefundNum_responce.queryResult.totalCount;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
    
        #endregion 
        
        #region  SKU
        
        /// <summary>
        /// 增加SKU信息（与正式环境相连，未做测试）
        /// </summary>
        /// <param name="ware_id">商品id</param>
        /// <param name="attributes">Sku属性</param>
        /// <param name="jd_price">京东价格</param>
        /// <param name="stock_num">库存</param>
        /// <param name="trade_no">可为空，流水号</param>
        /// <param name="outer_id">可为空，sku外部id（指商品本身的货号，对应商家后台“商家skuid”） </param>
        /// <returns></returns>
        public static DataResult jdSkuAdd(string ware_id,string attributes,string jd_price,int stock_num,string trade_no,string outer_id,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.ware.sku.add");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json","{\"ware_id\":\"" + ware_id + "\",\"attributes\":\"" + attributes + "\",\"jd_price\":\"" + jd_price + "\",\"stock_num\":\"" + stock_num + "\",\"trade_no\":\"" + trade_no + "\",\"outer_id\":\"" + outer_id + "\"  }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.ware_sku_add_response.code;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        ///  修改SKU库存信息
        /// </summary>
        /// <param name="sku_id">sku的id </param>
        /// <param name="ware_id">商品id</param>
        /// <param name="outer_id">可为空，外部id</param>
        /// <param name="jd_price">	京东价格 </param>
        /// <param name="stock_num">库存</param>
        /// <param name="trade_no">流水号</param>
        /// <returns></returns>
        public static DataResult jdSkuUpdate(string sku_id,string ware_id,string outer_id,string jd_price,string stock_num,string trade_no,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.ware.sku.update");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"sku_id\":\"" + sku_id + "\",\"ware_id\":\"" + ware_id + "\",\"outer_id\":\"" + outer_id + "\",\"jd_price\":\"" + jd_price + "\",\"stock_num\":\"" + stock_num + "\",\"trade_no\":\"" + trade_no + "\"  }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.ware_sku_update_response.code;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        ///  删除Sku信息
        /// </summary>
        public static DataResult jdSkuDelete(string sku_id,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.ware.sku.delete");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"sku_id\":\"" + sku_id + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.ware_sku_delete_response.code;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 根据外部ID获取商品SKU   
        /// </summary>
        /// <param name="outer_id">sku的外部商家ID 对应商家后台“商家SKU”字段 </param>
        /// <returns></returns>
        public static DataResult jdCustomGet(string outer_id,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.sku.custom.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"outer_id\":\"" + outer_id + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.sku_custom_get_response.sku;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 根据商品ID列表获取商品SKU信息
        /// </summary>
        /// <param name="ware_ids">sku所属商品id，必选。ware_ids个数不能超过10个</param>
        public static DataResult jdSkusGet(string ware_ids,string token) {
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.ware.skus.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"ware_ids\":\"" + ware_ids + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.ware_skus_get_response.skus;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 获取单个Sku信息
        /// </summary>
        /// <param name="sku_id"></param>
        /// <returns></returns>
        public static DataResult jdSkuGet(string sku_id,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.ware.sku.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"sku_id\":\"" + sku_id + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.ware_sku_get_response.sku;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        //根据Skuid 获取Sku信息
        public static DataResult jdFindSkuById(string skuId, string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.sku.read.findSkuById");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"skuId\":\"" + skuId + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_sku_read_findSkuById_responce.sku;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 获取商品上架的商品信息
        /// </summary>
        /// <param name="cid">可为空，类目id </param>
        /// <param name="page">分页（范围是0至999）</param>
        /// <param name="page_size">每页多少条（范围是0至100）</param>
        /// <param name="end_modified">	结束的上架修改时间(online_time) 如不输入，默认返回半年内的上架商品数据 </param>
        /// <param name="start_modified"></param>        
        public static DataResult jdListingGet(string cid,int page,int page_size,string end_modified,string start_modified,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.ware.listing.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"cid\":\"" + cid + "\",\"page\":\"" + page + "\",\"page_size\":\"" + page_size + "\",\"end_modified\":\"" + end_modified + "\",\"start_modified\":\"" + start_modified + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.ware_listing_get_response.ware_infos;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 获取商品下架的商品信息
        /// </summary>
        /// <param name="cid">可为空，类目id </param>
        /// <param name="page">分页（范围是0至999）</param>
        /// <param name="page_size">每页多少条（范围是0至100）</param>
        /// <param name="end_modified">	结束的上架修改时间(online_time) 如不输入，默认返回半年内的上架商品数据 </param>
        /// <param name="start_modified"> </param>        
        public static DataResult jdDelistingGet(string cid, int page, int page_size, string end_modified, string start_modified,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "360buy.ware.delisting.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"cid\":\"" + cid + "\",\"page\":\"" + page + "\",\"page_size\":\"" + page_size + "\",\"end_modified\":\"" + end_modified + "\",\"start_modified\":\"" + start_modified + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.ware_delisting_get_response.ware_infos;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// Sku搜索服务
        /// </summary>
        /// <param name="skuStatuValue">	SKU状态：1:上架 2:下架 4:删除  ,多选用英文逗号隔开</param>
        /// <param name="startCreatedTime">	创建时间 </param>
        /// <param name="endCreatedTime">结束时间</param>
        /// <param name="pageNo">页码</param>
        /// <returns></returns>
        public static DataResult jdSearchSkuList(string token,string skuStatuValue,string startCreatedTime, string endCreatedTime,int pageNo,string field){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.sku.read.searchSkuList");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"skuStatuValue\":\"" + skuStatuValue + "\",\"startCreatedTime\":\"" + startCreatedTime + "\",\"endCreatedTime\":\"" + endCreatedTime + "\",\"pageNo\":\"" + pageNo + "\",\"field\":\"" + field + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_sku_read_searchSkuList_responce.page;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region Supplier
        /// <summary>
        /// 厂商直送出库
        /// </summary>
        /// <param name="customOrderId">客单编号</param>
        /// <param name="memoByVendor">可为空,厂商备注 </param>
        /// <param name="isJdexpress">可为空，	是否是京东配送：1是，2</param>
        public static DataResult jdDpsOutbound(string customOrderId,string memoByVendor,string isJdexpress,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.ldop.receive.order.intercept");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"customOrderId\":\"" + customOrderId + "\",\"memoByVendor\":\"" + memoByVendor + "\",\"isJdexpress\":\"" + isJdexpress + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_dropship_dps_outbound_responce.outBoundResult.message;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 厂商直送发货
        /// </summary>
        /// <param name="customOrderId">客单编号 </param>
        /// <param name="carrierId">承运商Id 如：2100</param>
        /// <param name="carrierBusinessName">	承运商名称 如 圆通快递</param>
        /// <param name="shipNo">运单号</param>
        /// <param name="estimateDate">	预计到货时间 </param>
        /// <returns></returns>
        public static DataResult jdDpsDelivery (string customOrderId,string carrierId,string carrierBusinessName,string shipNo,string estimateDate,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.ldop.receive.order.intercept");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"customOrderId\":\"" + customOrderId + "\",\"carrierId\":\"" + carrierId + "\",\"carrierBusinessName\":\"" + carrierBusinessName + "\",\"shipNo\":\"" + shipNo + "\" ,\"estimateDate\":\"" + estimateDate + "\"}");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_dropship_dps_delivery_responce.deliverResult.message;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 订单发货
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="expressNo">运单号</param>
        public static DataResult jdEptDeliveryOrder(string orderId,string expressNo,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.ept.order.deliveryorder");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_ept_order_deliveryorder_responce.deliveryorder_result.success;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }


        #endregion

        #region WayBill
        /// <summary>
        /// 获取京东快递运单号
        /// </summary>
        /// <param name="preNum">获取运单号数量（最大100）</param>
        /// <param name="customerCode">商家编码（区分英文大小写） 此商家编码需由商家向京东快递运营人员（与商家签订京东快递合同的人）索取。 </param>
        /// <param name="orderType">可为空，运单类型。(普通外单：0，O2O外单：1)默认为0 </param>
        /// <returns></returns>
        public static DataResult jdWayBillCodeget(int preNum,string customerCode ,string orderType,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.etms.waybillcode.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"preNum\":\"" + preNum + "\",\"customerCode\":\"" + customerCode + "\",\"orderType\":\"" + orderType + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_etms_waybillcode_get_responce.resultInfo.deliveryIdList;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 是否可以京配
        /// </summary>
        /// <param name="customerCode">商家编码</param>
        /// <param name="orderId">订单号 </param>
        /// <param name="goodsType">配送业务类型（ 1:普通，3:填仓，4:特配，5:鲜活，6:控温，7:冷藏，8:冷冻，9:深冷）默认是1 </param>
        /// <param name="wareHouseCode">	发货仓 </param>
        /// <param name="receiveAddress">	收件人地址</param>
        /// <returns>jdRangeCheckModelResponseResultInfo</returns> 
        public static DataResult jdRangeCheck(string customerCode,string orderId,int goodsType,string wareHouseCode, string receiveAddress,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.etms.range.check");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"customerCode\":\"" + customerCode + "\",\"orderId\":\"" + orderId + "\",\"goodsType\":\"" + goodsType + "\",\"wareHouseCode\":\"" + wareHouseCode + "\",\"receiveAddress\":\"" + receiveAddress + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_etms_range_check_responce.resultInfo;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
/// <summary>
        /// 京东快递提交运单信息接口（青龙接单接口）
        /// </summary>
        /// <param name="deliveryId">运单号</param>
        /// <param name="salePlat">销售平台: 如为京东平台，则传入0010001。如没有以下平台，可传入0030001（即其他），数据字典如下： 
        /// 0010001 京东 0010002 天猫 0010003 苏宁 0010004 亚马逊中国 0020001 ChinaSkin 0030001 其他小型销售平台务必保证该字段的正确</param>
        /// <param name="customerCode">商家编码。雅鹿男装旗舰店为：021K5449</param>
        /// <param name="orderId">商家订单号，为商家ERP软件生成的新订单号。商家订单号未必等于平台订单号</param>
        /// <param name="thrOrderId">京东订单号  如果订单为京东平台订单，此字段必填。如果该订单为京东平台之外的订单，请为空。
        /// 货到付款订单，不支持合并发货；在线支付订单，此处可以回传多个订单号，用英文逗号分隔,总长度不要超过100。例如：7898675,7898676</param>
        /// <param name="senderName">	寄件人姓名（最大支持25个汉字）</param>
        /// <param name="senderAddress">寄件人地址（最大支持128个汉字）</param>
        /// <param name="receiveName">收件人名称（最大支持25个汉字）</param>
        /// <param name="receiveAddress">收件人地址（最大支持128个汉字）</param>
        /// <param name="packageCount">包裹数(大于0，小于1000)</param>
        /// <param name="weight">	重量(单位：kg，保留小数点后两位) 可默认为0</param>
        /// <param name="vloumn">体积(单位：cm3，保留小数点后两位) 可默认为0</param>
         public static DataResult jdWaybillSend(string deliveryId,string salePlat,string customerCode,string orderId, List<string> thrOrderIds,
            string senderName,string senderAddress,string receiveName,string receiveAddress,int packageCount,decimal weight, decimal vloumn,string token){
            var result = new DataResult(1,null);
            string thrOrderId = "";
            for(int i=0;i< thrOrderIds.Count;i++) {
                thrOrderId += thrOrderIds[i] + ",";                
            }
            try{                             
                jdparam.Add("method", "jingdong.etms.waybill.send");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"deliveryId\":\"" + deliveryId + "\" ,\"salePlat\":\"" + salePlat + "\",\"customerCode\":\"" + customerCode +"\",\"orderId\":\"" + orderId + 
                "\",\"thrOrderId\":\"" + thrOrderId + "\",\"senderName\":\"" + senderName + "\",\"senderAddress\":\"" + senderAddress + "\",\"receiveName\":\"" + receiveName + "\",\"receiveAddress\":\"" + receiveAddress + "\",\"packageCount\":\"" + packageCount + "\" ,\"weight\":\"" + weight + "\" ,\"vloumn\":\"" + vloumn + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_etms_waybill_send_responce.resultInfo;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
/// <summary>
        /// 获取京东快递运单打印
        /// </summary>
        /// <param name="customerCode">商家编码</param>
        /// <param name="deliveryId">	运单号 </param>
        /// <returns></returns>
        public static DataResult jdOrderPrint(string customerCode, string deliveryId, string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.etms.order.print");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"customerCode\":\"" + customerCode + "\",\"deliveryId\":\"" + deliveryId + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_etms_order_print_responce.response;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
/// <summary>
        /// 查询京东快递物流跟踪信息
        /// </summary>
        /// <param name="waybillCode">订单号</param>
        public static DataResult jdTraceGet(string waybillCode,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.etms.trace.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"waybillCode\":\"" + waybillCode + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_etms_trace_get_responce.trace_api_dtos;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

/// <summary>
        /// 修改京东快递包裹数
        /// </summary>
        /// <param name="customerCode">	商家编号 </param>
        /// <param name="deliveryId">	运单号</param>
        /// <param name="packageCount">	包裹数(大于0，小于1000) </param>
        public static DataResult jdPackageUpdate(string customerCode,string deliveryId,int packageCount,string token) {
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.etms.trace.get");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"customerCode\":\"" + customerCode + "\",\"packageCount\":\"" + packageCount + "\",\"deliveryId\":\"" + deliveryId + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_etms_package_update_responce.response;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

/// <summary>
        /// 接收Eport相关扩展信息 (参数必须都传)
        /// </summary>
        /// <param name="deliveryId">        	运单号	</param>
        /// <param name="customerCode">	商家编码	</param>
        /// <param name="packCategory">      	包装类型	</param>
        /// <param name="cbeCode">           	电商企业代码	</param>
        /// <param name="cbeName">    	电商企业名称	</param>
        /// <param name="senderUserCountry"> 	发货人所在国	</param>
        /// <param name="transferType">	运输方式代码	</param>
        /// <param name="transferTypeinsp">  	检验检疫运输方式代码	</param>
        /// <param name="shipNameInsp">	检验检疫运输工具名称	</param>
        /// <param name="shipCodeInsp">	检验检疫运输工具代码	</param>
        /// <param name="transferRegioninsp">	检验检疫起运国/抵运国	</param>
        /// <param name="packCategoryinsp">	检验检疫包装种类	</param>
        /// <param name="idType">	证件类型	</param>
        /// <param name="idCode">	证件编码	</param>
        /// <param name="billMode">	模式代码	</param>
        /// <param name="jcborderport">	进/出境口岸	</param>
        /// <param name="jcborderportInsp">	检验检疫进出境口岸	</param>
        /// <param name="declareport">	申报口岸	</param>
        /// <param name="applyportinsp">	检验检疫申报口岸代码	</param>
        /// <param name="batchCode">	批次号	</param>
        /// <param name="shipName">	运输工具名称	</param>
        /// <param name="tradeCountry">	贸易国别	</param>
        /// <param name="cbeCodeinsp">	电商企业检验检疫备案编号	</param>
        /// <param name="coininsp">	币制（检验检疫代码）	</param>
        /// <param name="ecpCode">	电商平台编码	</param>
        /// <param name="ecpName">	电商平台名称	</param>
        /// <param name="jcborderTime"> 	进/出境日期	</param>
        /// <param name="amount">	商品数量	</param>
        /// <param name="goodsName">  	商品名称	</param>
        /// <param name="weight">	毛重量	</param>
        /// <param name="netWeight">	净重量	</param>
        public static DataResult jdEportSend(string deliveryId, string customerCode, string packCategory, string cbeCode, string cbeName, string senderUserCountry, string transferType, string transferTypeinsp, string shipNameInsp,
                                       string shipCodeInsp, string transferRegioninsp, string packCategoryinsp, string idType, string idCode, string billMode, string jcborderport, string jcborderportInsp,
                                       string declareport, string applyportinsp, string batchCode, string shipName, string tradeCountry, string cbeCodeinsp, string coininsp, string ecpCode, string ecpName, string jcborderTime,
                                       string amount, string goodsName, string weight, string netWeight,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.ldop.receive.eport.send");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"deliveryId\":\"" + deliveryId + "\"	,\"customerCode\":\"" + customerCode + "\",\"packCategory\":\"" + packCategory + "\"	,\"cbeCode\":\"" + cbeCode + "\",\"cbeName\":\"" + cbeName + "\",\"senderUserCountry\":\"" + senderUserCountry + 
                "\",\"transferType\":\"" + transferType + "\",\"transferTypeinsp\":\"" + transferTypeinsp + "\",\"shipNameInsp\":\"" + shipNameInsp + "\",\"shipCodeInsp\":\"" + shipCodeInsp + "\",\"transferRegioninsp\":\"" + transferRegioninsp + "\",\"packCategoryinsp\":\"" + packCategoryinsp + "\",\"idType\":\"" + idType + 
                "\",\"idCode\":\"" + idCode + "\",\"billMode\":\"" + billMode + "\",\"jcborderport\":\"" + jcborderport + "\",\"jcborderportInsp\":\"" + jcborderportInsp + "\",\"declareport\":\"" + declareport + "\",\"applyportinsp\":\"" + applyportinsp + 
                "\"	,\"batchCode\":\"" + batchCode + "\",\"shipName\":\"" + shipName + "\",\"tradeCountry\":\"" + tradeCountry + "\",\"cbeCodeinsp\":\"" + cbeCodeinsp + "\",\"coininsp\":\"" + coininsp + "\",\"ecpCode\":\"" + ecpCode + 
                "\",\"ecpName\":\"" + ecpName + "\",\"jcborderTime\":\"" + jcborderTime + "\",\"amount\":\"" + amount + "\",\"goodsName\":\"" + goodsName + "\",\"weight\":\"" + weight + "\",\"netWeight\":\"" + netWeight + "\"}");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_ldop_receive_eport_send_responce.receiveextenmessagetoeport_result;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        /// <summary>
        /// 运单拦截
        /// </summary>
        /// <param name="vendorCode"></param>
        /// <param name="deliveryId"></param>
        /// <param name="interceptReason"></param>
        public static DataResult jdOrderIntercept(string vendorCode,string deliveryId,string interceptReason,string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "jingdong.ldop.receive.order.intercept");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "{\"vendorCode\":\"" + vendorCode + "\",\"deliveryId\":\"" + deliveryId + "\",\"interceptReason\":\"" + interceptReason + "\" }");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res.jingdong_ldop_receive_order_intercept_response.resultInfo;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        #endregion 

        public static DataResult demo(string token){
            var result = new DataResult(1,null);
            try{                             
                jdparam.Add("method", "");  
                jdparam.Add("access_token", token);  
                jdparam.Add("360buy_param_json", "");              
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = res.error_response.code+" : "+res.error_response.zh_desc;
                }else{
                    result.d = res;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }










    }
}