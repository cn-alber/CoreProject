using System;
using System.Collections.Generic;

namespace CoreModels.XyComm
{
       public class Shop
    {
        #region Model
        public int ID{get;set;}
        public string ShopName{get;set;}//店铺名称
        public int CoID{get;set;}
        public int SitType{get;set;}//店铺站点enum
        public string ShopSite{get;set;}//店铺归属平台
        public string ShopType{get;set;}
        public int? Istoken{get;set;}//是否被授权（0未授权，1授权，2过期）
        public string ShopUrl{get;set;}//店铺网址
        public string ShopSetting{get;set;}
        public bool Enable {get;set;}//启用店铺
        public string Creator{get;set;}
        public DateTime CreateDate {get;set;}
        public string ShortName{get;set;}//店铺简称
        public string Shopkeeper{get;set;}//掌柜昵称
        public string SendAddress{get;set;}//发货地址
        public string TelPhone{get;set;}//联系电话
        public string IDcard{get;set;}//身份证号
        public string ContactName{get;set;}//退货联系人
        public string ReturnAddress{get;set;}//退货地址
        public string ReturnMobile{get;set;}//退货手机
        public string ReturnPhone{get;set;}//退货固话
        public string Postcode{get;set;}//退货邮编
        public bool UpdateSku{get;set;}//上传库存（自动同步）
        public bool DownGoods{get;set;}//下载商品（自动同步）
        public bool UpdateWayBill{get;set;}//上传快递单（发货信息）
        public string Token{get;set;}    
        public DateTime ShopBegin {get;set;} // 创店日期
        #endregion
    }

     public class ShopQuery
    {
        #region Model
        public int ID{get;set;}
        public string ShopName{get;set;}
        public bool Enable {get;set;}
        public string ShopSite{get;set;}
        public string ShopUrl{get;set;}
        public string Shopkeeper{get;set;}
        public bool UpdateSku{get;set;}
        public bool DownGoods{get;set;}
        public bool UpdateWayBill{get;set;}
        public string TelPhone{get;set;}
        public string SendAddress{get;set;}
        public string CreateDate {get;set;}
        public int Istoken{get;set;} 
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
        public List<ShopQuery> ShopLst {get; set;}//返回资料        
    }

    public  class siteTree{
        public int id{get;set;}
        public string title{get;set;}
    }

}