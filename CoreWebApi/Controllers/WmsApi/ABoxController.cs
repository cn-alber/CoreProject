using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels.WmsApi;
using CoreData.CoreWmsApi;
using System;
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
        [HttpPostAttribute("Core/ABox/SetBoxCode")]
        public ResponseResult SetBoxCode([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiBoxParam>(obj.ToString());           
            cp.CoID = int.Parse(GetCoid());
            cp.Creator = GetUname();
            cp.CreateDate = DateTime.Now.ToString();
            var res = AWmsBoxHaddle.AddWmsBox(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion    

        #region 箱件码判断测试
        [HttpGetAttribute("Core/ABox/GetScanType")]
        public ResponseResult GetScanType(string BarCode)
        {
            var cp = new ASkuScanParam();
            cp.CoID = int.Parse(GetCoid());
            cp.BarCode = BarCode;
            var res = ASkuScanHaddles.GetType(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion    
    }
}