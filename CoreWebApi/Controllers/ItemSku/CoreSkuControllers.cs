using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreCore;
using CoreModels.XyCore;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
// using CoreModels;
namespace CoreWebApi.XyCore

{
    // [AllowAnonymous]
    public class CoreSkuControllers : ControllBase
    {
        #region 商品管理-获取商品主要资料
        [HttpGetAttribute("Core/XyCore/CoreSku/GoodsQueryLst")]
        public ResponseResult GoodsQueryLst(string Type,string GoodsCode,string GoodsName, string Filter, string Enable, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new CoreSkuParam();
            int x;
            cp.Filter = Filter;
            if (!string.IsNullOrEmpty(Type)&&int.TryParse(Type, out x))
            {
                cp.Type = int.Parse(Type);
            }
            if (!string.IsNullOrEmpty(Enable) && (Enable.ToUpper() == "TRUE" || Enable.ToUpper() == "FALSE"))
            {
                cp.Enable = Enable.ToUpper();
            }
            if (int.TryParse(PageIndex, out x))
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            if (int.TryParse(PageSize, out x))
            {
                cp.PageSize = int.Parse(PageSize);
            }
            //排序参数赋值
            if (!string.IsNullOrEmpty(SortField))
            {
                var res = CommHaddle.SysColumnExists(DbBase.CoreConnectString, "coresku", SortField);
                if (res.s == 1)
                {
                    cp.SortField = SortField;
                    if (!string.IsNullOrEmpty(SortDirection) && (SortDirection.ToUpper() == "DESC" || SortDirection.ToUpper() == "ASC"))
                    {
                        cp.SortDirection = SortDirection.ToUpper();
                    }
                }
            }
            cp.CoID = int.Parse(GetCoid());
            // var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreSkuParam>(obj["CoreSkuParam"].ToString());
            cp.CoID = int.Parse(GetCoid());
            var Result = CoreSkuHaddle.GetGoodsLst(cp);
            return  CoreResult.NewResponse(Result.s, Result.d, "General");
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