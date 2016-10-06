using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;
namespace CoreWebApi
{
    [AllowAnonymous]
    public class NoticeController : ControllBase
    {
        #region 系统通知 - 资料查询
        [HttpPostAttribute("/Core/XyUser/Notice/NoticeLst")]
        public ResponseResult NoticeLst([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<NoticeParam>(obj["NoticeParam"].ToString());
            cp.CoID = int.Parse(GetCoid());
            var res = NoticeHaddle.GetNoticeLst(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 单笔系统通知 - 编辑
        [HttpPostAttribute("/Core/XyUser/Notice/NoticeEdit")]
        public ResponseResult NoticeEdit([FromBodyAttribute]JObject obj)
        {
            var NotID = obj["NotID"].ToString();
            var res = NoticeHaddle.GetNoticeEdit(NotID);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 单笔系统通知 - 查询
        [HttpPostAttribute("/Core/XyUser/Notice/NoticeQuery")]
        public ResponseResult NoticeQuery([FromBodyAttribute]JObject obj)
        {
            var NotID = obj["NotID"].ToString();
            var res = NoticeHaddle.GetNoticeEdit(NotID);
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
            var res = NoticeHaddle.SaveInsertNot(not,CoID,UserName);
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
            var res = NoticeHaddle.SaveUpdateNot(not,CoID,UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 系统通知 - 删除
        [HttpPostAttribute("/Core/XyUser/Notice/DeleteNotice")]
        public ResponseResult DeleteNotice([FromBodyAttribute]JObject obj)
        {
            var IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(obj["IDsDic"].ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = NoticeHaddle.DeleteNot(IDsDic,CoID,UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}