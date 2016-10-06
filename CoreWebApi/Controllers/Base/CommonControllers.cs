using Microsoft.AspNetCore.Mvc;
using CoreData.CoreCore;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
// using CoreData.CoreCore;
using CoreModels.XyCore;
// using System.Collections.Generic;

namespace CoreWebApi
{
    [AllowAnonymous]
    public class CommonController : ControllBase
    {
        [HttpGetAttribute("/Core/Common/ScoCompanySimple")]
        public ResponseResult GetScoCompanySimple()
        {   
            int CoId = int.Parse(GetCoid());
            var data = ScoCompanyHaddle.GetScoCompanyAll(CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        #region 获取商品Sku
        [HttpPostAttribute("/Core/Common/CommSkuLst")]
        public ResponseResult CommSkuLst([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CommSkuParam>(obj["CommSkuParam"].ToString());
            cp.CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.GetCommSkuLst(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion
    }
}
