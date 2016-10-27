
using System.Collections.Generic;

namespace CoreModels.XyApi.Tmall
{
    public class WaybillCloudPrintApplyNewRequest{
         /// <summary>
	     /// 物流公司Code 顺丰(SF)（暂时不支持新接入，之前接入的可以正常使用）、EMS标准快递(EMS)、EMS经济快件(EYB)、宅急送(ZJS)、圆通(YTO)、中通(ZTO)、百世汇通(HTKY)、
         ///优速(UC)、申通(STO)、天天快递 (TTKDEX)、全峰(QFKD)、快捷(FAST)、邮政小包(POSTB)、国通(GTO)、韵达(YUNDA)、德邦快递(DBKD)。  
	     /// </summary>	 
        public string cp_code{get;set;}
        /// <summary>
        ///产品类型编码,不同物流公司支持的物流产品不同，需要通过search接口查询，当前物流公司支持的物流产品
        /// </summary>
        public string product_code{get;set;}
        
        public UserInfoDto sender{get;set;}
        public List<TradeOrderInfoDto> trade_order_info_dtos{get;set;}

    }

    public class UserInfoDto{
        public AddressDto  address{get;set;}
        public string mobile{get;set;}
        public string name{get;set;}
        public string phone{get;set;}
    }

    public class AddressDto{
        public string city{get;set;}
        public string detail{get;set;}
        public string district{get;set;}
        public string province{get;set;}
        public string town{get;set;}
    }

    public class TradeOrderInfoDto{
        public string logistics_services{get;set;}
        public long object_id{get;set;}
         /// <summary>
	     /// 订单信息
	     /// </summary>	 
        public OrderInfoDto order_info{get;set;}
        public PackageInfoDto package_info{get;set;}
        public UserInfoDto recipient{get;set;}
        /// <summary>
	     /// 标准模板 cainiao.cloudprint.stdtemplates.get 获取
	     /// </summary>	 
        public string template_url{get;set;}
        public long user_id{get;set;}

    }
    public class OrderInfoDto{
        public string order_channels_type{get;set;}
        public List<string> trade_order_list{get;set;}
    }

    public class PackageInfoDto{
        public string id{get;set;}
        public List<Item> items{get;set;}
        public long volume{get;set;}
        public long weight{get;set;}


    }
    public class Item{
        public long count{get;set;}
        public string name{get;set;}

    }

    ///<summary>
    ///根据面单号查询面单详细
    ///</summary>
    public class WaybillDetailQueryByWaybillCodeRequest{
		public string cp_code{get;set;}
		public string object_id{get;set;}
		public string waybill_code{get;set;}

	}

    ///<summary>
    ///电子面单云打印更新接口
    ///</summary>
    public class WaybillCloudPrintUpdateRequest{
        /// <summary>
	     /// 物流公司Code 顺丰(SF)（暂时不支持新接入，之前接入的可以正常使用）、EMS标准快递(EMS)、EMS经济快件(EYB)、宅急送(ZJS)、圆通(YTO)、中通(ZTO)、百世汇通(HTKY)、
         ///优速(UC)、申通(STO)、天天快递 (TTKDEX)、全峰(QFKD)、快捷(FAST)、邮政小包(POSTB)、国通(GTO)、韵达(YUNDA)、德邦快递(DBKD)。  
	     /// </summary>	 
        public string cp_code{get;set;}

        public string logistics_services{get;set;}
        public PackageInfoDto package_info{get;set;}

        public UserInfoDto recipient{get;set;}
        public UserInfoDto sender{get;set;}
        public string template_url{get;set;}
        ///<summary>
        ///必选
        ///</summary>
        public string waybill_code{get;set;}

        public string object_id{get;set;}

    }


    

}