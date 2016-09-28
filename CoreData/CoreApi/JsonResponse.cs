using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;


namespace CoreDate.CoreApi
{
    public class JsonResponse{
    private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
        public static async  Task<string> CreatePostHttpResponse(string url, IDictionary<string, string> parameters,dynamic obj)
        {
           
            using (var httpClient = new HttpClient()){      
                try{                            
                    httpClient.MaxResponseContentBufferSize = 300000;                            
                    httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");//
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");


                    List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                    foreach( KeyValuePair<string, string> kvp in parameters)
                    {
                        paramList.Add(kvp);
                        //Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    }

                    // var request = new StringContent(JsonConvert.SerializeObject(obj),
                    //     Encoding.UTF8, "application/x-www-form-urlencoded");//
                    HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(paramList));
                    string jsonData = "";
                    byte[] data;
                    //Console.WriteLine(Encoding.UTF8.GetString(data, 0, data.Length - 1));
                    if (response.IsSuccessStatusCode)
                    {
                        
                        data =  response.Content.ReadAsByteArrayAsync().Result; 
                        foreach(byte i in data){
                            Console.WriteLine(i);
                        }      
                        jsonData = Encoding.UTF8.GetString(data, 0, data.Length - 1);       
                        Console.WriteLine(jsonData);         
                    }
                        
                    return jsonData;


                    }catch(Exception ex){
                         Console.WriteLine("--------------------");
                         Console.WriteLine(ex);
                         Console.WriteLine("--------------------");
                         return ex.Message;
                     } 


            }


            
        }













    }
}