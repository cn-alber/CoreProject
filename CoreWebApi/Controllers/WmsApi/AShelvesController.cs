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
    public class AShelvesController : ControllBase
    {
        #region 货品上架 - 扫描Sku,检查货位库存,返回有效货位
        [HttpGetAttribute("Core/AShelves/GetUpSkuByCode")]
        public ResponseResult GetUpSkuByCode(string BoxCode, string WarehouseID, string PCode)
        {
            var res = new DataResult(1, null);
            int x;
            if (!string.IsNullOrEmpty(WarehouseID) && !int.TryParse(WarehouseID, out x))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfParam();
                cp.BoxCode = BoxCode;
                if (!string.IsNullOrEmpty(WarehouseID))
                {
                    cp.WarehouseID = int.Parse(WarehouseID);
                }
                cp.CoID = int.Parse(GetCoid());
                cp.TypeLst = new List<int>();
                cp.TypeLst.Add(1);
                cp.TypeLst.Add(2);
                res = AShelvesHaddles.GetUpBoxSku(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion
        #region 货品上架 - 修改仓库or货位，检查货位库存
        [HttpGetAttribute("Core/AShelves/CheckWhPCode")]
        public ResponseResult CheckWhPCode(string Skuautoid, string Qty, string WarehouseID, string PCode)
        {
            var res = new DataResult(1, null);
            int x;
            if (!string.IsNullOrEmpty(Skuautoid) && !int.TryParse(Skuautoid, out x) &&
                !string.IsNullOrEmpty(Qty) && !int.TryParse(Qty, out x) &&
                !string.IsNullOrEmpty(WarehouseID) && !int.TryParse(WarehouseID, out x))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfParam();
                if (!string.IsNullOrEmpty(Skuautoid))
                {
                    cp.Skuautoid = int.Parse(Skuautoid);
                }
                if (!string.IsNullOrEmpty(Qty))
                {
                    cp.Qty = int.Parse(Qty);
                }
                if (!string.IsNullOrEmpty(WarehouseID))
                {
                    cp.WarehouseID = int.Parse(WarehouseID);
                }
                cp.CoID = int.Parse(GetCoid());
                cp.TypeLst = new List<int>();
                cp.TypeLst.Add(1);
                cp.TypeLst.Add(2);
                res = AShelvesHaddles.GetUpPCode(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }

        #endregion
        #region 货品上架 - 新增上架功能
        [HttpPostAttribute("Core/AShelves/SetUpPile")]
        public ResponseResult SetUpPile([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["PileID"] != null && int.TryParse(obj["PileID"].ToString(), out x) && obj["SkuAuto"] != null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfSet();
                cp.PileID = int.Parse(obj["PileID"].ToString());
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.Type = 4;
                cp.Contents = "货品上架";
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = AShelvesHaddles.SetUpShelfPile(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 移库下架 - 货位扫描
        [HttpGetAttribute("Core/AShelves/SkuByArea")]
        public ResponseResult SkuByArea(string PCode)
        {
            var res = new DataResult(1, null);
            if (string.IsNullOrEmpty(PCode))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfParam();
                cp.CoID = int.Parse(GetCoid());
                cp.PCode = PCode;
                res = AShelvesHaddles.GetAreaSku(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 移库上架 - 产生上架资料
        [HttpPostAttribute("Core/AShelves/MovInPile")]
        public ResponseResult MovInPile([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["PileID"] != null && int.TryParse(obj["PileID"].ToString(), out x) && obj["SkuAuto"] != null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfSet();
                cp.PileID = int.Parse(obj["PileID"].ToString());
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.Type = 3;
                cp.Contents = "移库上架";
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = AShelvesHaddles.SetUpShelfPile(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 移库下架 - 产生下架资料
        [HttpPostAttribute("Core/AShelves/MoveOutShelfPile")]
        public ResponseResult MoveOutShelfPile([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["PileID"] != null && int.TryParse(obj["PileID"].ToString(), out x) && obj["SkuAuto"] != null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfSet();
                cp.PileID = int.Parse(obj["PileID"].ToString());
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.Type = 3;
                cp.Contents = "移库下架";
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = AShelvesHaddles.SetOffShelfPile(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 强制下架 - 产生下架资料
        [HttpPostAttribute("Core/AShelves/OffShelfPile")]
        public ResponseResult OffShelfPile([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["PileID"] != null && int.TryParse(obj["PileID"].ToString(), out x) && obj["SkuAuto"] != null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfSet();
                cp.PileID = int.Parse(obj["PileID"].ToString());
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.Type = 3;
                cp.Contents = "强制下架";
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = AShelvesHaddles.SetOffShelfPile(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

        #region 托盘上架 - 产生上架资料
        [HttpPostAttribute("Core/AShelves/TrayUp")]
        public ResponseResult TrayUp([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            // int x;
            if (!(obj["PCode"] != null && obj["SkuAuto"] != null))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfSet();
                cp.PCode = obj["PCode"].ToString();
                cp.SkuAuto = Newtonsoft.Json.JsonConvert.DeserializeObject<ASkuScan>(obj["SkuAuto"].ToString());
                cp.Type = 4;
                cp.Contents = "托盘上架";
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = AShelvesHaddles.TrayUp(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion 

        #region 库存盘点
        [HttpPostAttribute("Core/AShelves/SetAreaQty")]
        public ResponseResult SetAreaQty([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            int x;
            if (!(obj["PileID"] != null && int.TryParse(obj["PileID"].ToString(), out x) && obj["Qty"] != null && int.TryParse(obj["Qty"].ToString(), out x) ))
            {
                res.s = -1;
                res.d = "无效参数";
            }
            else
            {
                var cp = new AShelfSet();
                cp.PileID = int.Parse(obj["PileID"].ToString());
                cp.Qty = int.Parse(obj["Qty"].ToString());
                cp.Contents = "库存盘点";
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = AShelvesHaddles.SetAreaSkuQty(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "WmsApi");
        }
        #endregion

    }
}