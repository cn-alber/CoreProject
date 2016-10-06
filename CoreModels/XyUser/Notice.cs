using System;
using System.Collections.Generic;
namespace CoreModels.XyUser
{
    public class Notice
    {

        public int ID { get; set; }
        public int Coid { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public bool Type { get; set; }
    }

    public class NoticeParam
    {
        public int CoID { get; set; }
        public string Enable { get; set; }
        public string Filter { get; set; }//过滤条件
        public int PageSize { get; set; }//每页笔数
        public int PageIndex { get; set; }//页码
        public string SortField { get; set; }//排序字段
        public string SortDirection { get; set; }//DESC,ASC  
    }
    public class NoticeData
	{
        public int PageCount {get;set;}//总页数
        public int DataCount {get;set;} //总行数
		public List<Notice> NoticeLst {get;set;}//返回查询结果
	}
}