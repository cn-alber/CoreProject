using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi
{
    public class ControllBase : Controller
    {
        #region 账号相关
        ///<summary>
        ///获取用户ID
        ///</summary>
        public string GetUid()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("uid")).Value;
        }

        ///<summary>
        ///获取用户名
        ///</summary>
        public string GetUname()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("uname")).Value;
        }

        ///<summary>
        ///获取公司ID
        ///</summary>
        public string GetCoid()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("coid")).Value;
        }

        ///<summary>
        ///获取公司ID
        ///</summary>
        public string GetRoleid()
        {
            return HttpContext.User.Claims.FirstOrDefault(q => q.Type.Equals("roleid")).Value;
        }

        #endregion
    }
}