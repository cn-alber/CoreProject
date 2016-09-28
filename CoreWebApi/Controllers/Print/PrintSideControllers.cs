using Microsoft.AspNetCore.Mvc;
using CoreDate.CoreComm;
using Newtonsoft.Json.Linq;

namespace CoreWebApi.Print
{
    /// <summary>
	/// 打印模块 - 打印组件侧边栏 
	/// </summary>
    public class PrintSideController : ControllBase
    {

        #region 侧边栏设定，更新默认模板
        [HttpPostAttribute("/core/print/side/setdefed")]
        public ResponseResult sidesetdefed([FromBodyAttribute]JObject lo)
        {
            if(!checkInt(lo["my_tpl_id"].ToString())) return CoreResult.NewResponse(-4023, null, "Print");
            var admin_id = GetUid();
            var m = PrintHaddle.sideSetdefed(admin_id,lo["my_tpl_id"].ToString());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 侧边栏删除模板
        [HttpGetAttribute("/core/print/side/remove")]
        public ResponseResult remove(int my_tpl_id)
        {
            if(!checkInt(my_tpl_id)) return CoreResult.NewResponse(-4023, null, "Print");
            var admin_id = GetUid();
            var m = PrintHaddle.sideRemove(my_tpl_id.ToString());
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion

        #region 获取边栏模板内容
        [HttpGetAttribute("/core/print/tpl/sideTpls")]
        public ResponseResult sidetpls(int type)
        {
            if(!checkInt(type)) return CoreResult.NewResponse(-4001, null, "Print");
            var admin_id = GetUid();
            var m = PrintHaddle.GetSideTpls(type.ToString(),admin_id);
            return CoreResult.NewResponse(m.s, m.d, "Print");
        }
        #endregion


        





    
    }

}