using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class Invlock_main
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string DeadLine { get; set; }
        public bool AutoUnlock { get; set; }
        public string Unlock { get; set; }
        public string Unlocker { get; set; }
        public string UnlockDate { get; set; }
        public string ShopID { get; set; }
        public string CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }

    }

    public class Invlock_item
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int Skuautoid { get; set; }
        public int Qty { get; set; }
        public string CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }

    }
    public class Invlock_view
    {
        public Invlock_main_view Main { get; set; }
        public List<Invlock_item_view> ItemLst { get; set; }
    }
    public class Invlock_main_view
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string DeadLine { get; set; }
        public bool AutoUnlock { get; set; }
        public string ShopID { get; set; }
        public string ShopName { get; set; }
    }

    public class Invlock_item_view
    {
        public int ID { get; set; }
        public int Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public int Qty { get; set; }
        public int StockQty { get; set; }

    }

    public class InvLockParam
    {
        public string CoID { get; set; }
        public string Name { get; set; }//锁定名称
        public string ShopType { get; set; }//平台编号
        public string ShopID { get; set; }//店铺编号
        public string GoodsCode { get; set; }//款式编码
        public string SkuID { get; set; } //商品编码
        public string Site_GoodsCode { get; set; }//平台款式编码
        public string Site_SkuID { get; set; } //平台商品编码
        public string Status { get; set; }//状态：1=已解锁，2=未解锁，(default:0||empty)

        private int _PageSize = 20;//每页笔数
        private int _PageIndex = 1;//页码
        private string _SortField;//排序字段
        private string _SortDirection = "ASC";//DESC,ASC
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

    public class InvlockData
    {
        public int PageCount { get; set; }//总页数
        public int DataCount { get; set; } //总行数
        public List<Invlock_main_query> LockMainLst { get; set; }//返回查询结果
    }

    public class Invlock_main_query
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string DeadLine { get; set; }
        public bool AutoUnlock { get; set; }
        public string Unlock { get; set; }
        public string Unlocker { get; set; }
        public string UnlockDate { get; set; }
    }

}

