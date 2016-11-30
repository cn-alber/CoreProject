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
        [HttpGetAttribute("Core/Api/APur/GetBox")]
        public ResponseResult GetBox(string BoxCode)
        {
            // var cp =new WmsBoxParams();
            // cp.CoID=CoID;
            // cp.BoxCode=BoxCode;
            // cp.Type=4;//进货仓;
            // cp.SkuID = (BoxCode.Length - 6)>0?BoxCode.Substring(0, BoxCode.Length - 6):BoxCode;//假设sku,检查条码为件码or箱码
            // var res = APurHaddles.GetBoxSku(cp);
            var cp = new ASkuScanParam();
            cp.CoID = int.Parse(GetCoid());
            cp.BarCode = BoxCode;
            var res = ASkuScanHaddles.GetType(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
        #region 新增收料单
        [HttpPostAttribute("Core/Api/APur/SetPurRec")]
        public ResponseResult SetPurRec([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiRecParam>(obj.ToString());
            // var res = new DataResult(1,null);
            cp.CoID = int.Parse(GetCoid());
            cp.Creator = GetUname();
            cp.CreateDate = DateTime.Now.ToString();
            var res = APurHaddles.SetPurRecDetail(cp);//new {s=1,d=Args};

            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}
