using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using Microsoft.AspNetCore.Authorization;

namespace CoreWebApi
{
    [AllowAnonymous]
    public class PrintController : ControllBase
    {
        
        [HttpGetAttribute("/core/print/task/data")]
        public ResponseResult taskdata(string type)
        {
            var m = PrintHaddle.taskData(type);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }

        [HttpGetAttribute("/core/print/task/tpl")]
        public ResponseResult tasktpl(string my_id)
        {
            var admin_id = "1";//GetUid();
            var m = PrintHaddle.taskTpl(admin_id,my_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }

        [HttpGetAttribute("/core/print/side/setdefed")]
        public ResponseResult sidesetdefed(string my_tpl_id)
        {
            var admin_id = "1";//GetUid();
            var m = PrintHaddle.sideSetdefed(admin_id,my_tpl_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        
        [HttpGetAttribute("/core/print/tpl/getallsystypes")]
        public ResponseResult getallsystypes()
        {
            var m = PrintHaddle.getAllSysTypes();
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }











    }


}