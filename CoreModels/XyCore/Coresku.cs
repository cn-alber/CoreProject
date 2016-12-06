using System;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreModels.XyCore
{

    #region 
    public class Coresku
    {
        private int _Enable = 1;//可选值状态
        public int ID { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string SkuSimple { get; set; }
        public string Brand { get; set; }
        public string KindID { get; set; }
        public string KindName { get; set; }
        public int Type { get; set; }
        public bool SynStock { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string GBCode { get; set; }
        public string Unit { get; set; }
        public string ValUnit { get; set; }
        public string CnvRate { get; set; }
        public string Weight { get; set; }
        public string CostPrice { get; set; }
        public string PurPrice { get; set; }
        public string SalePrice { get; set; }
        public string MarketPrice { get; set; }
        public string pid1 { get; set; }
        public string val_id1 { get; set; }
        public string pid2 { get; set; }
        public string val_id2 { get; set; }
        public string pid3 { get; set; }
        public string val_id3 { get; set; }
        public string Norm { get; set; }
        public string Img { get; set; }
        public string BigImg { get; set; }
        public string SCoList { get; set; }
        public string ScoID { get; set; }
        public string ScoGoodsCode { get; set; }
        public string ScoSku { get; set; }
        public int SafeQty { get; set; }
        public string Remark { get; set; }
        public bool IsParent { get; set; }
        public string ParentID { get; set; }
        public string CoID { get; set; }
        public int Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public bool IsDelete { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }
    #endregion
    #region 商品主查詢
    public class GoodsQuery
    {
        public string ID { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string KindID { get; set; }
        public string KindName { get; set; }
        public string Enable { get; set; }
        public string SalePrice { get; set; }
        public string ScoGoodsCode { get; set; }
        public int Type { get; set; }
    }

    public class CoreKind
    {
        public string KindID { get; set; }
        public string KindName { get; set; }
    }
    #endregion

    #region 商品明細
    public class SkuQuery
    {
        public int ID { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string SkuSimple { get; set; }
        public string Norm { get; set; }
        public string GBCode { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string PurPrice { get; set; }
        public string MarketPrice { get; set; }
        public string CostPrice { get; set; }
        public string SalePrice { get; set; }
        public string Weight { get; set; }
        public string Enable { get; set; }
        public string ScoGoodsCode { get; set; }
        public string ScoSku { get; set; }
        public string SCoID { get; set; }
        public string Img { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }
    #endregion

    #region 商品管理 - 查询过滤条件
    public class CoreSkuParam
    {
        private int _CoID;//公司编号
        private string _Filter;//过滤条件
        private int _FilterType = 1;//过滤类型
        private string _Enable;//状态(0.禁用；1.启用；2.备用)
        private int _PageSize = 20;//每页笔数
        private int _PageIndex = 1;//页码
        private string _SortField;//排序字段
        private string _SortDirection = "ASC";//DESC,ASC
        private string _GoodsCode;
        private string _GoodsName;
        private string _SkuID;
        private string _KindID;//商品类目
        private int _Type = 0;
        public string SkuName { get; set; }//款式编码
        public string SkuSimple { get; set; }//商品简称
        public string Norm { get; set; }//颜色规格
        public string Brand { get; set; }//品牌
        public string ScoGoodsCode { get; set; }//供应商货号
        public string ScoSku { get; set; }//供应商款式编号
        public string SCoID { get; set; }//供应商名
        public string PriceS { get; set; }//成本价起
        public string PriceT { get; set; }//成本价讫

        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value; }
        }//公司编号
        public string Filter
        {
            get { return _Filter; }
            set { this._Filter = value; }
        }//过滤条件
        public int FilterType
        {
            get { return _FilterType; }
            set { this._FilterType = value; }
        }//过滤类型

        public string Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//状态(0.禁用；1.启用；2.备用)
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
        }//指定货号查询
        public string GoodsName
        {
            get { return _GoodsName; }
            set { this._GoodsName = value; }
        }//指定货品名称查询

        public string KindID
        {
            get { return _KindID; }
            set { this._KindID = value; }
        }//指定商品类目
        public string SkuID
        {
            get { return _SkuID; }
            set { this._SkuID = value; }
        }//指定Sku编码查询

        public int Type
        {
            get { return _Type; }
            set { this._Type = value; }
        }
    }
    #endregion
    #region 商品管理 - 查询返回列表
    public class CoreSkuQuery
    {
        public int DataCount { get; set; } //总行数
        public int PageCount { get; set; }//总页数
        public List<GoodsQuery> GoodsLst { get; set; }//货品主资料     
        public List<SkuQuery> SkuLst { get; set; }//商品Sku明细资料   
        // public List<ScoCompDDLB> ScoLst { get; set; }//供应商资料
        public Dictionary<string, object> ScoLst { get; set; }
        public Dictionary<string, object> BrandLst { get; set; }
        // public List<BrandDDLB> BrandLst { get; set; }//品牌资料
        // public Dictionary<string, string>{}
    }
    #endregion

    #region 商品维护 - Get单笔商品资料 
    public class CoreSkuAuto
    {
        public Coresku_main main { get; set; }
        public List<itemprops> itemprops_base { get; set; }//商品类目item属性基础资料
        public List<skuprops> skuprops_base { get; set; }//商品类目Sku属性基础资料
        public List<goods_item_props> itemprops { get; set; }//货品item属性
        public List<goods_sku_props> skuprops { get; set; }//货品Sku属性
        public List<CoreSkuItem> items { get; set; }

    }
    #endregion
    #region 商品维护 - Sku明细列表
    public class CoreSkuItem
    {
        public int ID { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string SkuSimple { get; set; }
        public string Norm { get; set; }
        public string ParentID { get; set; }
        public string pid1 { get; set; }
        public string val_id1 { get; set; }
        public string pid2 { get; set; }
        public string val_id2 { get; set; }
        public string pid3 { get; set; }
        public string val_id3 { get; set; }
        public string PurPrice { get; set; }
        public string SalePrice { get; set; }
        public string Weight { get; set; }

    }
    #endregion 

    #region 普通商品Sku明细 - sku属性资料
    public class DetailSkuProps
    {
        public string ParentID { get; set; }
        public string pid1 { get; set; }
        public string val_id1 { get; set; }
        public string pid2 { get; set; }
        public string val_id2 { get; set; }
        public string pid3 { get; set; }
        public string val_id3 { get; set; }
    }
    #endregion


    #region 商品资料-编号&名称&规格&小图
    public class CoreSkuView
    {
        public int ID { get; set; }
        public string GoodsCode { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
        public string Img { get; set; }

    }
    #endregion

    #region 商品吊牌打印 - 查询参数
    public class SkuPrintParam
    {
        public string GoodsCode { get; set; }
        public string ScoGoodsCode { get; set; }
        public string SkuID { get; set; }
        public string CoID { get; set; }
    }
    #endregion

    #region 商品吊牌打印-查询货号
    public class goods_print_goodscode
    {
        public int ID { get; set; }
        public string GoodsCode { get; set; }
    }
    #endregion

    public class goods_print_skuprops
    {
        public Dictionary<string, string> DicSkuProps { get; set; }
        public List<goods_sku_props> SkuProps { get; set; }
    }

    #region ######### CoreMat

    public class CoreSkuMatQuery
    {
        public int ID { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string KName { get; set; }
        public string Norm { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public decimal PurPrice { get; set; }
        public bool Enable { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }

    }


    public class CoreSkuMatParms
    {
        public int CoID { get; set; }
        public string SkuID { get; set; }
        public string Enable { get; set; }
        public string GoodsCode { get; set; }
        public string Search { get; set; }
        public int KID { get; set; }
        public List<int> KidList { get; set; }
    }


    public class MatQuery
    {
        public int DataCount { get; set; } //总行数
        public int PageCount { get; set; }//总页数
        public List<CoreSkuMatQuery> SkuLst { get; set; }//返回资料       
    }

    public partial class CoreSkuMatAuto
    {
        public int CoID { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string Norm { get; set; }
        public int KindID { get; set; }
        public string KindName { get; set; }
        public string SCoList { get; set; }
        public string Unit { get; set; }
        public string ValUnit { get; set; }
        public decimal CnvRate { get; set; }
        public decimal PurPrice { get; set; }
        public string ParentID { get; set; }
        public bool Enable { get; set; }
        public string Creator { get; set; }
        public string SkuID { get; set; }
        public int status { get; set; }
        public int mainew { get; set; }
        public List<CoreSkuMatItem> items { get; set; }
    }

    public partial class CoreSkuMatItem
    {
        public string GoodsCode { get; set; }
        public string SkuID { get; set; }
        public string ColorID { get; set; }
        public string ColorName { get; set; }
        public string SizeID { get; set; }
        public string SizeName { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public int status { get; set; }
    }
    #endregion

    #region 商品抓取Param（采购）
    public class SkuParam
    {
        public string GoodsCode { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
    }
    #endregion

    #region 通用查询参数 - Sku
    public class CommSkuParam
    {
        public int _CoID = 1;//公司编号
        public string _GoodsCode;//款式编码
        public string _SkuID;//商品编码
        public string _SCoID;//选择供应商
        public string _Brand;//商品品牌
        public string _Filter;//过滤条件
        public string _Enable = "all";//是否启用
        public int _PageSize = 20;//每页笔数
        public int _PageIndex = 1;//页码
        public string _SortField;//排序字段
        public string _SortDirection = "ASC";//DESC,ASC
        public string _Type;
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value; }
        }//公司编号
        public string GoodsCode
        {
            get { return _GoodsCode; }
            set { this._GoodsCode = value; }
        }//款式编码
        public string SkuID
        {
            get { return _SkuID; }
            set { this._SkuID = value; }
        }//商品编码
        public string SCoID
        {
            get { return _SCoID; }
            set { this._SCoID = value; }
        }//选择供应商
        public string Brand
        {
            get { return _Brand; }
            set { this._Brand = value; }
        }//商品品牌
        public string Filter
        {
            get { return _Filter; }
            set { this._Filter = value; }
        }//过滤条件
        public string Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//是否启用
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
        public string Type
        {
            get { return _Type; }
            set { this._Type = value; }
        }
    }
    #endregion

    #region 分仓抓取Sku
    public class wareSku
    {
        public int ID { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string Norm { get; set; }
    }

    public class wareGoods
    {
        public int ID { get; set; }
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string GoodsCode { get; set; }
        public string Norm { get; set; }
    }


    #endregion

    #region 天猫生成sku商品
    public class TmallSku
    {
        public string SkuID { get; set; }
        public string SkuName { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string SalePrice { get; set; }
        public string Norm { get; set; }
        public string Img { get; set; }
        public int CoID { get; set; }
        public bool Enable { get; set; }
        public string Creator { get; set; }
        public bool IsParent { set; get; }
        public int SafeQty { get; set; }

    }

    #endregion



}