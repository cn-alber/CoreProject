using System;
using System.Collections.Generic;
namespace CoreModels.WmsApi
{
    public class AWmsPile
    {
        private bool _Enable = true;
        public int ID { get; set; }
        public string PCode { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int Type { get; set; }
        public int PCType { get; set; }
        public int Order { get; set; }
        public int Qty { get; set; }
        public int lockqty { get; set; }
        public bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public int CoID { get; set; }
        public int MaxQty { get; set; }
        public string PCodeC { get; set; }
        public int WhIDC { get; set; }
        public string WhNameC { get; set; }

    }

    public class AWmsPileAuto
    {
        public int ID { get; set; }
        public int WarehouseID { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public int Qty { get; set; }
        public int MaxQty { get; set; }
        public int Type { get; set; }
        public int PCType { get; set; }//实体货位 = 1,临时货位 = 2(托盘)
        public string PCode { get; set; }
        public string Order { get; set; }
        Boolean _Enable = true;
        public Boolean Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }
        public string CoID { get; set; }

    }


    public class AShelfParam
    {
        private int _Qty = 1;
        public string BoxCode { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public int Qty
        {
            get { return _Qty; }
            set { this._Qty = value; }
        }

        public int WarehouseID { get; set; }
        public string PCode { get; set; }
        public int Type { get; set; }
        public List<int> TypeLst { get; set; }
        public int CoID { get; set; }
    }

    public class AShelfData
    {
        public ASkuScan SkuAuto { get; set; }
        public AWmsPileAuto PileAuto { get; set; }

    }

    public class AShelfSet
    {
        public ASkuScan SkuAuto { get; set; }
        public int PileID { get; set; }
        public int WarehouseID { get; set; }
        public int Type { get; set; }
        public string PCode { get; set; }
        public string Contents { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
    }

    public class APileQty
    {
        public int CoID { get; set; }
        public int ID { get; set; }
        public int Qty { get; set; }
    }
    public class ApiMoveParam
    {
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string BoxCode { get; set; }
        public Boolean IsBox { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int Type { get; set; }
        public string Contents { get; set; }
        public int PileID { get; set; }
        public string PCode { get; set; }

    }
}