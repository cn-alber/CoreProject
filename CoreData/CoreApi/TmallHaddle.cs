using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using CoreData;
using CoreModels;
using CoreModels.XyApi.Tmall;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CoreData.CoreApi
{
    public  class TmallHaddle{
        
        private static string SERVER_URL = "http://gw.api.taobao.com/router/rest";
        //private static string SERVER_URL = "http://gw.api.tbsandbox.com/router/rest";
        private static string SECRET ="f60e6b4c6565ecc865e7301ad02ef6a4";
        //private static string SECRET = "sandboxc6565ecc865e7301ad02ef6a4";//沙箱        

        public static string ITEM_PROPS = @"pid,name,must,multi,prop_values,features,is_color_prop,is_sale_prop,is_key_prop,is_enum_prop,is_item_prop, features,status,sort_order,
                                            is_allow_alias,is_input_prop,taosir_do,is_material,material_do,expr_el_list";
        private static IDictionary<string, string> Tmparam = new Dictionary<string, string>{
            {"app_key", "23476390"},
            //{"app_key", "1023476390"},//沙箱            
            {"format","json"},
            {"sign_method","md5"},
            {"timestamp", System.DateTime.Now.AddMinutes(6).ToString("yyyy-MM-dd HH:mm:ss")},
            {"v", "2.0"},            
        }; 
        private static void cleanParam(){
            List<string> rmlist = new List<string>();
            foreach (var item in Tmparam)
            {                
                if(item.Key !="app_key"&&item.Key !="format"&&item.Key !="sign_method"&&item.Key !="timestamp"&&item.Key !="v")                        
                    rmlist.Add(item.Key);                    
            }
            foreach(var rmkey in rmlist){
                Tmparam.Remove(rmkey);
            }        
        }
        
        private static void removeEmptyParam(){
            IEnumerator<KeyValuePair<string, string>> dem = Tmparam.GetEnumerator();
            List<string> rmlist = new List<string>();
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (string.IsNullOrEmpty(value))
                {
                     rmlist.Add(key); 
                     //Tmparam.Remove(key);               
                }
            }
            foreach(var rmkey in rmlist){
                Tmparam.Remove(rmkey);
            }  

        }



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
        /// <summary>
        ///  交易（订单）下载 参考网址： http://open.taobao.com/docs/api.htm?apiId=46
        /// </summary>
        /// <param name="fields">必选，需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段</param>
        /// <param name="start_created">查询三个月内交易创建时间开始。格式:yyyy-MM-dd HH:mm:ss</param>
        /// <param name="end_created">查询交易创建时间结束。格式:yyyy-MM-dd HH:mm:ss</param>
        /// <param name="status">交易状态</param>
        /// <param name="buyer_nick">买家昵称</param>
        /// <param name="type">交易类型列表</param>
        /// <param name="ext_type">扩展类型</param>
        /// <param name="rate_status">评价状态 </param>
        /// <param name="tag">卖家对交易的自定义分组标签</param>
        /// <param name="page_no">页码</param>
        /// <param name="page_size">每页条数</param>
        /// <param name="use_has_next">是否启用has_next的分页方式，如果指定true,则返回的结果中不包含总记录数，但是会新增一个是否存在下一页的的字段</param>        
        public static DataResult OrderDownload(string fields,string start_created, string end_created, string status,string buyer_nick,
                                            string type, string ext_type,string rate_status,string tag,int page, int pageSize, bool use_has_next,string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.trades.sold.get");                
                Tmparam.Add("session", token);                
                Tmparam.Add("buyer_nick", buyer_nick);
                Tmparam.Add("end_created", end_created);
                Tmparam.Add("ext_type", ext_type);
                Tmparam.Add("fields", fields);
                Tmparam.Add("page", page.ToString());
                Tmparam.Add("pageSize", pageSize.ToString());
                Tmparam.Add("rate_status", rate_status);
                Tmparam.Add("start_created",start_created);
                Tmparam.Add("status",status);
                Tmparam.Add("tag", tag);
                Tmparam.Add("type", type);
            
                //Tmparam.Add("use_has_next", use_has_next.ToString()); 
                removeEmptyParam();    
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.trades_sold_get_response.trades.trade;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region Order 获取单笔交易的部分信息(性能高)
        /// <summary>
        ///  单条交易（订单）下载 参考网址： http://open.taobao.com/docs/api.htm?apiId=47
        /// </summary>
        /// <param name="fields">必选， 需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段</param>
        /// <param name="tids">必选， 交易ids,多条id 以 , 结尾</param>
        public static DataResult getbytid(string fields,string tids,string token){
            var result = new DataResult(1,null);
            List<dynamic> orderinfolist = new List<dynamic>();
            try{                                        
                Tmparam.Add("method", "taobao.trade.get");
                Tmparam.Add("session", token);
                Tmparam.Add("fields",fields);
                var tidArr = tids.Split(',');
                foreach(string tid in tidArr){
                    Tmparam.Add("tid",tid);
                    string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                    Tmparam.Add("sign", sign);//                                      
                    var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                    var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                    if(response.Result.ToString().IndexOf("error") > 0){
                        result.s = -1;
                        result.d ="交易单号："+tid+" 获取失败, code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                        break;
                    }else{
                        orderinfolist.Add(res.trade_get_response.trade);
                    }
                    Tmparam.Remove("tid");
                    Tmparam.Remove("sign");
                }
                
                if(result.s == 1)
                    result.d = orderinfolist;

            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 在线订单发货处理（支持货到付款）
        /// <summary>
        ///  在线订单发货处理（支持货到付款） 参考网址： http://open.taobao.com/docs/api.htm?apiId=10687
        /// </summary>
        /// <param name="sub_tid">需要拆单发货的子订单集合，针对的是一笔交易下有多个子订单需要分开发货的场景；1次可传人多个子订单号，子订单间用逗号隔开；为空表示不做拆单发货。</param>
        /// <param name="tid">必选， 淘宝交易ID</param>
        /// <param name="is_split">表明是否是拆单，默认值0，1表示拆单</param>
        /// <param name="out_sid">运单号</param>
        /// <param name="company_code">必选，物流公司代码.如"POST"就代表中国邮政,"ZJS"就代表宅急送.调用 taobao.logistics.companies.get 获取。</param>
        /// <param name="sender_id">卖家联系人地址库ID，可以通过taobao.logistics.address.search接口查询到地址库ID。如果为空，取的卖家的默认取货地址</param>
        /// <param name="cancel_id">卖家联系人地址库ID，可以通过taobao.logistics.address.search接口查询到地址库ID。 如果为空，取的卖家的默认退货地址</param>
        /// <param name="feature"></param>
        /// <param name="seller_ip">商家的IP地址</param>
        public static DataResult onlineSend(string token,string sub_tid,string tid,string is_split,string out_sid,string company_code,string sender_id,
                                        string cancel_id,string feature,string seller_ip){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.logistics.online.send");
                Tmparam.Add("session", token);
                Tmparam.Add("tid", tid);
                Tmparam.Add("company_code", company_code);

                Tmparam.Add("sub_tid",sub_tid);            
                Tmparam.Add("is_split", is_split);
                Tmparam.Add("out_sid",out_sid);                
                Tmparam.Add("sender_id",sender_id);
                Tmparam.Add("cancel_id",cancel_id);
                Tmparam.Add("feature",feature);
                Tmparam.Add("seller_ip",seller_ip);

                
                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.shipping.is_success;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 取消物流订单接口
        /// <summary>
        ///  取消物流订单接口 参考网址： http://open.taobao.com/docs/api.htm?apiId=10688
        /// </summary>
        /// <param name="tid">淘宝交易ID</param>
        public static DataResult onlineCancel(string token,string tid){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.logistics.online.cancel");
                Tmparam.Add("session", token);
                Tmparam.Add("tid", tid);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion
        
        #region 确认发货通知接口
        /// <summary>
        ///  确认发货通知接口 参考网址： http://open.taobao.com/docs/api.htm?apiId=10689
        ///  仅使用taobao.logistics.online.send 发货时，未输入运单号的情况下，需要使用该接口确认发货
        /// </summary>
        /// <param name="tid">必选，淘宝交易ID</param>
        /// <param name="sub_tid">拆单子订单列表</param>
        /// <param name="is_split">	表明是否是拆单，默认值0，1表示拆单</param>
        /// <param name="out_sid">必选，运单号.具体一个物流公司的真实运单号码。淘宝官方物流会校验，请谨慎传入；</param>
        /// <param name="seller_ip">商家的IP地址</param>
        public static DataResult onlineConfirm(string token,string tid,string sub_tid,string is_split,string out_sid,string seller_ip){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.logistics.online.confirm");
                Tmparam.Add("session", token);
                Tmparam.Add("tid", tid);
                Tmparam.Add("sub_tid", sub_tid);
                Tmparam.Add("is_split", is_split);
                Tmparam.Add("out_sid", out_sid);
                Tmparam.Add("seller_ip", seller_ip);
                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.shipping.is_success;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  自己联系物流（线下物流）发货 参考网址： http://open.taobao.com/docs/api.htm?apiId=10690
        /// </summary>
        /// <param name="sub_tid">需要拆单发货的子订单集合，针对的是一笔交易下有多个子订单需要分开发货的场景；1次可传人多个子订单号，子订单间用逗号隔开；为空表示不做拆单发货。</param>
        /// <param name="tid">必选， 淘宝交易ID</param>
        /// <param name="is_split">表明是否是拆单，默认值0，1表示拆单</param>
        /// <param name="out_sid">运单号</param>
        /// <param name="company_code">必选，物流公司代码.如"POST"就代表中国邮政,"ZJS"就代表宅急送.调用 taobao.logistics.companies.get 获取。</param>
        /// <param name="sender_id">卖家联系人地址库ID，可以通过taobao.logistics.address.search接口查询到地址库ID。如果为空，取的卖家的默认取货地址</param>
        /// <param name="cancel_id">卖家联系人地址库ID，可以通过taobao.logistics.address.search接口查询到地址库ID。 如果为空，取的卖家的默认退货地址</param>
        /// <param name="feature"></param>
        /// <param name="seller_ip">商家的IP地址</param>

        public static DataResult offlineSend(string token,string sub_tid,string tid,string is_split,string out_sid,string company_code,string sender_id,
                                        string cancel_id,string feature,string seller_ip){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.logistics.offline.send");
                Tmparam.Add("session", token);
                Tmparam.Add("sub_tid",sub_tid);
                Tmparam.Add("tid", tid);
                Tmparam.Add("is_split", is_split);
                Tmparam.Add("out_sid",out_sid);
                Tmparam.Add("company_code", company_code);
                Tmparam.Add("sender_id",sender_id);
                Tmparam.Add("cancel_id",cancel_id);
                Tmparam.Add("feature",feature);
                Tmparam.Add("seller_ip",seller_ip);

                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.shipping.is_success;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  无需物流（虚拟）发货处理 参考网址： http://open.taobao.com/docs/api.htm?apiId=10691
        /// </summary>
        /// <param name="tid">必选， 淘宝交易ID</param>
        /// <param name="feature"></param>
        /// <param name="seller_ip">商家的IP地址</param>
        public static DataResult dummySend(string token,string tid,string feature,string seller_ip){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.logistics.dummy.send");
                Tmparam.Add("session", token);
                Tmparam.Add("tid", tid);
                Tmparam.Add("feature",feature);
                Tmparam.Add("seller_ip",seller_ip);

                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.shipping.is_success;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  创建订单并发货 参考网址： http://open.taobao.com/docs/api.htm?apiId=22172
        /// </summary>
        /// <param name="user_id	">必选，	用户ID	</param>
        /// <param name="order_source	">必选，	订单来源，值选择：30	</param>
        /// <param name="order_type	">必选，	订单类型，值固定选择：30	</param>
        /// <param name="logis_type	">必选，	物流订单物流类型，值固定选择：2	</param>
        /// <param name="company_id	">必选，	物流公司ID	</param>
        /// <param name="trade_id	">必选，	交易流水号，淘外订单号或者商家内部交易流水号	</param>
        /// <param name="mail_no	">	运单号	</param>
        /// <param name="shipping	">	费用承担方式 1买家承担运费 2卖家承担运费	</param>
        /// <param name="s_name	">必选，	发件人名称	</param>
        /// <param name="s_area_id	">必选，	发件人区域ID	</param>
        /// <param name="s_address	">必选，	发件人街道地址	</param>
        /// <param name="s_zip_code	">必选，	发件人出编	</param>
        /// <param name="s_mobile_phone	">	手机号码	</param>
        /// <param name="s_telephone	">	电话号码	</param>
        /// <param name="s_prov_name	">	省	</param>
        /// <param name="s_city_name	">	市	</param>
        /// <param name="s_dist_name	">	区	</param>
        /// <param name="r_name	">必选，	收件人名称	</param>
        /// <param name="r_area_id	">必选，	收件人区域ID	</param>
        /// <param name="r_address	">必选，	收件人街道地址	</param>
        /// <param name="r_zip_code	">必选，	收件人邮编	</param>
        /// <param name="r_mobile_phone	">	手机号码	</param>
        /// <param name="r_telephone	">	电话号码	</param>
        /// <param name="r_prov_name	">必选，	省	</param>
        /// <param name="r_city_name	">必选，	市	</param>
        /// <param name="r_dist_name	">	区	</param>
        /// <param name="item_json_string	">必选，	物品的json数据。	</param>
        public static DataResult orderCreateAndSend (orderCreateAndSendRequest order){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.logistics.consign.order.createandsend");
                Tmparam.Add("session", order.token);
                Tmparam.Add("user_id", order.user_id);
                Tmparam.Add("order_source", order.order_source);
                Tmparam.Add("order_type", order.order_type);
                Tmparam.Add("logis_type", order.logis_type);
                Tmparam.Add("company_id", order.company_id);
                Tmparam.Add("trade_id", order.trade_id);
                Tmparam.Add("mail_no", order.mail_no);
                Tmparam.Add("shipping", order.shipping);
                Tmparam.Add("s_name", order.s_name);
                Tmparam.Add("s_area_id", order.s_area_id);
                Tmparam.Add("s_address", order.s_address);
                Tmparam.Add("s_zip_code", order.s_zip_code);
                Tmparam.Add("s_mobile_phone", order.s_mobile_phone);
                Tmparam.Add("s_telephone", order.s_telephone);
                Tmparam.Add("s_prov_name", order.s_prov_name);
                Tmparam.Add("s_city_name", order.s_city_name);
                Tmparam.Add("s_dist_name", order.s_dist_name);
                Tmparam.Add("r_name", order.r_name);
                Tmparam.Add("r_area_id", order.r_area_id);
                Tmparam.Add("r_address", order.r_address);
                Tmparam.Add("r_zip_code", order.r_zip_code);
                Tmparam.Add("r_mobile_phone", order.r_mobile_phone);
                Tmparam.Add("r_telephone", order.r_telephone);
                Tmparam.Add("r_prov_name", order.r_prov_name);
                Tmparam.Add("r_city_name", order.r_city_name);
                Tmparam.Add("r_dist_name", order.r_dist_name);
                Tmparam.Add("item_json_string", Newtonsoft.Json.JsonConvert.SerializeObject(order.item_json_string));

                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  查询买家申请的退款列表 参考网址： http://open.taobao.com/doc2/apiDetail.htm?apiId=51&scopeId=11850
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="status">退款状态，默认查询所有退款状态的数据，除了默认值外每次只能查询一种状态</param>
        /// <param name="seller_nick">卖家昵称</param>
        /// <param name="type">交易类型列表，一次查询多种类型可用半角逗号分隔，默认同时查询guarantee_trade, auto_delivery的2种类型的数据。</param>
        /// <param name="page_no"></param>
        /// <param name="page_size">最大值:100</param>
        public static DataResult refundsApplyGet(string token,string fields,string status,string seller_nick,string type,int page_no,int page_size){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.refunds.apply.get");
                Tmparam.Add("session", token);                
                Tmparam.Add("fields", fields);
                Tmparam.Add("status", status);
                Tmparam.Add("seller_nick", seller_nick);
                Tmparam.Add("type", type);
                Tmparam.Add("page_no", page_no.ToString());
                Tmparam.Add("page_size", page_size.ToString());

                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.refunds_apply_get_response;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  查询卖家收到的退款列表 参考网址：http://open.taobao.com/docs/api.htm?scopeId=11850&apiId=52 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="status">退款状态，默认查询所有退款状态的数据，除了默认值外每次只能查询一种状态</param>
        /// <param name="buyer_nick">卖家昵称</param>
        /// <param name="type">交易类型列表，一次查询多种类型可用半角逗号分隔，默认同时查询guarantee_trade, auto_delivery的2种类型的数据。</param>
        /// <param name="start_modified"></param>
        /// <param name="end_modified"></param>
        /// <param name="page_no"></param>
        /// <param name="page_size">最大值:100</param>
        public static DataResult refundsReceiveGet(string token,string fields,string status,string buyer_nick,string type,string start_modified,string end_modified,int page_no,int page_size){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.refunds.receive.get");
                Tmparam.Add("session", token);
                Tmparam.Add("fields", fields);
                Tmparam.Add("status", status);
                Tmparam.Add("buyer_nick", buyer_nick);
                Tmparam.Add("type", type);
                Tmparam.Add("start_modified", start_modified);
                Tmparam.Add("end_modified", end_modified);
                Tmparam.Add("page_no", page_no.ToString());
                Tmparam.Add("page_size", page_size.ToString());             

                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.refunds_receive_get_response;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  获取单笔退款详情 参考网址： http://open.taobao.com/docs/api.htm?scopeId=11850&apiId=53
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="refund_id"> 必选，退款单号</param>
        public static DataResult refundOneGet(string token,string fields,string refund_id){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.refund.get");
                Tmparam.Add("session", token);
                Tmparam.Add("fields", fields);
                Tmparam.Add("refund_id", refund_id);
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="退款单ID： "+refund_id+"获取失败 code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                    
                }else{
                    result.d = res.refund_get_response;
                }                                                        
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  查询退款留言/凭证列表 参考网址： http://open.taobao.com/docs/api.htm?scopeId=11850&apiId=124
        /// </summary>
        /// <param name="fields">必选，</param>
        /// <param name="refund_id">必选，退款单号</param>
        /// <param name="page_no"></param>
        /// <param name="page_size"></param>
        /// <param name="refund_phase">退款阶段，可选值：onsale（售中），aftersale（售后），天猫退款为必传。</param>
        public static DataResult refundMessagesGet(string token,string fields ,string refund_id,int page_no,int page_size,string refund_phase){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.refund.messages.get");
                Tmparam.Add("session", token);
                Tmparam.Add("fields", fields);
                Tmparam.Add("refund_id",refund_id);
                Tmparam.Add("page_no", page_no.ToString());
                Tmparam.Add("page_size", page_size.ToString());
                Tmparam.Add("refund_phase", refund_phase);
        
                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  卖家拒绝退款 参考网址： http://open.taobao.com/docs/api.htm?scopeId=11850&apiId=10480
        /// </summary>
        /// <param name="refund_id">退款单号</param>
        /// <param name="refuse_message">拒绝退款时的说明信息，长度2-200</param>
        /// <param name="refuse_proof">拒绝退款时的退款凭证，最大长度130000字节，支持的图片格式：GIF, JPG, PNG。天猫退款为必填项。支持的文件类型：gif,jpg,png</param>
        /// <param name="refund_phase">可选值为：售中：onsale，售后：aftersale，天猫退款为必填项。</param>
        /// <param name="refund_version">退款版本号，天猫退款为必填项</param>
        /// <param name="refuse_reason_id">拒绝原因编号，会提供用户拒绝原因列表供选择</param>        
        public static DataResult refundRefuse(string token,string refund_id,string refuse_message,string refuse_proof,string refund_phase,string refund_version,string refuse_reason_id ){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.refund.refuse");
                Tmparam.Add("session", token);
                Tmparam.Add("refund_id", refund_id);
                Tmparam.Add("refuse_message", refuse_message);
                Tmparam.Add("refuse_proof", refuse_proof);
                Tmparam.Add("refund_phase", refund_phase);
                Tmparam.Add("refund_version", refund_version);
                Tmparam.Add("refuse_reason_id", refuse_reason_id);

                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.rp_returngoods_refuse_response;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  添加SKU 参考网址： http://open.taobao.com/docs/api.htm?apiId=27
        /// </summary>
        ///<param name="num_iid	">必选，	所属商品数字id，可通过 taobao.item.get 获取。必选	</param>
        ///<param name="properties	">必选，	Sku属性串。格式:pid:vid;pid:vid,如:1627207:3232483;1630696:3284570,表示:机身颜色:军绿色;手机套餐:一电一充。	</param>
        ///<param name="quantity	">必选，	Sku的库存数量。sku的总数量应该小于等于商品总数量(Item的NUM)。取值范围:大于零的整数	</param>
        ///<param name="price	">必选，	Sku的销售价格。商品的价格要在商品所有的sku的价格之间。精确到2位小数;单位:元。如:200.07，表示:200元7分	</param>
        ///<param name="outer_id	">	Sku的商家外部id	</param>
        ///<param name="item_price	">	sku所属商品的价格。当用户新增sku，使商品价格不属于sku价格之间的时候，用于修改商品的价格，使sku能够添加成功	</param>
        ///<param name="lang	">	Sku文字的版本。可选值:zh_HK(繁体),zh_CN(简体);默认值:zh_CN	</param>
        ///<param name="spec_id	">	产品的规格信息	</param>
        ///<param name="sku_hd_length	">	家装建材类目，商品SKU的长度，正整数，单位为cm，部分类目必选。天猫商家专用。 数据和SKU一一对应，用,分隔，如：20,30,30	</param>
        ///<param name="sku_hd_height	">	家装建材类目，商品SKU的高度，单位为cm，部分类目必选。	</param>
        ///<param name="sku_hd_lamp_quantity	">	家装建材类目，商品SKU的灯头数量，正整数，大于等于3，部分类目必选。天猫商家专用。 数据和SKU一一对应，用,分隔，如：3,5,7	</param>
        ///<param name="ignorewarning	">	忽略警告提示.	</param>
        public static DataResult itemSkuAdd (skuAddRequest sku){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.item.sku.add");
                Tmparam.Add("session", sku.token);
                Tmparam.Add("num_iid", sku.num_iid);
                Tmparam.Add("properties", sku.properties);
                Tmparam.Add("quantity", sku.quantity);
                Tmparam.Add("price", sku.price);
                Tmparam.Add("outer_id", sku.outer_id);
                Tmparam.Add("item_price", sku.item_price);
                Tmparam.Add("lang", sku.lang);
                Tmparam.Add("spec_id", sku.spec_id);
                Tmparam.Add("sku_hd_length", sku.sku_hd_length);
                Tmparam.Add("sku_hd_height", sku.sku_hd_height);
                Tmparam.Add("sku_hd_lamp_quantity", sku.sku_hd_lamp_quantity);
                Tmparam.Add("ignorewarning", sku.ignorewarning);
                
                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  获取SKU 参考网址： http://open.taobao.com/docs/api.htm?apiId=28 
        /// </summary>
        /// <param name="fields">必选</param>
        /// <param name="sku_id">必选，</param>
        /// <param name="num_iid">商品的数字IID（num_iid和nick必传一个，推荐用num_iid），传商品的数字id返回的结果里包含cspu（SKu上的产品规格信息）。</param>
        /// <param name="nick">卖家nick(num_iid和nick必传一个)，只传卖家nick时候，该api返回的结果不包含cspu（SKu上的产品规格信息）。</param>
        public static DataResult itemSkuGet(string token,string fields, string sku_id,string num_iid,string nick){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.item.sku.get");
                Tmparam.Add("session", token);
                Tmparam.Add("fields", fields);
                Tmparam.Add("sku_id", sku_id);
                Tmparam.Add("num_iid", num_iid);
                Tmparam.Add("nick", nick);
                    
                removeEmptyParam();    
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="Sku ID: "+sku_id+" 信息获取失败 code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                
                }else{
                    result.d =res.item_sku_get_response.sku;
                }                                                                      
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  更新SKU信息 参考网址： http://open.taobao.com/docs/api.htm?apiId=29
        /// </summary>
        ///<param name="num_iid	">	最小值：0	所属商品数字id，可通过 taobao.item.get 获取。必选	</param>
        ///<param name="properties	">	Sku属性串。格式:pid:vid;pid:vid,如:1627207:3232483;1630696:3284570,表示:机身颜色:军绿色;手机套餐:一电一充。	</param>
        ///<param name="quantity	">	Sku的库存数量。sku的总数量应该小于等于商品总数量(Item的NUM)。取值范围:大于零的整数	</param>
        ///<param name="price	">	Sku的销售价格。商品的价格要在商品所有的sku的价格之间。精确到2位小数;单位:元。如:200.07，表示:200元7分	</param>
        ///<param name="outer_id	">	Sku的商家外部id	</param>
        ///<param name="item_price	">	sku所属商品的价格。当用户新增sku，使商品价格不属于sku价格之间的时候，用于修改商品的价格，使sku能够添加成功	</param>
        ///<param name="lang	">	Sku文字的版本。可选值:zh_HK(繁体),zh_CN(简体);默认值:zh_CN	</param>
        ///<param name="spec_id	">	产品的规格信息	</param>
        ///<param name="barcode	">SKU条形码		</param>
        ///<param name="ignorewarning	">忽略警告提示.	</param>
        public static DataResult itemSkuUpdate(skuUpdateRequest sku){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.item.sku.update");
                Tmparam.Add("session", sku.token);
                Tmparam.Add("num_iid", sku.num_iid);
                Tmparam.Add("properties", sku.properties);
                if(string.IsNullOrEmpty(sku.quantity)){ Tmparam.Add("quantity", sku.quantity);}
                if(string.IsNullOrEmpty(sku.price)){ Tmparam.Add("price", sku.price);}
                if(string.IsNullOrEmpty(sku.outer_id)){ Tmparam.Add("outer_id", sku.outer_id);}
                if(string.IsNullOrEmpty(sku.item_price)){ Tmparam.Add("item_price", sku.item_price);}
                if(string.IsNullOrEmpty(sku.lang)){ Tmparam.Add("lang", sku.lang);}
                if(string.IsNullOrEmpty(sku.spec_id)){ Tmparam.Add("spec_id", sku.spec_id);}
                if(string.IsNullOrEmpty(sku.barcode)){ Tmparam.Add("barcode", sku.barcode);}
                if(string.IsNullOrEmpty(sku.ignorewarning)){ Tmparam.Add("ignorewarning", sku.ignorewarning);}

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.item_sku_update_response.sku;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        #region 
        /// <summary>
        ///  根据商品ID列表获取SKU信息 参考网址： http://open.taobao.com/docs/api.htm?apiId=30
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="num_iids">必选，sku所属商品数字id，必选。num_iid个数不能超过40个</param>
        public static DataResult itemSkusGet(string token,string fields,string num_iids){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.item.skus.get");
                Tmparam.Add("session", token);

                Tmparam.Add("fields", fields);
                Tmparam.Add("num_iids", num_iids);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.item_skus_get_response.skus;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion


        #region 
        /// <summary>
        ///   参考网址： 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult skuDelete(skuDeleteRequest sku){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "taobao.item.sku.delete");
                Tmparam.Add("session", sku.token);

                Tmparam.Add("num_iid", sku.num_iid);
                Tmparam.Add("properties", sku.properties);
                Tmparam.Add("item_price", sku.item_price);
                Tmparam.Add("item_num", sku.item_num);
                Tmparam.Add("lang", sku.lang);
                Tmparam.Add("ignorewarning", sku.ignorewarning);
                
                removeEmptyParam();
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.item_sku_delete_response.sku;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        public static DataResult itemProps(string cid){
            string token = "6202620e6f344bc7a7adb2886ba4ZZ9bd8442fbe60465632058964557";
            string fields = ITEM_PROPS;
            return itempropsGet (token,fields, cid);
        }


        #region  获取类目属性
        /// <summary>
        ///  获取标准商品类目属性 参考网址：http://open.taobao.com/docs/api.htm?apiId=121 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult itempropsGet (string token,string fields,string cid){
            var result = new DataResult(1,null);
            try{          
                 string timestamp = "";
                Tmparam.TryGetValue("timestamp",out timestamp); 
                Console.WriteLine(timestamp);                            
                Tmparam.Add("method", "taobao.itemprops.get");
                Tmparam.Add("session", token);
                Tmparam.Add("fields", fields);
                Tmparam.Add("cid", cid);            
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);

                //Console.OutputEncoding = Encoding.UTF8;                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);                            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\'"," ").Replace("\"","\'")+"}");  
                                                                                                        
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;                    
                }else{
                    var sku_props = new List<dynamic>();
                    var item_props = new List<dynamic>();
                    if(response.Result.ToString().IndexOf("item_props") > -1){
              
                        foreach(var item in res.itemprops_get_response.item_props.item_prop){
                            if(!Convert.ToBoolean(JsonConvert.SerializeObject(item.is_item_prop)) && Convert.ToBoolean(JsonConvert.SerializeObject(item.is_sale_prop))){ 
                                sku_props.Add(item);
                            }else{
                                item_props.Add(item);
                            }
                        }
                        var d = new System.Collections.Generic.Dictionary<string, object>();
                        d.Add("sku_props", sku_props);
                        d.Add("item_props", item_props);
                        result.d = d;
                    }else{
                        var d = new System.Collections.Generic.Dictionary<string, object>();
                        d.Add("sku_props", sku_props);
                        d.Add("item_props", item_props);
                        result.d = d;
                    }                                                
                }                                                  
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion

        public static List<cateStandard> getItemCates(){
            List<cateStandard> cates = new List<cateStandard>();
            using (var conn = new MySqlConnection(DbBase.CommConnectString)){
                string sql = "SELECT i.id,i.tb_cid as cid FROM item_cates_standard as i WHERE i.parent_id != 0 AND i.tb_cid >1100 ORDER BY i.tb_cid ASC;";
                cates = conn.Query<cateStandard>(sql).AsList();
            }
            
            return cates;
        }




        #region 
        /// <summary>
        ///   参考网址： 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult demo(string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "");
                Tmparam.Add("session", token);


                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion












    }
}