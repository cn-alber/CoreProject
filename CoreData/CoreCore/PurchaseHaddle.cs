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
            string wheresql = "select id,purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,warehouseid,warehousename,status,purtype,buyyer,remark,taxrate " + 
                              "from purchase where purchasedate between '" + cp.PurdateStart + "' and '" + cp.PurdateEnd + "'"; //采购日期;
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
                    var u = conn.Query<Purchase>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<Purchase>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Pur = u;
                    // if (count == 0)
                    // {
                    //     result.s = -3001;
                    //     result.d = null;
                    // }
                    // else
                    // {
                        result.d = res;
                    // }               
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
            string wheresql = "select id,purchaseid,img,skuid,skuname,purqty,suggestpurqty,recqty,price,puramt,remark,goodscode,supplynum,supplycode,planqty,planamt,recievedate,norm,packingnum " + 
                              "from purchasedetail where purchaseid = " + cp.Purid; //采购单号
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
                    string pursql = "select id,purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,warehouseid,warehousename,status,purtype,buyyer,remark,taxrate from purchase where id = " + cp.Purid;
                    var pur = conn.Query<Purchase>(pursql).AsList();
                    if (pur.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        res.status = pur[0].status;
                        // if(pur[0].status == 0)
                        // {
                        //     res.enable = true;
                        // }
                        // else
                        // {
                        //     res.enable = false;
                        // }
                    }           
                    var u = conn.Query<PurchaseDetail>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<PurchaseDetail>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Pur = u;
                    // if (count == 0)
                    // {
                    //     result.s = -3001;
                    //     result.d = null;
                    // }
                    // else
                    // {
                        result.d = res;
                    // }               
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
        public static DataResult CanclePurchase(List<int> PuridList,int CoID)
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
                string wheresql = @"select count(*) from purchase where id in @PurID and coid = @Coid and status <> 0" ;
                int u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "未审核状态的采购单才可作废!";
                }
                else
                {
                    string delsql = @"update purchase set status = 5 where id in @PurID and coid = @Coid";
                    int count = CoreDBconn.Execute(delsql, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        delsql = @"update purchasedetail set detailstatus = 5 where purchaseid in @PurID and coid = @Coid";
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
        ///查询单笔采购单
        ///</summary>
        public static DataResult GetPurchaseEdit(int id,int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,warehouseid,warehousename,status,purtype,buyyer,remark,taxrate from purchase where id =" + id + " and coid =" + CoID ;
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
        ///采购单新增
        ///</summary>
        public static DataResult InsertPurchase(Purchase pur,string UserName,int CoID)
        {
            var result = new DataResult(1,null);   
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
                        string wheresql = "select id,sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark from supplycompany where id =" + pur.scoid;
                        var u = conn.Query<ScoCompanySingle>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "供应商编号不存在!!";
                        }
                        else
                        {
                            if(u[0].enable == false)
                            {
                                result.s = -1;
                                result.d = "供应商编号已停用!!";
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
                    if ( pur.purtype == 0)
                    {
                        result.s = -1;
                        result.d = "商品类型必须有值!";
                        return result;
                    }
                    if ( pur.warehouseid == 0)
                    {
                        result.s = -1;
                        result.d = "仓库编号必须有值!";
                        return result;
                    }
                    else
                    {
                        string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where warehouseid =" + pur.warehouseid + " and coid =" + CoID;
                        var u = DbBase.CommDB.Query<Warehouse>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "仓库编号不存在!!";
                        }
                        else
                        {
                            if(u[0].enable == false)
                            {
                                result.s = -1;
                                result.d = "仓库编号已停用!!";
                            }
                            else
                            {
                                if(u[0].parentid > 0)
                                {
                                    result.s = -1;
                                    result.d = "请选择主仓库!";
                                }
                                else
                                {
                                    pur.warehousename = u[0].warehousename;
                                }
                            }
                        }
                        if(result.s == -1)
                        {
                            return result;
                        }
                    }
                    if ( pur.taxrate == null)
                    {
                        pur.taxrate = "0";
                    }
                    string sqlCommandText = @"INSERT INTO purchase(purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,purtype,buyyer,remark,warehouseid,warehousename,taxrate,coid,creator) VALUES(
                            @Purdate,@Scoid,@Sconame,@Contract,@Shplogistics,@Shpcity,@Shpdistrict,@Shpaddress,@Purtype,@Buyyer,@Remark,@Warehouseid,@Warehousename,@Taxrate,@Coid,@UName)";
                    var args = new {Purdate=pur.purchasedate,Scoid = pur.scoid,Sconame = pur.sconame,Contract = pur.contract,Shplogistics = pur.shplogistics,Shpcity = pur.shpcity,Shpdistrict = pur.shpdistrict,
                                    Shpaddress = pur.shpaddress,Purtype = pur.purtype,Buyyer = UserName,Remark = pur.remark,Warehouseid = pur.warehouseid,Warehousename = pur.warehousename,
                                    Taxrate = pur.taxrate,Coid = CoID,UName = UserName};
                    int count =conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3002;
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
        public static DataResult UpdatePurchase(Purchase pur,int CoID)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{      
                    var p = new DynamicParameters();      
                    string uptsql = @"update purchase set ";
                    if ( pur.purchasedate != null)
                    {
                        uptsql = uptsql + "purchasedate = @Purdate,";
                        p.Add("@Purdate", pur.purchasedate);
                    }
                    if ( pur.scoid != 0)
                    {
                        string wheresql = "select id,sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark from supplycompany where id =" + pur.scoid;
                        var u = conn.Query<ScoCompanySingle>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "供应商编号不存在!!";
                        }
                        else
                        {
                            if(u[0].enable == false)
                            {
                                result.s = -1;
                                result.d = "供应商编号已停用!!";
                            }
                            else
                            {
                                uptsql = uptsql + "scoid = @Scoid,sconame = @Sconame,";
                                p.Add("@Scoid", pur.scoid);
                                p.Add("@Sconame", u[0].sconame );
                            }
                        }
                        if(result.s == -1)
                        {
                            return result;
                        }
                    }
                    if ( pur.remark != null)
                    {
                        uptsql = uptsql + "remark=@Remark,";
                        p.Add("@Remark", pur.remark);
                    }
                    if ( pur.shplogistics != null)
                    {
                        uptsql = uptsql + "shplogistics = @Shplogistics,";
                        p.Add("@Shplogistics", pur.shplogistics);
                    }
                    if ( pur.shpcity != null)
                    {
                        uptsql = uptsql + "shpcity = @Shpcity,";
                        p.Add("@Shpcity", pur.shpcity);
                    }
                    if ( pur.shpdistrict != null)
                    {
                        uptsql = uptsql + "shpdistrict = @Shpdistrict,";
                        p.Add("@Shpdistrict", pur.shpdistrict);
                    }
                    if ( pur.shpaddress != null)
                    {
                        uptsql = uptsql + "shpaddress=@Shpaddress,";
                        p.Add("@Shpaddress", pur.shpaddress);
                    }
                    if ( pur.contract != null)
                    {
                        uptsql = uptsql + "contract = @Contract,";
                        p.Add("@Contract", pur.contract);
                    }
                    if ( pur.purtype != 0)
                    {
                        uptsql = uptsql + "purtype=@Purtype,";
                        p.Add("@Purtype", pur.purtype);
                    }
                    if ( pur.buyyer != null)
                    {
                        uptsql = uptsql + "buyyer=@Buyyer,";
                        p.Add("@Buyyer", pur.buyyer);
                    }
                    if(pur.warehouseid != 0)
                    {
                        string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where warehouseid =" + pur.warehouseid + " and coid =" + CoID;
                        var u = DbBase.CommDB.Query<Warehouse>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "仓库编号不存在!!";
                        }
                        else
                        {
                            if(u[0].enable == false)
                            {
                                result.s = -1;
                                result.d = "仓库编号已停用!!";
                            }
                            else
                            {
                                if(u[0].parentid > 0)
                                {
                                    result.s = -1;
                                    result.d = "请选择主仓库!!";
                                }
                                else
                                {
                                    uptsql = uptsql + "warehouseid = @Warehouseid,warehousename = @Warehousename,";
                                    p.Add("@Warehouseid", pur.warehouseid);
                                    p.Add("@Warehousename", pur.warehousename);
                                }
                            }
                        }
                        if(result.s == -1)
                        {
                            return result;
                        }
                    }
                    if ( pur.taxrate != null)
                    {
                        uptsql = uptsql + "taxrate=@Taxrate,puramttot = puramtnet*(1 + taxrate/100),";
                        p.Add("@Taxrate", pur.taxrate);
                    }
                    if(uptsql.Substring(uptsql.Length - 1, 1) == ",")
                    {
                        uptsql = uptsql.Substring(0,uptsql.Length - 1);
                        uptsql = uptsql + " where id = @ID";
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
        public static DataResult ConfirmPurchase(List<int> PuridList,int CoID)
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
                string wheresql = @"select count(*) from purchase where id in @PurID and coid = @Coid and status <> 0" ;
                int u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "未审核状态的采购单才可执行审核操作!";
                }
                else
                {
                    string delsql = @"update purchase set status = 1 where id in @PurID and coid = @Coid";
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
        ///采购单完成
        ///</summary>
        public static DataResult CompletePurchase(List<int> PuridList,int CoID)
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
                string wheresql = @"select count(*) from purchase where id in @PurID and coid = @Coid and status not in (1,3,4)" ;
                var u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "已确认&&待发货&&待收货状态的采购单才可执行此操作!";
                }
                else
                {
                    string delsql = @"update purchase set status = 2 where id in @PurID and coid = @Coid";
                    int count = CoreDBconn.Execute(delsql, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        delsql = @"update purchasedetail set detailstatus = 2 where purchaseid in @PurID and coid = @Coid";
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
        public static DataResult InsertPurDetail(int id,List<int> skuid,int CoID)
        {
            var result = new DataResult(1,null);  
            var res = new PurchaseDetailInsert();
            var cc = new InsertFailReason();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select id,purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,warehouseid,warehousename,status,purtype,buyyer,remark,taxrate from purchase where id =" + id + " and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此采购单不存在!";
                }
                else
                {
                    if(u[0].status != 0)
                    {
                        result.s = -1;
                        result.d = "只有待审核的采购单才可以新增明细!";
                    }
                }
                if(result.s == -1)
                {
                    return result;
                }
                List<InsertFailReason> rt = new List<InsertFailReason>();
                List<int> rr = new List<int>();
                foreach(int a in skuid)
                {
                    InsertFailReason rf = new InsertFailReason();
                    string skusql = "select skuid,skuname,norm,img,goodscode,enable from coresku where id =" + a;
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
                    int x = CoreDBconn.QueryFirst<int>("select count(*) from purchasedetail where purchaseid = "+ id+" and coid =" + CoID + " and skuautoid = "+a);
                    if( x > 0)
                    {
                        rf.id = a;
                        rf.reason = null;
                        rt.Add(rf);
                        continue;
                    }
                    string sqlCommandText = @"INSERT INTO purchasedetail(purchaseid,skuautoid,skuid,skuname,norm,img,goodscode,coid) 
                                            VALUES(@PurID,@Skuautoid,@Skuid,@Skuname,@Norm,@Img,@Goodscode,@Coid)";
                    var args = new {PurID = id,Skuautoid = a,Skuid=s[0].skuid,Skuname = s[0].skuname,Norm = s[0].norm ,Img = s[0].img,Goodscode = s[0].goodscode ,Coid = CoID};
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
        ///采购单明细更新
        ///</summary>
        public static DataResult UpdatePurDetail(PurchaseDetail detail,int CoID)
        {
            var result = new DataResult(1,null);  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select id,purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,warehouseid,warehousename,status,purtype,buyyer,remark,taxrate " + 
                "from purchase where id =" + detail.purchaseid + " and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此采购单不存在!";
                }
                else
                {
                    if(u[0].status == 2 || u[0].status == 5)
                    {
                        result.s = -1;
                        result.d = "已完成或已作废的明细不允许修改!";
                    }
                }
                if(result.s == -1)
                {
                    return result;
                }
                wheresql = "select id,purchaseid,img,skuid,skuname,purqty,suggestpurqty,recqty,price,puramt,remark,goodscode,supplynum,supplycode,planqty,planamt,recievedate,norm,packingnum "+
                "from purchasedetail where purchaseid = " + detail.purchaseid + " and skuautoid =" + detail.id + " and coid =" + CoID;
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
                    sqlCommandText = sqlCommandText.Substring(0,sqlCommandText.Length - 1);
                    sqlCommandText = sqlCommandText + " where purchaseid = @Purchaseid and skuautoid = @ID and coid = @Coid";
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
                }
                if(flag > 0)
                {
                    string uptsql = @"update purchase set purqtytot = purqtytot - @Qty + @Purqty,puramtnet = puramtnet - @Amt + @Puramt,puramttot = puramtnet*(1 + taxrate/100) where id = @PurID and coid = @Coid";
                    var upargs = new {Purqty = qtynew,Qty = qty,Puramt = amtnew,Amt = amt,PurID = detail.purchaseid,Coid = CoID};
                    count = CoreDBconn.Execute(uptsql,upargs,TransCore);
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
                string wheresql = "select id,purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,warehouseid,warehousename,status,purtype,buyyer,remark,taxrate from purchase where id =" + id + " and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此采购单不存在!";
                }
                else
                {
                    if(u[0].status != 0)
                    {
                        result.s = -1;
                        result.d = "只有待审核的采购单明细才可删除!";
                    }
                }
                if(result.s == -1)
                {
                    return result;
                }
                wheresql = "select sum(purqty) as purqty,sum(puramt) as puramt from purchasedetail where purchaseid = @Purid and skuautoid in @ID and coid = @Coid";
                var argss = new {ID = detailid,Purid = id,Coid = CoID};
                var pur = CoreDBconn.Query<CalPurchase>(wheresql,argss).AsList();
                if (pur.Count == 0)
                {
                    result.s = -3001;
                }
                else
                {
                    decimal qty = pur[0].purqty;
                    decimal amt = pur[0].puramt;
                    string sqlCommandText = @"delete from  purchasedetail where purchaseid = @Purid and skuautoid in @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlCommandText,argss,TransCore);
                    if(count <= 0)
                    {
                        result.s = -3004;
                    }
                    string uptsql = @"update purchase set purqtytot = purqtytot - @Qty ,puramtnet = puramtnet - @Amt ,puramttot = puramtnet*(1 + taxrate/100) where id = @ID and coid = @Coid";
                    var upargs = new {Qty = qty,Amt = amt,ID = id,Coid = CoID};
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
        ///质检资料查询
        ///</summary>
        public static DataResult QualityRevList(int purid,int CoID,int PageIndex,int NumPerPage)
        {
            var result = new DataResult(1,null);
            string wheresql = "select id,purchaseid,recorddate,recorder,drawrate,type,conclusion,remark,status from qualityrev where purchaseid = " + purid + " and coid = " + CoID + " order by id asc";
            var res =new QualityRevData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<QualityRev>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(NumPerPage.ToString()));

                    int dataindex = (PageIndex - 1)* NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + NumPerPage.ToString();
                    u = conn.Query<QualityRev>(wheresql).AsList();
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
                    string wheresql = @"select count(*) from purchase where id = " + qua.purchaseid + " and status in (0,1)";
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
                    string sqlCommandText = @"INSERT INTO qualityrev(recorddate,recorder,drawrate,type,conclusion,remark,purchaseid,coid) 
                                                VALUES(@Recorddate,@Recorder,@Drawrate,@Type,@Conclusion,@Remark,@Purchaseid,@Coid)";
                    var args = new {Recorddate = qua.recorddate,Recorder=qua.recorder,Drawrate = qua.drawrate,Type = qua.type,
                                    Conclusion = qua.conclusion,Remark = qua.remark,Purchaseid = qua.purchaseid ,Coid = CoID};
                    int count = conn.Execute(sqlCommandText,args);
                    if(count <= 0)
                    {
                        result.s = -3002;
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
        public static DataResult UpdateQualityRev(QualityRev qua)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string wheresql = @"select count(*) from purchase where id = " + qua.purchaseid + " and status in (0,1)";
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
                        sqlCommandText = sqlCommandText.Substring(0,sqlCommandText.Length - 1);
                        sqlCommandText = sqlCommandText + " where id = @ID";
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
                    int u = conn.QueryFirst<int>("select count(*) from qualityrev where id in @ID and status <> 0",new {ID = id});
                    if (u > 0)
                    {
                        result.s = -1;
                        result.d = "已确认的资料不可删除";
                    }
                    else
                    {
                        string sqlCommandText = @"delete from qualityrev where id in @ID";
                        var args = new {ID = id};
                        int count = conn.Execute(sqlCommandText,args);
                        if(count < 0)
                        {
                            result.s = -3004;
                        }
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
        public static DataResult ConfirmQualityRev(int id)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    int u = conn.QueryFirst<int>("select count(*) from qualityrev where id = @ID and status <> 0",new {ID = id});
                    if (u > 0)
                    {
                        result.s = -1;
                        result.d = "待审核的资料才可执行此操作";
                    }
                    else
                    {
                        string sqlCommandText = @"update qualityrev set status = 1 where id = @ID";
                        var args = new {ID = id};
                        int count = conn.Execute(sqlCommandText,args);
                        if(count < 0)
                        {
                            result.s = -3003;
                        }
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
            var wh = CoreComm.WarehouseHaddle.GetParentWarehouseList(CoID);
            if(wh.s == 1)
            {
                res.warehouse = wh.d as List<Warehouse>;//Newtonsoft.Json.JsonConvert.DeserializeObject<>(wh.d.ToString());
            }
            //采购单基本资料
            string wheresql = "select id,purchasedate,scoid,sconame,contract,shplogistics,shpcity,shpdistrict,shpaddress,warehouseid,warehousename,status,purtype,buyyer,remark,taxrate " + 
                              "from purchase where 1 = 1"; 
            if(CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + CoID;
            }
            wheresql = wheresql + " order by id desc";
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<Purchase>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/20);

                    wheresql = wheresql + " limit 0,20";
                    u = conn.Query<Purchase>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Pur = u;
                    // if (count == 0)
                    // {
                    //     result.s = -3001;
                    //     result.d = null;
                    // }
                    // else
                    // {
                        result.d = res;
                    // }               
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
        public static DataResult UpdatePurRemark(int id,string remark)
        {
            var result = new DataResult(1,null);  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{      
                    int cnt = conn.QueryFirst<int>("select count(*) from purchase where id in @ID and status in (2,5)",new {ID = id});
                    if(cnt > 0)
                    {
                        result.s = -1;
                        result.d = "已完成&已作废采购单不可以更新备注";
                        return  result;
                    }
                    string uptsql = @"update purchase set remark=@Remark where id = @ID";
                    var args = new {Remark = remark,ID = id};
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
        ///采购单还原
        ///</summary>
        public static DataResult RestorePur(List<int> PuridList,int CoID)
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
                string wheresql = @"select count(*) from purchase where id in @PurID and coid = @Coid and status <> 1" ;
                int u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "已确认状态的采购单才可执行还原操作!";
                }
                else
                {
                    string delsql = @"update purchase set status = 0 where id in @PurID and coid = @Coid";
                    int count = CoreDBconn.Execute(delsql, p, TransCore);
                    if (count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        delsql = @"update purchasedetail set detailstatus = 0 where purchaseid in @PurID and coid = @Coid";
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
    }
}
            
                