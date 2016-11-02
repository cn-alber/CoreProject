using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using CoreModels.XyComm;
using System.Collections.Generic;
using System;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
namespace CoreWebApi
{

    [AllowAnonymous]
    public class SkuPropsController : ControllBase
    {

        #region 商品类目属性维护 - 打开属性列表
        [HttpGetAttribute("Core/XyComm/CustomKindSkuProps/SkuProps")]
        public ResponseResult SkuProps()
        {
            string CoID = GetCoid();
            var res = SkuPropsHaddle.GetSkuProps(CoID);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion
        #region 商品规格属性维护 - 删除Sku属性值
        [HttpPostAttribute("Core/XyComm/CustomKindSkuProps/DelSkuPropsValues")]
        public ResponseResult DelSkuPropsValues([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            string ID = obj["ID"].ToString();
            int x = 0;
            if (!int.TryParse(ID, out x))
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            else
            {
                string CoID = GetCoid();
                string UserName = GetUname();
                res = SkuPropsHaddle.DelSkuProps(ID, CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品规格属性维护 - 修改Sku属性值
        [HttpPostAttribute("Core/XyComm/CustomKindSkuProps/UptSkuPropsValues")]
        public ResponseResult UptSkuPropsValues([FromBodyAttribute]JObject obj)
        {
            var SkuPropLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<skuprops>>(obj["SkuPropLst"].ToString());
            string CoID = GetCoid();
            string UserName = GetUname();
            var res = SkuPropsHaddle.UptSkuProps(SkuPropLst, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品资料新增 - 根据商品类目获取Sku属性值
        [HttpGetAttribute("Core/XyComm/CustomKindSkuProps/SkuPropsByKind")]
        public ResponseResult SkuPropsByKind(string KindID)
        {
            var res = new DataResult(1, null);
            int x = 0;
            if (!int.TryParse(KindID, out x))
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            else
            {
                string CoID = GetCoid();
                res = SkuPropsHaddle.GetSkuPropsByKind(KindID,CoID);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion
        // #region 商品资料新增 - 根据商品类目获取颜色列表
        // [HttpGetAttribute("Core/XyComm/ColorSize/GetColorLst")]
        // public ResponseResult GetColorLst(string ID)
        // {
        //     var res = new DataResult(1,null);
        //     string CoID = GetCoid();
        //     int KindID = 0;
        //     if (int.TryParse(ID, out KindID))
        //     {
        //         KindID = int.Parse(ID);
        //         res = CoreColorHaddle.GetKindColorLst(KindID,CoID);
        //     }
        //     else
        //     {
        //         res.s = -1;
        //         res.d = "无效参数ID";
        //     }
        //     return CoreResult.NewResponse(res.s, res.d, "General");
        // }
        // #endregion

        // #region 商品资料新增 - 根据商品类目获取颜色列表
        // [HttpGetAttribute("Core/XyComm/ColorSize/GetSizeLst")]
        // public ResponseResult GetSizeLst(string ID)
        // {
        //     var res = new DataResult(1,null);
        //     string CoID = GetCoid();
        //     int KindID = 0;
        //     if (int.TryParse(ID, out KindID))
        //     {
        //         KindID = int.Parse(ID);
        //         res = CoreSizeHaddle.GetKindSizeLst(KindID,CoID);
        //     }
        //     else
        //     {
        //         res.s = -1;
        //         res.d = "无效参数ID";
        //     }
        //     return CoreResult.NewResponse(res.s, res.d, "General");
        // }
        // #endregion


    }
}