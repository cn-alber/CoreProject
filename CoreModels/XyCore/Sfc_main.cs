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
        public int Qty { get; set; }
        public int InvQty { get; set; }
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

    public class Sfc_item_Init_view
    {
        public int ID { get; set; }
        public string Skuautoid { get; set; }
        public string InvQty { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
    }
    public class Sfc_main_query
    {
        public int DataCount { get; set; } //总行数
        public int PageCount { get; set; }//总页数
        public List<Sfc_main_view> MainLst { get; set; }
        public Dictionary<string, object> DicWh { get; set; }
    }

    public class Sfc_item_query
    {
        public int DataCount { get; set; } //总行数
        public int PageCount { get; set; }//总页数
        public List<Sfc_item_Init_view> InitItemLst { get; set; }
        public List<Sfc_item_view> ItemLst { get; set; }
        public Dictionary<string, object> DicSku { get; set; }
    }


    public class Sfc_item_param
    {
        private string _CoID;//公司编号
        private int _PageSize = 20;//每页笔数
        private int _PageIndex = 1;//页码
        private string _SortField = "CreateDate";//排序字段
        private string _SortDirection = "DESC";//DESC,ASC
        public string WhID { get; set; }//仓库id
        public string DateF { get; set; }//单据日期起
        public string DateT { get; set; }//单据日期讫
        public string Status { get; set; }//盘点状态
        public string Skuautoid { get; set; }//商品编号
        public int Type { get; set; }//单据类型(1.期初，2.盘点，3.调拨)
        public int ParentID { get; set; }//主表sfc_main.ID
        public string CoID
        {
            get { return _CoID; }
            set { this._CoID = value; }
        }//公司编号
        public int PageSize
        {
            get { return _PageSize; }
            set { this._PageSize = value; }
        }//每页笔数
        public int PageIndex
        {
            get { return _PageIndex; }
            set { this._PageIndex = value; }
        }//页码
        public string SortField
        {
            get { return _SortField; }
            set { this._SortField = value; }
        }//排序字段
        public string SortDirection
        {
            get { return _SortDirection; }
            set { this._SortDirection = value; }
        }//DESC,ASC 


    }
}