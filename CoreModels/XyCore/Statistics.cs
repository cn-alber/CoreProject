using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class orderStatic
    {
        public int Status{get;set;}
        public String Name{get;set;}
        public int Num{get;set;}
        public decimal Amount{get;set;}
    }

    public class lastday_7{
        public string SkuID{get;set;}
        public int Qty{get;set;}
    }
    public class lastday_15{
        public decimal SoID{get;set;}
        public string D{get;set;}
        public decimal Pay{get;set;}
        public int Qty{get;set;}
    }

    public class AreaSale{
        public decimal SoID{get;set;}
        public string RecLogistics{get;set;}
        public decimal Amount{get;set;}
    }
   
   public class areaItem{
       public decimal SoID{get;set;}
       public decimal Amount{get;set;}
   }

}