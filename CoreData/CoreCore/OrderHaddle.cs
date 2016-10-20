using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreData.CoreCore
{
    public static class OrderHaddle
    {
        ///<summary>
        ///查询订单List
        ///</summary>
        public static DataResult GetOrderList(OrderParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select * from order where 1=1";
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(cp.ID > 0)//内部订单号
            {
                wheresql = wheresql + " and id = " + cp.ID;
            }
            if(cp.SoID > 0)//外部订单号
            {
                wheresql = wheresql + " and soid = " + cp.SoID;
            }
            if(!string.IsNullOrEmpty(cp.PayNbr))//付款单号
            {
               wheresql = wheresql + " and paynbr = '" + cp.PayNbr + "'";
            }
            if(!string.IsNullOrEmpty(cp.BuyerShopID))//买家账号
            {
               wheresql = wheresql + " and buyershopid like '%" + cp.BuyerShopID + "%'";
            }
            if(!string.IsNullOrEmpty(cp.ExCode))//快递单号
            {
               wheresql = wheresql + " and excode like '%" + cp.ExCode + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecName))//收货人
            {
               wheresql = wheresql + " and recname like '%" + cp.RecName + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecPhone))//手机
            {
               wheresql = wheresql + " and recphone like '%" + cp.RecPhone + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecTel))//固定电话
            {
               wheresql = wheresql + " and rectel like '%" + cp.RecTel + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecLogistics))//收货人省
            {
               wheresql = wheresql + " and reclogistics like '%" + cp.RecLogistics + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecCity))//收货人城市
            {
               wheresql = wheresql + " and reccity like '%" + cp.RecCity + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecDistrict))//收货人县区
            {
               wheresql = wheresql + " and recdistrict like '%" + cp.RecDistrict + "%'";
            }
            if(!string.IsNullOrEmpty(cp.RecAddress))//详细地址
            {
               wheresql = wheresql + " and recaddress like '%" + cp.RecAddress + "%'";
            }
            if (cp.StatusList != null)//状态List
            {
                wheresql = wheresql + " AND status in ("+ string.Join(",", cp.StatusList) + ")" ;
            }
            // if(cp.Status >= 0)//状态
            // {
            //    wheresql = wheresql + " and status = " + cp.Status;
            // }
            // if(cp.Scoid > 0)//供应商
            // {
            //    wheresql = wheresql + " and scoid = " + cp.Scoid;
            // }
            // if (cp.Warehouseid > 0)//仓库代号
            // {
            //     wheresql = wheresql + " AND warehouseid = "+ cp.Warehouseid ;
            // }
            // if(!string.IsNullOrEmpty(cp.Buyyer))//采购员
            // {
            //    wheresql = wheresql + " and buyyer = '" + cp.Buyyer + "'";
            // }
            // if(!string.IsNullOrEmpty(cp.Skuid))//商品编码
            // {
            //    wheresql = wheresql + " and exists(select skuid from purchasedetail where purchaseid = purchase.id and skuid = '" + cp.Skuid + "')";
            // }
            // if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            // {
            //     wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            // }
            var res = new OrderData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<Order>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<Order>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Ord = u;
                    result.d = res;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
    }
}