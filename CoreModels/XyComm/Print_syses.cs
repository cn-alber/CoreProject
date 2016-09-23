using System;

namespace CoreModels.XyComm
{
    public partial class print_syses
	{
		#region Model
		private long _id;
		private long _type;
		private string _name;
		private string _tpl_data;
		private string _setting;
		private DateTime? _mtime;
		/// <summary>
		/// 
		/// </summary>
		public long id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public long type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string tpl_data
		{
			set{ _tpl_data=value;}
			get{return _tpl_data;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string setting
		{
			set{ _setting=value;}
			get{return _setting;}
		}
		/// <summary>
		/// 编辑时间
		/// </summary>
		public DateTime? mtime
		{
			set{ _mtime=value;}
			get{return _mtime;}
		}
		#endregion Model

	}

	public class printSysesList{
		public int id{get;set;}
		public string name{get;set;}
		public string mtime{get;set;}

	}

	///<summary>
	///分页	
	///<summary>
	 /// <param name="Filter">过滤条件</param>
	public class printParam{

        public string Filter {get;set;}//过滤条件
        public int PageSize {get;set;}//每页笔数
        public int PageIndex {get;set;}//页码
        public int PageCount {get;set;}//总页数
        public int DataCount {get;set;} //总行数
        public string SortField {get; set;}//排序字段
        public string SortDirection {get;set;}//DESC,ASC
          
	}





}