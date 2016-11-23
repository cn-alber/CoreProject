using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreModels.XyCore
{
    public class OrderParm
    {
        public int _CoID ;//公司id
        public int _ID = 0;//内部订单号
        public long _SoID = 0;//线上订单号
        public string _PayNbr = null;//付款单号
        public string _BuyerShopID = null;//买家账号
        public string _ExCode = null;//快递单号
        public string _RecName = null;//收货人
        public string _RecPhone = null;//手机
        public string _RecTel = null;//固定电话
        public string _RecLogistics = null;//收货人省
        public string _RecCity = null;//收货人城市
        public string _RecDistrict = null;//收货人区县
        public string _RecAddress = null;//详细地址
        public List<int> _StatusList = null;//状态List
        public List<int> _AbnormalStatusList = null;//异常状态List
        public string _IsRecMsgYN = "A";//是否过滤买家留言
        public string _RecMessage = null;//买家留言
        public string _IsSendMsgYN = "A";//是否过滤卖家留言
        public string _SendMessage = null;//卖家留言
        public string _datetype = "odate";//日期过滤基准
        public DateTime _dateStart = DateTime.Parse("1900-01-01");//日期起
        public DateTime _dateEnd = DateTime.Parse("2999-12-31");//日期迄
        public string _Skuid = null;//商品编码
        public string _GoodsCode = null;//款式编码 add 2016-11-12
        public int _ordqtystart = 0;//数量起
        public int _ordqtyend = 0;//数量迄
        public decimal _ordamtstart = 0;//金额起
        public decimal _ordamtend = 0;//金额迄
        public string _skuname = null;//商品名称
        public string _norm = null;//商品规格
        public List<string> _ShopStatus = null;//淘宝状态
        public int _osource = -1;//订单来源
        public List<int> _type = null;//订单类型
        public string _IsCOD = "A";//是否货到付款
        public string _IsPaid = "A";//是否付款 add 2016-11-12
        public bool _IsShopSelectAll = false;//店铺是否全部选中 add 2016-11-12     
        public List<int> _ShopID= null;//店铺
        public bool _IsDisSelectAll = false;//分销商是否全部选中
        public List<string> _Distributor=null;//分销商
        public List<int> _ExID = null;//快递公司
        public List<int> _SendWarehouse = null;//仓库
        public List<int> _Others = null;//其他 add 2016-11-12
        public string _SortField = "id";//排序栏位
        public string _SortDirection = "DESC";//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public long SoID
        {
            get { return _SoID; }
            set { this._SoID = value;}
        }
        public int ID
        {
            get { return _ID; }
            set { this._ID = value;}
        }
        public string PayNbr
        {
            get { return _PayNbr; }
            set { this._PayNbr = value;}
        }
        public string BuyerShopID
        {
            get { return _BuyerShopID; }
            set { this._BuyerShopID = value;}
        }
        public string ExCode
        {
            get { return _ExCode; }
            set { this._ExCode = value;}
        }
        public string RecName
        {
            get { return _RecName; }
            set { this._RecName = value;}
        }
        public string RecPhone
        {
            get { return _RecPhone; }
            set { this._RecPhone = value;}
        }
        public string RecTel
        {
            get { return _RecTel; }
            set { this._RecTel = value;}
        }
        public string RecLogistics
        {
            get { return _RecLogistics; }
            set { this._RecLogistics = value;}
        }
        public string RecCity
        {
            get { return _RecCity; }
            set { this._RecCity = value;}
        }
        public string RecDistrict
        {
            get { return _RecDistrict; }
            set { this._RecDistrict = value;}
        }
        public string RecAddress
        {
            get { return _RecAddress; }
            set { this._RecAddress = value;}
        }
        public List<int> StatusList
        {
            get { return _StatusList; }
            set { this._StatusList = value;}
        }
        public List<int> AbnormalStatusList
        {
            get { return _AbnormalStatusList; }
            set { this._AbnormalStatusList = value;}
        }
        public string IsRecMsgYN
        {
            get { return _IsRecMsgYN; }
            set { this._IsRecMsgYN = value;}
        }
        public string RecMessage
        {
            get { return _RecMessage; }
            set { this._RecMessage = value;}
        }
        public string IsSendMsgYN
        {
            get { return _IsSendMsgYN; }
            set { this._IsSendMsgYN = value;}
        }
        public string SendMessage
        {
            get { return _SendMessage; }
            set { this._SendMessage = value;}
        }
        public string Datetype
        {
            get { return _datetype; }
            set { this._datetype = value;}
        }
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { this._dateStart = value;}
        }
        public DateTime DateEnd
        {
            get { return _dateEnd; }
            set { this._dateEnd = value;}
        }
        public string Skuid
        {
            get { return _Skuid; }
            set { this._Skuid = value;}
        }
        public string GoodsCode
        {
            get { return _GoodsCode; }
            set { this._GoodsCode = value;}
        }
        public int Ordqtystart
        {
            get { return _ordqtystart; }
            set { this._ordqtystart = value;}
        }
        public int Ordqtyend
        {
            get { return _ordqtyend; }
            set { this._ordqtyend = value;}
        }
        public decimal Ordamtstart
        {
            get { return _ordamtstart; }
            set { this._ordamtstart = value;}
        }
        public decimal Ordamtend
        {
            get { return _ordamtend; }
            set { this._ordamtend = value;}
        }
        public string Skuname
        {
            get { return _skuname; }
            set { this._skuname = value;}
        }
        public string Norm
        {
            get { return _norm; }
            set { this._norm = value;}
        }
        public List<string> ShopStatus
        {
            get { return _ShopStatus; }
            set { this._ShopStatus = value;}
        }
        public int OSource
        {
            get { return _osource; }
            set { this._osource = value;}
        }
        public List<int> Type
        {
            get { return _type; }
            set { this._type = value;}
        }
        public string IsCOD
        {
            get { return _IsCOD; }
            set { this._IsCOD = value;}
        }
        public string IsPaid
        {
            get { return _IsPaid; }
            set { this._IsPaid = value;}
        }
        public bool IsShopSelectAll
        {
            get { return _IsShopSelectAll; }
            set { this._IsShopSelectAll = value;}
        }
        public List<int> ShopID
        {
            get { return _ShopID; }
            set { this._ShopID = value;}
        }
        public bool IsDisSelectAll
        {
            get { return _IsDisSelectAll; }
            set { this._IsDisSelectAll = value;}
        }
        public List<string> Distributor
        {
            get { return _Distributor; }
            set { this._Distributor = value;}
        }
        public List<int> ExID
        {
            get { return _ExID; }
            set { this._ExID = value;}
        }
        public List<int> SendWarehouse
        {
            get { return _SendWarehouse; }
            set { this._SendWarehouse = value;}
        }
        public List<int> Others
        {   
            get { return _Others; }
            set { this._Others = value;}
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
    public class Order
    {
        public int ID {get;set;}
        public int MergeOID{get;set;}
        public int Type{get;set;}
        public int DealerType{get;set;}
        public bool IsMerge{get;set;}
        public bool IsSplit{get;set;}
        public int OSource{get;set;}
        public DateTime ODate{get;set;}
        public int CoID{get;set;}
        public int BuyerID{get;set;}
        public string BuyerShopID{get;set;}
        public int ShopID{get;set;}
        public string ShopName{get;set;}
        public string ShopSit{get;set;}
        public long SoID{get;set;}
        public long MergeSoID{get;set;}
        public string CurrencyType{get;set;}
        public decimal ExchangeRate{get;set;}
        public int OrdQty{get;set;}
        public string Amount{get;set;}
        public string SkuAmount{get;set;}
        public string PaidAmount{get;set;}
        public string PayAmount{get;set;}
        public string ExAmount{get;set;}
        public bool IsInvoice{get;set;}
        public string InvoiceType{get;set;}
        public string InvoiceTitle{get;set;}
        public DateTime InvoiceDate{get;set;}
        public bool IsPaid{get;set;}
        public DateTime PayDate{get;set;}
        public string PayNbr{get;set;}
        public bool IsCOD{get;set;}
        public int Status{get;set;}
        public int AbnormalStatus{get;set;}
        public string StatusDec{get;set;}
        public string ShopStatus{get;set;}
        public string RecName{get;set;}
        public string RecCountry{get;set;}
        public string RecLogistics{get;set;}
        public string RecCity{get;set;}
        public string RecDistrict{get;set;}
        public string RecAddress{get;set;}
        public string RecZip{get;set;}
        public string RecTel{get;set;}
        public string RecPhone{get;set;}
        public string RecMessage{get;set;}
        public string SendMessage{get;set;}
        public string Express{get;set;}
        public int ExID{get;set;}
        public string ExCode{get;set;}
        public string ExCost{get;set;}
        public string ExWeight{get;set;}
        public string Distributor{get;set;}
        public string SupDistributor{get;set;}
        public DateTime PlanDate{get;set;}
        public string WarehouseID{get;set;}
        public string SendWarehouse{get;set;}
        public DateTime SendDate{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Modifier{get;set;}
        public DateTime ModifyDate{get;set;}
    }
    public class OrderData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<OrderQuery> Ord {get;set;}//订单资料List
    }
    public class RecInfo
    {
        public int ID{get;set;}
        public string BuyerId{get;set;}
        public string Receiver{get;set;}
        public string Tel{get;set;}
        public string Phone{get;set;}
        public string Address{get;set;}
        public string Logistics{get;set;}
        public string City{get;set;}
        public string District{get;set;}
        public string ZipCode{get;set;}
        public string Express{get;set;}
        public int ExID{get;set;}
        public string ExCode{get;set;}
        public decimal ExCost{get;set;}
        public decimal Weight{get;set;}
        public int OID{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public string ShopSit{get;set;}
    }
    public class RecInfoParm
    {
        public int _CoID ;//公司id
        public string _BuyerId = null;//买家账号
        public string _Receiver = null;//收货人
        public string _ShopSit = null;//来源平台
        public string _SortField;//排序栏位
        public string _SortDirection;//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public string BuyerId
        {
            get { return _BuyerId; }
            set { this._BuyerId = value;}
        }
        public string Receiver
        {
            get { return _Receiver; }
            set { this._Receiver = value;}
        }
        public string ShopSit
        {
            get { return _ShopSit; }
            set { this._ShopSit = value;}
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
    public class RecInfoData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<RecInfo> Recinfo {get;set;}
    }
    public class Log
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public int Type{get;set;}
        public DateTime LogDate{get;set;}
        public string UserName{get;set;}
        public string Title{get;set;}
        public string Remark{get;set;}
        public int CoID{get;set;}
    }
    public class OrderDetailInsert
    {
        public List<int> successIDs {get;set;}
        public List<InsertFailReason> failIDs {get;set;}
        public SingleOrderItem Order{get;set;}
    }
    public class OrderItem
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public int CoID{get;set;}
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
        public string Weight{get;set;}
        public string TotalWeight{get;set;}
        public bool IsGift{get;set;}
        public string ShopSkuID{get;set;}
        public string Remark{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Modifier{get;set;}
        public DateTime ModifyDate{get;set;}
    }
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
    }
    public class AbnormalReason
    {
        public int ID{get;set;}
        public string Name{get;set;}
    }
    public class MergerOrd
    {
        public string type{get;set;}//H高风险;M中风险;L推荐项;A主订单
        public List<Order> MOrd{get;set;}
    }
    public class SplitOrd
    {
        public int Skuid{get;set;}
        public int Qty{get;set;}
        public int QtyNew{get;set;}
        public decimal Price{get;set;}
        public decimal Weight{get;set;}
    }
    public class OrdInitData
    {
        public List<OStatus> OrdStatus{get;set;}//订单状态
        public List<OStatus> OrdAbnormalStatus{get;set;}//异常状态
        public List<Filter> BuyerRemark{get;set;} //买家留言
        public List<Filter> SellerRemark{get;set;}//卖家备注
        public List<Filter> OSource {get;set;}//订单来源
        public List<Filter> OType {get;set;} //订单类型
        public List<Filter> LoanType{get;set;}//贷款类型
        public List<Filter> IsPaid{get;set;}//是否付款
        public List<Filter> Shop {get;set;}//店铺
        public List<Filter> Distributor {get;set;}//分销商
        public List<Filter> Express {get;set;}//快递
        public List<Filter> Warehouse{get;set;}//仓库
        public List<Filter> Others {get;set;}//其他
    }
    public class OStatus
    {
        public string value{get;set;}
        public string label{get;set;}
        public int count{get;set;}
    }
    public class Filter
    {
        public string value{get;set;}
        public string label{get;set;}
    }
    public class OrderQuery
    {
        public int ID {get;set;}
        public int Type{get;set;}
        public string TypeString{get;set;}
        public int DealerType{get;set;}
        public bool IsMerge{get;set;}
        public bool IsSplit{get;set;}
        public int OSource{get;set;}
        public long SoID{get;set;}
        public string ODate{get;set;}
        public string PayDate{get;set;}
        public string BuyerShopID{get;set;}
        public string ShopName{get;set;}
        public string Amount{get;set;}
        public string PaidAmount{get;set;}
        public string ExAmount{get;set;}
        public bool IsCOD{get;set;}
        public string RecTel{get;set;}
        public string RecPhone{get;set;}
        public int Status{get;set;}
        public string StatusDec{get;set;}
        public int AbnormalStatus{get;set;}
        public string AbnormalStatusDec{get;set;}
        public string RecMessage{get;set;}
        public string SendMessage{get;set;}
        public string Express{get;set;}
        public string RecLogistics{get;set;}
        public string RecCity{get;set;}
        public string RecDistrict{get;set;}
        public string RecAddress{get;set;}
        public string RecName{get;set;}
        public string ExWeight{get;set;}
        public string Distributor{get;set;}
        public string SupDistributor{get;set;}
        public string InvoiceTitle{get;set;}
        public string PlanDate{get;set;}
        public string SendWarehouse{get;set;}
        public string SendDate{get;set;}
        public int ExID{get;set;}
        public string ExpNamePinyin{get;set;}
        public string ExCode{get;set;}
        public string Creator{get;set;}
        public List<long> SoIDList{get;set;}
        public List<SkuList> SkuList{get;set;}
    }
    public class SkuList
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public int SkuAutoID{get;set;}
        public string Img{get;set;}
        public int Qty{get;set;}
        public string GoodsCode{get;set;}
        public string SkuID{get;set;}
        public string SkuName{get;set;}
        public string Norm{get;set;}
        public string RealPrice{get;set;}
        public string Amount{get;set;}
        public string ShopSkuID{get;set;}
        public bool IsGift{get;set;}
        public string Weight{get;set;}
        public int InvQty{get;set;}
    }
    public class StatusCount
    {
        public List<OStatusCnt> OrdStatus{get;set;}
        public List<OStatusCnt> OrdAbnormalStatus{get;set;}
    }
    public class OStatusCnt
    {
        public int value{get;set;}
        public int count{get;set;}
    }
    public class ImportOrderInsert
    {
        public int Type{get;set;}//必输
        public int OSource{get;set;}//必输
        public DateTime ODate{get;set;}//必输
        public string BuyerShopID{get;set;}//必输
        public string ShopName{get;set;}//必输
        public long SoID{get;set;}//必输
        public string Amount{get;set;}//必输
        public string PaidAmount{get;set;}
        public string PayAmount{get;set;}
        public string ExAmount{get;set;}
        public bool IsInvoice{get;set;}
        public string InvoiceType{get;set;}
        public string InvoiceTitle{get;set;}
        public DateTime InvoiceDate{get;set;}
        public bool IsCOD{get;set;}//必输
        public string ShopStatus{get;set;}
        public string RecName{get;set;}//必输
        public string RecLogistics{get;set;}//必输
        public string RecCity{get;set;}//必输
        public string RecDistrict{get;set;}//必输
        public string RecAddress{get;set;}//必输
        public string RecZip{get;set;}
        public string RecTel{get;set;}
        public string RecPhone{get;set;}//必输
        public string RecMessage{get;set;}
        public string Distributor{get;set;}
        public string SupDistributor{get;set;}
        public List<ImportOrderItemDetail> Item{get;set;}//必输
        public List<ImportPayinfo> Pay{get;set;}
    }
    public class ImportOrderItemDetail
    {
        public string SkuID{get;set;}//必输
        public int Qty{get;set;}//必输
        public decimal Price{get;set;}//必输
        public decimal Amount{get;set;}//必输
        public string Remark{get;set;}
        public string ShopSkuID{get;set;}
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
    public class ImportOrderUpdate
    {
        public long SoID{get;set;}//必输
        public string ShopStatus{get;set;}
        public List<ImportPayinfo> Pay{get;set;}
    }
    public class SingleOrderItem
    {
        public string Amount{get;set;}
        public int Status{get;set;}
        public string StatusDec{get;set;}
        public string Weight{get;set;}
        public List<SkuList> SkuList{get;set;}
    }
    public class OrderSingle
    {
        public OrderEdit Order{get;set;}
        public List<OrderPay> Pay{get;set;}
        public List<OrderItemEdit> OrderItem{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class OrderEdit
    {
        public int ID {get;set;}
        public string ShopName{get;set;}
        public string ODate{get;set;}
        public string OSource{get;set;}
        public long SoID{get;set;}
        public string PayDate{get;set;}
        public string BuyerShopID{get;set;}
        public string ExAmount{get;set;}
        public string Express{get;set;}
        public string ExCode{get;set;}
        public string RecMessage{get;set;}
        public string SendMessage{get;set;}
        public string InvoiceTitle{get;set;}
        public string RecLogistics{get;set;}
        public string RecCity{get;set;}
        public string RecDistrict{get;set;}
        public string RecAddress{get;set;}
        public string RecName{get;set;}
        public string RecTel{get;set;}
        public string RecPhone{get;set;}
        public int Status{get;set;}
        public string StatusDec{get;set;}
        public int AbnormalStatus{get;set;}
        public string AbnormalStatusDec{get;set;}
        public int ExID{get;set;}
        public string ExpNamePinyin{get;set;}
        public string SkuAmount{get;set;}
        public string Amount{get;set;}
        public string PaidAmount{get;set;}
    }
    public class OrderPay
    {
        public int ID{get;set;}
        public string PayNbr{get;set;}
        public string Payment{get;set;}
        public string PayDate{get;set;}
        public string PayAmount{get;set;}
        public int Status{get;set;}
    }
    public class OrderItemEdit
    {
        public int ID{get;set;}
        public int SkuAutoID{get;set;}
        public string SkuID{get;set;}
        public string SkuName{get;set;}
        public string Norm{get;set;}
        public string GoodsCode{get;set;}
        public int Qty{get;set;}
        public string SalePrice{get;set;}
        public string RealPrice{get;set;}
        public string Amount{get;set;}
        public string img{get;set;}
        public bool IsGift{get;set;}
        public string ShopSkuID{get;set;}
        public int InvQty{get;set;}
    }
    public class OrderLog
    {
        public string ID{get;set;}
        public string LogDate{get;set;}
        public string UserName{get;set;}
        public string Title{get;set;}
        public string Remark{get;set;}
    }
    public class UpdateOrd
    {
        public OrderEdit Order{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class UpdatePay
    {
        public OrderEdit Order{get;set;}
        public List<OrderPay> Pay{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class CanclePay
    {
        public List<OrderPay> Pay{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class RefreshItem
    {
        public OrderEdit Order{get;set;}
        public List<OrderItemEdit> OrderItem{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class OrderDetailInsertI
    {
        public List<int> successIDs {get;set;}
        public List<InsertFailReason> failIDs {get;set;}
        public RefreshItem Order{get;set;}
    }
    public class Inv
    {
        public int Skuautoid{get;set;}
        public int InvQty{get;set;}
    }
    public class SetExp
    {
        public List<ExpressSimple> Express;
        public List<LogisticsNetwork> LogisticsNetwork;
    }
    public class LogisticsNetwork
    {
        public string kd_name{get;set;}
        public string cp_name_raw{get;set;}
        public string cp_location{get;set;}
        public string delivery_contact{get;set;}
        public string delivery_area_1{get;set;}
        public string delivery_area_0{get;set;}
    }
    public class OrdWhStrategy
    {
        public int ID{get;set;}
        public string Name{get;set;}
        public string Level{get;set;}
        public string Wid{get;set;}
        public string Wname{get;set;}
        public string Province{get;set;}
        public string Shopid{get;set;}
        public string Did{get;set;}
        public string ContainSkus{get;set;}
        public string RemoveSkus{get;set;}
        public string ContainGoods{get;set;}
        public string RemoveGoods{get;set;}
        public string MinNum{get;set;}
        public string MaxNum{get;set;}
        public int Payment{get;set;}
        public int CoID{get;set;}
    }
    public class OrdSum
    {
        public int QtyTot{get;set;}
        public decimal AmtTot{get;set;}
        public decimal WeightTot{get;set;}
    }
    public class TransferNormalReturnSuccess
    {
        public int ID{get;set;}
        public int Status{get;set;}
        public string  StatusDec{get;set;}
    }
    public class TransferNormalReturnFail
    {
        public int ID{get;set;}
        public string Reason{get;set;}
    }
    public class TransferNormalReturn
    {
        public List<TransferNormalReturnSuccess> SuccessIDs{get;set;}
        public List<TransferNormalReturnFail> FailIDs{get;set;}
    }
    public class ModifyFreightSuccess
    {
        public int ID{get;set;}
        public int Status{get;set;}
        public string  StatusDec{get;set;}
        public int AbnormalStatus{get;set;}
        public string AbnormalStatusDec{get;set;}
        public string ExAmount{get;set;}
        public string Amount{get;set;}
    }
    public class ModifyFreightReturn
    {
        public List<ModifyFreightSuccess> SuccessIDs{get;set;}
        public List<TransferNormalReturnFail> FailIDs{get;set;}
    }
    public class SetExpSuccess
    {
        public int ID{get;set;}
        public string ExID{get;set;}
        public string  Express{get;set;}
        public string ExpNamePinyin{get;set;}
    }
    public class SetExpReturn
    {
        public List<SetExpSuccess> SuccessIDs{get;set;}
        public List<TransferNormalReturnFail> FailIDs{get;set;}
    }
    public class SetWarehouseSuccess
    {
        public int ID{get;set;}
        public string  Warehouse{get;set;}
    }
    public class SetWarehouseReturn
    {
        public List<SetWarehouseSuccess> SuccessIDs{get;set;}
        public List<TransferNormalReturnFail> FailIDs{get;set;}
    }
    public class SaleOutInsert
    {
        public int ID{get;set;}
        public int OID{get;set;}
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
    public class ExpnoUnused
    {
        public string ExCode{get;set;}
    }
    public class OrderAbnormal
    {
        public string value{get;set;}
        public string label{get;set;}
        public bool iscustom{get;set;}
    }
    public class TransferAbnormalReturn
    {
        public List<int> successIDs {get;set;}
        public List<TransferNormalReturnFail> failIDs {get;set;}
    }
}