using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class Invinout
    {
        public int ID { get; set; }
        public string RecordID { get; set; }
        public int Type { get; set; }
        public string CusType { get; set; }
        public int Status { get; set; }
        public string WhID { get; set; }
        public string WhName { get; set; }
        // public int LinkWhID { get; set; }
        // public string LinkWhName { get; set; }
        // public string LinkIoID { get; set; }
        public bool IsExport { get; set; }
        public int RecID { get; set; }
        public int InvoiceID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string CoID { get; set; }
        // public bool IsUpdate { get; set; }
        public int Qty { get; set; }
    }

    public class InvinoutData
    {
        public int PageCount { get; set; }//总页数
        public int DataCount { get; set; } //总行数
        public List<Invinout> InvIOLst { get; set; }//返回查询结果
    }

    public class InvinoutAuto
    {
        public int CoID { get; set; }
        public decimal Qty { get; set; }
        public string UserName { get; set; }
        public string CusType { get; set; }
        public int Type { get; set; }
        public string RecordID { get; set; }
        public Inventory inv { get; set; }
        public List<Inventory> InvLst { get; set; }
    }

}