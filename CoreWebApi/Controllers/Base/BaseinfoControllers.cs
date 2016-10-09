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
    // [AllowAnonymous]
    public class BaseinfoController : ControllBase
    {
        #region 数据字典 - 资料查询
        [HttpGetAttribute("/Core/XyComm/Baseinfo/BaseinfoLst")]
        public ResponseResult BaseinfoLst(string Kind, string Filter, string Enable, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new BaseinfoParam();
            cp.Filter = Filter;
            if (!string.IsNullOrEmpty(Kind))
            {
                cp.Kind = Kind;
            }
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
                var res = CommHaddle.SysColumnExists(DbBase.CommConnectString, "Baseinfo", SortField);
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
            var result = BaseinfoHaddle.GetBaseinfoLst(cp);
            return CoreResult.NewResponse(result.s, result.d, "General");
        }
        #endregion

        #region 数据字典 - 单笔资料编辑
        [HttpGetAttribute("/Core/XyUser/Baseinfo/BaseinfoEdit")]
        public ResponseResult BaseinfoEdit(string ID)
        {
            var res = new DataResult(1, null);
            int x;
            if (int.TryParse(ID, out x))
            {
                res = BaseinfoHaddle.GetBaseinfoEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 数据字典 - 单笔资料查询
        [HttpGetAttribute("/Core/XyUser/Baseinfo/BaseinfoQuery")]
        public ResponseResult BaseinfoQuery(string ID)
        {
            var res = new DataResult(1, null);
            int x;
            if (int.TryParse(ID, out x))
            {
                res = BaseinfoHaddle.GetBaseinfoEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 数据字典 - 启用|停用
        [HttpGetAttribute("/Core/XyUser/Baseinfo/BaseinfoEnable")]
        public ResponseResult BaseinfoEnable([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1,null);
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
                // res = UserHaddle.UptUserEnable(IDLst, CoID, UserName, Enable);
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}