using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyComm;

namespace CoreWebApi
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

        #region 获取个人模板 print_uses
        [HttpGetAttribute("/core/print/task/tpl")]
        public ResponseResult tasktpl(string my_id)
        {
            var admin_id = "1";//GetUid();
            var m = PrintHaddle.taskTpl(admin_id,my_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 设定，更新默认模板
        [HttpGetAttribute("/core/print/side/setdefed")]
        public ResponseResult sidesetdefed(string my_tpl_id)
        {
            var admin_id = "1";//GetUid();
            var m = PrintHaddle.sideSetdefed(admin_id,my_tpl_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion
        
        #region 获取print_sys_types list
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

        #region 获取个人模板数据
       [HttpGetAttribute("/core/print/tpl/my")]
        public ResponseResult tplmy(string my_id)
        {
            string admin_id = "1";
            var m = PrintHaddle.tplMy(admin_id,my_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 根据type获取系统模板 list
        [HttpGetAttribute("/core/print/tpl/sysesbytype")]
        public ResponseResult sysesbytype(string type,int PageIndex,int PageSize)
        {

            printParam param = new printParam();
            param.Filter = "type = "+type;
            param.PageIndex = PageIndex;
            param.PageSize = PageSize;
            
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






    }


}