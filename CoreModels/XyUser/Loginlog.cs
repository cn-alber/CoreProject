using System;

namespace CoreModels.XyUser
{
    public class Loginlog
    {
#region Model
		private int _id;
		private int? _uid;
		private string _useragent;
		private DateTime? _logintime;
		private string _ip;
		/// <summary>
		/// 
		/// </summary>
		public int id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// 浏览器代理
		/// </summary>
		public string useragent
		{
			set{ _useragent=value;}
			get{return _useragent;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? logintime
		{
			set{ _logintime=value;}
			get{return _logintime;}
		}
		/// <summary>
		/// ip地址
		/// </summary>
		public string ip
		{
			set{ _ip=value;}
			get{return _ip;}
		}
		#endregion Model

    }
}