namespace CoreModels.XyUser
{
    public class Role
    {
        #region Model
		private int _id;
		private string _name;
		private string _viewlist;
		private string _actionlist;
		private bool _issystem= false;
		private bool _isdrp= false;
		private bool _iswms= false;
		private int? _whid;
		private int? _companyid;
		/// <summary>
		/// RoleID
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 角色名称
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 浏览权限组
		/// </summary>
		public string ViewList
		{
			set{ _viewlist=value;}
			get{return _viewlist;}
		}
		/// <summary>
		/// 功能权限组
		/// </summary>
		public string ActionList
		{
			set{ _actionlist=value;}
			get{return _actionlist;}
		}
		/// <summary>
		/// 是否为系统权限
		/// </summary>
		public bool IsSystem
		{
			set{ _issystem=value;}
			get{return _issystem;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsDrp
		{
			set{ _isdrp=value;}
			get{return _isdrp;}
		}
		/// <summary>
		/// 是否物流账号
		/// </summary>
		public bool IsWms
		{
			set{ _iswms=value;}
			get{return _iswms;}
		}
		/// <summary>
		/// 仓库编号
		/// </summary>
		public int? WhID
		{
			set{ _whid=value;}
			get{return _whid;}
		}
		/// <summary>
		/// 公司编号
		/// </summary>
		public int? CompanyID
		{
			set{ _companyid=value;}
			get{return _companyid;}
		}
		#endregion Model
    }

	public class RoleList{
		public int id{get;set;}
		public string name{get;set;}
	}



}