using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreData.CoreCore
{
    public static class PurchaseReceiveHaddle
    {
        ///<summary>
        ///查询收料单List
        ///</summary>
        public static DataResult GetPurchaseRecList(PurchaseReceiveParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select * from purchasereceive where receivedate between '" + cp.RecdateStart + "' and '" + cp.RecdateEnd + "'"; //收料日期
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if (cp.Purid > 0)//采购单号
            {
                wheresql = wheresql + " AND purchaseid = "+ cp.Purid;
            }
            if(cp.IsNotPur == true)///无采购单件
            {
               wheresql = wheresql + " and purchaseid = ''";
            }
            if(cp.Status > 0)//状态
            {
               wheresql = wheresql + " and status = " + cp.Status;
            }
            if(cp.FinStatus > 0)//状态
            {
               wheresql = wheresql + " and finstatus = " + cp.FinStatus;
            }
            if(cp.Scoid > 0)//供应商
            {
               wheresql = wheresql + " and scoid = " + cp.Scoid;
            }
            if(!string.IsNullOrEmpty(cp.Skuid))//商品编码
            {
               wheresql = wheresql + " and exists(select skuid from purchaserecdetail where recid = purchasereceive.id and skuid = '" + cp.Skuid + "')";
            }
            if(!string.IsNullOrEmpty(cp.Remark))//备注
            {
               wheresql = wheresql + " and remark like '%" + cp.Skuid + "%'";
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new PurchaseReceiveData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<PurchaseReceive>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<PurchaseReceive>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.PurRec = u;
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
        ///查询收料单明细List
        ///</summary>
        public static DataResult GetPurchaseRecDetailList(PurchaseRecDetailParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select * from purchaserecdetail where recid = " + cp.Recid; //收料单号
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new PurchaseRecDetailData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    string pursql = "select * from purchasereceive where id = " + cp.Recid;
                    var pur = conn.Query<PurchaseReceive>(pursql).AsList();
                    if (pur.Count == 0)
                    {
                        result.s = -1;
                        result.d = "此收料单不存在";
                        return result;
                    }
                    else
                    {
                        res.status=pur[0].status ;
                    }           
                    var u = conn.Query<PurchaseRecDetail>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<PurchaseRecDetail>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.PurRecDetail = u;   
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
        ///收料单作废
        ///</summary>
        public static DataResult CancleReceive(List<int> RecidList,int CoID,string Username)
        {
            var result = new DataResult(1,null);     
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) )
            {
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@RecID", RecidList);
                    p.Add("@Coid", CoID);
                    string wheresql = @"select count(*) from purchasereceive where id in @RecID and coid = @Coid and status <> 0" ;
                    int u = conn.QueryFirst<int>(wheresql,p);
                    if(u > 0)
                    {
                        result.s = -1;
                        result.d = "未审核状态的收料单才可作废!";
                        return result;
                    }
                    string delsql = @"update purchasereceive set status = 3,modifier=@Modifier,modifydate=@Modifydate where id in @RecID and coid = @Coid";
                    p.Add("@Modifier", Username);
                    p.Add("@Modifydate", DateTime.Now);
                    int count = conn.Execute(delsql, p);
                    if (count < 0)
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
        ///收料单新增
        ///</summary>
        public static DataResult InsertReceive(int id,string type,string UserName,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlCommandText = "";
                    var p = new DynamicParameters();
                    if(type == "purchase")
                    {
                        string wheresql = "select * from purchase where id =" + id + " and coid =" + CoID ;
                        var u = conn.Query<Purchase>(wheresql).AsList();
                        if(u.Count == 0)
                        {
                            result.s = -1;
                            result.d = "此采购单不存在!";
                            return result;
                        }
                        else
                        {
                            if(u[0].status == 0 || u[0].status == 2 || u[0].status == 5)
                            {
                                result.s = -1;
                                result.d = "待审核&已作废&已完成的采购单不可执行此操作!";
                                return result;
                            }
                        }
                        sqlCommandText = @"INSERT INTO purchasereceive(scoid,sconame,purchaseid,coid,creator,receivedate,modifier) 
                                            VALUES(@Scoid,@Sconame,@Purchaseid,@Coid,@UName,@Receivedate,@UName)";
                        p.Add("@Scoid",u[0].scoid);
                        p.Add("@Sconame",u[0].sconame);
                        p.Add("@Purchaseid",id);
                        p.Add("@Coid",CoID);
                        p.Add("@UName",UserName);
                        p.Add("@Receivedate",DateTime.Now);
                    }
                    if(type == "supply")
                    {
                        string wheresql = "select id,sconame,scosimple,enable,scocode,address,country,contactor,tel,phone,fax,url,email,typelist,bank,bankid,taxid,remark from supplycompany where id =" + id;
                        var u = conn.Query<ScoCompanySingle>(wheresql).AsList();
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
                        }
                        sqlCommandText = @"INSERT INTO purchasereceive(scoid,sconame,coid,creator,receivedate,modifier) 
                                            VALUES(@Scoid,@Sconame,@Coid,@UName,@Receivedate,@UName)";
                        p.Add("@Scoid",u[0].id);
                        p.Add("@Sconame",u[0].sconame);
                        p.Add("@Coid",CoID);
                        p.Add("@UName",UserName);
                        p.Add("@Receivedate",DateTime.Now);
                    }
                    int count =conn.Execute(sqlCommandText,p);
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
        ///收料单更新
        ///</summary>
        public static DataResult UpdateReceive(PurchaseReceive rec,int CoID,string UserName)
        {
            var result = new DataResult(1,null);  
            var p = new DynamicParameters();
            string uptsql = @"update purchasereceive set ";
            int status = 0;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{   
                    string wheresql = "select * from purchasereceive where id =" + rec.id + " and coid =" + CoID ;
                    var u = conn.Query<PurchaseReceive>(wheresql).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "此收料单不存在!";
                        return result;
                    }
                    else
                    {
                        status = u[0].status;
                        if(u[0].status == 3)
                        {
                            result.s = -1;
                            result.d = "已作废的收料单不允许修改!";
                            return result;
                        }
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    if(rec.warehouseid != 0 && status == 0)
                    {
                        string wheresql = "select * from warehouse where id =" + rec.warehouseid + " and coid =" + CoID;
                        var a = conn.Query<Warehouse>(wheresql).AsList();
                        if(a.Count == 0)
                        {
                            result.s = -1;
                            result.d = "仓库编号不存在!!";
                            return result;
                        }
                        else
                        {
                            if(a[0].enable == false)
                            {
                                result.s = -1;
                                result.d = "仓库编号已停用!!";
                                return result;
                            }
                            else
                            {
                                if(a[0].parentid == 0)
                                {
                                    result.s = -1;
                                    result.d = "请选择小仓仓库!!";
                                    return result;
                                }
                                else
                                {
                                    uptsql = uptsql + "warehouseid = @Warehouseid,warehousename = @Warehousename,";
                                    p.Add("@Warehouseid", a[0].id);
                                    p.Add("@Warehousename", a[0].warehousename);
                                }
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
                    if(rec.remark != null)
                    {
                        uptsql = uptsql + "remark = @Remark,";
                        p.Add("@Remark", rec.remark);
                    }
                    if(uptsql.Substring(uptsql.Length - 1, 1) == ",")
                    {
                        uptsql = uptsql + "modifydate = @Modifydate,modifier = @Modifier where id = @ID";
                        p.Add("@Modifydate", DateTime.Now);
                        p.Add("@Modifier", UserName);
                        p.Add("@ID", rec.id);
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
        ///收料单明细新增
        ///</summary>
        public static DataResult InsertRecDetail(int id,List<int> skuid,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            var res = new PurchaseDetailInsert();
            var cc = new InsertFailReason();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from purchasereceive where id =" + id + " and coid =" + CoID ;
                var u = CoreDBconn.Query<PurchaseReceive>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此收料单不存在!";
                    return result;
                }
                else
                {
                    if(u[0].status != 0)
                    {
                        result.s = -1;
                        result.d = "只有待入库的收料单才可以新增明细!";
                        return result;
                    }
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
                    if(u[0].purchaseid > 0)
                    {
                        int y = CoreDBconn.QueryFirst<int>("select count(*) from purchasedetail where purchaseid = "+ u[0].purchaseid +" and coid =" + CoID + " and skuautoid = " + a);
                        if( y == 0)
                        {
                            rf.id = a;
                            rf.reason = "采购单不包含此商品编码，不能收料";
                            rt.Add(rf);
                            continue;
                        }
                    }
                    int x = CoreDBconn.QueryFirst<int>("select count(*) from purchaserecdetail where recid = "+ id +" and coid =" + CoID + " and skuautoid = "+ a );
                    if( x > 0)
                    {
                        rf.id = a;
                        rf.reason = null;
                        rt.Add(rf);
                        continue;
                    }
                    string sqlCommandText = @"INSERT INTO purchaserecdetail(recid,skuautoid,skuid,skuname,norm,img,goodscode,coid,creator,modifier) 
                                            VALUES(@RecID,@Skuautoid,@Skuid,@Skuname,@Norm,@Img,@Goodscode,@Coid,@Creator,@Creator)";
                    var args = new {RecID = id,Skuautoid = a,Skuid=s[0].skuid,Skuname = s[0].skuname,Norm = s[0].norm ,Img = s[0].img,Goodscode = s[0].goodscode ,Coid = CoID,Creator=Username};
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
        ///收料单明细更新
        ///</summary>
        public static DataResult UpdateRecDetail(PurchaseRecDetail detail,int CoID,string Username)
        {
            var result = new DataResult(1,null);  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string wheresql = "select * from purchasereceive where id =" + detail.recid + " and coid =" + CoID ;
                var u = CoreDBconn.Query<Purchase>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此收料单不存在!";
                     return result;
                }
                else
                {
                    if(u[0].status == 3)
                    {
                        result.s = -1;
                        result.d = "已作废的收料单不允许修改!";
                         return result;
                    }
                }
                wheresql = "select * from purchaserecdetail where recid = " + detail.recid + " and skuautoid =" + detail.id + " and coid =" + CoID;
                var pur = CoreDBconn.Query<PurchaseDetail>(wheresql).AsList();
                if (pur.Count == 0)
                {
                    result.s = -1;
                    result.d = "此收料单明细不存在!";
                    return result;
                }
                var p = new DynamicParameters();
                string sqlCommandText = @"update purchaserecdetail set ";
                int flag= 0;
                if(detail.price != null && u[0].status == 0)
                {
                    flag ++;
                    sqlCommandText = sqlCommandText + "price = @Price,";
                    p.Add("@Price", detail.price);
                }
                if(detail.recqty != null && u[0].status == 0)
                {
                    flag ++;
                    sqlCommandText = sqlCommandText + "recqty = @Recqty,";
                    p.Add("@Recqty", detail.recqty);
                }
                if(flag > 0)
                {
                    sqlCommandText = sqlCommandText + "Amount = price * recqty,";
                }
                if(detail.remark != null)
                {
                    sqlCommandText = sqlCommandText + "remark = @Remark,";
                    p.Add("@Remark", detail.remark);
                }
                if(sqlCommandText.Substring(sqlCommandText.Length - 1, 1) == ",")
                {
                    sqlCommandText = sqlCommandText + "modifier=@Modifier,modifydate=@Modifydate where recid = @Recid and skuautoid = @ID and coid = @Coid";
                    p.Add("@Modifier",Username);
                    p.Add("@Modifydate",DateTime.Now);
                    p.Add("@ID",detail.id);
                    p.Add("@Recid",detail.recid);
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
        ///收料单明细删除
        ///</summary>
        public static DataResult DelRecDetail(List<int> detailid,int recid,int CoID)
        {
            var result = new DataResult(1,null);    
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {         
                    string wheresql = "select * from purchasereceive where id =" + recid + " and coid =" + CoID ;
                    var u = conn.Query<Purchase>(wheresql).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "此收料单不存在!";
                        return result;
                    }
                    else
                    {
                        if(u[0].status != 0)
                        {
                            result.s = -1;
                            result.d = "待入库的收料单才可删除明细!";
                            return result;
                        }
                    }                              
                    string sqlCommandText = @"delete from  purchaserecdetail where recid = @Recid and skuautoid in @ID and coid = @Coid ";
                    var argss = new {Recid = recid,ID = detailid,Coid = CoID};
                    int count = conn.Execute(sqlCommandText,argss);
                    if(count <= 0)
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
        ///查询单笔收料单
        ///</summary>
        public static DataResult PurReceiveSingle(int id,int CoID)
        {
            var result = new DataResult(1,null);   
            string wheresql = "select * from purchasereceive where id =" + id + " and coid =" + CoID ;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<PurchaseReceive>(wheresql).AsList();
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
        ///收料单更新备注
        ///</summary>
        public static DataResult UpdateRecRemark(List<int> id,string remark,string Username)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{      
                    int cnt = conn.QueryFirst<int>("select count(*) from purchasereceive where id in @ID and status = 3",new {ID = id});
                    if(cnt > 0)
                    {
                        result.s = -1;
                        result.d = "作废的收料单不可以更新备注";
                        return  result;
                    }         
                    string uptsql = @"update purchasereceive set remark=@Remark,modifier=@Modifier,modifydate=@Modifydate where id in @ID";
                    var args = new {Remark = remark,ID = id,Modifier=Username,Modifydate = DateTime.Now};
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
        ///采购入库初始资料
        ///</summary>
        public static DataResult GetPurchaseRecInit(int CoID)
        {
            var result = new DataResult(1,null);   
            var res = new PurchaseRecInitData();  
            //采购入库单状态
            Dictionary<int,string> status = new Dictionary<int,string>();
            status.Add(0,"待入库");
            status.Add(1,"已入库");
            status.Add(2,"归档");
            status.Add(3,"作废");
            res.status = status;
            //财务状态
            Dictionary<int,string> finstatus = new Dictionary<int,string>();
            finstatus.Add(0,"待审核");
            finstatus.Add(1,"已审核");
            res.finstatus = finstatus;
            //采购入库基本资料
            string wheresql = "select * from purchasereceive where 1 = 1"; 
            if(CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + CoID;
            }
            wheresql = wheresql + " order by id desc";
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    var u = conn.Query<PurchaseReceive>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/20);

                    wheresql = wheresql + " limit 0,20";
                    u = conn.Query<PurchaseReceive>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.PurRec = u;
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
        ///收料入库审核
        ///</summary>
        public static DataResult ConfirmPurRec(List<int> RecidList,int CoID,string UserName)
        {
            var result = new DataResult(1,null);  
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                var p = new DynamicParameters();
                p.Add("@RecID", RecidList);
                p.Add("@Coid", CoID);
                string wheresql = @"select count(*) from purchasereceive where id in @RecID and coid = @Coid and status <> 0" ;
                int u = CoreDBconn.QueryFirst<int>(wheresql,p);
                if(u > 0)
                {
                    result.s = -1;
                    result.d = "未审核状态的收料单才可审核!";
                    return result;
                }
                wheresql = "select * from purchasereceive where id in @RecID and coid = @Coid";
                var rec = CoreDBconn.Query<PurchaseReceive>(wheresql,p).AsList();
                if(rec.Count == 0)
                {
                    result.s = -1;
                    result.d = "收料单号有误!";
                    return result;
                }
                foreach(var x in rec)
                {
                    wheresql = "select * from purchaserecdetail where recid =" + x.id + " and coid = " + CoID;
                    var recdetail = CoreDBconn.Query<PurchaseRecDetail>(wheresql).AsList();
                    if(recdetail.Count == 0)
                    {
                        result.s = -1;
                        result.d = "没有符合条件的收料单明细!";
                        return result;
                    }
                    string custypenew = "收料入库";
                    if(x.purchaseid > 0)
                    {
                        custypenew = "采购收料";
                    }
                    //新增出入库记录
                    string uptsql = @"INSERT INTO invinout(recordid,type,custype,whid,whname,isexport,creator,coid,modifier) 
                                    VALUES(@Recordid,@Type,@Custype,@Whid,@Whname,@Isexport,@Creator,@Coid,@Creator)";
                    var args = new {Recordid = x.id,Type = 1,Custype =custypenew,Whid = x.warehouseid,Whname = x.warehousename,Isexport = false,Creator = UserName,Coid = CoID};
                    int count = CoreDBconn.Execute(uptsql,args,TransCore);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    int rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    foreach(var y in recdetail)
                    {
                        //新增出入库明细记录
                        uptsql = @"INSERT INTO invinoutitem(ioid,coid,img,skuid,skuname,qty,creator,custype,norm,whid,whname,modifier) 
                                        VALUES(@Ioid,@Coid,@Img,@Skuid,@Skuname,@Qty,@Creator,@Custype,@Norm,@Whid,@Whname,@Creator)";
                        var argss = new {Ioid = rtn,Coid = CoID,Img = y.img,Skuid = y.skuid,Skuname = y.skuname,Qty = y.recqty,Creator = UserName,Custype =custypenew,
                                            Norm = y.norm,Whid = x.warehouseid,Whname = x.warehousename};
                        count = CoreDBconn.Execute(uptsql,argss,TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        //更新库存资料
                        count = CoreDBconn.QueryFirst<int>("select count(*) from inventory where skuid = '" + y.skuid + "' and warehouseid =" + x.warehouseid + " and coid = " + CoID);
                        if(count == 0)
                        {
                            uptsql = @"INSERT INTO inventory(goodscode,skuid,name,norm,warehouseid,warehousename,stockqty,pic,coid,modifier,creator) 
                                        VALUES(@Goodscode,@Skuid,@Skuname,@Norm,@Whid,@Whname,@Qty,@Img,@Coid,@Creator,@Creator)";
                            var argst = new {Goodscode = y.goodscode,Skuid = y.skuid,Skuname = y.skuname,Norm = y.norm,Whid = x.warehouseid,Whname = x.warehousename,
                                            Qty = y.recqty,Img = y.img,Coid = CoID,Creator=UserName};
                            count = CoreDBconn.Execute(uptsql,argst,TransCore);
                            if(count < 0)
                            {
                                result.s = -3002;
                                return result;
                            }
                        }
                        else
                        {
                            uptsql = @"update inventory set stockqty = stockqty + @Qty,modifier=@Modifier,modifydate=@Modifydate where skuid = @Skuid and warehouseid = @Whid and coid = @Coid";
                            var argst = new {Skuid = y.skuid,Whid = x.warehouseid,Qty = y.recqty,Coid = CoID,Modifier=UserName,Modifydate=DateTime.Now};
                            count = CoreDBconn.Execute(uptsql,argst,TransCore);
                            if(count < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                        }
                        //更新采购单明细资料
                        if(x.purchaseid > 0)
                        {
                            wheresql = "select * from purchasedetail where purchaseid = " + x.purchaseid + " and skuautoid = " + y.skuautoid + " and coid =" + CoID;
                            var purdetail = CoreDBconn.Query<PurchaseDetail>(wheresql).AsList();
                            if(purdetail.Count == 0)
                            {
                                result.s = -1;
                                result.d = "收料单明细资料异常";
                                return result;
                            }
                            uptsql = @"update purchasedetail set ";
                            if(decimal.Parse(purdetail[0].recqty) + decimal.Parse(y.recqty) >= decimal.Parse(purdetail[0].purqty))
                            {
                                uptsql = uptsql + "recqty = recqty + @Qty,detailstatus = 2,";
                            }
                            else
                            {
                                uptsql = uptsql + "recqty = recqty + @Qty,";
                            }
                            uptsql = uptsql + "modifier=@Modifier,modifydate=@Modifydate where purchaseid = @Purid and skuautoid = @Skuid and coid = @Coid";
                            var argst = new {Skuid = y.skuautoid,Purid = x.purchaseid,Qty = y.recqty,Coid = CoID,Modifier=UserName,Modifydate=DateTime.Now};
                            count = CoreDBconn.Execute(uptsql,argst,TransCore);
                            if(count < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                        }
                    }
                    //更新采购单资料
                    if(x.purchaseid > 0)
                    {
                        wheresql = "select count(*) from purchasedetail where purchaseid = " + x.purchaseid + " and coid =" + CoID + " and detailstatus <> 2";
                        int i = CoreDBconn.QueryFirst<int>(wheresql);
                        if(i == 0)
                        {
                            uptsql = @"update purchase set status = 2,modifier=@Modifier,modifydate=@Modifydate where id =" + x.purchaseid;
                            var argst = new {Modifier=UserName,Modifydate=DateTime.Now};
                            count = CoreDBconn.Execute(uptsql,argst,TransCore);
                            if(count < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                        }
                        
                    }
                }
                string uptsqln = @"update purchasereceive set status = 1,modifier=@Modifier,modifydate=@Modifydate where id in @RecID and coid = @Coid" ;
                p.Add("@Modifier",UserName);
                p.Add("@Modifydate",DateTime.Now);
                int t = CoreDBconn.Execute(uptsqln,p,TransCore);
                if(t < 0)
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
        ///财务审核
        ///</summary>
        public static DataResult FinConfirmRec(List<int> RecidList,int CoID,string UserName)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) )
            {
                try
                {
                    var p = new DynamicParameters();
                    p.Add("@RecID", RecidList);
                    p.Add("@Coid", CoID);
                    string wheresql = @"select count(*) from purchasereceive where id in @RecID and coid = @Coid and status <> 1" ;
                    int u = conn.QueryFirst<int>(wheresql,p);
                    if(u > 0)
                    {
                        result.s = -1;
                        result.d = "已入库的收料单才可财务审核!";
                        return result;
                    }
                    string delsql = @"update purchasereceive set finstatus = 1,modifier=@Modifier,modifydate=@Modifydate,finconfirmer=@Modifier,finconfirmdate=@Modifydate where id in @RecID and coid = @Coid";
                    p.Add("@Modifier", UserName);
                    p.Add("@Modifydate", DateTime.Now);
                    int count = conn.Execute(delsql, p);
                    if (count < 0)
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
    }
}