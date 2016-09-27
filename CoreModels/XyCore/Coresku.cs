using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{
    public class GoodsQuery
    {
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string KName { get; set; }
        public string Brand { get; set; }
        public int Type { get; set; }
    }

    public class SkuQuery
    {
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool Enable { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
    }


    public class CoreSkuParam
    {
        public int CoID { get; set; }//公司编号
        public string GoodsCode { get; set; }//指定货号查询
        public string GoodsName { get; set; }//指定货品名称查询
        public string Filter {get;set;}//过滤条件
        public string Enable { get; set; }//是否启用
        public int PageSize { get; set; }//每页笔数
        public int PageIndex { get; set; }//页码
        public string SortField { get; set; }//排序字段
        public string SortDirection { get; set; }//DESC,ASC
        public int Type { get; set; }       
    }

    public class CoreSkuQuery
    {        
        public int DataCount { get; set; } //总行数
        public int PageCount { get; set; }//总页数
        public List<GoodsQuery> GoodsLst { get; set; }//返回资料     
        public List<SkuQuery> SkuLst { get; set; }//返回资料       
    }

    public partial class CoreSkuAuto
{
    public string SkuName { get; set; }
    public string GoodsCode { get; set; }
    public string GoodsName { get; set; }
    public string Brand { get; set; }
    public int KID { get; set; }
    public string KName { get; set; }   
    public int CoID { get; set; }
    public string Unit { get; set; }
    public decimal Weight { get; set; }
    public string SCoList { get; set; }
    public string Creator { get; set; }
    public int? status { get; set; }

    public List<CoreSkuItem> items { get; set; }
}

public partial class CoreSkuItem
{
    public string GoodsCode { get; set; }
    public string SkuID { get; set; }
    public string SkuName { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public string ColorID { get; set; }
    public string ColorName { get; set; }
    public string SizeID { get; set; }
    public string SizeName { get; set; }
    public string ParentID { get; set; }
    public string Remark { get; set; }
    public string Creator { get; set; }
    public int? status { get; set; }
}
}