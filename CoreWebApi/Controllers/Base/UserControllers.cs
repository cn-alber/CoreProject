using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoreData.CoreUser;
// using Microsoft.AspNetCore.Authorization;
using CoreModels.XyUser;
using System.Collections.Generic;
using CoreData.CoreComm;
using CoreData;
using CoreModels;

namespace CoreWebApi
{
    // [AllowAnonymous]
    public class UserController : ControllBase
    {
        #region 用户管理 - 查询
        [HttpGetAttribute("/Core/XyUser/User/UserLst")]
        public ResponseResult UserLst(string Filter, string Enable, string PageIndex, string PageSize, string SortField, string SortDirection)
        {
            var cp = new UserParam();
            //var cp = Newtonsoft.Json.JsonConvert.DeserializeObject<UserParam>(obj["UserParam"].ToString());
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
                var res = CommHaddle.SysColumnExists(DbBase.CommConnectString, "coresku", SortField);
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
            var result = UserHaddle.GetUserLst(cp);
            return CoreResult.NewResponse(result.s, result.d, "General");
        }
        #endregion

        #region 编辑单笔用户资料
        [HttpGetAttribute("/Core/XyUser/User/UserEdit")]
        public ResponseResult UserEdit(string ID)
        {
            var res = new DataResult(1, null);
            int x;
            if (int.TryParse(ID, out x))
            {
                res = UserHaddle.GetUserEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 查询单笔用户资料
        [HttpGetAttribute("/Core/XyUser/User/UserQuery")]
        public ResponseResult UserQuery(string ID)
        {
            var res = new DataResult(1, null);
            int x;
            if (int.TryParse(ID, out x))
            {
                res = UserHaddle.GetUserEdit(ID);
            }
            else
            {
                res.s = -1;
                res.d = "无效参数ID";
            }
            return CoreResult.NewResponse(res.s, res.d, "General");
        }
        #endregion

        #region 从缓存读取数据
        [HttpGetAttribute("/Core/XyUser/User/UserCache")]
        public ResponseResult UserCache(string Account)
        {
            //var Account = obj["Account"].ToString();
            var res = new DataResult(1, null);
            if (string.IsNullOrEmpty(Account))
            {
                res.s = -1;
                res.d = "请指定读取账号";
            }
            int CoID = int.Parse(GetCoid());
            res = UserHaddle.GetUserCache(CoID, Account);
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

        #region 删除用户
        [HttpPostAttribute("/Core/XyUser/User/DeleteUser")]
        public ResponseResult DeleteUser([FromBodyAttribute]JObject obj)
        {
            var res = new DataResult(1, null);
            var IDLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(obj["IDLst"].ToString());
            var isdel = obj["IsDelete"].ToString();
            int IsDelete = 0;
            if (!string.IsNullOrEmpty(isdel))
            {
                int x;
                if (int.TryParse(isdel, out x))
                {
                    IsDelete = int.Parse(isdel);
                }
            }
            if (IDLst.Count > 0)
            {
                int CoID = int.Parse(GetCoid());
                string UserName = GetUname();
                res = UserHaddle.DeleteUserAccount(IDLst,IsDelete, CoID, UserName);
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