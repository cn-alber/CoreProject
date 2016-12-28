using System;

namespace CoreModels.XyComm
{
    public partial class Printer
    {
        public int ID { get; set; }
        public int CoID { get; set; }
        public int PrintType { get; set; }
        public string PrintName { get; set; }
        public string PrintID { get; set; }
        public string IPAddress { get; set; }
        public bool IsDefault { get; set; }
        public bool Enabled { get; set; }

    }

    public class PrinterQuery{
        public int ID{get;set;}
        public string Name{get;set;}
        public int PrintType{get;set;}
        public string PrintName{get;set;}
        public string IPAddress{get;set;}
        public bool Enabled{get;set;}
        public bool IsDefault{get;set;}
        public int PrinterPort{get;set;}
    }
    public class PrinterInsert{
        public int ID{get;set;}
        public int CoID{get;set;}
        public string Name{get;set;}
        public int PrintType{get;set;}
        public string PrintName{get;set;}
        public string IPAddress{get;set;}
        public bool Enabled{get;set;}
        public int PrinterPort{get;set;}
    }

    public class PrinterParam
    {
        public int CoID {get;set;}//公司编号
        public int Type{get;set;}//搜索标识
        public string Enabled{get;set;}
        public string Filter {get;set;}//过滤条件
        public int PageSize {get;set;}//每页笔数
        public int PageIndex {get;set;}//页码

    }
}