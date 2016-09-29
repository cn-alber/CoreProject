using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;

namespace CoreData.CoreCore
{
    public static class PurchaseHaddle
    {
        ///<summary>
        ///查询采购单List
        ///</summary>
        public static DataResult GetPurchaseList(PurchaseParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "where purchasedate between '" + cp.PurdateStart + "' and '" + cp.PurdateEnd + "'"; //采购日期
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if (!string.IsNullOrEmpty(cp.Purid))//采购单号
            {
                wheresql = wheresql + " AND purchaseid like '%"+ cp.Purid + "%'";
            }
            if(cp.Status >= 0)//状态
            {
               wheresql = wheresql + " and status = " + cp.Status;
            }
            if(!string.IsNullOrEmpty(cp.CoName))//供应商
            {
               wheresql = wheresql + " and coname = '" + cp.CoName + "'";
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            wheresql = "select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase " + wheresql ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<Purchase>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<Purchase>(wheresql).AsList();

                    cp.Datacnt = count;
                    cp.Pagecnt = pagecnt;
                    cp.Com = u;
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
        ///<summary>
        ///查询采购单明细List
        ///</summary>
        public static DataResult GetPurchaseDetailList(PurchaseDetailParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "where purchaseid = '" + cp.Purid + "'"; //采购单号
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if (!string.IsNullOrEmpty(cp.Skuid))//商品代号
            {
                wheresql = wheresql + " AND skuid like '%"+ cp.Skuid + "%'";
            }
            if(!string.IsNullOrEmpty(cp.SkuName))//商品名称
            {
               wheresql = wheresql + " and skuname like '%" + cp.SkuName + "%'";
            }
            if(!string.IsNullOrEmpty(cp.GoodsCode))//款式编号
            {
               wheresql = wheresql + " and goodscode like '%" + cp.GoodsCode + "%'";
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            wheresql = "select id,purchaseid,skuid,skuname,colorname,sizename,purqty,price,puramt,remark,goodscode,supplynum,recievedate,detailstatus from purchasedetail " + wheresql ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<PurchaseDetailMulti>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<PurchaseDetailMulti>(wheresql).AsList();

                    cp.Datacnt = count;
                    cp.Pagecnt = pagecnt;
                    cp.Com = u;
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
        ///<summary>
        ///采购单删除
        ///</summary>
        public static DataResult DeletePur(List<string> PuridList,int CoID)
        {
            var result = new DataResult(1,null);     
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string delsql = @"delete from purchase where purchaseid in @PurID and coid = @Coid";
                var p = new DynamicParameters();
                p.Add("@PurID", PuridList);
                p.Add("@Coid", CoID);
                int count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count <= 0)
                {
                    result.s = -3004;
                }
                else
                {
                    delsql = @"delete from purchasedetail where purchaseid in @PurID and coid = @Coid";
                    count = CoreDBconn.Execute(delsql, p, TransCore);
                    if (count <= 0)
                    {
                        result.s = -3004;
                    }
                }
                if(result.s == 1)
                {
                    TransCore.Commit();
                }
            }
            catch (Exception e)
            {
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Close();
            }
            return result;
        }
        ///<summary>
        ///查询单笔采购单
        ///</summary>
        public static DataResult GetPurchaseEdit(int id,int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where id =" + id + " and coid =" + CoID ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<Purchase>(wheresql).AsList();
                    if (u.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        result.d = u[0];
                    }               
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
        ///<summary>
        ///采购单保存
        ///</summary>
        public static DataResult SavePurchase(string modifyFlag,Purchase pur,string UserName,int CoID)
        {
            var result = new DataResult(1,null);        
            if (modifyFlag == "new")
            {
                var iresult = InsertPurchase(pur,UserName,CoID);
                result.s = iresult.s;
                result.d = iresult.d;
            }
            else
            {
                var mresult = UpdatePurchase(pur);
                result.s = mresult.s;
                result.d = mresult.d;
            }
            return result;
        }    
        public static DataResult InsertPurchase(Purchase pur,string UserName,int CoID)
        {
            var result = new DataResult(1,"资料新增成功!");   
            //采购单号产生
            string purid = CoID.ToString();
            int len = purid.Length;
            if (len == 1)
            { purid = purid + "000"; }
            if (len == 2)
            { purid = purid + "00"; }
            if (len == 3)
            { purid =purid + "0"; }
            if (len > 3)
            { purid = purid.Substring(0, 4); }
            purid = purid + DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 4);
            purid = purid + DateTime.Now.ToString("yyyy-MM-dd").Substring(5, 2);
            Random Rd = new Random();
            purid = purid + Rd.Next(1000, 10000).ToString();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlCommandText = @"INSERT INTO purchase(purchaseid,purchasedate,coname,contract,shpaddress,purtype,buyyer,remark,taxrate,coid,creator) VALUES(
                            @PurID,
                            @Purdate,
                            @CoName,
                            @Contract,
                            @Shpaddress,
                            @Purtype,
                            @Buyyer,
                            @Remark,
                            @Taxrate,
                            @Coid,
                            @UName
                        )";
                    var args = new {PurID = purid,Purdate=pur.purchasedate,CoName = pur.coname,Contract = pur.contract,Shpaddress = pur.shpaddress,
                                    Purtype = pur.purtype,Buyyer = pur.buyyer,Remark = pur.remark,Taxrate = pur.taxrate,Coid = CoID,UName = UserName};
                    int count =conn.Execute(sqlCommandText,args);
                    if(count <= 0)
                    {
                        result.s = -3002;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return  result;
        }
        public static DataResult UpdatePurchase(Purchase pur)
        {
            var result = new DataResult(1,"资料更新成功!");  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{               
                    string uptsql = @"update purchase set purchasedate = @Purdate,coname = @CoName,contract = @Contract,shpaddress=@Shpaddress,purtype=@Purtype,buyyer=@Buyyer,
                                      remark=@Remark,taxrate=@Taxrate,puramttot = puramtnet*(1+@Taxrate) where id = @ID";
                    var args = new {Purdate=pur.purchasedate,CoName = pur.coname,Contract = pur.contract,Shpaddress = pur.shpaddress,
                                    Purtype = pur.purtype,Buyyer = pur.buyyer,Remark = pur.remark,Taxrate = pur.taxrate,ID = pur.id};
                    int count = conn.Execute(uptsql,args);
                    if(count<=0)
                    {
                        result.s= -3003;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return  result;
        }  
    }
}
            
                