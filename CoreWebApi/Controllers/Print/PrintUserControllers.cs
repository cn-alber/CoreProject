using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Print
{
    /// <summary>
	/// 打印模块 - 个人用户相关 
	/// </summary>
    
    public class PrintUserController : ControllBase
    {
        #region 获取个人模板 print_uses
        [HttpGetAttribute("/core/print/task/tpl")]
        public ResponseResult tasktpl(int my_id)
        {
            if(!checkInt(my_id)) return CoreResult.NewResponse(-4023, null, "Print");
            var admin_id = GetUid();
            var m = PrintHaddle.taskTpl(admin_id,my_id.ToString());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

         #region 获取个人模板数据
       [HttpGetAttribute("/core/print/tpl/my")]
        public ResponseResult tplmy(int my_id)
        {
            if(!checkInt(my_id)) return CoreResult.NewResponse(-4023, null, "Print");
            string admin_id = GetUid();
            var m = PrintHaddle.tplMy(admin_id,my_id.ToString());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 新建个人模板数据
        [HttpPostAttribute("/core/print/tpl/createMy")]
        public ResponseResult createMy([FromBodyAttribute]JObject lo)
        {
            if(!isJson(lo["print_setting"].ToString(),lo["state"].ToString())){
                return CoreResult.NewResponse(-4024, null, "Print");
            }
            if(string.IsNullOrEmpty(lo["tpl_name"].ToString())){ return CoreResult.NewResponse(-4012, null, "Print");}

            string admin_id = GetUid();
            string my_id =string.IsNullOrEmpty(lo["my_id"].ToString()) ? "0" :lo["my_id"].ToString();
            string sys_id ="0";
            string type = string.IsNullOrEmpty(lo["type"].ToString()) ? "0": lo["type"].ToString();
            string name = lo["tpl_name"].ToString();
            var print_setting = lo["print_setting"];
            var state = lo["state"];
            var lodop_target = lo["lodop_target"].ToString();
            string coid = GetCoid();
            var m = PrintHaddle.postSaveMy(admin_id, my_id, sys_id, type, name, print_setting, state,lodop_target,coid); 
            return CoreResult.NewResponse(m.s, m.d, "Print");           
        }
        #endregion

        #region 新建个人模板数据
        [HttpPostAttribute("/core/print/tpl/modifyMy")]
        public ResponseResult modifyMy([FromBodyAttribute]JObject lo)
        {
            if(!isJson(lo["print_setting"].ToString(),lo["state"].ToString())){
                return CoreResult.NewResponse(-4024, null, "Print");
            }
            if(string.IsNullOrEmpty(lo["tpl_name"].ToString())){ return CoreResult.NewResponse(-4012, null, "Print");}

            string admin_id = GetUid();
            string my_id =string.IsNullOrEmpty(lo["my_id"].ToString()) ? "0" :lo["my_id"].ToString();
            string sys_id =string.IsNullOrEmpty(lo["sys_id"].ToString()) ? "0" : lo["sys_id"].ToString();
            string type = string.IsNullOrEmpty(lo["type"].ToString()) ? "0": lo["type"].ToString();
            string name = lo["tpl_name"].ToString();
            var print_setting = lo["print_setting"];
            var state = lo["state"];
            var lodop_target = lo["lodop_target"].ToString();
            string coid = GetCoid();
            var m = PrintHaddle.postSaveMy(admin_id, my_id, sys_id, type, name, print_setting, state,lodop_target,coid); 
            return CoreResult.NewResponse(m.s, m.d, "Print");           
        }
        #endregion



        #region 删除个人模板 
        [HttpPostAttribute("/core/print/tpl/delmine")]
        public ResponseResult delMyTpl([FromBodyAttribute]JObject lo){
            string ids =String.Join(",",lo["ids"]); 
            if(!checkInt(ids)) return CoreResult.NewResponse(-4023, null, "Print");
            string coid = GetCoid();
            var m = PrintHaddle.sideRemove(ids,coid);
            return CoreResult.NewResponse(1,null,"Print");
        }
        #endregion

        #region 获取个人模板list  print_uses list
        [HttpGetAttribute("/core/print/tpl/minebytype")]    
        public ResponseResult useslist(int type,int Page = 1,int PageSize = 20){
            
            if(!checkInt(type)) return CoreResult.NewResponse(-4023, null, "Print");
            var admin_id = GetUid();
            printParam param = new printParam();
            param.Filter = " a.type = "+type+" AND a.admin_id = "+admin_id+" ";
            
            param.PageIndex = Math.Max(Page,1);
            param.PageSize = Math.Max(PageSize,20);

            
            var m = PrintHaddle.GetUsesList(param);
            return CoreResult.NewResponse(m.s,m.d,"Print");
        }

        #endregion

        #region 获取打印参数配置
        [HttpGetAttribute("/core/print/tpl/getPrintSet")]    
        public ResponseResult getPrintSet(int tpl_id=0,int tpl_type = 0){
            
            var admin_id = GetUid();
            if(tpl_id == 0 && tpl_type==0){
                return CoreResult.NewResponse(-1,"模板类型或ID必有其一","Print");
            }
            
            var m = PrintHaddle.getPrintSet(admin_id,tpl_id,tpl_type);
            return CoreResult.NewResponse(m.s,m.d,"Print",m.m);
        }

        #endregion


    }

}