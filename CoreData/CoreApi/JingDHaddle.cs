using System;
using System.Collections.Generic;
using CoreModels;
using CoreModels.XyApi.JingDong;
using Newtonsoft.Json;

namespace CoreDate.CoreApi
{
    
    public static class JingDHaddle{

        public static SystemP systemParam = new SystemP() {
                                                            app_key = "7888CE9C0F3AAD424FEE8EEAAC99E10E",
                                                            sign = "123",
                                                            app_secret = "a88b70e6c629465785088f9a9151701a",
                                                            timestamp = System.DateTime.Now.AddMinutes(6).ToString(),
                                                            url = "https://api.jd.com/routerjson",
                                                            v="2.0"
                                                        } ;
                                                        


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
            result.d = response.Result;//JsonConvert.DeserializeObject(response.Result);
            
            return result;
        }


        public static DataResult jdOrderDownload(string start_date, string end_date, string order_state, string page, string page_size, string token){
            var result = new DataResult(1,null);

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            
            parameters.Add("method", "360buy.order.search");
            parameters.Add("access_token", token);
            parameters.Add("app_key", systemParam.app_key);
            parameters.Add("sign", systemParam.sign);
            parameters.Add("timestamp", systemParam.timestamp);
            parameters.Add("v", systemParam.v);
            parameters.Add("360buy_param_json", "{\"start_date\":\"" + start_date + "\",\"end_date\":\"" + end_date + "\",\"order_state\":\"" + order_state + "\",\"page\":\"" + page + "\",\"page_size\":\"" + page_size + "\" }");

            var response = JsonResponse.CreatePostHttpResponse(systemParam.url, parameters);
            // HttpWebResponse response = jsonResponse.CreatePostHttpResponse(systemParam.url, parameters, encoding);
            // //打印返回值  
            // Stream stream = response.GetResponseStream();
            // StreamReader sr = new StreamReader(stream);
            // string html = sr.ReadToEnd();
            // if (html.IndexOf("error") > 0)
            // {
            //     return new BaseResult(false, null, html);
            // }
            // else
            // {
            //     BuyOrderSearchResponse orderResponce = JsonConvert.DeserializeObject<BuyOrderSearchResponse>(html);
            //     return new BaseResult(true, orderResponce.order_search_response.order_search.order_info_list, "success", int.Parse(orderResponce.order_search_response.order_search.order_total));
            // }       


            return result;
        }

        public static DataResult orderDownByIds(string order_id,string optional_fields,string order_state,string token){
            var result = new DataResult(1,null);
            try{
                List<order_info_list> orderinfolist = new List<order_info_list>();
                
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                string html = "";
                bool isSuccess = true;

                parameters.Add("method", "360buy.order.get");
                parameters.Add("access_token", token);
                parameters.Add("app_key", systemParam.app_key);
                parameters.Add("sign", systemParam.sign);
                parameters.Add("timestamp", systemParam.timestamp);
                parameters.Add("v", systemParam.v);
                parameters.Add("360buy_param_json", "{\"order_id\":\"" + order_id + "\",\"optional_fields\":\"" + optional_fields + "\",\"order_state\":\"" + order_state + "\"}");
                //HttpWebResponse response = jsonResponse.CreatePostHttpResponse(systemParam.url, parameters, encoding);
                var response = JsonResponse.CreatePostHttpResponse(systemParam.url, parameters);
                Console.WriteLine(response.Result);
                result.d = JsonConvert.DeserializeObject<BuyOrderGetResponse>("{\"order_get_response\":{\"code\":\"0\",\"order\":{\"orderInfo\":{\"modified\":\"2016-09-11 11:29:19\",\"order_id\":\"22919473317\"}}}");
                
                //result.d = response.Result.ToString().Replace("\\","");
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                result.d =  ex.Message;
            }

            // foreach (string order_id in order_ids)
            // {
            //     parameters.Add("method", "360buy.order.get");
            //     parameters.Add("access_token", token);
            //     parameters.Add("app_key", systemParam.app_key);
            //     parameters.Add("sign", systemParam.sign);
            //     parameters.Add("timestamp", systemParam.timestamp);
            //     parameters.Add("v", systemParam.v);
            //     parameters.Add("360buy_param_json", "{\"order_id\":\"" + order_id + "\",\"optional_fields\":\"" + optional_fields + "\",\"order_state\":\"" + order_state + "\"}");
            //     //HttpWebResponse response = jsonResponse.CreatePostHttpResponse(systemParam.url, parameters, encoding);
            //     var response = JsonResponse.CreatePostHttpResponse(systemParam.url, parameters);

            //     // Stream stream = response.GetResponseStream();
            //     // StreamReader sr = new StreamReader(stream);
            //     // html = sr.ReadToEnd();
            //     // if (html.IndexOf("error") > 0)
            //     // {
            //     //     isSuccess = false;
            //     //     break;
            //     // }                
            //     // orderinfolist.Add(JsonConvert.DeserializeObject<BuyOrderGetResponse>(html).order_get_response.order.orderInfo);
            //     // parameters = new Dictionary<string, string>();
            // }
            // if (!isSuccess)
            // {
            //     return new BaseResult(false, null, html);
            // }else {
            //     return new BaseResult(true, orderinfolist, "success");
            // }       



            return result;
        }














    }
}