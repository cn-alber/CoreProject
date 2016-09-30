namespace CoreModels.XyUser
{
    public class Power
    {

        #region Model
		private int _id;
		private string _name;
		private string _groupname;
		private string _title;
		private string _remark;
		private int _type=0;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 权限名称
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 权限组
		/// </summary>
		public string GroupName
		{
			set{ _groupname=value;}
			get{return _groupname;}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string Title
		{
			set{ _title=value;}
			get{return _title;}
		}
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		/// <summary>
		/// 权限类型(0:浏览;1:功能)
		/// </summary>
		public int Type
		{
			set{ _type=value;}
			get{return _type;}
		}
		#endregion Model
    }

    public class powerParam{

        public string Filter {get;set;}//过滤条件
        public int PageSize {get;set;}//每页笔数
        public int page {get;set;}//页码
        public decimal pageTotal {get;set;}//总页数
        public int total {get;set;} //总行数
        public string SortField {get; set;}//排序字段
        public string SortDirection {get;set;}//DESC,ASC

      
	}

    public class ViewPower{
        public int id{get;set;}
        public string acess{get;set;}
    }    



}