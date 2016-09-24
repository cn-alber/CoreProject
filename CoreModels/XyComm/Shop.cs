using System;
using System.Collections.Generic;

namespace CoreModels.XyComm
{
       public class Shop
    {
        #region Model
        public int ID{get;set;}
        public string ShopName{get;set;}
        public int? CoID{get;set;}
        public int? SitType{get;set;}
        public string ShopSite{get;set;}
        public string ShopType{get;set;}
        public int? Istoken{get;set;}
        public string ShopUrl{get;set;}
        public string ShopSetting{get;set;}
        public bool Enable {get;set;}
        public string Creator{get;set;}
        public DateTime CreateDate {get;set;}
        public string ShortName{get;set;}
        public string Shopkeeper{get;set;}
        public string SendAddress{get;set;}
        public string TelPhone{get;set;}
        public string IDcard{get;set;}
        public string ContactName{get;set;}
        public string ReturnAddress{get;set;}
        public string ReturnMobile{get;set;}
        public string ReturnPhone{get;set;}
        public string Postcode{get;set;}
        public bool? UpdateSku{get;set;}
        public bool? DownGoods{get;set;}
        public bool? UpdateWayBill{get;set;}
        public string Token{get;set;}      
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
        public DateTime CreateDate {get;set;}
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

}