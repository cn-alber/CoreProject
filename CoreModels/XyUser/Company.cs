using System;
namespace CoreModels.XyUser
{
    public class Company
    {
        #region Model
        //private int _id;
        private string _name;
        private bool _enable = false;
       // private string _contacts;
        private string _address;
       // private string _email;
        //private string _telphone;
        //private string _mobile;
        private string _remark;
        private string _typelist;
        //private string _warehouselist;
        //private string _sender;
        //private string _sendphone;
        //private string _sendlogistics;
        //private string _sendcity;
        //private string _senddistrict;
       // private string _sendaddress;
        //private string _sendremark;
        private string _creator;
        private DateTime _createdate = DateTime.UtcNow;
        /// <summary>
		/// 
        /// </summary>		
		// public int ID
        // {
        //     get{ return _id; }
        //     set{ _id = value; }
        // }        
		/// <summary>
		/// 公司名称
        /// </summary>				
        public string Name
        {
            get{ return _name; }
            set{ _name = value; }
        }        
		/// <summary>
		/// 是否启用
        /// </summary>				
        public bool Enable
        {
            get{ return _enable; }
            set{ _enable = value; }
        }        
		/// <summary>
		/// 联系人
        /// </summary>				
        // public string Contacts
        // {
        //     get{ return _contacts; }
        //     set{ _contacts = value; }
        // }        
		/// <summary>
		/// 公司地址
        /// </summary>				
        public string Address
        {
            get{ return _address; }
            set{ _address = value; }
        }        
		/// <summary>
		/// 邮箱
        /// </summary>				
        // public string Email
        // {
        //     get{ return _email; }
        //     set{ _email = value; }
        // }        
		// /// <summary>
		// /// 固定电话
        // /// </summary>				
        // public string TelPhone
        // {
        //     get{ return _telphone; }
        //     set{ _telphone = value; }
        // }        
		// /// <summary>
		// /// 移动电话
        // /// </summary>		
        // public string Mobile
        // {
        //     get{ return _mobile; }
        //     set{ _mobile = value; }
        // }        
		/// <summary>
		/// 备注
        /// </summary>		
        public string Remark
        {
            get{ return _remark; }
            set{ _remark = value; }
        }        
		/// <summary>
		/// 公司类型
        /// </summary>		
        public string TypeList
        {
            get{ return _typelist; }
            set{ _typelist = value; }
        }        
		/// <summary>
		/// 仓库列表
        /// </summary>		
        // public string WareHouseList
        // {
        //     get{ return _warehouselist; }
        //     set{ _warehouselist = value; }
        // }        
		// /// <summary>
		// /// 寄件人
        // /// </summary>		
        // public string Sender
        // {
        //     get{ return _sender; }
        //     set{ _sender = value; }
        // }        
		// /// <summary>
		// /// 寄件人电话
        // /// </summary>		
        // public string SendPhone
        // {
        //     get{ return _sendphone; }
        //     set{ _sendphone = value; }
        // }        
		// /// <summary>
		// /// 寄件人省份
        // /// </summary>		
        // public string SendLogistics
        // {
        //     get{ return _sendlogistics; }
        //     set{ _sendlogistics = value; }
        // }        
		// /// <summary>
		// /// 寄件人市区
        // /// </summary>		
        // public string SendCity
        // {
        //     get{ return _sendcity; }
        //     set{ _sendcity = value; }
        // }        
		// /// <summary>
		// /// 寄件人县市
        // /// </summary>		
        // public string SendDistrict
        // {
        //     get{ return _senddistrict; }
        //     set{ _senddistrict = value; }
        // }        
		// /// <summary>
		// /// 寄件人地址
        // /// </summary>		
        // public string SendAddress
        // {
        //     get{ return _sendaddress; }
        //     set{ _sendaddress = value; }
        // }        
		// /// <summary>
		// /// 寄件人备注
        // /// </summary>		
        // public string SendRemark
        // {
        //     get{ return _sendremark; }
        //     set{ _sendremark = value; }
        // }        
		/// <summary>
		/// 创建人
        /// </summary>		
        public string Creator
        {
            get{ return _creator; }
            set{ _creator = value; }
        }        
		/// <summary>
		/// 创建日期
        /// </summary>		
        public DateTime CreateDate
        {
            get{ return _createdate; }
            set{ _createdate = value; }
        }     
        #endregion Model
    }
}