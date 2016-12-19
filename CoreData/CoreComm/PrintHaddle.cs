using CoreModels;
using Dapper;
using System;
using CoreModels.XyComm;
using CoreData;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using CoreData.CoreComm;
using System.Threading.Tasks;

namespace CoreDate.CoreComm
{
    public static class PrintHaddle
    {
        
         /// <summary>
		/// 获取print_sys_types 单条数据
		/// </summary>
        public static DataResult GetSysesType(string id){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    var list = conn.Query<print_sys_types>(@"SELECT * FROM
                                                                print_sys_types as a 
                                                            WHERE 
                                                                 a.deleted = FALSE AND a.id = @id",new {id = id}).AsList();  
                    if(list.Count>0){
                        result.d = list[0];
                    }else{
                        result.s = -4023;
                    }                                                         
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }


        public static DataResult GetSyses(string id){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    var list = conn.Query<print_syses>(@"SELECT * FROM
                                                             print_syses as a 
                                                        WHERE   a.id = @id",new {id = id}).AsList();   
                    if(list.Count>0){
                        result.d = list[0];
                    }else{
                        result.s = -4008;
                    }                                     
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }


        /// <summary>
		/// 获取用户模板 print_uses 单条数据
		/// </summary>
        public static DataResult GetUses(string id){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    string sql = @"SELECT * FROM 
                                        print_uses AS a 
                                   WHERE 
                                        a.deleted = FALSE AND a.id = @id";
                    
                    var list = conn.Query<print_uses>(sql,new {id = id}).AsList();   
                    if(list.Count>0){
                        result.d = list[0];
                    }else{
                        result.s = -4008;
                    }                                     
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }
            } 
            return result;
        }

        


        #region 
        /// <summary>
		/// 获取print_sys_types -> emu_data
		/// </summary>
        public static DataResult taskData(string type)
        {
            var result = new DataResult(1,null);    
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var list = conn.Query<print_sys_types>(@"SELECT 
                                                                print_sys_types.emu_data 
                                                            FROM 
                                                                print_sys_types 
                                                            WHERE  
                                                                print_sys_types.deleted =FALSE AND  print_sys_types.type =@type",new {type = type}).AsList();
                    if (list.Count >0)
                    {                   
                        result.d = new{ emu_data = JsonConvert.DeserializeObject<dynamic>(list[0].emu_data) };                                  
                    }else {
                        result.s = -4001;                   
                    }               
                }
                catch (Exception e){                
                    result.s = -1;
                    result.d= e.Message;     
                    conn.Dispose();           
                }
            }
           
            return result;
        }
        #endregion

        #region 
        /// <summary>
		/// 获取个人模板 print_uses
		/// </summary>
        public static DataResult taskTpl(string admin_id,string my_id){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var my = conn.Query<print_uses>(@"SELECT * FROM 
                                                            print_uses as a
                                                      WHERE 
                                                            a.id = @my_id AND a.admin_id =@admin_id",new {my_id = my_id ,admin_id=admin_id}).AsList();
                    if (my.Count >0 ) {  
                        result.d = new{
                            currentTplID = int.Parse(my_id),
                            tpl_name = my[0].name,
                            sys_id = my[0].sys_id,
                            states = JsonConvert.DeserializeObject<dynamic>(my[0].tpl_data),
                            print_setting = JsonConvert.DeserializeObject<dynamic>(my[0].print_setting),
                            type = my[0].type,
                            tpls = conn.Query<usesModel>(@"SELECT
                                                             a.id,a.name 
                                                           FROM 
                                                             print_uses as a 
                                                           WHERE 
                                                             a.type = "+my[0].type+" ").AsList(),
                            print_ori = "http://localhost:8000/CLodopfuncs.js?priority=1"
                        };      
                    }else{
                        result.s = -4002;
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
        #endregion

        #region 
         /// <summary>
		/// 设定，更新默认模板
		/// </summary>
        public static DataResult sideSetdefed(string admin_id,string my_tpl_id,string type,string coid){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var oldmodel = conn.Query<print_use_setting>(@"SELECT * FROM 
                                                                    print_use_setting as a 
                                                                  WHERE 
                                                                    a.admin_id = "+admin_id + " AND a.type="+type).AsList();
                    int rnt = 0;
                    if (oldmodel.Count > 0){                   
                        rnt = conn.Execute(@"UPDATE print_use_setting SET 
                                                print_use_setting.admin_id = @admin_id ,
                                                print_use_setting.defed_id = @defed_id,
                                                print_use_setting.type = @type   
                                            WHERE 
                                                print_use_setting.id = @id 
                                            AND 
                                                print_use_setting.coid = @coid ;",new {
                                                    admin_id=admin_id,
                                                    defed_id =my_tpl_id,
                                                    id = oldmodel[0].id,
                                                    type = type,
                                                    coid = coid});
                        if (rnt > 0){
                        result.s = 1;
                        }else{
                            result.s = -4003;
                        }
                    }else {
                        rnt = conn.Execute(@"INSERT INTO 
                                                print_use_setting(
                                                    print_use_setting.admin_id,
                                                    print_use_setting.defed_id,
                                                    print_use_setting.type = @type,
                                                    print_use_setting.coid) 
                                            VALUES(@admin_id,@my_tpl_id,@coid)",new {
                                                admin_id = admin_id,
                                                my_tpl_id=my_tpl_id,
                                                type = type,
                                                coid = coid});
                        if (rnt > 0){
                            result.s = 1;
                        }else{
                            result.s = -4004;
                        }
                    }                
                }catch (Exception e){
                    result.s = -1;
                    result.d= e.Message;
                    conn.Dispose();
                }   
            }   
            return result;      
        }
        #endregion

        #region 
        /// <summary>
		/// 设置lodop_target
		/// </summary>
        public static DataResult setLodop(string admin_id,string lodop_target){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = @"SELECT * FROM 
                                        print_use_setting as a 
                                   WHERE a.admin_id = "+admin_id;
                    Console.WriteLine(sql);
                    var oldmodel = conn.Query<print_use_setting>(sql).AsList();
                    int rnt = 0;
                    if (oldmodel.Count>0){        
                        sql = @"UPDATE print_use_setting SET 
                                    print_use_setting.admin_id = "+admin_id+
                                    " ,print_use_setting.lodop_target = '"+lodop_target+
                                    "' WHERE print_use_setting.id = "+oldmodel[0].id;      
                        rnt = conn.Execute(sql);
                        if (rnt > 0){
                        result.s = 1;
                        }else{
                            result.s = -4025;
                        }
                    }else {
                        sql = @"INSERT INTO 
                                    print_use_setting(print_use_setting.admin_id,print_use_setting.lodop_target) 
                                    VALUES("+admin_id+",'"+lodop_target+"')";
                        Console.WriteLine(sql);
                        rnt = conn.Execute(sql);
                        if (rnt > 0){
                            result.s = 1;
                        }else{
                            result.s = -4025;
                        }
                    }                
                }catch (Exception e){
                    result.s = -1;
                    result.d= e.Message;
                    conn.Dispose();
                }   
            }   
            return result;      
        }
        #endregion



        #region 
        /// <summary>
		/// 移除用户模板
		/// </summary>
        public static DataResult sideRemove(string ids,string coid){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var oldmoder = conn.Query<print_use_setting>(@"SELECT a.id FROM 
                                                                        print_use_setting as a 
                                                                    WHERE a.defed_id in( "+ids+")").AsList();                                        
                    if (oldmoder!=null) {
                        result.s = -4005;
                    }
                    string sql = "";
                    var idsArr = ids.Split(','); 
                    foreach(string my_tpl_id in idsArr){
                        sql += @"UPDATE print_uses SET 
                                    print_uses.deleted = TRUE 
                                WHERE print_uses.id = "+my_tpl_id+" AND print_uses.coid="+coid;
                    } 
                    int rnt = conn.Execute(sql);
                    if (rnt > 0)
                    {
                        result.s = 1;
                    }else{
                        result.s = -4006;
                    }                
                }catch (Exception e)
                {
                    result.s = -1;
                    result.d= e.Message;
                    conn.Dispose();
                }     
            }
            return result;       
        }
        #endregion
        /// <summary>
		/// 获取print_sys_types list
		/// </summary>
        public static DataResult getAllSysTypes(){
            var result = new DataResult(1,null);        
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    var list = conn.Query<AllSysTypes>(@"SELECT a.id, a.`name`,a.type 
                                                            FROM 
                                                                print_sys_types as a 
                                                            WHERE  a.deleted = FALSE  ").AsList();                 
                    if (list != null)
                    {
                    result.d = list;                 
                    }else{
                        result.s=-4007;
                    }              
                }catch(Exception ex){
                    result.s = -1;
                    result.d = ex.Message;
                    conn.Dispose();
                }

            } 
            return result;
        }

        #region 
        /// <summary>
		/// 获取系统模板 print_syses
		/// </summary>
        public static DataResult tplSys(string sys_id){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var sql = @"SELECT 
                                    a.tpl_data,
                                    a.type,
                                    a.`name` 
                                FROM 
                                    print_syses as a 
                                WHERE 
                                    a.deleted = FALSE AND  a.id = "+sys_id;                    
                    var sysList = conn.Query<print_syses>(sql).AsList();                    
                    if(sysList.Count == 0){
                        result.s = -4008;
                    }else{  
                        var sys = sysList[0];          
                        var type = conn.Query<print_sys_types>(@"SELECT 
                                                                    a.setting,
                                                                    a.presets 
                                                                FROM 
                                                                    print_sys_types as a 
                                                                WHERE   
                                                                    a.deleted =FALSE AND a.type = "+sys.type).AsList()[0];
                        if (type == null){
                            result.s = -4009;
                        }else{
                            result.d = new
                            {
                                currentTplID = 0,
                                states = JsonConvert.DeserializeObject<dynamic>(sys.tpl_data),
                                presets = type.presets!=null? JsonConvert.DeserializeObject<dynamic>(type.presets):"",
                                print_setting = type.setting!=null? JsonConvert.DeserializeObject<dynamic>(type.setting):"",
                                type = sys.type,
                                tpl_name = sys.name //+ DateTime.Now.ToString("d")
                            };
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
        #endregion


        /// <summary>
		/// 
		/// </summary>
        public static DataResult tplMy(string admin_id,string my_id){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = @"SELECT 
                                        a.`name`,
                                        a.sys_id,
                                        a.tpl_data,
                                        a.print_setting,
                                        a.type,b.lodop_target 
                                    FROM 
                                        print_uses as a 
                                    LEFT JOIN 
                                        print_use_setting as b 
                                    on 
                                        b.admin_id = a.admin_id 
                                    WHERE 
                                        a.id = "+my_id+" AND a.admin_id = "+admin_id;                                                                                                               
                    var my = conn.Query<singalUsesModel>(sql).AsList();

                    if (my.Count == 0){
                        result.s = -4002;
                    }else{
                        var type = conn.Query<print_sys_types>(@"SELECT * FROM 
                                                                    print_sys_types as a 
                                                                WHERE 
                                                                    a.deleted=FALSE AND a.type="+my[0].type).AsList();
                        if(type.Count>0){
                            result.d = new
                            {
                                currentTplID = my_id,
                                tpl_name = my[0].name,
                                sys_id = my[0].sys_id,
                                states = JsonConvert.DeserializeObject<dynamic>(my[0].tpl_data),
                                presets = JsonConvert.DeserializeObject < dynamic >(type[0].presets),
                                print_setting = JsonConvert.DeserializeObject<dynamic>(my[0].print_setting),
                                type = my[0].type,
                               lodop_target = my[0].lodop_target
                            };  
                        }else{
                            result.s = -4010;
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

        /// <summary>
		/// 
		/// </summary>
        public static DataResult GetSysesByType(printParam param){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string wheresql = " a.deleted = FALSE AND ";
                    string totalsql = ""; 
                    var totallist = new List<printSysesList>();
                    if(!string.IsNullOrEmpty(param.Filter)){
                        wheresql += param.Filter;
                    }   
                    if(!string.IsNullOrEmpty(param.SortField)&& !string.IsNullOrEmpty(param.SortDirection))//排序
                    {
                        wheresql += " ORDER BY "+param.SortField +" "+ param.SortDirection;
                    }
                    if(param.PageIndex == 1){//pageindex 不为 1 时，不再传total 
                        totalsql = @"SELECT 
                                        a.id, 
                                        a.`name`,
                                        a.mtime 
                                    FROM 
                                        print_syses as a 
                                    WHERE  "+wheresql;
                        totallist = conn.Query<printSysesList>(totalsql).AsList();
                    }
                                    
                    if(param.PageIndex>-1&&param.PageSize>-1){
                        wheresql += " limit "+(param.PageIndex -1)*param.PageSize +" ,"+ param.PageIndex*param.PageSize;
                    }

                    wheresql =@"SELECT 
                                    a.id, 
                                    a.`name`,
                                    a.mtime 
                                FROM 
                                    print_syses as a 
                                WHERE  "+wheresql; 

                    var list = conn.Query<printSysesList>(wheresql).AsList();

                    if (list != null)
                    {
                        if(param.PageIndex == 1){
                            result.d = new {
                                list = list,
                                page = param.PageIndex,
                                pageSize = param.PageSize,
                                pageTotal =  Math.Ceiling(decimal.Parse(totallist.Count.ToString())/decimal.Parse(param.PageSize.ToString())),
                                total = totallist.Count
                            };
                        }else{
                            result.d = new {
                                list = list,
                                page = param.PageIndex,
                            };
                        }                    
                    }
                    else
                    {
                        result.s = -4007;
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


        /// <summary>
		/// 获取类型预设
		/// </summary>
        public static DataResult tplType(string t){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var type = conn.Query<print_sys_types>(@"SELECT * FROM 
                                                                print_sys_types as a 
                                                            WHERE 
                                                                a.deleted=FALSE AND a.type="+t).AsList();
                    if(type.Count == 0){
                        result.s = -4010;
                    }else{
                        result.d = new
                        {
                            presets = JsonConvert.DeserializeObject<dynamic>(type[0].presets),           
                            print_setting = JsonConvert.DeserializeObject<dynamic>(type[0].setting),       
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
            return result;
        }

        
        public static DataResult DelSysesTypeByID(string ids,string coid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    // var hasSys = DbBase.CommDB.Query<print_syses>("").AsList()[0]; //判断是否在预设模板中被调用
                    // if (hasSys != null)
                    // {
                    //    result.s = -4011;
                    // }else {
                    //     int rnt = DbBase.CommDB.Execute("");
                    //     if (rnt > 0)
                    //     {
                    //         result.s = 2;
                    //     }
                    //     else
                    //     {
                    //         result.s = -4010;                        
                    //     }
                    // }     
                    string sql = "";
                    var idsArr = ids.Split(','); 
                    foreach(string id in idsArr){
                       sql += @"UPDATE 
                                    print_sys_types 
                                SET 
                                    print_sys_types.deleted = TRUE 
                                WHERE 
                                    print_sys_types.id="+id+" AND print_sys_types.deleted=FALSE AND print_sys_types.coid = "+coid+";";
                    }

                    int rnt = DbBase.CommDB.Execute(sql);
                    if (rnt > 0)
                    {
                        result.s = 4002;
                    }
                    else
                    {
                        result.s = -4010;                        
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

        /// <summary>
		/// 
		/// </summary>
        public static DataResult DelSysesByID(string ids,string coid){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
   
                    string sql = "";
                    var idsArr = ids.Split(','); 
                    foreach(string id in idsArr){
                       sql += @"UPDATE 
                                    print_syses 
                                SET 
                                    print_syses.deleted = TRUE 
                                WHERE 
                                    print_syses.id="+id+" AND print_syses.deleted=FALSE AND print_syses.coid="+coid+";";
                    }


                    int rnt = DbBase.CommDB.Execute(sql);
                    if (rnt > 0)
                    {
                        result.s = 4002;
                    }
                    else
                    {
                        result.s = -4010;                        
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

      

        /// <summary>
		/// 保存预设系统模板
		/// </summary>
        public static DataResult saveSysesType(int id,string name,dynamic presets,dynamic emu_data,dynamic setting,string coid){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var current = GetSysesType(id.ToString()).d as print_sys_types;
                    string sql = "";
                    int rnt =0;
                    if(current!=null){ //更新
                        sql = @"UPDATE print_sys_types SET 
                                    print_sys_types.emu_data = @emu_data,
                                    print_sys_types.`name` = @name,
                                    print_sys_types.presets = @presets,
                                    print_sys_types.setting = @setting 
                                WHERE 
                                    print_sys_types.id = @id AND print_sys_types.coid=@coid";
                        rnt = conn.Execute(sql,new { 
                                     emu_data = emu_data.ToString(),
                                     name = name,
                                     presets = presets.ToString(),
                                     setting = setting.ToString(),
                                     id = id.ToString(),
                                     coid = coid
                                });
                        if(rnt>0){
                            result.s= 4006;
                            result.d = new {
                                id = id,
                                name = name,
                                type = current.type
                            };
                        }else{
                            result.s = -4018;
                        }
                    }else{ //新增
                        sql = "SELECT MAX(print_sys_types.type) FROM print_sys_types;";
                        int type = conn.Query<int>(sql).AsList()[0]+1;
                        sql = @"INSERT INTO 
                                    print_sys_types( 
                                        print_sys_types.type,
                                        print_sys_types.emu_data,
                                        print_sys_types.`name`,
                                        print_sys_types.presets,
                                        print_sys_types.setting,
                                        print_sys_types.coid)"+
                                "VALUES(@type,@emu_data,@name,@presets,@setting,@coid);"+
                                "SELECT LAST_INSERT_ID() as lastid;";
                        rnt = conn.Query<int>(sql,new { 
                                     type = type.ToString(),
                                     emu_data = emu_data.ToString(),
                                     name = name,
                                     presets = presets.ToString(),
                                     setting = setting.ToString(),
                                     coid = coid
                                }).AsList()[0];                                               
                        conn.Execute("UPDATE print_sys_types SET print_sys_types.type = "+type+" WHERE print_sys_types.id = "+rnt.ToString()+" AND print_sys_types.coid="+coid);                        
                        
                        if(rnt>0){
                            result.s= 4005;
                            result.d = new{
                                id = rnt,
                                name = name,
                                type = type
                            };
                        }else{
                            result.s = -4017;
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

        /// <summary>
		/// 保存系统模板 print_syses
		/// </summary>
        public static DataResult saveSyses(int sys_id, string type,dynamic state, string name,string coid){
            
            var result = new DataResult(1,null);            
            if(GetSysesType(type).d == null){ result.s=-4001;  return result;}            
            using(var conn = new MySqlConnection(DbBase.CommConnectString)){
                try
                {
                   
                    var tpl = state;
                    var stateObj = JsonConvert.DeserializeObject<dynamic>(state);
                    var setting = new{
                        pageW = stateObj.setting.pageW,
                        pageH = stateObj.setting.pageH
                    };
                    int rnt = 0;                    
                    string sql = "";
                    if (sys_id > 0){//更新
                        sql = @"UPDATE print_syses SET print_syses.`name` = @name,
                                    print_syses.setting = @setting,
                                    print_syses.tpl_data = @tpl_data,
                                    print_syses.type = @type,
                                    print_syses.mtime = NOW() 
                                WHERE 
                                    print_syses.id =@id AND print_syses.coid=@coid;";
                        rnt = conn.Execute(sql,new { 
                                name = name,
                                setting = setting.ToString(),
                                tpl_data = tpl.ToString(),
                                type = type,
                                id = sys_id,
                                coid = coid
                        });
                        if (rnt>0) {
                            result.d = new 
                            {
                                sys_id = sys_id,
                            };
                            result.s = 4008;
                        }else
                        {
                            result.s = -4020;
                        }
                    }
                    else {
                        if(name.Equals("新模板")){
                             name = name+DateTime.Now.ToString("d");
                        }
                         sql=@"INSERT INTO print_syses(print_syses.type,print_syses.`name`,print_syses.setting,print_syses.tpl_data,print_syses.mtime,print_syses.coid) 
                              VALUES(@type,@name,@setting,@tpl_data,NOW(),@coid);
                              SELECT LAST_INSERT_ID() as lastid;";            
                        rnt = conn.Query<int>(sql,new {
                                type = type,
                                name = name,
                                setting = setting.ToString(),
                                tpl_data = tpl.ToString(),
                                coid = coid
                        }).AsList()[0];   
                  

                        if (rnt>0) {
                            result.d = new
                            {
                                sys_id = rnt,
                            };
                            result.s = 4007;
                        }else
                        {
                            result.s = -4019;
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


        /// <summary>
		/// 保存个人模板
		/// </summary>
        public static DataResult postSaveMy(string admin_id,string my_id, string sys_id, string type, string name, dynamic print_setting, dynamic state,string lodop_target,string coid){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    if (string.IsNullOrEmpty(name)){ 
                        result.s = -4012;
                    }else{
                        if (state.setting == null || (state == null && print_setting == null)){
                          result.s = -4013;  
                        } else{
                            print_setting.pageW = state.setting.pageW;
                            print_setting.pageH = state.setting.pageH;
                            string sql = "";
                            if (int.Parse(my_id) > 0) 
                            { //my_id >0 为更新                                
                                if (GetUses(my_id).d as print_uses == null){
                                    result.s = -4015;
                                }else{                                          
                                    sql = @"UPDATE print_uses SET 
                                                print_uses.mdate = NOW(),
                                                print_uses.`name` = @name,
                                                print_uses.print_setting = @setting,print_uses.tpl_data = tpl_data 
                                            WHERE print_uses.id = @id AND print_uses.coid = @coid";

                                    int rnt = conn.Execute(sql ,new {
                                                name = name,
                                                print_setting = print_setting,
                                                tpl_data = state,
                                                id = my_id,
                                                coid = coid
                                            });
                                    if (rnt > 0){
                                        result.d = new {
                                            my_id = int.Parse(my_id),
                                            name = name,
                                            type = type,
                                            id = my_id,
                                            coid = coid
                                        };
                                        //保存 lodop_target
                                        if(setLodop(admin_id,lodop_target).s == -4025){
                                            result.s = -4025;
                                        }else{
                                            result.s = 4004;
                                        }
                                        
                                    }else {
                                        result.s = -4016;
                                    }                        
                                }                                
                            }else{//新增                                                                                            
                                if(type == "0"){
                                    result.s = -4023;
                                }else{                                    
                                    if(name.Equals("新模板")){
                                        name = name+DateTime.Now.ToString("d");
                                    }
                                    sql =@"INSERT INTO 
                                                print_uses(
                                                    print_uses.admin_id,
                                                    print_uses.mdate,
                                                    print_uses.`name`,
                                                    print_uses.print_setting,
                                                    print_uses.sys_id,
                                                    print_uses.tpl_data,
                                                    print_uses.type,
                                                    print_uses.coid)
                                         VALUES(
                                                    @admin_id,
                                                    NOW(),
                                                    @name,
                                                    @print_setting,
                                                    @sys_id,
                                                    @state,
                                                    @type,
                                                    @coid);
                                        SELECT LAST_INSERT_ID();";
                                    int reqn = conn.Query<int>(sql,new {
                                                admin_id = admin_id,
                                                name = name,
                                                print_setting = print_setting,
                                                sys_id = sys_id,
                                                state = state,
                                                type = type,
                                                coid = coid
                                            }).AsList()[0];
                                    if (reqn > 0)
                                    {                                           
                                        result.d = new
                                        {
                                            my_id = reqn,
                                            name = name,
                                            type = type
                                        };
                                        //保存 lodop_target
                                        if(setLodop(admin_id,lodop_target).s == -4025){
                                            result.s = -4025;
                                        }else{
                                            result.s = 4003;
                                        }
                                        
                                    }
                                    else
                                    {
                                        result.s = 4014;
                                    }                                    
                                }
                                                            
                            }// end of else
                       
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

        /// <summary>
		/// 
		/// </summary>
        public static DataResult GetUsesList(printParam param){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string wheresql = " a.deleted = FALSE AND ";
                    string totalsql = ""; 
                    var totallist = new List<useslist>();
                    if(!string.IsNullOrEmpty(param.Filter)){
                        wheresql += param.Filter;
                    }   
                    if(!string.IsNullOrEmpty(param.SortField)&& !string.IsNullOrEmpty(param.SortDirection))//排序
                    {
                        wheresql += " ORDER BY "+param.SortField +" "+ param.SortDirection;
                    }
                    if(param.PageIndex == 1){//pageindex 不为 1 时，不再传total 
                        totalsql = @"SELECT 
                                        a.id, 
                                        a.`name`,
                                        a.mdate 
                                    FROM 
                                        print_uses as a 
                                    WHERE  "+wheresql;                        
                        totallist = conn.Query<useslist>(totalsql).AsList();
                    }
                                    
                    if(param.PageIndex>-1&&param.PageSize>-1){
                        wheresql += " limit "+(param.PageIndex -1)*param.PageSize +" ,"+ param.PageIndex*param.PageSize;
                    }

                    wheresql =@"SELECT 
                                    a.id, 
                                    a.`name`,
                                    a.mdate,
                                    b.defed_id,
                                    if(b.defed_id = a.id ,1,0) as defed 
                                FROM 
                                    print_uses as a 
                                LEFT JOIN 
                                    print_use_setting as b 
                                on 
                                    a.admin_id = b.admin_id 
                                WHERE "+wheresql;
                                        
                    var list = conn.Query<useslist>(wheresql).AsList();
                    if (list != null)
                    {
                        if(param.PageIndex == 1){
                            result.d = new {
                                list = list,
                                page = param.PageIndex,
                                pageSize = param.PageSize,
                                pageTotal =  Math.Ceiling(decimal.Parse(totallist.Count.ToString())/decimal.Parse(param.PageSize.ToString())),
                                total = totallist.Count
                            };
                        }else{
                            result.d = new {
                                list = list,
                                page = param.PageIndex,
                            };
                        }                    
                    }
                    else
                    {
                        result.s = -4007;
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

        /// <summary>
		/// 
		/// </summary>
        public static DataResult GetSideTpls(string t,string admin_id){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                   
                    List<print_uses> type = conn.Query<print_uses>(@"SELECT 
                                                                        a.id , 
                                                                        a.`name` 
                                                                    FROM 
                                                                        print_uses as a 
                                                                    WHERE 
                                                                        a.deleted = FALSE AND a.type = "+t).AsList();
                    
                    #region 个人模板
                    List<myTplModel> myTpls = new List<myTplModel>();
                    myTplModel mytpl = new myTplModel();
                    if (type != null) {
                        var res = conn.Query<print_use_setting>(@"SELECT * FROM 
                                                                    print_use_setting 
                                                                  WHERE 
                                                                    print_use_setting.admin_id = "+admin_id).AsList()[0];
                        var defed_id = res != null ? res.defed_id : 0;
                        foreach (print_uses pintuse in type) {                      
                            mytpl.id = pintuse.id;
                            mytpl.name = pintuse.name;
                            mytpl.defed = defed_id == pintuse.id;
                            myTpls.Add(mytpl);
                            mytpl = new myTplModel();
                        }                    
                    }
                    #endregion                    
                    List<print_syses> type_syses = conn.Query<print_syses>(@"SELECT 
                                                                                a.id,
                                                                                a.`name`,
                                                                                a.setting 
                                                                             FROM 
                                                                                print_syses as a 
                                                                             WHERE 
                                                                                a.deleted = FALSE AND  a.type = "+t).AsList();
                    result.d = new
                    {
                        myTpls = myTpls,    
                        sysTpls = type_syses,
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

        public static DataResult GetShopAndMytpl(string t,string admin_id, string coid){

            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                   
                    List<print_uses> type = conn.Query<print_uses>(@"SELECT 
                                                                        a.id , 
                                                                        a.`name` 
                                                                    FROM 
                                                                        print_uses as a 
                                                                    WHERE 
                                                                        a.deleted = FALSE AND a.type = "+t).AsList();                    
                    List<myTplModel> myTpls = new List<myTplModel>();
                    myTplModel mytpl = new myTplModel();
                    if (type != null) {
                        var res = conn.Query<print_use_setting>(@"SELECT * FROM 
                                                                    print_use_setting 
                                                                  WHERE 
                                                                    print_use_setting.admin_id = "+admin_id).AsList()[0];
                        var defed_id = res != null ? res.defed_id : 0;
                        foreach (print_uses pintuse in type) {                      
                            mytpl.id = pintuse.id;
                            mytpl.name = pintuse.name;
                            mytpl.defed = defed_id == pintuse.id;
                            myTpls.Add(mytpl);
                            mytpl = new myTplModel();
                        }                    
                    }          

                    string sql = @"SELECT ID as value ,ShopName as label, printID FROM shop WHERE CoID="+coid+" AND Enable=TRUE AND Deleted=FALSE ;";
                    var shopEnum = conn.Query<shopWithPrint>(sql).AsList();  
                    result.d = new
                    {
                        myTpls = myTpls,  
                        shopEnum = shopEnum  
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


        /// <summary>
		/// 获取用户打印配置
		/// </summary>
        public static DataResult getPrintSet(string uid, int tpl_id = 0, int tpl_type = 0)
        {

            var result = new DataResult(1, null);
            using (var conn = new MySqlConnection(DbBase.CommConnectString))
            {
                try
                {
                    string sql = "";
                    if (tpl_id == 0)
                    {
                        sql = @"SELECT a.defed_id, a.lodop_target FROM 
                                    print_use_setting as a 
                                    WHERE 
                                    a.admin_id = " + uid + " AND a.type=" + tpl_type;
                        var defed = conn.Query<dynamic>(sql).AsList();
                        var tpls = new List<tpl>();
                        var presets = "";
                        var tasks = new Task[2];
                        tasks[0] = Task.Factory.StartNew(() =>
                        {
                            tpls = getTpls(tpl_type);
                        });
                        tasks[1] = Task.Factory.StartNew(() =>
                        {
                            presets = getPreset(tpl_type);
                        });
                        Task.WaitAll(tasks);
                        if (defed.Count == 0)
                        {
                            result.s = -1;
                            result.d = "用户未定义默认模板";
                        }
                        else
                        {
                            sql = @"SELECT name as title, print_setting ,tpl_data as states FROM print_uses WHERE type = " + tpl_type;
                            var print_uses = conn.Query<dynamic>(sql).AsList();
                            if (print_uses.Count == 0)
                            {
                                result.s = -4008;
                            }
                            else
                            {
                                result.d = new
                                {
                                    tpl_id = defed[0].defed_id,
                                    tpls = tpls,
                                    tpl_type = tpl_type,
                                    print_setting = JsonConvert.DeserializeObject<dynamic>(print_uses[0].print_setting),
                                    states = JsonConvert.DeserializeObject<dynamic>(print_uses[0].states),
                                    title = print_uses[0].title,
                                    rows = JsonConvert.DeserializeObject<dynamic>(presets)
                                };
                            }
                        }
                    }
                    else
                    {
                        sql = @"SELECT type, name as title, print_setting ,tpl_data as states FROM print_uses WHERE id = " + tpl_id;
                        var print_uses = conn.Query<dynamic>(sql).AsList();
                        if (print_uses.Count == 0)
                        {
                            result.s = -4023;
                        }
                        else
                        {
                            var presets = "";
                            var tasks = new Task[1];
                            tasks[0] = Task.Factory.StartNew(() =>
                            {
                                presets = getPreset(print_uses[0].type);
                            });
                            Task.WaitAll(tasks);
                            
                            result.d = new
                            {
                                print_setting = JsonConvert.DeserializeObject<dynamic>(print_uses[0].print_setting),
                                states = JsonConvert.DeserializeObject<dynamic>(print_uses[0].states),
                                title = print_uses[0].title,
                                rows = JsonConvert.DeserializeObject<dynamic>(presets)
                            };
                        }
                    }
                }
                catch (Exception e)
                {
                    result.s = -1;
                    result.d = e.Message;
                    conn.Dispose();
                }
            }
            return result;
        }

        public static List<tpl> getTpls(int type){

            var result = new List<tpl>();
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = "SELECT id,`name` FROM print_uses WHERE type ="+type;
                    result = conn.Query<tpl>(sql).AsList();
                }
                catch
                {
                    conn.Dispose();
                }
            }
            return result;
        }

       public static string getPreset(int type){
            var result = "";
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    string sql = "SELECT presets FROM print_sys_types WHERE type ="+type;
                    var presets= conn.Query<string>(sql).AsList();
                    if(presets.Count > 0){           
                        result = presets[0];
                    }
                }
                catch
                {
                    conn.Dispose();
                }
            }
            return result;
        }


        /// <summary>
		/// 
		/// </summary>
        public static DataResult demo(int sys_id){

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