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
        public string Distributor{get;set;}
    }
    public class RefundInfoParm
    {
        public int _CoID ;//公司id
        public int _ID = 0;//内部支付单号
        public int _OID = 0;//内部订单号
        public long _SoID = 0;//线上订单号
        public string _RefundNbr = null;//退款单号
        public string _DateType = "RefundDate";
        public DateTime _DateStart = DateTime.Parse("1900-01-01");//日期起
        public DateTime _DateEnd = DateTime.Parse("1900-01-01");//日期迄
        public int _Status = -1;//状态
        public int _ShopID = -1;
        public string _BuyerShopID = null;//买家账号
        public string _Refundment = null;//支付方式
        public string _Distributor = null;//支付方式
        public string _SortField = "id";//排序栏位
        public string _SortDirection = "DESC";//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public int ID
        {
            get { return _ID; }
            set { this._ID = value;}
        }
        public int OID
        {
            get { return _OID; }
            set { this._OID = value;}
        }
        public long SoID
        {
            get { return _SoID; }
            set { this._SoID = value;}
        }
        public string RefundNbr
        {
            get { return _RefundNbr; }
            set { this._RefundNbr = value;}
        }
        public string DateType
        {
            get { return _DateType; }
            set { this._DateType = value;}
        }
        public DateTime DateStart
        {
            get { return _DateStart; }
            set { this._DateStart = value;}
        }
        public DateTime DateEnd
        {
            get { return _DateEnd; }
            set { this._DateEnd = value;}
        }
        public int Status
        {
            get { return _Status; }
            set { this._Status = value;}
        }
        public int ShopID
        {
            get { return _ShopID; }
            set { this._ShopID = value;}
        }
        public string BuyerShopID
        {
            get { return _BuyerShopID; }
            set { this._BuyerShopID = value;}
        }
        public string Refundment
        {
            get { return _Refundment; }
            set { this._Refundment = value;}
        }
        public string Distributor
        {
            get { return _Distributor; }
            set { this._Distributor = value;}
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
    public class RefundInfoQuery
    {
        public string ShopName{get;set;}
        public int ID{get;set;}
        public string RefundDate{get;set;}
        public string ConfirmDate{get;set;}
        public string ModifyDate{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public string RefundNbr{get;set;}
        public string Amount{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
        public string Refundment{get;set;}
        public string PayAccount{get;set;}
        public string BuyerShopID{get;set;}
        public int RID{get;set;}
        public int RType{get;set;}
        public string RTypeString{get;set;}
        public int IssueType{get;set;}
        public string IssueTypeString{get;set;}
        public string RRmark{get;set;}
    }
    public class RefundInfoData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<RefundInfoQuery> Refund {get;set;}
        public List<Filter> Payment{get;set;}
    }
}