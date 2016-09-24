using CoreModels;
using Dapper;
using System;
using CoreModels.XyComm;
using CoreData;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

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
            try
            {
                var list = DbBase.CommDB.Query<print_sys_types>("SELECT print_sys_types.emu_data FROM print_sys_types WHERE  print_sys_types.deleted =FALSE AND  print_sys_types.type ="+type).AsList()[0];
                if (list != null)
                {                   
                    result.d = new{ emu_data = JsonConvert.DeserializeObject<dynamic>(list.emu_data) };                                  
                }else {
                    result.s = -4001;                   
                }               
            }
            catch (Exception e){                
                result.s = -1;
                result.d= e.Message;                
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
            try
            {
                var my = DbBase.CommDB.Query<print_uses>("SELECT * FROM print_uses as a WHERE a.id = "+my_id+" AND a.admin_id ="+admin_id).AsList()[0];
                if (my == null) { result.s = -4002; }               
                result.d = new{
                    currentTplID = int.Parse(my_id),
                    tpl_name = my.name,
                    sys_id = my.sys_id,
                    states = JsonConvert.DeserializeObject<dynamic>(my.tpl_data),
                    print_setting = JsonConvert.DeserializeObject<dynamic>(my.print_setting),
                    type = my.type,
                    tpls = DbBase.CommDB.Query<usesModel>("SELECT a.id,a.name FROM print_uses as a WHERE a.type = "+my.type+" ").AsList(),
                    print_ori = "http://localhost:8000/CLodopfuncs.js?priority=1"
                };                
            }
            catch (Exception e)
            {
                result.s = -1;
                result.d= e.Message;
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
            try
            {
                var oldmodel = DbBase.CommDB.Query<print_use_setting>("SELECT * FROM print_use_setting as a WHERE a.admin_id = "+admin_id).AsList()[0];
                int rnt = 0;
                if (oldmodel != null){                   
                    rnt = DbBase.CommDB.Execute("UPDATE print_use_setting SET print_use_setting.admin_id = "+admin_id+" ,print_use_setting.defed_id = "+my_tpl_id+" WHERE print_use_setting.id = "+oldmodel.id);
                    if (rnt > 0){
                       result.s = 1;
                    }else{
                        result.s = -4003;
                    }
                }else {
                    rnt = DbBase.CommDB.Execute("INSERT INTO print_use_setting(print_use_setting.admin_id,print_use_setting.defed_id) VALUES("+admin_id+","+my_tpl_id+")");
                    if (rnt > 0){
                        result.s = 1;
                    }else{
                        result.s = -4004;
                    }
                }                
            }catch (Exception e){
                result.s = -1;
                result.d= e.Message;
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
            try
            {
                var oldmoder = DbBase.CommDB.Query<print_use_setting>("SELECT a.id FROM print_use_setting as a WHERE a.defed_id = "+my_tpl_id).AsList();
                if (oldmoder!=null) {
                    result.s = -4005;
                }
                int rnt = DbBase.CommDB.Execute("DELETE FROM print_uses WHERE print_uses.id = "+my_tpl_id);
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
            }     
            return result;       
        }
        #endregion
//new MySqlConnection("server=xieyuntestout.mysql.rds.aliyuncs.com;database=xycomm;uid=xieyun;pwd=xieyun123;Port=3306;SslMode=None")
        /// <summary>
		/// 获取print_sys_types list
		/// </summary>
        public static DataResult getAllSysTypes(){
            var result = new DataResult(1,null);        
            using(var conn = new MySqlConnection(DbBase.CommConnectString) ){
                try{
                    var list = conn.Query<AllSysTypes>("SELECT a.id, a.`name`,a.type FROM print_sys_types as a WHERE  a.deleted =FALSE  ").AsList();                 
                    if (list != null)
                    {
                    result.d = list;                 
                    }else{
                        result.s=-4007;
                    }
                    conn.Dispose();
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
            try
            {
                var sys = DbBase.CommDB.Query<print_syses>("SELECT a.tpl_data,a.type,a.`name` FROM print_syses as a WHERE a.id = "+sys_id).AsList()[0];
                if(sys==null){
                    result.s = -4008;
                }else{            
                    var type = DbBase.CommDB.Query<print_sys_types>("SELECT a.setting,a.presets FROM print_sys_types as a WHERE   print_sys_types.deleted =FALSE AND a.type = "+sys.type).AsList()[0];
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
            }
            return result;
        }
        #endregion


        /// <summary>
		/// 
		/// </summary>
        public static DataResult tplMy(string admin_id,string my_id){
            var result = new DataResult(1,null);
            try
            {                
                var my = DbBase.CommDB.Query<print_uses>("SELECT * FROM print_uses as a WHERE a.id = "+my_id+" AND a.admin_id = "+admin_id).AsList()[0];
                if (my == null){
                    result.s = -4002;
                }else{
                    var type = DbBase.CommDB.Query<print_sys_types>("SELECT * FROM print_sys_types as a WHERE a.deleted=FALSE AND a.type="+my.type).AsList()[0];
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
            }
            return result;
        }

        /// <summary>
		/// 
		/// </summary>
        public static DataResult GetSysesByType(printParam param){

            var result = new DataResult(1,null);
            try
            {
                string wheresql = " "; 
                if(!string.IsNullOrEmpty(param.Filter)){
                    wheresql += param.Filter;
                }   
                if(!string.IsNullOrEmpty(param.SortField)&& !string.IsNullOrEmpty(param.SortDirection))//排序
                {
                    wheresql += " ORDER BY "+param.SortField +" "+ param.SortDirection;
                }
                var totalsql = "SELECT a.id, a.`name`,a.mtime FROM print_syses as a WHERE  "+wheresql;
                var totallist = DbBase.CommDB.Query<printSysesList>(totalsql).AsList();
                if(param.PageIndex>-1&&param.PageSize>-1){
                    wheresql += " limit "+(param.PageIndex -1)*param.PageSize +" ,"+ param.PageIndex*param.PageSize;
                }

                wheresql ="SELECT a.id, a.`name`,a.mtime FROM print_syses as a WHERE  "+wheresql; 

                var list = DbBase.CommDB.Query<printSysesList>(wheresql).AsList();

                if (list != null)
                {
                    if(param.PageIndex == 1){
                        result.d = new {
                            list = list,
                            Page = param.PageIndex,
                            PageSize = param.PageSize,
                            PageTotal =  Math.Ceiling(decimal.Parse(totallist.Count.ToString())/decimal.Parse(param.PageSize.ToString())),
                            Total = totallist.Count
                        };
                    }else{
                        result.d = new {
                            list = list,
                            Page = param.PageIndex,
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
            }
            return result;
        }


        /// <summary>
		/// 获取类型预设
		/// </summary>
        public static DataResult tplType(string t){
            var result = new DataResult(1,null);
            try
            {
                var type = DbBase.CommDB.Query<print_sys_types>("SELECT * FROM print_sys_types as a WHERE a.deleted=FALSE AND a.type=1").AsList()[0];
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
            }

            return result;
        }

        



        /// <summary>
		/// 
		/// </summary>
        public static DataResult DelSysesTypeByID(string id){
            var result = new DataResult(1,null);
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
                int rnt = DbBase.CommDB.Execute("UPDATE print_sys_types SET print_sys_types.deleted = TRUE WHERE print_sys_types.id="+id+" AND print_sys_types.deleted=FALSE;");
                if (rnt > 0)
                {
                    result.s = 2;
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
            }

            return result;
        }


        /// <summary>
		/// 
		/// </summary>
        public static DataResult demo(int sys_id){

            var result = new DataResult(1,null);
            try
            {

            }
            catch (Exception e)
            {
                result.s = -1;
                result.d= e.Message; 
            }

            return result;
        }















   
   
    }
}