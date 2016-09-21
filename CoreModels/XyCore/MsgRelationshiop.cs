using System;
namespace CoreModels.XyCore
{

	public  class MsgRelationshiop
	{
		
			#region Model
		private int _uid;
		private int _msgid;
		private bool _readed= false;
		private DateTime? _readtime;
		private string _uname;
		/// <summary>
		/// 
		/// </summary>
		public int Uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int MsgId
		{
			set{ _msgid=value;}
			get{return _msgid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool Readed
		{
			set{ _readed=value;}
			get{return _readed;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? ReadTime
		{
			set{ _readtime=value;}
			get{return _readtime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Uname
		{
			set{ _uname=value;}
			get{return _uname;}
		}
		#endregion Model

	}
}