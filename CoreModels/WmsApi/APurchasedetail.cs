using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
     public class APurchaseDetail
    {
        public int id {get;set;}
        public int purchaseid{get;set;}
        public string img{get;set;}
        public int skuautoid{get;set;}
        public string skuid{get;set;}
        public string skuname{get;set;}
        public string purqty{get;set;}
        public string suggestpurqty{get;set;}
        public string recqty{get;set;}
        public string price{get;set;}
        public string puramt{get;set;}
        public string returnqty{get;set;}
        public int detailstatus{get;set;}
        public string remark{get;set;}
        public string goodscode{get;set;}
        public string supplynum{get;set;}
        public string supplycode{get;set;}
        public string planqty{get;set;}
        public string planamt{get;set;}
        public string _recievedate;
        public string norm {get;set;}
        public string packingnum {get;set;}
        public int coid{get;set;}
        public string creator{get;set;}
        public string modifier{get;set;}
        public DateTime createdate{get;set;}
        public DateTime modifydate{get;set;}
        public string recievedate
        {
            get { return _recievedate; }
            set { this._recievedate = value.ToString().Substring(0,10);}
        }
    }
}