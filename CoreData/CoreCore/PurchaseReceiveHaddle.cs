using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
// using System.Collections.Generic;

namespace CoreData.CoreCore
{
    public static class PurchaseReceiveHaddle
    {
        ///<summary>
        ///查询采购单收料单List
        ///</summary>
        public static DataResult GetPurchaseRecList(PurchaseReceiveParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "where 1 = 1"; //采购日期
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if (!string.IsNullOrEmpty(cp.Recid))//收料单号
            {
                wheresql = wheresql + " AND receiveid like '%"+ cp.Recid + "%'";
            }
            if (!string.IsNullOrEmpty(cp.Purid))//采购单号
            {
                wheresql = wheresql + " AND purchaseid like '%"+ cp.Purid + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Skuname))//商品名称
            {
               wheresql = wheresql + " and skuname like '%" + cp.Skuname + "%'";
            }
            if(!string.IsNullOrEmpty(cp.Warehousename))//仓库名称
            {
               wheresql = wheresql + " and warehousename like '%" + cp.Warehousename + "%'";
            }
            if(cp.Status >= 0)//状态
            {
               wheresql = wheresql + " and status = " + cp.Status;
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            wheresql = "select id,receiveid,purchaseid,warehouseid,warehousename,receivedate,skuid,skuname,norm,status,recqty,creator,createdate from purchasereceive " + wheresql ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<PurchaseReceive>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<PurchaseReceive>(wheresql).AsList();

                    cp.Datacnt = count;
                    cp.Pagecnt = pagecnt;
                    cp.Rec = u;
                    if (count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        result.d = cp;
                    }               
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