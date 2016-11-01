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
    public class CustomkindController : ControllBase
    {
        #region 获取商品类目列表
        [HttpGetAttribute("/Core/XyComm/Customkind/SkuKindLst")]
        public ResponseResult SkuKindLst(string ParentID, string Enable)
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
                cp.Enable = Enable;
                cp.ParentID = int.Parse(ParentID);

                res = CustomKindHaddle.GetKindLst(cp);
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
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
        #region 获取商品属性
        [HttpGetAttribute("/Core/XyComm/Customkind/SkuKindProps")]
        public ResponseResult SkuKindProps(string ID, string Enable)
        {
            var res = new DataResult(1, null);
            string CoID = GetCoid();
            int PID = 0;
            if (int.TryParse(ID, out PID))
            {
                PID = int.Parse(ID);
                res = CustomKindHaddle.GetSkuKindProps(PID, Enable, CoID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion


        #region 获取商品单个属性
        [HttpGetAttribute("/Core/XyComm/CustomKindProps/SkuKindProp")]
        public ResponseResult SkuKindProp(string ID)
        {
            var res = new DataResult(1, null);
            string CoID = GetCoid();
            int PID = 0;
            if (int.TryParse(ID, out PID))
            {
                PID = int.Parse(ID);
                res = CustomKindHaddle.GetSkuKindProp(PID, CoID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion


        #region 新增商品类目
        [HttpPostAttribute("/Core/XyComm/Customkind/InsertSkuKind")]
        public ResponseResult InsertSkuKind([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            // var cp = new CustomKind();
            // cp.KindName = obj["KindName"].ToString();
            string PID = obj["ParentID"].ToString();
            int x = 0;
            if (!int.TryParse(PID, out x))
            {
                res.s = -1;
                res.d = "无效参数ParentID";
            }
            else
            {
                var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomKind>(obj.ToString());
                cp.Type = "商品类目";
                cp.ParentID = int.Parse(PID);
                cp.CoID = int.Parse(GetCoid());
                cp.Creator = GetUname();
                cp.CreateDate = DateTime.Now.ToString();
                res = CustomKindHaddle.InsertKind(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 修改商品类目
        [HttpPostAttribute("/Core/XyComm/Customkind/UpdateSkuKind")]
        public ResponseResult UpdateSkuKind([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            bool eb = true;
            if (obj["Enable"] == null || !bool.TryParse(obj["Enable"].ToString(), out eb))
            {
                res.s = -1;
                res.d = "无效参数Enable";
            }
            else
            {
                CustomKind Kind = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomKind>(obj.ToString());
                res = CustomKindHaddle.UptKind(Kind, GetCoid(), GetUname());
            }
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
            if (IDLst.Count > 0)
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

        #region 商品类目启用|停用
        [HttpPostAttribute("/Core/XyComm/Customkind/SkuKindEnable")]
        public ResponseResult SkuKindEnable([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            if (IDLst.Count == 0)
            {
                res.s = -1;
                res.d = "请先选中操作明细";
            }
            else
            {
                bool Enable = obj["Enable"].ToString().ToUpper() == "TRUE" ? true : false;
                string CoID = GetCoid();
                string UserName = GetUname();
                res = CustomKindHaddle.UptKindEnable(IDLst, CoID, UserName, Enable);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 导入标准商品类目
        [HttpPostAttribute("/Core/XyComm/Customkind/InsertStandardKind")]
        public ResponseResult InsertStandardKind([FromBodyAttribute]JObject obj)
        {
            int StID;
            var res = new DataResult(1, null);
            if (!int.TryParse(obj["ID"].ToString(), out StID))
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            else
            {
                var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomKind>(obj.ToString());
                cp.Creator = GetUname();
                cp.CoID = int.Parse(GetCoid());
                cp.CreateDate = DateTime.Now.ToString();
                res = CustomKindHaddle.InsertKindProps(cp);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion

        #region 导入淘宝商品类目
        [HttpPostAttribute("/Core/XyComm/Customkind/InsertTmaoKind")]
        public ResponseResult InsertTmaoKind()
        {
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = CustomKindHaddle.InsertTmaoKind(CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 新增商品属性值
        [HttpPostAttribute("/Core/XyComm/CustomKindProps/InsertSkuProps")]
        public ResponseResult InsertSkuProps([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<Customkind_props>(obj.ToString());
            cp.CoID = int.Parse(GetCoid());
            cp.Creator = GetUname();
            cp.CreateDate = DateTime.Now.ToString();
            var res = CustomKindPropsHaddle.InsertProps(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion 


        #region 修改商品类目属性
        [HttpPostAttribute("/Core/XyComm/CustomKindProps/UpdateSkuProps")]
        public ResponseResult UpdateSkuProps([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<Customkind_props>(obj.ToString());
            cp.CoID = int.Parse(GetCoid());
            cp.Creator = GetUname();
            cp.CreateDate = DateTime.Now.ToString();
            var res = CustomKindPropsHaddle.UpdateProps(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 商品类目属性- 停用|启用
        [HttpPostAttribute("/Core/XyComm/CustomKindProps/UpdateSkuPropsEnable")]
        public ResponseResult UpdateSkuPropsEnable([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            if (IDLst.Count == 0)
            {
                res.s = -1;
                res.d = "请先选中操作明细";
            }
            else
            {
                bool Enable = obj["Enable"].ToString().ToUpper() == "TRUE" ? true : false;
                string CoID = GetCoid();
                string UserName = GetUname();
                res = CustomKindPropsHaddle.UptPropsEnable(IDLst, CoID, UserName, Enable);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion

        #region 拷贝商品类目属性--来源类目列表
        [HttpGetAttribute("/Core/XyComm/CustomKindProps/CopyToKindLst")]
        public ResponseResult CopyToKindLst()
        {
            var res = CustomKindHaddle.GetCopyToKindLst(GetCoid(), "");
            return CoreResult.NewResponse(res.s, res.d, "General");
        }

        #endregion


        #region 拷贝商品类目属性--更新资料
        [HttpPostAttribute("/Core/XyComm/CustomKindProps/SaveCopyToProps")]
        public ResponseResult SaveCopyToProps([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            var KindIDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["KindIDLst"].ToString());
            var Type = obj["Type"].ToString();
            int i;
            if (KindIDLst.Count == 0)
            {
                res.s = -1;
                res.d = "请先选中目的类目";
            }
            else if(!int.TryParse(Type,out i))
            {
                res.s = -1;
                res.d = "无效参数Type";
            }
            else
            {
                string CoID = GetCoid();
                string UserName = GetUname();
                CustomKindPropsHaddle.CopyProps(IDLst, KindIDLst, int.Parse(Type), CoID, UserName);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
     



        #region
        [HttpGetAttribute("/Core/XyComm/Customkind/GetSkuProps")]
        public ResponseResult GetSkuProps(string Cid)
        {
            // var res = CoreData.CoreApi.TmallHaddle.GetSellercatsList("南极人羽绒旗舰店");
            var res = CoreData.CoreApi.TmallHaddle.itemProps(Cid);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion


    }
}

