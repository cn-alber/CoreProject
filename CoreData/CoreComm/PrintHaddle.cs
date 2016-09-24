using CoreModels;
using Dapper;
using System;
using CoreModels.XyComm;
using CoreData;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace CoreDate.CoreComm
{
    public static class PrintHaddle
    {
        // DapperDbBase _dbBase;
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
                    var list = conn.Query<print_sys_types>("SELECT print_sys_types.emu_data FROM print_sys_types WHERE  print_sys_types.deleted =FALSE AND  print_sys_types.type ="+type).AsList()[0];
                    if (list != null)
                    {                   
                        result.d = new{ emu_data = list.emu_data };                                  
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
                    var my = conn.Query<print_uses>("SELECT * FROM print_uses as a WHERE a.id = "+my_id+" AND a.admin_id ="+admin_id).AsList()[0];
                    if (my == null) { result.s = -4002; }               
                    result.d = new{
                        currentTplID = int.Parse(my_id),
                        tpl_name = my.name,
                        sys_id = my.sys_id,
                        states = JsonConvert.DeserializeObject<dynamic>(my.tpl_data),
                        print_setting = JsonConvert.DeserializeObject<dynamic>(my.print_setting),
                        type = my.type,
                        tpls = conn.Query<usesModel>("SELECT a.id,a.name FROM print_uses as a WHERE a.type = "+my.type+" ").AsList(),
                        print_ori = "http://localhost:8000/CLodopfuncs.js?priority=1"
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
        #endregion

        #region 
         /// <summary>
		/// 设定，更新默认模板
		/// </summary>
        public static DataResult sideSetdefed(string admin_id,string my_tpl_id){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var oldmodel = conn.Query<print_use_setting>("SELECT * FROM print_use_setting as a WHERE a.admin_id = "+admin_id).AsList()[0];
                    int rnt = 0;
                    if (oldmodel != null){                   
                        rnt = conn.Execute("UPDATE print_use_setting SET print_use_setting.admin_id = "+admin_id+" ,print_use_setting.defed_id = "+my_tpl_id+" WHERE print_use_setting.id = "+oldmodel.id);
                        if (rnt > 0){
                        result.s = 1;
                        }else{
                            result.s = -4003;
                        }
                    }else {
                        rnt = conn.Execute("INSERT INTO print_use_setting(print_use_setting.admin_id,print_use_setting.defed_id) VALUES("+admin_id+","+my_tpl_id+")");
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
		/// 移除用户模板
		/// </summary>
        public static DataResult sideRemove(string my_tpl_id){
            var result = new DataResult(1,null); 
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
                    var oldmoder = conn.Query<print_use_setting>("SELECT a.id FROM print_use_setting as a WHERE a.defed_id = "+my_tpl_id).AsList();
                    if (oldmoder!=null) {
                        result.s = -4005;
                    }
                    int rnt = conn.Execute("DELETE FROM print_uses WHERE print_uses.id = "+my_tpl_id);
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
                    var list = conn.Query<AllSysTypes>("SELECT a.id, a.`name`,a.type FROM print_sys_types as a WHERE  a.deleted = FALSE  ").AsList();                 
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
                    var sys = conn.Query<print_syses>("SELECT a.tpl_data,a.type,a.`name` FROM print_syses as a WHERE a.deleted = FALSE AND  a.id = "+sys_id).AsList()[0];
                    if(sys==null){
                        result.s = -4008;
                    }else{            
                        var type = conn.Query<print_sys_types>("SELECT a.setting,a.presets FROM print_sys_types as a WHERE   print_sys_types.deleted =FALSE AND a.type = "+sys.type).AsList()[0];
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
                                tpl_name = sys.name + DateTime.Now.ToString("d")
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
                    var my = conn.Query<print_uses>("SELECT * FROM print_uses as a WHERE a.id = "+my_id+" AND a.admin_id = "+admin_id).AsList()[0];
                    if (my == null){
                        result.s = -4002;
                    }else{
                        var type = conn.Query<print_sys_types>("SELECT * FROM print_sys_types as a WHERE a.deleted=FALSE AND a.type="+my.type).AsList()[0];
                        if(type!=null){
                            result.d = new
                            {
                                currentTplID = my_id,
                                tpl_name = my.name,
                                sys_id = my.sys_id,
                                states = JsonConvert.DeserializeObject<dynamic>(my.tpl_data),
                                presets = JsonConvert.DeserializeObject < dynamic >(type.presets),
                                print_setting = JsonConvert.DeserializeObject<dynamic>(my.print_setting),
                                type = my.type
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
                        totalsql = "SELECT a.id, a.`name`,a.mtime FROM print_syses as a WHERE  "+wheresql;
                        totallist = conn.Query<printSysesList>(totalsql).AsList();
                    }
                                    
                    if(param.PageIndex>-1&&param.PageSize>-1){
                        wheresql += " limit "+(param.PageIndex -1)*param.PageSize +" ,"+ param.PageIndex*param.PageSize;
                    }

                    wheresql ="SELECT a.id, a.`name`,a.mtime FROM print_syses as a WHERE  "+wheresql; 

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
                    var type = conn.Query<print_sys_types>("SELECT * FROM print_sys_types as a WHERE a.deleted=FALSE AND a.type=1").AsList()[0];
                    if(type == null){
                        result.s = -4010;
                    }else{
                        result.d = new
                        {
                            states = JsonConvert.DeserializeObject<dynamic>(type.presets),           
                            print_setting = JsonConvert.DeserializeObject<dynamic>(type.setting),       
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

        



        /// <summary>
		/// 
		/// </summary>
        public static DataResult DelSysesTypeByID(string ids){
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
                       sql += "UPDATE print_sys_types SET print_sys_types.deleted = TRUE WHERE print_sys_types.id="+id+" AND print_sys_types.deleted=FALSE;";
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
        public static DataResult DelSysesByID(string ids){
            var result = new DataResult(1,null);
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try
                {
   
                    string sql = "";
                    var idsArr = ids.Split(','); 
                    foreach(string id in idsArr){
                       sql += "UPDATE print_syses SET print_syses.deleted = TRUE WHERE print_syses.id="+id+" AND print_syses.deleted=FALSE;";
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
		/// 保存个人模板
		/// </summary>
        public static DataResult postSaveMy(string admin_id,string my_id, string sys_id, string type, string name, dynamic print_setting, dynamic state){

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

                            if (int.Parse(my_id) > 0)
                            { //my_id >0 为更新
                                var oldmodel = conn.Query<print_uses>("").AsList()[0];
                                if (oldmodel == null){
                                    result.s = -4015;
                                }else{
                                    oldmodel.name = name;
                                    oldmodel.tpl_data = Convert.ToString(state);
                                    oldmodel.print_setting = Convert.ToString(print_setting);
                                    oldmodel.mdate = DateTime.Now;
                                    int rnt = conn.Execute("");
                                    if (rnt > 0){
                                        result.s = 4004;
                                    }else {
                                        result.s = -4016;
                                    }                        
                                }                                
                            }else{//新增
                                if (int.Parse(sys_id) > 0)
                                {
                                    var oldmodel = conn.Query<print_syses>("").AsList()[0];
                                    if (oldmodel == null){
                                        result.s = -4012;
                                    } else{
                                        print_uses newmodel = new print_uses();
                                        newmodel.sys_id = int.Parse(sys_id);
                                        newmodel.type = int.Parse(type);
                                        newmodel.admin_id = int.Parse(admin_id);
                                        newmodel.name = name;
                                        newmodel.print_setting = Convert.ToString(print_setting);
                                        newmodel.tpl_data = Convert.ToString(state);
                                        newmodel.mdate = DateTime.Now;
                                        int reqn = conn.Execute("");
                                        if (reqn > 0)
                                        {
                                            print_uses new_my_id = conn.Query<print_uses>("").AsList()[0];
                                            result.d = new
                                            {
                                                my_id = new_my_id.id,
                                            };
                                            result.s = 4003;
                                        }
                                        else
                                        {
                                            result.s = 4014;
                                        }
                                    }
                                } // end of  int.Parse(sys_id)                                
                            }
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
		/// 保存预设系统模板
		/// </summary>
        public static DataResult saveSysesType(){

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