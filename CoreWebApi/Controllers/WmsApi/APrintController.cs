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
    public class APrintController : ControllBase
    {
        #region 打印设置 - 获取打印机列表
        [HttpGetAttribute("Core/APrint/GetPrinter")]
        public ResponseResult GetPrinter(string PrintType)
        {
            var res = new DataResult(1, null);
            int x;
            if (string.IsNullOrEmpty(PrintType) || !string.IsNullOrEmpty(PrintType) && int.TryParse(PrintType, out x))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new APrintParams();
                cp.CoID = int.Parse(GetCoid());
                cp.PrintType = int.Parse(PrintType);
                res = APrintHaddles.GetPrintDetail(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 打印设置 - 设置打印机默认IP
        [HttpPostAttribute("Core/APrint/SetPrint")]
        public ResponseResult SetPrint([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["PrintType"] != null && int.TryParse(obj["PrintType"].ToString(), out x) &&
               obj["PrintID"] == null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new APrintParams();
                cp.CoID = int.Parse(GetCoid());
                cp.Modifier = GetUname();
                cp.ModifyDate = DateTime.Now.ToString();
                cp.PrintID = obj["PrintID"].ToString();
                cp.PrintType = int.Parse(obj["PrintType"].ToString());
                res = APrintHaddles.SetPrintIP(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 其他作业 - 快递单打印 原CallOtherExpress
        [HttpPostAttribute("Core/APrint/CallExpressBySku")]
        public ResponseResult CallExpressBySku(string BarCode)
        {
            var res = new DataResult(1, null);
            if (string.IsNullOrEmpty(BarCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new APrintParams();
                cp.CoID = int.Parse(GetCoid());
                cp.BarCode = BarCode;
                res = APrintHaddles.ExpressBySku(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }

        #endregion

        #region 其他作业 - 条码扫描确认货位
        [HttpPostAttribute("Core/APrint/CallExpressBySku")]
        public ResponseResult CallBarCodeLocat(string BarCode)
        {
            var res = new DataResult(1, null);
            if (string.IsNullOrEmpty(BarCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new APrintParams();
                cp.CoID = int.Parse(GetCoid());
                cp.BarCode = BarCode;
                res = APrintHaddles.ExpressBySku(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion
    }
}