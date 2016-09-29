using System.Collections.Generic;

namespace CoreModels.XyApi.JingDong
{

    public class BuyOrderSearchResponse  //订单下载同步接口
    {
        public order_search_response order_search_response { get; set; }
    }
    public class order_search_response
    {
        public string code { get; set; }
        public order_search order_search { get; set; }
    }


    public class order_search
    {
        public string order_total { get; set; }
        public List<order_info_list> order_info_list { get; set; }
    }

    public class order_info_list
    {
        public string modified { get; set; }
       // public string customs { get; set; }
        public string order_id { get; set; }
        // public string vender_id { get; set; }
        // public string pay_type { get; set; }
        // public string order_total_price { get; set; }
        // public string order_seller_price { get; set; }
        // public string order_payment { get; set; }
        // public string freight_price { get; set; }
        // public string seller_discount { get; set; }
        // public string order_state { get; set; }
        // public string delivery_type { get; set; }
        // public string invoice_info { get; set; }
        // public string order_remark { get; set; }
        // public string order_start_time { get; set; }
        // public string order_type { get; set; }
        // public string order_source { get; set; }
        // public string store_order { get; set; }
        // public string customs_model { get; set; }
      
        // public consignee_info consignee_info {  get; set;}
        // public List<item_info_list> item_info_list {get; set;  }
        // public List<coupon_detail_list> coupon_detail_list { get; set; }
    }

    public class consignee_info {
        public string fullname { get; set; }
        public string telephone { get; set; }
        public string mobile { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string full_address { get; set; }

    }

    public class item_info_list {
        public string sku_id { get; set; }
        public string outer_sku_id { get; set; }
        public string sku_name { get; set; }
        public string jd_price { get; set; }
        public string gift_point { get; set; }
        public string ware_id { get; set; }
        public string item_total { get; set; }
    }

    public class coupon_detail_list {
        public string order_id { get; set; }
        public string sku_id { get; set; }
        public string coupon_type { get; set; }
        public string coupon_price { get; set; }

    }

    public class BuyOrderGetResponse
    {  //获取某个订单
        public order_get_response order_get_response { get; set; }
    }
    public class order_get_response
    {
        public order order { get; set; }
    }

    public class order
    {
        public order_info_list orderInfo { get; set; }
    }

    public class LocalOrderFrom {
        public string Oid { get; set; }
        public string OrderStartTime { get; set; }
        public string BuyerID { get; set; }
        public string PayID { get; set; }
        public string Payment { get; set; }
        public string PayTime { get; set; }
        public string OrderRemark { get; set; }
        public string BuyerMsg { get; set; }
        public string Freight { get; set; }
        public string PayAmount { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerMobile { get; set; }
        public string BuyerProvince { get; set; }
        public string BuyerCity { get; set; }
        public string BuyerCounty { get; set; }
        public string BuyerAddress { get; set; }
        public string ZipCode { get; set; }
        public string WebSubOid { get; set; }
        public string WebGoodsCode { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string ColorAndSize { get; set; }
        public string Count { get; set; }
        public string GoodPrice { get; set; }
        public string GoodAmount { get; set; }
        public string InvoiceTitle { get; set; }
        public string ExCompany { get; set; }
        public string ExID { get; set; }
        public string SendTime { get; set; }
        public string State { get; set; }


    }

    public class SystemP
    {
        public string url { get; set; }
        public string access_token { get; set; }
        public string app_key { get; set; }
        public string sign { get; set; }
        public string timestamp { get; set; }
        public string v { get; set; }
        public string app_secret { get; set; }

    }


   












}
