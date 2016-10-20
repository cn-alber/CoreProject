using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreModels.XyCore
{
    public class OrderParm
    {
        public int _CoID ;//公司id
        public int _ID = -1;//内部订单号
        public int _SoID = -1;//线上订单号
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
        public List<string> _StatusDecList = null;//异常状态List
        // public int _Purid = -1;//采购单号
        // public DateTime _PurdateStart = DateTime.Parse("1900-01-01");//采购日期起
        // public DateTime _PurdateEnd = DateTime.Parse("2999-12-31");//采购日期迄
        // public int _Status = -1;//状态
        // public int _Scoid = 0;//供应商
        // public string _Skuid = null;//商品编码
        // public int _Warehouseid = 0;//仓库代号
        // public string _Buyyer = null;//采购员
        public string _SortField = "id";//排序栏位
        public string _SortDirection= "desc";//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public int SoID
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
        public List<string> StatusDecList
        {
            get { return _StatusDecList; }
            set { this._StatusDecList = value;}
        }
        // public DateTime PurdateStart
        // {
        //     get { return _PurdateStart; }
        //     set { this._PurdateStart = value;}
        // }
        // public DateTime PurdateEnd
        // {
        //     get { return _PurdateEnd; }
        //     set { this._PurdateEnd = value;}
        // }
        // public int Status
        // {
        //     get { return _Status; }
        //     set { this._Status = value;}
        // }
        // public int Scoid
        // {
        //     get { return _Scoid; }
        //     set { this._Scoid = value;}
        // }
        // public int Warehouseid
        // {
        //     get { return _Warehouseid; }
        //     set { this._Warehouseid = value;}
        // }
        // public string Skuid
        // {
        //     get { return _Skuid; }
        //     set { this._Skuid = value;}
        // }
        // public string Buyyer
        // {
        //     get { return _Buyyer; }
        //     set { this._Buyyer = value;}
        // }
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
    }
    public class OrderData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<Order> Ord {get;set;}//订单资料List
    }
}