
using System.Collections.Generic;

namespace CoreModels.XyApi.Tmall
{
    public class WaybillApplyNewRequest{
 
        public string cp_code{get;set;}
        public WaybillAddress shipping_address{get;set;}

        public List<TradeOrderInfo> trade_order_info_cols{get;set;}

    }

    public class WaybillAddress{
        public string  area{get;set;}
        public string province{get;set;}
        public string town{get;set;}
        public string address_detail{get;set;}
        public string city{get;set;}

    }

    public class TradeOrderInfo {
        public string consignee_name{get;set;}
        public string order_channels_type{get;set;}
        public List<string> trade_order_list{get;set;}
        public string consignee_phone{get;set;}
        public WaybillAddress consignee_address{get;set;}
        public string send_phone{get;set;}
        public long weight{get;set;}
        public string send_name{get;set;}
        public List<PackageItem> package_items{get;set;}
        public List<LogisticsService> logistics_service_list{get;set;}
        public string product_type{get;set;}
        public long real_user_id{get;set;}
        public long volume{get;set;}
        public long package_id{get;set;}
    }

    public class PackageItem{
        public string item_name{get;set;} 
        public long count{get;set;}
    }

    public class LogisticsService {
        public string service_value4_json{get;set;}
        public string service_code{get;set;}
    }

    public class WaybillCloudPrintApplyNewRequest{

        public string cp_code{get;set;}
        public string product_code{get;set;}
        
        public UserInfoDto sender{get;set;}

    }

    public class UserInfoDto{}




    

}