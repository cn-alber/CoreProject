using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class PurchaseReceive
    {
        public int id{get;set;}
        public int scoid{get;set;}
        public string sconame{get;set;}
        public int purchaseid{get;set;}
        public int warehouseid{get;set;}
        public string warehousename{get;set;}
        public string _receivedate;
        public string remark{get;set;}
        public string logisticsno{get;set;}
        public string finconfirmer{get;set;}
        public int status{get;set;}
        public int finstatus{get;set;}
        public string creator{get;set;}
        public DateTime modifydate{get;set;}
        public DateTime finconfirmdate{get;set;}
        public string receivedate
        {
            get { return _receivedate; }
            set { this._receivedate = value.ToString().Substring(0,10);}
        }
    }
    public class PurchaseRecDetail
    {
        public int id{get;set;}
        public int recid{get;set;}
        public string img{get;set;}
        public int skuautoid{get;set;}
        public string skuid{get;set;}
        public string skuname{get;set;}
        public string norm {get;set;}
        public string recqty{get;set;}
        public string price{get;set;}
        public string planrecqty{get;set;}
        public string amount{get;set;}
        public string remark{get;set;}
        public string goodscode{get;set;}
        public string supplynum{get;set;}
    }
    public class PurchaseReceiveParm
    {
        public int _CoID ;//公司id
        public int _Purid = -1;//采购单号
        public bool _IsNotPur = false;//无采购
        public DateTime _RecdateStart = DateTime.Parse("1900-01-01");//入库日期起
        public DateTime _RecdateEnd = DateTime.Parse("2999-12-31");//入库日期迄
        public int _Status = -1;//状态
        public int _FinStatus = -1;//财务状态
        public int _Scoid = 0;//供应商
        public string _Skuid = null;//商品编码
        public string _Remark = null;//备注模糊查询
        public string _SortField = "id";//排序栏位
        public string _SortDirection= "desc";//排序方式
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
        public bool IsNotPur
        {
            get { return _IsNotPur; }
            set { this._IsNotPur = value;}
        }
        public DateTime RecdateStart
        {
            get { return _RecdateStart; }
            set { this._RecdateStart = value;}
        }
        public DateTime RecdateEnd
        {
            get { return _RecdateEnd; }
            set { this._RecdateEnd = value;}
        }
        public int Status
        {
            get { return _Status; }
            set { this._Status = value;}
        }
        public int FinStatus
        {
            get { return _FinStatus; }
            set { this._FinStatus = value;}
        }
        public int Scoid
        {
            get { return _Scoid; }
            set { this._Scoid = value;}
        }
        public string Skuid
        {
            get { return _Skuid; }
            set { this._Skuid = value;}
        }
        public string Remark
        {
            get { return _Remark; }
            set { this._Remark = value;}
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
    public class PurchaseReceiveData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<PurchaseReceive> PurRec {get;set;}//收料单资料List
    }
    public class PurchaseRecDetailParm
    {
        public int _CoID ;//公司id
        public int _Recid ;//收料单号
        public string _SortField = "id";//排序栏位
        public string _SortDirection = "ASC";//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public int Recid
        {
            get { return _Recid; }
            set { this._Recid = value;}
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
    public class PurchaseRecDetailData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public bool enable {get;set;} //明细是否允许修改
        public List<PurchaseRecDetail> PurRecDetail {get;set;}//收料单明细资料List
    }
    public class PurchaseRecInitData
    {
        public Dictionary<int,string> status{get;set;}//状态
        public Dictionary<int,string> finstatus{get;set;}//财务状态状态
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<PurchaseReceive> PurRec {get;set;}//采购入库单资料List
    }
}