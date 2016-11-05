using System;
using System.Collections.Generic;

namespace CoreModels.XyComm
{
    public class Customkind_skuprops
    {
        private int _kindid = 0;
        private bool _Enable = true;//是否启用
        private bool _IsDelete = false;//是否已删除

        public int id { get; set; }
        public string name { get; set; }//属性可选值value
        public int kindid
        {
            get { return _kindid; }
            set { this._kindid = value; }
        }//商品类目ID
        public bool is_color_prop { get; set; }
        public long pid { get; set; }
        public long tb_cid { get; set; }
        public int Order { get; set; }
        public bool Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//是否启用
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
        public bool IsDelete
        {
            get { return _IsDelete; }
            set { this._IsDelete = value; }
        }
        public int CoID { get; set; }
    }

    public class skuprops_data
    {
        public long pid { get; set; }
        public string name { get; set; }//属性可选值value        
    }


    public class skuprops
    {
        public string pid { get; set; }
        public string name { get; set; }
        public string KindNames { get; set; }
        public List<skuprops_value> skuprops_values { get; set; }
    }

}