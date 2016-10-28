using System.Collections.Generic;

namespace CoreModels.XyComm
{
    public class Customkind_props
    {
        private bool _must = false; //是否必选
        private bool _multi = false; //是否多选
        private bool _is_allow_alias = false;//是否允许别名        
        private bool _is_enum_prop = false;//是否枚举      
        private bool _is_input_prop = false;//在is_enum_prop是true的前提下，是否是卖家可以自行输入      
        private bool _is_key_prop = false;//是否关键属性      
        private bool _is_sale_prop = false;//是否销售属性
        private bool _Enable = true;//是否启用
        private long _ParentID = 0;
        public int id { get; set; }
        public int kindid { get; set; }
        public string name { get; set; }
        public string values { get; set; }
        public bool must
        {
            get { return _must; }
            set { this._must = value; }
        }
        public bool multi
        {
            get { return _multi; }
            set { this._multi = value; }
        }
        public bool is_allow_alias
        {
            get { return _is_allow_alias; }
            set { this._is_allow_alias = value; }
        }
        public bool is_enum_prop
        {
            get { return _is_enum_prop; }
            set { this._is_enum_prop = value; }
        }
        public bool is_input_prop
        {
            get { return _is_input_prop; }
            set { this._is_input_prop = value; }
        }
        public bool is_key_prop
        {
            get { return _is_key_prop; }
            set { this._is_key_prop = value; }
        }
        public bool is_sale_prop
        {
            get { return _is_sale_prop; }
            set { this._is_sale_prop = value; }
        }
        public bool Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//是否启用
        public long ParentID
        {
            get { return _ParentID; }
            set { this._ParentID = value; }
        }
        public long pid { get; set; }
        public long tb_cid { get; set; }
        public int Order { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
        public int CoID { get; set; }
        public string PropValues { get; set; }
        public List<string> ValLst { get; set; }
    }

    public class item_props
    {
        public object item_prop { get; set; }
    }
    public class sku_props
    {
        public object item_prop { get; set; }
    }
    public class item_sku_props
    {
        public List<item_props> item { get; set; }
        public List<sku_props> sku { get; set; }
    }
}