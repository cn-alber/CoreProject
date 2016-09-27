using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using CoreData.CoreComm;
using CoreModels.XyComm;
using System.Collections.Generic;
using CoreModels;
namespace CoreWebApi.Base
{
    [AllowAnonymous]
    public class ShopController:ControllBase
    {
        //查询多笔店铺资料
        [HttpPostAttribute("/Core/Shop/ShopQueryLst")]
        public ResponseResult ShopQueryLst([FromBodyAttribute]JObject obj)
        {
            var cp = new ShopParam();
            cp.CoID = int.Parse(GetCoid());
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

        //查询单笔店铺资料
        [HttpPostAttribute("/Core/Shop/ShopQuery")]
        public ResponseResult ShopQuery([FromBodyAttribute]JObject obj)
        {
            var CoID = GetCoid();
        //    var CoID = "1";
            string shopid = obj["ShopID"].ToString();
            var res = ShopHaddle.ShopQuery(CoID,shopid);
            return CoreResult.NewResponse(res.s,res.d,"General");
        }

        //修改店铺状态（启用|停用）
        [HttpPostAttribute("/Core/Shop/ShopEnable")]
        public ResponseResult ShopEnable([FromBodyAttribute]JObject obj)
        {    
            Dictionary<int,string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,string>>(obj["IDsDic"].ToString());
            string Company = obj["Company"].ToString(); 
            string UserName = obj["UserName"].ToString(); 
            bool Enable = obj["Enable"].ToString().ToUpper()=="TRUE"?true:false;
            var Coid = GetCoid();
            
            var res = ShopHaddle.UptShopEnable(IDsDic,Company,UserName,Enable,Coid);
            return CoreResult.NewResponse(res.s,res.d,"General");
        }


        //保存店铺资料
        [HttpPostAttribute("/Core/Shop/ShopSave")]
         public ResponseResult ShopSave([FromBodyAttribute]JObject obj)
        {
            var Type = obj["EditType"].ToString();//new or edit
            var shop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(obj["shop"].ToString());
            var Res = new DataResult(1,null);
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();            
            // int CoID=1;
            // string UserName = "携云科技";
            if(Type.ToUpper()=="NEW")
            {
                Res = ShopHaddle.InsertShop(shop, CoID, UserName);
            }
            else
            {
                Res = ShopHaddle.UpdateShop(shop, CoID, UserName);
            }
            return CoreResult.NewResponse(Res.s,Res.d,"General");
        }

        
        //判断店铺名称是否已经存在    
        [HttpPostAttribute("/Core/Shop/ShopIsExist")]
         public ResponseResult ShopIsExist([FromBodyAttribute]JObject obj)
         {
             string ShopName = obj["ShopName"].ToString();
             int ID = int.Parse(obj["ID"].ToString());
             int CoID = int.Parse(GetCoid());
             var isexist = ShopHaddle.ExistShop(ShopName, ID, CoID);
             return CoreResult.NewResponse(1,isexist.ToString(),"General");
         }

         //获取所有授权店铺
         [HttpPostAttribute("/Core/Shop/TokenShopLst")]
         public ResponseResult TokenShopLst()
         {
             int CoID = int.Parse(GetCoid());
             var res = ShopHaddle.GetTokenShopLst(CoID);
             return CoreResult.NewResponse(res.s,res.d,"General");
         }

         //获取所有线下店铺
         [HttpPostAttribute("/Core/Shop/OfflineShopLst")]
         public ResponseResult OfflineShopLst()
         {
             int CoID = int.Parse(GetCoid());
             var res = ShopHaddle.GetOfflineShopLst(CoID);
             return CoreResult.NewResponse(res.s,res.d,"General");
         }

         [HttpPostAttribute("/Core/Shop/TokenExpired")]
         public ResponseResult TokenExpired([FromBodyAttribute]JObject obj)
         {
             string shopid = obj["shopid"].ToString();
             string coid = GetCoid();
             var res = ShopHaddle.TokenExpired(shopid,coid);
             return CoreResult.NewResponse(res.s,res.d,"General");
         }







    }
}