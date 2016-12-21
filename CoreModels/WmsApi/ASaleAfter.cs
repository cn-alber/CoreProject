using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class ASaleAfter
    {
        public int ID { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public string RegisterDate { get; set; }
        public string BuyerShopID { get; set; }
        public string RecName { get; set; }
        public int Type { get; set; }
        public string RecPhone { get; set; }
        public decimal SalerReturnAmt { get; set; }
        public decimal BuyerUpAmt { get; set; }
        public decimal RealReturnAmt { get; set; }
        public string ReturnAccount { get; set; }
        public string ShopName { get; set; }
        public string RecWarehouse { get; set; }
        public int IssueType { get; set; }
        public int OrdType { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
        public string ShopStatus { get; set; }
        public string GoodsStatus { get; set; }
        public string RefundStatus { get; set; }
        public string Express { get; set; }
        public string ExCode { get; set; }
        public bool IsSubmit { get; set; }
        public string Distributor { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
        public string ConfirmDate { get; set; }
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
