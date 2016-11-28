using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class ACoreSku
    {
        public int ID { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string GoodsCode { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string Norm { get; set; }
        public int Qty { get; set; }//装箱件数
        public string PCode { get; set; }//货位
        public int MaxQty { get; set; }//货位最大库存
        public Boolean IsBox { get; set; }//true->箱码 or false->件码
        public int PCodeID { get; set; }
        public string SortCode { get; set; }//分拣格
        public int Type { get; set; }//仓库类型
    }
    public class ASkuScan
    {
        public int ID { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string GoodsCode { get; set; }
        public string Norm { get; set; }
        public int SkuType { get; set; }
    }

    public class ASkuScanParam
    {
        public int CoID { get; set; }
        public string BarCode { get; set; }
    }
}