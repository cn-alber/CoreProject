using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class Sfc_main
    {
        private int _Status = 0;//0:待确认;1:生效;2.作废
        public int ID { get; set; }
        public string WhID { get; set; }
        public string WhName { get; set; }
        public string Parent_WhID { get; set; }

        public int Status
        {
            get { return _Status; }
            set { this._Status = value; }
        }
        public int Type { get; set; }//(1.期初，2.盘点，3.调拨)
        public string Remark { get; set; }
        public string CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }
    public class Sfc_item
    {
        // private int _Status = 0;//0:待确认;1:生效;2.作废
        public int ID { get; set; }
        public string WhID { get; set; }
        public string WhName { get; set; }
        public string Parent_WhID { get; set; }
        public string Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public string Qty { get; set; }
        public string InvQty { get; set; }
        public string ParentID { get; set; }

        // public int Status
        // {
        //     get { return _Status; }
        //     set { this._Status = value; }
        // }
        public int Type { get; set; }//(1.期初，2.盘点，3.调拨)
        public string CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }
    public class Sfc_main_view
    {
        private int _Status = 0;//0:待确认;1:生效;2.作废
        public int ID { get; set; }
        public string WhID { get; set; }
        public string Remark { get; set; }
        public int Status
        {
            get { return _Status; }
            set { this._Status = value; }
        }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
    }

    public class Sfc_item_view
    {
        public int ID { get; set; }
        public string Skuautoid { get; set; }
        public string Qty { get; set; }
        public string InvQty { get; set; }
        public string ParentID { get; set; }
        public string Type { get; set; }
    }

    public class Sfc_main_query
    {
        public List<Sfc_main_view> MainLst { get; set; }
        public Dictionary<string, object> DicWh { get; set; }
    }

    public class Sfc_item_query
    {
        public List<Sfc_item_view> ItemLst { get; set; }
        public Dictionary<string, object> DicSku { get; set; }
    }

}