using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class ASaleParams
    {
        public int CoID { get; set; }
        public int BatchType { get; set; }
        public int BatchID { get; set; }
        public string BarCode { get; set; }
        public int Skuautoid { get; set; }
        public string SortCode { get; set; }
    }
    public class OutItemBatch
    {
        public int BatchID { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public int ItemQty { get; set; }
        public int OutID { get; set; }
        public string ExCode { get; set; }
        public string ExpName { get; set; }
        public string IsExpPrint { get; set; }
        public int OrdQty { get; set; }
        public int OutQty { get; set; }
        public string SortCode { get; set; }
    }

    public class ASaleOutData
    {
        public ASkuScan SkuAuto { get; set; }
        public OutItemBatch OItemAuto { get; set; }
        public int ID { get; set; }//拣货记录ID

    }
    public class ASaleOutSet
    {
        public ASkuScan SkuAuto { get; set; }
        public OutItemBatch OItemAuto { get; set; }
        public int ID { get; set; }//拣货记录ID
        public string Contents { get; set; }
        // public int BatchID { get; set; }
        // public int OID { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }

    }

    public class ASaleOutQty
    {
        public int CoID { get; set; }
        public int Skuautoid { get; set; }
        public int Qty { get; set; }
    }


    public class ASaleOutTemp
    {
        public int ID { get; set; }
        public int CoID { get; set; }
        public string BoxCode { get; set; }
        public string BarCode { get; set; }
        public string SkuID { get; set; }
        public int qty { get; set; }
        public int OID { get; set; }
        public string Express { get; set; }
        public string ExCode { get; set; }
        public int BatchID { get; set; }
        public int Type { get; set; }
        public int WarehouseID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }

    }

    public class ASaleOutItem
    {
        
    }
    public class ASaleOutPrint
    {
        public int OID { get; set; }
        public long SoID { get; set; }
        public int BatchID { get; set; }
        public DateTime DocDate { get; set; }
        public string ExpName { get; set; }
        public string ExCode { get; set; }
        public string RecZip { get; set; }
        public string RecMessage { get; set; }
        public string RecAddress { get; set; }
        public string RecLogistics { get; set; }
        public string RecDistrict { get; set; }
        public string RecCity { get; set; }
        public string RecName { get; set; }
        public string RecPhone { get; set; }
        public decimal ExWeight { get; set; }
        public decimal RealWeight { get; set; }
        public string ShipType { get; set; }
        public decimal ExCost { get; set; }
        public decimal Amount { get; set; }
        public int OrdQty { get; set; }
        public int GiftQty { get; set; }
        public string Remark { get; set; }
        public string SendWarehouse { get; set; }
        public string PayDate { get; set; }
        public string SendMessage { get; set; }
        public int CoID { get; set; }
        public string SendLogistics { get; set; }
        public string SendCity { get; set; }
        public string SendDistrict { get; set; }
        public string SendAddress { get; set; }
        public string SendPhone { get; set; }
        public string Sender { get; set; }
        public string SendRemark { get; set; }
        public ASaleOutSku OutSku { get; set; }
        public List<ASaleOutSku> OutSkuLst { get; set; }
    }

    public class ASaleOutSku
    {
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public int OutQty { get; set; }
        public Boolean IsBox { get; set; }
        public string SkuID { get; set; }
    }
}