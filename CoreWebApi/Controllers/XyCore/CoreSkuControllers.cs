using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreCore;
using CoreModels.XyCore;
// using System.Collections.Generic;
// using CoreModels;
namespace CoreWebApi.XyCore
{
    [AllowAnonymous]
    public class CoreSkuControllers:ControllBase
    {
        #region 商品管理-获取商品主要资料
        [HttpPostAttribute("Core/XyCore/CoreSku/GoodsQueryLst")]
        public ResponseResult GoodsQueryLst([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuParam>(obj["CoreSkuParam"].ToString());
            cp.CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.GetGoodsLst(cp);
            var Result = CoreResult.NewResponse(res.s,res.d,"General");
            return Result;   
        }
        #endregion
        #region 商品管理 - 获取商品明细列表
        [HttpPostAttribute("Core/XyCore/CoreSku/SkuQueryLst")]
        public ResponseResult SkuQueryLst([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuParam>(obj["CoreSkuParam"].ToString());
            cp.CoID = int.Parse(GetCoid());            
            // var Result = CoreResult.NewResponse(-3001,null,"General");
            var res = CoreSkuHaddle.GetSkuLst(cp);
            var Result = CoreResult.NewResponse(res.s,res.d,"General");
            return Result;   
        }
        #endregion

        #region 商品管理 - 获取单笔Sku详情
        [HttpPostAttribute("Core/XyCore/CoreSku/GoodsQuery")]
        public ResponseResult GoodsQuery([FromBodyAttribute]JObject obj)
        {
            string GoodsCode = obj["GoodsCode"].ToString();
            int CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.GetCoreSkuEdit(GoodsCode,CoID);
            var Result = CoreResult.NewResponse(res.s,res.d,"General");
            return Result; 
        }
        #endregion

    }

}