using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class Invinoutitem
    {
        public int ID { get; set; }
        public string IoID { get; set; }
        public int Type { get; set; }
        public string CusType { get; set; }
        public int Status { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public int Qty { get; set; }
        public string WhID { get; set; }
        public string WhName { get; set; }
        // public string Unit { get; set; }
        public string CoID { get; set; }
        public string Img { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string LinkWhID { get; set; }
        // public string LinkWhName { get; set; }
        // public string LinkIoID { get; set; }
        // public bool IsUpdate { get; set; }
        public string RefID { get; set; }//相关单据编号(来源ID)
        public string OID { get; set; }//内部订单号
    }
    public class InvItemData
    {
        public int PageCount { get; set; }//总页数
        public int DataCount { get; set; } //总行数
        public List<Invinoutitem> InvitemLst { get; set; }//返回查询结果
    }
}