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
        public List<int> _ShopID= null;//店铺
        public bool _IsDisSelectAll = false;//分销商是否全部选中
        public List<string> _Distributor=null;//分销商
        public List<int> _ExID = null;//快递公司
        public List<string> _SendWarehouse = null;//仓库
        public string _SortField;//排序栏位
        public string _SortDirection;//排序方式
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
        public List<string> SendWarehouse
        {
            get { return _SendWarehouse; }
            set { this._SendWarehouse = value;}
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
        public int id {get;set;}
        public int mergeoid{get;set;}
        public int type{get;set;}
        public int dealertype{get;set;}
        public bool ismerge{get;set;}
        public bool issplit{get;set;}
        public int osource{get;set;}
        public DateTime odate{get;set;}
        public int coid{get;set;}
        public int buyerid{get;set;}
        public string buyershopid{get;set;}
        public int shopid{get;set;}
        public string shopname{get;set;}
        public string shopsit{get;set;}
        public long soid{get;set;}
        public long mergesoid{get;set;}
        public int ordqty{get;set;}
        public string amount{get;set;}
        public string skuamount{get;set;}
        public string paidamount{get;set;}
        public string payamount{get;set;}
        public string examount{get;set;}
        public bool isinvoice{get;set;}
        public string invoicetype{get;set;}
        public string invoicetitle{get;set;}
        public DateTime invoicedate{get;set;}
        public bool ispaid{get;set;}
        public DateTime paydate{get;set;}
        public string paynbr{get;set;}
        public bool iscod{get;set;}
        public int status{get;set;}
        public int abnormalstatus{get;set;}
        public string statusdec{get;set;}
        public string shopstatus{get;set;}
        public string recname{get;set;}
        public string reclogistics{get;set;}
        public string reccity{get;set;}
        public string recdistrict{get;set;}
        public string recaddress{get;set;}
        public string reczip{get;set;}
        public string rectel{get;set;}
        public string recphone{get;set;}
        public string recmessage{get;set;}
        public string sendmessage{get;set;}
        public string express{get;set;}
        public int exid{get;set;}
        public string excode{get;set;}
        public string excost{get;set;}
        public string exweight{get;set;}
        public string distributor{get;set;}
        public string supdistributor{get;set;}
        public DateTime plandate{get;set;}
        public string sendwarehouse{get;set;}
        public DateTime senddate{get;set;}
        public string creator{get;set;}
        public DateTime createdate{get;set;}
        public string modifier{get;set;}
        public DateTime modifydate{get;set;}
    }
    public class OrderData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<Order> Ord {get;set;}//订单资料List
    }
}