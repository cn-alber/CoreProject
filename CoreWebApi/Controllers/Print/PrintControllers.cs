using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;


//  print_sys_types   系统预设模板
//  print_syses         根据系统预设模板(print_sys_types.type) 生成的 “系统模板”
//  print_use

namespace CoreWebApi.Print
{
    [AllowAnonymous]
    public class PrintController : ControllBase
    {
        #region 获取print_sys_types -> emu_data
        [HttpGetAttribute("/core/print/task/data")]
        public ResponseResult taskdata(string type)
        {
            var m = PrintHaddle.taskData(type);
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
        public ResponseResult tplsys(string sys_id)
        {
            var m = PrintHaddle.tplSys(sys_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

       

        #region 根据预设模板 type( print_sys_types->type ) 获取系统模板 list
        [HttpGetAttribute("/core/print/tpl/sysesbytype")]
        public ResponseResult sysesbytype(string type,int Page = 1,int PageSize = 20)
        {

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
        public ResponseResult tpltype(string type)
        {
            
            var m = PrintHaddle.tplType(type);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 根据type获取 print_sys_types 系统模板数据 emu_data
        //与 /core/print/task/data 返回相同，不同地方引用，区分为两个路由
        [HttpGetAttribute("/core/print/tpl/emu_data")]
        public ResponseResult tplemu_data(string type)
        {
            
            var m = PrintHaddle.taskData(type); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 保存系统预设模板
        [HttpPostAttribute("/core/print/tpl/savesysestype")]
        public ResponseResult savesysestype([FromBodyAttribute]JObject lo)
        {
           
            if(string.IsNullOrEmpty(lo["type"].ToString())){ return CoreResult.NewResponse(-4012, null, "Print");}
            if(string.IsNullOrEmpty(lo["name"].ToString())){ return CoreResult.NewResponse(-4009, null, "Print");}

            int type = int.Parse(lo["type"].ToString());            
            string name = lo["name"].ToString();            
            var presets = lo["presets"];
            var emu_data = lo["emu_data"];
            var setting = lo["setting"];            
            var m = PrintHaddle.saveSysesType(type,name,presets,emu_data,setting); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 保存系统模板
        [HttpPostAttribute("/core/print/tpl/savesys")]
        public ResponseResult savesyses([FromBodyAttribute]JObject lo)
        {                   
            if(string.IsNullOrEmpty(lo["name"].ToString())){ return CoreResult.NewResponse(-4009, null, "Print");}
            string type = lo["type"].ToString();            
            string name = lo["name"].ToString();            
            int  sys_id = string.IsNullOrEmpty(lo["sys_id"].ToString())?0 :int.Parse(lo["sys_id"].ToString());
            var state = lo["state"];
              
            var m = PrintHaddle.saveSyses(sys_id, type,state,name );
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }


        #endregion

        #region 删除系统预设模板 print_sys_types 
        [HttpPostAttribute("/core/print/tpl/delsysestype")]
        public ResponseResult delsysestype([FromBodyAttribute]JObject lo)
        {
   
            string ids =String.Join(",",lo["ids"]); 
            var m = PrintHaddle.DelSysesTypeByID(ids); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 删除系统模板 print_syses 
        [HttpPostAttribute("/core/print/tpl/delsyses")]
        public ResponseResult delsyses([FromBodyAttribute]JObject lo)
        {
   
            string ids =String.Join(",",lo["ids"]); 
            var m = PrintHaddle.DelSysesByID(ids); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion



    }


}