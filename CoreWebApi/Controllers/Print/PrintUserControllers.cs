using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using Microsoft.AspNetCore.Authorization;
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
        public ResponseResult tasktpl(string my_id)
        {
            var admin_id = GetUid();
            var m = PrintHaddle.taskTpl(admin_id,my_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

         #region 获取个人模板数据
       [HttpGetAttribute("/core/print/tpl/my")]
        public ResponseResult tplmy(string my_id)
        {
            string admin_id = GetUid();
            var m = PrintHaddle.tplMy(admin_id,my_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 保存个人模板数据
        [HttpPostAttribute("/core/print/tpl/saveMy")]
        public ResponseResult saveMy([FromBodyAttribute]JObject lo)
        {
            string admin_id = GetUid();
            string my_id = lo["my_id"].ToString();
            string sys_id = lo["sys_id"].ToString();
            string type = lo["type"].ToString();
            string name = lo["tpl_name"].ToString();
            var print_setting = lo["print_setting"];
            var state = lo["state"];
            var m = PrintHaddle.postSaveMy(admin_id, my_id, sys_id, type, name, print_setting, state); 
            return CoreResult.NewResponse(m.s, m.d, "Print");
           
        }
        #endregion





    }

}