using System;
using System.Collections.Generic;
using CoreData;
using CoreData.CoreComm;
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
        public static DataResult jdOrderDownload(string start_date, string end_date, string order_state, string page, string page_size, string token){
            var result = new DataResult(1,null);
            try{              
                jdparam.Add("method", "360buy.order.search");
                jdparam.Add("access_token", token);
                jdparam.Add("360buy_param_json", "{\"start_date\":\"" + start_date + "\",\"end_date\":\"" + end_date + "\",\"order_state\":\"" + order_state + "\",\"page\":\"" + page + "\",\"page_size\":\"" + page_size + "\" }");
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, jdparam);            
                result.d = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}").order_search_response.order_search.order_info_list;
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
                    orderinfolist.Add(JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}").order_get_response.order.orderInfo);
                    jdparam.Remove("360buy_param_json");
                }
                result.d = orderinfolist;
            }catch(Exception ex){                
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }
            return result;
        }














    }
}