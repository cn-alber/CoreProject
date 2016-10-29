namespace CoreModels.XyComm
{
    public class wareThirdParty{
        public int id{get;set;}
        public string warename{get;set;}
        public string ourremark{get;set;}
        public string otherremark{get;set;}
        public int enabel{get;set;}
        public int  soure{get;set;} 
        public string cdate{get;set;}
        public string mdate{get;set;}
        public string pdate{get;set;}
        public string enddate{get;set;}
        public string endman{get;set;}
        

    }

    public class openWareRequset{
        public string username{get;set;}
        public string pwd{get;set;}
        public string warename{get;set;}
        public string wareadmin{get;set;}
    }

    public class remarkSqlRes{
        public string ThirdCode{get;set;}
        public string Code{get;set;}

    }



}