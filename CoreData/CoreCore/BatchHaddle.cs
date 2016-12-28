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
            if(!string.IsNullOrEmpty(cp.SortField) && !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new BatchData();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    int count = conn.QueryFirst<int>(sqlcount + wheresql);
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));
                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    Console.WriteLine(sqlcommand + wheresql);
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
        ///<summary>
        ///修改标志
        ///</summary>
        public static DataResult ModifyRemark(int CoID,List<int> ID,string UserName,string Remark)
        {
            var result = new DataResult(1,null);
            var res = new ModifyRemarkReturn();
            var su = new List<ModifyRemarkSuccess>();
            var fa = new List<TransferNormalReturnFail>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select ID,status from batch where id in @ID and coid = @Coid";
                var u = CoreDBconn.Query<Batch>(sqlcommand,new{ID = ID,Coid = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "参数异常";
                    return result;
                }
                foreach(var a in u)
                {
                    if(a.Status != 0)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "等待拣货的批次才可以修改标志!";
                        fa.Add(ff);
                        continue;
                    }
                    sqlcommand = "update batch set mark=@Remark,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,new{Remark=Remark,Modifier=UserName,ModifyDate=DateTime.Now,ID=a.ID,Coid=CoID},TransCore);
                    if(count <= 0)
                    {   
                        result.s = -3003;
                        return result;
                    }
                    var ss = new ModifyRemarkSuccess();
                    ss.ID = a.ID;
                    ss.Remark = Remark;
                    su.Add(ss);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res;
            return result;
        }
        ///<summary>
        ///设定批次标志
        ///</summary>
        public static DataResult ModifyRemarkAll(int CoID,string UserName,string Remark)
        {
            var result = new DataResult(1,null);
            var ID = new List<int>();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{    
                    string sqlcommand = "select ID from batch where status = 0 and coid = " + CoID;
                    var u = conn.Query<Batch>(sqlcommand).AsList();
                    if(u.Count == 0)
                    {
                        result.s = -1;
                        result.d = "没有符合条件的资料需要设定标志";
                        return result;
                    }
                    foreach(var a in u)
                    {
                        ID.Add(a.ID);
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            if(ID.Count > 0)
            {
                var data = ModifyRemark(CoID,ID,UserName,Remark);
                if(data.s == -1)
                {
                    result.s = -1;
                    result.d = data.d;
                }
            }       
            return result;
        }
        ///<summary>
        ///标记拣货单已打印
        ///</summary>
        public static DataResult MarkPrint(int CoID,List<int> ID,string UserName)
        {
            var result = new DataResult(1,null);
            var res = new MarkPrintReturn();
            var su = new List<MarkPrintSuccess>();
            var fa = new List<TransferNormalReturnFail>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select ID,status,PickingPrint from batch where id in @ID and coid = @Coid";
                var u = CoreDBconn.Query<Batch>(sqlcommand,new{ID = ID,Coid = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "参数异常";
                    return result;
                }
                foreach(var a in u)
                {
                    if(a.Status == 6 || a.Status == 7)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "终止拣货&已完成的批次不可标记拣货单已打印!";
                        fa.Add(ff);
                        continue;
                    }
                    if(a.PickingPrint == true)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "已标记拣货单已打印!";
                        fa.Add(ff);
                        continue;
                    }
                    sqlcommand = "update batch set PickingPrint=true,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,new{Modifier=UserName,ModifyDate=DateTime.Now,ID=a.ID,Coid=CoID},TransCore);
                    if(count <= 0)
                    {   
                        result.s = -3003;
                        return result;
                    }
                    var ss = new MarkPrintSuccess();
                    ss.ID = a.ID;
                    ss.PickingPrint = true;
                    su.Add(ss);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res;
            return result;
        }
        ///<summary>
        ///取消标记拣货单已打印
        ///</summary>
        public static DataResult CancleMarkPrint(int CoID,List<int> ID,string UserName)
        {
            var result = new DataResult(1,null);
            var res = new MarkPrintReturn();
            var su = new List<MarkPrintSuccess>();
            var fa = new List<TransferNormalReturnFail>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select ID,status,PickingPrint from batch where id in @ID and coid = @Coid";
                var u = CoreDBconn.Query<Batch>(sqlcommand,new{ID = ID,Coid = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "参数异常";
                    return result;
                }
                foreach(var a in u)
                {
                    if(a.Status == 6 || a.Status == 7)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "终止拣货&已完成的批次不可取消标记拣货单已打印!";
                        fa.Add(ff);
                        continue;
                    }
                    if(a.PickingPrint == false)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "拣货单已打印未标记!";
                        fa.Add(ff);
                        continue;
                    }
                    sqlcommand = "update batch set PickingPrint=false,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,new{Modifier=UserName,ModifyDate=DateTime.Now,ID=a.ID,Coid=CoID},TransCore);
                    if(count <= 0)
                    {   
                        result.s = -3003;
                        return result;
                    }
                    var ss = new MarkPrintSuccess();
                    ss.ID = a.ID;
                    ss.PickingPrint = false;
                    su.Add(ss);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res;
            return result;
        }
        ///<summary>
        ///拣货人员安排初始资料
        ///</summary>
        public static DataResult GetPickorInit(int CoID)
        {
            var result = new DataResult(1,null);
            var res = new GetPickorInit();
            var Role = GetWmsRole(CoID);
            if(Role.s == 1)
            {
                res.Role = Role.d as List<Filter>;
                var RoleID = new List<int>();
                foreach(var a in res.Role)
                {
                    RoleID.Add(int.Parse(a.value));
                }
                var User = GetWmsUser(CoID,RoleID).d as List<Filter>;
                res.Pickor = User;
            }
            result.d = res;
            return result;
        }
        ///<summary>
        ///根据角色过滤拣货人员
        ///</summary>
        public static DataResult GetPickorByRole(int CoID,int RoleID)
        {
            var result = new DataResult(1,null);
            var RoleIDLst = new List<int>();
            if(RoleID == 0)
            {
                var Role = GetWmsRole(CoID).d as List<Filter>;
                foreach(var a in Role)
                {
                    RoleIDLst.Add(int.Parse(a.value));
                }
            }
            else
            {
                RoleIDLst.Add(RoleID);
            }
            var User = GetWmsUser(CoID,RoleIDLst).d as List<Filter>;
            result.d = User;
            return result;
        }
        ///<summary>
        ///设定拣货人员
        ///</summary>
        public static DataResult SetPickor(int CoID,List<int> ID,List<int> Pickor,string UserName)
        {
            var result = new DataResult(1,null);
            var User = new List<Filter>();
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{     
                    foreach(var a in Pickor)
                    {
                        string wheresql = "select Account from user where Enable = true and CompanyID = " + CoID + " and ID =" + a;
                        var u = conn.Query<string>(wheresql).AsList();
                        if(u.Count > 0)
                        {
                            var ff = new Filter();
                            ff.value = a.ToString();
                            ff.label = u[0];
                            User.Add(ff);
                        }
                    }           
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }      
            if(User.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合条件的拣货员";
                return result;
            }   
            var res = new SetPickorReturn();
            var su = new List<SetPickorSuccess>();
            var fa = new List<TransferNormalReturnFail>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select ID,status,PickorID,Pickor from batch where id in @ID and coid = @Coid";
                var u = CoreDBconn.Query<Batch>(sqlcommand,new{ID = ID,Coid = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "参数异常";
                    return result;
                }
                int i = 0;
                foreach(var a in u)
                {
                    if(a.Status != 0)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "等待拣货的批次才可以安排拣货员";
                        fa.Add(ff);
                        continue;
                    }
                    if(!string.IsNullOrEmpty(a.Pickor))
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "未安排拣货员的批次才可以操作";
                        fa.Add(ff);
                        continue;
                    }
                    string PickorID = User[i].value;
                    string PickorName = User[i].label;
                    sqlcommand = "update batch set PickorID=@PickorID,Pickor=@Pickor,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,new{PickorID =PickorID, Pickor=PickorName,Modifier=UserName,ModifyDate=DateTime.Now,ID=a.ID,Coid=CoID},TransCore);
                    if(count <= 0)
                    {   
                        result.s = -3003;
                        return result;
                    }
                    var ss = new SetPickorSuccess();
                    ss.ID = a.ID;
                    ss.Pickor = PickorName;
                    su.Add(ss);
                    i ++;
                    if(i == User.Count)
                    {
                        i = 0;
                    }
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res;
            return result;
        }
        ///<summary>
        ///重新安排拣货人员
        ///</summary>
        public static DataResult ReSetPickor(int CoID,List<int> ID,List<int> Pickor,string UserName)
        {
            var result = new DataResult(1,null);
            var User = new List<Filter>();
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try{     
                    foreach(var a in Pickor)
                    {
                        string wheresql = "select Account from user where Enable = true and CompanyID = " + CoID + " and ID =" + a;
                        var u = conn.Query<string>(wheresql).AsList();
                        if(u.Count > 0)
                        {
                            var ff = new Filter();
                            ff.value = a.ToString();
                            ff.label = u[0];
                            User.Add(ff);
                        }
                    }           
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }      
            if(User.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合条件的拣货员";
                return result;
            }   
            var res = new SetPickorReturn();
            var su = new List<SetPickorSuccess>();
            var fa = new List<TransferNormalReturnFail>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                string sqlcommand = "select ID,status,PickorID,Pickor from batch where id in @ID and coid = @Coid";
                var u = CoreDBconn.Query<Batch>(sqlcommand,new{ID = ID,Coid = CoID}).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "参数异常";
                    return result;
                }
                int i = 0;
                foreach(var a in u)
                {
                    if(a.Status == 6 || a.Status == 7)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "终止拣货/已完成的批次不可以重新安排拣货人";
                        fa.Add(ff);
                        continue;
                    }
                    if(string.IsNullOrEmpty(a.Pickor))
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "未安排拣货员的批次不可以安排拣货人";
                        fa.Add(ff);
                        continue;
                    }
                    string PickorID = User[i].value;
                    string PickorName = User[i].label;
                    sqlcommand = "update batch set PickorID=@PickorID,Pickor=@Pickor,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    int count = CoreDBconn.Execute(sqlcommand,new{PickorID =PickorID, Pickor=PickorName,Modifier=UserName,ModifyDate=DateTime.Now,ID=a.ID,Coid=CoID},TransCore);
                    if(count <= 0)
                    {   
                        result.s = -3003;
                        return result;
                    }
                    var ss = new SetPickorSuccess();
                    ss.ID = a.ID;
                    ss.Pickor = PickorName;
                    su.Add(ss);
                    i ++;
                    if(i == User.Count)
                    {
                        i = 0;
                    }
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res;
            return result;
        }
        ///<summary>
        ///获取一单一件，一单多件，大单的订单数量
        ///</summary>
        public static DataResult GetOrdCount(int CoID)
        {
            var result = new DataResult(1,null);    
            var res = new GetOrdCountReturn();
            int bigord = int.Parse(GetConfigure(CoID,"E").d.ToString());
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select count(id) from saleout where status = 0 and BatchID = 0 and OrdQty = 1 and ExpName != '现场取货' and coid = " + CoID;
                    int count = conn.QueryFirst<int>(sqlcommand);
                    res.SingleOrd = count;
                    if(bigord == 0)
                    {
                        sqlcommand = @"select count(id) from saleout where status = 0 and BatchID = 0 and OrdQty > 1 and ExpName != '现场取货' and coid = " + CoID;
                    }
                    else
                    {
                        sqlcommand = @"select count(id) from saleout where status = 0 and BatchID = 0 and OrdQty > 1 and OrdQty < " + bigord + " and ExpName != '现场取货' and coid = " + CoID;
                    }
                    count = conn.QueryFirst<int>(sqlcommand);
                    res.MultiOrd = count;
                    if(bigord == 0)
                    {
                        sqlcommand = @"select count(id) from saleout where status = 0 and BatchID = 0 and ExpName = '现场取货' and coid = " + CoID;
                    }
                    else
                    {
                        sqlcommand = @"select count(id) from saleout where status = 0 and BatchID = 0 and (ExpName = '现场取货' or OrdQty >= " + bigord + ") and coid = " + CoID;
                    }
                    count = conn.QueryFirst<int>(sqlcommand);
                    res.BigOrd = count;

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
        ///一单一件
        ///</summary>
        public static DataResult SingleOrd(int CoID,List<int> ID,string UserName)
        {
            var result = new DataResult(1,null);  
            var SkuList = new List<Sku>();
            string sqlcommand = string.Empty;
            int SingleOrdQty = int.Parse(GetConfigure(CoID,"A").d.ToString());
            int SingleSkuQty = int.Parse(GetConfigure(CoID,"C").d.ToString());
            int count  = 0;
            int rtn = 0;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    sqlcommand = @"select distinct saleoutitem.SkuAutoID,saleoutitem.SkuID,saleoutitem.SkuName from saleoutitem,saleout where saleoutitem.SID = saleout.ID and 
                                   saleout.status = 0 and saleoutitem.BatchID = 0 and saleout.OrdQty = 1 and saleout.ExpName != '现场取货' and saleout.Coid = " + CoID + 
                                   " and saleout.ID in (" + string.Join(",",ID) + ") and saleoutitem.isgift = false"; 
                    SkuList = conn.Query<Sku>(sqlcommand).AsList();
                    if(SkuList.Count == 0)
                    {
                        result.s = -1;
                        result.d = "没有符合条件的一单一件资料";
                        return result;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }   
            int i = 0;//计算商品数
            int j = 0;//订单数     
            foreach(var a in SkuList)
            {
                var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
                CoreDBconn.Open();
                var TransCore = CoreDBconn.BeginTransaction();
                try
                {
                    //抓取库存
                    sqlcommand = @"select ID,Qty - lockqty as Qty,PCode,`Order` from wmspile where Skuautoid = " + a.SkuAutoID + " and Type in (1,2) and Enable = true and CoID = " + CoID + 
                                  " and Qty > lockqty order by Type,`Order`";
                    var invqty = CoreDBconn.Query<InvQty>(sqlcommand).AsList();
                    if(invqty.Count == 0) 
                    {
                        continue;
                    }
                    int totqty = 0;
                    foreach(var inv in invqty)
                    {
                        totqty = totqty + inv.Qty;
                    }
                    //抓取符合条件的订单
                    sqlcommand = @"select saleout.ID from saleoutitem,saleout where saleoutitem.SID = saleout.ID and saleout.status = 0 
                                   and saleoutitem.BatchID = 0 and saleout.OrdQty = 1 and saleout.ExpName != '现场取货' and saleout.Coid = " + CoID + 
                                   " and saleout.ID in (" + string.Join(",",ID) + ") and saleoutitem.isgift = false and saleoutitem.SkuAutoID = " + a.SkuAutoID; 
                    var order = CoreDBconn.Query<int>(sqlcommand).AsList();
                    int invcount = order.Count;
                    var sale = new List<int>();
                    if(j + invcount > SingleOrdQty)
                    {
                        foreach(var o in order)
                        {
                            sale.Add(o);
                            if(j + sale.Count == SingleOrdQty) break;
                        }
                        order = sale;
                        invcount = order.Count;
                    }
                    sale = new List<int>();
                    if(totqty >= invcount)
                    {
                        sale = order;
                    }
                    else
                    {
                        foreach(var o in order)
                        {
                            sale.Add(o);
                            if(sale.Count == totqty) break;
                        }
                    }
                    invcount = sale.Count;
                    //产生批次资料
                    if(rtn == 0)
                    {
                        sqlcommand = @"INSERT INTO batch(Type,OrderQty,SkuQty,Qty,CoID,Creator,Modifier) 
                                                  VALUES(@Type,@OrderQty,@SkuQty,@OrderQty,@CoID,@Creator,@Creator)";
                        count = CoreDBconn.Execute(sqlcommand,new{Type=0,OrderQty = invcount,SkuQty=1,CoID=CoID,Creator=UserName},TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    }
                    else
                    {
                        sqlcommand = @"update batch set OrderQty=OrderQty + @Qty,SkuQty=SkuQty+ 1,Qty=Qty + @Qty,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID";
                        count = CoreDBconn.Execute(sqlcommand,new{Qty = invcount,ModifyDate = DateTime.Now,Modifier=UserName,ID = rtn},TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }
                    foreach(var inv in invqty)
                    {
                        int taskqty = 0;
                        if(inv.Qty >= invcount)
                        {
                            taskqty = invcount;
                            invcount = 0;
                        }
                        else
                        {
                            taskqty = inv.Qty;
                            invcount = invcount - taskqty;
                        }
                        //产生批次任务
                        sqlcommand = @"INSERT INTO batchtask(BatchID,Skuautoid,SkuID,SkuName,CoID,PCode,Qty,`Index`) 
                                                  VALUES(@BatchID,@Skuautoid,@SkuID,@SkuName,@CoID,@PCode,@Qty,@Index)";
                        count = CoreDBconn.Execute(sqlcommand,new{BatchID=rtn,Skuautoid=a.SkuAutoID,SkuID=a.SkuID,SkuName=a.SkuName,PCode=inv.PCode,Qty=taskqty,Index =inv.Order,CoID=CoID},TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        //更新库存
                        sqlcommand = @"update wmspile set lockqty=lockqty + @Qty where id = @ID";
                        count = CoreDBconn.Execute(sqlcommand,new{Qty = taskqty,ID = inv.ID},TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                        if(invcount == 0) break;
                    }
                    //更新出库单
                    sqlcommand = @"update saleout set BatchID=@BatchID,Modifier=@Modifier,ModifyDate=@ModifyDate where id in @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{BatchID = rtn,ModifyDate = DateTime.Now,Modifier=UserName,ID = sale},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    sqlcommand = @"update saleoutitem set BatchID=@BatchID,Modifier=@Modifier,ModifyDate=@ModifyDate where sid in @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{BatchID = rtn,ModifyDate = DateTime.Now,Modifier=UserName,ID = sale},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    sqlcommand = "select id,oid from saleout where id in @ID and coid = @Coid";
                    var l = CoreDBconn.Query<SaleOutInsert>(sqlcommand,new{ID = sale,Coid = CoID}).AsList();
                    foreach(var ll in l)
                    {
                        sqlcommand = @"INSERT INTO batch_log(BatchID,SaleID,Operate,UniqueCode,CoID,Creator) 
                                                  VALUES(@BatchID,@SaleID,@Operate,@UniqueCode,@CoID,@Creator)";
                        count = CoreDBconn.Execute(sqlcommand,new{BatchID=rtn,SaleID = ll.ID,Operate="绑定批次",UniqueCode="订单:" + ll.OID,CoID=CoID,Creator=UserName},TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                    }
                    i ++;
                    j = j + sale.Count;
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
                if(j == SingleOrdQty) break;
                if(SingleSkuQty > 0 && i == SingleSkuQty) break;
            }    
            return result;  
        }
        ///<summary>
        ///一单一件批次产生
        ///</summary>
        public static DataResult SetSingleOrd(int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var ID = new List<int>();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select saleout.ID from saleout where status = 0 and BatchID = 0 and OrdQty = 1 and ExpName != '现场取货' and coid = " + CoID;
                    ID = conn.Query<int>(sqlcommand).AsList();         
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            if(ID.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合的条件资料生成批次!";
                return result;
            }
            int SingleOrdQty = int.Parse(GetConfigure(CoID,"A").d.ToString());
            var sinID = new List<int>();
            foreach(var a in ID)
            {
                sinID.Add(a);
                if(sinID.Count == SingleOrdQty)
                {
                    result = SingleOrd(CoID,sinID,UserName);
                    if(result.s == -1)
                    {
                        return result;
                    }
                    sinID = new List<int>();
                }
            }
            if(sinID.Count > 0)
            {
                result = SingleOrd(CoID,sinID,UserName);
            }
            return result;
        }
        ///<summary>
        ///批次策略查询List
        ///</summary>
        public static DataResult GetStrategySimple(int CoID,int Type)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select ID,StrategyName from batchstrategy where Type = " + Type + " and coid = " + CoID;
                    var s = conn.Query<StrategyList>(sqlcommand).AsList();
                    result.d = s;             
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }   
            return result;
        }
        ///<summary>
        ///新增批次策略
        ///</summary>
        public static DataResult InsertStrategy(BatchStrategy strategy)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"INSERT INTO batchstrategy(Type,StrategyName,SkuIn,SkuNotIn,OrdGift,KindIDIn,PCodeIn,ExpPrint,ExpressIn,DistributorIn,ShopIn,AmtMin,AmtMax,
                                                                    PayDateStart,PayDateEnd,RecMessage,SendMessage,PrioritySku,OrdQty,CoID) 
                                          VALUES(@Type,@StrategyName,@SkuIn,@SkuNotIn,@OrdGift,@KindIDIn,@PCodeIn,@ExpPrint,@ExpressIn,@DistributorIn,@ShopIn,@AmtMin,@AmtMax,
                                                 @PayDateStart,@PayDateEnd,@RecMessage,@SendMessage,@PrioritySku,@OrdQty,@CoID)";     
                    int count = conn.Execute(sqlcommand,strategy);
                    if(count <= 0)
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
            result = GetStrategySimple(strategy.CoID,strategy.Type);
            return result;
        }
        ///<summary>
        ///查询单笔策略资料
        ///</summary>
        public static DataResult GetStrategyEdit(int id,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select * from batchstrategy where id = " + id + " and coid = " + CoID;     
                    var u = conn.Query<BatchStrategy>(sqlcommand).AsList();
                    if(u.Count > 0)
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
        ///修改策略资料
        ///</summary>
        public static DataResult UpdateStrategy(int id,int CoID,string StrategyName,string SkuIn,string SkuNotIn,string OrdGift,string KindIDIn,string PCodeIn,string ExpPrint,
                                                string ExpressIn,string DistributorIn,string ShopIn,string AmtMin,string AmtMax,string PayDateStart,string PayDateEnd,
                                                string RecMessage,string SendMessage,string PrioritySku,string OrdQty)
        {
            var result = new DataResult(1,null);
            var strategy = GetStrategyEdit(id,CoID).d as BatchStrategy;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = "update batchstrategy set ";
                    int i = 0;
                    if(StrategyName != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "StrategyName=@StrategyName,";
                        strategy.StrategyName = StrategyName;
                    }
                    if(SkuIn != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "SkuIn=@SkuIn,";
                        strategy.SkuIn = SkuIn;
                    }
                    if(SkuNotIn != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "SkuNotIn=@SkuNotIn,";
                        strategy.SkuNotIn = SkuNotIn;
                    }
                    if(OrdGift != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "OrdGift=@OrdGift,";
                        strategy.OrdGift = int.Parse(OrdGift);
                    }
                    if(KindIDIn != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "KindIDIn=@KindIDIn,";
                        strategy.KindIDIn = KindIDIn;
                    }
                    if(PCodeIn != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "PCodeIn=@PCodeIn,";
                        strategy.PCodeIn = PCodeIn;
                    }
                    if(ExpPrint != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "ExpPrint=@ExpPrint,";
                        strategy.ExpPrint = int.Parse(ExpPrint);
                    }
                    if(ExpressIn != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "ExpressIn=@ExpressIn,";
                        strategy.ExpressIn = ExpressIn;
                    }
                    if(DistributorIn != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "DistributorIn=@DistributorIn,";
                        strategy.DistributorIn = DistributorIn;
                    }
                    if(ShopIn != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "ShopIn=@ShopIn,";
                        strategy.ShopIn = ShopIn;
                    }
                    if(AmtMin != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "AmtMin=@AmtMin,";
                        strategy.AmtMin = AmtMin;
                    }
                    if(AmtMax != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "AmtMax=@AmtMax,";
                        strategy.AmtMax = AmtMax;
                    }
                    if(PayDateStart != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "PayDateStart=@PayDateStart,";
                        strategy.PayDateStart = PayDateStart;
                    }
                    if(PayDateEnd != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "PayDateEnd=@PayDateEnd,";
                        strategy.PayDateEnd = PayDateEnd;
                    }
                    if(RecMessage != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "RecMessage=@RecMessage,";
                        strategy.RecMessage = RecMessage;
                    }
                    if(SendMessage != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "SendMessage=@SendMessage,";
                        strategy.SendMessage = SendMessage;
                    }
                    if(PrioritySku != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "PrioritySku=@PrioritySku,";
                        strategy.PrioritySku = PrioritySku;
                    }
                    if(OrdQty != null)
                    {
                        i ++;
                        sqlcommand = sqlcommand + "OrdQty=@OrdQty,";
                        strategy.OrdQty = OrdQty;
                    }
                    if(i > 0)
                    {
                        sqlcommand = sqlcommand.Substring(0,sqlcommand.Length - 1);
                        sqlcommand = sqlcommand + " where id = @ID and coid = @Coid";
                        int count = conn.Execute(sqlcommand,strategy);
                        if(count <= 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }   
            result = GetStrategySimple(strategy.CoID,strategy.Type);
            return result;
        }
        ///<summary>
        ///删除批次策略
        ///</summary>
        public static DataResult DeleteStrategy(int id,int CoID,int Type)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"delete from batchstrategy where id = " + id + " and coid = " + CoID;     
                    int count = conn.Execute(sqlcommand);
                    if(count < 0)
                    {
                        result.s = -3004;
                        return result;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }   
            result = GetStrategySimple(CoID,Type);
            return result;
        }
        ///<summary>
        ///一单一件策略生成
        ///</summary>
        public static DataResult SetSingleOrdStrategy(int CoID,string UserName,int id)
        {
            var result = new DataResult(1,null);
            var strategy = GetStrategyEdit(id,CoID).d as BatchStrategy;
            var ID = new List<int>();
            string shop = GetConfigure(CoID,"G").d.ToString();
            string exp = GetConfigure(CoID,"F").d.ToString();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select saleout.ID from saleout where status = 0 and BatchID = 0 and OrdQty = 1 and ExpName != '现场取货' and coid = " + CoID;
                    if(!string.IsNullOrEmpty(strategy.AmtMin))
                    {
                        sqlcommand = sqlcommand + " and Amount >= " + strategy.AmtMin;
                    }
                    if(!string.IsNullOrEmpty(strategy.AmtMax))
                    {
                        sqlcommand = sqlcommand + " and Amount <= " + strategy.AmtMax;
                    }
                    if(!string.IsNullOrEmpty(strategy.PayDateStart) && DateTime.Parse(strategy.PayDateStart) > DateTime.Parse("1900-01-01"))
                    {
                        sqlcommand = sqlcommand + " and PayDate <= '" + strategy.PayDateStart + "'";
                    }
                    if(!string.IsNullOrEmpty(strategy.PayDateEnd) && DateTime.Parse(strategy.PayDateEnd) > DateTime.Parse("1900-01-01"))
                    {
                        sqlcommand = sqlcommand + " and PayDate >= '" + strategy.PayDateEnd + "'";
                    }
                    if(!string.IsNullOrEmpty(strategy.RecMessage))
                    {
                        sqlcommand = sqlcommand + " and RecMessage like '%" + strategy.RecMessage + "%'";
                    }
                    if(!string.IsNullOrEmpty(strategy.SendMessage))
                    {
                        sqlcommand = sqlcommand + " and SendMessage like '%" + strategy.SendMessage + "%'";
                    }
                    if(!string.IsNullOrEmpty(strategy.ShopIn))
                    {
                        if(strategy.ShopIn == "B")
                        {
                            strategy.ShopIn = shop;
                        }
                        if(strategy.ShopIn != "A")
                        {
                            sqlcommand = sqlcommand + " and ShopID in (" + strategy.ShopIn + ")";
                        }
                    }
                    if(!string.IsNullOrEmpty(strategy.ExpressIn))
                    {
                        if(strategy.ExpressIn == "B")
                        {
                            strategy.ExpressIn = exp;
                        }
                        if(strategy.ExpressIn != "A")
                        {
                            sqlcommand = sqlcommand + " and ExID in (" + strategy.ExpressIn + ")";
                        }
                    }
                    if(strategy.ExpPrint == 1)
                    {
                        sqlcommand = sqlcommand + " and IsExpPrint == false";
                    }
                    if(strategy.ExpPrint == 2)
                    {
                        sqlcommand = sqlcommand + " and IsExpPrint == true";
                    }
                    if(!string.IsNullOrEmpty(strategy.DistributorIn))
                    {
                        string[] a = strategy.DistributorIn.Split(',');
                        List<string> dis = new List<string>();
                        bool isflag = false;
                        foreach(var aa in a)
                        {
                            if(aa == "0")
                            {
                                isflag = true;
                            }
                            else
                            {
                                string name = DistributorHaddle.getDisName(CoID.ToString(),aa);
                                dis.Add(name);
                            }
                        }
                        if(isflag == true && dis.Count == 0)
                        {
                            sqlcommand = sqlcommand + " and Distributor == ''";
                        }
                        if(isflag == true && dis.Count > 0)
                        {
                            string distributor = string.Empty;
                            foreach(var x in dis)
                            {
                                distributor = distributor + "'" + x + "',";
                            }
                            distributor = distributor.Substring(0,distributor.Length - 1);
                            sqlcommand = sqlcommand + " and (Distributor == '' or Distributor in (" + distributor + "))";
                        }
                        if(isflag == false && dis.Count > 0)
                        {
                            string distributor = string.Empty;
                            foreach(var x in dis)
                            {
                                distributor = distributor + "'" + x + "',";
                            }
                            distributor = distributor.Substring(0,distributor.Length - 1);
                            sqlcommand = sqlcommand + " and Distributor in (" + distributor + ")";
                        }
                    }
                    ID = conn.Query<int>(sqlcommand).AsList();    
                    if(ID.Count == 0)
                    {
                        result.s = -1;
                        result.d = "没有符合的条件资料生成批次!";
                        return result;
                    }   
                    if(!string.IsNullOrEmpty(strategy.SkuIn) || !string.IsNullOrEmpty(strategy.SkuNotIn) || strategy.OrdGift != 0 || 
                       !string.IsNullOrEmpty(strategy.KindIDIn) || !string.IsNullOrEmpty(strategy.PCodeIn))  
                    {
                        var NID = new List<int>();
                        foreach(var a in ID)
                        {
                            sqlcommand = "select count(id) from saleoutitem where sid = " + a + " and coid = " + CoID + " and isgift = true";
                            int count = conn.QueryFirst<int>(sqlcommand);    
                            if(strategy.OrdGift == 1 && count == 0) continue;
                            if(strategy.OrdGift == 2 && count > 0) continue;
                            sqlcommand = "select SkuAutoID,SkuID from saleoutitem where sid = " + a + " and coid = " + CoID + " and isgift = false";
                            var sku = conn.Query<SaleOutItemInsert>(sqlcommand).AsList();  
                            if(!string.IsNullOrEmpty(strategy.SkuIn))
                            {
                                int i = 0;
                                foreach(var s in sku)
                                {
                                    if(strategy.SkuIn.Contains(s.SkuID))
                                    {
                                        i ++;
                                        break;
                                    }
                                }
                                if(i == 0) continue;
                            } 
                            if(!string.IsNullOrEmpty(strategy.SkuNotIn))
                            {
                                int i = 0;
                                foreach(var s in sku)
                                {
                                    if(strategy.SkuNotIn.Contains(s.SkuID))
                                    {
                                        i ++;
                                        break;
                                    }
                                }
                                if(i > 0) continue;
                            } 
                            if(!string.IsNullOrEmpty(strategy.KindIDIn))
                            {
                                var skuid = new List<int>();
                                foreach(var s in sku)
                                {
                                    skuid.Add(s.SkuAutoID);
                                }
                                sqlcommand = @"select count(id) from coresku where id in (" + string.Join(",",skuid) + ") and KindID not in (" + strategy.KindIDIn + ")" + 
                                              " and coid =" + CoID;
                                count = conn.QueryFirst<int>(sqlcommand);  
                                if(count > 0) continue;
                            } 
                            if(!string.IsNullOrEmpty(strategy.PCodeIn))
                            {
                                var skuid = new List<int>();
                                foreach(var s in sku)
                                {
                                    skuid.Add(s.SkuAutoID);
                                }
                                string[] pcode = strategy.PCodeIn.Split(',');
                                string wheresql = "";
                                foreach(var p in pcode)
                                {
                                    wheresql = wheresql + "PCode like '%" + p + "%' or";
                                }
                                wheresql = "and (" + wheresql.Substring(0,wheresql.Length - 3) + ")";
                                sqlcommand = @"select count(id) from wmspile where Skuautoid in (" + string.Join(",",skuid) + ") and coid =" + CoID + wheresql + " and Qty <= lockqty";
                                count = conn.QueryFirst<int>(sqlcommand);  
                                if(count > 0) continue;
                            } 
                            NID.Add(a);
                        }
                        ID = NID;
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            if(ID.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合的条件资料生成批次!";
                return result;
            }
            int SingleOrdQty = int.Parse(GetConfigure(CoID,"A").d.ToString());
            var sinID = new List<int>();
            foreach(var a in ID)
            {
                sinID.Add(a);
                if(sinID.Count == SingleOrdQty)
                {
                    result = SingleOrd(CoID,sinID,UserName);
                    if(result.s == -1)
                    {
                        return result;
                    }
                    sinID = new List<int>();
                }
            }
            if(sinID.Count > 0)
            {
                result = SingleOrd(CoID,sinID,UserName);
            }
            return result;
        }
        ///<summary>
        ///一单多件
        ///</summary>
        public static DataResult MultiOrd(int CoID,List<int> ID,string UserName)
        {
            var result = new DataResult(1,null);  
            string sqlcommand = string.Empty;
            int count  = 0;
            int rtn = 0;  
            foreach(var a in ID)
            {
                var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
                CoreDBconn.Open();
                var TransCore = CoreDBconn.BeginTransaction();
                try
                {
                    //产生批次资料
                    if(rtn == 0)
                    {
                        sqlcommand = @"INSERT INTO batch(Type,OrderQty,SkuQty,Qty,CoID,Creator,Modifier) 
                                                VALUES(@Type,@OrderQty,@SkuQty,@Qty,@CoID,@Creator,@Creator)";
                        count = CoreDBconn.Execute(sqlcommand,new{Type=1,OrderQty=0,SkuQty=0,Qty=0,CoID=CoID,Creator=UserName},TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
                        rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    }
                    //抓取出库明细
                    sqlcommand = "select * from saleoutitem where sid = " + a + " and coid = " + CoID + " and isgift = false";
                    var item = CoreDBconn.Query<SaleOutItemInsert>(sqlcommand).AsList();
                    bool isflag = false;
                    int i = 0;//计算商品总数
                    int j = 0;//商品种类数
                    foreach(var it in item)
                    {                        
                        //抓取库存
                        sqlcommand = @"select ID,Qty - lockqty as Qty,PCode,`Order` from wmspile where Skuautoid = " + it.SkuAutoID + " and Type in (1,2) and Enable = true and CoID = " + CoID + 
                                    " and Qty > lockqty order by Type,`Order`";
                        var invqty = CoreDBconn.Query<InvQty>(sqlcommand).AsList();
                        if(invqty.Count == 0) 
                        {
                            isflag = true;
                            break;
                        }
                        int totqty = 0;
                        foreach(var inv in invqty)
                        {
                            totqty = totqty + inv.Qty;
                        }
                        if(totqty < it.Qty)
                        {
                            isflag = true;
                            break;
                        }
                        int saleqty = it.Qty;
                        int p = 0;
                        foreach(var inv in invqty)
                        {
                            int taskqty = 0;
                            if(inv.Qty >= it.Qty)
                            {
                                taskqty = saleqty;
                                saleqty = 0;
                            }
                            else
                            {
                                taskqty = inv.Qty;
                                saleqty = saleqty - taskqty;
                            }
                            //产生批次任务
                            sqlcommand = "select count(*) from batchtask where BatchID = " + rtn + " and Skuautoid = " + it.SkuAutoID + " and CoID = " + CoID + " and PCode = '" + inv.PCode + "'";
                            count = CoreDBconn.QueryFirst<int>(sqlcommand);
                            if(count == 0)
                            {
                                sqlcommand = @"INSERT INTO batchtask(BatchID,Skuautoid,SkuID,SkuName,CoID,PCode,Qty,`Index`) 
                                                    VALUES(@BatchID,@Skuautoid,@SkuID,@SkuName,@CoID,@PCode,@Qty,@Index)";
                                count = CoreDBconn.Execute(sqlcommand,new{BatchID=rtn,Skuautoid=it.SkuAutoID,SkuID=it.SkuID,SkuName=it.SkuName,PCode=inv.PCode,Qty=taskqty,Index =inv.Order,CoID=CoID},TransCore);
                                if(count < 0)
                                {
                                    result.s = -3002;
                                    return result;
                                }
                            }
                            else
                            {
                                sqlcommand = @"update batchtask set Qty = Qty + " + taskqty + " where BatchID = " + rtn + " and Skuautoid = " + it.SkuAutoID + " and CoID = " + CoID + 
                                              " and PCode = '" + inv.PCode + "'";
                                count = CoreDBconn.Execute(sqlcommand,TransCore);
                                if(count < 0)
                                {
                                    result.s = -3003;
                                    return result;
                                }
                                p ++ ;
                            }
                            //更新库存
                            sqlcommand = @"update wmspile set lockqty=lockqty + @Qty where id = @ID";
                            count = CoreDBconn.Execute(sqlcommand,new{Qty = taskqty,ID = inv.ID},TransCore);
                            if(count < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                            if(saleqty == 0) break;
                        }
                        if(p == 0)
                        {
                            j ++;
                        }
                        i = i + it.Qty;
                    }
                    if(isflag == true) continue;
                    //更新批次资料
                    sqlcommand = @"update batch set OrderQty=OrderQty + 1,SkuQty=SkuQty+ @SkuQty,Qty=Qty + @Qty,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{SkuQty = j,Qty = i,ModifyDate = DateTime.Now,Modifier=UserName,ID = rtn},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    //抓取分拣格
                    sqlcommand = @"select sortcode from batchsort where CoID = 1 and sortcode not in (select distinct SortCode from saleout 
                                   where (SortCode != null or SortCode != '') and `Status` not in (6,7))";
                    var sort = CoreDBconn.Query<string>(sqlcommand).AsList();
                    if(sort.Count == 0)
                    {
                        result.s = -1;
                        result.d = "分拣格异常!";
                        return result;
                    }              
                    //更新出库单
                    sqlcommand = @"update saleout set BatchID=@BatchID,SortCode=@SortCode,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{BatchID = rtn,SortCode=sort[0],ModifyDate = DateTime.Now,Modifier=UserName,ID = a},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    sqlcommand = @"update saleoutitem set BatchID=@BatchID,Modifier=@Modifier,ModifyDate=@ModifyDate where sid = @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{BatchID = rtn,ModifyDate = DateTime.Now,Modifier=UserName,ID = a},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    sqlcommand = "select id,oid from saleout where id = @ID and coid = @Coid";
                    var l = CoreDBconn.Query<SaleOutInsert>(sqlcommand,new{ID = a,Coid = CoID}).AsList();
                    foreach(var ll in l)
                    {
                        sqlcommand = @"INSERT INTO batch_log(BatchID,SaleID,Operate,UniqueCode,CoID,Creator) 
                                                  VALUES(@BatchID,@SaleID,@Operate,@UniqueCode,@CoID,@Creator)";
                        count = CoreDBconn.Execute(sqlcommand,new{BatchID=rtn,SaleID = ll.ID,Operate="绑定批次",UniqueCode="订单:" + ll.OID,CoID=CoID,Creator=UserName},TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
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
            }    
            return result;  
        }
        ///<summary>
        ///一单多件批次产生
        ///</summary>
        public static DataResult SetMultiOrd(int CoID,string UserName)
        {
            var result = new DataResult(1,null);
            var ID = new List<int>();
            int bigord = int.Parse(GetConfigure(CoID,"E").d.ToString());
            string sqlcommand= string.Empty;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    if(bigord == 0)
                    {
                        sqlcommand = @"select saleout.ID from saleout where status = 0 and BatchID = 0 and OrdQty > 1 and ExpName != '现场取货' and coid = " + CoID;
                    }
                    else
                    {
                        sqlcommand = @"select saleout.ID from saleout where status = 0 and BatchID = 0 and OrdQty > 1 and OrdQty < " + bigord + " and ExpName != '现场取货' and coid = " + CoID;
                    }
                    ID = conn.Query<int>(sqlcommand).AsList();         
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            if(ID.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合的条件资料生成批次!";
                return result;
            }
            int MultiOrdQty = int.Parse(GetConfigure(CoID,"B").d.ToString());
            var sinID = new List<int>();
            foreach(var a in ID)
            {
                sinID.Add(a);
                if(sinID.Count == MultiOrdQty)
                {
                    result = MultiOrd(CoID,sinID,UserName);
                    if(result.s == -1)
                    {
                        return result;
                    }
                    sinID = new List<int>();
                }
            }
            if(sinID.Count > 0)
            {
                result = MultiOrd(CoID,sinID,UserName);
            }
            return result;
        }
        ///<summary>
        ///一单多件策略生成
        ///</summary>
        public static DataResult SetMultiOrdStrategy(int CoID,string UserName,int id)
        {
            var result = new DataResult(1,null);
            var strategy = GetStrategyEdit(id,CoID).d as BatchStrategy;
            var ID = new List<int>();
            string shop = GetConfigure(CoID,"G").d.ToString();
            string exp = GetConfigure(CoID,"F").d.ToString();
            int bigord = int.Parse(GetConfigure(CoID,"E").d.ToString());
            string sqlcommand = string.Empty;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    if(bigord == 0)
                    {
                        sqlcommand = @"select saleout.ID from saleout where status = 0 and BatchID = 0 and OrdQty > 1 and ExpName != '现场取货' and coid = " + CoID;
                    }
                    else
                    {
                        sqlcommand = @"select saleout.ID from saleout where status = 0 and BatchID = 0 and OrdQty > 1 and OrdQty < " + bigord + " and ExpName != '现场取货' and coid = " + CoID;
                    }
                    if(!string.IsNullOrEmpty(strategy.AmtMin))
                    {
                        sqlcommand = sqlcommand + " and Amount >= " + strategy.AmtMin;
                    }
                    if(!string.IsNullOrEmpty(strategy.AmtMax))
                    {
                        sqlcommand = sqlcommand + " and Amount <= " + strategy.AmtMax;
                    }
                    if(!string.IsNullOrEmpty(strategy.PayDateStart) && DateTime.Parse(strategy.PayDateStart) > DateTime.Parse("1900-01-01"))
                    {
                        sqlcommand = sqlcommand + " and PayDate <= '" + strategy.PayDateStart + "'";
                    }
                    if(!string.IsNullOrEmpty(strategy.PayDateEnd) && DateTime.Parse(strategy.PayDateEnd) > DateTime.Parse("1900-01-01"))
                    {
                        sqlcommand = sqlcommand + " and PayDate >= '" + strategy.PayDateEnd + "'";
                    }
                    if(!string.IsNullOrEmpty(strategy.RecMessage))
                    {
                        sqlcommand = sqlcommand + " and RecMessage like '%" + strategy.RecMessage + "%'";
                    }
                    if(!string.IsNullOrEmpty(strategy.SendMessage))
                    {
                        sqlcommand = sqlcommand + " and SendMessage like '%" + strategy.SendMessage + "%'";
                    }
                    if(!string.IsNullOrEmpty(strategy.ShopIn))
                    {
                        if(strategy.ShopIn == "B")
                        {
                            strategy.ShopIn = shop;
                        }
                        if(strategy.ShopIn != "A")
                        {
                            sqlcommand = sqlcommand + " and ShopID in (" + strategy.ShopIn + ")";
                        }
                    }
                    if(!string.IsNullOrEmpty(strategy.ExpressIn))
                    {
                        if(strategy.ExpressIn == "B")
                        {
                            strategy.ExpressIn = exp;
                        }
                        if(strategy.ExpressIn != "A")
                        {
                            sqlcommand = sqlcommand + " and ExID in (" + strategy.ExpressIn + ")";
                        }
                    }
                    if(strategy.ExpPrint == 1)
                    {
                        sqlcommand = sqlcommand + " and IsExpPrint == false";
                    }
                    if(strategy.ExpPrint == 2)
                    {
                        sqlcommand = sqlcommand + " and IsExpPrint == true";
                    }
                    if(!string.IsNullOrEmpty(strategy.DistributorIn))
                    {
                        string[] a = strategy.DistributorIn.Split(',');
                        List<string> dis = new List<string>();
                        bool isflag = false;
                        foreach(var aa in a)
                        {
                            if(aa == "0")
                            {
                                isflag = true;
                            }
                            else
                            {
                                string name = DistributorHaddle.getDisName(CoID.ToString(),aa);
                                dis.Add(name);
                            }
                        }
                        if(isflag == true && dis.Count == 0)
                        {
                            sqlcommand = sqlcommand + " and Distributor == ''";
                        }
                        if(isflag == true && dis.Count > 0)
                        {
                            string distributor = string.Empty;
                            foreach(var x in dis)
                            {
                                distributor = distributor + "'" + x + "',";
                            }
                            distributor = distributor.Substring(0,distributor.Length - 1);
                            sqlcommand = sqlcommand + " and (Distributor == '' or Distributor in (" + distributor + "))";
                        }
                        if(isflag == false && dis.Count > 0)
                        {
                            string distributor = string.Empty;
                            foreach(var x in dis)
                            {
                                distributor = distributor + "'" + x + "',";
                            }
                            distributor = distributor.Substring(0,distributor.Length - 1);
                            sqlcommand = sqlcommand + " and Distributor in (" + distributor + ")";
                        }
                    }
                    ID = conn.Query<int>(sqlcommand).AsList();    
                    if(ID.Count == 0)
                    {
                        result.s = -1;
                        result.d = "没有符合的条件资料生成批次!";
                        return result;
                    }   
                    if(!string.IsNullOrEmpty(strategy.SkuIn) || !string.IsNullOrEmpty(strategy.SkuNotIn) || strategy.OrdGift != 0 || 
                       !string.IsNullOrEmpty(strategy.KindIDIn) || !string.IsNullOrEmpty(strategy.PCodeIn) || !string.IsNullOrEmpty(strategy.PrioritySku))  
                    {
                        var NID = new List<int>();
                        var PID = new List<int>();
                        foreach(var a in ID)
                        {
                            sqlcommand = "select count(id) from saleoutitem where sid = " + a + " and coid = " + CoID + " and isgift = true";
                            int count = conn.QueryFirst<int>(sqlcommand);    
                            if(strategy.OrdGift == 1 && count == 0) continue;
                            if(strategy.OrdGift == 2 && count > 0) continue;
                            sqlcommand = "select SkuAutoID,SkuID from saleoutitem where sid = " + a + " and coid = " + CoID + " and isgift = false";
                            var sku = conn.Query<SaleOutItemInsert>(sqlcommand).AsList();  
                            if(!string.IsNullOrEmpty(strategy.SkuIn))
                            {
                                int i = 0;
                                foreach(var s in sku)
                                {
                                    if(strategy.SkuIn.Contains(s.SkuID))
                                    {
                                        i ++;
                                        break;
                                    }
                                }
                                if(i == 0) continue;
                            } 
                            if(!string.IsNullOrEmpty(strategy.SkuNotIn))
                            {
                                int i = 0;
                                foreach(var s in sku)
                                {
                                    if(strategy.SkuNotIn.Contains(s.SkuID))
                                    {
                                        i ++;
                                        break;
                                    }
                                }
                                if(i > 0) continue;
                            } 
                            if(!string.IsNullOrEmpty(strategy.KindIDIn))
                            {
                                var skuid = new List<int>();
                                foreach(var s in sku)
                                {
                                    skuid.Add(s.SkuAutoID);
                                }
                                sqlcommand = @"select count(id) from coresku where id in (" + string.Join(",",skuid) + ") and KindID not in (" + strategy.KindIDIn + ")" + 
                                              " and coid =" + CoID;
                                count = conn.QueryFirst<int>(sqlcommand);  
                                if(count > 0) continue;
                            } 
                            if(!string.IsNullOrEmpty(strategy.PCodeIn))
                            {
                                var skuid = new List<int>();
                                foreach(var s in sku)
                                {
                                    skuid.Add(s.SkuAutoID);
                                }
                                string[] pcode = strategy.PCodeIn.Split(',');
                                string wheresql = "";
                                foreach(var p in pcode)
                                {
                                    wheresql = wheresql + "PCode like '%" + p + "%' or";
                                }
                                wheresql = "and (" + wheresql.Substring(0,wheresql.Length - 3) + ")";
                                sqlcommand = @"select count(id) from wmspile where Skuautoid in (" + string.Join(",",skuid) + ") and coid =" + CoID + wheresql + " and Qty <= lockqty";
                                count = conn.QueryFirst<int>(sqlcommand);  
                                if(count > 0) continue;
                            } 
                            int pp = 0;
                            if(!string.IsNullOrEmpty(strategy.PrioritySku))
                            {
                                foreach(var s in sku)
                                {
                                    if(strategy.PrioritySku.Contains(s.SkuID))
                                    {
                                        pp ++;
                                        break;
                                    }
                                }
                            } 
                            if(pp > 0)
                            {
                                PID.Add(a);
                            }
                            else
                            {
                                NID.Add(a);
                            }
                        }
                        ID = PID;
                        foreach(var n in NID)
                        {
                            ID.Add(n);
                        }
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }    
            if(ID.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合的条件资料生成批次!";
                return result;
            }
            int MultiOrdqty = 0;
            if(!string.IsNullOrEmpty(strategy.OrdQty))
            {
                MultiOrdqty = int.Parse(strategy.OrdQty);
            }
            else
            {
                MultiOrdqty = int.Parse(GetConfigure(CoID,"B").d.ToString());
            }
            var sinID = new List<int>();
            foreach(var a in ID)
            {
                sinID.Add(a);
                if(sinID.Count == MultiOrdqty)
                {
                    result = MultiOrd(CoID,sinID,UserName);
                    if(result.s == -1)
                    {
                        return result;
                    }
                    sinID = new List<int>();
                }
            }
            if(sinID.Count > 0)
            {
                result = MultiOrd(CoID,sinID,UserName);
            }
            return result;
        }
        ///<summary>
        ///订单任务资料查询
        ///</summary>
        public static DataResult GetOrdTask(int CoID,int id,int OrdQtyS,int OrdQtyE)
        {
            var result = new DataResult(1,null);
            var ID = new List<OrdTask>();
            string shop = GetConfigure(CoID,"G").d.ToString();
            string exp = GetConfigure(CoID,"F").d.ToString();
            string sqlcommand = string.Empty;
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    sqlcommand = @"select ID,oid,ExpName,OrdQty from saleout where status = 0 and BatchID = 0 and ExpName != '现场取货' and coid = " + CoID;
                    if(OrdQtyS > 0)
                    {
                        sqlcommand = sqlcommand + " and OrdQty >= " + OrdQtyS;
                    }
                    if(OrdQtyE > 0)
                    {
                        sqlcommand = sqlcommand + " and OrdQty <= " + OrdQtyS;
                    }
                    if(id > 0)
                    {
                        var strategy = GetStrategyEdit(id,CoID).d as BatchStrategy;
                        if(!string.IsNullOrEmpty(strategy.AmtMin))
                        {
                            sqlcommand = sqlcommand + " and Amount >= " + strategy.AmtMin;
                        }
                        if(!string.IsNullOrEmpty(strategy.AmtMax))
                        {
                            sqlcommand = sqlcommand + " and Amount <= " + strategy.AmtMax;
                        }
                        if(!string.IsNullOrEmpty(strategy.PayDateStart) && DateTime.Parse(strategy.PayDateStart) > DateTime.Parse("1900-01-01"))
                        {
                            sqlcommand = sqlcommand + " and PayDate <= '" + strategy.PayDateStart + "'";
                        }
                        if(!string.IsNullOrEmpty(strategy.PayDateEnd) && DateTime.Parse(strategy.PayDateEnd) > DateTime.Parse("1900-01-01"))
                        {
                            sqlcommand = sqlcommand + " and PayDate >= '" + strategy.PayDateEnd + "'";
                        }
                        if(!string.IsNullOrEmpty(strategy.RecMessage))
                        {
                            sqlcommand = sqlcommand + " and RecMessage like '%" + strategy.RecMessage + "%'";
                        }
                        if(!string.IsNullOrEmpty(strategy.SendMessage))
                        {
                            sqlcommand = sqlcommand + " and SendMessage like '%" + strategy.SendMessage + "%'";
                        }
                        if(!string.IsNullOrEmpty(strategy.ShopIn))
                        {
                            if(strategy.ShopIn == "B")
                            {
                                strategy.ShopIn = shop;
                            }
                            if(strategy.ShopIn != "A")
                            {
                                sqlcommand = sqlcommand + " and ShopID in (" + strategy.ShopIn + ")";
                            }
                        }
                        if(!string.IsNullOrEmpty(strategy.ExpressIn))
                        {
                            if(strategy.ExpressIn == "B")
                            {
                                strategy.ExpressIn = exp;
                            }
                            if(strategy.ExpressIn != "A")
                            {
                                sqlcommand = sqlcommand + " and ExID in (" + strategy.ExpressIn + ")";
                            }
                        }
                        if(strategy.ExpPrint == 1)
                        {
                            sqlcommand = sqlcommand + " and IsExpPrint == false";
                        }
                        if(strategy.ExpPrint == 2)
                        {
                            sqlcommand = sqlcommand + " and IsExpPrint == true";
                        }
                        if(!string.IsNullOrEmpty(strategy.DistributorIn))
                        {
                            string[] a = strategy.DistributorIn.Split(',');
                            List<string> dis = new List<string>();
                            bool isflag = false;
                            foreach(var aa in a)
                            {
                                if(aa == "0")
                                {
                                    isflag = true;
                                }
                                else
                                {
                                    string name = DistributorHaddle.getDisName(CoID.ToString(),aa);
                                    dis.Add(name);
                                }
                            }
                            if(isflag == true && dis.Count == 0)
                            {
                                sqlcommand = sqlcommand + " and Distributor == ''";
                            }
                            if(isflag == true && dis.Count > 0)
                            {
                                string distributor = string.Empty;
                                foreach(var x in dis)
                                {
                                    distributor = distributor + "'" + x + "',";
                                }
                                distributor = distributor.Substring(0,distributor.Length - 1);
                                sqlcommand = sqlcommand + " and (Distributor == '' or Distributor in (" + distributor + "))";
                            }
                            if(isflag == false && dis.Count > 0)
                            {
                                string distributor = string.Empty;
                                foreach(var x in dis)
                                {
                                    distributor = distributor + "'" + x + "',";
                                }
                                distributor = distributor.Substring(0,distributor.Length - 1);
                                sqlcommand = sqlcommand + " and Distributor in (" + distributor + ")";
                            }
                        }
                    }
                    ID = conn.Query<OrdTask>(sqlcommand).AsList();    
                    if(ID.Count > 0 && id > 0)
                    {
                        var strategy = GetStrategyEdit(id,CoID).d as BatchStrategy;
                        if(!string.IsNullOrEmpty(strategy.SkuIn) || !string.IsNullOrEmpty(strategy.SkuNotIn) || strategy.OrdGift != 0 || 
                        !string.IsNullOrEmpty(strategy.KindIDIn) || !string.IsNullOrEmpty(strategy.PCodeIn))  
                        {
                            var NID = new List<OrdTask>();
                            foreach(var a in ID)
                            {
                                sqlcommand = "select count(id) from saleoutitem where sid = " + a.id + " and coid = " + CoID + " and isgift = true";
                                int count = conn.QueryFirst<int>(sqlcommand);    
                                if(strategy.OrdGift == 1 && count == 0) continue;
                                if(strategy.OrdGift == 2 && count > 0) continue;
                                sqlcommand = "select SkuAutoID,SkuID from saleoutitem where sid = " + a.id + " and coid = " + CoID + " and isgift = false";
                                var sku = conn.Query<SaleOutItemInsert>(sqlcommand).AsList();  
                                if(!string.IsNullOrEmpty(strategy.SkuIn))
                                {
                                    int i = 0;
                                    foreach(var s in sku)
                                    {
                                        if(strategy.SkuIn.Contains(s.SkuID))
                                        {
                                            i ++;
                                            break;
                                        }
                                    }
                                    if(i == 0) continue;
                                } 
                                if(!string.IsNullOrEmpty(strategy.SkuNotIn))
                                {
                                    int i = 0;
                                    foreach(var s in sku)
                                    {
                                        if(strategy.SkuNotIn.Contains(s.SkuID))
                                        {
                                            i ++;
                                            break;
                                        }
                                    }
                                    if(i > 0) continue;
                                } 
                                if(!string.IsNullOrEmpty(strategy.KindIDIn))
                                {
                                    var skuid = new List<int>();
                                    foreach(var s in sku)
                                    {
                                        skuid.Add(s.SkuAutoID);
                                    }
                                    sqlcommand = @"select count(id) from coresku where id in (" + string.Join(",",skuid) + ") and KindID not in (" + strategy.KindIDIn + ")" + 
                                                " and coid =" + CoID;
                                    count = conn.QueryFirst<int>(sqlcommand);  
                                    if(count > 0) continue;
                                } 
                                if(!string.IsNullOrEmpty(strategy.PCodeIn))
                                {
                                    var skuid = new List<int>();
                                    foreach(var s in sku)
                                    {
                                        skuid.Add(s.SkuAutoID);
                                    }
                                    string[] pcode = strategy.PCodeIn.Split(',');
                                    string wheresql = "";
                                    foreach(var p in pcode)
                                    {
                                        wheresql = wheresql + "PCode like '%" + p + "%' or";
                                    }
                                    wheresql = "and (" + wheresql.Substring(0,wheresql.Length - 3) + ")";
                                    sqlcommand = @"select count(id) from wmspile where Skuautoid in (" + string.Join(",",skuid) + ") and coid =" + CoID + wheresql + " and Qty <= lockqty";
                                    count = conn.QueryFirst<int>(sqlcommand);  
                                    if(count > 0) continue;
                                } 
                                NID.Add(a);
                            }
                            ID = NID;
                        }
                    }
                    if(ID.Count > 0)
                    {
                        var ItemID = new List<int>();
                        foreach(var a in ID)
                        {
                            ItemID.Add(a.id);
                        }
                        if(ItemID.Count > 0)
                        {
                            sqlcommand = @"select * from saleoutitem  where sid in @ID and coid = @Coid and IsGift = false";
                            var item = conn.Query<SaleOutItemInsert>(sqlcommand,new{ID = ItemID,Coid = CoID}).AsList();
                            foreach(var a in ID)
                            {
                                string remark = a.OrdQty + ".";
                                if(a.OrdQty == 1)
                                {
                                    foreach(var i in item)
                                    {
                                        if(a.id == i.SID)
                                        {
                                            remark = remark + i.SkuID + ",";
                                        }
                                    }
                                }
                                else
                                {
                                    foreach(var i in item)
                                    {
                                        if(a.id == i.SID)
                                        {
                                            remark = remark + i.SkuID + "*" + i.Qty + ",";
                                        }
                                    }
                                }
                                remark = remark.Substring(0,remark.Length - 1);
                                a.Sku = remark;
                            }
                        }
                        result.d = ID;
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
        ///选择订单生成任务
        ///</summary>
        public static DataResult OrdTaskBatch(int CoID,string UserName,List<int> ID)
        {
            var result = new DataResult(1,null);            
            int MultiOrdqty = int.Parse(GetConfigure(CoID,"B").d.ToString());
            var sinID = new List<int>();
            foreach(var a in ID)
            {
                sinID.Add(a);
                if(sinID.Count == MultiOrdqty)
                {
                    result = MultiOrd(CoID,sinID,UserName);
                    if(result.s == -1)
                    {
                        return result;
                    }
                    sinID = new List<int>();
                }
            }
            if(sinID.Count > 0)
            {
                result = MultiOrd(CoID,sinID,UserName);
            }
            return result;
        }
        ///<summary>
        ///现场大单
        ///</summary>
        public static DataResult SetBigOrd(int CoID,string UserName)
        {
            var result = new DataResult(1,null);  
            string sqlcommand = string.Empty;
            var ID = new List<int>();
            int bigord = int.Parse(GetConfigure(CoID,"E").d.ToString());
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    if(bigord == 0)
                    {
                        sqlcommand = @"select ID from saleout where status = 0 and BatchID = 0 and ExpName = '现场取货' and coid = " + CoID;
                    }
                    else
                    {
                        sqlcommand = @"select ID from saleout where status = 0 and BatchID = 0 and (ExpName = '现场取货' or OrdQty >= " + bigord + ") and coid = " + CoID;
                    }
                    ID = conn.Query<int>(sqlcommand).AsList();         
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }  
            if(ID.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合条件的资料!";
                return result;
            }
            int count  = 0;
            int rtn = 0;  
            foreach(var a in ID)
            {
                var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
                CoreDBconn.Open();
                var TransCore = CoreDBconn.BeginTransaction();
                try
                {
                    //产生批次资料
                    sqlcommand = @"INSERT INTO batch(Type,OrderQty,SkuQty,Qty,CoID,Creator,Modifier) 
                                            VALUES(@Type,@OrderQty,@SkuQty,@Qty,@CoID,@Creator,@Creator)";
                    count = CoreDBconn.Execute(sqlcommand,new{Type=2,OrderQty=0,SkuQty=0,Qty=0,CoID=CoID,Creator=UserName},TransCore);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    rtn = CoreDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    //抓取出库明细
                    sqlcommand = "select * from saleoutitem where sid = " + a + " and coid = " + CoID + " and isgift = false";
                    var item = CoreDBconn.Query<SaleOutItemInsert>(sqlcommand).AsList();
                    bool isflag = false;
                    int i = 0;//计算商品总数
                    foreach(var it in item)
                    {                        
                        //抓取库存
                        sqlcommand = @"select ID,Qty - lockqty as Qty,PCode,`Order` from wmspile where Skuautoid = " + it.SkuAutoID + " and Type in (1,2) and Enable = true and CoID = " + CoID + 
                                    " and Qty > lockqty order by Type,`Order`";
                        var invqty = CoreDBconn.Query<InvQty>(sqlcommand).AsList();
                        if(invqty.Count == 0) 
                        {
                            isflag = true;
                            break;
                        }
                        int totqty = 0;
                        foreach(var inv in invqty)
                        {
                            totqty = totqty + inv.Qty;
                        }
                        if(totqty < it.Qty)
                        {
                            isflag = true;
                            break;
                        }
                        int saleqty = it.Qty;
                        foreach(var inv in invqty)
                        {
                            int taskqty = 0;
                            if(inv.Qty >= it.Qty)
                            {
                                taskqty = saleqty;
                                saleqty = 0;
                            }
                            else
                            {
                                taskqty = inv.Qty;
                                saleqty = saleqty - taskqty;
                            }
                            //产生批次任务
                            sqlcommand = @"INSERT INTO batchtask(BatchID,Skuautoid,SkuID,SkuName,CoID,PCode,Qty,`Index`) 
                                                    VALUES(@BatchID,@Skuautoid,@SkuID,@SkuName,@CoID,@PCode,@Qty,@Index)";
                            count = CoreDBconn.Execute(sqlcommand,new{BatchID=rtn,Skuautoid=it.SkuAutoID,SkuID=it.SkuID,SkuName=it.SkuName,PCode=inv.PCode,Qty=taskqty,Index =inv.Order,CoID=CoID},TransCore);
                            if(count < 0)
                            {
                                result.s = -3002;
                                return result;
                            }
                            //更新库存
                            sqlcommand = @"update wmspile set lockqty=lockqty + @Qty where id = @ID";
                            count = CoreDBconn.Execute(sqlcommand,new{Qty = taskqty,ID = inv.ID},TransCore);
                            if(count < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                            if(saleqty == 0) break;
                        }
                        i = i + it.Qty;
                    }
                    if(isflag == true) continue;
                    //更新批次资料
                    sqlcommand = @"update batch set OrderQty=1,SkuQty=SkuQty+ @SkuQty,Qty=Qty + @Qty,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{SkuQty =item.Count,Qty = i,ModifyDate = DateTime.Now,Modifier=UserName,ID = rtn},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    //更新出库单
                    sqlcommand = @"update saleout set BatchID=@BatchID,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{BatchID = rtn,ModifyDate = DateTime.Now,Modifier=UserName,ID = a},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    sqlcommand = @"update saleoutitem set BatchID=@BatchID,Modifier=@Modifier,ModifyDate=@ModifyDate where sid = @ID";
                    count = CoreDBconn.Execute(sqlcommand,new{BatchID = rtn,ModifyDate = DateTime.Now,Modifier=UserName,ID = a},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    sqlcommand = "select id,oid from saleout where id = @ID and coid = @Coid";
                    var l = CoreDBconn.Query<SaleOutInsert>(sqlcommand,new{ID = a,Coid = CoID}).AsList();
                    foreach(var ll in l)
                    {
                        sqlcommand = @"INSERT INTO batch_log(BatchID,SaleID,Operate,UniqueCode,CoID,Creator) 
                                                  VALUES(@BatchID,@SaleID,@Operate,@UniqueCode,@CoID,@Creator)";
                        count = CoreDBconn.Execute(sqlcommand,new{BatchID=rtn,SaleID = ll.ID,Operate="绑定批次",UniqueCode="订单:" + ll.OID,CoID=CoID,Creator=UserName},TransCore);
                        if(count < 0)
                        {
                            result.s = -3002;
                            return result;
                        }
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
            }    
            return result;  
        }
        ///<summary>
        ///结束任务
        ///</summary>
        public static DataResult CancleBatch(int CoID,string UserName,List<int> ID)
        {
            var result = new DataResult(1,null);
            string sqlcommand = string.Empty;
            var batch = new List<Batch>();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    sqlcommand = @"select * from batch where id in @ID and coid = @Coid";
                    batch = conn.Query<Batch>(sqlcommand,new{ID = ID,Coid = CoID}).AsList();    
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            }  
            if(batch.Count == 0)
            {
                result.s = -1;
                result.d = "没有符合条件的资料!";
                return result;
            }
            int count = 0;
            var res = new CancleBatchReturn();
            var fa = new List<TransferNormalReturnFail>();
            var su = new List<CancleBatchSuccess>();
            var CoreDBconn = new MySqlConnection(DbBase.CoreConnectString);
            CoreDBconn.Open();
            var TransCore = CoreDBconn.BeginTransaction();
            try
            {
                foreach(var a in batch)
                {
                    if(a.Status == 6 || a.Status == 7)
                    {
                        var ff = new TransferNormalReturnFail();
                        ff.ID = a.ID;
                        ff.Reason = "已完成或终止拣货的批次不可结束任务!";
                        fa.Add(ff);
                        continue;
                    }
                    //更新批次资料
                    sqlcommand = "update batch set status = 7,Modifier=@Modifier,ModifyDate=@ModifyDate where id = @ID and coid = @Coid";
                    count = CoreDBconn.Execute(sqlcommand,new{ModifyDate=DateTime.Now,CoID=CoID,Modifier=UserName,ID = a.ID},TransCore);
                    if(count < 0)
                    {
                        result.s = -3003;
                        return result;
                    }
                    //更新批次任务和库存
                    sqlcommand = "select * from batchtask where BatchID = " + a.ID + " and coid = " + CoID;
                    var task = CoreDBconn.Query<BatchTask>(sqlcommand).AsList();
                    foreach(var t in task)
                    {
                        if(t.Qty != t.PickQty)
                        {
                            t.NoQty = t.Qty - t.PickQty;
                            // //更新批次任务
                            // sqlcommand = @"update batchtask set NoQty=@NoQty where id = @ID and coid = @Coid";
                            // count = CoreDBconn.Execute(sqlcommand,t,TransCore);
                            // if(count < 0)
                            // {
                            //     result.s = -3003;
                            //     return result;
                            // }
                            //更新库存
                            sqlcommand = @"update wmspile set lockqty=lockqty - @Qty where Skuautoid = @Skuautoid and Type in (1,2) and coid = @Coid and PCode = @PCode";
                            count = CoreDBconn.Execute(sqlcommand,new{Qty = t.NoQty,Skuautoid = t.Skuautoid,Coid=CoID,PCode=t.PCode},TransCore);
                            if(count < 0)
                            {
                                result.s = -3003;
                                return result;
                            }
                        }
                    }
                    //更新出库单
                    sqlcommand = "select ID from saleout where BatchID = " + a.ID + " and coid = " + CoID + " and status = 0";
                    var saleID = CoreDBconn.Query<int>(sqlcommand).AsList();
                    if(saleID.Count > 0)
                    {
                        sqlcommand = @"update saleout set BatchID=0,SortCode=null,Modifier=@Modifier,ModifyDate=@ModifyDate where ID in @ID and coid = @Coid";
                        count = CoreDBconn.Execute(sqlcommand,new{ID = saleID,ModifyDate = DateTime.Now,Modifier=UserName,Coid = CoID},TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                        sqlcommand = @"update saleoutitem set BatchID=0,Modifier=@Modifier,ModifyDate=@ModifyDate where sid in @ID and coid = @Coid";
                        count = CoreDBconn.Execute(sqlcommand,new{ID = saleID,ModifyDate = DateTime.Now,Modifier=UserName,Coid = CoID},TransCore);
                        if(count < 0)
                        {
                            result.s = -3003;
                            return result;
                        }
                    }   
                    sqlcommand = @"INSERT INTO batch_log(BatchID,Operate,Qty,CoID,Creator) 
                                                    VALUES(@BatchID,@Operate,@Qty,@CoID,@Creator)";
                    count = CoreDBconn.Execute(sqlcommand,new{BatchID=a.ID,Operate="拣货批次:终止",Qty=1,CoID=CoID,Creator=UserName},TransCore);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    var ss = new CancleBatchSuccess();
                    ss.ID = a.ID;
                    ss.NotPickedQty = a.Qty - a.PickedQty;
                    ss.NoQty = 0;
                    ss.Status = 7;
                    ss.StatusString = Enum.GetName(typeof(BatchStatus), ss.Status);
                    su.Add(ss);
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
            res.SuccessIDs = su;
            res.FailIDs = fa;
            result.d = res;  
            return result;
        }
        ///<summary>
        ///查看待补货待上架商品
        ///</summary>
        public static DataResult GetLackSku(int CoID)
        {
            var result = new DataResult(1,null);
            string sqlcommand = string.Empty;
            var res = new List<LackSkuList>();
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    sqlcommand = @"select SkuAutoID,SkuID,GoodsCode,Norm,sum(Qty) as OrdQty from saleout,saleoutitem where saleout.ID = saleoutitem.SID and saleout.`Status`=0 
                                   and saleout.coid = " +CoID + " and saleoutitem.IsGift = false and saleoutitem.BatchID = 0 group by SkuAutoID,SkuID,GoodsCode,Norm";
                    var u = conn.Query<LackSkuList>(sqlcommand).AsList();
                    foreach(var a in u)
                    {
                        sqlcommand = @"select sum(Qty - lockqty) from wmspile where Skuautoid = " + a.SkuAutoID + " and Type in (1,2) and Enable = true and CoID = " + CoID;
                        int invqty = conn.QueryFirst<int>(sqlcommand);
                        if(a.OrdQty > invqty)
                        {
                            a.NoQty = a.OrdQty - invqty;
                            res.Add(a);
                        }
                    }      
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
        ///批次日志查询
        ///</summary>
        public static DataResult GetBatchLog(int ID,int CoID)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select SaleID,Operate,UniqueCode,SkuID,Qty,Remark,Remark2,Creator,CreateDate from batch_log where BatchID = " + ID + 
                                         " and coid = " + CoID + " order by CreateDate desc";
                    var u = conn.Query<BatchLog>(sqlcommand).AsList();
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
        ///拣货明细信息
        ///</summary>
        public static DataResult GetBatchItem(int CoID,int id)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select coresku.SkuID,coresku.SkuName,coresku.Norm,sum(batchtask.qty) as Qty,sum(batchtask.PickQty) as PickQty,
                                          sum(batchtask.qty - batchtask.PickQty) as NoQty from batchtask,coresku where batchtask.Skuautoid = coresku.ID and 
                                          batchtask.CoID = coresku.CoID and batchtask.BatchID=" + id + " and batchtask.CoID = " + CoID + 
                                          " group by batchtask.Skuautoid,coresku.SkuName,coresku.Norm";
                    var u = conn.Query<BatchItemList>(sqlcommand).AsList();
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
        ///拣货批次唯一码
        ///</summary>
        public static DataResult GetBatchUnique(int CoID,int id)
        {
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string sqlcommand = @"select BarCode,Sku,status,OutID,PCode,ModifyDate from batchpicked where BatchID = " + id + " and CoID = " + CoID + 
                                         " order by Sku,BarCode,ModifyDate";
                    var u = conn.Query<BatchUniqueList>(sqlcommand).AsList();
                    foreach(var a in u)
                    {
                        a.StatusString = Enum.GetName(typeof(PickStatus), a.Status);
                        sqlcommand = "select status from saleout where id = " + a.OutID + " and coid = " + CoID;
                        var sa = conn.Query<SaleOutInsert>(sqlcommand).AsList();
                        if(sa[0].Status == 1)
                        {
                            a.OutIDString = a.OutID + " - 已发货";
                        }
                    }
                    result.d = u;               
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