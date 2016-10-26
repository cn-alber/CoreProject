using System;
using System.Collections.Generic;
using System.Linq;
using CoreModels;
using CoreModels.XyApi.Tmall;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CoreData.CoreApi
{
    public  class CaiNiaoHaddle:TmallBase
    { 
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

        #region 
        /// <summary>
        ///   参考网址： 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult cntmsMailnoGet (){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.cntms.mailno.get");
                Tmparam.Add("session", TOKEN);
                
                CnTmsMailnoGetContentDomain content  = new CnTmsMailnoGetContentDomain();
                content.receiver_info = new CnTmsMailnoReceiverinfoDomain();
                content.receiver_info.receiver_province="江苏省";
                content.receiver_info.receiver_city="苏州市";
                content.receiver_info.receiver_address = "江苏省苏州市常熟市梅李镇将泾村33号（226村道西150米瑞益纺织旁）";
                content.receiver_info.receiver_mobile = "13776218043";

                content.sender_info = new CnTmsMailnoSenderinfoDomain();
                content.sender_info.sender_province="江苏省";
                content.sender_info.sender_city = "苏州市";
                content.sender_info.sender_address = "江苏省苏州市常熟市虞山镇莫城管理区可美大厦七楼";
                content.sender_info.sender_name = "南极云商";
                content.sender_info.sender_mobile = "13776218043";


                var item = new CnTmsMailnoItemDomain();
                item.item_name = "南极人2016冬季休闲男士毛领连帽保暖羽绒服中长款修身加厚外套男";
                item.item_qty = 1;
                content.items = new List<CnTmsMailnoItemDomain>();
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                content.items.Add(item);
                                
                content.order_source = "TB";
                content.solutions_code="DM092876";
                content.trade_id = "2605837640524916";
                
                Console.WriteLine("-----------------");
                Console.WriteLine(JsonConvert.SerializeObject(content));
                Console.WriteLine("-----------------");

                Tmparam.Add("content",JsonConvert.SerializeObject(content).Replace("\"","\'"));// 
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