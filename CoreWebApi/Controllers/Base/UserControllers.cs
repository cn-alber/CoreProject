using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;

namespace CoreWebApi
{
    [AllowAnonymous]
    public class UserController : ControllBase
    {
        #region 用户管理 - 查询
        [HttpPostAttribute("/Core/XyUser/User/UserLst")]
        public ResponseResult UserLst([FromBodyAttribute]JObject obj)
        {
            var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<UserParam>(obj["UserParam"].ToString());
            cp.CoID = int.Parse(GetCoid());
            var res = UserHaddle.GetUserLst(cp);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 编辑单笔用户资料
        [HttpPostAttribute("/Core/XyUser/User/UserEdit")]
        public ResponseResult UserEdit([FromBodyAttribute]JObject obj)
        {
            var UserID = obj["UserID"].ToString();
            var res = UserHaddle.GetUserEdit(UserID);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 查询单笔用户资料
        [HttpPostAttribute("/Core/XyUser/User/UserQuery")]
        public ResponseResult UserQuery([FromBodyAttribute]JObject obj)
        {
            var UserID = obj["UserID"].ToString();
            var res = UserHaddle.GetUserEdit(UserID);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 从缓存读取数据
        [HttpPostAttribute("/Core/XyUser/User/UserCache")]
        public ResponseResult UserCache([FromBodyAttribute]JObject obj)
        {
            var Account = obj["Account"].ToString();
            int CoID = int.Parse(GetCoid());
            var res = UserHaddle.GetUserCache(CoID, Account);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 启用|停用用户
        [HttpPostAttribute("/Core/XyUser/User/UserEnable")]
        public ResponseResult UserEnable([FromBodyAttribute]JObject obj)
        {
            Dictionary<int, string> IDsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(obj["IDsDic"].ToString());
            string Company = obj["Company"].ToString();
            bool Enable = obj["Enable"].ToString().ToUpper() == "TRUE" ? true : false;
            string CoID = GetCoid();
            string UserName = GetUname();
            var res = UserHaddle.UptUserEnable(IDsDic, Company, UserName, Enable, CoID);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 检查用户账号是否存在
        [HttpPostAttribute("/Core/XyUser/User/IsExistUser")]
        public ResponseResult IsExistUser([FromBodyAttribute]JObject obj)
        {
            string Account = obj["Account"].ToString();
            int ID = int.Parse(obj["ID"].ToString());
            int CoID = int.Parse(GetCoid());
            var res = UserHaddle.ExistUser(Account, ID, CoID);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 新增用户
        [HttpPostAttribute("/Core/XyUser/User/InsertUser")]
        public ResponseResult InsertUser([FromBodyAttribute]JObject obj)
        {
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEdit>(obj["User"].ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = UserHaddle.SaveInsertUser(user, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 修改用户        
        [HttpPostAttribute("/Core/XyUser/User/UpdateUser")]
        public ResponseResult UpdateUser([FromBodyAttribute]JObject obj)
        {
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEdit>(obj["User"].ToString());
            int CoID = int.Parse(GetCoid());
            string UserName = GetUname();
            var res = UserHaddle.SaveUpdateUser(user, CoID, UserName);
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion
    }
}