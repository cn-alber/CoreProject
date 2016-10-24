using System.Collections.Generic;
namespace CoreModels.XyComm
{
    public class Item_cates_standard
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parentid { get; set; }
        public int tb_cid { get; set; }
        public int parent_tb_cid { get; set; }
        public bool is_tb_parent { get; set; }
        public bool tb_loaded { get; set; }
    }

    public class ItemCateStdData
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parent_id { get; set; }
        // public int tb_cid { get; set; }
        // public int parent_tb_cid { get; set; }
        public bool is_parent { get; set; }
        // // public bool tb_loaded { get; set; }
        // public List<ItemCateStdData> Children { get; set; }
    }

}