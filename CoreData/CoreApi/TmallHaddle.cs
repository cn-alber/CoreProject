using System;
using System.Collections.Generic;
using CoreDate.CoreApi;
using CoreModels;
using Newtonsoft.Json;

namespace CoreDate.CoreApi
{
    public  class TmallHaddle{
        
        private static string SERVER_URL = "http://gw.api.taobao.com/router/rest";
        private static string SECRET = "f60e6b4c6565ecc865e7301ad02ef6a4";        

        private static IDictionary<string, string> Tmparam = new Dictionary<string, string>{
            {"app_key", "23476390"},            
            {"format","json"},
            {"sign_method","md5"},            
            {"timestamp", System.DateTime.Now.AddMinutes(6).ToString()},
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
        /// <param name="fields">需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段</param>
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
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.msg;
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
        /// <param name="fields">需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段</param>
        /// <param name="tids">交易ids,多条id 以 , 结尾</param>
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
                        result.d ="交易单号："+tid+" 获取失败, code:"+res.error_response.code+" "+res.error_response.msg;
                        break;
                    }else{
                        orderinfolist.Add(res.trade_get_response.trade);
                    }
                    Tmparam.Remove("tid");
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





        #region 
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
                    result.d ="code:"+res.error_response.code+" "+res.error_response.msg;
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