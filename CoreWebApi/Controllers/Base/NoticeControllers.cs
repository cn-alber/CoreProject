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
    // [AllowAnonymous]
    public class NoticeController : ControllBase
    {
        #region 系统通知 - 资料查询
        [HttpGetAttribute("/Core/XyUser/Notice/NoticeGet")]
        public ResponseResult NoticeLst()
        {            
            var result = NoticeHaddle.GetNoticeLst();
            return CoreResult.NewResponse(result.s, result.d, "General");
        }
        #endregion

       


        #region 系统通知 - 新增
        [HttpPostAttribute("/Core/XyUser/Notice/InsertNotice")]
        public ResponseResult InsertNotice([FromBodyAttribute]JObject obj)
        {
            var not = Newtonsoft.Json.JsonConvert.DeserializeObject<Notice>(obj["Notice"].ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();            
            not.userid = GetUid();
            var res = NoticeHaddle.SaveInsertNot(not, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

      
    }
}