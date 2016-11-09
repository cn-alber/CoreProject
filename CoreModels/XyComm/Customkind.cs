using System.Collections.Generic;
namespace CoreModels.XyComm
{
    public class CusKindParam
    {
        private int _CoID;//公司编号
        private string _Enable = "all";//是否启用
        private string _Type = "商品类目";
        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value; }
        }//公司编号

        public string Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//是否启用

        public string Type
        {
            get { return _Type; }
            set { this._Type = value; }
        }
        public int ParentID { get; set; }
    }


    public class CustomKind
    {

        private bool _Enable = true;//是否启用
        private string _Type = "商品类目";
        private int _ParentID = 0;
        private long _tb_cid = 0;
        public int ID { get; set; }
        public string Type
        {
            get { return _Type; }
            set { this._Type = value; }
        }
        public string KindName { get; set; }
        public string FullName { get; set; }
        public bool Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//是否启用
        public int Order { get; set; }
        public int ParentID
        {
            get { return _ParentID; }
            set { this._ParentID = value; }
        }
        public long tb_cid
        {
            get { return _tb_cid; }
            set { this._tb_cid = value; }
        }
        public long cid { get; set; }//卖家自定义类目编号
        public long parent_cid { get; set; } //卖家自定义父类目编号，
        public string pic_url { get; set; }//链接图片地址
        public int mode { get; set; }
        public string norm { get; set; }
        public List<string> NormLst { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public string Modifier { get; set; }
        public string ModifyDate { get; set; }
    }

    public class CustomKindname
    {
        public int ID { get; set; }
        public string KindName { get; set; }
        public bool Enable { get; set; }
        public int ParentID { get; set; }
        public List<CustomKindname> Children { get; set; }
    }
    public class CustomKindData
    {
        public int ID { get; set; }
        // public string Type { get; set; }
        public string KindName { get; set; }
        public bool Enable { get; set; }
        public int Order { get; set; }
        // public string FullName { get; set; }
        public int ParentID { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
        public List<CustomKindData> Children { get; set; }
    }

    public class CustomCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }        
        public bool IsParent{get;set;}

    }


    public class TmaoData
    {
        public long cid { get; set; }
        public string name { get; set; }
        public string pic_url { get; set; }
        public long parent_cid { get; set; }
        public int sort_order { get; set; }
        public string type { get; set; }
        public List<TmaoData> children { get; set; }
    }
}
