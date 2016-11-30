using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreModels.XyCore
{
    public class GiftRule
    {
        public int ID{get;set;}
        public string GiftName{get;set;}
        public int Priority{get;set;}
        public DateTime DateFrom{get;set;}
        public DateTime DateTo{get;set;}
        public string AppointSkuID{get;set;}
        public string AppointGoodsCode{get;set;}
        public string ExcludeSkuID{get;set;}
        public string ExcludeGoodsCode{get;set;}
        public string AmtMin{get;set;}
        public string AmtMax{get;set;}
        public string QtyMin{get;set;}
        public string QtyMax{get;set;}
        public bool IsSkuIDValid{get;set;}
        public string DiscountRate{get;set;}
        public string AppointShop{get;set;}
        public string OrdType{get;set;}
        public bool IsStock{get;set;}
        public bool IsAdd{get;set;}
        public string QtyEach{get;set;}
        public string AmtEach{get;set;}
        public bool IsMarkGift{get;set;}
        public string MaxGiftQty{get;set;}
        public bool Enable{get;set;}
        public int CoID{get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate{get;set;}
        public string Modifier{get;set;}
        public DateTime ModifyDate{get;set;}
        public List<string> GiftNo{get;set;}
    }
    public class GiftRuleEdit
    {
        public int ID{get;set;}
        public string GiftName{get;set;}
        public int Priority{get;set;}
        public DateTime DateFrom{get;set;}
        public DateTime DateTo{get;set;}
        public string AppointSkuID{get;set;}
        public string AppointGoodsCode{get;set;}
        public string ExcludeSkuID{get;set;}
        public string ExcludeGoodsCode{get;set;}
        public string AmtMin{get;set;}
        public string AmtMax{get;set;}
        public string QtyMin{get;set;}
        public string QtyMax{get;set;}
        public bool IsSkuIDValid{get;set;}
        public string DiscountRate{get;set;}
        public string AppointShop{get;set;}
        public string OrdType{get;set;}
        public bool IsStock{get;set;}
        public bool IsAdd{get;set;}
        public string QtyEach{get;set;}
        public string AmtEach{get;set;}
        public bool IsMarkGift{get;set;}
        public string MaxGiftQty{get;set;}
        public List<string> GiftNo{get;set;}
    }
    public class GiftRuleData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<GiftRuleQuery> Gift {get;set;}
    }
    public class GiftRuleQuery
    {
        public int ID{get;set;}
        public string GiftName{get;set;}
        public string Status{get;set;}
        public string AppointShop{get;set;}
        public List<string> GiftNo{get;set;}
        public string AppointSkuID{get;set;}
        public string ExcludeSkuID{get;set;}
        public string AmtMin{get;set;}
        public string AmtMax{get;set;}
        public string QtyMin{get;set;}
        public string QtyMax{get;set;}
        public string DateFrom{get;set;}
        public string DateTo{get;set;}
        public bool IsSkuIDValid{get;set;}
        public string DiscountRate{get;set;}
        public string MaxGiftQty{get;set;}
        public string GivenQty{get;set;}
        public string QtyEach{get;set;}
        public string AmtEach{get;set;}
        public bool IsStock{get;set;}
        public bool IsAdd{get;set;}
        public bool Enable{get;set;}
        public string CreateDate{get;set;}
        public string ModifyDate{get;set;}    
    }
    public class GiftRuleParm
    {
        public int _CoID ;//公司id
        public int _ID ;//规则号
        public string _GiftNo = null;//赠品
        public string _GiftName = null;//名称
        public DateTime _DateFrom = DateTime.Parse("1900-01-01");//规则开始时间
        public DateTime _DateTo = DateTime.Parse("1900-01-01");//规则结束时间
        public string _AppointSkuID = null;//包含商品
        public string _ExcludeSkuID = null;//不包含商品
        public string _AmtMinStart = null;//最小金额大于等于
        public string _AmtMinEnd = null;//最小金额小于等于
        public string _AmtMaxStart = null;//最大金额大于等于
        public string _AmtMaxEnd = null;//最大金额小于等于
        public string _QtyMinStart = null;//最小数量大于等于
        public string _QtyMinEnd = null;//最小数量小于等于
        public string _QtyMaxStart = null;//最大数量大于等于
        public string _QtyMaxEnd = null;//最大数量小于等于
        public bool _IsEnable = false;//启用
        public bool _IsDisable = false;//禁用
        public string _QtyEachStart = null;//每多少数量送一组大于等于
        public string _QtyEachEnd = null;//每多少数量送一组小于等于
        public string _AmtEachStart = null;//每多少金额送一组大于等于
        public string _AmtEachEnd = null;//每多少金额送一组小于等于
        public DateTime _CreateDateStart = DateTime.Parse("1900-01-01");//登记日期起
        public DateTime _CreateDateEnd = DateTime.Parse("1900-01-01");//登记日期迄
        public string _AppointShop = null;//店铺
        public string _SortField = null;//排序栏位
        public string _SortDirection = null;//排序方式
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
        public string GiftNo
        {
            get { return _GiftNo; }
            set { this._GiftNo = value;}
        }
        public string GiftName
        {
            get { return _GiftName; }
            set { this._GiftName = value;}
        }
        public DateTime DateFrom
        {
            get { return _DateFrom; }
            set { this._DateFrom = value;}
        }
        public DateTime DateTo
        {
            get { return _DateTo; }
            set { this._DateTo = value;}
        }
        public string AppointSkuID
        {
            get { return _AppointSkuID; }
            set { this._AppointSkuID = value;}
        }
        public string ExcludeSkuID
        {
            get { return _ExcludeSkuID; }
            set { this._ExcludeSkuID = value;}
        }
        public string AmtMinStart
        {
            get { return _AmtMinStart; }
            set { this._AmtMinStart = value;}
        }
        public string AmtMinEnd
        {
            get { return _AmtMinEnd; }
            set { this._AmtMinEnd = value;}
        }
        public string AmtMaxStart
        {
            get { return _AmtMaxStart; }
            set { this._AmtMaxStart = value;}
        }
        public string AmtMaxEnd
        {
            get { return _AmtMaxEnd; }
            set { this._AmtMaxEnd = value;}
        }
        public string QtyMinStart
        {
            get { return _QtyMinStart; }
            set { this._QtyMinStart = value;}
        }
        public string QtyMinEnd
        {
            get { return _QtyMinEnd; }
            set { this._QtyMinEnd = value;}
        }
        public string QtyMaxStart
        {
            get { return _QtyMaxStart; }
            set { this._QtyMaxStart = value;}
        }
        public string QtyMaxEnd
        {
            get { return _QtyMaxEnd; }
            set { this._QtyMaxEnd = value;}
        }
        public bool IsEnable
        {
            get { return _IsEnable; }
            set { this._IsEnable = value;}
        }
        public bool IsDisable
        {
            get { return _IsDisable; }
            set { this._IsDisable = value;}
        }
        public string QtyEachStart
        {
            get { return _QtyEachStart; }
            set { this._QtyEachStart = value;}
        }
        public string QtyEachEnd
        {
            get { return _QtyEachEnd; }
            set { this._QtyEachEnd = value;}
        }
        public string AmtEachStart
        {
            get { return _AmtEachStart; }
            set { this._AmtEachStart = value;}
        }
        public string AmtEachEnd
        {
            get { return _AmtEachEnd; }
            set { this._AmtEachEnd = value;}
        }
        public DateTime CreateDateStart
        {
            get { return _CreateDateStart; }
            set { this._CreateDateStart = value;}
        }
        public DateTime CreateDateEnd
        {
            get { return _CreateDateEnd; }
            set { this._CreateDateEnd = value;}
        }
        public string AppointShop
        {
            get { return _AppointShop; }
            set { this._AppointShop = value;}
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