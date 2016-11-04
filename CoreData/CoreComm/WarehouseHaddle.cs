using CoreModels;
using CoreModels.XyComm;
using Dapper;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CoreModels.XyUser;
using CoreData.CoreUser;
using System.Threading.Tasks;

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
            // //检查相关必输栏位
            // if(wh.name0 == null)        
            //     result.s = -3101;            
            // if(wh.name1 == null)            
            //     result.s = -3102;    
            // if(wh.name3 == null)
            // {
            //     result.s = -1;
            //     result.d = "请输入销售退货仓库名称!";
            //     return result;
            // }
            // if(wh.name4 == null)
            // {
            //     result.s = -1;
            //     result.d = "请输入进货仓库名称!";
            //     return result;
            // }
            // if(wh.name5 == null)
            // {
            //     result.s = -1;
            //     result.d = "请输入次品仓库名称!";
            //     return result;
            // }

            // //检查仓库名称是否重复
            // var res = WarehouseHaddle.IsWarehouseExist(wh.name0,CoID);
            // if (bool.Parse(res.d.ToString()) == true)
            // {
            //     result.s = -1;
            //     result.d = "仓库已存在,不允许新增!";
            //     return result;
            // }
            // res = WarehouseHaddle.IsWarehouseExist(wh.name1,CoID);
            // if (bool.Parse(res.d.ToString()) == true)
            // {
            //     result.s = -1;
            //     result.d = "仓库已存在,不允许新增!";
            //     return result;
            // }
            // if(wh.name2 != null)
            // {
            //     res = WarehouseHaddle.IsWarehouseExist(wh.name2,CoID);
            //     if (bool.Parse(res.d.ToString()) == true)
            //     {
            //         result.s = -1;
            //         result.d = "仓库已存在,不允许新增!";
            //         return result;
            //     }
            // }
            // res = WarehouseHaddle.IsWarehouseExist(wh.name3,CoID);
            // if (bool.Parse(res.d.ToString()) == true)
            // {
            //     result.s = -1;
            //     result.d = "仓库已存在,不允许新增!";
            //     return result;
            // }
            // res = WarehouseHaddle.IsWarehouseExist(wh.name4,CoID);
            // if (bool.Parse(res.d.ToString()) == true)
            // {
            //     result.s = -1;
            //     result.d = "仓库已存在,不允许新增!";
            //     return result;
            // }
            // res = WarehouseHaddle.IsWarehouseExist(wh.name5,CoID);
            // if (bool.Parse(res.d.ToString()) == true)
            // {
            //     result.s = -1;
            //     result.d = "仓库已存在,不允许新增!";
            //     return result;
            // }
            // if(result.s != 1)   return result;  //格式验证


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

        public static DataResult InsertWare(string UserName,string Company,int CoID){
            var result = new DataResult(1,null);
            var warehouse = new Warehouse();
            warehouse.parentid = 0;
            warehouse.warehousename = Company;
            warehouse.type = 0;
            warehouse.contract = "";
            warehouse.phone = "";
            warehouse.logistics = 0;
            warehouse.city = 0;
            warehouse.district = 0;
            warehouse.address = "";
            warehouse.enable = true; 
            warehouse.creator = UserName;
            warehouse.createdate = DateTime.Now;
            warehouse.modifier = UserName;
            warehouse.modifydate = DateTime.Now;
            warehouse.coid = CoID;    

            var wh = new WarehouseInsert();
            wh.name1 = Company+"配送1仓1";
            wh.name2 = Company+"配送2仓";
            wh.name3 = Company+"销退1仓2";
            wh.name4 = Company+"进货1仓3";
            wh.name5 = Company+"次品1仓4";

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
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"初始化仓库资料", "warehouse", Company ,UserName, CoID, DateTime.Now);
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
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"初始化仓库资料", "warehouse", wh.name1 ,UserName, CoID, DateTime.Now);
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
                        CoreUser.LogComm.InsertUserLogTran(TransUser,"初始化仓库资料", "warehouse", wh.name2 ,UserName, CoID, DateTime.Now);
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
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"初始化仓库资料", "warehouse", wh.name3 ,UserName, CoID, DateTime.Now);
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
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"初始化仓库资料", "warehouse", wh.name4 ,UserName, CoID, DateTime.Now);
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
                    CoreUser.LogComm.InsertUserLogTran(TransUser,"初始化仓库资料", "warehouse", wh.name5 ,UserName, CoID, DateTime.Now);
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
 
            return result;
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
        public static DataResult UpdateWarehouse(WarehouseInsert wh,string UserName,int CoID)
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
                string wheresql = "select * from warehouse where parentid=0 and coid = " + CoID ;
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
                    wheresql = "select * from warehouse where  coid = " + CoID + " and type = 1" ;
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
                    wheresql = "select * from warehouse where coid = " + CoID + " and type = 2" ;
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
                    wheresql = "select * from warehouse where coid = " + CoID + " and type = 3" ;
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
                    wheresql = "select * from warehouse where coid = " + CoID + " and type = 4" ;
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
                    wheresql = "select * from warehouse where coid = " + CoID + " and type = 5" ;
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
            var res = new List<WarehouseResponse>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    var u = conn.Query<Warehouse>(wheresql).AsList();

                    if(u.Count>0)
                    { 
                        var a = u[0];
                        var rr = new WarehouseResponse();
                        rr.area = new List<int>(){0,0,0};                        
                        rr.name0 = a.warehousename;
                        rr.contract = a.contract;
                        rr.phone = a.phone;
                        rr.area[0] = a.logistics;
                        rr.area[1] = a.city;
                        rr.area[2] = a.district;
                        rr.address = a.address;
                        rr.enable = a.enable;
                        wheresql = "select * from warehouse where coid = " + CoID;
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
                    result.d = res[0];              
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

        ///<summary>
        ///重新生成第三方物流服务号
        ///</summary>
         public static DataResult serviceCodeRebuild (string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.UserConnectString) ){
                try
                {
                    string code =  DateTime.Now.ToString("yyMMddHHmmss")+CoID;                    
                    string sql = "UPDATE Company SET Company.`WareCode` =@Code WHERE Company.ID = @CoID";
                    var rnt = conn.Execute(sql,new {
                        CoID = CoID,
                        Code = code
                    });
                    if(rnt>0){
                        result.s = 1;
                        result.d = new {
                            code = code
                        };
                    }else{
                        result.s = -3103;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        ///<summary>
        /// 申请加入第三方仓储服务
        ///</summary>
         public static DataResult askFor(string CoID,string code,string itremark,string uid,string uname){
            var result = new DataResult(1,null);
            var comRes = CompanyHaddle.GetByWarecode(code);
            var myCom = CompanyHaddle.GetCompanyEdit(int.Parse(CoID)).d as Company;
            if(comRes.s == 1){
                var itCom = comRes.d as Company;
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try
                    {      
                           
                           string  sql = "SELECT ware_third_party.ID FROM ware_third_party where ware_third_party.CoID = "+CoID+" AND ware_third_party.ItCoid = "+itCom.id;
                           var res = conn.Query<int>(sql).AsList();
                           if(res.Count > 0){
                               sql = @"INSERT ware_third_party SET 
                                            ware_third_party.Enable = 1,                                                                   
                                            ware_third_party.Cdate = Now(),                                                          
                                            ware_third_party.CoID = @CoID,
                                            ware_third_party.WareName =@warename,
                                            ware_third_party.ItName = @itname,
                                            ware_third_party.ItCoid = @itcoid,
                                            ware_third_party.ItRemark = @itremark,
                                            ware_third_party.ApplyCoid = @CoID;";
                           }else{
                               sql = @"UPDATE ware_third_party SET 
                                            ware_third_party.Enable = 1,                                                                                                                                                                         
                                            ware_third_party.CoID = @CoID,
                                            ware_third_party.WareName =@warename,
                                            ware_third_party.ItName = @itname,
                                            ware_third_party.ItCoid = @itcoid,
                                            ware_third_party.ItRemark = @itremark,
                                            ware_third_party.ApplyCoid = @CoID;";
                           }
                           
                            var rnt = conn.Execute(sql,new {   
                                itremark = itremark,        
                                CoID = CoID,
                                itcoid = itCom.id,
                                warename = myCom.name,
                                itname = itCom.name                                       
                            });  
                            if(rnt>0){   
                                Task.Factory.StartNew(()=>{
                                    NotifyHaddle.MsgPoint(itCom.name + "申请成为贵公司第三方仓储","3",CoID,CoID,uid,uname);
                                });                                                             
                            }else{
                                result.s = -1;
                            }

                    }
                    catch (Exception e)
                    {
                        result.s = -1;
                        result.d= e.Message; 
                        conn.Dispose();
                    }
                }
            }
            
            return result;
        }

        ///<summary>
        /// 开通分仓
        ///</summary>
         public static DataResult openOtherWare(int itCoid,string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = @"INSERT ware_third_party SET 
                                        ware_third_party.Enable = 2,                                                                   
                                        ware_third_party.Cdate = Now(),                                                          
                                        ware_third_party.CoID = @CoID,
                                        ware_third_party.ItCoid = @itCoid,                   
                                        ware_third_party.ApplyCoid = @CoID;";
                    var rnt = conn.Execute(sql,new {
                                    CoID = CoID,
                                    itCoid = itCoid                     
                                });                                                                                         
                               
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }


        ///<summary>
        /// 获取仓储列表
        ///</summary>
         public static DataResult storageLst(string CoID,string[] contains,string[] status){
            var result = new DataResult(1,null);
            var com = CompanyHaddle.GetCompanyEdit(int.Parse(CoID)).d as Company;
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {            
                    string wh = " WHERE 1 !=1 ";
                    if(contains.Length>0 || status.Length >0){
                        wh = " WHERE 1 = 1";
                    }                        
                    string c = " ";
                    string s = status.Length>0?" AND w.Enable in("+string.Join(",", status)+")":" AND w.Enable in (1,2,3)";
                    string sql = "";              

                    if(string.Join(",", contains).IndexOf("1")>-1){                        
                        c = " AND w.CoID = "+CoID;
                    }
                    if(string.Join(",", contains).IndexOf("2")>-1){
                        c = " AND w.ItCoid = "+CoID+" ";
                    }
                    if(string.Join(",", contains).IndexOf("1")>-1 && string.Join(",", contains).IndexOf("2")>-1 ){
                        c = " AND (w.CoID = "+CoID + " OR w.ItCoid = "+CoID+") ";
                    }


                    sql =@"SELECT 
                            w.ID,w.WareName,w.myremark,w.itremark,w.Enable,w.Source ,w.ItName,w.Mdate,w.CoID,w.ItCoid,w.ApplyCoid
                           FROM 
                            ware_third_party as w "+wh+c+s;
                    Console.WriteLine(sql);                                                      
                    var storageLst = conn.Query<wareThirdParty>(sql).AsList();                    
                    result.d = new {
                        Lst = storageLst,
                        code = com.warecode,
                        coid = CoID
                    };
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

       ///<summary>
        /// 获取仓储列表
        ///</summary>
         public static DataResult wareLst(string CoID){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {                                             
                    string sql ="";                                        
                  
                    sql =@"SELECT 
                            w.ID,w.WareName,w.CoID
                           FROM 
                            ware_third_party as w 
                           WHERE w.Enable = 2 AND   w.CoID ="+CoID;                                                                     
                    var storageLst = conn.Query<wareLst>(sql).AsList();                    
        
                    result.d = new {
                        Lst = storageLst,             
                    };
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }




        ///<summary>
        /// 修改备注
        ///</summary>
        ///<param name="type">1：我方；2：第三方仓储方 </param>
         public static DataResult editRemark(string CoID,string uname,string id,string remark,string uid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = "SELECT ware_third_party.ApplyCoid ,ware_third_party.ItCoid,ware_third_party.MyRemark ,ware_third_party.ItRemark FROM ware_third_party WHERE ware_third_party.ID = "+id;
                    var res = conn.Query<remarkSqlRes>(sql).AsList();
                    if(res.Count>0){
                        string oldRemark = "";
                        var a = res[0];
                        if(CoID != a.ApplyCoid){
                            oldRemark = a.MyRemark;
                            sql = @"UPDATE ware_third_party SET
                                        ware_third_party.myremark = @remark,
                                        ware_third_party.EndMan = @endman,
                                        ware_third_party.Mdate = Now()
                                    WHERE 
                                        ware_third_party.ID = @id;";                                                      
                        }else{
                            oldRemark = a.ItRemark;
                            sql = @"UPDATE ware_third_party SET
                                        ware_third_party.itremark = @remark,
                                        ware_third_party.EndMan = @endman,
                                        ware_third_party.Mdate = Now()
                                    WHERE 
                                        ware_third_party.ID = @id;";                            
                        }
                        var rnt =  conn.Execute(sql,new {
                                id = id,
                                remark = remark,
                                endman = uname
                            });
                        if(rnt>0){
                            result.s = 1;                            
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + "修改备注 "+oldRemark+" => "+remark,"3",res[0].ItCoid,CoID,uid,uname);
                            });    
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + "修改备注 "+oldRemark+" => "+remark,"3",CoID,CoID,uid,uname);
                            });  
                                                                                 
                        }else{
                            result.s = -1;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        ///<summary>
        /// 审核第三方仓储
        ///</summary>
         public static DataResult passThird(string id,string coid,string uname,string uid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = "SELECT ware_third_party.ApplyCoid ,ware_third_party.ItCoid ,ware_third_party.ItName,ware_third_party.WareName FROM ware_third_party WHERE ware_third_party.ID = "+id;
                    var res = conn.Query<remarkSqlRes>(sql).AsList();
                    if(res.Count>0){
                        sql =@"UPDATE ware_third_party SET                               
                                        ware_third_party.Enable = 2,
                                        ware_third_party.EndMan = @endman,
                                        ware_third_party.Pdate = Now(),
                                        ware_third_party.Mdate = Now()
                                    WHERE 
                                        ware_third_party.ID =@id AND ware_third_party.CoID = @CoID;";
                        var rnt = conn.Execute(sql,new {
                                endman = uname,
                                id = id,
                                CoID = coid
                        });       
                        if(rnt > 0){
                            result.s = 1;
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + " 审核通过公司 "+res[0].ItName +" 成为 "+res[0].WareName+" 第三方仓储","3",res[0].ItCoid,coid,uid,uname);
                            });    
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + " 审核通过公司 "+res[0].ItName +" 成为 "+res[0].WareName+" 第三方仓储","3",coid,coid,uid,uname);
                            });  
                        }else{
                            result.s = -1;
                        }
                    }else{
                        result.s = -3008;
                    }
                    
         
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        ///<summary>
        /// 终止合作
        ///</summary>
         public static DataResult wareCancle(string id,string coid,string uname,string uid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = "SELECT ware_third_party.ApplyCoid ,ware_third_party.ItCoid ,ware_third_party.ItName,ware_third_party.WareName FROM ware_third_party WHERE ware_third_party.ID = "+id;
                    var res = conn.Query<remarkSqlRes>(sql).AsList();
                    if(res.Count>0){
                        sql =@"UPDATE ware_third_party SET                               
                                            ware_third_party.Enable = 3,
                                            ware_third_party.EndMan = @endman,
                                            ware_third_party.Pdate = Now(),
                                            ware_third_party.Mdate = Now()
                                        WHERE 
                                            ware_third_party.ID =@id AND (ware_third_party.CoID = @CoID OR ware_third_party.ItCoid = @CoID );";
                        var rnt = conn.Execute(sql,new {                                    
                                        endman = uname,
                                        CoID = coid,
                                        id = id
                                });       
                        if(rnt > 0){
                            result.s = 1;
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + " 终止公司 "+res[0].ItName +" 与 "+res[0].WareName+" 的合作","3",res[0].ItCoid,coid,uid,uname);
                            });    
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + " 终止公司 "+res[0].ItName +" 与 "+res[0].WareName+" 的合作","3",coid,coid,uid,uname);
                            });  
                        }else{
                            result.s = -1;
                        }
                    }else{
                        result.s = -3008;
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        ///<summary>
        /// 取消申请合作
        ///</summary>
         public static DataResult wareGiveUp(string id,string coid,string uname, string uid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = "SELECT ware_third_party.ApplyCoid ,ware_third_party.ItCoid,ware_third_party.ItName,ware_third_party.WareName FROM ware_third_party WHERE ware_third_party.ID = "+id;
                    var res = conn.Query<remarkSqlRes>(sql).AsList();
                    if(res.Count>0){
                        sql =@"UPDATE ware_third_party SET                               
                                            ware_third_party.Enable = 0,
                                            ware_third_party.EndMan = @endman,                                        
                                            ware_third_party.Mdate = Now()
                                        WHERE 
                                            ware_third_party.ID =@id AND ware_third_party.ItCoid = @CoID;";
                        var rnt = conn.Execute(sql,new {                                    
                                        endman = uname,
                                        id = id,
                                        CoID = coid
                                });       
                        if(rnt > 0){
                            result.s = 1;
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + " 取消 "+res[0].ItName +" 成为 "+res[0].WareName+" 第三方仓储申请","3",res[0].ItCoid,coid,uid,uname);
                            });    
                            Task.Factory.StartNew(()=>{
                                NotifyHaddle.MsgPoint(uname + " 取消 "+res[0].ItName +" 成为 "+res[0].WareName+" 第三方仓储申请","3",coid,coid,uid,uname);
                            });  
                        }else{
                            result.s = -1;
                        }
                    }else{
                        result.s = -3008;
                    }

                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }
        ///<summary>
        /// 指定仓库
        ///</summary>
    public static List<wareLst> getWarelist(string CoID){
        var res = new List<wareLst>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = @"SELECT 
                            w.ID,w.WareName,w.CoID
                        FROM 
                            ware_third_party as w 
                        WHERE w.CoID ="+CoID; 
                    res = conn.Query<wareLst>(sql).AsList();                                  
                }
                catch
                {
                    conn.Dispose();
                }
            }

        return res;
    }

    ///<summary>
    /// 限定省份
    ///</summary>
    public static List<AreaAll> getAreaAll(){
        var res = new List<AreaAll>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = @"SELECT  area.ID as value,  area.ParentId ,  area.`Name` as label FROM area WHERE LevelType = 1 AND ParentId = 100000 "; 
                    res = conn.Query<AreaAll>(sql).AsList();                                  
                }
                catch
                {
                    conn.Dispose();
                }
            }

        return res;
    }

    ///<summary>
    /// 指定店铺
    ///</summary>
    public static List<shopEnum> getShopEnum(string CoID){
        var res = new List<shopEnum>();
        using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
            try
            {
                string sql = @"SELECT ID as value ,ShopName as label FROM shop WHERE CoID in(0,"+CoID+");";
                res = conn.Query<shopEnum>(sql).AsList();                                  
            }
            catch
            {
                conn.Dispose();
            }
        }

        return res;
    }

    public static DataResult editploy(string CoID,string id = ""){
            var result = new DataResult(1,null);
            var warelist = new List<wareLst>();
            var province = new List<AreaAll>();
            var shop = new List<shopEnum>();
            Task[] tasks = new Task[3];
            tasks[0] =  Task.Factory.StartNew(()=>{
                warelist = getWarelist(CoID);
            });  
            tasks[1] =   Task.Factory.StartNew(()=>{
                province = getAreaAll();
            }); 
            tasks[2] =  Task.Factory.StartNew(()=>{
                shop = getShopEnum(CoID);
            });
              
            Task.WaitAll(tasks);

            if(string.IsNullOrEmpty(id)){
                result.d = new{
                    warelist = warelist,
                    province = province,
                    shop = shop
                };
            }else{
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try
                    {
                        string sql ="SELECT * FROM wareploy WHERE ID = "+id;                                                                         
                        var res = conn.Query<WarePloy>(sql).AsList();
                        if(res.Count>0){
                            result.d = new{
                                warelist = warelist,
                                province = province,
                                shop = shop,
                                ploy = res[0]
                            };
                        }else{
                            result.d = new{
                                warelist = warelist,
                                province = province,
                                shop = shop
                            };
                        }

                    }
                    catch (Exception e)
                    {
                        result.s = -1;
                        result.d= e.Message; 
                        conn.Dispose();
                    }
                }
            }
            
            return result;
        }

        public static DataResult createploy(string CoID,WarePloy wareploy,string uname){
            var result = new DataResult(1,null);
            if(!string.IsNullOrEmpty(wareploy.ContainGoods)){
                wareploy.ContainGoods = ","+ wareploy.ContainGoods+","; 
            }
            if(!string.IsNullOrEmpty(wareploy.ContainSkus)){
                wareploy.ContainSkus = ","+ wareploy.ContainSkus+","; 
            }
            if(!string.IsNullOrEmpty(wareploy.RemoveGoods)){
                wareploy.RemoveGoods = ","+ wareploy.RemoveGoods+","; 
            }
            if(!string.IsNullOrEmpty(wareploy.RemoveSkus)){
                wareploy.RemoveSkus = ","+ wareploy.RemoveSkus+","; 
            }
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    wareploy.CoID = Convert.ToInt16(CoID);
                    string sql = @"INSERT wareploy SET
                                        wareploy.`Name` = @Name,
                                        wareploy.CoID = @CoID,
                                        wareploy.`Level`= @Level,
                                        wareploy.Wid= @Wid,
                                        wareploy.Province = @Province,
                                        wareploy.Shopid = @Shopid,
                                        wareploy.Did = @Did,
                                        wareploy.ContainGoods= @ContainGoods,
                                        wareploy.RemoveGoods= @RemoveGoods,
                                        wareploy.ContainSkus = @ContainSkus,
                                        wareploy.RemoveSkus = @RemoveSkus,
                                        wareploy.MinNum = @MinNum,
                                        wareploy.MaxNum= @MaxNum,
                                        wareploy.Payment = @Payment;";                                                                         
                     var rnt = conn.Execute(sql,wareploy);
                     if(rnt>0){
                         result.s = 1;
                         LogComm.InsertLog("新增分仓策略：","WareHouse","",uname,Convert.ToInt16(CoID));
                     }else{
                         result.s = -1;
                     }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        public static DataResult modifyploy(string CoID,WarePloy wareploy,string uname){
            var result = new DataResult(1,null);
            string content = "";
        
            if(!string.IsNullOrEmpty(wareploy.ContainGoods)){
                wareploy.ContainGoods = ","+ wareploy.ContainGoods+","; 
            }
            if(!string.IsNullOrEmpty(wareploy.ContainSkus)){
                wareploy.ContainSkus = ","+ wareploy.ContainSkus+","; 
            }
            if(!string.IsNullOrEmpty(wareploy.RemoveGoods)){
                wareploy.RemoveGoods = ","+ wareploy.RemoveGoods+","; 
            }
            if(!string.IsNullOrEmpty(wareploy.RemoveSkus)){
                wareploy.RemoveSkus = ","+ wareploy.RemoveSkus+","; 
            }
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var res = conn.Query<WarePloy>("SELECT * FROM wareploy WHERE ID = "+wareploy.ID).AsList();
                    if(res.Count>0){
                        var oldPloy = res[0];
                        if(oldPloy.ContainGoods != wareploy.ContainGoods){
                            content += " 包含货号："+wareploy.ContainGoods +" => "+ oldPloy.ContainGoods+",";
                        }
                        if(oldPloy.ContainSkus != wareploy.ContainSkus){
                            content += " 包含商品："+wareploy.ContainSkus +" => "+ oldPloy.ContainSkus+",";
                        }
                        if(oldPloy.Did != wareploy.Did){
                            content += " 限定分销商："+wareploy.Did +" => "+ oldPloy.Did+",";
                        }
                        if(oldPloy.Level != wareploy.Level){
                            content += " 优先级："+wareploy.Level +" => "+ oldPloy.Level+",";
                        }
                        if(oldPloy.MaxNum != wareploy.MaxNum){
                            content += " 商品最大数："+wareploy.MaxNum +" => "+ oldPloy.MaxNum+",";
                        }
                        if(oldPloy.MinNum != wareploy.MinNum){
                            content += " 商品最小数："+wareploy.MinNum +" => "+ oldPloy.MinNum+",";
                        }
                        if(oldPloy.Name != wareploy.Name){
                            content += " 策略名："+wareploy.Name +" => "+ oldPloy.Name+",";
                        }
                        if(oldPloy.Payment != wareploy.Payment){
                            content += " 付款方式："+wareploy.Payment +" => "+ oldPloy.Payment+",";
                        }
                        if(oldPloy.Province != wareploy.Province){
                            content += " 省份："+wareploy.Province +" => "+ oldPloy.Province+",";
                        }
                        if(oldPloy.RemoveGoods != wareploy.RemoveGoods){
                            content += " 排除货号："+wareploy.RemoveGoods +" => "+ oldPloy.RemoveGoods+",";
                        }
                        if(oldPloy.RemoveSkus != wareploy.RemoveSkus){
                            content += " 排除商品："+wareploy.RemoveSkus +" => "+ oldPloy.RemoveSkus+",";
                        }
                        if(oldPloy.Shopid != wareploy.Shopid){
                            content += " 店铺："+wareploy.Shopid +" => "+ oldPloy.Shopid+",";
                        }
                        if(oldPloy.Wid != wareploy.Wid){
                            content += " 仓位："+wareploy.Wid +" => "+ oldPloy.Wid+",";
                        }                       
                        
                        wareploy.CoID = Convert.ToInt16(CoID);
                        string sql = @"UPDATE wareploy SET
                                            wareploy.`Name` = @Name,                                        
                                            wareploy.`Level`= @Level,
                                            wareploy.Wid= @Wid,
                                            wareploy.Province = @Province,
                                            wareploy.Shopid = @Shopid,
                                            wareploy.Did = @Did,
                                            wareploy.ContainGoods= @ContainGoods,
                                            wareploy.RemoveGoods= @RemoveGoods,
                                            wareploy.ContainSkus = @ContainSkus,
                                            wareploy.RemoveSkus = @RemoveSkus,
                                            wareploy.MinNum = @MinNum,
                                            wareploy.MaxNum= @MaxNum,
                                            wareploy.Payment = @Payment
                                        WHERE
                                            wareploy.CoID = @CoID AND wareploy.ID = @ID ;";                                                                         
                        var rnt = conn.Execute(sql,wareploy);
                        if(rnt>0){
                            result.s = 1;
                            LogComm.InsertLog("更新ID："+ wareploy.ID+" 分仓策略：","WareHouse",content,uname,Convert.ToInt16(CoID));
                        }else{
                            result.s = -1;
                        }
                     }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }

        ///<summary>
        /// 获取第三方仓储服务设置
        ///</summary>
         public static DataResult wareSettingGet(string coid){
            var result = new DataResult(1,null);
            var setting = CacheBase.Get<ware_setting>("waresettingh"+coid);
            if(setting == null){
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try
                    {
                        var res = conn.Query<ware_setting>("SELECT * FROM ware_setting WHERE CoID = "+coid).AsList();
                        if(res.Count >0){
                            setting =res[0]; 
                            CacheBase.Set<ware_setting>("waresettingh"+coid,setting);
                        }else{
                            result.s= -3010;
                        }
                    }
                    catch (Exception e)
                    {
                        result.s = -1;
                        result.d= e.Message; 
                        conn.Dispose();
                    }
                }
            }
            result.d = setting;
            return result;
        }

        public static DataResult modifyWareSetting(ware_setting setting,string coid){
            var result = new DataResult(1,null);
            var oldset = wareSettingGet(coid).d as ware_setting;
            string content = "";


            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    

                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;

        }








        ///<summary>
        /// 
        ///</summary>
         public static DataResult demo(){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
        

                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message; 
                    conn.Dispose();
                }
            }
            return result;
        }










    }
}