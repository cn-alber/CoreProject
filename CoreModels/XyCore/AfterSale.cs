using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreModels.XyCore
{
    public class ASInitData
    {
        public List<Filter> Shop {get;set;}//店铺
        public List<Filter> ASStatus{get;set;}//售后状态
        public List<Filter> ASType{get;set;}//售后类型
        public List<Filter> OrdType {get;set;} //订单类型
        public List<Filter> Distributor {get;set;}//分销商
        public List<Filter> IssueType{get;set;} //问题类型
        public List<Filter> Result{get;set;}//处理结果
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
}