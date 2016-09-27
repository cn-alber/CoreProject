using System.Collections.Generic;

namespace CoreModels.XyApi.JingDong
{
    public class jdSkuAddModel {  //增加Sku信息
        public wareSkuAddResponse ware_sku_add_response { get; set; }
    }

    public class wareSkuAddResponse
    {
        public string created { get; set; }
        public string sku_id { get; set; }
        public string code { get; set; }
    }

    public class jdSkuUpdateModel {  //修改Sku信息
        public wareSkuUpdateResponse ware_sku_update_response { get; set; }
    }
    public class wareSkuUpdateResponse {
        public string created { get; set; }
        public string sku_id { get; set; }
        public string code { get; set; }
    }

    public class jdSkuDeleteModel { //删除Sku

        public wareSkuDeleteResponse ware_sku_delete_response { get; set; }
    }
    public class wareSkuDeleteResponse {
        public string modified { get; set; }
        public string sku_id { get; set; }
        public string code { get; set; }
    }

    public class jdCustomGetModel{ //根据外部ID获取商品SKU

        public skuCustomGetResponse sku_custom_get_response { get; set; }
    }
    public class skuCustomGetResponse {
        public string code { get; set; }
        public skuModel sku { get; set; }
    }

    public class skuModel {
        public string shop_id { get; set; }
        public string cost_price { get; set; }
        public string status { get; set; }
        public string outer_id { get; set; }
        public string color_value { get; set; }
        public string stock_num { get; set; }
        public string modified { get; set; }
        public string jd_price { get; set; }
        public string market_price { get; set; }
        public string ware_id { get; set; }
        public string created { get; set; }
        public string sku_id { get; set; }
        public string attributes { get; set; }
        public string size_value { get; set; }

    }

    public class jdSkusGetModel { //根据商品ID列表获取商品SKU信息
        public wareSkusGetResponse ware_skus_get_response { get; set; }
    }

    public class wareSkusGetResponse {
        public List<skuModel> skus { get; set; }
    }

    public class jdSkuGetModel {//获取单个SKU信息
        public wareSkuGetResponse ware_sku_get_response { get; set; }
    }
    public class wareSkuGetResponse {
        public skuModel sku { get; set; }
    }

    public class jdListingGetModel { //获取上架商品信息
        public wareListingGetResponse ware_listing_get_response { get; set; }

    }
    public class wareListingGetResponse
    {
        public int total { get; set; }
        public List<wareInfo> ware_infos { get; set; }
    }

    public class wareInfo {
        public string title { get; set; }
        public string attributes { get; set; }
        public string logo { get; set; }
        public string creator { get; set; }
        public string status { get; set; }
        public string weight { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string ware_id { get; set; }
        public string spu_id { get; set; }
        public string cid { get; set; }
        public string vender_id { get; set; }
        public string shop_id { get; set; }
        public string ware_status { get; set; }
        public string item_num { get; set; }
        public string upc_code { get; set; }
        public string transport_id { get; set; }
        public string online_time { get; set; }
        public string offline_time { get; set; }
        public string cost_price { get; set; }
        public string market_price { get; set; }
        public string jd_price { get; set; }
        public string stock_num { get; set; }


    }

    public class jdDelistingGetModel {  //获取下家商品信息
        public wareDelistingGetResponse ware_delisting_get_response { get; set; }
    }

    public class wareDelistingGetResponse {
        public int total { get; set; }
        public List<wareInfo> ware_infos { get; set; }
    }

    public class jdSearchSkuListModel { //Sku搜索服务
        public jdSearchSkuListModelResponce jingdong_sku_read_searchSkuList_responce { get; set; }

    }


    public class jdSearchSkuListModelResponce {
        public jdSearchSkuListModelResponcePage page { get; set; }
    }
    public class jdSearchSkuListModelResponcePage {
        public List<jdSearchSkuListData> data { get; set; }
        public string pageNo { get; set; }
        public string pageSize { get; set; }
        public string totalItem { get; set; }

    }

    public class jdSearchSkuListData {
        public string created { get; set; }
        public decimal jdPrice { get; set; }
        public string outerId { get; set; }
        public List<SkuSaleAttr> saleAttrs { get; set; }
        public string skuId { get; set; }
        public string status { get; set; }
        public int stockNum { get; set; }
        public string wareTitle { get; set; }
    }

    public class SkuSaleAttr {
        public string attrId { get; set; }
        public List<string> attrValueAlias { get; set; }
        public List<string> attrValues { get; set; }
    }


    public class jdFindSkuByIdModel
    { // 获取单个Sku
        public jdFindSkuByIdModelResponce jingdong_sku_read_findSkuById_responce { get; set; }

    }

    public class jdFindSkuByIdModelResponce
    {
        public jdSearchSkuListData sku { get; set; }
    }

}
