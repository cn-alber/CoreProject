namespace CoreModels.XyUser
{
    public class Business
    {
        public int id {get;set;}
        public int coid {get;set;}
        public bool ismergeorder {get;set;}
        public bool isautosetexpress {get;set;}
        public bool isignoresku {get;set;}
        public bool isautogoodsreviewed {get;set;}
        public bool isupdateskuall {get;set;}
        public bool isupdatepresalesku {get;set;}
        public bool isskulock {get;set;}
        public bool ispresaleskulock {get;set;}
        public bool ischeckfirst {get;set;}
        public bool isjustcheckex {get;set;}
        public bool isautosendafftercheck {get;set;}
        public bool isneedkg {get;set;}
        public bool isautoremarks {get;set;}
        public bool isexceptions {get;set;}
        public int cabinetheight {get;set;}
        public int cabinetnumber {get;set;}
        public bool ispositionaccurate {get;set;}
        public bool goodsuniquecode {get;set;}
        public bool isgoodsrule {get;set;}
        public bool isbeyondcount {get;set;}
        public bool pickingmethod {get;set;}
        public bool tempnominus {get;set;}
        public bool mixedpicking {get;set;}
    }
}