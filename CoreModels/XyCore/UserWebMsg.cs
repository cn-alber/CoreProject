using System;

namespace CoreModels.XyCore{
    public  class UserWebMsg
        {
            #region Model
		private int _id;
		private int? _coid;
		private string _content;
		private int? _level;
		private DateTime? _create;
		private int? _creater;
		private int? _appoint;
		private string _roletype;
		/// <summary>
		/// 
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? CoId
		{
			set{ _coid=value;}
			get{return _coid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Content
		{
			set{ _content=value;}
			get{return _content;}
		}
		/// <summary>
		/// 消息等级
		/// </summary>
		public int? Level
		{
			set{ _level=value;}
			get{return _level;}
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime? Create
		{
			set{ _create=value;}
			get{return _create;}
		}
		/// <summary>
		/// 消息创建者id
		/// </summary>
		public int? Creater
		{
			set{ _creater=value;}
			get{return _creater;}
		}
		/// <summary>
		/// 全局或分组或个人
		/// </summary>
		public int? Appoint
		{
			set{ _appoint=value;}
			get{return _appoint;}
		}
		/// <summary>
		/// 推送角色
		/// </summary>
		public string RoleType
		{
			set{ _roletype=value;}
			get{return _roletype;}
		}
		#endregion Model

        }

    public class MsgParam 
    {
        public string LevelList { get; set; }
        public string IsRead { get; set; }

        public int PageIndex{get;set;}
        public int PageSize{get;set;}
        public int PageCount {get;set;}//总页数
        public int DataCount {get;set;} //总行数
        public string SortField {get; set;}//排序字段
        public string SortDirection {get;set;}//DESC,ASC




    }

    public class NotifyMsg{
		public int 	Id{get;set;}
        public string  MsgLevel{get;set;}
        public string Msg{get;set;}
        public string CreateDate{get;set;}
        public bool Isreaded{get;set;}
        public string Reador{get;set;}
        public string  ReadDate{get;set;}       

    }




}