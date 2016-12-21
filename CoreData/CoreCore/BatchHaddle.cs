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
            int SingleOrdQty = int.Parse(GetConfigure(CoID,"A").d.ToString());
            for(int i = 0;i < 100;i++)  
            {
                var ID = new List<int>();
                using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                    try{
                        string sqlcommand = @"select saleout.ID from saleout where status = 0 and BatchID = 0 and OrdQty = 1 and ExpName != '现场取货' and 
                                              coid = " + CoID + " limit 0," + SingleOrdQty;
                        ID = conn.Query<int>(sqlcommand).AsList();         
                    }catch(Exception ex){
                        result.s = -1;
                        result.d = ex.Message;
                        conn.Dispose();
                    }
                }    
                if(ID.Count > 0)
                {
                    result = SingleOrd(CoID,ID,UserName);
                    if(result.s == -1)
                    {
                        break;
                    }
                }    
                else
                {
                    break;
                }
            }
            return result;
        }
    }
}