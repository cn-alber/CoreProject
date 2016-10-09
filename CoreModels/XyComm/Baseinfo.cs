using System;
using System.Collections.Generic;
namespace CoreModels.XyComm
{
    public class Baseinfo
    {
        public int ID { get; set; }
        public string Kind { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Value { get; set; }
        public bool Enable { get; set; }
        public string Remark { get; set; }
        public int CoID { get; set; }
        public string Creator { get; set; }
        public string CreateDate { get; set; }
    }
    public class BaseinfoParam
    {
        private int _CoID;//公司编号
        private string _Filter;//过滤条件
        private string _Enable = "all";//是否启用
        private int _PageSize = 20;//每页笔数
        private int _PageIndex = 1;//页码
        private string _SortField;//排序字段
        private string _SortDirection = "ASC";//DESC,ASC
        private string _Kind = "数据类型";
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
        public string Kind
        {
            get { return _Kind; }
            set { this._Kind = value; }
        }
    }
    public class BaseinfoData
    {
        public int PageCount { get; set; }//总页数
        public int DataCount { get; set; } //总行数
        public List<Baseinfo> BaseinfoLst { get; set; }//返回查询结果
    }
}