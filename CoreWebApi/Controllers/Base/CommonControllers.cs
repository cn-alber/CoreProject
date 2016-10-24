using Microsoft.AspNetCore.Mvc;
using CoreData.CoreCore;
// using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
// using CoreData.CoreCore;
using CoreModels.XyCore;
using CoreModels.XyComm;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
// using System.Collections.Generic;

namespace CoreWebApi
{
    [AllowAnonymous]
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
            int x;
            if (!string.IsNullOrEmpty(Type))
            {
                if(int.TryParse(Type, out x))
                {
                    cp.Type = Type;
                }                
            }
            cp.GoodsCode = GoodsCode;
            cp.SkuID = SkuID;
            cp.SCoID = SCoID;
            cp.Brand = Brand;
            cp.Filter = Filter;
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
                var res = CommHaddle.SysColumnExists(DbBase.CommConnectString, "coresku", SortField);
                if (res.s == 1)
                {
                    cp.SortField = SortField;
                    if (!string.IsNullOrEmpty(SortDirection) && (SortDirection.ToUpper() == "DESC" || SortDirection.ToUpper() == "ASC"))
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

        #region 站点列表
        [HttpGetAttribute("/Core/Common/shopsite")]
        public ResponseResult shopsite()
        {
            var data = ShopHaddle.GetShopSite();
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
        #endregion

        #region 0.国/1.省/2.市/3.区列表
        [HttpGetAttribute("/Core/Common/AreaLst")]
        public ResponseResult AreaLst(string LevelType, string ParentId)
        {
            var res = new DataResult(1, null);
            int level, pid;
            if (!int.TryParse(LevelType, out level))
            {
                res.s = -1;
                res.d = "无效参数LevelType";
            }
            if (!int.TryParse(ParentId, out pid))
            {
                res.s = -1;
                res.d = "无效参数ParentId";
            }
            if(res.s==1)
            {
                res = CommHaddle.GetAreaLst(int.Parse(LevelType), int.Parse(ParentId));
            }            
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region //获取省市区所有列表 
        [HttpGetAttribute("/Core/Common/allArea")]
        public ResponseResult allArea()
        {            
            var res = CommHaddle.GetAllArea();            
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 获取品牌列表
        [HttpGetAttribute("/Core/Common/CommBrandLst")]
        public ResponseResult CommBrandLst()
        {
            int CoId = int.Parse(GetCoid());
            var data = BrandHaddle.GetBrandALL(CoId);
            return CoreResult.NewResponse(data.s, data.d, "General");
        }
        #endregion
        #region 获取主仓库列表
        [HttpGetAttribute("/Core/Common/GetParentWarehouseList")]
        public ResponseResult GetParentWarehouseList()
        {   
            int CoID = int.Parse(GetCoid());
            var data = WarehouseHaddle.GetParentWarehouseList(CoID);
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        #endregion
        #region 获取子仓库列表
        [HttpGetAttribute("/Core/Common/GetChildWarehouseList")]
        public ResponseResult GetChildWarehouseList(string ID)
        {   
            var data = new DataResult(1,null);  
            int CoID = int.Parse(GetCoid());
            int id,x;
            if (int.TryParse(ID, out x))
            {
                id = int.Parse(ID);
                data = WarehouseHaddle.GetChildWarehouseList(CoID,id);
            }
            else
            {
                data.s = -1;
                data.d = "参数无效!";
            }
            return CoreResult.NewResponse(data.s, data.d, "General"); 
        }
        #endregion

        #region 获取商品类目列表
        [HttpGetAttribute("/Core/XyComm/Customkind/CommSkuKindLst")]
        public ResponseResult CommSkuKindLst(string ParentID)
        {
            var res = new DataResult(1, null);
            var cp = new CusKindParam();
            cp.CoID = int.Parse(GetCoid());
            int PID = 0;
            if (!int.TryParse(ParentID, out PID))
            {
                res.s = -1;
                res.d = "无效参数ParentID";
            }
            else
            {
                cp.ParentID = int.Parse(ParentID);
                res = CustomKindHaddle.GetCommKindLst(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}
