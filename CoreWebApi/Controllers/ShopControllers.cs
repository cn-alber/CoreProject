using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreComm;
using CoreModels;

namespace CoreWebApi
{
    public class ShopController:ControllBase
    {
        [AllowAnonymous]
        [HttpPostAttribute("/Core/Shop/CallShop")]
        public ResponseResult CallShop([FromBodyAttribute]JObject obj)
        {
            var cp = new ShopParam();
            cp.CoID = int.Parse(obj["CoID"].ToString()); 
            cp.Enable = obj["Enable"].ToString(); 
            cp.Filter = obj["Filter"].ToString();
            cp.PageSize = int.Parse(obj["PageSize"].ToString());
            cp.PageIndex = int.Parse(obj["PageIndex"].ToString());
            cp.SortField = obj["SortField"].ToString();
            cp.SortDirection = obj["SortDirection"].ToString();
            var res = ShopHaddle.GetShopAll(cp);
            var Result = CoreResult.NewResponse(res.s,res.d,"Basic");
            return Result;             
        }

        [AllowAnonymous]
        [HttpPostAttribute("/Core/Shop/ShopEdit")]
        public ResponseResult ShopEdit([FromBodyAttribute]JObject obj)
        {
            var CoID = obj["CoID"].ToString(); 
            //CoID = GetCoid();
            string shopid = obj["ShopID"].ToString();
            var res = ShopHaddle.GetShopEdit(CoID,shopid);
            return CoreResult.NewResponse(res.s,res.d,"General");
        }
    }
}