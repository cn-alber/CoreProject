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
                if (!string.IsNullOrEmpty(obj["BatchID"].ToString()))
                {
                    cp.BatchID = int.Parse(obj["BatchID"].ToString());
                }
                res = ABatchHaddles.GetSortCode(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 单件发货-更新库存
        [HttpPostAttribute("Core/ASaleOut/SetSaleOutSingle")]
        public ResponseResult SetSaleOutSingle([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (string.IsNullOrEmpty(obj["ID"].ToString()) ||
               string.IsNullOrEmpty(obj["SkuAuto"].ToString()) ||
               string.IsNullOrEmpty(obj["OItemAuto"].ToString()) ||
             !string.IsNullOrEmpty(obj["ID"].ToString()) && !int.TryParse(obj["ID"].ToString(), out x))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleOutSet();
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                cp.ID = int.Parse(obj["ID"].ToString());
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.OItemAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<OutItemBatch>(obj["OItemAuto"].ToString());
                cp.Contents = "销售出货";
                ASaleOutHaddles.SaleOutSingle(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 多件发货 - 扫描格号
        [HttpGetAttribute("Core/ASaleOut/ScanOutSortCode")]
        public ResponseResult ScanOutSortCode(string SortCode)
        {
            var res = new DataResult(1, null);
            if (string.IsNullOrEmpty(SortCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleParams();
                cp.CoID = int.Parse(GetCoid());
                cp.SortCode = SortCode;
                res = ASaleOutHaddles.GetScanOutSortCode(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 多件发货 - 扫描件码
        [HttpGetAttribute("Core/ASaleOut/ScanOutScanSkuMulti")]
        public ResponseResult ScanOutScanSkuMulti(string SortCode,string BarCode)
        {
            var res = new DataResult(1, null);
            if (string.IsNullOrEmpty(SortCode)||string.IsNullOrEmpty(BarCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleParams();
                cp.CoID = int.Parse(GetCoid());
                cp.SortCode = SortCode;
                cp.BarCode = BarCode;
                res = ASaleOutHaddles.GetScanOutSkuMulti(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 多件发货 - 更新库存
        [HttpPostAttribute("Core/ASaleOut/SetSaleOutMulti")]
        public ResponseResult SetSaleOutMulti([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (string.IsNullOrEmpty(obj["ID"].ToString()) ||
               string.IsNullOrEmpty(obj["SkuAuto"].ToString()) ||
               string.IsNullOrEmpty(obj["OItemAuto"].ToString()) ||
             !string.IsNullOrEmpty(obj["ID"].ToString()) && !int.TryParse(obj["ID"].ToString(), out x))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASaleOutSet();
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                cp.ID = int.Parse(obj["ID"].ToString());
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.OItemAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<OutItemBatch>(obj["OItemAuto"].ToString());
                cp.Contents = "销售出货";
                ASaleOutHaddles.SaleOutSingle(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion


    }
}