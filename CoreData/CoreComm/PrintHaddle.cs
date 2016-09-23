using CoreModels;
using Dapper;
using System;
using CoreModels.XyComm;
using CoreData;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreDate.CoreComm
{
    public static class PrintHaddle
    {
        #region 获取print_sys_types -> emu_data
        public static DataResult taskData(string type)
        {
            int s = 1;     
            var result = new DataResult(s,null);    
            try
            {
                var list = DbBase.CommDB.Query<print_sys_types>("SELECT print_sys_types.emu_data FROM print_sys_types WHERE print_sys_types.type ="+type).AsList()[0];
                if (list != null)
                {                   
                    result.d = new{ emu_data = JsonConvert.DeserializeObject<dynamic>(list.emu_data) };                                  
                }else {
                    result.s = 3001;                   
                }               
            }
            catch (Exception e){                
                result.s = -1;
                result.d= e.Message;                
            }
            return result;
        }
        #endregion

        #region 获取个人模板 print_uses
        public static DataResult taskTpl(string admin_id,string my_id){
            int s = 1;
            var result = new DataResult(s,null); 
            try
            {
                var my = DbBase.CommDB.Query<print_uses>("SELECT * FROM print_uses as a WHERE a.id = "+my_id+" AND a.admin_id ="+admin_id).AsList()[0];
                if (my == null) { result.s = 3002; }               
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

        #region 设定，更新默认模板
        public static DataResult sideSetdefed(string admin_id,string my_tpl_id){
            int s = 1;
            var result = new DataResult(s,null); 
            try
            {
                var oldmodel = DbBase.CommDB.Query<print_use_setting>("SELECT * FROM print_use_setting as a WHERE a.admin_id = "+admin_id).AsList()[0];
                int rnt = 0;
                if (oldmodel != null){                   
                    rnt = DbBase.CommDB.Execute("UPDATE print_use_setting SET print_use_setting.admin_id = "+admin_id+" ,print_use_setting.defed_id = "+my_tpl_id+" WHERE print_use_setting.id = "+oldmodel.id);
                    if (rnt > 0){
                       result.s = 1;
                    }else{
                        result.s = 3003;
                    }
                }else {
                    rnt = DbBase.CommDB.Execute("INSERT INTO print_use_setting(print_use_setting.admin_id,print_use_setting.defed_id) VALUES("+admin_id+","+my_tpl_id+")");
                    if (rnt > 0){
                        result.s = 1;
                    }else{
                        result.s = 3004;
                    }
                }                
            }catch (Exception e){
                result.s = -1;
                result.d= e.Message;
            }      
            return result;      
        }
        #endregion

        #region 移除用户模板
        public static DataResult sideRemove(string my_tpl_id){
            int s = 1;
            var result = new DataResult(s,null); 
            try
            {
                var oldmoder = DbBase.CommDB.Query<print_use_setting>("SELECT a.id FROM print_use_setting as a WHERE a.defed_id = "+my_tpl_id).AsList();
                if (oldmoder!=null) {
                    result.s = 3005;
                }
                int rnt = DbBase.CommDB.Execute("DELETE FROM print_uses WHERE print_uses.id = "+my_tpl_id);
                if (rnt > 0)
                {
                   result.s = 1;
                }else{
                    result.s = 3006;
                }                
            }catch (Exception e)
            {
                result.s = -1;
                result.d= e.Message;
            }     
            return result;       
        }
        #endregion

        public static DataResult getAllSysTypes(){
            int s = 1;
            var result = new DataResult(s,null);
            try
            {                
                var list = DbBase.CommDB.Query<AllSysTypes>("SELECT a.id, a.`name`,a.type FROM print_sys_types as a").AsList();
                if (list != null)
                {
                   result.d = list;                 
                }else{
                    result.s=4007;
                }               
            }catch (Exception e){
                result.s = -1;
                result.d= e.Message;
            }
            return result;
        }








   
   
    }
}