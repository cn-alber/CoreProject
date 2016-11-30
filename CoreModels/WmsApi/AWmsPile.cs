using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class AWmsPile
    {        
        public int ID { get; set; }
        public string PCode { get; set; }
        public int Skuautoid{get;set;}
        public string SkuID { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int Type { get; set; }
        public int PCType { get; set; }
        public int Order { get; set; }
        public int Qty { get; set; }
        public int lockqty { get; set; }
        public bool Enable { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public int CoID { get; set; }
        public int maxqty { get; set; }
        public string PCodeC { get; set; }
        public int WhIDC { get; set; }
        public string WhNameC { get; set; }

    }

    public class WmsPileAuto
    {
        public string CoID { get; set; }
        public string GoodsCode { get; set; }
        public int Skuautoid{get;set;}
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string Creator { get; set; }
        public int Qty { get; set; }
        public int MaxQty { get; set; }
        public int status { get; set; }//1.新增,2.修改
        public int Type { get; set; }
        public int PCType { get; set; }//实体货位 = 1,临时货位 = 2(托盘)
        public string PCode { get; set; }
        public string PCodeC { get; set; }
        public int WhIDC { get; set; }
        public string WhNameC { get; set; }
        public string Order { get; set; }
        public string CreateDate { get; set; }
        Boolean _Enable = true;
        public Boolean Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }

    }
}