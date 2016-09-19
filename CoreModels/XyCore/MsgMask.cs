
namespace CoreModels.XyCore
{
	
	public  class MsgMask
	{
		#region Model
		private int _uid;
		private int? _msgmaskid;
		/// <summary>
		/// 用户id
		/// </summary>
		public int Uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// 获取到的最新消息id
		/// </summary>
		public int? MsgMaskId
		{
			set{ _msgmaskid=value;}
			get{return _msgmaskid;}
		}
		#endregion Model

	}
}