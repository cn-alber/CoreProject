namespace CoreModels.XyComm
{
    public class wareThirdParty{
        public int id{get;set;}
        public string warename{get;set;}
        public string ourremark{get;set;}
        public string otherremark{get;set;}
        public bool enabel{get;set;}
        public int  soure{get;set;} 

    }

    public class openWareRequset{
        public string username{get;set;}
        public string pwd{get;set;}
        public string warename{get;set;}
        public string wareadmin{get;set;}
    }



}