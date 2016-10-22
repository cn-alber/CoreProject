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

//  [AllowAnonymous]
 public class BrandController : ControllBase
    {        
        #region 品牌 - 资料查询
        [HttpGetAttribute("/Core/XyComm/Brand/BrandLst")]
        public ResponseResult BrandLst(string Filter, string Enable, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
             var cp = new BrandParam();
            cp.Filter = Filter;
            if (!string.IsNullOrEmpty(Enable) && (Enable.ToUpper() == "TRUE" || Enable.ToUpper() == "FALSE"))
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
            if (!string.IsNullOrEmpty(SortField))
            {
                var res = CommHaddle.SysColumnExists(DbBase.CommConnectString, "Brand", SortField);
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
            var result = BrandHaddle.GetBrandLst(cp);
            return CoreResult.NewResponse(result.s, result.d, "General");
        }
        #endregion

        #region 品牌 - 编辑
        [HttpGetAttribute("/Core/XyComm/Brand/BrandEdit")]
        public ResponseResult BrandEdit(string ID)
        {
            var res = new DataResult(1,null);
            int x;
            if(int.TryParse(ID,out x))
            {
                res = BrandHaddle.GetBrandEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 品牌 - 查询
        [HttpGetAttribute("/Core/XyComm/Brand/BrandQuery")]
        public ResponseResult BrandQuery(string ID)
        {
            var res = new DataResult(1,null);
            int x;
            if(int.TryParse(ID,out x))
            {
                res = BrandHaddle.GetBrandEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 品牌 - 状态(启用|停用)
        [HttpPostAttribute("/Core/XyComm/Brand/BrandEnable")]
        public ResponseResult BrandEnable([FromBodyAttribute]JObject obj)
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
                res = BrandHaddle.UptBrandEnable(IDLst, CoID, UserName, Enable);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 品牌 - 新增
        [HttpPostAttribute("/Core/XyComm/Brand/InsertBrand")]
        public ResponseResult InsertBrand([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1,null);
            var brand = Newtonsoft.Json.JsonConvert.DeserializeObject<Brand>(obj.ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            res = BrandHaddle.SaveInsertBrand(brand,CoID,UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 品牌 - 修改
        [HttpPostAttribute("/Core/XyComm/Brand/UpdateBrand")]
        public ResponseResult UpdateBrand([FromBodyAttribute]JObject obj)
        {            
            var brand = Newtonsoft.Json.JsonConvert.DeserializeObject<Brand>(obj.ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = BrandHaddle.SaveUpdateBrand(brand,CoID,UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 品牌 - 删除
        [HttpPostAttribute("/Core/XyComm/Brand/DeleteBrand")]
        public ResponseResult DeleteBrand([FromBodyAttribute]JObject obj)
        {   
            var res = new DataResult(1,null);         
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            if (IDLst.Count == 0)
            {
                res.s = -1;
                res.d = "请先选中要删除的资料";
            }
            else
            {
                int CoID = int.Parse(GetCoid());
                string UserName = GetUname();
                res = BrandHaddle.DelBrand(IDLst,CoID,UserName);
            }
            return CoreResult.NewResponse(res.s,res.d,"General");
        }
        #endregion
    }
}