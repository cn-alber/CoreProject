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