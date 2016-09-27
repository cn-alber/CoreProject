using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Api.JingDong{
    [AllowAnonymous]
    public class JdOrderControllers : ControllBase
    {

        #region 
        [HttpGetAttribute("/core/Api/JdOrder/hello")]
        public ResponseResult taskdata()
        {
            
            return CoreResult.NewResponse(1, null, "Api");
        }
        #endregion




    }
}