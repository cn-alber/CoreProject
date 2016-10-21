using System;
using System.Collections.Generic;
namespace CoreModels.XyUser
{
    public class Company
    {
        public int id {get;set;}
        public string name {get;set;}
        public bool enable {get;set;}
        public string address {get;set;}
        public string email {get;set;}
        public string contacts {get;set;}
        public string telphone {get;set;}
        public string mobile {get;set;}
        public string remark {get;set;}
        public string creator {get;set;}
        public DateTime createdate {get;set;}
        public string modifier {get;set;}
        public DateTime modifydate {get;set;}
    }
    public class CompanyParm
    {
        public int _CoID ;//公司id
        public string _Enable = "all" ;//启用状态
        public string _Filter;//名称过滤条件
        public string _SortField ;//排序栏位
        public string _SortDirection;//排序方式
        public int _NumPerPage = 20;//每页显示资料笔数
        public int _PageIndex = 1;//页码
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
    }
    public class CompanyData
    {
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt {get;set;}//总页数
        public List<Company> Com {get;set;}//公司资料List
    }
    public class CompanyMulti
    {
        public int id {get;set;}
        public string name {get;set;}
        public bool enable {get;set;}
        public string address {get;set;}
        public string remark {get;set;}
        public string creator {get;set;}
        public DateTime createdate {get;set;}
    }


}