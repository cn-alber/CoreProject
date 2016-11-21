namespace CoreModels.XyComm
{
    public class wareThirdParty{
        public int id{get;set;}
        public int coid{get;set;}
        public int itcoid{get;set;}
        public int applycoid{get;set;}
        public string warename{get;set;}
        public string itname{get;set;}
        public string myremark{get;set;}
        public string itremark{get;set;}
        public int enable{get;set;}
        //public int  source{get;set;} 
        public string mdate{get;set;}
        // public string pdate{get;set;}
        // public string enddate{get;set;}
        // public string endman{get;set;}
        

    }

    public class wareLst{
        public int id{get;set;}
        public int coid{get;set;}
        public string warename{get;set;}
    }

    public class wareInfo{
        public int coid{get;set;}
        public string itcoid{get;set;}
        public string warename{get;set;}

    }

    public class wareploylist{
        public int id{get;set;}
        public string name{get;set;}

    }



    public class itWare{
        public int id{get;set;}
        public int coid{get;set;}
        public string warename{get;set;}
        public string itname{get;set;}
        public string myremark{get;set;}
        public string itremark{get;set;}
        public int enable{get;set;}
        //public int  source{get;set;} 
        //public string cdate{get;set;}
        public string mdate{get;set;}
    }



    public class openWareRequset{
        public string username{get;set;}
        public string pwd{get;set;}
        public string warename{get;set;}
        public string wareadmin{get;set;}
    }

    public class remarkSqlRes{
        public string Coid{get;set;}
        public string ItCoid{get;set;}
        public string ApplyCoid{get;set;}

        public string MyRemark{get;set;}
        public string ItRemark{get;set;}
        public string WareName{get;set;}
        public string ItName{get;set;}
    }

    public class editRemarkRequest{
        public string id{get;set;}
        public string remark{get;set;}
    }



}