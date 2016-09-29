using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using System.Net;

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
                    httpClient.MaxResponseContentBufferSize = 300000;                                                                    
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













    }
}