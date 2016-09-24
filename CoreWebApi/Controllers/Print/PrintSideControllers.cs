using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;
using System;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Print
{

    public class PrintSideController : ControllBase
    {

        #region 侧边栏设定，更新默认模板
        [HttpGetAttribute("/core/print/side/setdefed")]
        public ResponseResult sidesetdefed(string my_tpl_id)
        {
            var admin_id = GetUid();
            var m = PrintHaddle.sideSetdefed(admin_id,my_tpl_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 侧边栏删除模板
        //[HttpGetAttribute("/core/print/side/remove")]
        // public ResponseResult remove(string my_tpl_id)
        // {
        //     var admin_id = GetUid();
        //     var m = PrintHaddle.removeDefed(admin_id,my_tpl_id);
        //     return CoreResult.NewResponse(m.s, m.d, "Print");
        // }
        #endregion


    
    }

}