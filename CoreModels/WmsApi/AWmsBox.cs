using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class AWmsBox
    {
        public int ID { get; set; }
        public string BarCode { get; set; }
        public string SkuID { get; set; }
        public string BoxCode { get; set; }
        public int WarehouseID { get; set; }
        public string PCode { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
    }
    public class ApiBoxPrint
    {
        public string BoxCode { get; set; }
        public string SkuID { get; set; }
        public string GoodsCode { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public int Nums { get; set; }
    }

    public class ApiBarCodeLoc
    {
        public string GoodsCode { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public int Nums { get; set; }
        public int BoxNums { get; set; }
        public string PCode { get; set; }
        public string Contents { get; set; }
    }

    public class ApiBoxParam
    {
        public int CoID { get; set; }
        public string BarCode { get; set; }
        public string SkuID { get; set; }
        public string Creator { get; set; }
        public int WarehouseID { get; set; }
        public int Type { get; set; }
        public List<string> ABarCodeLst { get; set; }
        public string BoxCode { get; set; }
    }

    public class WmsBoxParams 
    {
        public int CoID { get; set; }
        public string BarCode { get; set; }
        public string SkuID { get; set; }
        public string Creator { get; set; }
        public List<string> ABarCodeLst { get; set; }
        public string BoxCode { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public List<string> ABoxCodeLst { get; set; }
        public List<string> SkuIDLst { get; set; }
        public List<AWmsBox> WmsboxLst { get; set; }
        public string RecordID { get; set; }
        public string Contents { get; set; }
        public int Qty { get; set; }
        public List<AWarehouse> AWhLst { get; set; }
        public List<int> WhIDLst { get; set; }
        public int Type { get; set; }
        public int TempType { get; set; }
        public string PCode { get; set; }
        public Boolean _IsBox = true;
        public Boolean IsBox
        {
            get
            {
                return _IsBox;
            }
            set
            {
                _IsBox = value;
            }
        }
        public List<int> TempTypeLst { get; set; }
        public ASkuScan SkuAuto { get; set; }
    }
}