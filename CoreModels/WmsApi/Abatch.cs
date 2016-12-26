using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class ABatchParams
    {
        public int CoID { get; set; }
        public int Type { get; set; }
        public string Pickor { get; set; }
        public int Status { get; set; }
        public int ID { get; set; }
        public int BatchID { get; set; }
        public string BarCode { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string SortCode { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
    }
    public class ABatch
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public int Qty { get; set; }
        public int PickedQty { get; set; }
        public int NoQty { get; set; }
        public int Status { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
    }
    public class ABatchAuto
    {
        public int BatchID { get; set; }
        public int Qty { get; set; }
        public int PickQty { get; set; }
        public int NoQty { get; set; }
        public int Status { get; set; }
    }
    public class ABatchTask
    {
        public int ID { get; set; }
        public int BatchID { get; set; }
        public int Skuautoid { get; set; }
        public string PCode { get; set; }
        public int Qty { get; set; }
        public int Index { get; set; }
        public int PickQty { get; set; }
        public int NoQty { get; set; }
        public string Pickor { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string GoodsCode { get; set; }
        public string Sku { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public int PCodeQty { get; set; }
        public string PCodeSku { get; set; }
    }
    public class ABatchPicked
    {
        public int ID { get; set; }
        public int BatchID { get; set; }
        public int CoID { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public string BarCode { get; set; }
        public int Skuautoid { get; set; }
        public string Sku { get; set; }
        public int Status { get; set; }
        public string PCode { get; set; }
        public int OutID { get; set; }
        public string Express { get; set; }
        public string ExCode { get; set; }
        public int BatchtaskID { get; set; }
        public string SortCode { get; set; }
        public string Creator { get; set; }
        public string CreatDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }

    //记录拣货类型&任务数量
    public class TypeNum
    {
        public int Type { get; set; }
        public int Num { get; set; }
    }

    public class OrderItemBatch
    {
        public int BatchID { get; set; }
        public int OID { get; set; }
        public long SoID { get; set; }
        public string SortCode { get; set; }
        public int ItemQty { get; set; }
        public int PickedQty { get; set; }
    }
    public class ABatchPickData
    {
        public ASkuScan SkuAuto { get; set; }
        public OrderItemBatch OItemAuto { get; set; }
        public int ID { get; set; }//拣货记录ID

    }


}
