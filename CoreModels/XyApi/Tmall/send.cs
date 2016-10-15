namespace CoreModels.XyApi.Tmall
{
    public class onlineConfirm{
        public string tid{get;set;}
        public string sub_tid{get;set;}
        public string is_split{get;set;}
        public string out_sid{get;set;}
        public string seller_ip{get;set;}
        public string token{get;set;}
    }

    public class offlineSend{
        public string sub_tid{get;set;}
        public string tid{get;set;}
        public string is_split{get;set;}
        public string out_sid{get;set;}
        public string company_code{get;set;}
        public string sender_id{get;set;}
        public string cancel_id{get;set;}
        public string feature{get;set;}
        public string seller_ip{get;set;}
        public string token{get;set;}
    }

    public class dummySend{
        public string token{get;set;}
        public string tid{get;set;}
        public string feature{get;set;}
        public string seller_ip{get;set;}
    }


    
}