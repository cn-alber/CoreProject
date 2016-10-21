using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreCore;
using CoreModels.XyCore;

namespace CoreWebApi.XyCore
{
    // [AllowAnonymous]
    public class CoreSkuMatControllers : ControllBase
    {
        #region 物料管理-获取物料资料
        [HttpPostAttribute("Core/XyCore/CoreSku/MatQueryLst")]
        public ResponseResult MatQueryLst([FromBodyAttribute]JObject obj)
        {
            CoreSkuParam cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuParam>(obj["CoreSkuParam"].ToString());
            cp.CoID = int.Parse(GetCoid());
            var res = CoreSkuMatHaddle.GetMatLst(cp);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion

        #region 物料管理 - 获取单笔物料详情
        [HttpPostAttribute("Core/XyCore/CoreSku/MatQuery")]
        public ResponseResult GoodsQuery([FromBodyAttribute]JObject obj)
        {
            string GoodsCode = obj["GoodsCode"].ToString();
            int CoID = int.Parse(GetCoid());
            var res = CoreSkuMatHaddle.GetCoreMatEdit(GoodsCode, CoID);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion

        #region 物料管理 - 保存物料资料
        [HttpPostAttribute("Core/XyCore/CoreSku/SaveMat")]
        public ResponseResult SaveMat([FromBodyAttribute]JObject obj)
        {
            CoreSkuMatAuto ckm = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuMatAuto>(obj["CoreSkuMatAuto"].ToString());
            var res = CoreSkuMatHaddle.SaveSkuMat(ckm);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion

    }
}