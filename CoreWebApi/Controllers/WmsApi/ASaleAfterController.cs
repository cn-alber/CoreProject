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
            // int x;
            if (string.IsNullOrEmpty(BarCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleAfterParam();
                cp.CoID = int.Parse(GetCoid());
                cp.BarCode = BarCode;
                res = ASaleAfterHaddles.AfterScanSku(cp);
            }

            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion


        #region 售后退回-产生收货单&更新库存
        [HttpGetAttribute("Core/ASaleAfter/SetSaleAfter")]
        public ResponseResult SetSaleAfter([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (obj["SkuAuto"] == null || obj["ExCode"] == null || obj["issueName"] == null ||
            !string.IsNullOrEmpty(obj["OID"].ToString()) && int.TryParse(obj["OID"].ToString(), out x) &&
            !string.IsNullOrEmpty(obj["ASID"].ToString()) && int.TryParse(obj["ASID"].ToString(), out x) &&
            !string.IsNullOrEmpty(obj["SoID"].ToString()) && int.TryParse(obj["SoID"].ToString(), out x)
            )
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleAfterParam();
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                cp.Type  =3;//3.销退仓
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.ExCode = obj["ExCode"].ToString();
                cp.issueName = obj["issueName"].ToString();
                if(!string.IsNullOrEmpty(obj["OID"].ToString()))
                {
                    cp.OID = int.Parse(obj["OID"].ToString());
                }
                if(!string.IsNullOrEmpty(obj["ASID"].ToString()))
                {
                    cp.OID = int.Parse(obj["ASID"].ToString());
                }
                if(!string.IsNullOrEmpty(obj["SoID"].ToString()))
                {
                    cp.OID = int.Parse(obj["SoID"].ToString());
                }            
                res = ASaleAfterHaddles.SetSOAfter(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion
    }
}