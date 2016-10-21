using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.WmsApi;
using CoreData.CoreWmsApi;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class APileController : ControllBase
    {
        #region 新增收料单
        [HttpPostAttribute("Core/Api/APile/SetPurRec")]
        public ResponseResult SetPurRec([FromBodyAttribute]JObject obj)
        {
            
            // var Args
            var res = new{s=1,d=2};
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}
