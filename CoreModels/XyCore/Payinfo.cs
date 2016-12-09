using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class PayInfo
    {
        public int ID{get;set;}
        public string PayNbr{get;set;}
        public int RecID{get;set;}
        public string RecName{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public string Payment{get;set;}
        public string PayAccount{get;set;}
        public string SellerAccount{get;set;}
        public string Platform{get;set;}
        public DateTime PayDate{get;set;}
        public string Bank{get;set;}
        public string BankName{get;set;}
        public string Title{get;set;}
        public string Name{get;set;}
        public string Amount{get;set;}
        public string PayAmount{get;set;}
        public string DiscountFree{get;set;}
        public int DataSource{get;set;}
        public int Status{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Confirmer{get;set;}
        public DateTime ConfirmDate{get;set;}
        public string BuyerShopID{get;set;}
    }
    public class ImportPayinfo
    {
        public string PayNbr{get;set;}//必输
        public string Payment{get;set;}//必输
        public string PayAccount{get;set;}
        public string SellerAccount{get;set;}
        public string Platform{get;set;}
        public DateTime PayDate{get;set;}//必输
        public string Bank{get;set;}
        public string BankName{get;set;}
        public string Title{get;set;}
        public string Name{get;set;}
        public string Amount{get;set;}
        public string PayAmount{get;set;}//必输
    }
    public class PayInfoParm
    {
        public int _CoID ;//公司id
        public int _ID = 0;//内部支付单号
        public int _OID = 0;//内部订单号
        public long _SoID = 0;//线上订单号
        public string _PayNbr = null;//付款单号
        public DateTime _DateStart = DateTime.Parse("1900-01-01");//日期起
        public DateTime _DateEnd = DateTime.Parse("1900-01-01");//日期迄
        public int _Status = -1;//状态
        public string _BuyerShopID = null;//买家账号
        public string _Payment = null;//支付方式
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
        public string PayNbr
        {
            get { return _PayNbr; }
            set { this._PayNbr = value;}
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
        public string BuyerShopID
        {
            get { return _BuyerShopID; }
            set { this._BuyerShopID = value;}
        }
        public string Payment
        {
            get { return _Payment; }
            set { this._Payment = value;}
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
    public class PayInfoQuery
    {
        public int ID{get;set;}
        public string PayDate{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public string PayNbr{get;set;}
        public string PayAmount{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
        public string Payment{get;set;}
        public string PayAccount{get;set;}
        public string BuyerShopID{get;set;}
    }
    public class PayInfoData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<PayInfoQuery> Pay {get;set;}
    }
}