using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreCore;
using CoreModels.XyCore;
using System.Collections.Generic;
// using CoreModels;
namespace CoreWebApi.XyCore
{
    [AllowAnonymous]
    public class CoreSkuControllers : ControllBase
    {
        #region 商品管理-获取商品主要资料
        [HttpPostAttribute("Core/XyCore/CoreSku/GoodsQueryLst")]
        public ResponseResult GoodsQueryLst([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuParam>(obj["CoreSkuParam"].ToString());
            cp.CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.GetGoodsLst(cp);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
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
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion

        #region 商品管理 - 获取单笔Sku详情
        [HttpPostAttribute("Core/XyCore/CoreSku/GoodsQuery")]
        public ResponseResult GoodsQuery([FromBodyAttribute]JObject obj)
        {
            string GoodsCode = obj["GoodsCode"].ToString();
            int CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.GetCoreSkuEdit(GoodsCode, CoID);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion

        #region Update商品删除标记
        [HttpPostAttribute("Core/XyCore/CoreSku/UptGoodsDel")]
        public ResponseResult UptGoodsDel([FromBodyAttribute]JObject obj)
        {
            List<string> GoodsLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(obj["GoodsLst"].ToString());
            // string UserName = obj["UserName"].ToString();
            bool IsDelete = (obj["IsDelete"].ToString().ToUpper() == "TRUE") ? true : false;
            // int CoID = 1;
            string UserName = GetUname();
            int CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.DelGoods(GoodsLst, IsDelete, UserName, CoID);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion

        #region Update商品删除标记
        [HttpPostAttribute("Core/XyCore/CoreSku/UptSkuDel")]
        public ResponseResult UptSkuDel([FromBodyAttribute]JObject obj)
        {
            string Sku = obj["Sku"].ToString();
            //string UserName = obj["UserName"].ToString();
            bool IsDelete = (obj["IsDelete"].ToString().ToUpper() == "TRUE") ? true : false;
            // int CoID = 1;
            string UserName = GetUname();
            int CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.DelSku(Sku, IsDelete, UserName, CoID);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion


        #region 删除回收站
        [HttpPostAttribute("Core/XyCore/CoreSku/DelGoodsRec")]
        public ResponseResult DelGoodsRec([FromBodyAttribute]JObject obj)
        {
            List<string> GoodsLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(obj["GoodsLst"].ToString());
            // int CoID = 1;
            int CoID = int.Parse(GetCoid());
            var res = CoreSkuHaddle.DelGoodsRec(GoodsLst, CoID);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion

        #region 新增Sku
        [HttpPostAttribute("Core/XyCore/CoreSku/InsertGoods")]
        public ResponseResult InsertGoods([FromBodyAttribute]JObject obj)
        {
            CoreSkuAuto ckm = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuAuto>(obj["CoreSkuAuto"].ToString());
            CoreSkuItem cki = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuItem>(obj["CoreSkuItem"].ToString());
            var res = CoreSkuHaddle.NewCore(ckm, cki);
            var Result = CoreResult.NewResponse(res.s, res.d, "General");
            return Result;
        }
        #endregion    

        #region 采购查询
        [HttpPostAttribute("Core/XyCore/CoreSku/SkuQuery")]
        public ResponseResult SkuQuery([FromBodyAttribute]JObject obj)
        {
        var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<SkuParam>(obj["SkuParam"].ToString());
        int CoID = int.Parse(GetCoid());
        var res = CoreSkuHaddle.GetSkuAll(cp,CoID,2);
        var Result = CoreResult.NewResponse(res.s, res.d, "General");
        return Result;
        }

        #endregion
    }

}