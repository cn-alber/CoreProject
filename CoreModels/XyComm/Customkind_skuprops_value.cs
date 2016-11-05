using System;
using System.Collections.Generic;

namespace CoreModels.XyComm
{
    public class Customkind_skuprops_value
    {
        private bool _Enable = true;//是否启用
        private bool _IsDelete = false;//是否已删除
        public int id { get; set; }
        public string mapping { get; set; }//映射
        public string name { get; set; }//属性可选值value
        public string pid { get; set; }
        public long vid { get; set; }
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

    public class skuprops_value_data
    {
        public long pid { get; set; }
        public long vid { get; set; }
    }

    public class skuprops_value
    {
        
        public string pid { get; set; }
        public int id { get; set; }
        public string mapping { get; set; }//映射
        public string name { get; set; }//属性可选值value
    }
}