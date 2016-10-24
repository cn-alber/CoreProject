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
        public int ID { get; set; }
        public string Type { get; set; }
        public string KindName { get; set; }
        public string FullName { get; set; }
        public bool Enable
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//是否启用
        public int Order { get; set; }
        public int ParentID { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
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
}