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
            wheresql = "select id,purchaseid,skuid,skuname,colorname,sizename,purqty,price,puramt,remark,goodscode,supplynum,recievedate,detailstatus,norm,coid from purchasedetail " + wheresql ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    string pursql = "select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where purchaseid = '" + cp.Purid + "' and coid =" + cp.CoID ;
                    var pur = conn.Query<Purchase>(pursql).AsList();
                    if (pur.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        if(pur[0].status == 0)
                        {
                            cp.enable = true;
                        }
                        else
                        {
                            cp.enable = false;
                        }
                    }           
                    var u = conn.Query<PurchaseDetail>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<PurchaseDetail>(wheresql).AsList();

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
                var p = new DynamicParameters();
                p.Add("@PurID", PuridList);
                p.Add("@Coid", CoID);
                string wheresql = @"select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where purchaseid in @PurID and coid = @Coid and status <> 0" ;
                var u = CoreDBconn.Query<Purchase>(wheresql,p).AsList();
                if(u.Count > 0)
                {
                    result.s = -1;
                    result.d = "未审核状态的采购单才可删除!";
                }
                else
                {
                    string delsql = @"delete from purchase where purchaseid in @PurID and coid = @Coid";
                    int count = CoreDBconn.Execute(delsql, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3004;
                    }
                    else
                    {
                        delsql = @"delete from purchasedetail where purchaseid in @PurID and coid = @Coid";
                        count = CoreDBconn.Execute(delsql, p, TransCore);
                        if (count < 0)
                        {
                            result.s = -3004;
                        }
                    }
                    if(result.s == 1)
                    {
                        TransCore.Commit();
                    }
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
                    if(count < 0)
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
                    if(count < 0)
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
        ///<summary>
        ///采购单审核
        ///</summary>
        public static DataResult ConfirmPurchase(List<string> PuridList,int CoID)
        {
            var result = new DataResult(1,"采购单审核成功!");  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                var p = new DynamicParameters();
                p.Add("@PurID", PuridList);
                p.Add("@Coid", CoID);
                string wheresql = @"select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where purchaseid in @PurID and coid = @Coid and status <> 0" ;
                var u = CoreDBconn.Query<Purchase>(wheresql,p).AsList();
                if(u.Count > 0)
                {
                    result.s = -1;
                    result.d = "未审核状态的采购单才可执行审核操作!";
                }
                else
                {
                    string delsql = @"update purchase set status = 1 where purchaseid in @PurID and coid = @Coid";
                    int count = CoreDBconn.Execute(delsql, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        delsql = @"update purchasedetail set detailstatus = 1 where purchaseid in @PurID and coid = @Coid";
                        count = CoreDBconn.Execute(delsql, p, TransCore);
                        if (count < 0)
                        {
                            result.s = -3003;
                        }
                    }
                    if(result.s == 1)
                    {
                        TransCore.Commit();
                    }
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
        ///采购单审核
        ///</summary>
        public static DataResult ForcePurchase(List<string> PuridList,int CoID)
        {
            var result = new DataResult(1,"采购单强制完成成功!");  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                var p = new DynamicParameters();
                p.Add("@PurID", PuridList);
                p.Add("@Coid", CoID);
                string wheresql = @"select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where purchaseid in @PurID and coid = @Coid and status not in (1,2)" ;
                var u = CoreDBconn.Query<Purchase>(wheresql,p).AsList();
                if(u.Count > 0)
                {
                    result.s = -1;
                    result.d = "审核通过&&部分完成状态的采购单才可执行此操作!";
                }
                else
                {
                    string delsql = @"update purchase set status = 4 where purchaseid in @PurID and coid = @Coid";
                    int count = CoreDBconn.Execute(delsql, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        delsql = @"update purchasedetail set detailstatus = 4 where purchaseid in @PurID and coid = @Coid";
                        count = CoreDBconn.Execute(delsql, p, TransCore);
                        if (count < 0)
                        {
                            result.s = -3003;
                        }
                    }
                    if(result.s == 1)
                    {
                        TransCore.Commit();
                    }
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
        ///采购单明细新增
        ///</summary>
        public static DataResult InsertPurDetail(PurchaseDetail detail,int CoID)
        {
            var result = new DataResult(1,"采购单明细新增成功!");  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlCommandText = @"INSERT INTO purchasedetail(purchaseid,skuid,skuname,price,purqty,puramt,recievedate,remark,colorname,sizename,norm,goodscode,supplynum,coid) 
                                            VALUES(@PurID,@Skuid,@Skuname,@Price,@Purqty,@Puramt,@Recdate,@Remark,@Colorname,@Sizename,@Norm,@Goodscode,@Supplynum,@Coid)";
                var args = new {PurID = detail.purchaseid,Skuid=detail.skuid,Skuname = detail.skuname,Price = detail.price,Purqty = detail.purqty,Puramt = detail.puramt,
                                Recdate = detail.recievedate,Remark = detail.remark,Colorname = detail.colorname,Sizename = detail.sizename ,Norm = detail.norm ,
                                Goodscode = detail.goodscode ,Supplynum = detail.supplynum,Coid = CoID};
                int count = CoreDBconn.Execute(sqlCommandText,args,TransCore);
                if(count <= 0)
                {
                    result.s = -3002;
                }
                string wheresql = "select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where purchaseid = '" + detail.purchaseid + "' and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if (u.Count == 0)
                {
                    result.s = -3001;
                }
                else
                {
                    string uptsql = @"update purchase set purqtytot = purqtytot + @Purqty,puramtnet = puramtnet + @Puramt,puramttot = puramtnet*(1 + taxrate) where purchaseid = @PurID and coid = @Coid";
                    var upargs = new {Purqty = detail.purqty,Puramt = detail.puramt,PurID = detail.purchaseid,Coid = CoID};
                    count = CoreDBconn.Execute(uptsql,upargs);
                    if(count < 0)
                    {
                        result.s= -3003;
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
        ///采购单明细新增
        ///</summary>
        public static DataResult UpdatePurDetail(PurchaseDetail detail,int CoID)
        {
            var result = new DataResult(1,"采购单明细更新成功!");  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select id,purchaseid,skuid,skuname,colorname,sizename,purqty,price,puramt,remark,goodscode,supplynum,recievedate,detailstatus,norm,coid from purchasedetail where id = " + detail.id;
                var pur = CoreDBconn.Query<PurchaseDetail>(wheresql).AsList();
                if (pur.Count == 0)
                {
                    result.s = -3001;
                }
                else
                {
                    decimal qty = pur[0].purqty;
                    decimal amt = pur[0].puramt;
                    string sqlCommandText = @"update purchasedetail set skuid = @Skuid,skuname = @Skuname,price = @Price,purqty = @Purqty,puramt = @Puramt,recievedate = @Recdate,
                                            remark = @Remark,colorname = @Colorname,sizename = @Sizename,norm = @Norm,goodscode = @Goodscode,supplynum = @Supplynum where id = @ID ";
                    var args = new {Skuid=detail.skuid,Skuname = detail.skuname,Price = detail.price,Purqty = detail.purqty,Puramt = detail.puramt,
                                    Recdate = detail.recievedate,Remark = detail.remark,Colorname = detail.colorname,Sizename = detail.sizename ,Norm = detail.norm ,
                                    Goodscode = detail.goodscode ,Supplynum = detail.supplynum,ID = detail.id};
                    int count = CoreDBconn.Execute(sqlCommandText,args,TransCore);
                    if(count <= 0)
                    {
                        result.s = -3003;
                    }
                    wheresql = "select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where purchaseid = '" + detail.purchaseid + "' and coid =" + CoID ;
                    var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                    if (u.Count == 0)
                    {
                        result.s = -3001;
                    }
                    else
                    {
                        string uptsql = @"update purchase set purqtytot = purqtytot - @Qty + @Purqty,puramtnet = puramtnet - @Amt + @Puramt,puramttot = puramtnet*(1 + taxrate) where purchaseid = @PurID and coid = @Coid";
                        var upargs = new {Purqty = detail.purqty,Qty = qty,Puramt = detail.puramt,Amt = amt,PurID = detail.purchaseid,Coid = CoID};
                        count = CoreDBconn.Execute(uptsql,upargs);
                        if(count < 0)
                        {
                            result.s= -3003;
                        }
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
        ///采购单明细删除
        ///</summary>
        public static DataResult DeletePurDetail(int id,List<int> detailid,int CoID)
        {
            var result = new DataResult(1,"采购单明细删除成功!");    
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select sum(purqty) as purqty,sum(puramt) as puramt from purchasedetail where id in @ID";
                var argss = new {ID = detailid};
                var pur = CoreDBconn.Query<CalPurchase>(wheresql,argss).AsList();
                if (pur.Count == 0)
                {
                    result.s = -3001;
                }
                else
                {
                    decimal qty = pur[0].purqty;
                    decimal amt = pur[0].puramt;
                    string sqlCommandText = @"delete from  purchasedetail where id = @ID ";
                    int count = CoreDBconn.Execute(sqlCommandText,argss,TransCore);
                    if(count <= 0)
                    {
                        result.s = -3004;
                    }
                    wheresql = "select id,purchaseid,purchasedate,coname,contract,shpaddress,status,purtype,buyyer,remark,taxrate from purchase where id = '" + id + "' and coid =" + CoID ;
                    var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                    if (u.Count == 0)
                    {
                        result.s = -3001;
                    }
                    else
                    {
                        string uptsql = @"update purchase set purqtytot = purqtytot - @Qty ,puramtnet = puramtnet - @Amt ,puramttot = puramtnet*(1 + taxrate) where id = @ID and coid = @Coid";
                        var upargs = new {Qty = qty,Amt = amt,ID = id,Coid = CoID};
                        count = CoreDBconn.Execute(uptsql,upargs);
                        if(count < 0)
                        {
                            result.s= -3003;
                        }
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
    }
}
            
                