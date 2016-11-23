using System;
using System.Collections.Generic;
namespace CoreModels.XyCore
{

    #region 商品主表
    public class Coresku_main
    {
        private int _Enable = 1;//可选值状态
        public int ID { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string KindID { get; set; }
        public string KindName { get; set; }
        public string ScoID { get; set; }
        public string ScoName { get; set; }
        public string ScoGoodsCode { get; set; }
        public string ScoSku { get; set; }
        public string Weight { get; set; }
        public string CostPrice { get; set; }
        public string PurPrice { get; set; }
        public string SalePrice { get; set; }
        public string MarketPrice { get; set; }
        public string TempShopID { get; set; }
        public string TempShopName { get; set; }
        public string TempID { get; set; }
        public string Remark { get; set; }
        public string Img { get; set; }
        public string CoID { get; set; }
        public int Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }
        public int Type { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }
    #endregion
    #region 商品属性表
    public class coresku_item_props
    {
        private int _Enable = 1;//可选值状态
        public int ID { get; set; }
        // public string GoodsCode { get; set; }
        public string pid { get; set; }
        public string val_id { get; set; }
        public string val_name { get; set; }
        public string CoID { get; set; }
        public int Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }
        public string ParentID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }
    #endregion

    public class coresku_sku_props
    {
        private int _Enable = 1;//可选值状态
        public int ID { get; set; }
        // public string GoodsCode { get; set; }
        public string pid { get; set; }
        public string val_id { get; set; }
        public string val_name { get; set; }
        public string mapping { get; set; }
        public int IsOther { get; set; }
        public string CoID { get; set; }
        public int Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }
        public string ParentID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }

    public class goods_item_props
    {
        private int _Enable = 1;//可选值状态
        public int ID { get; set; }
        public string pid { get; set; }
        public string val_id { get; set; }
        public string val_name { get; set; }
        // public bool Enable { get; set; }
        public int Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }
    }
    public class goods_sku_props
    {
        private int _IsOther = 0;//是否属于其他属性
        private int _Enable = 1;//可选值状态
        public int ID { get; set; }
        public string pid { get; set; }
        public string val_id { get; set; }
        public string val_name { get; set; }
        public string mapping { get; set; }
        public int IsOther
        {
            get { return _IsOther; }
            set { this._IsOther = value; }
        }
        public int Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }
    }
}