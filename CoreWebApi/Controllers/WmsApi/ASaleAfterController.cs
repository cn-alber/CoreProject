using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels;
using CoreModels.WmsApi;
using CoreData.CoreWmsApi;
using System;
using System.Collections.Generic;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class ASaleAfterController : ControllBase
    {
         #region 发货-指定批次（单件，大单|现场）原CallSOAfterSku
        [HttpGetAttribute("Core/ASaleAfter/SaleAfterScanSku")]
        public ResponseResult SaleAfterScanSku(string BarCode)
        {
            var res = new DataResult(1, null);
            int x;
            if (string.IsNullOrEmpty(BarCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleParams();
                cp.CoID = int.Parse(GetCoid());
                cp.BarCode = BarCode;
                res = ASaleOutHaddles.OutBatch(cp);
            }

            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion
    }
}