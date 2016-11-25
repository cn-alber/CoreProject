using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class Inventory
    {
        public int ID { get; set; }
        public string GoodsCode { get; set; }
        public string Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string Name { get; set; }
        public string Norm { get; set; }
        public string WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public decimal StockQty { get; set; }
        public decimal LockQty { get; set; }
        public decimal PickQty { get; set; }
        public decimal WaitInQty { get; set; }
        public decimal SaleRetuQty { get; set; }
        public decimal SafeQty { get; set; }
        public decimal DefectiveQty { get; set; }
        public decimal VirtualQty { get; set; }
        public decimal PurchaseQty { get; set; }
        public string Img { get; set; }
        public string CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }

    }

    public class Inventory_sale
    {
        public int ID { get; set; }
        public string GoodsCode { get; set; }
        public string Skuautoid { get; set; }
        public string SkuID { get; set; }
        public string Name { get; set; }
        public string Norm { get; set; }
        public decimal StockQty { get; set; }
        public decimal LockQty { get; set; }
        public decimal PickQty { get; set; }
        public decimal WaitInQty { get; set; }
        public decimal SaleRetuQty { get; set; }
        public decimal SafeQty { get; set; }
        public decimal DefectiveQty { get; set; }
        public decimal VirtualQty { get; set; }
        public decimal PurchaseQty { get; set; }
        public string Img { get; set; }
        public string CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }

    }

    public class InvQueryParam
    {
        private int _CoID;//公司编号
        // private string _Filter;//过滤条件
        // private string _Enable = "all";//是否启用
        private int _PageSize = 20;//每页笔数
        private int _PageIndex = 1;//页码
        private string _SortField;//排序字段
        private string _SortDirection = "ASC";//DESC,ASC
        private string _GoodsCode;//商品编号        
        private string _SkuID;//商品编号
        private string _SkuName;//商品名称
        private string _Norm;//颜色及规格
        private int _StockQtyb = 0;//主仓实际库存数量起
        private int _StockQtye = 999999999;//主仓实际库存数量迄
        private int _WarehouseID = 0;//仓库ID
        private int _Status = 0;//库存状态:0.全部,1.充足,2.预警
        private DateTime _DocDateB = Convert.ToDateTime("1999/01/01");//单据日期起
        private DateTime _DocDateE = DateTime.Now;//单据日期迄        
        private string _DocType;//单据类型
        private string _RecordID;//单据编号
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value; }
        }//公司编号
        // public string Filter
        // {
        //     get { return _Filter; }
        //     set { this._Filter = value; }
        // }//过滤条件        
        // public string Enable
        // {
        //     get { return _Enable; }
        //     set { this._Enable = value; }
        // }//是否启用
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
        public string GoodsCode
        {
            get { return _GoodsCode; }
            set { this._GoodsCode = value; }
        }//指定款式编号
        public string SkuID
        {
            get { return _SkuID; }
            set { this._SkuID = value; }
        }//指定商品编号
        public string SkuName
        {
            get { return _SkuName; }
            set { this._SkuName = value; }
        }//指定商品名称
        public string Norm
        {
            get { return _Norm; }
            set { this._Norm = value; }
        }//颜色及规格
        public int StockQtyb
        {
            get { return _StockQtyb; }
            set { this._StockQtyb = value; }
        }//主仓实际库存数量起
        public int StockQtye
        {
            get { return _StockQtye; }
            set { this._StockQtye = value; }
        }//主仓实际库存数量迄
        public int WarehouseID
        {
            get { return _WarehouseID; }
            set { this._WarehouseID = value; }
        }//商品仓库
        public int Status
        {
            get { return _Status; }
            set { this._Status = value; }
        }//库存状态:0.全部,1.充足,2.预警
        public DateTime DocDateB
        {
            get { return _DocDateB; }
            set { this._DocDateB = value; }
        }//单据日期起
        public DateTime DocDateE
        {
            get { return _DocDateE; }
            set { this._DocDateE = value; }
        }//单据日期迄
        public string DocType
        {
            get { return _DocType; }
            set { this._DocType = value; }
        }//单据类型
        public string RecordID
        {
            get { return _RecordID; }
            set { this._RecordID = value; }
        }//单据编号
    }

    public class InventoryData
    {
        public int PageCount { get; set; }//总页数
        public int DataCount { get; set; } //总行数
        public List<Inventory> InvLst { get; set; }//返回查询结果
        public List<Inventory_sale> InvMainLst { get; set; }
    }

    public class Sfc_InvStock
    {
        public int ID { get; set; }
        public string Skuautoid { get; set; }
        public string SkuID { get; set; }
        public int StockQty { get; set; }

    }
    public class InventParams
    {
        public string CoID { get; set; }
        public int ID { get; set; }
        public decimal SafeQty { get; set; }//安全库存下限
        public decimal UpSafeQty { get; set; }//安全库存上限
        public decimal VirtualQty { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
        public int Type { get; set; }
    }

    public class InvavailQty //获取可用库存量实体
    {
        public string CoID { get; set; }
        public string Skuautoid { get; set; }
        public decimal StockQty { get; set; }
        public decimal InvLockQty { get; set; }
        public decimal LockQty { get; set; }
        public decimal VirtualQty { get; set; }
    }

    //盘点库存导出模板excel,顺序不可调
    public class SetInvQtyExcel
    {
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string SkuID { get; set; }
        public string Name { get; set; }
        public string Norm { get; set; }
        public decimal StockQty { get; set; }
        public decimal SetQty { get; set; }
    }

    public class InitInvQtyExcel
    {
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string ColorName { get; set; }
        public string ParentSize { get; set; }
        public string SizeName { get; set; }
        public decimal StockQty { get; set; }
    }

}