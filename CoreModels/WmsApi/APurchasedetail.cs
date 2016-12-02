using System;
using System.Collections.Generic;
using CoreModels.XyCore;

namespace CoreModels.WmsApi
{
    public class APurchaseDetail
    {
        public int ID { get; set; }
        public int PurchaseID { get; set; }
        public int Skuautoid { get; set; }
        public int PurQty { get; set; }
        public int RecQty { get; set; }
        public decimal Price { get; set; }
        public string ReturnQty { get; set; }
    }

    public class ApiRecParam
    {
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public int WhID { get; set; }
        public int PWhID { get; set; }
        public int PurID { get; set; }
        public List<ARecSku> RecSkuLst { get; set; }
        // public List<BoxPieceCode> BPLst { get; set; }
    }


    public class ARecSku
    {
        private int _Qty = 1;
        public string BarCode { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public int Qty
        {
            get { return _Qty; }
            set { this._Qty = value; }
        }
        public int SkuType { get; set; }
    }

    public class ARecSkuSum
    {
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public int Qty { get; set; }
    }
    public class APurchaseReceive
    {
        public int ID { get; set; }
        public int SCoID { get; set; }
        public string SCoName { get; set; }
        public int PurchaseID { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string ReceiveDate { get; set; }
        public int Status { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public int CoID { get; set; }
    }

    public class APurchaseRecDetail
    {
        public int ID { get; set; }
        public int RecID { get; set; }
        public string img { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public int RecQty { get; set; }
        public decimal Price { get; set; }
        public string PlanRecQty { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public string GoodsCode { get; set; }
        public string SupplyNum { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
    }

    // public class BoxPieceCode
    // {
    //     public string BoxCode { get; set; }
    //     public Boolean IsBox { get; set; }
    // }


    // public class APurParams
    // {
    //     public string CoID { get; set; }
    //     public string Creator { get; set; }
    //     public int WarehouseID { get; set; }
    //     public string WarehouseName { get; set; }
    //     public int PWID { get; set; }
    //     public string PWName { get; set; }
    //     public int PurID { get; set; }
    //     public int PurRecID { get; set; }
    //     public string RecordID { get; set; }
    //     public int RecQty { get; set; }
    //     public int invType { get; set; }
    //     public int Type { get; set; }
    //     public string CusType { get; set; }
    //     public int Status { get; set; }
    //     Boolean _IsUpdate = true;//是否更新库存
    //     public Boolean IsUpdate
    //     {
    //         get
    //         {
    //             return _IsUpdate;
    //         }
    //         set
    //         {
    //             _IsUpdate = value;
    //         }
    //     }
    //     public string Contents { get; set; }
    //     public PurchaseDetail ApurDetail { get; set; }
    //     public Purchase Apur { get; set; }
    //     public List<PurchaseDetail> APurDetailLst { get; set; }
    //     public List<string> ABoxCodeLst { get; set; }
    //     public List<AWmsBox> WmsboxLst { get; set; }
    //     Boolean _IsPur = true;//是否采购收入
    //     public Boolean IsPur
    //     {
    //         get
    //         {
    //             return _IsPur;
    //         }
    //         set
    //         {
    //             _IsPur = value;
    //         }
    //     }
    //     public int Skuautoid { get; set; }
    //     public string SkuID { get; set; }
    //     public string SkuName { get; set; }
    //     public string Norm { get; set; }
    //     public List<BoxPieceCode> BPLst { get; set; }//箱件码参数List
    //     public List<string> BarCodeLst { get; set; }//件码
    // }
}