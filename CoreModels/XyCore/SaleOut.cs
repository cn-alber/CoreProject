using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class SaleOutInsert
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public int BatchID{get;set;}
        public long SoID{get;set;}
        public DateTime DocDate{get;set;}
        public int Status{get;set;}
        public string ExpName{get;set;}
        public string ExCode{get;set;}
        public string RecMessage{get;set;}
        public string RecLogistics{get;set;}
        public string RecDistrict{get;set;}
        public string RecCity{get;set;}
        public string RecAddress{get;set;}
        public string RecZip{get;set;}
        public string RecName{get;set;}
        public string RecPhone{get;set;}
        public string ExWeight{get;set;}
        public string RealWeight{get;set;}
        public string ExCost{get;set;}
        public string Amount{get;set;}
        public int OrdQty{get;set;}
        public string Remark{get;set;}
        public string SendWarehouse{get;set;}
        public DateTime PayDate{get;set;}
        public string SendMessage{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public string Modifier{get;set;}
        public int ExID{get;set;}
        public int ShopID{get;set;}
    }
    public class SaleOutItemInsert
    {
        public int ID{get;set;}
        public int SID{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public int SkuAutoID{get;set;}
        public string SkuID{get;set;}
        public string SkuName{get;set;}
        public string Norm{get;set;}
        public string GoodsCode{get;set;}
        public int Qty{get;set;}
        public string SalePrice{get;set;}
        public string RealPrice{get;set;}
        public string Amount{get;set;}
        public string DiscountRate{get;set;}
        public string img{get;set;}
        public string ShopSkuID{get;set;}
        public string Weight{get;set;}
        public string TotalWeight{get;set;}
        public bool IsGift{get;set;}
        public string Remark{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public string Modifier{get;set;}
    }
    public class SaleOutParm
    {
        public int _CoID ;//公司id
        public int _ID = 0;//出库单号
        public int _OID = 0;//内部订单号
        public long _SoID = 0;//线上订单号
        public string _ExCode = null;//快递单号
        public DateTime _DateStart = DateTime.Parse("1900-01-01");//日期起
        public DateTime _DateEnd = DateTime.Parse("1900-01-01");//日期迄
        public int _Status = -1;//状态
        public string _IsWeightYN = "A";//是否称重
        public string _SkuID = null;//商品编码
        public string _GoodsCode = null;//款式编码
        public int _ExID = 0;//快递
        public string _IsExpPrint = "A";//快递打印否
        public int _ShopID = -1;//店铺ID
        public string _RecName = null;//收货人
        public int _BatchID = -1;//批次号
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
        public string ExCode
        {
            get { return _ExCode; }
            set { this._ExCode = value;}
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
        public string IsWeightYN
        {
            get { return _IsWeightYN; }
            set { this._IsWeightYN = value;}
        }
        public string SkuID
        {
            get { return _SkuID; }
            set { this._SkuID = value;}
        }
        public string GoodsCode
        {
            get { return _GoodsCode; }
            set { this._GoodsCode = value;}
        }
        public int ExID
        {
            get { return _ExID; }
            set { this._ExID = value;}
        }
        public string IsExpPrint
        {
            get { return _IsExpPrint; }
            set { this._IsExpPrint = value;}
        }
        public int ShopID
        {
            get { return _ShopID; }
            set { this._ShopID = value;}
        }
        public string RecName
        {
            get { return _RecName; }
            set { this._RecName = value;}
        }
        public int BatchID
        {
            get { return _BatchID; }
            set { this._BatchID = value;}
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
    public class SaleOutQuery
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public string DocDate{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
        public string ExpName{get;set;}
        public string ExCode{get;set;}
        public int BatchID{get;set;}
        public bool IsOrdPrint{get;set;}
        public bool IsExpPrint{get;set;}
        public string RecMessage{get;set;}
        public string RecLogistics{get;set;}
        public string RecDistrict{get;set;}
        public string RecCity{get;set;}
        public string RecAddress{get;set;}
        public string RecName{get;set;}
        public string RecPhone{get;set;}
        public string ExWeight{get;set;}
        public string RealWeight{get;set;}
        public string ShipType{get;set;}
        public string ExCost{get;set;}
        public bool IsDeliver{get;set;}
        public string Remark{get;set;}
        public string Sku{get;set;}
        public int OrdQty{get;set;}
    }
    public class SaleOutData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<SaleOutQuery> SaleOut {get;set;}//订单资料List
    }
}