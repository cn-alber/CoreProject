using System;
using System.Collections.Generic;

namespace CoreModels.XyCore
{
    public class ScoCompanyMulti
    {
        public int id {get;set;}
        public string scocode {get;set;}
        public bool enable {get;set;}
        public string sconame {get;set;}
        public string scosimple {get;set;}
        public string typelist {get;set;}
        public string remark {get;set;}
        public string creator {get;set;}
        public DateTime createdate {get;set;}
    }
    public class ScoCompanySingle
    {
        public int id {get;set;}
        public string sconame {get;set;}
        public bool enable {get;set;}
        public string scosimple {get;set;}
        public string scocode {get;set;}
        public string address {get;set;}
        public string country {get;set;}
        public string contactor {get;set;}
        public string tel {get;set;}
        public string phone {get;set;}
        public string fax {get;set;}
        public string url {get;set;}
        public string email {get;set;}
        public string typelist {get;set;}
        public string bank {get;set;}
        public string bankid {get;set;}
        public string taxid {get;set;}
        public string remark {get;set;}
    }
    public class ScoCompanyParm
    {
        public int _CoID ;//公司id
        public string _Enable = "all" ;//启用状态
        public string _Filter = null;//名称过滤条件
        public string _SortField = "id";//排序栏位
        public string _SortDirection = "desc";//排序方式
        public int _NumPerPage = 20 ;//每页显示资料笔数
        public int _PageIndex = 1;//页码
        public int _Datacnt ;//总资料笔数
        public decimal _Pagecnt;//总页数
        public List<ScoCompanyMulti> _Com;//公司资料List
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value;}
        }
        public string Enable
        {
            get { return _Enable; }
            set { this._Enable = value;}
        }
        public string Filter
        {
            get { return _Filter; }
            set { this._Filter = value;}
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
        public int Datacnt
        {
            get { return _Datacnt; }
            set { this._Datacnt = value;}
        }
        public decimal Pagecnt
        {
            get { return _Pagecnt; }
            set { this._Pagecnt = value;}
        }
        public List<ScoCompanyMulti> Com
        {
            get { return _Com; }
            set { this._Com = value;}
        }
    }
}