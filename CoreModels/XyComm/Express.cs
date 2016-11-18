using System;
using System.Collections.Generic;
namespace CoreModels.XyComm
{
    public class Express
    {
        public int ID { get; set; }
        public string ExpID { get; set; }
        public string ExpName { get; set; }
        public bool Enable { get; set; }
        public string Priority { get; set; }
        public string PriorityLogistics { get; set; }
        public string PrioritySku { get; set; }
        public string FreightFirst { get; set; }
        public string OrdAmtStart { get; set; }
        public string OrdAmtEnd { get; set; }
        public bool IsCOD { get; set; }
        public string LimitedShop { get; set; }
        public string LimitedWarehouse { get; set; }
        public string DisableArea { get; set; }
        public string DisableSku { get; set; }
        public bool IgnoreArrival { get; set; }
        public string ExpCalMethod { get; set; }
        public string UseProbability { get; set; }
        public bool OnlineOrder { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifyDate { get; set; }
    }
    public class ExpressQuery
    {
        public int ID { get; set; }
        public string ExpID { get; set; }
        public string ExpName { get; set; }
        public int Priority { get; set; }
        public string PriorityLogistics { get; set; }
        public int FreightFirst { get; set; }
        public decimal OrdAmtStart { get; set; }
        public decimal OrdAmtEnd { get; set; }
        public string PrioritySku { get; set; }
        public string DisableArea { get; set; }
        public int ExpCalMethod { get; set; }
        public bool IgnoreArrival { get; set; }
        public bool Enable { get; set; }        
        public bool IsCOD { get; set; }
        public string ModifyDate { get; set; }
    }
    public class ExpressData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<ExpressQuery> Exp {get;set;}//订单资料List
    }
    public class ExpressEdit
    {
        public int ID { get; set; }
        public string ExpName { get; set; }
        public bool Enable { get; set; }
        public string Priority { get; set; }
        public string PriorityLogistics { get; set; }
        public string PrioritySku { get; set; }
        public string FreightFirst { get; set; }
        public string OrdAmtStart { get; set; }
        public string OrdAmtEnd { get; set; }
        public bool IsCOD { get; set; }
        public string LimitedShop { get; set; }
        public string LimitedWarehouse { get; set; }
        public string DisableArea { get; set; }
        public string DisableSku { get; set; }
        public bool IgnoreArrival { get; set; }
        public string ExpCalMethod { get; set; }
        public string UseProbability { get; set; }
        public bool OnlineOrder { get; set; }
    }
    // public class ExpFeeQuery
    // {
    //     public int ID { get; set; }
    //     public string ExpName { get; set; }
    // }
    // public class ExpFeeDetailQuery
    // {
    //     public int ID{get;set;}
    //     public string Destination{get;set;}
    //     public string WeightStart{get;set;}
    //     public string WeightEnd{get;set;}
    //     public string WeightFirst{get;set;}
    //     public string AmtFirst{get;set;}
    //     public string WeightAdd{get;set;}
    //     public string AmtAdd{get;set;}
    // }
}