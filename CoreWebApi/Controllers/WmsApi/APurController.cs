using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreModels;
using CoreModels.WmsApi;
using CoreData.CoreWmsApi;
using System;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class APurController : ControllBase
    {
        #region 收货作业-根据箱码or件码_获取SKU
        [HttpGetAttribute("Core/APur/GetBox")]
        public ResponseResult GetBox(string BoxCode)
        {
            // var cp =new WmsBoxParams();
            // cp.CoID=CoID;
            // cp.BoxCode=BoxCode;
            // cp.Type=4;//进货仓;
            // cp.SkuID = (BoxCode.Length - 6)>0?BoxCode.Substring(0, BoxCode.Length - 6):BoxCode;//假设sku,检查条码为件码or箱码
            // var res = APurHaddles.GetBoxSku(cp);
            var res = new DataResult(1, null);
            if (string.IsNullOrEmpty(BoxCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new ASkuScanParam();
                cp.CoID = int.Parse(GetCoid());
                cp.BarCode = BoxCode;
                res = ASkuScanHaddles.GetType(cp);
            }

            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion
        #region 新增采购收料单
        [HttpPostAttribute("Core/APur/SetPurRec")]
        public ResponseResult SetPurRec([FromBodyAttribute]JObject obj)
        {
            int x;
            var res = new DataResult(1, null);
            if (!(obj["WhID"] != null && int.TryParse(obj["WhID"].ToString(), out x) && obj["PurID"] != null && int.TryParse(obj["PurID"].ToString(), out x) && obj["RecSkuLst"] != null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRecParam>(obj.ToString());
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = APurHaddles.SetPurRecDetail(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 新增其他收料单
        [HttpPostAttribute("Core/APur/SetOtherRec")]
        public ResponseResult SetOtherRec([FromBodyAttribute]JObject obj)
        {
            int x;
            var res = new DataResult(1, null);
            if (!(obj["WhID"] != null && int.TryParse(obj["WhID"].ToString(), out x) && obj["RecSkuLst"] != null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRecParam>(obj.ToString());
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = APurHaddles.SetRecDetail(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion
    }
}
