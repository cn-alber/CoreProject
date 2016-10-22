using System;
using System.Collections.Generic;
using CoreModels.XyCore;

namespace CoreModels.WmsApi
{
    public class APurchaseDetail
    {
        public int id { get; set; }
        public int purchaseid { get; set; }
        public string img { get; set; }
        public int skuautoid { get; set; }
        public string skuid { get; set; }
        public string skuname { get; set; }
        public string purqty { get; set; }
        public string suggestpurqty { get; set; }
        public string recqty { get; set; }
        public string price { get; set; }
        public string puramt { get; set; }
        public string returnqty { get; set; }
        public int detailstatus { get; set; }
        public string remark { get; set; }
        public string goodscode { get; set; }
        public string supplynum { get; set; }
        public string supplycode { get; set; }
        public string planqty { get; set; }
        public string planamt { get; set; }
        public string _recievedate;
        public string norm { get; set; }
        public string packingnum { get; set; }
        public int coid { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public DateTime createdate { get; set; }
        public DateTime modifydate { get; set; }
        public string recievedate
        {
            get { return _recievedate; }
            set { this._recievedate = value.ToString().Substring(0, 10); }
        }
    }

    public class ApiPurParam
    {
        public int CoID { get; set; }
        public string Creator { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int PWID { get; set; }
        public string PWName { get; set; }
        public int PurID { get; set; }
        public int RecQty { get; set; }
        public List<BoxPieceCode> BPLst { get; set; }
    }
    public class BoxPieceCode
    {
        public string BoxCode { get; set; }
        public Boolean IsBox { get; set; }
    }


    public class APurParams
    {
        public int CoID { get; set; }
        public string Creator { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int PWID { get; set; }
        public string PWName { get; set; }
        public int PurID { get; set; }
        public int PurRecID{get;set;}
        public string RecordID { get; set; }
        public int RecQty { get; set; }
        public int invType { get; set; }
        public int Type { get; set; }
        public string CusType { get; set; }
        public string Status { get; set; }
        Boolean _IsUpdate = true;//是否更新库存
        public Boolean IsUpdate
        {
            get
            {
                return _IsUpdate;
            }
            set
            {
                _IsUpdate = value;
            }
        }
        public string Contents { get; set; }
        public PurchaseDetail ApurDetail { get; set; }
        public Purchase Apur { get; set; }
        public List<PurchaseDetail> APurDetailLst { get; set; }
        public List<string> ABoxCodeLst { get; set; }
        public List<AWmsBox> WmsboxLst { get; set; }
        Boolean _IsPur = true;//是否采购收入
        public Boolean IsPur
        {
            get
            {
                return _IsPur;
            }
            set
            {
                _IsPur = value;
            }
        }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public List<BoxPieceCode> BPLst { get; set; }//箱件码参数List
        public List<string> BarCodeLst { get; set; }//件码
    }
}