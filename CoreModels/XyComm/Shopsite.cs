using System;

namespace CoreModels.XyComm
{
    public  class shopsite
	{

		#region Model
		private int _id;
		private string _shopsite;
		private string _shoptype;
		private string _shopalias;
		private bool _enable= false;
		private int? _sort;
		private int? _coid;
		private string _creator;
		private DateTime? _createdate;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 平台名称
		/// </summary>
		public string ShopSite
		{
			set{ _shopsite=value;}
			get{return _shopsite;}
		}
		/// <summary>
		/// 平台类型
		/// </summary>
		public string ShopType
		{
			set{ _shoptype=value;}
			get{return _shoptype;}
		}
		/// <summary>
		/// 平台别名
		/// </summary>
		public string ShopAlias
		{
			set{ _shopalias=value;}
			get{return _shopalias;}
		}
		/// <summary>
		/// 是否启用
		/// </summary>
		public bool Enable
		{
			set{ _enable=value;}
			get{return _enable;}
		}
		/// <summary>
		/// 排序
		/// </summary>
		public int? Sort
		{
			set{ _sort=value;}
			get{return _sort;}
		}
		/// <summary>
		/// 公司编号
		/// </summary>
		public int? CoID
		{
			set{ _coid=value;}
			get{return _coid;}
		}
		/// <summary>
		/// 创建者
		/// </summary>
		public string Creator
		{
			set{ _creator=value;}
			get{return _creator;}
		}
		/// <summary>
		/// 创建日期
		/// </summary>
		public DateTime? CreateDate
		{
			set{ _createdate=value;}
			get{return _createdate;}
		}
		#endregion Model
    }
}