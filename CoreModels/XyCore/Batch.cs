using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class GetBatchInit
    {
        public List<Filter> BatchStatus{get;set;}
        public List<Filter> Pickor{get;set;}
        public List<Filter> Task{get;set;}
        public List<Filter> BatchType{get;set;}
    }
    public class Batch
    {
        public int ID{get;set;}
        public int Type{get;set;}
        public int PickorID{get;set;}
        public string Pickor{get;set;}
        public int OrderQty{get;set;}
        public int SkuQty{get;set;}
        public int Qty{get;set;}
        public int PickedQty{get;set;}
        public int NoQty{get;set;}
        public int Status{get;set;}
        public string Mark{get;set;}
        public int CoID{get;set;}
        public bool MixedPicking{get;set;}
        public bool PickingPrint{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Modifier{get;set;}
        public DateTime ModifyDate{get;set;}
    }
    public class BatchQuery
    {
        public int ID{get;set;}
        public int Type{get;set;}
        public string TypeString{get;set;}
        public string Pickor{get;set;}
        public int OrderQty{get;set;}
        public int SkuQty{get;set;}
        public int Qty{get;set;}
        public int PickedQty{get;set;}
        public int NotPickedQty{get;set;}
        public int NoQty{get;set;}
        public int Status{get;set;}
        public string StatusString{get;set;}
        public string CreateDate{get;set;}
        public string Mark{get;set;}
        public bool MixedPicking{get;set;}
        public bool PickingPrint{get;set;}
    }
    public class BatchData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<BatchQuery> Batch {get;set;}
    }
    public class BatchParm
    {
        public int _CoID ;//公司id
        public List<int> _Status = null;
        public int _ID = 0;//批次号
        public string _Remark = null;
        public List<int> _PickorID = null;
        public string _Task = "A";
        public List<int> _Type = null;
        public DateTime _DateStart = DateTime.Parse("1900-01-01");//日期起
        public DateTime _DateEnd = DateTime.Parse("1900-01-01");//日期迄
        public string _SortField = "id";//排序栏位
        public string _SortDirection = "DESC";//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public List<int> Status
        {
            get { return _Status; }
            set { this._Status = value;}
        }
        public int ID
        {
            get { return _ID; }
            set { this._ID = value;}
        }
        public string Remark
        {
            get { return _Remark; }
            set { this._Remark = value;}
        }
        public List<int> PickorID
        {
            get { return _PickorID; }
            set { this._PickorID = value;}
        }
        public string Task
        {
            get { return _Task; }
            set { this._Task = value;}
        }
        public List<int> Type
        {
            get { return _Type; }
            set { this._Type = value;}
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
    public class BatchConfigure
    {
        public int ID{get;set;}
        public int SingleOrdQty{get;set;}
        public int MultiOrdQty{get;set;}
        public int SingleSkuQty{get;set;}
        public int MultiNotOrdQty{get;set;}
        public int BigQty{get;set;}
        public string Express{get;set;}
        public string Shop{get;set;}
        public bool SpecialOrd{get;set;}
    }
    public class ModifyRemarkSuccess
    {
        public int ID{get;set;}
        public string Remark{get;set;}
    }
    public class ModifyRemarkReturn
    {
        public List<ModifyRemarkSuccess> SuccessIDs{get;set;}
        public List<TransferNormalReturnFail> FailIDs{get;set;}
    }
    public class MarkPrintSuccess
    {
        public int ID{get;set;}
        public bool PickingPrint{get;set;}
    }
    public class MarkPrintReturn
    {
        public List<MarkPrintSuccess> SuccessIDs{get;set;}
        public List<TransferNormalReturnFail> FailIDs{get;set;}
    }
    public class GetPickorInit
    {
        public List<Filter> Role{get;set;}
        public List<Filter> Pickor{get;set;}
    }
    public class SetPickorSuccess
    {
        public int ID{get;set;}
        public string Pickor{get;set;}
    }
    public class SetPickorReturn
    {
        public List<SetPickorSuccess> SuccessIDs{get;set;}
        public List<TransferNormalReturnFail> FailIDs{get;set;}
    }
    public class GetOrdCountReturn
    {
        public int SingleOrd{get;set;}
        public int MultiOrd{get;set;}
        public int BigOrd{get;set;}
    }
}