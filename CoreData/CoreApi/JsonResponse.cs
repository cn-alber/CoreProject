using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Cryptography;

namespace CoreDate.CoreApi
{
    public class JsonResponse{
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
//         private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
//         {
//             return true; //总是接受     
//         }
        public static async  Task<string> CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {               
            using (var httpClient = new HttpClient(
                new HttpClientHandler {              
                      AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate 
                    })
            ){                                                   
                try{                            
                    httpClient.MaxResponseContentBufferSize = 9999999;                                                                    
                    httpClient.DefaultRequestHeaders.Add("user-agent", DefaultUserAgent);//                    
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    httpClient.DefaultRequestHeaders.Add("Accept-Charset", "gb2312");                     
                
                    List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                    foreach( KeyValuePair<string, string> kvp in parameters)
                    {
                        paramList.Add(kvp); 
                    }
                    // var request = new StringContent(JsonConvert.SerializeObject(obj),
                    //         Encoding.UTF8, "application/x-www-form-urlencoded");
                    // Console.WriteLine(request.ToString());
                    HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(paramList));
                    //HttpResponseMessage response = await httpClient.PostAsync(url, request);

                    string jsonData = "";
                    byte[] data;
                    if (response.IsSuccessStatusCode)
                    {              
                                  
                        data =  response.Content.ReadAsByteArrayAsync().Result;                                                 
                        jsonData = Encoding.UTF8.GetString(data, 0, data.Length - 1);
                               
                    }
                                                      
                    return jsonData;

                }catch(Exception ex){    
                    Console.WriteLine(ex.Message);    
                    return ex.Message;
                } 
            }
            
        }//end of public static async  Task<string> CreatePostHttpResponse


        /// <summary>
        /// 给Tmall请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的TOP请求参数</param>
        /// <param name="body">请求主体内容</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="signMethod">签名方法，可选值：md5, hmac</param>
        /// <returns>签名</returns>
       public static string SignTopRequest(IDictionary<string, string> parameters, string secret, string signMethod)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();
        
            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            if ("md5".Equals(signMethod))
            {
                query.Append(secret);
            }
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    query.Append(key).Append(value);                    
                }
            }
        
            // 第三步：使用MD5/HMAC加密
            byte[] bytes;
            if ("hmac".Equals(signMethod))
            {
                HMACMD5 hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
                bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
            }
            else
            {
                query.Append(secret);
                MD5 md5 = MD5.Create();
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
            }
        
            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }
        
            return result.ToString();
        }









    }
}