using Microsoft.AspNetCore.Mvc;
using CoreData.CoreCore;
// using Newtonsoft.Json.Linq;
//using Microsoft.AspNetCore.Authorization;
// using CoreData.CoreCore;
using CoreModels.XyCore;
using CoreData.CoreComm;
using CoreData;
// using System.Collections.Generic;

namespace CoreWebApi
{
    //[AllowAnonymous]
    public class CommonController : ControllBase
    {
        [HttpGetAttribute("/Core/Common/ScoCompanySimple")]
        public ResponseResult GetScoCompanySimple()
        {
            int CoId = int.Parse(GetCoid());
            var data = ScoCompanyHaddle.GetScoCompanyAll(CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }

        #region 获取商品Sku
        [HttpGetAttribute("/Core/Common/CommSkuLst")]
        public ResponseResult CommSkuLst(string Type, string GoodsCode, string SkuID, string SCoID, string Brand, string Filter, string Enable, string PageIndex, string PageSize, string SortField, string SortDirection)
        {

            var cp = new CommSkuParam();
            if(!string.IsNullOrEmpty(Type))
            {
                cp.Type = Type;
            }
            cp.GoodsCode = GoodsCode;
            cp.SkuID = SkuID;
            cp.SCoID = SCoID;
            cp.Brand = Brand;
            cp.Filter = Filter;
            if(!string.IsNullOrEmpty(Enable)&&(Enable.ToUpper()=="TRUE"||Enable.ToUpper()=="FALSE"))
            {
                cp.Enable = Enable.ToUpper();
            }
            int x;
            if (int.TryParse(PageIndex, out x))
            {
                cp.PageIndex = int.Parse(PageIndex);
            }
            if (int.TryParse(PageSize, out x))
            {
                cp.PageSize = int.Parse(PageSize);
            }
            //排序参数赋值
            if(!string.IsNullOrEmpty(SortField))
            {
                var res = CommHaddle.SysColumnExists(DbBase.CommConnectString,"coresku",SortField);
                if(res.s == 1)
                {
                    cp.SortField = SortField;
                    if(!string.IsNullOrEmpty(SortDirection)&&(SortDirection.ToUpper()=="DESC"||SortDirection.ToUpper()=="ASC"))
                    {
                        cp.SortDirection = SortDirection.ToUpper();
                    }
                }
            }     
            var result = CoreSkuHaddle.GetCommSkuLst(cp);
            return CoreResult.NewResponse(result.s, result.d, "General");
        }

        // [HttpPostAttribute("/Core/Common/CommSkuLst")]
        // public ResponseResult CommSkuLst([FromBodyAttribute]JObject obj)
        // {
        //     var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CommSkuParam>(obj["CommSkuParam"].ToString());
        //     cp.CoID = int.Parse(GetCoid());
        //     var res = CoreSkuHaddle.GetCommSkuLst(cp);
        //     return CoreResult.NewResponse(res.s, res.d, "General");
        // }

        #endregion
        [HttpGetAttribute("/Core/Common/shopsite")]
        public ResponseResult shopsite()
        {            
            var data = ShopHaddle.GetShopSite();
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
    }
}
