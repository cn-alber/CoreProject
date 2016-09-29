namespace CoreModels.XyComm
{

    public partial class print_use_setting
	{

		#region Model
		private long _id;
		private long _admin_id;
		private long? _defed_id=0;
		private string _lodop_target;
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
		public long admin_id
		{
			set{ _admin_id=value;}
			get{return _admin_id;}
		}
		/// <summary>
		/// 设为默认的模板ID
		/// </summary>
		public long? defed_id
		{
			set{ _defed_id=value;}
			get{return _defed_id;}
		}
		/// <summary>
 		/// 云打印目标地址
 		/// </summary>
 		public string lodop_target
 		{
 			set{ _lodop_target=value;}
 			get{return _lodop_target;}
 		}
		#endregion Model

	}
    
}