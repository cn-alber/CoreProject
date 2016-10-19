using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.WmsApi;
using CoreData.CoreWmsApi;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class ABoxController : ControllBase
    {
         #region
         [HttpPostAttribute("Core/Api/ABox/SkuByBarCode")]
         public ResponseResult SkuByBarCode([FromBodyAttribute]JObject obj)
         {
            var Args = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiBoxParam>(obj.ToString());
            var cp = new WmsBoxParams();
            cp.CoID = Args.CoID;
            cp.BarCode = Args.BarCode;
            cp.SkuID = Args.SkuID;
            var res =AWmsBoxHaddle.CheckBarCode(cp);
            return CoreResult.NewResponse(res.s, res.d, "Indentity");
         }
         #endregion
    }
}