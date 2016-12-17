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
    public class ASaleOutController : ControllBase
    {
        #region 发货-指定批次（单件，大单|现场）
        [HttpGetAttribute("Core/ASaleOut/GetOutBatch")]
        public ResponseResult GetOutBatch(string BatchType)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(!string.IsNullOrEmpty(BatchType) && int.TryParse(BatchType, out x)) || string.IsNullOrEmpty(BatchType))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleParams();
                cp.CoID = int.Parse(GetCoid());
                cp.BatchType = int.Parse(BatchType);

            }

            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 单件发货-扫描件码
        [HttpPostAttribute("Core/ASaleOut/SaleOutScanSku")]
        public ResponseResult SaleOutScanSku([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (string.IsNullOrEmpty(obj["BarCode"].ToString()) ||
             !string.IsNullOrEmpty(obj["BatchID"].ToString()) && !int.TryParse(obj["BatchID"].ToString(), out x))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ABatchParams();
                cp.CoID = int.Parse(GetCoid());
                cp.BarCode = obj["BarCode"].ToString();
                if(!string.IsNullOrEmpty(obj["BatchID"].ToString()))
                {
                    cp.BatchID = int.Parse(obj["BatchID"].ToString());
                }
                res = ABatchHaddles.GetSortCode(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 单件发货-更新库存
        
        #endregion
    }
}