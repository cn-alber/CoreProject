using System.Collections.Generic;
namespace CoreModels.XyComm
{
    public class Brand
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
        public string Link { get; set; }
        public bool Enable { get; set; }
        // public string Remark { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
    }
    public class BrandParam
    {
        private int _CoID;//公司编号
        private string _Filter;//过滤条件
        private string _Enable = "all";//是否启用
        private int _PageSize = 20;//每页笔数
        private int _PageIndex = 1;//页码
        private string _SortField;//排序字段
        private string _SortDirection = "ASC";//DESC,ASC
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
    }
     public class BrandData
    {
        public int PageCount { get; set; }//总页数
        public int DataCount { get; set; } //总行数
        public List<Brand> BrandLst { get; set; }//返回查询结果
    }

    public class BrandDDLB
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
    }

}