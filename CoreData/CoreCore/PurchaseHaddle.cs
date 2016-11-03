using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
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
            string sqlcount =  "select count(id) from purchase where purchasedate between '" + cp.PurdateStart + "' and '" + cp.PurdateEnd + "'"; //采购日期;
            string sqlCommand = "select * from purchase where purchasedate between '" + cp.PurdateStart + "' and '" + cp.PurdateEnd + "'"; //采购日期;
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if (cp.Purid >= 0)//采购单号
            {
                wheresql = wheresql + " AND id = "+ cp.Purid ;
            }
            if(cp.Status >= 0)//状态
            {
               wheresql = wheresql + " and status = " + cp.Status;
            }
            if(cp.Scoid > 0)//供应商
            {
               wheresql = wheresql + " and scoid = " + cp.Scoid;
            }
            if (cp.Warehouseid > 0)//仓库代号
            {
                wheresql = wheresql + " AND warehouseid = "+ cp.Warehouseid ;
            }
            if(!string.IsNullOrEmpty(cp.Buyyer))//采购员
            {
               wheresql = wheresql + " and buyyer = '" + cp.Buyyer + "'";
            }
            if(!string.IsNullOrEmpty(cp.Skuid))//商品编码
            {
               wheresql = wheresql + " and exists(select skuid from purchasedetail where purchaseid = purchase.id and skuid = '" + cp.Skuid + "')";
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new PurchaseData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<Purchase>(sqlCommand + wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Pur = u;
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
        ///查询采购单明细List
        ///</summary>
        public static DataResult GetPurchaseDetailList(PurchaseDetailParm cp)
        {
            var result = new DataResult(1,null);   
            string sqlcount =  "select count(id) from purchasedetail where purchaseid = " + cp.Purid; //采购单号
            string sqlCommand = "select * from purchasedetail where purchaseid = " + cp.Purid; //采购单号
            string wheresql = string.Empty;
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
            var res = new PurchaseDetailData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    string pursql = "select * from purchase where id = " + cp.Purid + " and coid =" + cp.CoID;
                    var pur = conn.Query<Purchase>(pursql).AsList();
                    if (pur.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                        return result;
                    }
                    else
                    {
                        res.status = pur[0].status;
                    }           
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<PurchaseDetail>(sqlCommand + wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Pur = u;
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
        ///采购单作废
        ///</summary>
        public static DataResult CanclePurchase(List<int> PuridList,int CoID,string UserName)
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
                string wheresql = @"select count(id) from purchase where id in @PurID and coid = @Coid and status <> 0" ;
                int u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "未审核状态的采购单才可作废!";
                    return result;
                }
                p.Add("@Modifier", UserName);
                p.Add("@Modifydate", DateTime.Now);
                string delsql = @"update purchase set status = 5,modifier = @Modifier,modifydate = @Modifydate where id in @PurID and coid = @Coid";
                int count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                delsql = @"update purchasedetail set detailstatus = 5,modifier = @Modifier,modifydate = @Modifydate where purchaseid in @PurID and coid = @Coid";
                count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///查询单笔采购单
        ///</summary>
        public static DataResult GetPurchaseEdit(int id,int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select * from purchase where id =" + id + " and coid =" + CoID ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<Purchase>(wheresql).AsList();
                    if (u.Count > 0)
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
        ///采购单新增
        ///</summary>
        public static DataResult InsertPurchase(Purchase pur,string UserName,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    if ( pur.warehouseid == 0)
                    {
                        pur.warehousename = "";
                    }
                    else
                    {
                        string wheresql = "select ID,WareName,Enable from ware_third_party where id =" + pur.warehouseid + " and coid =" + CoID;
                        var u = DbBase.CommDB.Query<wareThirdParty>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "仓库编号不存在!!";
                            return result;
                        }
                        else
                        {
                            if(u[0].enable != 2)
                            {
                                result.s = -1;
                                result.d = "仓库编号不是生效状态!!";
                                return result;
                            }
                            else
                            {
                                pur.warehousename = u[0].warename;
                            }
                        }
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    //检查必输栏位是否都有值
                    if ( pur.purchasedate == null)
                    {
                        result.s = -1;
                        result.d = "采购日期必须有值!";
                        return result;
                    }
                    if ( pur.scoid == 0)
                    {
                        result.s = -1;
                        result.d = "供应商编号必须有值!";
                        return result;
                    }
                    else
                    {
                        string wheresql = "select * from supplycompany where id =" + pur.scoid + " and coid =" + CoID;
                        var u = conn.Query<ScoCompany>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "供应商编号不存在!!";
                            return result;
                        }
                        else
                        {
                            if(u[0].enable == false)
                            {
                                result.s = -1;
                                result.d = "供应商编号已停用!!";
                                return result;
                            }
                            else
                            {
                                pur.sconame = u[0].sconame;
                            }
                        }
                        if(result.s == -1)
                        {
                            return result;
                        }
                    }
                    if ( pur.shplogistics == null || pur.shpcity == null || pur.shpdistrict == null || pur.shpaddress == null)
                    {
                        result.s = -1;
                        result.d = "采购地址必须有值!";
                        return result;
                    }
                    if ( pur.purtype == null)
                    {
                        result.s = -1;
                        result.d = "商品类型必须有值!";
                        return result;
                    }
                    if ( pur.taxrate == null)
                    {
                        pur.taxrate = "0";
                    }
                    string sqlCommandText = @"INSERT INTO purchase(purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,purtype,buyyer,remark,warehouseid,warehousename,taxrate,coid,creator,Modifier) VALUES(
                            @Purdate,@Scoid,@Sconame,@Contract,@Shplogistics,@Shpcity,@Shpdistrict,@Shpaddress,@Purtype,@Buyyer,@Remark,@Warehouseid,@Warehousename,@Taxrate,@Coid,@UName,@UName)";
                    var args = new {Purdate=pur.purchasedate,Scoid = pur.scoid,Sconame = pur.sconame,Contract = pur.contract,Shplogistics = pur.shplogistics,Shpcity = pur.shpcity,Shpdistrict = pur.shpdistrict,
                                    Shpaddress = pur.shpaddress,Purtype = pur.purtype,Buyyer = UserName,Remark = pur.remark,Warehouseid = pur.warehouseid,Warehousename = pur.warehousename,
                                    Taxrate = pur.taxrate,Coid = CoID,UName = UserName};
                    int count =conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    else
                    {
                        int rtn = conn.QueryFirst<int>("select LAST_INSERT_ID()");
                        result.d = rtn;
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
        ///采购单更新
        ///</summary>
        public static DataResult UpdatePurchase(Purchase pur,int CoID,string UserName)
        {
            var result = new DataResult(1,null);  
            var purOlder = new Purchase();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{      
                    string wheresql = "select * from purchase where id =" + pur.id + " and coid = " + CoID;
                    var u = conn.Query<Purchase>(wheresql).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "该采购单不存在!!";
                        return result;
                    }
                    else
                    {
                        if(u[0].status != 0)
                        {
                            result.s = -1;
                            result.d = "该采购单不允许修改!!";
                            return result;
                        }
                        purOlder = u[0];
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            var p = new DynamicParameters();      
            string uptsql = @"update purchase set ";
            if(purOlder.warehouseid != pur.warehouseid)
            {
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try{   
                        if(pur.warehouseid == 0)
                        {
                            pur.warehousename = "";
                        }
                        else
                        {
                            string wheresql = "select ID,WareName,Enable from ware_third_party where id =" + pur.warehouseid + " and coid =" + CoID;
                            var u = DbBase.CommDB.Query<wareThirdParty>(wheresql).AsList();
                            if(u.Count == 0)
                            {
                                result.s = -1;
                                result.d = "仓库编号不存在!!";
                                return result;
                            }
                            else
                            {
                                if(u[0].enable != 2)
                                {
                                    result.s = -1;
                                    result.d = "仓库编号不是生效状态!!";
                                    return result;
                                }
                                else
                                {
                                    pur.warehousename = u[0].warename;
                                }
                            }
                        }
                        uptsql = uptsql + "warehouseid = @Warehouseid,warehousename = @Warehousename,";
                        p.Add("@Warehouseid", pur.warehouseid);
                        p.Add("@Warehousename", pur.warehousename);
                    }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                } 
            }
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{      
                    if ( pur.purchasedate != null && pur.purchasedate != purOlder.purchasedate)
                    {
                        uptsql = uptsql + "purchasedate = @Purdate,";
                        p.Add("@Purdate", pur.purchasedate);
                    }
                    if ( pur.scoid != 0 && pur.scoid != purOlder.scoid)
                    {
                        string wheresql = "select * from supplycompany where id =" + pur.scoid + " and coid = " + CoID;
                        var u = conn.Query<ScoCompany>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "供应商编号不存在!!";
                            return result;
                        }
                        else
                        {
                            if(u[0].enable == false)
                            {
                                result.s = -1;
                                result.d = "供应商编号已停用!!";
                                return result;
                            }
                            else
                            {
                                uptsql = uptsql + "scoid = @Scoid,sconame = @Sconame,";
                                p.Add("@Scoid", pur.scoid);
                                p.Add("@Sconame", u[0].sconame );
                            }
                        }
                    }
                    if ( pur.remark != null && pur.remark != purOlder.remark)
                    {
                        uptsql = uptsql + "remark=@Remark,";
                        p.Add("@Remark", pur.remark);
                    }
                    if ( pur.shplogistics != null && pur.shplogistics != purOlder.shplogistics)
                    {
                        uptsql = uptsql + "shplogistics = @Shplogistics,";
                        p.Add("@Shplogistics", pur.shplogistics);
                    }
                    if ( pur.shpcity != null && pur.shpcity != purOlder.shpcity)
                    {
                        uptsql = uptsql + "shpcity = @Shpcity,";
                        p.Add("@Shpcity", pur.shpcity);
                    }
                    if ( pur.shpdistrict != null && pur.shpdistrict != purOlder.shpdistrict)
                    {
                        uptsql = uptsql + "shpdistrict = @Shpdistrict,";
                        p.Add("@Shpdistrict", pur.shpdistrict);
                    }
                    if ( pur.shpaddress != null && pur.shpaddress != purOlder.shpaddress)
                    {
                        uptsql = uptsql + "shpaddress=@Shpaddress,";
                        p.Add("@Shpaddress", pur.shpaddress);
                    }
                    if ( pur.contract != null && pur.contract != purOlder.contract)
                    {
                        uptsql = uptsql + "contract = @Contract,";
                        p.Add("@Contract", pur.contract);
                    }
                    if ( pur.purtype != null && pur.purtype != purOlder.purtype)
                    {
                        uptsql = uptsql + "purtype=@Purtype,";
                        p.Add("@Purtype", pur.purtype);
                    }
                    if ( pur.buyyer != null && pur.buyyer != purOlder.buyyer)
                    {
                        uptsql = uptsql + "buyyer=@Buyyer,";
                        p.Add("@Buyyer", pur.buyyer);
                    }
                    if ( pur.taxrate != null && pur.taxrate != purOlder.taxrate)
                    {
                        uptsql = uptsql + "taxrate=@Taxrate,";
                        p.Add("@Taxrate", pur.taxrate);
                    }
                    if(uptsql.Substring(uptsql.Length - 1, 1) == ",")
                    {
                        uptsql = uptsql + "modifier = @Modifier,modifydate = @Modifydate where id = @ID";
                        p.Add("@Modifier", UserName);
                        p.Add("@Modifydate", DateTime.Now);
                        p.Add("@ID", pur.id);
                    }
                    else
                    {
                        result.s = 1;
                        result.d = null;
                        return result;
                    }
                    int count = conn.Execute(uptsql,p);
                    if(count < 0)
                    {
                        result.s= -3003;
                        return result;
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
        public static DataResult ConfirmPurchase(List<int> PuridList,int CoID,string UserName)
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
                string wheresql = @"select count(id) from purchase where id in @PurID and coid = @Coid and status <> 0" ;
                int u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "未审核状态的采购单才可执行审核操作!";
                    return result;
                }
                p.Add("@Modifier", UserName);
                p.Add("@Modifydate", DateTime.Now);
                string delsql = @"update purchase set status = 1,modifier = @Modifier,modifydate = @Modifydate where id in @PurID and coid = @Coid";
                int count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                else
                delsql = @"update purchasedetail set detailstatus = 1,modifier = @Modifier,modifydate = @Modifydate where purchaseid in @PurID and coid = @Coid";
                count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///采购单完成
        ///</summary>
        public static DataResult CompletePurchase(List<int> PuridList,int CoID,string UserName)
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
                string wheresql = @"select count(id) from purchase where id in @PurID and coid = @Coid and status not in (1,3,4)" ;
                var u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "已确认&&待发货&&待收货状态的采购单才可执行此操作!";
                    return result;
                }
                p.Add("@Modifier", UserName);
                p.Add("@Modifydate", DateTime.Now);
                string delsql = @"update purchase set status = 2,modifier = @Modifier,modifydate = @Modifydate where id in @PurID and coid = @Coid";
                int count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                delsql = @"update purchasedetail set detailstatus = 2,modifier = @Modifier,modifydate = @Modifydate where purchaseid in @PurID and coid = @Coid";
                count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                TransCore.Commit();
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
        public static DataResult InsertPurDetail(int id,List<int> skuid,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            var res = new PurchaseDetailInsert();
            var cc = new InsertFailReason();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from purchase where id =" + id + " and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此采购单不存在!";
                    return result;
                }
                else
                {
                    if(u[0].status != 0)
                    {
                        result.s = -1;
                        result.d = "只有待审核的采购单才可以新增明细!";
                        return result;
                    }
                }
                List<InsertFailReason> rt = new List<InsertFailReason>();
                List<int> rr = new List<int>();
                foreach(int a in skuid)
                {
                    InsertFailReason rf = new InsertFailReason();
                    string skusql = "select skuid,skuname,norm,img,goodscode,enable from coresku where id =" + a + " and coid =" + CoID;
                    var s = CoreDBconn.Query<SkuInsert>(skusql).AsList();
                    if(s.Count == 0)
                    {
                        rf.id = a;
                        rf.reason = "此商品不存在!";
                        rt.Add(rf);
                        continue;
                    }
                    else
                    {
                        if(s[0].enable == false)
                        {
                            rf.id = a;
                            rf.reason = "此商品已停用!";
                            rt.Add(rf);
                            continue;
                        }
                    }
                    int x = CoreDBconn.QueryFirst<int>("select count(id) from purchasedetail where purchaseid = "+ id+" and coid =" + CoID + " and skuautoid = "+a);
                    if( x > 0)
                    {
                        rf.id = a;
                        rf.reason = null;
                        rt.Add(rf);
                        continue;
                    }
                    string sqlCommandText = @"INSERT INTO purchasedetail(purchaseid,skuautoid,skuid,skuname,norm,img,goodscode,coid,creator,modifier) 
                                            VALUES(@PurID,@Skuautoid,@Skuid,@Skuname,@Norm,@Img,@Goodscode,@Coid,@Creator,@Creator)";
                    var args = new {PurID = id,Skuautoid = a,Skuid=s[0].skuid,Skuname = s[0].skuname,Norm = s[0].norm ,Img = s[0].img,Goodscode = s[0].goodscode ,Coid = CoID,Creator = Username};
                    int count = CoreDBconn.Execute(sqlCommandText,args,TransCore);
                    if(count <= 0)
                    {
                        rf.id = a;
                        rf.reason = "新增明细失败!";
                        rt.Add(rf);
                    }
                    else
                    {
                        rr.Add(a);
                    }
                }
                res.successIDs = rr;
                res.failIDs = rt;
                result.d = res;
                if(result.s == 1)
                {
                    TransCore.Commit();
                }
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///采购单明细更新
        ///</summary>
        public static DataResult UpdatePurDetail(PurchaseDetail detail,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from purchase where id =" + detail.purchaseid + " and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此采购单不存在!";
                    return result;
                }
                else
                {
                    if(u[0].status == 2 || u[0].status == 5)
                    {
                        result.s = -1;
                        result.d = "已完成或已作废的明细不允许修改!";
                        return result;
                    }
                }
                wheresql = "select * from purchasedetail where purchaseid = " + detail.purchaseid + " and skuautoid =" + detail.id + " and coid =" + CoID;
                var pur = CoreDBconn.Query<PurchaseDetail>(wheresql).AsList();
                if (pur.Count == 0)
                {
                    result.s = -1;
                    result.d = "此采购单明细不存在!";
                    return result;
                }
                decimal qty = decimal.Parse(pur[0].purqty);
                decimal amt = decimal.Parse(pur[0].purqty) * decimal.Parse(pur[0].price);
                decimal qtynew = qty,amtnew = amt;
                var p = new DynamicParameters();
                string sqlCommandText = @"update purchasedetail set ";
                int flag = 0;
                if(detail.price != null && u[0].status == 0)
                {
                    flag ++;
                    sqlCommandText = sqlCommandText + "price = @Price,";
                    p.Add("@Price", detail.price);
                    amtnew = qtynew * decimal.Parse(detail.price);
                }
                if(detail.purqty != null && u[0].status == 0)
                {
                    qtynew = decimal.Parse(detail.purqty);
                    if(flag > 0)
                    {
                        amtnew = qtynew * decimal.Parse(detail.price);
                    }
                    else
                    {
                        amtnew = qtynew * decimal.Parse(pur[0].price);
                    }
                    flag ++;
                    sqlCommandText = sqlCommandText + "purqty = @Purqty,";
                    p.Add("@Purqty", detail.purqty);
                }
                if(flag > 0)
                {
                    sqlCommandText = sqlCommandText + "puramt = purqty * price,";
                }
                if(detail.remark != null)
                {
                    sqlCommandText = sqlCommandText + "remark = @Remark,";
                    p.Add("@Remark", detail.remark);
                }
                if(detail.planqty != null)
                {
                    sqlCommandText = sqlCommandText + "planqty = @Planqty,planamt = planqty * price,";
                    p.Add("@Planqty", detail.planqty);
                }
                if(detail.recievedate != null)
                {
                    sqlCommandText = sqlCommandText + "recievedate = @Recdate,";
                    p.Add("@Recdate", detail.recievedate);
                }
                if(detail.packingnum != null)
                {
                    sqlCommandText = sqlCommandText + "packingnum = @Packingnum,";
                    p.Add("@Packingnum", detail.packingnum);
                }
                if(sqlCommandText.Substring(sqlCommandText.Length - 1, 1) == ",")
                {
                    sqlCommandText = sqlCommandText + "modifier=@Modifier,modifydate = @Modifydate where purchaseid = @Purchaseid and skuautoid = @ID and coid = @Coid";
                    p.Add("@Modifier",Username);
                    p.Add("@Modifydate",DateTime.Now);
                    p.Add("@ID",detail.id);
                    p.Add("@Purchaseid",detail.purchaseid);
                    p.Add("@Coid",CoID);
                }
                else
                {
                    result.s = 1;
                    result.d = null;
                    return result;
                }
                int count = CoreDBconn.Execute(sqlCommandText,p,TransCore);
                if(count <= 0)
                {
                    result.s = -3003;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///采购单明细删除
        ///</summary>
        public static DataResult DeletePurDetail(int id,List<int> detailid,int CoID)
        {
            var result = new DataResult(1,null);    
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from purchase where id =" + id + " and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此采购单不存在!";
                    return result;
                }
                else
                {
                    if(u[0].status != 0)
                    {
                        result.s = -1;
                        result.d = "只有待审核的采购单明细才可删除!";
                        return result;
                    }
                }
                var argss = new {ID = detailid,Purid = id,Coid = CoID};
                string sqlCommandText = @"delete from  purchasedetail where purchaseid = @Purid and skuautoid in @ID and coid = @Coid";
                int count = CoreDBconn.Execute(sqlCommandText,argss,TransCore);
                if(count <= 0)
                {
                    result.s = -3004;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///质检资料查询
        ///</summary>
        public static DataResult QualityRevList(int purid,int CoID,int PageIndex,int NumPerPage)
        {
            var result = new DataResult(1,null);
            string sqlcount = "select count(id) from qualityrev where purchaseid = " + purid + " and coid = " + CoID + " order by id asc";
            string wheresql = "select * from qualityrev where purchaseid = " + purid + " and coid = " + CoID + " order by id asc";
            var res =new QualityRevData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(NumPerPage.ToString()));
                    int dataindex = (PageIndex - 1)* NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + NumPerPage.ToString();
                    var u = conn.Query<QualityRev>(wheresql).AsList();
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Qua = u;
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
        ///质检资料新增
        ///</summary>
        public static DataResult InsertQualityRev(QualityRev qua,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string wheresql = @"select count(id) from purchase where id = " + qua.purchaseid + " and status in (0,1)";
                    int u = conn.QueryFirst<int>(wheresql);
                    if(u == 0)
                    {
                        result.s = -1;
                        result.d = "不允许新增质检资料";
                        return result;
                    }
                    if(qua.recorddate== null)
                    {
                        result.s = -1;
                        result.d = "检验日期必须有值!";
                        return result;
                    }
                    if(qua.recorder== null)
                    {
                        qua.recorder = Username;
                    }
                    if(qua.drawrate== null)
                    {
                        result.s = -1;
                        result.d = "抽检比率必须有值!";
                        return result;
                    }
                    if(qua.type== null)
                    {
                        result.s = -1;
                        result.d = "质检类型必须有值!";
                        return result;
                    }
                    string sqlCommandText = @"INSERT INTO qualityrev(recorddate,recorder,drawrate,type,conclusion,remark,purchaseid,coid,creator,modifier) 
                                                VALUES(@Recorddate,@Recorder,@Drawrate,@Type,@Conclusion,@Remark,@Purchaseid,@Coid,@Creator,@Creator)";
                    var args = new {Recorddate = qua.recorddate,Recorder=qua.recorder,Drawrate = qua.drawrate,Type = qua.type,
                                    Conclusion = qua.conclusion,Remark = qua.remark,Purchaseid = qua.purchaseid ,Coid = CoID,Creator = Username};
                    int count = conn.Execute(sqlCommandText,args);
                    if(count <= 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    else
                    {
                        int rtn = conn.QueryFirst<int>("select LAST_INSERT_ID()");
                        result.d = rtn;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }

        ///<summary>
        ///质检资料更新
        ///</summary>
        public static DataResult UpdateQualityRev(QualityRev qua,string Username)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string wheresql = @"select count(id) from purchase where id = " + qua.purchaseid + " and status in (0,1)";
                    int u = conn.QueryFirst<int>(wheresql);
                    if(u == 0)
                    {
                        result.s = -1;
                        result.d = "不允许更新质检资料";
                        return result;
                    }
                    var p = new DynamicParameters();
                    string sqlCommandText = @"update qualityrev set ";
                    if(qua.recorddate != null)
                    {
                        sqlCommandText = sqlCommandText + "recorddate = @Recorddate,";
                        p.Add("@Recorddate", qua.recorddate);
                    }
                    if(qua.recorder != null)
                    {
                        sqlCommandText = sqlCommandText + "recorder = @Recorder,";
                        p.Add("@Recorder", qua.recorder);
                    }
                    if(qua.drawrate != null)
                    {
                        sqlCommandText = sqlCommandText + "drawrate = @Drawrate,";
                        p.Add("@Drawrate", qua.drawrate);
                    }
                    if(qua.type != null)
                    {
                        sqlCommandText = sqlCommandText + "type = @Type,";
                        p.Add("@Type", qua.type);
                    }
                    if(qua.conclusion != null)
                    {
                        sqlCommandText = sqlCommandText + "conclusion = @Conclusion,";
                        p.Add("@Conclusion", qua.conclusion);
                    }
                    if(qua.remark != null)
                    {
                        sqlCommandText = sqlCommandText + "remark = @Remark,";
                        p.Add("@Remark", qua.remark);
                    }
                    if(sqlCommandText.Substring(sqlCommandText.Length - 1, 1) == ",")
                    {
                        sqlCommandText = sqlCommandText + "modifier = @Modifier,modifydate=@Modifydate where id = @ID";
                        p.Add("@Modifier",Username);
                        p.Add("@Modifydate",DateTime.Now);
                        p.Add("@ID",qua.id);
                    }
                    else
                    {
                        result.s = 1;
                        result.d = null;
                        return result;
                    }
                    int count = conn.Execute(sqlCommandText,p);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }        
        ///<summary>
        ///质检资料删除
        ///</summary>
        public static DataResult DeleteQualityRev(List<int> id)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    int u = conn.QueryFirst<int>("select count(id) from qualityrev where id in @ID and status <> 0",new {ID = id});
                    if (u > 0)
                    {
                        result.s = -1;
                        result.d = "已确认的资料不可删除";
                        return result;
                    }
                    string sqlCommandText = @"delete from qualityrev where id in @ID";
                    var args = new {ID = id};
                    int count = conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3004;
                        return result;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }

        ///<summary>
        ///质检资料确认
        ///</summary>
        public static DataResult ConfirmQualityRev(int id,string Username)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    int u = conn.QueryFirst<int>("select count(id) from qualityrev where id = @ID and status <> 0",new {ID = id});
                    if (u > 0)
                    {
                        result.s = -1;
                        result.d = "待审核的资料才可执行此操作";
                        return result;
                    }
                    string sqlCommandText = @"update qualityrev set status = 1,modifier=@Modifier,modifydate=@Modifydate where id = @ID";
                    var args = new {Modifier = Username,Modifydate = DateTime.Now,ID = id};
                    int count = conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                }
            }
            return result;
        }

        ///<summary>
        ///采购单初始资料
        ///</summary>
        public static DataResult GetPurchaseInit(int CoID)
        {
            var result = new DataResult(1,null);   
            var res = new PurchaseInitData();  
            //采购单状态
            Dictionary<int,string> status = new Dictionary<int,string>();
            status.Add(0,"待审核");
            status.Add(1,"已确认");
            status.Add(2,"已完成");
            status.Add(3,"待发货");
            status.Add(4,"待收货");
            status.Add(5,"作废");
            res.status = status;
            //仓库列表
            var wh = CoreComm.WarehouseHaddle.getWarelist(CoID.ToString());
            res.warehouse = wh as List<wareLst>;//Newtonsoft.Json.JsonConvert.DeserializeObject<>(wh.d.ToString());
            //采购单基本资料
            string sqlcount = "select count(id) from purchase where 1 = 1"; 
            string sqlCommand = "select * from purchase where 1 = 1"; 
            string wheresql = string.Empty;
            if(CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + CoID;
            }
            wheresql = wheresql + " order by id desc";
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/20);

                    wheresql = wheresql + " limit 0,20";
                    var u = conn.Query<Purchase>(sqlCommand + wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Pur = u;
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
        ///采购单更新备注
        ///</summary>
        public static DataResult UpdatePurRemark(int id,string remark,string Username)
        {
            var result = new DataResult(1,null);  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{      
                    int cnt = conn.QueryFirst<int>("select count(id) from purchase where id = @ID and status in (2,5)",new {ID = id});
                    if(cnt > 0)
                    {
                        result.s = -1;
                        result.d = "已完成&已作废采购单不可以更新备注";
                        return  result;
                    }
                    string uptsql = @"update purchase set remark=@Remark,modifier=@Modifier,modifydate=@Modifydate where id = @ID";
                    var args = new {Remark = remark,ID = id,Modifier=Username,Modifydate=DateTime.Now};
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                        return  result;
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
        ///采购单还原
        ///</summary>
        public static DataResult RestorePur(List<int> PuridList,int CoID,string Username)
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
                string wheresql = @"select count(id) from purchase where id in @PurID and coid = @Coid and status <> 1" ;
                int u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "已确认状态的采购单才可执行还原操作!";
                    return result;
                }
                p.Add("@Modifier", Username);
                p.Add("@Modifydate", DateTime.Now);
                string delsql = @"update purchase set status = 0,modifier = @Modifier,modifydate = @Modifydate where id in @PurID and coid = @Coid";
                int count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                delsql = @"update purchasedetail set detailstatus = 0,modifier = @Modifier,modifydate = @Modifydate where purchaseid in @PurID and coid = @Coid";
                count = CoreDBconn.Execute(delsql, p, TransCore);
                if (count < 0)
                {
                    result.s = -3003;
                    return result;
                }
                TransCore.Commit();
            }
            catch (Exception e)
            {
                TransCore.Rollback();
                TransCore.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransCore.Dispose();
                CoreDBconn.Dispose();
            }
            return result;
        }
    }
}
            
                