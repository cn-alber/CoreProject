using System.Collections.Generic;

namespace CoreModels.XyComm
{
    public class Customkind_props
    {
        public int id { get; set; }
        public bool name { get; set; }
        public long pid { get; set; }
        public bool is_allow_alias { get; set; }
        public bool is_color_prop { get; set; }
        public bool is_enum_prop { get; set; }
        public bool is_input_prop { get; set; }
        public bool is_item_prop { get; set; }
        public bool is_key_prop { get; set; }
        public bool is_material { get; set; }
        public string material_do { get; set; }
        public bool is_sale_prop { get; set; }
        public bool must { get; set; }
        public bool multi { get; set; }
        public long tb_cid { get; set; }
        public string values { get; set; }
        public bool Creator { get; set; }
        public bool CreateDate { get; set; }
        public bool Modifier { get; set; }
        public bool ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public bool CoID { get; set; }
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