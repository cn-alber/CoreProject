using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class Purchase
    {
        public int id{get;set;}
        public string purchaseid{get;set;}
        public DateTime purchasedate{get;set;}
        public string coname{get;set;}
        public string contract{get;set;}
        public string shpaddress{get;set;}
        public int status{get;set;}
        public string purtype{get;set;}
        public string buyyer{get;set;}
        public string remark{get;set;}
        public decimal taxrate{get;set;}
    }
    public class PurchaseDetail
    {
        public int id{get;set;}
        public string purchaseid{get;set;}
        public string skuid{get;set;}
        public string skuname{get;set;}
        public string colorname{get;set;}
        public string sizename{get;set;}
        public decimal purqty{get;set;}
        public decimal price{get;set;}
        public decimal puramt{get;set;}
        public string remark{get;set;}
        public string goodscode{get;set;}
        public string supplynum{get;set;}
        public DateTime recievedate{get;set;}
        public int detailstatus{get;set;}
        public string norm {get;set;}
        public int coid {get;set;}
    }
    public class PurchaseParm
    {
        public int CoID {get;set;}//公司id
        public string Purid {get;set;}//采购单号
        public DateTime PurdateStart{get;set;}//采购日期起
        public DateTime PurdateEnd{get;set;}//采购日期迄
        public int Status{get;set;}//状态
        public string CoName{get;set;}//供应商
        public string SortField{get;set;}//排序栏位
        public string SortDirection{get;set;}//排序方式
        public int NumPerPage{get;set;}//每页显示资料笔数
        public int PageIndex{get;set;}//页码
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<Purchase> Com {get;set;}//采购单资料List
    }
    public class PurchaseDetailParm
    {
        public int CoID {get;set;}//公司id
        public string Purid {get;set;}//采购单号
        public string Skuid{get;set;}//商品编码
        public string SkuName{get;set;}//商品名称
        public string GoodsCode{get;set;}//款式编号
        public string SortField{get;set;}//排序栏位
        public string SortDirection{get;set;}//排序方式
        public int NumPerPage{get;set;}//每页显示资料笔数
        public int PageIndex{get;set;}//页码
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public bool enable {get;set;} //明细是否允许修改
        public List<PurchaseDetail> Com {get;set;}//采购单明细资料List
    }
}