using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class Invinoutitem
    {
        public int ID { get; set; }
        public string IoID { get; set; }
        public string CusType { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public int Qty { get; set; }
        public int WhID { get; set; }
        public string WhName { get; set; }
        public string Unit { get; set; }
        public int CoID { get; set; }
        public string Img { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        // public int LinkWhID { get; set; }
        // public string LinkWhName { get; set; }
        // public string LinkIoID { get; set; }
        // public bool IsUpdate { get; set; }
    }
    public class InvItemData
    {
        public int PageCount { get; set; }//总页数
        public int DataCount { get; set; } //总行数
        public List<Invinoutitem> InvitemLst { get; set; }//返回查询结果
    }
}