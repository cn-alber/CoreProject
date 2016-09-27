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
        public int CoID {get;set;}//公司id
        public string Enable {get;set;}//启用状态
        public string Filter{get;set;}//名称过滤条件
        public string SortField{get;set;}//排序栏位
        public string SortDirection{get;set;}//排序方式
        public int NumPerPage{get;set;}//每页显示资料笔数
        public int PageIndex{get;set;}//页码
        public int Datacnt {get;set;}//总资料笔数
        public decimal Pagecnt{get;set;}//总页数
        public List<ScoCompanyMulti> Com {get;set;}//公司资料List
    }
}