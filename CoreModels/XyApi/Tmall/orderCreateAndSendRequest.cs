using System.Collections.Generic;

namespace CoreModels.XyApi.Tmall
{
    public class orderCreateAndSendRequest
    {
        public string user_id { get; set; }
        public string order_source { get; set; }
        public string order_type { get; set; }
        public string logis_type { get; set; }
        public string company_id { get; set; }
        public string trade_id { get; set; }
        public string mail_no { get; set; }
        public string shipping { get; set; }
        public string s_name { get; set; }
        public string s_area_id { get; set; }
        public string s_address { get; set; }
        public string s_zip_code { get; set; }
        public string s_mobile_phone { get; set; }
        public string s_telephone { get; set; }
        public string s_prov_name { get; set; }
        public string s_city_name { get; set; }
        public string s_dist_name { get; set; }
        public string r_name { get; set; }
        public string r_area_id { get; set; }
        public string r_address { get; set; }
        public string r_zip_code { get; set; }
        public string r_mobile_phone { get; set; }
        public string r_telephone { get; set; }
        public string r_prov_name { get; set; }
        public string r_city_name { get; set; }
        public string r_dist_name { get; set; }
        public List<ItemJson> item_json_string { get; set; }
        public string token{get;set;}
    }

    //物品的json
    public class ItemJson{
        public string itemName{get;set;}
        public decimal singlePrice{get;set;}
        public decimal itemCount{get;set;}

    }

    public class itemprops_get_response{
        public item_props item_props{set;get;}
        
    }
    public class item_props{
        public List<itemprops> item_prop{set;get;}
    }

    public class itemprops{
        public long  pid{get;set;}
        public string  name{get;set;}
        public bool must{get;set;}
        public bool multi{get;set;}
        public prop_values prop_values{get;set;}

    }

    public class prop_values{
        public List<prop_value> prop_value{get;set;}
    }
    public class prop_value{
        //public string name{get;set;}
        public string vid{get;set;}
    }

    public class cateStandard{
        public long id{get;set;}
        public long cid{get;set;}
    }





}