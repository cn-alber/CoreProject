using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;


//  print_sys_types   系统预设模板
//  print_syses         根据系统预设模板(print_sys_types.type) 生成的 “系统模板”
//  print_use

namespace CoreWebApi.Print
{
    /// <summary>
	/// 打印模块 - 系统模块相关 
	/// </summary>    
    public class PrintController : ControllBase
    {
        #region 获取print_sys_types -> emu_data
        [HttpGetAttribute("/core/print/task/data")]
        public ResponseResult taskdata(int type)
        {
            if(!checkInt(type)) return CoreResult.NewResponse(-4001, null, "Print");
            var m = PrintHaddle.taskData(type.ToString());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        
        
        #region 获取左侧 系统预设模板列表 print_sys_types list
        [HttpGetAttribute("/core/print/tpl/getallsystypes")]
        public ResponseResult getallsystypes()
        {
            var m = PrintHaddle.getAllSysTypes();
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 获取系统模板 print_syses
        [HttpGetAttribute("/core/print/tpl/sys")]
        public ResponseResult tplsys(int sys_id)
        {
            if(!checkInt(sys_id)) return CoreResult.NewResponse(-4001, null, "Print");
            var m = PrintHaddle.tplSys(sys_id.ToString());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

       

        #region 根据预设模板 type( print_sys_types->type ) 获取系统模板 list
        [HttpGetAttribute("/core/print/tpl/sysesbytype")]
        public ResponseResult sysesbytype(int type,int Page = 1,int PageSize = 20)
        {
            if(!checkInt(type)) return CoreResult.NewResponse(-4001, null, "Print");
            printParam param = new printParam();
            param.Filter = "type = "+type;            
            param.PageIndex = Math.Max(Page,1);
            param.PageSize = Math.Max(PageSize,20);
            
            var m = PrintHaddle.GetSysesByType(param);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 获取类型预设
        [HttpGetAttribute("/core/print/tpl/type")]
        public ResponseResult tpltype(int type)
        {            
            if(!checkInt(type)) return CoreResult.NewResponse(-4001, null, "Print");
            var m = PrintHaddle.tplType(type.ToString());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 根据type获取 print_sys_types 系统模板数据 emu_data
        //与 /core/print/task/data 返回相同，不同地方引用，区分为两个路由
        [HttpGetAttribute("/core/print/tpl/emu_data")]
        public ResponseResult tplemu_data(int type)
        {            
            if(!checkInt(type)) return CoreResult.NewResponse(-4001, null, "Print");
            var m = PrintHaddle.taskData(type.ToString());  
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 获取系统预设模板 单条数据
        [HttpGetAttribute("/core/print/tpl/sysesType")]
        public ResponseResult sysestype(int id)
        {
            if(!checkInt(id)) return CoreResult.NewResponse(-4023, null, "Print");
            var m = PrintHaddle.GetSysesType(id.ToString()); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion


        #region 新增系统预设模板
        [HttpPostAttribute("/core/print/tpl/createSysesType")]
        public ResponseResult createSysesType([FromBodyAttribute]JObject lo)
        {
            //只有系统管理员才编辑新增
            if(!checkIsAdmin() ){ return CoreResult.NewResponse(-1008, null, "Basic");}                      
            if(string.IsNullOrEmpty(lo["name"].ToString())){ return CoreResult.NewResponse(-4012, null, "Print");}
            if(!isJson(lo["presets"].ToString(),lo["emu_data"].ToString(),lo["setting"].ToString())){
                return CoreResult.NewResponse(-4024, null, "Print");
            }
                     
            string name = lo["name"].ToString();            
            var presets = lo["presets"] !=null ? JsonEscape(lo["presets"].ToString()):"";
            var emu_data = lo["emu_data"] !=null ? JsonEscape(lo["emu_data"].ToString()):"";
            var setting = lo["setting"] !=null ? JsonEscape(lo["setting"].ToString()) :""; 
            string coid = GetCoid();

            var m = PrintHaddle.saveSysesType(0,name,presets,emu_data,setting,coid); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 编辑系统预设模板
        [HttpPostAttribute("/core/print/tpl/modifySysesType")]
        public ResponseResult modifySysesType([FromBodyAttribute]JObject lo)
        {
            //只有系统管理员才编辑新增
            if(!checkIsAdmin() ){ return CoreResult.NewResponse(-1008, null, "Basic");}                      
            if(string.IsNullOrEmpty(lo["name"].ToString())){ return CoreResult.NewResponse(-4012, null, "Print");}            
            if(!checkInt(int.Parse(lo["id"].ToString()))) return CoreResult.NewResponse(-4023, null, "Print");
            if(!isJson(lo["presets"].ToString(),lo["emu_data"].ToString(),lo["setting"].ToString())){
                return CoreResult.NewResponse(-4024, null, "Print");
            }

            int id =int.Parse(lo["id"].ToString());            
            string name = lo["name"].ToString();            
            // var presets = lo["presets"] !=null ? JsonEscape(lo["presets"].ToString()):"";
            // var emu_data = lo["emu_data"] !=null ? JsonEscape(lo["emu_data"].ToString()):"";
            // var setting = lo["setting"] !=null ? JsonEscape(lo["setting"].ToString()) :""; 
            var presets = lo["presets"] !=null ? lo["presets"].ToString():"";
            var emu_data = lo["emu_data"] !=null ? lo["emu_data"].ToString():"";
            var setting = lo["setting"] !=null ? lo["setting"].ToString() :""; 
            string coid = GetCoid();
            var m = PrintHaddle.saveSysesType(id,name,presets,emu_data,setting,coid); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 删除系统预设模板 print_sys_types 
        [HttpPostAttribute("/core/print/tpl/deleteSysesType")]
        public ResponseResult deleteSysesType([FromBodyAttribute]JObject lo)
        {   
            string ids =lo["id"].ToString();                  
            if(!checkInt(ids)) return CoreResult.NewResponse(-4023, null, "Print");
            string coid = GetCoid(); 
            var m = PrintHaddle.DelSysesTypeByID(ids,coid); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 保存系统模板
        [HttpPostAttribute("/core/print/tpl/createSys")]
        public ResponseResult createSys([FromBodyAttribute]JObject lo)
        {                   

            if(!checkIsAdmin() ){ return CoreResult.NewResponse(-1008, null, "Basic");}
            //if(string.IsNullOrEmpty(lo["name"].ToString())){ return CoreResult.NewResponse(-4009, null, "Print");}
            string type = lo["type"].ToString();            
            string name = lo["name"].ToString();            
            int  sys_id = 0;
            var state = JsonEscape(lo["state"].ToString());
            string coid = GetCoid();  
            var m = PrintHaddle.saveSyses(sys_id, type,state,name,coid);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 编辑系统模板
        [HttpPostAttribute("/core/print/tpl/modifySys")]
        public ResponseResult modifySys([FromBodyAttribute]JObject lo)
        {                   
            if(!checkIsAdmin() ){ return CoreResult.NewResponse(-1008, null, "Basic");}
            if(string.IsNullOrEmpty(lo["name"].ToString())){ return CoreResult.NewResponse(-4009, null, "Print");}
            string type = lo["type"].ToString();            
            string name = lo["name"].ToString();            
            int  sys_id = string.IsNullOrEmpty(lo["sys_id"].ToString())?0 :int.Parse(lo["sys_id"].ToString());
            var state = JsonEscape(lo["state"].ToString());
              string coid = GetCoid();
            var m = PrintHaddle.saveSyses(sys_id, type,state,name,coid);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 删除系统模板 print_syses 
        [HttpPostAttribute("/core/print/tpl/delsyses")]
        public ResponseResult delsyses([FromBodyAttribute]JObject lo)
        {   
            string ids =String.Join(",",lo["ids"]); 
            string coid = GetCoid();
            var m = PrintHaddle.DelSysesByID(ids,coid); 
            
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion



    }


}