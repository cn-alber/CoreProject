using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class RefundInfo
    {
        public int ID{get;set;}
        public DateTime RefundDate{get;set;}
        public string RefundNbr{get;set;}
        public int ShopID{get;set;}
        public string ShopName{get;set;}
        public string BuyerShopID{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public string Refundment{get;set;}
        public string PayAccount{get;set;}
        public decimal Amount{get;set;}
        public int Status{get;set;}
        public int RID{get;set;}
        public int RType{get;set;}
        public int IssueType{get;set;}
        public string RRmark{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Modifier{get;set;}
        public DateTime ModifyDate{get;set;}
        public string Confirmer{get;set;}
        public DateTime ConfirmDate{get;set;}
    }
}