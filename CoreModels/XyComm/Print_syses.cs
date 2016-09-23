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

}