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
            try
            {
                var list = DbBase.CommDB.Query<print_sys_types>("SELECT print_sys_types.emu_data FROM print_sys_types WHERE print_sys_types.type ="+type).AsList()[0];
                if (list != null)
                {
                    var date = new{ emu_data = JsonConvert.DeserializeObject<dynamic>(list.emu_data) };
                    return new DataResult(s,date);
                }else {
                   s = 3001;
                }
                return new DataResult(s,null);
            }
            catch (Exception e)
            {
                
                return new DataResult(-1,e.Message);
            }
        }
        #endregion

        #region 获取个人模板 print_uses
        public static DataResult taskTpl(string admin_id,string my_id){
            int s = 1;
            try
            {
                var my = DbBase.CommDB.Query<print_uses>("SELECT * FROM print_uses as a WHERE a.id = "+my_id+" AND a.admin_id ="+admin_id).AsList()[0];
                if (my == null)
                {                 
                    s = 3002;
                    return new DataResult(s,null);
                }
                var uses = new List<usesModel>();
                var usesmodel  = new usesModel();
                var usesM = DbBase.CommDB.Query<print_uses>("SELECT a.id,a.name FROM print_uses as a WHERE a.type = "+my.type+" ").AsList();
                foreach(print_uses p in usesM){
                    usesmodel.id = p.id;
                    usesmodel.name = p.name;
                    uses.Add(usesmodel);
                }
                var states = JsonConvert.DeserializeObject<dynamic>(my.tpl_data);            
                var print_setting = JsonConvert.DeserializeObject<dynamic>(my.print_setting);
               
                var result = new{
                    currentTplID = int.Parse(my_id),
                    tpl_name = my.name,
                    sys_id = my.sys_id,
                    states = states,
                    print_setting = print_setting,
                    type = my.type,
                    tpls = uses,
                    print_ori = "http://localhost:8000/CLodopfuncs.js?priority=1"
                };
                return new DataResult(s,result);
            }
            catch (Exception e)
            {
                return new DataResult(-1,e.Message);
            }            
        }
        #endregion

        #region 设定，更新默认模板
        public static DataResult sideSetdefed(string admin_id,string my_tpl_id){
            int s = 1;
             try
            {
                var oldmodel = DbBase.CommDB.Query<print_use_setting>("SELECT * FROM print_use_setting as a WHERE a.admin_id = "+admin_id).AsList()[0];
                int rnt = 0;
                if (oldmodel != null){                   
                    rnt = DbBase.CommDB.Execute("UPDATE print_use_setting SET print_use_setting.admin_id = "+admin_id+" ,print_use_setting.defed_id = "+my_tpl_id+" WHERE print_use_setting.id = "+oldmodel.id);
                    if (rnt > 0){
                       s = 1;
                    }else{
                        s = 3003;
                    }
                }else {
                    rnt = DbBase.CommDB.Execute("INSERT INTO print_use_setting(print_use_setting.admin_id,print_use_setting.defed_id) VALUES("+admin_id+","+my_tpl_id+")");
                    if (rnt > 0){
                        s = 1;
                    }else{
                        s = 3004;
                    }
                }
                return new DataResult(s,null);
            }catch (Exception e){
                return new DataResult(-1,e.Message);
            }            
        }
        #endregion

        #region 移除用户模板
        public static DataResult sideRemove(string my_tpl_id){
            int s = 1;
            try
            {
                var oldmoder = DbBase.CommDB.Query<print_use_setting>("SELECT a.id FROM print_use_setting as a WHERE a.defed_id = "+my_tpl_id).AsList();
                if (oldmoder!=null) {
                    s = 3005;
                    return new DataResult(s,null);
                }
                int rnt = DbBase.CommDB.Execute("DELETE FROM print_uses WHERE print_uses.id = "+my_tpl_id);
                if (rnt > 0)
                {
                   s = 1;
                }else{
                    s = 3006;
                }
                return new DataResult(s,null);
            }catch (Exception e)
            {
                return new DataResult(-1,e.Message);
            }            
        }
        #endregion










   
   
    }
}