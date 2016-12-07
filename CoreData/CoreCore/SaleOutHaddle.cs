using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
using static CoreModels.Enum.OrderE;
using CoreData.CoreUser;
using CoreModels.XyUser;
namespace CoreData.CoreCore
{
    public static class SaleOutHaddle
    {
        ///<summary>
        ///查询销售出库List
        ///</summary>
        public static DataResult GetSaleOutList(SaleOutParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from saleout where 1=1";
            string sqlcommand = @"select ID,OID,SoID,DocDate,Status,ExpName,ExCode,BatchID,IsOrdPrint,IsExpPrint,RecMessage,RecLogistics,RecDistrict,RecCity,RecAddress,
                                  RecName,RecPhone,ExWeight,RealWeight,ShipType,ExCost,IsDeliver,Remark,OrdQty from saleout where 1=1"; 
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(cp.ID > 0)//出库单号
            {
                wheresql = wheresql + " and id = " + cp.ID;
            }
            if(cp.OID > 0)//内部订单号
            {
                wheresql = wheresql + " and oid = " + cp.OID;
            }
            if(cp.SoID > 0)//外部订单号
            {
                wheresql = wheresql + " and soid = " + cp.SoID;
            }
            if(!string.IsNullOrEmpty(cp.ExCode))//快递单号
            {
               wheresql = wheresql + " and excode like '%" + cp.ExCode + "%'";
            }
            if(cp.DateStart > DateTime.Parse("1900-01-01"))//日期起
            {
                wheresql = wheresql + " AND DocDate >= '" + cp.DateStart + "'" ;
            }
            if(cp.DateEnd > DateTime.Parse("1900-01-01"))//日期迄
            {
                wheresql = wheresql + " AND DocDate <= '" + cp.DateEnd + "'" ;
            }
            if(cp.Status >= 0)//状态
            {
                wheresql = wheresql + " and status = " + cp.Status;
            }
            if(!string.IsNullOrEmpty(cp.IsWeightYN) && cp.IsWeightYN.ToUpper() == "Y")
            {
                wheresql = wheresql + " and RealWeight = null";
            }
            if(!string.IsNullOrEmpty(cp.IsWeightYN) && cp.IsWeightYN.ToUpper() == "N")
            {
                wheresql = wheresql + " and RealWeight != null";
            }
            if(!string.IsNullOrEmpty(cp.SkuID))
            {
               wheresql = wheresql + " and exists(select id from saleoutitem where sid = saleout.id and skuid = '" + cp.SkuID + "')";
            }
            if(!string.IsNullOrEmpty(cp.GoodsCode))
            {
               wheresql = wheresql + " and exists(select id from saleoutitem where sid = saleout.id and GoodsCode = '" + cp.GoodsCode + "')";
            }
            if(cp.ExID > 0)//快递公司
            {
                wheresql = wheresql + " and ExID = " + cp.ExID;
            }
            if(!string.IsNullOrEmpty(cp.IsExpPrint) && cp.IsExpPrint.ToUpper() == "Y")
            {
                wheresql = wheresql + " and IsExpPrint = true";
            }
            if(!string.IsNullOrEmpty(cp.IsExpPrint) && cp.IsExpPrint.ToUpper() == "N")
            {
                wheresql = wheresql + " and IsExpPrint = false";
            }
            if(cp.ShopID >= 0)//店铺
            {
                wheresql = wheresql + " and ShopID = " + cp.ShopID;
            }
            if(!string.IsNullOrEmpty(cp.RecName))//收货人
            {
               wheresql = wheresql + " and recname like '%" + cp.RecName + "%'";
            }
            if(cp.BatchID == 0)//批次
            {
                wheresql = wheresql + " and BatchID = 0";
            }
            else if(cp.BatchID == 1)
            {
                wheresql = wheresql + " and BatchID > 0";
            }
            else if(cp.BatchID > 10)//店铺
            {
                wheresql = wheresql + " and BatchID = " + cp.BatchID;
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new SaleOutData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<SaleOutQuery>(sqlcommand + wheresql).AsList();
                    List<int> ItemID = new List<int>();
                    foreach(var a in u)
                    {
                        a.StatusString = Enum.GetName(typeof(SaleOutStatus), a.Status);
                        ItemID.Add(a.ID);
                    }
                    sqlcommand = @"select * from saleoutitem  where sid in @ID and coid = @Coid";
                    var item = conn.Query<SaleOutItemInsert>(sqlcommand,new{ID = ItemID,Coid = cp.CoID}).AsList();
                    foreach(var a in u)
                    {
                        string remark = a.OrdQty + ".";
                        foreach(var i in item)
                        {
                            if(a.ID == i.SID)
                            {
                                if(i.Qty == 1)
                                {
                                    remark = remark + i.SkuID + ",";
                                }
                                else
                                {
                                    remark = remark + i.SkuID + "*" + i.Qty + ",";
                                }
                            }
                        }
                        remark = remark.Substring(0,remark.Length - 1);
                        a.Sku = remark;
                    }
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.SaleOut = u;

                    result.d = res;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
        ///<summary>
        ///初始状态资料
        ///</summary>
        public static DataResult GetStatusInitData()                
        {
            var result = new DataResult(1,null);
            var filter = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(SaleOutStatus)))
            {
                var s = new Filter();
                s.value = myCode.ToString();
                s.label = Enum.GetName(typeof(SaleOutStatus), myCode);//获取名称
                filter.Add(s);
            }
            result.d = filter;
            return result;
        }
    }
}