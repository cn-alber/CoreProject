using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class PurchaseReceive
    {
        public int id{get;set;}
        public string receiveid{get;set;}
        public string purchaseid{get;set;}
        public int warehouseid{get;set;}
        public string warehousename{get;set;}
        public DateTime receivedate{get;set;}
        public string skuid{get;set;}
        public string skuname{get;set;}
        public string norm{get;set;}
        public int status{get;set;}
        public decimal recqty{get;set;}
        public string creator{get;set;}
        public DateTime createdate{get;set;}
    }
    // public class PurchaseDetail
    // {
    //     public int id{get;set;}
    //     public string purchaseid{get;set;}
    //     public string skuid{get;set;}
    //     public string skuname{get;set;}
    //     public string colorname{get;set;}
    //     public string sizename{get;set;}
    //     public decimal purqty{get;set;}
    //     public decimal price{get;set;}
    //     public decimal puramt{get;set;}
    //     public string remark{get;set;}
    //     public string goodscode{get;set;}
    //     public string supplynum{get;set;}
    //     public DateTime recievedate{get;set;}
    //     public int detailstatus{get;set;}
    //     public string norm {get;set;}
    //     public int coid {get;set;}
    // }
    public class PurchaseReceiveParm
    {
        public int CoID {get;set;}//公司id
        public string Recid{get;set;}//收料单号
        public string Purid {get;set;}//采购单号
        public string Skuname {get;set;}//商品名称
        public string Warehousename{get;set;}//仓库名称
        public int Status{get;set;}//状态
        public string SortField{get;set;}//排序栏位
        public string SortDirection{get;set;}//排序方式
        public int NumPerPage{get;set;}//每页显示资料笔数
        public int PageIndex{get;set;}//页码
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<PurchaseReceive> Rec {get;set;}//采购收料单资料List
    }
}