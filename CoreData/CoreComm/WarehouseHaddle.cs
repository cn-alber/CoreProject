using CoreModels;
using CoreModels.XyComm;
using Dapper;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
namespace CoreData.CoreComm
{
    public static class WarehouseHaddle
    {
        ///<summary>
        ///仓库基本资料新增
        ///</summary>
        public static DataResult InsertWarehouse(WarehouseInsert wh,string UserName,string Company,int CoID)
        {
            var result = new DataResult(1,null);   
            //检查相关必输栏位
            if(wh.name0 == null)        
                result.s = -3101;            
            if(wh.name1 == null)            
                result.s = -3102;    
            if(wh.name3 == null)
            {
                result.s = -1;
                result.d = "请输入销售退货仓库名称!";
                return result;
            }
            if(wh.name4 == null)
            {
                result.s = -1;
                result.d = "请输入进货仓库名称!";
                return result;
            }
            if(wh.name5 == null)
            {
                result.s = -1;
                result.d = "请输入次品仓库名称!";
                return result;
            }
            if(wh.area.Count< 3 || string.IsNullOrEmpty(wh.address))
            {
                result.s = -1;
                result.d = "请输入完整的仓库地址!";
                return result;
            }
            //检查仓库名称是否重复
            var res = WarehouseHaddle.IsWarehouseExist(wh.name0,CoID);
            if (bool.Parse(res.d.ToString()) == true)
            {
                result.s = -1;
                result.d = "仓库已存在,不允许新增!";
                return result;
            }
            res = WarehouseHaddle.IsWarehouseExist(wh.name1,CoID);
            if (bool.Parse(res.d.ToString()) == true)
            {
                result.s = -1;
                result.d = "仓库已存在,不允许新增!";
                return result;
            }
            if(wh.name2 != null)
            {
                res = WarehouseHaddle.IsWarehouseExist(wh.name2,CoID);
                if (bool.Parse(res.d.ToString()) == true)
                {
                    result.s = -1;
                    result.d = "仓库已存在,不允许新增!";
                    return result;
                }
            }
            res = WarehouseHaddle.IsWarehouseExist(wh.name3,CoID);
            if (bool.Parse(res.d.ToString()) == true)
            {
                result.s = -1;
                result.d = "仓库已存在,不允许新增!";
                return result;
            }
            res = WarehouseHaddle.IsWarehouseExist(wh.name4,CoID);
            if (bool.Parse(res.d.ToString()) == true)
            {
                result.s = -1;
                result.d = "仓库已存在,不允许新增!";
                return result;
            }
            res = WarehouseHaddle.IsWarehouseExist(wh.name5,CoID);
            if (bool.Parse(res.d.ToString()) == true)
            {
                result.s = -1;
                result.d = "仓库已存在,不允许新增!";
                return result;
            }
            if(result.s != 1)   return result;  //格式验证


            var warehouse = new Warehouse();
            warehouse.parentid = 0;
            warehouse.warehousename = wh.name0;
            warehouse.type = 0;
            warehouse.contract = wh.contract;
            warehouse.phone = wh.phone;
            warehouse.logistics = wh.area[0];
            warehouse.city = wh.area[1];
            warehouse.district = wh.area[2];
            warehouse.address = wh.address;
            warehouse.enable = true;
            warehouse.creator = UserName;
            warehouse.createdate = DateTime.Now;
            warehouse.modifier = UserName;
            warehouse.modifydate = DateTime.Now;
            warehouse.coid = CoID;
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            CommDBconn.Open();
            UserDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            var TransUser = UserDBconn.BeginTransaction();
            int rtn = 0;
            try{
                string sqlCommandText = @"INSERT INTO warehouse(parentid,warehousename,type,contract,phone,logistics,city,district,address,enable,creator,createdate,modifier,modifydate,coid) 
                VALUES(@Parentid,@Warehousename,@Type,@Contract,@Phone,@Logistics,@City,@District,@Address,@Enable,@Creator,@Createdate,@Modifier,@Modifydate,@Coid)";
                //新增主仓库资料
                int count =CommDBconn.Execute(sqlCommandText,warehouse,TransComm);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    rtn = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"新增仓库资料", "warehouse", wh.name0 ,UserName, CoID, DateTime.Now);
                    CacheBase.Set<Warehouse>("warehouse" + CoID + rtn, warehouse);
                }    
                //新增销售主仓库(零数)资料
                warehouse.parentid = rtn;
                warehouse.warehousename = wh.name1;
                warehouse.type = 1;
                count =CommDBconn.Execute(sqlCommandText,warehouse,TransComm);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    int a = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"新增仓库资料", "warehouse", wh.name1 ,UserName, CoID, DateTime.Now);
                    CacheBase.Set<Warehouse>("warehouse"  +CoID + a, warehouse);
                }   
                //新增销售主仓库(整数)资料
                if(wh.name2 != null)
                {
                    warehouse.parentid = rtn;
                    warehouse.warehousename = wh.name2;
                    warehouse.type = 2;
                    count =CommDBconn.Execute(sqlCommandText,warehouse,TransComm);
                    if(count < 0)
                    {
                        result.s = -3002;
                        return result;
                    }
                    else
                    {
                        int a = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                        CoreUser.LogComm.InsertUserLogTran(TransUser,"新增仓库资料", "warehouse", wh.name2 ,UserName, CoID, DateTime.Now);
                        CacheBase.Set<Warehouse>("warehouse"  +CoID + a, warehouse);
                    }        
                }
                //新增销售退货仓库资料
                warehouse.parentid = rtn;
                warehouse.warehousename = wh.name3;
                warehouse.type = 3;
                count =CommDBconn.Execute(sqlCommandText,warehouse,TransComm);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    int a = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"新增仓库资料", "warehouse", wh.name3 ,UserName, CoID, DateTime.Now);
                    CacheBase.Set<Warehouse>("warehouse"  +CoID + a, warehouse);
                }   
                //新增销进货仓库资料
                warehouse.parentid = rtn;
                warehouse.warehousename = wh.name4;
                warehouse.type = 4;
                count =CommDBconn.Execute(sqlCommandText,warehouse,TransComm);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    int a = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"新增仓库资料", "warehouse", wh.name4 ,UserName, CoID, DateTime.Now);
                    CacheBase.Set<Warehouse>("warehouse"  +CoID + a, warehouse);
                }   
                //新增次品仓库资料
                warehouse.parentid = rtn;
                warehouse.warehousename = wh.name5;
                warehouse.type = 5;
                count =CommDBconn.Execute(sqlCommandText,warehouse,TransComm);
                if(count < 0)
                {
                    result.s = -3002;
                    return result;
                }
                else
                {
                    int a = CommDBconn.QueryFirst<int>("select LAST_INSERT_ID()");
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"新增仓库资料", "warehouse", wh.name5 ,UserName, CoID, DateTime.Now);
                    CacheBase.Set<Warehouse>("warehouse"  +CoID + a, warehouse);
                }        
                TransComm.Commit();
                TransUser.Commit();       
            }
            catch(Exception ex){
                TransComm.Rollback();
                TransUser.Rollback();
                TransComm.Dispose();
                TransUser.Dispose();
                result.s = -1;
                result.d = ex.Message;
            }
            finally
            {
                TransComm.Dispose();
                TransUser.Dispose();
                CommDBconn.Dispose();
                UserDBconn.Dispose();
            }
            return  result;
        }
        ///<summary>
        ///检查仓库资料是否已经存在
        ///</summary>
        public static DataResult IsWarehouseExist(string name,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string wheresql = "select count(*) from warehouse where warehousename ='" + name + "' and coid = " + CoID ;
                    int u = conn.QueryFirst<int>(wheresql);            
                    if (u > 0)
                    {
                        result.d = true;                 
                    }
                    else
                    {
                        result.d = false;   
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
        ///仓库基本资料更新
        ///</summary>
        public static DataResult UpdateWarehouse(WarehouseInsert wh,string UserName,string Company,int CoID)
        {
            var result = new DataResult(1,null);  
            string contents = ""; 
            string uptsql = @"update warehouse set parentid = @Parentid,warehousename = @Warehousename,type=@Type,contract = @Contract,phone = @Phone,logistics = @Logistics,
                              city = @City,district = @District,address = @Address,enable = @Enable,creator=@Creator,createdate=@Createdate,modifier=@Modifier,
                              modifydate=@Modifydate,coid=@Coid where id = @ID";
            var CommDBconn = new MySqlConnection(DbBase.CommConnectString);
            var UserDBconn = new MySqlConnection(DbBase.UserConnectString);
            CommDBconn.Open();
            UserDBconn.Open();
            var TransComm = CommDBconn.BeginTransaction();
            var TransUser = UserDBconn.BeginTransaction();
            try{
                string wheresql = "select * from warehouse where id =" + wh.id + " and coid = " + CoID ;
                var u = CommDBconn.Query<Warehouse>(wheresql).AsList();
                if(u.Count == 0)
                {
                    result.s = -1;
                    result.d = "此仓库不存在!";
                    return result;
                }
                var warehouse = u[0] as Warehouse;                
                if(wh.contract != null && wh.contract != u[0].contract)
                {
                    contents = contents + "联络人" + ":" +u[0].contract + "=>" + wh.contract + ";";
                    warehouse.contract = wh.contract;
                }
                   
                if(wh.area[0] != u[0].logistics)
                {
                    contents = contents + "省" + ":" +u[0].logistics + "=>" + wh.area[0] + ";";
                    warehouse.logistics = wh.area[0];
                }
                if(wh.area[1] != u[0].city)
                {
                    contents = contents + "市" + ":" +u[0].city + "=>" + wh.area[1] + ";";
                    warehouse.city = wh.area[1];
                }

                if(wh.area[2] != u[0].district)
                {
                    contents = contents + "区" + ":" +u[0].district + "=>" + wh.area[2] + ";";
                    warehouse.district = wh.area[2];
                }
                
                if(wh.address != null)
                {
                    if(wh.address != u[0].address)
                    {
                        contents = contents + "详细地址" + ":" +u[0].address + "=>" + wh.address + ";";
                        warehouse.address = wh.address;
                    }
                }
                if(wh.phone != null)
                {
                    if(wh.phone != u[0].phone)
                    {
                        contents = contents + "联系电话" + ":" +u[0].phone + "=>" + wh.phone + ";";
                        warehouse.phone = wh.phone;
                    }
                }
                //主仓库资料更新
                if(wh.name0 != null || contents != "")
                {
                    string contents_new = contents;
                    var warehouse_new = warehouse as Warehouse;
                    if(wh.name0 != null)
                    {
                        if(wh.name0 != u[0].warehousename)
                        {
                            contents_new = contents_new + "仓库名称" + ":" +u[0].warehousename + "=>" + wh.name0 + ";";
                            warehouse_new.warehousename = wh.name0;
                        }
                    }
                    warehouse_new.modifier = UserName;
                    warehouse_new.modifydate = DateTime.Now;
                    int count = CommDBconn.Execute(uptsql,warehouse_new,TransComm);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser,"修改仓库资料", "warehouse", contents_new,UserName, CoID, DateTime.Now);        
                        CacheBase.Set<Warehouse>("warehouse"  + CoID + warehouse_new.id, warehouse_new);
                    }
                }
                //销售主仓库（零数）更新
                if(wh.name1 != null || contents != "")
                {
                    wheresql = "select * from warehouse where parentid =" + wh.id + " and coid = " + CoID + " and type = 1" ;
                    var a = CommDBconn.Query<Warehouse>(wheresql).AsList();
                    if(a.Count == 0)
                    {
                        result.s = -1;
                        result.d = "销售主仓库(零数)资料有误!";
                        return result;
                    }
                    string contents_new = contents;
                    var warehouse_new = warehouse as Warehouse;
                    warehouse_new.warehousename = a[0].warehousename;
                    if(wh.name1 != null)
                    {
                        if(wh.name1 != a[0].warehousename)
                        {
                            contents_new = contents_new + "仓库名称" + ":" +a[0].warehousename + "=>" + wh.name1 + ";";
                            warehouse_new.warehousename = wh.name1;
                        }
                    }
                    warehouse_new.id = a[0].id;
                    warehouse_new.parentid = wh.id;
                    warehouse_new.type = 1;
                    warehouse_new.modifier = UserName;
                    warehouse_new.modifydate = DateTime.Now;
                    int count = CommDBconn.Execute(uptsql,warehouse_new,TransComm);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser,"修改仓库资料", "warehouse", contents_new,UserName, CoID, DateTime.Now);        
                        CacheBase.Set<Warehouse>("warehouse"  + CoID + warehouse_new.id, warehouse_new);
                    }
                }
                //销售主仓库（整数）更新
                if(wh.name2 != null || contents != "")
                {
                    wheresql = "select * from warehouse where parentid =" + wh.id + " and coid = " + CoID + " and type = 2" ;
                    var a = CommDBconn.Query<Warehouse>(wheresql).AsList();
                    if(a.Count > 0)
                    {
                        string contents_new = contents;
                        var warehouse_new = warehouse as Warehouse;
                        warehouse_new.warehousename = a[0].warehousename;
                        if(wh.name2 != null)
                        {
                            if(wh.name2 != a[0].warehousename)
                            {
                                contents_new = contents_new + "仓库名称" + ":" +a[0].warehousename + "=>" + wh.name2 + ";";
                                warehouse_new.warehousename = wh.name2;
                            }
                        }
                        warehouse_new.id = a[0].id;
                        warehouse_new.parentid = wh.id;
                        warehouse_new.type = 2;
                        warehouse_new.modifier = UserName;
                        warehouse_new.modifydate = DateTime.Now;
                        int count = CommDBconn.Execute(uptsql,warehouse_new,TransComm);
                        if(count < 0)
                        {
                            result.s= -3003;
                        }
                        else
                        {
                            CoreUser.LogComm.InsertUserLogTran(TransUser,"修改仓库资料", "warehouse", contents_new,UserName, CoID, DateTime.Now);        
                            CacheBase.Set<Warehouse>("warehouse"  + CoID + warehouse_new.id, warehouse_new);
                        }
                    }
                    
                }
                //销售退货仓库更新
                if(wh.name3 != null || contents != "")
                {
                    wheresql = "select * from warehouse where parentid =" + wh.id + " and coid = " + CoID + " and type = 3" ;
                    var a = CommDBconn.Query<Warehouse>(wheresql).AsList();
                    if(a.Count == 0)
                    {
                        result.s = -1;
                        result.d = "销售退货仓库资料有误!";
                        return result;
                    }
                    string contents_new = contents;
                    var warehouse_new = warehouse as Warehouse;
                    warehouse_new.warehousename = a[0].warehousename;
                    if(wh.name3 != null)
                    {
                        if(wh.name3 != a[0].warehousename)
                        {
                            contents_new = contents_new + "仓库名称" + ":" +a[0].warehousename + "=>" + wh.name3 + ";";
                            warehouse_new.warehousename = wh.name3;
                        }
                    }
                    warehouse_new.id = a[0].id;
                    warehouse_new.parentid = wh.id;
                    warehouse_new.type = 3;
                    warehouse_new.modifier = UserName;
                    warehouse_new.modifydate = DateTime.Now;
                    int count = CommDBconn.Execute(uptsql,warehouse_new,TransComm);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser,"修改仓库资料", "warehouse", contents_new,UserName, CoID, DateTime.Now);        
                        CacheBase.Set<Warehouse>("warehouse"  + CoID + warehouse_new.id, warehouse_new);
                    }
                }
                //进货仓库更新
                if(wh.name4 != null || contents != "")
                {
                    wheresql = "select * from warehouse where parentid =" + wh.id + " and coid = " + CoID + " and type = 4" ;
                    var a = CommDBconn.Query<Warehouse>(wheresql).AsList();
                    if(a.Count == 0)
                    {
                        result.s = -1;
                        result.d = "进货仓库资料有误!";
                        return result;
                    }
                    string contents_new = contents;
                    var warehouse_new = warehouse as Warehouse;
                    warehouse_new.warehousename = a[0].warehousename;
                    if(wh.name4 != null)
                    {
                        if(wh.name4 != a[0].warehousename)
                        {
                            contents_new = contents_new + "仓库名称" + ":" +a[0].warehousename + "=>" + wh.name4 + ";";
                            warehouse_new.warehousename = wh.name4;
                        }
                    }
                    warehouse_new.id = a[0].id;
                    warehouse_new.parentid = wh.id;
                    warehouse_new.type = 4;
                    warehouse_new.modifier = UserName;
                    warehouse_new.modifydate = DateTime.Now;
                    int count = CommDBconn.Execute(uptsql,warehouse_new,TransComm);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser,"修改仓库资料", "warehouse", contents_new,UserName, CoID, DateTime.Now);        
                        CacheBase.Set<Warehouse>("warehouse"  + CoID + warehouse_new.id, warehouse_new);
                    }
                }
                //次品仓库资料更新
                if(wh.name5 != null || contents != null)
                {
                    wheresql = "select * from warehouse where parentid =" + wh.id + " and coid = " + CoID + " and type = 5" ;
                    var a = CommDBconn.Query<Warehouse>(wheresql).AsList();
                    if(a.Count == 0)
                    {
                        result.s = -1;
                        result.d = "次品仓库资料有误!";
                        return result;
                    }
                    string contents_new = contents;
                    var warehouse_new = warehouse as Warehouse;
                    warehouse_new.warehousename = a[0].warehousename;
                    if(wh.name5 != null)
                    {
                        if(wh.name5 != a[0].warehousename)
                        {
                            contents_new = contents_new + "仓库名称" + ":" +a[0].warehousename + "=>" + wh.name5 + ";";
                            warehouse_new.warehousename = wh.name5;
                        }
                    }
                    warehouse_new.id = a[0].id;
                    warehouse_new.parentid = wh.id;
                    warehouse_new.type = 5;
                    warehouse_new.modifier = UserName;
                    warehouse_new.modifydate = DateTime.Now;
                    int count = CommDBconn.Execute(uptsql,warehouse_new,TransComm);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLogTran(TransUser,"修改仓库资料", "warehouse", contents_new,UserName, CoID, DateTime.Now);        
                        CacheBase.Set<Warehouse>("warehouse"  + CoID + warehouse_new.id, warehouse_new);
                    }
                }
                TransComm.Commit();
                TransUser.Commit();       
            }
            catch(Exception ex){
                TransComm.Rollback();
                TransUser.Rollback();
                TransComm.Dispose();
                TransUser.Dispose();
                result.s = -1;
                result.d = ex.Message;
            }
            finally
            {
                TransComm.Dispose();
                TransUser.Dispose();
                CommDBconn.Dispose();
                UserDBconn.Dispose();
            }
            return  result;
        }
        ///<summary>
        ///检查仓库是否可停用
        ///</summary>
        public static DataResult IsWarehouseEnable(int whid,int CoID)
        {
            var result = new DataResult(1,null);   
            List<int> whidlist = new List<int>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string wheresql = "select * from warehouse where (id = " + whid + " or parentid = " + whid + ") and coid = " + CoID ;
                    var u = conn.Query<Warehouse>(wheresql).AsList();   
                    if(u.Count == 0)         
                    {
                        result.s = -1;
                        result.d = "不存在此仓库代号资料";    
                        return result;             
                    }     
                    foreach(var a in u)
                    {
                        whidlist.Add(a.id);
                    }
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select count(*) from inventory where warehouseid in @Whidlist and coid = @Coid and (stockqty > 0 or defectiveqty > 0)";
                    int u = conn.QueryFirst<int>(wheresql,new {Whidlist = whidlist,Coid = CoID});            
                    if (u > 0)
                    {
                        result.d = false;                 
                    }
                    else
                    {
                        result.d = true;   
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
        ///查询仓库资料List
        ///</summary>
        public static DataResult GetWarehouseList(int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select * from warehouse where type = 0 and coid = " + CoID;
            var res = new List<WarehouseInsert>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    var u = conn.Query<Warehouse>(wheresql).AsList();
                    foreach(var a in u)
                    {
                        var rr = new WarehouseInsert();
                        rr.area = new List<int>(){0,0,0};
                        rr.id = a.id;
                        rr.name0 = a.warehousename;
                        rr.contract = a.contract;
                        rr.phone = a.phone;
                        rr.area[0] = a.logistics;
                        rr.area[1] = a.city;
                        rr.area[2] = a.district;
                        rr.address = a.address;
                        rr.enable = a.enable;
                        wheresql = "select * from warehouse where parentid = " + a.id + " and coid = " + CoID;
                        var lst = conn.Query<Warehouse>(wheresql).AsList();
                        foreach(var b in lst)
                        {
                            if(b.type == 1)
                            {
                                rr.name1 = b.warehousename;
                            }
                            if(b.type == 2)
                            {
                                //rr.name2 = b.warehousename;
                            }
                            if(b.type == 3)
                            {
                                rr.name3 = b.warehousename;
                            }
                            if(b.type == 4)
                            {
                                rr.name4 = b.warehousename;
                            }
                            if(b.type == 5)
                            {
                                rr.name5 = b.warehousename;
                            }
                        }
                        res.Add(rr);
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
        ///仓库启用停用设置
        ///</summary>
        public static DataResult UpdateWarehouseEnable(int whid,string Company,string UserName,int CoID,bool Enable)
        {
            var result = new DataResult(1,null);   
            string contents = string.Empty;
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    if(Enable == false)
                    {
                        var flag = IsWarehouseEnable(whid,CoID);
                        if(bool.Parse(flag.d.ToString()) == false)
                        {
                            result.s = -1;
                            result.d = "仓库有库存，不可停用";
                            return result;
                        }
                    }
                    string uptsql = @"update warehouse set enable = @Enable where (id = @ID or parentid = @ID) and coid = @Coid";
                    var args = new {ID = whid,Enable = Enable,Coid = CoID};          
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        if(Enable)
                        {
                            contents = "仓库状态启用";
                        }
                        else
                        {
                            contents = "仓库状态停用";
                        }
                        CoreUser.LogComm.InsertUserLog("修改仓库资料", "warehouse", contents, UserName, CoID, DateTime.Now);
                    }
                    result.d = contents;           
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return  result;
        }
        ///<summary>
        ///查询主仓库List
        ///</summary>
        public static DataResult GetParentWarehouseList(int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select * from warehouse where parentid = 0 and enable = true"+ " and coid = " + CoID;
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    var u = conn.Query<Warehouse>(wheresql).AsList();
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
        ///根据主仓库代号查询子仓库List
        ///</summary>
        public static DataResult GetChildWarehouseList(int CoID,int parentid)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select * from warehouse where parentid = " +parentid + " and enable = true" + " and coid = " + CoID;
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    var u = conn.Query<Warehouse>(wheresql).AsList();
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