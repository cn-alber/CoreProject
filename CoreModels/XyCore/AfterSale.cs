using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreModels.XyCore
{
    public class ASInitData
    {
        public List<Filter> Shop {get;set;}//店铺
        public List<Filter> Status{get;set;}//售后状态
        public List<Filter> Type{get;set;}//售后类型
        public List<Filter> OrdType {get;set;} //订单类型
        public List<Filter> Distributor {get;set;}//分销商
        public List<Filter> IssueType{get;set;} //问题类型
        public List<Filter> Result{get;set;}//处理结果
        public List<Filter> ShopStatus{get;set;}//线上状态
        public List<Filter> GoodsStatus{get;set;}//货物状态
        public List<Filter> RefundStatus{get;set;}//退款状态
    }
    public class AfterSaleParm
    {
        public int _CoID ;//公司id
        public string _ExCode = null;//快递单号
        public long _SoID = 0;//线上订单号
        public int _OID = 0;//内部订单号
        public int _ID = 0;//售后单号
        public string _BuyerShopID = null;//买家账号
        public string _RecName = null;//收货人
        public string _Modifier = null;//修改人
        public string _RecPhone = null;//手机
        public string _RecTel = null;//固定电话
        public string _Creator = null;//制单人     
        public string _Remark = null;//备注
        public string _DateType = "RegisterDate";//日期过滤基准
        public DateTime _DateStart = DateTime.Parse("1900-01-01");//日期起
        public DateTime _DateEnd = DateTime.Parse("1900-01-01");//日期迄
        public string _SkuID = null;//商品编码
        public string _GoodsCode = null;//款式编码
        public string _IsNoOID = "A";//是否无信息件
        public string _IsInterfaceLoad = "A";//接口下载
        public string _IsSubmitDis = "A";//分销提交
        public int _ShopID = -1;//店铺
        public int _Status = -1;//状态
        public string _GoodsStatus = null;//货物状态
        public int _Type = -1;//售后分类
        public int _OrdType = -1;//订单类型
        public List<string> _ShopStatus = null;//淘宝状态
        public List<string> _RefundStatus = null;//退款状态
        public int _Distributor = -1;//分销商
        public string _IsSubmit = "A";//供销提交
        public int _IssueType = -1;//问题类型
        public int _Result = -1;//处理结果
        public string _SortField = "id";//排序栏位
        public string _SortDirection = "DESC";//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public string ExCode
        {
            get { return _ExCode; }
            set { this._ExCode = value;}
        }
        public long SoID
        {
            get { return _SoID; }
            set { this._SoID = value;}
        }
        public int OID
        {
            get { return _OID; }
            set { this._OID = value;}
        }
        public int ID
        {
            get { return _ID; }
            set { this._ID = value;}
        }
        public string Modifier
        {
            get { return _Modifier; }
            set { this._Modifier = value;}
        }
        public string BuyerShopID
        {
            get { return _BuyerShopID; }
            set { this._BuyerShopID = value;}
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
        public string Creator
        {
            get { return _Creator; }
            set { this._Creator = value;}
        }
        public string Remark
        {
            get { return _Remark; }
            set { this._Remark = value;}
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
        public string IsNoOID
        {
            get { return _IsNoOID; }
            set { this._IsNoOID = value;}
        }
        public string IsInterfaceLoad
        {
            get { return _IsInterfaceLoad; }
            set { this._IsInterfaceLoad = value;}
        }
        public string IsSubmitDis
        {
            get { return _IsSubmitDis; }
            set { this._IsSubmitDis = value;}
        }
        public int ShopID
        {
            get { return _ShopID; }
            set { this._ShopID = value;}
        }
        public int Status
        {
            get { return _Status; }
            set { this._Status = value;}
        }
        public string GoodsStatus
        {
            get { return _GoodsStatus; }
            set { this._GoodsStatus = value;}
        }
        public int Type
        {
            get { return _Type; }
            set { this._Type = value;}
        }
        
        public int OrdType
        {
            get { return _OrdType; }
            set { this._OrdType = value;}
        }
        public List<string> ShopStatus
        {
            get { return _ShopStatus; }
            set { this._ShopStatus = value;}
        }
        public List<string> RefundStatus
        {
            get { return _RefundStatus; }
            set { this._RefundStatus = value;}
        }
        public int Distributor
        {
            get { return _Distributor; }
            set { this._Distributor = value;}
        }
        public string IsSubmit
        {
            get { return _IsSubmit; }
            set { this._IsSubmit = value;}
        }
        public int IssueType
        {
            get { return _IssueType; }
            set { this._IssueType = value;}
        }
        public int Result
        {
            get { return _Result; }
            set { this._Result = value;}
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
    public class AfterSale
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public long SoID{get;set;}
        public DateTime RegisterDate{get;set;}
        public string BuyerShopID{get;set;}
        public string RecName{get;set;}
        public int Type{get;set;}
        public string RecTel{get;set;}
        public string RecPhone{get;set;}
        public decimal SalerReturnAmt{get;set;}
        public decimal BuyerUpAmt{get;set;}
        public decimal RealReturnAmt{get;set;}
        public string ReturnAccount{get;set;}
        public int ShopID{get;set;}
        public string ShopName{get;set;}
        public int WarehouseID{get;set;}
        public string RecWarehouse{get;set;}
        public int IssueType{get;set;}
        public int OrdType{get;set;}
        public string Remark{get;set;}
        public int Status{get;set;}
        public string ShopStatus{get;set;}
        public string GoodsStatus{get;set;}
        public string RefundStatus{get;set;}
        public int Result{get;set;}
        public string Express{get;set;}
        public string ExCode{get;set;}
        public bool IsSubmit{get;set;}
        public bool IsSubmitDis{get;set;}
        public bool IsInterfaceLoad{get;set;}
        public string Distributor{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Modifier{get;set;}
        public DateTime ModifyDate{get;set;}
        public DateTime ConfirmDate{get;set;}
    }
    public class AfterSaleQuery
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public string RegisterDate{get;set;}
        public string BuyerShopID{get;set;}
        public string RecName{get;set;}
        public int Type{get;set;}
        public string TypeString{get;set;}
        public string RecPhone{get;set;}
        public decimal SalerReturnAmt{get;set;}
        public decimal BuyerUpAmt{get;set;}
        public decimal RealReturnAmt{get;set;}
        public string ReturnAccount{get;set;}
        public string ShopName{get;set;}
        public int WarehouseID{get;set;}
        public string RecWarehouse{get;set;}
        public long SoID{get;set;}
        public int IssueType{get;set;}
        public string IssueTypeString{get;set;}
        public int OrdType{get;set;}
        public string OrdTypeString{get;set;}
        public string Remark{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
        public string ShopStatus{get;set;}
        public int Result{get;set;}
        public string ResultString{get;set;}
        public string GoodsStatus{get;set;}
        public string ModifyDate{get;set;}
        public string Modifier{get;set;}
        public string Creator{get;set;}
        public string RefundStatus{get;set;}
        public string Express{get;set;}
        public string ExCode{get;set;}
        public bool IsSubmit{get;set;}        
        public string ConfirmDate{get;set;}
    }
    public class AfterSaleData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<AfterSaleQuery> AfterSale {get;set;}
        public List<Filter> Type{get;set;}//售后类型
        public List<Filter> Warehouse{get;set;}
        public List<Filter> Result{get;set;}//处理结果
    }
    public class ASOrderParm
    {
        public int _CoID ;//公司id
        public int _ID = 0;//内部订单号
        public long _SoID = 0;//线上订单号
        public string _PayNbr = null;//付款单号
        public string _ExCode = null;//快递单号
        public DateTime _DateStart = DateTime.Parse("1900-01-01");//日期起
        public DateTime _DateEnd = DateTime.Parse("1900-01-01");//日期迄
        public string _Distributor=null;//分销商
        public int _ExID = 0;//快递公司
        public int _ShopID= -1;//店铺
        public int _SendWarehouse = 0;//仓库
        public string _BuyerShopID = null;//买家账号
        public string _RecName = null;//收货人
        public string _RecPhone = null;//手机
        public string _RecTel = null;//固定电话
        public int _Status = -1;//状态
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
        public int Status
        {
            get { return _Status; }
            set { this._Status = value;}
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
        public int ShopID
        {
            get { return _ShopID; }
            set { this._ShopID = value;}
        }
        public string Distributor
        {
            get { return _Distributor; }
            set { this._Distributor = value;}
        }
        public int ExID
        {
            get { return _ExID; }
            set { this._ExID = value;}
        }
        public int SendWarehouse
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
    public class ASOrderQuery
    {
        public int ID {get;set;}
        public int Type{get;set;}
        public string TypeString{get;set;}
        public string ODate{get;set;}
        public string BuyerShopID{get;set;}
        public string ShopName{get;set;}
        public long SoID{get;set;}
        public string Amount{get;set;}
        public string PaidAmount{get;set;}
        public string ExAmount{get;set;}
        public string RecName{get;set;}
        public string RecLogistics{get;set;}
        public string RecCity{get;set;}
        public string RecDistrict{get;set;}
        public string RecAddress{get;set;}
        public string RecTel{get;set;}
        public string RecPhone{get;set;}
        public string RecMessage{get;set;}
        public string SendMessage{get;set;}
        public string Express{get;set;}
        public string ExCode{get;set;}
        // public List<SkuList> SkuList{get;set;}
    }
    public class ASOrderData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<ASOrderQuery> Ord {get;set;}//订单资料List
    }
    public class ASOrderInit
    {
        public List<Filter> IssueType{get;set;}
        public List<Filter> Type{get;set;}
        public List<Filter> Warehouse{get;set;}
        public int DefaultWare {get;set;}
    }
    public class ASOrderItem
    {
        public int ID{get;set;}
        public string SkuID{get;set;}
        public string SkuName{get;set;}
        public string Norm{get;set;}
        public int Qty{get;set;}
        public string SalePrice{get;set;}
        public string RealPrice{get;set;}
        public string Amount{get;set;}
        public string DiscountRate{get;set;}
        public string img{get;set;}
        public bool IsGift{get;set;}
    }
    public class AfterSaleItem
    {
        public int ID{get;set;}
        public int RID{get;set;}
        public int ReturnType{get;set;}
        public int SkuAutoID{get;set;}
        public string SkuID{get;set;}
        public string SkuName{get;set;}
        public string Norm{get;set;}
        public string GoodsCode{get;set;}
        public int RegisterQty{get;set;}
        public int ReturnQty{get;set;}
        public decimal Price{get;set;}
        public decimal Amount{get;set;}
        public string img{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Modifier{get;set;}
        public DateTime ModifyDate{get;set;}
    }
    public class AfterSaleItemQuery
    {
        public int ID{get;set;}
        public int ReturnType{get;set;}
        public string ReturnTypeString{get;set;}
        public int SkuAutoID{get;set;}
        public string SkuID{get;set;}
        public string SkuName{get;set;}
        public string Norm{get;set;}
        public string GoodsCode{get;set;}
        public int RegisterQty{get;set;}
        public int ReturnQty{get;set;}
        public string Price{get;set;}
        public string Amount{get;set;}
        public string img{get;set;}
        public string Creator{get;set;}
    }
    public class InsertASItemSkuReturn
    {
        public List<AfterSaleItemQuery> SuccessIDs{get;set;}
        public List<InsertFailReason> FailIDs{get;set;}
    }
    public class AfterSaleEdit
    {
        public int ID{get;set;}
        public int OID{get;set;}
        public string ShopName{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
        public string GoodsStatus{get;set;}
        public long SoID{get;set;}
        public string BuyerShopID{get;set;}
        public string RecName{get;set;}
        public string RecTel{get;set;}
        public string RecPhone{get;set;}
        public int Type{get;set;}
        public string TypeString{get;set;}
        public int IssueType{get;set;}
        public string IssueTypeString{get;set;}
        public string RegisterDate{get;set;}
        public string SalerReturnAmt{get;set;}
        public string BuyerUpAmt{get;set;}
        public string RealReturnAmt{get;set;}
        public string ReturnAccount{get;set;}
        public int WarehouseID{get;set;}
        public string RecWarehouse{get;set;}
        public string Express{get;set;}
        public string ExCode{get;set;}
        public string Remark{get;set;}
    }
    public class AfterSaleEditReturn
    {
        public AfterSaleEdit AfterSale{get;set;}
        public List<Filter> IssueType{get;set;}
        public List<Filter> Type{get;set;}
        public List<Filter> Warehouse{get;set;}
        public List<AfterSaleItemQuery> AfterSaleItem{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class UpdateAfterSaleEReturn
    {
        public AfterSaleEdit AfterSale{get;set;}
        public List<AfterSaleItemQuery> AfterSaleItem{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class InsertASItemOrderEReturn
    {
        public List<AfterSaleItemQuery> AfterSaleItem{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class InsertASItemSkuEReturn
    {
        public List<AfterSaleItemQuery> SuccessIDs{get;set;}
        public List<InsertFailReason> FailIDs{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class BindOrdReturn
    {
        public List<AfterSaleQuery> SuccessIDs{get;set;}
        public List<InsertFailReason> FailIDs{get;set;}
    }
    public class RefreshASReturn
    {
        public AfterSaleQuery AfterSale{get;set;}
        public List<AfterSaleItemQuery> AfterSaleItem{get;set;}
    }
    public class GetAfterSaleItemReturn
    {
        public int Status{get;set;}
        public int Type{get;set;}
        public int OID{get;set;}
        public List<AfterSaleItemQuery> AfterSaleItem{get;set;}
    }
    public class CancleAfterSaleSuccess
    {
        public int ID{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
    }
    public class CancleAfterSaleReturn
    {
        public List<CancleAfterSaleSuccess> SuccessIDs{get;set;}
        public List<InsertFailReason> FailIDs{get;set;}
    }
    public class AgressReturnSuccess
    {
        public int ID{get;set;}
        public string ShopStatus{get;set;}
    }
    public class AgressReturn
    {
        public List<AgressReturnSuccess> SuccessIDs{get;set;}
        public List<InsertFailReason> FailIDs{get;set;}
    }
    public class ConfirmAfterSaleSuccess
    {
        public int ID{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
        public string Modifier{get;set;}
        public string ModifyDate{get;set;}
        public string ConfirmDate{get;set;}
    }
    public class ConfirmAfterSaleReturn
    {
        public List<ConfirmAfterSaleSuccess> SuccessIDs{get;set;}
        public List<InsertFailReason> FailIDs{get;set;}
    }
    public class ConfirmAfterSaleEEReturn
    {
        public AfterSaleEdit AfterSale{get;set;}
        public List<OrderLog> Log{get;set;}
    }
    public class ConfirmGoodsSuccess
    {
        public int ID{get;set;}
        public string GoodsStatus{get;set;}
        public string Modifier{get;set;}
        public string ModifyDate{get;set;}
    }
    public class ConfirmGoodsReturn
    {
        public List<ConfirmGoodsSuccess> SuccessIDs{get;set;}
        public List<InsertFailReason> FailIDs{get;set;}
    }
}