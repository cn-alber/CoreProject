using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;

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
            string wheresql = "select id,scoid,sconame,purchaseid,creator,warehouseid,warehousename,status,finstatus,receivedate,remark,logisticsno,modifydate,finconfirmer,finconfirmdate " + 
                              "from purchasereceive where receivedate between '" + cp.RecdateStart + "' and '" + cp.RecdateEnd + "'"; //收料日期
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
        ///查询采购单明细List
        ///</summary>
        public static DataResult GetPurchaseRecDetailList(PurchaseRecDetailParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,recid,img,skuid,skuname,norm,recqty,planrecqty,price,amount,remark,goodscode,supplynum " + 
                              "from purchaserecdetail where recid = " + cp.Recid; //收料单号
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
                    string pursql = "select id,scoid,sconame,purchaseid,creator,warehouseid,warehousename,status,finstatus,receivedate,remark,logisticsno,modifydate,finconfirmer,finconfirmdate from purchasereceive where id = " + cp.Recid;
                    var pur = conn.Query<PurchaseReceive>(pursql).AsList();
                    if (pur.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        if(pur[0].status == 0)
                        {
                            res.enable = true;
                        }
                        else
                        {
                            res.enable = false;
                        }
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
        public static DataResult CancleReceive(List<int> RecidList,int CoID)
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
                    }
                    else
                    {
                        string delsql = @"update purchasereceive set status = 1 where id in @RecID and coid = @Coid";
                        int count = conn.Execute(delsql, p);
                        if (count < 0)
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
        ///收料单新增
        ///</summary>
        public static DataResult InsertReceive(PurchaseReceive rec,string UserName,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlCommandText = @"INSERT INTO purchasereceive(scoid,sconame,purchaseid,coid,creator,warehouseid,warehousename,receivedate,remark,logisticsno) 
                                VALUES(@Scoid,@Sconame,@Purchaseid,@Coid,@UName,@Warehouseid,@Warehousename,@Receivedate,@Remark,@Logisticsno)";
                    var args = new {Scoid = rec.scoid,Sconame = rec.sconame,Purchaseid = rec.purchaseid,Coid = CoID,UName = UserName,
                                    Warehouseid = rec.warehouseid,Warehousename = rec.warehousename,Receivedate = rec.receivedate,Remark = rec.remark,Logisticsno = rec.logisticsno};
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
        ///收料单更新
        ///</summary>
        public static DataResult UpdateReceive(PurchaseReceive rec)
        {
            var result = new DataResult(1,null);  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{               
                    string uptsql = @"update purchasereceive set scoid = @Scoid,sconame = @Sconame,remark=@Remark,warehouseid = @Warehouseid,warehousename = @Warehousename,
                                        purchaseid = @Purchaseid,receivedate = @Receivedate,logisticsno = @Logisticsno,modifydate = @Modifydate where id = @ID";
                    var args = new {Scoid = rec.scoid,Sconame = rec.sconame,Remark = rec.remark,Warehouseid = rec.warehouseid,Warehousename = rec.warehousename,
                                    Purchaseid = rec.purchaseid,Receivedate = rec.receivedate,Logisticsno = rec.logisticsno,Modifydate = DateTime.Now,ID = rec.id};
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
        ///收料单明细新增
        ///</summary>
        public static DataResult InsertRecDetail(PurchaseRecDetail detail,int CoID)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sqlCommandText = @"INSERT INTO purchaserecdetail(recid,skuid,skuname,price,recqty,planrecqty,amount,remark,norm,img,goodscode,supplynum,coid) 
                                                VALUES(@RecID,@Skuid,@Skuname,@Price,@Recqty,@Planrecqty,@Amount,@Remark,@Norm,@Img,@Goodscode,@Supplynum,@Coid)";
                    var args = new {RecID = detail.recid,Skuid=detail.skuid,Skuname = detail.skuname,Price = detail.price,Recqty = detail.recqty,Planrecqty = detail.planrecqty,
                                    Amount = detail.amount,Remark = detail.remark,Norm = detail.norm ,Img = detail.img,Goodscode = detail.goodscode ,Supplynum = detail.supplynum,Coid = CoID};
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
        ///收料单明细更新
        ///</summary>
        public static DataResult UpdateRecDetail(PurchaseRecDetail detail,int CoID)
        {
            var result = new DataResult(1,null);  
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {
                    string sqlCommandText = @"update purchaserecdetail set skuid = @Skuid,skuname = @Skuname,img = @Img,norm = @Norm,price = @Price,recqty = @Recqty,
                                            planrecqty = @Planrecqty,amount = @Amount,goodscode = @Goodscode,supplynum = @Supplynum,remark = @Remark where id = @ID ";
                    var args = new {Skuid=detail.skuid,Skuname = detail.skuname,Img = detail.img,Norm = detail.norm ,Price = detail.price,Recqty = detail.recqty,
                                    Planrecqty = detail.planrecqty,Amount = detail.amount,Remark = detail.remark,Goodscode = detail.goodscode ,Supplynum = detail.supplynum,ID = detail.id};
                    int count = conn.Execute(sqlCommandText,args);
                    if(count <= 0)
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
        ///采购单明细删除
        ///</summary>
        public static DataResult DelRecDetail(List<int> detailid)
        {
            var result = new DataResult(1,null);    
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try
                {                       
                    string sqlCommandText = @"delete from  purchaserecdetail where id in @ID ";
                    var argss = new {ID = detailid};
                    int count = conn.Execute(sqlCommandText,argss);
                    if(count <= 0)
                    {
                        result.s = -3004;
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
            string wheresql = "select id,scoid,sconame,purchaseid,creator,warehouseid,warehousename,status,finstatus,receivedate,remark,logisticsno,modifydate,finconfirmer,finconfirmdate " + 
                              "from purchasereceive where id =" + id + " and coid =" + CoID ;
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
    }
}