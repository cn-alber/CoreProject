namespace CoreModels.XyApi.Tmall
{
    public class skuAddRequest{
        public string token {get;set;}
        public string	num_iid	{get;set;}
        public string	properties	{get;set;}
        public string	quantity	{get;set;}
        public string	price	{get;set;}
        public string	outer_id	{get;set;}
        public string	item_price	{get;set;}
        public string	lang	{get;set;}
        public string	spec_id	{get;set;}
        public string	sku_hd_length	{get;set;}
        public string	sku_hd_height	{get;set;}
        public string	sku_hd_lamp_quantity	{get;set;}
        public string	ignorewarning	{get;set;}
    }

    public class skuUpdateRequest{
        public string token {get;set;}
        public string	num_iid	{get;set;}
        public string	properties	{get;set;}
        public string	quantity	{get;set;}
        public string	price	{get;set;}
        public string	outer_id	{get;set;}
        public string	item_price	{get;set;}
        public string	lang	{get;set;}
        public string	spec_id	{get;set;}
        public string   barcode{get;set;}
        public string ignorewarning{get;set;}
    }

    public class skuDeleteRequest{
        public string token{get;set;}
        public string num_iid{get;set;}
        public string properties{get;set;}
        public string item_price{get;set;}
        public string item_num{get;set;}
        public string lang{get;set;}
        public string ignorewarning{get;set;}

    }

    







}