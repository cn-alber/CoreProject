using System.Collections.Generic;

namespace CoreModels.XyComm
{
    public class Customkind_props_value
    {
        private int _kindid=0;
        private long _propid = 0;
        private bool _Enable = true;//是否启用
        private long _ParentID = 0;

        public int id { get; set; }
        public string name { get; set; }//属性可选值value
        public int kindid 
        {
            get { return _kindid; }
            set { this._kindid = value; }
        }//商品类目ID
        public long propid 
        {
            get { return _propid; }
            set { this._propid = value; }
        }//商品类目属性ID
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

        public long vid { get; set; }//线上属性可选值id     
        public long pid { get; set; }//线上属性prop ID      
        public long tb_cid { get; set; }//所属淘宝商品类目ID
        public int Order { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
        public int CoID { get; set; }
    }
}