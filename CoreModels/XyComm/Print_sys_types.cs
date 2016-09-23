namespace CoreModels.XyComm
{
    public partial class print_sys_types
	{
		#region Model
		private long _id;
		private int _type;
		private string _name;
		private string _presets;
		private string _emu_data;
		private string _setting;
		/// <summary>
		/// 打印模板-系统组
		/// </summary>
		public long id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 模板名
		/// </summary>
		public string name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 预设数据
		/// </summary>
		public string presets
		{
			set{ _presets=value;}
			get{return _presets;}
		}
		/// <summary>
		/// 模拟测试数据
		/// </summary>
		public string emu_data
		{
			set{ _emu_data=value;}
			get{return _emu_data;}
		}
		/// <summary>
		/// pageW and pageH
		/// </summary>
		public string setting
		{
			set{ _setting=value;}
			get{return _setting;}
		}
		#endregion Model

	}

}