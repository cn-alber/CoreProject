using System.Collections.Generic;

namespace CoreData.CoreApi
{
    public abstract class TmallBase //: MarshalByRefObject
    {
        public readonly static string SERVER_URL = "http://gw.api.taobao.com/router/rest";
        //private static string SERVER_URL = "http://gw.api.tbsandbox.com/router/rest";
        public readonly static string SECRET ="f60e6b4c6565ecc865e7301ad02ef6a4";
        //private static string SECRET = "sandboxc6565ecc865e7301ad02ef6a4";//沙箱        

        public readonly static string ITEM_PROPS = @"pid,name,must,multi,prop_values,features,is_color_prop,is_sale_prop,is_key_prop,is_enum_prop,is_item_prop, features,status,sort_order,
                                            is_allow_alias,is_input_prop,taosir_do,is_material,material_do,expr_el_list";

        public readonly static string TOKEN = "6201025aabf03fc7e2a6e8c4587cd5096ZZa6f4d579d8302058964557";                                            

        

         public static IDictionary<string, string> Tmparam = new Dictionary<string, string>{
            {"app_key", "23476390"},
            //{"app_key", "1023476390"},//沙箱            
            {"format","json"},
            {"sign_method","md5"},
            {"timestamp", System.DateTime.Now.AddMinutes(6).ToString("yyyy-MM-dd HH:mm:ss")},
            {"v", "2.0"},            
        };

        public  static void cleanParam(){
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
        
        public static  void removeEmptyParam(){
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
    





    }

    
}