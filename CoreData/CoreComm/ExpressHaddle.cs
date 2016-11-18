using CoreModels;
using MySql.Data.MySqlClient;
using System;
// using CoreModels.XyCore;
using Dapper;
// using System.Collections.Generic;
using CoreModels.XyComm;
// using static CoreModels.Enum.OrderE;
namespace CoreData.CoreComm
{
    public static class ExpressHaddle
    {
        ///<summary>
        ///抓取快递List
        ///</summary>
        public static DataResult GetExpressList(int CoID,string SortField,string SortDirection,int PageIndex,int NumPerPage)
        {
            var result = new DataResult(1,null);
            var res = new ExpressData();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                        string sqlquery = "select count(id) From express where coid = " + CoID;
                        string sqlcommand = @"select ID,ExpID,ExpName,Priority,PriorityLogistics,FreightFirst,OrdAmtStart,OrdAmtEnd,PrioritySku,DisableArea,ExpCalMethod,
                                              IgnoreArrival,Enable,IsCOD,ModifyDate From express where coid = " + CoID;
                        if(!string.IsNullOrEmpty(SortField) && !string.IsNullOrEmpty(SortDirection))//排序
                        {
                            sqlcommand = sqlcommand + " ORDER BY "+SortField +" "+ SortDirection;
                        }
                        int count = conn.QueryFirst<int>(sqlquery);
                        decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(NumPerPage.ToString()));
                        int dataindex = (PageIndex - 1)* NumPerPage;
                        sqlcommand = sqlcommand + " limit " + dataindex.ToString() + " ," + NumPerPage.ToString();
                        var u = conn.Query<ExpressQuery>(sqlcommand).AsList();
                        res.Datacnt = count;
                        res.Pagecnt = pagecnt;
                        res.Exp = u;      
                        result.d = res;              
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
        ///绑定新的物流公司
        ///</summary>
        public static DataResult InsertExpress(int CoID,string ExpID,string ExpName,string UserName)
        {
            var result = new DataResult(1,null);
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            try{  
                //新增快递资料
                string sqlcommand = @"INSERT INTO express(ExpID,ExpName,CoID,Creator,Modifier) 
                                                   VALUES(@ExpID,@ExpName,@CoID,@Creator,@Creator)";
                int count =CommDBconn.Execute(sqlcommand,new{ExpID = ExpID,ExpName = ExpName,CoID = CoID,Creator = UserName},TransComm);
                if(count < 0)
                {
                    result.s = -3002;
                    return  result;
                }
                int rtn = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                var exp = CommDBconn.Query<Express>("select * from express where id = " + rtn + " and coid = " + CoID).AsList();
                //新增日志
                CoreUser.LogComm.InsertUserLogTran(TransComm, "新增快递资料", "Express", "快递名称:"+exp[0].ExpName, UserName, CoID, DateTime.Now);
                //新增缓存
                CacheBase.Set<Express>("express" + rtn, exp[0]);//缓存

                result.d = rtn;  
                TransComm.Commit();        
            }catch(Exception ex){
                TransComm.Rollback();
                TransComm.Dispose();
                result.s = -1;
                result.d = ex.Message;
            }
            finally
            {
                TransComm.Dispose();
                CommDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///查询单笔快递资料
        ///</summary>
        public static DataResult GetExpressEdit(int CoID,int id)
        {
            var result = new DataResult(1,null);
            var parent = CacheBase.Get<ExpressEdit>("express" + id.ToString());  
            if (parent == null)
            {
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try{  
                            string sqlcommand = @"select ID,ExpName,Enable,Priority,PriorityLogistics,PrioritySku,FreightFirst,OrdAmtStart,OrdAmtEnd,IsCOD,LimitedShop,
                                                LimitedWarehouse,DisableArea,DisableSku,IgnoreArrival,ExpCalMethod,UseProbability,OnlineOrder From express 
                                                where coid = " + CoID + " and id = " + id;
                            var u = conn.Query<ExpressEdit>(sqlcommand).AsList();
                            if (u.Count > 0)
                            {
                                CacheBase.Set<ExpressEdit>("express" + id.ToString(),u[0]);
                                result.d = u[0];
                            }        
                        }
                        catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }
            }
            else
            {
                result.d = parent;
            }
            return result;
        }
        ///<summary>
        ///保存快递资料
        ///</summary>
        public static DataResult UpdateExpress(int CoID,ExpressEdit ExpNew,string UserName)
        {
            var result = new DataResult(1,null);
            string contents = string.Empty;
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            CommDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            try{  
                //查询目前资料库资料
                string sqlcommand = @"select * from express where id = " + ExpNew.ID + " and coid = " + CoID;
                var u = CommDBconn.Query<Express>(sqlcommand).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "参数无效";
                    return result;
                }
                int i= 0;
                if(ExpNew.ExpName != u[0].ExpName)
                {
                    contents = contents + "快递名称" + ":" +u[0].ExpName + "=>" + ExpNew.ExpName + ";";
                    u[0].ExpName = ExpNew.ExpName;
                    i ++;
                }
                if(ExpNew.Enable != u[0].Enable)
                {
                    contents = contents + "启用状态" + ":" +u[0].Enable + "=>" + ExpNew.Enable + ";";
                    u[0].Enable = ExpNew.Enable;
                    i ++;
                }
                if(ExpNew.Priority != u[0].Priority)
                {
                    contents = contents + "优先级" + ":" +u[0].Priority + "=>" + ExpNew.Priority + ";";
                    u[0].Priority = ExpNew.Priority;
                    i ++;
                }
                if(ExpNew.PriorityLogistics != u[0].PriorityLogistics)
                {
                    contents = contents + "优先省份" + ":" +u[0].PriorityLogistics + "=>" + ExpNew.PriorityLogistics + ";";
                    u[0].PriorityLogistics = ExpNew.PriorityLogistics;
                    i ++;
                }
                if(ExpNew.PrioritySku != u[0].PrioritySku)
                {
                    contents = contents + "优先商品" + ":" +u[0].PrioritySku + "=>" + ExpNew.PrioritySku + ";";
                    u[0].PrioritySku = ExpNew.PrioritySku;
                    i ++;
                }
                if(ExpNew.FreightFirst != u[0].FreightFirst)
                {
                    contents = contents + "运费优先" + ":" +u[0].FreightFirst + "=>" + ExpNew.FreightFirst + ";";
                    u[0].FreightFirst = ExpNew.FreightFirst;
                    i ++;
                }
                if(ExpNew.OrdAmtStart != u[0].OrdAmtStart)
                {
                    contents = contents + "订单金额大于等于" + ":" +u[0].OrdAmtStart + "=>" + ExpNew.OrdAmtStart + ";";
                    u[0].OrdAmtStart = ExpNew.OrdAmtStart;
                    i ++;
                }
                if(ExpNew.OrdAmtEnd != u[0].OrdAmtEnd)
                {
                    contents = contents + "订单金额小于等于" + ":" +u[0].OrdAmtEnd + "=>" + ExpNew.OrdAmtEnd + ";";
                    u[0].OrdAmtEnd = ExpNew.OrdAmtEnd;
                    i ++;
                }
                if(ExpNew.IsCOD != u[0].IsCOD)
                {
                    contents = contents + "支持货到付款" + ":" +u[0].IsCOD + "=>" + ExpNew.IsCOD + ";";
                    u[0].IsCOD = ExpNew.IsCOD;
                    i ++;
                }
                if(ExpNew.LimitedShop != u[0].LimitedShop)
                {
                    contents = contents + "限定店铺" + ":" +u[0].LimitedShop + "=>" + ExpNew.LimitedShop + ";";
                    u[0].LimitedShop = ExpNew.LimitedShop;
                    i ++;
                }
                if(ExpNew.LimitedWarehouse != u[0].LimitedWarehouse)
                {
                    contents = contents + "限定仓库" + ":" +u[0].LimitedWarehouse + "=>" + ExpNew.LimitedWarehouse + ";";
                    u[0].LimitedWarehouse = ExpNew.LimitedWarehouse;
                    i ++;
                }
                if(ExpNew.DisableArea != u[0].DisableArea)
                {
                    contents = contents + "禁止发送地区" + ":" +u[0].DisableArea + "=>" + ExpNew.DisableArea + ";";
                    u[0].DisableArea = ExpNew.DisableArea;
                    i ++;
                }
                if(ExpNew.DisableSku != u[0].DisableSku)
                {
                    contents = contents + "禁止发送商品" + ":" +u[0].DisableSku + "=>" + ExpNew.DisableSku + ";";
                    u[0].DisableSku = ExpNew.DisableSku;
                    i ++;
                }
                if(ExpNew.IgnoreArrival != u[0].IgnoreArrival)
                {
                    contents = contents + "忽略到达判断" + ":" +u[0].IgnoreArrival + "=>" + ExpNew.IgnoreArrival + ";";
                    u[0].IgnoreArrival = ExpNew.IgnoreArrival;
                    i ++;
                }
                if(ExpNew.ExpCalMethod != u[0].ExpCalMethod)
                {
                    contents = contents + "自动配快递计算方式" + ":" +u[0].ExpCalMethod + "=>" + ExpNew.ExpCalMethod + ";";
                    u[0].ExpCalMethod = ExpNew.ExpCalMethod;
                    i ++;
                }
                if(ExpNew.UseProbability != u[0].UseProbability)
                {
                    contents = contents + "采用概率" + ":" +u[0].UseProbability + "=>" + ExpNew.UseProbability + ";";
                    u[0].UseProbability = ExpNew.UseProbability;
                    i ++;
                }
                if(ExpNew.OnlineOrder != u[0].OnlineOrder)
                {
                    contents = contents + "在线订单" + ":" +u[0].OnlineOrder + "=>" + ExpNew.OnlineOrder + ";";
                    u[0].OnlineOrder = ExpNew.OnlineOrder;
                    i ++;
                }
                if(i > 0)
                {
                    u[0].Modifier = UserName;
                    u[0].ModifyDate = DateTime.Now;
                    sqlcommand = @"update express set ExpName=@ExpName,Enable=@Enable,Priority=@Priority,PriorityLogistics=@PriorityLogistics,PrioritySku=@PrioritySku,
                                   FreightFirst=@FreightFirst,OrdAmtStart=@OrdAmtStart,OrdAmtEnd=@OrdAmtEnd,IsCOD=@IsCOD,LimitedShop=@LimitedShop,DisableSku=@DisableSku,
                                   LimitedWarehouse=@LimitedWarehouse,DisableArea=@DisableArea,IgnoreArrival=@IgnoreArrival,ExpCalMethod=@ExpCalMethod,
                                   UseProbability=@UseProbability,OnlineOrder=@OnlineOrder,Modifier=@Modifier,ModifyDate=@ModifyDate where ID = @ID and CoID = @CoID";
                    int count =CommDBconn.Execute(sqlcommand,u[0],TransComm);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return  result;
                    }
                    //新增日志
                    CoreUser.LogComm.InsertUserLogTran(TransComm, "修改快递资料", "Express", contents, UserName, CoID, DateTime.Now);
                    //新增缓存
                    CacheBase.Set<Express>("express" + u[0].ID.ToString(), u[0]);
                }
                TransComm.Commit();        
            }catch(Exception ex){
                TransComm.Rollback();
                TransComm.Dispose();
                result.s = -1;
                result.d = ex.Message;
            }
            finally
            {
                TransComm.Dispose();
                CommDBconn.Dispose();
            }
            return result;
        }
        ///<summary>
        ///抓取快递List
        ///</summary>
        public static DataResult GetExpressSimple(int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{  
                    //快递公司
                    string sqlcommand = "select ID,ExpName as Name from express where coid =" + CoID + " and enable = true";
                    var Express = conn.Query<ExpressSimple>(sqlcommand).AsList();
                    result.d = Express;   
                    }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            return result;
        }
        // ///<summary>
        // ///查询运费设定资料List
        // ///</summary>
        // public static DataResult GetExpFeeList(int CoID,int id)
        // {
        //     var result = new DataResult(1,null);
        //     var parent = CacheBase.Get<ExpressEdit>("express" + id.ToString());  
        //     using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
        //         try{  
        //                 string sqlcommand = @"";
        //                 var u = conn.Query<ExpressEdit>(sqlcommand).AsList();  
        //             }
        //             catch(Exception ex){
        //             result.s = -1;
        //             result.d = ex.Message;
        //             conn.Dispose();
        //         }
        //     }
        //     return result;
        // }
    }
}
       