using System;
using System.Collections.Generic;

namespace CoreModels.XyUser
{
    public class User
    {
        #region Model
        private int _id;
        private string _account;
        // private string _secretid;
        private string _name;
        private string _password;
        private bool _enable = false;
        // private string _email;
        // private string _gender= "男";
        // private string _mobile;
        // private string _qq;
        private int? _companyid;
        private int? _roleid;
        // private string _creator;
        // private DateTime? _createdate= DateTime.UtcNow;
        private bool _islocked = false;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account
        {
            set { _account = value; }
            get { return _account; }
        }
        // /// <summary>
        // /// 密匙
        // /// </summary>
        // public string SecretID
        // {
        // 	set{ _secretid=value;}
        // 	get{return _secretid;}
        // }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            set { _password = value; }
            get { return _password; }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable
        {
            set { _enable = value; }
            get { return _enable; }
        }
        // /// <summary>
        // /// 邮箱
        // /// </summary>
        // public string Email
        // {
        // 	set{ _email=value;}
        // 	get{return _email;}
        // }
        // /// <summary>
        // /// 性别
        // /// </summary>
        // public string Gender
        // {
        // 	set{ _gender=value;}
        // 	get{return _gender;}
        // }
        // /// <summary>
        // /// 联系电话
        // /// </summary>
        // public string Mobile
        // {
        // 	set{ _mobile=value;}
        // 	get{return _mobile;}
        // }
        // /// <summary>
        // /// QQ
        // /// </summary>
        // public string QQ
        // {
        // 	set{ _qq=value;}
        // 	get{return _qq;}
        // }
        /// <summary>
        /// 公司编号
        /// </summary>
        public int? CompanyID
        {
            set { _companyid = value; }
            get { return _companyid; }
        }
        /// <summary>
        /// 角色组
        /// </summary>
        public int? RoleID
        {
            set { _roleid = value; }
            get { return _roleid; }
        }
        // /// <summary>
        // /// 创建人
        // /// </summary>
        // public string Creator
        // {
        // 	set{ _creator=value;}
        // 	get{return _creator;}
        // }
        // /// <summary>
        // /// 创建时间
        // /// </summary>
        // public DateTime? CreateDate
        // {
        // 	set{ _createdate=value;}
        // 	get{return _createdate;}
        // }
        /// <summary>
        /// 用户是否锁屏
        /// </summary>
        public bool IsLocked
        {
            set { _islocked = value; }
            get { return _islocked; }
        }
        #endregion Model
    }

    #region 用户管理-查询结果
    public class UserQuery
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string CompanyName { get; set; }
        public string RoleName { get; set; }
        public string CreateDate { get; set; }
    }
    #endregion

	#region 用户管理 - 查询参数
	public class UserParam
	{
        private int _CoID ;//公司编号
        private string _Filter;//过滤条件
        private int _FilterType = 1;//过滤类型
        private string _Enable = "all";//是否启用
        private int _PageSize = 20;//每页笔数
        private int _PageIndex = 1;//页码
        private string _SortField;//排序字段
        private string _SortDirection = "ASC";//DESC,ASC

        public int CoID
        {
            get { return _CoID; }
            set { this._CoID = value; }
        }//公司编号
        public string Filter 
        {
            get { return _Filter; }
            set { this._Filter = value; }
        }//过滤条件
         public int FilterType 
        {
            get { return _FilterType; }
            set { this._FilterType = value; }
        }//过滤类型
        public string Enable 
        {
            get { return _Enable; }
            set { this._Enable = value; }
        }//是否启用
        public int PageSize 
        {
            get { return _PageSize; }
            set { this._PageSize = value; }
        }//每页笔数
        public int PageIndex 
        {
            get { return _PageIndex; }
            set { this._PageIndex = value; }
        }//页码
        public string SortField 
        {
            get { return _SortField; }
            set { this._SortField = value; }
        }//排序字段
        public string SortDirection 
        {
            get { return _SortDirection; }
            set { this._SortDirection = value; }
        }//DESC,ASC 
	}

	public class UserData
	{
        public int PageCount {get;set;}//总页数
        public int DataCount {get;set;} //总行数
		public List<UserQuery> UserLst {get;set;}//返回查询结果
	}
	#endregion

    #region
    public class UserEdit
    {
        public int ID{get;set;}
        public string Account{get;set;}
        public string SecretID{get;set;}
        public string Name{get;set;}
        public string PassWord{get;set;}
        public bool Enable{get;set;}
        public string Email{get;set;}
        public string Gender{get;set;}
        public string Mobile{get;set;}
        public string QQ{get;set;}
        public int CompanyID{get;set;}
        public int RoleID{get;set;}
        public string Creator{get;set;}
        public string CreateDate{get;set;}
        public string CompanyName { get; set; }
        public string RoleName { get; set; }
    }

    #endregion
}
