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

        public readonly static string TOKEN = "6202620e6f344bc7a7adb2886ba4ZZ9bd8442fbe60465632058964557";                                            



    }

    
}