using System;
using System.Collections.Generic;
using CoreModels;
using Newtonsoft.Json;

namespace CoreDate.CoreApi
{
    public  class TmallHaddle{
        
        private static string SERVER_URL = "https://eco.taobao.com/router/rest";

        private static IDictionary<string, string> Tmparam = new Dictionary<string, string>{
            {"app_key", "23476390"},            
            {"format","json"},
            {"sign_method","md5"},
            {"sign", "123"},
            {"timestamp", System.DateTime.Now.AddMinutes(6).ToString()},
            {"v", "2.0"}
        }; 
        private static void cleanParam(){
        
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
                Tmparam.Add("use_has_next", use_has_next.ToString());                            
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");
                                
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d = "code:"+res.error_response.code+" "+res.error_response.msg;
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












    }
}