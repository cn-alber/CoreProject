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
        #region 重装箱作业-根据件码获取SKU
        [HttpGetAttribute("Core/ABox/SkuByBarCode")]
        public ResponseResult SkuByBarCode(string BarCode)
        {
            // var Args = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiBoxParam>(obj.ToString());
            var cp = new WmsBoxParams();
            cp.CoID = int.Parse(GetCoid());
            cp.BarCode = BarCode;
            // cp.SkuID = Args.SkuID;
            var res = AWmsBoxHaddle.CheckBarCode(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 重装箱作业-产生装箱资料WmsBox
        [HttpPostAttribute("Core/Api/ABox/SetBoxCode")]
        public ResponseResult SetBoxCode([FromBodyAttribute]JObject obj)
        {
            var Args = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiBoxParam>(obj.ToString());
            var cp = new WmsBoxParams();
            cp.CoID = Args.CoID;
            cp.Creator = Args.Creator;
            cp.SkuID = Args.SkuID;
            cp.ABarCodeLst = Args.ABarCodeLst;
            var res = AWmsBoxHaddle.AddWmsBox(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion        
    }
}