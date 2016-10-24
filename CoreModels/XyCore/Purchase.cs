using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreModels.XyCore
{
    public class Purchase
    {
        public int id{get;set;}
        public string _purchasedate {get;set;}
        public int scoid {get;set;}
        public string sconame{get;set;}
        public string contract{get;set;}
        public string shplogistics{get;set;}
        public string shpcity{get;set;}
        public string shpdistrict{get;set;}
        public string shpaddress{get;set;}
        public int warehouseid{get;set;}
        public string warehousename{get;set;}
        public int status{get;set;}
        public string purtype{get;set;}
        public string buyyer{get;set;}
        public string remark{get;set;}
        public string taxrate{get;set;}
        public int coid{get;set;}
        public string creator{get;set;}
        public string modifier{get;set;}
        public DateTime createdate{get;set;}
        public DateTime modifydate{get;set;}
        public string purchasedate
        {
            get { return _purchasedate; }
            set { this._purchasedate = value.ToString().Substring(0,10);}
        }
    }
    public class PurchaseDetail
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
    public class PurchaseParm
    {
        public int _CoID ;//公司id
        public int _Purid = -1;//采购单号
        public DateTime _PurdateStart = DateTime.Parse("1900-01-01");//采购日期起
        public DateTime _PurdateEnd = DateTime.Parse("2999-12-31");//采购日期迄
        public int _Status = -1;//状态
        public int _Scoid = 0;//供应商
        public string _Skuid = null;//商品编码
        public int _Warehouseid = 0;//仓库代号
        public string _Buyyer = null;//采购员
        public string _SortField;//排序栏位
        public string _SortDirection;//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public int Purid
        {
            get { return _Purid; }
            set { this._Purid = value;}
        }
        public DateTime PurdateStart
        {
            get { return _PurdateStart; }
            set { this._PurdateStart = value;}
        }
        public DateTime PurdateEnd
        {
            get { return _PurdateEnd; }
            set { this._PurdateEnd = value;}
        }
        public int Status
        {
            get { return _Status; }
            set { this._Status = value;}
        }
        public int Scoid
        {
            get { return _Scoid; }
            set { this._Scoid = value;}
        }
        public int Warehouseid
        {
            get { return _Warehouseid; }
            set { this._Warehouseid = value;}
        }
        public string Skuid
        {
            get { return _Skuid; }
            set { this._Skuid = value;}
        }
        public string Buyyer
        {
            get { return _Buyyer; }
            set { this._Buyyer = value;}
        }
        public string SortField
        {
            get { return _SortField; }
            set { this._SortField = value;}
        }
        public string SortDirection
        {
            get { return _SortDirection; }
            set { this._SortDirection = value;}
        }
        public int NumPerPage
        {
            get { return _NumPerPage; }
            set { this._NumPerPage = value;}
        }
        public int PageIndex
        {
            get { return _PageIndex; }
            set { this._PageIndex = value;}
        }
    }
    public class PurchaseDetailParm
    {
        public int _CoID ;//公司id
        public int _Purid ;//采购单号
        public string _Skuid;//商品编码
        public string _SkuName;//商品名称
        public string _GoodsCode;//款式编号
        public string _SortField ;//排序栏位
        public string _SortDirection;//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public int Purid
        {
            get { return _Purid; }
            set { this._Purid = value;}
        }
        public string Skuid
        {
            get { return _Skuid; }
            set { this._Skuid = value;}
        }
        public string SkuName
        {
            get { return _SkuName; }
            set { this._SkuName = value;}
        }
        public string GoodsCode
        {
            get { return _GoodsCode; }
            set { this._GoodsCode = value;}
        }
        public string SortField
        {
            get { return _SortField; }
            set { this._SortField = value;}
        }
        public string SortDirection
        {
            get { return _SortDirection; }
            set { this._SortDirection = value;}
        }
        public int NumPerPage
        {
            get { return _NumPerPage; }
            set { this._NumPerPage = value;}
        }
        public int PageIndex
        {
            get { return _PageIndex; }
            set { this._PageIndex = value;}
        }
    }

    public class CalPurchase
    {
        public decimal purqty{get;set;}
        public decimal puramt{get;set;}
    }
    public class PurchaseData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<Purchase> Pur {get;set;}//采购单资料List
    }
    public class PurchaseDetailData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public int status {get;set;} //采购单状态
        public List<PurchaseDetail> Pur {get;set;}//采购单明细资料List
    }
    public class QualityRev
    {
        public int id {get;set;}
        public int purchaseid{get;set;}
        public string _recorddate;
        public string recorder {get;set;}
        public string drawrate {get;set;}
        public string type {get;set;}
        public string  conclusion {get;set;}
        public string remark {get;set;}
        public int status {get;set;}
        public int coid{get;set;}
        public string creator{get;set;}
        public string modifier{get;set;}
        public DateTime createdate{get;set;}
        public DateTime modifydate{get;set;}
        public string recorddate
        {
            get { return _recorddate; }
            set { this._recorddate = value.ToString().Substring(0,10);}
        }
    }
    public class PurchaseInitData
    {
        public Dictionary<int,string> status{get;set;}//状态
        public List<Warehouse> warehouse {get;set;}//仓库列表
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<Purchase> Pur {get;set;}//采购单资料List
    }
    public class QualityRevData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<QualityRev> Qua {get;set;}//资料List
    }
    public class InsertFailReason
    {
        public int id {get;set;}
        public string reason{get;set;}
    }
    public class PurchaseDetailInsert
    {
        public List<int> successIDs {get;set;}
        public List<InsertFailReason> failIDs {get;set;}
    }
    public class SkuInsert
    {
        public string skuid { get; set; }
        public string skuname { get; set; }
        public string norm { get; set; }
        public string img { get; set; }
        public string goodscode { get; set; }
        public bool enable{ get; set; }
    }
}