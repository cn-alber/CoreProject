using System;
using System.Collections.Generic;
using CoreModels.XyCore;
namespace CoreModels.WmsApi
{
    public class ASaleAfterParam
    {
        public int CoID { get; set; }
        public string BarCode { get; set; }
        public ASkuScan SkuAuto { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public int ASID { get; set; }
        public string issueName { get; set; }
        public string ExCode { get; set; }
        public int Type { get; set; }
        public int WhID { get; set; }//销退仓
        public string WhName { get; set; }
        public string Contents { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public decimal Price { get; set; }
        public string PCode { get; set; }
        // public SaleOutItemInsert SOItem { get; set; }
    }
    public class ASaleAfter
    {
        public ASkuScan SkuAuto { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public string IssueName { get; set; }
        public int ASID { get; set; }
    }
    public class ASaleAfterItem
    {
        public int ID { get; set; }
        public int RID { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public int ReturnType { get; set; }
        public string BarCode { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public string GoodsCode { get; set; }
        public int RegisterQty { get; set; }
        public int ReturnQty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Img { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
    }
}
