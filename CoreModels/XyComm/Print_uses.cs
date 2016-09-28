using System;

namespace CoreModels.XyComm
{
    public partial class print_uses
	{
		#region Model
		private long _id;
		private long _sys_id=0;
		private int _type;
		private long _admin_id;
		private string _name;
		private string _print_setting;
		private string _tpl_data;
		private DateTime? _mdate;
		private bool _deleted= false;
		/// <summary>
		/// 
		/// </summary>
		public long id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 系统模板，0表示自创
		/// </summary>
		public long sys_id
		{
			set{ _sys_id=value;}
			get{return _sys_id;}
		}
		/// <summary>
		/// 冗余
		/// </summary>
		public int type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 
		/// </summary>
		public long admin_id
		{
			set{ _admin_id=value;}
			get{return _admin_id;}
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
		public string print_setting
		{
			set{ _print_setting=value;}
			get{return _print_setting;}
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
		public DateTime? mdate
		{
			set{ _mdate=value;}
			get{return _mdate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool deleted
		{
			set{ _deleted=value;}
			get{return _deleted;}
		}
		#endregion Model

	}



	public class singalUsesModel{
		public string name{get;set;}
		public int sys_id{get;set;}
		public string tpl_data{get;set;}
		public string print_setting{get;set;}
		public int type{get;set;}
		public string lodop_target{get;set;}
		
	}
	public class usesModel {
		public long id { get; set; }
		public string name { get; set; }
	}

	public class useslist{
		public long id { get; set; }
		public string name { get; set; }
		public string mdate{get;set;}

	}

	public class myTplModel {
		public long id { get; set; }
		public string name { get; set; }
		public bool defed { get; set; }
	}



}