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
        public static DataResult InsertWarehouse(Warehouse wh,string UserName,string Company,int CoID)
        {
            var result = new DataResult(1,"资料新增成功!");   
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string sqlCommandText = @"INSERT INTO warehouse(warehouseid,warehousename,enable,creator,coid,parentid,type) VALUES(
                            @Warehouseid,@Warehousename,@Enable,@UName,@Coid,@Parentid,@Type)";
                    var args = new {Warehouseid = wh.warehouseid,Warehousename = wh.warehousename,Enable=wh.enable,UName = UserName,
                                    Coid =CoID, Parentid = wh.parentid,Type = wh.type};
                    int count =conn.Execute(sqlCommandText,args);
                    if(count < 0)
                    {
                        result.s = -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLog("新增仓库资料", "warehouse", "新增仓库" + wh.warehousename ,UserName, Company, DateTime.Now);
                        string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where warehousename ='" + wh.warehousename + "' and coid = " + CoID ;
                        var u = conn.Query<Warehouse>(wheresql).AsList();
                        if (u.Count > 0)
                        {
                            CacheBase.Set<Warehouse>("warehouse" + u[0].id.ToString(), u[0]);
                        }
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
        public static DataResult UpdateWarehouse(Warehouse wh,string UserName,string Company,int CoID)
        {
            var result = new DataResult(1,"资料更新成功!");  
            string contents = string.Empty; 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where warehousename ='" + wh.warehousename + "' and coid = " + CoID ;
                    var u = conn.Query<Warehouse>(wheresql).AsList();
                    if(wh.warehouseid != u[0].warehouseid)
                    {
                        contents = contents + "仓库代号" + ":" +u[0].warehouseid + "=>" + wh.warehouseid + ";";
                    }
                    if(wh.enable != u[0].enable)
                    {
                        contents = contents + "启用状态" + ":" +u[0].enable + "=>" + wh.enable + ";";
                    }
                    if(wh.warehousename != u[0].warehousename)
                    {
                        contents = contents + "仓库名称" + ":" +u[0].warehousename + "=>" + wh.warehousename + ";";
                    }
                    if(wh.parentid != u[0].parentid)
                    {
                        contents = contents + "主仓库代号" + ":" +u[0].parentid + "=>" + wh.parentid + ";";
                    }
                    if(wh.type != u[0].type)
                    {
                        contents = contents + "仓库类型" + ":" +u[0].type + "=>" + wh.type + ";";
                    }
                    string uptsql = @"update warehouse set warehouseid = @Warehouseid,enable = @Enable,warehousename = @Warehousename,parentid=@Parentid,type=@Type where id = @ID";
                    var args = new {Warehouseid = wh.warehouseid,Enable=wh.enable,Warehousename = wh.warehousename,Parentid = wh.parentid,Type = wh.type,ID = wh.id};
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        CoreUser.LogComm.InsertUserLog("修改仓库资料", "warehouse", contents, UserName, Company, DateTime.Now);               
                        CacheBase.Set<Warehouse>("warehouse" + wh.id.ToString(), wh);
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
        ///查询单笔仓库
        ///</summary>
        public static DataResult GetWarehouseSingle(int ID)
        {
            var result = new DataResult(1,null);        
            var parent = CacheBase.Get<Warehouse>("warehouse" + ID.ToString());  
            if (parent == null)
            {
                using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                    try{
                        string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where id ='" + ID.ToString() + "'" ;
                        var u = conn.Query<Warehouse>(wheresql).AsList();
                        if (u.Count == 0)
                        {
                            result.s = -3001;
                            result.d = null;
                        }
                        else
                        {
                            CacheBase.Set<Warehouse>("warehouse" + ID.ToString(), u[0]);
                            result.d = u;
                        }
                    }catch(Exception ex){
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
        ///检查仓库是否可停用
        ///</summary>
        public static DataResult IsWarehouseEnable(int whid,int CoID)
        {
            var result = new DataResult(1,null);   
            using(var conn = new MySqlConnection(DbBase.CoreConnectString) ){
                try{
                    string wheresql = "select count(*) from inventory where warehouseid = " + whid + " and coid = " + CoID + " and (stockqty > 0 or defectiveqty > 0)";
                    int u = conn.QueryFirst<int>(wheresql);            
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
        public static DataResult GetWarehouseList(WarehouseParm cp)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where 1 = 1";
            if(cp.CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + cp.CoID;
            }
            if(!string.IsNullOrEmpty(cp.Enable) && cp.Enable.ToUpper()!="ALL")//是否启用
            {
                wheresql = wheresql + " AND enable = "+ (cp.Enable.ToUpper()=="TRUE"?true:false);
            }
            if(!string.IsNullOrEmpty(cp.Filter))//过滤条件
            {
               wheresql = wheresql + " and warehousename like '%"+ cp.Filter +"%'";
            }
            if(!string.IsNullOrEmpty(cp.SortField)&& !string.IsNullOrEmpty(cp.SortDirection))//排序
            {
                wheresql = wheresql + " ORDER BY "+cp.SortField +" "+ cp.SortDirection;
            }
            var res = new WarehouseData();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    var u = conn.Query<Warehouse>(wheresql).AsList();
                    int count = u.Count;
                    decimal pagecnt = Math.Ceiling(decimal.Parse(count.ToString())/decimal.Parse(cp.NumPerPage.ToString()));

                    int dataindex = (cp.PageIndex - 1)* cp.NumPerPage;
                    wheresql = wheresql + " limit " + dataindex.ToString() + " ," + cp.NumPerPage.ToString();
                    u = conn.Query<Warehouse>(wheresql).AsList();

                    res.Datacnt = count;
                    res.Pagecnt = pagecnt;
                    res.Warehouse = u;
                    if (count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        result.d = res;
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
        ///仓库启用停用设置
        ///</summary>
        public static DataResult UpdateWarehouseEnable(Dictionary<int,string> IDsDic,string Company,string UserName,int CoID,bool Enable)
        {
            var result = new DataResult(1,null);   
            string contents = string.Empty;
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    if(Enable == false)
                    {
                        string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where id in @ID and coid = @Coid" ;
                        var argss = new {ID = IDsDic.Keys.AsList(),Coid = CoID};   
                        var u = conn.Query<Warehouse>(wheresql,argss).AsList();
                        if(u.Count > 0)
                        {
                            List<int> whidlist = new List<int>();
                            foreach(var a in u)
                            {
                                whidlist.Add(a.warehouseid);
                            }   
                            wheresql = "select count(*) from inventory where warehouseid in @Whid and coid = @Coid and (stockqty > 0 or defectiveqty > 0)";
                            int cnt = DbBase.CoreDB.QueryFirst<int>(wheresql,new {Whid = whidlist,Coid = CoID});
                            if(cnt > 0)
                            {
                                result.s = -1;
                                result.d = "仓库有库存，不可停用";
                                return result;
                            }
                        }
                    }
                    string uptsql = @"update warehouse set enable = @Enable where id in @ID and coid = @Coid";
                    var args = new {ID = IDsDic.Keys.AsList(),Enable = Enable,Coid = CoID};          
                    int count = conn.Execute(uptsql,args);
                    if(count < 0)
                    {
                        result.s= -3003;
                    }
                    else
                    {
                        if(Enable)
                        {
                            contents = "仓库状态启用：";
                        }
                        else
                        {
                            contents = "仓库状态停用：";
                        }
                        contents+= string.Join(",", IDsDic.Values.AsList().ToArray());
                        CoreUser.LogComm.InsertUserLog("修改仓库资料", "warehouse", contents, UserName, Company, DateTime.Now);
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
        ///查询所有有效的仓库资料
        ///</summary>
        public static DataResult GetWarehouseAll(int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where enable = true";
            if(CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + CoID;
            }
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{    
                    var u = conn.Query<Warehouse>(wheresql).AsList();
                    if (u.Count == 0)
                    {
                        result.s = -3001;
                        result.d = null;
                    }
                    else
                    {
                        result.d = u;
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
        ///查询主仓库List
        ///</summary>
        public static DataResult GetParentWarehouseList(int CoID)
        {
            var result = new DataResult(1,null);     
            string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where parentid = 0 and enable = true";
            if(CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + CoID;
            }
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
            string wheresql = "select id,warehouseid,warehousename,enable,parentid,type,creator,createdate from warehouse where parentid = " +parentid + " and enable = true";
            if(CoID != 1)//公司编号
            {
                wheresql = wheresql + " and coid = " + CoID;
            }
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