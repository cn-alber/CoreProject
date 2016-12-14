using CoreModels;
using MySql.Data.MySqlClient;
using System;
using CoreModels.XyCore;
using Dapper;
using System.Collections.Generic;
// using CoreModels.XyComm;
using static CoreModels.Enum.BatchE;
// using CoreModels.XyUser;
namespace CoreData.CoreCore
{
    public static class BatchHaddle
    {
        ///<summary>
        ///初始资料
        ///</summary>
        public static DataResult GetBatchInit(int CoID)
        {
            var result = new DataResult(1,null);
            var res = new GetBatchInit();
            var filter = new List<Filter>();
            //状态
            foreach (int  myCode in Enum.GetValues(typeof(BatchStatus)))
            {
                var s = new Filter();
                s.value = myCode.ToString();
                s.label = Enum.GetName(typeof(BatchStatus), myCode);//获取名称
                filter.Add(s);
                res.BatchStatus = filter;
            }
            //类型
            filter = new List<Filter>();
            foreach (int  myCode in Enum.GetValues(typeof(BatchType)))
            {
                var s = new Filter();
                s.value = myCode.ToString();
                s.label = Enum.GetName(typeof(BatchType), myCode);//获取名称
                filter.Add(s);
                res.BatchType = filter;
            }
            //任务状态
            filter = new List<Filter>();
            var ff = new Filter();
            ff.value = "N";
            ff.label = "未安排任务";
            filter.Add(ff);
            ff = new Filter();
            ff.value = "Y";
            ff.label = "已安排任务";
            filter.Add(ff);
            res.Task = filter;
            //操作人
            var role = GetWmsRole(CoID).d as List<Filter>;
            var RoleID = new List<int>();
            foreach(var a in role)
            {
                RoleID.Add(int.Parse(a.value));
            }
            var User = GetWmsUser(CoID,RoleID).d as List<Filter>;
            res.Pickor = User;
            result.d = res;
            return result;
        }
        ///<summary>
        ///获取仓库角色资料
        ///</summary>
        public static DataResult GetWmsRole(int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{    
                    string wheresql = "select ID as value,Name as label from role where IsWms = true and CompanyID = " + CoID;
                    var u = conn.Query<Filter>(wheresql).AsList();
                    result.d = u;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
        ///<summary>
        ///获取仓库角色资料
        ///</summary>
        public static DataResult GetWmsUser(int CoID,List<int> RoleID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{    
                    string wheresql = "select ID as value,Account as label from user where Enable = true and CompanyID = " + CoID + " and RoleID in (" + string.Join(",",RoleID) + ")";
                    var u = conn.Query<Filter>(wheresql).AsList();
                    result.d = u;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }           
            return result;
        }
        ///<summary>
        ///查询批次List
        ///</summary>
        public static DataResult GetBatchList(BatchParm cp)
        {
            var result = new DataResult(1,null);    
            string sqlcount = "select count(id) from batch where 1=1";
            string sqlcommand = @"select ID,Type,Pickor,OrderQty,SkuQty,Qty,PickedQty,(Qty - PickedQty - NoQty) as NotPickedQty,NoQty,Status,CreateDate,Mark,MixedPicking,PickingPrint 
                                  from batch where 1=1";
            string wheresql = string.Empty;
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(cp.ID > 0)
            {
                wheresql = wheresql + " and id = " + cp.ID;
            }
            if (cp.Status != null)//状态List
            {
                wheresql = wheresql + " AND status in ("+ string.Join(",", cp.Status) + ")" ;
            }
            if(!string.IsNullOrEmpty(cp.Remark))
            {
               wheresql = wheresql + " and Remark like '%" + cp.Remark + "%'";
            }
            if (cp.PickorID != null)
            {
                wheresql = wheresql + " AND PickorID in ("+ string.Join(",", cp.PickorID) + ")" ;
            }
            if(!string.IsNullOrEmpty(cp.Task) && cp.Task == "N")
            {
                wheresql = wheresql + " AND (PickorID = null || PickorID = 0)";
            }
            if(!string.IsNullOrEmpty(cp.Task) && cp.Task == "Y")
            {
                wheresql = wheresql + " AND PickorID > 0";
            }
            if (cp.Type != null)//状态List
            {
                wheresql = wheresql + " AND Type in ("+ string.Join(",", cp.Type) + ")" ;
            }
            if(cp.DateStart > DateTime.Parse("1900-01-01"))
            {
                 wheresql = wheresql + " AND CreateDate >= '" + cp.DateStart + "'" ;
            }
            if(cp.DateEnd > DateTime.Parse("1900-01-01"))
            {
                wheresql = wheresql + " AND CreateDate <= '" + cp.DateEnd + "'" ;
            }
            var res = new BatchData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    var u = conn.Query<BatchQuery>(sqlcommand + wheresql).AsList();
                    foreach(var a in u)
                    {
                        a.StatusString = Enum.GetName(typeof(BatchStatus), a.Status);
                        a.TypeString = Enum.GetName(typeof(BatchType), a.Type);
                    }
                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Batch = u;
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
        ///获取参数资料
        ///</summary>
        public static DataResult GetConfigure(int CoID,string Type)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    string wheresql = "select * from batch_configure where CoID = " + CoID;
                    var u = conn.Query<BatchConfigure>(wheresql).AsList();
                    if(Type == "A")
                    {
                        result.d = u[0].SingleOrdQty;
                    }
                    if(Type == "B")
                    {
                        result.d = u[0].MultiOrdQty;
                    }
                    if(Type == "C")
                    {
                        result.d = u[0].SingleSkuQty;
                    }
                    if(Type == "D")
                    {
                        result.d = u[0].MultiNotOrdQty;
                    }
                    if(Type == "E")
                    {
                        result.d = u[0].BigQty;
                    }
                    if(Type == "F")
                    {
                        result.d = u[0].Express;
                    }
                    if(Type == "G")
                    {
                        result.d = u[0].Shop;
                    }
                    if(Type == "H")
                    {
                        result.d = u[0].SpecialOrd;
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
        ///更新参数资料
        ///</summary>
        public static DataResult SetConfigure(int CoID,string Type,string TypeValue)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    string wheresql = string.Empty;
                    if(Type == "A")
                    {
                        wheresql = "update batch_configure set SingleOrdQty=@TypeValue where coid = @Coid";
                    }
                    if(Type == "B")
                    {
                        wheresql = "update batch_configure set MultiOrdQty=@TypeValue where coid = @Coid";
                    }
                    if(Type == "C")
                    {
                        wheresql = "update batch_configure set SingleSkuQty=@TypeValue where coid = @Coid";
                    }
                    if(Type == "D")
                    {
                        wheresql = "update batch_configure set MultiNotOrdQty=@TypeValue where coid = @Coid";
                    }
                    if(Type == "E")
                    {
                        wheresql = "update batch_configure set BigQty=@TypeValue where coid = @Coid";
                    }
                    if(Type == "F")
                    {
                        wheresql = "update batch_configure set Express=@TypeValue where coid = @Coid";
                    }
                    if(Type == "G")
                    {
                        wheresql = "update batch_configure set Shop=@TypeValue where coid = @Coid";
                    }
                    if(Type == "H")
                    {
                        wheresql = "update batch_configure set SpecialOrd=@TypeValue where coid = @Coid";
                    }
                    int count = conn.Execute(wheresql,new{TypeValue=TypeValue,Coid=CoID});
                    if(count <= 0)
                    {
                        result.s = -3003;
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
    }
}