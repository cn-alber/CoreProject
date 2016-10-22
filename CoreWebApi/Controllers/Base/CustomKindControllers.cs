using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using CoreModels.XyComm;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
namespace CoreWebApi
{

    [AllowAnonymous]
    public class CustomkindController : ControllBase
    {
        #region 获取商品类目列表
        [HttpGetAttribute("/Core/XyComm/Customkind/SkuKindLst")]
        public ResponseResult SkuKindLst(string ParentID)
        {
            var res = new DataResult(1, null);
            string CoID = GetCoid();
            int PID = 0;
            if (int.TryParse(ParentID, out PID))
            {
                PID = int.Parse(ParentID);
                res = CustomKindHaddle.GetKindLst("商品类目", PID, CoID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ParentID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 获取商品标准类目列表
        [HttpGetAttribute("/Core/XyComm/ItemCateStd/ItemStdKindLst")]
        public ResponseResult ItemStdKindLst(string ParentID)
        {
            var res = new DataResult(1, null);
            int PID = 0;
            if (int.TryParse(ParentID, out PID))
            {
                PID = int.Parse(ParentID);
                res = CustomKindHaddle.GetStdKindLst(PID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ParentID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 获取单笔商品类目资料
        [HttpGetAttribute("/Core/XyComm/Customkind/SkuKind")]
        public ResponseResult SkuKind(string ID)
        {
            var res = new DataResult(1, null);
            string CoID = GetCoid();
            int PID = 0;
            if (int.TryParse(ID, out PID))
            {
                PID = int.Parse(ID);
                res = CustomKindHaddle.GetKind(PID, CoID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ParentID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 新增商品类目
        [HttpPostAttribute("/Core/XyComm/Customkind/InsertSkuKind")]
        public ResponseResult InsertSkuKind([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            string Type = "商品类目";
            string KindName = obj["KindName"].ToString();
            string ParentID = obj["ParentID"].ToString();
            int PID = 0;
            if (int.TryParse(ParentID, out PID))
            {
                PID = int.Parse(ParentID);
                res = CustomKindHaddle.InsertKind(Type, KindName, PID, GetCoid(), GetUname());
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ParentID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 修改商品类目
        [HttpPostAttribute("/Core/XyComm/Customkind/UpdateSkuKind")]
        public ResponseResult UpdateSkuKind([FromBodyAttribute]JObject obj)
        {
            CustomKind Kind = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomKind>(obj.ToString());
            var res = CustomKindHaddle.UptKind(Kind, GetCoid(), GetUname());
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 删除商品类目
        [HttpPostAttribute("/Core/XyComm/Customkind/DeleteSkuKind")]
        public ResponseResult DeleteSkuKind([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            if (IDLst.Count>0)
            {
                res = CustomKindHaddle.DelKind(IDLst, CoID, UserName);
            }
            else
            {
                res.s = -1;
                res.d = "请选中要删除的资料";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}

