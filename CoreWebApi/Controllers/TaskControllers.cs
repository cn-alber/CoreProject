using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi
{
    public class TaskController : ControllBase
    {
        [HttpGetAttribute("/core/task/data")]
        public ResponseResult taskdata()
        {
            // var roleid = GetRoleid();
            // var coid = GetCoid();
            // // var roleid = "1";
            // // var coid = "1";
            // var m = UserHaddle.GetRefreshList(roleid, coid,GetUname(),GetUid());
            return CoreResult.NewResponse(1, null, "Basic");
        }

    }


}