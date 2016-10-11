using System;
using System.Collections.Generic;
using CoreData;
//using CoreData.CoreComm;
using CoreModels;
using CoreModels.XyComm;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CoreDate.CoreApi
{
    
    public  class JingDHaddle{

        public static string SERVER_URL = "https://api.jd.com/routerjson";                                                        
        public static IDictionary<string, string> jdparam = new Dictionary<string, string>{
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
        public static DataResult jdOrderDownload(string start_date, string end_date, string order_state, string page, string page_size, string token){
            var result = new DataResult(1,null);
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
                    result.d = res.order_search_response.order_search.order_info_list;
                }
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }

        public static DataResult orderDownByIds(List<string> order_ids,string optional_fields,string order_state,string token){
            var result = new DataResult(1,null);
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
                    result.d = orderinfolist;
                
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
        /// <param name="status">可为空，退款申请单状态 0、未审核 1、审核通过2、审核不通过 3、京东财务审核通过
        /// 4、京东财务审核不通过 5、人工审核通过 6、拦截并退款 7、青龙拦截成功 8、青龙拦截失败 9、强制关单并退款。不传是查询全部状态 </param>
        /// <param name="orderId">可为空，订单id </param>
        /// <param name="buyerId">可为空，客户帐号 </param>
        /// <param name="buyerName">可为空，	客户姓名 </param>
        /// <param name="applyTimeStart">可为空，	申请日期（start）格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）</param>
        /// <param name="applyTimeEnd">可为空，申请日期（end）格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）</param>
        /// <param name="checkTimeStart">可为空，	审核日期(start) 格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07） </param>
        /// <param name="checkTimeEnd">可为空，审核日期(END) 格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）</param>
        /// <param name="pageIndex">页码(显示多少页，区间为1-1000) </param>
        /// <param name="pageSize">	每页显示多少条（区间为1-50）</param>
        public static DataResult jdRefundList(string ids, string status, string orderId, string buyerId, string buyerName, string applyTimeStart, string applyTimeEnd, string checkTimeStart, string checkTimeEnd, string pageIndex, string pageSize,string token){
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
                    result.d = res;
                }
                
            }catch(Exception ex){
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
                    
        public static DataResult jdRefundById(string id,string token){
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
                    result.d = res.jingdong_pop_afs_soa_refundapply_queryById_responce.queryResult.result[0].refundApplyVo;
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
        /// <param name="status">退款单ID</param>
        /// <param name="checkUserName">审核人 </param>
        /// <param name="remark">	审核备注,除强制关单外的其它审核状态下必填 </param>
        /// <param name="rejectType">仅在审核不通过时填写该值。1: 商品已在配送途中,无法取消,烦请谅解；2：商品已签收,无法取消,烦请谅解；3：国际站保税区订单，已报关</param>
        /// <param name="outWareStatus">实际是否已出库:1已出库 2未出</param>
        public static DataResult jdReplyRefund(string status, string id , string checkUserName,string remark,string rejectType,string outWareStatus,string token){
            var result = new DataResult(1,null);
            if (status != "9")
            {
                if (remark == "")
                {            
                    result.s = -5002;         
                    return result;
                }
            }
            else if (status == "2")
            {                
                result.s = -5003;
                return result;
            }
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
        public static DataResult jdSkuAdd(string ware_id,string attributes,string jd_price,string stock_num,string trade_no,string outer_id,string token){
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