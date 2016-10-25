using System.Collections.Generic;

namespace CoreModels.XyApi.Tmall
{
    public class sellercats_list_get_response_re{
        public sellercats_list_get_response sellercats_list_get_response{get;set;}
    }

    public class sellercats_list_get_response{
        public seller_cats seller_cats{get;set;}
    }

    public class seller_cats{
        public List<cat_item> seller_cat{get;set;}
    }
    
    public class  seller_cat{
        public List<cat_item> cat_item{get;set;}
    }

    public class  cat_item{
        public long cid{get;set;}
        public string name{get;set;}
        public long parent_cid{get;set;}
        public string pic_url{get;set;}
        public int  sort_order{get;set;}
        public string type{get;set;}
        public List<cat_item> children{get;set;}
    }

    



}