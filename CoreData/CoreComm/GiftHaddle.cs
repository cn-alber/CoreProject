using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
using CoreModels.XyComm;
namespace CoreData.CoreCore
{
    public static class GiftHaddle
    {
        ///<summary>
        ///店铺资料List
        ///</summary>
        public static DataResult GetShopInitData(int CoID)                
        {
            var result = new DataResult(1,null);
            //获取店铺List
            var shop = CoreComm.ShopHaddle.getShopEnum(CoID.ToString()) as List<shopEnum>;
            var ff = new List<Filter>();
            var f = new Filter();
            f.value = "-1";
            f.label = "---不限---";
            ff.Add(f);
            foreach(var t in shop)
            {
                f = new Filter();
                f.value = t.value.ToString();
                f.label = t.label;
                ff.Add(f);
            }     
            f = new Filter();
            f.value = "0";
            f.label = "{线下}";
            ff.Add(f); 
            result.d = ff;
            return result;
        }
        ///<summary>
        ///新增赠品规则
        ///</summary>
        public static DataResult InsertGiftRule(GiftRule gift,int CoID,string UserName)
        {
            var result = new DataResult(1,null);   
            int rtn = 0;
            var log = new LogInsert();
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            try
            {
                string sqlCommandText = @"INSERT INTO gift(GiftName,Priority,DateFrom,DateTo,AppointSkuID,AppointGoodsCode,ExcludeSkuID,ExcludeGoodsCode,AmtMin,AmtMax,
                                                           QtyMin,QtyMax,IsSkuIDValid,DiscountRate,AppointShop,OrdType,IsStock,IsAdd,QtyEach,AmtEach,IsMarkGift,MaxGiftQty,
                                                           Enable,CoID,Creator,Modifier) 
                                                    VALUES(@GiftName,@Priority,@DateFrom,@DateTo,@AppointSkuID,@AppointGoodsCode,@ExcludeSkuID,@ExcludeGoodsCode,@AmtMin,@AmtMax,
                                                           @QtyMin,@QtyMax,@IsSkuIDValid,@DiscountRate,@AppointShop,@OrdType,@IsStock,@IsAdd,@QtyEach,@AmtEach,@IsMarkGift,@MaxGiftQty,
                                                           @Enable,@CoID,@Creator,@Modifier)";
                int count = CommDBconn.Execute(sqlCommandText,gift,TransComm);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }      
                else
                {
                    rtn = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                }      
                if(gift.GiftNo != null)
                {
                    foreach(var a in gift.GiftNo)
                    {
                        sqlCommandText = @"INSERT INTO giftitem(GiftID,GiftNo,CoID) 
                                                        VALUES(@GiftID,@GiftNo,@CoID)";
                        count = CommDBconn.Execute(sqlCommandText,new{GiftID = rtn,GiftNo = a,CoID = CoID},TransComm);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }                     
                    }
                }   
                log.OID = rtn;            
                log.Type = 2;
                log.LogDate = DateTime.Now;
                log.UserName = UserName;
                log.Title = "新增规则";
                log.CoID = CoID;
                
                result.d = rtn;
                TransComm.Commit();
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransComm.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransComm.Dispose();
                CommDBconn.Dispose();
            }
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                       VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                    int count =conn.Execute(loginsert,log);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
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
        ///查看修改日志
        ///</summary>
        public static DataResult GetGiftLog(int id,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        string sqlcommand = @"select ID,LogDate,UserName,Title,Remark From orderlog where
                                            oid = @ID and coid = @Coid and type = 2 order by LogDate Asc";
                        var Log = conn.Query<OrderLog>(sqlcommand,new{ID=id,Coid=CoID}).AsList();
                        result.d = Log;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///禁用规则
        ///</summary>
        public static DataResult DisableRule(int id,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                        string sqlCommandText = @"update gift set Enable=@Enable,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid ";
                        int count = conn.Execute(sqlCommandText,new{Enable=false,Modifier=UserName,ModifyDate=DateTime.Now,ID=id,Coid=CoID});
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }        
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        var log = new LogInsert();
                        log.OID = id;            
                        log.Type = 2;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "禁用规则";
                        log.CoID = CoID;

                        string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                        VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                        int count =conn.Execute(loginsert,log);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///启用规则
        ///</summary>
        public static DataResult EnableRule(int id,int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                        string sqlCommandText = @"update gift set Enable=@Enable,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid ";
                        int count = conn.Execute(sqlCommandText,new{Enable=true,Modifier=UserName,ModifyDate=DateTime.Now,ID=id,Coid=CoID});
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }        
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{  
                        var log = new LogInsert();
                        log.OID = id;            
                        log.Type = 2;
                        log.LogDate = DateTime.Now;
                        log.UserName = UserName;
                        log.Title = "启用规则";
                        log.CoID = CoID;

                        string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                        VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                        int count =conn.Execute(loginsert,log);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        ///抓取单笔规则资料
        ///</summary>
        public static DataResult GetRuleEdit(int id,int CoID)
        {
            var result = new DataResult(1,null);
            var res = new GiftEditData();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                        string sqlCommandText = @"select ID,GiftName,Priority,DateFrom,DateTo,AppointSkuID,AppointGoodsCode,ExcludeSkuID,ExcludeGoodsCode,AmtMin,AmtMax,QtyMin,
                                                  QtyMax,IsSkuIDValid,DiscountRate,AppointShop,OrdType,IsStock,IsAdd,QtyEach,AmtEach,IsMarkGift,MaxGiftQty from gift where id =" + 
                                                  id + " and coid = " + CoID;
                        var u = conn.Query<GiftRuleEdit>(sqlCommandText).AsList();
                        if(u.Count > 0)
                        {
                            sqlCommandText = "select GiftNo from giftitem where GiftID = " + id + " and coid = " + CoID;
                            var a = new List<string>();
                            a = conn.Query<string>(sqlCommandText).AsList();
                            u[0].GiftNo = a;
                        }
                        res.Gift = u;
                        // result.d = u;
                    }
                    catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }
            var re = GetInitData(CoID);
            if(re.s == 1)
            {
                var a = re.d as GiftInitData;
                res.Shop = a.Shop;
                res.Type = a.Type;
            }
            result.d = res;
            return result;
        }
        ///<summary>
        ///更新赠品规则
        ///</summary>
        public static DataResult UpdateGiftRule(GiftRuleEdit gift,int CoID,string UserName,string IsSkuIDValid,string IsStock,string IsAdd,string IsMarkGift)
        {
            var result = new DataResult(1,null);   
            string content = string.Empty;
            var log = new LogInsert();
            bool isLog = false;
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            try
            {
                string sqlCommandText = @"select * from gift where id =" + gift.ID + " and coid = " + CoID;
                var u = CommDBconn.Query<GiftRule>(sqlCommandText).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "赠品规则ID无效";
                    return result;
                }
                int i = 0,y = 0;
                if(gift.GiftName != null && gift.GiftName != u[0].GiftName)
                {
                    content = content + "【规则名称】:" + gift.GiftName + ",";
                    i ++;
                    u[0].GiftName = gift.GiftName;
                }
                if(gift.Priority != 0 && gift.Priority != u[0].Priority)
                {
                    content = content + "【优先级】:" + gift.Priority + ",";
                    i ++;
                    u[0].Priority = gift.Priority;
                }
                if(gift.DateFrom > DateTime.Parse("190-01-01") && gift.DateFrom != u[0].DateFrom)
                {
                    content = content + "【开始时间】:" + gift.DateFrom + ",";
                    i ++;
                    u[0].DateFrom = gift.DateFrom;
                }
                if(gift.DateTo > DateTime.Parse("190-01-01") && gift.DateTo != u[0].DateTo)
                {
                    content = content + "【结束时间】:" + gift.DateTo + ",";
                    i ++;
                    u[0].DateTo = gift.DateTo;
                }
                if(gift.AppointSkuID != null && gift.AppointSkuID != u[0].AppointSkuID)
                {
                    content = content + "【指定商品编码】:" + gift.AppointSkuID + ",";
                    i ++;
                    u[0].AppointSkuID = gift.AppointSkuID;
                }
                if(gift.AppointGoodsCode != null && gift.AppointGoodsCode != u[0].AppointGoodsCode)
                {
                    content = content + "【指定款式编码】:" + gift.AppointGoodsCode + ",";
                    i ++;
                    u[0].AppointGoodsCode = gift.AppointGoodsCode;
                }
                if(gift.ExcludeSkuID != null && gift.ExcludeSkuID != u[0].ExcludeSkuID)
                {
                    content = content + "【排除商品编码】:" + gift.ExcludeSkuID + ",";
                    i ++;
                    u[0].ExcludeSkuID = gift.ExcludeSkuID;
                }
                if(gift.ExcludeGoodsCode != null && gift.ExcludeGoodsCode != u[0].ExcludeGoodsCode)
                {
                    content = content + "【排除款式编码】:" + gift.ExcludeGoodsCode + ",";
                    i ++;
                    u[0].ExcludeGoodsCode = gift.ExcludeGoodsCode;
                }
                if(gift.AmtMin != null && gift.AmtMin != u[0].AmtMin)
                {
                    content = content + "【最小金额】:" + gift.AmtMin + ",";
                    i ++;
                    u[0].AmtMin = gift.AmtMin;
                }
                if(gift.AmtMax != null && gift.AmtMax != u[0].AmtMax)
                {
                    content = content + "【最大金额】:" + gift.AmtMax + ",";
                    i ++;
                    u[0].AmtMax = gift.AmtMax;
                }
                if(gift.QtyMin != null && gift.QtyMin != u[0].QtyMin)
                {
                    content = content + "【最小数量】:" + gift.QtyMin + ",";
                    i ++;
                    u[0].QtyMin = gift.QtyMin;
                }
                if(gift.QtyMax != null && gift.QtyMax != u[0].QtyMax)
                {
                    content = content + "【最大数量】:" + gift.QtyMax + ",";
                    i ++;
                    u[0].QtyMax = gift.QtyMax;
                }
                if(IsSkuIDValid != null && gift.IsSkuIDValid != u[0].IsSkuIDValid)
                {
                    content = content + "【处理规则】:" + gift.IsSkuIDValid + ",";
                    i ++;
                    u[0].IsSkuIDValid = gift.IsSkuIDValid;
                }
                if(gift.DiscountRate != null && gift.DiscountRate != u[0].DiscountRate)
                {
                    content = content + "【折扣率】:" + gift.DiscountRate + ",";
                    i ++;
                    u[0].DiscountRate = gift.DiscountRate;
                }
                if(gift.AppointShop != null && gift.AppointShop != u[0].AppointShop)
                {
                    content = content + "【指定店铺】:" + gift.AppointShop + ",";
                    i ++;
                    u[0].AppointShop = gift.AppointShop;
                }
                if(gift.OrdType != null && gift.OrdType != u[0].OrdType)
                {
                    content = content + "【限定订单类型】:" + gift.OrdType + ",";
                    i ++;
                    u[0].OrdType = gift.OrdType;
                }
                if(IsStock != null && gift.IsStock != u[0].IsStock)
                {
                    content = content + "【有库存才赠送】:" + gift.IsStock + ",";
                    i ++;
                    u[0].IsStock = gift.IsStock;
                }
                if(IsAdd != null && gift.IsAdd != u[0].IsAdd)
                {
                    content = content + "【叠加赠送】:" + gift.IsAdd + ",";
                    i ++;
                    u[0].IsAdd = gift.IsAdd;
                }
                if(gift.QtyEach != null && gift.QtyEach != u[0].QtyEach)
                {
                    content = content + "【每多少数量赠送一组】:" + gift.QtyEach + ",";
                    i ++;
                    u[0].QtyEach = gift.QtyEach;
                }
                if(gift.AmtEach != null && gift.AmtEach != u[0].AmtEach)
                {
                    content = content + "【每多少金额赠送一组】:" + gift.AmtEach + ",";
                    i ++;
                    u[0].AmtEach = gift.AmtEach;
                }
                if(IsMarkGift != null && gift.IsMarkGift != u[0].IsMarkGift)
                {
                    content = content + "【是否标记为赠品】:" + gift.IsMarkGift + ",";
                    i ++;
                    u[0].IsMarkGift = gift.IsMarkGift;
                }
                if(gift.MaxGiftQty != null && gift.MaxGiftQty != u[0].MaxGiftQty)
                {
                    content = content + "【累积最大赠送数】:" + gift.MaxGiftQty + ",";
                    i ++;
                    u[0].MaxGiftQty = gift.MaxGiftQty;
                }
                if(gift.GiftNo != null)
                {
                    var giftnoNew = gift.GiftNo as List<string>;
                    sqlCommandText = "select GiftNo from giftitem where GiftID = " + gift.ID + " and coid = " + CoID;
                    var giftnoOlder = new List<string>();
                    giftnoOlder = CommDBconn.Query<string>(sqlCommandText).AsList();
                    if(giftnoNew.Count != giftnoOlder.Count)
                    {
                        y ++;
                    }
                    else
                    {
                        int sq = 0;
                        foreach(var a in giftnoNew)
                        {
                            foreach(var b in giftnoOlder)
                            {
                                if(a == b)
                                {
                                    sq ++;
                                }
                            }
                        }
                        if(sq != giftnoNew.Count)
                        {
                            y ++;
                        }
                        else
                        {
                            sq = 0;
                            foreach(var a in giftnoOlder)
                            {
                                foreach(var b in giftnoNew)
                                {
                                    if(a == b)
                                    {
                                        sq ++;
                                    }
                                }
                            }
                            if(sq != giftnoNew.Count)
                            {
                                y ++;
                            }
                        }
                    }
                    
                }
                if(i > 0)
                {
                    u[0].Modifier = UserName;
                    u[0].ModifyDate = DateTime.Now;
                    sqlCommandText = @"update gift set GiftName=@GiftName,Priority=@Priority,DateFrom=@DateFrom,DateTo=@DateTo,AppointSkuID=@AppointSkuID,MaxGiftQty=@MaxGiftQty,
                                       AppointGoodsCode=@AppointGoodsCode,ExcludeSkuID=@ExcludeSkuID,ExcludeGoodsCode=@ExcludeGoodsCode,AmtMin=@AmtMin,AmtMax=@AmtMax,QtyMin=@QtyMin,
                                       QtyMax=@QtyMax,IsSkuIDValid=@IsSkuIDValid,DiscountRate=@DiscountRate,AppointShop=@AppointShop,OrdType=@OrdType,IsStock=@IsStock,IsAdd=@IsAdd,
                                       QtyEach=@QtyEach,AmtEach=@AmtEach,IsMarkGift=@IsMarkGift,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CommDBconn.Execute(sqlCommandText,u[0],TransComm);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }      
                }
                if(y > 0)
                {
                    content = content + "【赠品】:" + string.Join(",",gift.GiftNo) + ",";
                    sqlCommandText = "delete from giftitem where GiftID = " + gift.ID + " and coid = " + CoID;
                    int count = CommDBconn.Execute(sqlCommandText,TransComm);
                    if(count < 0)
                    {
                        result.s = -3004;
                        return result;
                    }      
                    foreach(var a in gift.GiftNo)
                    {
                        sqlCommandText = @"INSERT INTO giftitem(GiftID,GiftNo,CoID) 
                                                        VALUES(@GiftID,@GiftNo,@CoID)";
                        count = CommDBconn.Execute(sqlCommandText,new{GiftID = gift.ID,GiftNo = a,CoID = CoID},TransComm);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }                     
                    }
                }
                if( i + y > 0)
                {
                    isLog = true;
                    log.OID = gift.ID;            
                    log.Type = 2;
                    log.LogDate = DateTime.Now;
                    log.UserName = UserName;
                    log.Title = "修改规则";
                    log.Remark = content;
                    log.CoID = CoID;
                }               
                
                TransComm.Commit();
            }
            catch (Exception e)
            {
                TransComm.Rollback();
                TransComm.Dispose();
                result.s = -1;
                result.d = e.Message;
            }
            finally
            {
                TransComm.Dispose();
                CommDBconn.Dispose();
            }
            if(isLog == true)
            {
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                        string loginsert = @"INSERT INTO orderlog(OID,SoID,Type,LogDate,UserName,Title,Remark,CoID) 
                                                        VALUES(@OID,@SoID,@Type,@LogDate,@UserName,@Title,@Remark,@CoID)";
                        int count =conn.Execute(loginsert,log);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                    }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                } 
            }
            return result;
        }
        ///<summary>
        ///查询赠品规则List
        ///</summary>
        public static DataResult GetGiftRuleList(GiftRuleParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from gift where 1=1";
            string sqlcommand = @"select ID,GiftName,AppointShop,AppointSkuID,ExcludeSkuID,AmtMin,AmtMax,QtyMin,QtyMax,DateFrom,DateTo,IsSkuIDValid,DiscountRate,
                                  MaxGiftQty,GivenQty,QtyEach,AmtEach,IsStock,IsAdd,Enable,CreateDate,ModifyDate from gift where 1=1"; 
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(cp.ID > 0)//规则ID
            {
                wheresql = wheresql + " and id = " + cp.ID;
            }
            if(!string.IsNullOrEmpty(cp.GiftNo))//赠品
            {
               wheresql = wheresql + " and exists(select id from giftitem where GiftID = gift.id and GiftNo like '%" + cp.GiftNo + "%')";
            }
            if(!string.IsNullOrEmpty(cp.GiftName))//名称
            {
               wheresql = wheresql + " and GiftName like '%" + cp.GiftName + "%'";
            }
            if(cp.DateFrom > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " and '" + cp.DateFrom + "' between DateFrom and DateTo";
            }
            if(cp.DateTo > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " and '" + cp.DateTo + "' between DateFrom and DateTo";
            }
            if(!string.IsNullOrEmpty(cp.AppointSkuID))
            {
               wheresql = wheresql + " and AppointSkuID like '%" + cp.AppointSkuID + "%'";
            }
            if(!string.IsNullOrEmpty(cp.ExcludeSkuID))
            {
               wheresql = wheresql + " and ExcludeSkuID like '%" + cp.ExcludeSkuID + "%'";
            }
            if(!string.IsNullOrEmpty(cp.AmtMinStart))
            {
               wheresql = wheresql + " and AmtMin >= " + cp.AmtMinStart;
            }
            if(!string.IsNullOrEmpty(cp.AmtMinEnd))
            {
               wheresql = wheresql + " and AmtMin <= " + cp.AmtMinEnd;
            }
            if(!string.IsNullOrEmpty(cp.AmtMaxStart))
            {
               wheresql = wheresql + " and AmtMax >= " + cp.AmtMaxStart;
            }
            if(!string.IsNullOrEmpty(cp.AmtMaxEnd))
            {
               wheresql = wheresql + " and AmtMax <= " + cp.AmtMaxEnd;
            }
            if(!string.IsNullOrEmpty(cp.QtyMinStart))
            {
               wheresql = wheresql + " and QtyMin >= " + cp.QtyMinStart;
            }
            if(!string.IsNullOrEmpty(cp.QtyMinEnd))
            {
               wheresql = wheresql + " and QtyMin <= " + cp.QtyMinEnd;
            }
            if(!string.IsNullOrEmpty(cp.QtyMaxStart))
            {
               wheresql = wheresql + " and QtyMax >= " + cp.QtyMaxStart;
            }
            if(!string.IsNullOrEmpty(cp.QtyMaxEnd))
            {
               wheresql = wheresql + " and QtyMax <= " + cp.QtyMaxEnd;
            }
            if(cp.IsEnable == true && cp.IsDisable == false)
            {
                wheresql = wheresql + " and Enable = true";
            }
            if(cp.IsEnable == false && cp.IsDisable == true)
            {
                wheresql = wheresql + " and Enable = false";
            }
            if(!string.IsNullOrEmpty(cp.QtyEachStart))
            {
               wheresql = wheresql + " and QtyEach >= " + cp.QtyEachStart;
            }
            if(!string.IsNullOrEmpty(cp.QtyEachEnd))
            {
               wheresql = wheresql + " and QtyEach <= " + cp.QtyEachEnd;
            }
            if(!string.IsNullOrEmpty(cp.AmtEachStart))
            {
               wheresql = wheresql + " and AmtEach >= " + cp.AmtEachStart;
            }
            if(!string.IsNullOrEmpty(cp.AmtEachEnd))
            {
               wheresql = wheresql + " and AmtEach <= " + cp.AmtEachEnd;
            }
            if(cp.CreateDateStart > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " and CreateDate >= '" + cp.CreateDateStart + "'";
            }
            if(cp.CreateDateEnd > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " and CreateDate <= '" + cp.CreateDateEnd + "'";
            }
            if(!string.IsNullOrEmpty(cp.AppointShop))
            {
               wheresql = wheresql + " and AppointShop like '%" + cp.AppointShop + "%'";
            }
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new GiftRuleData();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<GiftRuleQuery>(sqlcommand + wheresql).AsList();
                    foreach(var a in u)
                    {
                        if(DateTime.Parse(a.DateTo) > DateTime.Now)
                        {
                            a.Status = "生效";
                        }
                        else
                        {
                            a.Status = "过期";
                        }
                        string sqlCommandText = "select GiftNo from giftitem where GiftID = " + a.ID + " and coid = " + cp.CoID;
                        var t = new List<string>();
                        t = conn.Query<string>(sqlCommandText).AsList();
                        a.GiftNo = t;
                    }
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Gift = u;
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
        ///新增赠品初始资料
        ///</summary>
        public static DataResult GetInitData(int CoID)                
        {
            var result = new DataResult(1,null);
            var res = new GiftInitData();
            //获取店铺List
            var shop = CoreComm.ShopHaddle.getShopEnum(CoID.ToString()) as List<shopEnum>;
            var ff = new List<Filter>();
            foreach(var t in shop)
            {
                var f = new Filter();
                f.value = t.value.ToString();
                f.label = t.label;
                ff.Add(f);
            }      
            res.Shop = ff;
            //订单类型
            var oo = new List<Filter>();
            var o = new Filter();
            o.value = "0";
            o.label = "普通订单";
            oo.Add(o);
            o = new Filter();
            o.value = "1";
            o.label = "补发订单";
            oo.Add(o);
            o = new Filter();
            o.value = "2";
            o.label = "换货订单";
            oo.Add(o);
            o = new Filter();
            o.value = "3";
            o.label = "天猫分销";
            oo.Add(o);
            o = new Filter();
            o.value = "4";
            o.label = "天猫供销";
            oo.Add(o);
            o = new Filter();
            o.value = "5";
            o.label = "协同订单";
            oo.Add(o);
            o = new Filter();
            o.value = "6";
            o.label = "普通订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "7";
            o.label = "补发订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "8";
            o.label = "换货订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "9";
            o.label = "天猫供销,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "10";
            o.label = "协同订单,分销+";
            oo.Add(o);
            o = new Filter();
            o.value = "11";
            o.label = "普通订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "12";
            o.label = "补发订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "13";
            o.label = "换货订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "14";
            o.label = "天猫供销,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "15";
            o.label = "协同订单,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "16";
            o.label = "普通订单,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "17";
            o.label = "补发订单,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "18";
            o.label = "换货订单,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "19";
            o.label = "天猫供销,分销+,供销+";
            oo.Add(o);
            o = new Filter();
            o.value = "20";
            o.label = "协同订单,分销+,供销+";
            oo.Add(o);
            res.Type = oo;
            result.d = res;
            return result;
        }
        public static DataResult SetGiftItem(Order ord,List<OrderItem> item,int CoID)
        {
            var result = new DataResult(1,null); 
            var giftNo = new List<GiftInsertOrder>();   
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    string sqlcommand = "select * from gift where coid = " + CoID + " and Enable = true order by Priority ASC";
                    var u = conn.Query<GiftRule>(sqlcommand).AsList();
                    //获取赠品明细
                    foreach(var a in u)
                    {
                        //日期区间判断
                        DateTime ordDate = DateTime.Parse(ord.ODate.ToString("d"));
                        if(ordDate < a.DateFrom || ordDate > a.DateTo) continue;
                        //最大赠送数量判断
                        if(!string.IsNullOrEmpty(a.MaxGiftQty) && !string.IsNullOrEmpty(a.GivenQty) && decimal.Parse(a.MaxGiftQty) <= decimal.Parse(a.GivenQty)) continue;
                        //指定店铺判断
                        if(!string.IsNullOrEmpty(a.AppointShop))
                        {
                            int i = 0;
                            string[] shop = a.AppointShop.Split(',');
                            foreach(var s in shop)
                            {
                                if(s == ord.ShopID.ToString())
                                {
                                    i ++;
                                    break;
                                }
                            }
                            if(i > 0 ) continue;
                        }
                        //指定订单类型判断
                        if(!string.IsNullOrEmpty(a.OrdType))
                        {
                            int i = 0;
                            string[] type = a.OrdType.Split(',');
                            foreach(var s in type)
                            {
                                if(s == ord.Type.ToString())
                                {
                                    i ++;
                                    break;
                                }
                            }
                            if(i > 0 ) continue;
                        }
                        //排除商品判断
                        if(!string.IsNullOrEmpty(a.ExcludeSkuID))
                        {
                            int i = 0;
                            foreach(var it in item)
                            {
                                if(it.IsGift == true) continue;
                                if(a.ExcludeSkuID.Contains(it.SkuID))
                                {
                                    i ++;
                                    break;
                                }
                            }
                            if(i > 0 ) continue;
                        }
                        //排除款式判断
                        if(!string.IsNullOrEmpty(a.ExcludeGoodsCode))
                        {
                            int i = 0;
                            foreach(var it in item)
                            {
                                if(it.IsGift == true) continue;
                                if(a.ExcludeGoodsCode.Contains(it.GoodsCode))
                                {
                                    i ++;
                                    break;
                                }
                            }
                            if(i > 0 ) continue;
                        }
                        //指定商品判断
                        if(!string.IsNullOrEmpty(a.AppointSkuID))
                        {
                            int i = 0;
                            foreach(var it in item)
                            {
                                if(it.IsGift == true) continue;
                                if(a.AppointSkuID.Contains(it.SkuID))
                                {
                                    i ++;
                                    break;
                                }
                            }
                            if(i == 0 ) continue;
                        }
                        //指定款式判断
                        if(!string.IsNullOrEmpty(a.AppointGoodsCode))
                        {
                            int i = 0;
                            foreach(var it in item)
                            {
                                if(a.AppointGoodsCode.Contains(it.GoodsCode))
                                {
                                    if(it.IsGift == true) continue;
                                    i ++;
                                    break;
                                }
                            }
                            if(i == 0 ) continue;
                        }
                        //数量金额判断
                        if(!string.IsNullOrEmpty(a.AmtMin) || !string.IsNullOrEmpty(a.AmtMax) || !string.IsNullOrEmpty(a.QtyMin) || !string.IsNullOrEmpty(a.QtyMax))
                        {
                            int qty = 0;
                            decimal amt = 0;
                            foreach(var it in item)
                            {
                                if(it.IsGift == true) continue;
                                if(a.IsSkuIDValid == false)
                                {
                                    qty = qty + it.Qty;
                                    amt = amt + decimal.Parse(it.Amount);
                                }
                                else
                                {
                                    if(!string.IsNullOrEmpty(a.AppointSkuID) && a.AppointSkuID.Contains(it.SkuID))
                                    {
                                        qty = qty + it.Qty;
                                        amt = amt + decimal.Parse(it.Amount);
                                    }
                                    else if(!string.IsNullOrEmpty(a.AppointGoodsCode) && a.AppointGoodsCode.Contains(it.GoodsCode))
                                    {
                                        qty = qty + it.Qty;
                                        amt = amt + decimal.Parse(it.Amount);
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(a.AmtMin) && amt < decimal.Parse(a.AmtMin)) continue;
                            if(!string.IsNullOrEmpty(a.AmtMax) && amt > decimal.Parse(a.AmtMax)) continue;
                            if(!string.IsNullOrEmpty(a.QtyMin) && qty < decimal.Parse(a.QtyMin)) continue;
                            if(!string.IsNullOrEmpty(a.QtyMax) && qty > decimal.Parse(a.QtyMax)) continue;
                        }
                        //折扣率判断
                        if(!string.IsNullOrEmpty(a.DiscountRate))
                        {
                            decimal amtOlder = 0,amtNew = 0;
                            foreach(var it in item)
                            {
                                if(it.IsGift == true) continue;
                                amtOlder = amtOlder + it.Qty * decimal.Parse(it.SalePrice);
                                amtNew = amtOlder + it.Qty * decimal.Parse(it.RealPrice);
                            }
                            decimal dis = amtNew/amtOlder;
                            if(dis < decimal.Parse(a.DiscountRate)) continue;
                        }
                        //计算赠品数量
                        decimal giftQty = 0;
                        if(string.IsNullOrEmpty(a.AmtEach) && string.IsNullOrEmpty(a.QtyEach))
                        {
                            giftQty = 1;
                        }
                        else
                        {
                            int qty = 0;
                            decimal amt = 0;
                            foreach(var it in item)
                            {
                                if(it.IsGift == true) continue;
                                if(a.IsSkuIDValid == false)
                                {
                                    qty = qty + it.Qty;
                                    amt = amt + decimal.Parse(it.Amount);
                                }
                                else
                                {
                                    if(!string.IsNullOrEmpty(a.AppointSkuID) && a.AppointSkuID.Contains(it.SkuID))
                                    {
                                        qty = qty + it.Qty;
                                        amt = amt + decimal.Parse(it.Amount);
                                    }
                                    else if(!string.IsNullOrEmpty(a.AppointGoodsCode) && a.AppointGoodsCode.Contains(it.GoodsCode))
                                    {
                                        qty = qty + it.Qty;
                                        amt = amt + decimal.Parse(it.Amount);
                                    }
                                }
                            }
                            if(!string.IsNullOrEmpty(a.AmtEach))
                            {
                                giftQty = Math.Floor(amt/decimal.Parse(a.AmtEach));
                            }
                            else if(!string.IsNullOrEmpty(a.QtyEach))
                            {
                                giftQty = Math.Floor(qty/decimal.Parse(a.QtyEach));
                            }
                        }
                        sqlcommand = "select GiftNo from giftitem where GiftID = " + a.ID + " and coid = " + CoID;
                        var giftitem = new List<string>();
                        giftitem = conn.Query<string>(sqlcommand).AsList();
                        foreach(var it in giftitem)
                        {
                            var yy = new GiftInsertOrder();
                            string[] giftno = it.Split(',');
                            if(a.IsStock == false)
                            {
                                sqlcommand = "select ID from coresku where skuid = '" + giftno[0] + "' and coid = " + CoID;
                                int skuid = conn.QueryFirst<int>(sqlcommand);
                                yy.SkuAutoID = skuid;
                                yy.Qty = int.Parse(giftQty.ToString());
                                yy.IsGift = a.IsMarkGift;
                                giftNo.Add(yy);
                            }
                            else
                            {
                                foreach(var n in giftno)
                                {
                                    sqlcommand = "select ID from coresku where skuid = '" + n + "' and coid = " + CoID;
                                    int skuid = conn.QueryFirst<int>(sqlcommand);
                                    //检查库存是否足够
                                    int invqty = OrderHaddle.GetInvQty(CoID,skuid);
                                    if(invqty >= giftQty)
                                    {
                                        yy.SkuAutoID = skuid;
                                        yy.Qty = int.Parse(giftQty.ToString());
                                        yy.IsGift = a.IsMarkGift;
                                        giftNo.Add(yy);
                                        break;
                                    }
                                }
                            }
                        }
                        //叠加赠送
                        if(a.IsAdd == false) break;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }  
            if(giftNo.Count > 0)
            {
                result.d = giftNo;
            }
            else
            {
                result.s = 0;
            }
            return result;
        }
        public static DataResult SetGift(Order ord,List<OrderItem> item,int CoID,string UserName)
        {
            var result = new DataResult(1,null); 
            var res = new GiftInsertOrderReturn();
            var re = SetGiftItem(ord,item,CoID);
            if(re.s != 1)
            {
                result.s = 0;
                return result;
            }
            var giftNo = re.d as List<GiftInsertOrder>; 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    

                    if(giftNo.Count > 0)
                    {
                        int Qty = 0;
                        decimal Exweight = 0;
                        foreach(var a in giftNo)
                        {
                            string skusql = "select skuid,skuname,norm,img,goodscode,enable,saleprice,weight from coresku where id =" + a + " and coid =" + CoID;
                            var s = conn.Query<SkuInsert>(skusql).AsList();
                            if (s.Count == 0)
                            {
                                continue;
                            }
                            if (s[0].enable == false)
                            {
                                continue;
                            }
                            Qty = Qty + a.Qty;
                            Exweight = Exweight + decimal.Parse(s[0].weight) * a.Qty;
                            int i = 0;
                            foreach(var it in item)
                            {
                                if(it.IsGift == false) continue;
                                if(it.SkuAutoID == a.SkuAutoID && a.IsGift == true)
                                {
                                    it.Qty = it.Qty + a.Qty;
                                    it.TotalWeight = it.TotalWeight + decimal.Parse(s[0].weight) * a.Qty;
                                    i ++ ;
                                    break;
                                }
                            }
                            if(i == 0)
                            {
                                var it = new OrderItem();
                                it.SoID = item[0].SoID;
                                it.CoID = CoID;
                                it.SkuAutoID = a.SkuAutoID;
                                it.SkuID = s[0].skuid;
                                it.Qty = a.Qty;
                                it.RealPrice = "0";
                                it.Amount = "0";
                                it.SkuName = s[0].skuname;
                                it.Norm = s[0].norm;
                                it.GoodsCode = s[0].goodscode;
                                it.SalePrice = s[0].saleprice;
                                it.DiscountRate = "0";
                                it.img = s[0].img;
                                it.Weight = s[0].weight;
                                it.TotalWeight = (a.Qty * decimal.Parse(s[0].weight)).ToString();
                                it.IsGift = a.IsGift;
                                it.Creator = UserName;
                                it.CreateDate = DateTime.Now;
                                it.Modifier = UserName;
                                it.ModifyDate= DateTime.Now;
                                item.Add(it);
                            }
                        }
                        res.Qty = Qty;
                        res.Exweight = Exweight;
                        res.Item = item;
                        result.d = res;
                    }
                    else
                    {
                        result.s = 0;
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