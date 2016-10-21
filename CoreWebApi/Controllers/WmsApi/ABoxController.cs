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
        [HttpPostAttribute("Core/Api/ABox/SkuByBarCode")]
        public ResponseResult SkuByBarCode([FromBodyAttribute]JObject obj)
        {
            var Args = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiBoxParam>(obj.ToString());
            var cp = new WmsBoxParams();
            cp.CoID = Args.CoID;
            cp.BarCode = Args.BarCode;
            cp.SkuID = Args.SkuID;
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
        #region 收货作业-根据箱码or件码_获取SKU
        [HttpGetAttribute("Core/Api/ABox/GetBox")]
        public ResponseResult GetBox(int CoID, string BoxCode)
        {
            var cp =new WmsBoxParams();
            cp.CoID=CoID;
            cp.BoxCode=BoxCode;
            cp.Type=4;//进货仓;
            cp.SkuID = (BoxCode.Length - 6)>0?BoxCode.Substring(0, BoxCode.Length - 6):BoxCode;//假设sku,检查条码为件码or箱码
            var res = AWmsBoxHaddle.GetBoxSku(cp);
            return CoreResult.NewResponse(res.s,res.d,"General");
        }
        #endregion
    }
}