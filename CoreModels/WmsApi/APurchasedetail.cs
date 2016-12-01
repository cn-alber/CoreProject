using System;
using System.Collections.Generic;
using CoreModels.XyCore;

namespace CoreModels.WmsApi
{
    public class APurchaseDetail
    {
        public int id { get; set; }
        public int purchaseid { get; set; }
        public int skuautoid { get; set; }
        public int purqty { get; set; }
        public int recqty { get; set; }
        public string returnqty { get; set; }
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