using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class NoticeController : ControllBase
    {
        #region 系统通知 - 资料查询
        [HttpGetAttribute("/Core/XyUser/Notice/NoticeLst")]
        public ResponseResult NoticeLst(string Filter, string Enable, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new NoticeParam();
            //var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<NoticeParam>(obj["NoticeParam"].ToString());
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
                var res = CommHaddle.SysColumnExists(DbBase.UserConnectString, "Notice", SortField);
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
            var result = NoticeHaddle.GetNoticeLst(cp);
            return CoreResult.NewResponse(result.s, result.d, "General");
        }
        #endregion

        #region 单笔系统通知 - 编辑
        [HttpGetAttribute("/Core/XyUser/Notice/NoticeEdit")]
        public ResponseResult NoticeEdit(string ID)
        {
            // var NotID = obj["NotID"].ToString();
            var res = new DataResult(1, null);
            int x;
            if (int.TryParse(ID, out x))
            {
                res = NoticeHaddle.GetNoticeEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 单笔系统通知 - 查询
        [HttpGetAttribute("/Core/XyUser/Notice/NoticeQuery")]
        public ResponseResult NoticeQuery(string ID)
        {
            var res = new DataResult(1, null);
            int x;
            if (int.TryParse(ID, out x))
            {
                res = NoticeHaddle.GetNoticeEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 系统通知 - 新增
        [HttpPostAttribute("/Core/XyUser/Notice/InsertNotice")]
        public ResponseResult InsertNotice([FromBodyAttribute]JObject obj)
        {
            var not = Newtonsoft.Json.JsonConvert.DeserializeObject<Notice>(obj["Notice"].ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = NoticeHaddle.SaveInsertNot(not, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 系统通知 - 修改
        [HttpPostAttribute("/Core/XyUser/Notice/UpdateNotice")]
        public ResponseResult UpdateNotice([FromBodyAttribute]JObject obj)
        {
            var not = Newtonsoft.Json.JsonConvert.DeserializeObject<Notice>(obj["Notice"].ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = NoticeHaddle.SaveUpdateNot(not, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 系统通知 - 删除
        [HttpPostAttribute("/Core/XyUser/Notice/DeleteNotice")]
        public ResponseResult DeleteNotice([FromBodyAttribute]JObject obj)
        {
            // var IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(obj["IDsDic"].ToString());     
            var res = new DataResult(1, null);
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            if (IDLst.Count > 0)
            {
                int CoID = int.Parse(GetCoid());
                string UserName = GetUname();
                res = NoticeHaddle.DeleteNot(IDLst, CoID, UserName);
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