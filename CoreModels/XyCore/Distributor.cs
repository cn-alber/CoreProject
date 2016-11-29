using System;

namespace CoreModels.XyCore
{
    public class distributor{
        #region Model
		private int _id;
		private int _distributorid;
		private string _distributorname;
		private int? _class;
		private string _remark1;
		private string _remark2;
		private int? _type=0;
		private bool _enable= false;
		private int? _coid;
		private string _creator;
		private DateTime? _createdate;
		private string _modifier;
		private DateTime? _modifydate;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 分销商代号
		/// </summary>
		public int DistributorID
		{
			set{ _distributorid=value;}
			get{return _distributorid;}
		}
		/// <summary>
		/// 分销商
		/// </summary>
		public string DistributorName
		{
			set{ _distributorname=value;}
			get{return _distributorname;}
		}
		/// <summary>
		/// 分销等级
		/// </summary>
		public int? Class
		{
			set{ _class=value;}
			get{return _class;}
		}
		/// <summary>
		/// 我方备注
		/// </summary>
		public string Remark1
		{
			set{ _remark1=value;}
			get{return _remark1;}
		}
		/// <summary>
		/// 对方备注
		/// </summary>
		public string Remark2
		{
			set{ _remark2=value;}
			get{return _remark2;}
		}
		/// <summary>
		/// 类别(0:分销商;1:供销商)
		/// </summary>
		public int? Type
		{
			set{ _type=value;}
			get{return _type;}
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
		/// <summary>
		/// 修改者
		/// </summary>
		public string Modifier
		{
			set{ _modifier=value;}
			get{return _modifier;}
		}
		/// <summary>
		/// 修改日期
		/// </summary>
		public DateTime? ModifyDate
		{
			set{ _modifydate=value;}
			get{return _modifydate;}
		}
		#endregion Model
    }

    public class distributorEnum{
        public int value{get;set;}
		public string label{get;set;}
    }

    public class supplierEnum{
        public int value{get;set;}
		public string label{get;set;}
    }
    
}