using System;
namespace CoreModels.XyMessage
{
    public class Worklog
    {
        public int ID{get;set;}
       public string BarCode{get;set;}
       public string SkuID{get;set;}
       public string BoxCode{get;set;}
       public int WarehouseID{get;set;}
       public int qty{get;set;}
       public string PCode{get;set;}
       public string Contents{get;set;}
       public string RecordID{get;set;}
       public int Type{get;set;}
       public int CoID{get;set;}
       public string Creator{get;set;}
       public DateTime CreateDate{get;set;}
    }
}