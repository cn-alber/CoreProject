using System;
using System.Collections.Generic;
namespace CoreModels.XyComm
{
    public class Shop
    {
        #region Model
        
        private int _ID;
        private string _ShopName;
        private int? _CoID;
        private int? _SitType;
        private string _ShopSite;
        private string _ShopType;
        private int? _Istoken;
        private string _ShopUrl;
        private string _ShopSetting;
        private bool _Enable = false;
        private string _Creator;
        private DateTime _CreateDate = DateTime.UtcNow;
        private string _ShortName;
        private string _Shopkeeper;
        private string _SendAddress;
        private string _TelPhone;
        private string _IDcard;
        private string _ContactName;
        private string _ReturnAddress;
        private string _ReturnMobile;
        private string _ReturnPhone;
        private string _Postcode;
        private bool? _UpdateSku;
        private bool? _DownGoods;
        private bool? _UpdateWayBill;
        private string _Token;
        // private DateTime? _ShopBegin;
        /// <summary>
		/// 
        /// </summary>	
         public int ID
        {
            get { return _ID; }
            set
            {                
                _ID = value;
            }
        }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string ShopName
        {
            get { return _ShopName; }
            set
            {                
                _ShopName = value;
            }
        }
        /// <summary>
        /// 公司编号
        /// </summary>
        public int? CoID
        {
            get { return _CoID; }
            set
            {
                _CoID = value;
            }
        }
        /// <summary>
        /// 店铺站点enum
        /// </summary>
        public int? SitType
        {
            get { return _SitType; }
            set
            {
                _SitType = value;
            }
        }
        /// <summary>
        /// 店铺归属平台
        /// </summary>
        public string ShopSite
        {
            get { return _ShopSite; }
            set
            {
                _ShopSite = value;
            }
        }
        /// <summary>
        /// 平台类型
        /// </summary>
        public string ShopType
        {
            get { return _ShopType; }
            set
            {
                _ShopType = value;
            }
        }
        /// <summary>
        /// 是否被授权（0未授权，1授权，2过期）
        /// </summary>
        public int? Istoken
        {
            get { return _Istoken; }
            set
            {
                _Istoken = value;
            }
        }
        /// <summary>
        /// 店铺地址
        /// </summary>
        public string ShopUrl
        {
            get { return _ShopUrl; }
            set
            {
                _ShopUrl = value;
            }
        }
        /// <summary>
        /// 店铺设置
        /// </summary>
        public string ShopSetting
        {
            get { return _ShopSetting; }
            set
            {
                _ShopSetting = value;
            }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable
        {
            get { return _Enable; }
            set
            {
               _Enable = value;
            }
        }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator
        {
            get { return _Creator; }
            set
            {
                _Creator = value;
            }
        }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value;}
        }
        /// <summary>
        /// 店铺简称
        /// </summary>
        public string ShortName
        {
            get { return _ShortName; }
            set
            {
                _ShortName = value;
            }
        }
        /// <summary>
        /// 掌柜昵称
        /// </summary>
        public string Shopkeeper
        {
            get { return _Shopkeeper; }
            set
            {
                _Shopkeeper = value;
            }
        }
        /// <summary>
        /// 发货地址
        /// </summary>
        public string SendAddress
        {
            get { return _SendAddress; }
            set
            {
                _SendAddress = value;
            }
        }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string TelPhone
        {
            get { return _TelPhone; }
            set
            {
                _TelPhone = value;
            }
        }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDcard
        {
            get { return _IDcard; }
            set
            {
                _IDcard = value;
            }
        }
        /// <summary>
        /// 退货联系人
        /// </summary>
        public string ContactName
        {
            get { return _ContactName; }
            set
            {
                _ContactName = value;
            }
        }
        /// <summary>
        /// 退货地址
        /// </summary>
        public string ReturnAddress
        {
            get { return _ReturnAddress; }
            set
            {
                _ReturnAddress = value;
            }
        }
        /// <summary>
        /// 退货手机
        /// </summary>
        public string ReturnMobile
        {
            get { return _ReturnMobile; }
            set
            {
                _ReturnMobile = value;
            }
        }
        /// <summary>
        /// 退货固话
        /// </summary>
        public string ReturnPhone
        {
            get { return _ReturnPhone; }
            set
            {
                _ReturnPhone = value;
            }
        }
        /// <summary>
        /// 退货邮编
        /// </summary>
        public string Postcode
        {
            get { return _Postcode; }
            set
            {
               _Postcode = value;
            }
        }
        /// <summary>
        /// 上传库存
        /// </summary>
        public bool? UpdateSku
        {
            get { return _UpdateSku; }
            set
            {
                _UpdateSku = value;
            }
        }
        /// <summary>
        /// 下载商品
        /// </summary>
        public bool? DownGoods
        {
            get { return _DownGoods; }
            set
            {
                _DownGoods = value;
            }
        }
        /// <summary>
        /// 下载快递单（发货信息）
        /// </summary>
        public bool? UpdateWayBill
        {
            get { return _UpdateWayBill; }
            set
            {
                _UpdateWayBill = value;
            }
        }
        /// <summary>
        /// 授权token
        /// </summary>
        public string Token
        {
            get { return _Token; }
            set
            {
                _Token = value;
            }
        }
        /// <summary>
        /// 店铺创建运营时间
        /// </summary>        
        // public DateTime? ShopBegin
        // {
        //     get { return _ShopBegin; }
        //     set
        //     {
        //         ShopBegin = value;
        //     }
        // }
        #endregion
    }

     public class ShopParam
    {
        public int CoID {get;set;}//公司编号
        public string Enable {get;set;}//是否启用
        public string Filter {get;set;}//过滤条件
        public int PageSize {get;set;}//每页笔数
        public int PageIndex {get;set;}//页码
        public int PageCount {get;set;}//总页数
        public int DataCount {get;set;} //总行数
        public string SortField {get; set;}//排序字段
        public string SortDirection {get;set;}//DESC,ASC
        public List<Shop> ShopLst {get; set;}//返回资料        
    }

}