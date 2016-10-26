
using System.Collections.Generic;

namespace CoreModels.XyApi.Tmall
{
public class CnTmsMailnoGetContentDomain 
	{
	        /// <summary>
	        /// 拓展字段
	        /// </summary>	        
	        public string extend_field { get; set; }
	
	        /// <summary>
	        /// 发货商品信息
	        /// </summary>
	        public List<CnTmsMailnoItemDomain> items { get; set; } 
	
	        /// <summary>
	        /// ERP订单编码
	        /// </summary>
	        public string order_code { get; set; }
	
	        /// <summary>
	        /// 来源渠道（TB 淘宝，JD 京东，TM 天猫，1688 1688（阿里中文站），YHD 1号店，DD 当当，VANCL 凡客，PP 拍拍，YX 易讯，EBAY 易贝ebay，AMAZON 亚马逊，SN 苏宁在线，GM 国美在线，WPH 唯品会，JM 聚美优品，LF 乐蜂网，MGJ 蘑菇街，JS 聚尚网，YG 优购，YT 银泰，YL 邮乐，PX 拍鞋网，POS POS门店，OTHERS 其他）
	        /// </summary>
	        public string order_source { get; set; }
	
	        /// <summary>
	        /// 包裹序号,如果同一订单获取多个包裹时,需要标记当前请求为第几个包裹
	        /// </summary>
	        public long package_no { get; set; }
	
	        /// <summary>
	        /// 收件人信息
	        /// </summary>
	        public CnTmsMailnoReceiverinfoDomain receiver_info { get; set; }
	
	        /// <summary>
	        /// 发件人信息
	        /// </summary>	       
	        public CnTmsMailnoSenderinfoDomain sender_info { get; set; }
	
	        /// <summary>
	        /// 店铺编码
	        /// </summary>	        
	        public string shop_code { get; set; }
	
	        /// <summary>
	        /// 解决方案CODE，由菜鸟提供
	        /// </summary>
	        public string solutions_code { get; set; }
	
	        /// <summary>
	        /// 交易单号
	        /// </summary>	        
	        public string trade_id { get; set; }
	}

    public class CnTmsMailnoReceiverinfoDomain 
	{
	        /// <summary>
	        /// 收件人地址
	        /// </summary>
	
	        public string receiver_address { get; set; }
	
	        /// <summary>
	        /// 收件人区县
	        /// </summary>
	   
	        public string receiver_area { get; set; }
	
	        /// <summary>
	        /// 收件人城市
	        /// </summary>
	       
	        public string receiver_city { get; set; }
	
	        /// <summary>
	        /// 收件人手机，手机与电话必须有一值不为空
	        /// </summary>	      
	        public string receiver_mobile { get; set; }
	
	        /// <summary>
	        /// 收件人姓名
	        /// </summary>	      
	        public string receiver_name { get; set; }
	
	        /// <summary>
	        /// 收件人昵称
	        /// </summary>
	        public string receiver_nick { get; set; }
	
	        /// <summary>
	        /// 收件人电话，手机与电话必须有一值不为空
	        /// </summary>
	        public string receiver_phone { get; set; }
	
	        /// <summary>
	        /// 收件人省份
	        /// </summary>
	        public string receiver_province { get; set; }
	
	        /// <summary>
	        /// 收件方邮编
	        /// </summary>
	        public string receiver_zip_code { get; set; }
	}

    public class CnTmsMailnoSenderinfoDomain 
	{
	        /// <summary>
	        /// 发件人地址
	        /// </summary>
	        public string sender_address { get; set; }
	
	        /// <summary>
	        /// 发件人区县
	        /// </summary>
	        public string sender_area { get; set; }
	
	        /// <summary>
	        /// 发件人城市
	        /// </summary>
	        public string sender_city { get; set; }
	
	        /// <summary>
	        /// 发件人手机，手机与电话必须有一值不为空
	        /// </summary>
	        public string sender_mobile { get; set; }
	
	        /// <summary>
	        /// 发件人姓名
	        /// </summary>	    
	        public string sender_name { get; set; }
	
	        /// <summary>
	        /// 发件人电话，手机与电话必须有一值不为空
	        /// </summary>	       
	        public string sender_phone { get; set; }
	
	        /// <summary>
	        /// 发件人省份
	        /// </summary>
	        public string sender_province { get; set; }
	
	        /// <summary>
	        /// 发件人邮编
	        /// </summary>	      
	        public string sender_zip_code { get; set; }
	}

    public class CnTmsMailnoItemDomain 
	{
	        /// <summary>
	        /// 发货商品名称
	        /// </summary>	   
	        public string item_name { get; set; }
	
	        /// <summary>
	        /// 发货商品数量
	        /// </summary>
	        public long item_qty { get; set; }
	}

}